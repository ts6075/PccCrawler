using HtmlAgilityPack;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using System.Net.Http.Headers;
using System.Text;
using PccCrawler.Service;
using PccCrawler.Extension;
using System.Net;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PccCrawler.Service.Interface;

namespace PccCrawler
{
    public class Crawler
    {
        private readonly IHttpService _httpService;

        public Crawler(IHttpService httpService)
        {
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
                    break;
                }
                Console.WriteLine($"TotalItem:{pks.Count} count");

                Stopwatch stopWatch = new Stopwatch();
                var allDatas = new List<Dictionary<string, string>>();
                foreach (var pk in pks)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    Console.WriteLine($"Crawling Detail:{pk}...");
                    var detailDoc = await GetDetailHtmlDoc(pk);
                    var detailTrNodes = detailDoc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
                    if (detailTrNodes == null)
                    {
                        Console.WriteLine("Get Detail Fail");
                        return;
                    }
                    var region1 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_1").ToList();
                    var region2 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_2").ToList();
                    var region3 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_3").ToList();
                    var region4 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_4").ToList();
                    var region5 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_5").ToList();
                    allDatas.Add(AnalyzeAndConsole(region1));
                    allDatas.Add(AnalyzeAndConsole(region2));
                    allDatas.Add(AnalyzeAndConsole(region3));
                    allDatas.Add(AnalyzeAndConsole(region4));
                    allDatas.Add(AnalyzeAndConsole(region5));
                    stopWatch.Stop();
                    int totalSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    Console.WriteLine("RunTime:" + totalSeconds);
                    if (totalSeconds < 15)
                    {
                        Console.WriteLine($"Use time is too short, a little delay:{15 - totalSeconds}");
                        Thread.Sleep((15 - totalSeconds) * 1000);
                    }
                    break;
                }
                WriteExcel($@"C:\Users\User.DESKTOP-4FQUFIT\source\repos\PccCrawler\PccCrawler\{radProctrgCate}.xls", allDatas);
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

        private Dictionary<string,string> AnalyzeAndConsole(List<HtmlNode> htmlNodes)
        {
            var result = new Dictionary<string,string>();
            foreach (var trNode in htmlNodes)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().InnerHtml.TrimEmpty();
                }

                var tds = trNode.SelectNodes("./td");
                if (tds.Count == 0)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else if (tds.Count > 1)
                {
                    value = tds.Skip(1).First().InnerText.TrimEmpty();
                }
                else
                {
                    value = tds.First().InnerHtml.TrimEmpty();
                }
                result.Add(key, value);
                Console.WriteLine($"Key:{key}\tValue:{value}");
            }
            return result;
        }

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