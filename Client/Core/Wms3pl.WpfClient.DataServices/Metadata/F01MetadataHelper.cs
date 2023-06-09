using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F01DataService
{

    [MetadataType(typeof(F010202.MetaData))]
    public partial class F010202 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STOCK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 STOCK_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 STOCK_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RECV_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
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
                return string.Join<string>(",", InputValidator<F010202>.Validate(this));
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
                return InputValidator<F010202>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010101.MetaData))]
    public partial class F010101 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHOP_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime SHOP_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELIVER_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime INVOICE_DATE { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string CONTACT_TEL { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHOP_CAUSE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            public string MEMO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
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
                return string.Join<string>(",", InputValidator<F010101>.Validate(this));
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
                return InputValidator<F010101>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010102.MetaData))]
    public partial class F010102 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHOP_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHOP_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 SHOP_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
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
                return string.Join<string>(",", InputValidator<F010102>.Validate(this));
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
                return InputValidator<F010102>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010201.MetaData))]
    public partial class F010201 : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STOCK_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime STOCK_DATE { get; set; }
            public DateTime? SHOP_DATE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime DELIVER_DATE { get; set; }
            public string SOURCE_TYPE { get; set; }
            public string SOURCE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ORD_PROP { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string VNR_CODE { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string CUST_COST { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string STATUS { get; set; }
            public string MEMO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
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
                return string.Join<string>(",", InputValidator<F010201>.Validate(this));
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
                return InputValidator<F010201>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010301.MetaData))]
    public partial class F010301 : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 流水號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 ID { get; set; }

            /// <summary>
            /// 物流中心
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// 物流商編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID;

            /// <summary>
            /// 收貨日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime? RECV_DATE { get; set; }

            /// <summary>
            /// 收貨時間
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RECV_TIME { get; set; }

            /// <summary>
            /// 收貨人員帳號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RECV_USER { get; set; }

            /// <summary>
            /// 收貨人員名稱
            /// </summary>
            public string RECV_NAME { get; set; }

            /// <summary>
            /// 貨運單號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHIP_ORD_NO { get; set; }

            /// <summary>
            /// 箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 BOX_CNT { get; set; }

            /// <summary>
            /// 簽單核對狀態(0:未核對;1:已核對)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public char CHECK_STATUS { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            public string MEMO { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F010301>.Validate(this));
            }
        }
        public string this[string columnName]
        {
            get
            {

                return InputValidator<F010301>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010301_HISTORY.MetaData))]
    public partial class F010301_HISTORY : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 流水號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 ID { get; set; }

            /// <summary>
            /// 物流中心
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// 物流商編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID;

            /// <summary>
            /// 收貨日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime? RECV_DATE { get; set; }

            /// <summary>
            /// 收貨時間
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RECV_TIME { get; set; }

            /// <summary>
            /// 收貨人員帳號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RECV_USER { get; set; }

            /// <summary>
            /// 收貨人員名稱
            /// </summary>
            public string RECV_NAME { get; set; }

            /// <summary>
            /// 貨運單號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHIP_ORD_NO { get; set; }

            /// <summary>
            /// 箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 BOX_CNT { get; set; }

            /// <summary>
            /// 簽單核對狀態(0:未核對;1:已核對)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public char CHECK_STATUS { get; set; }

            /// <summary>
            /// 備註
            /// </summary>
            public string MEMO { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F010301_HISTORY>.Validate(this));
            }
        }
        public string this[string columnName]
        {
            get
            {

                return InputValidator<F010301_HISTORY>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010302.MetaData))]
    public partial class F010302 : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 流水號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 ID { get; set; }

            /// <summary>
            /// 物流中心
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// 物流商編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID;

            /// <summary>
            /// 刷單核對日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime? CHECK_DATE { get; set; }

            /// <summary>
            /// 刷單核對時間
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_TIME { get; set; }

            /// <summary>
            /// 刷單人員帳號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_USER { get; set; }

            /// <summary>
            /// 刷單人員名稱
            /// </summary>
            public string CHECK_NAME { get; set; }

            /// <summary>
            /// 貨運單號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHIP_ORD_NO { get; set; }

            /// <summary>
            /// 核對箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 CHECK_BOX_CNT { get; set; }

            /// <summary>
            /// 貨單箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 SHIP_BOX_CNT { get; set; }

            /// <summary>
            /// 核對箱數結果 0:核對失敗;1:核對成功
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_STATUS { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F010302>.Validate(this));
            }
        }
        public string this[string columnName]
        {
            get
            {

                return InputValidator<F010302>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F010302_HISTORY.MetaData))]
    public partial class F010302_HISTORY : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 流水號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 ID { get; set; }

            /// <summary>
            /// 物流中心
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// 物流商編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID;

            /// <summary>
            /// 刷單核對日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime? CHECK_DATE { get; set; }

            /// <summary>
            /// 刷單核對時間
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_TIME { get; set; }

            /// <summary>
            /// 刷單人員帳號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_USER { get; set; }

            /// <summary>
            /// 刷單人員名稱
            /// </summary>
            public string CHECK_NAME { get; set; }

            /// <summary>
            /// 貨運單號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string SHIP_ORD_NO { get; set; }

            /// <summary>
            /// 核對箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 CHECK_BOX_CNT { get; set; }

            /// <summary>
            /// 貨單箱數
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int16 SHIP_BOX_CNT { get; set; }

            /// <summary>
            /// 核對箱數結果 0:核對失敗;1:核對成功
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHECK_STATUS { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F010302_HISTORY>.Validate(this));
            }
        }
        public string this[string columnName]
        {
            get
            {

                return InputValidator<F010302_HISTORY>.Validate(this, columnName);
            }
        }
    }
}
