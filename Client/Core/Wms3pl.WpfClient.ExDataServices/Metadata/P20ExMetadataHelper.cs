using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P20ExDataService
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

	[MetadataType(typeof(F200101Data.MetaData))]
	public partial class F200101Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			public string ADJUST_NO { get; set; }
			public string ADJUST_TYPE { get; set; }
			public string ADJUST_TYPE_NAME { get; set; }
			public string WORK_TYPE { get; set; }
			public string WORK_TYPE_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 IsCanEdit { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ADJUST_DATE { get; set; }
			public string CRT_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F200101Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F200101Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F200102Data.MetaData))]
	public partial class F200102Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			public string ADJUST_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ADJUST_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELV_DATE { get; set; }
			public string PICK_TIME { get; set; }
			public string CUST_ORD_NO { get; set; }
			public string ORG_PICK_TIME { get; set; }
			public string ALL_ID { get; set; }
			public string ALL_COMP { get; set; }
			public string ADDRESS { get; set; }
			public string NEW_DC_CODE { get; set; }
			public string NEW_DC_NAME { get; set; }
			public string CAUSE { get; set; }
			public string CAUSENAME { get; set; }
			public string CAUSE_MEMO { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string CHECKOUT_TIME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F200102Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F200102Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F050301Data.MetaData))]
	public partial class F050301Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSelected { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELV_DATE { get; set; }
			public string PICK_TIME { get; set; }
			public string CUST_ORD_NO { get; set; }
			public string ORD_NO { get; set; }
			public string CUST_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string DC_CODE { get; set; }
			public string ORD_TYPE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F050301Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F050301Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F0513Data.MetaData))]
	public partial class F0513Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DELV_DATE { get; set; }
			public string PICK_TIME { get; set; }
			public string ALL_ID { get; set; }
			public string ALL_COMP { get; set; }
			public string CUST_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string DC_CODE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F0513Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F0513Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F200103Data.MetaData))]
	public partial class F200103Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			public string ADJUST_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ADJUST_SEQ { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string LOC_CODE { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ITEM_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ADJ_QTY_IN { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ADJ_QTY_OUT { get; set; }
			public string CAUSE { get; set; }
			public string CAUSENAME { get; set; }
			public string CAUSE_MEMO { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }
			public string UPD_STAFF { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F200103Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F200103Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1913Data.MetaData))]
	public partial class F1913Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSelected { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]

			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(13,ErrorMessage = "最大長度為13碼")] 
			public string LOC_CODE { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			public DateTime? ENTER_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string BUNDLE_SERIALNO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 QTY { get; set; }
			public Int32? ADJ_QTY_IN { get; set; }
			public Int32? ADJ_QTY_OUT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CAUSE { get; set; }
			public string CAUSENAME { get; set; }
			public string CAUSE_MEMO { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string SERIALNO_SCANOK { get; set; }
			public string WORK_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean ISADD { get; set; }
			public string DC_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1913Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1913Data>.Validate(this, columnName);
			}
		}
	}
}
