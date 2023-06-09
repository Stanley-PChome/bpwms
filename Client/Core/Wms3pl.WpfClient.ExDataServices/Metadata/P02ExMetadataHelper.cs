using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P02ExDataService
{

    [MetadataType(typeof(ExecuteResult.MetaData))]
    public partial class ExecuteResult : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean IsSuccessed { get; set; }
            public string Message { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<ExecuteResult>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<ExecuteResult>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(VendorInfo.MetaData))]
    public partial class VendorInfo : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<VendorInfo>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<VendorInfo>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F020103Detail.MetaData))]
    public partial class F020103Detail : IDataErrorInfo
    {
        public class MetaData
        {
            public string VNR_NAME { get; set; }
            public string CUST_NAME { get; set; }
            public string ARRIVE_TIME_DESC { get; set; }
            public Decimal? TOTALTIME { get; set; }
            public DateTime? ARRIVE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ARRIVE_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            public string PIER_CODE { get; set; }
            public string VNR_CODE { get; set; }
            public string CAR_NUMBER { get; set; }
            public string BOOK_INTIME { get; set; }
            public string INTIME { get; set; }
            public string OUTTIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public Int32? ITEM_QTY { get; set; }
            public Int32? ORDER_QTY { get; set; }
            public Decimal? ORDER_VOLUME { get; set; }
            public Int16? SERIAL_NO { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020103Detail>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F020103Detail>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F020104Detail.MetaData))]
    public partial class F020104Detail : IDataErrorInfo
    {
        public class MetaData
        {
            public DateTime? BEGIN_DATE { get; set; }
            public DateTime? END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PIER_CODE { get; set; }
            public Int16? TEMP_AREA { get; set; }
            public string ALLOW_IN { get; set; }
            public string ALLOW_OUT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020104Detail>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F020104Detail>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(P020203Data.MetaData))]
    public partial class P020203Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROW_NUM { get; set; }
            public string PURCHASE_NO { get; set; }
            public string PURCHASE_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public Int32? ORDER_QTY { get; set; }
            public string ITEM_NAME { get; set; }
            public Decimal? SUM_RECV_QTY { get; set; }
            public Int32? RECV_QTY { get; set; }
            public Int32? DEFECT_QTY { get; set; }
            public Int32? CHECK_QTY { get; set; }
            public string BUNDLE_SERIALNO { get; set; }
            public string CHECK_SERIAL { get; set; }
            public string CHECK_ITEM { get; set; }
            public string ISPRINT { get; set; }
            public string ISUPLOAD { get; set; }
            public string STATUS { get; set; }
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }
            public string CLA_NAME { get; set; }
            public string ITEM_COLOR { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean ISREADONLY { get; set; }
            public string ISSPECIAL { get; set; }
            public string SPECIAL_CODE { get; set; }
            public string SPECIAL_DESC { get; set; }
            public string RT_NO { get; set; }
            public string RT_SEQ { get; set; }
            public DateTime? RECE_DATE { get; set; }
            public string IsNotNeedCheckScan { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string VIRTUAL_TYPE { get; set; }
            public DateTime? VALI_DATE { get; set; }
            public string EAN_CODE1 { get; set; }

            public DateTime UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<P020203Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<P020203Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190206CheckName.MetaData))]
    public partial class F190206CheckName : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_TYPE { get; set; }
            public string CHECK_NAME { get; set; }
            public string UCC_CODE { get; set; }
            public string ISPASS { get; set; }
            public string MEMO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190206CheckName>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190206CheckName>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(SerialData.MetaData))]
    public partial class SerialData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean ISPASS { get; set; }
            public string MESSAGE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<SerialData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<SerialData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(SerialDataEx.MetaData))]
    public partial class SerialDataEx : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean ISPASS { get; set; }
            public string MESSAGE { get; set; }
            public string ITEMCODE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<SerialDataEx>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<SerialDataEx>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(AcceptancePurchaseReport.MetaData))]
    public partial class AcceptancePurchaseReport : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROW_NUM { get; set; }
            public string RT_NO { get; set; }
            public string RT_SEQ { get; set; }
            public string BUSPER { get; set; }
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }
            public string RECV_TIME { get; set; }
            public string NAME { get; set; }
            public string FAX { get; set; }
            public string ORG_ORDER_NO { get; set; }
            public string ORDER_NO { get; set; }
            public string EMP_NAME { get; set; }
            public string ORD_TEL { get; set; }
            public string ORDER_DATE { get; set; }
            public string ORDER_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public Decimal? PACK_QTY1 { get; set; }
            public string ORDER_UNIT { get; set; }
            public Decimal? CASE_QTY { get; set; }
            public Decimal? ORDER_QTY { get; set; }
            public Decimal? AMOUNT { get; set; }
            public DateTime? ACE_DATE1 { get; set; }
            public DateTime? ACE_DATE2 { get; set; }
            public string ADDRESS { get; set; }
            public string GUP_CODE { get; set; }
            public string RET_UNIT { get; set; }
            public string CUST_ITEM_CODE { get; set; }
            public string VEN_ITEM_CODE { get; set; }
            public Decimal? RECV_QTY { get; set; }
            public string EAN_CODE1 { get; set; }
            public string EAN_CODE2 { get; set; }
            public string EAN_CODE3 { get; set; }
            public decimal PACK_WEIGHT { get; set; }
            public string VOLUME_UNIT { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string QUICK_CHECK { get; set; }
            public string MAKE_NO { get; set; }
            public int SUM_RECV_QTY { get; set; }
            public string FAST_PASS_TYPE { get; set; }
            public string TARWAREHOUSE_ID { get; set; }
            public string ALLOCATION_NO { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<AcceptancePurchaseReport>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<AcceptancePurchaseReport>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(AcceptanceReturnData.MetaData))]
    public partial class AcceptanceReturnData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_NO { get; set; }
            public string OrderNo { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<AcceptanceReturnData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<AcceptanceReturnData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(FileUploadData.MetaData))]
    public partial class FileUploadData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROW_NUM { get; set; }
            public string UPLOAD_TYPE { get; set; }
            public string UPLOAD_NAME { get; set; }
            public string PURCHASE_SEQ { get; set; }
            public string RT_NO { get; set; }
            public string RT_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public string UPLOAD_DESC { get; set; }
            public Decimal? UPLOADED_COUNT { get; set; }
            public Decimal? SELECTED_COUNT { get; set; }
            public string SELECTED_FILES { get; set; }
            public string ISREQUIRED { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<FileUploadData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<FileUploadData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1510Data.MetaData))]
    public partial class F1510Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean IsSelected { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ALLOCATION_DATE { get; set; }
            public string ALLOCATION_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 QTY { get; set; }
            public string SUG_LOC_CODE { get; set; }
            public string TAR_LOC_CODE { get; set; }
            public string BUNDLE_SERIALLOC { get; set; }
            public string CHECK_SERIALNO { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime VALID_DATE { get; set; }
            public string TAR_DC_CODE { get; set; }
            public string TAR_WAREHOUSE_ID { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1510Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1510Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1510BundleSerialLocData.MetaData))]
    public partial class F1510BundleSerialLocData : IDataErrorInfo
    {
        public class MetaData
        {
            public string ChangeStatus { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ALLOCATION_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOCATION_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ALLOCATION_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 QTY { get; set; }
            public string SERIAL_NO { get; set; }
            public string SUG_LOC_CODE { get; set; }
            public string TAR_LOC_CODE { get; set; }
            public string BUNDLE_SERIALLOC { get; set; }
            public string CHECK_SERIALNO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime VALID_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1510BundleSerialLocData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1510BundleSerialLocData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F2501ItemData.MetaData))]
    public partial class F2501ItemData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string STATUS { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F2501ItemData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F2501ItemData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1510ItemLocData.MetaData))]
    public partial class F1510ItemLocData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string ChangeStatus { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ALLOCATION_DATE { get; set; }
            public string ALLOCATION_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string SUG_LOC_CODE { get; set; }
            public string TAR_LOC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ORGINAL_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(0, Int32.MaxValue, ErrorMessage = "上架數必須介於{1}~{2}之間")]
            public Int32 QTY { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1510ItemLocData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1510ItemLocData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1510QueryData.MetaData))]
    public partial class F1510QueryData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ALLOCATION_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOCATION_NO { get; set; }
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string SUG_LOC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TAR_LOC_CODE { get; set; }
            public string UPD_NAME { get; set; }
            public string BUNDLE_SERIALLOC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1510QueryData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1510QueryData>.Validate(this, columnName);
            }
        }
    }
    [MetadataType(typeof(F020302Data.MetaData))]
    public partial class F020302Data : IDataErrorInfo
    {
        public class MetaData
        {
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string PO_NO { get; set; }
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 SERIAL_LEN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime VALID_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020302Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F020302Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020109Data.MetaData))]
    public partial class F02020109Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string ChangeFlag { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string STOCK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 STOCK_SEQ { get; set; }
            public int? DEFECT_QTY { get; set; }
            public string SERIAL_NO { get; set; }
            public string UCC_CODE { get; set; }
            public string CAUSE { get; set; }
            public string OTHER_CAUSE { get; set; }
            public string WAREHOUSE_ID { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F02020109Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F02020109Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F0202Data.MetaData))]
    public partial class F0202Data : IDataErrorInfo
    {
        public class MetaData
        {
            public DateTime CRT_DATE { get; set; }
            public DateTime? CHECKIN_DATE { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ORDER_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public int STOCK_QTY { get; set; }
            public string CRT_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }
            public string STATUS { get; set; }
            public string FAST_PASS_TYPE { get; set; }
            public DateTime? DELIVER_DATE { get; set; }
            public DateTime? STOCK_DATE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F0202Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F0202Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(P020105100LogisticsGroup.MetaData))]
    public partial class P020105100LogisticsGroup : IDataErrorInfo
    {
        public class MetaData
        {
            public DateTime? RECV_DATE { get; set; }
            public String ALL_NAME { get; set; }
            public int RecordCount { get; set; }
            public int BOX_CNTSum { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<P020105100LogisticsGroup>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<P020105100LogisticsGroup>.Validate(this, columnName);
            }
        }

    }

	[MetadataType(typeof(ContainerDetailData.MetaData))]
	public partial class ContainerDetailData : IDataErrorInfo
	{
		public class MetaData
		{
			public string RT_NO { get; set; }
			
			public string BIN_CODE { get; set; }
			
			public string ITEM_CODE { get; set; }
		
			public string ITEM_NAME { get; set; }
		
			public int QTY { get; set; }

			public string STATUS { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<ContainerDetailData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<ContainerDetailData>.Validate(this, columnName);
			}
		}

	}

	[MetadataType(typeof(DefectDetailReport.MetaData))]
	public partial class DefectDetailReport : IDataErrorInfo
	{
		public class MetaData
		{
			public Int64 ID { get; set; }
			public string ITEM_CODE { get; set; }
			public string SERIAL_NO { get; set; }
			public string UCC_CODE_NAME { get; set; }
			public string CAUSE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<DefectDetailReport>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<DefectDetailReport>.Validate(this, columnName);
			}
		}

	}
}