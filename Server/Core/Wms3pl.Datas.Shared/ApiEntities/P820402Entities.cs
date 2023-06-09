using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 商品主檔中介_傳入
    /// </summary>
    public class WcsItemReq
    {
        /// <summary>
        /// 業主編號=貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 品項數
        /// </summary>
        public int SkuTotal { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<WcsItemSkuModel> SkuList { get; set; }
    }

    /// <summary>
    /// 商品主檔中介明細物件
    /// </summary>
    public class WcsItemSkuModel
    {
        /// <summary>
        /// 庫內品號=商品編號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string SkuName { get; set; }
        /// <summary>
        /// 供應商編號
        /// </summary>
        public string VendorCode { get; set; }
        /// <summary>
        /// 商品辨識條碼清單
        /// </summary>
        public string[] BarCodeList { get; set; }
        /// <summary>
        /// 業主品號=貨主商品編碼
        /// </summary>
        public string OwnerSkuId { get; set; }
        /// <summary>
        /// 分類ID (路標) = 商品大分類
        /// </summary>
        public string SkuType { get; set; }
        /// <summary>
        /// 商品圖
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 商品規格
        /// </summary>
        public string SkuSpec { get; set; }
        /// <summary>
        /// 長(cm)四捨五入
        /// </summary>
        public decimal SkuLength { get; set; }
        /// <summary>
        /// 寬(cm)
        /// </summary>
        public decimal SkuWidth { get; set; }
        /// <summary>
        /// 高(cm)
        /// </summary>
        public decimal SkuHeight { get; set; }
        /// <summary>
        /// 重量(kg) 
        /// </summary>
        public decimal SkuWeight { get; set; }
        /// <summary>
        /// 保值期天數
        /// </summary>
        public int? ShelfLife { get; set; }
        /// <summary>
        /// 需效期管理(0無1有)
        /// </summary>
        public int HasExpiry { get; set; }
        /// <summary>
        /// 需以序號識別
        /// </summary>
        public int HasSerial { get; set; }
    }

    /// <summary>
    /// 商品主檔中介_傳入
    /// </summary>
    public class WcsItemResData
    {
        /// <summary>
        /// 錯誤品號
        /// </summary>
        public string SkuCode { get; set; }
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
