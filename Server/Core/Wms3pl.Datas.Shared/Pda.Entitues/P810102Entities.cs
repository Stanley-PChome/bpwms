using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.Pda.Entitues
{
	public class PostLogoutRes
	{
		public bool IsSuccessed { get; set; }
		public string MsgCode { get; set; }
		public string MsgContent { get; set; }
	}

	public class PostLogoutReq : StaffModel
	{
		// 功能編號
		public string FuncNo { get; set; }

		// 裝置驗證碼
		public string DevCode { get; set; }
	}

	public class PostLoginReq : StaffModel
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }

		/// <summary>
		/// 密碼
		/// </summary>
		public string Pwd { get; set; }

		/// <summary>
		/// 裝置驗證碼
		/// </summary>
		public string DevCode { get; set; }

		/// <summary>
		/// 機器號碼
		/// </summary>
		public string McCode { get; set; }
	}

	public class PostLoginRes
	{
    /// <summary>
    /// 功能權限資料
    /// </summary>
    public List<FuncList> FuncList { get; set; }

    /// <summary>
    /// 物流中心資料
    /// </summary>
    public List<DcList> DcList { get; set; }

		/// <summary>
		/// 業主資料
		/// </summary>
		public List<GupList> GupList { get; set; }

		/// <summary>
		/// 貨主資料
		/// </summary>
		public List<CustList> CustList { get; set; }

		/// <summary>
		/// 人員資料
		/// </summary>
		public UserInfo UserInfo { get; set; }

		/// <summary>
		/// 倉別資料
		/// </summary>
		public List<WarehouseInfo> WarehouseInfo { get; set; }

		/// <summary>
		/// PDA資訊
		/// </summary>
		public PdaInfo PdaInfo { get; set; }

		/// <summary>
		/// 集貨場資料
		/// </summary>
		public List<CollectionInfo> CollectionInfo { get; set; }

		/// <summary>
		/// 集貨場儲格資料
		/// </summary>
		public List<CellInfo> CellInfo { get; set; }

		/// <summary>
		/// 便利倉資料
		/// </summary>
		public List<ConvenientInfo> ConvenientInfo { get; set; }

    /// <summary>
    /// 紙箱工作站樓層資料
    /// </summary>
    public List<CartonWorkStationFloorInfo> CartonWorkStationFloorInfo { get; set; }

    /// 跨庫調撥入自動倉選單
    /// </summary>
    public List<MoveInAutoWarehouseInfo> MoveInAutoWarehouseInfo { get; set; }
  }

	public class FuncList
	{
		/// <summary>
		/// 功能編號
		/// </summary>
		public string FuncNo { get; set; }

		/// <summary>
		/// 功能名稱
		/// </summary>
		public string FuncName { get; set; }

		/// <summary>
		/// 功能順序
		/// </summary>
		public int FuncSeq { get; set; }

		/// <summary>
		/// 主功能選單顯示
		/// </summary>
		public string MainShow { get; set; }

		/// <summary>
		/// 側邊選單顯示
		/// </summary>
		public string SideShow { get; set; }
	}

	public class DcList
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcNo { get; set; }

		/// <summary>
		/// 物流中心名稱
		/// </summary>
		public string DcName { get; set; }
	}

	public class GupList
	{
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupNo { get; set; }

		/// <summary>
		/// 業主名稱
		/// </summary>
		public string GupName { get; set; }
	}

	public class CustList
	{
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CustNo { get; set; }

		/// <summary>
		/// 貨主名稱
		/// </summary>
		public string CustName { get; set; }

		/// <summary>
		/// 業主編號
		/// </summary>
		public string GupNo { get; set; }

		/// <summary>
		/// 允許修改箱入數
		/// </summary>
		public string AllowEditBoxQty { get; set; }

		/// <summary>
		/// 顯示上架確認訊息
		/// </summary>
		public string ShowMessage { get; set; }

		/// <summary>
		/// 顯示預帶確認數量
		/// </summary>
		public string ShowQty { get; set; }
	}

	public class UserInfo
	{
		/// <summary>
		/// 帳號
		/// </summary>
		public string AccNo { get; set; }

		/// <summary>
		/// 使用者名稱
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 主機授權碼
		/// </summary>
		public string SCCode { get; set; }
	}

	public class WarehouseInfo
	{
		/// <summary>
		/// 倉別編號
		/// </summary>
		public string WhNo { get; set; }

		/// <summary>
		/// 倉別名稱
		/// </summary>
		public string WhName { get; set; }
	}

	/// <summary>
	/// 集貨場資料
	/// </summary>
	public class CollectionInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 集貨場編號
		/// </summary>
		public string CollectionCode { get; set; }

		/// <summary>
		/// 集貨場名稱
		/// </summary>
		public string CollectionName { get; set; }
	}

	/// <summary>
	/// 集貨場儲格資料
	/// </summary>
	public class CellInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 儲格類型編號
		/// </summary>
		public string CellType { get; set; }

		/// <summary>
		/// 儲格類型名稱
		/// </summary>
		public string CellName { get; set; }
	}

	/// <summary>
	/// 便利倉資料
	/// </summary>
	public class ConvenientInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 便利場編號
		/// </summary>
		public string ConvenientCode { get; set; }

		/// <summary>
		/// 便利場名稱
		/// </summary>
		public string ConvenientName { get; set; }
	}

	public class CartonWorkStationFloorInfo
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DcCode { get; set; }
		/// <summary>
		/// 樓層代碼
		/// </summary>
		public string FloorCode { get; set; }
		/// <summary>
		/// 樓層名稱
		/// </summary>
		public string FloorName { get; set; }
	}

	public class PdaInfo
	{
		public string VersionNo { get; set; }
		public string ApkUrl { get; set; }
        /// <summary>
        /// 是否要同步
        /// </summary>
        public string IsSync { get; set; }
    }

	/// <summary>
	/// 帳號密碼檢核
	/// </summary>
	public class GetValidateUser
	{
		/// <summary>
		/// 帳號
		/// </summary>
		public string EmpId { get; set; }

		/// <summary>
		/// 密碼
		/// </summary>
		public string Password { get; set; }
		public DateTime CrtDate { get; set; }
		public DateTime? UpdDate { get; set; }
		public string CrtStaff { get; set; }
		public string UpdStaff { get; set; }
		public DateTime? LastActivityDate { get; set; }
		public DateTime? LastLoginDate { get; set; }
		public DateTime? LastPasswordChangedDate { get; set; }
		public DateTime? LastLockoutDate { get; set; }
		public int? FailedPasswordAttemptCount { get; set; }
		public int? Status { get; set; }
		public string CrtName { get; set; }
		public string UpdName { get; set; }
	}

	public class GetWmsNoByContainerCodeRes
	{
		public string WMS_NO { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

  /// <summary>
  /// 跨庫調撥入自動倉選單
  /// </summary>
  public class MoveInAutoWarehouseInfo
  {
    /// <summary>
    /// 物流中心
    /// </summary>
    public string DcNo { get; set; }

    /// <summary>
    /// 倉別編號
    /// </summary>
    public string WhNo { get; set; }

    /// <summary>
    /// 倉別名稱
    /// </summary>
    public string WhName { get; set; }

  }
}
