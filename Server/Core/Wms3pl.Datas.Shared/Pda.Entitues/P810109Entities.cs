using System;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
    /// <summary>
    /// 序號查詢_傳入
    /// </summary>
    public class GetSerialReq : StaffModel
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
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        public string Sn { get; set; }
    }

    /// <summary>
    /// 序號查詢_傳回
    /// </summary>
    public class GetSerialRes
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustNo { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        public string Sn { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 廠商編號 + 廠商名稱
        /// </summary>
        public string VnrName { get; set; }
        /// <summary>
        /// 單號
        /// </summary>
        public string WmsNo { get; set; }
        /// <summary>
        /// 入庫日
        /// </summary>
        public DateTime? EnterDate { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }
    }
}
