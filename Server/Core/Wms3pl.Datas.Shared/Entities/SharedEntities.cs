using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services.Common;
using System.Runtime.Serialization;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
  [Serializable]
  [DataContract]
  [DataServiceKey("IsSuccessed")]
  public class ExecuteResult
  {
    public ExecuteResult() { }
    public ExecuteResult(bool isSuccessed)
    {
      IsSuccessed = isSuccessed;
    }
    public ExecuteResult(bool isSuccessed, string message)
    {
      IsSuccessed = isSuccessed;
      Message = message;
    }

    public ExecuteResult(bool isSuccessed, string message, string no)
    {
      IsSuccessed = isSuccessed;
      Message = message;
      No = no;
    }

    [DataMember]
    public bool IsSuccessed { get; set; }
    [DataMember]
    public string Message { get; set; }

    /// <summary>
    /// 單號
    /// </summary>
    [DataMember]
    public string No { get; set; }
  }

  [DataContract]
  public class BarcodeData
  {
    public BarcodeData(BarcodeType barcode = BarcodeType.None)
    {
      Barcode = barcode;
    }
    private BarcodeType _barcode;
    [DataMember]
    public BarcodeType Barcode
    {
      get { return _barcode; }
      set
      {
        _barcode = value;
        BarcodeText = _barcode.DisplayName();
      }
    }
    [DataMember]
    public string BarcodeText { get; set; }
  }

  [Serializable]
  [DataServiceKey("ITEM_CODE", "GUP_CODE", "CUST_CODE")]
  public class LoProductItem
  {
    public string ITEM_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string BUNDLE_SERIALNO { get; set; }
    public string BUNDLE_SERIALLOC { get; set; }
    public int QTY { get; set; }
    public string SERIAL_NO { get; set; }
  }



  /// <summary>
  /// LoService 的 CreateLo 大量建立參數 
  /// </summary>
  public class TicketMaster
  {
    public string TicketNo { get; set; }
    public List<TicketItem> TicketItems { get; set; }

    public TicketMaster()
    {
      TicketItems = new List<TicketItem>();
    }
  }

  [Serializable]
  [DataServiceKey("ITEM_CODE", "GUP_CODE", "CUST_CODE")]
  public class TicketItem
  {
    public string ITEM_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public int QTY { get; set; }
    public string SERIAL_NO { get; set; }
  }

  #region 序號檢核
  /// <summary>
  /// 序號檢核回傳
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("SerialNo")]
  public class SerialNoResult
  {
    [DataMember]
    public string SerialNo { get; set; }
    [DataMember]
    public string CurrentlyStatus { get; set; }
    [DataMember]
    public string ItemCode { get; set; }
    [DataMember]
    public string ItemName { get; set; }
    [DataMember]
    public bool Checked { get; set; }
    [DataMember]
    public string Message { get; set; }
    [DataMember]
    public string Puk { get; set; }
    [DataMember]
    public string CellNum { get; set; }
    [DataMember]
    public string BatchNo { get; set; }
    [DataMember]
    public string BoxSerail { get; set; }
    [DataMember]
    public string CaseNo { get; set; }
    [DataMember]
    public decimal? SEQ { get; set; }
    [DataMember]
    public decimal? CombinNo { get; set; }
    [DataMember]
    public string boxCtrlNo { get; set; }
    [DataMember]
    public string palletCtrlNo { get; set; }
    [DataMember]
    public string ItemSize { get; set; }
    [DataMember]
    public string ItemColor { get; set; }
    [DataMember]
    public string MakeNo { get; set; }
  }
  #endregion



  #region 共用標籤

  [Serializable]
  [DataServiceKey("LableCode")]
  [DataContract]
  public class LableItem
  {
    // 標籤編號
    [DataMember]
    public string LableCode { get; set; }
    // 標籤名稱
    [DataMember]
    public string LableName { get; set; }
    // 標籤類型
    [DataMember]
    public string LableType { get; set; }
    // 業主代碼
    [DataMember]
    public string GupCode { get; set; }
    // 業主名稱
    [DataMember]
    public string GupName { get; set; }
    // 貨主代碼
    [DataMember]
    public string CustCode { get; set; }
    // 貨主名稱
    [DataMember]
    public string CustName { get; set; }
    //客戶品號 
    [DataMember]
    public string CustItemCode { get; set; }
    // 卡號
    [DataMember]
    public string SerialNo { get; set; }
    // 門號
    [DataMember]
    public string CellNum { get; set; }
    // 盒號
    [DataMember]
    public string BoxSerial { get; set; }
    // 盒號
    [DataMember]
    public string BoxNo { get; set; }
    // 箱號
    [DataMember]
    public string CaseNo { get; set; }
    // 商品編號
    [DataMember]
    public string ItemCode { get; set; }
    // 商品名稱
    [DataMember]
    public string ItemName { get; set; }
    // 商品尺寸
    [DataMember]
    public string ItemSize { get; set; }
    // 商品規格
    [DataMember]
    public string ItemSpec { get; set; }
    // 商品顏色
    [DataMember]
    public string ItemColor { get; set; }
    // PUK
    [DataMember]
    public string PUK { get; set; }
    // SIM 卡規格
    [DataMember]
    public string SUGR { get; set; }
    // 廠商編號
    [DataMember]
    public string VnrCode { get; set; }
    // 廠商名稱
    [DataMember]
    public string VnrName { get; set; }
    // 保固類型 (保固期限(0貳 1壹 2半)F000904)
    [DataMember]
    public string WarrantyType { get; set; }
    // 保固類型起始年
    [DataMember]
    public string WarrantyTypeYear { get; set; }
    // 保固類型起始月
    [DataMember]
    public string WarrantyTypeMonth { get; set; }
    // 保固代碼兩碼
    [DataMember]
    public string WarrantyCode { get; set; }
    // 保固日期
    [DataMember]
    public string WarrantyDate { get; set; }
    // 保固代碼 + 保固日期 四碼
    [DataMember]
    public string WarrantyCodeDate { get; set; }
    // 委外廠商代碼
    [DataMember]
    public string OutSource { get; set; }
    // 檢驗員
    [DataMember]
    public string CheckStaff { get; set; }
    // 物料說明1
    [DataMember]
    public string ItemDesc1 { get; set; }
    // 物料說明2
    [DataMember]
    public string ItemDesc2 { get; set; }
    // 物料說明3
    [DataMember]
    public string ItemDesc3 { get; set; }
    // 列印日期
    [DataMember]
    public string PrintDate { get; set; }
    // 列印日期-年份/月份
    [DataMember]
    public string PrintYearMonth { get; set; }
    // 列印時間
    [DataMember]
    public string PrintTime { get; set; }
    //有效期限
    [DataMember]
    public string ValidDate { get; set; }
    //數量
    [DataMember]
    public string Qty { get; set; }
    //單位
    [DataMember]
    public string Unit { get; set; }
    //盒重量
    [DataMember]
    public string BoxWeight { get; set; }
    //裝箱盒號清單
    [DataMember]
    public List<string> BoxSerialList { get; set; }
    //備註
    [DataMember]
    public string Memo { get; set; }
    //箱重量
    [DataMember]
    public string CaseWeight { get; set; }
    //進倉單號
    [DataMember]
    public string StockNo { get; set; }
    //建議儲位
    [DataMember]
    public string LocCode { get; set; }
    // 商品編號+商品名稱
    [DataMember]
    public string Item { get; set; }
    // 廠商編號+廠商名稱
    [DataMember]
    public string Vnr { get; set; }
    //BarCode
    [DataMember]
    public string EanCode { get; set; }
    //託運單欄位
    [DataMember]
    public List<string> input { get; set; }
  }

  #endregion


  #region 秤重標籤報表物件
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class P710201WeightReport
  {
    public int ROWNUM { get; set; }
    public string WEIGHT { get; set; }

    public string ORDER_NO { get; set; }

    public string CUST_NAME { get; set; }
  }
  #endregion

  #region 物流中心-看板 共用物件
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class DcWmsNoStatusItem
  {
    public decimal ROWNUM { get; set; }
    public string WMS_NO { get; set; }

    public string MEMO { get; set; }
    public string STAFF { get; set; }

    public string STAFF_NAME { get; set; }
    public DateTime START_DATE { get; set; }
  }
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class DcWmsNoOrdPropItem
  {
    public decimal ROWNUM { get; set; }
    public string ORD_PROP { get; set; }

    public string CUST_CODE { get; set; }

    public int CUST_FINISHCOUNT { get; set; }

    public int CUST_TOTALCOUNT { get; set; }
  }
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class DcWmsNoDateItem
  {
    public decimal ROWNUM { get; set; }
    public DateTime WmsDate { get; set; }
    public int WmsCount { get; set; }
  }
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class DcWmsNoLocTypeItem
  {
    public decimal ROWNUM { get; set; }
    public string LOC_TYPE_ID { get; set; }

    public decimal USEDLOCCOUNT { get; set; }
    public decimal UNUSEDLOCCOUNT { get; set; }

    public decimal TOTALLOCCOUNT { get; set; }
  }
  #endregion

  [DataContract]
  [Serializable]
  [DataServiceKey("SERIAL_NO", "GUP_CODE", "CUST_CODE")]
  public class F250102Data
  {
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string FREEZE_TYPE { get; set; }
    [DataMember]
    public string CONTROL { get; set; }
  }

  /// <summary>
  /// 來源單據項目
  /// </summary>
  public class SourceItem
  {
    public string SourceNo { get; set; }
    public string SourceType { get; set; }
    public string SourceStatus { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("WAREHOUSE_ID", "DC_CODE")]
  public class F1912WareHouseData
  {
    [DataMember]
    public string WAREHOUSE_ID { get; set; }
    [DataMember]
    public string WAREHOUSE_NAME { get; set; }
    [DataMember]
    public string WAREHOUSE_TYPE { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string DEVICE_TYPE { get; set; }
  }

  public class ItemQty
  {
    public string ItemCode { get; set; }
    public int Qty { get; set; }

    //指定來源儲位
    public string LocCode { get; set; }
    /// <summary>
    /// 指定入庫日
    /// </summary>
    public DateTime? ENTER_DATE { get; set; }
    /// <summary>
    /// 指定效期
    /// </summary>
    public DateTime? VALID_DATE { get; set; }

    public string SrcWareouseId { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    public string MakeNo { get; set; }
		/// <summary>
		/// 指定序號
		/// </summary>
		public string SerialNo { get; set; }
  }

  public class CheckItemTarLocMixLoc
  {
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string ItemCode { get; set; }
    public string TarLocCode { get; set; }
    public int CountValidDate { get; set; }
  }

  public class CheckStockItemTarLocMixLoc
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string ItemCode { get; set; }
    public string TarLocCode { get; set; }
    public DateTime? ValidDate { get; set; }
  }

  public class CheckItemTarLocAndParamsMixLoc
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string ItemCode { get; set; }
    public string TarLocCode { get; set; }
  }
  [DataContract]
  [Serializable]
  public class ScheduleParamBase
  {
    [DataMember]
    /// <summary>
    /// 物流中心
    /// </summary>
    public string DcCode { get; set; }
    [DataMember]
    /// <summary>
    ///  業主
    /// </summary>
    public string GupCode { get; set; }
    [DataMember]
    /// <summary>
    /// 貨主
    /// </summary>
    public string CustCode { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("DcCode", "GupCode", "CustCode", "CustomerId")]

  public class EgsReturnConsignParam : ScheduleParamBase
  {
    [DataMember]
    /// <summary>
    /// 配送商
    /// </summary>
    public string AllId { get; set; }
    [DataMember]
    /// <summary>
    /// 通路
    /// </summary>
    public string Channel { get; set; }
    [DataMember]
    /// <summary>
    /// 配次
    /// </summary>
    public string DelvTimes { get; set; }
    [DataMember]
    /// <summary>
    /// 訂單起日
    /// </summary>
    public DateTime? OrdSDate { get; set; }
    [DataMember]
    /// <summary>
    /// 訂單迄日
    /// </summary>
    public DateTime? OrdEDate { get; set; }
    [DataMember]
    /// <summary>
    /// 批次日期(起)
    /// </summary>
    public DateTime? DelvSDate { get; set; }
    [DataMember]
    /// <summary>
    /// 批次日期(迄)
    /// </summary>
    public DateTime? DelvEDate { get; set; }
    [DataMember]
    /// <summary>
    /// 契客代號
    /// </summary>
    public string CustomerId { get; set; }
    [DataMember]
    /// <summary>
    /// 指定物流方向  01:正物流 02:逆物流 空白或null:正/逆物流都取得
    /// </summary>
    public string DISTR_USE { get; set; }
  }
  [DataContract]
  [Serializable]
  [DataServiceKey("CONSIGN_NO", "CUSTOMER_ID")]
  //[IgnoreProperties("EncryptedProperties")]
  public class EgsReturnConsign : IEncryptable
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string ALL_ID { get; set; }
    [DataMember]
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string CONSIGN_TYPE { get; set; }
    [DataMember]
    public string CONSIGN_NO { get; set; }
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    [DataMember]
    public string CUSTOMER_ID { get; set; }
    [DataMember]
    public string TEMPERATURE { get; set; }
    [DataMember]
    public string DISTANCE { get; set; }
    [DataMember]
    public string SPEC { get; set; }
    [DataMember]
    public string ISCOLLECT { get; set; }
    [DataMember]
    public decimal? COLLECT_AMT { get; set; }
    [DataMember]
    public string ARRIVEDPAY { get; set; }
    [DataMember]
    public string PAYCASH { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string RECEIVER_NAME { get; set; }
    [DataMember]
    public string RECEIVER_MOBILE { get; set; }
    [DataMember]
    public string RECEIVER_PHONE { get; set; }
    [DataMember]
    public string RECEIVER_SUDA5 { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string RECEIVER_ADDRESS { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string SENDER_NAME { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string SENDER_TEL { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string SENDER_MOBILE { get; set; }
    [DataMember]
    public string SENDER_SUDA5 { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string SENDER_ADDRESS { get; set; }
    [DataMember]
    public string SHIP_DATE { get; set; }
    [DataMember]
    public string PICKUP_TIMEZONE { get; set; }
    [DataMember]
    public string DELV_TIMEZONE { get; set; }
    [DataMember]
    public string MEMBER_ID { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ISFRAGILE { get; set; }
    [DataMember]
    public string ISPRECISON_INSTRUMENT { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string SD_ROUTE { get; set; }
    [DataMember]
    public string DISTR_USE { get; set; }
    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get
    //	{
    //		return new Dictionary<string, string>
    //					{
    //						{"RECEIVER_ADDRESS", "ADDR"}, {"RECEIVER_NAME", "NAME"}, {"SENDER_NAME","NAME" }, {"SENDER_TEL","TEL" }, {"SENDER_MOBILE","TEL" }, {"SENDER_ADDRESS","ADDR" }
    //					};
    //	}
    //}
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("CONSIGN_NO", "CUSTOMER_ID")]
  public class EgsReturnConsign2
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string ALL_ID { get; set; }
    [DataMember]
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string CONSIGN_TYPE { get; set; }
    [DataMember]
    public string CONSIGN_NO { get; set; }
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    [DataMember]
    public string CUSTOMER_ID { get; set; }
    [DataMember]
    public string TEMPERATURE { get; set; }
    [DataMember]
    public string DISTANCE { get; set; }
    [DataMember]
    public string SPEC { get; set; }
    [DataMember]
    public string ISCOLLECT { get; set; }
    [DataMember]
    public decimal? COLLECT_AMT { get; set; }
    [DataMember]
    public string ARRIVEDPAY { get; set; }
    [DataMember]
    public string PAYCASH { get; set; }
    [DataMember]
    public string RECEIVER_NAME { get; set; }
    [DataMember]
    public string RECEIVER_MOBILE { get; set; }
    [DataMember]
    public string RECEIVER_PHONE { get; set; }
    [DataMember]
    public string RECEIVER_SUDA5 { get; set; }
    [DataMember]
    public string RECEIVER_ADDRESS { get; set; }
    [DataMember]
    public string SENDER_NAME { get; set; }
    [DataMember]
    public string SENDER_TEL { get; set; }
    [DataMember]
    public string SENDER_MOBILE { get; set; }
    [DataMember]
    public string SENDER_SUDA5 { get; set; }
    [DataMember]
    public string SENDER_ADDRESS { get; set; }
    [DataMember]
    public string SHIP_DATE { get; set; }
    [DataMember]
    public string PICKUP_TIMEZONE { get; set; }
    [DataMember]
    public string DELV_TIMEZONE { get; set; }
    [DataMember]
    public string MEMBER_ID { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ISFRAGILE { get; set; }
    [DataMember]
    public string ISPRECISON_INSTRUMENT { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string SD_ROUTE { get; set; }
    [DataMember]
    public string DISTR_USE { get; set; }

  }

  [DataContract]
  [Serializable]
  [DataServiceKey("DcCode", "GupCode", "CustCode", "Channel", "CustomerId", "ConsignType", "IsTest")]
  public class AutoGenConsignParam : ScheduleParamBase
  {
    [DataMember]
    public string Channel { get; set; }
    [DataMember]
    /// <summary>
    /// 配送商
    /// </summary>
    public string AllId { get; set; }
    [DataMember]
    /// <summary>
    /// 託運單類別(A:一般託運單 B:代收託運單)
    /// </summary>
    public string ConsignType { get; set; }
    [DataMember]
    /// <summary>
    /// 契客代號
    /// </summary>
    public string CustomerId { get; set; }
    [DataMember]
    /// <summary>
    /// 是否測試契客
    /// </summary>
    public string IsTest { get; set; }

  }
  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class SearchItemResult
  {
    [DataMember]
    public int ROWNUM { get; set; }
    [DataMember]
    public string ItemCode { get; set; }
    [DataMember]
    public string ItemName { get; set; }
    [DataMember]
    public string SerialNo { get; set; }
    [DataMember]
    public int? AuditQty { get; set; }
    [DataMember]
    public string Msg { get; set; }
    [DataMember]
    public string IsPass { get; set; }
    [DataMember]
    public string ReturnNo { get; set; }
    [DataMember]
    public int? RtnQty { get; set; }
    [DataMember]
    public string CustOrderNo { get; set; }
  }
  public enum InputSerialType
  {
    /// <summary>
    /// 無法確定
    /// </summary>
    None,
    /// <summary>
    /// 品號
    /// </summary>
    ProductNo,
    /// <summary>
    /// 序號
    /// </summary>
    SerialNo
  }

  public class CheckLoc : ExecuteResult
  {
    /// <summary>
    /// 物流中心
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 倉庫編號(有設定才檢查儲位是否為此倉庫)
    /// </summary>
    public string WarehouseId { get; set; }
    /// <summary>
    /// 儲位編號
    /// </summary>
    public string LocCode { get; set; }
    /// <summary>
    ///  儲位類型 1:來源儲位 2:目的儲位
    /// </summary>
    public string LocType { get; set; }
  }

  public class CheckLocItem : CheckLoc
  {
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
  }
  [DataContract]
  [Serializable]
  [DataServiceKey("DcCode", "GupCode", "CustCode", "CustomerId")]
  /// <summary>
  /// 新竹貨運-出貨託運單回檔參數
  /// </summary>
  public class HctShipReturnParam : ScheduleParamBase
  {
    [DataMember]

    /// <summary>
    /// 配送商編號
    /// </summary>
    public string AllId { get; set; }

    [DataMember]
    /// <summary>
    /// 通路
    /// </summary>
    public string Channel { get; set; }
    [DataMember]
    /// <summary>
    /// 配次
    /// </summary>
    public string DelvTimes { get; set; }
    [DataMember]
    /// <summary>
    /// 訂單起日
    /// </summary>
    public DateTime? OrdSDate { get; set; }
    [DataMember]
    /// <summary>
    /// 訂單迄日
    /// </summary>
    public DateTime? OrdEDate { get; set; }
    [DataMember]
    /// <summary>
    /// 批次日期(起)
    /// </summary>
    public DateTime? DelvSDate { get; set; }
    [DataMember]
    /// <summary>
    /// 批次日期(迄)
    /// </summary>
    public DateTime? DelvEDate { get; set; }
    [DataMember]
    /// <summary>
    /// 契客代號
    /// </summary>
    public string CustomerId { get; set; }
    [DataMember]
    /// <summary>
    /// 指定物流方向  01:正物流 02:逆物流 空白或null:正/逆物流都取得
    /// </summary>
    public string DistrUse { get; set; }

  }
  [DataContract]
  [Serializable]
  [DataServiceKey("CONSIGN_NO", "CONTRACT_CUST_NO")]
  //[IgnoreProperties("EncryptedProperties")]

  /// <summary>
  /// 新竹貨運-出貨託運單回檔
  /// </summary>
  public class HctShipReturn : IEncryptable
  {
    [DataMember]
    /// <summary>
    /// 物流中心
    /// </summary>
    public string DC_CODE { get; set; }
    [DataMember]
    /// <summary>
    /// 業主編號
    /// </summary>
    public string GUP_CODE { get; set; }
    [DataMember]
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CUST_CODE { get; set; }
    [DataMember]
    /// <summary>
    /// 配送商編號
    /// </summary>
    public string ALL_ID { get; set; }
    [DataMember]
    /// <summary>
    /// 出貨單號 
    /// </summary>
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    /// <summary>
    /// 清單編號(訂單編號)
    /// </summary>
    public string CUST_ORD_NO { get; set; }
    [DataMember]
    /// <summary>
    /// 查貨號碼(託運單號)
    /// </summary>
    public string CONSIGN_NO { get; set; }

    [DataMember]
    /// <summary>
    /// 客戶代號(契客代號)
    /// </summary>
    public string CONTRACT_CUST_NO { get; set; }
    [DataMember]
    /// <summary>
    /// 收貨人代號
    /// </summary>
    public string RECEIVER_CODE { get; set; }

    [DataMember]
    /// <summary>
    /// 收貨人名稱
    /// </summary>
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string RECEIVER_NAME { get; set; }

    [DataMember]
    /// <summary>
    /// 收貨人電話1
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string RECEIVER_PHONE { get; set; }

    [DataMember]
    /// <summary>
    /// 收貨人電話2
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string RECEIVER_MOBILE { get; set; }

    [DataMember]
    /// <summary>
    /// 收貨人地址
    /// </summary>
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string RECEIVER_ADDRESS { get; set; }

    [DataMember]
    /// <summary>
    /// 代收貨款
    /// </summary>
    public decimal COLLECT_AMT { get; set; }

    [DataMember]
    /// <summary>
    /// EgAmt(帶空白)
    /// </summary>
    public string EGAMT { get; set; }

    [DataMember]
    /// <summary>
    /// 發送日期(YYYYMMDD)
    /// </summary>
    public string SEND_DATE { get; set; }

    [DataMember]
    /// <summary>
    /// 發送站代號(4碼)
    /// </summary>
    public string SEND_CODE { get; set; }

    [DataMember]
    /// <summary>
    /// 到著站代號(4碼)
    /// </summary>
    public string ARRIVAL_CODE { get; set; }

    [DataMember]
    /// <summary>
    /// EkAmt(帶空白)
    /// </summary>
    public string EKAMT { get; set; }

    [DataMember]
    /// <summary>
    /// 件數
    /// </summary>
    public string PIECES { get; set; }

    [DataMember]
    /// <summary>
    /// 追加件數(帶空白)
    /// </summary>
    public string ADD_PIECES { get; set; }

    [DataMember]
    /// <summary>
    /// 重量
    /// </summary>
    public string WEIGHT { get; set; }

    [DataMember]
    /// <summary>
    /// EbAmt(帶空白)
    /// </summary>
    public string EBAMT { get; set; }
    [DataMember]
    /// <summary>
    /// ErAmt(帶空白)
    /// </summary>
    public string ERAMT { get; set; }
    [DataMember]
    /// <summary>
    /// EsAmt(帶空白)
    /// </summary>
    public string ESAMT { get; set; }
    [DataMember]
    /// <summary>
    /// EdAmt(帶空白)
    /// </summary>
    public string EDAMT { get; set; }
    [DataMember]
    /// <summary>
    /// ElAmt(帶空白)
    /// </summary>
    public string ELAMT { get; set; }

    [DataMember]
    /// <summary>
    /// 傳票區分 -(11元付;21到付;41代收貨款) 即託運單分類
    /// </summary>
    public string SUMMON_TYPE { get; set; }

    [DataMember]
    /// <summary>
    /// 商品種類(1一般;2宅配;3冷凍;4蔬果類;5當日配;6來回件;7貨到刷卡;8冷藏;9糕餅)
    /// </summary>
    public string ITEM_KIND { get; set; }

    [DataMember]
    /// <summary>
    /// 商品區分(固定帶1)
    /// </summary>
    public string ITEM_TYPE { get; set; }

    [DataMember]
    /// <summary>
    /// 指定日期(YYYYMMDD)
    /// </summary>
    public string ASSIGN_DATE { get; set; }

    [DataMember]
    /// <summary>
    /// 指定時間(只能代(09-12;12-17;17-20或空白))
    /// </summary>
    public string ASSIGN_TIME { get; set; }
    [DataMember]
    /// <summary>
    /// 供貨人代號
    /// </summary>
    public string SUPPLIER_CODE { get; set; }
    [DataMember]
    /// <summary>
    /// 供貨人名稱
    /// </summary>
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string SUPPLIER_NAME { get; set; }
    [DataMember]
    /// <summary>
    /// 供貨人電話1
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string SUPPLIER_PHONE { get; set; }
    [DataMember]
    /// <summary>
    /// 供貨人電話2
    /// </summary>
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string SUPPLIER_MOBILE { get; set; }
    [DataMember]
    /// <summary>
    /// 供貨人地址
    /// </summary>
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string SUPPLIER_ADDRESS { get; set; }
    [DataMember]
    /// <summary>
    /// 備註
    /// </summary>
    public string MEMO { get; set; }
    [DataMember]
    /// <summary>
    /// Esel(帶空白)
    /// </summary>
    public string ESEL { get; set; }
    [DataMember]
    /// <summary>
    /// Eprint(帶空白)
    /// </summary>
    public string EPRINT { get; set; }
    [DataMember]
    /// <summary>
    /// 收貨人郵遞區號(3碼)
    /// </summary>
    public string RECEIVER_ZIP_CODE { get; set; }


    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get
    //	{
    //		return new Dictionary<string, string>
    //					{
    //						 {"RECEIVER_NAME", "NAME"}, {"RECEIVER_PHONE","TEL" }, {"RECEIVER_MOBILE","TEL" },{"RECEIVER_ADDRESS", "ADDR"},
    //						{ "SUPPLIER_NAME","NAME" }, {"SUPPLIER_PHONE","TEL" }, {"SUPPLIER_MOBILE","TEL" }, {"SUPPLIER_ADDRESS","ADDR" }
    //					};
    //	}
    //}
  }

  [DataContract]
  [Serializable]

  public class ByteData
  {
    /// <summary>
    /// 檔案
    /// </summary>
    [DataMember]
    public string Data { get; set; }
    /// <summary>
    /// 檔名
    /// </summary>
    [DataMember]
    public string FileName { get; set; }
    /// <summary>
    /// 訊息
    /// </summary>
    [DataMember]
    public string Message { get; set; }
    /// <summary>
    /// 結果
    /// </summary>
    [DataMember]
    public bool IsSucess { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("CONSIGN_NO", "CONTRACT_CUST_NO")]
  //[IgnoreProperties("EncryptedProperties")]
  public class KTJShipReturn : IEncryptable
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string ALL_ID { get; set; }
    [DataMember]
    public string WMS_ORD_NO { get; set; }
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    [DataMember]
    public string CONSIGN_NO { get; set; }
    [DataMember]
    public string CONTRACT_CUST_NO { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string RECEIVER_NAME { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string RECEIVER_PHONE { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string RECEIVER_ADDRESS { get; set; }
    [DataMember]
    public Decimal? COLLECT_AMT { get; set; }
    [DataMember]
    public string SEND_DATE { get; set; }
    [DataMember]
    public string SEND_CODE { get; set; }
    [DataMember]
    public string PIECES { get; set; }
    [DataMember]
    public string ASSIGN_TIME { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("NAME")]
    public string SUPPLIER_NAME { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("TEL")]
    public string SUPPLIER_PHONE { get; set; }
    [DataMember]
    [Encrypted]
    [SecretPersonalData("ADDR")]
    public string SUPPLIER_ADDRESS { get; set; }
    [DataMember]
    public string MEMO { get; set; }
    [DataMember]
    public string RECEIVER_ZIP_CODE { get; set; }
    [DataMember]
    public DateTime? A_ARRIVAL_DATE { get; set; }
    [DataMember]
    public string DELV_ORD_NO { get; set; }
    [DataMember]
    public string VOLUME_QTY { get; set; }
    [DataMember]
    public string SUPPLIER_ZIP { get; set; }
    //public Dictionary<string, string> EncryptedProperties
    //{
    //	get
    //	{
    //		return new Dictionary<string, string>
    //		{
    //			{ "RECEIVER_NAME", "NAME"}, {"RECEIVER_PHONE","TEL" }, {"RECEIVER_ADDRESS", "ADDR"},
    //			{ "SUPPLIER_NAME","NAME" }, {"SUPPLIER_PHONE","TEL" }, {"SUPPLIER_ADDRESS","ADDR" }
    //		};
    //	}
    //}
  }

  public class SeqQtyModel
  {
    public short Seq { get; set; }
    public Int32 Qty { get; set; }
  }

  #region LmsApi上架倉別指示回傳物件
  public class LmsStowShelfAreaGuideResult
  {
    public bool IsSucessed { get; set; }
    public string Msg { get; set; }
    public List<LmsStowShelfAreaGuideRespense> Data { get; set; }
  }

  public class LmsStowShelfAreaGuideRespense
  {
    public string ItemCode { get; set; }
    public string WhId { get; set; }
    public string WhName { get; set; }
  }
  #endregion

  public class ContainerExecuteResult
  {
    public string ContainerCode { get; set; }

    public string WMS_NO { get; set; }

    public long f0701_ID { get; set; }

    public int Qty { get; set; }

    public string WAREHOUSE_ID { get; set; }
  }


  public class BulkUpdateF2501Result
  {
    public Boolean IsSuccessed { get; set; }
    public string Message { get; set; }
    /// <summary>
    /// 資料異動模式(add,edit,delete)
    /// </summary>
    public ModifyMode ModifyMode { get; set; }
    public F2501 f2501 { get; set; }
  }
  public class NewSuggestLocParam
  {
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 業主編號
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    public long Qty { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    public DateTime ValidDate { get; set; }
    /// <summary>
    /// 入庫日期
    /// </summary>
    public DateTime EnterDate { get; set; }
    /// <summary>
    /// 指定上架倉別型態(注意:這不是 WAREHOUSE_ID ) 與TarWarehouseId必須擇一必填
    /// </summary>
    public string TarWarehouseType { get; set; }
    /// <summary>
    /// 指定上架倉別編號 與TarWarehouseType必須擇一必填
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 建議儲位是否包含補貨區儲位
    /// </summary>
    public bool IsIncludeResupply { get; set; }
    /// <summary>
    /// 指定不允許混品(原本會根據設定去判斷，使用此屬性代表不用判斷設定，直接不準混品，設為false則為依設定判斷)
    /// </summary>
    public bool AppointNeverItemMix { get; set; }
    /// <summary>
    /// 是否不允許商品拆儲位放
    /// </summary>
    public bool NotAllowSeparateLoc { get; set; }

  }
  public class NewSuggestLoc
  {
    public string WarehouseId { get; set; }
    public string LocCode { get; set; }
    public long PutQty { get; set; }
  }
  public class ItemKey
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string ItemCode { get; set; }
  }
  public class EmptyLoc
  {
    public List<LocPriorityInfo> DataList { get; set; }
    public string WAREHOUSE_TYPE { get; set; }
    public string ATYPE_CODE { get; set; }
    public string WAREHOUSE_ID { get; set; }
    public bool IsAll { get; set; }
    public string DcCode { get; set; }
  }
  public class NewMixItemLoc
  {
    public List<MixLocPriorityInfo> DataList { get; set; }
    public string WAREHOUSE_TYPE { get; set; }
    public string ATYPE_CODE { get; set; }
    public string WAREHOUSE_ID { get; set; }
    public decimal? Volume { get; set; }
    public bool IsAll { get; set; }
    public string DcCode { get; set; }
  }
  public class NewNearestLoc
  {
    public List<NearestLocPriorityInfo> DataList { get; set; }
    public string WAREHOUSE_TYPE { get; set; }
    public string ATYPE_CODE { get; set; }
    public string WAREHOUSE_ID { get; set; }
    public bool IsAll { get; set; }
    public string DcCode { get; set; }
  }

  #region 取得揀貨單對應的出貨單單號及狀態
  // 取得揀貨單對應的出貨單單號及狀態
  public class F051202WithF050801
  {
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 揀貨單號
    /// </summary>
    public string PICK_ORD_NO { get; set; }
    /// <summary>
    /// 出貨單號
    /// </summary>
    public string WMS_ORD_NO { get; set; }
    /// <summary>
    /// 出貨單狀態
    /// </summary>
    public decimal STATUS { get; set; }
  }
  #endregion

  /// <summary>
  /// 商品檢驗與綁定容器-關箱結果
  /// </summary>
  public class ContainerCloseBoxRes : ExecuteResult
  {
    public F020501 f020501;
    public List<F020502> f020502s;
  }

  #region 商品檢驗共用服務(WarehouseInRecvService)用Entity

  #region LockAcceptenceOrder
  public class LockAcceptenceOrderReq
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string StockNo { get; set; }
    public Boolean IsChangeUser { get; set; }
		public string DeviceTool { get; set; }
  }
  #endregion LockAcceptenceOrder

  #region UnLockAcceptenceOrder用請求參數
  public class UnLockAcceptenceOrderReq
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string StockNo { get; set; }
  }
  #endregion UnLockAcceptenceOrder用請求參數

  #region AcceptanceConfirm
  /// <summary>
  /// 驗收確認參數
  /// </summary>
  public class AcceptanceConfirmPara
  {
    /// <summary>
    /// 物流中心代碼
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 業主代碼
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主代碼
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string PurchaseNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RTNo { get; set; }
    /// <summary>
    /// 驗收單所有明細
    /// </summary>
    public List<F02020101> f02020101s { get; set; }
    /// <summary>
    /// 進倉單
    /// </summary>
    public F010201 f010201 { get; set; }
    /// <summary>
    /// 是否揀貨區優先儲位
    /// </summary>
    public bool IsPickLocFirst { get; set; }
    ///// <summary>
    ///// 舊商品檢驗=0 商品檢驗與容器綁定=1
    ///// </summary>
    //public string RT_MODE { get; set; } = "0";
  }
	#endregion

	public class NotAcceptData
	{
		/// <summary>
		/// 商品品號
		/// </summary>
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 進貨序號
		/// </summary>
		public string STOCK_SEQ { get; set; }
		/// <summary>
		/// 訂購數
		/// </summary>
		public int STOCK_QTY { get; set; }
		/// <summary>
		/// 未驗收數
		/// </summary>
		public int NOT_STOCK_QTY { get; set; }
		/// <summary>
		/// 商品有效日期
		/// </summary>
		public DateTime VALI_DATE { get; set; }
	}
	public class GetOrCreateRecvDataRes
	{
		public List<F02020101> F02020101List { get; set; }
		public List<string> ApiInfoList { get; set; }
	}

  #region DeleteAcceptanceData
  /// <summary>
  /// 刪除驗收紀錄參數
  /// </summary>
  public class DeleteAcceptanceDataParam
  {
    /// <summary>
    /// 物流中心代碼
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 業主代碼
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主代碼
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string PurchaseNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string PurchaseSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RTNo { get; set; }
  }
	#endregion

  #region SaveRecvItem
  /// <summary>
  /// 儲存商品驗收結果參數
  /// </summary>
  public class SaveRecvItemParam
  {
    /// <summary>
    /// 物流中心代碼
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 業主代碼
    /// </summary>
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主代碼
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string PurchaseNo { get; set; }
    /// <summary>
    /// 進倉單序號
    /// </summary>
    public string PurchaseSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }

		/// <summary>
		/// 採購單號
		/// </summary>
		public string PoNo { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string ItemNo { get; set; }
    /// <summary>
    /// 國條
    /// </summary>
    public string EanCode1 { get; set; }
    /// <summary>
    /// 條碼二
    /// </summary>
    public string EanCode2 { get; set; }
    /// <summary>
    /// 條碼三
    /// </summary>
    public string EanCode3 { get; set; }
    /// <summary>
    /// 是否效期商品
    /// </summary>
    public string NeedExpired { get; set; }
    /// <summary>
    /// 保存天數
    /// </summary>
    public int? SaveDay { get; set; }
    /// <summary>
    /// 允收天數
    /// </summary>
    public int? AllowDelvDay { get; set; }
    /// <summary>
    /// 警示天數
    /// </summary>
    public int? AllowShipDay { get; set; }
    /// <summary>
    /// 是否序號商品
    /// </summary>
    public string BundleSerialNo { get; set; }
    /// <summary>
    /// 商品長
    /// </summary>
    public decimal PackLength { get; set; }
    /// <summary>
    /// 商品寬
    /// </summary>
    public decimal PackWidth { get; set; }
    /// <summary>
    /// 商品高
    /// </summary>
    public decimal PackHight { get; set; }
    /// <summary>
    /// 商品重量
    /// </summary>
    public decimal PackWeight { get; set; }
    /// <summary>
    /// 商品溫層
    /// </summary>
    public string TmprType { get; set; }
    /// <summary>
    /// 是否蘋果商品
    /// </summary>
    public string IsApple { get; set; }
    /// <summary>
    /// 驗收數量
    /// </summary>
    public int RecvQty { get; set; }
    /// <summary>
    /// 是否列印商品ID標
    /// </summary>
    public string IsPrintItemId { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    public DateTime ValidDate { get; set; }
    /// <summary>
    /// 是否貴重品
    /// </summary>
    public string IsPrecious { get; set; }
    /// <summary>
    /// 是否易碎品
    /// </summary>
    public string IsFragile { get; set; }
    /// <summary>
    /// 是否易遺失
    /// </summary>
    public string IsEasyLose { get; set; }
    /// <summary>
    /// 是否強磁標示
    /// </summary>
    public string IsMagentic { get; set; }
    /// <summary>
    /// 是否防溢漏包裝
    /// </summary>
    public string IsSpill { get; set; }
    /// <summary>
    /// 是否易變質標示
    /// </summary>
    public string IsPerishable { get; set; }
    /// <summary>
    /// 是否需溫控標示
    /// </summary>
    public string IsTempControl { get; set; }
    /// <summary>
    /// 是否首次驗收商品
    /// </summary>
    public string IsFirstInDate { get; set; }
    /// <summary>
    /// 上架倉別
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 商品檢驗項目清單
    /// </summary>
    public List<CheckItem> CheckItemList { get; set; }
  }

  public class CheckItem
  {
    /// <summary>
    /// 檢項項目代碼
    /// </summary>
    public string CheckNo { get; set; }
  }
  #endregion

  #region ClearSerilaNoData
  /// <summary>
  /// 驗收數量更改，清除已收集的序號內容以及刷讀記錄參數
  /// </summary>
  public class UpdateRecvQtyReq
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string PurchaseNo { get; set; }
    public string PurchaseSeq { get; set; }
    public string RtNo { get; set; }
  }
  #endregion

  #endregion

  #region 商品檢驗綁容器共用服務(WarehouseInRecvBindBoxService)用Entity

  #region LockBindContainerAcceptenceOrder用請求參數
  public class LockBindContainerAcceptenceOrderReq
  {
    //dcCode,gupCode,custCode,rtNo,IsChangeUser
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string RtNo { get; set; }
    public Boolean IsChangeUser { get; set; }
		public string DeviceTool { get; set; }
  }

  #endregion LockBindContainerAcceptenceOrder用請求參數

  #region UnLockBindContainerAcceptenceOrder用請求參數
  public class UnLockBindContainerAcceptenceOrderReq
  {
    public string DcCode { get; set; }
    public string GupCode { get; set; }
    public string CustCode { get; set; }
    public string RtNo { get; set; }
  }
  #endregion UnLockBindContainerAcceptenceOrder用請求參數

  #region DeleteContainerBindData用請求參數
  /// <summary>
  /// 進貨容器綁定-驗收單各區綁定容器放入確認_傳入
  /// </summary>
  public class DeleteContainerBindDataReq
  {
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 業主編號
    /// </summary>
    public string GupCode { get; set; } 
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 該區識別碼(F0205.ID)
    /// </summary>
    public int AreaId { get; set; }
    /// <summary>
    /// 進貨容器識別碼
    /// </summary>
    public long F020501_ID { get; set; }
    /// <summary>
    /// 進貨容器明細識別碼
    /// </summary>
    public long F020502_ID { get; set; }
    /// <summary>
    /// 進貨容器明細放入數量
    /// </summary>
    public int Qty { get; set; }
  }
	#endregion DeleteContainerBindData用請求參數

	public class AddContainerBindDataReq
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string PurchaseNo { get; set; }
		public string PurchaseSeq { get; set; }
		public string RtNo { get; set; }
		public string RtSeq { get; set; }
		public string ContainerCode { get; set; }
		public string TypeCode { get; set; }
		public long AreaId { get; set; }
		public string WarehouseId { get; set; }
		public int PutQty { get; set; }
	}

	public class AddContainerBindDataRes
	{
		public long F020501_ID { get; set; }
		public long F020502_ID { get; set; }
	}

	public class ShareRecvBindContainerFinishedReq
	{
		public string DcCode { get; set; }
		public string GupCode { get; set; }
		public string CustCode { get; set; }
		public string PurchaseNo { get; set; }
		public string RtNo { get; set; }
		public string RtSeq { get; set; }
	}

	public class ShareRecvBindContainerFinishedRes
	{
		public List<string> LockContainers { get; set; }
		public List<string> AllocationNoList { get; set; }
	}

	#endregion


	/// <summary>
	/// 貨主商品主檔
	/// </summary>
	[Serializable]
	public class CommonProduct
	{

		/// <summary>
		/// 商品編號
		/// </summary>
		public string ITEM_CODE { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		public string GUP_CODE { get; set; }

		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CUST_CODE { get; set; }

		

		/// <summary>
		/// 允收天數
		/// </summary>
		public Int16? ALL_DLN { get; set; }

	

		/// <summary>
		/// 客戶品號
		/// </summary>
		public string CUST_ITEM_CODE { get; set; }

		
		/// <summary>
		/// 是否可原箱出貨(0:否1是)
		/// </summary>
		public string ALLOWORDITEM { get; set; }

		/// <summary>
		/// 序號綁儲位(只要有儲位異動就需刷序號)
		/// </summary>
		public string BUNDLE_SERIALLOC { get; set; }

		/// <summary>
		/// 序號商品(商品進出或跨倉(上架,揀貨(ex.良品倉移動至加工倉),包裝)需刷序號)
		/// </summary>
		public string BUNDLE_SERIALNO { get; set; }

		

		/// <summary>
		/// 是否可儲位混商品(0:否1是)
		/// </summary>
		public string LOC_MIX_ITEM { get; set; }

	

		/// <summary>
		/// 序號碼數
		/// </summary>
		public Int16? SERIALNO_DIGIT { get; set; }

		/// <summary>
		/// 序號開頭
		/// </summary>
		public string SERIAL_BEGIN { get; set; }

		/// <summary>
		/// 序號檢查規則F000904 : 0純數/1非純數
		/// </summary>
		public string SERIAL_RULE { get; set; }

		

		/// <summary>
		/// 保存天數
		/// </summary>
		public Int32? SAVE_DAY { get; set; }

	

		/// <summary>
		/// 補貨安全庫存量
		/// </summary>
		public Int64 PICK_SAVE_QTY { get; set; }

		/// <summary>
		/// 是否為紙箱(0否1是)
		/// </summary>
		[Required]
		public string ISCARTON { get; set; }

		/// <summary>
		/// 是否為蘋果商品(0否1是)
		/// </summary>
		public string ISAPPLE { get; set; }


		/// <summary>
		/// 商品名稱
		/// </summary>
		public string ITEM_NAME { get; set; }

		/// <summary>
		/// 商品條碼一
		/// </summary>
		public string EAN_CODE1 { get; set; }

		/// <summary>
		/// 商品條碼二
		/// </summary>
		public string EAN_CODE2 { get; set; }

		/// <summary>
		/// 商品條碼三
		/// </summary>
		public string EAN_CODE3 { get; set; }

		/// <summary>
		/// EAN/ISBN
		/// </summary>
		public string EAN_CODE4 { get; set; }

		/// <summary>
		/// 商品類別(F000904)
		/// </summary>
		public string TYPE { get; set; }

		/// <summary>
		/// 商品規格
		/// </summary>
		public string ITEM_SPEC { get; set; }

		/// <summary>
		/// 溫層(F000904：01:常溫26-30、02:恆溫8-18、03冷藏-2~10、04:冷凍-18~-25) 
		/// </summary>
		public string TMPR_TYPE { get; set; }

		/// <summary>
		/// 易碎品包裝(0否1是)
		/// </summary>
		public string FRAGILE { get; set; }

		/// <summary>
		/// 防溢漏包裝(0否1是)
		/// </summary>
		public string SPILL { get; set; }

		/// <summary>
		/// 出貨是否附上保證書(0:否1是)
		/// </summary>
		public string LG { get; set; }


		/// <summary>
		/// 虛擬商品類別
		/// </summary>
		public string VIRTUAL_TYPE { get; set; }

		/// <summary>
		/// 大分類 F1915
		/// </summary>
		public string LTYPE { get; set; }

		/// <summary>
		/// 商品顏色
		/// </summary>
		public string ITEM_COLOR { get; set; }

		/// <summary>
		/// 商品尺寸
		/// </summary>
		public string ITEM_SIZE { get; set; }

		/// <summary>
		/// 停售日期
		/// </summary>
		public DateTime? STOP_DATE { get; set; }
	
		/// <summary>
		/// 是否為效期商品(null: 未選擇、0: 否、1: 是)
		/// </summary>
		public string NEED_EXPIRED { get; set; }
		/// <summary>
		/// 警示天數(原本的允售天數)
		/// </summary>
		public Int32? ALL_SHP { get; set; }
		
		/// <summary>
		/// 首次進貨日
		/// </summary>
		public DateTime? FIRST_IN_DATE { get; set; }
		/// <summary>
		/// 廠商代碼
		/// </summary>
		public string VNR_CODE { get; set; }
		/// <summary>
		/// 是否易遺失(0: 否, 1: 是)
		/// </summary>
		public string IS_EASY_LOSE { get; set; }
		/// <summary>
		/// 貴重品標示(0: 否, 1: 是)
		/// </summary>
		public string IS_PRECIOUS { get; set; }
		/// <summary>
		/// 強磁標示(0: 否, 1: 是)
		/// </summary>
		public string IS_MAGNETIC { get; set; }
	
		/// <summary>
		/// 易變質標示(0: 否, 1: 是)
		/// </summary>
		public string IS_PERISHABLE { get; set; }
		/// <summary>
		/// 需溫控標示(0: 否, 1: 是)
		/// </summary>
		public string IS_TEMP_CONTROL { get; set; }
		/// <summary>
		/// 廠商料號 (料號)
		/// </summary>
		public string VNR_ITEM_CODE { get; set; }
		/// <summary>
		/// 驗收註記
		/// </summary>
		public string RCV_MEMO { get; set; }
		/// <summary>
		/// 原廠商編號
		/// </summary>
		public string ORI_VNR_CODE { get; set; }
		/// <summary>
		/// 是否可混批(效期)擺放於儲位
		/// </summary>
		public string MIX_BATCHNO { get; set; }

		/// <summary>
		/// 越庫商品註記
		/// </summary>
		public string C_D_FLAG { get; set; }

		/// <summary>
		/// 批號管控商品(0:否 1:是)
		/// </summary>
		public string MAKENO_REQU { get; set; }

		/// <summary>
		/// 建檔日期
		/// </summary>
		public DateTime CRT_DATE { get; set; }

		/// <summary>
		/// 異動日期
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
	}
}
