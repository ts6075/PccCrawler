using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PccCrawler.Service
{
    public class DaoService
    {
        private readonly IDbConnection _conn;

        public DaoService(IDbConnection conn)
        {
            _conn = conn;
        }

        public IEnumerable<T> Query<T>(string query)
        {
            //Dapper查詢資料，注意不能用IEnumerable<DataRow>來接結果
            IEnumerable<T> result = this._conn.Query<T>(query);
            return result;
        }
    }
}
