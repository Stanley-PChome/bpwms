using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckUserService
	{
		private TransApiBaseService _tacService;
		private CommonService _commonService;

		public CheckUserService()
		{
			_tacService = new TransApiBaseService();
			_commonService = new CommonService();
		}

		/// <summary>
		/// 檢核物流中心必填、是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckDcCode(ref ApiResult res, object req)
		{
			// 檢核必填欄位 DcCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "DcCode"))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };

			// 檢核物流中心是否存在 DcCode
			if (res.IsSuccessed)
			{
				string dcCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "DcCode"));

				if (!_commonService.CheckDcExist(dcCode))
				{
					res = new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };
				}
			}
		}


		/// <summary>
		/// 檢核貨主編號必填、是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckOwnerCode(ref ApiResult res, object req)
		{
			// 檢核必填欄位 OwnerCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "OwnerCode"))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20097", MsgContent = _tacService.GetMsg("20097") };

			// 檢核貨主編號是否存在 OwnerCode
			if (res.IsSuccessed)
			{
				string custCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "OwnerCode"));

				if (!_commonService.CheckCustExist(custCode))
				{
					res = new ApiResult { IsSuccessed = false, MsgCode = "20097", MsgContent = _tacService.GetMsg("20097") };
				}
			}
		}

		/// <summary>
		/// 檢查UserType是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="item"></param>
		public void CheckUserType(List<ApiResponse> res, PostUserDataResult item, List<string> notExistF1925)
		{
			var f1925Repo = new F1925Repository(Schemas.CoreSchema);
			var result = f1925Repo.GetF1925Datas(item.UserType,string.Empty).FirstOrDefault();

			if (result == null)
			{
				notExistF1925.Add(item.UserType);
			}
		}
		
		public void CheckUserGroup(List<ApiResponse> res, List<string> existF1953,List<string> notExistF1953, PostUserDataResult item)
		{
			var f1953Repo = new F1953Repository(Schemas.CoreSchema);
			if (!string.IsNullOrWhiteSpace(item.UserGroup))
			{
				List<string> userGroups = item.UserGroup.Split(',').ToList();
				var f1953s = f1953Repo.GetF1953DataByGrpId(userGroups).Select(x => x.GRP_ID.ToString());
				existF1953.AddRange(f1953s.ToList());
				notExistF1953.AddRange(userGroups.Except(f1953s));
			}
		}

	}
}
