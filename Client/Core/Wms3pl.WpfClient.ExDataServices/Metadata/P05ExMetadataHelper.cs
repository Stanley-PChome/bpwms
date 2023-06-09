using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
namespace Wms3pl.WpfClient.ExDataServices.P05ExDataService
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

    [MetadataType(typeof(F051201Data.MetaData))]
    public partial class F051201Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ORDCOUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal PICKCOUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ITEMCOUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TOTALPICK_QTY { get; set; }
            public string ISPRINTED { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string PICK_TYPE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051201Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051201Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051202Data.MetaData))]
    public partial class F051202Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_NAME { get; set; }
            public string TMPR_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string FLOOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ITEMCOUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TOTALPICK_QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051202Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051202Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051201SelectedData.MetaData))]
    public partial class F051201SelectedData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean IsSelected { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051201SelectedData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051201SelectedData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051201ReportDataA.MetaData))]
    public partial class F051201ReportDataA : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string TMPR_TYPE_NAME { get; set; }
            public string FLOOR { get; set; }
            public string PICK_LOC { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string VALID_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 B_PICK_QTY { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051201ReportDataA>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051201ReportDataA>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051201ReportDataB.MetaData))]
    public partial class F051201ReportDataB : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string WMS_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 B_DELV_QTY { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051201ReportDataB>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051201ReportDataB>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F0513WithF1909.MetaData))]
    public partial class F0513WithF1909 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            public string DEFAULT_PIER_CODE { get; set; }
            public string PIER_CODE { get; set; }
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F0513WithF1909>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F0513WithF1909>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F0513WithF050801Batch.MetaData))]
    public partial class F0513WithF050801Batch : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            public string ALL_ID { get; set; }
            public string CAR_NO_A { get; set; }
            public string CAR_NO_B { get; set; }
            public string CAR_NO_C { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 WMS_ORD_COUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 AUDIT_COUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 DEBIT_COUNT { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F0513WithF050801Batch>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F0513WithF050801Batch>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050801WithF055001.MetaData))]
    public partial class F050801WithF055001 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WMS_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string PAST_NO { get; set; }
            public string CUST_ORD_NO { get; set; }
            public Decimal? STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ISMERGE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050801WithF055001>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050801WithF055001>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051206PickList.MetaData))]
    public partial class F051206PickList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            public DateTime? DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051206PickList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051206PickList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051206AllocationList.MetaData))]
    public partial class F051206AllocationList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string TAR_DC_CODE { get; set; }
            public string ALLOCATION_NO { get; set; }
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051206AllocationList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051206AllocationList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051206LackList.MetaData))]
    public partial class F051206LackList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public Int32? IsUpdate { get; set; }
            public string ITEM_Name { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 LACK_SEQ { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string PICK_ORD_NO { get; set; }
            public string PICK_ORD_SEQ { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public Int32? ORD_QTY { get; set; }
            public Int32? LACK_QTY { get; set; }
            public string REASON { get; set; }
            public string MEMO { get; set; }
            public string STATUS { get; set; }
            public string RETURN_FLAG { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string ISDELETED { get; set; }
            public string ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean ISSELECTED { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051206LackList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051206LackList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F0010List.MetaData))]
    public partial class F0010List : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 HELP_NO { get; set; }
            public string ORD_NO { get; set; }
            public string HELP_TYPE { get; set; }
            public string CRT_STAFF { get; set; }
            public string FLOOR { get; set; }
            public string LOC_CODE { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F0010List>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F0010List>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050801StatisticsData.MetaData))]
    public partial class F050801StatisticsData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            public Decimal? UNFINISHEDCOUNT { get; set; }
            public Decimal? FINISHEDCOUNT { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050801StatisticsData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050801StatisticsData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(PickingStatistics.MetaData))]
    public partial class PickingStatistics : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_AREA { get; set; }
            public Decimal? FINISHEDCOUNT { get; set; }
            public Decimal? UNFINISHEDCOUNT { get; set; }
            public Decimal? TOTALCOUNT { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<PickingStatistics>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<PickingStatistics>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050101Ex.MetaData))]
    public partial class F050101Ex : IDataErrorInfo
    {
        public class MetaData
        {
            public string WMS_ORD_NO { get; set; }
            public string ORD_NO { get; set; }
            public string CUST_ORD_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_TYPE { get; set; }

            public string RETAIL_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ORD_DATE { get; set; }

            public string STATUS { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(100, ErrorMessage = "長度限制{1}碼")]
            public string CUST_NAME { get; set; }

            public string SELF_TAKE { get; set; }
            public string FRAGILE_LABEL { get; set; }
            public string GUARANTEE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SA { get; set; }

            public string GENDER { get; set; }
            public Int16? AGE { get; set; }
            public Int16? SA_QTY { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(15, ErrorMessage = "長度限制{1}碼")]
            public string TEL { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(60, ErrorMessage = "長度限制{1}碼")]
            public string ADDRESS { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(25, ErrorMessage = "長度限制{1}碼")]
            public string CONSIGNEE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ARRIVAL_DATE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TRAN_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SP_DELV { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(10, ErrorMessage = "長度限制{1}碼")]
            public string CUST_COST { get; set; }

            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BATCH_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHANNEL { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string POSM { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(25, ErrorMessage = "長度限制{1}碼")]
            public string CONTACT { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(15, ErrorMessage = "長度限制{1}碼")]
            public string CONTACT_TEL { get; set; }

            public string TEL_2 { get; set; }
            public string SPECIAL_BUS { get; set; }
            public string ALL_ID { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COLLECT { get; set; }

            public Decimal? COLLECT_AMT { get; set; }
            public string MEMO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TYPE_ID { get; set; }

            public string CAN_FAST { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(20, ErrorMessage = "長度限制{1}碼")]
            public string TEL_1 { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [MaxLength(5, ErrorMessage = "長度限制{1}碼")]
            public string TEL_AREA { get; set; }

            public string PRINT_RECEIPT { get; set; }
            public string RECEIPT_NO { get; set; }
            public string RECEIPT_NO_HELP { get; set; }
            public string RECEIPT_TITLE { get; set; }
            public string RECEIPT_ADDRESS { get; set; }
            public string BUSINESS_NO { get; set; }
            public string DISTR_CAR_NO { get; set; }
            public string EDI_FLAG { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SUBCHANNEL { get; set; }
			public string ESERVICE { get; set; }
			public string ROUND_PIECE { get; set; }
			public string FAST_DEAL_TYPE { get; set; }
			public string SUG_BOX_NO { get; set; }
			public string MOVE_OUT_TARGET { get; set; }
			public string PACKING_TYPE { get; set; }
			public int ISPACKCHECK { get; set; }
		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050101Ex>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050101Ex>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050102Ex.MetaData))]
    public partial class F050102Ex : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_NO { get; set; }
            public string ORD_SEQ { get; set; }
            public string SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ORD_QTY { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            public string CRT_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Boolean ISSELECTED { get; set; }
            public string BUNDLE_SERIALLOC { get; set; }
            public string NO_DELV { get; set; }
			public string MAKE_NO { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050102Ex>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050102Ex>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050102WithF050801.MetaData))]
    public partial class F050102WithF050801 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string ORD_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public Int32? B_DELV_QTY { get; set; }
            public Int32? A_DELV_QTY { get; set; }
            public Int32? LACK_QTY { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            public string WMS_ORD_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050102WithF050801>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050102WithF050801>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(P05030201BasicData.MetaData))]
    public partial class P05030201BasicData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string DC_CODE { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ORD_NO { get; set; }
            public DateTime? ARRIVAL_DATE { get; set; }
            public string RETAIL_CODE { get; set; }
            public string CUST_NAME { get; set; }
            public DateTime? ORD_DATE { get; set; }
            public string STATUS { get; set; }
            public DateTime? APPROVE_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string PICK_ORD_NO { get; set; }
            public string SOURCE_TYPE { get; set; }
            public string SOURCE_NO { get; set; }
            public string INCAR_DATE { get; set; }
            public string LACK_DO_STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<P05030201BasicData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<P05030201BasicData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050801NoShipOrders.MetaData))]
    public partial class F050801NoShipOrders : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string WMS_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string STATUS { get; set; }
            public Int16? PICK_STATUS { get; set; }
            public string PICK_STAFF { get; set; }
            public string PICK_NAME { get; set; }
            public string PICK_ORD_NO { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ORD_NO { get; set; }
            public DateTime? NEW_CHECKOUT_DATE { get; set; }
            public string NEW_CHECKOUT_TIME { get; set; }
            public string NEW_PIER_CODE { get; set; }
            public string PACKAGE_NAME { get; set; }
            public string ORD_TYPE { get; set; }
            public string ORD_TIME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050801NoShipOrders>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050801NoShipOrders>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050001Data.MetaData))]
    public partial class F050001Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_NO { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ORD_TYPE { get; set; }
            public DateTime? ORD_DATE { get; set; }
            public string STATUS { get; set; }
            public string CONSIGNEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ARRIVAL_DATE { get; set; }
            public string BATCH_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string DC_CODE { get; set; }
			public string MOVE_OUT_TARGET { get; set; }

		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050001Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050001Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F050802GroupItem.MetaData))]
    public partial class F050802GroupItem : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WMS_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            public Int32? SUM_B_SET_QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F050802GroupItem>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F050802GroupItem>.Validate(this, columnName);
            }
        }
    }

    // P單貼紙
    [MetadataType(typeof(RP0501010004Model.MetaData))]
    public partial class RP0501010004Model : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WMS_ORD_NO { get; set; }
			public string CORSS_NAME { get; set; }
			public string VNR_CODE { get; set; }
		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<RP0501010004Model>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<RP0501010004Model>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(P050103ReportData.MetaData))]
    public partial class P050103ReportData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public DateTime DELV_DATE { get; set; }
            public string PICK_ORD_NO { get; set; }
            public string PICK_ORD_NO_BARCODE { get; set; }
            public Decimal PICK_QTY_SUM { get; set; }
            public string SERIAL_NO { get; set; }
            public string PICK_LOC { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string TMPR_TYPE { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string AREA_NAME { get; set; }
            public string ACC_UNIT_NAME { get; set; }
            public string PICK_LOC_BARCODE { get; set; }
            public string ITEM_CODE_BARCODE { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_NAME { get; set; }


        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<P050103ReportData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<P050103ReportData>.Validate(this, columnName);
            }
        }
    }

    // O單貼紙
    [MetadataType(typeof(RP0501010005Model.MetaData))]
    public partial class RP0501010005Model : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WMS_ORD_NO { get; set; }


        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<RP0501010005Model>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<RP0501010005Model>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F051202WithF055002.MetaData))]
    public partial class F051202WithF055002 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_OUT_SERIAL_NO_READ { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F051202WithF055002>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F051202WithF055002>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(BatchPickNoList.MetaData))]
    public partial class BatchPickNoList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            public string ORD_TYPE { get; set; }
            public string CUST_COST { get; set; }
            public string FAST_DEAL_TYPE { get; set; }
            public string SOURCE_TYPE { get; set; }
            public int ATFL_N_PICK_CNT { get; set; }
            public int ATFL_B_PICK_CNT { get; set; }
            public int ATFL_NP_PICK_CNT { get; set; }
            public int ATFL_BP_PICK_CNT { get; set; }
            public int PDA_PICK_PERCENT { get; set; }
            public string CROSS_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<BatchPickNoList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<BatchPickNoList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(RePickNoList.MetaData))]
    public partial class RePickNoList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
            public string CUST_COST { get; set; }
            public string FAST_DEAL_TYPE { get; set; }
            public string SOURCE_TYPE { get; set; }
            public string PICK_TOOL { get; set; }
            public string COLLECTION_CODE { get; set; }
            public string CROSS_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<RePickNoList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<RePickNoList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(RePrintPickNoList.MetaData))]
    public partial class RePrintPickNoList : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WMS_ORD_NO { get; set; }
            public string PICK_TOOL { get; set; }
            public string CROSS_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<RePrintPickNoList>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<RePrintPickNoList>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(SinglePickingReportData.MetaData))]
    public partial class SinglePickingReportData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string TMPR_TYPE_NAME { get; set; }
            public string FLOOR { get; set; }
            public string PICK_LOC { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string VALID_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 B_PICK_QTY { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_NAME { get; set; }
            public string CUST_COST { get; set; }
            public string FAST_DEAL_TYPE { get; set; }
            public string SPLIT_CODE { get; set; }
            public string NEXT_STEP { get; set; }
            public string ORD_TYPE { get; set; }
            public string WMS_NO { get; set; }
            public string EAN_CODE2 { get; set; }
            public string EAN_CODE3 { get; set; }
			public string CROSS_NAME { get; set; }
		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<SinglePickingReportData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<SinglePickingReportData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(BatchPickingReportData.MetaData))]
    public partial class BatchPickingReportData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string PICK_ORD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELV_DATE { get; set; }
            public string PICK_TIME { get; set; }
            public string WMS_ORD_NO { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public string TMPR_TYPE_NAME { get; set; }
            public string FLOOR { get; set; }
            public string PICK_LOC { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string VALID_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 B_PICK_QTY { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_NAME { get; set; }
            public string CUST_COST { get; set; }
            public string FAST_DEAL_TYPE { get; set; }
            public string SPLIT_CODE { get; set; }
            public string NEXT_STEP { get; set; }
            public string ORD_TYPE { get; set; }
            public string WMS_NO { get; set; }
            public string EAN_CODE2 { get; set; }
            public string EAN_CODE3 { get; set; }
            public string CROSS_NAME { get; set; }
            public string CHANNEL { get; set; }
            public string NAME { get; set; }
            public string TAKE_DATE { get; set; }
            public string TAKE_TIME { get; set; }
            public string ALL_COMP { get; set; }
            public string AREA_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public string ACC_UNIT_NAME { get; set; }
            public string SERIAL_NO { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string ORDER_CUST_NAME { get; set; }
            public string MEMO { get; set; }
            public string EAN_CODE1 { get; set; }
            public string SHORT_NAME { get; set; }
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<BatchPickingReportData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<BatchPickingReportData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(SinglePickingTickerData.MetaData))]
    public partial class SinglePickingTickerData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
			public string PickOrdNoBarcode { get; set; }
			public string SPLIT_CODE { get; set; }
			public string CROSS_NAME { get; set; }
			public string VNR_CODE { get; set; }
		}

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<SinglePickingTickerData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<SinglePickingTickerData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(BatchPickingTickerData.MetaData))]
    public partial class BatchPickingTickerData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_ORD_NO { get; set; }
            public string PickOrdNoBarcode { get; set; }
            public string CROSS_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<BatchPickingTickerData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<BatchPickingTickerData>.Validate(this, columnName);
            }
        }
    }
}
