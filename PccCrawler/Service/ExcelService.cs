using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PccCrawler.Service
{
    public class ExcelService
    {
        /// <summary>
        /// 寫入Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="savePath"></param>
        /// <param name="list"></param>
        public void WriteExcel<T>(string savePath, List<T> list)
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

        /// <summary>
        /// 取得指定Type所有PropertiesName
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 設置指定物件所有Property Value寫至Excel，且該Property Name必須於Excel第一列中找到，並寫於對應的Column Index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        /// <param name="obj"></param>
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
