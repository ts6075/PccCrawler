using Microsoft.Extensions.Options;
using PccCrawler.Model;
using PccCrawler.Service.Interface;

namespace PccCrawler.Service
{
    public class MockHttpService : IHttpService
    {
        private readonly HttpOption _options;

        public MockHttpService(IOptions<HttpOption> options)
        {
            _options = options.Value;
        }

        public IEnumerable<KeyValuePair<string, string>> GetFormData<T>(T query)
        {
            var formData = new List<KeyValuePair<string, string>>();
            if (query != null)
            {
                query.GetType().GetProperties().ToList().ForEach(p =>
                {
                    var keyPair = new KeyValuePair<string, string>(p.Name, p.GetValue(query)?.ToString());
                    formData.Add(keyPair);
                });
            }
            return formData;
        }

        public async Task<string> DoGetAsync(string url)
        {
            try
            {
                var path = _options.Mock招標公告資料;
                //var path = _options.Mock公開徵求資料;
                using var sr = new StreamReader(path);
                string resultContent = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                return resultContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> DoPostAsync(string url, IEnumerable<KeyValuePair<string, string>>? formData = null)
        {
            try
            {
                var path = _options.Mock招標公告列表;
                //var path = _options.Mock公開徵求列表;
                using var sr = new StreamReader(path);
                string resultContent = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                return resultContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
