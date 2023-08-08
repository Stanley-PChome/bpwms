using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.Enums
{
	public enum AllocationType
	{
		/// <summary>
		/// 有來源且有目的倉
		/// </summary>
		Both,
		/// <summary>
		/// 只有來源倉，沒有目的倉
		/// </summary>
		NoTarget,
		/// <summary>
		/// 只有目的倉，沒有來源倉
		/// </summary>
		NoSource
	}

	public enum EmptyLocSubjectType
	{
		/// <summary>
		/// 貨主
		/// </summary>
		Cust,
		/// <summary>
		/// 業主
		/// </summary>
		Group,
		/// <summary>
		/// DC
		/// </summary>
		Dc
	}

	/// <summary>
	/// 新竹貨運的託運單 Sequence 名稱
	/// </summary>
	public enum OrdhSequence
	{
		/// <summary>
		/// 非一日二配 134416502~134476501
		/// </summary>
		SEQ_ORDH_TOMORROWDELIVID,
		/// <summary>
		/// 一日二配 814185180~814197179
		/// </summary>
		SEQ_ORDH_TODAYDELIVID,
		/// <summary>
		/// 無單派車 217236500~217239499
		/// </summary>
		SEQ_ORDH_NOHAVEORDELIVID
		//20160606 by patrick 
		////宅配通對每個貨主會有不同的取號區間及客代，所以改由table控制
		///// <summary>
		///// 無單派車 5770928017~5770931017
		///// </summary>
		//SEQ_ORDH_PELICAN
	}


	public enum AdjustQtyType
	{
		/// <summary>
		/// 寫入盤盈數
		/// </summary>
		Profit,
		/// <summary>
		/// 寫入盤損數
		/// </summary>
		Loss
	}

	public enum StockRecoveryType
	{
		/// <summary>
		/// 增加
		/// </summary>
		Add,
		/// <summary>
		/// 減少
		/// </summary>
		Subtract
	}

	public enum ApiLogType
	{
		WMSAPI_DF,
		WMSAPI_OD,
		WMSAPI_WI,
		WMSAPI_CR,
		WMSAPI_VR,
		WMSAPI_RT,
		WMSAPI_PL,
		WMSAPI_PD,
		WMSAPI_PC,
		WMSAPI_PB,
		WMSAPI_VD,
		WMSAPI_TS,
		WMSAPI_SD,
		WMSAPI_SN,
		WMSAPI_FT,
		WMSAPI_F009001,
		WMSAPI_F009002,
		WCSAPI_OW,
		WCSAPI_SDB,
		WCSAPI_IW,
		WCSAPI_IT,
		WCSAPI_IA,
		WCSAPI_SS,
		WCSAPI_ITEM,
		WCSAPI_ITEMSN,
		WCSAPI_F009003,
		WCSAPI_F009004,
		WCSAPI_CRQ,
		WMSSH_PLS,
		WCSAPI_OPA,
		WMSSH_PLO,
		WMSSH_CPO,
		WMSSH_SDB,
		WCSSCH_ITEM,
		WCSSCH_ITEMSN,
		WCSSH_F009005,
        WMSSH_DPR,
		LMSAPI_F000906,
		WCSSH_F009007,
		WCSSH_F009008,
		WCSSH_F009006,
        WCSSRAPI_F009007,
		PDA
	}
}
