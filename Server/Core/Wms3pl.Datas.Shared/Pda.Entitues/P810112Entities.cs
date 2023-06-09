using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	#region 進貨收發-收貨確認
	/// <summary>
	/// 進貨收發-收貨確認_傳入
	/// </summary>
	public class ConfirmRcvStockDataReq : StaffModel
	{
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
		/// 單據編號(進倉單號、貨主單號)
		/// </summary>
		public string StockNo { get; set; }
        /// <summary>
        /// 工作站編號
        /// </summary>
        public string WorkstationCode { get; set; }
    }

	/// <summary>
	/// 進貨收發-收貨確認_傳回
	/// </summary>
	public class ConfirmRcvStockDataRes
	{
		/// <summary>
		/// 進倉單號
		/// </summary>
		public string StockNo { get; set; }
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CustOrdNo { get; set; }
		/// <summary>
		/// 快速通關分類
		/// </summary>
		public string FastPassType { get; set; }
		/// <summary>
		/// 快速通關分類名稱
		/// </summary>
		public string FastPassTypeName { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 貨主品編
		/// </summary>
		public string CustItemCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 上架倉別編號
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 上架倉別名稱
		/// </summary>
		public string WarehouseName { get; set; }
		/// <summary>
		/// 上架倉別類型
		/// </summary>
		public string WarehouseIdType { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string ApiInfo { get; set; }
    }
	#endregion

	#region 進貨收發-詳細資訊
	/// <summary>
	/// 進貨收發-詳細資訊_傳入
	/// </summary>
	public class RcvStockDetailDataReq : StaffModel
	{
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
		/// 單據編號(進倉單號、貨主單號)
		/// </summary>
		public string StockNo { get; set; }
	}

	/// <summary>
	/// 進貨收發-詳細資訊_傳回
	/// </summary>
	public class RcvStockDetailDataRes
	{
		/// <summary>
		/// 進倉單號
		/// </summary>
		public string StockNo { get; set; }
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CustOrdNo { get; set; }
		/// <summary>
		/// 預定進倉日
		/// </summary>
		public DateTime DeliverDate { get; set; }
		/// <summary>
		/// 預定進倉時段
		/// </summary>
		public string BookingInPeriod { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		public string VnrCode { get; set; }
		/// <summary>
		/// 快速通關分類
		/// </summary>
		public string FastPassType { get; set; }
		/// <summary>
		/// 商品明細
		/// </summary>
		public List<RcvStockDetailDataDetail> Detail { get; set; }
        /// <summary>
        /// 訊息
        /// </summary>
        public string ApiInfo { get; set; }
    }

	public class RcvStockDetailDataDetail
	{
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 貨主品編
		/// </summary>
		public string CustItemCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 上架倉別編號
		/// </summary>
		public string WarehouseId { get; set; }
		/// <summary>
		/// 上架倉別名稱
		/// </summary>
		public string WarehouseName { get; set; }
	}
	#endregion
}
