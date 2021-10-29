using Microsoft.Extensions.DependencyInjection;
using PccCrawler.Service;
using PccCrawler.Service.Interface;
using System.Text;
using System.Text.Json;

namespace PccCrawler
{
    public class Batch
    {
        public static void Main(string[] args)
        {
            // 1. 建立依賴注入的容器
            var serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            serviceCollection.AddTransient<Crawler>();
            serviceCollection.AddSingleton<HttpClient>();
            serviceCollection.AddTransient<IHttpService, HttpService>();
            // 建立依賴服務提供者
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // 3. 執行主服務
            var crawler = serviceProvider.GetRequiredService<Crawler>();
            Task task = Task.Factory.StartNew(async () => await crawler.RunAsync());
            task.Wait();
        }
    }
}