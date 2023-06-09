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
    public partial class F1917Repository : RepositoryBase<F1917, Wms3plDbContext, F1917Repository>
    {
        public List<P192019Item> GetF1917SearchData(string gupCode, string custCode, string cCode, string clsName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@p0", gupCode));
            parameters.Add(new SqlParameter("@p1", custCode));

            var sql = @"
SELECT ROW_NUMBER ()OVER(ORDER BY A.GUP_CODE,A.CUST_CODE,A.ACODE,A.BCODE,A.CCODE) ROWNUM,
       A.ACODE,
       C.CLA_NAME ANAME,
       A.BCODE,
       B.CLA_NAME BNAME,
       A.CCODE,
       A.CLA_NAME CNAME,
       A.CRT_STAFF,
       A.CRT_NAME,
       A.CRT_DATE,
       A.UPD_STAFF,
       A.UPD_NAME,
       A.UPD_DATE,       
       A.GUP_CODE,
       CONVERT(CHAR,A.CHECK_PERCENT) CHECK_PERCENT
  FROM F1917 A
       LEFT OUTER JOIN F1916 B
          ON     A.ACODE = B.ACODE
             AND A.BCODE = B.BCODE
             AND A.GUP_CODE = B.GUP_CODE
             AND A.CUST_CODE = B.CUST_CODE
       LEFT OUTER JOIN F1915 C
          ON     A.ACODE = C.ACODE
             AND A.GUP_CODE = C.GUP_CODE
             AND A.CUST_CODE = C.CUST_CODE
 WHERE A.GUP_CODE = @p0
   AND A.CUST_CODE = @p1
";

            if (!string.IsNullOrWhiteSpace(cCode))
            {
                sql += " AND A.CCODE = @p2 ";
                parameters.Add(new SqlParameter("@p2", cCode));
            }
            if (!string.IsNullOrWhiteSpace(clsName))
            {
                sql += " AND A.CLA_NAME LIKE CONCAT('%' , @p3 , '%') ";
                parameters.Add(new SqlParameter("@p3", clsName));
            }

            var query = SqlQuery<P192019Item>(sql, parameters.ToArray()).ToList();
            return query;
        }

        public void DeleteACodeByCustCodes(string gupCode, string aCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1917
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1 ";
            var parms = new List<object> { gupCode, aCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }


        public void DeleteABCodeByCustCodes(string gupCode, string aCode, string bCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1917
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1 
                      AND BCODE = @p2 ";
            var parms = new List<object> { gupCode, aCode, bCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }

        public void DeleteByCustCodes(string gupCode, string aCode, string bCode, string cCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1917
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1
                      AND BCODE = @p2
                      AND CCODE = @p3 ";
            var parms = new List<object> { gupCode, aCode, bCode, cCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
