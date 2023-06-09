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
    #region 進貨蒐集單維護_匯出進貨清冊 箱數表
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301Data 
    {
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string COLLECT_NO { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
    }
    #endregion

    #region 進貨蒐集單維護_匯出品號
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE")]
    public class F010301ITEM_CODE
    { 
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_ENGNAME { get; set; }
        [DataMember]
        public string ITEM_NICKNAME { get; set; }
        [DataMember]
        public string ITEM_TYPE { get; set; }
        [DataMember]
        public string ITEM_UNIT { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string ITEM_CLASS { get; set; }
        [DataMember]
        public string SIM_SPEC { get; set; }
        [DataMember]
        public string EAN_CODE1 { get; set; }
        [DataMember]
        public string EAN_CODE2 { get; set; }
        [DataMember]
        public string EAN_CODE3 { get; set; }
        [DataMember]
        public string LTYPE { get; set; }
        [DataMember]
        public string MTYPE { get; set; }
        [DataMember]
        public string STYPE { get; set; }
        [DataMember]
        public string TYPE { get; set; }
        [DataMember]
        public string ITEM_ATTR { get; set; }
        [DataMember]
        public string TMPR_TYPE { get; set; }
        [DataMember]
        public string ITEM_HUMIDITY { get; set; }
        [DataMember]
        public string VIRTUAL_TYPE { get; set; }
        [DataMember]
        public string LENGTH { get; set; }
        [DataMember]
        public string WIDTH { get; set; }
        [DataMember]
        public string HIGHT { get; set; }
        [DataMember]
        public string WEIGHT { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public string FRAGILE { get; set; }
        [DataMember]
        public string SPILL { get; set; }
        [DataMember]
        public string ISAPPLE { get; set; }
        [DataMember]
        public string CHECK_PERCENT { get; set; }
        [DataMember]
        public string PICK_SAVE_QTY { get; set; }
        [DataMember]
        public string SAVE_DAY { get; set; }
        [DataMember]
        public string PICK_SAVE_ORD { get; set; }
        [DataMember]
        public string ORD_SAVE_QTY { get; set; }
        [DataMember]
        public string BORROW_DAY { get; set; }
        [DataMember]
        public string EP_TAX { get; set; }
        [DataMember]
        public string SERIALNO_DIGIT { get; set; }
        [DataMember]
        public string SERIAL_BEGIN { get; set; }
        [DataMember]
        public string SERIAL_RULE { get; set; }
        [DataMember]
        public string PICK_WARE { get; set; }
        [DataMember]
        public string CUST_ITEM_CODE { get; set; }
        [DataMember]
        public string C_D_FLAG { get; set; }
        [DataMember]
        public string BUNDLE_SERIALLOC { get; set; }
        [DataMember]
        public string BUNDLE_SERIALNO { get; set; }
        [DataMember]
        public string MIX_BATCHNO { get; set; }
        [DataMember]
        public string LOC_MIX_ITEM { get; set; }
        [DataMember]
        public string ALLOWORDITEM { get; set; }
        [DataMember]
        public string NO_PRICE { get; set; }
        [DataMember]
        public string ISCARTON { get; set; }
        [DataMember]
        public string VEN_ORD { get; set; }
        [DataMember]
        public string RET_ORD { get; set; }
        [DataMember]
        public string ALL_DLN { get; set; }
        [DataMember]
        public string ALLOW_ALL_DLN { get; set; }
        [DataMember]
        public string ITEM_STAFF { get; set; }
        [DataMember]
        public string CAN_SPILT_IN { get; set; }
        [DataMember]
        public string LG { get; set; }
        [DataMember]
        public string ACC_TYPE { get; set; }
        [DataMember]
        public string DELV_QTY_AVG { get; set; }
    }
    #endregion

    #region 進貨蒐集單維護_匯出進貨清冊 箱數表
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301BOX_NO
    { 
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public decimal? COLLECT_QTY { get; set; }
    }
    #endregion
     
    #region 進貨蒐集單維護_匯出進貨清冊 箱明細
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301BOX_NO_ITEM 
    {
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public decimal? COLLECT_QTY { get; set; }
       
    }
    #endregion

    #region 進貨蒐集單維護_匯出進貨清冊 彙整總表
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301BOX_NO_ITEMTOTAL
    {
        [DataMember]
        public string CRT_DATE { get; set; }
        [DataMember]
        public string COLLECT_NO { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public decimal? COLLECT_QTY { get; set; }
        [DataMember]
        public string NAME { get; set; }

    }
    #endregion

    #region 進貨蒐集單維護_匯出進貨清冊 異常
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301BOX_NO_ITEMTOTALodd
    {
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public decimal? COLLECT_QTY { get; set; }
        [DataMember]
        public string NAME { get; set; }

    }
    #endregion

    #region 進貨蒐集單維護_匯出進貨清冊 單箱明細
    [Serializable]
    [DataContract]
    [DataServiceKey("BOX_NO")]
    public class F010301BOX_NO_ITEMTOTALoddONE
    {
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string COLLECT_STATUS { get; set; } 

    }
    #endregion

    #region 進貨蒐集單維護_匯出進貨清冊 單箱明細1
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE")]
    public class F010301BOX_NO_ITEMTOTALoddONE1
    {
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string COLLECT_STATUS { get; set; }

    }
    #endregion

    #region 進貨蒐集單維護_產生進倉單
    [Serializable]
    [DataContract]
    [DataServiceKey("ITEM_CODE")]
    public class F010301STOCK_NO
    {
        [DataMember]
        public string PO_NO { get; set; }
        [DataMember]
        public string VNR_CODE { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public int STOCK_QTY { get; set; }
        [DataMember] 
        public string COLLECT_NO { get; set; }
    }
    #endregion

    #region 進貨蒐集單維護_調撥資料
    [Serializable]
    [DataServiceKey("GUP_CODE")]
    public class F010301ALLOCATION_NO : F150201ImportData
    {
        [DataMember]
        public string ALLOCATION_NO { get; set; }    //調撥單單號
        [DataMember]
        public int TEST { get; set; }             //拆單暫存欄位
        [DataMember]
        public string COLLECT_NO { get; set; }             //蒐集單單號
    }
    #endregion
} 
