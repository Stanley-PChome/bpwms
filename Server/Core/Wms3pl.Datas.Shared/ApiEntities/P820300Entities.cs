using System;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 資料轉出排程_傳入
	/// </summary>
	public class ExportResultReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 參數選擇執行哪一個排程回檔 
		/// 01 ->進倉回檔
		/// 02 ->訂單回檔
		/// 03 ->廠退回檔
		/// null or Empty ->全部都執行
		/// </summary>
		public string ScheduleNo { get; set; }
	}

	public class ExportDataPickModel
	{
		public string ORD_NO { get; set; }
		public string ORD_SEQ { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string MAKE_NO { get; set; }
		public int QTY { get; set; }
        public DateTime? VALID_DATE { get; set; }

    }
}
