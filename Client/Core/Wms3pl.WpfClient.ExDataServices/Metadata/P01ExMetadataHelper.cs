using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P01ExDataService
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
				return InputValidator<ExecuteResult>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010201Data.MetaData))]
	public partial class F010201Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STOCK_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime STOCK_DATE { get; set; }
			public DateTime? SHOP_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELIVER_DATE { get; set; }
			public string SOURCE_TYPE { get; set; }
			public string SOURCE_NAME { get; set; }
			public string SOURCE_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string VNR_ADDRESS { get; set; }
			public string CUST_ORD_NO { get; set; }
			public string CUST_COST { get; set; }
			public string STATUS { get; set; }
			public string STATUSNAME { get; set; }
			public string MEMO { get; set; }
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ORD_PROP { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string FAST_PASS_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string BOOKING_IN_PERIOD { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010201Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010201Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010202Data.MetaData))]
	public partial class F010202Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			public string ChangeFlag { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string STOCK_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 STOCK_SEQ { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 STOCK_QTY { get; set; }
            public string EAN_CODE1 { get; set; }
            public string EAN_CODE2 { get; set; }
			public string CUST_ORD_NO { get; set; }
			public string VNR_NAME { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010202Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010202Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010101ShopNoList.MetaData))]
	public partial class F010101ShopNoList : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime SHOP_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SHOP_NO { get; set; }
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELIVER_DATE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010101ShopNoList>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010101ShopNoList>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010101Data.MetaData))]
	public partial class F010101Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime SHOP_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SHOP_NO { get; set; }
			public string CUST_ORD_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELIVER_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string TEL { get; set; }
			public string ADDRESS { get; set; }
			public string BUSPER { get; set; }
			public string PAY_TYPE { get; set; }
			public string INVO_TYPE { get; set; }
			public string TAX_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime INVOICE_DATE { get; set; }
			public string CONTACT_TEL { get; set; }
			public string INV_ADDRESS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SHOP_CAUSE { get; set; }
			public string MEMO { get; set; }
			public string STATUS { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public string CRT_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ORD_PROP { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010101Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010101Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010102Data.MetaData))]
	public partial class F010102Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SHOP_NO { get; set; }
			public string SHOP_SEQ { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SHOP_QTY { get; set; }
            public string SERIAL_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010102Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010102Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010101ReportData.MetaData))]
	public partial class F010101ReportData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime SHOP_DATE { get; set; }
			public string SHOP_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELIVER_DATE { get; set; }
			public string CUST_ORD_NO { get; set; }
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string TEL { get; set; }
			public string ADDRESS { get; set; }
			public string BUSPER { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime INVOICE_DATE { get; set; }
			public string CONTACT_TEL { get; set; }
			public string INV_ADDRESS { get; set; }
			public string SHOP_CAUSE { get; set; }
			public string MEMO { get; set; }
			public string PAY_TYPE { get; set; }
			public string TAX_TYPE { get; set; }
			public string INVO_TYPE { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SHOP_QTY { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010101ReportData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010101ReportData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010201QueryData.MetaData))]
	public partial class F010201QueryData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string STOCK_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010201QueryData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010201QueryData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F010201ImportData.MetaData))]
	public partial class F010201ImportData : IDataErrorInfo
	{
		public class MetaData
		{
			public string PO_NO { get; set; }
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string FAST_PASS_TYPE { get; set; }
			public string BOOKING_IN_PERIOD { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public int STOCK_QTY { get; set; }
			public DateTime? VALI_DATE { get; set; }
			public string MAKE_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F010201ImportData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F010201ImportData>.Validate(this, columnName);
			}
		}
	}
}
