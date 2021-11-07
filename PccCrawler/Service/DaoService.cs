using Dapper;
using System.Data;

namespace PccCrawler.Service
{
    public class DaoService
    {
        private readonly IDbConnection _conn;

        public DaoService(IDbConnection conn)
        {
            _conn = conn;
        }

        public IEnumerable<T> Query<T>(string query, IDictionary<string, object>? args = null)
        {
            // Dapper查詢資料，注意不能用IEnumerable<DataRow>來接結果
            IEnumerable<T> result;

            if (args != null)
            {
                var dynamicParams = new DynamicParameters();
                foreach (var pair in args)
                {
                    dynamicParams.Add(pair.Key, pair.Value);
                }
                result = this._conn.Query<T>(query, dynamicParams);
            }
            else
            {
                result = this._conn.Query<T>(query);
            }
            return result;
        }
    }
}
