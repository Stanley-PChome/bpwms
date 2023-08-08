using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
				public class ShipOrderInsertReq
				{
						/// <summary>
						/// 物流中心編號
						/// </summary>
						public string DcCode { get; set; }
						/// <summary>
						/// 貨主編號
						/// </summary>
						public string CustCode { get; set; }
						public ShipOrderInsertPackage Packages { get; set; }
				}

        public class ShipOrderInsertPackage
        {
            /// <summary>
            /// 貨主出貨單號
            /// </summary>
            public string CustOrdNo { get; set; }
            /// <summary>
            /// 單號類型
            /// </summary>
            public string OrderType { get; set; }
            /// <summary>
            /// 出貨單號
            /// </summary>
            public string WmsNo { get; set; }
            /// <summary>
            /// 箱數編號
            /// </summary>
            public int? BoxNo { get; set; }
            /// <summary>
            /// 箱號
            /// </summary>
            public string BoxNum { get; set; }
            /// <summary>
            /// 狀態(0:待處理 ;D:刪除)
            /// </summary>
            public string ProcFlag { get; set; }
            public List<ShipOrderInsertDetail> Details { get; set; } = new List<ShipOrderInsertDetail>();
        }

        public class ShipOrderInsertDetail
        {
            /// <summary>
            /// 原單據的品號項次
            /// </summary>
            public string ItemSeq { get; set; }
            /// <summary>
            /// 商品編號
            /// </summary>
            public string ItemCode { get; set; }
            /// <summary>
            /// 出貨商品數量
            /// </summary>
            public int OutQty { get; set; }
            /// <summary>
            /// 本商品的序號清單
            /// </summary>
            public string[] SnList { get; set; }
        }
}
