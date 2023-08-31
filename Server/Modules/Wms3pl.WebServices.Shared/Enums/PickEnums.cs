namespace Wms3pl.WebServices.Shared.Enums
{
  /// <summary>
  /// 揀貨單類型
  /// </summary>
  public enum PickTypeEnums
  {
    /// <summary>
    /// 全人工倉單一揀貨單
    /// </summary>
    FullArtificialSinglePick = 0,
    /// <summary>
    /// 全人工倉自我滿足批量揀貨單
    /// </summary>
    FullArtificialSelfBatchPick = 1,
    /// <summary>
    /// 自動倉非同條件揀貨單
    /// </summary>
    AutoPick = 2,
    /// <summary>
    /// 人工倉非同條件批量揀貨單
    /// </summary>
    ArtificialBatchPick = 3,
    /// <summary>
    /// 特殊結構揀貨單
    /// </summary>
    SpecialOrderPick = 4,
    /// <summary>
    /// 揀缺揀貨單
    /// </summary>
    LackPick = 5,
    /// <summary>
    /// 自動倉自我滿足揀貨單
    /// </summary>
    AutoSelfPick = 6,
    /// <summary>
    /// 特殊結構1單1品多PCS
    /// </summary>
    SpecialPick1 = 7,
    /// <summary>
    /// 特殊結構1單2品1PCS
    /// </summary>
    SpecialPick2 = 8
  }
  /// <summary>
  /// 批量拆單方式
  /// </summary>
  public enum SplitType
  {
    /// <summary>
    /// 依倉庫
    /// </summary>
    Warehouse = 1,
    /// <summary>
    /// 依PK區
    /// </summary>
    PkArea = 2,
    /// <summary>
    /// 出貨單
    /// </summary>
    ShipOrder = 3,
  }

  public enum NextStep
  {
    /// <summary>
    /// 未確認
    /// </summary>
    NoConfirm = 0,
    /// <summary>
    /// 分貨站
    /// </summary>
    AllotStation = 1,
    /// <summary>
    /// 集貨站
    /// </summary>
    CollectionStation = 2,
    /// <summary>
    /// 包裝站
    /// </summary>
    PackingStation = 3,

    /// <summary>
    /// 異常區
    /// </summary>
    ErrorStation = 4,


    /// <summary>
    /// 跨庫調撥出貨碼頭
    /// </summary>
    CrossAllotPier = 6,


  }
}
