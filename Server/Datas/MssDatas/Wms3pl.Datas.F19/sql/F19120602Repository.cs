using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19120602Repository : RepositoryBase<F19120602, Wms3plDbContext, F19120602Repository>
    {
        public IQueryable<F19120602> GetF19120602s(string dcCode, string pkArea)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", pkArea)
            };

            var sql = $@"SELECT *
                        FROM F19120602
                        WHERE DC_CODE = @p0
                              AND PK_AREA = @p1
                        ORDER BY PK_AREA";
            return SqlQuery<F19120602>(sql, param.ToArray());
        }

        /// <summary>
        /// 先增資料前檢查F19120601是否有重複
        /// </summary>
        /// <param name="DC_CODE"></param>
        /// <param name="PK_AREA"></param>
        /// <param name="Loc_Code"></param>
        /// <returns></returns>
        public Boolean CheckF19120602Duplicate(F19120602 f19120602s)
        {
            StringBuilder sbsqlLoc_Code = new StringBuilder();
            StringBuilder sbsql;

            var param = new List<SqlParameter>() {
                new SqlParameter("@p0", f19120602s.DC_CODE),
                new SqlParameter("@p1", f19120602s.PK_AREA)
            };
            sbsqlLoc_Code.Append($"@p{param.Count},");
            param.Add(new SqlParameter($"@p{param.Count}", f19120602s.CHK_LOC_CODE));

            sbsqlLoc_Code.Remove(sbsqlLoc_Code.Length - 1, 1);

            sbsql = new StringBuilder(
            $@"SELECT TOP 1
	                *
                FROM
	                F19120602
                WHERE 
	                DC_CODE = @p0
	                AND PK_AREA = @p1
	                AND CHK_LOC_CODE IN ({sbsqlLoc_Code})");
            return SqlQuery<F19120602>(sbsql.ToString(), param.ToArray()).Any();
        }

        public void Delete(List<F19120602> f19120602s)
        {
            List<SqlParameter> param;
            foreach (var item in f19120602s)
            {
                param = new List<SqlParameter>();
                param.Add(new SqlParameter("@p0", f19120602s.First().DC_CODE));
                param.Add(new SqlParameter("@p1", item.CHK_LOC_CODE));
                ExecuteSqlCommand("DELETE FROM F19120602 WHERE DC_CODE=@p0 AND CHK_LOC_CODE=@p1", param.ToArray());
            }
        }
        public void Delete(F19120602 f19120602s)
        {
            List<SqlParameter> param;
            param = new List<SqlParameter>();
            param.Add(new SqlParameter("@p0", f19120602s.DC_CODE));
            param.Add(new SqlParameter("@p1", f19120602s.CHK_LOC_CODE));
            ExecuteSqlCommand("DELETE FROM F19120602 WHERE DC_CODE=@p0 AND CHK_LOC_CODE=@p1", param.ToArray());

        }

    }
}
