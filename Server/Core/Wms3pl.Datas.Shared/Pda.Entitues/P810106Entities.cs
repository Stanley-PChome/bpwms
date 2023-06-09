using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	public class PickBaseReq : StaffModel
	{
        /// <summary>
        /// 是否同步
        /// </summary>
        public string IsSync { get; set; }
        /// <summary>
        /// 功能編號
        /// </summary>
        public string FuncNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		public string WmsNo { get; set; }
	}
	/// <summary>
	/// 揀貨單據查詢_傳入
	/// </summary>
	public class GetPickReq : PickBaseReq
	{
		/// <summary>
		/// 作業模式(01:批量揀貨;02:單一揀貨)
		/// </summary>
		public string Mode { get; set; }

		/// <summary>
		/// 彙總/揀貨/出貨日期
		/// </summary>
		public DateTime? ShipDate { get; set; }
	}

	/// <summary>
	/// 揀貨單據查詢_回傳
	/// </summary>
	public class GetPickRes
	{
		/// <summary>
		/// 類別代碼
		/// </summary>
		public string OTNo { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 彙總/揀貨/出貨單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 彙總/揀貨/出貨日期
		/// </summary>
		public DateTime WmsDate { get; set; }
		/// <summary>
		/// 儲區名稱
		/// </summary>
		public string AreaName { get; set; }
		/// <summary>
		/// 品項總數
		/// </summary>
		public int ItemCnt { get; set; }
		/// <summary>
		/// 商品總數
		/// </summary>
		public int ItemQty { get; set; }
		/// <summary>
		/// 已揀品項數
		/// </summary>
		public int PickItemCnt { get; set; }
		/// <summary>
		/// 狀態代碼
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 狀態名稱
		/// </summary>
		public string StatusName { get; set; }
		/// <summary>
		/// 作業人員編號
		/// </summary>
		public string AccNo { get; set; }
		/// <summary>
		/// 作業人員名稱
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 跨庫目的地
		/// </summary>
		public string MoveOutTarget { get; set; }
	}

	/// <summary>
	/// 揀貨明細查詢_傳入
	/// </summary>
	public class GetPickDetailReq : PickBaseReq
	{
    }

	/// <summary>
	/// 揀貨明細查詢_回傳
	/// </summary>
	public class GetPickDetailRes
	{
		/// <summary>
		/// 揀貨明細資料集
		/// </summary>
		public List<GetPickDetailDetail> DetailList { get; set; }
		/// <summary>
		/// 商品序號資料集
		/// </summary>
		public List<GetAllocItemSn> ItemSnList { get; set; }
	}

	/// <summary>
	/// 揀貨明細資料
	/// </summary>
	public class GetPickDetailDetail
	{
		/// <summary>
		/// 彙總/揀貨/出貨單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 明細序號
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		public string WHName { get; set; }
		/// <summary>
		/// 儲位
		/// </summary>
		public string Loc { get; set; }
		/// <summary>
		/// 效期(yyyy/MM/dd)
		/// </summary>
		public DateTime? ValidDate { get; set; }
		/// <summary>
		/// 入庫日(yyyy/MM/dd)
		/// </summary>
		public DateTime? EnterDate { get; set; }
		/// <summary>
		/// 揀貨數量
		/// </summary>
		public int ShipQty { get; set; }
		/// <summary>
		/// 順序
		/// </summary>
		public int Route { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		public string PickNo { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		public string Sn { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MkNo { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		public string PackRef { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletNo { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		public DateTime CrtDate { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string ProductSize { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        public string ProductColor { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string ProductSpec { get; set; }
        /// <summary>
        /// 條碼一
        /// </summary>
        public string Barcode1 { get; set; }
        /// <summary>
        /// 條碼二
        /// </summary>
        public string Barcode2 { get; set; }
        /// <summary>
        /// 條碼三
        /// </summary>
        public string Barcode3 { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }
        /// <summary>
        /// 箱入數（商品預設值）
        /// </summary>
        public int? BoxQty { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustNo { get; set; }
    }

	/// <summary>
	/// 揀貨單據檢核_傳入
	/// </summary>
	public class PostPickCheckReq : PickBaseReq
	{

	}

	/// <summary>
	/// 揀貨單據更新_傳入
	/// </summary>
	public class PostPickUpdateReq : PickBaseReq
	{

	}

	/// <summary>
	/// 揀貨完成確認_傳入
	/// </summary>
	public class PostPickConfirmReq : PickBaseReq
	{
		/// <summary>
		/// 明細序號
		/// </summary>
		public string WmsSeq { get; set; }
		/// <summary>
		/// 實際揀貨數量
		/// </summary>
		public int ActQty { get; set; }
	}
}
