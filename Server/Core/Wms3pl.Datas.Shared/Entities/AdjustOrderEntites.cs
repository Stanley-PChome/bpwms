using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	public enum AdjustType
	{
		/// <summary>
		/// 訂單取消
		/// </summary>
		CancelOrder = 0,
		/// <summary>
		/// 商品庫存調整
		/// </summary>
		ItemStock = 1,
		/// <summary>
		/// 盤點庫存調整
		/// </summary>
		InventoryStock = 2,
		/// <summary>
		/// 揀缺庫存調整
		/// </summary>
		LackStock = 3,
		/// <summary>
		/// 快速移轉庫存調整
		/// </summary>
		FastStockTransfer = 4,
		/// <summary>
		/// 調撥缺貨調整
		/// </summary>
		AllocationLack = 5

	}

	/// <summary>
	/// 調整單
	/// </summary>
	public class AdjustOrderParam
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
		/// 調整類別 F000904 TOPIC='F200101' AND SUTOPIC='ADJUST_TYPE'
		/// </summary>
		public AdjustType AdjustType { get; set; }
		/// <summary>
		/// 來源類別 F000902
		/// </summary>
		public string SourceType { get; set; }
		/// <summary>
		/// 來源單號
		/// </summary>
    public string SourceNo { get; set; }
		/// <summary>
		/// 作業類別 F000904 TOPIC='F200101' AND SUTOPIC='WORK_TYPE'
		/// </summary>
		public string WorkType { get; set; }

		/// <summary>
		/// 庫存調整明細
		/// </summary>
		public List<AdjustStockDetail> AdjustStockDetails { get; set; }

		/// <summary>
		/// 是否檢查序號商品需要傳入序號
		/// </summary>
		public bool CheckSerialItem { get; set; }

	}
	/// <summary>
	/// 庫存調整明細
	/// </summary>
	public class AdjustStockDetail
	{
		/// <summary>
		/// 倉庫編號
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 儲位
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public DateTime ValidDate { get; set; }
		/// <summary>
		/// 入庫日期
		/// </summary>
		public DateTime EnterDate { get; set; }
		/// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		public string BoxCtrlNo { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletCtrlNo { get; set; }

		/// <summary>
		/// 調整方式(0:調入 1:調出)
		/// </summary>
		public string WORK_TYPE { get; set; }
		/// <summary>
		/// 調整數量
		/// </summary>
		public int AdjQty { get; set; }

		/// <summary>
		/// 原因代碼 F1951 TOPIC=AI
		/// </summary>
		public string Cause { get; set; }
		/// <summary>
		/// 如果Cause =999 需輸入原因內容
		/// </summary>
		public string CasueMemo { get; set; }

		
		/// <summary>
		/// 序號清單
		/// </summary>
		public List<string> SerialNoList { get; set; }

	}
}
