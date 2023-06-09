using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
    #region 調撥匯入資料
    [Serializable]
    [DataServiceKey("GUP_CODE")]
    public class F150201ImportData
    {
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string SRC_DC_CODE { get; set; }
        [DataMember]
        public string TAR_DC_CODE { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        [DataMember]
        public DateTime? VALID_DATE { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
        [DataMember]
        public int QTY { get; set; }


    }
    #endregion

    #region 調撥匯出資料
    [Serializable]
    [DataContract]
    [DataServiceKey("GUP_CODE")]
    public class GetF150201CSV
    {  
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string SRC_DC_CODE { get; set; }
        [DataMember]
        public string TAR_DC_CODE { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        [DataMember]
        public int? SRC_QTY { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string SOURCE_NO { get; set; }
        [DataMember]
        public string CRTDateS { get; set; }
        [DataMember]
        public string CRTDateE { get; set; }
        [DataMember]
        public DateTime? PostingDateS { get; set; }
        [DataMember]
        public DateTime? PostingDateE { get; set; }
    }
    #endregion
}

