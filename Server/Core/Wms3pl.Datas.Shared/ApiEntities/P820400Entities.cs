using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 中介排程_傳入
    /// </summary>
    public class WcsExportReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }
        /// <summary>
        /// 業主編號
        /// </summary>
        public string GupCode { get; set; }
        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        /// <summary>
        /// 參數選擇執行哪一個排程回檔 
        /// 1 ->入庫任務
        /// 2 ->入庫任務取消
        /// 3 ->盤點任務
        /// 4 ->盤點任務取消
        /// null or Empty ->全部都執行
        /// </summary>
        public string ScheduleNo { get; set; }
    }

    public class WcsItemSnData
    {
        public List<ApiResponse> ItemErrorDatas { get; set; }
        public List<ApiResponse> ItemSnErrorDatas { get; set; }
    }
}
