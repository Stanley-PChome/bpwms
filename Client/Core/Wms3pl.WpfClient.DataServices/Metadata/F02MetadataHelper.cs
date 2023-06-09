using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F02DataService
{

    [MetadataType(typeof(F020103.MetaData))]
    public partial class F020103 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ARRIVE_DATE { get; set; }

            public string ARRIVE_TIME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PIER_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_CODE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
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

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            public DateTime? UPD_DATE { get; set; }

            public string UPD_STAFF { get; set; }

            public Int32? ITEM_QTY { get; set; }

            public Int32? ORDER_QTY { get; set; }

            public Decimal? ORDER_VOLUME { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 SERIAL_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020103>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F020103>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F020104.MetaData))]
    public partial class F020104 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime BEGIN_DATE { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime END_DATE { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PIER_CODE { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 TEMP_AREA { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOW_IN { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOW_OUT { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }//

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }//

            public DateTime? UPD_DATE { get; set; }

            public string UPD_STAFF { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }//

            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020104>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F020104>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F0202.MetaData))]
    public partial class F0202 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORDER_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            public string VNR_CODE { get; set; }
            public DateTime? CHECKIN_DATE { get; set; }
            public Decimal? ORDER_WEIGHT { get; set; }
            public Int32? BOX_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
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
            public Int16? PALLET_QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F0202>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F0202>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020101.MetaData))]
    public partial class F02020101 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_SEQ { get; set; }

            public string VNR_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            public DateTime? RECE_DATE { get; set; }
            public DateTime? VALI_DATE { get; set; }
            public DateTime? MADE_DATE { get; set; }
            public Int32? ORDER_QTY { get; set; }
            public Int32? RECV_QTY { get; set; }
            public Int32? CHECK_QTY { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_ITEM { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_SERIAL { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISPRINT { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISUPLOAD { get; set; }

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
            public string ISSPECIAL { get; set; }

            public string SPECIAL_DESC { get; set; }

            public string SPECIAL_CODE { get; set; }

            public string RT_NO { get; set; }

            public string RT_SEQ { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F02020101>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020101>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020102.MetaData))]
    public partial class F02020102 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISPASS { get; set; }
            public string UCC_CODE { get; set; }
            public string MEMO { get; set; }
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
			public string RT_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F02020102>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020102>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020104.MetaData))]
    public partial class F02020104 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SERIAL_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISPASS { get; set; }
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
			public string RT_NO { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F02020104>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020104>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020105.MetaData))]
    public partial class F02020105 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UPLOAD_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 UPLOAD_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UPLOAD_S_PATH { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UPLOAD_C_PATH { get; set; }
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

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F02020105>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020105>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F020201.MetaData))]
    public partial class F020201 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_SEQ { get; set; }
            public string VNR_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            public DateTime? RECE_DATE { get; set; }
            public DateTime? VALI_DATE { get; set; }
            public DateTime? MADE_DATE { get; set; }
            public Int32? ORDER_QTY { get; set; }
            public Int32? RECV_QTY { get; set; }
            public Int32? CHECK_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_ITEM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_SERIAL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISPRINT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISUPLOAD { get; set; }
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
            public string SPECIAL_DESC { get; set; }
            public string SPECIAL_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ISSPECIAL { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020201>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F020201>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020103.MetaData))]
    public partial class F02020103 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CUR_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 CUR_VAL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
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
                return string.Join<string>(",", InputValidator<F02020103>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020103>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F02020106.MetaData))]
    public partial class F02020106 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UPLOAD_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string UPLOAD_NAME { get; set; }
            public string ISREQUIRED { get; set; }
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
                return string.Join<string>(",", InputValidator<F02020106>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //  case "VNR_NAME":
                //    if (string.IsNullOrWhiteSpace(VNR_NAME))
                //    {
                //      return "不可為空白";
                //    }
                //    break;

                //  default:
                //    throw new ArgumentException("Unknown property: " + columnName, "columnName");
                //}

                //return "";
                return InputValidator<F02020106>.Validate(this, columnName);
            }
        }
    }
}
