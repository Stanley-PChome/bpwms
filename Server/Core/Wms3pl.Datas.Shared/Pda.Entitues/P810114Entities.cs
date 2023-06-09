using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
    #region 上架移動-容器查詢
    /// <summary>
    /// 上架移動-容器查詢_傳入
    /// </summary>
    public class GetApprovedDataReq : StaffModel
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
    /// 上架移動-容器查詢_傳回
    /// </summary>
    public class GetApprovedDataRes
    {
        /// <summary>
        /// F020501的流水號
        /// </summary>
        public long? F020501_ID { get; set; }
        /// <summary>
        /// 倉庫編號
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 倉庫名稱
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 移動狀況(0:不可上架、1:可上架、2:到達上架處)
        /// </summary>
        public string MoveFlag { get; set; }
        /// <summary>
        /// 上架訊息
        /// </summary>
        public string ApiInfo { get; set; }
        /// <summary>
        /// 移動中容器箱數
        /// </summary>
        public int MovingNum { get; set; }
        /// <summary>
        /// 移動中容器清單
        /// </summary>
        public List<GetApprovedMovingData> MovingListData { get; set; }
    }

    public class GetApprovedMovingData
    {
        /// <summary>
        /// 倉庫編號
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 容器編號
        /// </summary>
        public string ContainerCode { get; set; }
    }
    #endregion

    #region 上架移動-容器移動
    /// <summary>
    /// 上架移動-容器移動_傳入
    /// </summary>
    public class MoveContainerReq : StaffModel
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
        /// F020501的流水號
        /// </summary>
        public long? F020501_ID { get; set; }
    }

    /// <summary>
    /// 上架移動-容器移動_傳回
    /// </summary>
    public class MoveContainerRes
    {
        /// <summary>
        /// 移動中容器箱數
        /// </summary>
        public int MovingNum { get; set; }
        /// <summary>
        /// 移動中容器清單
        /// </summary>
        public List<GetApprovedMovingData> MovingListData { get; set; }
    }
    #endregion

    #region 上架移動-移動完成
    /// <summary>
    /// 上架移動-容器移動_傳入
    /// </summary>
    public class MoveCompletedReq : StaffModel
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
        /// F020501的流水號
        /// </summary>
        public long? F020501_ID { get; set; }
    }

    /// <summary>
    /// 上架移動-移動完成_傳回
    /// </summary>
    public class MoveCompletedRes
    {
        /// <summary>
        /// 移動中容器箱數
        /// </summary>
        public int MovingNum { get; set; }
        /// <summary>
        /// 移動中容器清單
        /// </summary>
        public List<GetApprovedMovingData> MovingListData { get; set; }
    }
    #endregion
}
