using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    public class ChangeTransportProviderReq
    {
        /// <summary>
        /// 物流中心編號
        /// </summary>
        public string DcCode { get; set; }

        /// <summary>
        /// 貨主編號
        /// </summary>
        public string CustCode { get; set; }
        
        /// <summary>
        /// 出貨類型(1=訂單)
        /// </summary>
        public string OrderType { get; set; }
        
        /// <summary>
        /// 貨主出貨單號
        /// </summary>
        public string CustOrdNo { get; set; }

        public List<Result> PackingList { get; set; }
    }

    public class Result
    {
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsNo { get; set; }
       
        /// <summary>
        /// 箱數編號 (必須大於0且為正整數)
        /// </summary>
        public Int16 BoxNo { get; set; }
      
        /// <summary>
        /// 宅配單號
        /// </summary>
        public string TransportCode { get; set; }
     
        /// <summary>
        /// 物流商編號
        /// </summary>
        public string TransportProvider { get; set; }
    }
}
