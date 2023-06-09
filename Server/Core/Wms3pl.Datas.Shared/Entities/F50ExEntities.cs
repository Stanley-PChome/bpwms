using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.Shared.Entities
{

	#region F500101 計價查詢主檔 - 倉租
	[DataContract]
	[Serializable]
	[DataServiceKey("QUOTE_NO", "DC_CODE")]
	public class F500101QueryData
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
		public string QUOTE_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public decimal NET_RATE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public string LOC_TYPE_ID { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public int ACC_NUM { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public decimal UNIT_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_UNIT_FEE { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string UPLOAD_FILE { get; set; }
	}
	#endregion

	#region F500104 計價查詢主檔 - 作業
	[DataContract]
	[Serializable]
	[DataServiceKey("QUOTE_NO", "DC_CODE")]
	public class F500104QueryData
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
		public string QUOTE_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public decimal NET_RATE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public int ACC_NUM { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public decimal FEE { get; set; }
		[DataMember]
		public decimal BASIC_FEE { get; set; }
		[DataMember]
		public decimal OVER_FEE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public decimal? APPROV_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_BASIC_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_OVER_FEE { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string UPLOAD_FILE { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
	}
	#endregion

	#region F500103 計價查詢主檔 - 出貨
	[DataContract]
	[Serializable]
	[DataServiceKey("QUOTE_NO", "DC_CODE")]
	public class F500103QueryData
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
		public string QUOTE_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public decimal NET_RATE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public int ACC_NUM { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public decimal FEE { get; set; }
		[DataMember]
		public decimal BASIC_FEE { get; set; }
		[DataMember]
		public decimal OVER_FEE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public decimal? APPROV_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_BASIC_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_OVER_FEE { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string UPLOAD_FILE { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
	}
	#endregion

	#region F500102 計價查詢主檔 - 運費
	[DataContract]
	[Serializable]
	[DataServiceKey("QUOTE_NO", "DC_CODE")]
	public class F500102QueryData
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
		public string QUOTE_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public decimal NET_RATE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }		
		[DataMember]
		public string LOGI_TYPE { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public string IS_SPECIAL_CAR { get; set; }
		[DataMember]
		public int? CAR_KIND_ID { get; set; }
		[DataMember]
		public int? ACC_AREA_ID { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public decimal ACC_NUM { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public decimal? MAX_WEIGHT { get; set; }
		[DataMember]
		public decimal FEE { get; set; }
		[DataMember]
		public decimal? OVER_VALUE { get; set; }
		[DataMember]
		public decimal? OVER_UNIT_FEE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public decimal? APPROV_FEE { get; set; }
		[DataMember]
		public decimal? APPROV_OVER_UNIT_FEE { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string UPLOAD_FILE { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
	}
	#endregion

	#region F500105 計價查詢主檔 - 其他
	[DataContract]
	[Serializable]
	[DataServiceKey("QUOTE_NO", "DC_CODE")]
	public class F500105QueryData
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
		public string QUOTE_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public decimal NET_RATE { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public int ACC_NUM { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public decimal FEE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public decimal? APPROV_FEE { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string UPLOAD_FILE { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
	}
	#endregion

	#region F500201 結算作業
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F500201ClearingData
	{
		public Decimal ROWNUM { get; set; }
		public DateTime CNT_DATE { get; set; }
		public string CONTRACT_NO { get; set; }
		public string QUOTE_NO { get; set; }
		public string ITEM_TYPE_ID { get; set; }
		public string ITEM_TYPE { get; set; }
		public string ACC_ITEM_NAME { get; set; }		
		public Decimal? COST { get; set; }
		public Decimal? AMOUNT { get; set; }
		public string IS_TAX { get; set; }

		public string STATUS { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP7105100001
	{
		public Decimal ROWNUM { get; set; }
		public string GUP_NAME { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string QUOTE_NO { get; set; }
		public string DETAIL { get; set; }
		public Decimal NOTAX { get; set; }
		public Decimal TAX { get; set; }
		public Decimal AMOUNT { get; set; }
		public string CNT_DATE_RANGE { get; set; }
		public string GROUPNAME { get; set; }
		public string ITEM_TYPE_ID { get; set; }
	}
	[Serializable]	
	[DataServiceKey("ROWNUM")]
	//[IgnoreProperties("EncryptedProperties")]
	public class RP7105100002 : IEncryptable
	{
		public Decimal ROWNUM { get; set; }
		public string COST_CENTER { get; set; }
		public string DELV_DATE { get; set; }
		public string CUST_NAME { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string PAST_NO { get; set; }
		public string WMS_ORD_NO { get; set; }
		public string COLLECT_DESC { get; set; }
		public Decimal COLLECT_AMT { get; set; }
		public Decimal SERVICE_CHARGE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public Decimal A_DELV_QTY { get; set; }
        [Encrypted]
        [SecretPersonalData("ADDR")]
        public string ADDRESS { get; set; }
		public string TRANS_TYPE_DESC { get; set; }
		public Decimal UNIONFEE { get; set; }
		public Decimal SPECIALFEE { get; set; }
		public Decimal SA_QTY { get; set; }
		public Decimal FEE { get; set; }
		public Decimal TOTAL_AMOUNT { get; set; }
		public string REMARK { get; set; }
		public string CNT_DATE_RANGE { get; set; }
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get
		//	{
		//		return new Dictionary<string, string> 
  //            {
  //              {"ADDRESS", "ADDR"}
  //            };
		//	}
		//}
	}
	
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP7105100003
	{
		public Decimal ROWNUM { get; set; }
		public string COST_CENTER { get; set; }
		public string PROCESS_DATE { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string WMS_NO { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public Int32 PROCESS_QTY { get; set; }
		public Decimal PRICE { get; set; }
		public Decimal FEE { get; set; }
		public string CAUSE { get; set; }
		public string CNT_DATE_RANGE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP7105100004
	{
		public Decimal ROWNUM { get; set; }
		public string COST_CENTER { get; set; }
		public string CRT_WMS_DATE { get; set; }
		public string CUST_ORD_NO { get; set; }
		public string WMS_NO { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string CAUSE { get; set; }
		public int PROCESS_QTY { get; set; }
		public Decimal PRICE { get; set; }
		public Decimal TOTAL { get; set; }
		public string CNT_DATE_RANGE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RP7105100005
	{
		public Decimal ROWNUM { get; set; }
		public string COST_CENTER { get; set; }
		public string TAKE_DATE { get; set; }
		public string DISTR_DATE { get; set; }
		public string PAST_NO { get; set; }
		public string CUST_NAME { get; set; }
		public string CAUSE { get; set; }
		public Decimal ACC_NUM { get; set; }
		public Decimal FEE { get; set; }
		public string CNT_DATE_RANGE { get; set; }
	}

	#endregion

	#region 取基期
	[Serializable]
	[DataContract]
	[DataServiceKey("DC_CODE")]
	public class BaseDay
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public int BASE_DAY { get; set; }

	}
	#endregion
}
