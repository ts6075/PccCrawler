using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using PccCrawler.Service.Interface;

namespace PccCrawler.Service
{
    public class 公開徵求Service : I公開徵求Service
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;
        private readonly DaoService _dao;
        private readonly IAnalyzeService _analyzeService;

        public 公開徵求Service(IOptions<CrawlerOption> options, IHttpService httpService, DaoService dao, IAnalyzeService analyzeService)
        {
            _options = options.Value;
            _httpService = httpService;
            _dao = dao;
            _analyzeService = analyzeService;
        }

        public async Task DoJob()
        {
            var masterList = _dao.Query<PccMasterPo>("select * from PccMaster where Category = '公開徵求'");
            Console.WriteLine($"Crawling List:公開徵求(全)...");
            var tenderCaseNos = new List<string>();
            #region 取得所有Url
            var total = await GetTotalItem();
            var totalPage = total switch
            {
                0 => 0,
                > 0 and <= 10 => 1,
                > 10 => total / 10 + (total % 10 != 0 ? 1 : 0),
                _ => 0
            };
            for (var pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            {
                var doc = await GetSearchHtmlDoc(pageIndex);
                var formNodes = doc.GetElementbyId("page")?.SelectNodes("./table//center//table//tr//form");
                if (formNodes == null)
                {
                    Console.WriteLine("Get List Fail");
                    return;
                }
                for (var i = 0; i < formNodes.Count - 1; i++)
                {
                    // 取得tenderCaseNo
                    var tenderCaseNo = formNodes[i].SelectSingleNode(".//input[@id=\"tenderCaseNo\"]")?.GetAttributeValue("value", string.Empty);
                    // 檢查資料是否曾爬取過且成功
                    if (!masterList.Any(x => x.CaseNo == tenderCaseNo && x.Status == 900))
                    {
                        var url = GetUrl(UrlType.公開徵求_詳細資料頁, tenderCaseNo);
                        var pairs = new Dictionary<string, object>
                            {
                                { nameof(tenderCaseNo), tenderCaseNo },
                                { nameof(url), url }
                            };
                        if (masterList.Any(x => x.CaseNo == tenderCaseNo))
                        {
                            _dao.Query<int>($"update PccMaster set Url = @url, Status = 100, UpdateTime = getdate() where CaseNo = @tenderCaseNo and Category = '公開徵求'", pairs);
                        }
                        else
                        {
                            // TODO: 待移除try-catch
                            try
                            {
                                _dao.Query<int>($"insert into PccMaster (CaseNo, Category, Url, Status) values (@tenderCaseNo, '公開徵求', @url, 100)", pairs);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"CaseNo: {tenderCaseNo} {Environment.NewLine}" +
                                                  $"Msg: {ex.Message}");
                                var pairs2 = new Dictionary<string, object>
                                {
                                    { nameof(tenderCaseNo), tenderCaseNo },
                                    { nameof(ex.Message), ex.Message }
                                };
                                _dao.Query<int>($"insert into LogEvent(EventLevel ,EventType, EventContent, CaseNo) " +
                                                $"values('Error', '公開徵求', @Message, @tenderCaseNo)", pairs2);
                            }
                        }
                        tenderCaseNos.Add(tenderCaseNo);
                    }
                    else
                    {
                        ;
                    }
                }

                if (_options.Mode == "Debug")
                {
                    break;
                }
            }
            Console.WriteLine($"TotalItem: {total} count，NewItem: {tenderCaseNos.Count} count");
            #endregion

            foreach (var tenderCaseNo in tenderCaseNos)
            {
                Console.WriteLine($"Crawling Detail:{tenderCaseNo}...");
                try
                {
                    var detailDoc = await GetDetailHtmlDoc(tenderCaseNo);
                    _analyzeService.Analyze公開徵求(detailDoc);
                    var pairs = new Dictionary<string, object>
                        {
                            { nameof(tenderCaseNo), tenderCaseNo },
                        };
                    _dao.Query<int>($"update PccMaster set Status = 900 where CaseNo = @tenderCaseNo and Category = '公開徵求'", pairs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"tenderCaseNo: {tenderCaseNo} {Environment.NewLine}" +
                                      $"Msg: {ex.Message}");
                    var pairs = new Dictionary<string, object>
                        {
                            { nameof(tenderCaseNo), tenderCaseNo },
                            { nameof(ex.Message), ex.Message }
                        };
                    _dao.Query<int>($"insert into LogEvent(EventLevel ,EventType, EventContent, CaseNo) " +
                                    $"values('Error', '公開徵求', @Message, @tenderCaseNo)", pairs);
                }
                if (_options.Mode == "Debug")
                {
                    break;
                }
            }
        }

        #region Done
        /// <summary>
        /// 取得Pcc網址
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetUrl(UrlType type, string? tenderCaseNo = null)
        {
            var domain = "https://web.pcc.gov.tw";
            var url = type switch
            {
                UrlType.公開徵求_搜尋結果頁 => $"{domain}/tps/tps/tp/main/tps/tp/searchAppealVendor.do?pMenu=common",
                // TODO: Url有誤,缺少必要參數
                UrlType.公開徵求_詳細資料頁 => $"{domain}/tps/tps/tp/main/tps/tp/tp.do?method=initialAppealViewVendor&pMenu=common&tenderCaseNo={tenderCaseNo}",
                _ => $"{domain}/tps/tps/tp/main/tps/tp/searchAppealVendor.do?pMenu=common"
            };
            return url;
        }

        /// <summary>
        /// 取得搜尋結果頁Html
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <returns></returns>
        public async Task<HtmlDocument> GetSearchHtmlDoc(int pageIndex)
        {
            var url = GetUrl(UrlType.公開徵求_搜尋結果頁);
            var formData = _httpService.GetFormData(new 公開徵求SearchVo
            {
                __PageIndex = pageIndex
            });
            var resp = await _httpService.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }

        /// <summary>
        /// 取得搜尋結果總筆數
        /// </summary>
        /// <returns></returns>
        private async Task<int> GetTotalItem()
        {
            var doc = await GetSearchHtmlDoc(1);
            var totalStr = doc.GetElementbyId("page")?.SelectSingleNode(".//span[@class=\"T11b\"]")?.InnerText;
            _ = int.TryParse(totalStr, out var total);
            return total;
        }

        /// <summary>
        /// 取得詳細資料頁Html
        /// </summary>
        /// <param name="tenderCaseNo">識別碼</param>
        /// <returns></returns>
        public async Task<HtmlDocument> GetDetailHtmlDoc(string tenderCaseNo)
        {
            var url = GetUrl(UrlType.公開徵求_詳細資料頁, tenderCaseNo);
            var resp = await _httpService.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion
    }
}
