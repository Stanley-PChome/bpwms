using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19120601Repository : RepositoryBase<F19120601, Wms3plDbContext, F19120601Repository>
    {

        public IQueryable<F19120601> GetF19120601sSelectItem(string dcCode, string pkArea)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", pkArea)
            };

            var sql = $@"SELECT *
                        FROM F19120601
                        WHERE DC_CODE = @p0
                              AND PK_AREA = @p1
                        ORDER BY BEGIN_LOC_CODE";
            var result = SqlQuery<F19120601>(sql, param.ToArray());
            return result;
        }

        public void Delete(List<F19120601> f19120601s)
        {
            List<SqlParameter> param;
            foreach (var item in f19120601s)
            {
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@p0", f19120601s.First().DC_CODE));
                param.Add(new SqlParameter("@p1", item.BEGIN_LOC_CODE));
                param.Add(new SqlParameter("@p2", item.END_LOC_CODE));
                ExecuteSqlCommand("DELETE FROM F19120601 WHERE DC_CODE=@p0 AND BEGIN_LOC_CODE=@p1 AND END_LOC_CODE=@p2", param.ToArray());
            }
        }

        public void Delete(F19120601 f19120601s)
        {
            List<SqlParameter> param;
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", f19120601s.DC_CODE));
            param.Add(new SqlParameter("@p1", f19120601s.BEGIN_LOC_CODE));
            param.Add(new SqlParameter("@p2", f19120601s.END_LOC_CODE));
            ExecuteSqlCommand("DELETE FROM F19120601 WHERE DC_CODE=@p0 AND BEGIN_LOC_CODE=@p1 AND END_LOC_CODE=@p2", param.ToArray());

        }

    }
}
