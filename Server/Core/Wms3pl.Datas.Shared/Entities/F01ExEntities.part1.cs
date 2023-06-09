using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
    #region 進倉明細查詢
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P010202SearchResult
    {
        [DataMember]
        public int ROWNUM { get; set; }
        [DataMember]
        public DateTime? ChangeDate { get; set; }
        [DataMember]
        public string ReceiptNo { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public int ChangeNumber { get; set; }
        [DataMember]
        public string GUP_NAME { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
    }
    #endregion
}
