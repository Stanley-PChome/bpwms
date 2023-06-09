using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;

namespace Wms3pl.Datas.Shared.Entities
{
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryDetailItem
	{
		[DataMember]
		public string ChangeStatus { get; set; }

		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsSelected { get; set; }
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
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public int? QTY { get; set; }
		[DataMember]
		public int? FIRST_QTY_ORG { get; set; }
		[DataMember]
		public int? FIRST_QTY { get; set; }
		[DataMember]
		public int? SECOND_QTY_ORG { get; set; }
		[DataMember]
		public int? SECOND_QTY { get; set; }
		[DataMember]
		public string FLUSHBACK_ORG { get; set; }
		[DataMember]
		public string FLUSHBACK { get; set; }
		[DataMember]
		public string FLUSHBACKNAME { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
    [DataMember]
    public string MAKE_NO { get; set; }
		[DataMember]
		public int? UNMOVE_STOCK_QTY { get; set; }
		[DataMember]
		public int? DEVICE_STOCK_QTY { get; set; }
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F140101Expansion
	{
		public Decimal ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
		public string ISCHARGE { get; set; }
		public Decimal? FEE { get; set; }
		public string INVENTORY_TYPE { get; set; }
		public Int16? INVENTORY_CYCLE { get; set; }
		public Int16? INVENTORY_YEAR { get; set; }
		public Int16? INVENTORY_MON { get; set; }
		public Int16? CYCLE_TIMES { get; set; }
		public string SHOW_CNT { get; set; }
		public string STATUS { get; set; }
		public Int64 ITEM_CNT { get; set; }
		public Int64 ITEM_QTY { get; set; }
		public string MEMO { get; set; }
		public DateTime? PRINT_DATE { get; set; }
		public DateTime? POSTING_DATE { get; set; }
		public string ISSECOND { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string STATUS_DESC { get; set; }
		public string INVENTORY_TYPE_DESC { get; set; }
		public string INVENTORY_CYCLE_DESC { get; set; }

		public string CHECK_TOOL { get; set; }
		public string CHECK_TOOL_DESC { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryQueryData
	{
		public Decimal ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
		public DateTime POSTING_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public Int32 PROFIT_QTY { get; set; }
		public Int32 LOSS_QTY { get; set; }
		public string DC_NAME { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_NAME { get; set; }
		public string INVENTORY_TYPE { get; set; }
		public string INVENTORY_TYPE_NAME { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryQueryDataForDc
	{
		public Decimal ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
		public DateTime POSTING_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public Int32 PROFIT_QTY { get; set; }
		public Int32 LOSS_QTY { get; set; }
		public string FLUSHBACK { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string LOC_CODE { get; set; }
		public string SERIAL_NO { get; set; }
		public string DC_NAME { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_UNIT { get; set; }
		public Int64 QTY { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string INVENTORY_TYPE { get; set; }
		public string INVENTORY_TYPE_NAME { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        public string BOX_CTRL_NO { get; set; }
        /// <summary>
        /// 板號、箱號
        /// </summary>
        public string PALLET_BOX_CTRL_NO { get; set; }

    }

	[Serializable]
	[DataServiceKey("INVENTORY_NO", "DC_CODE")]
	public class F140106QueryData
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
		public string STATUS { get; set; }
		public string STATUS_NAME { get; set; }
		public double? ITEMQTY { get; set; }
		public double? QTY { get; set; }
		public double? ITEM_CNT { get; set; }
		public string MEMO { get; set; }
		public string CHECK_TOOL_NAME { get; set; }
		public string CHECK_TOOL { get; set; }
		public string IS_AUTOMATIC { get; set; }
    public string INVENTORY_TYPE { get; set; }
  }
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryByLocDetail
	{
		public Decimal ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
		public string INVENTORY_TYPE_DESC { get; set; }
		public string STATUS_DESC { get; set; }
		public string INVENTORY_TYPE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_UNIT { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string QTY { get; set; }
		public decimal QTY_Num { get; set; }
		public string FIRST_QTY { get; set; }
		public decimal FIRST_QTY_Num { get; set; }
		public string SECOND_QTY { get; set; }
		public decimal SECOND_QTY_Num { get; set; }
		public string DC_NAME { get; set; }
		public string PRINT_STAFF { get; set; }
		public DateTime PRINT_DATE { get; set; }
		public string PRINT_TITLE { get; set; }

		public string UNIT_QTY { get; set; }
		public string SHOW_CNT { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        public string PALLET_CTRL_NOP { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        public string BOX_CTRL_NO { get; set; }
        /// <summary>
        /// 板號箱號放一起
        /// </summary>
        public string PALLET_BOX_CTRL_NO { get; set; }
		/// <summary>
		/// 未搬動數量
		/// </summary>
		public string UNMOVE_STOCK_QTY { get; set; }
		public decimal UNMOVE_STOCK_QTY_Num { get; set; }
		public string CHECK_TOOL_NAME { get; set; }
		public string CUST_ITEM_CODE { get; set; }
		public string EAN_CODE1 { get; set; }
		public string EAN_CODE2 { get; set; }
		public string EAN_CODE3 { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryDetailItemsByIsSecond
	{
		public Decimal ROWNUM { get; set; }
		public bool IsSelected { get; set; }
		public string INVENTORY_NO { get; set; }
		/// <summary>
		/// 初/複盤點日期
		/// </summary>
		public DateTime INVENTORY_DATE { get; set; }
		public string INVENTORY_TYPE_DESC { get; set; }
		public string STATUS_DESC { get; set; }
		public string INVENTORY_TYPE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_UNIT { get; set; }
		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string QTY { get; set; }
		public decimal QTY_Num { get; set; }
		public string FIRST_QTY { get; set; }
		public decimal FIRST_QTY_Num { get; set; }
		public string SECOND_QTY { get; set; }
		public decimal SECOND_QTY_Num { get; set; }
		public string DIFF_FIRST_QTY { get; set; }
		public string DIFF_SECOND_QTY { get; set; }
		public string FST_INVENTORY_DATE { get; set; }
		public string FST_INVENTORY_NAME { get; set; }
		public string SEC_INVENTORY_DATE { get; set; }
		public string SEC_INVENTORY_NAME { get; set; }
		public string FLUSHBACK { get; set; }
		public string DC_NAME { get; set; }
		public string PRINT_STAFF { get; set; }
		public DateTime PRINT_DATE { get; set; }
		public string PRINT_TITLE { get; set; }

		public string UNIT_QTY { get; set; }

		public string SHOW_CNT { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MAKE_NO { get; set; }
    /// <summary>
    ///板號
    /// </summary>
    public string PALLET_CTRL_NO { get; set; }
    /// <summary>
    /// 箱號
    /// </summary>
    public string BOX_CTRL_NO { get; set; }
    /// <summary>
    /// 板號、箱號
    /// </summary>
    public string PALLET_BOX_CTRL_NO { get; set; }
		/// <summary>
		/// 初/複盤點數量
		/// </summary>
		public string INVENTORY_QTY { get; set; }
		/// <summary>
		/// 初/複盤差數
		/// </summary>
		public string DIFF_QTY { get; set; }
		/// <summary>
		/// 初/複盤點人員
		/// </summary>
		public string INVENTORY_NAME { get; set; }
		/// <summary>
		/// 初/複WMS庫差數
		/// </summary>
		public string STOCK_DIFF_QTY { get; set; }
		/// <summary>
		/// 未搬移數量
		/// </summary>
		public string UNMOVE_STOCK_QTY { get; set; }
		public decimal UNMOVE_STOCK_QTY_Num { get; set; }
		/// <summary>
		/// 自動倉庫存數
		/// </summary>
		public string DEVICE_STOCK_QTY { get; set; }
		/// <summary>
		/// 盤盈/損 產生的調整單號/調撥單號
		/// </summary>
		public string PROC_WMS_NO { get; set; }
		public string CUST_ITEM_CODE { get; set; }
		public string EAN_CODE1 { get; set; }
		public string EAN_CODE2 { get; set; }
		public string EAN_CODE3 { get; set; }
		public string CHECK_TOOL_NAME { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P140102ReportData
	{
		public string ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public string INVENTORY_DATE { get; set; }
		public string INVENTORY_TYPE_DESC { get; set; }
		public string CHECK_TOOL_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string CUST_ITEM_CODE { get; set; }
		public string EAN_CODE1 { get; set; }
		public string EAN_CODE2 { get; set; }
		public string EAN_CODE3 { get; set; }
		public string VALID_DATE { get; set; }
		public string ENTER_DATE { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string DC_NAME { get; set; }
		public string PRINT_DATE { get; set; }
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 初盤 系統庫存數(A)
		/// </summary>
		public string FST_DEVICE_STOCK_QTY { get; set; }
		/// <summary>
		/// 複盤 系統庫存數(A)
		/// </summary>
		public string SEC_DEVICE_STOCK_QTY { get; set; }
		/// <summary>
		/// 初盤 盤點數(B)
		/// </summary>
		public string FST_QTY { get; set; }
		/// <summary>
		/// 複盤 盤點數(B)
		/// </summary>
		public string SEC_QTY { get; set; }
		/// <summary>
		/// 初盤 盤點差異數(B-A)
		/// </summary>
		public string FST_DIFF_FIRST_QTY { get; set; }
		/// <summary>
		/// 複盤 盤點差異數(B-A)
		/// </summary>
		public string SEC_DIFF_SECOND_QTY { get; set; }
		/// <summary>
		/// 初盤 WMS庫存數(X)
		/// </summary>
		public string FST_STOCK_QTY { get; set; }
		/// <summary>
		/// 複盤 WMS庫存數(X)
		/// </summary>
		public string SEC_STOCK_QTY { get; set; }
		/// <summary>
		/// 初盤 WMS未搬動數(Y)
		/// </summary>
		public string FST_UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 複盤 WMS未搬動數(Y)
		/// </summary>
		public string SEC_UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 初盤 WMS庫差數(B-X-Y)
		/// </summary>
		public string FST_STOCK_DIFF_QTY { get; set; }
		/// <summary>
		/// 複盤 WMS庫差數(B-X-Y)
		/// </summary>
		public string SEC_STOCK_DIFF_QTY { get; set; }
		public string FST_INVENTORY_NAME { get; set; }
		public string SEC_INVENTORY_NAME { get; set; }
		public string FST_INVENTORY_DATE { get; set; }
		public string SEC_INVENTORY_DATE { get; set; }
		
	}

  [DataContract]
  [Serializable]
  [DataServiceKey("IsSuccessed")]
  public class CheckInventoryItemRes
  {
    [DataMember]
    public bool IsSuccessed { get; set; }
    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public List<InventoryItem> InventoryItems { get; set; }
  }

  [DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryItem
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsSelected { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
    [DataMember]
    public string CUST_ITEM_CODE { get; set; }

  }
	
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryItemQty
	{
		public int ROWNUM { get; set; }

		public string ITEM_CODE { get; set; }

		public string ITEM_NAME { get; set; }


		public int QTY { get; set; }

		public bool IsSuccess { get; set; }

		public string Message { get; set; }

	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryScanLoc
	{
		public decimal ROWNUM { get; set; }
		public string LOC_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public int? TOTAL_CNT { get; set; }

		public int? TOTAL_QTY { get; set; }

		public bool IsSuccess { get; set; }
		public string Message { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryScanItem
	{
		public decimal ROWNUM { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_UNIT { get; set; }
		public int? INBOXTQTY { get; set; }
		public string PACKCOUNT_MAX_UNIT { get; set; }

		public string MAXUNIT { get; set; }
		public string MINUNIT { get; set; }
		public int? INVENTORY_QTY { get; set; }

		public bool IsSuccess { get; set; }
		public string Message { get; set; }
	}


	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventorySimpleData
	{
		public Decimal ROWNUM { get; set; }
		public string INVENTORY_NO { get; set; }
		public DateTime INVENTORY_DATE { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ImportInventorySerial
	{
		public decimal ROWNUM { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string SERIAL_NO { get; set; }

		public bool IsSuccess { get; set; }
		public string Message { get; set; }
	}

	public class InventoryDiffLocItemQty
	{
		public decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

		public string INVENTORY_NO { get; set; }
		public string WAREHOUSE_ID { get; set; }

		public string LOC_CODE { get; set; }

		public string ITEM_CODE { get; set; }

		public string WORK_TYPE { get; set; }
		public int QTY { get; set; }
		public int ADJ_QTY_IN { get; set; }
		public int ADJ_QTY_OUT { get; set; }
		public int INVENTORY_QTY { get; set; }
		public string BUNDLE_SERIALLOC { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryLocItem
	{
		public decimal ROWNUM { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ImportInventoryDetailItem
	{
		/// <summary>
		/// 序號
		/// </summary>
		[DataMember]
		public decimal ROWNUM { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[DataMember]
		public string ItemCode { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        [DataMember]
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        [DataMember]
        public string ITEM_SPEC { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        [DataMember]
        public string ITEM_SIZE { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        [DataMember]
        public string ITEM_COLOR { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        [DataMember]
		public string ValidDate { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		[DataMember]
		public string EnterDate { get; set; }
        /// <summary>
        /// 倉別
        /// </summary>
        [DataMember]
        public string WAREHOUSE_NAME { get; set; }
        /// <summary>
        /// 儲位
        /// </summary>
        [DataMember]
		public string LocCode { get; set; }
        /// <summary>
        /// 庫存數
        /// </summary>
        [DataMember]
        public string QTY { get; set; }
        /// <summary>
        /// 初盤數
        /// </summary>
        [DataMember]
		public string FIRST_QTY { get; set; }
		/// <summary>
		/// 複盤數
		/// </summary>
		[DataMember]
		public string SECOND_QTY { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [DataMember]
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        [DataMember]
        public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        [DataMember]
        public string BOX_CTRL_NO { get; set; }
		/// <summary>
		/// 貨主品編
		/// </summary>
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		/// <summary>
		/// 國條
		/// </summary>
		[DataMember]
		public string EAN_CODE1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		[DataMember]
		public string EAN_CODE2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		[DataMember]
		public string EAN_CODE3 { get; set; }
		/// <summary>
		/// 未搬移數
		/// </summary>
		[DataMember]
		public string UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 自動倉數
		/// </summary>
		[DataMember]
		public string DEVICE_STOCK_QTY { get; set; }
	}

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class ExportFailInventoryDetailItem
    {
        /// <summary>
        /// 序號
        /// </summary>
        [DataMember]
        public decimal ROWNUM { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ItemCode { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        [DataMember]
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        [DataMember]
        public string ITEM_SPEC { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        [DataMember]
        public string ITEM_SIZE { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        [DataMember]
        public string ITEM_COLOR { get; set; }    
        /// <summary>
        /// 效期
        /// </summary>
        [DataMember]
        public string ValidDate { get; set; }
        /// <summary>
        /// 入庫日
        /// </summary>
        [DataMember]
        public string EnterDate { get; set; }
        /// <summary>
        /// 倉別
        /// </summary>
        [DataMember]
        public string WAREHOUSE_NAME { get; set; }
        /// <summary>
        /// 儲位
        /// </summary>
        [DataMember]
        public string LocCode { get; set; }
        /// <summary>
        /// 庫存數
        /// </summary>
        [DataMember]
        public string QTY { get; set; }
        /// <summary>
        /// 初盤數
        /// </summary>
        [DataMember]
        public string FIRST_QTY { get; set; }
        /// <summary>
        /// 複盤數
        /// </summary>
        [DataMember]
        public string SECOND_QTY { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [DataMember]
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 盤盈可回沖Name
        /// </summary>
        [DataMember]
        public string FLUSHBACKNAME { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        [DataMember]
        public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        [DataMember]
        public string BOX_CTRL_NO { get; set; }
        /// <summary>
        /// 失敗原因
        /// </summary>
        [DataMember]
        public string FailMessage { get; set; }
		/// <summary>
		/// 未搬移數
		/// </summary>
		[DataMember]
		public string UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 自動倉數
		/// </summary>
		[DataMember]
		public string DEVICE_STOCK_QTY { get; set; }
		/// <summary>
		/// 貨主品編
		/// </summary>
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		/// <summary>
		/// 國條
		/// </summary>
		[DataMember]
		public string EAN_CODE1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		[DataMember]
		public string EAN_CODE2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		[DataMember]
		public string EAN_CODE3 { get; set; }
	}

    [DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P081204Data
	{
		/// <summary>
		/// 序號
		/// </summary>
		[DataMember]
		public decimal ROWNUM { get; set; }
		/// <summary>
		/// 盤點單號
		/// </summary>
		[DataMember]
		public string INVENTORY_NO { get; set; }
		/// <summary>
		/// 盤點日期
		/// </summary>
		[DataMember]
		public DateTime INVENTORY_DATE { get; set; }
		/// <summary>
		/// 盤點狀態
		/// </summary>
		[DataMember]
		public string STATUS { get; set; }
		/// <summary>
		/// 品項數
		/// </summary>
		[DataMember]
		public int ITEM_CNT { get; set; }
		/// <summary>
		/// 盤點工作站
		/// </summary>
		[DataMember]
		public string UPD_NAME { get; set; }
		/// <summary>
		/// 盤點工作站
		/// </summary>
		[DataMember]
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// 盤點類型
		/// </summary>
		[DataMember]
		public string INVENTORY_TYPE { get; set; }
		/// <summary>
		/// 盤點狀態描述
		/// </summary>
		[DataMember]
		public string STATUS_DESC { get; set; }
		/// <summary>
		/// 是否顯示盤點數
		/// </summary>
		public string SHOW_CNT { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P08120401Data
	{
		/// <summary>
		/// 序號
		/// </summary>
		[DataMember]
		public decimal ROWNUM { get; set; }
		/// <summary>
		/// 盤點單號
		/// </summary>
		[DataMember]
		public string INVENTORY_NO { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 盤點數
		/// </summary>
		[DataMember]
		public int INVENTORY_QTY { get; set; }
		/// <summary>
		/// 箱入數
		/// </summary>
		[DataMember]
		public string BOX_IN_QTY { get; set; }
		/// <summary>
		/// 箱數
		/// </summary>
		[DataMember]
		public string BOX_QTY { get; set; }
		/// <summary>
		/// 零散數
		/// </summary>
		[DataMember]
		public string SCATTER_QTY { get; set; }
		/// <summary>
		/// 合計
		/// </summary>
		[DataMember]
		public decimal? TOTAL_QTY { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[DataMember]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		[DataMember]
		public string ITEM_NAME { get; set; }
		/// <summary>
		/// 尺寸
		/// </summary>
		[DataMember]
		public string ITEM_SIZE { get; set; }
		/// <summary>
		/// 顏色
		/// </summary>
		[DataMember]
		public string ITEM_COLOR { get; set; }
		/// <summary>
		/// 規格
		/// </summary>
		[DataMember]
		public string ITEM_SPEC { get; set; }
		/// <summary>
		/// 單位
		/// </summary>
		[DataMember]
		public string ITEM_UNIT { get; set; }
		/// <summary>
		/// 包裝參考
		/// </summary>
		[DataMember]
		public string UNIT_TRANS { get; set; }
		/// <summary>
		/// 是否新增商品
		/// </summary>
		[DataMember]
		public string NEW_ITEM { get; set; }
		/// <summary>
		/// 儲區
		/// </summary>
		[DataMember]
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
	}

	public class ImportInventoryDetailResult
	{
		public ExecuteResult Result { get; set; }
		public List<InventoryDetailItem> InventoryDetailItems { get; set; }

        public List<ExportFailInventoryDetailItem> FailDetailItems { get; set; }
	}

	[Serializable]
	[DataServiceKey("WMS_NO")]
	public class InventoryDoc
	{
		[DataMember]
		public string WMS_NO { get; set; }
	}

    public class InventoryConfirmResultModel
    {
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string WMS_NO { get; set; }
        public string WAREHOUSE_ID { get; set; }
        public string SKUCODE { get; set; }
        public DateTime EXPIRYDATE { get; set; }
        public string OUTBATCHCODE { get; set; }
        public int SKUSYSQTY { get; set; }
        public int SKUQTY { get; set; }
        public DateTime OPERATORTIME { get; set; }
    }

    public class FirstInventoryModel
    {
        public int AllQty { get; set; }
        public F140104 F140104 { get; set; }
    }

    public class SecondInventoryModel
    {
        public int AllQty { get; set; }
        public F140105 F140105 { get; set; }
    }
}
