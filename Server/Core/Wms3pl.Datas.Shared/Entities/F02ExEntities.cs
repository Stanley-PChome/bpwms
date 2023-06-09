using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Runtime.Serialization;
using Wms3pl.Datas.F02;

namespace Wms3pl.Datas.Shared.Entities
{
  //[Serializable]
  //[DataServiceKey("IsSuccessed")]
  //public class ExecuteResult
  //{
  //	public bool IsSuccessed { get; set; }
  //	public string Message { get; set; }
  //}

  /// <summary>
  /// 進倉預排的資料, 包含了VNR_NAME, ARRIVE_TIME的敘述
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("PURCHASE_NO", "SERIAL_NO", "DC_CODE", "GUP_CODE", "CUST_CODE", "ARRIVE_TIME", "ARRIVE_DATE")]
  public class F020103Detail
  {
    [DataMember]
    public string VNR_NAME { get; set; }
    [DataMember]
    public string CUST_NAME { get; set; }
    [DataMember]
    public string ARRIVE_TIME_DESC { get; set; }
    [DataMember]
    public System.Decimal? TOTALTIME { get; set; }
    [DataMember]
    public System.DateTime? ARRIVE_DATE { get; set; }
    [DataMember]
    public string ARRIVE_TIME { get; set; }
    [DataMember]
    public string PURCHASE_NO { get; set; }
    [DataMember]
    public string PIER_CODE { get; set; }
    [DataMember]
    public string VNR_CODE { get; set; }
    [DataMember]
    public string CAR_NUMBER { get; set; }
    [DataMember]
    public string BOOK_INTIME { get; set; }
    [DataMember]
    public string INTIME { get; set; }
    [DataMember]
    public string OUTTIME { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public System.DateTime? CRT_DATE { get; set; }
    [DataMember]
    public string CRT_STAFF { get; set; }
    [DataMember]
    public System.DateTime? UPD_DATE { get; set; }
    [DataMember]
    public string UPD_STAFF { get; set; }
    [DataMember]
    public System.Int32? ITEM_QTY { get; set; }
    [DataMember]
    public System.Int32? ORDER_QTY { get; set; }
    [DataMember]
    public System.Decimal? ORDER_VOLUME { get; set; }
    [DataMember]
    public System.Int16? SERIAL_NO { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
  }

  /// <summary>
  /// 碼頭期間設定資料
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("BEGIN_DATE", "END_DATE", "DC_CODE", "PIER_CODE")]
  public class F020104Detail
  {
    [DataMember]
    public System.DateTime? BEGIN_DATE { get; set; }
    [DataMember]
    public System.DateTime? END_DATE { get; set; }
    [DataMember]
    public string PIER_CODE { get; set; }
    [DataMember]
    public System.Int16? TEMP_AREA { get; set; }
    [DataMember]
    public string ALLOW_IN { get; set; }
    [DataMember]
    public string ALLOW_OUT { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public System.DateTime? CRT_DATE { get; set; }
    [DataMember]
    public string CRT_STAFF { get; set; }
    [DataMember]
    public System.DateTime? UPD_DATE { get; set; }
    [DataMember]
    public string UPD_STAFF { get; set; }
    [DataMember]
    public string CRT_NAME { get; set; }
    [DataMember]
    public string UPD_NAME { get; set; }
  }

  /// <summary>
  /// 商品檢驗
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class P020203Data
  {
    [DataMember]
    public Decimal ROW_NUM { get; set; }
    [DataMember]
    public string PURCHASE_NO { get; set; }
    [DataMember]
    public string PURCHASE_SEQ { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public System.Int32? ORDER_QTY { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public System.Decimal? SUM_RECV_QTY { get; set; }
    [DataMember]
    public System.Int32? RECV_QTY { get; set; }
    [DataMember]
    public System.Int32? DEFECT_QTY { get; set; }
    [DataMember]
    public System.Int32? CHECK_QTY { get; set; }
    [DataMember]
    public string BUNDLE_SERIALNO { get; set; }
    [DataMember]
    public string CHECK_SERIAL { get; set; }
    [DataMember]
    public string CHECK_ITEM { get; set; }
    [DataMember]
    public string ISPRINT { get; set; }
    [DataMember]
    public string ISUPLOAD { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string VNR_CODE { get; set; }
    [DataMember]
    public string VNR_NAME { get; set; }
    [DataMember]
    public string CLA_NAME { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public string ITEM_SIZE { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
    [DataMember]
    public bool ISREADONLY { get; set; }
    [DataMember]
    public string ISSPECIAL { get; set; }
    [DataMember]
    public string SPECIAL_CODE { get; set; }
    [DataMember]
    public string SPECIAL_DESC { get; set; }
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string RT_SEQ { get; set; }
    [DataMember]
    public DateTime? RECE_DATE { get; set; }

    [DataMember]
    public string IsNotNeedCheckScan { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string VIRTUAL_TYPE { get; set; }
    [DataMember]
    public DateTime? VALI_DATE { get; set; }
    [DataMember]
    public string SHOP_NO { get; set; }
    [DataMember]
    public int SERAIL_COUNT { get; set; }
    [DataMember]
    public string TARWAREHOUSE_ID { get; set; }
    [DataMember]
    public decimal PACK_HIGHT { get; set; }
    [DataMember]
    public decimal PACK_LENGTH { get; set; }
    [DataMember]
    public decimal PACK_WIDTH { get; set; }
    [DataMember]
    public decimal PACK_WEIGHT { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [DataMember]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 包裝參考
    /// </summary>
    [DataMember]
    public string UNIT_TRANS { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    /// <summary>
    /// 調撥單號
    /// </summary>
    [DataMember]
    public string ALLOCATION_NO { get; set; }

    /// <summary>
    /// 是否自有商品
    /// </summary>
    [DataMember]
    public string ISOEM { get; set; }
    [DataMember]

    /// <summary>
    /// 是否已有驗收單
    /// </summary>
    public string HasRecvData { get; set; }
    /// <summary>
    /// 允許快速進貨檢驗(0:否1:是) 若為是，在商品檢驗中可進行快驗
    /// </summary>
    public string IS_QUICK_CHECK { get; set; }
    /// <summary>
    /// 國際條碼一
    /// </summary>
    public string EAN_CODE1 { get; set; }
    /// <summary>
    /// 國際條碼二
    /// </summary>
    public string EAN_CODE2 { get; set; }
    /// <summary>
    /// 國際條碼三
    /// </summary>
    public string EAN_CODE3 { get; set; }
    /// <summary>
    /// EAN/ISBN
    /// </summary>
    public string EAN_CODE4 { get; set; }
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
    /// 允收天數
    /// </summary>
    public Int16? ALL_DLN { get; set; }
    /// <summary>
    /// 保存天數
    /// </summary>
    public Int32? SAVE_DAY { get; set; }
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
    /// 防溢漏包裝(0否1是)
    /// </summary>
    public string SPILL { get; set; }
    /// <summary>
    /// 易碎品包裝(0否1是)
    /// </summary>
    public string FRAGILE { get; set; }
    /// <summary>
    /// 快速通關分類
    /// </summary>
    public string FAST_PASS_TYPE { get; set; }
    /// <summary>
    /// 總品項數
    /// </summary>
    public System.Int32? ITEM_COUNT { get; set; }

    public DateTime UPD_DATE { get; set; }
    public string UPD_NAME { get; set; }

    public String REAL_RT_NO { get; set; }

    public String REAL_RT_SEQ { get; set; }
    /// <summary>
    /// 易變質標示(0: 否, 1: 是)
    /// </summary>
    public string IS_PERISHABLE { get; set; }
    /// <summary>
    /// 商品溫層
    /// </summary>
    public string TMPR_TYPE { get; set; }
    /// <summary>
    /// 需溫控標示
    /// </summary>
    public string IS_TEMP_CONTROL { get; set; }
  }
  /// <summary>
  /// 序號刷讀資料結構
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("SERIAL_NO")]
  public class SerialData
  {
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public bool ISPASS { get; set; }
    [DataMember]
    public string MESSAGE { get; set; }

  }

  [Serializable]
  [DataContract]
  [DataServiceKey("SERIAL_NO")]
  public class SerialDataEx
  {
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public bool ISPASS { get; set; }
    [DataMember]
    public string MESSAGE { get; set; }
    [DataMember]
    public string ITEMCODE { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string BOX_NO { get; set; }
  }

  /// <summary>
  /// 驗收單報表
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class AcceptancePurchaseReport
  {
    [DataMember]
    public System.Decimal ROW_NUM { get; set; }
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string RT_SEQ { get; set; }
    [DataMember]
    public string BUSPER { get; set; }
    [DataMember]
    public string VNR_CODE { get; set; }
    [DataMember]
    public string VNR_NAME { get; set; }
    [DataMember]
    public string RECV_TIME { get; set; }
    [DataMember]
    public string NAME { get; set; }
    [DataMember]
    public string FAX { get; set; }
    [DataMember]
    public string ORG_ORDER_NO { get; set; }
    [DataMember]
    public string ORDER_NO { get; set; }
    [DataMember]
    public string EMP_NAME { get; set; }
    [DataMember]
    public string ORD_TEL { get; set; }
    [DataMember]
    public string ORDER_DATE { get; set; }
    [DataMember]
    public string ORDER_SEQ { get; set; }
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
    public System.Decimal? PACK_QTY1 { get; set; }
    [DataMember]
    public string ORDER_UNIT { get; set; }
    [DataMember]
    public System.Decimal? CASE_QTY { get; set; }
    [DataMember]
    public System.Decimal ORDER_QTY { get; set; }
    [DataMember]
    public System.Decimal? AMOUNT { get; set; }
    [DataMember]
    public System.DateTime? ACE_DATE1 { get; set; }
    [DataMember]
    public string ACE_DATE2 { get; set; }
    [DataMember]
    public string ADDRESS { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string RET_UNIT { get; set; }
    [DataMember]
    public string CUST_ITEM_CODE { get; set; }
    [DataMember]
    public string VEN_ITEM_CODE { get; set; }
    [DataMember]
    public string ItemCodeBarcode { get; set; }
    [DataMember]
    public int RECV_QTY { get; set; }
    [DataMember]
    public string EAN_CODE1 { get; set; }
    [DataMember]
    public string EAN_CODE2 { get; set; }
    [DataMember]
    public string EAN_CODE3 { get; set; }
    [DataMember]
    public decimal PACK_WEIGHT { get; set; }

    /// <summary>
    /// 材積單位
    /// </summary>
    [DataMember]
    public string VOLUME_UNIT { get; set; }
    /// <summary>
    /// 上架倉別
    /// </summary>
    [DataMember]
    public string WAREHOUSE_NAME { get; set; }
    /// <summary>
    /// 快驗
    /// </summary>
    [DataMember]
    public string QUICK_CHECK { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [DataMember]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 驗收單條碼
    /// </summary>
    [DataMember]
    public string OrdNoBarcode { get; set; }
    /// <summary>
    /// 良品數
    /// </summary>
    [DataMember]
    public int SUM_RECV_QTY { get; set; }
    [DataMember]
    public string FAST_PASS_TYPE { get; set; }

    [DataMember]
    public string TARWAREHOUSE_ID { get; set; }
    [DataMember]
    public string ALLOCATION_NO { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("RT_NO")]
  public class AcceptanceReturnData
  {
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string OrderNo { get; set; }
    [DataMember]
    public ExecuteResult ExecuteResult { get; set; }
    [DataMember]
    public bool HasVirtualItem { get; set; }
    [DataMember]
    public List<AcceptanceSerialData> AcceptanceSerialDatas { get; set; }
    [DataMember]
    public bool IsOverWarehouseItem { get; set; }
  }
  [DataContract]
  [Serializable]
  [DataServiceKey("ITEM_CODE", "SERIAL_NO")]
  public class AcceptanceSerialData
  {
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string BARCODE { get; set; }
    [DataMember]
    public string MEMO { get; set; }
  }

  #region 商品檢驗-列印版箱貼紙資料

  /// <summary>
  /// 商品檢驗版箱貼紙列印
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class P0202030500PalletData
  {
    [DataMember]
    public Decimal ROWNUM { get; set; }

    /// <summary>
    /// 儲位
    /// </summary>
    [DataMember]
    public string LOC_CODE { get; set; }

    /// <summary>
    /// 進倉單號
    /// </summary>
    [DataMember]
    public string STOCK_NO { get; set; }

    /// <summary>
    /// 廠商名稱
    /// </summary>
    [DataMember]
    public string VNR_NAME { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    [DataMember]
    public string ITEM_CODE { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    [DataMember]
    public string ITEM_NAME { get; set; }

    /// <summary>
    /// 棧板疊法
    /// </summary>
    [DataMember]
    public string PALLET_LEVEL { get; set; }

    /// <summary>
    /// 商品箱入數
    /// </summary>
    [DataMember]
    public string ITEM_CASE_QTY { get; set; }

    /// <summary>
    /// 商品小包裝數
    /// </summary>
    [DataMember]
    public string ITEM_PACKAGE_QTY { get; set; }

    /// <summary>
    /// 棧板貼紙數
    /// </summary>
    [DataMember]
    public string STICKER_REF { get; set; }

    /// <summary>
    /// 商品條碼
    /// </summary>
    [DataMember]
    public string ENA_CODE1 { get; set; }

    /// <summary>
    /// 外箱條碼
    /// </summary>
    [DataMember]
    public string ENA_CODE3 { get; set; }

    /// <summary>
    /// 訂貨數說明
    /// </summary>
    [DataMember]
    public string ORDER_QTY_DESC { get; set; }

    /// <summary>
    /// 驗收數說明
    /// </summary>
    [DataMember]
    public string RECV_QTY_DESC { get; set; }

    /// <summary>
    /// 上架數說明
    /// </summary>
    [DataMember]
    public string TAR_QTY_DESC { get; set; }

    /// <summary>
    /// 入庫日(空白)
    /// </summary>
    [DataMember]
    public string ENTER_DATE { get; set; }

    /// <summary>
    /// 驗收數
    /// </summary>
    [DataMember]
    public int RECV_QTY { get; set; }

    /// <summary>
    /// 效期
    /// </summary>
    [DataMember]
    public string VALID_DATE { get; set; }

    /// <summary>
    /// 棧板編號
    /// </summary>
    [DataMember]
    public string STICKER_NO { get; set; }

    /// <summary>
    /// 棧板編號條碼
    /// </summary>
    [DataMember]
    public string STICKER_BARCODE { get; set; }

    /// <summary>
    /// 列印日期
    /// </summary>
    [DataMember]
    public string PRINT_DATE { get; set; }
  }

  #endregion

  /// <summary>
  /// 檔案上傳
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class FileUploadData
  {
    [DataMember]
    public System.Decimal ROW_NUM { get; set; }
    [DataMember]
    public string UPLOAD_TYPE { get; set; }
    [DataMember]
    public string UPLOAD_NAME { get; set; }
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string UPLOAD_DESC { get; set; }
    [DataMember]
    public System.Decimal? UPLOADED_COUNT { get; set; }
    [DataMember]
    public System.Decimal? SELECTED_COUNT { get; set; }
    /// <summary>
    /// 以 "|" 符號區隔的檔案列表
    /// </summary>
    [DataMember]
    public string SELECTED_FILES { get; set; }
    [DataMember]
    public string ISREQUIRED { get; set; }
  }


  /// <summary>
  /// 檔案上傳
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("SERIAL_NO")]
  public class F020302Data
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string PO_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public short SERIAL_LEN { get; set; }
    [DataMember]
    public DateTime VALID_DATE { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string SYS_CUST_CODE { get; set; }
    [DataMember]
    public string CELL_NUM { get; set; }
    [DataMember]
    public string PUK { get; set; }
    [DataMember]
    public string BATCH_NO { get; set; }
  }

  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class P020205Main
  {
    public Decimal ROWNUM { get; set; }
    public string FILE_NAME { get; set; }
    public string DC_CODE { get; set; }
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    public string PO_NO { get; set; }
  }

  [Serializable]
  [DataContract]
  [DataServiceKey("ROWNUM")]
  public class P020205Detail
  {
    [DataMember]
    public Decimal ROWNUM { get; set; }
    [DataMember]
    public string SYS_CUST_CODE { get; set; }
    [DataMember]
    public string PO_NO { get; set; }
    [DataMember]
    public string FILE_NAME { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public Int16 SERIAL_LEN { get; set; }
    [DataMember]
    public DateTime VALID_DATE { get; set; }
    [DataMember]
    public string STATUS_NAME { get; set; }
  }


  [DataContract]
  [Serializable]
  [DataServiceKey("PURCHASE_NO", "RT_NO", "ITEM_CODE")]
  public class F020201WithF02020101
  {
    [DataMember]
    public DateTime? RECE_DATE { get; set; }
    [DataMember]
    public string PURCHASE_NO { get; set; }
    [DataMember]
    public string PURCHASE_SEQ { get; set; }
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public Decimal? ORDER_QTY { get; set; }
    [DataMember]
    public Decimal? RECV_QTY { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string BUNDLE_SERIALNO { get; set; }

    /// <summary>
    /// 有效日期
    /// </summary>
    [DataMember]
    public string ACE_DATE2 { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [DataMember]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 快驗
    /// </summary>
    [DataMember]
    public string QUICK_CHECK { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    /// <summary>
    /// 廠商代碼
    /// </summary>
    [DataMember]
    public string VNR_CODE { get; set; }
    /// <summary>
    /// 廠商名稱
    /// </summary>
    [DataMember]
    public string VNR_NAME { get; set; }
    /// <summary>
    /// 商品尺寸
    /// </summary>
    [DataMember]
    public string ITEM_SIZE { get; set; }
    /// <summary>
    /// 商品規格
    /// </summary>
    [DataMember]
    public string ITEM_SPEC { get; set; }
    /// <summary>
    /// 商品顏色
    /// </summary>
    [DataMember]
    public string ITEM_COLOR { get; set; }
    /// <summary>
    /// 應刷數
    /// </summary>
    [DataMember]
    public Decimal? CHECK_QTY { get; set; }
    [DataMember]
    public string EAN_CODE1 { get; set; }
    [DataMember]
    public string EAN_CODE2 { get; set; }
    [DataMember]
    public DateTime? VALI_DATE { get; set; }
    [DataMember]
    public string ALLOCATION_NO { get; set; }
    [DataMember]
    public Int64? QTY { get; set; }
    [DataMember]
    public string UCC_CODE { get; set; }
    [DataMember]
    public string CAUSE { get; set; }
    [DataMember]
    public string TAR_WAREHOUSE_ID { get; set; }

  }

  /// <summary>
  /// 驗收確認參數
  /// </summary>
  [DataContract]
  [Serializable]
  [DataServiceKey("DcCode", "GupCode", "CustCode", "PurchaseNo", "RTNo")]
  public class AcceptanceConfirmParam
  {
    /// <summary>
    /// 物流中心代碼
    /// </summary>
    [DataMember]
    public string DcCode { get; set; }
    /// <summary>
    /// 業主代碼
    /// </summary>
    [DataMember]
    public string GupCode { get; set; }
    /// <summary>
    /// 貨主代碼
    /// </summary>
    [DataMember]
    public string CustCode { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    [DataMember]
    public string PurchaseNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    [DataMember]
    public string RTNo { get; set; }
    /// <summary>
    /// 是否揀貨區優先儲位
    /// </summary>
    [DataMember]
    public bool IsPickLocFirst { get; set; }
    /// <summary>
    /// 舊商品檢驗=0 商品檢驗與容器綁定=1
    /// </summary>
    [DataMember]
    public string RT_MODE { get; set; } = "0";
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ROWNUM")]
  public class F02020109Data
  {

    [DataMember]
    public Int64 ID { get; set; }
    [DataMember]
    public string ChangeFlag { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string STOCK_NO { get; set; }
    [DataMember]
    public int STOCK_SEQ { get; set; }
    [DataMember]
    public int? DEFECT_QTY { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string UCC_CODE { get; set; }
    [DataMember]
    public string CAUSE { get; set; }
    [DataMember]
    public string OTHER_CAUSE { get; set; }
    [DataMember]
    public string WAREHOUSE_ID { get; set; }

  }

	[DataContract]
	[Serializable]
	[DataServiceKey("ORDER_NO")]
	public class F0202Data
	{
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public DateTime? CHECKIN_DATE { get; set; }
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string ORDER_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public int STOCK_QTY { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }

    /// <summary>
    /// 快速通關分類
    /// </summary>
    [DataMember]
    public string FAST_PASS_TYPE { get; set; }
    /// <summary>
    /// 預定進倉日期
    /// </summary>
    [DataMember]
    public DateTime DELIVER_DATE { get; set; }
    /// <summary>
    /// 預定進倉時段
    /// </summary>
    [DataMember]
    public string BOOKING_IN_PERIOD { get; set; }
    /// <summary>
    /// 等待驗收時間
    /// </summary>
    [DataMember]
    public string ACCEPTANCE_WAITTIME { get; set; }
    /// <summary>
    /// 點收時間
    /// </summary>
    [DataMember]
    public DateTime? CHECKACCEPT_TIME { get; set; }
    /// <summary>
    /// 開始驗收時間
    /// </summary>
    [DataMember]
    public string BEGIN_CHECKACCEPT_TIME { get; set; }
    /// <summary>
    /// 建立進倉單日期
    /// </summary>
    [DataMember]
    public DateTime? STOCK_DATE { get; set; }
  }



  [DataContract]
  [Serializable]
  [DataServiceKey("RT_NO")]
  public class ContainerDetailData
  {

    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string BIN_CODE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public int QTY { get; set; }
    [DataMember]
    public string STATUS { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ID")]
  public class ItemBindContainerData : F0205
  {
    [DataMember]
    public String ITEM_NAME { get; set; }
    [DataMember]
    public String PICK_WARE_Name { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("TYPE_CODE", "PICK_WARE_ID", "CONTAINER_CODE")]
  public class BindContainerData
  {
    [DataMember]
    public Boolean IsSelected { get; set; }

    [DataMember]
    public Int64 F020501_ID { get; set; }
    /// <summary>
    /// 上架區域
    /// </summary>
    [DataMember]
    public String TYPE_CODE { get; set; }

    /// <summary>
    /// 上架區域名稱
    /// </summary>
    [DataMember]
    public String TYPE_CODE_NAME { get; set; }

    /// <summary>
    /// 上架倉別
    /// </summary>
    [DataMember]
    public String PICK_WARE_ID { get; set; }

    /// <summary>
    /// 上架倉別名稱
    /// </summary>
    [DataMember]
    public String PICK_WARE_NAME { get; set; }

    /// <summary>
    /// 容器編號
    /// </summary>
    [DataMember]
    public String CONTAINER_CODE { get; set; }

    /// <summary>
    /// F020502.RT_NO (驗收單號)
    /// </summary>
    [DataMember]
    public String RT_NO { get; set; }

    /// <summary>
    /// F020502.RT_SEQ (驗收序號)
    /// </summary>
    [DataMember]
    public String RT_SEQ { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ID")]
  public class AreaContainerData : F020502
  {
    /// <summary>
    /// F020501.CONTAINER_CODE
    /// </summary>
    [DataMember]
    public string MCONTAINER_CODE { get; set; }

    /// <summary>
    /// F020501.TYPE_CODE
    /// </summary>
    [DataMember]
    public string TYPE_CODE { get; set; }

  }

  public class AddF020501Result : ExecuteResult
  {
    public Int64 F020502_ID { get; set; }
    public Int64 F020501_ID { get; set; }
    public Boolean NeedFocuseContanerCode { get; set; } = false;
  }

  /// <summary>
  /// 商品檢驗與容器綁定
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class P020206Data
  {
    [DataMember]
    public Decimal ROW_NUM { get; set; }
    [DataMember]
    public string PURCHASE_NO { get; set; }
    [DataMember]
    public string PURCHASE_SEQ { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public System.Int32? ORDER_QTY { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public System.Decimal? SUM_RECV_QTY { get; set; }
    [DataMember]
    public System.Int32? RECV_QTY { get; set; }
    [DataMember]
    public System.Int32? DEFECT_QTY { get; set; }
    [DataMember]
    public System.Int32? CHECK_QTY { get; set; }
    [DataMember]
    public string BUNDLE_SERIALNO { get; set; }
    [DataMember]
    public string CHECK_SERIAL { get; set; }
    [DataMember]
    public string CHECK_ITEM { get; set; }
    [DataMember]
    public string ISPRINT { get; set; }
    [DataMember]
    public string ISUPLOAD { get; set; }
    [DataMember]
    public string STATUS { get; set; }
    [DataMember]
    public string VNR_CODE { get; set; }
    [DataMember]
    public string VNR_NAME { get; set; }
    [DataMember]
    public string CLA_NAME { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public string ITEM_SIZE { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
    [DataMember]
    public bool ISREADONLY { get; set; }
    [DataMember]
    public string ISSPECIAL { get; set; }
    [DataMember]
    public string SPECIAL_CODE { get; set; }
    [DataMember]
    public string SPECIAL_DESC { get; set; }
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string RT_SEQ { get; set; }
    [DataMember]
    public DateTime? RECE_DATE { get; set; }

    [DataMember]
    public string IsNotNeedCheckScan { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    [DataMember]
    public string VIRTUAL_TYPE { get; set; }
    [DataMember]
    public DateTime? VALI_DATE { get; set; }
    [DataMember]
    public string SHOP_NO { get; set; }
    [DataMember]
    public int SERAIL_COUNT { get; set; }
    [DataMember]
    public string TARWAREHOUSE_ID { get; set; }
    [DataMember]
    public decimal PACK_HIGHT { get; set; }
    [DataMember]
    public decimal PACK_LENGTH { get; set; }
    [DataMember]
    public decimal PACK_WIDTH { get; set; }
    [DataMember]
    public decimal PACK_WEIGHT { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [DataMember]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 包裝參考
    /// </summary>
    [DataMember]
    public string UNIT_TRANS { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    [DataMember]
    public string CUST_ORD_NO { get; set; }
    /// <summary>
    /// 調撥單號
    /// </summary>
    [DataMember]
    public string ALLOCATION_NO { get; set; }

    /// <summary>
    /// 是否自有商品
    /// </summary>
    [DataMember]
    public string ISOEM { get; set; }
    [DataMember]

    /// <summary>
    /// 是否已有驗收單
    /// </summary>
    public string HasRecvData { get; set; }
    /// <summary>
    /// 允許快速進貨檢驗(0:否1:是) 若為是，在商品檢驗中可進行快驗
    /// </summary>
    public string IS_QUICK_CHECK { get; set; }
    /// <summary>
    /// 國際條碼一
    /// </summary>
    public string EAN_CODE1 { get; set; }
    /// <summary>
    /// 國際條碼二
    /// </summary>
    public string EAN_CODE2 { get; set; }
    /// <summary>
    /// 國際條碼三
    /// </summary>
    public string EAN_CODE3 { get; set; }
    /// <summary>
    /// EAN/ISBN
    /// </summary>
    public string EAN_CODE4 { get; set; }
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
    /// 允收天數
    /// </summary>
    public Int16? ALL_DLN { get; set; }
    /// <summary>
    /// 保存天數
    /// </summary>
    public Int32? SAVE_DAY { get; set; }
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
    /// 防溢漏包裝(0否1是)
    /// </summary>
    public string SPILL { get; set; }
    /// <summary>
    /// 易碎品包裝(0否1是)
    /// </summary>
    public string FRAGILE { get; set; }
    /// <summary>
    /// 快速通關分類
    /// </summary>
    public string FAST_PASS_TYPE { get; set; }
    /// <summary>
    /// 總品項數
    /// </summary>
    public System.Int32? ITEM_COUNT { get; set; }

    public DateTime UPD_DATE { get; set; }
    public string UPD_NAME { get; set; }
  }

  [Serializable]
  [DataContract]
  [DataServiceKey("RT_NO")]
  public class AcceptanceDetail
  {
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string EAN_CODE1 { get; set; }
    [DataMember]
    public string EAN_CODE2 { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
    [DataMember]
    public string ITEM_SPEC { get; set; }
    [DataMember]
    public string ITEM_COLOR { get; set; }
    [DataMember]
    public DateTime? VALI_DATE { get; set; }
    [DataMember]
    public string TYPE_CODE_NAME { get; set; }
    [DataMember]
    public string PICK_WARE_NAME { get; set; }
    [DataMember]
    public int B_QTY { get; set; }
    [DataMember]
    public string NEED_DOUBLE_CHECK { get; set; }
  }

  [Serializable]
  [DataContract]
  [DataServiceKey("RT_NO")]
  public class AcceptanceContainerDetail
  {
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string CONTAINER_CODE { get; set; }
    [DataMember]
    public string BIN_CODE { get; set; }
    [DataMember]
    public string F020501_STATUS { get; set; }
    [DataMember]
    public string PICK_WARE_NAME { get; set; }
    [DataMember]
    public string ALLOCATION_NO { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string TYPE_CODE_NAME { get; set; }
    [DataMember]
    public int QTY { get; set; }
    [DataMember]
    public string F020502_STATUS { get; set; }
    [DataMember]
    public string RECHECK_CAUSE { get; set; }
    [DataMember]
    public string RECHECK_MEMO { get; set; }
    [DataMember]
    public string RCV_MEMO { get; set; }

  }

  [Serializable]
  [DataContract]
  [DataServiceKey("RT_NO")]
  public class DefectDetail
  {
    [DataMember]
    public string RT_NO { get; set; }
    [DataMember]
    public string WAREHOUSE_NAME { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public int QTY { get; set; }
    [DataMember]
    public string UCC_CODE_NAME { get; set; }
    [DataMember]
    public string CAUSE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
  }

  [DataContract]
  [Serializable]
  [DataServiceKey("ID")]
  public class DefectDetailReport
  {
    [DataMember]
    public Int64 ID { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public string UCC_CODE_NAME { get; set; }
    [DataMember]
    public string CAUSE { get; set; }
  }

  /// <summary>
  /// 複驗異常處理
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class F020504ExData
  {
    [DataMember]
    public Decimal ROW_NUM { get; set; }
    /// <summary>
    /// 流水號
    /// </summary>
    [DataMember]
    public Int64 ID { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public String STOCK_NO { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public String RT_NO { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    [DataMember]

    public String CONTAINER_CODE { get; set; }
    /// <summary>
    /// 容器分格條碼
    /// </summary>
    [DataMember]
    public String BIN_CODE { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    [DataMember]

    public String ITEM_CODE { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    [DataMember]

    public String ITEM_NAME { get; set; }
    /// <summary>
    /// 處理方式
    /// </summary>
    [DataMember]
    public String PROC_CODE { get; set; }
    /// <summary>
    /// 原驗收數
    /// </summary>
    [DataMember]
    public int QTY { get; set; }
    /// <summary>
    /// 拒絕驗收數
    /// </summary>
    [DataMember]
    public int? REMOVE_RECV_QTY { get; set; }
    /// <summary>
    /// 不良品數
    /// </summary>
    [DataMember]
    public int? NOTGOOD_QTY { get; set; }
    /// <summary>
    /// 處理狀態
    /// </summary>
    [DataMember]
    public String STATUS { get; set; }
  }

  /// <summary>
  /// 複驗異常處理紀錄
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]

  public class UnnormalItemRecheckLog
  {
    [DataMember]
    public Decimal ROW_NUM { get; set; }

    /// <summary>
    /// 項目
    /// </summary>
    [DataMember]
    public string PROC_DESC { get; set; }
    /// <summary>
    /// 作業人員
    /// </summary>
    [DataMember]
    public string PROC_NAME { get; set; }
    /// <summary>
    /// 作業時間
    /// </summary>
    [DataMember]
    public DateTime PROC_TIME { get; set; }
    /// <summary>
    /// 不通過原因
    /// </summary>
    [DataMember]
    public string RECHECK_CAUSE { get; set; }
    /// <summary>
    /// 備註
    /// </summary>
    [DataMember]
    public string MEMO { get; set; }

  }

  /// <summary>
  /// 複驗異常處理紀錄
  /// </summary>
  [Serializable]
  [DataContract]
  [DataServiceKey("ROW_NUM")]
  public class ContainerRecheckFaildItem
  {
    [DataMember]

    public Decimal ROW_NUM { get; set; }
    [DataMember]
    public long F020502_ID { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    [DataMember]
    public string STOCK_NO { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    [DataMember]
    public string STOCK_SEQ { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    [DataMember]
    public string RT_NO { get; set; }
    /// <summary>
    /// 驗收序號
    /// </summary>
    [DataMember]
    public string RT_SEQ { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    [DataMember]
    public string CONTAINER_CODE { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    [DataMember]
    public string BIN_CODE { get; set; }
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
    /// 數量
    /// </summary>
    [DataMember]
    public int QTY { get; set; }
    /// <summary>
    /// 驗收狀態名稱
    /// </summary>
    [DataMember]
    public string ACCE_STATUS { get; set; }
    /// <summary>
    /// 驗收狀態
    /// </summary>
    [DataMember]
    public string STATUS { get; set; }
		/// <summary>
		/// F020501 流水號
		/// </summary>
		[DataMember]
		public long F020501_ID { get; set; }

	}
	public class RtNoContainerStatus
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string STOCK_NO { get; set; }
		public string RT_NO { get; set; }
		public long F020501_ID { get; set; }
		public string F020501_STATUS { get; set; }
		public string ALLOCATION_NO { get; set; }
	}
}
