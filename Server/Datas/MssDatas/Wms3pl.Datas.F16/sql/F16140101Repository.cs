using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F16140101Repository : RepositoryBase<F16140101, Wms3plDbContext, F16140101Repository>
	{
		/// <summary>
		/// 取得退貨檢驗的序號刷讀明細，包含ISRETURN表示是否為退貨商品，ISPASS2表示是否可編輯通過、編輯通過是否需要選擇原因
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <returns></returns>
		public IQueryable<F16140101Data> GetSerialItems(string dcCode, string gupCode, string custCode, string returnNo)
		{
			// DISTINCT 是因為序號凍結有序號區間、盒號、批號，會造成 JOIN 出多筆的可能性
			var sql = @"  SELECT DISTINCT
								A.*,
								CASE WHEN B.ITEM_CODE IS NOT NULL THEN '1' ELSE '0' END AS ISRETURN,
								CASE
								WHEN C.STATUS = 'A1' THEN '2'                        -- 2表示不可以編輯通過
								WHEN C.STATUS = 'C1' AND E.CONTROL IS NULL THEN '1' -- 當沒有凍結且是C1時，1表示可直接打勾通過
								ELSE '0'                              -- 0 表示冷凍或其他狀態則可以編輯通過，且要選擇原因
								END
								AS ISPASS2
						FROM F16140101 A
								LEFT JOIN F161202 B
								ON     A.DC_CODE = B.DC_CODE
									AND A.GUP_CODE = B.GUP_CODE
									AND A.CUST_CODE = B.CUST_CODE
									AND A.RETURN_NO = B.RETURN_NO
									AND A.ITEM_CODE = B.ITEM_CODE
								LEFT JOIN F2501 C
								ON     A.GUP_CODE = C.GUP_CODE
									AND A.CUST_CODE = C.CUST_CODE
									AND A.ITEM_CODE = ISNULL(C.BOUNDLE_ITEM_CODE, C.ITEM_CODE)
									AND A.SERIAL_NO = C.SERIAL_NO
								LEFT JOIN F250102 D
								ON     C.GUP_CODE = D.GUP_CODE
									AND C.CUST_CODE = D.CUST_CODE
									AND (   (    C.SERIAL_NO >= D.SERIAL_NO_BEGIN
											AND C.SERIAL_NO <= D.SERIAL_NO_END
											AND LEN (D.SERIAL_NO_BEGIN) = LEN (C.SERIAL_NO)) --序號
										OR (    C.BOX_SERIAL = D.BOX_SERIAL
											AND D.BOX_SERIAL IS NOT NULL)                     --盒號
										OR (C.BATCH_NO = D.BATCH_NO AND D.BATCH_NO IS NOT NULL) --儲值卡盒號
																								)
									AND (CONVERT(DATE,dbo.GetSysDate()) BETWEEN D.FREEZE_BEGIN_DATE
															AND D.FREEZE_END_DATE)
									AND D.FREEZE_TYPE = '0'  -- 凍結中
								LEFT JOIN F25010201 E
								ON D.LOG_SEQ = E.FREEZE_LOG_SEQ AND E.CONTROL = '04' -- 管制作業 04:退貨
						WHERE     A.DC_CODE = @p0
								AND A.GUP_CODE = @p1
								AND A.CUST_CODE = @p2
								AND A.RETURN_NO = @p3
								AND A.AUDIT_STAFF = @p4
								AND A.AUDIT_NAME = @p5
					ORDER BY A.LOG_SEQ";

			return SqlQuery<F16140101Data>(sql, new object[] { dcCode, gupCode, custCode, returnNo, Current.Staff, Current.StaffName });
		}
        /// <summary>
        /// 刪除未過帳商品序號紀錄
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returnNo"></param>
        /// <param name="postingStatus"></param>
        public void DeleteByNoPostSerial(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, string postingStatus)
        {
            var sql = @" DELETE A FROM F16140101 A
                          WHERE EXISTS(
                          SELECT 1
                            FROM F16140101 B
                            LEFT JOIN F2501 C
                              ON B.GUP_CODE = C.GUP_CODE
                             AND B.CUST_CODE = C.CUST_CODE
                             AND B.SERIAL_NO = C.SERIAL_NO
                           WHERE B.DC_CODE = @p0
                             AND B.GUP_CODE = @p1
                             AND B.CUST_CODE = @p2
                             AND B.RETURN_NO = @p3
                             AND B.ITEM_CODE = @p4
                             AND B.SERIAL_NO IS NOT NULL
                             AND ((B.ISPASS ='1' AND C.STATUS <> @p5) OR B.ISPASS ='0') --刪除未過帳序號或不通過序號
                             AND B.AUDIT_STAFF = @p6
                             AND B.AUDIT_NAME = @p7
                             AND A.DC_CODE = B.DC_CODE
                             AND A.GUP_CODE = B.GUP_CODE
                             AND A.CUST_CODE = B.CUST_CODE
                             AND A.RETURN_NO = B.RETURN_NO
                             AND A.LOG_SEQ = B.LOG_SEQ
                            )
                            ";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnNo),
                new SqlParameter("@p4", itemCode),
                new SqlParameter("@p5", postingStatus),
                new SqlParameter("@p6", Current.Staff),
                new SqlParameter("@p7", Current.StaffName)
            };
            ExecuteSqlCommand(sql, parameters.ToArray());
        }
    }
}
