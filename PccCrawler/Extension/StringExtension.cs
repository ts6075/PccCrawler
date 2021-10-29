namespace PccCrawler.Extension
{
    public static class StringExtension
    {
        public static string TrimEmpty(this string str)
        {
            return str.Replace("\r\n", string.Empty).Replace("\t", string.Empty).Trim();
        }
    }
}