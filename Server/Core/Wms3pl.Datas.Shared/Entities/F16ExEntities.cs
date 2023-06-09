using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{

	#region - F161201 退貨單維護相關
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161201DetailDatas
	{
		[DataMember]
		public System.Decimal ROWNUM { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string RETURN_SEQ { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public System.Int32 RTN_QTY { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public System.Int32? AUDIT_QTY { get; set; }
		[DataMember]
		public System.Int32? MOVED_QTY { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public System.Decimal? AUDIT_QTY_SUM { get; set; }
	}
	#endregion

	#region 退貨點收維護
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161302Data
	{
		public System.Decimal ROWNUM { get; set; }
		public string RTN_CHECK_NO { get; set; }
		public System.Int32? RTN_CHECK_SEQ { get; set; }
		public string RETURN_NO { get; set; }
		public string PAST_NO { get; set; }
		public string EAN_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime? CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
	}
	#endregion

	#region 退貨檢驗
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161202Data
	{
		public System.Decimal ROWNUM { get; set; }
		public string RETURN_NO { get; set; }
		public string RETURN_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public System.Int32? RTN_QTY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime? CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string BUNDLE_SERIALNO { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161402Data
	{
		public System.Decimal ROWNUM { get; set; }
		public string RETURN_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string LOC_CODE { get; set; }
		public System.Int32? MOVED_QTY { get; set; }
		public System.Int32? RTN_QTY { get; set; }
		public System.Int32? AUDIT_QTY { get; set; }
		public string AUDIT_STAFF { get; set; }
		public string AUDIT_NAME { get; set; }
		public string MEMO { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime? CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string CAUSE { get; set; }
		public string ITEM_NAME { get; set; }
		public string BUNDLE_SERIALNO { get; set; }
		public System.Int32? TOTAL_AUDIT_QTY { get; set; }
		public System.Int32? DIFFERENT_QTY { get; set; }
        public string MULTI_FLAG { get; set; }
        public int HasNotInReturnItem { get; set; }
        /// <summary>
        /// 國際條碼1
        /// </summary>
        public string EAN_CODE1 { get; set; }
        /// <summary>
        /// 國際條碼2
        /// </summary>
        public string EAN_CODE2 { get; set; }
        /// <summary>
        /// 國際條碼3
        /// </summary>
        public string EAN_CODE3 { get; set; }

    }

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161202SelectedData
	{
		public System.Decimal ROWNUM { get; set; }
		public System.DateTime? RETURN_DATE { get; set; }
		public string RETURN_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public System.Int32? RTN_QTY { get; set; }
	}

	[Serializable]
	[DataServiceKey("RETURN_NO", "DC_CODE", "GUP_CODE", "CUST_CODE", "LOG_SEQ")]
	public class F16140101Data
	{
		public string RETURN_NO { get; set; }
		public System.Int32 LOG_SEQ { get; set; }
		public string SERIAL_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string ISPASS { get; set; }
		public string ERR_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string AUDIT_STAFF { get; set; }
		public string AUDIT_NAME { get; set; }
		public string MESSAGE { get; set; }
		public string ISRETURN { get; set; }
		public string ISPASS2 { get; set; }
	}
	#endregion

	#region F161601 退貨上架申請相關
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161601DetailDatas
	{
		public System.Decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string RTN_APPLY_NO { get; set; }
		public System.DateTime RTN_APPLY_DATE { get; set; }
		public string STATUS { get; set; }
		public string MEMO { get; set; }
		public System.Int32 RTN_APPLY_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public string SRC_LOC { get; set; }
		public string TRA_LOC { get; set; }
		public System.Int32 LOC_QTY { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public System.Int32 MOVED_QTY { get; set; }
		public System.Int32? POST_QTY { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public string VNR_CODE { get; set; }
		public string VNR_NAME { get; set; }
		public string TMPR_TYPE { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string MAKE_NO { get; set; }
		public string TAR_BOX_CTRL_NO { get; set; }
		public string TAR_PALLET_CTRL_NO { get; set; }
		public string TAR_MAKE_NO { get; set; }
		public DateTime TAR_VALID_DATE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ITEM_CODE", "LOC_CODE", "LOC_QTY", "MOVED_QTY")]
	public class F161401ReturnWarehouse
	{
		public int ROWNUM { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public string LOC_CODE { get; set; }
		public System.Decimal LOC_QTY { get; set; }
		public System.Int32 MOVED_QTY { get; set; }
		public string VNR_CODE { get; set; }
		public string VNR_NAME { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string TMPR_TYPE { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string MAKE_NO { get; set; }
		public string TAR_BOX_CTRL_NO { get; set; }
		public string TAR_PALLET_CTRL_NO { get; set; }
		public string TAR_MAKE_NO { get; set; }
		public DateTime TAR_VALID_DATE { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class PrintF161601Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string RTN_APPLY_NO { get; set; }
		[DataMember]
		public DateTime RTN_APPLY_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public Int32 RTN_APPLY_SEQ { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string SRC_LOC { get; set; }
		[DataMember]
		public string TRA_LOC { get; set; }
		[DataMember]
		public Int32 LOC_QTY { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public Int32 MOVED_QTY { get; set; }
		[DataMember]
		public Int32 POST_QTY { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string APPROVE_STAFF { get; set; }
		[DataMember]
		public string APPROVE_NAME { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
	}

	[Serializable]
	[DataServiceKey("RTN_APPLY_NO", "RTN_APPLY_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F161602Ex
	{
		public string RTN_APPLY_NO { get; set; }
		public System.Int32 RTN_APPLY_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public string SRC_LOC { get; set; }
		public string TRA_LOC { get; set; }
		public System.Int32 LOC_QTY { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public System.Int32 MOVED_QTY { get; set; }
		public System.Int32? POST_QTY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string VNR_CODE { get; set; }
		public string SRC_WAREHOUSE_ID { get; set; }
		public DateTime? VALID_DATE { get; set; }
		public string MAKE_NO { get; set; }
		public DateTime? TAR_VALID_DATE { get; set; }
		public string TAR_BOX_CTRL_NO { get; set; }
		public string TAR_MAKE_NO { get; set; }
		public string TAR_PALLET_CTRL_NO { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
        public DateTime? ENTER_DATE { get; set; }

    }

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P160102Report
	{
		public System.Decimal ROWNUM { get; set; }
		public string ALLOCATION_NO { get; set; }
		public DateTime ALLOCATION_DATE { get; set; }
		public string SRC_WAREHOUSE_NAME { get; set; }
		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string RTN_APPLY_NO { get; set; }
		public string APPROVE_STAFF { get; set; }
		public string APPROVE_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string SRC_LOC_CODE { get; set; }
		public string SUG_LOC_CODE { get; set; }
		public System.Int64 TAR_QTY { get; set; }

		public string RtnApplyNoBarcode { get; set; }
		public string AllocationNoBarcode { get; set; }
		public string ItemCodeBarcode { get; set; }
	}
	#endregion

	#region F160201 廠退單
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160201Data
	{
		[DataMember]
		public System.Decimal ROWNUM { get; set; }
		[DataMember]
		public string RTN_VNR_NO { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public System.DateTime RTN_VNR_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string ORD_PROP { get; set; }
		[DataMember]
		public string RTN_VNR_TYPE_ID { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE { get; set; }
	
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string COST_CENTER { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public System.DateTime? POSTING_DATE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string STATUS_TEXT { get; set; }
		[DataMember]
		public string ORD_PROP_TEXT { get; set; }
		[DataMember]
		public string RTN_VNR_TYPE_TEXT { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE_TEXT { get; set; }
		[DataMember]
		public bool IS_SELECTED { get; set; }
    [DataMember]
    public string ADDRESS { get; set; }
    [DataMember]
    public string ITEM_CONTACT { get; set; }
    [DataMember]
    public string ITEM_TEL { get; set; }
		[DataMember]
		public string DELIVERY_WAY { get; set; }
		[DataMember]
		public string DELIVERY_WAY_NAME { get; set; }
		[DataMember]
		public string TYPE_ID { get; set; }
		[DataMember]
		public string TYPE_NAME { get; set; }

	}

	//查詢結果的明細
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160201DataDetail
	{
		[DataMember]
		public System.Decimal ROWNUM { get; set; }
		[DataMember]
		public string RTN_VNR_NO { get; set; }
		[DataMember]
		public int RTN_VNR_SEQ { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
			[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string ORG_WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int RTN_VNR_QTY { get; set; }
		[DataMember]
		public int RTN_WMS_QTY { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		/*
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		*/
		[DataMember]
		public int NOT_RTN_WMS_QTY { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public int INVENTORY_QTY { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public DateTime RTN_VNR_DATE { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE_NAME { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
	}

	//廠退單-新增-廠退明細
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160201ReturnDetail
	{
		[DataMember]
		public System.Decimal ROWNUM { get; set; }
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
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public int INVENTORY_QTY { get; set; }
		[DataMember]
		public int RTN_VNR_QTY { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public System.DateTime? ENTER_DATE { get; set; }
		[DataMember]
		public System.DateTime? VALID_DATE { get; set; }
		[DataMember]
		public int ORIGINAL_RTN_VNR_QTY { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE { get; set; }
		[DataMember]
		public string RTN_VNR_CAUSE_NAME { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
	}
	#endregion

	#region F160204 廠退出貨
	//廠退出貨查詢結果
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160204SearchResult
	{
		[DataMember]
		public int ROWNUM { get; set; }

		[DataMember]
		public string RTN_WMS_NO { get; set; }	//廠退出貨單號


		[DataMember]
		public string ORD_PROP { get; set; }		//作業類別

		[DataMember]
		public string ORD_PROP_TEXT { get; set; }	//作業類別文字

		[DataMember]
		public string RTN_VNR_NO { get; set; }    //廠退單號

		[DataMember]
		public string RTN_VNR_SEQ { get; set; }    //廠退序號


		[DataMember]
		public string VNR_CODE { get; set; }		//廠商編號

		[DataMember]
		public string VNR_NAME { get; set; }		//廠商名稱

		[DataMember]
		public string ITEM_CODE { get; set; }		//商品品號

		[DataMember]
		public string ITEM_NAME { get; set; }		//商品品名

		[DataMember]
		public string ITEM_SIZE { get; set; }		//商品尺寸

		[DataMember]
		public string ITEM_SPEC { get; set; }		//商品規格

		[DataMember]
		public string ITEM_COLOR { get; set; }		//商品顏色

		[DataMember]
		public int RTN_WMS_QTY { get; set; }		//廠退數量

		[DataMember]
		public DateTime? CRT_DATE { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [DataMember]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人名
    /// </summary>
    [DataMember]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 廠退出貨單建立日期
    /// </summary>
    [DataMember]
    public DateTime RTN_WMS_DATE { get; set; }

		/// <summary>
		/// 配送方式
		/// </summary>
		[DataMember]
		public string DELIVERY_WAY { get; set; }

		/// <summary>
		/// 配送方式名稱
		/// </summary>
		[DataMember]
		public string DELIVERY_WAY_NAME { get; set; }

		/// <summary>
		/// 出貨倉別
		/// </summary>
		[DataMember]
		public string TYPE_ID { get; set; }
		/// <summary>
		/// 出貨倉別名稱
		/// </summary>
		[DataMember]
		public string TYPE_NAME { get; set; }
		/// <summary>
		/// 出貨地址
		/// </summary>
		[DataMember]
		public string ADDRESS { get; set; }

		/// <summary>
		/// 貨主單號
		/// </summary>
		[DataMember]
		public string CUST_ORD_NO { get; set; }

		/// <summary>
		/// 聯絡電話
		/// </summary>
		[DataMember]
		public string ITEM_TEL { get; set; }

		/// <summary>
		/// 聯絡人
		/// </summary>
		[DataMember]
		public string ITEM_CONTACT { get; set; }

		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 備註/自取訊息
		/// </summary>
		[DataMember]
		public string MEMO { get; set; }
	}

	//廠退出貨明細
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160204Detail
	{
		[DataMember]
		public int ROWNUM { get; set; }

		[DataMember]
		public string RTN_VNR_NO { get; set; }		//廠退單號

		[DataMember]
		public string VNR_CODE { get; set; }		//廠商編號

		[DataMember]
		public string VNR_NAME { get; set; }		//廠商名稱

		[DataMember]
		public string ITEM_CODE { get; set; }		//商品品號

		[DataMember]
		public string ITEM_NAME { get; set; }		//商品品名

		[DataMember]
		public string ITEM_SIZE { get; set; }		//商品尺寸

		[DataMember]
		public string ITEM_SPEC { get; set; }		//商品規格

		[DataMember]
		public string ITEM_COLOR { get; set; }		//商品顏色

		[DataMember]
		public int RTN_VNR_QTY_SUM { get; set; }		//廠退數量(A)

		[DataMember]
		public int RTN_VNR_QTY_GRAND_TOTAL { get; set; }		//累計廠退數量(B)

		[DataMember]
		public int RTN_VNR_QTY_REMAINDER { get; set; }		//廠退出貨數量(C)

		// * A、B、C 
		//   加入到廠退出貨明細的廠退明細(F160202)，依照同廠商、同商品併成一筆，RTN_VNR_QTY 加總變成 F160204Detail.RTN_VNR_QTY(A)，
		//   RTN_WMS_QTY 加總變成 F160204Detail.RTN_VNR_QTY_GRAND_TOTAL(B)，
		//   F160204Detail.RTN_VNR_QTY_REMAINDER(C) = A - B

			/// <summary>
			/// 配送方式
			/// </summary>
		[DataMember]
		public string DELIVERY_WAY { get; set; }			

		[DataMember]
		public string RTN_WMS_NO { get; set; }

		[DataMember]
		public short RTN_WMS_SEQ { get; set; }

		[DataMember]
		public string CUST_CODE { get; set; }

		[DataMember]
		public string GUP_CODE { get; set; }

		[DataMember]
		public string DC_CODE { get; set; }

		[DataMember]
		public DateTime? RTN_WMS_DATE { get; set; }

		[DataMember]
		public string DISTR_CAR_NO { get; set; }

		[DataMember]
		public int RTN_VNR_SEQ { get; set; }  //對應F160202

		[DataMember]
		public int RTN_VNR_SEQ_SHOW { get; set; }  //對應F160202
		[DataMember]
		public int RTN_WMS_QTY { get; set; }	//對應F160202

		[DataMember]
		public string ORD_PROP { get; set; }

		[DataMember]
		public string CRT_STAFF { get; set; }

		[DataMember]
		public string CRT_NAME { get; set; }

		[DataMember]
		public DateTime? CRT_DATE { get; set; }

		[DataMember]
		public string UPD_STAFF { get; set; }

		[DataMember]
		public string UPD_NAME { get; set; }

		[DataMember]
		public DateTime? UPD_DATE { get; set; }

		[DataMember]
		public string DISTR_CAR { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string TYPE_ID { get; set; }
		[DataMember]
		public string ITEM_TEL { get; set; }
		[DataMember]
		public string ITEM_CONTACT { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
	}

	
	#endregion

	#region F160502Data 銷毀明細
	[DataContract]
	[Serializable]
	[DataServiceKey("ITEM_CODE", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F160502Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
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
		public string VIRTUAL_TYPE { get; set; }
		[DataMember]
		public string ITEM_SERIALNO { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }
		[DataMember]
		public int SCRAP_QTY { get; set; }
		[DataMember]
		public int DESTROY_QTY { get; set; }


	}
	#endregion

	#region F160501Data 銷毀主檔
	[DataContract]
	[Serializable]
	[DataServiceKey("DESTROY_NO", "DC_CODE")]
	public class F160501Data
	{
		[DataMember]
		public string DESTROY_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DESTROY_DATE { get; set; }
		[DataMember]
		public string DISTR_CAR { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime? POSTING_DATE { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public string EDI_FLAG { get; set; }
        [DataMember]
        public string All_ID { get; set; }


	}
	#endregion

	#region 報廢單維護

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "SCRAP_NO", "SCRAP_SEQ")]
	public class F160402Data
	{
		public string SCRAP_NO { get; set; }
		public System.Int32 SCRAP_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public System.Int32 SCRAP_QTY { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string LOC_CODE { get; set; }
		public string SCRAP_CAUSE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public System.Decimal? QTY { get; set; }
		public System.Decimal? ALL_QTY { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
	}


	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F160402AddData
	{
		public System.Decimal ROWNUM { get; set; }
		public string ITEM_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string LOC_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public string SCRAP_CAUSE { get; set; }
		public System.Int32? SCRAP_QTY { get; set; }
		public System.Decimal? QTY { get; set; }
		public System.Decimal? ALL_QTY { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
        public string MAKE_NO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ITEM_CODE", "LOC_CODE", "WAREHOUSE_ID")]
	public class F160402StockSum
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? ALL_QTY { get; set; }
	}

	#endregion

	#region F160501 更新狀態檢查
	[DataContract]
	[Serializable]
	[DataServiceKey("DESTROY_NO")]
	public class F160501Status
	{
		[DataMember]
		public string DESTROY_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }

	}
	#endregion

	#region F160501FileData 上傳檔案資訊
	[DataContract]
	[Serializable]
	[DataServiceKey("DESTROY_NO")]
	public class F160501FileData
	{
		[DataMember]
		public string DESTROY_NO { get; set; }
		[DataMember]
		public string UPLOAD_SEQ { get; set; }
		[DataMember]
		public string UPLOAD_S_PATH { get; set; }
		[DataMember]
		public string UPLOAD_C_PATH { get; set; }
		[DataMember]
		public string UPLOAD_DESC { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string DB_Flag { get; set; }
		[DataMember]
		public string IsDelete { get; set; }


	}
	#endregion

	#region F161301 退貨點收維護
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "RTN_CHECK_NO")]
	public class F161301Data
	{
		[DataMember]
		public string RTN_CHECK_NO { get; set; }
		[DataMember]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public DateTime RECEIPT_DATE { get; set; }
		[DataMember]
		public string TRANSPORT { get; set; }
		[DataMember]
		public string CAR_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public Decimal? RTN_CHECK_SEQ_COUNT { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161301Report
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public DateTime RECEIPT_DATE { get; set; }
		[DataMember]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public string CAR_NO { get; set; }
		[DataMember]
		public Decimal RTN_CHECK_SEQ_COUNT { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
		[DataMember]
		public string EAN_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string TRANSPORT { get; set; }
		[DataMember]
		public string PRINTER { get; set; }
		[DataMember]
		public string PRINT_DATE { get; set; }
	}

	#endregion

	#region API - 退貨名細
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ApiF161201Data
	{
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public DateTime? POSTING_DATE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int MOVED_QTY { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string LTYPE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string SUPPORT_CODE { get; set; }
		[DataMember]
		public string SUPPORT_NAME { get; set; }

	}
	#endregion

	#region P017_退貨報表
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P160201Report
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime RETURN_DATE { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public Decimal AUDIT_QTY { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string RETAIL_NAME { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string TYPE_NAME { get; set; }
	}
	#endregion

	#region P107_退貨記錄總表報表
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P17ReturnAuditReport
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime RETURN_DATE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string RTN_CUST_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public Int32 RTN_QTY { get; set; }
		[DataMember]
		public Int32 AUDIT_QTY { get; set; }
		[DataMember]
		public Int32 SHORTFALL { get; set; }
		[DataMember]
		public Int32 MULTIRETURN { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public string AUDIT_NAME { get; set; }
		[DataMember]
		public DateTime UPD_DATE { get; set; }
	}
	#endregion

	#region RTO17840退貨記錄表
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RTO17840ReturnAuditReport
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public DateTime RETURN_DATE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32 RTN_QTY { get; set; }
		[DataMember]
		public Int32 AUDIT_QTY { get; set; }
		[DataMember]
		public Decimal DIFFERENCE { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public Decimal ABNORMAL { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
	}
	#endregion

	#region B2C退貨記錄表(Friday退貨記錄表)
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class B2CReturnAuditReport
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string TEL { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public DateTime RETURN_DATE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32 RTN_QTY { get; set; }
		[DataMember]
		public Int32 AUDIT_QTY { get; set; }
		[DataMember]
		public Decimal DIFFERENCE { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string DESCRIPTION { get; set; }
	}
	#endregion

	#region P106_退貨未上架明細表
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P106ReturnNotMoveDetail
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime AUDIT_DATE { get; set; }
		[DataMember]
		public DateTime? POSTING_DATE { get; set; }
		[DataMember]
		public string RTN_CUST_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32 RTN_QTY { get; set; }
		[DataMember]
		public Int32 AUDIT_QTY { get; set; }
		[DataMember]
		public Decimal ALLOW_QTY { get; set; }
		[DataMember]
		public Decimal NOTALLOW_QTY { get; set; }
		[DataMember]
		public Decimal NOTMOVED_QTY { get; set; }
	}
	#endregion

	#region 退貨詳細資料
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class TxtFormatReturnDetail
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string SECOND_ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public DateTime? DELV_DATE { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
	}
	#endregion

	#region 退貨資料
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReturnSerailNoByType
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string STATUS { get; set; }
	}
	#endregion

	#region P015_預計退貨明細表
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P015ForecastReturnDetail
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string RTN_CUST_CODE { get; set; }
		[DataMember]
		public string RTN_CUST_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32 RTN_QTY { get; set; }
	}
	#endregion

	#region F1612ImportData 退貨匯入資料
	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE")]
	public class F1612ImportData
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_PROP { get; set; }
		[DataMember]
		public string COST_CENTER { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string RETAIL_CODE_NAME { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string TEL { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string DISTR_CAR { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE	{ get; set; }
		[DataMember]
		public int RTN_QTY { get; set; }
		[DataMember]
		public string RTN_CAUSE { get; set; }
		[DataMember]
		public string RTN_TYPE_ID { get; set; }

	}
	#endregion

	#region F160501ItemType 銷毀單Item 類別
	[DataContract]
	[Serializable]
	[DataServiceKey("DESTROY_NO")]
	public class F160501ItemType
	{
		[DataMember]
		public string DESTROY_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string VIRTUAL_TYPE { get; set; }

	}
	#endregion


	#region 產生退貨檢驗身擋明細-退貨檢驗身擋與BOM表JOIN
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class F161402ToF16140201Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public int RETURN_AUDIT_SEQ { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int RTN_QTY { get; set; }
		[DataMember]
		public int AUDIT_QTY { get; set; }
		[DataMember]
		public string MATERIAL_CODE { get; set; }
		[DataMember]
		public int? BOM_QTY { get; set; }
		[DataMember]
		public int UNAUDIT_QTY { get; set; }
		[DataMember]
		public int ITEM_QTY { get; set; }
		[DataMember]
		public string MULTI_FLAG { get; set; }
	}

	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ReturnDetailSummary
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public int RTN_QTY { get; set; }
		[DataMember]
		public int AUDIT_QTY { get; set; }
		[DataMember]
		public int MISS_QTY { get; set; }
		[DataMember]
		public int GOOD_QTY { get; set; }
		[DataMember]
		public int BAD_QTY { get; set; }
		[DataMember]
		public string BOM_ITEM_CODE { get; set; }
		[DataMember]
		public string BOM_NAME { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F161402RtnData
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string RETURN_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int MOVED_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P160203Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string RP_TYPE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public int ITEM_CNT { get; set; }
		[DataMember]
		public int REMIND_DAYS { get; set; }
		[DataMember]
		public string RP_NO { get; set; }
		[DataMember]
		public string RP_TYPE_DESC { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P160203Detail
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string RP_NO { get; set; }
		[DataMember]
		public decimal RP_SEQ { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public decimal RET_DAYS { get; set; }
		[DataMember]
		public decimal STOCK_QTY { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("VNR_CODE")]
	public class F160204Data
	{
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string PROC_FLAG { get; set; }
		[DataMember]
		public string DELIVERY_WAY { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public int? SHEET_NUM { get; set; }
		[DataMember]
		public string MEMO { get; set; }

		//[DataMember]
		//public string SheetNum { get; set; }
	}

	public class ExecuteLmsPdfApiResult
	{
		public bool IsSuccessed { get; set; }
		public string HttpCode { get; set; }
		public string Message { get; set; }
		public byte[] Data { get; set; }
	}
}
