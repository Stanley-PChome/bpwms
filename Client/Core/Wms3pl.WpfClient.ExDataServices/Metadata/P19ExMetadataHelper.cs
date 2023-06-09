using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P19ExDataService
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


    [MetadataType(typeof(F197001Data.MetaData))]
    public partial class F197001Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int32 LABEL_SEQ { get; set; }
            public string LABEL_CODE { get; set; }
            public string LABEL_NAME { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_SIZE { get; set; }
            public string ITEM_SPEC { get; set; }
            public string ITEM_COLOR { get; set; }
            public string CUST_ITEM_CODE { get; set; }
            public string SUGR { get; set; }
            public string VNR_CODE { get; set; }
            public string VNR_NAME { get; set; }
            public string WARRANTY { get; set; }
            public Int16? WARRANTY_S_Y { get; set; }
            public string WARRANTY_S_M { get; set; }
            public string WARRANTY_Y { get; set; }
            public string WARRANTY_M { get; set; }
            public Int16? WARRANTY_D { get; set; }
            public string OUTSOURCE { get; set; }
            public string CHECK_STAFF { get; set; }
            public string ITEM_DESC_A { get; set; }
            public string ITEM_DESC_B { get; set; }
            public string ITEM_DESC_C { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CUST_CODE { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string GUP_CODE { get; set; }
            public string CRT_STAFF { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string CRT_NAME { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string UPD_NAME { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F197001Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F197001Data>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F91000302SearchData.MetaData))]
    public partial class F91000302SearchData : IDataErrorInfo
    {
        public class MetaData
        {
            public Decimal ROWNUM { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ITEM_TYPE_ID { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string ACC_UNIT_NAME { get; set; }
            public string CRT_STAFF { get; set; }
            public string CRT_NAME { get; set; }
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public string UPD_NAME { get; set; }
            public DateTime? UPD_DATE { get; set; }
            public string ITEM_TYPE { get; set; }

        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F91000302SearchData>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F91000302SearchData>.Validate(this, columnName);
            }
        }
    }

    [MetadataType(typeof(F19120601Data.MetaData))]
    public partial class F19120601Data : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 物流中心編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// PK區編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PK_AREA { get; set; }

            /// <summary>
            /// 路線順序
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int LINE_SEQ { get; set; }

            /// <summary>
            /// 路線頭碼(儲位前5碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string BEGIN_LOC_CODE { get; set; }

            /// <summary>
            /// 路線尾碼(儲位前5碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string END_LOC_CODE { get; set; }

            /// <summary>
            /// 水平動線(0: 依序進行 1: 單側先行) F000904 topic =F19120601, subtopic = MOVING_HORIZON
            /// </summary>
            public string MOVING_HORIZON { get; set; }

            /// <summary>
            /// 垂直動線 (0: 由小到大 1:由大到小) F000904 topic=F19120601, subtopic=MOVING_VERTICAL
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string MOVING_VERTICAL { get; set; }

            /// <summary>
            /// 處理狀態 (0: 建立 9: 刪除)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PROC_FLAG { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19120601Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19120601Data>.Validate(this, columnName);
            }
        }

    }

    [MetadataType(typeof(F19120602Data.MetaData))]
    public partial class F19120602Data : IDataErrorInfo
    {
        public class MetaData
        {
            /// <summary>
            /// 資料處理方式(Add,Edit)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public String OperateType { get; set; }

            /// <summary>
            /// 流水號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public Int64 ID { get; set; }

            /// <summary>
            /// 物流中心編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string DC_CODE { get; set; }

            /// <summary>
            /// PK區編號
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PK_AREA { get; set; }

            /// <summary>
            /// PK區順序
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int PK_LINE_SEQ { get; set; }

            /// <summary>
            /// 揀貨(實際)樓層
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_FLOOR { get; set; }

            /// <summary>
            /// 路線類型0: 魚骨型(頭接頭)、1: S型(尾接尾) F000904 topic = F191206, subtopic = PICK_TYPE
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string PICK_TYPE { get; set; }

            /// <summary>
            /// 儲位比對值(儲位前5碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CHK_LOC_CODE { get; set; }

            /// <summary>
            /// 路線順序(2碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int LINE_SEQ { get; set; }

            /// <summary>
            /// 座別路順(3碼)
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int PLAIN_SEQ { get; set; }

            /// <summary>
            /// 建立日期
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public DateTime CRT_DATE { get; set; }

            /// <summary>
            /// 建立人名
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_NAME { get; set; }

            /// <summary>
            /// 建立人員
            /// </summary>
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public string CRT_STAFF { get; set; }

            /// <summary>
            /// 異動日期
            /// </summary>
            public DateTime? UPD_DATE { get; set; }

            /// <summary>
            /// 異動人名
            /// </summary>
            public string UPD_NAME { get; set; }

            /// <summary>
            /// 異動人員
            /// </summary>
            public string UPD_STAFF { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F19120602Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F19120602Data>.Validate(this, columnName);
            }
        }

    }

    [MetadataType(typeof(F190106Data.MetaData))]
    public partial class F190106Data : IDataErrorInfo
    {
        public class MetaData
        {
            [Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
            public int ID { get; set; }
            public string DC_CODE { get; set; }
            public string SCHEDULE_TYPE { get; set; }
            public string START_TIME { get; set; }
            public string END_TIME { get; set; }
            public byte PERIOD { get; set; }
            public string ChangeFlag { get; set; }
            public string CRT_STAFF { get; set; }
            public DateTime? CRT_DATE { get; set; }
            public string UPD_STAFF { get; set; }
            public DateTime? UPD_DATE { get; set; }
        }

        [DoNotSerialize]
        public string Error
        {
            get
            {
                return string.Join<string>(",", InputValidator<F190106Data>.Validate(this));
            }
        }

        public string this[string columnName]
        {
            get
            {
                return InputValidator<F190106Data>.Validate(this, columnName);
            }
        }
    }

	[MetadataType(typeof(F1946Data.MetaData))]
	public partial class F1946Data : IDataErrorInfo
	{
		public class MetaData
		{
			public string DC_CODE { get; set; }
			public string WORKSTATION_CODE { get; set; }
			public string WORKSTATION_TYPE { get; set; }
			public string WORKSTATION_GROUP { get; set; }
			public string PACKING_LINE_NO { get; set; }
			public string STATUS { get; set; }
			public DateTime CRT_DATE { get; set; }
			public string CRT_STAFF { get; set; }
			public string CRT_NAME { get; set; }
			public DateTime? UPD_DATE { get; set; }
			public string UPD_STAFF { get; set; }
			public string UPD_NAME { get; set; }
		}

		[DoNotSerialize]
		public string Error
		{
			get
			{
				return string.Join<string>(",", InputValidator<F1946Data>.Validate(this));
			}
		}

		public string this[string columnName]
		{
			get
			{
				return InputValidator<F1946Data>.Validate(this, columnName);
			}
		}
	}
}