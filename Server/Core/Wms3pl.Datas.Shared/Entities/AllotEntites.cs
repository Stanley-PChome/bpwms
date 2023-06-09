using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;

namespace Wms3pl.Datas.Shared.Entities
{
	public class AllotedStockOrder
	{
		/// <summary>
		/// 配庫後的訂單明細
		/// </summary>
		public F050302 F050302 { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public DateTime ValidDate { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		public DateTime EnterDate { get; set; }
		/// <summary>
		/// 序號綁儲位商品序號
		/// </summary>
		public string SerialNo { get; set; }
		/// <summary>
		/// 樓層
		/// </summary>
		public string Floor { get; set; }
		/// <summary>
		/// 溫層
		/// </summary>
		public string TmprType { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		public string BoxCtrlNo { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletCtrlNo { get; set; }

		/// <summary>
		/// 倉庫編號
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 儲區
		/// </summary>
		public string AreaCode { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		///  原訂購數
		/// </summary>
		public int Orginal_OrdQty { get; set; }

		/// <summary>
		/// 倉庫揀貨樓層
		/// </summary>
		public string PickFloor { get; set; }

		/// <summary>
		/// 倉庫揀貨裝置類型
		/// </summary>
		public string DeviceType { get; set; }

		/// <summary>
		/// PK區
		/// </summary>
		public string PkArea { get; set; }

    /// <summary>
    /// PK區名稱
    /// </summary>
    public string PKAreaName { get; set; }

    /// <summary>
    /// 倉別名稱
    /// </summary>
    public string WarehouseName { get; set; }

	}

	public class ItemMakeNoTotalStockQty
	{
		public string ITEM_CODE { get; set; }
		public string MAKE_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public int QTY { get; set; }
	}

  /// <summary>
  /// 自動配庫取得符合自動配庫時間且開啟自動配庫的物流中心清單回傳內容
  /// </summary>
  public class CanAutoAllotStockDcListRes
  {
    public List<string> LogMsgs { get; set; }
    public List<string> CanAutoAllotStockDcList { get; set; }
  }

}
