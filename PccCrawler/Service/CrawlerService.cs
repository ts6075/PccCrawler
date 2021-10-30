using HtmlAgilityPack;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using PccCrawler.Extension;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PccCrawler.Service.Interface;
using Microsoft.Extensions.Options;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;

        public CrawlerService(IOptions<CrawlerOption> options, IHttpService httpService)
        {
            _options = options.Value;
            _httpService = httpService;
        }

        public async Task RunAsync()
        {
            /* 需求：
               1、當日招標公告、公開徵求、公開閱覽、政府採購預告(全)
               2、工程、財務、勞務
               3、決標公告
               4、無法決標公告
               5、更正公告
             */
            Console.WriteLine("Start");
            foreach (var radProctrgCate in new RadProctrgCate[] { RadProctrgCate.工程, RadProctrgCate.財物, RadProctrgCate.勞務 })
            {
                Console.WriteLine($"Crawling List:{radProctrgCate}...");
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
                        pks.Add(pk);
                    }

                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                }
                Console.WriteLine($"TotalItem:{pks.Count} count");

                var stopWatch = new Stopwatch();
                var analyzeService = new AnalyzeService();
                var allDatas = new List<Dictionary<string, string>>();
                foreach (var pk in pks)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    Console.WriteLine($"Crawling Detail:{pk}...");
                    var detailDoc = await GetDetailHtmlDoc(pk);
                    var po = analyzeService.Analyze<招標公告Po>(detailDoc);

                    stopWatch.Stop();
                    int totalSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    Console.WriteLine("RunTime:" + totalSeconds);
                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                    if (totalSeconds < 15)
                    {
                        Console.WriteLine($"Use time is too short, a little delay:{15 - totalSeconds}");
                        Thread.Sleep((15 - totalSeconds) * 1000);
                    }
                }
                WriteExcel($"{Environment.CurrentDirectory}/output/{radProctrgCate}.xls", allDatas);
            }
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
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

        private void WriteExcel(string savePath, List<Dictionary<string, string>> dictList)
        {
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet("報表");

            //key
            var rowIndex = 0;
            var columnIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            foreach (var key in dictList.SelectMany(x => x.Keys).Distinct())
            {
                var cell = row.CreateCell(columnIndex);
                cell.SetCellValue(key);
                sheet.AutoSizeColumn(columnIndex);
                columnIndex++;
            }
            rowIndex++;
            row = sheet.CreateRow(rowIndex);
            //value
            foreach (var dict in dictList)
            {
                row = sheet.CreateRow(rowIndex);
                foreach (var key in dict.Keys)
                {
                    columnIndex = sheet.GetRow(0).First(x => x.StringCellValue == key).ColumnIndex;
                    row.CreateCell(columnIndex).SetCellValue(dict[key]);
                }
                rowIndex++;
            }
            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            wb.Write(fileStream);
        }
    }
}