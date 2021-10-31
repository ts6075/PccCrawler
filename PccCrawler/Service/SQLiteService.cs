using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace PccCrawler.Service
{
    public class SQLiteService
    {
        private readonly string _databaseName;
        private readonly SQLiteConnection _conn;
        private SQLiteCommand _cmd;
        private SQLiteDataAdapter _adapter;
        private string _connectionString => $"Data Source={_databaseName}";

        public SQLiteService(string databaseName)
        {
            _databaseName = databaseName;
            _conn = new SQLiteConnection(_connectionString);
            if (!File.Exists(_databaseName))
            {
                Init();
            }
        }

        /// <summary>
        /// 初始化SQLite
        /// </summary>
        private void Init()
        {
            if (File.Exists(_databaseName))
            {
                File.Delete(_databaseName);
            }
            SQLiteConnection.CreateFile(_databaseName);
            Execute("CREATE TABLE IF NOT EXISTS PccMaster( Id varchar(10), Url TEXT, Status int ) ");
        }

        public int Execute(string sql_statement)
        {
            _conn.Open();
            _cmd = _conn.CreateCommand();
            _cmd.CommandText = sql_statement;
            int row_updated;
            try
            {
                row_updated = _cmd.ExecuteNonQuery();
            }
            catch
            {
                _conn.Close();
                return 0;
            }
            _conn.Close();
            return row_updated;
        }

        public DataTable GetDataTable(string tablename)
        {
            var dt = new DataTable();
            _conn.Open();
            _cmd = _conn.CreateCommand();
            _cmd.CommandText = $"SELECT * FROM {tablename}";
            _adapter = new SQLiteDataAdapter(_cmd);
            _adapter.AcceptChangesDuringFill = false;
            _adapter.Fill(dt);
            _conn.Close();
            dt.TableName = tablename;
            return dt;
        }

        public IList<T> GetList<T>(string tablename)
        {
            var ds = new DataSet();
            _conn.Open();
            _cmd = _conn.CreateCommand();
            _cmd.CommandText = $"SELECT * FROM {tablename}";
            _adapter = new SQLiteDataAdapter(_cmd);
            _adapter.AcceptChangesDuringFill = false;
            _adapter.Fill(ds);
            _conn.Close();
            return DataSetToList<T>(ds, 0);
        }

        /// <summary>         
        /// DataSetToList         
        /// </summary>          
        /// <typeparam name="T">轉換類別</typeparam>         
        /// <param name="dataSet">資料源</param>         
        /// <param name="tableIndex">需要轉換表的索引</param>        
        /// /// <returns>泛型集合</returns>
        private IList<T> DataSetToList<T>(DataSet dataset, int tableIndex)
        {
            if (dataset == null || dataset.Tables.Count <= 0 || tableIndex < 0)
            {
                return null;
            }

            DataTable dt = dataset.Tables[tableIndex];
            IList<T> list = new List<T>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //建立泛型實體
                T rowData = Activator.CreateInstance<T>();

                //取得實體所有屬性
                PropertyInfo[] propertyInfo = rowData.GetType().GetProperties();

                //屬性和名稱相同時賦值
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        if (dt.Columns[j].ColumnName.Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                info.SetValue(rowData, dt.Rows[i][j], null);
                            }
                            else
                            {
                                info.SetValue(rowData, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(rowData);
            }
            return list;
        }
    }
}
