using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataContract]
	[DataServiceKey("NAME")]
	public class ScheduleDetail
	{
		[DataMember]
		public string NAME { get; set; }
		[DataMember]
		public string ITEMNUMBER { get; set; }
		[DataMember]
		public string EXTERNORDERKEY { get; set; }
		[DataMember]
		public DateTime D_INSERTTIME { get; set; }
		[DataMember]
		public string NOWFLAG { get; set; }
		[DataMember]
		public string RQ_UUID { get; set; }
		[DataMember]
		public string SOURCE_ID { get; set; }
		[DataMember]
		public int PIECES { get; set; }
		[DataMember]
		public DateTime SEND_DATE { get; set; }
		[DataMember]
		public DateTime UPDATE_DTM { get; set; }
		[DataMember]
		public string STATUS_CODE { get; set; }
		[DataMember]
		public string FEEDBACK_LOG { get; set; }

	}



	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class QuoteData
	{
		public Decimal ROWNUM { get; set; }
		public string QUOTE_NO { get; set; }
		public string ACC_ITEM_KIND_ID { get; set; }
		public string ALL_ID { get; set; }
		public string ACC_ITEM_NAME { get; set; }
		public string ACC_KIND { get; set; }
		public string ACC_UNIT { get; set; }
		public string ACC_UNIT_NAME { get; set; }
		public Int32 ACC_NUM { get; set; }					
		public string DELV_ACC_TYPE { get; set; }
		public string CUST_TYPE { get; set; }	
		public Decimal FEE { get; set; }
		public Decimal? APPROV_FEE { get; set; }
		public Decimal? APPROV_BASIC_FEE { get; set; }
		public Decimal? APPROV_OVER_FEE { get; set; }
		public Decimal? APPROV_OVER_UNIT_FEE { get; set; }
		public decimal? ACC_DELVNUM_ID { get; set; }
		public decimal? DELVNUM { get; set; }
		public string ITEM_TYPE_ID { get; set; }		
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }		
		public Single NET_RATE { get; set; }				
		public string LOGI_TYPE { get; set; }		
		public string IS_SPECIAL_CAR { get; set; }
		public int? CAR_KIND_ID { get; set; }
		public int? ACC_AREA_ID { get; set; }
		public string DELV_TMPR { get; set; }
		public string DELV_EFFIC { get; set; }					
		public Decimal? MAX_WEIGHT { get; set; }		
		public Decimal? OVER_VALUE { get; set; }
		public string ACC_TYPE { get; set; }	
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class SettleData
	{
		public Decimal ROWNUM { get; set; }
		public DateTime CAL_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public DateTime DELV_DATE { get; set; }
		public string WMS_NO { get; set; }
		public string RETAIL_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string PAST_NO { get; set; }
		public short? PACKAGE_BOX_NO { get; set; }
		public decimal BOX_NO { get; set; }
		public Decimal AMT { get; set; }
		public Decimal INVOICE_CNT { get; set; }
		public Decimal SA_QTY { get; set; }
		public Decimal QTY { get; set; }				
		public string ITEM_CODE_BOM { get; set; }		
		public string QUOTE_NO { get; set; }
		public Decimal? APPROV_FEE { get; set; }
		public string PROCESS_ID { get; set; }
		public Int32 ACC_NUM { get; set; }		
		public string ACC_UNIT_NAME { get; set; }
		public string DISTR_CAR_NO { get; set; }
		public string ALL_ID { get; set; }
		public string SP_CAR { get; set; }
		public string CUST_NAME { get; set; }
		public string TAKE_TIME { get; set; }
		public string DISTR_USE { get; set; }
		public string ORD_TYPE { get; set; }
		public string DELV_EFFIC { get; set; }
		public string CAN_FAST { get; set; }
		public string DELV_TMPR { get; set; }
		public string ZIP_CODE { get; set; }
		public decimal? VOLUMN { get; set; }
		public decimal? WEIGHT { get; set; }
		public string DELV_ACC_TYPE { get; set; }
		public string ACC_TYPE { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("LO_MESSAGE_POOL_ID")]
	public class LoMessage
	{

		[DataMember]
		public Decimal LO_MESSAGE_POOL_ID { get; set; }
		[DataMember]
		public string MSG_SUBJECT { get; set; }
		[DataMember]
		public string MESSAGE_CONTENT { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public Decimal? DAYS { get; set; }
		[DataMember]
		public string TARGET_TYPE { get; set; }
		[DataMember]
		public string TARGET_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public List<string> ReceiverMails { get; set; }
		[DataMember]
		public List<string> ReceiverMobiles { get; set; }
		[DataMember]
		public bool IsMail { get; set; }
		[DataMember]
		public bool IsSms { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }

	}


	[DataContract]
	[Serializable]
	//[IgnoreProperties("EncryptedProperties")]
	[DataServiceKey("ROWNUM")]
	public class DeliveryFeeReport
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public DateTime CNT_DATE { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string CUST_COST { get; set; }
		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
        [Encrypted]
        [SecretPersonalData("NAME")]
        public string CONSIGNEE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string COLLECT { get; set; }
		[DataMember]
		public Decimal? COLLECT_AMT { get; set; }
		[DataMember]
		public Decimal? SERVICE_CHARGE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Int32? PACKAGE_QTY { get; set; }
		[DataMember]
        [Encrypted]
        [SecretPersonalData("ADDR")]
        public string ADDRESS { get; set; }
		[DataMember]
		public Decimal? FEE { get; set; }
		[DataMember]
		public string TRANS_TYPE { get; set; }
		[DataMember]
		public string SELF_TAKE { get; set; }
		[DataMember]
		public string SPECIAL_BUS { get; set; }
		[DataMember]
		public Int16 SA_QTY { get; set; }
		[DataMember]
		public Decimal? TOTAL_AMOUNT { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CONTRACT_NO { get; set; }
		[DataMember]
		public string QUOTE_NO { get; set; }

		[IgnoreDataMember]
		public Dictionary<string, string> EncryptedProperties
		{
			get
			{
				return new Dictionary<string, string>
				{
					{"CONSIGNEE", "NAME"},{"ADDRESS", "ADDR"}
				};
			}
		}
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("MESSAGE_ID")]
	public class WMSMessage
	{

		[DataMember]
		public Decimal MESSAGE_ID { get; set; }
		[DataMember]
		public string MSG_SUBJECT { get; set; }
		[DataMember]
		public string MESSAGE_CONTENT { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public Decimal? DAYS { get; set; }
		[DataMember]
		public string TARGET_TYPE { get; set; }
		[DataMember]
		public string TARGET_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public List<string> ReceiverMails { get; set; }
		[DataMember]
		public List<string> ReceiverMobiles { get; set; }
		[DataMember]
		public bool IsMail { get; set; }
		[DataMember]
		public bool IsSms { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }

	}
	
	public class WmsScheduleParam
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
    public string SettleType { get; set; }
    public string SelectDate { get; set; }
    public string SelectEndDate { get; set; }
  }

	public class CustItemStock
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string ItemCode { get; set; }
		public List<ItemLocPriorityInfo> ItemLocPriorityInfos { get; set; }
	}

	/// <summary>
	/// 快速移轉庫存調整單_傳入
	/// </summary>
	public class BatchFlashStockTransferReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 交易編號(不可重複)
		/// </summary>
		public string TransactionNo { get; set; }
		/// <summary>
		/// 商品清單
		/// </summary>
		public List<BatchFlashStockTransferResult> Result { get; set; }
	}

	/// <summary>
	/// 快速移轉庫存調整單商品清單_傳入
	/// </summary>
	public class BatchFlashStockTransferResult
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 調整數量
		/// </summary>
		public int AdjQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public DateTime? ValidDate { get; set; }
		/// <summary>
		/// 驗收批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 序號清單
		/// </summary>
		public List<string> SnList { get; set; }
	}

	/// <summary>
	/// 快速移轉庫存調整單_傳入
	/// </summary>
	public class BatchFlashStockTransferCheck
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string MsgCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string MsgContent { get; set; }
		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LocCode { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ItemCode { get; set; }
	}
	
	public class CheckOutSidePackageDataResult
	{
		public bool IsSuccessed { get; set; }
		public F051201 F051201 { get; set; }
		public F050301 F050301 { get; set; }
		public List<F2501> F2501List { get; set; }
	}

	public class OutSidePackageBoxDetailResult
	{
		public bool IsSuccessed { get; set; }
		public List<OutSidePackageBoxDetail> OutSidePackageBoxDetailList { get; set; }
		public List<F05500101> F05500101List { get; set; }
	}
	public class OutSidePackageBoxDetail
	{
		public F055001 F055001 { get; set; }
		public F050901 F050901 { get; set; }
		public List<F055002> F055002List { get; set; }
		
	}

  public class NameValuePair<T>
  {
    public string Name { get; set; }
    public T Value { get; set; }

    public NameValuePair()
    {
    }

    public NameValuePair(string name, T value)
    {
      this.Name = name;
      this.Value = value;
    }
  }
}
