using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P16ExDataService
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

    [MetadataType(typeof(F161201DetailDatas.MetaData))]
    public partial class F161201DetailDatas : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string RETURN_NO { get; set; }
            public string RETURN_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_QTY { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public Int32? AUDIT_QTY { get; set; }
            public Int32? MOVED_QTY { get; set; }
            public string MEMO { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public string CAUSE { get; set; }
            public Decimal? AUDIT_QTY_SUM { get; set; }
            public string SERIAL_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F161201DetailDatas>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F161201DetailDatas>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F161601DetailDatas.MetaData))]
    public partial class F161601DetailDatas : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string RTN_APPLY_NO { get; set; }
            public DateTime? RTN_APPLY_DATE { get; set; }
            public string STATUS { get; set; }
            public string MEMO { get; set; }
            public Int32? RTN_APPLY_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public string SRC_LOC { get; set; }
            public string TRA_LOC { get; set; }
            public Int32? LOC_QTY { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            public Int32? MOVED_QTY { get; set; }
            public Int32? POST_QTY { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F161601DetailDatas>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F161601DetailDatas>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F161401ReturnWarehouse.MetaData))]
    public partial class F161401ReturnWarehouse : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string CUST_CODE { get; set; }
            public string RETURN_NO { get; set; }
            public string LOC_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 MOVED_QTY { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string WAREHOUSE_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F161401ReturnWarehouse>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F161401ReturnWarehouse>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(PrintF161601Data.MetaData))]
    public partial class PrintF161601Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 ROWNUM { get; set; }
            public string DC_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string GUP_NAME { get; set; }
            public string CUST_CODE { get; set; }
            public string CUST_NAME { get; set; }
            public string RTN_APPLY_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime RTN_APPLY_DATE { get; set; }
            public string STATUS { get; set; }
            public string MEMO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_APPLY_SEQ { get; set; }
            public string ITEM_CODE { get; set; }
            public string SRC_LOC { get; set; }
            public string TRA_LOC { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 LOC_QTY { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string WAREHOUSE_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 MOVED_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 POST_QTY { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public string APPROVE_STAFF { get; set; }
            public string APPROVE_NAME { get; set; }
            public string ALLOCATION_NO { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<PrintF161601Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<PrintF161601Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F160201Data.MetaData))]
    public partial class F160201Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string RTN_VNR_NO { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime RTN_VNR_DATE { get; set; }
            public string STATUS { get; set; }
            public string ORD_PROP { get; set; }
            public string RTN_VNR_TYPE_ID { get; set; }
            public string RTN_VNR_CAUSE { get; set; }
            public string SELF_TAKE { get; set; }
            public string VNR_CODE { get; set; }
            public string COST_CENTER { get; set; }
            public string MEMO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime POSTING_DATE { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            public string CUST_ORD_NO { get; set; }
            public string VNR_NAME { get; set; }
            public string STATUS_TEXT { get; set; }
            public string ORD_PROP_TEXT { get; set; }
            public string RTN_VNR_TYPE_TEXT { get; set; }
            public string RTN_VNR_CAUSE_TEXT { get; set; }
            public string ADDRESS { get; set; }
            public string ITEM_CONTACT { get; set; }
            public string ITEM_TEL { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F160201Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F160201Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F160201DataDetail.MetaData))]
    public partial class F160201DataDetail : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Decimal ROWNUM { get; set; }
            public string RTN_VNR_NO { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_VNR_SEQ { get; set; }
            public string CUST_CODE { get; set; }
            public string GUP_CODE { get; set; }
            public string DC_CODE { get; set; }
            public string ORG_WAREHOUSE_ID { get; set; }
            public string WAREHOUSE_ID { get; set; }
            public string LOC_CODE { get; set; }
            public string ITEM_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_VNR_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 RTN_WMS_QTY { get; set; }
            public string MEMO { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 NOT_RTN_WMS_QTY { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 INVENTORY_QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F160201DataDetail>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F160201DataDetail>.Validate(this, columnName);
            }
        }
    }

	[MetadataType(typeof(F160501Data.MetaData))]
	public partial class F160501Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DESTROY_NO { get; set; }
			public string WMS_ORD_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			public string DC_NAME { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime DESTROY_DATE { get; set; }
			public string DISTR_CAR { get; set; }
			public string STATUS { get; set; }
			public string STATUS_NAME { get; set; }
			public DateTime? POSTING_DATE { get; set; }
			public string MEMO { get; set; }
			public string CUST_ORD_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }
            public string DISTR_CAR_NO { get; set; }
            public string EDI_FLAG { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160501Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160501Data>.Validate(this, columnName);
			}
		}
	}
	[MetadataType(typeof(F160502Data.MetaData))]
	public partial class F160502Data : IDataErrorInfo
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
			public string ITEM_CODE { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			public string VIRTUAL_TYPE { get; set; }
			public string ITEM_SERIALNO { get; set; }
			public string BUNDLE_SERIALNO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SCRAP_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 DESTROY_QTY { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160502Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160502Data>.Validate(this, columnName);
			}
		}
	}
	[MetadataType(typeof(F160402AddData.MetaData))]
	public partial class F160402AddData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Decimal ROWNUM { get; set; }
			public string ITEM_CODE { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string LOC_CODE { get; set; }
			public string WAREHOUSE_ID { get; set; }
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCRAP_CAUSE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32? SCRAP_QTY { get; set; }
			public Decimal? QTY { get; set; }
			public Decimal? ALL_QTY { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160402AddData>.Validate(this));
			}
		}
	
		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160402AddData>.Validate(this, columnName);
			}
		}
	}
	[MetadataType(typeof(F160501FileData.MetaData))]
	public partial class F160501FileData : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DESTROY_NO { get; set; }
			public string UPLOAD_SEQ { get; set; }
			public string UPLOAD_S_PATH { get; set; }
			public string UPLOAD_C_PATH { get; set; }
			public string UPLOAD_DESC { get; set; }
			public string DC_CODE { get; set; }
			public string GUP_CODE { get; set; }
			public string CUST_CODE { get; set; }
			public string DB_Flag { get; set; }
			public string IsDelete { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160501FileData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160501FileData>.Validate(this, columnName);
			}
		}
	}
	[MetadataType(typeof(F160402Data.MetaData))]
	public partial class F160402Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCRAP_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SCRAP_SEQ { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ITEM_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 SCRAP_QTY { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime VALID_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string WAREHOUSE_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string LOC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCRAP_CAUSE { get; set; }
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
			public string ITEM_NAME { get; set; }
			public string ITEM_SIZE { get; set; }
			public string ITEM_SPEC { get; set; }
			public string ITEM_COLOR { get; set; }
			public Decimal? QTY { get; set; }
			public Decimal? ALL_QTY { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160402Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160402Data>.Validate(this, columnName);
			}
		}
	}


	[MetadataType(typeof(F160204Data.MetaData))]
	public partial class F160204Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string VNR_CODE { get; set; }
			public string VNR_NAME { get; set; }
			public string PROC_FLAG { get; set; }
			public string DELIVERY_WAY { get; set; }
			public string ALL_ID { get; set; }
			public int? SHEET_NUM { get; set; }
			public string MEMO { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F160204Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F160204Data>.Validate(this, columnName);
			}
		}
	}
}
