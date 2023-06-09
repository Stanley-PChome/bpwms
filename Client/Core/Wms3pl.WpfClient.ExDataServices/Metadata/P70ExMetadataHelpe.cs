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
namespace Wms3pl.WpfClient.ExDataServices.P70ExDataService
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

    [MetadataType(typeof(F700101EX.MetaData))]
    public partial class F700101EX : IDataErrorInfo
    {
        public class MetaData
        {

            public string DISTR_CAR_NO { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public System.DateTime TAKE_DATE { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ALL_ID { get; set; }

            public decimal? CAR_KIND_ID { get; set; }

            public string SP_CAR { get; set; }

            public string CHARGE_CUST { get; set; }

            public string CHARGE_DC { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public decimal FEE { get; set; }

            public string STATUS { get; set; }

            public string EDI_FLAG { get; set; }

            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            public string CRT_STAFF { get; set; }

            public System.DateTime CRT_DATE { get; set; }

            public string UPD_STAFF { get; set; }

            public Nullable<System.DateTime> UPD_DATE { get; set; }

            public string CRT_NAME { get; set; }

            public string UPD_NAME { get; set; }

            public string WMS_NO { get; set; }

            public string CAR_SIZE { get; set; }

            public string CAR_KIND_NAME { get; set; }

            public string CHARGE_GUP_CODE { get; set; }

            public string CHARGE_CUST_CODE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F700101EX>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F700101EX>.Validate(this, columnName);
            }
        }

    }

	[MetadataType(typeof(F700501Ex.MetaData))]
	public partial class F700501Ex : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCHEDULE_NO { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime SCHEDULE_DATE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCHEDULE_TIME { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SCHEDULE_TYPE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string IMPORTANCE { get; set; }

			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string SUBJECT { get; set; }


			public string CONTENT { get; set; }
			public string CRT_STAFF { get; set; }
			public DateTime CRT_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public string UPD_STAFF { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }
			public string SCHEDULE_TYPE_TEXT { get; set; }
			public string IMPORTANCE_TEXT { get; set; }			
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F700501Ex>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F700501Ex>.Validate(this, columnName);
			}
		}
	}

	[MetadataType(typeof(F700101Data.MetaData))]
	public partial class F700101Data : IDataErrorInfo
	{
		public class MetaData
		{
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public Int32 ROWNUM { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DC_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime TAKE_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DELV_TMPR { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string ALL_ID { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CHARGE_CUST { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string CHARGE_DC { get; set; }
			public string CHARGE_GUP_CODE { get; set; }
			public string CHARGE_CUST_CODE { get; set; }
			public string DISTR_CAR_NO { get; set; }
			public DateTime? DELV_DATE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DISTR_USE { get; set; }
			public string STATUS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(50, ErrorMessage = "長度限制{1}碼")]
			public string CONTACT { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(20, ErrorMessage = "長度限制{1}碼")]
			public string CONTACT_TEL { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(5, ErrorMessage = "長度限制{1}碼")]
			public string ZIP_CODE { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			[MaxLength(200, ErrorMessage = "長度限制{1}碼")]
			public string ADDRESS { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public string DELV_PERIOD { get; set; }
			[Range(typeof(Decimal), "0", "99999999999.99", ErrorMessage = "必須介於{1}~{2}之間")]
			public Decimal? VOLUMN { get; set; }
			[MaxLength(100, ErrorMessage = "長度限制{1}碼")]
			public string MEMO { get; set; }
			public string CONSIGN_STATUS { get; set; }
			public string CONSIGN_NO { get; set; }
			[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
			public DateTime CRT_DATE { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_NAME { get; set; }

		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F700101Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F700101Data>.Validate(this, columnName);
			}
		}
	}
}
