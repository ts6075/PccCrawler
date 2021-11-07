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
            services.Configure<CrawlerOption>(configuration.GetSection(nameof(CrawlerOption)));
            services.Configure<HttpOption>(configuration.GetSection(nameof(HttpOption)));
            services.AddTransient<CrawlerService>();
            services.AddSingleton<HttpClient>();
            if (configuration.GetSection("HttpOption").GetValue<string>("Mode") == "Debug")
            {
                services.AddTransient<IHttpService, MockHttpService>();
            }
            else
            {
                services.AddTransient<IHttpService, HttpService>();
            }

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