using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P08ExDataService
{
	[MetadataType(typeof(HomeDeliveryOrderNumberData.MetaData))]
	public partial class HomeDeliveryOrderNumberData : IDataErrorInfo
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
			public string WMS_ORD_NO { get; set; }
			public string STATUS { get; set; }
			public string ALL_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 PACKAGE_BOX_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<HomeDeliveryOrderNumberData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<HomeDeliveryOrderNumberData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(BatchPickData.MetaData))]
	public partial class BatchPickData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PICK_ORD_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELV_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PICK_TIME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CROSS_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PICK_STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PICK_TOOL { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SPLIT_CODE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<BatchPickData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<BatchPickData>.Validate(this, columnName);
			}
		}
	}
}
