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
  [DataServiceKey("QUOTE_NO", "ITEM_CODE", "DC_CODE", "GUP_CODE", "CRT_STAFF", "CRT_DATE", "CRT_NAME")]
  public class F910403Data
  {
    [DataMember]
    public string QUOTE_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public System.Int32? STANDARD { get; set; }
    [DataMember]
    public System.Decimal? STANDARD_COST { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CRT_STAFF { get; set; }
    [DataMember]
    public System.DateTime CRT_DATE { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public string UPD_STAFF { get; set; }
    [DataMember]
    public System.DateTime? UPD_DATE { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string CLA_NAME { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class F910101Ex
  {
    [DataMember]
    public System.Decimal ROWNUM { get; set; }
    [DataMember]
    public string BOM_NO { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string BOM_TYPE { get; set; }
    [DataMember]
    public string BOM_NAME { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public string UNIT { get; set; }
    [DataMember]
    public System.Decimal CHECK_PERCENT { get; set; }
    [DataMember]
    public string SPEC_DESC { get; set; }
    [DataMember]
    public string PACKAGE_DESC { get; set; }
    [DataMember]
    public string CRT_STAFF { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public System.DateTime CRT_DATE { get; set; }
    [DataMember]
    public string UPD_STAFF { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
    [DataMember]
    public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string ISPROCESS { get; set; }

	}

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class F910102Ex
  {
    [DataMember]
    public System.Decimal ROWNUM { get; set; }
    [DataMember]
    public string BOM_NO { get; set; }
    [DataMember]
    public string MATERIAL_CODE { get; set; }
    [DataMember]
    public string MATERIAL_NAME { get; set; }
    [DataMember]
    public System.Int16 COMBIN_ORDER { get; set; }
    [DataMember]
    public string ITEM_SIZE { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public string BUNDLE_SERIALNO { get; set; }
    [DataMember]
    public System.Int32 BOM_QTY { get; set; }
    [DataMember]
    public string CRT_STAFF { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public System.DateTime CRT_DATE { get; set; }
    [DataMember]
    public string UPD_STAFF { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
    [DataMember]
    public System.DateTime? UPD_DATE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("PROCESS_ID", "OUTSOURCE_ID")]
  public class F910302WithF1928
  {
    [DataMember]
    public string CONTRACT_TYPE { get; set; }
    [DataMember]
    public string PROCESS_ID { get; set; }
    [DataMember]
    public string OUTSOURCE_ID { get; set; }
    [DataMember]
    public System.Decimal? APPROVE_PRICE { get; set; }
    [DataMember]
    public System.Decimal? TASK_PRICE { get; set; }
    [DataMember]
    public string OUTSOURCE_NAME { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CONTRACT_NO")]
  public class F910301Data
  {
    [DataMember]
    public string CONTRACT_NO { get; set; }
    [DataMember]
    public DateTime ENABLE_DATE { get; set; }
    [DataMember]
    public DateTime DISABLE_DATE { get; set; }
    [DataMember]
    public string OBJECT_TYPE { get; set; }
    [DataMember]
    public string UNI_FORM { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
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
    public Decimal? CYCLE_DATE { get; set; }
    [DataMember]
    public string CLOSE_CYCLE { get; set; }
    [DataMember]
    public string OBJECT_NAME { get; set; }
    [DataMember]
    public string CONTACT { get; set; }
    [DataMember]
    public string TEL { get; set; }
    [DataMember]
    public Decimal? DUEDATE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
  }



  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CONTRACT_NO", "CONTRACT_SEQ")]
  public class F910302Data
  {
    [DataMember]
    public string CONTRACT_NO { get; set; }
    [DataMember]
    public Int32 CONTRACT_SEQ { get; set; }
    [DataMember]
    public string SUB_CONTRACT_NO { get; set; }
    [DataMember]
    public string CONTRACT_TYPE { get; set; }
    [DataMember]
    public DateTime ENABLE_DATE { get; set; }
    [DataMember]
    public DateTime DISABLE_DATE { get; set; }
    [DataMember]
    public string ITEM_TYPE { get; set; }
    [DataMember]
    public string QUOTE_NO { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public Decimal? TASK_PRICE { get; set; }
    [DataMember]
    public Int32? WORK_HOUR { get; set; }
    [DataMember]
    public string PROCESS_ID { get; set; }
    [DataMember]
    public Decimal? OUTSOURCE_COST { get; set; }
    [DataMember]
    public Decimal? APPROVE_PRICE { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
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
    public string CONTRACT_FEE { get; set; }
    [DataMember]
    public string CONTRACT_TYPE1 { get; set; }
    [DataMember]
    public string CONTRACT_TYPENAME { get; set; }
    [DataMember]
    public string QUOTE_NAME { get; set; }
    [DataMember]
    public string ITEM_TYPE_NAME { get; set; }
    [DataMember]
    public string UNIT { get; set; }
    [DataMember]
    public string PROCESS_ACT { get; set; }
  }


  [DataContract]
  [Serializable]
  [DataServiceKey("QUOTE_NAME")]
  public class F910401Report
  {
    [DataMember]
    public string QUOTE_NO { get; set; }
    [DataMember]
    public string QUOTE_NAME { get; set; }
    [DataMember]
    public System.Decimal APPLY_PRICE { get; set; }
    [DataMember]
    public string OUTSOURCE_NAME { get; set; }
    [DataMember]
    public string CONTACT { get; set; }
  }



  [DataContract]
  [Serializable]
  [DataServiceKey("PROCESS_ID")]
  public class F910402Report
  {
    [DataMember]
    public string PROCESS_ID { get; set; }
    [DataMember]
    public string PROCESS_ACT { get; set; }
    [DataMember]
    public string UNIT { get; set; }
  }


  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE")]
  public class F910403Report
  {
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string UNIT { get; set; }
  }

  /// <summary>
  /// 動作分析
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "QUOTE_NO", "PROCESS_ID")]
  public class F910402Detail
  {
    [DataMember]
    public string QUOTE_NO { get; set; }
    [DataMember]
    public string PROCESS_ID { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public System.Int32 WORK_HOUR { get; set; }
    [DataMember]
    public System.Double? WORK_COST { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string PROCESS_ACT { get; set; }
    [DataMember]
    public string UNIT { get; set; }
  }

  /// <summary>
  /// 耗材統計
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "QUOTE_NO", "ITEM_CODE")]
  public class F910403Detail
  {
    [DataMember]
    public string QUOTE_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public System.Int32 STANDARD { get; set; }
    [DataMember]
    public System.Double? STANDARD_COST { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string UNIT { get; set; }
    [DataMember]
    public string LTYPE { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("GUP_CODE", "CUST_CODE", "ITEM_CODE", "BOM_NO")]
  public class F910101Ex2
  {
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string GUP_NAME { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string CUST_NAME { get; set; }
    [DataMember]
    public string BOM_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string BOM_TYPE { get; set; }
    [DataMember]
    public string BOM_NAME { get; set; }
    [DataMember]
    public string BOM_TYPE_NAME { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public string UNIT { get; set; }
  }

  /// <summary>
  /// 取得揀料庫存和所需數量
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class BomQtyData
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
    public string ITEM_CODE_BOM { get; set; }
    [DataMember]
    public string MATERIAL_CODE { get; set; }
    [DataMember]
    public Int32 BOM_QTY { get; set; }
    [DataMember]
    public Int32 PROCESS_QTY { get; set; }
    [DataMember]
    public Decimal? NEED_QTY { get; set; }
    [DataMember]
    public Decimal? AVAILABLE_QTY { get; set; }
    [DataMember]
    public string WAREHOUSE_TYPE { get; set; }
    [DataMember]
    public Decimal ROWNUM { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE")]
  public class ProcessItem
  {
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public string ITEM_SIZE { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
  }

  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CONTRACT_NO")]
  public class F910301Report
  {
    [DataMember]
    public string CONTRACT_NO { get; set; }
    [DataMember]
    public string MAIN_ENABLE_DATE { get; set; }
    [DataMember]
    public string MAIN_DISABLE_DATE { get; set; }
    [DataMember]
    public string OBJECT_NAME { get; set; }
    [DataMember]
    public string CONTACT { get; set; }
    [DataMember]
    public string TEL { get; set; }
    [DataMember]
    public string UNI_FORM { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string CONTRACT_TYPENAME { get; set; }
    [DataMember]
    public string SUB_CONTRACT_NO { get; set; }
    [DataMember]
    public string ITEM_TYPE_NAME { get; set; }
    [DataMember]
    public string QUOTE_NAME { get; set; }
    [DataMember]
    public string UNIT { get; set; }
    [DataMember]
    public string WORK_HOUR { get; set; }
    [DataMember]
    public string TASK_PRICE { get; set; }
    [DataMember]
    public string ENABLE_DATE { get; set; }
    [DataMember]
    public string DISABLE_DATE { get; set; }
    [DataMember]
    public string PROCESS_ACT { get; set; }
    [DataMember]
    public string OUTSOURCE_COST { get; set; }
    [DataMember]
    public string APPROVE_PRICE { get; set; }
  }

  /// <summary>
  /// P9101010200選擇生產線
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("PRODUCE_NO")]
  public class F910004Data
  {
    [DataMember]
    public string PRODUCE_NO { get; set; }
    [DataMember]
    public string PRODUCE_NAME { get; set; }
    [DataMember]
    public string PRODUCE_DESC { get; set; }
    [DataMember]
    public System.Decimal? PROCCNT { get; set; }
    [DataMember]
    public System.Decimal? SUMPROCQTY { get; set; }
  }

  /// <summary>
  /// 給P910103主視窗查詢結果用的資料集
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("PROCESS_NO")]
  public class P910103Data
  {
    [DataMember]
    public DateTime CRT_DATE { get; set; }
    [DataMember]
    public string PROCESS_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public Int32 PROCESS_QTY { get; set; }
    [DataMember]
    public string BOM_TYPE { get; set; }
    [DataMember]
    public string ITEM_CODE_BOM { get; set; }
    [DataMember]
    public string ITEM_NAME_BOM { get; set; }
    [DataMember]
    public DateTime FINISH_DATE { get; set; }
    [DataMember]
    public string STATUS { get; set; }
  }


  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class SerialCheckLog
  {
    public System.Decimal ROWNUM { get; set; }
    public System.DateTime CRT_DATE { get; set; }
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }

    public string PROCESS_NO { get; set; }
    public string ITEM_CODE { get; set; }
    public string ITEM_NAME { get; set; }
    public string ITEM_SERIAL_NO { get; set; }
    public System.Int32 PROCESS_QTY { get; set; }
    public string BOM_NO { get; set; }
    public string BOM_TYPE { get; set; }
    public string BOM_NAME { get; set; }
    public System.DateTime FINISH_DATE { get; set; }
    public string STATUS_TEXT { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("BOM_NO", "ITEM_CODE", "MATERIAL_CODE", "COMBIN_ORDER")]
  public class DisassembleData
  {
    [DataMember]
    public string BOM_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string MATERIAL_CODE { get; set; }
    [DataMember]
    public System.Int16 COMBIN_ORDER { get; set; }
    [DataMember]
    public System.Int32 BOM_QTY { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string BUNDLE_SERIALNO { get; set; }
    [DataMember]
    public string EAN_CODE1 { get; set; }
    [DataMember]
    public string EAN_CODE2 { get; set; }
    [DataMember]
    public string EAN_CODE3 { get; set; }
    [DataMember]
    public decimal? LOG_SEQ { get; set; }
		[DataMember]
		public string SERIALNO { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
	}

  /// <summary>
  /// 回倉歷史明細
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("BACK_NO", "ITEM_CODE", "STATUS", "BACK_ITEM_TYPE")]
  public partial class BackData
  {
    [DataMember]
    public string ALLOCATION_NO { get; set; }
    [DataMember]
    public Int64 BACK_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
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
    public Int64 GOOD_BACK_QTY { get; set; }
    [DataMember]
    public Int64 BREAK_BACK_QTY { get; set; }
    [DataMember]
    public string BACK_ITEM_TYPE { get; set; }
    [DataMember]
    public int A_TAR_QTY { get; set; }
    }

  public partial class BackData
  {
    /// <summary>
    /// 操作狀態:0無變化，1新增，2修改，3刪除
    /// </summary>
    [DataMember]
    public int OperateStatus { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey()]
  public class ProcessAction
  {
    [DataMember]
    public string ACTION_NO { get; set; }
    [DataMember]
    public Decimal? SORT { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE", "SERIAL_NO")]
  public class ItemSumQty
  {
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public Decimal? SumQty { get; set; }
    [DataMember]
    public string BOX_NO { get; set; }
    [DataMember]
    public string CASE_NO { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey()]
  public class BackItemSumQty
  {
    [DataMember]
    public Int64 BACK_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string BACK_ITEM_TYPE { get; set; }
    [DataMember]
    public string IS_GOOD { get; set; }
    [DataMember]
    public Decimal? SUMQTY { get; set; }
    [DataMember]
    public string ALLOCATION_NO { get; set; }
    }

  #region P7105020000 作業計價
  [DataContract]
  [Serializable]
  [DataServiceKey("ACC_ITEM_KIND_ID")]
  public class F91000301Data
  {
    [DataMember]
    public string ACC_ITEM_KIND_ID { get; set; }
    [DataMember]
    public string ACC_ITEM_KIND_NAME { get; set; }
  }
  #endregion

  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE", "WAREHOUSE_TYPE", "WAREHOUSE_ID")]
  public class PickData
  {
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ITEM_SIZE { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
    [DataMember]
    public string WAREHOUSE_TYPE { get; set; }
    [DataMember]
    public string TYPE_NAME { get; set; }
    [DataMember]
    public string WAREHOUSE_ID { get; set; }
    [DataMember]
    public string WAREHOUSE_NAME { get; set; }
    [DataMember]
    public int QTY { get; set; }
    [DataMember]
    public int A_PROCESS_QTY { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "PROCESS_NO")]
  public class P910101Report
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string OUTSOURCE_NAME { get; set; }
    [DataMember]
    public string PROCESS_NO { get; set; }
    [DataMember]
    public DateTime FINISH_DATE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ITEM_CODE_BOM { get; set; }
    [DataMember]
    public string ITEM_NAME_BOM { get; set; }
    [DataMember]
    public int PROCESS_QTY { get; set; }
    [DataMember]
    public int A_PROCESS_QTY { get; set; }
    [DataMember]
    public int BOX_QTY { get; set; }
    [DataMember]
    public int CASE_QTY { get; set; }
    [DataMember]
    public string ORDER_NO { get; set; }
    [DataMember]
    public int UNFINISHED_QTY { get; set; }
    [DataMember]
    public int BREAK_QTY { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string OUTSOURCE_ID { get; set; }
    [DataMember]
    public string OUTSOURCE { get; set; }
	[DataMember]
	public string QUOTE_NO { get; set; }
    [DataMember]
	public string QUOTE_NAME { get; set; }
	public DateTime CRT_DATE { get; set; }
	[DataMember]
	public string CRT_NAME { get; set; }
	[DataMember]
	public string PROCESS_SOURCE { get; set; }


  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class CaseBoxData
  {
		[DataMember]
    public decimal ROWNUM { get; set; }
		[DataMember]
    public string CASE_NO { get; set; }
		[DataMember]
    public string BOX_NO { get; set; }
		[DataMember]
    public decimal? WEIGHT { get; set; }
		[DataMember]
		public List<string> BOX_NO_LIST { get; set; }
		[DataMember]
    public int QTY { get; set; }		
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
  }

	[DataContract]
	[Serializable]
	[DataServiceKey("BOX_NO")]
	public class BoxNo
	{
		[DataMember]
		public string BOX_NO { get; set; }		
	}

  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "PROCESS_NO", "LOG_SEQ")]
  public class SerialCheckData
  {
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string PROCESS_NO { get; set; }
    public int LOG_SEQ { get; set; }
    public string PROCESS_IP { get; set; }
    public string SERIAL_STATUS { get; set; }
    public string MESSAGE { get; set; }
    public string ITEM_CODE { get; set; }
    public string ITEM_NAME { get; set; }
    public string SERIAL_NO { get; set; }
    public string STATUS { get; set; }
    public string ISPASS { get; set; }
    public string BOX_NO { get; set; }
    public string CASE_NO { get; set; }
    public string CELL_NUM { get; set; }
    public int? COMBIN_NO { get; set; }
    public double? WEIGHT { get; set; }
	public string ITEM_SIZE { get; set; }
	public string ITEM_COLOR { get; set; }
	}

  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "PROCESS_NO")]
  public class SerialStatistic
  {
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string PROCESS_NO { get; set; }
    /// <summary>
    /// 應刷已通過數(本盒/箱應刷已通過數)
    /// </summary>
    public int ValidProcessCnt { get; set; }
    /// <summary>
    /// 本機刷讀數(不分是否通過)
    /// </summary>
    public int CurrentCount { get; set; }
    /// <summary>
    /// 本機實刷(通過)數
    /// </summary>
    public int ValidCount { get; set; }
    /// <summary>
    /// 本機刷讀錯誤數
    /// </summary>
    public int InvalidCount { get; set; }
    /// <summary>
    /// 本機實刷(通過)成品數
    /// </summary>
    public int CurrentIpValidCount { get; set; }
    /// <summary>
    /// 累積實刷(通過)成品數
    /// </summary>
    public int TotalValidCount { get; set; }
  }

  #region P7105050000 派車計費
  [DataContract]
  [Serializable]
  [DataServiceKey("ACC_UNIT")]
  public class F91000302Data
  {
    [DataMember]
    public string ACC_UNIT { get; set; }
    [DataMember]
    public string ACC_UNIT_NAME { get; set; }
  }
  #endregion

  #region P710405流通加工看板
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class ProduceLineStatusItem
  {
    public decimal ROWNUM { get; set; }
    public string PRODUCE_NO { get; set; }

    public int FINISHCOUNT { get; set; }

    public int UNFINISHCOUNT { get; set; }
  }
  #endregion

  #region P710702 對帳報表
  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class F910201ProcessData
  {
    [DataMember]
    public Decimal ROWNUM { get; set; }
    [DataMember]
    public DateTime CRT_DATE { get; set; }
    [DataMember]
    public string PROCESS_NO { get; set; }
    [DataMember]
    public string PROCESS_ID { get; set; }
    [DataMember]
    public string PROCESS_ACT { get; set; }
    [DataMember]
    public Int32 PROCESS_QTY { get; set; }
    [DataMember]
    public string UNIT_ID { get; set; }
    [DataMember]
    public string ACC_ITEM_KIND_NAME { get; set; }
    [DataMember]
    public Decimal OUTSOURCE_COST { get; set; }
    [DataMember]
    public Decimal SUBTOTAL { get; set; }

		public Decimal TOTALWORKHOURS { get; set; }
  }
  #endregion

  #region 流通加工維護 - 開立揀料單
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class PickReport
  {
    public decimal ROWNUM { get; set; }
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string PROCESS_NO { get; set; }
    public string PICK_NO { get; set; }
    public string ALLOCATION_NO { get; set; }
    public string ITEM_CODE { get; set; }
    public string ITEM_NAME { get; set; }
    public string ITEM_SIZE { get; set; }
    public string ITEM_SPEC { get; set; }
    public string ITEM_COLOR { get; set; }
    public string VALID_DATE { get; set; }
    public string PICK_LOC { get; set; }
    public decimal PICK_QTY { get; set; }
    public string SERIAL_NO { get; set; }
    public string WAREHOUSE_ID { get; set; }
    public string WAREHOUSE_NAME { get; set; }
    public string TMPR_TYPE { get; set; }
    public string NAME { get; set; }
    public string FLOOR { get; set; }
	public string ItemCodeBarcode { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    public string MAKE_NO { get; set; }
  }

  #endregion

  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class F91000302SearchData
  {
    public Decimal ROWNUM { get; set; }
    public string ITEM_TYPE_ID { get; set; }
    public string ACC_UNIT { get; set; }
    public string ACC_UNIT_NAME { get; set; }
    public string CRT_STAFF { get; set; }
    public string CRT_NAME { get; set; }
    public DateTime CRT_DATE { get; set; }
    public string UPD_STAFF { get; set; }
    public string UPD_NAME { get; set; }
    public DateTime? UPD_DATE { get; set; }
    public string ITEM_TYPE { get; set; }
  }

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ProcessLabel
	{
		public Decimal ROWNUM { get; set; }
		public string PROCESS_NO { get; set; }
		public string ACTION_NO { get; set; }
		public string LABEL_NO { get; set; }
		public Int32 ORDER_BY { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_STAFF { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string UPD_NAME { get; set; }
		public string LABEL_TYPE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P91010105Report
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
		[DataMember]
		public string TAR_WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string SUG_LOC_CODE { get; set; }
		[DataMember]
		public Decimal TAR_QTY { get; set; }

		[DataMember]
		public string AllocationNoBarcode { get; set; }
		[DataMember]
		public string ItemCodeBarcode { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE","GUP_CODE","CONTRACT_NO")]
	public class ContractSettleData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CONTRACT_NO { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public Decimal? CYCLE_DATE { get; set; }
		[DataMember]
		public string CLOSE_CYCLE { get; set; }
		[DataMember]
		public string SUB_CONTRACT_NO { get; set; }
		[DataMember]
		public string ITEM_TYPE { get; set; }
		[DataMember]
		public string QUOTE_NO { get; set; }
		[DataMember]
		public string UNIT_ID { get; set; }
		[DataMember]
		public Decimal? TASK_PRICE { get; set; }
		[DataMember]
		public Int32? WORK_HOUR { get; set; }
		[DataMember]
		public string PROCESS_ID { get; set; }
		[DataMember]
		public Decimal? OUTSOURCE_COST { get; set; }
		[DataMember]
		public Decimal? APPROVE_PRICE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class SettleReportData
	{
		public decimal ROWNUM { get; set; }
		public DateTime CNT_DATE { get; set; }
		public string CONTRACT_NO { get; set; }
		public string QUOTE_NO { get; set; }
		public string ITEM_TYPE_ID { get; set; }
		public string ACC_ITEM_NAME { get; set; }
		public int? PRICE { get; set; }
		public int? PRICE_CNT { get; set; }
		public Decimal? COST { get; set; }
		public Decimal? AMOUNT { get; set; }
		public string IS_TAX { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string OUTSOURCE_ID { get; set; }
		public DateTime CNT_DATE_S { get; set; }
		public Decimal? CYCLE_DATE { get; set; }
		public string ITEM_TYPE { get; set; }
	}

	public class BomItemDetail
	{
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string MATERIAL_CODE { get; set; }
		public int BOM_QTY { get; set; }

	}

	public class F910102Data
	{
		public string MATERIAL_CODE { get; set; }
        public string ITEM_NAME { get; set; }

        public int BOM_QTY { get; set; }
		public int ITEM_QTY { get; set; }
		public string MULTI_FLAG { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class P910102Data
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

		public string PROCESS_NO { get; set; }
		[DataMember]

		public string PROCESS_SOURCE { get; set; }
		[DataMember]

		public string PROCESS_ITEM { get; set; }
		[DataMember]

		public string PROCESS_ITEM_NAME { get; set; }
		[DataMember]

		public decimal? PACK_QTY { get; set; }
		[DataMember]

		public string ORDER_NO { get; set; }
		[DataMember]

		public string QUOTE_NO { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public string ITEM_UNIT { get; set; }
		[DataMember]

		public string ITEM_NAME { get; set; }
		[DataMember]

		public decimal PROCESS_QTY { get; set; }
		[DataMember]

		public string GOOD_CODE { get; set; }
		[DataMember]

		public string GOOD_UNIT { get; set; }
		[DataMember]

		public string GOOD_NAME { get; set; }
		[DataMember]

		public decimal GOOD_QTY { get; set; }
		[DataMember]

		public string MEMO { get; set; }
		[DataMember]

		public string STATUS { get; set; }
		[DataMember]

		public string STATUS_NAME { get; set; }
		[DataMember]

		public string PROC_STATUS { get; set; }
		[DataMember]

		public string PROC_STATUS_NAME { get; set; }
		[DataMember]

		public DateTime? PROC_BEGIN_TIME { get; set; }
		[DataMember]

		public DateTime? PROC_END_TIME { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class P910102PickData
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

		public string PROCESS_NO { get; set; }
		[DataMember]
		public string PICK_NO { get; set; }
		[DataMember]

		public string WAREHOUSE_ID { get; set; }
		[DataMember]

		public string WAREHOUSE_NAME { get; set; }
		[DataMember]

		public string ALLOCATION_NO { get; set; }
		[DataMember]

		public string STATUS { get; set; }
		[DataMember]

		public string STATUS_NAME { get; set; }
		[DataMember]

		public decimal PICK_QTY { get; set; }
		[DataMember]

		public string PICK_LOC { get; set; }
		[DataMember]

		public DateTime VALID_DATE { get; set; }
		[DataMember]

		public DateTime ENTER_DATE { get; set; }
		[DataMember]

		public string MAKE_NO { get; set; }
		[DataMember]

		public string BOX_CTRL_NO { get; set; }
		[DataMember]

		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class P910102ProcessRecord
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

		public string PROCESS_NO { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string PROCESS_ACTION { get; set; }
		[DataMember]
		public string PROCESS_ACTION_NAME { get; set; }
		[DataMember]
		public decimal GOOD_QTY { get; set; }
		[DataMember]
		public decimal SUM_GOOD_QTY { get; set; }
		[DataMember]
		public string ORGINAL_CHANGE_QTY { get; set; }
		[DataMember]
		public decimal ORGINAL_REMAIN_QTY { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
	  [DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class P910102Stock
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

		public string WAREHOUSE_ID { get; set; }
		[DataMember]

		public string WAREHOUSE_NAME { get; set; }

		[DataMember]

		public decimal STOCK_QTY { get; set; }

		[DataMember]

		public decimal? PICK_QTY { get; set; }
		[DataMember]

		public string LOC_CODE { get; set; }
		[DataMember]

		public DateTime VALID_DATE { get; set; }
		[DataMember]

		public DateTime ENTER_DATE { get; set; }
		[DataMember]

		public string MAKE_NO { get; set; }
		[DataMember]

		public string BOX_CTRL_NO { get; set; }
		[DataMember]

		public string PALLET_CTRL_NO { get; set; }
		[DataMember]

		public string SERIAL_NO { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	[DataContract]
	public class P910102PickNotReturnData
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

		public string PROCESS_NO { get; set; }
		[DataMember]
		public string PICK_NO { get; set; }
		[DataMember]

		public string WAREHOUSE_ID { get; set; }
		[DataMember]

		public string WAREHOUSE_NAME { get; set; }
		
		[DataMember]

		public decimal PICK_QTY { get; set; }
		[DataMember]
		public decimal ORGINAL_REMAIN_QTY { get; set; }
		[DataMember]
		public decimal? GOOD_QTY { get; set; }
		[DataMember]
		public string TAR_WAREHOUSE_ID { get; set; }
		[DataMember]

		public string PICK_LOC { get; set; }
		[DataMember]

		public DateTime VALID_DATE { get; set; }
		[DataMember]

		public DateTime ENTER_DATE { get; set; }
		[DataMember]

		public string MAKE_NO { get; set; }
		[DataMember]

		public string BOX_CTRL_NO { get; set; }
		[DataMember]

		public string PALLET_CTRL_NO { get; set; }
		[DataMember]

		public string SERIAL_NO { get; set; }


	}

	[Serializable]
	[DataServiceKey("DC_CODE", "DEVICE_IP")]
	[DataContract]
	public class DeviceData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DEVICE_IP { get; set; }
		[DataMember]
		public string LABELING { get; set; }
		[DataMember]
		public string PRINTER { get; set; }
		[DataMember]
		public string MATRIX_PRINTER { get; set; }
		[DataMember]
		public string WORKSTATION_CODE { get; set; }
		[DataMember]
		public string WORKSTATION_TYPE { get; set; }
		[DataMember]
		public string WORKSTATION_GROUP { get; set; }
	}

	[Serializable]
	[DataServiceKey("DC_CODE", "WORKSTATION_CODE")]
	[DataContract]
	public class WorkstationData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string WORKSTATION_GROUP { get; set; }
		[DataMember]
		public string WORKSTATION_TYPE { get; set; }
		[DataMember]
		public string WORKSTATION_CODE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
	}
}

