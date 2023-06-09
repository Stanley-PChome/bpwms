using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
    #region 容器查詢
    /// <summary>
    /// 容器查詢_傳入
    /// </summary>
    public class GetContainerInfoReq : StaffModel
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
        /// 容器條碼
        /// </summary>
        public string ContainerCode { get; set; }
	}

    /// <summary>
    /// 容器查詢_傳回
    /// </summary>
    public class GetContainerInfoRes
    {
        /// <summary>
        /// 容器條碼
        /// </summary>
        public string ContainerCode { get; set; }
        /// <summary>
        /// 單據編號
        /// </summary>
        public string WmsNo { get; set; }
        /// <summary>
        /// 作業指示
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 重點提示1
        /// </summary>
        public string Notify1 { get; set; }
        /// <summary>
        /// 重點提示2
        /// </summary>
        public string Notify2 { get; set; }
        /// <summary>
        /// 重點提示文字顏色
        /// </summary>
        public string NotifyFontColor { get; set; }
        /// <summary>
        /// 容器分格清單
        /// </summary>
        public List<GetContainerInfoDetailModel> Detail { get; set; } = new List<GetContainerInfoDetailModel>();
    }

    public class GetContainerInfoDetailModel
    {
        /// <summary>
        /// 容器分格條碼
        /// </summary>
        public string BinCode { get; set; }
        /// <summary>
        /// 容器分格商品清單
        /// </summary>
        public List<GetContainerInfoItemDetailModel> ItemDetail { get; set; } = new List<GetContainerInfoItemDetailModel>();
    }

    public class GetContainerInfoItemDetailModel
    {
        /// <summary>
        /// 商品編號
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
        /// 數量
        /// </summary>
        public int Qty { get; set; }
    }
    #endregion
}
