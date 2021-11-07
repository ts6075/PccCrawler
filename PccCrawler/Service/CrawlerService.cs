using PccCrawler.Model;
using Microsoft.Extensions.Options;
using PccCrawler.Process.Interface;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly I�ۼФ��iProcess _�ۼФ��iProcess;

        public CrawlerService(IOptions<CrawlerOption> options, I�ۼФ��iProcess �ۼФ��iProcess)
        {
            _options = options.Value;
            _�ۼФ��iProcess = �ۼФ��iProcess;
        }

        public async Task RunAsync()
        {
            /* �ݨD�G
               1�B���ۼФ��i�B���}�x�D�B���}�\���B�F�����ʹw�i(��)
               2�B�u�{�B�]�ȡB�Ұ�
               3�B�M�Ф��i
               4�B�L�k�M�Ф��i
               5�B�󥿤��i
             */
            Console.WriteLine("Start");
            Console.WriteLine("Search Database...");
            await _�ۼФ��iProcess.DoJob();
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
        }
    }
}