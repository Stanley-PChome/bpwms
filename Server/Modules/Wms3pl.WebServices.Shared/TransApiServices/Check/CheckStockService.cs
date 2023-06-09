using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckStockService
	{
		private TransApiBaseService _tacService;

		public CheckStockService()
		{
			_tacService = new TransApiBaseService();
		}

		#region 檢核倉別庫存資料
		/// <summary>
		/// 檢核搜尋條件是否為(0: 品號、1: 廠商編號)
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckSearchRule(ref ApiResult res, string searchRule)
		{
			var searchRuleList = new List<string> { "0", "1" };
			if (!searchRuleList.Contains(searchRule))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20114", MsgContent = _tacService.GetMsg("20114") };
		}

		/// <summary>
		/// 檢核搜尋清單(品號、廠商編號)是否有資料
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckCodeList(ref ApiResult res, List<string> codeList)
		{
			if (codeList == null || (codeList != null && !codeList.Any()))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20115", MsgContent = _tacService.GetMsg("20115") };
		}
		#endregion

	}
}
