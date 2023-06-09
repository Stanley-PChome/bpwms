using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1915Repository : RepositoryBase<F1915, Wms3plDbContext, F1915Repository>
    {
        public List<P192019Item> GetF1915SearchData(string gupCode, string custCode, string aCode, string clsName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@p0", gupCode));
            parameters.Add(new SqlParameter("@p1", custCode));

            var sql = @"
SELECT ROW_NUMBER ()OVER(ORDER BY F1915.GUP_CODE,F1915.CUST_CODE,F1915.ACODE) ROWNUM,
       ACODE,
       CLA_NAME ANAME,
       '' BCODE,
       '' BNAME,
       '' CCODE,
       '' CNAME,
       CRT_STAFF,
       CRT_NAME,
       CRT_DATE,
       UPD_STAFF,
       UPD_NAME,
       UPD_DATE,       
       GUP_CODE,
       CONVERT(CHAR,ISNULL(CHECK_PERCENT, 0)) CHECK_PERCENT
  FROM F1915
 WHERE GUP_CODE = @p0
   AND CUST_CODE = @p1
";

            if (!string.IsNullOrWhiteSpace(aCode))
            {
                sql += " AND ACODE = @p2 ";
                parameters.Add(new SqlParameter("@p2", aCode));
            }
            if (!string.IsNullOrWhiteSpace(clsName))
            {
                sql += " AND CLA_NAME LIKE CONCAT('%',@p3 ,'%')";
                parameters.Add(new SqlParameter("@p3", clsName));
            }

            var query = SqlQuery<P192019Item>(sql, parameters.ToArray()).ToList();
            return query;
        }

        public void DeleteByCustCodes(string gupCode, string aCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1915
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1 ";
            var parms = new List<object> { gupCode, aCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
