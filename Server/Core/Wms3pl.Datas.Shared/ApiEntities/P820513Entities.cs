using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
  /// <summary>
  /// 包裝完成回報(12)請求參數
  /// </summary>
  public class PackageFinishReq
  {
    /// <summary>
    /// 業主編號=WMS貨主編號
    /// </summary>
    public string OwnerCode { get; set; }
    /// <summary>
    /// 目的倉庫編號=物流中心編號
    /// </summary>
    public string DcCode { get; set; }
    /// <summary>
    /// 儲區編號
    /// </summary>
    public string ZoneCode { get; set; }
    /// <summary>
    /// 出庫單號=任務單號
    /// </summary>
    public string OrderCode { get; set; }
    /// <summary>
    /// 登錄人員=作業人員(工號)
    /// </summary>
    public string Operator { get; set; }
    /// <summary>
    /// 包裝開始時間(yyyy/MM/dd HH:mm:ss)
    /// </summary>
    public string StartTime { get; set; }
    /// <summary>
    /// 包裝完成時間(yyyy/MM/dd HH:mm:ss)
    /// </summary>
    public string CompleteTime { get; set; }
    /// <summary>
    /// 包裝箱資訊
    /// </summary>
    public List<PackageFinishBoxList> BoxList { get; set; }

  }

  /// <summary>
  /// 包裝箱資訊
  /// </summary>
  public class PackageFinishBoxList
  {
    /// <summary>
    /// 箱序(ex:1,2,3,4….)
    /// </summary>
    public int BoxSeq { get; set; }
    /// <summary>
    /// 紙箱編號(ex: 24H-01,ORI)
    /// </summary>
    public string BoxNo { get; set; }
    /// <summary>
    /// 列印箱明細時間(yyyy/MM/dd HH:mm:ss)
    /// </summary>
    public string PrintBoxTime { get; set; }
    /// <summary>
    /// 包裝箱明細
    /// </summary>
    public List<PackageFinishBoxDetail> BoxDetail { get; set; }
  }

  /// <summary>
  /// 包裝箱明細
  /// </summary>
  public class PackageFinishBoxDetail
  {
    /// <summary>
    /// 庫內品號
    /// </summary>
    public string SkuCode { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    public int SkuQty { get; set; }
    /// <summary>
    /// 商品序號(序號商品必填)
    /// </summary>
    public List<string> SerialNoList { get; set; } = new List<string>();
  }

  /// <summary>
  /// 包裝完成回報(12)回覆內容
  /// </summary>
  public class WcsApiPackageFinishResultData
  {
    /// <summary>
    /// 出庫單號
    /// </summary>
    public string No { get; set; }
    /// <summary>
    /// 錯誤欄位
    /// </summary>
    public string ErrorColumn { get; set; }
    /// <summary>
    /// 錯誤回應
    /// </summary>
    public WcsApiErrorResult errors { get; set; }

  }

}
