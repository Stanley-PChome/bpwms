using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.Enums
{
	/// <summary>
	/// 刪除調撥單方式
	/// </summary>
	public enum DeleteAllocationType
	{
		/// <summary>
		/// 調撥單號
		/// </summary>
		AllocationNo,
		/// <summary>
		/// 來源單號
		/// </summary>
		SourceNo,
	}

	/// <summary>
	/// 分配方式
	/// </summary>
	public enum ShareUnitQtyType
	{
		/// <summary>
		/// 箱號
		/// </summary>
		Case,
		/// <summary>
		/// 盒號(無箱號)
		/// </summary>
		BoxNoCase,
		/// <summary>
		/// 盒號(有箱號)
		/// </summary>
		BoxHasCase,
		/// <summary>
		/// 儲值卡盒號
		/// </summary>
		BatchBox,
		/// <summary>
		/// 散裝
		/// </summary>
		Bulk
	}

	/// <summary>
	/// 建議儲位取得方式
	/// </summary>
	public enum GetSuggestLocType
	{
		/// <summary>
		/// 無序號
		/// </summary>
		None,
		/// <summary>
		/// 箱號
		/// </summary>
		CaseNo,
		/// <summary>
		/// 盒號
		/// </summary>
		BoxNo,
		/// <summary>
		/// 儲值卡盒號
		/// </summary>
		BatchNo,
		/// <summary>
		/// 序號
		/// </summary>
		SerialNo
	}
}
