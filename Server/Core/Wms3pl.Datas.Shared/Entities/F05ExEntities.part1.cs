using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataContract]
	[DataServiceKey("ORD_NO")]
	public class F050101CustOrdNo
	{
		[DataMember]
		public string CUST_ORD_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
	}

    [Serializable]
    [DataContract]
    [DataServiceKey("WMS_ORD_NO")]
    public class F050801Data
    {
        [DataMember]
        public string WMS_ORD_NO { get; set; }
        [DataMember]
        public string ORD_NO { get; set; }
        [DataMember]
        public string BATCH_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string CONSIGN_NO { get; set; }
        [DataMember]
        public string ITEM_MAIL { get; set; }
        [DataMember]
        public string BILL_MAIL { get; set; }
        [DataMember]
        public string EXTENSION_A { get; set; }
    }

    #region 宅配通DEI
    [Serializable]
    [DataContract]
    [DataServiceKey("ORD_NO")]
    //[IgnoreProperties("EncryptedProperties")]
    public class GetF050901Data : IEncryptable
    {
        [DataMember]
        public string ORD_NO { get; set; }
        [DataMember]
        public string CONSIGN_NO { get; set; }
        [DataMember]
        public string BOXQTY { get; set; }
        [DataMember]
        public string ORD_TYPE { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string TEL { get; set; }
        [DataMember]
        public string CUST_ADDRESS { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("NAME")]
        public string CONSIGNEE { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("TEL")]
        public string CONTACT_TEL { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("ADDR")]
        public string ADDRESS { get; set; }
        [DataMember]
        public Decimal? COLLECT_AMT { get; set; }
        [DataMember]
        public string ERST_NO { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public string DELV_STATUS { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public Decimal STATUS { get; set; }
        [DataMember]
        public string CUST_EDI_STATUS { get; set; }
        [DataMember]
        public string ALL_ID { get; set; }
        [DataMember]
        public string ZIP_CODE { get; set; }
        [DataMember]
        public string INVO_ZIP { get; set; }
        [DataMember]
        public DateTime? PAST_DATE { get; set; }
        [DataMember]
        public DateTime? SEND_DATE { get; set; }

        //[IgnoreDataMember]
        //public Dictionary<string, string> EncryptedProperties
        //{
        //    get
        //    {
        //        return new Dictionary<string, string>
        //        {
        //            {"CONSIGNEE", "NAME"},{"ADDRESS", "ADDR"},{"CONTACT_TEL", "TEL"}
        //        };
        //    }
        //}
    }
    #endregion


    #region 無訂單派車所需欄位資訊
    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    //[IgnoreProperties("EncryptedProperties")]
    public class GetF050301Data : IEncryptable
    {
        [DataMember]
        public Decimal ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("NAME")]
        public string CONSIGNEE { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("TEL")]
        public string TEL { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public int ORD_QTY { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }

        //[IgnoreDataMember]
        //public Dictionary<string, string> EncryptedProperties
        //{
        //    get
        //    {
        //        return new Dictionary<string, string>
        //        {
        //            {"CONSIGNEE", "NAME"},{"TEL", "TEL"}
        //        };
        //    }
        //}
    }
    #endregion


    # region 出貨回傳檔下載
    [Serializable]
    [DataContract]
    [DataServiceKey("CUST_ORD_NO")]
    public class GetF050901CSV
    {

        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string ALL_ID { get; set; }
        [DataMember]
        public string CONSIGN_NO { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        
       
    }
    #endregion

    #region 排程-黑貓托運回檔
    [Serializable]
    [DataContract]
    [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE")]
    //[IgnoreProperties("EncryptedProperties")]
    public class EzcatConsign : IEncryptable
    {
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ORD_NO { get; set; }
        [DataMember]
        public string WMS_ORD_NO { get; set; }
        [DataMember]
        public string CONSIGN_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("NAME")]
        public string CONSIGNEE { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("TEL")]
        public string TEL_1 { get; set; }
        [DataMember]
        [Encrypted]
        [SecretPersonalData("ADDR")]
        public string ADDRESS { get; set; }
        [DataMember]
        public decimal? COLLECT_AMT { get; set; }
        [DataMember]
        public string ERST_NO { get; set; }
        [DataMember]
        public string DELV_TIMES { get; set; }
        [DataMember]
        public string DC_TEL { get; set; }
        [DataMember]
        public string DC_ADDRESS { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public DateTime ORD_DATE { get; set; }
        [DataMember]
        public string LOGCENTER_ID { get; set; }
        [DataMember]
        public string LOGCENTER_NAME { get; set; }
        [DataMember]
        public Decimal? LENGTH { get; set; }
        [DataMember]
        public Decimal? WIDTH { get; set; }
        [DataMember]
        public Decimal? HIGHT { get; set; }
        [DataMember]
        public string ZIP_CODE { get; set; }

        //[IgnoreDataMember]
        //public Dictionary<string, string> EncryptedProperties
        //{
        //    get { return new Dictionary<string, string> { { "CONSIGNEE", "NAME" }, { "TEL_1", "TEL" }, { "ADDRESS", "ADDR" } }; }
        //}
    }
    #endregion

    [Serializable]
    [DataContract]
    [DataServiceKey("ORD_NO", "ITEM_CODE")]
    public class F05080505Data
    {
        [DataMember]
        public string ORD_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
        [DataMember]
        public string CUST_COST { get; set; }
        [DataMember]
        public string FAST_DEAL_TYPE { get; set; }
        [DataMember]
        public string MOVE_OUT_TARGET { get; set; }
        [DataMember]
        public string WAREHOUSE_INFO { get; set; }
        [DataMember]
        public string IS_LACK_ORDER { get; set; }
    }


    [Serializable]
    [DataContract]
    [DataServiceKey("ORD_NO", "ITEM_CODE")]
    public class F05080506Data
    {
        [DataMember]
        public string ORD_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public int B_QTY { get; set; }
        [DataMember]
        public int A_QTY { get; set; }
        [DataMember]
        public string IS_LACK { get; set; }
        [DataMember]
        public string WAREHOUSE_INFO { get; set; }
    }

}
