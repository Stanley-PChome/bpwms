using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
namespace Wms3pl.WpfClient.DataServices.F25DataService
{

	[MetadataType(typeof(F2501.MetaData))]
	public partial class F2501 : IDataErrorInfo
	{
		public class MetaData
		{
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string BATCH_NO { get; set; }
			public DateTime? VALID_DATE { get; set; }
			public string PO_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ACTIVATED { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
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
			public Int16? COMBIN_NO { get; set; }
			public string CAMERA_NO { get; set; }
			public string CLIENT_IP { get; set; }
			public string BOUNDLE_ITEM_CODE { get; set; }
			public string IS_ASYNC { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F2501>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F2501>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F250102.MetaData))]
	public partial class F250102 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int64 LOG_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime FREEZE_BEGIN_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime FREEZE_END_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string FREEZE_TYPE { get; set; }
			public string SERIAL_NO_BEGIN { get; set; }
			public string SERIAL_NO_END { get; set; }
			public string BOX_SERIAL { get; set; }
			public string BATCH_NO { get; set; }
			public string CONTROL { get; set; }
			public string CAUSE { get; set; }
			public string MEMO { get; set; }			
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
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F250102>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F250102>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F250101.MetaData))]
	public partial class F250101 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int64 LOG_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime LOG_DATE { get; set; }
			public string SERIAL_NO { get; set; }
			public string ITEM_CODE { get; set; }
			public string BOX_SERIAL { get; set; }
			public string TAG3G { get; set; }
			public string CELL_NUM { get; set; }
			public string PUK { get; set; }
			public string STATUS { get; set; }
			public string BATCH_NO { get; set; }
			public DateTime? VALID_DATE { get; set; }
			public string PO_NO { get; set; }
			public string ACTIVATED { get; set; }
			public string SEND_CUST { get; set; }
			public string VNR_CODE { get; set; }
			public string SYS_VNR { get; set; }
			public string WMS_NO { get; set; }
			public string PROCESS_NO { get; set; }
			public string ORD_PROP { get; set; }
			public string CASE_NO { get; set; }
			public DateTime? IN_DATE { get; set; }
			public string RETAIL_CODE { get; set; }
			public Int16? COMBIN_NO { get; set; }
			public string CAMERA_NO { get; set; }
			public string CLIENT_IP { get; set; }			
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
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
				return string.Join<string>(",", InputValidator<F250101>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F250101>.Validate(this, columnName);
			}
		}
	}
}
