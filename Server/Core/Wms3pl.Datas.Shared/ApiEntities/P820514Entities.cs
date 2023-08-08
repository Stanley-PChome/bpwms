using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
  /// <summary>
  /// LMS更新商品主檔
  /// </summary>
  public class UpdateItemInfoReq
  {
    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustCode { get; set; }
    /// <summary>
    /// 客戶品號
    /// </summary>
    public string CustItemCode { get; set; }
    /// <summary>
    /// 序號商品 (0: 否 1: 是)
    /// </summary>
    public string BundleSerialno { get; set; }
    /// <summary>
    /// 貼標用的條碼
    /// </summary>
    public string ItemBarCode { get; set; }
  }

  /// <summary>
  /// 預留
  /// </summary>
  public class UpdateItemInfoResultData
  {

  }

}
