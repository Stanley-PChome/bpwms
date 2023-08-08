using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class StockMovementResultsReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public List<StockMovement> Data { get; set; }
	}

	public class StockMovement
	{
		/// <summary>
		/// 調整單號(T開頭為調撥單、J開頭為調整單)
		/// </summary>
		public string ShiftWmsNo { get; set; }
		/// <summary>
		/// 調整類型(0: 調撥、1: 庫存調整)
		/// </summary>
		public string ShiftType { get; set; }
		/// <summary>
		/// 來源倉別類型
		/// </summary>
		public string SourceWhType { get; set; }
		/// <summary>
		/// 來源倉別編號
		/// </summary>
		public string SourceWhNo { get; set; }
		/// <summary>
		/// 目的倉別類型
		/// </summary>
		public string TargetWhType { get; set; }
		/// <summary>
		/// 目的倉別編號
		/// </summary>
		public string TargetWhNo { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 調整類別代碼
		/// </summary>
		public string ShiftCause { get; set; }
		/// <summary>
		/// 調整類別原因
		/// </summary>
		public string ShiftCaseMemo { get; set; }
		/// <summary>
		/// 調整時間 (調撥為上架完成時間，庫調為庫存調整完成時間)yyyy/MM/dd HH:mm:ss
		/// </summary>
		public string ShiftTime { get; set; }
		/// <summary>
		/// 調整數量
		/// </summary>
		public long ShiftQty { get; set; }
        /// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
    }

	public class StockMovementResultsRes : WcsApiErrorResult
	{
		/// <summary>
		/// 調整單號
		/// </summary>
		public string ShiftWmsNo { get; set; }
	}
}
