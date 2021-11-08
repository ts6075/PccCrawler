using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using PccCrawler.Service.Interface;
using System.Diagnostics;

namespace PccCrawler.Service
{
    public class 決標公告Service : I決標公告Service
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;
        private readonly DaoService _dao;
        private readonly IAnalyzeService _analyzeService;

        public 決標公告Service(IOptions<CrawlerOption> options, IHttpService httpService, DaoService dao, IAnalyzeService analyzeService)
        {
            _options = options.Value;
            _httpService = httpService;
            _dao = dao;
            _analyzeService = analyzeService;
        }

        public async Task DoJob()
        {
        }
    }
}
