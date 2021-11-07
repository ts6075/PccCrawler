using HtmlAgilityPack;

namespace PccCrawler.Service.Interface
{
    public interface IAnalyzeService
    {
        public void Analyze招標公告(HtmlDocument detailDoc);
    }
}
