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
namespace Wms3pl.WpfClient.DataServices.F91DataService
{

	[MetadataType(typeof(F915102.MetaData))]
	public partial class F915102 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_DESC { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string VNR_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal UNITPRICE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNIT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F915102>.Validate(this));
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
				return InputValidator<F915102>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910301.MetaData))]
	public partial class F910301 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CONTRACT_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DISABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string OBJECT_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNI_FORM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			public string MEMO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
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
				return string.Join<string>(",", InputValidator<F910301>.Validate(this));
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
				return InputValidator<F910301>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910302.MetaData))]
	public partial class F910302 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CONTRACT_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 CONTRACT_SEQ { get; set; }
			public string SUB_CONTRACT_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CONTRACT_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DISABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_TYPE { get; set; }
			public string QUOTE_NO { get; set; }
			public string UNIT_ID { get; set; }
			public Decimal? TASK_PRICE { get; set; }
			public Int32? WORK_HOUR { get; set; }
			public string OUTSOURCE_ID { get; set; }
			public string PROCESS_ID { get; set; }
			public Decimal? OUTSOURCE_COST { get; set; }
			public Decimal? APPROVE_PRICE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
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
				return string.Join<string>(",", InputValidator<F910302>.Validate(this));
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
				return InputValidator<F910302>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910401.MetaData))]
	public partial class F910401 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DISABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			//[Range(typeof(decimal), "0.00", "100", ErrorMessage = "毛利率的範圍必須為{1}~{2}%")]
			public Decimal NET_RATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[Range(typeof(decimal),"0.00", "1000000000", ErrorMessage="成本價的範圍必須為{1}~{2}")]
			public Decimal COST_PRICE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "貨主加工申請價的範圍必須為{1}~{2}")]
			public Decimal APPLY_PRICE { get; set; }
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "貨主加工核定價的範圍必須為{1}~{2}")]
			public Decimal? APPROVED_PRICE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string OUTSOURCE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910401>.Validate(this));
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
				return InputValidator<F910401>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910402.MetaData))]
	public partial class F910402 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNIT_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 WORK_HOUR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "動作金額的範圍必須為{1}~{2}")]
			public Decimal WORK_COST { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910402>.Validate(this));
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
				return InputValidator<F910402>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910403.MetaData))]
	public partial class F910403 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			public string UNIT_ID { get; set; }
			[Range(typeof(decimal), "0", "10000000", ErrorMessage = "耗用標準的範圍必須為{1}~{2}")]
			public Int32? STANDARD { get; set; }
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "標準費用的範圍必須為{1}~{2}")]
			public Decimal? STANDARD_COST { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910403>.Validate(this));
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
				return InputValidator<F910403>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910001.MetaData))]
	public partial class F910001 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_ACT { get; set; }
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
				return string.Join<string>(",", InputValidator<F910001>.Validate(this));
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
				return InputValidator<F910001>.Validate(this, columnName);
			}
		}
	}

	
	[MetadataType(typeof(F910404.MetaData))]
	public partial class F910404 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
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
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
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
				return string.Join<string>(",", InputValidator<F910404>.Validate(this));
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
				return InputValidator<F910404>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910003.MetaData))]
	public partial class F910003 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_TYPE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_TYPE { get; set; }
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
				return string.Join<string>(",", InputValidator<F910003>.Validate(this));
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
				return InputValidator<F910003>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910101.MetaData))]
	public partial class F910101 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOM_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNIT_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal CHECK_PERCENT { get; set; }
			public string SPEC_DESC { get; set; }
			public string PACKAGE_DESC { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
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
				return string.Join<string>(",", InputValidator<F910101>.Validate(this));
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
				return InputValidator<F910101>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910102.MetaData))]
	public partial class F910102 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string MATERIAL_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 COMBIN_ORDER { get; set; }
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
				return string.Join<string>(",", InputValidator<F910102>.Validate(this));
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
				return InputValidator<F910102>.Validate(this, columnName);
			}
		}
	}

  [MetadataType(typeof(F910201.MetaData))]
  public partial class F910201 : IDataErrorInfo
  {
    public class MetaData
    {
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string PROCESS_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string PROCESS_SOURCE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string OUTSOURCE_ID { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public DateTime FINISH_DATE { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string ITEM_CODE { get; set; }
      public string ITEM_CODE_BOM { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 PROCESS_QTY { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 A_PROCESS_QTY { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 BREAK_QTY { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 BOX_QTY { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public Int32 CASE_QTY { get; set; }
      public string ORDER_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string STATUS { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string PROC_STATUS { get; set; }
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
      public string QUOTE_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string FINISH_TIME { get; set; }

    }

    [DoNotSerialize]
    public string Error
    {
      get
      {
        return string.Join<string>(",", InputValidator<F910201>.Validate(this));
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
        return InputValidator<F910201>.Validate(this, columnName);
      }
    }
  }

	[MetadataType(typeof(F910004.MetaData))]
	public partial class F910004 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PRODUCE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PRODUCE_NAME { get; set; }
			public string PRODUCE_DESC { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PRODUCE_IP { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
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
				return string.Join<string>(",", InputValidator<F910004>.Validate(this));
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
				return InputValidator<F910004>.Validate(this, columnName);
			}
		}

	}
}
