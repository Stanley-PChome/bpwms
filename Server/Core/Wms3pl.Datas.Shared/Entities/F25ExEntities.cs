using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataServiceKey("SERIAL_NO")]
	public class F2501ItemData
	{
		public string SERIAL_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string STATUS { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F250102Item
	{
		public int ROWNUM { get; set; }
		public System.Int64 LOG_SEQ { get; set; }
		public System.DateTime FREEZE_BEGIN_DATE { get; set; }
		public System.DateTime FREEZE_END_DATE { get; set; }
		public string SERIAL_NO_BEGIN { get; set; }
		public string SERIAL_NO_END { get; set; }
		public string BOX_SERIAL { get; set; }
		public string BATCH_NO { get; set; }
		public string CONTROL { get; set; }
		public string CAUSE { get; set; }
		public string MEMO { get; set; }
		public string CONTROL_DESC { get; set; }
		public string CAUSE_DESC { get; set; }
		public bool ISSELECTED { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM", "SERIAL_NO")]
	public class F2501QueryData
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string STATUS_NAME { get; set; }
		[DataMember]
		public string ITEM_TYPE { get; set; }
		[DataMember]
		public string BOX_SERIAL { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string TAG3G { get; set; }
		[DataMember]
		public string PUK { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string CASE_NO { get; set; }
		[DataMember]
		public string PO_NO { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public DateTime? IN_DATE { get; set; }
		[DataMember]
		public string ORD_PROP_NAME { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string ACTIVATED { get; set; }
		[DataMember]
		public string PROCESS_NO { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string SYS_NAME { get; set; }
		[DataMember]
		public string CAMERA_NO { get; set; }
		[DataMember]
		public string CLIENT_IP { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public string SEND_CUST { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? CRT_DATE { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }

		[DataMember]
		public string BOUNDLE_ITEM_CODE { get; set; }
		[DataMember]
		public decimal? COMBIN_NO { get; set; }

	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM", "SERIAL_NO")]
	public class P2502QueryData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string TYPE { get; set; }
		[DataMember]
		public string BOX_SERIAL { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public string TAG3G { get; set; }
		[DataMember]
		public string PUK { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string CASE_NO { get; set; }
		[DataMember]
		public string PO_NO { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public DateTime? IN_DATE { get; set; }
		[DataMember]
		public string ORD_PROP_NAME { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string ACTIVATED { get; set; }
		[DataMember]
		public string PROCESS_NO { get; set; }
		[DataMember]
		public Int32? COMBIN_NO { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string SYS_NAME { get; set; }
		[DataMember]
		public string CAMERA_NO { get; set; }
		[DataMember]
		public string CLIENT_IP { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public string SEND_CUST { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
	}


	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F2501WcfData
	{
		[DataMember]

		public Decimal ROWNUM { get; set; }
		[DataMember]
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
		public string SERIAL_NO { get; set; }
		[DataMember]
		[Required(ErrorMessage = "必要欄位", AllowEmptyStrings = false)]
		public string ITEM_CODE { get; set; }
    /// <summary>
    /// 未使用
    /// </summary>
    [DataMember]
		public string BOX_SERIAL { get; set; }
		[DataMember]
		public string TAG3G { get; set; }
    /// <summary>
    /// 未使用
    /// </summary>
    [DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string PUK { get; set; }
		[DataMember]
		public string STATUS { get; set; }
    /// <summary>
    /// 未使用
    /// </summary>
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
    /// <summary>
    /// 未使用
    /// </summary>
    [DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string PO_NO { get; set; }
		[DataMember]
		public string ACTIVATED { get; set; }
		[DataMember]
		public string SEND_CUST { get; set; }
		[DataMember]
		public string WMS_NO { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string SYS_VNR { get; set; }
		[DataMember]
		public string PROCESS_NO { get; set; }
		[DataMember]
		public string ORD_PROP { get; set; }
		[DataMember]
		public string CASE_NO { get; set; }
		[DataMember]
		public DateTime? IN_DATE { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public int? COMBIN_NO { get; set; }
		[DataMember]
		public string CAMERA_NO { get; set; }
		[DataMember]
		public string CLIENT_IP { get; set; }

		[DataMember]
		public string BUNDLE_SERIALLOC { get; set; }
		[DataMember]
		public string LOC_MIX_ITEM { get; set; }
		[DataMember]
		public bool IsChangeItemCode { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
        [DataMember]
        public string MAKE_NO { get; set; }
	}

	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F250103Verification
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string SerialNo { get; set; }
		[DataMember]
		public string Status { get; set; }
		[DataMember]
		public string Verification { get; set; }
		[DataMember]
		public string Message { get; set; }
	}



	[DataContract]
	[DataServiceKey("ITEM_CODE")]
	public class F2501SerialItemData
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public string CELL_NUM { get; set; }
		[DataMember]
		public string PUK { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ClearSerialBoxOrCaseNo
	{
		public decimal ROWNUM { get; set; }
		public string BoxOrCaseNo { get; set; }
	}

	[Serializable]
	[DataServiceKey("LOG_SEQ")]
	public class P250301QueryItem
	{
		public Int64 LOG_SEQ { get; set; }
		public string SERIAL_NO { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime ORG_VALID_DATE { get; set; }
		public DateTime? VALID_DATE { get; set; }
		public string SERIAL_STATUS { get; set; }
		public string ISPASS { get; set; }
		public string MESSAGE { get; set; }
		public string STATUS { get; set; }
		public string CLIENT_IP { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string CRT_STAFF { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_NAME { get; set; }
		public string ITEM_NAME { get; set; }
		public string ISPASS_DESC { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("LOG_SEQ")]
	public class P250302QueryItem
	{
		[DataMember]
		public Int64 LOG_SEQ { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string NEW_SERIAL_NO { get; set; }
		[DataMember]
		public string SERIAL_STATUS { get; set; }
		[DataMember]
		public string ISPASS { get; set; }
		[DataMember]
		public string MESSAGE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CLIENT_IP { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ISPASS_DESC { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("SERIAL_NO")]
	public class SerialNoWithStatus
	{
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string STATUS { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class NotDbFindSerial
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
	}
}
