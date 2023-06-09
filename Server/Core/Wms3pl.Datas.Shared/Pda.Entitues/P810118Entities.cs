using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
  /// <summary>
  /// 跨庫調撥驗收入自動倉-驗收完成_傳入
  /// </summary>
  public class MoveInToAutoRecvFinishReq : StaffModel
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
    /// 上架倉庫編號
    /// </summary>
    public string WarehouseId { get; set; }
    /// <summary>
    /// 容器編號
    /// </summary>
    public string ContainerCode { get; set; }
    /// <summary>
    /// 商品條碼(品號、國條1~3、序號)
    /// </summary>
    public string ItemCode { get; set; }
  }

}
