
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
	public partial class F02020106Repository : RepositoryBase<F02020106, Wms3plDbContext, F02020106Repository>
	{
		/// <summary>
		/// 取得檔案上傳資訊. 包含商品圖檔和其它檔案 (F02020106裡設定的檔案)
		/// Memo: 用來判斷該驗收單的每個商品是否皆已完成上傳時要包含已上傳商品圖檔的項目, 然後才能把資訊往前拋以便更改檢驗單狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="inCludeAllItems">false: 只含未上傳圖檔的商品, true: 含所有商品</param>
		/// <returns></returns>
		public IQueryable<FileUploadData> GetFileUploadSetting(string dcCode, string gupCode, string custCode
			, string purchaseNo, string rtNo, bool inCludeAllItems = false)
		{
			string sql = @"
				SELECT 
					 ROW_NUMBER()OVER(ORDER BY A.ITEM_CODE,A.RT_NO ASC) AS ROW_NUM
					,A.*
				FROM ( 
					/* 取出該上傳的檔案類型 */
					SELECT A.UPLOAD_TYPE, A.UPLOAD_NAME
						   ,  NULL AS ITEM_CODE, NULL AS RT_NO
						   , NULL AS UPLOAD_DESC, A.ISREQUIRED
						   , (SELECT COUNT(*) 
							  FROM F02020105 B 
							  WHERE B.DC_CODE = @p0 AND B.GUP_CODE = @p1 AND B.CUST_CODE = @p2 AND B.RT_NO = @p3 AND B.PURCHASE_NO = @p4 AND B.UPLOAD_TYPE = A.UPLOAD_TYPE
							 ) AS UPLOADED_COUNT
						   , 0 AS SELECTED_COUNT
					FROM F02020106 A
					UNION ALL
					/* 取出該驗收單的所有商品(不含已上傳商品圖檔的商品), 包含商品名稱及檔案數量 */
					SELECT '00' AS UPLOAD_TYPE, '商品圖檔' AS UPLOAD_NAME
						   ,  A.ITEM_CODE, A.RT_NO
						   , A.ITEM_CODE + ' ' + (SELECT B.ITEM_NAME FROM F1903 B WHERE B.GUP_CODE = @p1 AND B.ITEM_CODE = A.ITEM_CODE AND B.CUST_CODE = @p2) AS UPLOAD_DESC
						   , '0' AS ISREQUIRED
						   , (SELECT COUNT(*) FROM F190207 C WHERE C.ITEM_CODE = A.ITEM_CODE AND C.GUP_CODE = @p1 AND C.CUST_CODE = @p2) AS UPLOADED_COUNT
						   , 0 AS SELECTED_COUNT
					FROM F020201 A
					WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND A.RT_NO = @p3 {0}
					GROUP BY A.ITEM_CODE,A.RT_NO
				) A
			";
			sql = string.Format(sql, (!inCludeAllItems ? " AND A.ISUPLOAD = '0'" : ""));
			var param = new[] {
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", rtNo),
				new SqlParameter("@p4", purchaseNo)
			};
			var result = SqlQuery<FileUploadData>(sql, param);
			return result.AsQueryable();
		}
    }
}
