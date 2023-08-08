using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class PurchaseOrderReplyReq
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustCode { get; set; }
		public List<PurchaseOrderReplyWarehouseIn> WarehouseIns { get; set; } = new List<PurchaseOrderReplyWarehouseIn>();
	}

	public class PurchaseOrderReplyWarehouseIn
	{
		/// <summary>
		/// 貨主進倉單號
		/// </summary>
		public string CustInNo { get; set; }
		/// <summary>
		/// 單號類型
		/// </summary>
		public string OrderType { get; set; }
		/// <summary>
		/// 處理狀態
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// 作業時間
		/// </summary>
		public string WorkTime { get; set; }
		public List<PurchaseOrderReplyWarehouseInDetail> WarehouseInDetails { get; set; } = new List<PurchaseOrderReplyWarehouseInDetail>();
		public List<PurchaseOrderReplyRcvData> RcvDatas { get; set; } = new List<PurchaseOrderReplyRcvData>();
		public List<PurchaseOrderReplyRcvSnData> RcvSnDatas { get; set; } = new List<PurchaseOrderReplyRcvSnData>();
		public List<PurchaseOrderReplyAllocData> AllocDatas { get; set; } = new List<PurchaseOrderReplyAllocData>();

	}

	public class PurchaseOrderReplyWarehouseInDetail
	{
		/// <summary>
		/// 原進倉單的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 進倉訂購數量
		/// </summary>
		public int InQty { get; set; }
		/// <summary>
		/// 實際點收總數量
		/// </summary>
		public int? TotalRcvQty { get; set; }
	}

	public class PurchaseOrderReplyRcvData
	{
		/// <summary>
		/// 原進倉單的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 驗收單號
		/// </summary>
		public string RcvNo { get; set; }
		/// <summary>
		/// 驗收單明細項次
		/// </summary>
		public string RcvSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 驗收量(含不良品數量)
		/// </summary>
		public int RcvQty { get; set; }
		/// <summary>
		/// 不良品量
		/// </summary>
		public int DefectQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 長(cm)
		/// </summary>
		public decimal PackLength { get; set; }
		/// <summary>
		/// 寬(cm)
		/// </summary>
		public decimal PackWidth { get; set; }
		/// <summary>
		/// 高(cm)
		/// </summary>
		public decimal PackHeight { get; set; }
		/// <summary>
		/// 重量(kg)
		/// </summary>
		public decimal PackWeight { get; set; }
		/// <summary>
		/// 國際條碼
		/// </summary>
		public string ItemBarCode1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		public string ItemBarCode2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		public string ItemBarCode3 { get; set; }
		/// <summary>
		/// 保存天數
		/// </summary>
		public int? SaveDay { get; set; }
		/// <summary>
		/// 是否為效期商品(0:否1:是)
		/// </summary>
		public string NeedExpired { get; set; }
		/// <summary>
		/// 首次進倉日
		/// </summary>
		public string FirstInDate { get; set; }
		/// <summary>
		/// 允收天數
		/// </summary>
		public short? AllDln { get; set; }
		/// <summary>
		/// 警示天數(允售天數)
		/// </summary>
		public int? AllShp { get; set; }
		/// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
		/// <summary>
		/// 易碎品(0: 否, 1: 是)
		/// </summary>
		public bool IsFragile { get; set; }
		/// <summary>
		/// 液體(0: 否, 1: 是)
		/// </summary>
		public bool IsSpill { get; set; }
		/// <summary>
		/// 是否易遺失(0: 否, 1: 是)
		/// </summary>
		public string IsEasyLose { get; set; }
		/// <summary>
		/// 貴重品標示(0: 否, 1: 是)
		/// </summary>
		public string IsPrecious { get; set; }
		/// <summary>
		/// 強磁標示(0: 否, 1: 是)
		/// </summary>
		public string IsMagnetic { get; set; }
        /// <summary>
        /// 易變質標示(0: 否, 1: 是)
        /// </summary>
        public string IsPerishable { get; set; }
        /// <summary>
        /// 溫層(01:常溫26-30、02:恆溫8-18、03:冷藏-2~10、04:冷凍-18~-25)
        /// </summary>
        public string TmprType { get; set; }
		/// <summary>
		/// 需溫控標示(0: 否, 1: 是)
		/// </summary>
		public string IsTempControl { get; set; }

		public List<PurchaseOrderReplyDefectDatas> DefectDatas { get; set; }
	}

	public class PurchaseOrderReplyDefectDatas
	{
		/// <summary>
		/// 不良品的原因類型
		/// </summary>
		public string DefectCause { get; set; }
		/// <summary>
		/// 不良品的原因
		/// </summary>
		public string DefectCauseMemo { get; set; }
		/// <summary>
		/// 不良品數量
		/// </summary>
		public int DefectQty { get; set; }
	}

	public class PurchaseOrderReplyRcvSnData
	{
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 驗收時的序號清單
		/// </summary>
		public string[] SnList { get; set; }
	}

	public class PurchaseOrderReplyAllocData
	{
		/// <summary>
		/// 原進倉單的品號項次
		/// </summary>
		public string ItemSeq { get; set; }
		/// <summary>
		/// 上架單號
		/// </summary>
		public string AllocNo { get; set; }
		/// <summary>
		/// 上架明細項次
		/// </summary>
		public string AllocSeq { get; set; }
		/// <summary>
		/// 商品編號
		/// </summary>
		public string ItemCode { get; set; }
		/// <summary>
		/// 倉別編號
		/// </summary>
		public string WhNo { get; set; }
		/// <summary>
		/// 上架數量
		/// </summary>
		public long ActQty { get; set; }
		/// <summary>
		/// 有效日期
		/// </summary>
		public string ValidDate { get; set; }
		/// <summary>
		/// 入庫日
		/// </summary>
		public string EnterDate { get; set; }
		/// <summary>
		/// 商品序號(序號綁儲位商品)
		/// </summary>
		public string Sn { get; set; }
		/// <summary>
		/// 商品批號
		/// </summary>
		public string MakeNo { get; set; }
	}
}