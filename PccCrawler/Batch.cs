using System.Text;
using System.Text.Json;

namespace PccCrawler
{
    public class Batch
    {
        public static void Main(string[] args)
        {
            #region Test
            /*
            Task task1 = Task.Factory.StartNew(async() =>
            {
                var person = new
                {
                    Name = "John Doe",
                    Occupation = "gardener"
                };

                var json = JsonSerializer.Serialize(person);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "https://httpbin.org/post";
                using var client = new HttpClient();

                var response = await client.PostAsync(url, data);

                string result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            });
            task1.Wait();
            return;
            */
            #endregion

            var crawler = new Crawler(new HttpClient());
            Task task = Task.Factory.StartNew(async () => await crawler.RunAsync());
            task.Wait();
        }
    }
}