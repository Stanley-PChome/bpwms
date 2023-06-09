using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Shared.Entities
{
	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE", "CUST_CODE")]
	////[IgnoreProperties("EncryptedProperties")]
	public class F1929WithF1909Test //: IEncryptable
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }

		//[IgnoreDataMember]
		//public Dictionary<string, string> EncryptedProperties
		//{
		//	get { return new Dictionary<string, string> { { "GUP_NAME", "NOT" } }; }
		//}
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("FUN_CODE", "GRP_ID")]
	public class P1905GroupFunctionMapping
	{
		[DataMember]
		public string FUN_CODE { get; set; }
		[DataMember]
		public string FUN_NAME { get; set; }
		[DataMember]
		public string FUN_TYPE { get; set; }
		[DataMember]
		public string FUN_DESC { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public Decimal? GRP_ID { get; set; }
		[DataMember]
		public Decimal? ISENABLED { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("EMP_ID")]
	public class F1924Data
	{
		[DataMember]
		public string EMP_ID { get; set; }
		[DataMember]
		public string EMP_NAME { get; set; }
		[DataMember]
		public string EMAIL { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string ISDELETED { get; set; }
		[DataMember]
		public string PACKAGE_UNLOCK { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ISCOMMON { get; set; }
		[DataMember]
		public string DEP_ID { get; set; }
		[DataMember]
		public System.Decimal? WORK_ID { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "CUST_CODE", "GUP_CODE")]
	public class F190101Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
	}

	[Serializable]
	[DataServiceKey("EMP_ID", "GRP_ID")]
	public class F1924With192401
	{
		public string EMP_ID { get; set; }
		public decimal? GRP_ID { get; set; }
		public string GRP_NAME { get; set; }
		public string GRP_DESC { get; set; }
	}

	[Serializable]
	[DataServiceKey("EMP_ID", "WORK_ID")]
	public class F1924With192403
	{
		public string EMP_ID { get; set; }
		public decimal? WORK_ID { get; set; }
		public string WORK_NAME { get; set; }
		public string WORK_DESC { get; set; }
	}

	[Serializable]
	[DataServiceKey("EMP_ID")]
	public class GetUserPassword
	{
		public string EMP_ID { get; set; }
		public string PASSWORD { get; set; }
	}

	[Serializable]
	//[DataServiceKey("EMP_ID", "GRP_ID", "FUN_CODE")]
	[DataServiceKey("ROW_NUM")]
	public class EmpWithFuncionName
	{
		public decimal ROW_NUM { get; set; }
		public string EMP_ID { get; set; }
		public string FUN_CODE { get; set; }
		public string FUN_NAME { get; set; }
		public string BTNAME { get; set; }
		public decimal BTOPT { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("UCT_ID,CUST_CODE")]
	public class F1950Ex
	{
		[DataMember]
		public string TYPE_DESC { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("GRP_NAME,FUN_CODE,SHOWINFO")]
	public class P190510Data
	{
		[DataMember]
		public string GRP_NAME { get; set; }
		[DataMember]
		public string FUN_CODE { get; set; }
		[DataMember]
		public string SHOWINFO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("GRP_NAME,EMP_ID")]

	public class P190511Data
	{
		[DataMember]
		public string GRP_NAME { get; set; }
		[DataMember]
		public string EMP_ID { get; set; }
	}

	/// <summary>
	/// 給上版程式(P190503)用的Data Entity
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("FUN_CODE")]
	public class F1954Ex
	{
		[DataMember]
		public string FUN_CODE { get; set; }
		[DataMember]
		public string FUN_NAME { get; set; }
		[DataMember]
		public string FUN_TYPE { get; set; }
		[DataMember]
		public string FUN_DESC { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public System.DateTime? UPLOAD_DATE { get; set; }
		[DataMember]
		public string DISABLE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		/// <summary>
		/// 錯誤訊息. 包括未設定FunctionId, 未設定FunctionName, FunctionId重複等等.
		/// </summary>
		[DataMember]
		public string WarningMessage { get; set; }
		/// <summary>
		/// 主程式的FunctionId
		/// </summary>
		[DataMember]
		public string MainProgram { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("LOC_CODE", "DC_CODE", "WAREHOUSE_ID")]
	public class F1912StatusEx
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		/// <summary>
		/// Source: F1919
		/// </summary>
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string NOW_STATUS_ID { get; set; }
		/// <summary>
		/// Source: F1943
		/// </summary>
		[DataMember]
		public string LOC_STATUS_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// Source: F1980
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string UCC_CODE { get; set; }
		/// <summary>
		/// Source: F1951
		/// </summary>
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// Source: F1929
		/// </summary>
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// Source: F1909
		/// </summary>
		[DataMember]
		public string CUST_NAME { get; set; }

	}

	/// <summary>
	/// 給依商品編號查詢儲位狀態使用的資料集, 因相同ITEM_CODE可能會有不同ITEM_NAME, 所以加上此KEY值
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("LOC_CODE", "DC_CODE", "WAREHOUSE_ID", "ITEM_CODE", "ITEM_NAME")]
	public class F1912StatusEx2
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		/// <summary>
		/// Source: F1919
		/// </summary>
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string NOW_STATUS_ID { get; set; }
		/// <summary>
		/// Source: F1943
		/// </summary>
		[DataMember]
		public string LOC_STATUS_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// Source: F1980
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string UCC_CODE { get; set; }
		/// <summary>
		/// Source: F1951
		/// </summary>
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// Source: F1929
		/// </summary>
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// Source: F1909
		/// </summary>
		[DataMember]
		public string CUST_NAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1980Data
	{
		public int ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_Name { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public string CAL_STOCK { get; set; }
		[DataMember]
		public string CAL_FEE { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public bool IsModifyDate { get; set; }

		[DataMember]
		public DateTime? RENT_BEGIN_DATE { get; set; }
		[DataMember]
		public DateTime? RENT_END_DATE { get; set; }
		[DataMember]
		public string LOC_TYPE_ID { get; set; }
		[DataMember]
		public string LOC_TYPE_NAME { get; set; }
		[DataMember]
		public string HANDY { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string MINCHANNEL { get; set; }
		[DataMember]
		public string MAXCHANNEL { get; set; }
		[DataMember]
		public string MINPLAIN { get; set; }
		[DataMember]
		public string MAXPLAIN { get; set; }
		[DataMember]
		public string MINLOC_LEVEL { get; set; }
		[DataMember]
		public string MAXLOC_LEVEL { get; set; }
		[DataMember]
		public string MINLOC_TYPE { get; set; }
		[DataMember]
		public string MAXLOC_TYPE { get; set; }
		[DataMember]
		public string DEVICE_TYPE { get; set; }
		[DataMember]
		public string PICK_FLOOR { get; set; }

	}

	/// <summary>
	/// 供P710302儲位統計查詢使用
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ROW_NUM")]
	public class F1912StatisticReport
	{
		[DataMember]
		public decimal ROW_NUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE_NAME { get; set; }
		[DataMember]
		public System.Decimal? LOCCOUNT { get; set; }
		[DataMember]
		public System.Decimal? PERCENTAGE { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROW_NUM")]
	public class F191202Ex
	{
		[DataMember]
		public decimal ROW_NUM { get; set; }
		[DataMember]
		public DateTime TRANS_DATE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string TRANS_STATUS { get; set; }
		[DataMember]
		public string LOC_STATUS_NAME { get; set; }
		[DataMember]
		public string EMP_NAME { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("LOC_TYPE_ID")]
	public class F1942Ex
	{
		[DataMember]
		public string LOC_TYPE_ID { get; set; }
		[DataMember]
		public string LOC_TYPE_NAME { get; set; }
		[DataMember]
		public System.Int32 LENGTH { get; set; }
		[DataMember]
		public System.Int32 DEPTH { get; set; }
		[DataMember]
		public System.Int32 HEIGHT { get; set; }
		[DataMember]
		public System.Decimal WEIGHT { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public System.Decimal VOLUME_RATE { get; set; }
		[DataMember]
		public string HANDY { get; set; }
	}

	/// <summary>
	/// 儲位屬性維護(F1921JoinF1980)用的Data Entity
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "WAREHOUSE_ID", "LOC_CODE")]
	public class F1912WithF1980
	{
		[DataMember]
		public bool IsSelected { get; set; }

		/// <summary>
		/// 物流中心
		/// </summary>
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		/// <summary>
		/// 倉別
		/// </summary>
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		/// <summary>
		/// 儲位編號(F1921)
		/// </summary>
		[DataMember]
		public string LOC_CODE { get; set; }
		/// <summary>
		/// 倉別名稱(F1980)
		/// </summary>
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		/// <summary>
		/// 倉別型態
		/// </summary>
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE_NAME { get; set; }
		/// <summary>
		/// 溫層(01:常溫 02:低溫 03:冷凍)
		/// </summary>
		[DataMember]
		public string TEMP_TYPE { get; set; }
		[DataMember]
		public string TEMP_TYPE_NAME { get; set; }
		/// <summary>
		/// 計庫存
		/// </summary>
		[DataMember]
		public string CAL_STOCK { get; set; }
		/// <summary>
		/// 計費
		/// </summary>
		[DataMember]
		public string CAL_FEE { get; set; }
		/// <summary>
		/// 樓層(F1921)
		/// </summary>
		[DataMember]
		public string FLOOR { get; set; }
		/// <summary>
		/// 儲區型態
		/// </summary>
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		/// <summary>
		/// 通道別
		/// </summary>
		[DataMember]
		public string CHANNEL { get; set; }
		/// <summary>
		/// 座別
		/// </summary>
		[DataMember]
		public string PLAIN { get; set; }
		/// <summary>
		/// 層別
		/// </summary>
		[DataMember]
		public string LOC_LEVEL { get; set; }
		/// <summary>
		/// 儲位別
		/// </summary>
		[DataMember]
		public string LOC_TYPE { get; set; }
		/// <summary>
		/// 儲位性質:1:揀貨儲位　2:暫存儲位
		/// </summary>
		//[DataMember]
		//public string LOC_CHAR { get; set; }
		/// <summary>
		/// 儲位料架編號(F1942)
		/// </summary>
		[DataMember]
		public string LOC_TYPE_ID { get; set; }
		[DataMember]
		public string LOC_TYPE_NAME { get; set; }
		[DataMember]
		public string HANDY { get; set; }
		/// <summary>
		/// 目前儲位狀態代碼(F1943)
		/// </summary>
		[DataMember]
		public string NOW_STATUS_ID { get; set; }
		/// <summary>
		/// 修改前儲位狀態代碼
		/// </summary>
		[DataMember]
		public string PRE_STATUS_ID { get; set; }
		/// <summary>
		/// 異動原因
		/// </summary>
		[DataMember]
		public string UCC_CODE { get; set; }
		/// <summary>
		/// 業主(0:共用)
		/// </summary>
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		/// <summary>
		/// 貨主(0:共用)
		/// </summary>
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		/// <summary>
		/// 水平距離(公尺)
		/// </summary>
		[DataMember]
		public Nullable<decimal> HOR_DISTANCE { get; set; }
		[DataMember]
		public Nullable<DateTime> RENT_BEGIN_DATE { get; set; }
		[DataMember]
		public Nullable<DateTime> RENT_END_DATE { get; set; }
	}

	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F1919Data
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }

		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_Name { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string ATYPE_CODE { get; set; }
		[DataMember]
		public string ATYPE_NAME { get; set; }

		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public string MINCHANNEL { get; set; }
		[DataMember]
		public string MAXCHANNEL { get; set; }
		[DataMember]
		public string MINPLAIN { get; set; }
		[DataMember]
		public string MAXPLAIN { get; set; }
		[DataMember]
		public string MINLOC_LEVEL { get; set; }
		[DataMember]
		public string MAXLOC_LEVEL { get; set; }
		[DataMember]
		public string MINLOC_TYPE { get; set; }
		[DataMember]
		public string MAXLOC_TYPE { get; set; }

		[DataMember]
		public string PICK_TYPE { get; set; }

		[DataMember]
		public string PICK_TYPE_NAME { get; set; }

		[DataMember]
		public string PICK_TOOL { get; set; }

		[DataMember]
		public string PICK_TOOL_NAME { get; set; }

		[DataMember]
		public string PUT_TOOL { get; set; }

		[DataMember]
		public string PUT_TOOL_NAME { get; set; }

		[DataMember]
		public string PICK_SEQ { get; set; }

		[DataMember]
		public string PICK_SEQ_NAME { get; set; }

		[DataMember]
		public string SORT_BY { get; set; }

		[DataMember]
		public string SORT_BY_NAME { get; set; }

		[DataMember]
		public string SINGLE_BOX { get; set; }
		/// <summary>
		/// 單箱計算
		/// </summary>
		[DataMember]
		public string SINGLE_BOX_NAME { get; set; }

		[DataMember]
		public string PICK_CHECK { get; set; }
		/// <summary>
		/// 揀貨稽核
		/// </summary>
		[DataMember]
		public string PICK_CHECK_NAME { get; set; }

		[DataMember]
		public string PICK_UNIT { get; set; }
		/// <summary>
		/// 揀貨單位
		/// </summary>
		[DataMember]
		public string PICK_UNIT_NAME { get; set; }

		[DataMember]
		public string PICK_MARTERIAL { get; set; }
		/// <summary>
		/// 撿貨載具
		/// </summary>
		[DataMember]
		public string PICK_MARTERIAL_NAME { get; set; }

		[DataMember]
		public string DELIVERY_MARTERIAL { get; set; }
		/// <summary>
		/// 出貨資材類型
		/// </summary>
		[DataMember]
		public string DELIVERY_MARTERIAL_NAME { get; set; }

		/// <summary>
		/// 是否建立儲區設定資料
		/// </summary>
		[DataMember]
		public bool IsCreateStorageAreaPickSetting { get; set; }
		/// <summary>
		/// 上架工具
		/// </summary>
		[DataMember]
		public string MOVE_TOOL { get; set; }
		/// <summary>
		/// 上架工具名稱
		/// </summary>
		[DataMember]
		public string MOVE_TOOL_NAME { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE", "CUST_CODE", "ITEM_CODE")]
	public class F1903Data
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string SRC_CUST { get; set; }
		[DataMember]
		public System.Int16? LNG_RTN_TYPE { get; set; }
		[DataMember]
		public System.Int16? SELF_TYPE { get; set; }
		[DataMember]
		public string TRANS_TYPE { get; set; }
		[DataMember]
		public string AGENT_FOR { get; set; }
		[DataMember]
		public System.Int16? OWNER_FLAG { get; set; }
		[DataMember]
		public string PICK_TYPE { get; set; }
		[DataMember]
		public string ITEM_GUP { get; set; }
		[DataMember]
		public System.Int32? PICK_QTY { get; set; }
		[DataMember]
		public System.Int32? MCASE_QTY { get; set; }
		[DataMember]
		public string VEN_UNIT { get; set; }
		[DataMember]
		public string RET_UNIT { get; set; }
		[DataMember]
		public System.Int32? RET_ORD { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
		[DataMember]
		public System.DateTime? OPEN_DATE { get; set; }
		[DataMember]
		public System.DateTime? STOP_DATE { get; set; }
		[DataMember]
		public string LTYPE { get; set; }
		[DataMember]
		public string MTYPE { get; set; }
		[DataMember]
		public string STYPE { get; set; }
		[DataMember]
		public System.Int32? CASE_QTY { get; set; }
		[DataMember]
		public System.Int32? PACK_QTY1 { get; set; }
		[DataMember]
		public System.Int16? RET_DLN { get; set; }
		[DataMember]
		public System.Int16? ALL_DLN { get; set; }
		[DataMember]
		public string SEND_WARE { get; set; }
		[DataMember]
		public string DELV_WARE { get; set; }
		[DataMember]
		public System.Int32? DELV_LIM { get; set; }
		[DataMember]
		public System.Int16? URGEGEN_TYPE { get; set; }
		[DataMember]
		public string TAX_TYPE { get; set; }
		[DataMember]
		public System.Int16? CIREXT_TYPE { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public System.Int16? GAT_SND_TYPE { get; set; }
		[DataMember]
		public string SND_TYPE { get; set; }
		[DataMember]
		public System.Int16? HAND_TYPE { get; set; }
		[DataMember]
		public System.Int16? KEYMAN_TYPE { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public System.Int16? ITM_LEADTIME { get; set; }
		[DataMember]
		public System.Int16? PFACE { get; set; }
		[DataMember]
		public System.Int16? PLEVEL { get; set; }
		[DataMember]
		public System.Int16? LEVEL_BOX { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string C_D_FLAG { get; set; }
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_ENGNAME { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string VEN_ITEM_CODE { get; set; }
		[DataMember]
		public System.Decimal? CARTON_SIZE { get; set; }
		[DataMember]
		public string I_T_FLAG { get; set; }
		[DataMember]
		public string DISTR_TYPE { get; set; }
		[DataMember]
		public System.Decimal? PICK_LIM { get; set; }
		[DataMember]
		public string SPICK_UNIT { get; set; }
		[DataMember]
		public System.Int16? STORE_ALL_DLN { get; set; }
		[DataMember]
		public System.Int16? ALLOW_ALL_DLN { get; set; }
		[DataMember]
		public string FAMILY_CODE { get; set; }
		[DataMember]
		public string SEASONAL { get; set; }
		[DataMember]
		public string VNR_DELV { get; set; }
		[DataMember]
		public string TMPR_FLAG { get; set; }
		[DataMember]
		public string PICK_WAY { get; set; }
		[DataMember]
		public string SND_TYPE_R { get; set; }
		[DataMember]
		public string MULTI_FLAG { get; set; }
		[DataMember]
		public string SND_TYPE_MEMO { get; set; }
		[DataMember]
		public System.Int32? CIREXT_QTY { get; set; }
		[DataMember]
		public string FABRICATE_FLAG { get; set; }
		[DataMember]
		public System.Single? MANPOWER { get; set; }
		[DataMember]
		public System.Single? WORKING_HOUR { get; set; }
		[DataMember]
		public string BUYER { get; set; }
		[DataMember]
		public string OEM_FLAG { get; set; }
		[DataMember]
		public string GUARANTEE_FLAG { get; set; }
		[DataMember]
		public string MOISTUREPROOF_FLAG { get; set; }
		[DataMember]
		public string DESCRIPTION { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public System.Double? HEALTHY_COST { get; set; }
		[DataMember]
		public string BUY_CUT { get; set; }
		[DataMember]
		public string MANUFACTURE { get; set; }
		[DataMember]
		public System.Int16? FRESH_TABLE { get; set; }
		[DataMember]
		public string CARRY_TYPE { get; set; }
		[DataMember]
		public string SPICK_LOC { get; set; }
		[DataMember]
		public string ITEM_TYPE_AES { get; set; }
		[DataMember]
		public string ITEM_KIND { get; set; }
		[DataMember]
		public string ORGITEM_FLAG { get; set; }
		[DataMember]
		public string REN_TYPE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("BARCODE")]
	public class F1912DataLoc
	{
		[DataMember]
		public string AREA { get; set; }
		[DataMember]
		public string LOC { get; set; }
		[DataMember]
		public string BARCODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("LOC_CODE")]
	public class LocVolumnData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public decimal? VOLUMN { get; set; }
	}

	/// <summary>
	/// 新報表-R71010803
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("BARCODE")]
	public class F1912DataLocByLocType
	{
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string LOC { get; set; }
		[DataMember]
		public string BARCODE { get; set; }
		[DataMember]
		public string LOC_CODE_TYPE { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
	}

	/// <summary>
	/// 商品的檢驗項目
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ITEM_CODE", "CHECK_TYPE", "CHECK_NO")]
	public class F190206CheckName
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string CHECK_NO { get; set; }
		[DataMember]
		public string CHECK_TYPE { get; set; }
		[DataMember]
		public string CHECK_NAME { get; set; }
		[DataMember]
		public string UCC_CODE { get; set; }
		[DataMember]
		public string ISPASS { get; set; }
		[DataMember]
		public string MEMO { get; set; }
	}

	/// <summary>
	/// 商品的檢驗項目
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ITEM_CODE", "CHECK_TYPE", "CHECK_NO")]
	public class F190206QuickCheckName
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string CHECK_NO { get; set; }
		[DataMember]
		public string CHECK_TYPE { get; set; }
		[DataMember]
		public string CHECK_NAME { get; set; }
		[DataMember]
		public string UCC_CODE { get; set; }
		[DataMember]
		public string ISPASS { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string PURCHASE_SEQ { get; set; }
	}
	/// <summary>
	/// 商品材積
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("ITEM_CODE", "GUP_CODE")]
	public class F1905Data
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public Double? PACK_WEIGHT { get; set; }
	}

	#region 配送商指定碼頭
	[Serializable]
	[DataServiceKey("DC_CODE", "ALL_ID", "DELV_TIME")]
	public class F1947WithF194701
	{
		public string DC_CODE { get; set; }
		public string ALL_ID { get; set; }
		public string ALL_COMP { get; set; }
		public string PIER_CODE { get; set; }
		public string DELV_TIME { get; set; }
	}
	#endregion
	[Serializable]
	[DataServiceKey("GRP_ID", "SHOWINFO", "FUN_CODE")]
	public class FunctionShowInfo
	{
		public System.Decimal GRP_ID { get; set; }
		public string SHOWINFO { get; set; }
		public string FUN_CODE { get; set; }
	}


	[Serializable]
	[DataServiceKey("FUN_CODE")]
	public class FunctionCodeName
	{
		public string FUN_NAME { get; set; }
		public string FUN_CODE { get; set; }
	}

	/// <summary>
	/// 配送商主檔，包含顯示名稱。
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "ALL_ID")]
	public class F1947Ex
	{
		public string DC_CODE { get; set; }
		public string DC_NAME { get; set; }
		public string ALL_ID { get; set; }
		public string ALL_COMP { get; set; }
		public string PIER_CODE { get; set; }
		public string CHECK_ROUTE { get; set; }
		public string TYPE { get; set; }
		public string TYPENAME { get; set; }
		public string ALLOW_ROUND_PIECE { get; set; }

	}

	/// <summary>
	/// 商品的檢驗項目名稱
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ITEM_CODE", "CHECK_TYPE", "CHECK_NO")]
	public class F190206CheckItemName
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string CHECK_NO { get; set; }
		[DataMember]
		public string CHECK_TYPE { get; set; }
		[DataMember]
		public string CHECK_NAME { get; set; }
	}

	#region P1601020000 使用者被設定的作業區(倉別清單)
	[DataContract]
	[Serializable]
	[DataServiceKey("Value")]
	public class UserWarehouse
	{
		public string DC_CODE { get; set; }
		[DataMember]
		public string Value { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
	}
	#endregion

	#region P1502010000 調撥商品查詢
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F1913WithF1912Moved
	{
		[DataMember]
		public System.Decimal ROWNUM { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public System.Int64? QTY { get; set; }
		[DataMember]
		public System.Decimal? MOVE_QTY { get; set; }
		[DataMember]
		public DateTime? VALID_DATE { get; set; }
		[DataMember]
		public DateTime? ENTER_DATE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
  }


  public class StockQuery
	{
		public string DC_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }

		public decimal QTY { get; set; }

	}
	#endregion

	#region P9103020000 報價單維護
	/// <summary>
	/// 取得該業主下商品編號的名稱與大分類名稱
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE", "ITEM_CODE")]
	public class F1903WithF1915
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string CLA_NAME { get; set; }
	}
	#endregion

	#region P7110010000 貨主單據維護
	/// <summary>
	/// 貨主單據查詢結果
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("TICKET_ID", "MILESTONE_ID")]
	public class F190001Data
	{
		[DataMember]
		public System.Decimal TICKET_ID { get; set; }
		[DataMember]
		public string TICKET_NAME { get; set; }
		[DataMember]
		public string TICKET_TYPE { get; set; }
		[DataMember]
		public string TICKET_CLASS { get; set; }
		[DataMember]
		public string SHIPPING_ASSIGN { get; set; }
		[DataMember]
		public string FAST_DELIVER { get; set; }
		[DataMember]
		public string ASSIGN_DELIVER { get; set; }
		[DataMember]
		public string OUT_TYPE { get; set; }
		[DataMember]
		public System.Int16? PRIORITY { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
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
		public string SHIPPING_ASSIGN_NAME { get; set; }
		[DataMember]
		public string FAST_DELIVER_NAME { get; set; }
		[DataMember]
		public string ALL_COMP { get; set; }
		[DataMember]
		public System.Decimal MILESTONE_ID { get; set; }
		[DataMember]
		public string MILESTONE_NO { get; set; }
		[DataMember]
		public string SORT_NO { get; set; }
		[DataMember]
		public string MILESTONE_NAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("TICKET_TYPE", "TICKET_CLASS")]
	public class F19000103Data
	{
		[DataMember]
		public string TICKET_TYPE { get; set; }
		[DataMember]
		public string TICKET_CLASS { get; set; }
		[DataMember]
		public string MILESTONE_NO_A { get; set; }
		[DataMember]
		public string MILESTONE_NO_B { get; set; }
		[DataMember]
		public string MILESTONE_NO_C { get; set; }
		[DataMember]
		public string MILESTONE_NO_D { get; set; }
		[DataMember]
		public string MILESTONE_NO_E { get; set; }
		[DataMember]
		public string MILESTONE_NO_F { get; set; }
		[DataMember]
		public string MILESTONE_NO_G { get; set; }
		[DataMember]
		public string MILESTONE_NO_H { get; set; }
		[DataMember]
		public string MILESTONE_NO_I { get; set; }
		[DataMember]
		public string MILESTONE_NO_J { get; set; }
		[DataMember]
		public string MILESTONE_NO_K { get; set; }
		[DataMember]
		public string MILESTONE_NO_L { get; set; }
		[DataMember]
		public string MILESTONE_NO_M { get; set; }
		[DataMember]
		public string MILESTONE_NO_N { get; set; }
		[DataMember]
		public string MILESTONE_NO_A_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_B_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_C_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_D_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_E_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_F_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_G_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_H_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_I_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_J_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_K_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_L_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_M_NAME { get; set; }
		[DataMember]
		public string MILESTONE_NO_N_NAME { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public System.DateTime? CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public System.DateTime? UPD_DATE { get; set; }
	}

	/// <summary>
	/// 庫存所在的倉別跟儲位
	/// </summary>
	[DataContract]
	[Serializable]
	[DataServiceKey("ITEM_CODE", "LOC_CODE", "WAREHOUSE_ID", "WAREHOUSE_NAME", "WAREHOUSE_TYPE", "VALID_DATE")]
	public class StockData
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string TYPE_NAME { get; set; }
		[DataMember]
		public System.Decimal? QTY { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
	}

	#endregion

	#region 貨主單據倉別維護

	[Serializable]
	[DataServiceKey("TICKET_ID")]
	public class F190002Data
	{
		public System.Decimal TICKET_ID { get; set; }
		public Int16 SWAREHOUSE { get; set; }
		public Int16 TWAREHOUSE { get; set; }
		public Int16 OWAREHOUSE { get; set; }
		public Int16 BWAREHOUSE { get; set; }
		public Int16 GWAREHOUSE { get; set; }
		public Int16 NWAREHOUSE { get; set; }
		public Int16 WWAREHOUSE { get; set; }
		public Int16 RWAREHOUSE { get; set; }
		public Int16 DWAREHOUSE { get; set; }
		public Int16 MWAREHOUSE { get; set; }
		public Int16 UWAREHOUSE { get; set; }
		public Int16 VWAREHOUSE { get; set; }
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

	#endregion

	#region P2001010000 異動調整作業(訂單,商品,盤點庫存)
	[Serializable]
	[DataContract]
	[DataServiceKey("ROWNUM")]
	public class F1913Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public bool IsSelected { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }

		[DataMember]
		public string BUNDLE_SERIALLOC { get; set; }
		[DataMember]
		public long QTY { get; set; }
		[DataMember]
		public int? ADJ_QTY_IN { get; set; }
		[DataMember]
		public int? ADJ_QTY_OUT { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public string CAUSENAME { get; set; }
		[DataMember]
		public string CAUSE_MEMO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SERIALNO_SCANOK { get; set; }
		[DataMember]
		public string WORK_TYPE { get; set; }
		[DataMember]
		public bool ISADD { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }

		[DataMember]
		public string IS_FLUSHBACK { get; set; }

		[DataMember]
		public string BOX_CTRL_NO { get; set; }

		[DataMember]
		public string PALLET_CTRL_NO { get; set; }

		[DataMember]
		public string MAKE_NO { get; set; }
		/// <summary>
		/// WMS未搬動數量
		/// </summary>
		[DataMember]
		public int UNMOVE_STOCK_QTY { get; set; }
		/// <summary>
		/// 自動倉庫存數量
		/// </summary>
		[DataMember]
		public int DEVICE_STOCK_QTY { get; set; }
		/// <summary>
		/// 盤點差異數 
		/// </summary>
		[DataMember]
		public int DIFF_QTY { get; set; }
		/// <summary>
		/// 實際盤點數 
		/// </summary>
		[DataMember]
		public int INVENTORY_QTY { get; set; }
		/// <summary>
		/// 盤墊庫差數
		/// </summary>
		[DataMember]
		public int STOCK_DIFF_QTY { get; set; }
		/// <summary>
		/// 盤點狀態
		/// </summary>
		[DataMember]
		public string STATUS { get; set; }
		/// <summary>
		/// 盤點時間
		/// </summary>
		[DataMember]
		public DateTime? INVENTORY_DATE { get; set; }
		/// <summary>
		/// 盤點人員
		/// </summary>
		[DataMember]
		public string INVENTORY_NAME { get; set; }
		/// <summary>
		/// WMS調整狀態(0=待調整,1=調整成功,2=調整失敗,3=不調整)
		/// </summary>
		[DataMember]
		public string WMS_STATUS_NAME { get; set; }
		/// <summary>
		/// 人員調整狀態(0=不調整,1=調整)
		/// </summary>
		[DataMember]
		public string PERSON_CONFIRM_STATUS_NAME { get; set; }
		/// <summary>
		/// 處理單號(盤盈:調整單號 盤損:調撥單號)
		/// </summary>
		[DataMember]
		public string PROC_WMS_NO { get; set; }
		/// <summary>
		/// 貨主品編
		/// </summary>
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		/// <summary>
		/// 國條
		/// </summary>
		[DataMember]
		public string EAN_CODE1 { get; set; }
		/// <summary>
		/// 條碼2
		/// </summary>
		[DataMember]
		public string EAN_CODE2 { get; set; }
		/// <summary>
		/// 條碼3
		/// </summary>
		[DataMember]
		public string EAN_CODE3 { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1913DataImport
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string VALID_DATE { get; set; }
		[DataMember]
		public string VALID_DATE_FORMAT { get; set; }
		[DataMember]
		public string ENTER_DATE { get; set; }
		[DataMember]
		public string ENTER_DATE_FORMAT { get; set; }
		[DataMember]
		public string ADJ_QTY_IN { get; set; }
		[DataMember]
		public string ADJ_QTY_OUT { get; set; }
		[DataMember]
		public string CAUSE { get; set; }
		[DataMember]
		public string CAUSENAME { get; set; }
		[DataMember]
		public string CAUSE_MEMO { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string FailMessage { get; set; }
	}

	public class ImportF1913DataResult
	{
		public ExecuteResult Result { get; set; }

		public List<F1913Data> F1913DataItems { get; set; }
		public List<F1913DataImport> F1913DataFailItems { get; set; }
	}

	#endregion

	#region 配庫
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ItemWarehouseStock
	{
		public string WAREHOUSE_ID { get; set; }
		public System.Decimal? SUM_QTY { get; set; }
		public System.Decimal ROWNUM { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ItemLocStock
	{
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public System.DateTime VALID_DATE { get; set; }
		public System.DateTime ENTER_DATE { get; set; }
		public string VNR_CODE { get; set; }
		public string MAKE_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public System.Int64 QTY { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public System.Decimal? ROWNUM { get; set; }

		public string CASE_NO { get; set; }
		public string BOX_SERIAL { get; set; }

		public string BATCH_NO { get; set; }

		public string BOX_CTRL_NO { get; set; }

		public string PALLET_CTRL_NO { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ItemLocPriorityInfo
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public Int64 QTY { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }
		[DataMember]
		public string ATYPE_CODE { get; set; }
		[DataMember]
		public string HANDY { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public string BOX_SERIAL { get; set; }
		[DataMember]
		public string CASE_NO { get; set; }
		[DataMember]
		public string BATCH_NO { get; set; }
		[DataMember]
		public Decimal? ROWNUM { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string PICK_FLOOR { get; set; }
		[DataMember]
		public string DEVICE_TYPE { get; set; }
		[DataMember]
		public string PK_AREA { get; set; }
        [DataMember]
        public string PK_NAME { get; set; }
        [DataMember]
        public string WAREHOUSE_NAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class LocPriorityInfo
	{
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }
		[DataMember]
		public string ATYPE_CODE { get; set; }
		[DataMember]
		public string HANDY { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public Decimal? ROWNUM { get; set; }
    [DataMember]
    public string DC_CODE { get; set; }
    [DataMember]
    public string GUP_CODE { get; set; }
    [DataMember]
    public string CUST_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
    }

	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class MixLocPriorityInfo
	{
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }
		[DataMember]
		public string ATYPE_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public Decimal? ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class NearestLocPriorityInfo
	{
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public string FLOOR { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }

		[DataMember]
		public Decimal? ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
	}

	/// <summary>
	/// 路線
	/// </summary>
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class Route
	{
		public string AllId { get; set; }
		public string RouteCode { get; set; }
		public string RouteName { get; set; }
		public string DelvTimes { get; set; }
		public string ErstNo { get; set; }
		public string ZipCode { get; set; }
		public Decimal ROWNUM { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class DelvTimeArea
	{
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string DELV_TIME { get; set; }
		[DataMember]
		public string DELV_FREQ { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string DELV_TIMES { get; set; }
		[DataMember]
		public Int16 SORT { get; set; }
		[DataMember]
		public Decimal ROWNUM { get; set; }
	}

	[Serializable]
	[DataServiceKey()]
	public class RetailCarPeriod
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DELV_NO { get; set; }
		public string CAR_PERIOD { get; set; }
		public string RETAIL_CODE { get; set; }
	}
	#endregion 配庫

	#region 商品包裝主檔維護
	[Serializable]
	[DataServiceKey("GUP_CODE", "ITEM_CODE", "UNIT_ID")]
	public class F190301Data
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public Int16 UNIT_LEVEL { get; set; }
		[DataMember]
		public string UNIT_ID { get; set; }
		[DataMember]
		public Int32 UNIT_QTY { get; set; }
		[DataMember]
		public Decimal? LENGTH { get; set; }
		[DataMember]
		public Decimal? WIDTH { get; set; }
		[DataMember]
		public Decimal? HIGHT { get; set; }
		[DataMember]
		public Decimal? WEIGHT { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SYS_UNIT { get; set; }
	}
	#endregion

	#region P9103040000 標籤資料

	[Serializable]
	[DataContract]
	[DataServiceKey("LABEL_SEQ", "GUP_CODE", "CUST_CODE")]
	public class F197001Data
	{
		/// <summary>
		/// 全選下的打勾
		/// </summary>
		[DataMember]
		public bool IsChecked { get; set; }
		[DataMember]
		public Int32 LABEL_SEQ { get; set; }
		[DataMember]
		public string LABEL_CODE { get; set; }
		[DataMember]
		public string LABEL_NAME { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
		[DataMember]
		public string VNR_NAME { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string CUST_ITEM_CODE { get; set; }
		[DataMember]
		public string SUGR { get; set; }
		[DataMember]
		public string WARRANTY { get; set; }
		[DataMember]
		public Int16? WARRANTY_S_Y { get; set; }
		[DataMember]
		public string WARRANTY_S_M { get; set; }
		[DataMember]
		public string WARRANTY_Y { get; set; }
		[DataMember]
		public string WARRANTY_M { get; set; }
		[DataMember]
		public Int16? WARRANTY_D { get; set; }
		[DataMember]
		public string OUTSOURCE { get; set; }
		[DataMember]
		public string CHECK_STAFF { get; set; }
		[DataMember]
		public string ITEM_DESC_A { get; set; }
		[DataMember]
		public string ITEM_DESC_B { get; set; }
		[DataMember]
		public string ITEM_DESC_C { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		/// <summary>
		/// 列印張數
		/// </summary>
		[DataMember]
		public int Qty { get; set; }
		/// <summary>
		/// 保固期限
		/// </summary>
		[DataMember]
		public DateTime WarrantyDate { get; set; }
	}
	#endregion

	#region 貨主主檔維護
	[Serializable]
	[DataServiceKey()]
	public class F19000103WithF000906
	{
		public string TICKET_CLASS_NAME { get; set; }
		public string TICKET_TYPE { get; set; }
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
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }
		public System.DateTime? UPD_DATE { get; set; }
	}
	#endregion

	#region 商品召回
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class DistributionData
	{
		public Decimal ROWNUM { get; set; }
		public string COUDIV_ID { get; set; }
		public string COUDIV_NAME { get; set; }
		public string ALL_ID { get; set; }
		public string ALL_COMP { get; set; }
	}
	#endregion

	#region 配送商主檔
	[Serializable]
	[DataServiceKey("DELV_TIME", "COUDIV_ID")]
	public class F194701WithF1934
	{
		public string DELV_TIME { get; set; }
		public string COUDIV_ID { get; set; }
	}
	#endregion

	#region 配送商服務貨主維護

	[DataContract]
	[Serializable]
	[DataServiceKey("ALL_ID", "DC_CODE", "GUP_CODE", "CUST_CODE")]
	public class F194704Data
	{
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
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
		public string ALL_COMP { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string CONSIGN_FORMAT { get; set; }
		[DataMember]
		public string GET_CONSIGN_NO { get; set; }
		[DataMember]
		public string PRINT_CONSIGN { get; set; }
		[DataMember]
		public string PRINTER_TYPE { get; set; }
		[DataMember]
		public string AUTO_PRINT_CONSIGN { get; set; }
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string ADDBOX_GET_CONSIGN_NO { get; set; }

	}

	#endregion

	#region P180101庫存查詢
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class StockQueryData1
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
    [DataMember]
    public string WAREHOUSE_NAME { get; set; }
    [DataMember]
    public string WAREHOUSE_TYPE { get; set; }
    [DataMember]
    public string LOC_CODE { get; set; }
    [DataMember]
    public string ITEM_CODE { get; set; }
    [DataMember]
    public string ITEM_NAME { get; set; }
		[DataMember]
		public string LTYPE { get; set; }
		[DataMember]
		public string LTYPE_NAME { get; set; }
		[DataMember]
		public string MTYPE { get; set; }
		[DataMember]
		public string MTYPE_NAME { get; set; }
		[DataMember]
		public string STYPE { get; set; }
		[DataMember]
		public string STYPE_NAME { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public Int64 QTY { get; set; }
		[DataMember]
		public string NOW_STATUS_ID { get; set; }
		[DataMember]
		public string NOW_STATUS_NAME { get; set; }
		[DataMember]
		public Decimal LeftDay { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public Decimal PICK_SUM_QTY { get; set; }
		[DataMember]
		public Decimal ALLOCATION_SUM_QTY { get; set; }
		[DataMember]
		public Decimal PROCESS_PICK_SUM_QTY { get; set; }
		[DataMember]
		public Decimal RESERVATION_QTY { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }

		[DataMember]
		public Decimal? TOTAL_VOLUME { get; set; }
		[DataMember]
		public Decimal? UNIT { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string VNR_CODE { get; set; }
	}


	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class StockQueryData3
	{
		public Decimal ROWNUM { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string LTYPE { get; set; }
		public string LTYPE_NAME { get; set; }
		public string MTYPE { get; set; }
		public string MTYPE_NAME { get; set; }
		public string STYPE { get; set; }
		public string STYPE_NAME { get; set; }
		public string ITEM_SIZE { get; set; }
		public string ITEM_SPEC { get; set; }
		public string ITEM_COLOR { get; set; }
		public Decimal? QTY { get; set; }
		public Decimal? DELV_QTY { get; set; }
		public Decimal? END_QTY { get; set; }
		public Decimal? RETURN_RATE { get; set; }
		public Decimal? RETURN_DAY { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1909EX
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string SHORT_NAME { get; set; }
		[DataMember]
		public string BOSS { get; set; }
		[DataMember]
		public string CONTACT { get; set; }
		[DataMember]
		public string TEL { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string UNI_FORM { get; set; }
		[DataMember]
		public string ITEM_CONTACT { get; set; }
		[DataMember]
		public string ITEM_TEL { get; set; }
		[DataMember]
		public string ITEM_CEL { get; set; }
		[DataMember]
		public string ITEM_MAIL { get; set; }
		[DataMember]
		public string BILL_CONTACT { get; set; }
		[DataMember]
		public string BILL_TEL { get; set; }
		[DataMember]
		public string BILL_CEL { get; set; }
		[DataMember]
		public string BILL_MAIL { get; set; }
		[DataMember]
		public string CURRENCY { get; set; }
		[DataMember]
		public string PAY_FACTOR { get; set; }
		[DataMember]
		public string PAY_TYPE { get; set; }
		[DataMember]
		public string BANK_CODE { get; set; }
		[DataMember]
		public string BANK_NAME { get; set; }
		[DataMember]
		public string BANK_ACCOUNT { get; set; }
		[DataMember]
		public string ORDER_ADDRESS { get; set; }
		[DataMember]
		public string MIX_LOC_BATCH { get; set; }
		[DataMember]
		public string MIX_LOC_ITEM { get; set; }
		[DataMember]
		public string DC_TRANSFER { get; set; }
		[DataMember]
		public string BOUNDLE_SERIALLOC { get; set; }
		[DataMember]
		public string RTN_DC_CODE { get; set; }
		[DataMember]
		public string OVERAGE { get; set; }
		[DataMember]
		public string SAM_ITEM { get; set; }
		[DataMember]
		public string INSIDER_TRADING { get; set; }
		[DataMember]
		public Int32? INSIDER_TRADING_LIM { get; set; }
		[DataMember]
		public string SPILT_ORDER { get; set; }
		[DataMember]
		public Int32? SPILT_ORDER_LIM { get; set; }
		[DataMember]
		public string B2C_CAN_LACK { get; set; }
		[DataMember]
		public string CAN_FAST { get; set; }
		[DataMember]
		public string INSTEAD_INVO { get; set; }
		[DataMember]
		public string SPILT_INCHECK { get; set; }
		[DataMember]
		public string SPECIAL_IN { get; set; }
		[DataMember]
		public Decimal? CHECK_PERCENT { get; set; }
		[DataMember]
		public string NEED_SEAL { get; set; }
		[DataMember]
		public string DM { get; set; }
		[DataMember]
		public string RIBBON { get; set; }
		[DataMember]
		public DateTime? RIBBON_BEGIN_DATE { get; set; }
		[DataMember]
		public DateTime? RIBBON_END_DATE { get; set; }
		[DataMember]
		public string CUST_BOX { get; set; }
		[DataMember]
		public string SP_BOX { get; set; }
		[DataMember]
		public string SP_BOX_CODE { get; set; }
		[DataMember]
		public DateTime? SPBOX_BEGIN_DATE { get; set; }
		[DataMember]
		public DateTime? SPBOX_END_DATE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public string INVO_ZIP { get; set; }
		[DataMember]
		public string TAX_TYPE { get; set; }
		[DataMember]
		public string INVO_ADDRESS { get; set; }
		[DataMember]
		public Decimal? DUE_DAY { get; set; }
		[DataMember]
		public Int32? INVO_LIM_QTY { get; set; }
		[DataMember]
		public string AUTO_GEN_RTN { get; set; }
		[DataMember]
		public string SYS_CUST_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string GUPSHARE { get; set; }
	}
	#endregion

	#region P180201庫存異常處理
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class StockAbnormalData
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsSelected { get; set; }
		[DataMember]
		public long ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string SRC_WMS_NO { get; set; }
		[DataMember]
		public string SRC_TYPE { get; set; }
		[DataMember]
		public string SRC_TYPE_NAME { get; set; }
		[DataMember]
		public string ALLOCATION_NO { get; set; }
		[DataMember]
		public Int32 ALLOCATION_SEQ { get; set; }
		[DataMember]
		public string SRC_WAREHOUSE_ID { get; set; }
		[DataMember]
		public string SRC_WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string SRC_LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public Int32 QTY { get; set; }
		[DataMember]
		public string PROC_FLAG { get; set; }
		[DataMember]
		public string PROC_FLAG_NAME { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
    [DataMember]
    public string PROC_WMS_NO { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
  }

  public class InventoryData
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string INVENTORY_NO { get; set; }
	}
	#endregion

	#region P7105020000 作業計價

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "ACC_ITEM_KIND_ID", "ORD_TYPE", "ACC_KIND", "ACC_UNIT", "ACC_NUM", "DELV_ACC_TYPE")]
	public class F199002Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ORD_TYPE { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public Int16 ACC_NUM { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public Decimal FEE { get; set; }
		[DataMember]
		public Decimal BASIC_FEE { get; set; }
		[DataMember]
		public Decimal OVER_FEE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public string FEE_DESCRIBE { get; set; }
	}
	#endregion

	#region 儲位計價
	[Serializable]
	[DataServiceKey("DC_CODE", "TMPR_TYPE", "LOC_TYPE_ID", "ACC_UNIT", "ACC_NUM", "ACC_ITEM_KIND_ID")]
	public class F199001Ex
	{
		public string DC_CODE { get; set; }
		public string TMPR_TYPE { get; set; }
		public string LOC_TYPE_ID { get; set; }
		public string ACC_UNIT { get; set; }
		public short ACC_NUM { get; set; }
		public decimal UNIT_FEE { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public System.DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }
		public Nullable<System.DateTime> UPD_DATE { get; set; }
		public string IN_TAX { get; set; }
		public System.Int16 LENGTH { get; set; }
		public System.Int16 DEPTH { get; set; }
		public System.Int16 HEIGHT { get; set; }
		public System.Decimal WEIGHT { get; set; }
		public string STATUS { get; set; }
		public string ITEM_TYPE_ID { get; set; }
		public string ACC_ITEM_KIND_ID { get; set; }
		public string ACC_ITEM_NAME { get; set; }
	}
	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "AREA_CODE", "LOC_CODE")]
	public class F1912LocData
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ZIP_CODE")]
	public class F1934EX
	{
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string ZIP_NAME { get; set; }
		[DataMember]
		public string COUDIV_ID { get; set; }
		[DataMember]
		public int ISCHECKED { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("DELV_TIME", "DELV_EFFIC", "DELV_TMPR")]
	public class F1947JoinF194701
	{
		[DataMember]
		public string DELV_TIME { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string DELV_TMPR_NAME { get; set; }
		[DataMember]
		public string DELV_EFFIC_NAME { get; set; }
		[DataMember]
		public string Sun { get; set; }
		[DataMember]
		public string Mon { get; set; }
		[DataMember]
		public string Tue { get; set; }
		[DataMember]
		public string Wed { get; set; }
		[DataMember]
		public string Thu { get; set; }
		[DataMember]
		public string Fri { get; set; }
		[DataMember]
		public string Sat { get; set; }
		[DataMember]
		public string PAST_TYPE { get; set; }
		[DataMember]
		public string DELV_TIMES { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("ZIP_CODE", "DELV_EFFIC", "DELV_TIME", "DELV_TMPR")]
	public class F19470101Datas
	{
		[DataMember]
		public string ZIP_CODE { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string DELV_TIME { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
	}

	[Serializable]
	[DataServiceKey("DELV_TIME", "DELV_EFFIC", "DELV_TMPR")]
	public class NewF194701WithF1934
	{
		[DataMember]
		public string DELV_TIME { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string DELV_TMPR_NAME { get; set; }
		[DataMember]
		public string DELV_EFFIC_NAME { get; set; }
	}

	#region P7105050000 派車計價設定
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "ACC_ITEM_KIND_ID", "CUST_TYPE", "LOGI_TYPE",
			"ACC_KIND", "DELV_EFFIC", "IS_SPECIAL_CAR", "ACC_UNIT",
			"ACC_NUM", "DELV_ACC_TYPE", "ACC_ITEM_NAME")]
	public class F199005Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string LOGI_TYPE { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public string IS_SPECIAL_CAR { get; set; }
		[DataMember]
		public Decimal? CAR_KIND_ID { get; set; }
		[DataMember]
		public Decimal? ACC_AREA_ID { get; set; }
		[DataMember]
		public string DELV_TMPR { get; set; }
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public Decimal ACC_NUM { get; set; }
		[DataMember]
		public Decimal? MAX_WEIGHT { get; set; }
		[DataMember]
		public Decimal FEE { get; set; }
		[DataMember]
		public Decimal? OVER_VALUE { get; set; }
		[DataMember]
		public Decimal? OVER_UNIT_FEE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string ACC_UNIT_TEXT { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
	}
	#endregion

	#region 出貨計價設定

	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "ACC_ITEM_KIND_ID", "ACC_KIND", "ACC_UNIT", "ACC_NUM", "DELV_ACC_TYPE")]
	public class F199003Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string ACC_ITEM_KIND_ID { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public string ACC_UNIT { get; set; }
		[DataMember]
		public Int16 ACC_NUM { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public Decimal FEE { get; set; }
		[DataMember]
		public Decimal BASIC_FEE { get; set; }
		[DataMember]
		public Decimal OVER_FEE { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string ITEM_TYPE_ID { get; set; }
		[DataMember]
		public string ACC_ITEM_NAME { get; set; }
		[DataMember]
		public string FEE_DESCRIBE { get; set; }
		[DataMember]
		public string DELV_ACC_TYPE_NAME { get; set; }
	}
	#endregion
	[DataContract]
	[Serializable]
	[DataServiceKey("DELV_EFFIC")]
	public class F190102JoinF000904
	{
		[DataMember]
		public string DELV_EFFIC { get; set; }
		[DataMember]
		public string DELV_EFFIC_NAME { get; set; }
	}


	#region 車輛種類
	[DataContract]
	[Serializable]
	[DataServiceKey("CAR_KIND_ID")]
	public class F194702Data
	{
		[DataMember]
		public Decimal CAR_KIND_ID { get; set; }
		[DataMember]
		public string CAR_KIND_NAME { get; set; }
		[DataMember]
		public string CAR_SIZE { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public string TMPR_TYPE_TEXT { get; set; }
	}
	#endregion

	#region P5001010000 專案計價維護
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ACC_PROJECT_NO")]
	public class F199007Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ACC_PROJECT_NO { get; set; }
		[DataMember]
		public string ACC_PROJECT_NAME { get; set; }
		[DataMember]
		public DateTime ENABLE_DATE { get; set; }
		[DataMember]
		public DateTime DISABLE_DATE { get; set; }
		[DataMember]
		public string QUOTE_NO { get; set; }
		[DataMember]
		public string IN_TAX { get; set; }
		[DataMember]
		public Int32 FEE { get; set; }
		[DataMember]
		public string ACC_KIND { get; set; }
		[DataMember]
		public string SERVICES { get; set; }
		[DataMember]
		public string STATUS { get; set; }
		[DataMember]
		public string CRT_STAFF { get; set; }
		[DataMember]
		public string CRT_NAME { get; set; }
		[DataMember]
		public DateTime CRT_DATE { get; set; }
		[DataMember]
		public string UPD_STAFF { get; set; }
		[DataMember]
		public string UPD_NAME { get; set; }
		[DataMember]
		public DateTime? UPD_DATE { get; set; }
		[DataMember]
		public string PROJECT_DATE { get; set; }
	}

	#endregion

	#region 計費區域
	[DataContract]
	[Serializable]
	[DataServiceKey("ACC_AREA_ID")]
	public class F1948Data
	{
		[DataMember]
		public Decimal ACC_AREA_ID { get; set; }
		[DataMember]
		public string ACC_AREA { get; set; }
	}
	#endregion

	#region F19470801JoinF194708
	[DataContract]
	[Serializable]
	[DataServiceKey("ACC_AREA_ID")]
	public class F19470801JoinF194708
	{
		[DataMember]
		public Decimal ACC_AREA_ID { get; set; }
		[DataMember]
		public string ACC_AREA { get; set; }
	}
	#endregion

	#region 配送商計價設定查詢結果
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F194707Ex
	{
		public Decimal ROWNUM { get; set; }
		public string ALL_ID { get; set; }
		public string DC_CODE { get; set; }
		public Decimal ACC_AREA_ID { get; set; }
		public string DELV_EFFIC { get; set; }
		public string DELV_TMPR { get; set; }
		public string CUST_TYPE { get; set; }
		public string LOGI_TYPE { get; set; }
		public string ACC_KIND { get; set; }
		public Decimal ACC_DELVNUM_ID { get; set; }
		public string ACC_TYPE { get; set; }
		public Decimal BASIC_VALUE { get; set; }
		public Decimal MAX_WEIGHT { get; set; }
		public Decimal FEE { get; set; }
		public Decimal? OVER_VALUE { get; set; }
		public Decimal? OVER_UNIT_FEE { get; set; }
		public string CRT_STAFF { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string CRT_NAME { get; set; }
		public string UPD_STAFF { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string UPD_NAME { get; set; }
		public string IN_TAX { get; set; }
		public string STATUS { get; set; }
		public string ALL_COMP { get; set; }
		public string ACC_AREA { get; set; }
		public Int32? NUM { get; set; }
	}
	#endregion

	#region 計算儲位使用容量暫存用
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE", "LOC_CODE")]
	public class F1912LocVolumn
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public decimal? USED_VOLUMN { get; set; }
	}
	#endregion


	#region P140101盤點維護 -盤點倉別
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class InventoryWareHouse
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsSelected { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string FLOOR_BEGIN { get; set; }
		[DataMember]
		public string FLOOR_END { get; set; }
		[DataMember]
		public string CHANNEL_BEGIN { get; set; }
		[DataMember]
		public string CHANNEL_END { get; set; }
		[DataMember]
		public string PLAIN_BEGIN { get; set; }
		[DataMember]
		public string PLAIN_END { get; set; }
	}

	#endregion

	#region P140101 盤點維護
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class WareHouseFloor
	{
		public decimal ROWNUM { get; set; }

		public string WAREHOUSE_ID { get; set; }

		public string FLOOR { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class WareHouseChannel
	{
		public decimal ROWNUM { get; set; }

		public string WAREHOUSE_ID { get; set; }

		public string CHANNEL { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class WareHousePlain
	{
		public decimal ROWNUM { get; set; }

		public string WAREHOUSE_ID { get; set; }

		public string PLAIN { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1913Ex
	{
		public decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }

		public string LOC_CODE { get; set; }

		public string ITEM_CODE { get; set; }

		public DateTime VALID_DATE { get; set; }

		public DateTime ENTER_DATE { get; set; }

		public string WAREHOUSE_ID { get; set; }

		public int QTY { get; set; }
		public int UNMOVE_STOCK_QTY { get; set; }

		public string SERIAL_NO { get; set; }

		public string BOX_CTRL_NO { get; set; }

		public string PALLET_CTRL_NO { get; set; }
		/// <summary>
		/// 批號
		/// </summary>
		public string MAKE_NO { get; set; }

	}
	#endregion

	#region P710705 倉別管理報表
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710705BackWarehouseInventory
	{
		public Decimal ROWNUM { get; set; }
		public string GUP_CODE { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string VNR_CODE { get; set; }
		public string VNR_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public Decimal TOTAL_QTY { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710705MergeExecution
	{
		public Decimal ROWNUM { get; set; }
		public string DC_CODE { get; set; }
		public string DC_NAME { get; set; }
		public string GUP_CODE { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public DateTime VALID_DATE { get; set; }
		public DateTime ENTER_DATE { get; set; }
		public Decimal TOTAL_QTY { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string TAR_LOC_CODE { get; set; }

	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710705Availability
	{
		public Decimal ROWNUM { get; set; }
		public string GUP_CODE { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public Decimal USED_VOLUMN { get; set; }
		public Decimal USEFUL_VOLUMN { get; set; }
		public string RENT_END_DATE { get; set; }
		public string F1912_LOC_CODE { get; set; }
		public string F1913_LOC_CODE { get; set; }
		public string INVENTORYDATE { get; set; }
		public Decimal COUNTF1912 { get; set; }
		public Decimal COUNTF1913 { get; set; }
		public Decimal COUNTF1913NULL { get; set; }
		public Decimal FILLRATE { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710705ChangeDetail
	{
		public Decimal ROWNUM { get; set; }
		public DateTime? UPDATE_DATE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string SRC_LOC_CODE { get; set; }
		public string TAR_LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string ACTION { get; set; }
		public Decimal A_TAR_QTY { get; set; }
		public Decimal SUM_QTY { get; set; }
	}
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P710705WarehouseDetail
	{
		public Decimal ROWNUM { get; set; }
		public string GUP_CODE { get; set; }
		public string GUP_NAME { get; set; }
		public string CUST_CODE { get; set; }
		public string CUST_NAME { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public Decimal TOTAL_QTY { get; set; }
		public DateTime CRT_DATE { get; set; }

	}


	#endregion

	#region 排程 - 黃金揀貨區檢查
	//低週轉商品
	[DataContract]
	[Serializable]
	[DataServiceKey("DC_CODE")]
	public class SchF700501Data
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public int? A_DELV_QTY { get; set; }
		[DataMember]
		public DateTime? DELV_DATE { get; set; }
		[DataMember]
		public int? ORDER_COUNT { get; set; }
		[DataMember]
		public string MEMO { get; set; }
		[DataMember]
		public string MEMO1 { get; set; }
	}

	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class BomSameItem
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string BOM_NO { get; set; }
		[DataMember]
		public string BOM_TYPE { get; set; }
		[DataMember]
		public Int32? BOM_QTY { get; set; }
	}

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P192019Item
	{
		public Decimal ROWNUM { get; set; }
		public string ACODE { get; set; }
		public string ANAME { get; set; }
		public string BCODE { get; set; }
		public string BNAME { get; set; }
		public string CCODE { get; set; }
		public string CNAME { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public string GUP_CODE { get; set; }
		public string CHECK_PERCENT { get; set; }
	}

	[Serializable]
	[DataServiceKey("ITEM_CODE")]
	public class F1903Plus
	{
		public string ITEM_CODE { get; set; }
		public string ITEM_NAME { get; set; }
		public string EAN_CODE1 { get; set; }
		public string ITEM_ENGNAME { get; set; }
		public string ITEM_COLOR { get; set; }
		public string ITEM_SIZE { get; set; }
		public string TYPE { get; set; }
		public Int16? ITEM_HUMIDITY { get; set; }
		public string ITEM_NICKNAME { get; set; }
		public string ITEM_ATTR { get; set; }
		public string ITEM_SPEC { get; set; }
		public string TMPR_TYPE { get; set; }
		public string FRAGILE { get; set; }
		public string SPILL { get; set; }
		public string ITEM_TYPE { get; set; }
		public string ITEM_UNIT { get; set; }
		public string ITEM_CLASS { get; set; }
		public string SIM_SPEC { get; set; }
		public string VIRTUAL_TYPE { get; set; }
		public string CUST_ITEM_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("EMP_ID")]
	public class F1952Ex
	{
		[DataMember]
		public string EMP_ID { get; set; }
		/// <summary>
		/// 最後一次密碼修改的日期
		/// </summary>
		[DataMember]
		public DateTime? LAST_PASSWORD_CHANGED_DATE { get; set; }
		/// <summary>
		/// 上次啟用時間，若為 NULL，表示第一次啟用
		/// </summary>
		[DataMember]
		public DateTime? LAST_ACTIVITY_DATE { get; set; }
		/// <summary>
		/// 1=鎖定，其他為正常
		/// </summary>
		[DataMember]
		public Decimal? STATUS { get; set; }
		/// <summary>
		/// 是否為共用帳號
		/// </summary>
		[DataMember]
		public string ISCOMMON { get; set; }
		/// <summary>
		/// 是否帳號第一次要強制重設密碼
		/// </summary>
		[DataMember]
		[NotMapped]
		public bool IsAccountFirstResetPassword { get; set; }
		/// <summary>
		/// 是否超過密碼有效天數
		/// </summary>
		[DataMember]
		[NotMapped]
		public bool IsOverPasswordValidDays { get; set; }
		/// <summary>
		/// 處理訊息
		/// </summary>
		[DataMember]
		[NotMapped]
		public string Message { get; set; }

	}

	[DataContract]
	[Serializable]
	[DataServiceKey("LOG_ID", "LOGCENTER_ID")]
	public class F1947Data
	{
		[DataMember]
		public string ALL_ID { get; set; }
		[DataMember]
		public string LOG_ID { get; set; }
		[DataMember]
		public string LOGCENTER_ID { get; set; }
		[DataMember]
		public decimal START_NUMBER { get; set; }
		[DataMember]
		public decimal END_NUMBER { get; set; }
		[DataMember]
		public string LOGCENTER_NAME { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey()]
	public class GrpMailMobile
	{
		[DataMember]
		public Decimal GRP_ID { get; set; }
		[DataMember]
		public string EMAIL { get; set; }
		[DataMember]
		public string MOBILE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE")]
	public class SchItemInfo
	{
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string OrganizationCode { get; set; }
		[DataMember]
		public string ItemDesc { get; set; }
		[DataMember]
		public string SIMCardInfo { get; set; }
		[DataMember]
		public string F1903Data { get; set; }
		[DataMember]
		public string ItemMemo { get; set; }

	}

	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE")]
	public class SchItemReturnData
	{
		[DataMember]
		public List<SchItemInfo> ErrorItemList { get; set; }
		[DataMember]
		public List<SchItemInfo> AddItemList { get; set; }
		[DataMember]
		public List<SchItemInfo> EditItemList { get; set; }

	}

	#region Schedule - 根據[物流中心定期異常檢查設定]去找到該異常群組關聯的員工
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1924QueryData
	{
		[DataMember]
		public Decimal ROWNUM { get; set; }
		[DataMember]
		public string EMP_ID { get; set; }
		[DataMember]
		public string EMP_NAME { get; set; }
		[DataMember]
		public string EMAIL { get; set; }
	}

	#endregion

	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "ITEM_CODE")]
	public class StockSettleData
	{
		public DateTime? CAL_DATE { get; set; }
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public Decimal? BEGIN_QTY { get; set; }
		public Decimal? END_QTY { get; set; }
		public Decimal? RECV_QTY { get; set; }
		public Decimal? RTN_QTY { get; set; }
		public Decimal? DELV_QTY { get; set; }
		public Decimal? SRC_QTY { get; set; }
		public Decimal? TAR_QTY { get; set; }
		public Decimal? LEND_QTY { get; set; }
	}

	public class F194707WithF19470801
	{
		public string ALL_ID { get; set; }
		public string DC_CODE { get; set; }
		public Decimal ACC_AREA_ID { get; set; }
		public string DELV_EFFIC { get; set; }
		public string DELV_TMPR { get; set; }
		public string CUST_TYPE { get; set; }
		public string LOGI_TYPE { get; set; }
		public string ACC_KIND { get; set; }
		public Decimal ACC_DELVNUM_ID { get; set; }
		public string ACC_TYPE { get; set; }
		public Decimal BASIC_VALUE { get; set; }
		public Decimal MAX_WEIGHT { get; set; }
		public Decimal FEE { get; set; }
		public Decimal? OVER_VALUE { get; set; }
		public Decimal? OVER_UNIT_FEE { get; set; }
		public string ZIP_CODE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("GUP_CODE", "ITEM_CODE", "UNIT_ID")]
	public class F190301WithF91000302
	{
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string UNIT_ID { get; set; }
		[DataMember]
		public Int32 UNIT_QTY { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
	}

	public class WareHouseTmprTypeByLocCode
	{
		public string DC_CODE { get; set; }
		public string LOC_CODE { get; set; }
		public string TMPR_TYPE { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("BarCode")]
	public class ItemCodePair
	{
		[DataMember]
		public string BarCode { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
	}
	public class SuggestLocItem
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string BOX_CTRL_NO { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("WAREHOUSE_ID")]
	public class WareHouseIdByWareHouseType
	{
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("BOX_PALLET_NO")]
	public class BoxPalletNoData
	{
		[DataMember]
		public string BARCODE { get; set; }
		[DataMember]
		public string BOX_PALLET_NO { get; set; }
	}

	#region 匯總報表資料

	[DataContract]
	[Serializable]
	[DataServiceKey("DELV_DATE")]
	public class P050103ReportData
	{

		[DataMember]
		public DateTime DELV_DATE { get; set; }
		[DataMember]
		public string PICK_TIME { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string PICK_ORD_NO_BARCODE { get; set; }
		[DataMember]
		public Decimal PICK_QTY_SUM { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public string PICK_LOC { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public string ITEM_SIZE { get; set; }
		[DataMember]
		public string ITEM_SPEC { get; set; }
		[DataMember]
		public string ITEM_COLOR { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string AREA_NAME { get; set; }
		[DataMember]
		public string ACC_UNIT_NAME { get; set; }
		[DataMember]
		public string PICK_LOC_BARCODE { get; set; }
		[DataMember]
		public string ITEM_CODE_BARCODE { get; set; }
		[DataMember]
		public string GUP_NAME { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public DateTime VALID_DATE { get; set; }
	}

	#endregion
	#region 批次時段

	[DataContract]
	[Serializable]
	[DataServiceKey("PICK_TIME")]
	public class P050103PickTime
	{
		[DataMember]
		public string PICK_TIME { get; set; }
	}
	#endregion
	#region 匯總報表資料-揀貨單號

	[DataContract]
	[Serializable]
	[DataServiceKey("PICK_ORD_NO")]
	public class P050103PickOrdNo
	{
		[DataMember]
		public string PICK_ORD_NO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("WMS_ORD_NO")]
	public class P050103WmsOrdNo
	{
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
	}
	#endregion

	public class EServiceItem
	{
		public string ESERVICE { get; set; }
		public string ESERVICE_NAME { get; set; }
	}

	#region 檢查庫存量

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class F1913WithF1912Qty
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public string WAREHOUSE_NAME { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public Decimal? QTY { get; set; }

	}


	#endregion

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]

	public class P192019Data
	{
		[DataMember]
		public int ROWNUM { get; set; }
		[DataMember]
		public string GupCode { get; set; }
		[DataMember]
		public string CustCode { get; set; }
		[DataMember]
		public string ClaType { get; set; }
		[DataMember]
		public string ACode { get; set; }
		[DataMember]
		public string BCode { get; set; }
		[DataMember]
		public string CCode { get; set; }
		[DataMember]
		public string ClaName { get; set; }
		[DataMember]
		public decimal? CheckPercent { get; set; }
	}

	public class ItemLimitValidDay
	{
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string RETAIL_CODE { get; set; }
		/// <summary>
		/// 品號
		/// </summary>
		public string ITEM_CODE { get; set; }
		/// <summary>
		/// 效期天數
		/// </summary>
		public int DELIVERY_DAY { get; set; }
	}

	public class ItemUnit
	{
		public string GUP_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public int UNIT_LEVEL { get; set; }
		public int UNIT_QTY { get; set; }
		public string UNIT_ID { get; set; }
		public string UNIT_NAME { get; set; }

		public decimal? LENGTH { get; set; }
		public decimal? WIDTH { get; set; }
		public decimal? HEIGHT { get; set; }
		public decimal? WEIGHT { get; set; }
	}

	#region 車次明細
	[Serializable]
	[DataServiceKey("DC_CODE", "GUP_CODE", "CUST_CODE", "DELV_NO", "RETAIL_CODE")]
	public class F19471601Data
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string DELV_NO { get; set; }
		public string RETAIL_CODE { get; set; }
		public string RETAIL_NAME { get; set; }
		public string ARRIVAL_TIME_S { get; set; }
		public string ARRIVAL_TIME_E { get; set; }
		public string DELV_WAY { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
		public DateTime CRT_DATE { get; set; }
		public string UPD_STAFF { get; set; }
		public string UPD_NAME { get; set; }
		public DateTime? UPD_DATE { get; set; }
		public decimal REGION_FEE { get; set; }
		public decimal OIL_FEE { get; set; }
		public decimal OVERTIME_FEE { get; set; }
		/// <summary>
		/// 分配廠
		/// </summary>
		public string PACK_FIELD { get; set; }
	}
	#endregion
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class ShipLoadingReport
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string DC_NAME { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string CUST_NAME { get; set; }
		[DataMember]
		public string ARRIVAL_DATE { get; set; }
		[DataMember]
		public string CAR_PERIOD { get; set; }
		[DataMember]
		public string CAR_PERIOD_NAME { get; set; }
		[DataMember]
		public string DELV_NO { get; set; }
		[DataMember]
		public string CAR_GUP { get; set; }
		[DataMember]
		public string DRIVER_ID { get; set; }
		[DataMember]
		public string DRIVER_NAME { get; set; }
		[DataMember]
		public string DELV_WAY { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
		[DataMember]
		public string RETAIL_NAME { get; set; }
		[DataMember]
		public string BOX_NUM { get; set; }
		[DataMember]
		public string BOX_NAME { get; set; }
		[DataMember]
		public decimal? BOX_QTY { get; set; }
		[DataMember]
		public string ADDRESS { get; set; }
		[DataMember]
		public string REVERSE_BOX_NAME { get; set; }
		[DataMember]
		public decimal? REVERSE_BOX_QTY { get; set; }

	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class StockInfo
	{
		[DataMember]

		public string DC_CODE { get; set; }
		[DataMember]

		public string GUP_CODE { get; set; }
		[DataMember]

		public string CUST_CODE { get; set; }
		[DataMember]

		public string LOC_CODE { get; set; }
		[DataMember]

		public string ITEM_CODE { get; set; }
		[DataMember]

		public DateTime VALID_DATE { get; set; }
		[DataMember]

		public DateTime ENTER_DATE { get; set; }
		[DataMember]

		public string VNR_CODE { get; set; }
		[DataMember]

		public string SERIAL_NO { get; set; }
		[DataMember]

		public decimal QTY { get; set; }
		[DataMember]

		public string WAREHOUSE_ID { get; set; }
		[DataMember]

		public string CASE_NO { get; set; }
		[DataMember]

		public string BOX_SERIAL { get; set; }
		[DataMember]

		public string BATCH_NO { get; set; }
		[DataMember]

		public string BOX_CTRL_NO { get; set; }
		[DataMember]

		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P180301ImportData
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public bool IsError { get; set; }
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public string LocCode { get; set; }
		[DataMember]
		public DateTime? ValidDate { get; set; }
		[DataMember]
		public DateTime? EnterDate { get; set; }
		[DataMember]
		public string Qty { get; set; }
		[DataMember]
		public string ItemVerification { get; set; }
		[DataMember]
		public string LocVerification { get; set; }
		[DataMember]
		public string LocMixItem { get; set; }
		[DataMember]
		public string MixBatchno { get; set; }
		[DataMember]
		public string WarehouseName { get; set; }
		[DataMember]
		public string WarehouseId { get; set; }
    [DataMember]
    public string MakeNo { get; set; }
	}

	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class RetailDeliverReportDetail
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string ORD_SEQ { get; set; }
		[DataMember]
		public int ORD_QTY { get; set; }
		[DataMember]
		public int A_DELV_QTY { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string ITEM_UNIT { get; set; }
		[DataMember]
		public string SPILT_OUTCHECK { get; set; }
		[DataMember]
		public string PICK_UNIT { get; set; }
		[DataMember]
		public string WMS_ORD_NO { get; set; }
		[DataMember]
		public string RETAIL_CODE { get; set; }
	}


	[DataContract]
	[Serializable]
	[DataServiceKey("CONNECTID")]
	public class F0070LoginData
	{
		[DataMember]
		public bool IsSelected { get; set; }
		/// <summary>
		/// 連線ID
		/// </summary>
		[DataMember]
		public string CONNECTID { get; set; }
		/// <summary>
		/// 登入帳號
		/// </summary>
		[DataMember]
		public string USERNAME { get; set; }
		/// <summary>
		/// 登入主機名稱
		/// </summary>
		[DataMember]
		public string HOSTNAME { get; set; }
		/// <summary>
		/// 解鎖時間
		/// </summary>
		[DataMember]
		public DateTime? UNLOCKTIME { get; set; }
		/// <summary>
		/// 登入時間
		/// </summary>
		[DataMember]
		public DateTime CRT_DATE { get; set; }
	}
	public class ItemBarcode
	{
		public string ITEM_CODE { get; set; }
		public string BARCODE { get; set; }

	}

	/// <summary>
	/// 判斷F1903商品中的分類所屬，是否有設定到F190205的檢驗分類項目中所用的Model
	/// </summary>
	public class ItemTypeCheckData
	{
		/// <summary>
		/// 判斷是否存在
		/// </summary>
		[DataMember]
		public bool IsExist { get; set; }
		/// <summary>
		/// 不存在的資訊
		/// </summary>
		[DataMember]
		public string ErrMessage { get; set; }
	}
	
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P081301StockSumQty
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public decimal QTY { get; set; }
    [DataMember]
    public string SERIAL_NO { get; set; }
  }
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P08130101MoveLoc
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		public string WAREHOUSE_ID { get; set; }
		public string WAREHOUSE_NAME { get; set; }
		public string NOW_STATUS_ID { get; set; }
		public string LOC_STATUS_NAME { get; set; }
	}
	[DataContract]
	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class P08130101Stock
	{
		[DataMember]
		public decimal ROWNUM { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }

		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public string BOX_CTRL_NO { get; set; }
		[DataMember]
		public string PALLET_CTRL_NO { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public decimal QTY { get; set; }
		[DataMember]

		public decimal MOVE_QTY { get; set; }
	}

	/// <summary>
	/// 取得物流中心服務貨主檔_回傳物件
	/// </summary>
	public class GetDcCustRes
	{
		/// <summary>
		/// 物流中心編號
		/// </summary>
		public string DC_CODE { get; set; }
		/// <summary>
		/// 業主編號
		/// </summary>
		public string GUP_CODE { get; set; }
		/// <summary>
		/// 貨主編號
		/// </summary>
		public string CUST_CODE { get; set; }
	}

	public class StockDataByInventory
	{
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public string MAKE_NO { get; set; }
        public string LOC_CODE { get; set; }
        public DateTime ENTER_DATE { get; set; }
        public string BOX_CTRL_NO { get; set; }
        public string PALLET_CTRL_NO { get; set; }
        public int QTY { get; set; }
		public int UNMOVE_STOCK_QTY { get; set; }
	}

	public class StockSnapshotData
	{
		public string WAREHOUSE_ID { get; set; }
		public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
		public string MAKE_NO { get; set; }
		public int QTY { get; set; }
		public int B_PICK_QTY { get; set; }
	}

	public class StockDataByInventoryParam
	{
        public string LOC_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public DateTime VALID_DATE { get; set; }
        public DateTime ENTER_DATE { get; set; }
        public string MAKE_NO { get; set; }
        public string BOX_CTRL_NO { get; set; }
        public string PALLET_CTRL_NO { get; set; }
	}

	public class F190106Data
	{
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


	public class F1903WithF161402
	{
		public string DC_CODE { get; set; }
		public string GUP_CODE { get; set; }
		public string CUST_CODE { get; set; }
		public string ITEM_CODE { get; set; }
		public int RTN_QTY { get; set; }
		public int AUDIT_QTY { get; set; }

	}

    #region 補貨區庫存
    public class ReplensihModel
    {
        public string ItemCode { get; set; }
        public string MakeNo { get; set; }
        public int ReplensihQty { get; set; }
    }
    #endregion

    /// <summary>
    /// P1901920000集貨場清單
    /// </summary>
	[DataContract]
    [Serializable]
    [DataServiceKey("DC_CODE", "COLLECTION_CODE")]
    public class F1945CollectionList
    {
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string COLLECTION_CODE { get; set; }
        [DataMember]
        public string COLLECTION_NAME { get; set; }
        [DataMember]
        public string COLLECTION_TYPE { get; set; }
        [DataMember]
        public string COLLECTION_TYPE_NAME { get; set; }

    }

    [DataContract]
    [Serializable]
    [DataServiceKey("DC_CODE", "CELL_TYPE")]
    /// <summary>
    /// P1901920000集貨格清單
    /// </summary>
    public class F1945CellList
    {
        [DataMember]
        public string DC_CODE { get; set; }
        [DataMember]
        public string CELL_TYPE { get; set; }
        [DataMember]
        public string CELL_NAME { get; set; }
        [DataMember]
        public string CELL_START_CODE { get; set; }
        [DataMember]
        public int CELL_NUM { get; set; }
        [DataMember]
        public string CRT_NAME { get; set; }
        [DataMember]
        public DateTime CRT_DATE { get; set; }
        [DataMember]
        public string UPD_NAME { get; set; }
        [DataMember]
        public DateTime? UPD_DATE { get; set; }
        /// <summary>
        /// 狀態(新增(A):文字綠色 未異動(N):文字黑色 異動(C):文字藍色 待刪除(D):文字紅色)
        /// </summary>
        [DataMember]
        public String MODIFY_MODE { get; set; }

        [DataMember]
        public Boolean IS_SHOW_DELETE_BUTTON { get; set; }

    }

	[Serializable]
	[DataServiceKey("ROWNUM")]
	public class NewItemLocPriorityInfo
	{
		[DataMember]
		public Decimal? ROWNUM { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }

		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string LOC_CODE { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		
		[DataMember]
		public DateTime VALID_DATE { get; set; }
		[DataMember]
		public DateTime ENTER_DATE { get; set; }
		[DataMember]
		public string MAKE_NO { get; set; }
		[DataMember]
		public Int64 QTY { get; set; }
	
		[DataMember]
		public string WAREHOUSE_ID { get; set; }
		[DataMember]
		public Decimal? HOR_DISTANCE { get; set; }
		[DataMember]
		public Decimal? USEFUL_VOLUMN { get; set; }
		[DataMember]
		public Decimal? USED_VOLUMN { get; set; }
		[DataMember]
		public string AREA_CODE { get; set; }
		[DataMember]
		public string WAREHOUSE_TYPE { get; set; }
		[DataMember]
		public string TMPR_TYPE { get; set; }
		[DataMember]
		public string ATYPE_CODE { get; set; }
		[DataMember]
		public string HANDY { get; set; }
	}

    #region 跨庫出貨配庫補貨排程
    [Serializable]
    [DataServiceKey("GUP_CODE", "DC_CODE", "CUST_CODE", "ITEM_CODE")]
    // 商品各區庫存數
    public class F1913ByLocStauts
    {
        public string DC_CODE { get; set; }
        public string GUP_CODE { get; set; }
        public string CUST_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string MAKE_NO { get; set; }
        public string ATYPE_CODE { get; set; }
        public long QTY { get; set; }
    }
    #endregion
}
