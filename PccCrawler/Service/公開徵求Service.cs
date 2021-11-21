using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PccCrawler.Extension;
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
            var vos = new List<公開徵求InfoVo>();
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
                    // 取得form hidden資訊
                    var vo = new 公開徵求InfoVo
                    {
                        pkTpAppeal = formNodes[i].SelectSingleNode(".//input[@id=\"pkTpAppeal\"]")?.GetAttributeValue("value", string.Empty),
                        orgId = formNodes[i].SelectSingleNode(".//input[@id=\"orgId\"]")?.GetAttributeValue("value", string.Empty),
                        orgName = formNodes[i].SelectSingleNode(".//input[@id=\"orgName\"]")?.GetAttributeValue("value", string.Empty),
                        tenderCaseNo = formNodes[i].SelectSingleNode(".//input[@id=\"tenderCaseNo\"]")?.GetAttributeValue("value", string.Empty),
                        tenderSq = formNodes[i].SelectSingleNode(".//input[@id=\"tenderSq\"]")?.GetAttributeValue("value", string.Empty),
                        pkTpAppealHis = formNodes[i].SelectSingleNode(".//input[@id=\"pkTpAppealHis\"]")?.GetAttributeValue("value", string.Empty)
                    };
                    var caseNo = vo.GetCaseNo();

                    // 檢查資料是否曾爬取過且成功
                    if (!masterList.Any(x => x.CaseNo == caseNo && x.Status == 900))
                    {
                        var url = GetUrl(UrlType.公開徵求_詳細資料頁, vo);
                        var pairs = new Dictionary<string, object>
                        {
                            { nameof(caseNo), caseNo },
                            { nameof(url), url }
                        };
                        if (masterList.Any(x => x.CaseNo == caseNo))
                        {
                            _dao.Query<int>($"update PccMaster set Url = @url, Status = 100, UpdateTime = getdate() where CaseNo = @caseNo and Category = '公開徵求'", pairs);
                        }
                        else
                        {
                            _dao.Query<int>($"insert into PccMaster (CaseNo, Category, Url, Status) values (@caseNo, '公開徵求', @url, 100)", pairs);
                        }
                        vos.Add(vo);
                    }
                }

                if (_options.Mode == "Debug")
                {
                    break;
                }
            }
            Console.WriteLine($"TotalItem: {total} count，NewItem: {vos.Count} count");
            #endregion

            foreach (var vo in vos)
            {
                var caseNo = vo.GetCaseNo();
                Console.WriteLine($"Crawling Detail:{caseNo}...");
                try
                {
                    var detailDoc = await GetDetailHtmlDoc(vo);
                    _analyzeService.Analyze公開徵求(detailDoc);
                    var pairs = new Dictionary<string, object>
                    {
                        { nameof(caseNo), caseNo },
                    };
                    _dao.Query<int>($"update PccMaster set Status = 900 where CaseNo = @caseNo and Category = '公開徵求'", pairs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"vo: {vo.ToJson()} {Environment.NewLine}" +
                                      $"Msg: {ex.Message}");
                    var pairs = new Dictionary<string, object>
                    {
                        { nameof(caseNo), caseNo },
                        { nameof(ex.Message), ex.Message }
                    };
                    _dao.Query<int>($"insert into LogEvent(EventLevel ,EventType, EventContent, CaseNo) " +
                                    $"values('Error', '公開徵求', @Message, @caseNo)", pairs);
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
        /// <param name="vo">公開徵求InfoVo</param>
        /// <returns></returns>
        private string GetUrl(UrlType type, 公開徵求InfoVo? vo = null)
        {
            var domain = "https://web.pcc.gov.tw";
            var url = type switch
            {
                UrlType.公開徵求_搜尋結果頁 => $"{domain}/tps/tps/tp/main/tps/tp/searchAppealVendor.do?pMenu=common",
                UrlType.公開徵求_詳細資料頁 => $"{domain}/tps/tps/tp/main/tps/tp/tp.do?method=initialAppealViewVendor&pMenu=common" +
                                               $"&pkTpAppeal={vo?.pkTpAppeal}&orgId={vo?.orgId}&orgName={vo?.orgName}" +
                                               $"&tenderCaseNo={vo?.tenderCaseNo}&tenderSq={vo?.tenderSq}&pkTpAppealHis={vo?.pkTpAppealHis}",
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
        /// <param name="vo">公開徵求InfoVo</param>
        /// <returns></returns>
        public async Task<HtmlDocument> GetDetailHtmlDoc(公開徵求InfoVo vo)
        {
            var url = GetUrl(UrlType.公開徵求_詳細資料頁, vo);
            var resp = await _httpService.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion
    }
}
