using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{

	#region 出貨狀況控管
	[DataContract]
	[Serializable]
	[DataServiceKey("HELP_NO", "DC_CODE")]
	public class F0010List
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public System.Int32 HELP_NO { get; set; }
		[DataMember]
		public string ORD_NO { get; set; }
		[DataMember]
		public string HELP_TYPE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
	}

	[Serializable]
	[DataServiceKey("VALUE")]
	public class F000904Data
	{
		public string VALUE { get; set; }
		public string NAME { get; set; }
	}
	#endregion

	#region P7105020000 作業計價
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F000904DelvAccType
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE_NAME { get; set; }
	}
	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F0003Ex
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string AP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SYS_PATH { get; set; }
		[DataMember]
		public string FILENAME { get; set; }
		[DataMember]
		public string FILETYPE { get; set; }
		[DataMember]
		public string DESCRIPT { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public bool IsPassword { get; set; }
		[DataMember]
		public string DisplayText { get; set; }
	}

    #region 單據工號綁定_Grid資料
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class F0011BindData
    {
        [DataMember]
        public Decimal ROWNUM { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public int? ID { get; set; }
        /// <summary>
        /// 工號
        /// </summary>
        [DataMember]
        public string EMP_ID { get; set; }
        /// <summary>
        /// 工號名稱
        /// </summary>
        [DataMember]
        public string EMP_NAME { get; set; }
        /// <summary>
        /// 單據編號
        /// </summary>
        [DataMember]
        public string ORDER_NO { get; set; }
        /// <summary>
        /// 揀貨狀態
        /// </summary>
        [DataMember]
        public string PICK_STATUS { get; set; }
        /// <summary>
        /// 單據狀態(不顯示在Grid上)
        /// </summary>
        [DataMember]
        public string STATUS { get; set; }
        /// <summary>
        /// 物流中心
        /// </summary>
        [DataMember]
        public string DC_CODE { get; set; }
        /// <summary>
        /// 業主
        /// </summary>
        [DataMember]
        public string GUP_CODE { get; set; }
        /// <summary>
        /// 貨主
        /// </summary>
        [DataMember]
        public string CUST_CODE { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        [DataMember]
        public DateTime? CRT_DATE { get; set; }
        /// <summary>
        /// 建立人名
        /// </summary>
        [DataMember]
        public string CRT_NAME { get; set; }
        /// <summary>
        /// 作業開始時間
        /// </summary>
        [DataMember]
        public DateTime? START_DATE { get; set; }
        /// <summary>
        /// 作業結束時間
        /// </summary>
        [DataMember]
        public DateTime? CLOSE_DATE { get; set; }
        /// <summary>
        /// 異動日期
        /// </summary>
        [DataMember]
        public DateTime? UPD_DATE { get; set; }
        /// <summary>
        /// 異動人名
        /// </summary>
        [DataMember]
        public string UPD_NAME { get; set; }
}
    #endregion

    #region P7107030000 一般報表-作業異常

    [DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F0010AbnormalData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string HELP_NAME { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public DateTime HAPPEN_DATE { get; set; }
		[DataMember]
		public string UPD_DATE { get; set; }
		[DataMember]
		public Decimal TOTAL_DATE { get; set; }
	}

	#endregion

	#region P2115010000 今日工作指示查詢
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class WorkList
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string VALUE { get; set; }
		[DataMember]
		public string NAME { get; set; }
		[DataMember]
		public string FUNC_ID { get; set; }
		[DataMember]
		public string FUNC_NAME { get; set; }
		[DataMember]
		public Decimal COUNTS { get; set; }

		[DataMember]
		public Decimal COUNTS_B { get; set; }
		
		[DataMember]
		public Decimal COUNTS_C { get; set; }
		
	}
    #endregion

    #region P2102010000 健保追溯報保
    [DataContract]
    [Serializable]
    [DataServiceKey("ROWNUM")]
    public class HealthInsuranceReport
    {
        [DataMember]
        public Decimal ROWNUM { get; set; }

        /// <summary>
        /// 交易型態
        /// </summary>
        [DataMember]
        public string TRANSTYPE { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        [DataMember]
        public DateTime CRT_DATE { get; set; }
        /// <summary>
        /// 公司統編
        /// </summary>
        [DataMember]
        public string UNI_FORM { get; set; }
        /// <summary>
        /// 醫療機構代碼
        /// </summary>
        [DataMember]
        public string BOSS { get; set; }
        /// <summary>
        /// 公司名稱
        /// </summary>
        [DataMember]
        public string CONTACT { get; set; }
        /// <summary>
        /// 藥品許可證字號
        /// </summary>
        [DataMember]
        public string SIM_SPEC { get; set; }
        /// <summary>
        /// 品號
        /// </summary>
        [DataMember]
        public string ITEM_CODE { get; set; }
        /// <summary>
        /// 品名
        /// </summary>
        [DataMember]
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// 批號
        /// </summary>
        [DataMember]
        public string MAKE_NO { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        [DataMember]
        public DateTime VALID_DATE { get; set; }
        /// <summary>
        /// 供應商/下游業者名稱
        /// </summary>
        [DataMember]
        public string VNR_NAME { get; set; }
        /// <summary>
        /// 供應商/下游業者統編
        /// </summary>
        [DataMember]
        public string VNR_UNI_FORM { get; set; }
        /// <summary>
        /// 交易數量
        /// </summary>
        [DataMember]
        public int QTY { get; set; }
        /// <summary>
        /// 交易數量1
        /// </summary>
        [DataMember]
        public string QTY1 { get; set; }
        /// <summary>
        /// 廠商/門市代號
        /// </summary>
        [DataMember]
        public string VNR_RETIAL_CODE { get; set; }

    }

    #endregion

    [DataContract]
    [Serializable]
    [DataServiceKey("ID")]
    public class F0090x
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [DataMember]
        public Int64 ID { get; set; }

        /// <summary>
        /// API名稱
        /// </summary>
        [DataMember]
        public string NAME { get; set; }

        /// <summary>
        /// 傳遞資料
        /// </summary>
        [DataMember]
        public string SEND_DATA { get; set; }

        /// <summary>
        /// 回傳資料
        /// </summary>
        [DataMember]
        public string RETURN_DATA { get; set; }

        /// <summary>
        /// 狀態(0:失敗,1:成功)
        /// </summary>
        [DataMember]
        public string STATUS { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        [DataMember]
        public string ERRMSG { get; set; }

        /// <summary>
        /// 物流中心
        /// </summary>
        [DataMember]
        public string DC_CODE { get; set; }

        /// <summary>
        /// 業主
        /// </summary>
        [DataMember]
        public string GUP_CODE { get; set; }

        /// <summary>
        /// 貨主
        /// </summary>
        [DataMember]
        public string CUST_CODE { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [DataMember]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [DataMember]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [DataMember]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        [DataMember]
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        [DataMember]
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        [DataMember]
        public string UPD_NAME { get; set; }
        [DataMember]
        public String LOG_TABLE { get; set; }
    }

	public class F009X
	{
		public Int64 ID { get; set; }
		public string NAME { get; set; }
		public string SEND_DATA { get; set; }
		public string RETURN_DATA { get; set; }
		public string STATUS { get; set; }
		public string ERRMSG { get; set; }
		public DateTime CRT_DATE { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string TABLE_NAME { get; set; }
	}
  public class testClass
  {
    public string COLUMN_NAME { get; set; }
    public string DATA_TYPE { get; set; }
    public int? CHARACTER_MAXIMUM_LENGTH { get; set; }
    public string IS_NULLABLE { get; set; }
    public int? NUMERIC_PRECISION { get; set; }
    public int? NUMERIC_SCALE { get; set; }
  }



  public class F0000EX
  {
    public long ID { get; set; }
    public string IS_LOCK { get; set; }
  }
}
