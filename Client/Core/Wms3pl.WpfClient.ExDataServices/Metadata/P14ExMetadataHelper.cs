using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P14ExDataService
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

	[MetadataType(typeof(InventoryDetailItem.MetaData))]
	public partial class InventoryDetailItem : IDataErrorInfo
	{
		public class MetaData
		{
			public string ChangeStatus { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSelected { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime ENTER_DATE { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 QTY { get; set; }
			public Int32? FIRST_QTY_ORG { get; set; }
			[Range(typeof(Int32), "0", "999999999", ErrorMessage = "必須介於{1}~{2}之間")]
			public Int32? FIRST_QTY { get; set; }
			public Int32? SECOND_QTY_ORG { get; set; }
			[Range(typeof(Int32), "0", "999999999", ErrorMessage = "必須介於{1}~{2}之間")]
			public Int32? SECOND_QTY { get; set; }
			public string FLUSHBACK_ORG { get; set; }
			public string FLUSHBACK { get; set; }
			public string SERIAL_NO { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<InventoryDetailItem>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<InventoryDetailItem>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(InventoryWareHouse.MetaData))]
	public partial class InventoryWareHouse : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSelected { get; set; }
			public string DC_CODE { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string CHANNEL_BEGIN { get; set; }
			public string CHANNEL_END { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<InventoryWareHouse>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<InventoryWareHouse>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(InventoryItem.MetaData))]
	public partial class InventoryItem : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsSelected { get; set; }
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<InventoryItem>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<InventoryItem>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F140111Data.MetaData))]
	public partial class F140111Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string INVENTORY_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_TYPE { get; set; }
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
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F140111Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F140111Data>.Validate(this, columnName);
			}
		}
	}
}
