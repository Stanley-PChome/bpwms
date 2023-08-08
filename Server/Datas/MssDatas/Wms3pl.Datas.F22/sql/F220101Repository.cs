using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F22
{
	public partial class F220101Repository : RepositoryBase<F220101, Wms3plDbContext, F220101Repository>
	{

        public IQueryable<string> GetReturnBinCodeData(string dcCode, string gupCode, string custCode, string docNo, string shelfNo, string shelfDir)
        {
            var parms = new List<SqlParameter> {
                new SqlParameter("@p0",dcCode),
                new SqlParameter("@p1",gupCode),
                new SqlParameter("@p2",custCode),
                new SqlParameter("@p3",docNo),
                new SqlParameter("@p4",shelfNo),
                new SqlParameter("@p5",shelfDir)
            };
            string sql = @"
						SELECT 
	DISTINCT BIN_CODE 
	FROM (
							SELECT TOP 100 PERCENT UPATE, BIN_CODE FROM (
							  SELECT MAX (B.UPD_DATE) UPATE, B.BIN_CODE
								FROM F2201 A
									 INNER JOIN F220101 B
										ON     A.REQ_CODE = B.REQ_CODE
										   AND A.DC_CODE = B.DC_CODE
										   AND A.GUP_CODE = B.GUP_CODE
										   AND A.CUST_CODE = B.CUST_CODE
									 INNER JOIN F191203 C
										ON C.TPS_LOC_CODE = B.BIN_CODE AND C.DC_CODE = A.DC_CODE
							   WHERE     A.DC_CODE = @p0
									 AND A.GUP_CODE = @p1
									 AND A.CUST_CODE = @p2
									 AND A.DOC_NO = @p3
									 AND C.SHELF_NO = @p4
									 AND C.SHELF_DIR = @p5
									 AND B.UPD_NAME IS NOT NULL
							GROUP BY B.BIN_CODE
							UNION ALL
							SELECT B.UPD_DATE, B.BIN_CODE
							  FROM F2201 A
								   INNER JOIN F220101 B ON A.REQ_CODE = B.REQ_CODE
								   INNER JOIN F191203 C
									  ON     C.TPS_LOC_CODE = B.BIN_CODE
										 AND C.DC_CODE = A.DC_CODE
										 AND A.DC_CODE = B.DC_CODE
										 AND A.GUP_CODE = B.GUP_CODE
										 AND A.CUST_CODE = B.CUST_CODE
							 WHERE     A.DC_CODE = @p0
								   AND A.GUP_CODE = @p1
								   AND A.CUST_CODE = @p2
								   AND A.DOC_NO = @p3
								   AND C.SHELF_NO = @p4
								   AND C.SHELF_DIR = @p5)A
								   ORDER BY UPATE DESC
								   )G
";
            var result = SqlQuery<string>(sql, parms.ToArray()).AsQueryable();
            return result;
        }

    }
}
