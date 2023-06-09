using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Runtime.Serialization;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;

namespace Wms3pl.Datas.Shared.Entities
{
	public class StockCompareDetail
	{
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string ITEM_CODE { get; set; }
		public string VALID_DATE { get; set; }
		public string MAKE_NO { get; set; }
		public int QTY { get; set; }
	}

	public class ContainerDetail
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ALLOCATION_NO { get; set; }
		public int ALLOCATION_SEQ { get; set; }
		public string PRE_TAR_WAREHOUSE_ID { get; set; }
		public string ITEM_CODE { get; set; }
		public int QTY { get; set; }
		public string SERIALNUMLIST { get; set; }
		public string BIN_CODE { get; set; }
		public DateTime COMPLETE_TIME { get; set; }
		public int ISLASTCONTAINER { get; set; }
		public F151002 F151002 { get; set; }

	}

	public class GroupContainerDetail
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string WMS_NO { get; set; }
		public int ROWNUM { get; set; }
		public int ISLASTCONTAINER { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DOC_ID")]
	public class TaskDispatchData
	{
		/// <summary>
		/// 任務單號
		/// </summary>
		[DataMember]
		public string DOC_ID { get; set; }
		/// <summary>
		/// 單據號碼
		/// </summary>
		[DataMember]
		public string WMS_NO { get; set; }
		/// <summary>
		/// 倉別名稱
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 揀貨單號
		/// </summary>
		[DataMember]
		public string PICK_NO { get; set; }
		/// <summary>
		/// 命令類別
		/// </summary>
		[DataMember]
		public string CMD_TYPE_NAME { get; set; }
		/// <summary>
		/// 狀態
		/// </summary>
		[DataMember]
		public string STATUS { get; set; }
		/// <summary>
		/// 狀態(中文)
		/// </summary>
		[DataMember]
		public string STATUS_NAME { get; set; }
		/// <summary>
		/// 傳送時間
		/// </summary>
		[DataMember]
		public DateTime? PROC_DATE { get; set; }
		/// <summary>
		/// 是否有產生複盤資料(0否1是)
		/// </summary>
		[DataMember]
		public string ISSECOND { get; set; }
		/// <summary>
		/// 接收訊息
		/// </summary>
		[DataMember]
		public string MESSAGE { get; set; }
		/// <summary>
		/// 已派送次數
		/// </summary>
		[DataMember]
		public int RESENT_CNT { get; set; }
		/// <summary>
		/// 建立時間
		/// </summary>
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 異動時間
		/// </summary>
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// 原盤點任務單號
		/// </summary>
		[DataMember]
		public string CHECK_CODE { get; set; }
		/// <summary>
		/// 是否要ENABLE
		/// </summary>
		[DataMember]
		public bool ENABLE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DOC_ID")]
	public class F060802Data
	{
		/// <summary>
		/// 流水號
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// 分揀機編號
		/// </summary>
		public string SORTER_CODE { get; set; }
		/// <summary>
		/// 異常類型
		/// </summary>
		public string ABNORMAL_TYPE_NAME { get; set; }
		/// <summary>
		/// 紀錄時間
		/// </summary>
		public string RECORD_TIME { get; set; }
		/// <summary>
		/// 異常訊息
		/// </summary>
		public string ABNORMAL_MSG { get; set; }
		/// <summary>
		/// 異常物流單號
		/// </summary>
		public string ABNORMAL_CODE { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		public DateTime CRT_DATE { get; set; }



	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ID")]
	public class F060801Data
	{
		public Int64 ID { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string ABNORMALTYPE_NAME { get; set; }
		public string SHELFCODE { get; set; }
		public string BINCODE { get; set; }
		public string ORDERCODE { get; set; }
		public string SKUCODE { get; set; }
		public int SKUQTY { get; set; }
		public string OPERATOR { get; set; }
	}

  public class AutoAllotSerialNos
  {
    public string WMS_NO { get; set; }
    public string SERIALNUMLIST { get; set; }
  }

}
