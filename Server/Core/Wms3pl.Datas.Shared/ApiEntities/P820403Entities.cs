using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 商品序號主檔
    /// </summary>
    public class WcsSnReq
    {
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 1=新增, 2=刪除
        /// </summary>
        public int Action { get; set; }
        /// <summary>
        /// 品項數
        /// </summary>
        public int SkuTotal { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<WcsSnSkuModel> SkuList { get; set; }
    }

    /// <summary>
    /// 商品列表
    /// </summary>
    public class WcsSnSkuModel
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 商品序號
        /// </summary>
        public string SnCode { get; set; }
    }

    public class WcsSnResData
    {
        /// <summary>
        /// 錯誤序號
        /// </summary>
        public string SnCode { get; set; }
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }
        /// <summary>
        /// AGV錯誤回應
        /// </summary>
        public List<WcsErrorModel> errors { get; set; }
    }
}
