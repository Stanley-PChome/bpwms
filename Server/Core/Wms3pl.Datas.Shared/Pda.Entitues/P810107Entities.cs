using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
    /// <summary>
    /// 
    /// </summary>
    public class GetInvReq
    {
        /// <summary>
        /// 功能編號
        /// </summary>
        public string FuncNo { get; set; }

        /// <summary>
        /// 登入者帳號
        /// </summary>
        public string AccNo { get; set; }

        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcNo { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustNo { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string InvDate { get; set; }

        /// <summary>
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }
    }

    /// <summary>
    /// 盤點單號查詢
    /// </summary>
    public class GetInvRes
    {
        /// <summary>
        /// 盤點類型代碼
        /// </summary>
        public string InvType { get; set; }

        /// <summary>
        /// 盤點類型名稱
        /// </summary>
        public string InvTypeName { get; set; }

        /// <summary>
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcNo { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustNo { get; set; }

        /// <summary>
        /// 盤點日期
        /// </summary>
        public string InvDate { get; set; }

        /// <summary>
        /// 品項總數
        /// </summary>
        public int ItemCnt { get; set; }

        /// <summary>
        /// 商品總數
        /// </summary>
        public int ItemQty { get; set; }

        /// <summary>
        /// 是否暗盤
        /// </summary>
        public string StocktakeFlag { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 狀態名稱
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 是否為複盤作業
        /// </summary>
        public string FirstCountFlag { get; set; }

        /// <summary>
        /// 已盤點品項數量
        /// </summary>
        public int InvItemCnt { get; set; }

    }

    /// <summary>
    /// 盤點明細查詢-傳入
    /// </summary>
    public class GetDetailInvReq : StaffModel
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
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 是否同步
        /// </summary>
        public string IsSync { get; set; }

    }

    /// <summary>
    /// 盤點明細查詢-傳出
    /// </summary>
    public class GetDetailInvRes
    {
        /// <summary>
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 倉別編號
        /// </summary>
        public string WhNo { get; set; }

        /// <summary>
        /// 倉別名稱
        /// </summary>
        public string WhName { get; set; }

        /// <summary>
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 入庫日期
        /// </summary>
        public string EnterDate { get; set; }

        /// <summary>
        /// 庫存數
        /// </summary>
        public int StockQty { get; set; }

        /// <summary>
        /// 路線順序
        /// </summary>
        public int Router { get; set; }

        /// <summary>
        /// 儲區
        /// </summary>
        public string Zone { get; set; }

        /// <summary>
        /// 走道
        /// </summary>
        public string Aisle { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        public string MkNo { get; set; }

        /// <summary>
        /// 已盤點數量
        /// </summary>
        public int? ActQty { get; set; }

        /// <summary>
        /// 板號
        /// </summary>
        public string PalletNo { get; set; }

        /// <summary>
        /// 箱號
        /// </summary>
        public string BoxNo { get; set; }

		/// <summary>
		/// 商品類型
		/// 0 : 一般商品
		/// 1 : 序號商品
		/// 2 : 序號綁儲位
		/// </summary>
		public string SnType { get; set; }

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
        public long? BoxQty { get; set; }
    }

    /// <summary>
    /// DB新增傳出欄位
    /// </summary>
    public class GetDetailInvResData : GetDetailInvRes
    {
        public int[] Route { get; set; }
        public int InvSeq { get; set; }
    }

    public class PostInvConfirmReq : StaffModel
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
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 入庫日期
        /// </summary>
        public string EnterDate { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        public string MkNo { get; set; }

        /// <summary>
        /// 板號
        /// </summary>
        public string PalletNo { get; set; }

        /// <summary>
        /// 箱號
        /// </summary>
        public string BoxNo { get; set; }

        /// <summary>
        /// 盤點數量
        /// </summary>
        public int ActQty { get; set; }
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
        public long? BoxQty { get; set; }
    }

    public class PostInvNewItemReq : StaffModel
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
        /// 盤點單號
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 入庫日期
        /// </summary>
        public string EnterDate { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        public string Sn { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        public string MkNo { get; set; }

        /// <summary>
        /// 板號
        /// </summary>
        public string PalletNo { get; set; }

        /// <summary>
        /// 箱號
        /// </summary>
        public string BoxNo { get; set; }

        /// <summary>
        /// 盤點數量
        /// </summary>
        public int ActQty { get; set; }
    }
}
