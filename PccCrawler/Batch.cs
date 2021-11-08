using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PccCrawler.Model;
using PccCrawler.Service;
using PccCrawler.Service.Interface;
using System.Data;
using System.Data.SqlClient;

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
            var crawler = serviceProvider.GetRequiredService<CrawlerService>();
            Task task = Task.Factory.StartNew(async () => await crawler.RunAsync());
            task.Wait();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            // DI Config
            services.Configure<CrawlerOption>(configuration.GetSection(nameof(CrawlerOption)));
            services.Configure<HttpOption>(configuration.GetSection(nameof(HttpOption)));
            // DI Common
            services.AddSingleton<HttpClient>();
            // DI Service
            services.AddTransient<CrawlerService>();
            if (configuration.GetSection("HttpOption").GetValue<string>("Mode") == "Debug")
            {
                services.AddTransient<IHttpService, MockHttpService>();
            }
            else
            {
                services.AddTransient<IHttpService, HttpService>();
            }
            services.AddTransient<I招標公告Service, 招標公告Service>();
            services.AddTransient<I公開徵求Service, 公開徵求Service>();
            services.AddTransient<I公開閱覽Service, 公開閱覽Service>();
            services.AddTransient<I政府採購預告Service, 政府採購預告Service>();
            services.AddTransient<I決標公告Service, 決標公告Service>();
            services.AddTransient<I無法決標公告Service, 無法決標公告Service>();
            services.AddTransient<I更正公告Service, 更正公告Service>();
        services.AddTransient<IAnalyzeService, AnalyzeService>();
            // DI Process
            // DI Dao
            services.AddScoped<IDbConnection, SqlConnection>(serviceProvider => {
                //var connString = configuration.GetConnectionString("DefaultConnection");
                var connString = configuration.GetSection("ConnectString").GetValue<string>("MSSQLConn");
                var conn = new SqlConnection(connString);
                return conn;
            });
            services.AddTransient<DaoService>();
            return;
        }
    }
}