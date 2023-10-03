using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataServiceKey("DELV_DATE", "PICK_TIME")]
	public class F051201Data
	{
		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public decimal ORDCOUNT { get; set; }
		public decimal PICKCOUNT { get; set; }
		public decimal ITEMCOUNT { get; set; }
		public decimal TOTALPICK_QTY { get; set; }
		public string ISPRINTED { get; set; }

		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string SOURCE_TYPE { get; set; }
		public string SOURCE_NAME { get; set; }
		public string PICK_TYPE { get; set; }
	}
	[Serializable]
	[DataServiceKey("WAREHOUSE_NAME", "TEMP_TYPE", "FLOOR")]
	public class F051202Data
	{
		public string WAREHOUSE_NAME { get; set; }
		public string TMPR_TYPE { get; set; }
		public string FLOOR { get; set; }
		public decimal ITEMCOUNT { get; set; }
		public decimal TOTALPICK_QTY { get; set; }
	}
	[Serializable]
	[DataServiceKey("PICK_ORD_NO", "PICK_TIME")]
	public class F051201SelectedData
	{
		public bool IsSelected { get; set; }
		public string PICK_ORD_NO { get; set; }
		public string PICK_TIME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F051201ReportDataA : IEncryptable
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		//增加通路類型
		[DataMember]
		public string CHANNEL { get; set; }
		[DataMember]
		public string NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string TMPR_TYPE_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		[DataMember]
		public Int32 B_PICK_QTY { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		[DataMember]
		public string WmsOrdNoBarcode { get; set; }
		[DataMember]
		public string ItemCodeBarcode { get; set; }

		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string SerialNoBarcode { get; set; }
		[DataMember]
		public string PickLocBarcode { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string ORDER_CUST_NAME { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string MEMO { get; set; }

		[DataMember]
		public string EAN_CODE1 { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"ORDER_CUST_NAME", "NAME"}
		//					};
		//	}
		//}
		[DataMember]
		public string SHORT_NAME { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051201ReportDataB
	{
		public int ROWNUM { get; set; }
		public string PICK_ORD_NO { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string WMS_ORD_NO { get; set; }
		public int B_DELV_QTY { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_NAME { get; set; }
		public string PickOrdNoBarcode { get; set; }

		public string ITEM_UNIT { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ItemCodeBarcode { get; set; }
	}

	#region F0513 - 出貨碼頭分配
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_DATE", "PICK_TIME")]
	public class F0513WithF1909
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string ORD_PIER_CODE { get; set; }
		[DataMember]
		public string PIER_CODE { get; set; }
		[DataMember]
		public string STATUS { get; set; }

		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
	}
	#endregion

	#region F050801 - 出貨抽稽查詢
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_ORD_NO", "ORD_NO")]
	public class F050801WithF055001
	{
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
		public System.DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string PAST_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public System.Decimal? STATUS { get; set; }
		public string ORD_NO { get; set; }
		public System.Int32 ISMERGE { get; set; }
	}
	#endregion

	#region F0513 出貨時間
	[Serializable]
	[DataContract]
	[DataServiceKey("ROW_NUM", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F0513PickTime
	{
		[DataMember]
		public System.Decimal? ROW_NUM { get; set; }
		[DataMember]
		public System.DateTime? DELV_DATE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string ORD_TIME { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public System.Int16? RETAIL_QTY { get; set; }
		[DataMember]
		public System.Int32? PICK_CNT { get; set; }
		[DataMember]
		public string PROC_FLAG { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
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
		public string PIER_CODE { get; set; }
		[DataMember]
		public string CHECKOUT_TIME { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string CAR_NO_A { get; set; }
		[DataMember]
		public string CAR_NO_B { get; set; }
		[DataMember]
		public string CAR_NO_C { get; set; }
		[DataMember]
		public string ISSEAL { get; set; }
	}
	#endregion

	#region 缺貨作業-揀貨列表
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F051206Pick
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
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
	}
	#endregion

	#region 缺貨作業-調撥列表
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F051206AllocationList
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string TAR_DC_CODE { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }

	}
	#endregion

	#region 缺貨作業-缺貨明細
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051206LackList
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public int? IsUpdate { get; set; }
		[DataMember]
		public string ITEM_Name { get; set; }
		[DataMember]
		public int LACK_SEQ { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PICK_ORD_SEQ { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public Nullable<int> B_PICK_QTY { get; set; }
		[DataMember]
		public Nullable<int> LACK_QTY { get; set; }
		[DataMember]
		public string REASON { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string RETURN_FLAG { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public Nullable<System.DateTime> UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ISDELETED { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public bool ISSELECTED { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }

  }

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051206LackList_Allot
	{
		[DataMember]
		public decimal LACK_SEQ { get; set; }
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
		[DataMember]
		public short ALLOCATION_SEQ { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int LACK_QTY { get; set; }
		[DataMember]
		public int SRC_QTY { get; set; }
		[DataMember]
		public string REASON { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public bool ISSELECTED { get; set; }
		[DataMember]
		public string LACK_TYPE { get; set; }
		[DataMember]
		public string SRC_LOC_CODE { get; set; }
		[DataMember]
		public string TAR_LOC_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
	}
	#endregion

	#region 扣帳作業-批次
	/// <summary>
	/// 取得尚未扣帳,已列印出車明細表的批次時段
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_DATE", "PICK_TIME")]
	public class F0513WithF050801Batch
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public Decimal WMS_ORD_COUNT { get; set; }
		[DataMember]
		public Decimal AUDIT_COUNT { get; set; }
		[DataMember]
		public Decimal DEBIT_COUNT { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
		[DataMember]
		public string SOURCE_NAME { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string ORD_TYPE_DESC { get; set; }

		public Decimal NOSHIP_COUNT { get; set; }
	}
	#endregion

	#region F050801 虛擬儲位回復
	/// <summary>
	/// 虛擬儲位回復查詢結果
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("ORD_NO", "CUST_CODE", "GUP_CODE", "DC_CODE")]
	public class F05030101Ex
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public System.DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
	}

	/// <summary>
	/// 虛擬儲位回復勾選確認要處理的欄位
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("ORD_NO", "WMS_ORD_NO", "ORDER_NO", "ORDER_SEQ", "ITEM_CODE", "SERIAL_NO", "VALID_DATE", "CUST_CODE", "GUP_CODE", "DC_CODE", "VNR_CODE")]
	public class F05030101WithF051202
	{
    /// <summary>
    /// 訂單編號
    /// </summary>
		public string ORD_NO { get; set; }
    /// <summary>
    /// 出貨單編號
    /// </summary>
    public string WMS_ORD_NO { get; set; }
    /// <summary>
    /// 揀貨單編號
    /// </summary>
    public string ORDER_NO { get; set; }
    /// <summary>
    /// 揀貨單序號
    /// </summary>
    public string ORDER_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public string SERIAL_NO { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public string VNR_CODE { get; set; }
		public string PICK_LOC { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public System.Int32 A_PICK_QTY { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string PROC_FLAG { get; set; }
		public System.Decimal F581_STATUS { get; set; }

		public DateTime ENTER_DATE { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string FOREIGN_WMSNO { get; set; }
		public string MAKE_NO { get; set; }
    public string F1511_STATUS { get; set; }
    public string F050301_CUST_COST { get; set; }
  }
	#endregion

	#region 出貨裝車
	[Serializable]
	[DataContract]
	[DataServiceKey("GUP_CODE", "CUST_CODE", "DC_CODE", "DELV_DATE", "PICK_TIME")]
	public class F050801WithF700102
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public int LOAD_OWNER_QTY { get; set; }
		[DataMember]
		public int LOAD_OWNER_OK_QTY { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public int PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string NEED_SEAL { get; set; }
		[DataMember]
		public string CAR_NO_A { get; set; }
		[DataMember]
		public string CAR_NO_B { get; set; }
		[DataMember]
		public string CAR_NO_C { get; set; }
		[DataMember]
		public string ISSEAL { get; set; }
		[DataMember]
		public DateTime TAKE_DATE { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
	}
	#endregion

	#region 出貨包裝
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_ORD_NO", "ITEM_CODE")]
	public class F055002Summary
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int? TOTAL_PACK_QTY { get; set; }
	}

	/// <summary>
	/// 出貨包裝的資料集
	/// 只顯示, 不回存資料庫
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ItemCode")]
	//[IgnoreProperties("EncryptedProperties")]
	public class DeliveryData : IEncryptable
	{
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string ItemName { get; set; }
		[DataMember]
		public int OrderQty { get; set; }
		[DataMember]
		public int PackQty { get; set; }
		[DataMember]
		public int TotalPackQty { get; set; }
		[DataMember]
		public int DiffQty { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }
		[DataMember]
		public string BUNDLE_SERIALLOC { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string AllowOrdItem { get; set; }
		/// <summary>
		/// 是否易遺失(0: 否, 1: 是)
		/// </summary>
		[DataMember]
		public string IsEasyLose { get; set; }
		/// <summary>
		/// 貴重品標示(0: 否, 1: 是)
		/// </summary>
		[DataMember]
		public string IsPrecious { get; set; }
		/// <summary>
		/// 強磁標示(0: 否, 1: 是)
		/// </summary>
		[DataMember]
		public string IsMagnetic { get; set; }
		/// <summary>
		/// 易變質標示(0: 否, 1: 是)
		/// </summary>
		[DataMember]
		public string IsPerishable { get; set; }
		/// <summary>
		/// 易碎品包裝(0否1是)
		/// </summary>
		[DataMember]
		public string Fragile { get; set; }
		/// <summary>
		/// 防溢漏包裝(0否1是)
		/// </summary>
		[DataMember]
		public string Spill { get; set; }
		/// <summary>
		/// 商品溫層
		/// </summary>
		[DataMember]
		public string TmprTypeName { get; set; }
		/// <summary>
		/// 商品特徵
		/// </summary>
		[DataMember]
		public string Feature { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"CONSIGNEE", "NAME"}
		//					};
		//	}
		//}
	}

	/// <summary>
	/// 箱明細
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class DeliveryReport //: IEncryptable
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string OrdNo { get; set; }
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string ItemName { get; set; }
		[DataMember]
		public int PackQty { get; set; }
		//[DataMember]
		//[Encrypted]
		//[SecretPersonalData("NAME")]
		//public string CONSIGNEE { get; set; }
		//[DataMember]
		//public string ITEM_UNIT { get; set; }
		//[DataMember]
		//public string ITEM_SIZE { get; set; }
		//[DataMember]
		//public string ITEM_SPEC { get; set; }
		//[DataMember]
		//public string ITEM_COLOR { get; set; }
		[DataMember]
		public int PackageBoxNo { get; set; }
		//[DataMember]
		//public string BoxNo { get; set; }
		[DataMember]
		public string ItemCodeBarcode { get; set; }
		//[DataMember]
		//public string EAN_CODE1 { get; set; }
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		//[DataMember]
		//public string ORG_ITEM_CODE { get; set; }
		//[DataMember]
		//public string ItemName_MOMO { get; set; }
		//[DataMember]
		//public string RETAIL_CODE { get; set; }
		//[DataMember]
		//public string SHORT_SALESBASE_NAME { get; set; }
		//[DataMember]
		//public string EAN_CODE2 { get; set; }
		[DataMember]
		public string PackageBoxBarCode { get; set; }
    /// <summary>
    /// 物流商名稱
    /// </summary>
    [DataMember]
    public string LogisticName { get; set; }

    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get
    //	{
    //		return new Dictionary<string, string>
    //					{
    //						{"CONSIGNEE", "NAME"}
    //					};
    //	}
    //}
  }

	/// <summary>
	/// 箱明細_Pchome
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class PcHomeDeliveryReport
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string PRINT_MEMO { get; set; }
		[DataMember]
		public string PRINT_CUST_ORD_NO { get; set; }
		[DataMember]
		public DateTime? CRT_DATE { get; set; }
    [DataMember]
		public string ORDER_PROC_TYPE { get; set; }
		[DataMember]
		public string ORDER_ZIP_CODE { get; set; }
		[DataMember]
		public string IS_NORTH_ORDER { get; set; }
	}



	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class DelvdtlInfo
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string DELVDTL_FORMAT { get; set; }
		[DataMember]
		public string AUTO_PRINT_DELVDTL { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CVS_TAKE { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
  }
  #endregion

  #region 合流作業

  [Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F052902Sum
	{
		public System.Decimal? B_SET_QTY { get; set; }
		public System.Decimal? A_SET_QTY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050802Data
	{
		public System.Decimal ROWNUM { get; set; }
		public string WMS_ORD_NO { get; set; }
		public System.Decimal? B_DELV_QTY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050802ItemName
	{
		public System.Decimal ROWNUM { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string GUP_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string RET_UNIT { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050802Report
	{
		public System.Decimal ROWNUM { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public int B_DELV_QTY { get; set; }
		public string ALL_COMP { get; set; }
		public string RETAIL_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_UNIT { get; set; }
		public string ItemCodeBarcode { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051206Sum
	{
		public System.Decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string PICK_ORD_NO { get; set; }
		public System.Decimal? LACK_QTY { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F052902Data
	{
		public System.Decimal ROWNUM { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string MERGE_BOX_NO { get; set; }
		public System.Decimal? B_DELV_QTY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

	[Serializable]
	[DataServiceKey("WMS_ORD_NO", "DC_CODE", "GUP_CODE", "CUST_CODE", "ITEM_CODE")]
	public class F050802GroupItem
	{
		public string WMS_ORD_NO { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public Int32? SUM_B_SET_QTY { get; set; }
	}

	/// <summary>
	/// 合流箱：每次播種後，合流箱(對應一個出貨單)的品號與實揀與缺貨數
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO", "ITEM_CODE")]
	public class ConfluxBox
	{
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public Int32 A_SET_QTY { get; set; }
		[DataMember]
		public Int32 LackQty { get; set; }
	}
	#endregion

	#region 出貨狀況控管
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "DELV_DATE", "TAKE_TIME")]
	public class F050801StatisticsData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public System.DateTime DELV_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public System.Decimal? UNFINISHEDCOUNT { get; set; }
		[DataMember]
		public System.Decimal? FINISHEDCOUNT { get; set; }
	}
	/// <summary>
	/// 揀貨進度
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_DATE", "PICK_TIME", "PICK_TYPE", "PICK_AREA")]
	public class PickingStatistics
	{
		[DataMember]
		public System.DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_TYPE { get; set; }
		[DataMember]
		public string PICK_AREA { get; set; }
		[DataMember]
		public int FINISHEDCOUNT { get; set; }
		[DataMember]
		public int UNFINISHEDCOUNT { get; set; }
		[DataMember]
		public int TOTALCOUNT { get; set; }

		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}


	#endregion

	#region 訂單明細
	[Serializable]
	[DataContract]
	[DataServiceKey("ORD_NO", "ITEM_CODE")]
	public class F050102Ex
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string ORD_SEQ { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
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
		public int ORD_QTY { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public bool ISSELECTED { get; set; }
		[DataMember]
		public string BUNDLE_SERIALLOC { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }

		[DataMember]
		public string NO_DELV { get; set; }
		[DataMember]
		public int? B_DELV_QTY { get; set; }
		[DataMember]
		public int? A_DELV_QTY { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
	}
	#endregion

	#region Excel訂單匯入明細
	[Serializable]
	[DataContract]
	[DataServiceKey("ORD_NO", "ITEM_CODE", "ORD_SEQ")]
	public class F050102Excel
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string ORD_SEQ { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int ORD_QTY { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string NO_DELV { get; set; }
	}
	#endregion

	#region 未出貨訂單查詢
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050801NoShipOrders
	{
		public decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string STATUS { get; set; }
		public Int16? PICK_STATUS { get; set; }
		public string PICK_STAFF { get; set; }
		public string PICK_NAME { get; set; }
		public string PICK_ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string ORD_NO { get; set; }
		public DateTime? NEW_CHECKOUT_DATE { get; set; }
		public string NEW_CHECKOUT_TIME { get; set; }
		public string NEW_PIER_CODE { get; set; }
		public string PACKAGE_NAME { get; set; }
		public string ORD_TYPE { get; set; }
		public string ORD_TIME { get; set; }
	}
	#endregion

	#region 訂單主檔
	[Serializable]
	[DataContract]
	[DataServiceKey("ORD_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F050101Ex : IEncryptable
	{
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string ORD_TYPE { get; set; }
		public string RETAIL_CODE { get; set; }
		public System.DateTime? ORD_DATE { get; set; }
		public string STATUS { get; set; }
		public string CUST_NAME { get; set; }
		public string SELF_TAKE { get; set; }
		public string FRAGILE_LABEL { get; set; }
		public string GUARANTEE { get; set; }
		public string SA { get; set; }
		public string GENDER { get; set; }
		public System.Int16? AGE { get; set; }
		public System.Int16? SA_QTY { get; set; }
		public short SA_CHECK_QTY { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL { get; set; }
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		public System.DateTime ARRIVAL_DATE { get; set; }
		public string TRAN_CODE { get; set; }
		public string SP_DELV { get; set; }
		public string CUST_COST { get; set; }
		public string BATCH_NO { get; set; }
		public string CHANNEL { get; set; }
		public string POSM { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONTACT { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string CONTACT_TEL { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_2 { get; set; }
		public string SPECIAL_BUS { get; set; }
		public string ALL_ID { get; set; }
		public string COLLECT { get; set; }
		public System.Decimal? COLLECT_AMT { get; set; }
		public string MEMO { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string TYPE_ID { get; set; }
		public string CAN_FAST { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_1 { get; set; }
		[Encrypted]
		public string TEL_AREA { get; set; }
		public string PRINT_RECEIPT { get; set; }
		public string RECEIPT_NO { get; set; }
		public string RECEIPT_NO_HELP { get; set; }
		public string RECEIPT_TITLE { get; set; }
		public string RECEIPT_ADDRESS { get; set; }
		public string BUSINESS_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string DISTR_CAR_NO { get; set; }
		public string EDI_FLAG { get; set; }
		public string CVS_TAKE { get; set; }
		public string SUBCHANNEL { get; set; }
		public string ESERVICE { get; set; }

		public string CHANNEL_NAME { get; set; }
		public string DELV_NAME { get; set; }
		public string ALL_COMP { get; set; }

		public string ROUND_PIECE { get; set; }
		public string FAST_DEAL_TYPE { get; set; }
		public string SUG_BOX_NO { get; set; }
		public string MOVE_OUT_TARGET { get; set; }
		public string PACKING_TYPE { get; set; }
		public int ISPACKCHECK { get; set; }
    public string SUG_LOGISTIC_CODE { get; set; }
    public string NP_FLAG { get; set; }
    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get
    //	{
    //		return new Dictionary<string, string>
    //		{
    //			{"TEL", "TEL"},
    //			{"ADDRESS", "ADDR"},
    //			{"CONSIGNEE", "NAME"},
    //			{"CONTACT", "NAME"},
    //			{"CONTACT_TEL", "TEL"},
    //			{"TEL_AREA", "NOT" },
    //			{"TEL_1", "TEL"},
    //			{"TEL_2", "TEL"}
    //		};
    //	}
    //}
  }
	#endregion

	#region 出貨單特殊商品批次維護

	[DataContract]
	[Serializable]
	[DataServiceKey("SEQ_NO", "BEGIN_DELV_DATE", "END_DELV_DATE")]
	public class F050003Ex
	{
		[DataMember]
		public System.Int32 SEQ_NO { get; set; }
		[DataMember]
		public System.DateTime? BEGIN_DELV_DATE { get; set; }
		[DataMember]
		public System.DateTime? END_DELV_DATE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
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
		public string STATUS { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public decimal TICKET_ID { get; set; }
		[DataMember]
		public string TICKET_NAME { get; set; }
	}
	#endregion

	#region 出貨單批次參數維護
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "TICKET_ID")]
	public class F050004WithF190001
	{
		[DataMember]
		public System.Decimal TICKET_ID { get; set; }
		[DataMember]
		public System.Int32 SOUTH_PRIORITY_QTY { get; set; }
		[DataMember]
		public System.Int32 ORDER_LIMIT { get; set; }
		[DataMember]
		public System.Int32 DELV_DAY { get; set; }
		[DataMember]
		public string SPLIT_FLOOR { get; set; }
		[DataMember]
		public string MERGE_ORDER { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string TICKET_NAME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ORD_NAME { get; set; }
		[DataMember]
		public string TICKET_CLASS_NAME { get; set; }
		[DataMember]
		public string SPLIT_FLOOR_NAME { get; set; }
		[DataMember]
		public string MERGE_ORDER_NAME { get; set; }
		[DataMember]
		public string SPLIT_PICK_TYPE { get; set; }

	}
	#endregion

	#region 訂單維謢-檢視出貨明細用
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050102WithF050801
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
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
		public int? B_DELV_QTY { get; set; }
		[DataMember]
		public int? A_DELV_QTY { get; set; }
		[DataMember]
		public int? LACK_QTY { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO", "ORD_NO")]
	//[IgnoreProperties("EncryptedProperties")]
	public class P05030201BasicData : IEncryptable
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public Decimal STATUS { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
		[DataMember]
		public string SOURCE_NO { get; set; }
		[DataMember]
		public DateTime? ARRIVAL_DATE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime ORD_DATE { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_1 { get; set; }
		[DataMember]
		public string TEL_2 { get; set; }
		[DataMember]
		public int SA_QTY { get; set; }
		[DataMember]
		public string COLLECT { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string DISTR_CAR_ADDRESS { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public DateTime? APPROVE_DATE { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public DateTime? INCAR_DATE { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string LACK_DO_STATUS { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public DateTime? TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string DISTR_CAR_STATUS { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public string SUG_BOX_NO { get; set; }
		[DataMember]
		public int ISPACKCHECK { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE_NAME { get; set; }
    [DataMember]
    public string CUST_COST { get; set; }
    [DataMember]
    public string PACKING_TYPE { get; set; }
    [DataMember]
    public string SHIP_MODE { get; set; }
    [DataMember]
    public string SUG_LOGISTIC_CODE { get; set; }
    [DataMember]
    public string NP_FLAG { get; set; }
    [DataMember]
    public DateTime? UPD_DATE { get; set; }
    //[IgnoreDataMember]
    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get { return new Dictionary<string, string> { { "CONSIGNEE", "NAME" }, { "TEL", "TEL" }, { "TEL_1", "TEL_1" }, { "ADDRESS", "ADDR" } }; }
    //}
  }
	#endregion

	#region 貨主訂單手動挑單
	[Serializable]
	[DataServiceKey("ORD_NO", "CUST_CODE", "GUP_CODE")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F050001Data : IEncryptable
	{
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string ORD_TYPE { get; set; }
		public System.DateTime? ORD_DATE { get; set; }
		public string STATUS { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		public System.DateTime? ARRIVAL_DATE { get; set; }
		public string BATCH_NO { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string SOURCE_TYPE { get; set; }
		public string SOURCE_NAME { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{ "CONSIGNEE", "NAME" }
		//					};
		//	}
		//}
		public string CHANNEL { get; set; }
    public string SUBCHANNEL { get; set; }
    public string DELV_TYPE { get; set; }
		public string ALL_COMP { get; set; }
		public decimal? COLLECT_AMT { get; set; }
		public string RETAIL_CODE { get; set; }
		public string RETAIL_NAME { get; set; }
		public string CAR_PERIOD { get; set; }
		public string DELV_NO { get; set; }
		public string DELV_WAY { get; set; }
		public string CUST_COST { get; set; }
		public string FAST_DEAL_TYPE { get; set; }
		public string MOVE_OUT_TARGET { get; set; }
		public DateTime? CRT_DATE { get; set; }

	}
	#endregion

	#region 配庫
	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class SpecialItem
	{
		[DataMember]
		public Decimal TICKET_ID { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
	}
	#endregion 配庫

	#region 異動調整作業(訂單,商品,盤點庫存)
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]

	public class F050301Data : IEncryptable
	{
		public int ROWNUM { get; set; }
		public bool IsSelected { get; set; }
		public DateTime? DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string ORD_NO { get; set; }
		public string CUST_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string ORD_TYPE { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[Encrypted]
		[SecretPersonalData("ZIP")]
		public string ZIP_CODE { get; set; }
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"CONSIGNEE", "CONSIGNEE"},
		//						{"ZIP_CODE","ZIP_CODE" },
		//						{"ADDRESS","ADDRESS" }
		//					};
		//	}
		//}
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F0513Data
	{
		public int ROWNUM { get; set; }

		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }

		public string ALL_ID { get; set; }

		public string ALL_COMP { get; set; }
		public string CUST_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string DC_CODE { get; set; }
	}
	#endregion

	#region 揀貨主檔
	[Serializable]
	[DataServiceKey("ROWNUM", "PICK_AREA_NO")]
	public class F051205Data
	{
		public int ROWNUM { get; set; }
		public Int32 PICK_AREA_NO { get; set; }
		public string PICK_AREA { get; set; }
		public string FLOOR { get; set; }
		public string CHANNEL_BEGIN { get; set; }
		public string CHANNEL_END { get; set; }
		public string ISDELETED { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string DC_CODE { get; set; }
		public string UPD_STAFF { get; set; }
		public DateTime CRT_DATE { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string UCC_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
	}
	#endregion

	#region 虛擬商品出貨 P060102
	[Serializable]
	[DataServiceKey("ROW_NUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F050801VirtualItem : IEncryptable
	{
		public int ROW_NUM { get; set; }
		public string WMS_ORD_NO { get; set; }
		public System.DateTime DELV_DATE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_TYPE { get; set; }
		public string ORD_TIME { get; set; }
		public string PICK_TIME { get; set; }
		public string RETAIL_CODE { get; set; }
		public System.DateTime ORD_DATE { get; set; }
		public string PRINT_FLAG { get; set; }
		public string GUP_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
		public string PICK_ORD_NO { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public System.Decimal STATUS { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string TMPR_TYPE { get; set; }
		public string SELF_TAKE { get; set; }
		public string FRAGILE_LABEL { get; set; }
		public string GUARANTEE { get; set; }
		public string HELLO_LETTER { get; set; }
		public string SA { get; set; }
		public string CUST_NAME { get; set; }
		public System.Int16 INVOICE_PRINT_CNT { get; set; }
		public string GENDER { get; set; }
		public System.Int16? AGE { get; set; }
		public string ORD_PROP { get; set; }
		public string NO_SEAL { get; set; }
		public string NO_LOADING { get; set; }
		public System.Int16 SA_QTY { get; set; }
		public string SOURCE_TYPE { get; set; }
		public string SOURCE_NO { get; set; }
		public string NO_AUDIT { get; set; }
		public string PRINT_PASS { get; set; }
		public string PRINT_DELV { get; set; }
		public string PRINT_BOX { get; set; }
		public System.DateTime? APPROVE_DATE { get; set; }
		public System.DateTime? INCAR_DATE { get; set; }
		public System.DateTime? ARRIVAL_DATE { get; set; }
		public string VIRTUAL_ITEM { get; set; }
		public string ORD_NO { get; set; }
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		public string CUST_COST { get; set; }
		public string BATCH_NO { get; set; }
		public string CHANNEL { get; set; }
		public string TYPE_ID { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONTACT { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string CONTACT_TEL { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[Encrypted]
		public string TEL_AREA { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_1 { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_2 { get; set; }
		public string PRINT_RECEIPT { get; set; }
		public string RECEIPT_NO { get; set; }
		public string RECEIPT_TITLE { get; set; }
		public string BUSINESS_NO { get; set; }
		public string RECEIPT_ADDRESS { get; set; }
		public string MEMO { get; set; }
		public string TRAN_CODE { get; set; }
		public string SP_DELV { get; set; }

		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"TEL", "TEL"}, {"ADDRESS", "ADDR"}, {"CONSIGNEE", "NAME"}, {"CONTACT", "NAME"}, {"CONTACT_TEL", "TEL"}, {"TEL_AREA", "NOT"}, {"TEL_1", "TEL"}, {"TEL_2", "TEL"}
		//					};
		//	}
		//}
	}
	#endregion

	#region F050801 Data

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050801WmsOrdNo
	{
		[DataMember]
		public int ROWNUM { get; set; }

		[DataMember]
		public string WMS_ORD_NO { get; set; }

		[DataMember]
		public string CUST_CODE { get; set; }

		[DataMember]
		public string GUP_CODE { get; set; }

		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public int STATUS { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
	}

	#endregion

	#region F050805 總庫試算缺貨檔
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F050805Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public Decimal ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CAL_NO { get; set; }
		[DataMember]
		public string TYPE_ID { get; set; }
		[DataMember]
		public string TYPE_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public decimal? TTL_PICK_STOCK_QTY { get; set; }
		[DataMember]
		public decimal? TTL_RESUPPLY_STOCK_QTY { get; set; }
		[DataMember]
		public decimal? TTL_STOCK_QTY { get; set; }
		[DataMember]
		public decimal? TTL_VIRTUAL_STOCK_QTY { get; set; }
		[DataMember]
		public decimal? TTL_ORD_QTY { get; set; }
		[DataMember]
		public decimal? TTL_OUTSTOCK_QTY { get; set; }
		[DataMember]
		public decimal? SUG_RESUPPLY_STOCK_QTY { get; set; }
		[DataMember]
		public decimal? SUG_VIRTUAL_STOCK_QTY { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
  }

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F05080501Data
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
		public string CAL_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string RESULT_CODE { get; set; }
		[DataMember]
		public string RESULT_NAME { get; set; }

	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F05080502Data
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
		public string CAL_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string ORD_SEQ { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public decimal? ORD_QTY { get; set; }
		[DataMember]
		public decimal? ALLOT_QTY { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
	}
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F05080504Data
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
		public string CAL_NO { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public decimal DELV_QTY { get; set; }
		[DataMember]
		public decimal DELV_RATIO { get; set; }
	}
	#endregion

	#region 出貨單明細給換貨單維護用
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F050802FOR160301
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
		public bool ISSELECTED { get; set; }
		[DataMember]
		public int RTN_QTY { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string ITEM_SEQ { get; set; }

	}
	#endregion

	#region 商品檢驗用
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class F05500101Data
	{
		public int ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }

		public string WMS_ORD_NO { get; set; }

		public string ITEM_CODE { get; set; }

		public DateTime VALID_DATE { get; set; }
	}
	#endregion

	#region 出貨單 WMS_ORD_NO 對應 050301 相關單據資料
	[Serializable]
	[DataServiceKey("ORD_NO")]
	public class F050301WmsOrdNoData
	{
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public int STATUS { get; set; }
		public string SOURCE_NO { get; set; }
		public string SOURCE_TYPE { get; set; }
	}
	#endregion

	#region 託運單

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F055001Data : IEncryptable
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
		public string PAST_NO { get; set; } //託運單號
		[DataMember]
		public string DELV_DATE { get; set; }
		[DataMember]
		public string ARRIVAL_DATE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string COLLECT { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public Decimal COLLECT_AMT { get; set; }
		[DataMember]
		public Decimal SA_QTY { get; set; }
		[DataMember]
		public string ERST_NO { get; set; }
		[DataMember]
		public string SHORT_NAME { get; set; }
		[DataMember]
		public string CUST_TEL { get; set; }
		[DataMember]
		public string CUST_ADDRESS { get; set; }
		[DataMember]
		public Decimal TOTAL_AMOUNT { get; set; }
		[DataMember]
		public string ROUTE { get; set; }
		[DataMember]
		public string FIXED_CODE { get; set; }
		[DataMember]
		public string ADDRESS_TYPE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string RETAIL_NAME { get; set; } //門市名稱
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string PRINT_TIME { get; set; }
		[DataMember]
		public string CONSIGN_ID { get; set; }
		[DataMember]
		public string CONSIGN_NAME { get; set; }
		[DataMember]
		public string CONSIGN_MEMO { get; set; }
		[DataMember]
		public string VERSION_DATA { get; set; } //EGS版本日期
		[DataMember]
		public string VERSION_NUMBER { get; set; } //EGS版本號碼
		[DataMember]
		public string EGS_SUDA5 { get; set; } //5碼郵遞區號
		[DataMember]
		public string EGS_BASE { get; set; } //五碼郵遞區號對應的轉運站代號
		[DataMember]
		public string EGS_SUDA7 { get; set; } //速達7 碼條碼資料(+符號+轉運站 + 速達5 碼)
		[DataMember]
		public string EGS_SUDA7_DASH { get; set; } //速達7 碼資料(轉運站及速達5 碼有dash 分隔)
		[DataMember]
		public string EGS_VOLUME { get; set; } //包材尺寸
		[DataMember]
		public string CUST_ZIP { get; set; } //物流中心郵遞區號
		[DataMember]
		public string RETAIL_DELV_DATE { get; set; } //門市進貨日
		[DataMember]
		public string RETAIL_RETURN_DATE { get; set; }//門市退貨日
		[DataMember]
		public string CHANNEL { get; set; } //通路類型
		[DataMember]
		public string ALL_ID { get; set; } //配送商編號
		[DataMember]
		public string ORD_NO { get; set; } //訂單
		[DataMember]
		public string TCAT_DELV_DATE { get; set; }//速達預計配達日
		[DataMember]
		public string TCAT_ARRIVAL_DATE { get; set; }//速達預計到貨日期
		[DataMember]
		public string TCAT_MEMO { get; set; } //速達備註
		[DataMember]
		public string TCAT_PLACE { get; set; }//速達發貨所
		[DataMember]
		public string TCAT_SIZE { get; set; } //速達尺寸
		[DataMember]
		public string TCAT_TIME { get; set; } //速達指定時段
		[DataMember]
		public string CHANNEL_NAME { get; set; } //通路名稱
		[DataMember]
		public string CHANNEL_ADDRESS { get; set; }//通路地址
		[DataMember]
		public string CHANNEL_TEL { get; set; }//通路電話
		[DataMember]
		public string PAST_NOByCode128 { get; set; }
		[DataMember]
		public string PAST_NOBarCode { get; set; } //給託運單號條碼字型使用
		[DataMember]
		public string PAST_NOBarCodeShow { get; set; } //給託運單號條碼顯示使用
		[DataMember]
		public string CUSTOMER_ID { get; set; } //契客代號
		[DataMember]
		public string ESERVICE { get; set; } //超取服務商編號
		[DataMember]
		public string ESERVICE_NAME { get; set; }//超取服務商名稱
		[DataMember]
		public string ESHOP { get; set; } //母廠商編號
		[DataMember]
		public string ESHOP_ID { get; set; } //子廠商編號
		[DataMember]
		public string PLATFORM_NAME { get; set; } //平台名稱

		[DataMember]
		public string LAB_VNR_NAME { get; set; } //廠商名稱標籤
		[DataMember]
		public string VNR_NAME { get; set; } //廠商名稱
		[DataMember]
		public string CUST_INFO { get; set; } //客服資訊
		[DataMember]
		public string LAB_CUST_INFO { get; set; } //客服資訊
		[DataMember]
		public string LAB_NOTE1 { get; set; } //出貨標籤1
		[DataMember]
		public string LAB_NOTE2 { get; set; } //出貨標籤2
		[DataMember]
		public string LAB_NOTE3 { get; set; } //出貨標籤3
		[DataMember]
		public string NOTE1 { get; set; } //出貨標籤說明1
		[DataMember]
		public string NOTE2 { get; set; } //出貨標籤說明2
		[DataMember]
		public string NOTE3 { get; set; } //出貨標籤說明3
		[DataMember]
		public string SHOW_ISPAID_NOTE { get; set; } //未付款/已付款說明
		[DataMember]
		public string INVOICE { get; set; } //發票號碼
		[DataMember]
		public string INVOICE_DATE { get; set; } //發票日期
		[DataMember]
		public string IDENTIFIER { get; set; } //個人驗證碼
		[DataMember]
		public string BARCODE_TYPE { get; set; } //條碼類型
		[DataMember]
		public string ISPRINTSTAR { get; set; }//是否加印條碼前後星號
		[DataMember]
		public string CONCENTRATED_NO { get; set; } //集包地編號
		[DataMember]
		public string CONCENTRATED { get; set; } //集包地名稱
		[DataMember]
		public string SHIPPING_AREA_NO { get; set; } //到貨區碼
		[DataMember]
		public string CONCENTRATED_NOByCode128 { get; set; } //集包地編號Code128
		[DataMember]
		public string SHIPPINGCITY { get; set; } //收件人地址省市
		[DataMember]
		public string ORD_NOByCode128 { get; set; }//訂單編號Code128
		[DataMember]
		public string SELLER_NAME { get; set; }//賣家名稱
		[DataMember]
		public string SHIPPING_FLAG { get; set; }//出貨模式 0: 虛出 1: 實出(物流中心需揀貨包裝)
		[DataMember]
		public string PACK_WEIGHT { get; set; }//包裹重量(毛重)
		[DataMember]
		public string PACK_INSURANCE { get; set; }//保價金額

		[DataMember]
		public string HCT_STATION { get; set; }  //新竹貨運到著站簡碼
		[DataMember]
		public string PIECES { get; set; } //件數
		[DataMember]
		public short PACKAGE_BOX_NO { get; set; } //第幾箱

		/// <summary>
		/// HCT 新竹貨運 客戶代號
		/// </summary>
		[DataMember]
		public string LOGCENTER_ID { get; set; }

		/// <summary>
		/// 大榮貨運到著站簡碼
		/// </summary>
		[DataMember]
		public string KTJ_STATION { get; set; }  //大榮貨運到著站簡碼

		/// <summary>
		/// 大榮貨運發送站簡碼
		/// </summary>
		[DataMember]
		public string KTJ_STATION_S { get; set; }  //大榮貨運發送站簡碼

		/// <summary>
		/// 大榮貨運到著站名稱
		/// </summary>
		[DataMember]
		public string KTJ_STATION_NAME { get; set; }  //大榮貨運到著站簡碼

		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//		{
		//			{"CONSIGNEE", "NAME"},{"ADDRESS", "ADDR"},{"TEL", "TEL"}
		//		};
		//	}
		//}
	}

	#endregion

	#region 
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F055002Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public decimal PACKAGE_QTY { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
	}
	#endregion

	#region 送貨單
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class DeliveryNoteData : IEncryptable
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string TEXT01 { get; set; }//配次
		[DataMember]
		public string TEXT02 { get; set; }//配送路線
		[DataMember]
		public string CHANNEL { get; set; }//出貨通路(F050301) 
		[DataMember]
		public string RETAIL_CODE { get; set; }//客戶代號(F050301) 
		[DataMember]
		public string CUST_NAME { get; set; }//客戶名稱(F050301) 
		[DataMember]
		public string INVO_ADDRESS { get; set; }//發票地址(F1909)
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }//送貨地址(F050301)
		[DataMember]
		public string SHORT_NAME { get; set; }//簡稱(F1909)
		[DataMember]
		public string UNI_FORM { get; set; }//客戶統編(F1909)
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string CONTACT_TEL { get; set; }//連絡電話(F050301) 
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONTACT { get; set; }//連絡人(F050301) 
		[DataMember]
		public DateTime DELV_DATE { get; set; }//出貨日期(F050801) 
		[DataMember]
		public string PICK_ORD_NO { get; set; }//揀貨單號(F050801) 
		[DataMember]
		public string PAST_NO { get; set; }//託運單號(F055001) 
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_1 { get; set; }//行動電話(F050301) 
		[DataMember]
		public string PAY_TYPE { get; set; }//付款方式代碼(F000904)
		[DataMember]
		public string PAY_TYPE_TEXT { get; set; }//付款方式文字(F000904)
		[DataMember]
		public string SOURCE_NO { get; set; }//來源單號(F050301) 
		[DataMember]
		public string SOURCE_TYPE { get; set; }//來源單據(F000902)
		[DataMember]
		public string STOCK_NO { get; set; }//進倉單號(F010201) 
		[DataMember]
		public int TOTAL_QTY { get; set; }//出貨總數量(SUM(F050802.B_DELV_QTY)) 
		[DataMember]
		public string CUST_ORD_NO { get; set; }//貨主單號(F050301) 
		[DataMember]
		public string SHOP_NO { get; set; }//客戶採購單號(F050301) 
		[DataMember]
		public string RECEIPT_NO { get; set; }//指定發票號碼(F050301) 
		[DataMember]
		public string RECEIPT_NO_HELP { get; set; }//代產生發票號碼(F050301) 
		[DataMember]
		public string WMS_ORD_NO { get; set; }//出貨單號(F050801) 

		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//		{
		//			{"ADDRESS", "ADDR"}, {"CONTACT_TEL", "TEL"}, {"CONTACT", "NAME"}, {"TEL_1", "TEL"}
		//		};
		//	}
		//}
	}


	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class DeliveryNoteSubData
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }//出貨單號(F050801) 
		[DataMember]
		public string ITEM_CODE { get; set; }//商品編號(F050802) 
		[DataMember]
		public string ITEM_NAME { get; set; }//品名(F1903) 
		[DataMember]
		public int B_DELV_QTY { get; set; }//數量(F050802) 
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class DeliveryNoteSubDataA
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }//貨主單號(F050301) 
		[DataMember]
		public string SHOP_NO { get; set; }//客戶採購單號(F050301) 
		[DataMember]
		public string WMS_ORD_NO { get; set; }//出貨單號(F050801) 

	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class DeliveryNoteSubDataB
	{
		[DataMember]
		public int ROWNUM { get; set; }
		public string RECEIPT_NO { get; set; }//指定發票號碼(F050301) 
		[DataMember]
		public string RECEIPT_NO_HELP { get; set; }//代產生發票號碼(F050301) 
		[DataMember]
		public string RECEIPT_NO_TEXT { get; set; }//發票號碼文字(F050301)，RECEIPT_NO 或 RECEIPT_NO_HELP 
		[DataMember]
		public string WMS_ORD_NO { get; set; }//出貨單號(F050801) 
	}
	#endregion

	#region 出貨發票報表主檔

	[DataContract]
	[Serializable]
	//[IgnoreProperties("EncryptedProperties")]
	[DataServiceKey("ROWNUM", "WMS_ORD_NO")]
	public class F050801MainDataRpt : IEncryptable
	{
		[DataMember]
		public decimal PageNum { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string RECEIPT_NO { get; set; }
		[DataMember]
		public string RECEIPT_NO_HELP { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string UNIFORM { get; set; }
		[DataMember]
		public string INVO_PRINTED { get; set; }
		[DataMember]
		public string PRINT_RECEIPT { get; set; }
		[DataMember]
		public string ClientIP { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[DataMember]
		public string ValidNo { get; set; }
		[DataMember]
		public int INVOICE_PRINT_CNT { get; set; }
		[DataMember]
		public string ROUTE_CODE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string INVO_TAX_TYPE { get; set; }
		[DataMember]
		public string TaxtCredit { get; set; }
		[DataMember]
		public string TaxtZero { get; set; }
		[DataMember]
		public string TaxFree { get; set; }
		[DataMember]
		public decimal TAX { get; set; }
		[DataMember]
		public decimal TOTAL_AMT { get; set; }
		[DataMember]
		public decimal SALES_PRICE { get; set; }
		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//		{
		//			{"ADDRESS", "ADDR"}
		//		};
		//	}
		//}
	}

	#endregion

	#region 出貨發票報表明細檔

	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	public class F050801DetailDataRpt
	{
		[DataMember]
		public int PageNum { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public int ORD_QTY { get; set; }
		[DataMember]
		public Decimal PRICE { get; set; }
		[DataMember]
		public Decimal AMOUNT { get; set; }


	}

	#endregion

	#region Welcome Letter
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F05000201Data
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string HELLO_LETTER_PRINTED { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string EFFECT_DATE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
	}
	#endregion

	#region 取050801資料給新增 F1101 (借出/外送) 用
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	public class F050801DataForF1101
	{

		public string WMS_ORD_NO { get; set; }
		public string ORD_NO { get; set; }
		public string SOURCE_NO { get; set; }
		public string SOURCE_TYPE { get; set; }
		public string PICK_LOC { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string VNR_CODE { get; set; }
		public string SERIAL_NO { get; set; }
		public int A_PICK_QTY { get; set; }
		public string STATUS { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

		public string PALLET_CTRL_NO { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string MAKE_NO { get; set; }


	}
	#endregion

	#region F050801 出貨單 Item 明細
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	public class F050801ItemData
	{

		public string WMS_ORD_NO { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_SIZE { get; set; }
		public int ORD_QTY { get; set; }
		public string BUNDLE_SERIALLOC { get; set; }

	}
	#endregion

	#region F051202VolumnData 揀貨明細更新儲位容積量
	[Serializable]
	[DataServiceKey("PICK_ORD_NO", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F051202VolumnData
	{
		public string PICK_ORD_NO { get; set; }
		public string PICK_LOC { get; set; }
		public string ITEM_CODE { get; set; }
		public decimal B_PICK_QTY { get; set; }
		public decimal A_PICK_QTY { get; set; }
		public string PICK_STATUS { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}
	#endregion

	#region 出貨包裝、缺貨包裝共用
	[DataContract]
	[Serializable]
	[DataServiceKey("IsSuccessed")]
	public class ScanPackageCodeResult
	{
		[DataMember]
		public string SerialNo { get; set; }
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public string FRAGILE { get; set; }
		[DataMember]
		public string SPILL { get; set; }
		[DataMember]
		public bool IsCarton { get; set; }
		[DataMember]
		public bool IsInsertLog { get; set; }
		[DataMember]
		public bool IsPass { get; set; }
		[DataMember]
		public string Status { get; set; }
		/// <summary>
		/// 商品類別
		/// </summary>
		[DataMember]
		public string Type { get; set; }
		/// <summary>
		/// 是否關閉目前的箱子
		/// </summary>
		[DataMember]
		public bool IsFinishCurrentBox { get; set; }
		/// <summary>
		/// 記憶該次紙箱，只有NEWBOX後，才會清除該資訊
		/// </summary>
		[DataMember]
		public string BoxNum { get; set; }
    [DataMember]
    public string OrgWmsNo { get; set; }
    [DataMember]
    public string ScanCode { get; set; }

    public ScanPackageCodeResult()
		{
			IsInsertLog = true;
		}

		public ScanPackageCodeResult(bool isPass, string message)
			: this()
		{
			IsPass = isPass;
			Message = message;
		}

    public ScanPackageCodeResult(bool isPass, string message, string serialNo)
      : this()
    {
      IsPass = isPass;
      Message = message;
      SerialNo = serialNo;
    }
  }


	#endregion

	#region 排程 - 每日補貨調撥
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE")]
	public class SchReplenishStock
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
		public int OUTSTOCK_QTY { get; set; }
		[DataMember]
		public int QTY { get; set; }
		[DataMember]
		public int UNIT_LEVEL { get; set; }
		[DataMember]
		public int RESULT_QTY { get; set; }
		[DataMember]
		public int REPLENSIH_QTY { get; set; }
		[DataMember]
		public long PICK_SAVE_QTY { get; set; }
		[DataMember]
		public int PICK_SAVE_ORD { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }

	}
	#endregion

	#region 退貨單維護 查詢原出貨單
	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	//[IgnoreProperties("EncryptedProperties")]
	public class CustomerData : IEncryptable
	{
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONTACT { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }

		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get { return new Dictionary<string, string> { { "ADDRESS", "ADDR" }, { "CONTACT", "NAME" }, { "CONTACT_TEL", "TEL" } }; }
		//}
	}
	#endregion

	#region 排程 - B2C 出貨回檔
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE")]
	public class SchB2CDeliveryReturn
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string INVOICE_NO { get; set; }
		[DataMember]
		public DateTime? INVOICE_DATE { get; set; }
		[DataMember]
		public decimal? INVOICE_AMT { get; set; }
		[DataMember]
		public string SHIPPING_NAME { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public int? ORD_QTY { get; set; }
		[DataMember]
		public int? A_DELV_QTY { get; set; }
		[DataMember]
		public string HAVE_ITEM_INVO { get; set; }
		[DataMember]
		public string PROC_FLAG { get; set; }

	}
	#endregion

	#region 排程 - 訂單狀態回檔
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE")]
	public class SchOrderStatusReturn
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string STATUS_CODE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CUST_EDI_STATUS { get; set; }
		[DataMember]
		public string UPDATE_DTM { get; set; }
		[DataMember]
		public DateTime? PAST_DATE { get; set; }
		[DataMember]
		public DateTime? SEND_DATE { get; set; }
		[DataMember]
		public int WMS_ORD_STATUS { get; set; }
	}
	#endregion

	#region 排程-Apple 商品出貨序號
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class SchAppleItemDelivery
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string SHORT_NAME { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string DELV_DATE_TEXT { get; set; }
	}
	#endregion

	#region 排程-HTC 托運回檔
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE")]
	//[IgnoreProperties("EncryptedProperties")]
	public class SchHCTConsignReturn : IEncryptable
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string BOM_NO { get; set; }
		[DataMember]
		public string MATERIAL_CODE { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string TEL_1 { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[DataMember]
		public decimal? COLLECT_AMT { get; set; }
		[DataMember]
		public string ERST_NO { get; set; }
		[DataMember]
		public string DELV_TIMES { get; set; }
		[DataMember]
		public string DC_TEL { get; set; }
		[DataMember]
		public string DC_ADDRESS { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public int SA_QTY { get; set; }
		[DataMember]
		public string COLLECT { get; set; }
		[DataMember]
		public DateTime ORD_DATE { get; set; }
		[DataMember]
		public string LOGCENTER_ID { get; set; }
		[DataMember]
		public string LOGCENTER_NAME { get; set; }
		[DataMember]
		public int? CASE_COUNT { get; set; }
		[DataMember]
		public int? ORD_COUNT { get; set; }
		[DataMember]
		public int WMS_ORD_STATUS { get; set; }


		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get { return new Dictionary<string, string> { { "CONSIGNEE", "NAME" }, { "TEL_1", "TEL" }, { "ADDRESS", "ADDR" } }; }
		//}
	}
	#endregion

	#region 排程-出貨序號回傳 Card or 3C
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE")]
	//[IgnoreProperties("EncryptedProperties")]
	public class SchDeliveryReturn : IEncryptable
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SOURCE { get; set; }
		[DataMember]
		public string BUNDLE_SERIALLOC { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public string RETURN_GUP { get; set; }
		[DataMember]
		public string RETURN_CUST { get; set; }
		[DataMember]
		public string RETURN_MAIL { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string PUK { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public int? SERIALNO_DIGIT { get; set; }
		[DataMember]
		public string STATUS { get; set; }

		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get { return new Dictionary<string, string> { { "CONSIGNEE", "NAME" } }; }
		//}


	}
	#endregion



	#region P050303出貨查詢結果
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050303QueryItem
	{
		public Decimal ROWNUM { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public Decimal STATUS { get; set; }
		public Int16? PICK_STATUS { get; set; }
		public string DELV_STATUS { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
    public string SUG_LOGISTIC_CODE { get; set; }
  }
	#endregion

	#region Schedule
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F055001QueryItem
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public string PROCESS_DATE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CLIENT_PC { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
	}


	#endregion

	#region Schedule Check - 撿貨時間過長
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ExceedPickFinishTime
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime CREATE_DATE { get; set; }
		[DataMember]
		public string PICKFINISHTIME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
	}
	#endregion


	#region F050801 - 出貨拆單資料
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_ORD_NO", "ORD_NO")]
	public class F050801WithBill
	{
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DC_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string ORD_NO { get; set; }
	}
	#endregion

	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class WmsDistrCarItem : IEncryptable
	{
		public bool ISSELECTED { get; set; }
		public decimal ROWNUM { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string PICK_TIME { get; set; }
		public string WMS_ORD_NO { get; set; }

		public string DISTR_CAR_NO { get; set; }

		public DateTime? TAKE_DATE { get; set; }
		public string TAKE_TIME { get; set; }
		public string DELV_EFFIC { get; set; }
		public string DELV_EFFIC_NAME { get; set; }

		public string DELV_TMPR { get; set; }
		public string DELV_TMPR_NAME { get; set; }

		public string SA { get; set; }

		public string RETAIL_CODE { get; set; }

		public string CUST_NAME { get; set; }
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string CONTACT { get; set; }
		[Encrypted]
		[SecretPersonalData("TEL")]
		public string CONTACT_TEL { get; set; }

		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

		public decimal STATUS { get; set; }
		public string CONSIGN_REPORT { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"ADDRESS", "ADDR"}, {"CONTACT", "NAME"}, {"CONTACT_TEL", "TEL"}
		//					};
		//	}
		//}

	}

	#region 申請書配送檢核-報表
	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	public class F050801SaDataReport
	{
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public int SA_QTY { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
		[DataMember]
		public int SA_CHECK_QTY { get; set; }

	}
	#endregion

	#region F700101Data - 批次出車時段
	[Serializable]
	[DataContract]
	[DataServiceKey("TAKE_DATE")]
	public class F700101CarData
	{
		[DataMember]
		public DateTime TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string EFFIC_NAME { get; set; }


	}
	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey("ITEM_CODE")]
	public class F055002WithF2501
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public Int16 PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string BOX_SERIAL { get; set; }
		[DataMember]
		public string CASE_NO { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("CRT_NAME")]
	public class F055002WithGridLog
	{
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CLIENT_PC { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
	}

	public class F05030201Ex
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }
		public string ORD_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public string BOM_ITEM_CODE { get; set; }
		public int BOM_QTY { get; set; }
		public int ORD_QTY { get; set; }
		public string SERIAL_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
	}

	#region 重新取得託運單號 --取得查詢條件
	public class F050304Ex
	{
		public string CONSIGN_NO { get; set; }
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string ALL_ID { get; set; }
	}
	#endregion
	#region 存取訂單資料多加EService
	[DataContract]
	[Serializable]
	[DataServiceKey("ORD_NO")]
	public class F050304AddEService
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string DELV_RETAILCODE { get; set; }
		[DataMember]
		public string DELV_RETAILNAME { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public DateTime? DELV_DATE { get; set; }
		[DataMember]
		public DateTime? RETURN_DATE { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ESERVICE { get; set; }
		[DataMember]
		public string ORD_NO1 { get; set; }
		[DataMember]
		public string CUST_ORD_NO1 { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public DateTime? ORD_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string SELF_TAKE { get; set; }
		[DataMember]
		public string FRAGILE_LABEL { get; set; }
		[DataMember]
		public string GUARANTEE { get; set; }
		[DataMember]
		public string SA { get; set; }
		[DataMember]
		public string GENDER { get; set; }
		[DataMember]
		public Int16? AGE { get; set; }
		[DataMember]
		public Int16? SA_QTY { get; set; }
		[DataMember]
		public string TEL { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string CONSIGNEE { get; set; }
		[DataMember]
		public DateTime ARRIVAL_DATE { get; set; }
		[DataMember]
		public string TRAN_CODE { get; set; }
		[DataMember]
		public string SP_DELV { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string BATCH_NO1 { get; set; }
		[DataMember]
		public string CHANNEL { get; set; }
		[DataMember]
		public string POSM { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		public string TEL_2 { get; set; }
		[DataMember]
		public string SPECIAL_BUS { get; set; }
		[DataMember]
		public string ALL_ID1 { get; set; }
		[DataMember]
		public string COLLECT { get; set; }
		[DataMember]
		public Decimal? COLLECT_AMT { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string GUP_CODE1 { get; set; }
		[DataMember]
		public string CUST_CODE1 { get; set; }
		[DataMember]
		public string DC_CODE1 { get; set; }
		[DataMember]
		public string CRT_STAFF1 { get; set; }
		[DataMember]
		public DateTime CRT_DATE1 { get; set; }
		[DataMember]
		public string UPD_STAFF1 { get; set; }
		[DataMember]
		public DateTime? UPD_DATE1 { get; set; }
		[DataMember]
		public string CRT_NAME1 { get; set; }
		[DataMember]
		public string UPD_NAME1 { get; set; }
		[DataMember]
		public string TYPE_ID { get; set; }
		[DataMember]
		public string CAN_FAST { get; set; }
		[DataMember]
		public string TEL_1 { get; set; }
		[DataMember]
		public string TEL_AREA { get; set; }
		[DataMember]
		public string PRINT_RECEIPT { get; set; }
		[DataMember]
		public string RECEIPT_NO { get; set; }
		[DataMember]
		public string RECEIPT_NO_HELP { get; set; }
		[DataMember]
		public string RECEIPT_TITLE { get; set; }
		[DataMember]
		public string RECEIPT_ADDRESS { get; set; }
		[DataMember]
		public string BUSINESS_NO { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public string HAVE_ITEM_INVO { get; set; }
		[DataMember]
		public string NP_FLAG { get; set; }
		[DataMember]
		public string EXTENSION_A { get; set; }
		[DataMember]
		public string EXTENSION_B { get; set; }
		[DataMember]
		public string EXTENSION_C { get; set; }
		[DataMember]
		public string EXTENSION_D { get; set; }
		[DataMember]
		public string EXTENSION_E { get; set; }
		[DataMember]
		public Int16 SA_CHECK_QTY { get; set; }
		[DataMember]
		public string DELV_PERIOD { get; set; }
		[DataMember]
		public string CVS_TAKE { get; set; }
		[DataMember]
		public string SUBCHANNEL { get; set; }
		[DataMember]
		public string CHECK_CODE { get; set; }
	}
	#endregion

	public class CheckWmsStatusByOrder
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string WMS_STATUS { get; set; }
		public string FOREIGN_WMSNO { get; set; }
		public string ORD_STATUS { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class SaleOrderReport : IEncryptable
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 銷售單別(銷售單號前三碼)
		/// </summary>
		public string SALEGROUP_NO { get; set; }

		[DataMember]
		/// <summary>
		/// 銷售單別名稱
		/// </summary>
		public string SALEGROUP_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 銷售單號(去掉銷售單別和-)
		/// </summary>
		public string SALE_NO { get; set; }
		[DataMember]
		/// <summary>
		/// 客戶代號
		/// </summary>
		public string RETAIL_CODE { get; set; }
		[DataMember]
		/// <summary>
		/// 單據日期(要轉換成民國年)
		/// </summary>
		public string ORD_DATE { get; set; }
		[DataMember]
		/// <summary>
		/// 廠別編號
		/// </summary>
		public string FACTORY_NO { get; set; }
		[DataMember]
		/// <summary>
		/// 廠別名稱
		/// </summary>
		public string FACTORY_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 部門編號
		/// </summary>
		public string DEPART_NO { get; set; }
		[DataMember]
		/// <summary>
		/// 部門名稱
		/// </summary>
		public string DEPART_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 客戶全名
		/// </summary>
		public string CUST_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 預收款日
		/// </summary>
		public string PREPAID_COLLECT_DATE { get; set; }
		[DataMember]
		/// <summary>
		/// 統一編號
		/// </summary>
		public string UNI_FORM { get; set; }
		[DataMember]
		/// <summary>
		/// 業務員
		/// </summary>
		public string SALESMAN { get; set; }
		[DataMember]
		/// <summary>
		/// 客戶簡稱
		/// </summary>
		public string CUST_SHORT_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 發票日期(轉民國年)
		/// </summary>
		public string INVOICE_DATE { get; set; }
		[DataMember]
		/// <summary>
		/// 發票號碼
		/// </summary>
		public string INVOICE { get; set; }
		[DataMember]
		/// <summary>
		/// 發票聯數
		/// </summary>
		public string INVOICE_DESC { get; set; }
		[DataMember]
		/// <summary>
		/// 客戶電話
		/// </summary>
		public string CUST_TEL { get; set; }
		[DataMember]
		/// <summary>
		/// 客戶傳真
		/// </summary>
		public string CUST_FAX { get; set; }
		[DataMember]
		/// <summary>
		/// 製表人
		/// </summary>
		public string CRT_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 備註
		/// </summary>
		public string MEMO { get; set; }
		[DataMember]
		/// <summary>
		/// 送貨地址
		/// </summary>
		[Encrypted]
		[SecretPersonalData("ADDR")]
		public string ADDRESS { get; set; }
		[DataMember]
		/// <summary>
		/// 銷貨金額(未稅)
		/// </summary>
		public decimal AMOUNT { get; set; }

		[DataMember]
		/// <summary>
		/// 銷貨稅額
		/// </summary>
		public decimal TAX_AMOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 銷貨金額(含稅)
		/// </summary>
		public decimal TOTAL_AMOUNT { get; set; }


		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string>
		//					{
		//						{"ADDRESS", "ADDR"}
		//					};
		//	}
		//}
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class SaleOrderDetailReport
	{
		[DataMember]
		public int ROWNUM { get; set; }

		[DataMember]
		/// <summary>
		/// 銷售單號(去掉銷售單別和-)
		/// </summary>
		public string SALE_NO { get; set; }

		[DataMember]
		/// <summary>
		/// 序號(四碼前面補0)
		/// </summary>
		public string SEQ { get; set; }
		[DataMember]
		/// <summary>
		/// 品號
		/// </summary>
		public string ITEM_CODE { get; set; }
		[DataMember]
		/// <summary>
		/// 品名
		/// </summary>
		public string ITEM_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 單位
		/// </summary>
		public string ITEM_UNIT { get; set; }
		[DataMember]
		/// <summary>
		/// 數量
		/// </summary>
		public int QTY { get; set; }
		[DataMember]
		/// <summary>
		/// 單價
		/// </summary>
		public decimal UNIT_PRICE { get; set; }
		[DataMember]
		/// <summary>
		/// 銷貨金額(未稅)
		/// </summary>
		public decimal AMOUNT { get; set; }
		[DataMember]
		/// <summary>
		/// 材積單位
		/// </summary>
		public string VOLUME_UNIT { get; set; }
		[DataMember]
		/// <summary>
		/// 分錄備註
		/// </summary>
		public string ITEM_DESC { get; set; }
		[DataMember]
		/// <summary>
		/// 贈品數
		/// </summary>
		public int GIFT_QTY { get; set; }
		[DataMember]
		/// <summary>
		/// 整箱數
		/// </summary>
		public int FULL_BOX_QTY { get; set; }
		[DataMember]
		/// <summary>
		/// 零散箱數
		/// </summary>
		public int BULK_BOX_QTY { get; set; }
		[DataMember]
		/// <summary>
		/// 重量
		/// </summary>
		public decimal WEIGHT { get; set; }
		[DataMember]
		/// <summary>
		/// 材積
		/// </summary>
		public decimal VOLUME { get; set; }

	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class LackInfo
	{
		public int ROWNUM { get; set; }
		[DataMember]

		public string WMS_ORD_NO { get; set; }
		[DataMember]

		public string PICK_ORD_NO { get; set; }
		[DataMember]

		public string PICK_ORD_SEQ { get; set; }
		[DataMember]

		public string CUST_ORD_NO { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ORD_NO { get; set; }
		[DataMember]

		public string PICK_LOC { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class UploadDelvCheckCode
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 託運單號
		/// </summary>
		public string CONSIGN_NO { get; set; }
		[DataMember]
		/// <summary>
		/// 出貨驗證碼(ex: 報關單號)
		/// </summary>
		public string DELV_CHECKCODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class CarPeriodDelvNo
	{
		[DataMember]

		public decimal ROWNUM { get; set; }
		[DataMember]

		public string CAR_PERIOD { get; set; }
		[DataMember]

		public string DELV_NO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P080902Retail
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 門市編號
		/// </summary>
		public string RETAIL_CODE { get; set; }
		[DataMember]
		/// <summary>
		/// 門市名稱
		/// </summary>
		public string RETAIL_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 此門市最小出貨狀態代碼
		/// </summary>
		public decimal STATUS { get; set; }
		[DataMember]
		/// <summary>
		/// 此門市最小出貨狀態名稱
		/// </summary>
		public string STATUS_NAME { get; set; }

	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P080902Ship
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 門市編號
		/// </summary>
		public string RETAIL_CODE { get; set; }
		[DataMember]
		/// <summary>
		/// 門市名稱
		/// </summary>
		public string RETAIL_NAME { get; set; }
		[DataMember]
		/// <summary>
		/// 總件數(出貨包裝總箱數)
		/// </summary>
		public decimal PACKAGEBOX_QTY { get; set; }
		/// <summary>
		/// 出貨明細
		/// </summary>
		[DataMember]
		public List<P080902ShipDetail> Details { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P080902ShipDetail
	{
		[DataMember]

		public decimal ROWNUM { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ITEM_NAME { get; set; }
		[DataMember]

		public decimal B_DELV_QTY { get; set; }
		[DataMember]

		public decimal A_DELV_QTY { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P080902CheckRetail
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		/// <summary>
		/// 門市編號
		/// </summary>
		public string RETAIL_CODE { get; set; }
		[DataMember]
		/// <summary>
		/// 車次
		/// </summary>
		public string DELV_NO { get; set; }
		[DataMember]
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }


	}

	public class BatchDelv
	{
		/// <summary>
		/// 物流中心
		/// </summary>
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主
		/// </summary>
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主
		/// </summary>
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 揀貨時段
		/// </summary>
		public string PICK_TIME { get; set; }

		/// <summary>
		/// 指定出貨單(null代表此批次全部出貨單扣帳)
		/// </summary>
		public List<F050801> WmsOrders { get; set; }
	}

	#region S51結算運費
	public class DelvNoItem
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
		/// 車次/路線
		/// </summary>

		public string DELV_NO { get; set; }

		/// <summary>
		/// 加價費
		/// </summary>
		public decimal EXTRA_FEE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>

		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 商品長度
		/// </summary>
		public decimal PACK_LENGTH { get; set; }
		/// <summary>
		/// 商品寬度
		/// </summary>
		public decimal PACK_WIDTH { get; set; }
		/// <summary>
		/// 商品高度
		/// </summary>
		public decimal PACK_HIGHT { get; set; }
		/// <summary>
		/// 商品重量
		/// </summary>
		public decimal PACK_WEIGHT { get; set; }
		/// <summary>
		/// 商品數量
		/// </summary>
		public decimal QTY { get; set; }
		/// <summary>
		/// 區域加價
		/// </summary>
		public decimal REGION_FEE { get; set; }
		/// <summary>
		/// 油資補貼
		/// </summary>
		public decimal OIL_FEE { get; set; }
		/// <summary>
		/// 超點加價
		/// </summary>
		public decimal OVERTIME_FEE { get; set; }
	}

	#endregion

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050110DataSearch
	{
		public Decimal ROWNUM { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次時間
		/// </summary>
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 倉別名稱
		/// </summary>
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 倉別型態
		/// </summary>
		public string WAREHOUSE_TYPE { get; set; }
		/// <summary>
		/// 儲區編號
		/// </summary>
		public string AREA_CODE { get; set; }
		/// <summary>
		/// 儲區名稱
		/// </summary>
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 揀貨狀態
		/// </summary>
		public decimal? PICK_STATUS { get; set; }
		/// <summary>
		/// 揀貨狀態名稱
		/// </summary>
		public string PICK_STATUS_NAME { get; set; }
		/// <summary>
		/// 揀貨方式
		/// </summary>
		public string PICK_TYPE { get; set; }
		/// <summary>
		/// 揀貨方式名稱
		/// </summary>
		public string PICK_TYPE_NAME { get; set; }
		/// <summary>
		/// 摘取工具
		/// </summary>
		public string PICK_TOOL { get; set; }
		/// <summary>
		/// 摘取工具名稱
		/// </summary>
		public string PICK_TOOL_NAME { get; set; }
		/// <summary>
		/// 播種工具
		/// </summary>
		public string PUT_TOOL { get; set; }
		/// <summary>
		/// 播種工具名稱
		/// </summary>
		public string PUT_TOOL_NAME { get; set; }
		/// <summary>
		/// 揀貨單位
		/// </summary>
		public string PICK_UNIT { get; set; }
		/// <summary>
		/// 揀貨單位名稱
		/// </summary>
		public string PICK_UNIT_NAME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 是否已列印(0:否;1:是)
		/// </summary>
		public string ISPRINTED { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050113DataSearch
	{
		public Decimal ROWNUM { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次時間
		/// </summary>
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 倉別名稱
		/// </summary>
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 倉別型態
		/// </summary>
		public string WAREHOUSE_TYPE { get; set; }
		/// <summary>
		/// 儲區編號
		/// </summary>
		public string AREA_CODE { get; set; }
		/// <summary>
		/// 儲區名稱
		/// </summary>
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 揀貨狀態
		/// </summary>
		public decimal? PICK_STATUS { get; set; }
		/// <summary>
		/// 揀貨狀態名稱
		/// </summary>
		public string PICK_STATUS_NAME { get; set; }
		/// <summary>
		/// 揀貨方式
		/// </summary>
		public string PICK_TYPE { get; set; }
		/// <summary>
		/// 揀貨方式名稱
		/// </summary>
		public string PICK_TYPE_NAME { get; set; }
		/// <summary>
		/// 摘取工具
		/// </summary>
		public string PICK_TOOL { get; set; }
		/// <summary>
		/// 摘取工具名稱
		/// </summary>
		public string PICK_TOOL_NAME { get; set; }
		/// <summary>
		/// 播種工具
		/// </summary>
		public string PUT_TOOL { get; set; }
		/// <summary>
		/// 播種工具名稱
		/// </summary>
		public string PUT_TOOL_NAME { get; set; }
		/// <summary>
		/// 揀貨單位
		/// </summary>
		public string PICK_UNIT { get; set; }
		/// <summary>
		/// 揀貨單位名稱
		/// </summary>
		public string PICK_UNIT_NAME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 是否已列印(0:否;1:是)
		/// </summary>
		public string ISPRINTED { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

	}

	[Serializable]
	[DataServiceKey("TYPE_ID")]
	public class WarehouseTypeList
	{
		public string TYPE_ID { get; set; }
		public string TYPE_NAME { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050110ReportCustOrdNoData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		[DataMember]
		public string MEMO { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050113ReportCustOrdNoData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		[DataMember]
		public string MEMO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050110RetailCodeData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次時段
		/// </summary>
		[DataMember]
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 門市編號
		/// </summary>
		[DataMember]
		public string RETAIL_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050110ReportData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 業主名稱
		/// </summary>
		[DataMember]
		public string GUP_NAME { get; set; }
		/// <summary>
		/// 貨主名稱
		/// </summary>
		[DataMember]
		public string CUST_NAME { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次時段
		/// </summary>
		[DataMember]
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 門市編號
		/// </summary>
		[DataMember]
		public string RETAIL_CODE { get; set; }
		/// <summary>
		/// 車次路線
		/// </summary>
		[DataMember]
		public string DELV_NO { get; set; }
		/// <summary>
		/// 路順
		/// </summary>
		[DataMember]
		public string DELV_WAY { get; set; }
		/// <summary>
		/// 出車時段(早、午、晚、夜)
		/// </summary>
		[DataMember]
		public string CAR_PERIOD_NAME { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 儲區名稱
		/// </summary>
		[DataMember]
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 門市名稱
		/// </summary>
		[DataMember]
		public string RETAIL_NAME { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		[DataMember]
		public string SERIAL_NO { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		[DataMember]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[DataMember]
		public string PICK_LOC { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 預定揀貨數量
		/// </summary>
		[DataMember]
		public Int32 B_PICK_QTY { get; set; }
		/// <summary>
		/// 商品尺寸
		/// </summary>
		[DataMember]
		public string ITEM_SIZE { get; set; }
		/// <summary>
		/// 商品規格
		/// </summary>
		[DataMember]
		public string ITEM_SPEC { get; set; }
		/// <summary>
		/// 商品顏色
		/// </summary>
		[DataMember]
		public string ITEM_COLOR { get; set; }
		/// <summary>
		/// 商品名稱
		/// </summary>
		[DataMember]
		public string ITEM_NAME { get; set; }
		/// <summary>
		/// 計價單位名稱
		/// </summary>
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		/// <summary>
		/// 派車日期
		/// </summary>
		[DataMember]
		public string TAKE_DATE { get; set; }
		/// <summary>
		/// 取件時間
		/// </summary>
		[DataMember]
		public string TAKE_TIME { get; set; }
		/// <summary>
		/// 貨主單號
		/// </summary>
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨單號條碼
		/// </summary>
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		/// <summary>
		/// 出貨單號條碼
		/// </summary>
		[DataMember]
		public string WmsOrdNoBarcode { get; set; }
		/// <summary>
		/// 商品條碼
		/// </summary>
		[DataMember]
		public string ItemCodeBarcode { get; set; }
		/// <summary>
		/// 商品序號條碼
		/// </summary>
		[DataMember]
		public string SerialNoBarcode { get; set; }
		/// <summary>
		/// 揀貨儲位條碼
		/// </summary>
		[DataMember]
		public string PickLocBarcode { get; set; }
		/// <summary>
		/// 國際條碼
		/// </summary>
		[DataMember]
		public string EAN_CODE1 { get; set; }
		/// <summary>
		/// 國際條碼
		/// </summary>
		[DataMember]
		public Int32 DELV_NO_NUMBER { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		[DataMember]
		public string UNIT_TRANS { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 改變包裝參考顯示Lable的字樣
		/// </summary>
		[DataMember]
		public string UNIT_TRANS_LABLENAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050113ReportData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 業主名稱
		/// </summary>
		[DataMember]
		public string GUP_NAME { get; set; }
		/// <summary>
		/// 貨主名稱
		/// </summary>
		[DataMember]
		public string CUST_NAME { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次時段
		/// </summary>
		[DataMember]
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 儲區名稱
		/// </summary>
		[DataMember]
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 商品序號
		/// </summary>
		[DataMember]
		public string SERIAL_NO { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		[DataMember]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[DataMember]
		public string PICK_LOC { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 預定揀貨數量
		/// </summary>
		[DataMember]
		public Int32 B_PICK_QTY { get; set; }
		/// <summary>
		/// 商品尺寸
		/// </summary>
		[DataMember]
		public string ITEM_SIZE { get; set; }
		/// <summary>
		/// 商品規格
		/// </summary>
		[DataMember]
		public string ITEM_SPEC { get; set; }
		/// <summary>
		/// 商品顏色
		/// </summary>
		[DataMember]
		public string ITEM_COLOR { get; set; }
		/// <summary>
		/// 商品名稱
		/// </summary>
		[DataMember]
		public string ITEM_NAME { get; set; }
		/// <summary>
		/// 計價單位名稱
		/// </summary>
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		/// <summary>
		/// 派車日期
		/// </summary>
		[DataMember]
		public string TAKE_DATE { get; set; }
		/// <summary>
		/// 取件時間
		/// </summary>
		[DataMember]
		public string TAKE_TIME { get; set; }
		/// <summary>
		/// 貨主單號
		/// </summary>
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨單號條碼
		/// </summary>
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		/// <summary>
		/// 出貨單號條碼
		/// </summary>
		[DataMember]
		public string WmsOrdNoBarcode { get; set; }
		/// <summary>
		/// 商品條碼
		/// </summary>
		[DataMember]
		public string ItemCodeBarcode { get; set; }
		/// <summary>
		/// 商品序號條碼
		/// </summary>
		[DataMember]
		public string SerialNoBarcode { get; set; }
		/// <summary>
		/// 揀貨儲位條碼
		/// </summary>
		[DataMember]
		public string PickLocBarcode { get; set; }
		/// <summary>
		/// 國際條碼
		/// </summary>
		[DataMember]
		public string EAN_CODE1 { get; set; }
		/// <summary>
		/// 國際條碼
		/// </summary>
		[DataMember]
		public Int32 DELV_NO_NUMBER { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		[DataMember]
		public string UNIT_TRANS { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 改變包裝參考顯示Lable的字樣
		/// </summary>
		[DataMember]
		public string UNIT_TRANS_LABLENAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050110ReportStickerData
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
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public string DELV_DATE_STR { get; set; }
		/// <summary>
		/// 車次路線
		/// </summary>
		[DataMember]
		public string DELV_NO { get; set; }
		/// <summary>
		/// 路順
		/// </summary>
		[DataMember]
		public string DELV_WAY { get; set; }
		/// <summary>
		/// 出車時段(早、午、晚、夜)
		/// </summary>
		[DataMember]
		public string CAR_PERIOD_NAME { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		[DataMember]
		public string BOX_NO { get; set; }
		/// <summary>
		/// 批次時段
		/// </summary>
		[DataMember]
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 出貨貼紙編號
		/// </summary>
		[DataMember]
		public string STICKER_NO { get; set; }
		/// <summary>
		/// 門市編號
		/// </summary>
		[DataMember]
		public string RETAIL_CODE { get; set; }
		/// <summary>
		/// 門市名稱
		/// </summary>
		[DataMember]
		public string RETAIL_NAME { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[DataMember]
		public string AREA_CODE { get; set; }
		/// <summary>
		/// 出車日
		/// </summary>
		[DataMember]
		public DateTime ARRIVAL_DATE { get; set; }
		/// <summary>
		/// 出車日
		/// </summary>
		[DataMember]
		public string ARRIVAL_DATE_STR { get; set; }
		/// <summary>
		/// 出貨貼紙編號條碼
		/// </summary>
		[DataMember]
		public string StickerNoBarcode { get; set; }
	}

	public class P080612ReportResult
	{
		public ExecuteResult Result { get; set; }
		public int ReportType { get; set; }
		public List<F050110ReportStickerData> StickerDataByWmsOrdNos { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class StickerData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		/// <summary>
		/// 批次日期
		/// </summary>
		[DataMember]
		public string DELV_DATE_STR { get; set; }
		/// <summary>
		/// 車次路線
		/// </summary>
		[DataMember]
		public string DELV_NO { get; set; }
		/// <summary>
		/// 路順
		/// </summary>
		[DataMember]
		public string DELV_WAY { get; set; }
		/// <summary>
		/// 出車時段(早、午、晚、夜)
		/// </summary>
		[DataMember]
		public string CAR_PERIOD_NAME { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		[DataMember]
		public string BOX_NO { get; set; }
		/// <summary>
		/// 批次時段
		/// </summary>
		[DataMember]
		public string PICK_TIME { get; set; }
		/// <summary>
		/// 出貨貼紙編號
		/// </summary>
		[DataMember]
		public string STICKER_NO { get; set; }
		/// <summary>
		/// 門市編號
		/// </summary>
		[DataMember]
		public string RETAIL_CODE { get; set; }
		/// <summary>
		/// 門市名稱
		/// </summary>
		[DataMember]
		public string RETAIL_NAME { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[DataMember]
		public string AREA_CODE { get; set; }
		/// <summary>
		/// 出車日
		/// </summary>
		[DataMember]
		public DateTime ARRIVAL_DATE { get; set; }
		/// <summary>
		/// 出車日
		/// </summary>
		[DataMember]
		public string ARRIVAL_DATE_STR { get; set; }
		/// <summary>
		/// 出貨貼紙編號條碼
		/// </summary>
		[DataMember]
		public string StickerNoBarcode { get; set; }
		/// <summary>
		/// 出貨單號
		/// </summary>
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]

		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 出車時段
		/// </summary>
		[DataMember]

		public string CAR_PERIOD { get; set; }
	}

	public class F055001NewPackageBox
	{
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public short PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F055003Data
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string BOX_BARCODE { get; set; }
		[DataMember]
		public string BOX_BARCODE_CODE128 { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string RETAIL_NAME { get; set; }
		[DataMember]
		public string DELV_NO { get; set; }
		[DataMember]
		public string DELV_WAY { get; set; }
		[DataMember]
		public int PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string CAR_PERIOD { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public int BOX_NUMBER { get; set; }
	}

	public class WmsApproveActItemDetail
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string WMS_ORD_SEQ { get; set; }
		public DateTime APPROVE_DATE { get; set; }
		public string ITEM_CODE { get; set; }
		public int A_DELV_QTY { get; set; }
	}

	public class WmsMapOrderHasApprove
	{
		public Decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
	}

	public class OrderDetailMapWmsHasApprove
	{
		public Decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }

		public string ORD_SEQ { get; set; }
		public string ITEM_CODE { get; set; }
		public int ORD_QTY { get; set; }

		public int A_DELV_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050112Pick
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public int PICK_STATUS { get; set; }
		[DataMember]
		public string PICK_STATUS_NAME { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
		[DataMember]
		public string PICK_TOOL_NAME { get; set; }
		[DataMember]
		public decimal ITEM_CNT { get; set; }
		[DataMember]
		public decimal TOTAL_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050112Batch
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime BATCH_DATE { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string PICK_STATUS { get; set; }
		[DataMember]
		public string PICK_STATUS_NAME { get; set; }
		[DataMember]
		public string PUT_STATUS { get; set; }
		[DataMember]
		public string PUT_STATUS_NAME { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
		[DataMember]
		public string PICK_TOOL_NAME { get; set; }
		[DataMember]
		public string PUT_TOOL { get; set; }
		[DataMember]
		public string PUT_TOOL_NAME { get; set; }
		[DataMember]
		public int ALLOT_CNT { get; set; }
		[DataMember]
		public string ALLOT_TYPE { get; set; }
		[DataMember]
		public string ALLOT_TYPE_NAME { get; set; }
		[DataMember]
		public decimal RETAIL_CNT { get; set; }
		[DataMember]
		public decimal ITEM_CNT { get; set; }
		[DataMember]
		public decimal TOTAL_QTY { get; set; }
		[DataMember]
		public DateTime? TRANS_DATE { get; set; }
		[DataMember]
		public DateTime? RECV_DATE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class CreateBatchPick
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DcCode { get; set; }
		[DataMember]
		public string GupCode { get; set; }
		[DataMember]
		public string CustCode { get; set; }
		[DataMember]
		public List<string> PickOrdNos { get; set; }
		[DataMember]
		public string AllotType { get; set; }
		[DataMember]
		public short AllotCnt { get; set; }
		[DataMember]
		public string PickTool { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050112PickSummary
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public int ITEM_CNT { get; set; }
		[DataMember]
		public int TOTAL_QTY { get; set; }
		[DataMember]
		public short RETAIL_CNT { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050112PickSummaryDetail
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string SHELF_NO { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int B_PICK_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P050112PickSummaryRetail
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string RETAIL_NAME { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P081203Data
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime BATCH_DATE { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string BATCH_PICK_NO { get; set; }
		[DataMember]
		public string WORKSTATION { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class BatchPickItemInfo
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string BATCH_PICK_NO { get; set; }
		[DataMember]
		public decimal BATCH_PICK_SEQ { get; set; }
		[DataMember]
		public int ITEM_CNT { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public int B_PICK_QTY { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public int CTNS { get; set; }
		[DataMember]
		public string UNIT_TRANS { get; set; }
		[DataMember]
		public int UNIT_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class CallAGVBatchData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string TPS_LOC_CODE { get; set; }
		[DataMember]
		public string BATCH_PICK_NO { get; set; }
		[DataMember]
		public Decimal BATCH_PICK_SEQ { get; set; }
		[DataMember]
		public string SHELF_NO { get; set; }
	}

	public class wmsMapOrderItem
	{
		public decimal ROWNUM { get; set; }
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string ITEM_CODE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class PutReportData
	{
		[DataMember]

		public decimal ROWNUM { get; set; }
		[DataMember]

		public string LOC_CODE { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ITEM_NAME { get; set; }
		[DataMember]

		public string RETAIL_CODE { get; set; }
		[DataMember]

		public string RETAIL_NAME { get; set; }
		[DataMember]

		public int PLAN_QTY { get; set; }
		[DataMember]
		public long? ACT_QTY { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class PickReportData
	{
		[DataMember]

		public decimal ROWNUM { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string SHELF_NO { get; set; }
		[DataMember]

		public string LOC_CODE { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ITEM_NAME { get; set; }

		[DataMember]

		public int B_PICK_QTY { get; set; }
		[DataMember]
		public int? A_PICK_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class BatchPickStation
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]

		public string DC_CODE { get; set; }
		[DataMember]

		public string GUP_CODE { get; set; }
		[DataMember]

		public string CUST_CODE { get; set; }
		[DataMember]

		public string BATCH_PICK_NO { get; set; }
		[DataMember]

		public string STATION_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public int ITEM_CNT { get; set; }
		[DataMember]
		public int TOTAL_QTY { get; set; }
	}

	public class ShelfSort
	{
		public string ShelfNo { get; set; }
		public int SortLevel { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class PackageBoxDetail
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]

		public string DC_CODE { get; set; }
		[DataMember]

		public string GUP_CODE { get; set; }
		[DataMember]

		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public int PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string BOX_BARCODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public int PACKAGE_QTY { get; set; }
		[DataMember]
		public int A_PACKAGE_QTY { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P060202Data
	{
		[DataMember]

		public decimal ROWNUM { get; set; }
		[DataMember]

		public string DC_CODE { get; set; }
		[DataMember]

		public string GUP_CODE { get; set; }
		[DataMember]

		public string CUST_CODE { get; set; }
		[DataMember]

		public DateTime DELV_DATE { get; set; }
		[DataMember]

		public string WAREHOUSE_ID { get; set; }
		[DataMember]

		public string WAREHOUSE_NAME { get; set; }
		[DataMember]

		public string PICK_LOC { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ITEM_NAME { get; set; }
		[DataMember]

		public int PICK_STOCK_QTY { get; set; }
		[DataMember]

		public int LACK_QTY { get; set; }
	}

	public class P060202TransferData
	{
		public string DC_CODE { get; set; }

		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }

		public DateTime DELV_DATE { get; set; }

		public string WAREHOUSE_ID { get; set; }
		public string PICK_LOC { get; set; }

		public string ITEM_CODE { get; set; }

		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string VNR_CODE { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public string MAKE_NO { get; set; }
		public int LACK_QTY { get; set; }
		public decimal LACK_SEQ { get; set; }
	}

	public class P060202UpdateF051206
	{
		public string AllocationNo { get; set; }
		public int AllocationSeq { get; set; }
		public List<decimal> LackSeqs { get; set; }
	}

	public class P060202PickStock
	{
		public string DC_CODE { get; set; }

		public string GUP_CODE { get; set; }

		public string CUST_CODE { get; set; }


		public string WAREHOUSE_ID { get; set; }
		public string LOC_CODE { get; set; }

		public string ITEM_CODE { get; set; }

		public int QTY { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P0503050000CalHead
	{
		public Decimal ROWNUM { get; set; }

		public string CAL_NO { get; set; }
		/// <summary>
		/// 訂單數
		/// </summary>
		public string ORD_CNT { get; set; }
		/// <summary>
		/// 品項數
		/// </summary>
		public string ITEM_CNT { get; set; }
		/// <summary>
		/// 門店數
		/// </summary>
		public string RETAIL_CNT { get; set; }
		/// <summary>
		/// 出貨數
		/// </summary>
		public string DELV_QTY { get; set; }
		/// <summary>
		/// 貨架數
		/// </summary>
		public int SHELF_CNT { get; set; }
	}

	public class PickLackRtnStock
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public int LACK_SEQ { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string PICK_LOC { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string VNR_CODE { get; set; }
		public string MAKE_NO { get; set; }
		public string BOX_CTRL_NO { get; set; }
		public string PALLET_CTRL_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public int LACK_QTY { get; set; }
		public DateTime DELV_DATE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P080901ShipReport
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]

		public string DC_CODE { get; set; }
		[DataMember]

		public string GUP_CODE { get; set; }
		[DataMember]

		public string CUST_CODE { get; set; }
		[DataMember]

		public DateTime TAKE_DATE { get; set; }
		[DataMember]

		public string TAKE_TIME { get; set; }
		[DataMember]

		public string ALL_ID { get; set; }
		[DataMember]

		public string ALL_COMP { get; set; }
		[DataMember]

		public string CAR_NO_A { get; set; }
		[DataMember]

		public string CAR_NO_B { get; set; }
		[DataMember]

		public string CAR_NO_C { get; set; }
		[DataMember]

		public DateTime DELV_DATE { get; set; }
		[DataMember]

		public string PICK_TIME { get; set; }
		[DataMember]

		public string CONSIGN_NO { get; set; }
		[DataMember]

		public int BOXQTY { get; set; }
		[DataMember]

		public string WMS_ORD_NO { get; set; }
		[DataMember]

		public int STATUS { get; set; }
	}

	public class P8202050000_F050101
	{
		public F050301 F050301 { get; set; }
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public List<decimal> F050801_STATUS_List { get; set; }
		public string PROC_FLAG { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP0501010004Model
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string BARCODE { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP0501010005Model
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string BARCODE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ShipPackageNoAllotOrder
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string ORD_SEQ { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public Int32 B_DELV_QTY { get; set; }
		[DataMember]
		public Int32 PACKAGE_QTY { get; set; }

	}

	//出貨單查詢-出貨序號
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051202WithF055002
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
    }

	public class GetPickNosRes
	{
		public string WmsOrdNo { get; set; }
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string PickOrdNo { get; set; }
		public string WarehouseId { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class HomeDeliveryOrderDebitResult
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsSuccessed { get; set; }
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public HomeDeliveryOrderNumberData Data { get; set; }
	}

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_ORD_NO")]
	[DataContract]
	public class HomeDeliveryOrderNumberData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public short PACKAGE_BOX_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public string WORKSTATION_CODE { get; set; }
		[DataMember]
		public string LOGISTIC_CODE { get; set; }
	  [DataMember]
		public string LOGISTIC_NAME { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("VNR_CODE", "SOURCE_NO")]
	public class LittleWhiteReport : IEncryptable
	{
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string SOURCE_NO { get; set; }
		[DataMember]
		public string sourceNoBarcode { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_DATE", "PICK_TIME")]
	public class BatchPickNoList
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string ORD_TYPE_NAME { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string CUST_COST_NAME { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE_NAME { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
		[DataMember]
		public string SOURCE_NAME { get; set; }
		[DataMember]
		public int ATFL_N_PICK_CNT { get; set; }
		[DataMember]
		public int ATFL_B_PICK_CNT { get; set; }
		[DataMember]
		public int ATFL_NP_PICK_CNT { get; set; }
		[DataMember]
		public int ATFL_BP_PICK_CNT { get; set; }
		[DataMember]
		public int PDA_PICK_PERCENT { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
    [DataMember]
    public int REPICK_CNT { get; set; }
    [DataMember]
    public string ORDER_PROC_TYPE_NAME { get; set; }
  }

  [Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_DATE", "PICK_TIME")]
	public class CalcatePickPercent
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public int PDA_PICK_PERCENT { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "PICK_ORD_NO")]
	public class ChangePickTool
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "CRT_DATE", "PICK_ORD_NO")]
	public class RePickNoList
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE { get; set; }
		[DataMember]
		public string SOURCE_TYPE { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
		[DataMember]
		public string COLLECTION_CODE { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
    [DataMember]
    public string ORDER_PROC_TYPE_NAME { get; set; }
  }

	[Serializable]
	[DataContract]
	[DataServiceKey("PICK_ORD_NO", "WMS_ORD_NO")]
	public class RePrintPickNoList
	{

		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }

		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
		[DataMember]
		public string PICK_TOOL_NAME { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
  }

  [DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class SinglePickingReportData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		//增加通路類型
		[DataMember]
		public string CHANNEL { get; set; }
		[DataMember]
		public string NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string TMPR_TYPE_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		[DataMember]
		public Int32 B_PICK_QTY { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		[DataMember]
		public string WmsOrdNoBarcode { get; set; }
		[DataMember]
		public string ItemCodeBarcode { get; set; }

		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string SerialNoBarcode { get; set; }
		[DataMember]
		public string PickLocBarcode { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string ORDER_CUST_NAME { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string MEMO { get; set; }

		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string SHORT_NAME { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE { get; set; }
		[DataMember]
		public string SPLIT_CODE { get; set; }
		[DataMember]
		public string NEXT_STEP { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public string RTN_VNR_CODE { get; set; }
		[DataMember]
		public string RTN_VNR_NAME { get; set; }
    /// <summary>
    /// 郵遞區號＆北北基文字內容
    /// </summary>
    [DataMember]
    public string ORDER_PROC_TYPE_NAME { get; set; }
  }

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class BatchPickingReportData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		//增加通路類型
		[DataMember]
		public string CHANNEL { get; set; }
		[DataMember]
		public string NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string TMPR_TYPE_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		[DataMember]
		public Int32 B_PICK_QTY { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		[DataMember]
		public string WmsOrdNoBarcode { get; set; }
		[DataMember]
		public string ItemCodeBarcode { get; set; }

		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string SerialNoBarcode { get; set; }
		[DataMember]
		public string PickLocBarcode { get; set; }
		[DataMember]
		[Encrypted]
		[SecretPersonalData("NAME")]
		public string ORDER_CUST_NAME { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string MEMO { get; set; }

		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string SHORT_NAME { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public string FAST_DEAL_TYPE { get; set; }
		[DataMember]
		public string SPLIT_CODE { get; set; }
		[DataMember]
		public string NEXT_STEP { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public string RTN_VNR_CODE { get; set; }
		[DataMember]
		public string RTN_VNR_NAME { get; set; }
    /// <summary>
    /// 郵遞區號＆北北基文字內容(批量揀貨固定會是null)
    /// </summary>
    [DataMember]
    public string ORDER_PROC_TYPE_NAME { get; set; }

  }

  [DataContract]
	[Serializable]
	[DataServiceKey("PICK_ORD_NO")]
	public class SinglePickingTickerData
	{
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		[DataMember]
		public string SPLIT_CODE { get; set; }
		[DataMember]
		public string RTN_VNR_CODE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("PICK_ORD_NO")]
	public class BatchPickingTickerData
	{
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PickOrdNoBarcode { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("DcCode", "GupCode", "CustCode", "PickOrdNo")]
	public class PickAllotData
	{
		[DataMember]
		public string DcCode { get; set; }
		[DataMember]
		public string GupCode { get; set; }
		[DataMember]
		public string CustCode { get; set; }
		[DataMember]
		public string PickOrdNo { get; set; }
		[DataMember]
		public int WmsOrdCnt { get; set; }
		[DataMember]
		public List<ShipOrderPickAllot> ShipOrderPickAllots { get; set; }

	}
	[Serializable]
	[DataContract]
	[DataServiceKey("WmsOrdNo")]
	public class ShipOrderPickAllot
	{
		[DataMember]
		public string WmsOrdNo { get; set; }
		[DataMember]
		public string PickLocNo { get; set; }
		[DataMember]
		public string ContainerCode { get; set; }
		[DataMember]
		public string NextStep { get; set; }
		[DataMember]
		public string ColletionCode { get; set; }
		[DataMember]
		public string Status { get; set; }
		[DataMember]
		public List<ShipOrderPickAllotDetail> ShipOrderPickAllotDetails { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("PickOrdSeq")]
	public class ShipOrderPickAllotDetail
	{
		[DataMember]
		public string PickOrdSeq { get; set; }
		[DataMember]
		public string WmsOrdNo { get; set; }
		[DataMember]
		public string WmsOrdSeq { get; set; }
		[DataMember]
		public int BSetQty { get; set; }
		[DataMember]
		public int ASetQty { get; set; }
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string ItemName { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class PickAllotResult : ExecuteResult
	{
		[DataMember]
		public string PickLocNo { get; set; }
		[DataMember]
		public string PickOrdSeq { get; set; }
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string ItemName { get; set; }
		[DataMember]
		public bool IsPickSowFinished { get; set; }
		[DataMember]
		public List<string> CancelWmsOrdNos { get; set; }
	}

	public class OrdNoStatusModel
	{
		public string ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// F050801.STATUS
		/// </summary>
		public decimal STATUS { get; set; }
	}

	public class NextStepModel
	{
		public string COLLECTION_NAME { get; set; }
		public string NEXT_STEP_NAME { get; set; }
		public string NEXT_STEP { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class ScanContainerResult : ExecuteResult
	{
		[DataMember]
		public string ContainerCode { get; set; }
		[DataMember]
		public ContainerPickInfo ContainerPickInfo { get; set; }
		[DataMember]
		public BoxInfo NormalBox { get; set; }
		[DataMember]
		public BoxInfo CancelBox { get; set; }
		[DataMember]
		public bool IsPickLastBox { get; set; }
		[DataMember]
		public bool IsFisrtAllot { get; set; }

	}
	[Serializable]
	[DataContract]
	[DataServiceKey("PickOrdNo")]
	public class ContainerPickInfo
	{
		[DataMember]
		public Int64 Id { get; set; }
		[DataMember]
		public string PickOrdNo { get; set; }
		[DataMember]
		public DateTime DelvDate { get; set; }
		[DataMember]
		public string PickTime { get; set; }
		[DataMember]
		public int BatchPickCnt { get; set; }
		[DataMember]
		public int BatchPickQty { get; set; }
		[DataMember]
		public int PickQty { get; set; }
		[DataMember]
		public string MoveOutTarget { get; set; }
		[DataMember]
		public string MoveOutTargetName { get; set; }
		[DataMember]
		public int NormalOrderCnt { get; set; }
		[DataMember]
		public int CancelOrderCnt { get; set; }

	}
	[Serializable]
	[DataContract]
	[DataServiceKey("BoxNo")]
	public class BoxInfo
	{
		[DataMember]
		public string SowType { get; set; }
		[DataMember]
		public string BoxNo { get; set; }
		[DataMember]
		public int SowQty { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class BindBoxResult : ExecuteResult
	{
		[DataMember]
		public BoxInfo BoxInfo { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class SowItemResult : ExecuteResult
	{
		/// <summary>
		/// 箱資訊
		/// </summary>
		[DataMember]
		public BoxInfo BoxInfo { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[DataMember]
		public string ItemCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		[DataMember]
		public string ItemName { get; set; }
		/// <summary>
		/// 是否容器完成
		/// </summary>
		[DataMember]
		public bool IsContainerFinished { get; set; }
		/// <summary>
		/// 是否批次完成
		/// </summary>
		[DataMember]
		public bool IsBatchFinished { get; set; }

	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class LackItemResult : ExecuteResult
	{
		/// <summary>
		/// 揀貨缺品清單
		/// </summary>
		[DataMember]
		public List<LackItem> LackItemDetails { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ItemCode")]
	public class LackItem
	{
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string ItemName { get; set; }
		[DataMember]
		public int LackQty { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class WmsShipBoxDetail
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
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public int PACKAGE_BOX_NO { get; set; }

		[DataMember]
		public string PACKAGE_STAFF { get; set; }
		[DataMember]
		public string PACKAGE_NAME { get; set; }

		[DataMember]
		public int PACKAGE_BOX_SEQ { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]

		public int PACKAGE_QTY { get; set; }
		[DataMember]

		public string ORD_NO { get; set; }
		[DataMember]

		public string ORD_SEQ { get; set; }

		[DataMember]
		public Int64 ID { get; set; }
		[DataMember]
		public string WMS_ORD_SEQ { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PICK_ORD_SEQ { get; set; }

	}

	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class PickOutOfStockResult : ExecuteResult
	{
		/// <summary>
		/// 是否容器完成
		/// </summary>
		[DataMember]
		public bool IsContainerFinished { get; set; }
		/// <summary>
		/// 是否批次完成
		/// </summary>
		[DataMember]
		public bool IsBatchFinished { get; set; }

	}
	[Serializable]
	[DataServiceKey("ID")]
	public class P0808040100_BoxData
	{
		[DataMember]
		public Int64 ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET_NAME { get; set; }
		[DataMember]
		public string SOW_TYPE { get; set; }
		[DataMember]
		public string SOW_TYPE_NAME { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public string CONTAINER_CODE { get; set; }
	}


	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P0808040100_BoxDetailData
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32 A_SET_QTY { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P0808040100_PrintData
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string PACKAGE_BOX { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET { get; set; }
		[DataMember]
		public string CONTAINER_SEQ { get; set; }
		[DataMember]
		public string DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO_BARCODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
		[DataMember]
		public int A_SET_QTY { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("PICK_ORD_NO")]
	public class BatchPickData
	{
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public string PICK_STATUS { get; set; }
		[DataMember]
		public string PICK_TOOL { get; set; }
		[DataMember]
		public string SPLIT_CODE { get; set; }
	}

	public class WcsOutboundPickOrdData
	{
		public string PickOrdNo { get; set; }
		public string SourceType { get; set; }
		public string CustCost { get; set; }
		public string FastDealType { get; set; }
		public string PickType { get; set; }
		public string NextStepName { get; set; }
		public string NextStep { get; set; }
		public string MoveOutTarget { get; set; }
		public string PackingType { get; set; }
		public string ContainerType { get; set; }
		public string SplitCode { get; set; }
    public int? PriorityValue { get; set; }
		public Int16? PickStatus { get; set; }
		public string RtnVnrCode { get; set; }
		public string NpFlag { get; set; }
  }

	public class WcsOutboundCollectionData
	{
		public string WmsNo { get; set; }
		public string CollectionName { get; set; }
	}

	public class ContainerSingleByOrd
	{
		public string COLLECTION_CODE { get; set; }
		public string NEXT_STEP { get; set; }
		public string NEXT_STEP_NAME { get; set; }
		public string STATUS { get; set; }
		public string STATUS_NAME { get; set; }
	}

	public class ContainerSingleByPick
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string PICK_TYPE { get; set; }
		public string MERGE_NO { get; set; }
		public string PICK_ORD_NO { get; set; }
		public string NEXT_STEP { get; set; }
		public string NEXT_STEP_NAME { get; set; }
		public string PICK_STATUS_NAME { get; set; }
	}

	#region P0503030000 出貨單查詢
	// 揀貨單資料
	[DataContract]
	[Serializable]
	[DataServiceKey("PICK_ORD_NO")]
	public class F051201WithF051202
	{
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨狀態
		/// </summary>
		[DataMember]
		public string PICK_STATUS { get; set; }
		/// <summary>
		/// 揀貨單類型
		/// </summary>
		[DataMember]
		public string PICK_TYPE { get; set; }
		/// <summary>
		/// 揀貨工具
		/// </summary>
		[DataMember]
		public string PICK_TOOL { get; set; }
		/// <summary>
		/// 完成揀貨後的下一步
		/// </summary>
		[DataMember]
		public string NEXT_STEP { get; set; }
		/// <summary>
		/// 揀貨開始時間
		/// </summary>
		[DataMember]
		public DateTime? PICK_START_TIME { get; set; }
		/// <summary>
		/// 揀貨完成時間
		/// </summary>
		[DataMember]
		public DateTime? PICK_FINISH_DATE { get; set; }
		/// <summary>
		/// 指定容器
		/// </summary>
		[DataMember]
		public string CONTAINER_TYPE_NAME { get; set; }
		/// <summary>
		/// 派發系統	
		/// </summary>
		[DataMember]
		public string DISP_SYSTEM { get; set; }
		/// <summary>
		/// 揀貨人員
		/// </summary>
		[DataMember]
		public string PICK_NAME { get; set; }
    /// <summary>
		/// 揀貨優先權
		/// </summary>
		[DataMember]
    public int? PRIORITY_VALUE { get; set; }
  }

	// 揀貨單明細資料
	[DataContract]
	[Serializable]
	[DataServiceKey("PICK_ORD_NO")]
	public class PickDetail
	{
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// 揀貨單序號
		/// </summary>
		[DataMember]
		public string PICK_ORD_SEQ { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[DataMember]
		public string PICK_LOC { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[DataMember]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 預計揀貨數
		/// </summary>
		[DataMember]
		public int B_PICK_QTY { get; set; }
		/// <summary>
		/// 實際揀貨數
		/// </summary>
		[DataMember]
		public int A_PICK_QTY { get; set; }
		/// <summary>
		/// 揀貨狀態
		/// </summary>
		[DataMember]
		public string PICK_STATUS { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 路順
		/// </summary>
		[DataMember]
		public int ROUTE_SEQ { get; set; }
		/// <summary>
		/// PK區名稱
		/// </summary>
		[DataMember]
		public string PK_AREA_NAME { get; set; }

    [DataMember]
    public string SERIAL_NO { get; set; }
	}

	// 託運單
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ConsignmentNote
  {
    [DataMember]
    public int ROWNUM { get; set; }
    /// <summary>
    /// 配送商編號
    /// </summary>
    [DataMember]
		public string LOGISTIC_CODE { get; set; }
		/// <summary>
		/// 配送商名稱  
		/// </summary>
		[DataMember]
		public string LOGISTIC_NAME { get; set; }
		/// <summary>
		/// 宅配單號
		/// </summary>
		[DataMember]
		public string PAST_NO { get; set; }
		/// <summary>
		/// 包裝紙箱編號
		/// </summary>
		[DataMember]
		public string BOX_NUM { get; set; }
        /// <summary>
		/// 工作站編號
		/// </summary>
		[DataMember]
    public string WORKSTATION_CODE { get; set; }
    /// <summary>
    /// 建立人員
    /// </summary>
    [DataMember]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 建立時間
		/// </summary>
		[DataMember]
		public DateTime CRT_DATE { get; set; }
	}
	#endregion

	public class ItemQtyModel
	{
		public string ITEM_CODE { get; set; }
		public int QTY { get; set; }
	}

	public class ContainerSingleByAlloc
	{
		public string TAR_WAREHOUSE_ID { get; set; }
		public string STATUS_NAME { get; set; }
	}

    public class ContainerMixModel
    {
        public string ITEM_CODE { get; set; }
        public string CUST_ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public int QTY { get; set; }
        public string WMS_NO { get; set; }
        public string STATUS { get; set; }
  }

	#region 查詢4小時未配庫的出貨單
	// 未配庫訂單
	[DataContract]
	[Serializable]
	[DataServiceKey("ORD_NO")]
	public class UndistributedOrder
	{
		public DateTime ORD_DATE { get; set; }
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string SOURCE_NO { get; set; }
		public string CUST_COST_NAME { get; set; }
		public string FAST_DEAL_TYPE_NAME { get; set; }
		public string MOVE_OUT_TARGET { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string MORE_THEN_FOUR_HOURS { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ID")]
	// 已配庫但尚未產生揀貨單
	public class NotGeneratedPick
	{
		public Int64 ID { get; set; }
		public DateTime ORD_DATE { get; set; }
		public string WMS_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string SOURCE_NO { get; set; }
		public string CUST_COST_NAME { get; set; }
		public string FAST_DEAL_TYPE_NAME { get; set; }
		public string MOVE_OUT_TARGET { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string MORE_THEN_FOUR_HOURS { get; set; }
	}
    #endregion

    #region 跨庫出貨配庫補貨排程
    [Serializable]
    [DataServiceKey("GUP_CODE, CUST_CODE")]
    // 未配庫跨庫出貨訂單商品訂購數
    public class MoveoutOrdItemQty
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
        public string MAKE_NO { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
        public int ORD_QTY { get; set; }
    }
    #endregion

	public class CanDebitShipOrder
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
	}

	public class F050004Ex
	{
		public decimal TICKET_ID { get; set; }
		public string TICKET_CLASS { get; set; }
		public int ORDER_LIMIT { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public int DELV_DAY { get; set; }
	}

  /// <summary>
  /// 跨庫訂單整箱出庫-箱內明細 容器清單
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("F0531_ID", "OUT_CONTAINER_CODE")]
  public class F0532Ex
  {
    /// <summary>
    /// 跨庫出貨容器使用中流水號
    /// </summary>
    [DataMember]
    public long F0531_ID { get; set; }

    [DataMember]
    public string DC_CODE { get; set; } 

    /// <summary>
    /// 建立日期
    /// </summary>
    [DataMember]
    public DateTime CRT_DATE { get; set; }

    /// <summary>
    /// 關箱時間(列印用)
    /// </summary>
    [DataMember]
    public DateTime? CLOSE_DATE { get; set; }

    /// <summary>
    /// 跨庫箱號
    /// </summary>
    [DataMember]
    public string OUT_CONTAINER_CODE { get; set; }

    /// <summary>
    /// 容器類型
    /// </summary>
    [DataMember]
    public string SOW_TYPE { get; set; }

    /// <summary>
    /// 容器類型名稱
    /// </summary>
    [DataMember]
    public string SOW_TYPE_NAME { get; set; }

    /// <summary>
    /// 跨庫箱號條碼(列印用)
    /// </summary>
    [DataMember]
    public string OUT_CONTAINER_CODE_BARCODE { get; set; }

    /// <summary>
    /// 訂單單號條碼(列印用)
    /// </summary>
    [DataMember]
    public string ORDER_NO_BARCODE { get; set; }

    /// <summary>
    /// 目的地
    /// </summary>
    [DataMember]
    public string MOVE_OUT_TARGET_NAME { get; set; }

    /// <summary>
    /// 總PCS數
    /// </summary>
    [DataMember]
    public int TOTAL_PCS { get; set; }

    /// <summary>
    /// 狀態名稱
    /// </summary>
    [DataMember]
    public string STATUS_NAME { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    [DataMember]
    public string STATUS { get; set; }
  }

  /// <summary>
  /// 跨庫訂單整箱出庫-箱內明細 容器內商品清單
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE")]
  public class F053202Ex
  {
    /// <summary>
    /// 商品品號
    /// </summary>
    [DataMember]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 商品品名
    /// </summary>
    [DataMember]
    public string ITEM_NAME { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    [DataMember]
    public int QTY { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class P0808050000_PrintData
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [DataMember]
    public int ROWNUM { get; set; }

    /// <summary>
    /// 品編
    /// </summary>
    [DataMember]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [DataMember]
    public int QTY { get; set; }

    /// <summary>
    /// 品名
    /// </summary>
    [DataMember]
    public string ITEM_NAME { get; set; }

    [DataMember]
    public string EAN_CODE1 { get; set; }

    [DataMember]
    public string EAN_CODE2 { get; set; }

    [DataMember]
    public string EAN_CODE3 { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class P0808050000_CancelPrintData
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [DataMember]
    public int ROWNUM { get; set; }

    /// <summary>
    /// 品編
    /// </summary>
    [DataMember]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 品名
    /// </summary>
    [DataMember]
    public string ITEM_NAME { get; set; }

    [DataMember]
    public string EAN_CODE1 { get; set; }

    [DataMember]
    public string EAN_CODE2 { get; set; }

    [DataMember]
    public string EAN_CODE3 { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [DataMember]
    public int QTY { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    [DataMember]
    public string ORD_NO { get; set; }

    /// <summary>
    /// 訂單編號條碼
    /// </summary>
    [DataMember]
    public string ORDER_NO_BAR { get; set; }
  }

  /// <summary>
  /// 跨庫箱號資料
  /// </summary>
  [Serializable]
	[DataContract]
	[DataServiceKey("F0531_ID")]
	public class OutContainerInfo
	{
		[DataMember]
		public long F0531_ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string OUT_CONTAINER_CODE { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public int TOTAL { get; set; }
		[DataMember]
		public string WORK_TYPE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public long F0701_ID { get; set; }
		[DataMember]
		public string SOW_TYPE { get; set; }
	}

	/// <summary>
	/// 跨庫箱號資訊
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class OutContainerResult : ExecuteResult
	{
		[DataMember]
		public string ContainerCode { get; set; }
		[DataMember]
		public OutContainerInfo OutContainerInfo { get; set; }
		[DataMember]
		public string MoveOutTargetName { get; set; }
		[DataMember]
		public int TotalPcs { get; set; }
	}

	/// <summary>
	/// 跨庫箱號資訊
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class PickContainerPutIntoOutContainerResult : ExecuteResult
	{
		[DataMember]
		public bool IsOutContainerError { get; set; }
		[DataMember]
		public bool IsPickContainerError { get; set; }
		[DataMember]
		public OutContainerResult UpdateOutContainerResult { get; set; }
	}

	public class F0534_NotAllotPick
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string PICK_ORD_NO { get; set; }
	}

	public class F051202Ex : F051202
	{
		public int LACK_QTY { get; set; }
	}

	public class PickInfoWithLackItem
	{
		public string PICK_LOC { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public string MAKE_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public int LACK_QTY { get; set; }
	}

	public class MoveOutContainerDtl
	{
		public string OUT_CONTAINER_CODE { get; set; }
		public int OUT_CONTAINER_SEQ { get; set; }
		public string SOW_TYPE { get; set; }
		public long F0531_ID { get; set; }
		public long F0701_ID { get; set; }
		public string ITEM_CODE { get; set; }
		public string SERIAL_NO { get; set; }
		public int QTY { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string WMS_ORD_SEQ { get; set; }
	}

	public class F0535_NotDebitOrder
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_ORD_NO { get; set; }
	}

	public class ScanItemBarcodeResult : ExecuteResult
	{
		public bool bindNewNormalContainer { get; set; }
		public bool bindNewCancelContainer { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public bool IsNormalShipItem { get; set; }
		public bool IsFinishAllot { get; set; }
		public BindingPickContainerInfo BindingPickContainerInfo { get; set; }
	}

	public class F053601_NotAllotData
	{
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public int NOALLOT_QTY { get; set; }
	}

	public class F0537_LackData
	{
		public string ITEM_CODE { get; set; }
		public string PICK_ORD_SEQ { get; set; }
		public int B_LACK_QTY { get; set; }
		public int A_LACK_QTY { get; set; }
	}

	public class PickWithWmsMap
	{
		public string PICK_ORD_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
	}

  /// <summary>
  /// 跨庫調撥出貨分配扣帳排程用
  /// </summary>
  public class ShipFinishConfirmNotifyData
  {
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE{ get; set; }
    public string WMS_NO { get; set; }
  }

	public class OrderWithMakeNo
	{
		public string ORD_NO { get; set; }
		public string ORD_SEQ { get; set; }
		public string MAKE_NO { get; set; }
	}

	public class F051206LackData : F051206
	{
		/// <summary>
		/// 出貨項次
		/// </summary>
		public string WMS_ORD_SEQ { get; set; }
	}

	public class F050101WarehouseOutEx
	{
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string CUST_ORD_NO { get; set; }

		/// <summary>
		/// 貨主成本中心/貨主自定分類
		/// </summary>
		public string CUST_COST { get; set; }
	}

	public class F05030202LackData
	{
		/// <summary>
		/// 訂單序號
		/// </summary>
		[Required]
		public string ORD_SEQ { get; set; }

		/// <summary>
		/// 出貨單號
		/// </summary>
		[Required]
		public string WMS_ORD_NO { get; set; }

		/// <summary>
		/// 出貨序號
		/// </summary>
		[Required]
		public string WMS_ORD_SEQ { get; set; }

		/// <summary>
		/// 預計出貨數量
		/// </summary>
		public Int32 B_DELV_QTY { get; set; }

		/// <summary>
		/// 實際出貨數量
		/// </summary>
		public Int32? A_DELV_QTY { get; set; }
	}


	public class CustWms_F050101_Detail
	{
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string PROC_FLAG { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string F050301_ORD_NO { get; set; }
		public decimal F050801_STATUS { get; set; }
	}
	public class CustWms_F050101
	{
		public string ORD_NO { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string PROC_FLAG { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string F050301_ORD_NO { get; set; }
		public List<decimal> F050801_STATUS_List { get; set; }
	}

  /// <summary>
	/// 分貨資訊
	/// </summary>
	[Serializable]
  [DataContract]
  [DataServiceKey("PICK_ORD_NO")]
  public class DivideInfo
  {
    [DataMember]
    public string PICK_ORD_NO { get; set; }
    [DataMember]
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string CONTAINER_CODE { get; set; }
    [DataMember]
    public string NEXT_STEP { get; set; }
    [DataMember]
    public string STATUS { get; set; }
  }

  /// <summary>
	/// 分貨明細資料
	/// </summary>
	[Serializable]
  [DataContract]
  [DataServiceKey("PICK_ORD_NO", "PICK_ORD_SEQ")]
  public class DivideDetail
  {
    [DataMember]
    public string PICK_ORD_NO { get; set; }
    [DataMember]
    public string PICK_ORD_SEQ { get; set; }
    [DataMember]
    public int PICK_LOC_NO { get; set; }
    [DataMember]
    public string CONTAINER_CODE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public int B_SET_QTY { get; set; }
    [DataMember]
    public int A_SET_QTY { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
    [DataMember]
    public DateTime CRT_DATE { get; set; }
    [DataMember]
    public DateTime UPD_DATE { get; set; }
  }

  /// <summary>
	/// 集貨場進出紀錄
	/// </summary>
	[Serializable]
  [DataContract]
  [DataServiceKey("COLLECTION_CODE", "CELL_CODE")]
  public class CollectionRecord
  {
    [DataMember]
    public string COLLECTION_CODE { get; set; }
    [DataMember]
    public string CELL_CODE { get; set; }
    [DataMember]
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string CONTAINER_CODE { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public DateTime CRT_DATE { get; set; }
  }

  /// <summary>
	/// 託運單箱內明細資料
	/// </summary>
	[Serializable]
  [DataContract]
  [DataServiceKey("PACKAGE_BOX_NO", "PACKAGE_BOX_SEQ")]
  public class ConsignmentDetail
  {
    [DataMember]
    public string PAST_NO { get; set; }
    [DataMember]
    public int PACKAGE_BOX_NO { get; set; }
    [DataMember]
    public string BOX_NUM { get; set; }
    [DataMember]
    public int PACKAGE_BOX_SEQ { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public int PACKAGE_QTY { get; set; }
    [DataMember]
    public decimal PACK_WEIGHT { get; set; }
    [DataMember]
    public decimal TOTAL_WEIGHT { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public DateTime CRT_DATE { get; set; }
  }
}
