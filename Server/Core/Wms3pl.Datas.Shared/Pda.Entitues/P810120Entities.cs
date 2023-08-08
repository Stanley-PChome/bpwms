using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
  #region 進貨容器綁定-已驗收需進行容器綁定資料查詢
  /// <summary>
  /// 進貨容器綁定-已驗收需進行容器綁定資料查詢_傳入
  /// </summary>
  public class RecvNeedBindContainerQueryReq : StaffModel
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
    /// 進倉單號/貨主單據號碼
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 品號/國際條碼/貨主品編
    /// </summary>
    public string ItemNo { get; set; }
  }

  /// <summary>
  /// 進貨容器綁定-已驗收需進行容器綁定資料查詢_傳回
  /// </summary>
  public class RecvNeedBindContainerQueryRes
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
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// 貨主單號
    /// </summary>
    public string CustOrdNo { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 驗收狀態代碼
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// 驗收狀態名稱
    /// </summary>
    public string StatusDesc { get; set; }
    /// <summary>
    /// 廠商代碼
    /// </summary>
    public string VnrCode { get; set; }
    /// <summary>
    /// 廠商名稱
    /// </summary>
    public string VnrName { get; set; }
    /// <summary>
    /// 快速通關代碼
    /// </summary>
    public string FastPassType { get; set; }
    /// <summary>
    /// 快速通關名稱
    /// </summary>
    public string FastPassTypeDesc { get; set; }
    /// <summary>
    /// 商品編號
    /// </summary>
    public string ItemCode { get; set; }
    /// <summary>
    /// 貨主品編
    /// </summary>
    public string CustItemCode { get; set; }
    /// <summary>
    /// 料號
    /// </summary>
    public string VnrItemCode { get; set; }
    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// 驗收數量
    /// </summary>
    public int RecvQty { get; set; }
  }
  #endregion

  #region 進貨容器綁定-驗收單綁定容器資料檢核與查詢
  /// <summary>
  /// 進貨容器綁定-驗收單綁定容器資料檢核與查詢_傳入
  /// </summary>
  public class RecvBindContainerCheckAndQueryReq : StaffModel
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
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 是否確認更換作業人員
    /// </summary>
    public Boolean IsChangeUser { get; set; }
  }

  /// <summary>
  /// 進貨容器綁定-驗收單綁定容器資料檢核與查詢_傳回
  /// </summary>
  public class RecvBindContainerCheckAndQueryRes
  {
    /// <summary>
    /// 是否需要複驗
    /// </summary>
    public Boolean NeedCheck { get; set; }
    /// <summary>
    /// 複驗狀態
    /// </summary>
    public String NeedCheckDesc { get; set; }
    /// <summary>
    /// 已分播數
    /// </summary>
    public int SowQty { get; set; }
    /// <summary>
    /// 良品數
    /// </summary>
    public int GoodQty { get; set; }
    /// <summary>
    /// 不良品數
    /// </summary>
    public int NoGoodQty { get; set; }
    /// <summary>
    /// 揀區倉庫編號
    /// </summary>
    public String PickWarehouse { get; set; }
    /// <summary>
    /// 揀區未分播數
    /// </summary>
    public int PickNoSowQty { get; set; }
    /// <summary>
    /// 揀區是否可綁定
    /// </summary>
    public Boolean PickCanBind { get; set; }
    /// <summary>
    /// 揀區識別碼
    /// </summary>
    public int PickAreaId { get; set; }
    /// <summary>
    /// 補區倉別編號
    /// </summary>
    public String ReplenishWarehouse { get; set; }
    /// <summary>
    /// 補區未分播數
    /// </summary>
    public int ReplenishNoSowQty { get; set; }
    /// <summary>
    /// 補區是否可綁定
    /// </summary>
    public Boolean ReplenishCanBind { get; set; }
    /// <summary>
    /// 補區識別碼
    /// </summary>
    public int ReplenishAreaId { get; set; }
    /// <summary>
    /// 不良區倉庫編號
    /// </summary>
    public String NoGoodWarehouse { get; set; }
    /// <summary>
    /// 不良區未分播數
    /// </summary>
    public int NoGoodNoSowQty { get; set; }
    /// <summary>
    /// 不良區是否可綁定
    /// </summary>
    public Boolean NoGoodCanBind { get; set; }
    /// <summary>
    /// 不良區識別碼
    /// </summary>
    public int NoGoodAreaId { get; set; }
  }
  #endregion

  #region 進貨容器綁定-驗收單各區綁定容器資料檢核與查詢
  /// <summary>
  /// 進貨容器綁定-驗收單各區綁定容器資料檢核與查詢_傳入
  /// </summary>
  public class RecvBindContainerByAreaCheckAndQueryReq : StaffModel
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
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 上架區域(A:揀區;C:補區;R:不良區)
    /// </summary>
    public string TypeCode { get; set; }
  }

  /// <summary>
  /// 進貨容器綁定-驗收單綁定容器資料檢核與查詢_傳回
  /// </summary>
  public class RecvBindContainerByAreaCheckAndQueryRes
  {
    /// <summary>
    /// 容器號碼
    /// </summary>
    public string ContainerCode { get; set; }
    /// <summary>
    /// 數量
    /// </summary>
    public int Qty { get; set; }
    /// <summary>
    /// 進貨容器識別碼
    /// </summary>
    public long F020501_ID { get; set; }
    /// <summary>
    /// 進貨容器明細識別碼
    /// </summary>
    public long F020502_ID { get; set; }
  }
  #endregion 進貨容器綁定-驗收單各區綁定容器資料檢核與查詢

  #region 進貨容器綁定-驗收單各區綁定容器放入確認
  //這個沒有回傳結構與"進貨容器綁定-驗收單綁定容器資料檢核與查詢"共用
  /// <summary>
  /// 進貨容器綁定-驗收單各區綁定容器放入確認_傳入
  /// </summary>
  public class RecvBindContainerByAreaPutConfirmReq : StaffModel
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
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 該區識別碼
    /// </summary>
    public int AreaId { get; set; }
    /// <summary>
    /// 上架倉庫編號
    /// </summary>
    public string WarehouseId { get; set; }
    /// <summary>
    /// 上架區域(A:揀區;C:補區;R:不良區)
    /// </summary>
    public string TypeCode { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    public string ContainerCode { get; set; }
    /// <summary>
    /// 放入數量
    /// </summary>
    public int PutQty { get; set; }
  }
  #endregion 進貨容器綁定-驗收單各區綁定容器放入確認

  #region 進貨容器綁定-驗收單各區移除綁定容器(RecvRemoveBindContainerByArea)
  /// <summary>
  /// 進貨容器綁定-驗收單各區綁定容器放入確認_傳入
  /// </summary>
  public class RecvRemoveBindContainerByAreaReq : StaffModel
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
  #endregion 進貨容器綁定-驗收單各區移除綁定容器

  #region 進貨容器綁定-驗收單待關箱資料查詢(RecvBindContainerWaitClosedQuery)
  /// <summary>
  /// 進貨容器綁定-驗收單各區綁定容器放入確認_傳入
  /// </summary>
  public class RecvBindContainerWaitClosedQueryReq : StaffModel
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
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
  }

  /// <summary>
  /// 進貨容器綁定-驗收單待關箱資料查詢
  /// </summary>
  public class RecvBindContainerWaitClosedQueryRes
  {
    /// <summary>
    /// 是否選取
    /// </summary>
    public Boolean IsSelected { get; set; }
    /// <summary>
    /// 進貨容器識別ID
    /// </summary>
    public long F020501_ID { get; set; }
    /// <summary>
    /// 容器綁定識別ID
    /// </summary>
    public long F0701_id { get; set; }
    /// <summary>
    /// 上架區域代碼
    /// </summary>
    public String TypeCode { get; set; }
    /// <summary>
    /// 上架區域名稱
    /// </summary>
    public String TypeName { get; set; }
    /// <summary>
    /// 容器號碼
    /// </summary>
    public String ContainerCode { get; set; }
    /// <summary>
    /// 上架倉庫編號
    /// </summary>
    public String PickWareId { get; set; }
    /// <summary>
    /// 上架倉庫名稱
    /// </summary>
    public String PickWareName { get; set; }
  }
  #endregion 進貨容器綁定-驗收單待關箱資料查詢(RecvBindContainerWaitClosedQuery)

  #region 進貨容器綁定-驗收單容器關箱確認(RecvBindContainerCloseBoxConfirm)
  /// <summary>
  /// 進貨容器綁定-驗收單容器關箱確認_傳入
  /// </summary>
  public class RecvBindContainerCloseBoxConfirmReq : StaffModel
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
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 是否需要複驗
    /// </summary>
    public Boolean NeedCheck { get; set; }
    /// <summary>
    /// 關箱進貨容器清單
    /// </summary>
    public List<NeedCheck> ContainerList { get; set; }
  }

  public class NeedCheck
  {
    /// <summary>
    /// 進貨容器識別碼
    /// </summary>
    public long F020501_ID { get; set; }
    /// <summary>
    /// 容器綁定識別碼
    /// </summary>
    public long F0701_ID { get; set; }
    /// <summary>
    /// 容器條碼
    /// </summary>
    public string CONTAINER_CODE { get; set; }
  }

  /// <summary>
  /// 進貨容器綁定-驗收單容器關箱確認_傳入
  /// </summary>
  public class RecvBindContainerCloseBoxConfirmRes
  {
    /// <summary>
    /// 是否成功
    /// </summary>
    public Boolean IsSuccessed { get; set; }
    /// <summary>
    /// 容器號碼
    /// </summary>
    public string ContainerCode { get; set; }
    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; }
  }
  #endregion 進貨容器綁定-驗收單容器關箱確認(RecvBindContainerCloseBoxConfirm)

  #region 進貨容器綁定-驗收單綁定完成(RecvBindContainerFinished)
  /// <summary>
  /// 進貨容器綁定-驗收單容器關箱確認_傳入
  /// </summary>
  public class RecvBindContainerFinishedReq : StaffModel
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
    /// 進倉單號
    /// </summary>
    public string StockNo { get; set; }
    /// <summary>
    /// 進倉項次
    /// </summary>
    public string StockSeq { get; set; }
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string RtNo { get; set; }
    /// <summary>
    /// 驗收項次
    /// </summary>
    public string RtSeq { get; set; }
    /// <summary>
    /// 是否需要複驗
    /// </summary>
    public Boolean NeedCheck { get; set; }
  }

  #endregion 進貨容器綁定-驗收單綁定完成(RecvBindContainerFinished)

}
