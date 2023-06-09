using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
namespace Wms3pl.WpfClient.ExDataServices.P91ExDataService
{
	[MetadataType(typeof(F910403Data.MetaData))]
	public partial class F910403Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNIT_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32? STANDARD { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Double? STANDARD_COST { get; set; }
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
			public string ITEM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CLA_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910403Data>.Validate(this));
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
				return InputValidator<F910403Data>.Validate(this, columnName);
			}
		}

	}

	[MetadataType(typeof(F910101Ex.MetaData))]
	public partial class F910101Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOM_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOM_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string UNIT_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public decimal CHECK_PERCENT { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910101Ex>.Validate(this));
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
				return InputValidator<F910101Ex>.Validate(this, columnName);
			}
		}
		[MetadataType(typeof(F910302WithF1928.MetaData))]
		public partial class F910302WithF1928 : IDataErrorInfo
		{
			public class MetaData
			{
				public string CONTRACT_TYPE { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string PROCESS_ID { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string OUTSOURCE_ID { get; set; }
				public Decimal? APPROVE_PRICE { get; set; }
				public Decimal? TASK_PRICE { get; set; }
				public string OUTSOURCE_NAME { get; set; }

			}

			[DoNotSerialize]
			public string Error
			{
				get
				{
					return string.Join<string>(",", InputValidator<F910302WithF1928>.Validate(this));
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
					return InputValidator<F910302WithF1928>.Validate(this, columnName);
				}
			}
		}

		[MetadataType(typeof(F910301Data.MetaData))]
		public partial class F910301Data : IDataErrorInfo
		{
			public class MetaData
			{
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string CONTRACT_NO { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public DateTime? ENABLE_DATE { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public DateTime? DISABLE_DATE { get; set; }
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
				public string CRT_STAFF { get; set; }
				public DateTime? CRT_DATE { get; set; }
				public string UPD_STAFF { get; set; }
				public DateTime? UPD_DATE { get; set; }
				public string CRT_NAME { get; set; }
				public string UPD_NAME { get; set; }
				public Decimal? CYCLE_DATE { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string CLOSE_CYCLE { get; set; }
				public string OBJECT_NAME { get; set; }
				public string CONTACT { get; set; }
				public string TEL { get; set; }
				public Decimal? DUEDATE { get; set; }

			}


			[DoNotSerialize]
			public string Error
			{
				get
				{
					return string.Join<string>(",", InputValidator<F910301Data>.Validate(this));
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
					return InputValidator<F910301Data>.Validate(this, columnName);
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
				public Int32? STANDARD { get; set; }
				public Decimal? STANDARD_COST { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string DC_CODE { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string GUP_CODE { get; set; }
				public string CRT_STAFF { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public DateTime CRT_DATE { get; set; }
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

		[MetadataType(typeof(F1903WithF1915.MetaData))]
		public partial class F1903WithF1915 : IDataErrorInfo
		{
			public class MetaData
			{
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string GUP_CODE { get; set; }
				[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
				public string ITEM_CODE { get; set; }
				public string ITEM_NAME { get; set; }
				public string CLA_NAME { get; set; }

			}

			[DoNotSerialize]
			public string Error
			{
				get
				{
					return string.Join<string>(",", InputValidator<F1903WithF1915>.Validate(this));
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
					return InputValidator<F1903WithF1915>.Validate(this, columnName);
				}
			}
		}
	}

	[MetadataType(typeof(F910102Ex.MetaData))]
	public partial class F910102Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			public string ITEM_CODE { get; set; }
			public string MATERIAL_CODE { get; set; }
			public string MATERIAL_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 COMBIN_ORDER { get; set; }
			public string ITEM_SIZE { get; set; }
			public string CARTON_SIZE { get; set; }
			public string ITEM_COLOR { get; set; }
			public string BUNDLE_SERIALNO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 BOM_QTY { get; set; }
			public string CUST_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CRT_STAFF { get; set; }
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
				return string.Join<string>(",", InputValidator<F910102Ex>.Validate(this));
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
				return InputValidator<F910102Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910302WithF1928.MetaData))]
	public partial class F910302WithF1928 : IDataErrorInfo
	{
		public class MetaData
		{
			public string CONTRACT_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string OUTSOURCE_ID { get; set; }
			public Decimal? APPROVE_PRICE { get; set; }
			public Decimal? TASK_PRICE { get; set; }
			public string OUTSOURCE_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910302WithF1928>.Validate(this));
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
				return InputValidator<F910302WithF1928>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910301Data.MetaData))]
	public partial class F910301Data : IDataErrorInfo
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
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
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string OBJECT_NAME { get; set; }
			public string CONTACT { get; set; }
			public string TEL { get; set; }
			public Decimal? DUEDATE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910301Data>.Validate(this));
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
				return InputValidator<F910301Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F910302Data.MetaData))]
	public partial class F910302Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CONTRACT_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 CONTRACT_SEQ { get; set; }
			public string SUB_CONTRACT_NO { get; set; }
			public string CONTRACT_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENABLE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DISABLE_DATE { get; set; }
			public string ITEM_TYPE { get; set; }
			public string QUOTE_NO { get; set; }
			public string UNIT_ID { get; set; }
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "作業單價的範圍必須為{1}~{2}")]
			public Decimal? TASK_PRICE { get; set; }
			[Range(typeof(int), "0", "1000000000", ErrorMessage = "標準工時(秒)的範圍必須為{1}~{2}")]
			public Int32? WORK_HOUR { get; set; }
			public string PROCESS_ID { get; set; }
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "委外商成本價的範圍必須為{1}~{2}")]
			public Decimal? OUTSOURCE_COST { get; set; }
			[Range(typeof(decimal), "0.00", "1000000000", ErrorMessage = "貨主核定價的範圍必須為{1}~{2}")]
			public Decimal? APPROVE_PRICE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string CONTRACT_TYPE1 { get; set; }
			public string CONTRACT_TYPENAME { get; set; }
			public string QUOTE_NAME { get; set; }
			public string ITEM_TYPE_NAME { get; set; }
			public string UNIT { get; set; }
			public string PROCESS_ACT { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F910302Data>.Validate(this));
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
				return InputValidator<F910302Data>.Validate(this, columnName);
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
			public Int32? STANDARD { get; set; }
			public Decimal? STANDARD_COST { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
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

	[MetadataType(typeof(F1903WithF1915.MetaData))]
	public partial class F1903WithF1915 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string CLA_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1903WithF1915>.Validate(this));
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
				return InputValidator<F1903WithF1915>.Validate(this, columnName);
			}
		}
	}
	[MetadataType(typeof(P910102Data.MetaData))]
	public partial class P910102Data : IDataErrorInfo
	{
		public class MetaData
		{
			public decimal ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }

			public string PROCESS_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_SOURCE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PROCESS_ITEM { get; set; }

			public string PROCESS_ITEM_NAME { get; set; }

			public decimal? PACK_QTY { get; set; }

			public string ORDER_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string QUOTE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }

			public string ITEM_UNIT { get; set; }

			public string ITEM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public decimal PROCESS_QTY { get; set; }

			public string GOOD_CODE { get; set; }

			public string GOOD_UNIT { get; set; }

			public string GOOD_NAME { get; set; }

			public decimal GOOD_QTY { get; set; }

			public string MEMO { get; set; }

			public string STATUS { get; set; }

			public string STATUS_NAME { get; set; }

			public string PROC_STATUS { get; set; }

			public string PROC_STATUS_NAME { get; set; }

			public DateTime? PROC_BEGIN_TIME { get; set; }

			public DateTime? PROC_END_TIME { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<P910102Data>.Validate(this));
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
				return InputValidator<P910102Data>.Validate(this, columnName);
			}
		}
	}
}

