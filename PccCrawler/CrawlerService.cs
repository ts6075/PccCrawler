using PccCrawler.Model;
using Microsoft.Extensions.Options;
using PccCrawler.Service.Interface;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly I�ۼФ��iService _�ۼФ��iService;
        private readonly I���}�x�DService _���}�x�DService;
        private readonly I���}�\��Service _���}�\��Service;
        private readonly I�F�����ʹw�iService _�F�����ʹw�iService;
        private readonly I�M�Ф��iService _�M�Ф��iService;
        private readonly I�L�k�M�Ф��iService _�L�k�M�Ф��iService;
        private readonly I�󥿤��iService _�󥿤��iService;

        public CrawlerService(IOptions<CrawlerOption> options, I�ۼФ��iService �ۼФ��iService,
                              I���}�x�DService ���}�x�DService, I���}�\��Service ���}�\��Service,
                              I�F�����ʹw�iService �F�����ʹw�iService, I�M�Ф��iService �M�Ф��iService,
                              I�L�k�M�Ф��iService �L�k�M�Ф��iService, I�󥿤��iService �󥿤��iService)
        {
            _options = options.Value;
            _�ۼФ��iService = �ۼФ��iService;
            _���}�x�DService = ���}�x�DService;
            _���}�\��Service = ���}�\��Service;
            _�F�����ʹw�iService = �F�����ʹw�iService;
            _�M�Ф��iService = �M�Ф��iService;
            _�L�k�M�Ф��iService = �L�k�M�Ф��iService;
            _�󥿤��iService = �󥿤��iService;
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
            await _�ۼФ��iService.DoJob();
            await _���}�x�DService.DoJob();
            await _���}�\��Service.DoJob();
            await _�F�����ʹw�iService.DoJob();
            await _�M�Ф��iService.DoJob();
            await _�L�k�M�Ф��iService.DoJob();
            await _�󥿤��iService.DoJob();
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
        }
    }
}