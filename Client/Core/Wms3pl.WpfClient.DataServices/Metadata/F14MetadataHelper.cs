using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F14DataService
{

	[MetadataType(typeof(F140102.MetaData))]
	public partial class F140102 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			public string CHANNEL_BEGIN { get; set; }
			public string CHANNEL_END { get; set; }
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
				return string.Join<string>(",", InputValidator<F140102>.Validate(this));
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
				return InputValidator<F140102>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140103.MetaData))]
	public partial class F140103 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
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
				return string.Join<string>(",", InputValidator<F140103>.Validate(this));
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
				return InputValidator<F140103>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140101.MetaData))]
	public partial class F140101 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime INVENTORY_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ISCHARGE { get; set; }
			[Range(typeof(Decimal),"0.00","9999999.99",ErrorMessage = "必須介於{1}~{2}之間")]
			public Decimal? FEE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_TYPE { get; set; }
			[Range(typeof(Int16), "0", "99", ErrorMessage = "必須介於{1}~{2}之間")]
			public Int16? INVENTORY_CYCLE { get; set; }
			public Int16? INVENTORY_YEAR { get; set; }
			public Int16? INVENTORY_MON { get; set; }
			[Range(typeof(Int16), "0", "999", ErrorMessage = "必須介於{1}~{2}之間")]
			public Int16? CYCLE_TIMES { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SHOW_CNT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int64 ITEM_CNT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int64 ITEM_QTY { get; set; }
			public string MEMO { get; set; }
			public DateTime? PRINT_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ISSECOND { get; set; }
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
			public string CHECK_TOOL { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F140101>.Validate(this));
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
				return InputValidator<F140101>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140104.MetaData))]
	public partial class F140104 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENTER_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SERIAL_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 QTY { get; set; }
			public Int32? FIRST_QTY { get; set; }
			public Int32? SECOND_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string FLUSHBACK { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string FST_INVENTORY_STAFF { get; set; }
			public string FST_INVENTORY_NAME { get; set; }
			public DateTime? FST_INVENTORY_DATE { get; set; }
			public string FST_INVENTORY_PC { get; set; }
			public string SEC_INVENTORY_STAFF { get; set; }
			public string SEC_INVENTORY_NAME { get; set; }
			public DateTime? SEC_INVENTORY_DATE { get; set; }
			public string SEC_INVENTORY_PC { get; set; }
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
				return string.Join<string>(",", InputValidator<F140104>.Validate(this));
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
				return InputValidator<F140104>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140105.MetaData))]
	public partial class F140105 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENTER_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SERIAL_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32? FIRST_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32? SECOND_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string FLUSHBACK { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string FST_INVENTORY_STAFF { get; set; }
			public string FST_INVENTORY_NAME { get; set; }
			public DateTime? FST_INVENTORY_DATE { get; set; }
			public string FST_INVENTORY_PC { get; set; }
			public string SEC_INVENTORY_STAFF { get; set; }
			public string SEC_INVENTORY_NAME { get; set; }
			public DateTime? SEC_INVENTORY_DATE { get; set; }
			public string SEC_INVENTORY_PC { get; set; }
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
				return string.Join<string>(",", InputValidator<F140105>.Validate(this));
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
				return InputValidator<F140105>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140111.MetaData))]
	public partial class F140111 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			public string AREA_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE_PART { get; set; }
			public string MASK_CODE { get; set; }
			public string IMPORT_WAY { get; set; }
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
				return string.Join<string>(",", InputValidator<F140111>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F140111>.Validate(this, columnName);
			}
		}
	}
}
