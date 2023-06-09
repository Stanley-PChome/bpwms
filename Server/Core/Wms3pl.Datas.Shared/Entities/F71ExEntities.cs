using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710802SearchResult
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public DateTime? ChangeDate { get; set; }
		[DataMember]
		public string ReceiptType { get; set; }
		[DataMember]
		public string ReceiptNo { get; set; }
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
		public string SOURCE_LOC { get; set; }
		[DataMember]
		public string TARGET_LOC { get; set; }
		[DataMember]
		public int ChangeNumber { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }

    /// <summary>
    /// 作業時間
    /// </summary>
    [DataMember]
    public DateTime PROC_TIME { get; set; }
    /// <summary>
    /// 原調撥單號
    /// </summary>
    [DataMember]
    public string ALLOCATION_NO { get; set; }
  }

  #region 訂單處理進度查詢
  [DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F051201Progress
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
		public DateTime? DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_PERIOD { get; set; }
		[DataMember]
		public string PACKAGE_PERIOD { get; set; }
		[DataMember]
		public DateTime? RETURN_DATE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F050301ProgressData
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
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public DateTime? APPROVE_DATE { get; set; }
		[DataMember]
		public DateTime? INCAR_DATE { get; set; }
		[DataMember]
		public string PAST_NO { get; set; }
	}

	#endregion

	#region 儲位明細資料
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710101DetailData
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public int IsEdit { get; set; }
		[DataMember]
		public bool IsEditData { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string CHANNEL { get; set; }
		[DataMember]
		public string PLAIN { get; set; }
		[DataMember]
		public string LOC_LEVEL { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }

	}
	#endregion

	[Serializable]
	[DataServiceKey("TOPIC","SUBTOPIC","VALUE")]
	public class P710601LangData
	{
		public string TOPIC { get; set; }
		public string SUBTOPIC { get; set; }
		public string SUB_NAME { get; set; }
		public string VALUE { get; set; }
		public string NAME { get; set; }
		public string LANGNAME { get; set; }
		public string LANG { get; set; }
	}

    /// <summary>
    /// 進倉驗收檔
    /// </summary>
    [Serializable]
    [DataServiceKey("RT_NO", "RT_SEQ", "DC_CODE", "GUP_CODE", "CUST_CODE")]
    public class F020201Data
    {
        [DataMember]
        public string RT_NO { get; set; }
        [DataMember]
        public string RT_SEQ { get; set; }
        [DataMember]
        public string PURCHASE_NO { get; set; }
        [DataMember]
        public string PURCHASE_SEQ { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public DateTime? RECE_DATE { get; set; }
        [DataMember]
        public DateTime? VALI_DATE { get; set; }
        [DataMember]
        public DateTime? MADE_DATE { get; set; }
        [DataMember]
        public Int32? ORDER_QTY { get; set; }
        [DataMember]
        public Int32? RECV_QTY { get; set; }
        [DataMember]
        public Int32? CHECK_QTY { get; set; }
        [DataMember]
        public string F151001_STATUS { get; set; }
        [DataMember]
        public string F010201_STATUS { get; set; }
        [DataMember]
        public string CHECK_ITEM { get; set; }
        [DataMember]
        public string CHECK_SERIAL { get; set; }
        [DataMember]
        public string ISPRINT { get; set; }
        [DataMember]
        public string ISUPLOAD { get; set; }
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
        public string SPECIAL_DESC { get; set; }
        [DataMember]
        public string SPECIAL_CODE { get; set; }
        [DataMember]
        public string ISSPECIAL { get; set; }
        [DataMember]
        public DateTime? IN_DATE { get; set; }
        [DataMember]
        public string TARWAREHOUSE_ID { get; set; }
        [DataMember]
        public string QUICK_CHECK { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
    }

	[Serializable]
	[DataServiceKey("Name")]
	public class NameValueList
		{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Value { get; set; }
		}
}
