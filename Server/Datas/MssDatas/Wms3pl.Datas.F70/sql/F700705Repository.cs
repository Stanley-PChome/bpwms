using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F70
{
	public partial class F700705Repository : RepositoryBase<F700705, Wms3plDbContext, F700705Repository>
	{

        /// <summary>
        /// 從缺貨主檔取得人員錯誤狀況
        /// </summary>
        /// <param name="crtDate"></param>
        /// <returns></returns>
        public IQueryable<F700705ForSchedule> GetEmpLackPicks(DateTime beginCrtDate, DateTime endCrtDate)
        {
            var sql = @"  
                    SELECT A.DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                CONVERT (varchar,A.CRT_DATE,120) AS CNT_DATE,
                                C.GRP_ID,
                                B.EMP_ID,
                                B.EMP_NAME,
                                (DATEPART (DAY,A.CRT_DATE) - 1) AS CNT_DAY,
                                ISNULL (SUM (A.LACK_QTY), 0) AS ERROR_QTY
                        FROM F051206 A
                                JOIN F1924 B ON A.CRT_STAFF = B.EMP_ID AND A.CRT_NAME = B.EMP_NAME
                                JOIN F192401 C ON B.EMP_ID = C.EMP_ID
                        WHERE A.CRT_DATE BETWEEN @p0 AND @p1
                    GROUP BY A.DC_CODE,
                                A.GUP_CODE,
                                A.CUST_CODE,
                                CONVERT (varchar,A.CRT_DATE,120),
                                C.GRP_ID,
                                B.EMP_ID,
                                B.EMP_NAME,
			                    (DATEPART (DAY,A.CRT_DATE) - 1)
                ";

            return SqlQuery<F700705ForSchedule>(sql, new object[] { beginCrtDate, endCrtDate });
        }
    }
}
