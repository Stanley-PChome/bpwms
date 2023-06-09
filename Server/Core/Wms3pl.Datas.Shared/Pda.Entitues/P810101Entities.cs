using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	public class GetBatchItemReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 最後同步日期  
		/// </summary>
		public DateTime LastAsyncDate { get; set; }

		/// <summary>
		/// 批次排除已取得筆數
		/// </summary>
		public int BatchSkipNum { get; set; }

		/// <summary>
		/// 取得指定品號清單
		/// </summary>
		public string[] ItemNoList { get; set; }

	}

	public class GetBatchItemRes
	{
		/// <summary>
		/// 批次累積筆數
		/// </summary>
		public int BatchTotal { get; set; }
		/// <summary>
		/// 本批次資料最後異動日期yyyy/MM/dd HH:mm:ss
		/// </summary>
		public DateTime BatchLastDate { get; set; }
		/// <summary>
		/// 是否為最後批次
		/// </summary>
		public bool IsLastBatch { get; set; }

		/// <summary>
		/// 商品資料
		/// </summary>
		public List<ItemList> ItemList { get; set; }
	}

	public class ItemList
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 單位
		/// </summary>
		public string Unit { get; set; }
		/// <summary>
		/// 尺寸
		/// </summary>
		public string ItemSize { get; set; }
		/// <summary>
		/// 箱入數
		/// </summary>
		public int BoxQty { get; set; }
		/// <summary>
		/// 顏色
		/// </summary>
		public string ItemColor { get; set; }
		/// <summary>
		/// 規格
		/// </summary>
		public string ItemSpec { get; set; }
		/// <summary>
		/// 長
		/// </summary>
		public int ItemLength { get; set; }
		/// <summary>
		/// 寬
		/// </summary>
		public int ItemWidth { get; set; }
		/// <summary>
		/// 高
		/// </summary>
		public int ItemHeight { get; set; }
		/// <summary>
		/// 重量
		/// </summary>
		public double ItemWeight { get; set; }
		/// <summary>
		/// 序號類型(0:無;1:序號商品;2:序號綁儲位商品)
		/// </summary>
		public string SnType { get; set; }
		/// <summary>
		/// 條碼1
		/// </summary>
		public string Barcode1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		public string Barcode2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		public string Barcode3 { get; set; }

	}

	#region 產品主檔同步SqlModel
	public class P1903Data
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 單位
		/// </summary>
		public string Unit { get; set; }
		/// <summary>
		/// 尺寸
		/// </summary>
		public string ItemSize { get; set; }
		/// <summary>
		/// 箱入數
		/// </summary>
		public int BoxQty { get; set; }
		/// <summary>
		/// 顏色
		/// </summary>
		public string ItemColor { get; set; }
		/// <summary>
		/// 規格
		/// </summary>
		public string ItemSpec { get; set; }
		/// <summary>
		/// 長
		/// </summary>
		public int ItemLength { get; set; }
		/// <summary>
		/// 寬
		/// </summary>
		public int ItemWidth { get; set; }
		/// <summary>
		/// 高
		/// </summary>
		public int ItemHeight { get; set; }
		/// <summary>
		/// 重量
		/// </summary>
		public double ItemWeight { get; set; }
		/// <summary>
		/// 序號類型(0:無;1:序號商品;2:序號綁儲位商品)
		/// </summary>
		public string SnType { get; set; }
		/// <summary>
		/// 條碼1
		/// </summary>
		public string Barcode1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		public string Barcode2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		public string Barcode3 { get; set; }

		public DateTime? UpdDate { get; set; }
		public DateTime CrtDate { get; set; }
	}
	#endregion


	public class GetBatchItemSerialReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 最後同步日期
		/// </summary>
		public DateTime LastAsyncDate { get; set; }

		/// <summary>
		/// 批次排除已取得比數
		/// </summary>
		public int BatchSkipNum { get; set; }

		/// <summary>
		/// 取得指定序號清單    
		/// </summary>
		public string[] SnList { get; set; }
	}


	public class GetBatchItemSerialRes
	{
		/// <summary>
		/// 批次累積筆數
		/// </summary>
		public int BatchTotal { get; set; }
		/// <summary>
		/// 本批次資料最後異動日期yyyy/MM/dd HH:mm:ss
		/// </summary>
		public DateTime BatchLastDate { get; set; }
		/// <summary>
		/// 是否為最後批次
		/// </summary>
		public bool IsLastBatch { get; set; }

		/// <summary>
		/// 商品資料
		/// </summary>
		public List<ItemSnList> ItemSnList { get; set; }
	}

	public class ItemSnList
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }

		/// <summary>
		/// 商品序號
		/// </summary>
		public string Sn { get; set; }

		/// <summary>
		/// 效期(yyyy/MM/dd)
		/// </summary>
		public DateTime? ValidDate { get; set; }

		/// <summary>
		/// 狀態
		/// </summary>
		public string Status { get; set; }

	}

	#region 產品序號主檔同步SqlModel
	public class P2501Data
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }

		/// <summary>
		/// 商品序號    
		/// </summary>
		public string Sn { get; set; }

		/// <summary>
		/// 效期(yyyy/MM/dd)
		/// </summary>
		public DateTime? ValidDate { get; set; }

		/// <summary>
		/// 狀態
		/// </summary>
		public string Status { get; set; }

		public DateTime? UpdDate { get; set; }
		public DateTime CrtDate { get; set; }
	}
	#endregion

	#region 依據商品搜尋條件找出品號Model
	public class GetItemByConditionReq : StaffModel
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 商品搜尋條件  
		/// </summary>
		public string Condition { get; set; }
	}

	public class GetItemByConditionRes
	{
		/// <summary>
		/// 0 : 代表Condition找不到序號、商品
		/// 1 : 代表Condition為一般商品的品號、國條1、國條2、國條3
		/// 2 : 代表Condition為序號
		/// </summary>
		public string ConditionType { get; set; } = "0";
		/// <summary>
		/// ConditionType=2才有資料，並且該欄位為F2501.STATUS
		/// </summary>
		public string SnStatus { get; set; }
		/// <summary>
		/// 根據Condition找出的商品資料
		/// </summary>
		public List<GetItemByConditionItem> ItemList { get; set; } = new List<GetItemByConditionItem>();
	}

	public class GetItemByConditionItem
	{
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 商品類型(0:一般商品 1:序號商品 2:序號綁儲位)
		/// </summary>
		public string SnType { get; set; }
	}
	#endregion
}