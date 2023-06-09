using System;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F020103Repository : RepositoryBase<F020103, Wms3plDbContext, F020103Repository>
    {
        public ExecuteResult Delete(string date, int serialNo, string purchaseNo, string dcCode, string gupCode, string custCode)
        {
            ExecuteResult result = new ExecuteResult() { IsSuccessed = false };
            string sql = @"
	                        DELETE F020103 WHERE convert(char(10),ARRIVE_DATE,111) = convert(char(10),@p0,111)
		                        AND SERIAL_NO = @p1 AND PURCHASE_NO = @p2 AND DC_CODE = @p3
		                        AND GUP_CODE = @p4 AND CUST_CODE = @p5
			";
            var param = new[] {
                new SqlParameter("@p0", date),
                new SqlParameter("@p1", serialNo),
                new SqlParameter("@p2", purchaseNo),
                new SqlParameter("@p3", dcCode),
                new SqlParameter("@p4", gupCode),
                new SqlParameter("@p5", custCode)
            };
            ExecuteSqlCommand(sql, param);
            result.IsSuccessed = true;
            return result;
        }

        /// <summary>
		/// 取得進倉預排清單
		/// </summary>
		/// <param name="arriveDate"></param>
		/// <param name="time"></param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		public IQueryable<F020103Detail> GetF020103Detail(DateTime arriveDate, string time
            , string dcCode, string vendorCode, string custCode, string gupCode)
        {
            var param = new[] {
                new SqlParameter("@p0",  Current.Lang),
                new SqlParameter("@p1", arriveDate),
                new SqlParameter("@p2", time??(object)DBNull.Value),
                new SqlParameter("@p3", dcCode),
                new SqlParameter("@p4", gupCode),
                new SqlParameter("@p5", custCode),
                new SqlParameter("@p6", vendorCode??(object)DBNull.Value)
            };
            string sql = $@" 
                        SELECT DISTINCT B.VNR_NAME,
									D.CUST_NAME,
									C.NAME AS ARRIVE_TIME_DESC,
									CASE
									WHEN INTIME IS NOT NULL AND OUTTIME IS NOT NULL
									THEN
                                        ROUND(DATEDIFF(DAY, CONVERT(smalldatetime,'1900/01/01 '+SUBSTRING(OUTTIME,0,3)+':'+SUBSTRING(INTIME,3,2)+':00'), CONVERT(smalldatetime,'1900/01/01 '+SUBSTRING(INTIME,0,3)+':'+SUBSTRING(OUTTIME,3,2)+':00'))*24*60,0)
									END
									AS TOTALTIME,
									A.*
							FROM F020103 A
									LEFT JOIN F1908 B
									ON A.VNR_CODE = B.VNR_CODE AND A.GUP_CODE = B.GUP_CODE
									LEFT JOIN VW_F000904_LANG C
									ON     A.ARRIVE_TIME = C.VALUE
										AND TOPIC = 'F020103'
										AND SUBTOPIC = 'ARRIVE_TIME'
										AND LANG = @p0
									LEFT JOIN F1909 D
									ON A.GUP_CODE = D.GUP_CODE AND A.CUST_CODE = D.CUST_CODE
							WHERE     A.ARRIVE_DATE = @p1 
                                     AND A.ARRIVE_TIME = case when @p2 ='' or @p2 is null then A.ARRIVE_TIME else @p2 end
									AND A.DC_CODE = @p3
									AND A.GUP_CODE = @p4
									AND A.CUST_CODE = @p5
                                    AND A.VNR_CODE = CASE WHEN @p6= '' or @p6 is null then  A.VNR_CODE else @p6 end
						ORDER BY A.PURCHASE_NO, A.SERIAL_NO";

            return SqlQuery<F020103Detail>(sql, param.ToArray());
        }
    }
}
