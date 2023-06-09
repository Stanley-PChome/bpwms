using System.Collections.Generic;
using Wms3pl.Datas.F06;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 人員資訊中介_傳入
    /// </summary>
    public class WcsUserReq
    {
        /// <summary>
        /// 業主編號=貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 明細數
        /// </summary>
        public int UserTotal { get; set; }
        /// <summary>
        /// 帳號列表
        /// </summary>
        public List<WcsUserModel> UserList { get; set; }
    }

    public class WcsUserModel
    {
        /// <summary>
        /// 人員帳號
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 人員名稱
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 人員密碼
        /// </summary>
        public string UserPw { get; set; }
        /// <summary>
        /// 啟用狀態(0=停用, 1=啟用)
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// 人員資訊中介_回傳
    /// </summary>
    public class WcsUserResData
    {
        /// <summary>
        /// UserId人員帳號
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }
        /// <summary>
        /// AGV錯誤回應
        /// </summary>
        public List<WcsErrorModel> errors { get; set; }
    }

    public class WcsUserExecuteModel
    {
        public F060301 F060301 { get; set; }
        public WcsUserModel UserData { get; set; }
    }
}
