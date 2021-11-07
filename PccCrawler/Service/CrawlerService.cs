using PccCrawler.Model;
using Microsoft.Extensions.Options;
using PccCrawler.Process.Interface;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly I招標公告Process _招標公告Process;

        public CrawlerService(IOptions<CrawlerOption> options, I招標公告Process 招標公告Process)
        {
            _options = options.Value;
            _招標公告Process = 招標公告Process;
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
            await _招標公告Process.DoJob();
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
        }
    }
}