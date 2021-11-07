using HtmlAgilityPack;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using PccCrawler.Process.Interface;
using PccCrawler.Service;
using PccCrawler.Service.Interface;
using System.Diagnostics;

namespace PccCrawler.Process
{
    // TODO: 待改為Chain-of-responsibility pattern
    public class 招標公告Process : I招標公告Process
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;
        private readonly DaoService _dao;

        public 招標公告Process(CrawlerOption options, IHttpService httpService, DaoService dao)
        {
            _options = options;
            _httpService = httpService;
            _dao = dao;
        }

        public async Task DoJob()
        {
            var masterList = _dao.Query<PccMasterPo>("select * from PccMaster");
            foreach (var radProctrgCate in _options.RadProctrgCates)
            {
                Console.WriteLine($"Crawling List:招標公告-{radProctrgCate}...");
                var pks = new List<string>();
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
                    var doc = await GetHtmlDoc(pageIndex, radProctrgCate);
                    var trNodes = doc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
                    if (trNodes == null)
                    {
                        Console.WriteLine("Get List Fail");
                        return;
                    }
                    for (var i = 1; i < trNodes.Count - 1; i++)
                    {
                        var hrefNode = trNodes[i].SelectSingleNode("./td[4]/a");
                        var href = hrefNode.GetAttributeValue("href", null);
                        var pk = href.Contains("primaryKey=") ? href.Split("primaryKey=")[1] : "";
                        // 檢查資料是否曾爬取過
                        if (!masterList.Any(x => x.Status == 900 && x.Id == pk))
                        {
                            if (masterList.Any(x => x.Id == pk))
                            {
                                _dao.Query<int>($"update PccMaster set Id = {pk}, Url ='{GetUrl(UrlType.tpam_tender_detail, pk)}', Status = 100 where Id = {pk}");
                            }
                            else
                            {
                                _dao.Query<int>($"insert into PccMaster (Id, Url, Status) values ({pk}, '{GetUrl(UrlType.tpam_tender_detail, pk)}', 100)");
                            }
                            pks.Add(pk);
                        }
                    }

                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                }
                Console.WriteLine($"TotalItem: {total} count，NewItem: {pks.Count} count");

                var stopWatch = new Stopwatch();
                var analyzeService = new AnalyzeService();
                var 招標公告Pos = new List<招標公告Po>();
                foreach (var pk in pks)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    Console.WriteLine($"Crawling Detail:{pk}...");
                    try
                    {
                        var detailDoc = await GetDetailHtmlDoc(pk);
                        var po = analyzeService.Analyze<招標公告Po>(detailDoc);
                        po.Url = GetUrl(UrlType.tpam_tender_detail, pk);
                        招標公告Pos.Add(po);
                        _dao.Query<int>($"update PccMaster set Status = 900 where Id = {pk}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"PK: {pk} {Environment.NewLine}" +
                                          $"Msg: {ex.Message}");
                        _dao.Query<int>($"insert into LogEvent (Id, EventContent) values ({pk}, '{ex.Message}')");
                    }
                    stopWatch.Stop();
                    int totalSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    Console.WriteLine("RunTime:" + totalSeconds);
                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                    if (totalSeconds < _options.IntervalSeconds)
                    {
                        Console.WriteLine($"Use time is too short, a little delay:{_options.IntervalSeconds - totalSeconds}");
                        Thread.Sleep((_options.IntervalSeconds - totalSeconds) * 1000);
                    }
                }
                new ExcelService().WriteExcel($"{Environment.CurrentDirectory}/output/{radProctrgCate}.xlsx", 招標公告Pos);
            }
        }

        #region Done
        private string GetUrl(UrlType type, params string[] args)
        {
            var domain = "https://web.pcc.gov.tw";
            var area = "tps";
            var path = "pss/tender.do";
            var searchMode = SearchMode.common;
            var searchType = SearchType.advance;
            var url = type switch
            {
                UrlType.tender => $"{domain}/{area}/{path}?searchMode={searchMode}&searchType={searchType}",
                UrlType.tpam_tender_detail => $"{domain}/{area}/tpam/main/tps/tpam/tpam_tender_detail.do?searchMode={searchMode}&primaryKey={args[0]}",
                _ => $"{domain}/{area}/{path}?searchMode={searchMode}&searchType={searchType}"
            };
            return url;
        }

        private async Task<int> GetTotalItem(RadProctrgCate radProctrgCate)
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpService.GetFormData(new SearchVo
            {
                proctrgCate = (int)radProctrgCate,
                radProctrgCate = (int)radProctrgCate
            });
            var resp = await _httpService.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);

            var totalStr = doc.GetElementbyId("print_area")?.SelectSingleNode(".//span[@class=\"T11b\"]")?.InnerText;
            _ = int.TryParse(totalStr, out var total);
            return total;
        }

        public async Task<HtmlDocument> GetHtmlDoc(int pageIndex, RadProctrgCate radProctrgCate)
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpService.GetFormData(new SearchVo
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

        public async Task<HtmlDocument> GetDetailHtmlDoc(string pk)
        {
            var url = GetUrl(UrlType.tpam_tender_detail, pk);
            var resp = await _httpService.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion
    }
}
