using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 集貨等待通知傳入參數
    /// </summary>
    public class WcsCollectionReq
    {
        /// <summary>
        /// 業主編號=WMS貨主編號
        /// </summary>
        public string OwnerCode { get; set; }

        /// <summary>
        /// 出庫單號 (揀貨單號)
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 上游出貨單號 (WMS出貨單號)
        /// </summary>
        public string OriOrderCode { get; set; }

        /// <summary>
        /// 狀態 (0=到齊就出, 1=等待補揀, 2=異常處理)
        /// </summary>
        public int Status { get; set; }
    }
}
