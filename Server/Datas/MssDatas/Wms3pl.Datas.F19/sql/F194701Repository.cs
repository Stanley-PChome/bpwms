using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194701Repository : RepositoryBase<F194701, Wms3plDbContext, F194701Repository>
    {
        public IQueryable<DelvTimeArea> GetDelvTimeAreas(string dcCode, string allID, bool canFast, string delvTmpr, string minDelvTime, string maxDelvTime = null)
        {
            var parameters = new List<object>
            {
                allID,
                dcCode,
                delvTmpr,
                minDelvTime
            };

            var sql = @"SELECT A.ALL_ID,
							   A.DC_CODE,
							   A.DELV_TMPR,
							   A.DELV_TIME,
							   A.DELV_FREQ,
							   A.DELV_EFFIC,
							   B.ZIP_CODE,
							   A.DELV_TIMES,
							   C.SORT,
                                ROW_NUMBER ()OVER(ORDER BY A.ALL_ID,A.DELV_TIME,A.DC_CODE,A.DELV_EFFIC,A.DELV_TMPR,A.PAST_TYPE) ROWNUM
						  FROM F194701 A
							   JOIN F19470101 B
								  ON     A.ALL_ID = B.ALL_ID
									 AND A.DC_CODE = B.DC_CODE
									 AND A.DELV_TIME = B.DELV_TIME
									 AND A.DELV_TMPR = B.DELV_TMPR
									 AND A.DELV_EFFIC = B.DELV_EFFIC
									 AND A.PAST_TYPE = B.PAST_TYPE
							   LEFT JOIN F190102 C
								  ON A.DC_CODE = C.DC_CODE AND A.DELV_EFFIC = C.DELV_EFFIC
                 JOIN F1947 D
                   ON D.DC_CODE = A.DC_CODE
                  AND D.ALL_ID = A.ALL_ID
						 WHERE     A.ALL_ID = @p0
							   AND A.DC_CODE = @p1
							   AND A.DELV_TMPR = @p2
							   AND A.DELV_TIME >= @p3
							   AND A.PAST_TYPE = '0'
                 AND D.TYPE ='0' "; //只取得一般配送商
            if (canFast)
            {
                sql += @"
				   And A.DELV_EFFIC<>'01'";
            }

            if (!string.IsNullOrEmpty(maxDelvTime))
            {
                sql += @"
				   And A.DELV_TIME<=@p4";
                parameters.Add(maxDelvTime);
            }
            var result = SqlQuery<DelvTimeArea>(sql, parameters.ToArray()).ToList();
            return result.AsQueryable();
        }

        public IQueryable<NewF194701WithF1934> GetNewF194701WithF1934s(string dcCode, string allID)
        {
            var parameters = new object[]
            {
                dcCode,
                allID
            };

            var sql = $@"select b.DELV_TIME
                              ,b.DELV_EFFIC
                              ,b.DELV_TMPR  
                              ,(select  NAME from VW_F000904_LANG where TOPIC='F194701' and SUBTOPIC='DELV_TMPR' and VALUE=b.DELV_TMPR and LANG = '{Current.Lang}') DELV_TMPR_NAME          
                              ,(select  NAME from VW_F000904_LANG where TOPIC='F190102' and SUBTOPIC='DELV_EFFIC' and VALUE=b.DELV_EFFIC and LANG = '{Current.Lang}') DELV_EFFIC_NAME      
                        from F1947 a join F194701 b
                        on a.ALL_ID=b.ALL_ID 
                        and a.DC_CODE=b.DC_CODE 
                        where  a.DC_CODE=@p0
                        and a.ALL_ID=@p1";
            var result = SqlQuery<NewF194701WithF1934>(sql, parameters);
            return result;
        }

        /// <summary>
		/// 取得派車管理可出車的出車時段
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="allId"></param>
		/// <param name="delvEffic"></param>
		/// <param name="delvTmpr"></param>
		/// <param name="zipCode"></param>
		/// <param name="takeDate">取件日期</param>
		/// <returns></returns>
		public IQueryable<F194701> GetF194701sByP700104(string dcCode, string allId, string delvEffic, string delvTmpr, string zipCode, DateTime takeDate, string distrUse, int? intPastMax, int? intFastMax = null, string takeTime = null)
        {
            var sql = @"SELECT A.*
						  FROM F194701 A
							   JOIN F19470101 B
								  ON     A.ALL_ID = B.ALL_ID                                    -- 配送商
									 AND A.DC_CODE = B.DC_CODE
									 AND A.DELV_EFFIC = B.DELV_EFFIC                           -- 配送效率
									 AND A.DELV_TIME = B.DELV_TIME                             -- 出車時段
									 AND A.DELV_TMPR = B.DELV_TMPR                             -- 配送溫層
                   AND A.PAST_TYPE = B.PAST_TYPE
						 WHERE     A.ALL_ID = @p0
							   AND A.DC_CODE = @p1
							   AND A.DELV_EFFIC = @p2
							   AND A.DELV_TMPR = @p3
							   AND B.ZIP_CODE = @p4
							   AND A.DELV_FREQ LIKE CONCAT('%' , @p5 , '%')          -- 出車頻率(0~6:星期日~六,逗號分隔)
							   AND A.PAST_TYPE = @p6 
								";

            var dayOfWeek = Convert.ToString((int)takeDate.DayOfWeek);
            var paramList = new List<object>
            {
                allId,
                dcCode,
                delvEffic,
                delvTmpr,
                zipCode,
                dayOfWeek,
				//	取件:取件出車時段, 1:逆物流
				//	送件,來回件:送件出車時段, 0: 正物流
				(distrUse=="02") ? "1" : "0"
            };

            if (takeDate.Date <= DateTime.Today)
            {
                // 若取件日期是今天，則只能顯示超過intPastMax小時的出車時段
                if (intPastMax.HasValue)
                    sql += paramList.Combine(" AND A.DELV_TIME >= @p{0} ", DateTime.Now.AddMinutes(intPastMax.Value).ToString("HH:mm"));

                // 若是快速到貨，且有設定最慢出車分鐘數，則會變成限制今日出車時段是在一個範圍內
                bool canFast = delvEffic != "01";
                if (canFast && intFastMax.HasValue)
                    sql += paramList.Combine(" AND A.DELV_TIME <= @p{0} ", DateTime.Now.AddMinutes(intFastMax.Value).ToString("HH:mm"));
            }

            // 有填出車時段的話，就過濾(派車管理通常不會填，批次改派就會)
            sql += paramList.CombineNotNullOrEmpty(" AND A.DELV_TIME = @p{0}", takeTime);

            sql += " ORDER BY A.DELV_TIME ";

            var result = SqlQuery<F194701>(sql, paramList.ToArray());
            return result;
        }
    }
}
