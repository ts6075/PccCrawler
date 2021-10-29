using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PccCrawler.Model;
using PccCrawler.Service;
using PccCrawler.Service.Interface;

namespace PccCrawler
{
    public class Batch
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsetting.json");
            IConfiguration configuration = builder.Build();
            // 1. 建立依賴注入的容器
            var serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            ConfigureServices(serviceCollection, configuration);
            // 3. 建立依賴服務提供者
            var serviceProvider = serviceCollection.BuildServiceProvider();
            // 4. 執行主服務
            var crawler = serviceProvider.GetRequiredService<Crawler>();
            Task task = Task.Factory.StartNew(async () => await crawler.RunAsync());
            task.Wait();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<CrawlerOption>(configuration.GetSection(nameof(CrawlerOption)));
            services.AddTransient<Crawler>();
            services.AddSingleton<HttpClient>();
            services.AddTransient<IHttpService, HttpService>();
            return;
        }
    }
}