using System;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
  #region 商品複驗-容器查詢
  /// <summary>
  /// 商品複驗-容器查詢_傳入
  /// </summary>
  public class GetRcvDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 容器條碼/容器分格條碼
    /// </summary>
    public string ContainerCode { get; set; }
  }

  /// <summary>
  /// 商品複驗-容器查詢_傳回
  /// </summary>
  public class GetRcvDataRes
  {
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long F020502_ID { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 貨主品編
    /// </summary>
    public string CustItemCode { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 上架數
    /// </summary>
    public int TarQty { get; set; }
  }
  #endregion

  #region 商品複驗-複驗查詢
  /// <summary>
  /// 商品複驗-複驗查詢_傳入
  /// </summary>
  public class GetRecheckDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long? F020502_ID { get; set; }
  }

  /// <summary>
  /// 商品複驗-複驗查詢_傳回
  /// </summary>
  public class GetRecheckDataRes
  {
    /// <summary>
    /// 貨主
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    public string CustOrdNo { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 貨主品編
    /// </summary>
    public string CustItemCode { get; set; }
    /// <summary>
    /// 國條一
    /// </summary>
    public string EanCode1 { get; set; }
    /// <summary>
    /// 國條二
    /// </summary>
    public string EanCode2 { get; set; }
    /// <summary>
    /// 國條三
    /// </summary>
    public string EanCode3 { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 商品規格
    /// </summary>
    public string ItemSpec { get; set; }
    /// <summary>
    /// 商品材積(cm)
    /// </summary>
    public string ItemSizeText { get; set; }
    /// <summary>
    /// 重量(g)
    /// </summary>
    public string ItemWeight { get; set; }
    /// <summary>
    /// 驗收數量
    /// </summary>
    public int Qty { get; set; }
    /// <summary>
    /// 抽驗數量
    /// </summary>
    public int CheckQty { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long F020502_ID { get; set; }
    /// <summary>
    /// 訊息
    /// </summary>
    public string ApiInfo { get; set; }

    //java沒辦法支援int?因此用字串型態傳遞
    /// <summary>
    /// 年限
    /// </summary>
    public String SaveDay { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public String ValidDate { get; set; }

    /// <summary>
    /// 旗標
    /// </summary>
    public string ItemFlagText { get; set; }

    /// <summary>
    /// 廠商料號 (料號)
    /// </summary>
    public string VnrItemCode { get; set; }

    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RcvMemo { get; set; }
  }
  #endregion

  #region 商品複驗-商品條碼檢驗
  /// <summary>
  /// 商品複驗-商品條碼檢驗_傳入
  /// </summary>
  public class ConfirmInputItemDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 正確的商品品號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 刷入的商品品號(含品號、國條、商品序號)
    /// </summary>
    public string InputItemCode { get; set; }
  }
  #endregion

  #region 商品複驗-複驗開始
  /// <summary>
  /// 商品複驗-複驗開始_傳入
  /// </summary>
  public class StartToReCheckReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long? F020502_ID { get; set; }
  }

  /// <summary>
  /// 商品複驗-複驗開始_傳回
  /// </summary>
  public class ReCheckCauseData
  {
    /// <summary>
    /// 原因編號
    /// </summary>
    public string UccCode { get; set; }
    /// <summary>
    /// 原因內容
    /// </summary>
    public string Cause { get; set; }
  }
  #endregion

  #region 商品複驗-複驗確認
  /// <summary>
  /// 商品複驗-複驗確認_傳入
  /// </summary>
  public class ConfirmRecheckDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long? F020502_ID { get; set; }
    /// <summary>
    /// 複驗模式(0: 通過)
    /// </summary>
    public int? InputType { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RcvMemo { get; set; }
  }
  #endregion

  #region 商品複驗-不通過原因登錄與容器移轉
  /// <summary>
  /// 商品複驗-不通過原因登錄與容器移轉_傳入
  /// </summary>
  public class ChangeContainerDataReq : StaffModel
  {
    /// <summary>
    /// 功能編號
    /// </summary>
    public string FuncNo { get; set; }
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }
    /// <summary>
    /// F020502的流水號
    /// </summary>
    public long? F020502_ID { get; set; }
    /// <summary>
    /// 選擇容器模式(0: 更換、1: 不更換)
    /// </summary>
    public int? InputType { get; set; }
    /// <summary>
    /// 複驗不通過原因
    /// </summary>
    public string CauseCode { get; set; }
    /// <summary>
    /// 備註
    /// </summary>
    public string Note { get; set; }
    /// <summary>
    /// 新容器條碼
    /// </summary>
    public string NewContainerCode { get; set; }
    /// <summary>
    /// 驗收註記
    /// </summary>
    public string RcvMemo { get; set; }
  }
  #endregion
}
