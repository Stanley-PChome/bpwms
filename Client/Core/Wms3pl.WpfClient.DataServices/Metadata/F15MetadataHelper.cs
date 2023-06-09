using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F15DataService
{

	[MetadataType(typeof(F1510.MetaData))]
	public partial class F1510 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ALLOCATION_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 ALLOCATION_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SUG_LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 QTY { get; set; }
			public string SERIAL_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
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
			public string CHECK_SERIALNO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string SOURCE_TYPE { get; set; }
			public string SOURCE_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1510>.Validate(this));
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
				return InputValidator<F1510>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1511.MetaData))]
	public partial class F1511 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ORDER_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ORDER_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 B_PICK_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 A_PICK_QTY { get; set; }
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
				return string.Join<string>(",", InputValidator<F1511>.Validate(this));
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
				return InputValidator<F1511>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F151003.MetaData))]
	public partial class F151003 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 LACK_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 MOVE_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 LACK_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string REASON { get; set; }
			public string MEMO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
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
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F151003>.Validate(this));
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
				return InputValidator<F151003>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F151004.MetaData))]
	public partial class F151004 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string MOVE_BOX_NO { get; set; }
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
				return string.Join<string>(",", InputValidator<F151004>.Validate(this));
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
				return InputValidator<F151004>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F151001.MetaData))]
	public partial class F151001 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ALLOCATION_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_ALLOCATION_DATE { get; set; }
			public DateTime? POSTING_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string TAR_DC_CODE { get; set; }
			public string TAR_WAREHOUSE_ID { get; set; }
			public string SRC_WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SRC_DC_CODE { get; set; }
			public string SOURCE_TYPE { get; set; }
			public string SOURCE_NO { get; set; }
			public string BOX_NO { get; set; }
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
			public string SEND_CAR { get; set; }
			public string SRC_MOVE_STAFF { get; set; }
			public string SRC_MOVE_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOCK_STATUS { get; set; }
			public string TAR_MOVE_STAFF { get; set; }
			public string TAR_MOVE_NAME { get; set; }
			public string ISEXPENDDATE { get; set; }
			public string MOVE_TOOL { get; set; }
			public string ISMOVE_ORDER { get; set; }
			public DateTime? SRC_START_DATE { get; set; }
			public DateTime? TAR_START_DATE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F151001>.Validate(this));
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
				return InputValidator<F151001>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F151002.MetaData))]
	public partial class F151002 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALLOCATION_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 ALLOCATION_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ALLOCATION_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SRC_LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SUG_LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string TAR_LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SRC_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 A_SRC_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 TAR_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 A_TAR_QTY { get; set; }
			public string SERIAL_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CHECK_SERIALNO { get; set; }
			public string SRC_STAFF { get; set; }
			public DateTime? SRC_DATE { get; set; }
			public string SRC_NAME { get; set; }
			public string TAR_STAFF { get; set; }
			public DateTime? TAR_DATE { get; set; }
			public string TAR_NAME { get; set; }
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
			public DateTime? SRC_VALID_DATE { get; set; }
			public DateTime? TAR_VALID_DATE { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F151002>.Validate(this));
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
				return InputValidator<F151002>.Validate(this, columnName);
			}
		}
	}
}
