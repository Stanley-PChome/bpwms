using System;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
  public class GetStockReq : StaffModel
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
    /// 品號
    /// </summary>
    public string ItemNo { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    public string MkNo { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    public string Sn { get; set; }

    /// <summary>
    /// 倉別
    /// </summary>
    public string WhNo { get; set; }

    /// <summary>
    /// 儲位(起)
    /// </summary>
    public string BegLoc { get; set; }

    /// <summary>
    /// 儲位(迄)
    /// </summary>
    public string EndLoc { get; set; }

    /// <summary>
    /// 板號(起)
    /// </summary>
    public string BegPalletNo { get; set; }

    /// <summary>
    /// 板號(迄)
    /// </summary>
    public string EndPalletNo { get; set; }

    /// <summary>
    /// 入庫日(起)
    /// </summary>
    public DateTime? BegEnterDate { get; set; }

    /// <summary>
    /// 入庫日(迄)
    /// </summary>
    public DateTime? EndEnterDate { get; set; }

    /// <summary>
    /// 效期(起)
    /// </summary>
    public DateTime? BegValidDate { get; set; }

    /// <summary>
    /// 效期(迄)
    /// </summary>
    public DateTime? EndValidDate { get; set; }
  }

  public class GetStockRes
  {
    /// <summary>
    /// 物流中心編號
    /// </summary>
    public string DcNo { get; set; }

    /// <summary>
    /// 貨主編號
    /// </summary>
    public string CustNo { get; set; }

    /// <summary>
    /// 倉別
    /// </summary>
    public string WhName { get; set; }

    /// <summary>
    /// 儲位編號
    /// </summary>
    public string Loc { get; set; }

    /// <summary>
    /// 品號
    /// </summary>
    public string ItemNo { get; set; }

    /// <summary>
    /// 包裝參考
    /// </summary>
    public string PackRef { get; set; }

    /// <summary>
    /// 效期(yyyy/MM/dd)
    /// </summary>
    public DateTime ValidDate { get; set; }

    /// <summary>
    /// 入庫日(yyyy/MM/dd)
    /// </summary>
    public DateTime EnterDate { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 批號
    /// </summary>
    public string MkNo { get; set; }

    /// <summary>
    /// 商品序號
    /// </summary>
    public string Sn { get; set; }

    /// <summary>
    /// 庫存數
    /// </summary>
    public int StockQty { get; set; }

    /// <summary>
    /// 箱號
    /// </summary>
    public string BoxNo { get; set; }

    /// <summary>
    /// 板號
    /// </summary>
    public string PalletNo { get; set; }

    /// <summary>
    /// 儲位狀態
    /// </summary>
    public string LocStatus { get; set; }

    /// <summary>
    /// 剩餘效期天數
    /// </summary>
    public int DiffVDate { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 尺寸
    /// </summary>
    public string ItemSize { get; set; }
    /// <summary>
    /// 顏色
    /// </summary>
    public string ItemColor { get; set; }
    /// <summary>
    /// 規格
    /// </summary>
    public string ItemSpec { get; set; }

    /// <summary>
    /// 儲位虛擬數量
    /// </summary>
    public int BPickQty { get; set; }

    /// <summary>
    /// 國際條碼
    /// </summary>
    public string EANCode1 { get; set; }
    
    /// <summary>
    /// 序號商品 (0,1)
    /// </summary>
    public string BundleSerialNo { get; set; }
  }
}
