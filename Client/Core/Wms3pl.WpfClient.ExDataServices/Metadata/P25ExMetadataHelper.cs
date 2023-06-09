using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P25ExDataService
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
				return InputValidator<ExecuteResult>.Validate(this, columnName);
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
				return InputValidator<F2501ItemData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F250102Item.MetaData))]
	public partial class F250102Item : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int64 LOG_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime FREEZE_BEGIN_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime FREEZE_END_DATE { get; set; }
			public string SERIAL_NO_BEGIN { get; set; }
			public string SERIAL_NO_END { get; set; }
			public string BOX_SERIAL { get; set; }
			public string BATCH_NO { get; set; }
			public string CONTROL { get; set; }
			public string CAUSE { get; set; }
			public string MEMO { get; set; }
			public string CONTROL_DESC { get; set; }
			public string CAUSE_DESC { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean ISSELECTED { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F250102Item>.Validate(this));
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
				return InputValidator<F250102Item>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F2501QueryData.MetaData))]
	public partial class F2501QueryData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			public string SERIAL_NO { get; set; }
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SPEC { get; set; }
			public string STATUS { get; set; }
			public string STATUS_NAME { get; set; }
			public string ITEM_TYPE { get; set; }
			public string BOX_SERIAL { get; set; }
			public string TAG3G { get; set; }
			public string PUK { get; set; }
			public DateTime? VALID_DATE { get; set; }
			public string CASE_NO { get; set; }
			public string PO_NO { get; set; }
			public string WMS_NO { get; set; }
			public DateTime? IN_DATE { get; set; }
			public string ORD_PROP_NAME { get; set; }
			public string RETAIL_CODE { get; set; }
			public string ACTIVATED { get; set; }
			public string PROCESS_NO { get; set; }
			public Int16? COMBIN_NO { get; set; }
			public string CELL_NUM { get; set; }
			public string VNR_NAME { get; set; }
			public string SYS_NAME { get; set; }
			public string CAMERA_NO { get; set; }
			public string CLIENT_IP { get; set; }
			public string ITEM_UNIT { get; set; }
			public string SEND_CUST { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F2501QueryData>.Validate(this));
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
				return InputValidator<F2501QueryData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F2501WcfData.MetaData))]
	public partial class F2501WcfData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SERIAL_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			public string BOX_SERIAL { get; set; }
			public string TAG3G { get; set; }
			public string CELL_NUM { get; set; }
			public string PUK { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STATUS { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string CRT_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string BATCH_NO { get; set; }
			public DateTime? VALID_DATE { get; set; }
			public string PO_NO { get; set; }
			public string ACTIVATED { get; set; }
			public string SEND_CUST { get; set; }
			public string WMS_NO { get; set; }
      [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
      public string VNR_CODE { get; set; }
			public string SYS_VNR { get; set; }
			public string PROCESS_NO { get; set; }
			public string ORD_PROP { get; set; }
			public string CASE_NO { get; set; }
			public DateTime? IN_DATE { get; set; }
			public string RETAIL_CODE { get; set; }
			public Int32? COMBIN_NO { get; set; }
			public string CAMERA_NO { get; set; }
			public string CLIENT_IP { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSnTieStore { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsChangeItemCode { get; set; }
			public string LocCode { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F2501WcfData>.Validate(this));
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
				return InputValidator<F2501WcfData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F250103Verification.MetaData))]
	public partial class F250103Verification : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			public string SerialNo { get; set; }
			public string Status { get; set; }
			public string Verification { get; set; }
			public string Message { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F250103Verification>.Validate(this));
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
				return InputValidator<F250103Verification>.Validate(this, columnName);
			}
		}
	}
}
