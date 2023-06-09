using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
    /// <summary>
    /// 搬移作業-查詢Req
    /// </summary>
    public class GetMoveReq : StaffModel
    {
        /// <summary>
        /// /功能編號
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
        /// 品號
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public string Sn { get; set; }
    }
    /// <summary>
    /// 搬移作業-儲位查詢Req
    /// </summary>
    public class GetMoveLocReq : StaffModel
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
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }
    }

    /// <summary>
    /// 搬移作業-儲位查詢Res
    /// </summary>
    public class GetMoveLocRes
    {
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ItemName { get; set; }

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
        /// 庫存數 先註解等文件釐清再加入
        /// </summary>
        public int StockQty { get; set; }
    }

    /// <summary>
    /// 搬移作業-商品儲位查詢Req
    /// </summary>
    public class GetMoveItemLocReq : StaffModel
    {
        /// <summary>
        /// /功能編號
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
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }
		/// <summary>
		/// 序號
		/// </summary>
		public string Sn { get; set; }
	}

    /// <summary>
    /// 搬移作業-商品儲位查詢Res
    /// </summary>
    public class GetMoveItemLocRes
    {
        /// <summary>
        /// 倉別名稱
        /// </summary>
        public string WhName { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 儲位
        /// </summary>
        public string Loc { get; set; }

        /// <summary>
        /// 庫存數
        /// </summary>
        public int StockQty { get; set; }
    }

    public class PostMoveConfirmReq : StaffModel
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
        /// 來源儲位
        /// </summary>
        public string SrcLoc { get; set; }

        /// <summary>
        /// 目的儲位
        /// </summary>
        public string TarLoc { get; set; }

        public List<Items> Items { get; set; }

    }

    public class Items
    {
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 入庫日期
        /// </summary>
        public string EnterDate { get; set; }

        /// <summary>
        /// 搬移數量
        /// </summary>
        public int MoveQty { get; set; }

        /// <summary>
        /// 箱號
        /// </summary>
        public string BoxNo { get; set; }

        /// <summary>
        /// 板號
        /// </summary>
        public string PalletNo { get; set; }

        /// <summary>
        /// 批號
        /// </summary>
        public string MkNo { get; set; }

        /// <summary>
        /// 商品序號
        /// </summary>
        public string Sn { get; set; }
    }

    public class CheckCustCodeLoc
    {
        public string CUST_CODE { get; set; }
        public string NOW_CUST_CODE { get; set; }
    }
}
