using System;
using System.Data.Services.Common;
using System.Runtime.Serialization;
using Wms3pl.Datas.F01;

namespace Wms3pl.Datas.Shared.Entities
{

    #region Vendor (廠商)共用

    [Serializable]
    [DataContract]
    [DataServiceKey("VNR_CODE")]
    public class VendorInfo
    {
        [DataMember]
        public string VNR_CODE { get; set; }

        [DataMember]
        public string VNR_NAME { get; set; }
    }

    #endregion

    /// <summary>
    /// 廠商報到報表格式
    /// </summary>
    [Serializable]
    [DataContract]
    [DataServiceKey("ORDER_NO", "ORDER_SEQ")]//"STOCK_NO", "STOCK_SEQ"
    public class P020201ReportData
    {
        [DataMember]
        public string ORDER_NO { get; set; }//STOCK_NO
        [DataMember]
        public short ORDER_SEQ { get; set; }//STOCK_SEQ 
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string ORDER_UNIT { get; set; }//RET_UNIT 
        [DataMember]
        public int ORDER_QTY { get; set; }//STOCK_QTY 
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string SHOP_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public DateTime DELIVER_DATE { get; set; }
        [DataMember]
        public string SOURCE_NO { get; set; }

        /// <summary>
        /// 額外給報表轉換 barcode 用的欄位
        /// </summary>
        [DataMember]
        public string ItemCodeBarcode { get; set; }
        /// <summary>
        /// 材積單位
        /// </summary>
        [DataMember]
        public string VOLUME_UNIT { get; set; }
    }

    #region P010201 進倉單維護

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "STOCK_NO")]
	[DataContract]
	public class F010201Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string STOCK_NO { get; set; }
		[DataMember]
		public DateTime STOCK_DATE { get; set; }
		[DataMember]
		public DateTime? SHOP_DATE { get; set; }
		[DataMember]
		public DateTime DELIVER_DATE { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
		[DataMember]
		public string SOURCE_NAME { get; set; }
		[DataMember]
		public string SOURCE_NO { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string VNR_ADDRESS { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUSNAME { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ORD_PROP { get; set; }
		[DataMember]
		public string SHOP_NO { get; set; }
		[DataMember]
		public string EDI_FLAG { get; set; }
		[DataMember]
		public string FAST_PASS_TYPE { get; set; }
		[DataMember]
		public string BOOKING_IN_PERIOD { get; set; }
		[DataMember]
		public string USER_CLOSED_MEMO { get; set; }
	}

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F010202Data
    {
        [DataMember]
        public int ROWNUM { get; set; }
        public string ChangeFlag { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }
        [DataMember]
        public int STOCK_SEQ { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public int STOCK_QTY { get; set; }
        [DataMember]
        public DateTime? VALI_DATE { get; set; }
        [DataMember]
        public string UNIT_TRANS { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [DataMember]
        public string MAKE_NO { get; set; }
        [DataMember]
        public string EAN_CODE1 { get; set; }
        [DataMember]
        public string EAN_CODE2 { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]

    public class F010201MainData
    {
        [DataMember]
        public int ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string DC_NAME { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }

        [DataMember]
        public DateTime DELIVER_DATE { get; set; }

        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string VNR_ADDRESS { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }


        [DataMember]
        public string CRT_STAFF { get; set; }
        [DataMember]
        public DateTime CRT_DATE { get; set; }
        [DataMember]
        public string CRT_NAME { get; set; }
        [DataMember]
        public string UPD_STAFF { get; set; }
        [DataMember]
        public DateTime? UPD_DATE { get; set; }
        [DataMember]
        public string UPD_NAME { get; set; }

        [DataMember]

        public string BOOKING_IN_PERIOD { get; set; }
        [DataMember]

        public int ITEM_COUNT { get; set; }

    }
    #endregion

    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "GUP_CODE", "SHOP_NO")]
    public class F010101ShopNoList
    {
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public DateTime SHOP_DATE { get; set; }
        public string SHOP_NO { get; set; }
        public string VNR_CODE { get; set; }
        public string VNR_NAME { get; set; }
        public string STATUS { get; set; }
        public DateTime DELIVER_DATE { get; set; }
    }


    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "GUP_CODE", "SHOP_NO")]
    public class F010101Data
    {
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public System.DateTime SHOP_DATE { get; set; }
        public string SHOP_NO { get; set; }
        public System.DateTime DELIVER_DATE { get; set; }
        public string CUST_ORD_NO { get; set; }
        public string VNR_CODE { get; set; }
        public string VNR_NAME { get; set; }
        public string TEL { get; set; }
        public string ADDRESS { get; set; }
        public string BUSPER { get; set; }
        public string PAY_TYPE { get; set; }
        public string INVO_TYPE { get; set; }
        public string TAX_TYPE { get; set; }
        public System.DateTime INVOICE_DATE { get; set; }
        public string CONTACT_TEL { get; set; }
        public string INV_ADDRESS { get; set; }
        public string SHOP_CAUSE { get; set; }
        public string MEMO { get; set; }
        public string STATUS { get; set; }
        public System.DateTime CRT_DATE { get; set; }
        public string CRT_STAFF { get; set; }
        public string CRT_NAME { get; set; }
        public System.DateTime? UPD_DATE { get; set; }
        public string UPD_STAFF { get; set; }
        public string UPD_NAME { get; set; }
        public string ORD_PROP { get; set; }
    }


    [Serializable]
    [DataServiceKey("DC_CODE", "GUP_CODE", "GUP_CODE", "SHOP_NO")]
    public class F010102Data
    {
        public string SHOP_NO { get; set; }
        public string SHOP_SEQ { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_SIZE { get; set; }
        public string ITEM_SPEC { get; set; }
        public string ITEM_COLOR { get; set; }
        public System.Int32 SHOP_QTY { get; set; }
    }

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F010101ReportData
    {
        public System.Decimal ROWNUM { get; set; }
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public System.DateTime SHOP_DATE { get; set; }
        public string SHOP_NO { get; set; }
        public System.DateTime DELIVER_DATE { get; set; }
        public string CUST_ORD_NO { get; set; }
        public string VNR_CODE { get; set; }
        public string VNR_NAME { get; set; }
        public string TEL { get; set; }
        public string ADDRESS { get; set; }
        public string BUSPER { get; set; }
        public System.DateTime INVOICE_DATE { get; set; }
        public string CONTACT_TEL { get; set; }
        public string INV_ADDRESS { get; set; }
        public string SHOP_CAUSE { get; set; }
        public string MEMO { get; set; }
        public string PAY_TYPE { get; set; }
        public string TAX_TYPE { get; set; }
        public string INVO_TYPE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_SIZE { get; set; }
        public string ITEM_SPEC { get; set; }
        public string ITEM_COLOR { get; set; }
        public System.Int32 SHOP_QTY { get; set; }
        public string ShopNoBarcode { get; set; }
    }

    #region P5401 讀取進貨單資訊
    [Serializable]
    [DataServiceKey("ROWNUM", "STOCK_NO")]
    [DataContract]
    public class F010201QueryData
    {
        [DataMember]
        public int ROWNUM { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }

    }
    #endregion

    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class OrderIsProblem
    {
        [DataMember]
        public Decimal ROWNUM { get; set; }
        [DataMember]
        public string TYPE { get; set; }
        [DataMember]
        public string NO { get; set; }
        [DataMember]
        public DateTime CREATE_DATE { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string DC_NAME { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string GUP_NAME { get; set; }
    }


	#region 進倉匯入資料
	[DataContract]
	[Serializable]
	[DataServiceKey("PO_NO")]
	public class F010201ImportData
	{
		[DataMember]
		public string PO_NO { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string FAST_PASS_TYPE { get; set; }
		[DataMember]
		public string BOOKING_IN_PERIOD { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public int STOCK_QTY { get; set; }
		[DataMember]
		public DateTime? VALI_DATE { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
    }
    #endregion

    #region 進倉單維護-列印棧板標籤資料
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P010201PalletData
    {
        [DataMember]
        public Decimal ROWNUM { get; set; }

        /// <summary>
        /// 業主名稱
        /// </summary>
        [DataMember]
        public string GUP_NAME { get; set; }

        /// <summary>
        /// 進倉單號
        /// </summary>
        [DataMember]
        public string STOCK_NO { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        [DataMember]
        public string VNR_NAME { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ITEM_CODE { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [DataMember]
        public string ITEM_NAME { get; set; }

        /// <summary>
        /// 棧板疊法
        /// </summary>
        [DataMember]
        public string PALLET_LEVEL { get; set; }
        /// <summary>
        /// 商品箱入數
        /// </summary>
        [DataMember]
        public string ITEM_CASE_QTY { get; set; }

        /// <summary>
        /// 商品小包裝數
        /// </summary>
        [DataMember]
        public string ITEM_PACKAGE_QTY { get; set; }

        /// <summary>
        /// 板數
        /// </summary>
        [DataMember]
        public string PALLET_SEQ { get; set; }
        /// <summary>
        /// 商品條碼
        /// </summary>
        [DataMember]
        public string EAN_CODE1 { get; set; }
        /// <summary>
        /// 外箱條碼
        /// </summary>
        [DataMember]
        public string EAN_CODE3 { get; set; }


        /// <summary>
        /// 訂貨數說明
        /// </summary>
        [DataMember]
        public string ORDER_QTY_DESC { get; set; }


        /// <summary>
        /// 驗收數說明(空白)
        /// </summary>
        [DataMember]
        public string RECV_QTY_DESC { get; set; }

        /// <summary>
        /// 入庫日(空白)
        /// </summary>
        [DataMember]
        public string ENTER_DATE { get; set; }

        /// <summary>
        /// 效期
        /// </summary>
        [DataMember]
        public string VALID_DATE { get; set; }

        /// <summary>
        /// 棧板編號
        /// </summary>
        [DataMember]
        public string STICKER_NO { get; set; }

        /// <summary>
        /// 棧板編號條碼
        /// </summary>
        [DataMember]
        public string STICKER_BARCODE { get; set; }

        /// <summary>
        /// 列印日期
        /// </summary>
        [DataMember]
        public string PRINT_DATE { get; set; }
    }

    #endregion

    /// <summary>
    /// 第三方系統單號已產生WMS訂單
    /// </summary>
    [Serializable]
    [DataServiceKey("STOCK_NO")]
    public class ThirdPartOrders
    {
        public string STOCK_NO { get; set; }
        public string CUST_ORD_NO { get; set; }
        public string STATUS { get; set; }
        public string PROC_FLAG { get; set; }
        public string CUST_COST { get; set; }
    }

    [Serializable]
    [DataServiceKey("ID")]
    public class VW_F010301
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public Int64 ID { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        public string DC_CODE { get; set; }

        /// <summary>
        /// 物流商編號
        /// </summary>
        public string ALL_ID { get; set; }

        /// <summary>
        /// 物流商名稱
        /// </summary>
        public string ALL_NAME { get; set; }

        /// <summary>
        /// 收貨日期
        /// </summary>
        public DateTime? RECV_DATE { get; set; }

        /// <summary>
        /// 收貨時間
        /// </summary>
        public string RECV_TIME { get; set; }

        /// <summary>
        /// 收貨人員帳號
        /// </summary>
        public string RECV_USER { get; set; }

        /// <summary>
        /// 收貨人員名稱
        /// </summary>
        public string RECV_NAME { get; set; }

        /// <summary>
        /// 貨運單號
        /// </summary>
        public string SHIP_ORD_NO { get; set; }

        /// <summary>
        /// 箱數
        /// </summary>
        public Int16 BOX_CNT { get; set; }

        /// <summary>
        /// 簽單核對狀態(0:未核對;1:已核對)
        /// </summary>
        public string CHECK_STATUS { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string MEMO { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }
    }

    [Serializable]
    [DataServiceKey("ID")]
    public class VW_F010302
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public Int64 ID { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        public string DC_CODE { get; set; }

        /// <summary>
        /// 物流商編號
        /// </summary>
        public string ALL_ID { get; set; }

        /// <summary>
        /// 物流商名稱
        /// </summary>
        public string ALL_NAME { get; set; }

        /// <summary>
        /// 刷單核對日期
        /// </summary>
        public DateTime? CHECK_DATE { get; set; }

        /// <summary>
        /// 刷單核對時間
        /// </summary>
        public string CHECK_TIME { get; set; }

        /// <summary>
        /// 刷單人員帳號
        /// </summary>
        public string CHECK_USER { get; set; }

        /// <summary>
        /// 刷單人員名稱
        /// </summary>
        public string CHECK_NAME { get; set; }

        /// <summary>
        /// 貨運單號
        /// </summary>
        public string SHIP_ORD_NO { get; set; }

        /// <summary>
        /// 核對箱數
        /// </summary>
        public Int16 CHECK_BOX_CNT { get; set; }

        /// <summary>
        /// 貨單箱數
        /// </summary>
        public Int16 SHIP_BOX_CNT { get; set; }

        /// <summary>
        /// 核對箱數結果 0:核對失敗;1:核對成功
        /// </summary>
        public string CHECK_STATUS { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }
    }

    [Serializable]
    [DataServiceKey("ID")]
    public class ScanCargoData : F010301
    {
        public string LOGISTIC_NAME { get; set; }
        public bool IsSelected { get; set; }
    }

    [Serializable]
    [DataServiceKey("DC_CODE", "ALL_ID", "SHIP_ORD_NO", "ORD_CNT", "BOX_QTY")]
    public class ScanCargoStatistic
    {
        public string DC_CODE { get; set; }
        public string ALL_ID { get; set; }
        public string SHIP_ORD_NO { get; set; }
        public int? ORD_CNT { get; set; }
        public int? BOX_QTY { get; set; }
    }

    [Serializable]
    [DataServiceKey("RECV_DATE", "LOGISTIC_NAME", "SHIP_ORD_NO")]
    public class ReceiptUnCheckData
    {
        public DateTime? RECV_DATE { get; set; }
        public string LOGISTIC_NAME { get; set; }
        public string SHIP_ORD_NO { get; set; }
        public int? BOX_CNT { get; set; }
    }

    [Serializable]
    [DataServiceKey("ID")]
    public class ScanReceiptData : F010302
    {
        public string LOGISTIC_NAME { get; set; }
    }

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "STOCK_NO")]
	public class UserCloseStockParam
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DC_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CUST_CODE { get; set; }

		/// <summary>
		/// 進倉單號
		/// </summary>
		public string STOCK_NO { get; set; }

		/// <summary>
		///  備註原因
		/// </summary>
		public string USER_CLOSED_MEMO { get; set; }

		/// <summary>
		/// 是否人員確認要強制結案(0:否 1:是)
		/// </summary>
		public string IS_USER_CLOSED { get; set; }
	}

	public class UserCloseExecuteResult : ExecuteResult
	{
		public Boolean NeedConfirm { get; set; }
	}
}
