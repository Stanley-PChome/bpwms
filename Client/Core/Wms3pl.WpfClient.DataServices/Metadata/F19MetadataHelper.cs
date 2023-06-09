using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
namespace Wms3pl.WpfClient.DataServices.F19DataService
{

    [MetadataType(typeof(F1910.MetaData))]
    public partial class F1910 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RETAIL_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RETAIL_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CONTACT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TEL { get; set; }
            public string MAIL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ADDRESS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1910>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1910>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1951.MetaData))]
    public partial class F1951 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UCC_CODE { get; set; }
            public string CAUSE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UCT_ID { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1951>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1951>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1952_HISTORY.MetaData))]
    public partial class F1952_HISTORY : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string PASSWORD { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1952_HISTORY>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1952_HISTORY>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1901.MetaData))]
    public partial class F1901 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_NAME { get; set; }
            public string TEL { get; set; }
            public string FAX { get; set; }
            public string ADDRESS { get; set; }
            public Int16? LAND_AREA { get; set; }
            public Int16? BUILD_AREA { get; set; }
            public string SHORT_CODE { get; set; }
            public string BOSS { get; set; }
            public string MAIL_BOX { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ZIP_CODE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1901>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1901>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190101.MetaData))]
    public partial class F190101 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190101>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190101>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190206.MetaData))]
    public partial class F190206 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190206>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190206>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190207.MetaData))]
    public partial class F190207 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 IMAGE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IMAGE_PATH { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190207>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190207>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F191901.MetaData))]
    public partial class F191901 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ATYPE_CODE { get; set; }
            public string ATYPE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F191901>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F191901>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F192401.MetaData))]
    public partial class F192401 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal GRP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F192401>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F192401>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F192401_IMPORTLOG.MetaData))]
    public partial class F192401_IMPORTLOG : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal LOG_ID { get; set; }
            public string GRP_NAME { get; set; }
            public string EMP_ID { get; set; }
            public string ERRMSG { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F192401_IMPORTLOG>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F192401_IMPORTLOG>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F192402.MetaData))]
    public partial class F192402 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F192402>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F192402>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F192403.MetaData))]
    public partial class F192403 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal WORK_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F192403>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F192403>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1929.MetaData))]
    public partial class F1929 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SYS_GUP_CODE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1929>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1929>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1942.MetaData))]
    public partial class F1942 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_TYPE_NAME { get; set; }
            public Int32? LENGTH { get; set; }
            public Int32? DEPTH { get; set; }
            public Int32? HEIGHT { get; set; }
            public Decimal? WEIGHT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public Decimal? VOLUME_RATE { get; set; }
            public string HANDY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1942>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1942>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1943.MetaData))]
    public partial class F1943 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_STATUS_ID { get; set; }
            public string LOC_STATUS_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1943>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1943>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1952.MetaData))]
    public partial class F1952 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            public string PASSWORD { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? LAST_ACTIVITY_DATE { get; set; }
            public DateTime? LAST_LOGIN_DATE { get; set; }
            public DateTime? LAST_PASSWORD_CHANGED_DATE { get; set; }
            public DateTime? LAST_LOCKOUT_DATE { get; set; }
            public Decimal? FAILED_PASSWORD_ATTEMPT_COUNT { get; set; }
            public Decimal? STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1952>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1952>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1953.MetaData))]
    public partial class F1953 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal GRP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GRP_NAME { get; set; }
            public string GRP_DESC { get; set; }
            public string SHOWINFO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string UPD_STAFF { get; set; }
            public string ISDELETED { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1953>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1953>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F195301.MetaData))]
    public partial class F195301 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string FUN_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal GRP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F195301>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F195301>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F195301_IMPORTLOG.MetaData))]
    public partial class F195301_IMPORTLOG : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal LOG_ID { get; set; }
            public string GRP_NAME { get; set; }
            public string FUN_CODE { get; set; }
            public string SHOWINFO { get; set; }
            public string ERRMSG { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F195301_IMPORTLOG>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F195301_IMPORTLOG>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1963.MetaData))]
    public partial class F1963 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal WORK_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WORK_NAME { get; set; }
            public string WORK_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string UPD_STAFF { get; set; }
            public string ISDELETED { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1963>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1963>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F196301.MetaData))]
    public partial class F196301 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal WORK_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F196301>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F196301>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F198001.MetaData))]
    public partial class F198001 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TYPE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F198001>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F198001>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1981.MetaData))]
    public partial class F1981 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PIER_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 TEMP_AREA { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOW_IN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOW_OUT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1981>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1981>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1983.MetaData))]
    public partial class F1983 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1983>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1983>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1919.MetaData))]
    public partial class F1919 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string AREA_CODE { get; set; }
            public string AREA_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ATYPE_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1919>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1919>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1911.MetaData))]
    public partial class F1911 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            public Int32? INVO_QTY { get; set; }
            public Int32? SZONE_QTY { get; set; }
            public Int32? CONT_QTY { get; set; }
            public Int32? ADJ_QTY { get; set; }
            public Int32? DISTR_QTY { get; set; }
            public Int32? ORD_QTY { get; set; }
            public Int32? WAY_QTY { get; set; }
            public Int32? RESV_QTY { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1911>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1911>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F191206.MetaData))]
    public partial class F191206 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            /// <summary>
            /// 物流中心編號
            /// </summary>
            public string DC_CODE { get; set; }

            /// <summary>
            /// 揀貨樓層
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_FLOOR { get; set; }

            /// <summary>
            /// 路線類型0: 魚骨型(頭接頭)、1: S型(尾接尾) F000904 topic = F191206, subtopic = PICK_TYPE
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TYPE { get; set; }

            /// <summary>
            /// PK區編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [RegularExpression(@"^[A-Z|a-z|\d]+$", ErrorMessage ="僅能輸入英數字")]
            public string PK_AREA { get; set; }

            /// <summary>
            /// PK區名稱
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PK_NAME { get; set; }

            /// <summary>
            /// PK區順序
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int PK_LINE_SEQ { get; set; }

            /// <summary>
            /// 是否啟用(0: 否、 1:是)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISENABLED { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F191206>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F191206>.Validate(this, columnName);
            }
        }

    }

    [MetadataType(typeof(F19120601.MetaData))]
    public partial class F19120601 : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 物流中心編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// PK區編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PK_AREA { get; set; }

            /// <summary>
            /// 路線順序
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int LINE_SEQ { get; set; }

            /// <summary>
            /// 路線頭碼(儲位前5碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BEGIN_LOC_CODE { get; set; }

            /// <summary>
            /// 路線尾碼(儲位前5碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string END_LOC_CODE { get; set; }


            /// <summary>
            /// 水平動線(0: 依序進行 1: 單側先行) F000904 topic =F19120601, subtopic = MOVING_HORIZON
            /// </summary>
            public string MOVING_HORIZON { get; set; }
            /// <summary>
            /// 垂直動線 (0: 由小到大 1:由大到小) F000904 topic=F19120601, subtopic=MOVING_VERTICAL
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MOVING_VERTICAL { get; set; }

            /// <summary>
            /// 處理狀態 (0: 建立 9: 刪除)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PROC_FLAG { get; set; }
            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19120601>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19120601>.Validate(this, columnName);
            }
        }

    }

    [MetadataType(typeof(F191202.MetaData))]
    public partial class F191202 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TRANS_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime TRANS_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TRANS_STATUS { get; set; }
            public string TRANS_WAY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TRANS_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_CODE { get; set; }
            public string FLOOR { get; set; }
            public string AREA_CODE { get; set; }
            public string CHANNEL { get; set; }
            public string PLAIN { get; set; }
            public string LOC_LEVEL { get; set; }
            public string LOC_TYPE { get; set; }
            public string LOC_CHAR { get; set; }
            public string LOC_TYPE_ID { get; set; }
            public string NOW_STATUS_ID { get; set; }
            public string PRE_STATUS_ID { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string DC_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UCC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string NOW_GUP_CODE { get; set; }
            public string NOW_CUST_CODE { get; set; }
            public Decimal? HOR_DISTANCE { get; set; }
            public DateTime? RENT_BEGIN_DATE { get; set; }
            public DateTime? RENT_END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F191202>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F191202>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1938.MetaData))]
    public partial class F1938 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TAX_CODE { get; set; }
            public string TAX_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1938>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1938>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1932.MetaData))]
    public partial class F1932 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_GUP_CODE { get; set; }
            public string ITEM_GUP_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1932>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1932>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1939.MetaData))]
    public partial class F1939 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_CODE { get; set; }
            public string PICK_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1939>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1939>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1941.MetaData))]
    public partial class F1941 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_TYPE_CODE { get; set; }
            public string ORD_TYPE_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1941>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1941>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1912.MetaData))]
    public partial class F1912 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_CODE { get; set; }
            public string FLOOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string AREA_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHANNEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PLAIN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_LEVEL { get; set; }
            public string LOC_TYPE { get; set; }
            public string LOC_CHAR { get; set; }
            public string LOC_TYPE_ID { get; set; }
            public string NOW_STATUS_ID { get; set; }
            public string PRE_STATUS_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UCC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public Decimal? HOR_DISTANCE { get; set; }
            public DateTime? RENT_BEGIN_DATE { get; set; }
            public DateTime? RENT_END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1912>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1912>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1933.MetaData))]
    public partial class F1933 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COUDIV_ID { get; set; }
            public string COUDIV_NAME { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1933>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1933>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1934.MetaData))]
    public partial class F1934 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ZIP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ZIP_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COUDIV_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1934>.Validate(this));
            }
        }
        //[MetadataType(typeof(F1934.MetaData))]
        //public partial class F1934 : IDataErrorInfo
        //{
        //    public class MetaData
        //    {
        //        [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
        //        public string TOWDIV_ID { get; set; }
        //        public string TOWDIV_NAME { get; set; }
        //        public string COUDIV_ID { get; set; }
        //        public string CRT_STAFF { get; set; }
        //        public DateTime? CRT_DATE { get; set; }
        //        public string UPD_STAFF { get; set; }
        //        public DateTime? UPD_DATE { get; set; }

        //    }

        //    [DoNotSerialize]
        //    public string Error
        //    {
        //        get
        //        {
        //            return string.Join<string>(",", InputValidator<F1934>.Validate(this));
        //        }
        //    }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1934>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1980.MetaData))]
    public partial class F1980 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_TYPE { get; set; }
            public string TMPR_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAL_STOCK { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAL_FEE { get; set; }
            public string LOC_TYPE_ID { get; set; }
            public Decimal? HOR_DISTANCE { get; set; }
            public DateTime? RENT_BEGIN_DATE { get; set; }
            public DateTime? RENT_END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1980>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1980>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190002.MetaData))]
    public partial class F190002 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TICKET_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WAREHOUSE_ID { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190002>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190002>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1947.MetaData))]
    public partial class F1947 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_COMP { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PIER_CODE { get; set; }
            public string CAN_IN { get; set; }
            public string CAN_OUT { get; set; }
            public string CAN_FAST { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1947>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1947>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194701.MetaData))]
    public partial class F194701 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TIME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TMPR { get; set; }
            public string CAN_ALLREGION { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_FREQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_EFFIC { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194701>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194701>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1954.MetaData))]
    public partial class F1954 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string FUN_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string FUN_NAME { get; set; }
            public string FUN_TYPE { get; set; }
            public string FUN_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            public DateTime? UPLOAD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DISABLE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1954>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1954>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1913.MetaData))]
    public partial class F1913 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            public Int32? QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime VALID_DATE { get; set; }
            public DateTime? ENTER_DATE { get; set; }
            public string MAKE_NO { get; set; }
            public string REMARK { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string SERIAL_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1913>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1913>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1950.MetaData))]
    public partial class F1950 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UCT_ID { get; set; }
            public string TYPE_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1950>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1950>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1924.MetaData))]
    public partial class F1924 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_NAME { get; set; }
            public string EMAIL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string ISDELETED { get; set; }
            public string PACKAGE_UNLOCK { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISCOMMON { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DEP_ID { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1924>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1924>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1925.MetaData))]
    public partial class F1925 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DEP_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DEP_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1925>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1925>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1915.MetaData))]
    public partial class F1915 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACODE { get; set; }
            public Int16? CHECK_PERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1915>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1915>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1916.MetaData))]
    public partial class F1916 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BCODE { get; set; }
            public Int16? CHECK_PERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1916>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1916>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1917.MetaData))]
    public partial class F1917 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BCODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CCODE { get; set; }
            public Int16? CHECK_PERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1917>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1917>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F19000101.MetaData))]
    public partial class F19000101 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal MILESTONE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TICKET_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MILESTONE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SORT_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19000101>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19000101>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F19000102.MetaData))]
    public partial class F19000102 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MILESTONE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MILESTONE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19000102>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19000102>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F19000103.MetaData))]
    public partial class F19000103 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TICKET_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TICKET_CLASS { get; set; }
            public string MILESTONE_NO_A { get; set; }
            public string MILESTONE_NO_B { get; set; }
            public string MILESTONE_NO_C { get; set; }
            public string MILESTONE_NO_D { get; set; }
            public string MILESTONE_NO_E { get; set; }
            public string MILESTONE_NO_F { get; set; }
            public string MILESTONE_NO_G { get; set; }
            public string MILESTONE_NO_H { get; set; }
            public string MILESTONE_NO_I { get; set; }
            public string MILESTONE_NO_J { get; set; }
            public string MILESTONE_NO_K { get; set; }
            public string MILESTONE_NO_L { get; set; }
            public string MILESTONE_NO_M { get; set; }
            public string MILESTONE_NO_N { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19000103>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19000103>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190001.MetaData))]
    public partial class F190001 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal TICKET_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TICKET_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TICKET_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TICKET_CLASS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHIPPING_ASSIGN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string FAST_DELIVER { get; set; }
            public string ASSIGN_DELIVER { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string OUT_TYPE { get; set; }
            public Int16? PRIORITY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190001>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190001>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1928.MetaData))]
    public partial class F1928 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string OUTSOURCE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string OUTSOURCE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CONTACT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UNI_FORM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string BOSS { get; set; }
            public string ITEM_CONTACT { get; set; }
            public string ITEM_TEL { get; set; }
            public string ITEM_CEL { get; set; }
            public string ITEM_MAIL { get; set; }
            public string BILL_TEL { get; set; }
            public string BILL_CEL { get; set; }
            public string BILL_MAIL { get; set; }
            public string ADDRESS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CURRENCY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_FACTOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_TYPE { get; set; }
            public string BANK_ACCOUNT { get; set; }
            public string BANK_CODE { get; set; }
            public string BANK_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            public string ZIP { get; set; }
            public string INV_ZIP { get; set; }
            public string INV_ADDRESS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TAX_TYPE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1928>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1928>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1909.MetaData))]
    public partial class F1909 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHORT_NAME { get; set; }
            public string BOSS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CONTACT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ADDRESS { get; set; }
            public string UNI_FORM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CONTACT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TEL { get; set; }
            public string ITEM_CEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_MAIL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BILL_CONTACT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BILL_TEL { get; set; }
            public string BILL_CEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BILL_MAIL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CURRENCY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_FACTOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_TYPE { get; set; }
            public string BANK_CODE { get; set; }
            public string BANK_NAME { get; set; }
            public string BANK_ACCOUNT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORDER_ADDRESS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MIX_LOC_BATCH { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MIX_LOC_ITEM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_TRANSFER { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BOUNDLE_SERIALLOC { get; set; }
            public string RTN_DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string OVERAGE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SAM_ITEM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INSIDER_TRADING { get; set; }
            public Int32? INSIDER_TRADING_LIM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SPILT_ORDER { get; set; }
            public Int32? SPILT_ORDER_LIM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string B2C_CAN_LACK { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAN_FAST { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INSTEAD_INVO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SPILT_INCHECK { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SPECIAL_IN { get; set; }
            [Range(typeof(Decimal), "0.00000000001", "100", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? CHECK_PERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string NEED_SEAL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RIBBON { get; set; }
            public DateTime? RIBBON_BEGIN_DATE { get; set; }
            public DateTime? RIBBON_END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_BOX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SP_BOX { get; set; }
            public string SP_BOX_CODE { get; set; }
            public DateTime? SPBOX_BEGIN_DATE { get; set; }
            public DateTime? SPBOX_END_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string INVO_ZIP { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TAX_TYPE { get; set; }
            public string INVO_ADDRESS { get; set; }
            public Decimal? DUE_DAY { get; set; }
            public Int32? INVO_LIM_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string AUTO_GEN_RTN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SYS_CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUPSHARE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISPRINTDELV { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1909>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1909>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190902.MetaData))]
    public partial class F190902 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 DM_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime BEGIN_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime END_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190902>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190902>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190901.MetaData))]
    public partial class F190901 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_YEAR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_MON { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_TITLE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_NO_BEGIN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_NO_END { get; set; }
            public string INVO_NO { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190901>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190901>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1903.MetaData))]
    public partial class F1903 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_NAME { get; set; }
            public Int32? MCASE_QTY { get; set; }
            public string EAN_CODE1 { get; set; }
            public string EAN_CODE2 { get; set; }
            public string EAN_CODE3 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LTYPE { get; set; }
            public string MTYPE { get; set; }
            public string STYPE { get; set; }
            public string ITEM_ENGNAME { get; set; }
            public string ITEM_COLOR { get; set; }
            public string ITEM_SIZE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TYPE { get; set; }
            public Int16? ITEM_HUMIDITY { get; set; }
            public string ITEM_NICKNAME { get; set; }
            public string ITEM_ATTR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string ITEM_SPEC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TMPR_TYPE { get; set; }
            public string FRAGILE { get; set; }
            public string SPILL { get; set; }
            public string ITEM_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_UNIT { get; set; }
            public string ITEM_CLASS { get; set; }
            public string SIM_SPEC { get; set; }
            public string MEMO { get; set; }
            public string VIRTUAL_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_TYPE { get; set; }
            public Int32? VEN_ORD { get; set; }
            public string RET_UNIT { get; set; }
            public Int32? RET_ORD { get; set; }
            public Int16? ALL_DLN { get; set; }
            public string SND_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_WARE { get; set; }
            public string C_D_FLAG { get; set; }
            public string CUST_ITEM_CODE { get; set; }
            public Int16? ALLOW_ALL_DLN { get; set; }
            public string MULTI_FLAG { get; set; }
            public string MIX_BATCHNO { get; set; }
            public string ALLOWORDITEM { get; set; }
            public string BUNDLE_SERIALLOC { get; set; }
            public string BUNDLE_SERIALNO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ORD_SAVE_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 PICK_SAVE_QTY { get; set; }
            public string ITEM_EXCHANGE { get; set; }
            public string ITEM_RETURN { get; set; }
            public string ITEM_MERGE { get; set; }
            public Int16? BORROW_DAY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_MIX_ITEM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string NO_PRICE { get; set; }
            public Int32? EP_TAX { get; set; }
            public Int16? SERIALNO_DIGIT { get; set; }
            public string SERIAL_BEGIN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_RULE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAN_SELL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAN_SPILT_IN { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LG { get; set; }
            public Int16? SAVE_DAY { get; set; }
            public string ITEM_STAFF { get; set; }
            public Decimal? CHECK_PERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 PICK_SAVE_ORD { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 DELV_QTY_AVG { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISCARTON { get; set; }
            public string ISAPPLE { get; set; }
            public string NEED_EXPIRED { get; set; }
            public Int32? ALL_SHP { get; set; }
            public string EAN_CODE4 { get; set; }
            public DateTime? FIRST_IN_DATE { get; set; }
            public string VNR_CODE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1903>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1903>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1905.MetaData))]
    public partial class F1905 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            public Int32? ITEM_LENGTH { get; set; }
            public Int32? ITEM_WIDTH { get; set; }
            public Int32? ITEM_HIGHT { get; set; }
            public Int32? PACK_LENGTH { get; set; }
            public Int32? PACK_WIDTH { get; set; }
            public Int32? PACK_HIGHT { get; set; }
            public Int32? CASE_LENGTH { get; set; }
            public Int32? CASE_WIDTH { get; set; }
            public Int32? CASE_HIGHT { get; set; }
            public Decimal? ITEM_WEIGHT { get; set; }
            public Decimal? PACK_WEIGHT { get; set; }
            public Decimal? CASE_WEIGHT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
						[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
						public string CUST_CODE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1905>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1905>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1908.MetaData))]
    public partial class F1908 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UNI_FORM { get; set; }
            public string BOSS { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TEL { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ZIP { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ADDRESS { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CONTACT { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TEL { get; set; }
            public string ITEM_CEL { get; set; }
            public string ITEM_MAIL { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BILL_CONTACT { get; set; }
            //[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BILL_TEL { get; set; }
            public string BILL_CEL { get; set; }
            public string BILL_MAIL { get; set; }
            public string INV_ZIP { get; set; }
            public string INV_ADDRESS { get; set; }
            public string TAX_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CURRENCY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_FACTOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PAY_TYPE { get; set; }
            public string BANK_CODE { get; set; }
            public string BANK_NAME { get; set; }
            public string BANK_ACCOUNT { get; set; }
            public Int16? LEADTIME { get; set; }
            public string ORD_DATE { get; set; }
            public Int16? ORD_CIRCLE { get; set; }
            public string DELV_TIME { get; set; }
            public Int16? VNR_LIM_QTY { get; set; }
            public Int16? ORD_STOCK_QTY { get; set; }
            public string DELI_TYPE { get; set; }
            public string UPD_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string INVO_TYPE { get; set; }
            [Range(typeof(Decimal), "0.00000000001", "100", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? CHECKPERCENT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_TYPE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1908>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1908>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1970.MetaData))]
    public partial class F1970 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LABEL_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LABEL_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LABEL_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WARRANTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WARRANTY_Y { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WARRANTY_M { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string WARRANTY_D { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string OUTSOURCE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_DESC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1970>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1970>.Validate(this, columnName);
            }
        }
    }
    [MetadataType(typeof(F197001.MetaData))]
    public partial class F197001 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 LABEL_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LABEL_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            public string VNR_CODE { get; set; }
            public string WARRANTY { get; set; }
            public Int16? WARRANTY_S_Y { get; set; }
            public string WARRANTY_S_M { get; set; }
            public string WARRANTY_Y { get; set; }
            public string WARRANTY_M { get; set; }
            [Range(typeof(Int16), "1", "31", ErrorMessage = "日期期間為{1}~{2}")]
            public Int16? WARRANTY_D { get; set; }
            public string OUTSOURCE { get; set; }
            public string CHECK_STAFF { get; set; }
            public string ITEM_DESC_A { get; set; }
            public string ITEM_DESC_B { get; set; }
            public string ITEM_DESC_C { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F197001>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F197001>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F192404.MetaData))]
    public partial class F192404 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CLIENT_IP { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VIDEO_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F192404>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F192404>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190003.MetaData))]
    public partial class F190003 : IDataErrorInfo
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
            public string WORK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal? GRP_ID_1 { get; set; }
            public Decimal? GRP_ID_7 { get; set; }
            public Decimal? GRP_ID_14 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_MAIL_1 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_MAIL_7 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_MAIL_14 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_SMS_1 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_SMS_7 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_SMS_14 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190003>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190003>.Validate(this, columnName);
            }
        }
    }


    [MetadataType(typeof(F194702.MetaData))]
    public partial class F194702 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal CAR_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CAR_KIND_NAME { get; set; }
            public string CAR_SIZE { get; set; }
            public string TMPR_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194702>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194702>.Validate(this, columnName);
            }
        }
    }


    [MetadataType(typeof(F199002.MetaData))]
    public partial class F199002 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(1, 32767, ErrorMessage = "超出範圍{1}~{2}")]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal BASIC_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199002>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199002>.Validate(this, columnName);
            }
        }
    }



    [MetadataType(typeof(F199006.MetaData))]
    public partial class F199006 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(1, 32767, ErrorMessage = "超出範圍{1}~{2}")]
            public short ACC_NUM { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public decimal FEE { get; set; }

            public string CRT_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            public System.DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public Nullable<System.DateTime> UPD_DATE { get; set; }
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199006>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199006>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F199003.MetaData))]
    public partial class F199003 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(1, 32767, ErrorMessage = "超出範圍{1}~{2}")]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal BASIC_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199003>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199003>.Validate(this, columnName);
            }
        }
    }


    [MetadataType(typeof(F199005.MetaData))]
    public partial class F199005 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOGI_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IS_SPECIAL_CAR { get; set; }
            public Decimal? CAR_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ACC_AREA_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TMPR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_EFFIC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "1", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal FEE { get; set; }
            [Range(typeof(Decimal), "1", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal? OVER_VALUE { get; set; }
            [Range(typeof(Decimal), "1", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
            public Decimal? OVER_UNIT_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199005>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199005>.Validate(this, columnName);
            }
        }
    }


    [MetadataType(typeof(F199007.MetaData))]
    public partial class F199007 : IDataErrorInfo
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
            public string ACC_PROJECT_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_PROJECT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Int32), "0", "999999999", ErrorMessage = "超出範圍{1}~{2}")]
            public Int32 FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERVICES { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199007>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199007>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194708.MetaData))]
    public partial class F194708 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ACC_AREA_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_AREA { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194708>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194708>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194707.MetaData))]
    public partial class F194707 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(1, int.MaxValue)]
            public Decimal ACC_AREA_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_EFFIC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TMPR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOGI_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(1, int.MaxValue)]
            public Decimal ACC_DELVNUM_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0.01", "9999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal BASIC_VALUE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0.01", "9999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal MAX_WEIGHT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal FEE { get; set; }
            [Range(typeof(Decimal), "0.01", "9999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? OVER_VALUE { get; set; }
            [Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? OVER_UNIT_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194707>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194707>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F199001.MetaData))]
    public partial class F199001 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TMPR_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal UNIT_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F199001>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F199001>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194703.MetaData))]
    public partial class F194703 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "1", "99999999", ErrorMessage = "值必須大於0")]
            public Decimal CAR_KIND_ID { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_OUT { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            [Range(typeof(Decimal), "1", "999999.99", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal FEE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            public string UPD_STAFF { get; set; }

            public string UPD_NAME { get; set; }

            public DateTime? UPD_DATE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194703>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194703>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194705.MetaData))]
    public partial class F194705 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROUTE_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ROUTE_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TIMES { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ROUTE { get; set; }

            public string ADDRESS_A { get; set; }
            public string ADDRESS_B { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            public string PASSWAY { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            public string UPD_STAFF { get; set; }

            public string UPD_NAME { get; set; }

            public DateTime? UPD_DATE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194705>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194705>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F190205.MetaData))]
    public partial class F190205 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190205>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190205>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194713.MetaData))]
    public partial class F194713 : IDataErrorInfo
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
            public string ALL_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ESERVICE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ESERVICE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ESHOP { get; set; }
            public string ESHOP_ID { get; set; }
            public Int16? SHOP_DELV_DAY { get; set; }
            public Int16? SHOP_RETURN_DAY { get; set; }
            public string PLATFORM_NAME { get; set; }
            public string VNR_NAME { get; set; }
            public string CUST_INFO { get; set; }
            public string NOTE1 { get; set; }
            public string NOTE2 { get; set; }
            public string NOTE3 { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194713>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194713>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194716.MetaData))]
    public partial class F194716 : IDataErrorInfo
    {
        public class MetaData
        {
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_NO { get; set; }
            public string CAR_PERIOD { get; set; }
            public string CAR_GUP { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DRIVER_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DRIVER_NAME { get; set; }
            public decimal EXTRA_FEE { get; set; }
            public string CRT_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public decimal REGION_FEE { get; set; }
            public decimal OIL_FEE { get; set; }
            public decimal OVERTIME_FEE { get; set; }
            public string PACK_FIELD { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194716>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194716>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F1945.MetaData))]
    public partial class F1945 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COLLECTION_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COLLECTION_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string COLLECTION_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CELL_START_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CELL_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int CELL_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F1945>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F1945>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F194501.MetaData))]
    public partial class F194501 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CELL_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CELL_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int LENGTH { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int DEPTH { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int HEIGHT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public decimal VOLUME_RATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 MAX_VOLUME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F194501>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F194501>.Validate(this, columnName);
            }
        }
    }
}



namespace Wms3pl.Datas.Schedule
{
    [MetadataType(typeof(PREFERENCE.MetaData))]
    public partial class PREFERENCE : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string EMP_ID { get; set; }
            public byte[] DATA { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<PREFERENCE>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<PREFERENCE>.Validate(this, columnName);
            }
        }
    }
}
