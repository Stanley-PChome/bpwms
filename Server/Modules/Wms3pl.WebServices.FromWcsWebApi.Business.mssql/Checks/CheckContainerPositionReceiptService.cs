using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckContainerPositionReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();
		private CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
		private CommonService commonService = new CommonService();

		#region 容器位置回報檢核
		/// <summary>
		/// 檢查上游出貨單號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="receipt"></param>
		public void CheckOriOrderCodeExist(List<ApiResponse> res, string gupCode, ContainerPositionReceiptReq receipt)
		{
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			var f060201s = f060201Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == receipt.DcCode && o.GUP_CODE == gupCode && o.CUST_CODE == receipt.OwnerCode && o.DOC_ID == receipt.OriOrderCode);
			if (!f060201s.Any())
				res.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = "OriOrderCode", MsgCode = "20026", MsgContent = string.Format(tacService.GetMsg("20026"), receipt.OriOrderCode) });
		}

		/// <summary>
		/// 檢核容器總箱數必須大於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="receipt"></param>
		public void CheckBoxTotal(List<ApiResponse> res, string gupCode, ContainerPositionReceiptReq receipt)
		{
			if (receipt.BoxTotal <= 0)
			{
				res.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = "BoxTotal", MsgCode = "20027", MsgContent = tacService.GetMsg("20027") });
			}
		}

		/// <summary>
		/// 容器箱序必須大於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="receipt"></param>
		public void CheckBoxSerial(List<ApiResponse> res, string gupCode, ContainerPositionReceiptReq receipt)
		{
			if (receipt.BoxSerial <= 0)
			{
				res.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = "BoxSerial", MsgCode = "20028", MsgContent = tacService.GetMsg("20028") });
			}
		}

		/// <summary>
		/// 容器箱序不可大於容器總箱數
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="receipt"></param>
		public void CheckBoxSerialExceedBoxTal(List<ApiResponse> res, string gupCode, ContainerPositionReceiptReq receipt)
		{
			if (receipt.BoxSerial > receipt.BoxTotal)
			{
				res.Add(new ApiResponse { No = receipt.ContainerCode, ErrorColumn = "BoxTotal,BoxSerial", MsgCode = "20029", MsgContent = tacService.GetMsg("20029") });
			}
		}
		#endregion
	}
}
