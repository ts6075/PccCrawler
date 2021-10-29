using HtmlAgilityPack;

namespace PccCrawler
{
    /// <summary>
    /// 下載HTML幫助類
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// 從Url地址下載頁面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static ValueTask<HtmlDocument> LoadHtmlFromUrlAsync(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return await
                web?.LoadFromWebAsync(url);
        }

        /// <summary>
        /// 獲取單個節點擴充套件方法
        /// </summary>
        /// <param name="htmlDocument">文件物件</param>
        /// <param name="xPath">xPath路徑</param>
        /// <returns></returns>
        public static HtmlNode GetSingleNode(this HtmlDocument htmlDocument, string xPath)
        {
            return htmlDocument?.DocumentNode?.SelectSingleNode(xPath);
        }

        /// <summary>
        /// 獲取多個節點擴充套件方法
        /// </summary>
        /// <param name="htmlDocument">文件物件</param>
        /// <param name="xPath">xPath路徑</param>
        /// <returns></returns>
        public static HtmlNodeCollection GetNodes(this HtmlDocument htmlDocument, string xPath)
        {
            return htmlDocument?.DocumentNode?.SelectNodes(xPath);
        }

        /// <summary>
        /// 獲取多個節點擴充套件方法
        /// </summary>
        /// <param name="htmlDocument">文件物件</param>
        /// <param name="xPath">xPath路徑</param>
        /// <returns></returns>
        public static HtmlNodeCollection GetNodes(this HtmlNode htmlNode, string xPath)
        {
            return htmlNode?.SelectNodes(xPath);
        }

        /// <summary>
        /// 獲取單個節點擴充套件方法
        /// </summary>
        /// <param name="htmlDocument">文件物件</param>
        /// <param name="xPath">xPath路徑</param>
        /// <returns></returns>
        public static HtmlNode GetSingleNode(this HtmlNode htmlNode, string xPath)
        {
            return htmlNode?.SelectSingleNode(xPath);
        }

        /// <summary>
        /// 下載圖片
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="filpath">檔案路徑</param>
        /// <returns></returns>
        public async static ValueTask<bool> DownloadImg(string url, string filpath)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                var bytes = await httpClient.GetByteArrayAsync(url);
                using (FileStream fs = File.Create(filpath))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                return File.Exists(filpath);
            }
            catch (Exception ex)
            {

                throw new Exception("下載圖片異常", ex);
            }

        }
    }
}