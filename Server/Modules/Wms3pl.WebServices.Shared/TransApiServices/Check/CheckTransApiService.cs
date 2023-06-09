﻿using System;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckTransApiService
	{
		private TransApiBaseService _tacService;
		private CommonService _commonService;

		public CheckTransApiService()
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
			{
				res = new ApiResult { IsSuccessed = false, MsgCode = "20051", MsgContent = _tacService.GetMsg("20051") };
			}

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
		public void CheckCustCode(ref ApiResult res, object req)
		{
			// 檢核必填欄位 CustCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "CustCode"))
			{
				res = new ApiResult { IsSuccessed = false, MsgCode = "20052", MsgContent = _tacService.GetMsg("20052") };
			}

			// 檢核貨主編號是否存在 CustCode
			if (res.IsSuccessed)
			{
				string custCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "CustCode"));

				if (!_commonService.CheckCustExist(custCode))
				{
					res = new ApiResult { IsSuccessed = false, MsgCode = "20052", MsgContent = _tacService.GetMsg("20052") };
				}
			}
		}

		/// <summary>
		/// 檢核傳入物件
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckResult(ref ApiResult res, object req)
		{
			// 檢核必填欄位 CustCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "Result"))
			{
				res = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = _tacService.GetMsg("20056") };
			}
		}

		/// <summary>
		/// 檢核儲位編號必填、是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckLocCode(ref ApiResult res, object req)
		{
			// 檢核必填欄位 CustCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "LocCode"))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20116", MsgContent = _tacService.GetMsg("20116") };

			// 檢核貨主編號是否存在 CustCode
			if (res.IsSuccessed)
			{
				string dcCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "DcCode"));
				string locCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "LocCode"));

				if (!_commonService.CheckLocExist(dcCode, locCode))
					res = new ApiResult { IsSuccessed = false, MsgCode = "20116", MsgContent = _tacService.GetMsg("20116") };
			}
		}

		/// <summary>
		/// 檢核交易編號必填
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckTransactionNo(ref ApiResult res, object req)
		{
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "TransactionNo"))
				res = new ApiResult { IsSuccessed = false, MsgCode = "20116", MsgContent = _tacService.GetMsg("20116") };
		}

		/// <summary>
		/// 檢核品號必填、是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="req"></param>
		public void CheckItemCode(ref ApiResult res, object req)
		{
			CommonService commonService = new CommonService();

			// 檢核必填欄位 ItemCode
			if (res.IsSuccessed && !DataCheckHelper.CheckRequireColumn(req, "ItemCode"))
			{
				res = new ApiResult { IsSuccessed = false, MsgCode = "20090", MsgContent = _tacService.GetMsg("20090") };
			}

			// 檢核品號是否存在 ItemCode
			if (res.IsSuccessed)
			{
				string itemCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "ItemCode"));
				string custCode = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, "CustCode"));
				string gupCode = commonService.GetGupCode(custCode);

				if (!_commonService.CheckItemCodeExist(gupCode, custCode, itemCode))
				{
					res = new ApiResult { IsSuccessed = false, MsgCode = "20090", MsgContent = _tacService.GetMsg("20090") };
				}
			}
		}
	}
}
