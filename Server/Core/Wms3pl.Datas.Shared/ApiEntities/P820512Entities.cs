namespace Wms3pl.Datas.Shared.ApiEntities
{
  public class SorterAbnormalNotifyReq
  {
    /// <summary>
    /// 業主編號=WMS貨主編號
    /// </summary>
    public string OwnerCode { get; set; }
    public string DcCode { get; set; }
    /// <summary>
    /// 目的倉庫編號=物流中心編號
    /// </summary>
    public string SorterCode { get; set; }
    /// <summary>
    /// 異常類型(1=讀取失敗, 2=分揀設備異常, 3=流道滿載, 9=其他)
    /// </summary>
    public int AbnormalType { get; set; }
    /// <summary>
    /// 異常訊息
    /// </summary>
    public string AbnormalMsg { get; set; }
    /// <summary>
    /// 異常物流單號(若可判讀就寫入)
    /// </summary>
    public string ShipCode { get; set; }
    /// <summary>
    /// 紀錄時間 (yyyy/MM/dd HH:ii:ss)
    /// </summary>
    public string CreateTime { get; set; }
  }

  public class WcsApiSorterAbnormalNotifyResultData
  {
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
