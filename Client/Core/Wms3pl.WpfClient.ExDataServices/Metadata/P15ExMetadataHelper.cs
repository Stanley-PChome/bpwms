using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Services.Common;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P15ExDataService
{
	
	[MetadataType(typeof(ExecuteResult.MetaData))]
	public partial class ExecuteResult : IDataErrorInfo
	{
		public class MetaData
		{
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Boolean IsSuccessed {get; set;}
	public string Message {get; set;}
	
}

		[DoNotSerialize]
		public string Error
		{
			get {
				return string.Join<string>(",", InputValidator<ExecuteResult>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get {				
				return InputValidator<ExecuteResult>.Validate(this, columnName);
			}
		}
	}
	
	[MetadataType(typeof(F151001Data.MetaData))]
	public partial class F151001Data : IDataErrorInfo
	{
		public class MetaData
		{
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Decimal ROWNUM {get; set;}
	public DateTime? ALLOCATION_DATE {get; set;}
	public string ALLOCATION_NO {get; set;}
	public DateTime? CRT_ALLOCATION_DATE {get; set;}
	public DateTime? POSTING_DATE {get; set;}
	public string STATUS {get; set;}
	public string TAR_DC_CODE {get; set;}
	public string TAR_WAREHOUSE_ID {get; set;}
	public string SRC_WAREHOUSE_ID {get; set;}
	public string SRC_DC_CODE {get; set;}
	public string SOURCE_TYPE {get; set;}
	public string SOURCE_NO {get; set;}
	public string BOX_NO {get; set;}
	public string MEMO {get; set;}
	public string DC_CODE {get; set;}
	public string GUP_CODE {get; set;}
	public string CUST_CODE {get; set;}
	public string CRT_STAFF {get; set;}
	public DateTime? CRT_DATE {get; set;}
	public string UPD_STAFF {get; set;}
	public DateTime? UPD_DATE {get; set;}
	public string CRT_NAME {get; set;}
	public string UPD_NAME {get; set;}
	
}

		[DoNotSerialize]
		public string Error
		{
			get {
				return string.Join<string>(",", InputValidator<F151001Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get {				
				return InputValidator<F151001Data>.Validate(this, columnName);
			}
		}
	}
	
	[MetadataType(typeof(F151001DetailDatas.MetaData))]
	public partial class F151001DetailDatas : IDataErrorInfo
	{
		public class MetaData
		{
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Decimal ROWNUM {get; set;}
	public string DC_CODE {get; set;}
	public string GUP_CODE {get; set;}
	public string CUST_CODE {get; set;}
	public string ALLOCATION_NO {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public DateTime ALLOCATION_DATE {get; set;}
	public DateTime? POSTING_DATE {get; set;}
	public string STATUS {get; set;}
	public string SOURCE_TYPE {get; set;}
	public string SOURCE_NO {get; set;}
	public string SRC_DC_CODE {get; set;}
	public string TAR_DC_CODE {get; set;}
	public string SRC_WAREHOUSE_ID {get; set;}
	public string TAR_WAREHOUSE_ID {get; set;}
	public string SEND_CAR {get; set;}
	public string SRC_WAREHOUSE_NAME {get; set;}
	public string TAR_WAREHOUSE_NAME {get; set;}
	public string ITEM_CODE {get; set;}
	public string SRC_LOC_CODE {get; set;}
	public string SUG_LOC_CODE {get; set;}
	public string TAR_LOC_CODE {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int64 SRC_QTY {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int64 A_SRC_QTY {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int64 TAR_QTY {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int64 A_TAR_QTY {get; set;}
	public DateTime? SRC_DATE {get; set;}
	public string SRC_STAFF {get; set;}
	public DateTime? TAR_DATE {get; set;}
	public string TAR_STAFF {get; set;}
	public string ITEM_NAME {get; set;}
	public string ITEM_SIZE {get; set;}
	public string ITEM_SPEC {get; set;}
	public string ITEM_COLOR {get; set;}
	public string CHECK_SERIALNO {get; set;}
	public string BUNDLE_SERIALLOC {get; set;}
	
}

		[DoNotSerialize]
		public string Error
		{
			get {
				return string.Join<string>(",", InputValidator<F151001DetailDatas>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get {				
				return InputValidator<F151001DetailDatas>.Validate(this, columnName);
			}
		}
	}
	
	[MetadataType(typeof(F151001ReportData.MetaData))]
	public partial class F151001ReportData : IDataErrorInfo
	{
		public class MetaData
		{
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Decimal ROWNUM {get; set;}
	public string GUP_CODE {get; set;}
	public string CUST_CODE {get; set;}
	public string ALLOCATION_NO {get; set;}
	public string SRC_WAREHOUSE_ID {get; set;}
	public string TAR_WAREHOUSE_ID {get; set;}
	public string ITEM_CODE {get; set;}
	public string SRC_LOC_CODE {get; set;}
	public string SUG_LOC_CODE {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int32 SRC_QTY {get; set;}
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Int32 TAR_QTY {get; set;}
	public string ITEM_NAME {get; set;}
	public string SRC_WAREHOUSE_NAME {get; set;}
	public string TAR_WAREHOUSE_NAME {get; set;}
	public string GUP_NAME {get; set;}
	public string CUST_NAME {get; set;}
	
}

		[DoNotSerialize]
		public string Error
		{
			get {
				return string.Join<string>(",", InputValidator<F151001ReportData>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get {				
				return InputValidator<F151001ReportData>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F1913WithF1912Moved.MetaData))]
	public partial class F1913WithF1912Moved : IDataErrorInfo
	{
		public class MetaData
		{
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings=false)]	
	public Decimal ROWNUM {get; set;}
	public string WAREHOUSE_ID {get; set;}
	public string WAREHOUSE_NAME {get; set;}
	public string LOC_CODE {get; set;}
	public string ITEM_CODE {get; set;}
	public string ITEM_NAME {get; set;}
	public string ITEM_SIZE {get; set;}
	public string ITEM_SPEC {get; set;}
	public string ITEM_COLOR {get; set;}
	public Int64? QTY {get; set;}
	public Decimal? MOVE_QTY {get; set;}
	
}

		[DoNotSerialize]
		public string Error
		{
			get {
				return string.Join<string>(",", InputValidator<F1913WithF1912Moved>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get {
				return InputValidator<F1913WithF1912Moved>.Validate(this, columnName);
			}
		}
	}

    [MetadataType(typeof(F151002ItemData.MetaData))]
    public partial class F151002ItemData : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALLOCATION_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string SRC_LOC_CODE { get; set; }
            public string STATUS { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 SRC_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 A_SRC_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 TAR_QTY { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 A_TAR_QTY { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F151002ItemData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F151002ItemData>.Validate(this, columnName);
            }
        }
    }
}
