using HtmlAgilityPack;
using PccCrawler.Model;
using PccCrawler.PccEnum;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PccCrawler.Service.Interface;
using Microsoft.Extensions.Options;

namespace PccCrawler.Service
{
    public class CrawlerService
    {
        private readonly CrawlerOption _options;
        private readonly IHttpService _httpService;

        public CrawlerService(IOptions<CrawlerOption> options, IHttpService httpService)
        {
            _options = options.Value;
            _httpService = httpService;
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
            var dao = new SQLiteService($"{Environment.CurrentDirectory}/data/db2.db");
            var masterList = dao.GetList<PccMasterPo>("PccMaster");
            foreach (var radProctrgCate in new RadProctrgCate[] { RadProctrgCate.�u�{, RadProctrgCate.�]��, RadProctrgCate.�Ұ� })
            {
                Console.WriteLine($"Crawling List:{radProctrgCate}...");
                var pks = new List<string>();
                var total = await GetTotalItem(radProctrgCate);
                var totalPage = total switch
                {
                    0 => 0,
                    > 0 and <= 100 => 1,
                    > 100 => total / 100 + (total % 100 != 0 ? 1 : 0),
                    _ => 0
                };
                for (var pageIndex = 1; pageIndex <= totalPage; pageIndex++)
                {
                    var doc = await GetHtmlDoc(pageIndex, radProctrgCate);
                    var trNodes = doc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
                    if (trNodes == null)
                    {
                        Console.WriteLine("Get List Fail");
                        return;
                    }
                    for (var i = 1; i < trNodes.Count - 1; i++)
                    {
                        var hrefNode = trNodes[i].SelectSingleNode("./td[4]/a");
                        var href = hrefNode.GetAttributeValue("href", null);
                        var pk = href.Contains("primaryKey=") ? href.Split("primaryKey=")[1] : "";
                        // �ˬd��ƬO�_�������L
                        if (!masterList.Any(x => x.Status == 900 && x.Id == pk))
                        {
                            if (masterList.Any(x => x.Id == pk))
                            {
                                dao.Execute($"update PccMaster set Id = {pk}, Url ='{GetUrl(UrlType.tpam_tender_detail, pk)}', Status = 100 where Id = {pk}");
                            }
                            else
                            {
                                dao.Execute($"insert into PccMaster (Id, Url, Status) values ({pk}, '{GetUrl(UrlType.tpam_tender_detail, pk)}', 100) ");
                            }
                            pks.Add(pk);
                        }
                    }

                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                }
                Console.WriteLine($"TotalItem: {total} count�ANewItem: {pks.Count} count");

                var stopWatch = new Stopwatch();
                var analyzeService = new AnalyzeService();
                var �ۼФ��iPos = new List<�ۼФ��iPo>();
                foreach (var pk in pks)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    Console.WriteLine($"Crawling Detail:{pk}...");
                    try
                    {
                        var detailDoc = await GetDetailHtmlDoc(pk);
                        var po = analyzeService.Analyze<�ۼФ��iPo>(detailDoc);
                        po.Url = GetUrl(UrlType.tpam_tender_detail, pk);
                        �ۼФ��iPos.Add(po);
                        dao.Execute($"update PccMaster set Status = 900 where Id = {pk} ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    stopWatch.Stop();
                    int totalSeconds = (int)stopWatch.Elapsed.TotalSeconds;
                    Console.WriteLine("RunTime:" + totalSeconds);
                    if (_options.Mode == "Debug")
                    {
                        break;
                    }
                    if (totalSeconds < _options.IntervalSeconds)
                    {
                        Console.WriteLine($"Use time is too short, a little delay:{15 - totalSeconds}");
                        Thread.Sleep((_options.IntervalSeconds - totalSeconds) * 1000);
                    }
                }
                WriteExcel($"{Environment.CurrentDirectory}/output/{radProctrgCate}.xlsx", �ۼФ��iPos);
            }
            Console.WriteLine("Writing to file...");
            Console.WriteLine("End");
            return;
        }

        #region Done
        private string GetUrl(UrlType type, params string[] args)
        {
            var domain = "https://web.pcc.gov.tw";
            var area = "tps";
            var path = "pss/tender.do";
            var searchMode = SearchMode.common;
            var searchType = SearchType.advance;
            var url = type switch
            {
                UrlType.tender => $"{domain}/{area}/{path}?searchMode={searchMode}&searchType={searchType}",
                UrlType.tpam_tender_detail => $"{domain}/{area}/tpam/main/tps/tpam/tpam_tender_detail.do?searchMode={searchMode}&primaryKey={args[0]}",
                _ => $"{domain}/{area}/{path}?searchMode={searchMode}&searchType={searchType}"
            };
            return url;
        }

        private async Task<int> GetTotalItem(RadProctrgCate radProctrgCate)
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpService.GetFormData(new SearchVo
            {
                proctrgCate = (int)radProctrgCate,
                radProctrgCate = (int)radProctrgCate
            });
            var resp = await _httpService.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);

            var totalStr = doc.GetElementbyId("print_area")?.SelectSingleNode(".//span[@class=\"T11b\"]")?.InnerText;
            _ = int.TryParse(totalStr, out var total);
            return total;
        }

        public async Task<HtmlDocument> GetHtmlDoc(int pageIndex, RadProctrgCate radProctrgCate)
        {
            var url = GetUrl(UrlType.tender);
            var formData = _httpService.GetFormData(new SearchVo
            {
                pageIndex = pageIndex,
                proctrgCate = (int)radProctrgCate,
                radProctrgCate = (int)radProctrgCate
            });
            var resp = await _httpService.DoPostAsync(url, formData);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }

        public async Task<HtmlDocument> GetDetailHtmlDoc(string pk)
        {
            var url = GetUrl(UrlType.tpam_tender_detail, pk);
            var resp = await _httpService.DoGetAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(resp);
            return doc;
        }
        #endregion

        private void WriteExcel<T>(string savePath, List<T> list)
        {
            IWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet(Path.GetFileNameWithoutExtension(savePath));

            //key
            var rowIndex = 0;
            var columnIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            foreach (var key in GetAllPropertiesName(typeof(T)))
            {
                var cell = row.CreateCell(columnIndex);
                cell.SetCellValue(key);
                sheet.AutoSizeColumn(columnIndex);
                columnIndex++;
            }
            rowIndex++;
            row = sheet.CreateRow(rowIndex);
            //value
            foreach (var obj in list)
            {
                row = sheet.CreateRow(rowIndex);
                SetExcelOneRow(sheet, row, obj);
                rowIndex++;
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }
            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            wb.Write(fileStream);
        }

        private List<string> GetAllPropertiesName(Type type)
        {
            var result = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    result.Add(prop.Name);
                }
                else
                {
                    result = result.Concat(GetAllPropertiesName(prop.PropertyType)).ToList();
                }
            }
            return result;
        }

        private void SetExcelOneRow<T>(ISheet sheet, IRow row, T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string)prop.GetValue(obj);
                    var columnIndex = sheet.GetRow(0).First(x => x.StringCellValue == prop.Name).ColumnIndex;
                    row.CreateCell(columnIndex).SetCellValue(value);
                }
                else
                {
                    var propValueObj = obj.GetType().GetProperty(prop.Name).GetValue(obj);
                    if (propValueObj != null)
                    {
                        SetExcelOneRow(sheet, row, propValueObj);
                    }
                }
            }
        }
    }
}