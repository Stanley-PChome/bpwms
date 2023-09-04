using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;

namespace Wms3pl.Datas.Shared.Entities
{

    #region 調入上架
    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F1510Data
    {
        [DataMember]
        public int ROWNUM { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public DateTime ALLOCATION_DATE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string ALLOCATION_SEQ_LIST { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public int QTY { get; set; }
        [DataMember]
        public string SUG_LOC_CODE { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        [DataMember]
        public string BUNDLE_SERIALLOC { get; set; }
        [DataMember]
        public string CHECK_SERIALNO { get; set; }

        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }

        [DataMember]
        public DateTime? VALID_DATE { get; set; }
        [DataMember]
        public string TAR_DC_CODE { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }

        [DataMember]
        public string IS_NEW_ITEM { get; set; }

        [DataMember]
        public string MAKE_NO { get; set; }

        [DataMember]
        public string PALLET_CTRL_NO { get; set; }

        [DataMember]
        public string BOX_CTRL_NO { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F1510BundleSerialLocData
    {
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }

        [DataMember]
        public DateTime ALLOCATION_DATE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public short ALLOCATION_SEQ { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        [DataMember]
        public int TAR_QTY { get; set; }

    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F1510ItemLocData
    {
        [DataMember]
        public int ROWNUM { get; set; }
        /// <summary>
        /// Normal:資料庫撈出內容, Insert:修改上架儲位
        /// </summary>
        [DataMember]
        public string ChangeStatus { get; set; }

        [DataMember]
        public DateTime ALLOCATION_DATE { get; set; }

        [DataMember]
        public string ALLOCATION_NO { get; set; }

        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string WAREHOUSE_ID { get; set; }
        [DataMember]
        public string WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string SUG_LOC_CODE { get; set; }

        [DataMember]
        public string TAR_LOC_CODE { get; set; }

        [DataMember]
        public int ORGINAL_QTY { get; set; }
        [DataMember]
        public int QTY { get; set; }

        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public DateTime VALID_DATE { get; set; }

    }

    [Serializable]
    [DataServiceKey("ALLOCATION_NO", "ITEM_CODE", "TAR_LOC_CODE", "DC_CODE", "GUP_CODE", "CUST_CODE")]
    public class F1510QueryData
    {
        public DateTime ALLOCATION_DATE { get; set; }
        public string ALLOCATION_NO { get; set; }

        public string VNR_CODE { get; set; }

        public string VNR_NAME { get; set; }

        public string ITEM_CODE { get; set; }

        public string ITEM_NAME { get; set; }

        public string SUG_LOC_CODE { get; set; }

        public string TAR_LOC_CODE { get; set; }

        public string UPD_NAME { get; set; }

        public string BUNDLE_SERIALLOC { get; set; }

        public string DC_CODE { get; set; }

        public string GUP_CODE { get; set; }

        public string CUST_CODE { get; set; }

        public string STATUS { get; set; }

        public int QTY { get; set; }
    }
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class AllocationBundleSerialLocCount
    {
        public decimal ROWNUM { get; set; }

        public string AllocationNo { get; set; }

        public int RequiredQty { get; set; }

        public int CheckSerialNoQty { get; set; }
    }
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F15100101Data
    {
        public decimal ROWNUM { get; set; }
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string ALLOCATION_NO { get; set; }

        public string SERIAL_NO { get; set; }
        public string LOC_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_SIZE { get; set; }
        public string ITEM_COLOR { get; set; }
        public string ITEM_SPEC { get; set; }
        public string ISPASS { get; set; }
        public string MESSAGE { get; set; }
    }
    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class ImportBundleSerialLoc
    {
        [DataMember]
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public string LOC_CODE { get; set; }
    }

    #endregion

    #region 調撥相關
    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F151001Data
    {
        public System.Decimal ROWNUM { get; set; }
        public System.DateTime? ALLOCATION_DATE { get; set; }
        public string ALLOCATION_NO { get; set; }
        public System.DateTime? CRT_ALLOCATION_DATE { get; set; }
        public System.DateTime? POSTING_DATE { get; set; }
        public string STATUS { get; set; }
        public string TAR_DC_CODE { get; set; }
        public string TAR_WAREHOUSE_ID { get; set; }
        public string SRC_WAREHOUSE_ID { get; set; }
        public string SRC_DC_CODE { get; set; }
        public string SOURCE_TYPE { get; set; }
        public string SOURCE_NO { get; set; }
        public string BOX_NO { get; set; }
        public string MEMO { get; set; }
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string CRT_STAFF { get; set; }
        public System.DateTime? CRT_DATE { get; set; }
        public string UPD_STAFF { get; set; }
        public System.DateTime? UPD_DATE { get; set; }
        public string CRT_NAME { get; set; }
        public string UPD_NAME { get; set; }
    }

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P1502010000Data
    {
        public System.Decimal ROWNUM { get; set; }
        public string ALLOCATION_TYPE_NAME { get; set; }
        public string PRE_TAR_WAREHOUSE_NAME { get; set; }
        public string SRC_DC_NAME { get; set; }
        public string SRC_WH_NAME { get; set; }
        public string SRC_WH_DEVICE_TYPE { get; set; }
        public string TAR_DC_NAME { get; set; }
        public string TAR_WH_NAME { get; set; }
        public string TAR_WH_DEVICE_TYPE { get; set; }
        public string STATUS_NAME { get; set; }
        public string SOURCE_TYPE_NAME { get; set; }
        public DateTime ALLOCATION_DATE { get; set; }
        public string ALLOCATION_NO { get; set; }
        public DateTime CRT_ALLOCATION_DATE { get; set; }
        public DateTime? POSTING_DATE { get; set; }
        public string STATUS { get; set; }
        public string TAR_DC_CODE { get; set; }
        public string TAR_WAREHOUSE_ID { get; set; }
        public string SRC_WAREHOUSE_ID { get; set; }
        public string SRC_DC_CODE { get; set; }
        public string SOURCE_TYPE { get; set; }
        public string SOURCE_NO { get; set; }
        public string BOX_NO { get; set; }
        public string MEMO { get; set; }
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string CRT_STAFF { get; set; }
        public DateTime CRT_DATE { get; set; }
        public string UPD_STAFF { get; set; }
        public DateTime? UPD_DATE { get; set; }
        public string CRT_NAME { get; set; }
        public string UPD_NAME { get; set; }
        public string SEND_CAR { get; set; }
        public string SRC_MOVE_STAFF { get; set; }
        public string SRC_MOVE_NAME { get; set; }
        public string LOCK_STATUS { get; set; }
        public string TAR_MOVE_STAFF { get; set; }
        public string TAR_MOVE_NAME { get; set; }
        public string ISEXPENDDATE { get; set; }
        public string MOVE_TOOL { get; set; }
        public string ISMOVE_ORDER { get; set; }
        public DateTime? SRC_START_DATE { get; set; }
        public DateTime? TAR_START_DATE { get; set; }
        public string ALLOCATION_TYPE { get; set; }
        public string CONTAINER_CODE { get; set; }
        public Int64? F0701_ID { get; set; }
        public string PRE_TAR_WAREHOUSE_ID { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F151001DetailDatas
    {
        [DataMember]
        public System.Decimal ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string ALLOCATION_SEQ_LIST { get; set; }
        [DataMember]
        public System.DateTime ALLOCATION_DATE { get; set; }
        [DataMember]
        public System.DateTime? POSTING_DATE { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string SOURCE_TYPE { get; set; }
        [DataMember]
        public string SOURCE_NO { get; set; }
        [DataMember]
        public string SRC_DC_CODE { get; set; }
        [DataMember]
        public string TAR_DC_CODE { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string SEND_CAR { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }
        [DataMember]
        public string SUG_LOC_CODE { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }

        [DataMember]
        public string SHOW_SRC_LOC_CODE { get; set; }
        [DataMember]
        public string SHOW_SRC_QTY { get; set; }
        [DataMember]
        public string SHOW_A_SRC_QTY { get; set; }
        [DataMember]
        public string SHOW_TAR_LOC_CODE { get; set; }
        [DataMember]
        public System.Int64? SRC_QTY { get; set; }
        [DataMember]
        public System.Int64? A_SRC_QTY { get; set; }
        [DataMember]
        public System.Int64? TAR_QTY { get; set; }
        [DataMember]
        public System.Int64? A_TAR_QTY { get; set; }
        [DataMember]
        public System.DateTime? SRC_DATE { get; set; }
        [DataMember]
        public string SRC_STAFF { get; set; }
        [DataMember]
        public string SRC_NAME { get; set; }

        [DataMember]
        public System.DateTime? TAR_DATE { get; set; }
        [DataMember]
        public string TAR_STAFF { get; set; }
        [DataMember]
        public string TAR_NAME { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string CHECK_SERIALNO { get; set; }
        [DataMember]
        public string BUNDLE_SERIALLOC { get; set; }
        [DataMember]
        public string VnrCode { get; set; }
        [DataMember]
        public string SerialNo { get; set; }
        [DataMember]
        public string TarVnrCode { get; set; }
        [DataMember]
        public string BOX_CTRL_NO { get; set; }
        [DataMember]
        public string PALLET_CTRL_NO { get; set; }
        [DataMember]
        public DateTime? VALID_DATE { get; set; }
        [DataMember]
        public DateTime? ENTER_DATE { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
        [DataMember]
        public string SRC_MAKE_NO { get; set; }
        [DataMember]
        public string TAR_MAKE_NO { get; set; }
        [DataMember]
        public DateTime? SRC_VALID_DATE { get; set; }
        [DataMember]
        public DateTime? TAR_VALID_DATE { get; set; }
        [DataMember]
        public string CUST_ITEM_CODE { get; set; }
        [DataMember]
        public string EAN_CODE1 { get; set; }
        [DataMember]
        public string BIN_CODE { get; set; }
        [DataMember]
        public string ITEM_SOURCE_TYPE { get; set; }
        [DataMember]
        public string ITEM_SOURCE_TYPE_NAME { get; set; }
        [DataMember]
        public string ITEM_SOURCE_NO { get; set; }
        [DataMember]
        public string REFENCE_NO { get; set; }
        [DataMember]
        public string REFENCE_SEQ { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F151001ReportData
    {
        [DataMember]
        public System.Decimal ROWNUM { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }
        [DataMember]
        public string SUG_LOC_CODE { get; set; }
        [DataMember]
        public System.Int32 SRC_QTY { get; set; }
        [DataMember]
        public System.Int32 TAR_QTY { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string GUP_NAME { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string AllocationNoBarcode { get; set; }
        [DataMember]
        public string ItemBarcode { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public DateTime? VALID_DATE { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F151001ReportDataByExpendDate
    {
        [DataMember]
        public System.Decimal ROWNUM { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string SRC_LOC_CODE { get; set; }
        [DataMember]
        public string SUG_LOC_CODE { get; set; }
        [DataMember]
        public System.Int32 SRC_QTY { get; set; }
        [DataMember]
        public System.Int32 TAR_QTY { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string GUP_NAME { get; set; }
        [DataMember]
        public string CUST_NAME { get; set; }
        [DataMember]
        public string AllocationNoBarcode { get; set; }
        [DataMember]
        public string ItemBarcode { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public DateTime VALID_DATE { get; set; }
        [DataMember]
        public DateTime ENTER_DATE { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        /// <summary>
        /// 實際下架數
        /// </summary>
        [DataMember]
        public System.Int32 A_SRC_QTY { get; set; }
        /// <summary>
        /// 實際上架數
        /// </summary>
        [DataMember]
        public System.Int32 A_TAR_QTY { get; set; }
    }

    [Serializable]
    [DataContract]
    [DataServiceKey("ALLOCATION_NO")]
    public class F151001ReportDataByTicket
    {
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string AllocationNoBarcode { get; set; }
    }

    public class AllocTicketReportModel
    {
        public string DOC_ID { get; set; }
        public string WAREHOUSE_ID { get; set; }
        public string WAREHOUSE_NAME { get; set; }
    }
    #endregion

    [Serializable]
    [DataContract]
    [DataServiceKey("ROWNUM")]
    public class F151001ReportByAcceptance
    {
        [DataMember]
        public System.Decimal ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string WAREHOUSE_ID { get; set; }
        [DataMember]
        public string WAREHOUSE_NAME { get; set; }
        [DataMember]
        public string AllocationNoBarcode { get; set; }
    }

    #region P0803010000 同倉調撥下架
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F151002Data
    {
        public int ROWNUM { get; set; }
        public string ALLOCATION_NO { get; set; }

        public string ITEM_CODE { get; set; }

        public string ITEM_NAME { get; set; }

        public string SRC_LOC_CODE { get; set; }

        public string SERIAL_NO { get; set; }

        public DateTime VALID_DATE { get; set; }

        public string DC_CODE { get; set; }

        public string GUP_CODE { get; set; }

        public string CUST_CODE { get; set; }

        public string SRC_WAREHOUSE_ID { get; set; }

        public string WAREHOUSE_NAME { get; set; }

        public int SRC_QTY { get; set; }

        public string STATUS { get; set; }

        public string RET_UNIT { get; set; }

        public string SRC_MOVE_STAFF { get; set; }

        public string SRC_MOVE_NAME { get; set; }

        public string EAN_CODE1 { get; set; }
        public string EAN_CODE2 { get; set; }
        public string EAN_CODE3 { get; set; }

        public string BOX_NO { get; set; }

        public long? COMBIN_NO { get; set; }
        public string SERIAL_NOByShow { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        public string BOX_CTRL_NO { get; set; }

    }

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F151002ItemLocData
    {
        public int ROWNUM { get; set; }

        public string ALLOCATION_NO { get; set; }

        public string SRC_WAREHOUSE_ID { get; set; }

        public string WAREHOUSE_NAME { get; set; }

        public string ITEM_CODE { get; set; }

        public string SERIAL_NO { get; set; }

        public DateTime VALID_DATE { get; set; }

        public DateTime SRC_VALID_DATE { get; set; }

        public string SRC_LOC_CODE { get; set; }

        public string DC_CODE { get; set; }

        public string GUP_CODE { get; set; }

        public string CUST_CODE { get; set; }

        public int A_SRC_QTY { get; set; }
        /// <summary>
        /// 下架批號
        /// </summary>
        public string SRC_MAKE_NO { get; set; }
        /// <summary>
        /// 調撥單序號
        /// </summary>
        public int ALLOCATION_SEQ { get; set; }

    }
    #endregion

    #region P0803020000 同倉調撥上架
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F151002DataByTar
    {
        public int ROWNUM { get; set; }
        public string ALLOCATION_NO { get; set; }

        public string ITEM_CODE { get; set; }

        public string ITEM_NAME { get; set; }

        public string SUG_LOC_CODE { get; set; }

        public string SERIAL_NO { get; set; }

        public DateTime VALID_DATE { get; set; }

        public string DC_CODE { get; set; }

        public string GUP_CODE { get; set; }

        public string CUST_CODE { get; set; }

        public string TAR_WAREHOUSE_ID { get; set; }

        public string WAREHOUSE_NAME { get; set; }

        public int TAR_QTY { get; set; }

        public string STATUS { get; set; }

        public string RET_UNIT { get; set; }

        public string TAR_MOVE_STAFF { get; set; }

        public string TAR_MOVE_NAME { get; set; }

        public string EAN_CODE1 { get; set; }
        public string EAN_CODE2 { get; set; }
        public string EAN_CODE3 { get; set; }

        public string BOX_NO { get; set; }

        public string SERIAL_NOByShow { get; set; }
        public long? COMBIN_NO { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 板號
        /// </summary>
        public string PALLET_CTRL_NO { get; set; }
        /// <summary>
        /// 箱號
        /// </summary>
        public string BOX_CTRL_NO { get; set; }
    }

    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F151002ItemLocDataByTar
    {
        public int ROWNUM { get; set; }

        public string ALLOCATION_NO { get; set; }

        public string TAR_WAREHOUSE_ID { get; set; }

        public string WAREHOUSE_NAME { get; set; }

        public string ITEM_CODE { get; set; }

        public string SERIAL_NO { get; set; }

        public DateTime TAR_VALID_DATE { get; set; }

        public string TAR_LOC_CODE { get; set; }

        public string DC_CODE { get; set; }

        public string GUP_CODE { get; set; }

        public string CUST_CODE { get; set; }

        public int A_TAR_QTY { get; set; }
        /// <summary>
        /// 上架批號
        /// </summary>
        public string TAR_MAKE_NO { get; set; }
        /// <summary>
        /// 調撥單序號
        /// </summary>
        public int ALLOCATION_SEQ { get; set; }
    }
    #endregion

    #region P051002 Item 儲位數量查詢
    [Serializable]
    [DataServiceKey("ALLOCATION_NO")]
    public class F151002ItemData
    {
        public string ALLOCATION_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string SRC_LOC_CODE { get; set; }
        public string STATUS { get; set; }
        public Int32 SRC_QTY { get; set; }
        public Int32 A_SRC_QTY { get; set; }
        public Int32 TAR_QTY { get; set; }
        public Int32 A_TAR_QTY { get; set; }
        public DateTime? VALID_DATE { get; set; }
        public DateTime? ENTER_DATE { get; set; }

    }
    #endregion

    #region 虛擬儲位出貨數
    [DataContract]
    [Serializable]
    [DataServiceKey()]
    public class F1511WithF051202
    {
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string WMS_ORD_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public Int32? A_PICK_QTY_SUM { get; set; }
    }
    #endregion


    #region 取上架或下架儲位與數量資料
    [Serializable]
    [DataServiceKey("ALLOCATION_NO")]
    public class F151001LocData
    {
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string LOC_CODE { get; set; }
        [DataMember]
        public int QTY { get; set; }

    }
    #endregion

    [DataContract]
    [Serializable]
    [DataServiceKey("ALLOCATION_NO")]
    public class F151001WithF02020107
    {
        [DataMember]
        public DateTime ALLOCATION_DATE { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public DateTime CRT_ALLOCATION_DATE { get; set; }
        [DataMember]
        public DateTime? POSTING_DATE { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string TAR_DC_CODE { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string SRC_WAREHOUSE_ID { get; set; }
        [DataMember]
        public string SRC_DC_CODE { get; set; }
        [DataMember]
        public string SOURCE_TYPE { get; set; }
        [DataMember]
        public string SOURCE_NO { get; set; }
        [DataMember]
        public string BOX_NO { get; set; }
        [DataMember]
        public string MEMO { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string CRT_STAFF { get; set; }
        [DataMember]
        public DateTime CRT_DATE { get; set; }
        [DataMember]
        public string UPD_STAFF { get; set; }
        [DataMember]
        public DateTime? UPD_DATE { get; set; }
        [DataMember]
        public string CRT_NAME { get; set; }
        [DataMember]
        public string UPD_NAME { get; set; }
        [DataMember]
        public string SEND_CAR { get; set; }
        [DataMember]
        public string SRC_MOVE_STAFF { get; set; }
        [DataMember]
        public string SRC_MOVE_NAME { get; set; }
        [DataMember]
        public string LOCK_STATUS { get; set; }
        [DataMember]
        public string TAR_MOVE_STAFF { get; set; }
        [DataMember]
        public string TAR_MOVE_NAME { get; set; }
        [DataMember]
        public string RT_NO { get; set; }
    }

    [DataContract]
    [Serializable]
    [DataServiceKey("RT_NO", "PURCHASE_NO", "ALLOCATION_NO")]
    public class P081202Data
    {
        [DataMember]
        public string RT_NO { get; set; }
        [DataMember]
        public string PURCHASE_NO { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string WORKSTATION { get; set; }
        [DataMember]
        public Int32 ITEM_COUNT { get; set; }
        [DataMember]
        public string VNR_NAME { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
    }
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P08120201Data
    {
        public int ROWNUM { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public short ALLOCATION_SEQ { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
        [DataMember]
        public string TPS_LOC_CODE { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public string STATUS_DESC { get; set; }
        [DataMember]
        public DateTime VALID_DATE { get; set; }
    }

    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class AGVItemEnterData
    {
        [DataMember]
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string LOC_CODE { get; set; }
        [DataMember]
        public string AREA_CODE { get; set; }
        [DataMember]
        public string AREA_NAME { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
        [DataMember]
        public string ITEM_COLOR { get; set; }
        [DataMember]
        public string ITEM_SIZE { get; set; }
        [DataMember]
        public string ITEM_SPEC { get; set; }
        [DataMember]
        public DateTime VALID_DATE { get; set; }
        [DataMember]
        public int UNIT_QTY { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
        [DataMember]
        public string UNIT_TRANS { get; set; }
        [DataMember]
        public int TAR_QTY { get; set; }
        [DataMember]
        public string EAN_CODE1 { get; set; }
        [DataMember]
        public string EAN_CODE2 { get; set; }
        [DataMember]
        public string EAN_CODE3 { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public short ALLOCATION_SEQ { get; set; }
    }
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P08120502Data
    {
        [DataMember]
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string RT_NO { get; set; }
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        [DataMember]
        public string STOCK_NO { get; set; }
        [DataMember]
        public string CUST_ORD_NO { get; set; }
    }


    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P08120503Data
    {
        /// <summary>
        /// 序號
        /// </summary>
        [DataMember]
        public decimal ROWNUM { get; set; }
        /// <summary>
        /// 彙總上架單號
        /// </summary>
        [DataMember]
        public string BATCH_ALLOC_NO { get; set; }
        /// <summary>
        /// 彙總上架序號
        /// </summary>
        [DataMember]
        public decimal BATCH_ALLOC_SEQ { get; set; }
        /// <summary>
        /// 調撥單號
        /// </summary>
        [DataMember]
        public string ALLOCATION_NO { get; set; }
        /// <summary>
        /// 調撥單號
        /// </summary>
        [DataMember]
        public decimal ALLOCATION_SEQ { get; set; }
        /// <summary>
        /// 驗收單號
        /// </summary>
        [DataMember]
        public string RT_NO { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        [DataMember]
        public DateTime VALID_DATE { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [DataMember]
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 預計上架數
        /// </summary>
        [DataMember]
        public int B_QTY { get; set; }
        /// <summary>
        /// 實際上架數
        /// </summary>
        [DataMember]
        public int? A_QTY { get; set; }
        /// <summary>
        /// 預計上架儲位
        /// </summary>
        [DataMember]
        public string B_LOC_CODE { get; set; }
        /// <summary>
        /// 實際上架儲位
        /// </summary>
        [DataMember]
        public string A_LOC_CODE { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ITEM_CODE { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        [DataMember]
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        [DataMember]
        public string ITEM_SIZE { get; set; }
        /// <summary>
        /// 顏色
        /// </summary>
        [DataMember]
        public string ITEM_COLOR { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        [DataMember]
        public string ITEM_SPEC { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        [DataMember]
        public string ITEM_UNIT { get; set; }
        /// <summary>
        /// 條碼
        /// </summary>
        [DataMember]
        public string EAN_CODE2 { get; set; }
        /// <summary>
        /// 包裝參考
        /// </summary>
        [DataMember]
        public string UNIT_TRANS { get; set; }
        /// <summary>
        /// 物流中心編號
        /// </summary>
        [DataMember]
        public string DC_CODE { get; set; }
        /// <summary>
        /// 貨主
        /// </summary>
        [DataMember]
        public string GUP_CODE { get; set; }
        /// <summary>
        /// 業主
        /// </summary>
        [DataMember]
        public string CUST_CODE { get; set; }
        /// <summary>
        /// 原呼叫AGV儲位
        /// </summary>
        [DataMember]
        public string O_LOC_CODE { get; set; }
    }

    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P08120503ShelfItem
    {
        [DataMember]
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string GUP_CODE { get; set; }
        [DataMember]
        public string CUST_CODE { get; set; }
        [DataMember]
        public string BATCH_ALLOC_NO { get; set; }
        [DataMember]
        public string SHELF_NO { get; set; }
        [DataMember]
        public string SHELF_DIR { get; set; }
        [DataMember]
        public string SHELF_DIR_NAME { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string ITEM_NAME { get; set; }
    }

    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class P150201ExportSerial
    {
        [DataMember]
        public decimal ROWNUM { get; set; }
        [DataMember]
        public string ITEM_CODE { get; set; }
        [DataMember]
        public string SERIAL_NO { get; set; }
        [DataMember]
        public string TAR_LOC_CODE { get; set; }
    }

    [DataContract]
    [Serializable]
    [DataServiceKey("BACK_ITEM_TYPE", "TAR_WAREHOUSE_ID")]
    public class ProcessedData
    {
        [DataMember]
        public string BACK_ITEM_TYPE { get; set; }
        [DataMember]
        public string TAR_WAREHOUSE_ID { get; set; }
        [DataMember]
        public int TAR_QTY { get; set; }
    }

    #region 補貨調撥明細
    public class AllocDetailByReplenish
    {
        public string AllocNo { get; set; }
        public string ItemCode { get; set; }
		public string MakeNo { get; set; }
		public string SerialNo { get; set; }
		public Int64 TarQty { get; set; }
    }
  #endregion

  #region 調撥下架缺貨處理
  [DataContract]
  [Serializable]
  [DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ALLOCATION_NO", "ALLOCATION_SEQ")]
  public class P1502010500Data
  {
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
    /// <summary>
    /// 來源物流中心
    /// </summary>
    [DataMember]
    public string SRC_DC_CODE_NAME { get; set; }
    /// <summary>
    /// 來源倉別
    /// </summary>
    [DataMember]
    public string SRC_WAREHOUSE_NAME { get; set; }
    /// <summary>
    /// 調撥單號
    /// </summary>
    [DataMember]
    public string ALLOCATION_NO { get; set; }
    /// <summary>
    /// 調撥項次
    /// </summary>
    [DataMember]
    public int ALLOCATION_SEQ { get; set; }
    /// <summary>
    /// 來源儲位
    /// </summary>
    [DataMember]
    public string SRC_LOC_CODE { get; set; }
    /// <summary>
    /// 品號
    /// </summary>
    [DataMember]
    public string ITEM_CODE { get; set; }
    /// <summary>
    /// 品名
    /// </summary>
    [DataMember]
    public string ITEM_NAME { get; set; }
    /// <summary>
    /// 預計下架數
    /// </summary>
    [DataMember]
    public int SRC_QTY { get; set; }
    /// <summary>
    /// 缺貨數
    /// </summary>
    [DataMember]
    public int LACK_QTY { get; set; }
    /// <summary>
    /// 效期
    /// </summary>
    [DataMember]
    public DateTime VALID_DATE { get; set; }
    /// <summary>
    /// 批號
    /// </summary>
    [DataMember]
    public string MAKE_NO { get; set; }
    /// <summary>
    /// 入庫日
    /// </summary>
    [DataMember]
    public DateTime ENTER_DATE { get; set; }
    /// <summary>
    /// 序號
    /// </summary>
    [DataMember]
    public string SERIAL_NO { get; set; }
    /// <summary>
    /// 廠商編號
    /// </summary>
    [DataMember]
    public string VNR_CODE { get; set; }
  }
	#endregion 調撥下架缺貨處理

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class AddAllocationSuggestLocResult
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public ExecuteResult Result { get; set; }
		[DataMember]
		public List<F151001DetailDatas> F151001DetailDatas { get; set; }
	}

  #region 訂單取消資訊
  [DataContract]
  [Serializable]
  [DataServiceKey("ORD_NO")]
  public class OrderCancelInfo
  {
    [DataMember]
    public string TYPE { get; set; }
    [DataMember]
    public string ORD_NO { get; set; }
    [DataMember]
    public string SEQ_NO { get; set; }
    [DataMember]
    public string LOC_CODE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public DateTime? VALID_DATE { get; set; }
    [DataMember]
    public string MAKE_NO { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
    [DataMember]
    public int B_QTY { get; set; }
    [DataMember]
    public int A_QTY { get; set; }
    [DataMember]
    public string RETURN_LOC_CODE { get; set; }
  }
  #endregion 訂單取消資訊
}