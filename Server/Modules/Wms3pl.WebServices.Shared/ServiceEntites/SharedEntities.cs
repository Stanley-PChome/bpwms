using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
  [DebuggerDisplay("LOC_CODE = {F1912.LOC_CODE}, PutQty = {PutQty}, USEFUL_VOLUMN = {F1912.USEFUL_VOLUMN}, USED_VOLUMN = {F1912.USED_VOLUMN}")]
  public class SuggestLoc
  {
    public F1912 F1912 { get; set; }
    public long PutQty { get; set; }
  }

  public class SrcItemLocQty
  {
    /// <summary>
    /// 來源儲位
    /// </summary>
    public string LocCode { get; set; }
    public string ItemCode { get; set; }
    public DateTime ValidDate { get; set; }
    public DateTime EnterDate { get; set; }
    public long Qty { get; set; }
    /// <summary>
    /// 來源倉號
    /// </summary>
    public string SrcWarehouseId { get; set; }
    public string VnrCode { get; set; }
    public string SerialNo { get; set; }
    //指定建議上架儲位
    public string SugLocCode { get; set; }

    /// <summary>
    /// 指定上架廠商
    /// </summary>
    public string TarVnrCode { get; set; }

    /// <summary>
    /// 指定目的倉別
    /// </summary>
    public string TarWarehouseType { get; set; }
    /// <summary>
    /// 目的倉號(產生調撥單時回填，傳入時不需給)
    /// </summary>
    public string TarWarehouseId { get; set; }
    /// <summary>
    /// 調撥單號(產生調撥單時回填，傳入時不需給)
    /// </summary>
    public string AllocationNo { get; set; }

    /// <summary>
    /// 已產生明細
    /// </summary>
    public bool IsCreateDetail { get; set; }

    public string BankItemType { get; set; }

	/// <summary>
	/// 箱號
	/// </summary>
	public string BoxCtrlNo { get; set; }

	/// <summary>
	/// 板號
	/// </summary>
	public string PalletCtrlNo { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }
	}


  public class SrcItemLocQtyItem
  {
    /// <summary>
    /// 來源儲位
    /// </summary>
    public string LocCode { get; set; }
    public string ItemCode { get; set; }
    public int Qty { get; set; }
    /// <summary>
    /// 來源倉號
    /// </summary>
    public string WarehouseId { get; set; }
    //指定建議上架儲位
    public string SugLocCode { get; set; }
    //指定建議上架倉別
    public string SugWarehouseId { get; set; }

    /// <summary>
    /// 指定上架廠商
    /// </summary>
    public string TarVnrCode { get; set; }
    /// <summary>
    /// 指定目的倉別
    /// </summary>
    public string TarWarehouseType { get; set; }

		/// <summary>
		/// 指定商品效期
		/// </summary>
		public DateTime? VALID_DATE { get; set; }
		/// <summary>
		/// 指定商品入庫日
		/// </summary>
		public DateTime? ENTER_DATE { get; set; }

        /// <summary>
		/// 指定箱號(可不指定)
		/// </summary>
		public string BoxCtrlNo { get; set; }
        /// <summary>
        /// 指定板號(可不指定)
        /// </summary>
        public string PalletCtrlNo { get; set; }
        /// <summary>
		/// 批號
		/// </summary>
		public string MakeNo { get; set; }
    }

  /// <summary>
  /// 來源單據類型
  /// </summary>
  public enum SourceType
  {
    /// <summary>
    /// 進倉
    /// </summary>
    Stock = 1,
    /// <summary>
    /// 出貨
    /// </summary>
    Order = 2,
    /// <summary>
    /// 退貨
    /// </summary>
    Return = 3,
    /// <summary>
    /// 調撥
    /// </summary>
    Allocation = 4,

    /// <summary>
    /// 領用
    /// </summary>
    Receive = 7
  }

  /// <summary>
  /// 單據地址
  /// </summary>
  public class OrdAddress
  {
    /// <summary>
    /// 若是無單派車，則為派車單號ZC開頭，有單派車則可能為 "O", "R", "T", "D", "S"
    /// </summary>
    public string WmsNo { get; set; }
    /// <summary>
    /// 包裝箱號，一張出貨單可能有多張託運單，因每個要出車的包裝箱子都會貼託運單。預設1
    /// </summary>
    public short PackageBoxNo { get; set; }
    /// <summary>
    /// 為了取得路線配次
    /// </summary>
    public string ZipCode { get; set; }
    /// <summary>
    /// 為了取得新竹貨運到著站代碼
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// 派車用途
    /// </summary>
    public string DistrUse { get; set; }
    /// <summary>
    /// 配次
    /// </summary>
    public string DelvTimes { get; set; }
    /// <summary>
    /// 匯入檔案給的託運單號，可自訂託運單號產生F050901
    /// </summary>
    public string ConsignNo { get; set; }

    /// <summary>
    /// 新竹貨運到著站代碼 (外部服務取得)
    /// </summary>
    public string ErstNo { get; set; }
    /// <summary>
    /// 路線編號 (F194705)
    /// </summary>
    public string RouteCode { get; set; }
    /// <summary>
    /// 路線名稱 (F194705)
    /// </summary>
    public string RouteName { get; set; }
    /// <summary>
    /// 派車來源0:MANUALLY、1:API。預設 0(手動)
    /// </summary>
    public string DistrSource { get; set; }
    /// <summary>
    /// 託運單
    /// </summary>
    public F050901 F050901 { get; set; }
    /// <summary>
    /// 如果已經包裝後，這邊儲存原本舊的包裝箱，用來更新託運單號
    /// </summary>
    public F055001 OriginalF055001 { get; set; }

    /// <summary>
    /// 是否有用郵遞區號與配次找到路線編號?
    /// </summary>
    public bool HasFindZipCodeWithDelvTimes
    {
      get { return !string.IsNullOrEmpty(RouteCode); }
    }

		/// <summary>
		/// 產生託運單回傳錯誤訊息
		/// </summary>
		public string ErrorMessage { get; set; }
		/// <summary>
		///  加箱取號方式
		/// </summary>
		public string AddBoxGetConsignNo { get; set; }

		public OrdAddress()
    {
      DistrSource = "0";
      PackageBoxNo = 1;
			AddBoxGetConsignNo = "0"; //(預設0: 取新號碼)
		}
  }

  public class NearestLoc
  {
    public List<F1912> DataList { get; set; }
    public string WAREHOUSE_TYPE { get; set; }
    public string ATYPE_CODE { get; set; }
    public string WAREHOUSE_ID { get; set; }
		public bool IsAll { get; set; }
		public string DcCode { get; set; }
  }

  public class MixItemLoc
  {
    public List<F1912> DataList { get; set; }
    public string WAREHOUSE_TYPE { get; set; }
    public string ATYPE_CODE { get; set; }
    public string WAREHOUSE_ID { get; set; }
    public decimal? Volume { get; set; }
		public bool IsAll { get; set; }
		public string DcCode { get; set; }
  }

  /// <summary>
  /// 用於LO在增加量的傳遞參數物件
  /// </summary>
  //public class LoTicketItem
  //{
  //	public string TicketNo { get; set; }
  //	public string ItemCode { get; set; }
  //	public int AddQty { get; set; }

  //	public string SerialNo { get; set; }
  //	public string ManageValue { get; set; }

  //	/// <summary>
  //	/// 紀錄各個里程碑的LO
  //	/// </summary>
  //	public Dictionary<string, LO_MAIN> MileStoneLoMains { get; private set; }
  //	public LoTicketItem()
  //	{
  //		MileStoneLoMains = new Dictionary<string, LO_MAIN>();
  //	}
  //	/// <summary>
  //	/// 設定某個里程碑的LO
  //	/// </summary>
  //	/// <param name="mileStone"></param>
  //	/// <param name="loMain"></param>
  //	public void SetMileStoneLoMain(string mileStone, LO_MAIN loMain)
  //	{
  //		if (MileStoneLoMains.ContainsKey(mileStone))
  //			MileStoneLoMains[mileStone] = loMain;
  //		else
  //			MileStoneLoMains.Add(mileStone, loMain);
  //	}

  //	public LO_MAIN GetMileStoneLoMain(string mileStone)
  //	{
  //		if (MileStoneLoMains.ContainsKey(mileStone))
  //			return MileStoneLoMains[mileStone];
  //		else
  //			return null;
  //	}
  //}

  /// <summary>
  /// 用於配庫計算最低費用來挑選配送商時的資訊
  /// </summary>
  public class F050801ItemsInfo
  {
    /// <summary>
    /// 出貨單的總立方公分
    /// </summary>
    public decimal CubicCentimetre { get; private set; }
    /// <summary>
    /// 出貨單的總重量
    /// </summary>
    public decimal Weight { get; private set; }
    /// <summary>
    /// 出貨單的總材積
    /// </summary>
    public decimal Cuft { get; private set; }
    public F050801ItemsInfo(decimal cubicCentimetre, decimal weight, decimal cuft)
    {
      CubicCentimetre = cubicCentimetre;
      Weight = weight;
      Cuft = cuft;
    }
  }

  [DataContract]
  [Serializable]
  public class PackgeCode
  {
    /// <summary>
    /// 刷讀代號，如果第一次刷易通卡或挑卡，則這個 InputCode 會是上一次的序號
    /// </summary>
    [DataMember]
    public string InputCode { get; set; }
    /// <summary>
    /// 刷讀數量
    /// </summary>
    [DataMember]
    public int AddQty { get; set; }
    /// <summary>
    /// 商品類別
    /// </summary>
    [DataMember]
    public string Type { get; set; }
    /// <summary>
    /// 門號，當第一次刷易通卡或挑卡通過後，接著刷讀時才會帶到這個欄位
    /// </summary>
    [DataMember]
    public string CellNum { get; set; }
    /// <summary>
    /// 紙箱 ItemCode。一開始會由 Server 檢查後，暫時記憶在 Client，等到刷讀品號/序號時，在經由這個屬性傳遞到伺服器端做開箱。
    /// </summary>
    [DataMember]
    public string BoxNum { get; set; }
    /// <summary>
    /// 包裝箱號，開始刷品項後，會記錄1之後的包裝箱號，若只是刷紙箱，因不確定箱號，會記錄0(預設)
    /// </summary>
    [DataMember]
    public short PackageBoxNo { get; set; }

    [DataMember]
    public F050801 F050801Item { get; set; }

    [DataMember]
    public F050301[] F050301s { get; set; }

    [DataMember]
    public F1903[] F1903s { get; set; }
    [DataMember]
    public F050802[] F050802s { get; set; }
  }

  [Serializable]
  [DataContract]
  public class FinishCurrentBoxExecuteResult
  {
    [DataMember]
    public bool IsSuccessed { get; set; }
    [DataMember]
    public string Message { get; set; }
    [DataMember]
    public string LMSMessage { get; set; }
    [DataMember]
    public F050801 F050801Data { get; set; }
    [DataMember]
    public F055001 F055001Data { get; set; }
  }

  /// <summary>
  /// 地址解析的結果
  /// </summary>
  public class AddressParsedResult
  {
    public string GUP_CODE { get; set; }
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 是否需要解析地址
    /// </summary>
    public bool IsNeedParseAddress { get; set; }
    /// <summary>
    /// 原始地址
    /// </summary>
    public string ADDRESS { get; set; }
    /// <summary>
    /// 解析後的郵遞區號
    /// </summary>
    public string ZIP_CODE { get; set; }
    /// <summary>
    /// 解析後的地址
    /// </summary>
    public string ADDRESS_PARSE { get; set; }
    /// <summary>
    /// 解析時出現的例外錯誤
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// 有解析成功的話，就不在解析，不管有沒有取得郵遞區號
    /// </summary>
    public bool HasParsedSucceed { get; set; }

    /// <summary>
    /// 是否成功解析郵遞區號
    /// </summary>
    public bool IsSucceedParsedZipCode
    {
      get
      {
        return !string.IsNullOrWhiteSpace(ZIP_CODE);
      }
    }

    public AddressParsedResult()
    {
      ADDRESS_PARSE = string.Empty;
    }
  }

  /// <summary>
  /// 派車管理匯入
  /// </summary>
  public class DistrCarData
  {
    /// <summary>
    /// 儲存著要準備新增或修改的派車主檔
    /// </summary>
    public F700101 F700101 { get; set; }
    /// <summary>
    /// 儲存著要準備新增或修改的派車明細
    /// </summary>
    public List<F700102> F700102List { get; set; }

    /// <summary>
    /// 儲存著原本DB的派車主檔
    /// </summary>
    public F700101 OriginalF700101 { get; set; }
    /// <summary>
    /// 儲存著原本DB的派車明細
    /// </summary>
    public List<F700102> OriginalF700102List { get; set; }

    /// <summary>
    /// 託運單號
    /// </summary>
    public string ConsignNo { get; set; }
    public string Message { get; private set; }
    public string[] ImportItem { get; set; }

    public bool HasError
    {
      get { return !string.IsNullOrEmpty(Message); }
    }

    /// <summary>
    /// 設定錯誤訊息，當尚未有錯誤訊息之前，才會設定。
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public void SetErrorMessage(string message)
    {
      if (string.IsNullOrWhiteSpace(Message))
        Message = message;
    }
  }


    /// <summary>
    /// 加工回倉用
    /// </summary>
    public class F910101ToWarehouseData
    {
        public List<SrcItemLocQty> TarData { get; set; }

        public string TargetWarehouseType { get; set; }

        public bool IsDeleteOldAllocation { get; set; }

        public List<string> ExcludeAllocationNos { get; set; }
    }
}
