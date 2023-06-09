namespace Wms3pl.Datas.Shared
{
    /// <summary>
    /// 訊息代碼定義Model
    /// </summary>
    public class MsgModel
    {
        public string MsgCode { get; set; }

        public string MsgContent { get; set; }
    }

    public class StaffModel
    {
        public string AccNo { get; set; }
    }

    /// <summary>
    /// 商品溫層資料
    /// </summary>
    public class GetTmprData
    {
        /// <summary>
        /// 溫層(F000904：01:常溫26-30、02:恆溫8-18、03冷藏-2~10、04:冷凍-18~-25) 
        /// </summary>
        public string TmprType { get; set; }

        /// <summary>
        /// 溫層名稱
        /// </summary>
        public string TmprTypeName { get; set; }
    }
}
