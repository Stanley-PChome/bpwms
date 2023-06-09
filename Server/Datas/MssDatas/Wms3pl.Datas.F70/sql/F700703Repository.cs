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
	public partial class F700703Repository : RepositoryBase<F700703, Wms3plDbContext, F700703Repository>
	{
		/// <summary>
		/// 從出貨包裝頭檔取得包材耗用狀況報表資料
		/// </summary>
		/// <param name="crtDate"></param>
		/// <returns></returns>
		public IQueryable<F700703ForSchedule> GetBoxNumUsedStatus(DateTime beginCrtDate, DateTime endCrtDate)
		{
			var sql = @" SELECT A.DC_CODE,
			                    A.GUP_CODE,
			                    A.CUST_CODE,
			                    A.BOX_NUM AS ITEM_CODE,
			                    A.CRT_DATE AS CNT_DATE,
			                    LTRIM((DATEPART(DAY,A.CRT_DATE) - 1)) AS CNT_DAY,
			                    COUNT (A.BOX_NUM) AS QTY                              -- 取得包材耗用狀況報表資料
	                                FROM F055001 A
	                                WHERE A.CRT_DATE BETWEEN @p0 AND @p1 AND A.BOX_NUM IS NOT NULL
                                GROUP BY A.DC_CODE,
			                                A.GUP_CODE,
			                                A.CUST_CODE,
			                                A.BOX_NUM,
			                                A.CRT_DATE,
			                                LTRIM((DATEPART(DAY,A.CRT_DATE) - 1)) ";
			return SqlQuery<F700703ForSchedule>(sql, new object[] { beginCrtDate, endCrtDate });
		}
	}
}