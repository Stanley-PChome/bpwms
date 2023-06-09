using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
using System.Runtime.Serialization;
namespace Wms3pl.WpfClient.ExDataServices.P71ExDataService
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

	[MetadataType(typeof(F1912WithF1980.MetaData))]
	public partial class F1912WithF1980 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string WAREHOUSE_TYPE { get; set; }
			public string WAREHOUSE_TYPE_NAME { get; set; }
			public string TEMP_TYPE { get; set; }
			public string TEMP_TYPE_NAME { get; set; }
			public string CAL_STOCK { get; set; }
			public string CAL_FEE { get; set; }
			public string FLOOR { get; set; }
			public string AREA_CODE { get; set; }
			public string AREA_NAME { get; set; }
			public string CHANNEL { get; set; }
			public string PLAIN { get; set; }
			public string LOC_LEVEL { get; set; }
			public string LOC_TYPE { get; set; }
			public string LOC_CHAR { get; set; }
			public string LOC_TYPE_ID { get; set; }
			public string LOC_TYPE_NAME { get; set; }
			public string HANDY { get; set; }
			public string NOW_STATUS_ID { get; set; }
			public string PRE_STATUS_ID { get; set; }
			public string UCC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			public Decimal? HOR_DISTANCE { get; set; }
			public DateTime? RENT_BEGIN_DATE { get; set; }
			public DateTime? RENT_END_DATE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1912WithF1980>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1912WithF1980>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1912StatusEx2.MetaData))]
	public partial class F1912StatusEx2 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			public string AREA_CODE { get; set; }
			public string AREA_NAME { get; set; }
			public string NOW_STATUS_ID { get; set; }
			public string LOC_STATUS_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string UCC_CODE { get; set; }
			public string CAUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1912StatusEx2>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1912StatusEx2>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1912StatusEx.MetaData))]
	public partial class F1912StatusEx : IDataErrorInfo
	{
		public class MetaData
		{
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			public string AREA_CODE { get; set; }
			public string AREA_NAME { get; set; }
			public string NOW_STATUS_ID { get; set; }
			public string LOC_STATUS_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_NAME { get; set; }
			public string UCC_CODE { get; set; }
			public string CAUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1912StatusEx>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1912StatusEx>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1912StatisticReport.MetaData))]
	public partial class F1912StatisticReport : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROW_NUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			public string WAREHOUSE_TYPE { get; set; }
			public string WAREHOUSE_TYPE_NAME { get; set; }
			public Decimal? LOCCOUNT { get; set; }
			public Decimal? PERCENTAGE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1912StatisticReport>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1912StatisticReport>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1980Data.MetaData))]
	public partial class F1980Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_Name { get; set; }
			public string TMPR_TYPE { get; set; }
			public string CAL_STOCK { get; set; }
			public string CAL_FEE { get; set; }
			public string WAREHOUSE_TYPE { get; set; }
			public Decimal? HOR_DISTANCE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Boolean IsModifyDate { get; set; }
			public DateTime? RENT_BEGIN_DATE { get; set; }
			public DateTime? RENT_END_DATE { get; set; }
			public string LOC_TYPE_ID { get; set; }
			public string LOC_TYPE_NAME { get; set; }
			public string HANDY { get; set; }
			public string FLOOR { get; set; }
			public string MINCHANNEL { get; set; }
			public string MAXCHANNEL { get; set; }
			public string MINPLAIN { get; set; }
			public string MAXPLAIN { get; set; }
			public string MINLOC_LEVEL { get; set; }
			public string MAXLOC_LEVEL { get; set; }
			public string MINLOC_TYPE { get; set; }
			public string MAXLOC_TYPE { get; set; }
			public string DEVICE_TYPE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1980Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1980Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F191202Ex.MetaData))]
	public partial class F191202Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROW_NUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime TRANS_DATE { get; set; }
			public string CUST_NAME { get; set; }
			public string WAREHOUSE_TYPE_NAME { get; set; }
			public string LOC_CODE { get; set; }
			public string TRANS_STATUS { get; set; }
			public string LOC_STATUS_NAME { get; set; }
			public string EMP_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F191202Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F191202Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1919Data.MetaData))]
	public partial class F1919Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string WAREHOUSE_Name { get; set; }
			public string AREA_CODE { get; set; }
			public string AREA_NAME { get; set; }
			public string ATYPE_CODE { get; set; }
			public string ATYPE_NAME { get; set; }
			public string FLOOR { get; set; }
			public string MINCHANNEL { get; set; }
			public string MAXCHANNEL { get; set; }
			public string MINPLAIN { get; set; }
			public string MAXPLAIN { get; set; }
			public string MINLOC_LEVEL { get; set; }
			public string MAXLOC_LEVEL { get; set; }
			public string MINLOC_TYPE { get; set; }
			public string MAXLOC_TYPE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1919Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1919Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1947Ex.MetaData))]
	public partial class F1947Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			public string GUP_CODE { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_CODE { get; set; }
			public string CUST_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALL_ID { get; set; }
			public string ALL_COMP { get; set; }
			public string PIER_CODE { get; set; }
			public string CAN_IN { get; set; }
			public string CAN_OUT { get; set; }
			public string CAN_FAST { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1947Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1947Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F190001Data.MetaData))]
	public partial class F190001Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal TICKET_ID { get; set; }
			public string TICKET_NAME { get; set; }
			public string TICKET_TYPE { get; set; }
			public string TICKET_CLASS { get; set; }
			public string SHIPPING_ASSIGN { get; set; }
			public string FAST_DELIVER { get; set; }
			public string ASSIGN_DELIVER { get; set; }
			public string OUT_TYPE { get; set; }
			public Int16? PRIORITY { get; set; }
			public string CUST_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string DC_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string DC_NAME { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_NAME { get; set; }
			public string ORD_NAME { get; set; }
			public string ORD_PROP_NAME { get; set; }
			public string SHIPPING_ASSIGN_NAME { get; set; }
			public string FAST_DELIVER_NAME { get; set; }
			public string ALL_COMP { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal MILESTONE_ID { get; set; }
			public string MILESTONE_NO { get; set; }
			public string SORT_NO { get; set; }
			public string MILESTONE_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F190001Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F190001Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F19000103Data.MetaData))]
	public partial class F19000103Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string TICKET_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string TICKET_CLASS { get; set; }
			public string MILESTONE_NO_A { get; set; }
			public string MILESTONE_NO_B { get; set; }
			public string MILESTONE_NO_C { get; set; }
			public string MILESTONE_NO_D { get; set; }
			public string MILESTONE_NO_E { get; set; }
			public string MILESTONE_NO_F { get; set; }
			public string MILESTONE_NO_G { get; set; }
			public string MILESTONE_NO_H { get; set; }
			public string MILESTONE_NO_I { get; set; }
			public string MILESTONE_NO_J { get; set; }
			public string MILESTONE_NO_K { get; set; }
			public string MILESTONE_NO_L { get; set; }
			public string MILESTONE_NO_M { get; set; }
			public string MILESTONE_NO_N { get; set; }
			public string MILESTONE_NO_A_NAME { get; set; }
			public string MILESTONE_NO_B_NAME { get; set; }
			public string MILESTONE_NO_C_NAME { get; set; }
			public string MILESTONE_NO_D_NAME { get; set; }
			public string MILESTONE_NO_E_NAME { get; set; }
			public string MILESTONE_NO_F_NAME { get; set; }
			public string MILESTONE_NO_G_NAME { get; set; }
			public string MILESTONE_NO_H_NAME { get; set; }
			public string MILESTONE_NO_I_NAME { get; set; }
			public string MILESTONE_NO_J_NAME { get; set; }
			public string MILESTONE_NO_K_NAME { get; set; }
			public string MILESTONE_NO_L_NAME { get; set; }
			public string MILESTONE_NO_M_NAME { get; set; }
			public string MILESTONE_NO_N_NAME { get; set; }
			public string CRT_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F19000103Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F19000103Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F050003Ex.MetaData))]
	public partial class F050003Ex : IDataErrorInfo
	{
		public class MetaData
		{
			public Int32 SEQ_NO { get; set; }
			public DateTime? BEGIN_DELV_DATE { get; set; }
			public DateTime? END_DELV_DATE { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string STATUS { get; set; }
			public string DC_NAME { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_NAME { get; set; }
			public decimal TICKET_ID { get; set; }
			public string TICKET_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F050003Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F050003Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F05000301Ex.MetaData))]
	public partial class F05000301Ex : IDataErrorInfo
	{
		public class MetaData
		{
			public Int32? SEQ_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			public DateTime? CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string ITEM_COLOR { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F05000301Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F05000301Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F050004WithF190001.MetaData))]
	public partial class F050004WithF190001 : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal TICKET_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SOUTH_PRIORITY_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ORDER_LIMIT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 DELV_DAY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SPLIT_FLOOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string MERGE_ORDER { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CRT_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			[DataMember]
			public string TICKET_NAME { get; set; }
			[DataMember]
			public string DC_NAME { get; set; }
			[DataMember]
			public string GUP_NAME { get; set; }
			[DataMember]
			public string CUST_NAME { get; set; }
			[DataMember]
			public string ORD_NAME { get; set; }
			[DataMember]
			public string TICKET_CLASS_NAME { get; set; }
			[DataMember]
			public string SPLIT_FLOOR_NAME { get; set; }
			[DataMember]
			public string MERGE_ORDER_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F050004WithF190001>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{

				return InputValidator<F050004WithF190001>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F190002Data.MetaData))]
	public partial class F190002Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal TICKET_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 SWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 TWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 OWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 BWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 GWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 NWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 WWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 RWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 DWAREHOUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int16 MWAREHOUSE { get; set; }
			public string TICKET_NAME { get; set; }
			public string TICKET_TYPE { get; set; }
			public string TICKET_CLASS { get; set; }
			public string ORD_NAME { get; set; }
			public string TICKET_CLASS_NAME { get; set; }
			public string CUST_NAME { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F190002Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F190002Data>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F194704Data.MetaData))]
	public partial class F194704Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALL_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GUP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CUST_CODE { get; set; }
			public string CRT_STAFF { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_NAME { get; set; }
			public string ALL_COMP { get; set; }
			public string GUP_NAME { get; set; }
			public string CUST_NAME { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CONSIGN_FORMAT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string GET_CONSIGN_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PRINT_CONSIGN { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string PRINTER_TYPE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string AUTO_PRINT_CONSIGN { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F194704Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F194704Data>.Validate(this, columnName);
			}
		}
	}


	[MetadataType(typeof(F199001Ex.MetaData))]
	public partial class F199001Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string TMPR_TYPE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_TYPE_ID { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ACC_UNIT { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[Range(1, 32767, ErrorMessage = "超出範圍{1}~{2}")]
			public short ACC_NUM { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[Range(typeof(Decimal), "0", "9999999.99", ErrorMessage = "超出範圍{1}~{2}")]
			public decimal UNIT_FEE { get; set; }

			public string CRT_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			public System.DateTime CRT_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
			public Nullable<System.DateTime> UPD_DATE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string IN_TAX { get; set; }

			public System.Int16 LENGTH { get; set; }
			public System.Int16 DEPTH { get; set; }
			public System.Int16 HEIGHT { get; set; }
			public System.Decimal WEIGHT { get; set; }
			public string STATUS { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_TYPE_ID { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ACC_ITEM_KIND_ID { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(50, ErrorMessage = "長度限制{1}碼")]
			public string ACC_ITEM_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F199001Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F199001Ex>.Validate(this, columnName);
			}
		}
	}

    [MetadataType(typeof(F020201Data.MetaData))]
    public partial class F020201Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string RT_SEQ { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PURCHASE_SEQ { get; set; }
            public string VNR_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            public DateTime? RECE_DATE { get; set; }
            public DateTime? VALI_DATE { get; set; }
            public DateTime? MADE_DATE { get; set; }
            public Int32? ORDER_QTY { get; set; }
            public Int32? RECV_QTY { get; set; }
            public Int32? CHECK_QTY { get; set; }
            public string F151001_STATUS { get; set; }
            public string F010201_STATUS { get; set; }
            public string CHECK_ITEM { get; set; }
            public string CHECK_SERIAL { get; set; }
            public string ISPRINT { get; set; }
            public string ISUPLOAD { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string SPECIAL_DESC { get; set; }
            public string SPECIAL_CODE { get; set; }
            public string ISSPECIAL { get; set; }
            public DateTime? IN_DATE { get; set; }
            public string TARWAREHOUSE_ID { get; set; }
            public string QUICK_CHECK { get; set; }
            public string MAKE_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F020201Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F020201Data>.Validate(this, columnName);
            }
        }
    }
}
