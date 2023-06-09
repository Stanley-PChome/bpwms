using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	/// <summary>
	/// 調撥單據查詢_傳入
	/// </summary>
	public class GetAllocReq : StaffModel
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
		/// 單據類別 01:進倉上架   02:調撥下架   03:調撥上架 
		/// </summary>
		public string AllocType { get; set; }
		/// <summary>
		/// 調撥日期
		/// </summary>
		public DateTime? AllocDate { get; set; }
		/// <summary>
		/// 調撥單號/驗收單號
		/// </summary>
		public string WmsNo { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletNo { get; set; }
		/// <summary>
		/// 序號
		/// </summary>
		public string SerialNo { get; set; }
		/// <summary>
		/// 容器
		/// </summary>
		public string ContainerCode { get; set; }
	}

	/// <summary>
	/// 調撥單據查詢_傳回
	/// </summary>
	public class GetAllocRes : StaffModel
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }
		/// <summary>
		/// 調撥日期
		/// </summary>
		public DateTime AllocDate { get; set; }
		/// <summary>
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 單據類別
		/// </summary>
		public string AllocType { get; set; }
		/// <summary>
		/// 單據號碼(驗收單號)
		/// </summary>
		public string SrcNo { get; set; }
		/// <summary>
		/// 來源倉別
		/// </summary>
		public string SrcWhName { get; set; }
		/// <summary>
		/// 目的倉別
		/// </summary>
		public string TarWhName { get; set; }
		/// <summary>
		/// 原始品項總數
		/// </summary>
		public int ItemCnt { get; set; }
		/// <summary>
		/// 原始商品總數
		/// </summary>
		public int ItemQty { get; set; }
		/// <summary>
		/// 已完成品項數
		/// </summary>
		public int ActItemCnt { get; set; }
		/// <summary>
		/// 狀態代碼
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 狀態名稱
		/// </summary>
		public string StatusName { get; set; }
		/// <summary>
		/// 作業人員名稱
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 物流箱號
		/// </summary>
		public string BoxNo { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		public string Memo { get; set; }
	}

	/// <summary>
	/// 調撥單據檢核_傳入
	/// </summary>
	public class PostAllocCheckReq : StaffModel
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
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 單據類別
		/// </summary>
		public string AllocType { get; set; }
	}

	/// <summary>
	/// 調撥單據更新_傳入
	/// </summary>
	public class PostAllocUpdateReq : StaffModel
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
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 單據類別
		/// </summary>
		public string AllocType { get; set; }
	}

	/// <summary>
	/// 調撥明細查詢_傳入
	/// </summary>
	public class GetAllocDetailReq : StaffModel
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
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 單據類別
		/// </summary>
		public string AllocType { get; set; }
	}

	/// <summary>
	/// 調撥明細查詢_傳回
	/// </summary>
	public class GetAllocDetailRes
	{
		public GetAllocDetailRes()
		{
			DetailList = new List<GetAllocDetail>();
			ItemSnList = new List<GetAllocItemSn>();
		}

		/// <summary>
		/// 調撥明細資料
		/// </summary>
		public List<GetAllocDetail> DetailList { get; set; }
		/// <summary>
		/// 商品序號資料
		/// </summary>
		public List<GetAllocItemSn> ItemSnList { get; set; }
	}

	/// <summary>
	/// 調撥明細資料Model
	/// </summary>
	public class GetAllocDetail
	{
		/// <summary>
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 調撥序號
		/// </summary>
		public string AllocSeq { get; set; }
		/// <summary>
		/// 路線順序
		/// </summary>
		public int Route { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		public string WhName { get; set; }
		/// <summary>
		/// 建議儲位
		/// </summary>
		public string SugLoc { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public DateTime ValidDate { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		public DateTime EnterDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MkNo { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		public string Sn { get; set; }
		/// <summary>
		/// 調撥數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 已調撥完成數量
		/// </summary>
		public int ActQty { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletNo { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		public string PackRef { get; set; }
		/// <summary>
		/// 商品條碼一
		/// </summary>
		public string EanCode1 { get; set; }
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
	/// 調撥明細資料Model
	/// </summary>
	public class GetAllocDetailForData
	{
		/// <summary>
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 調撥序號
		/// </summary>
		public int AllocSeq { get; set; }
		/// <summary>
		/// 路線順序
		/// </summary>
		public int Route { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		public string WhName { get; set; }
		/// <summary>
		/// 建議儲位
		/// </summary>
		public string SugLoc { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		public DateTime ValidDate { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		public DateTime EnterDate { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MkNo { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		public string Sn { get; set; }
		/// <summary>
		/// 調撥數量
		/// </summary>
		public int Qty { get; set; }
		/// <summary>
		/// 已調撥完成數量
		/// </summary>
		public int ActQty { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		public string PalletNo { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		public string PackRef { get; set; }
		/// <summary>
		/// 商品條碼一
		/// </summary>
		public string EanCode1 { get; set; }
	}

	/// <summary>
	/// 商品序號資料Model
	/// </summary>
	public class GetAllocItemSn
	{
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemNo { get; set; }
		/// <summary>
		/// 序號
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

	/// <summary>
	/// 取得調撥路線編號_傳入
	/// </summary>
	public class GetRouteListReq
	{
		/// <summary>
		/// 單號
		/// </summary>
		public string No { get; set; }
		/// <summary>
		/// 序號
		/// </summary>
		public int Seq { get; set; }
		/// <summary>
		/// 儲位
		/// </summary>
		public string LocCode { get; set; }
	}

	/// <summary>
	/// 取得調撥路線編號_回傳
	/// </summary>
	public class GetRouteListRes
	{
		/// <summary>
		/// 調撥單號
		/// </summary>
		public string No { get; set; }
		/// <summary>
		/// 調撥序號
		/// </summary>
		public int Seq { get; set; }
		/// <summary>
		/// 建議儲位
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 路線順序
		/// </summary>
		public int Route { get; set; }
	}

	public class PostAllocConfirmReq : StaffModel
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
		/// 調撥單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 調撥明細序號
		/// </summary>
		public string AllocSeq { get; set; }
		/// <summary>
		/// 實際儲位
		/// </summary>
		public string ActLoc { get; set; }
		/// <summary>
		/// 實際數量
		/// </summary>
		public int ActQty { get; set; }
		/// <summary>
		/// 單據類別
		/// </summary>
		public string AllocType { get; set; }
	}


	public class TmprTypeModel
	{
		public string Name { get; set; }
		public string TmprType { get; set; }
		public string TmprTypeName { get; set; }
	}
}
