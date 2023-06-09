using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 出庫結果回報(按箱)
	/// </summary>
	public class OutWarehouseContainerReceiptReq
	{
		/// <summary>
		///業主編號=WMS貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 儲區編號
		/// </summary>
		public string ZoneCode { get; set; }
		/// <summary>
		/// 箱列表
		/// </summary>
		public List<OutWarehouseContainerReceiptContainerModel> ContainerList { get; set; } = new List<OutWarehouseContainerReceiptContainerModel>();
	}

	/// <summary>
	/// (裝箱資訊)
	/// </summary>
	public class OutWarehouseContainerReceiptContainerModel
	{
		/// <summary>
		/// 容器編號
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 工作站人員
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 工作站編號
		/// </summary>
		public string WorkStationNo { get; set; }
		/// <summary>
		/// 播種牆格口編號
		/// </summary>
		public string SeedBinCode { get; set; }
		/// <summary>
		/// 明細數
		/// </summary>
		public int? SkuTotal { get; set; }
		/// <summary>
		/// (裝箱明細)
		/// </summary>
		public List<OutWarehouseContainerReceiptSkuModel> SkuList { get; set; } = new List<OutWarehouseContainerReceiptSkuModel>();
		/// <summary>
		/// (下架儲位)保留人員揀貨記錄
		/// </summary>
		public List<OutWarehouseContainerReceiptShelfBinModel> ShelfBinList { get; set; } = new List<OutWarehouseContainerReceiptShelfBinModel>();
	}

	public class OutWarehouseContainerReceiptSkuModel
	{
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string OrderCode { get; set; }
		/// <summary>
		/// 明細項次
		/// </summary>
		public int RowNum { get; set; }
		/// <summary>
		/// 庫內品號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 裝箱數量
		/// </summary>
		public int SkuQty { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品) 
		/// </summary>
		public int SkuLevel { get; set; }
		/// <summary>
		/// 商品效期
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 外部批號=商品入庫日(yyMMdd)+序號3碼數字) 或喬巴15碼批號
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 外部批號=商品入庫日(yyMMdd)+序號3碼數字) 或喬巴15碼批號
		/// </summary>
		public List<string> SerialNumList { get; set; }
		/// <summary>
		/// 容器分隔編號
		/// </summary>
		public string BinCode { get; set; }
		/// <summary>
		/// 容器分隔編號
		/// </summary>
		public string CompleteTime { get; set; }
		/// <summary>
		/// 是否出庫單最後一箱(0=否, 1=是)
		/// </summary>
		public int? IsLastContainer { get; set; }
		/// <summary>
		/// 出庫單總箱數
		/// </summary>
		public int? ContainerTotal { get; set; }
	}

	public class OutWarehouseContainerReceiptShelfBinModel
	{
		/// <summary>
		/// 貨架編號
		/// </summary>
		public string ShelfCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string BinCode { get; set; }
		/// <summary>
		/// 商品數量
		/// </summary>
		public int SkuQty { get; set; }
	}

	public class WcsApiOutWarehouseContainerReceiptResultData
	{
		public string ContainerCode { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}


	public class WcsApiOutWarehouseContainerReceiptItemSerialModel
	{
		public string ItemCode { get; set; }
		public List<string> SerialNos { get; set; }
	}
}