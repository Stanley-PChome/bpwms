using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{

    #region 庫存API
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE")]
    public class StokInInfo
    {
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string QTY { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string VALID_DATE { get; set; }
    }
    #endregion


}
