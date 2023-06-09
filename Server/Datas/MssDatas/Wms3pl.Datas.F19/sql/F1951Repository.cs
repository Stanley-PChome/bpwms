using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1951Repository : RepositoryBase<F1951, Wms3plDbContext, F1951Repository>
    {

        public IQueryable<F1951> GetDataByUctId(string uctId)
        {
            var parms = new List<SqlParameter> {
                new SqlParameter("@p0", uctId)
            };

            string sql = @" SELECT * FROM F1951 WHERE UCT_ID = @p0 
                            ORDER BY UCC_CODE ";

            return SqlQuery<F1951>(sql, parms.ToArray());
        }
    }
}
