using PccCrawler.Model;
using Microsoft.Extensions.Options;
using PccCrawler.Service.Interface;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly I招標公告Service _招標公告Service;
        private readonly I公開徵求Service _公開徵求Service;
        private readonly I公開閱覽Service _公開閱覽Service;
        private readonly I政府採購預告Service _政府採購預告Service;
        private readonly I決標公告Service _決標公告Service;
        private readonly I無法決標公告Service _無法決標公告Service;
        private readonly I更正公告Service _更正公告Service;

        public CrawlerService(IOptions<CrawlerOption> options, I招標公告Service 招標公告Service,
                              I公開徵求Service 公開徵求Service, I公開閱覽Service 公開閱覽Service,
                              I政府採購預告Service 政府採購預告Service, I決標公告Service 決標公告Service,
                              I無法決標公告Service 無法決標公告Service, I更正公告Service 更正公告Service)
        {
            _options = options.Value;
            _招標公告Service = 招標公告Service;
            _公開徵求Service = 公開徵求Service;
            _公開閱覽Service = 公開閱覽Service;
            _政府採購預告Service = 政府採購預告Service;
            _決標公告Service = 決標公告Service;
            _無法決標公告Service = 無法決標公告Service;
            _更正公告Service = 更正公告Service;
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
            Console.WriteLine("Search Database...");
            await _招標公告Service.DoJob();
            await _公開徵求Service.DoJob();
            await _公開閱覽Service.DoJob();
            await _政府採購預告Service.DoJob();
            await _決標公告Service.DoJob();
            await _無法決標公告Service.DoJob();
            await _更正公告Service.DoJob();
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
        }
    }
}