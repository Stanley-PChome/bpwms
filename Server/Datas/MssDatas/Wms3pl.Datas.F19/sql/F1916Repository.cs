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
    public partial class F1916Repository : RepositoryBase<F1916, Wms3plDbContext, F1916Repository>
    {
        public List<P192019Item> GetF1916SearchData(string gupCode, string custCode, string bCode, string clsName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@p0", gupCode));
            parameters.Add(new SqlParameter("@p1", custCode));

            var sql = @"
SELECT  ROW_NUMBER ()OVER(ORDER BY A.GUP_CODE,A.CUST_CODE,A.ACODE,A.BCODE) ROWNUM,
       A.ACODE,
       B.CLA_NAME ANAME,
       A.BCODE,
       A.CLA_NAME BNAME,
       '' CCODE,
       '' CNAME,
       A.CRT_STAFF,
       A.CRT_NAME,
       A.CRT_DATE,
       A.UPD_STAFF,
       A.UPD_NAME,
       A.UPD_DATE,       
       A.GUP_CODE,
       CONVERT (char,A.CHECK_PERCENT) CHECK_PERCENT,
        A.CLA_NAME
  FROM F1916 A
       LEFT OUTER JOIN F1915 B
          ON     A.ACODE = B.ACODE
             AND A.GUP_CODE = B.GUP_CODE    
             AND A.CUST_CODE = B.CUST_CODE           
 WHERE A.GUP_CODE = @p0
   AND A.CUST_CODE = @p1
";

            if (!string.IsNullOrWhiteSpace(bCode))
            {
                sql += " AND A.BCODE = @p2 ";
                parameters.Add(new SqlParameter("@p2", bCode));
            }
            if (!string.IsNullOrWhiteSpace(clsName))
            {
                sql += " AND A.CLA_NAME LIKE CONCAT('%', @p3 , '%') ";
                parameters.Add(new SqlParameter("@p3", clsName));
            }
           
            var query = SqlQuery<P192019Item>(sql, parameters.ToArray()).ToList();
            return query;
        }

        public void DeleteACodeByCustCodes(string gupCode, string aCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1916
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1 ";
            var parms = new List<object> { gupCode, aCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }

        public void DeleteByCustCodes(string gupCode, string aCode, string bCode, List<string> custCodes)
        {
            var sql = @" DELETE 
                     FROM F1916
                    WHERE GUP_CODE = @p0
                      AND ACODE = @p1
                      AND BCODE = @p2 ";
            var parms = new List<object> { gupCode, aCode, bCode };
            sql += parms.CombineSqlInParameters(" AND CUST_CODE ", custCodes);
            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
