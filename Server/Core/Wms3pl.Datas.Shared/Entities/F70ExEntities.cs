using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{

	[Serializable]
	[DataContract]
	[DataServiceKey("DISTR_CAR_NO", "DC_CODE")]
	public class F700101EX
	{
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public System.DateTime TAKE_DATE { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public decimal? CAR_KIND_ID { get; set; }
		[DataMember]
		public string SP_CAR { get; set; }
		[DataMember]
		public string CHARGE_CUST { get; set; }
		[DataMember]
		public string CHARGE_DC { get; set; }
		[DataMember]
		public decimal FEE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string EDI_FLAG { get; set; }
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
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string CAR_SIZE { get; set; }
		[DataMember]
		public string CAR_KIND_NAME { get; set; }
		[DataMember]
		public string CHARGE_GUP_CODE { get; set; }
		[DataMember]
		public string CHARGE_CUST_CODE { get; set; }
		[DataMember]
		public string DISTR_SOURCE { get; set; }
		[DataMember]
		public string HAVE_WMS_NO { get; set; }
	}

	//刪除派車記錄使用
	[Serializable]
	[DataContract]
	[DataServiceKey("DISTR_CAR_NO", "DC_CODE")]
	public class F700102DirstCarNo
	{
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
	}

	#region 行事曆
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE", "SCHEDULE_NO")]
	public class F700501Ex
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string SCHEDULE_NO { get; set; }
		[DataMember]
		public DateTime SCHEDULE_DATE { get; set; }
		[DataMember]
		public string SCHEDULE_TIME { get; set; }
		[DataMember]
		public string SCHEDULE_TYPE { get; set; }
		[DataMember]
		public string IMPORTANCE { get; set; }
		[DataMember]
		public string SUBJECT { get; set; }
		[DataMember]
		public string CONTENT { get; set; }
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
		public string SCHEDULE_TYPE_TEXT { get; set; }
		[DataMember]
		public string IMPORTANCE_TEXT { get; set; }
		[DataMember]
		public string FILE_NAME { get; set; }
		[DataMember]
		public string STATUS { get; set; }
	}
	#endregion

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700201Ex
	{
		public System.Decimal ROWNUM { get; set; }
		public string COMPLAINT_NO { get; set; }
		public DateTime COMPLAINT_DATE { get; set; }
		public string RETAIL_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string COMPLAINT_NAME { get; set; }
		public string COMPLAINT_TYPE { get; set; }
		public string COMPLAINT_DESC { get; set; }
		public Int32 QTY { get; set; }
		public string DEP_ID { get; set; }
		public string HANDLE_STAFF { get; set; }
		public string RESPOND_DESC { get; set; }
		public string HANDLE_DESC { get; set; }
		public string STATUS { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_STAFF { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string UPD_NAME { get; set; }
		public string COMPLAINT_NAME1 { get; set; }
		public string DEP_NAME { get; set; }
		public string STATUS_DESC { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700102Data
	{
		[DataMember]
		public string ChangeStatus { get; set; }
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public Int32 DISTR_CAR_SEQ { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ENTRUST_DEPT { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public Int32 ITEM_QTY { get; set; }
		[DataMember]
		public string DISTR_USE { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
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
		public string CAR_NO_A { get; set; }
		[DataMember]
		public string CAR_NO_B { get; set; }
		[DataMember]
		public string CAR_NO_C { get; set; }
		[DataMember]
		public string ISSEAL { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string CAN_FAST { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public Decimal? VOLUMN { get; set; }
		[DataMember]
		public string DELV_TIMES { get; set; }
		[DataMember]
		public string DISTR_TYPE { get; set; }
		[DataMember]
		public string COST_CENTER { get; set; }
		[DataMember]
		public string ROUTE_CODE { get; set; }
		[DataMember]
		public string BACK_PAST_NO { get; set; }
		[DataMember]
		public DateTime? BACK_DATE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string COUDIV_ID { get; set; }
		[DataMember]
		public string COUDIV_NAME { get; set; }
		[DataMember]
		public string ZIP_NAME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string ORD_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string F1909_CUST_NAME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string DELV_EFFIC_NAME { get; set; }
		[DataMember]
		public string DELV_TMPR_NAME { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
		[DataMember]
		public string DELV_EFFIC_ORG { get; set; }
		[DataMember]
		public DateTime? DELV_DATE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700102DataReport
	{
		[DataMember]
		public string ChangeStatus { get; set; }
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public Int32 DISTR_CAR_SEQ { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ENTRUST_DEPT { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public Int32 ITEM_QTY { get; set; }
		[DataMember]
		public string DISTR_USE { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string CAR_NO_A { get; set; }
		[DataMember]
		public string CAR_NO_B { get; set; }
		[DataMember]
		public string CAR_NO_C { get; set; }
		[DataMember]
		public string ISSEAL { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string CAN_FAST { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string COUDIV_ID { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string ORD_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string F1909_CUST_NAME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string COUDIV_NAME { get; set; }
		[DataMember]
		public string ZIP_NAME { get; set; }
		[DataMember]
		public string DELV_EFFIC_NAME { get; set; }
		[DataMember]
		public string DELV_TMPR_NAME { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
	}

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_NO")]
	public class P700104WmsNoDetialData
	{
		public bool IsSuccessed { get; set; }
		public string Message { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_NO { get; set; }
		public string RETAIL_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string ADDRESS { get; set; }
		public string CONTACT { get; set; }

		public string CONTACT_TEL { get; set; }

		public int ITEM_QTY { get; set; }

	}

	//費用查詢資料
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE")]
	public class F700701QueryData
	{
		[DataMember]
		public DateTime IMPORT_DATE { get; set; }
		[DataMember]
		public int GRP_ID { get; set; }
		[DataMember]
		public short PERSON_NUMBER { get; set; }
		[DataMember]
		public int WORK_HOUR { get; set; }
		[DataMember]
		public decimal SALARY { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
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

	}

	#region 一般報表
	//各行政區配送失敗狀況
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700101DeliveryFailureData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
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
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string ZIP_NAME { get; set; }
		[DataMember]
		public Decimal FALSECOUNT { get; set; }
		[DataMember]
		public Decimal FALSERATE { get; set; }
		[DataMember]
		public string EDI_RESULT { get; set; }
	}
	//各行政區配送時效
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700101DistributionRate
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string ZIP_NAME { get; set; }
		[DataMember]
		public Double AVG_PAST_DATE { get; set; }
		[DataMember]
		public Double MAX_PAST_DATE { get; set; }
		[DataMember]
		public Double MIN_PAST_DATE { get; set; }
		[DataMember]
		public Double DELIVERYTIMES { get; set; }
		[DataMember]
		public Double FOUR_ARRIVALRATE { get; set; }
		[DataMember]
		public Double EIGHT_ARRIVALRATE { get; set; }
		[DataMember]
		public Double TOWFOUR_ARRIVALRATE { get; set; }
		[DataMember]
		public Double OVER_TOWFOUR_ARRIVALRATE { get; set; }
	}
	#endregion

	#region P710702 - 對帳報表
	/// <summary>
	/// 派車
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700101DistrCarData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public DateTime TAKE_DATE { get; set; }
		[DataMember]
		public string TAKE_TIME { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string CHARGE_CUST_CODE { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		//[DataMember]
		//public Decimal? CAR_KIND_ID { get; set; }
		[DataMember]
		public string ROUTE { get; set; }
		[DataMember]
		public Decimal ITEM_QTY { get; set; }
		[DataMember]
		public Decimal VOLUMN { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public Decimal FEE { get; set; }
		[DataMember]
		public string SP_CAR { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string CAR_KIND_NAME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string IS_SP_CAR { get; set; }
	}
	#endregion

	[Serializable]
	[DataServiceKey("Label")]
	public class P710701RptData
	{
		public double Value { get; set; }
		public int Label { get; set; }

		public string LegendName { get; set; }

	}


	public class F700702ForSchedule
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public DateTime CNT_DATE { get; set; }
		public int CNT1 { get; set; }
		public int CNT2 { get; set; }
		public int CNT3 { get; set; }
		public int CNT4 { get; set; }
		public int CNT5 { get; set; }
		public int CNT6 { get; set; }
		public int CNT7 { get; set; }
		public int CNT8 { get; set; }
		public int CNT9 { get; set; }
		public int CNT10 { get; set; }
		public int CNT11 { get; set; }
		public int CNT12 { get; set; }
		public int CNT13 { get; set; }
		public int CNT14 { get; set; }
		public int CNT15 { get; set; }
		public int CNT16 { get; set; }
		public int CNT17 { get; set; }
		public int CNT18 { get; set; }
		public int CNT19 { get; set; }
		public int CNT20 { get; set; }
		public int CNT21 { get; set; }
		public int CNT22 { get; set; }
		public int CNT23 { get; set; }
		public int CNT24 { get; set; }
	}

	public class F700703ForSchedule
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime CNT_DATE { get; set; }
		public int QTY { get; set; }
		public string CNT_DAY { get; set; }
	}

	public class F700705ForSchedule
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public DateTime CNT_DATE { get; set; }
		public Decimal GRP_ID { get; set; }
		public string EMP_ID { get; set; }
		public string EMP_NAME { get; set; }
		public Decimal ERROR_QTY { get; set; }
		public string CNT_DAY { get; set; }
	}

	public class F700706ForSchedule
	{
		public string DC_CODE { get; set; }
		public DateTime CNT_DATE { get; set; }
		public string CNT_DAY { get; set; }
		public short TIME_QTY { get; set; }
		public short TIME_FINISH_QTY { get; set; }
		public short OPTIMIZE_QTY { get; set; }
		public short OPTIMIZE_FINISH_QTY { get; set; }
	}

	public class F700707ForSchedule
	{
		public string DC_CODE { get; set; }
		public DateTime CNT_DATE { get; set; }
		public string CNT_DAY { get; set; }
		public int QTY { get; set; }
	}

	public class F700708ForSchedule
	{
		public Decimal GRP_ID { get; set; }
		public DateTime CNT_DATE { get; set; }
		public Int32 PERSON_NUMBER { get; set; }
		public Int32 WORK_HOUR { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public short STOCK_QTY { get; set; }
		public short ALLOCATION_QTY { get; set; }
		public short PICK_QTY { get; set; }
		public short PACKAGE_QTY { get; set; }
		public short AUDIT_QTY { get; set; }
		public short INCAR_QTY { get; set; }
		public int TOTAL_QTY { get; set; }
		public Decimal WORK_AVG { get; set; }
		public string CNT_DAY { get; set; }
	}

	public class F700709ForSchedule
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ORD_NO { get; set; }
		public DateTime CNT_DATE { get; set; }
		public int TOTAL_PICK_TIME { get; set; }
		public string CNT_DAY { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class TakeTimeItem
	{
		public decimal ROWNUM { get; set; }
		public string TAKE_TIME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class F050801WithF050301Data : IEncryptable
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
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public Decimal STATUS { get; set; }
		[DataMember]
		public string SPECIAL_BUS { get; set; }
		[DataMember]
		public string NO_LOADING { get; set; }
		[DataMember]
		public Decimal? VOLUMN { get; set; }
		[DataMember]
		public string NO_DELV { get; set; }
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
		//	get
		//	{
		//		return new Dictionary<string, string> { { "ADDRESS", "ADDR" }, { "CONTACT", "NAME" }, { "CONTACT_TEL", "TEL" } };
		//	}
		//}
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P700104ExportData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE_NAME { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string COST_CENTER { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string SP_CAR_NAME { get; set; }
		[DataMember]
		public string CAR_SIZE { get; set; }
		[DataMember]
		public Int32 ITEM_QTY { get; set; }
		[DataMember]
		public string TAKE_DATE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string DISTR_TYPE { get; set; }
		[DataMember]
		public string BACK_PAST_NO { get; set; }
		[DataMember]
		public string BACK_DATE { get; set; }
		[DataMember]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public string DISTR_USE_NAME { get; set; }
		[DataMember]
		public string CHARGE_NAME { get; set; }
		[DataMember]
		public Decimal FEE { get; set; }
		[DataMember]
		public string DISTR_SOURCE_NAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F700101Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public DateTime TAKE_DATE { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string CHARGE_CUST { get; set; }
		[DataMember]
		public string CHARGE_DC { get; set; }
		[DataMember]
		public string CHARGE_GUP_CODE { get; set; }
		[DataMember]
		public string CHARGE_CUST_CODE { get; set; }
		[DataMember]
		public string DISTR_CAR_NO { get; set; }
		[DataMember]
		public DateTime? DELV_DATE { get; set; }
		[DataMember]
		public string DISTR_USE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string CONTACT_TEL { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string DELV_PERIOD { get; set; }
		[DataMember]
		public decimal? VOLUMN { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string CONSIGN_STATUS { get; set; }
		[DataMember]
		public string CONSIGN_NO { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }

	}
}
