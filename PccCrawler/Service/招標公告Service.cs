using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using PccCrawler.Service.Interface;
using System.Diagnostics;

namespace PccCrawler.Service
{
    // TODO: 待改為Chain-of-responsibility pattern
    public class 招標公告Service : I招標公告Service
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;
        private readonly DaoService _dao;
        private readonly IAnalyzeService _analyzeService;

        public 招標公告Service(IOptions<CrawlerOption> options, IHttpService httpService, DaoService dao, IAnalyzeService analyzeService)
        {
            _options = options.Value;
            _httpService = httpService;
            _dao = dao;
            _analyzeService = analyzeService;
        }

        public async Task DoJob()
        {
            var masterList = _dao.Query<PccMasterPo>("select * from PccMaster where Category = '招標公告'");
            foreach (var radProctrgCate in _options.RadProctrgCates)
            {
                Console.WriteLine($"Crawling List:招標公告-{radProctrgCate}...");
                var caseNos = new List<string>();
                #region 取得所有Url
                var total = await GetTotalItem(radProctrgCate);
                var totalPage = total switch
                {
                    0 => 0,
                    > 0 and <= 100 => 1,
                    > 100 => total / 100 + (total % 100 != 0 ? 1 : 0),
                    _ => 0
                };
                for (var pageIndex = 1; pageIndex <= totalPage; pageIndex++)
                {
                    var doc = await GetSearchHtmlDoc(pageIndex, radProctrgCate);
                    var trNodes = doc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
                    if (trNodes == null)
                    {
                        Console.WriteLine("Get List Fail");
                        return;
                    }
                    for (var i = 0; i < trNodes.Count - 1; i++)
                    {
                        // 取得Url
                        var hrefNode = trNodes[i].SelectSingleNode("./td[4]/a");
                        var href = hrefNode.GetAttributeValue("href", null);
                        var caseNo = href.Contains("primaryKey=") ? href.Split("primaryKey=")[1] : "";
                        // 檢查資料是否曾爬取過且成功
                        if (!masterList.Any(x => x.CaseNo == caseNo && x.Status == 900))
                        {
                            var url = GetUrl(UrlType.招標公告_詳細資料頁, caseNo);
                            var pairs = new Dictionary<string, object>
                            {
                                { nameof(caseNo), caseNo },
                                { nameof(url), url }
                            };
                            if (masterList.Any(x => x.CaseNo == caseNo))
                            {
                                _dao.Query<int>($"update PccMaster set Url = @url, Status = 100, UpdateTime = getdate() where CaseNo = @caseNo and Category = '招標公告'", pairs);
                            }
                            else
                            {
                                _dao.Query<int>($"insert into PccMaster (CaseNo, Category, Url, Status) values (@caseNo, '招標公告', @url, 100)", pairs);
                            }
                            caseNos.Add(caseNo);
                        }
                    }

                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                }
                Console.WriteLine($"TotalItem: {total} count，NewItem: {caseNos.Count} count");
                #endregion

                var stopWatch = new Stopwatch();
                foreach (var caseNo in caseNos)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    Console.WriteLine($"Crawling Detail:{caseNo}...");
                    try
                    {
                        var detailDoc = await GetDetailHtmlDoc(caseNo);
                        _analyzeService.Analyze招標公告(detailDoc);
                        var pairs = new Dictionary<string, object>
                        {
                            { nameof(caseNo), caseNo },
                        };
                        _dao.Query<int>($"update PccMaster set Status = 900 where CaseNo = @caseNo and Category = '招標公告'", pairs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"CaseNo: {caseNo} {Environment.NewLine}" +
                                          $"Msg: {ex.Message}");
                        var pairs = new Dictionary<string, object>
                        {
                            { nameof(caseNo), caseNo },
                            { nameof(ex.Message), ex.Message }
                        };
                        _dao.Query<int>($"insert into LogEvent(EventLevel ,EventType, EventContent, CaseNo) " +
                                        $"values('Error', '招標公告', @Message, @caseNo)", pairs);
                    }
                    if (_options.Mode == "Debug")
                    {
                        break;
                    }

                    #region 延遲Request速度
                    // 延遲Request速度
                    stopWatch.Stop();
                    var totalSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    var intervalSeconds = _options.IntervalSeconds;
                    if (totalSeconds < intervalSeconds)
                    {
                        Console.WriteLine($"Use time is too short({totalSeconds}s), a little delay:{intervalSeconds - totalSeconds}s");
                        Thread.Sleep((intervalSeconds - totalSeconds) * 1000);
                    }
                    #endregion
                }
                // 一律改存DB
                //new ExcelService().WriteExcel($"{Environment.CurrentDirectory}/output/{radProctrgCate}.xlsx", 招標公告Pos);
            }
        }

        #region Done
        /// <summary>
        /// 取得Pcc網址
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetUrl(UrlType type, params string[] args)
        {
            var domain = "https://web.pcc.gov.tw";
            var area = "tps";
            var searchMode = SearchMode.common;
            var searchType = SearchType.advance;
            var url = type switch
            {
                UrlType.招標公告_搜尋結果頁 => $"{domain}/{area}/pss/tender.do?searchMode={searchMode}&searchType={searchType}",
                UrlType.招標公告_詳細資料頁 => $"{domain}/{area}/tpam/main/tps/tpam/tpam_tender_detail.do?searchMode={searchMode}&primaryKey={args[0]}",
                _ => $"{domain}/{area}/pss/tender.do?searchMode={searchMode}&searchType={searchType}"
            };
            return url;
        }

        /// <summary>
        /// 取得搜尋結果頁Html
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="radProctrgCate">標的分類</param>
        /// <returns></returns>
        public async Task<HtmlDocument> GetSearchHtmlDoc(int pageIndex, RadProctrgCate radProctrgCate)
        {
            var url = GetUrl(UrlType.招標公告_搜尋結果頁);
            var formData = _httpService.GetFormData(new 招標公告SearchVo
            {
                pageIndex = pageIndex,
                proctrgCate = (int)radProctrgCate,
                radProctrgCate = (int)radProctrgCate
            });
            var resp = await _httpService.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }

        /// <summary>
        /// 取得搜尋結果總筆數
        /// </summary>
        /// <param name="radProctrgCate">標的分類</param>
        /// <returns></returns>
        private async Task<int> GetTotalItem(RadProctrgCate radProctrgCate)
        {
            var doc = await GetSearchHtmlDoc(1, radProctrgCate);
            var totalStr = doc.GetElementbyId("print_area")?.SelectSingleNode(".//span[@class=\"T11b\"]")?.InnerText;
            _ = int.TryParse(totalStr, out var total);
            return total;
        }

        /// <summary>
        /// 取得詳細資料頁Html
        /// </summary>
        /// <param name="caseNo">識別碼</param>
        /// <returns></returns>
        public async Task<HtmlDocument> GetDetailHtmlDoc(string caseNo)
        {
            var url = GetUrl(UrlType.招標公告_詳細資料頁, caseNo);
            var resp = await _httpService.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion
    }
}
