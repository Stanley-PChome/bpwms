using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
using Wms3pl.WpfClient.DataServices.Interface;

namespace Wms3pl.WpfClient.DataServices.F50DataService
{

    [MetadataType(typeof(F500101.MetaData))]
    public partial class F500101 : IDataErrorInfo, IF500101To5
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Range(0.0, 100, ErrorMessage = "毛利率的範圍必須為{1}~{2}")]
            public Decimal NET_RATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string LOC_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string TMPR_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal UNIT_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_UNIT_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
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
                return string.Join<string>(",", InputValidator<F500101>.Validate(this));
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
                return InputValidator<F500101>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F500102.MetaData))]
    public partial class F500102 : IDataErrorInfo, IF500101To5
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Range(0.0, 100, ErrorMessage = "毛利率的範圍必須為{1}~{2}")]
            public Decimal NET_RATE { get; set; }
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
            public Decimal? ACC_AREA_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_TMPR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_EFFIC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ACC_NUM { get; set; }
            public Decimal? MAX_WEIGHT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal FEE { get; set; }
            public Decimal? OVER_VALUE { get; set; }
            public Decimal? OVER_UNIT_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
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
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_OVER_UNIT_FEE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F500102>.Validate(this));
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
                return InputValidator<F500102>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F500103.MetaData))]
    public partial class F500103 : IDataErrorInfo, IF500101To5
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Range(0.0, 100, ErrorMessage = "毛利率的範圍必須為{1}~{2}")]
            public Decimal NET_RATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal BASIC_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_BASIC_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
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
                return string.Join<string>(",", InputValidator<F500103>.Validate(this));
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
                return InputValidator<F500103>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F500104.MetaData))]
    public partial class F500104 : IDataErrorInfo, IF500101To5
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Range(0.0, 100, ErrorMessage = "毛利率的範圍必須為{1}~{2}")]
            public Decimal NET_RATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_KIND_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_KIND { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal BASIC_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_BASIC_FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_OVER_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
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
                return string.Join<string>(",", InputValidator<F500104>.Validate(this));
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
                return InputValidator<F500104>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F500105.MetaData))]
    public partial class F500105 : IDataErrorInfo, IF500101To5
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string QUOTE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime ENABLE_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DISABLE_DATE { get; set; }
            [Range(0.0, 100, ErrorMessage = "毛利率的範圍必須為{1}~{2}")]
            public Decimal NET_RATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_ITEM_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 ACC_NUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string IN_TAX { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DELV_ACC_TYPE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal FEE { get; set; }
            [Range(typeof(Decimal), "0", "9999999", ErrorMessage = "值必須介於{1}與{2}之間")]
            public Decimal? APPROV_FEE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
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
                return string.Join<string>(",", InputValidator<F500105>.Validate(this));
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
                return InputValidator<F500105>.Validate(this, columnName);
            }
        }
    }
}
