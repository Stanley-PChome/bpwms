using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 入庫完成結果_傳入
	/// </summary>
	public class InWarehouseReceiptReq
	{
		/// <summary>
		/// 業主編號=貨主編號
		/// </summary>
		public string OwnerCode { get; set; }
		/// <summary>
		/// 目的倉庫編號=物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 倉別編號(ex: G03)
		/// </summary>
		public string ZoneCode { get; set; }
		/// <summary>
		/// 單數
		/// </summary>
		public int? ReceiptTotal { get; set; }
		/// <summary>
		/// 入庫單列表
		/// </summary>
		public List<InWarehouseReceiptModel> ReceiptList { get; set; }
	}

	/// <summary>
	/// 入庫單物件
	/// </summary>
	public class InWarehouseReceiptModel
	{
		/// <summary>
		/// WMS單號
		/// </summary>
		public string ReceiptCode { get; set; }
		/// <summary>
		/// 入庫單狀態(0=派發失敗 4=入庫完成, 9=已取消)
		/// </summary>
		public int? Status { get; set; }
		/// <summary>
		/// 入庫開始時間
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 入庫完成時間
		/// </summary>
		public DateTime? CompleteTime { get; set; }
		/// <summary>
		/// 作業人員
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 是否異常(0=正常, 1=異常)
		/// </summary>
		public int? IsException { get; set; }
		/// <summary>
		/// 棧板編號(如果有的話，一單一板)
		/// </summary>
		public string PalletCode { get; set; }
		/// <summary>
		/// 品項數
		/// </summary>
		public int? SkuTotal { get; set; }
		/// <summary>
		/// 入庫明細
		/// </summary>
		public List<InWarehouseReceiptSkuModel> SkuList { get; set; } = new List<InWarehouseReceiptSkuModel>();
	}

	/// <summary>
	/// 入庫明細物件
	/// </summary>
	public class InWarehouseReceiptSkuModel
	{
		/// <summary>
		/// 單據項次
		/// </summary>
		public int? RowNum { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string SkuCode { get; set; }
		/// <summary>
		/// 預計入庫數量
		/// </summary>
		public int? SkuPlanQty { get; set; }
		/// <summary>
		/// 實際入庫數量
		/// </summary>
		public int? SkuQty { get; set; }
		/// <summary>
		/// 入庫標示(0=整收, 1=缺收, 2=超收)
		/// </summary>
		public int? ReceiptFlag { get; set; }
		/// <summary>
		/// 商品等級(0=殘品/客退品, 1=正品/新品) 
		/// </summary>
		public int SkuLevel { get; set; } = 1;
		/// <summary>
		/// 商品效期(yyyy/mm/dd)
		/// </summary>
		public string ExpiryDate { get; set; }
		/// <summary>
		/// 外部批次號 = WMS商品入庫日(yyyy/mm/dd)
		/// </summary>
		public string OutBatchCode { get; set; }
		/// <summary>
		/// 容器編碼
		/// </summary>
		public string ContainerCode { get; set; }
		/// <summary>
		/// 容器分隔條碼(A7)
		/// </summary>
		public string BinCode { get; set; }
		/// <summary>
		/// (入庫儲位) 保留人員上架紀錄
		/// </summary>
		public List<InWarehouseReceiptSkuShelfBinModel> ShelfBinList { get; set; } = new List<InWarehouseReceiptSkuShelfBinModel>();
	}

	/// <summary>
	/// 入庫儲位物件
	/// </summary>
	public class InWarehouseReceiptSkuShelfBinModel
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
		public int? SkuQty { get; set; }
		/// <summary>
		/// 作業人員
		/// </summary>
		public string Operator { get; set; }
	}

	public class WcsInWarehouseReceiptApiDataResult
	{
		public string ReceiptCode { get; set; }
		public string ErrorColumn { get; set; }
		public WcsApiErrorResult errors { get; set; }
	}
}
