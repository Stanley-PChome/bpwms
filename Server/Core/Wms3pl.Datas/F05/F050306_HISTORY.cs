﻿namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// 配庫後揀貨資料歷史檔
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F050306_HISTORY")]
	public class F050306_HISTORY : IAuditInfo
	{
		/// <summary>
		/// 流水號
		/// </summary>
		[Key]
		[Required]
		public Int64 ID { get; set; }
		/// <summary>
		/// 物流中心編號
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// 資料來源(01:訂單 02:揀缺單)
		/// </summary>
		[Required]
		public string SOURCE { get; set; }
		/// <summary>
		/// 訂單編號/揀缺揀貨單號
		/// </summary>
		[Required]
		public string WMS_NO { get; set; }
		/// <summary>
		/// 訂單項次/揀缺揀貨項次
		/// </summary>
		[Required]
		public string WMS_SEQ { get; set; }
		/// <summary>
		/// 倉庫編號
		/// </summary>
		[Required]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 揀貨樓層(抓F1980.FLOOR)
		/// </summary>
		[Required]
		public string PICK_FLOOR { get; set; }
		/// <summary>
		/// 裝置類型(0:人工, 1:AGV, 2:穿梭車 (抓F1980.DEVICE_TYPE))
		/// </summary>
		[Required]
		public string DEVICE_TYPE { get; set; }
		/// <summary>
		/// 倉庫溫層(F1980.TMPR_TYPE)
		/// </summary>
		[Required]
		public string WH_TMPR_TYPE { get; set; }
		/// <summary>
		/// 揀貨儲位
		/// </summary>
		[Required]
		public string PICK_LOC { get; set; }
		/// <summary>
		/// 人工倉的PK區
		/// </summary>
		public string PK_AREA { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		[Required]
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 訂單類別(0:B2B,1:B2C)
		/// </summary>
		[Required]
		public string ORD_TYPE { get; set; }
		/// <summary>
		/// 來源單據類別F000902
		/// </summary>
		public string SOURCE_TYPE { get; set; }
		/// <summary>
		/// 效期
		/// </summary>
		[Required]
		public DateTime VALID_DATE { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		[Required]
		public DateTime ENTER_DATE { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[Required]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// 廠商編號
		/// </summary>
		[Required]
		public string VNR_CODE { get; set; }
		/// <summary>
		/// 序號
		/// </summary>
		[Required]
		public string SERIAL_NO { get; set; }
		/// <summary>
		/// 板號
		/// </summary>
		[Required]
		public string PALLET_CTRL_NO { get; set; }
		/// <summary>
		/// 箱號
		/// </summary>
		[Required]
		public string BOX_CTRL_NO { get; set; }
		/// <summary>
		/// 預定揀貨數量
		/// </summary>
		[Required]
		public Int64 B_PICK_QTY { get; set; }
		/// <summary>
		/// 客戶自訂分類(Out:一般出貨 MoveOut:集貨出貨(跨庫調撥) CVS:超商出貨
		/// </summary>
		public string CUST_COST { get; set; }
		/// <summary>
		/// 優先處理旗標(1:一般 2:優先 3:急件)
		/// </summary>
		public string FAST_DEAL_TYPE { get; set; }
		/// <summary>
		/// 出貨單號(揀缺配庫後要寫入)
		/// </summary>
		public string WMS_ORD_NO { get; set; }
		/// <summary>
		/// 出貨項次(揀缺配庫後要寫入)
		/// </summary>
		public string WMS_ORD_SEQ { get; set; }
		/// <summary>
		/// F05120601.ID(揀缺配庫後要寫入)
		/// </summary>
		public Int64? LACK_ID { get; set; }
		/// <summary>
		/// 跨庫出貨目的地
		/// </summary>
		public string MOVE_OUT_TARGET { get; set; }
		/// <summary>
		/// 建議出貨包裝線類型(空白=不指定 PA1=小線 PA2=大線)
		/// </summary>
		public string PACKING_TYPE { get; set; }
		/// <summary>
		/// 指定容器(00=不限, 01=M-周轉箱, 02=2L周轉箱)
		/// </summary>
		public string CONTAINER_TYPE { get; set; }
		/// <summary>
		/// 人工倉的PK區名稱
		/// </summary>
		public string PK_AREA_NAME { get; set; }
		/// <summary>
		/// 廠退廠商編號
		/// </summary>
		public string RTN_VNR_CODE { get; set; }
		/// <summary>
		/// 揀貨API項次(呼叫揀貨API使用)
		/// </summary>
		public string PICKAPI_SEQ { get; set; }
		/// <summary>
		/// 對應WMS揀貨單號
		/// </summary>
		public string PICK_ORD_NO { get; set; }
		/// <summary>
		/// LMS揀貨批次號
		/// </summary>
		public string LMS_PICKING_BATCH_NO { get; set; }
		/// <summary>
		/// LMS揀貨類型
		/// </summary>
		public string LMS_PICKING_TYPE { get; set; }
		/// <summary>
		/// LMS產生時間
		/// </summary>
		public string LMS_CREATE_TIME { get; set; }
		/// <summary>
		/// LMS揀貨單號
		/// </summary>
		public string LMS_PICKING_NO { get; set; }
		/// <summary>
		/// LMS揀貨系統(0: 人工倉、1: 自動倉)
		/// </summary>
		public int LMS_PICKING_SYSTEM { get; set; }
		/// <summary>
		/// LMS揀貨區編號
		/// </summary>
		public string LMS_PICK_AREA_ID { get; set; }
		/// <summary>
		/// LMS揀貨區名稱
		/// </summary>
		public string LMS_PICK_AREA_NAME { get; set; }
		/// <summary>
		/// LMS指定容器(A7)
		/// </summary>
		public string LMS_CONTINER_TYPE { get; set; }
		/// <summary>
		/// LMS下一步指示
		/// </summary>
		public string LMS_NEXT_STEP_CODE { get; set; }
		/// <summary>
		/// LMS明細揀貨區編號
		/// </summary>
		public string LMS_DTL_PICK_AREA_ID { get; set; }
		/// <summary>
		/// LMS明細揀貨區名稱
		/// </summary>
		public string LMS_DTL_PICK_AREA_NAME { get; set; }
		/// <summary>
		/// LMS 出貨單號
		/// </summary>
		public string LMS_WMS_NO { get; set; }
		/// <summary>
		/// LMS 出貨明細項次
		/// </summary>
		public string LMS_WMS_SEQ { get; set; }
		/// <summary>
		/// 建立日期
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// 建立人員
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// 建立人名
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
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

    /// <summary>
    /// 訂單建立日期 (只存日期不存時間)
    /// </summary>
    public DateTime? ORDER_CRT_DATE { get; set; }

    /// <summary>
    /// 商品處理類別(0:一般 1:含安裝型商品)
    /// </summary>
    public string ORDER_PROC_TYPE { get; set; }

    /// <summary>
    /// 收貨郵遞區號
    /// </summary>
    public string ORDER_ZIP_CODE { get; set; }

    /// <summary>
    /// 是否北北基訂單(0:否 1:是)
    /// </summary>
    public string IS_NORTH_ORDER { get; set; }

    /// <summary>
    /// 建議物流商編號
    /// </summary>
    public string SUG_LOGISTIC_CODE { get; set; }

    /// <summary>
    /// 特別處理標記 (0: 無、1: Apple廠商的商品)
    /// </summary>
    public string NP_FLAG { get; set; }

  }
}