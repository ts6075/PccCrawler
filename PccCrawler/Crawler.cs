using HtmlAgilityPack;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using System.Net.Http.Headers;
using System.Text;
using PccCrawler.Service;
using PccCrawler.Extension;

namespace PccCrawler
{
    public class Crawler
    {
        private readonly HttpService _httpHelper;

        public Crawler(HttpClient client)
        {
            _httpHelper = new HttpService(client);
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
            Console.WriteLine("取得最新招標清單中...");
            var pks = new List<string>();
            var total = await GetTotalItem();
            for (var pageIndex = 1; pageIndex < total / 100 + 2; pageIndex++)
            {
                var doc = await GetHtmlDoc(pageIndex);
                var trNodes = doc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
                for (var i = 1; i < trNodes.Count - 1; i++)
                {
                    var hrefNode = trNodes[i].SelectSingleNode("./td[4]/a");
                    var href = hrefNode.GetAttributeValue("href", null);
                    var pk = href.Contains("primaryKey=") ? href.Split("primaryKey=")[1] : "";
                    pks.Add(pk);
                    break;
                }
                break;
            }
            Console.WriteLine($"{pks.Count}筆");
            Console.WriteLine("爬取中");
            var detailDoc = await GetDetailHtmlDoc(pks.First());
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
            AnalyzeAndConsole(region1);
            AnalyzeAndConsole(region2);
            AnalyzeAndConsole(region3);
            AnalyzeAndConsole(region4);
            AnalyzeAndConsole(region5);
            Console.WriteLine("結束");
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

        private async Task<int> GetTotalItem()
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpHelper.GetFormData(new SearchVo());
            var resp = await _httpHelper.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);

            var totalStr = doc.GetElementbyId("print_area")?.SelectSingleNode(".//span[@class=\"T11b\"]")?.InnerText;
            _ = int.TryParse(totalStr, out var total);
            return total;
        }

        public async Task<HtmlDocument> GetHtmlDoc(int pageIndex)
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpHelper.GetFormData(new SearchVo { pageIndex = pageIndex });
            var resp = await _httpHelper.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }

        public async Task<HtmlDocument> GetDetailHtmlDoc(string pk)
        {
            var url = GetUrl(UrlType.tpam_tender_detail, pk);
            var resp = await _httpHelper.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion

        private void AnalyzeAndConsole(List<HtmlNode> htmlNodes)
        {
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
                Console.WriteLine($"Key:{key}\tValue:{value}");
                //break;
            }
        }
    }
}