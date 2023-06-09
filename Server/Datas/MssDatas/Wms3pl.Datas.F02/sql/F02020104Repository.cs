using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
namespace Wms3pl.Datas.F02
{
	public partial class F02020104Repository : RepositoryBase<F02020104, Wms3plDbContext, F02020104Repository>
	{
		/// <summary>
		/// 更新進倉商品序號刷讀紀錄檔的通過為不通過，主要從進倉序號檔的檔名作為參數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="fileName"></param>
		public void UpdateIsPass0ByF020302(string dcCode, string gupCode, string custCode, string fileName)
		{
			var sql = @"UPDATE F02020104 A
						   SET UPD_STAFF = @p0,
							   UPD_NAME = @p1,
							   UPD_DATE = dbo.GetSysDate(),
							   A.ISPASS = '0'
						 WHERE     A.ISPASS = '1'
							   AND EXISTS
									  (SELECT 1
										 FROM F020302 B
											  JOIN F010201 C
												 ON     B.DC_CODE = C.DC_CODE
													AND B.GUP_CODE = C.GUP_CODE
													AND B.CUST_CODE = C.CUST_CODE
													AND (B.PO_NO = C.STOCK_NO OR B.PO_NO = C.SHOP_NO) -- PO_NO 可能為採購單號或進倉單號，由進倉單有無填採購單號決定
										WHERE     A.DC_CODE = C.DC_CODE
											  AND A.GUP_CODE = C.GUP_CODE
											  AND A.CUST_CODE = C.CUST_CODE
											  AND A.PURCHASE_NO = C.STOCK_NO
											  AND B.DC_CODE = @p2
											  AND B.GUP_CODE = @p3
											  AND B.CUST_CODE = @p4
											  AND B.FILE_NAME = @p5)";
			this.ExecuteSqlCommand(sql, new object[] { Current.Staff, Current.StaffName, dcCode, gupCode, custCode, fileName });
		}

		public void Delete(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var sql = @" DELETE FROM F02020104
                   WHERE DC_CODE =@p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE =@p2
                     AND PURCHASE_NO = @p3
                     AND RT_NO = @p4 ";
			ExecuteSqlCommand(sql, new object[] { dcCode, gupCode, custCode, purchaseNo, rtNo });
		}

        /// <summary>
        /// 刪除進倉商品序號刷讀紀錄檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="custCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="purchaseSeq"></param>
        /// <param name="rtNo"></param>
        public void DeleteF02020104(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string rtNo)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo };

			var sql = $@" DELETE F02020104
                         WHERE DC_CODE = @p0 
                         AND GUP_CODE = @p1 
                         AND CUST_CODE = @p2 
                         AND PURCHASE_NO = @p3 
                         AND PURCHASE_SEQ = @p4
                         AND RT_NO = @p5 ";
			ExecuteSqlCommand(sql, parameter.ToArray());
		}

        public IQueryable<F02020104> GetDatasByBarCode(string dcCode, string gupCode, string custCode, string rtNo, string itemCode, string inputItemCode)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, rtNo, itemCode, inputItemCode };
            var sql = @" SELECT *
                     FROM F02020104 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND RT_NO = @p3
                      AND ITEM_CODE = @p4
                      AND SERIAL_NO = @p5 ";
            return SqlQuery<F02020104>(sql, parms.ToArray());
        }
    }
}
