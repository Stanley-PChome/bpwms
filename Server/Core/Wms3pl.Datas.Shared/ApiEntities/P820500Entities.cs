using System;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// WcsApi排程_傳入
	/// </summary>
	public class WcsImportReq
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
		/// </summary>
		public string ScheduleNo { get; set; }
		/// <summary>
		/// 庫存比對日期
		/// </summary>
		public DateTime? StockCompareDate { get; set; }

		/// <summary>
		/// 外部包裝資料處理排程用-處理狀態
		/// </summary>
		public string ProcFlag { get; set; }
	}
}
