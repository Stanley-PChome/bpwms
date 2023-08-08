using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	public class ShipDebitService : BaseService
	{
		/// <summary>
		/// 分揀出貨資訊回報更新
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ShipDebit(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			CommonService commonService = new CommonService();
			// 取得物流中心服務貨主檔
			var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
			dcCustList.ForEach(item =>
			{
				// 新增API Log
				res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WMSSH_SDB, req.DcCode, req.GupCode, req.GupCode, "ImportShipDebitResults", req, () =>
				{
					var result = ImportShipDebitResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
					return result;
				}, true);

				data.Add(new ApiResponse { MsgCode = res.MsgCode, MsgContent = res.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
				if (res.Data != null && res.Data is List<ApiResponse>)
					data.AddRange((List<ApiResponse>)res.Data);

			});
			return res;
		}

		/// <summary>
		/// 分揀出貨資訊回報
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="compareDate"></param>
		/// <returns></returns>
		private ApiResult ImportShipDebitResults(string dcCode,string gupCode, string custCode)
		{
			#region 變數設定
			ApiResult res = new ApiResult { IsSuccessed = true };
			var f060701Repo = new F060701Repository(Schemas.CoreSchema);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			int successCnt = 0;
			#endregion

			// 取得資料
			var executeDatas = f060701Repo.GetDatasByStatus(dcCode, gupCode, custCode).ToList();
			var totalCount = executeDatas.Count();
			var messageList = new List<string>();
			executeDatas.ForEach(f060701=> {
				try
				{
					var wmsTransaction = new WmsTransaction();
					var f050801Repo = new F050801Repository(Schemas.CoreSchema);
					var currf055001 = f055001Repo.GetF055001(dcCode, gupCode, custCode, f060701.SHIP_CODE).FirstOrDefault();
					// 如果包裝頭檔[F055001]不存在
					if (currf055001 == null)
					{
						f060701.STATUS = "2";
						f060701.MSG_CONTENT = string.Format("物流單號{0}不存在", f060701.SHIP_CODE);
					}
					// 如果包裝頭檔的狀態為已稽核[F055001.STATUS=1]
					else if (currf055001.STATUS == "1")
					{
						f060701.STATUS = "2";
						f060701.MSG_CONTENT = string.Format("物流單號{0}已稽核", f060701.SHIP_CODE);
					}
					else
					{
						// 取得出貨單[F050801]
						var f050801 = f050801Repo.GetData(currf055001.WMS_ORD_NO, currf055001.GUP_CODE, currf055001.CUST_CODE, currf055001.DC_CODE);
						// 如果出貨單狀態為已取消[F050801.STATUS=9]
						if (f050801.STATUS == 9)
						{
							f060701.STATUS = "2";
							f060701.MSG_CONTENT = string.Format("物流單號{0}出貨單已取消", f060701.SHIP_CODE);
						}
						// 如果出貨單狀態為未包裝[F050801.STATUS = 0]
						else if (f050801.STATUS == 0 || f050801.STATUS == 1)
						{
							f060701.STATUS = "2";
							f060701.MSG_CONTENT = string.Format("物流單號{0}出貨單未包裝", f060701.SHIP_CODE);
						}
						// 如果出貨單狀態為已扣帳[F050801.STATUS = 5]
						else if (f050801.STATUS == 5)
						{
							f060701.STATUS = "2";
							f060701.MSG_CONTENT = string.Format("物流單號{0}出貨單已出貨", f060701.SHIP_CODE);
						}
						//如果出貨單狀態為已稽核或已扣帳[F050801.STATUS=2 or F050801.STATUS=6]
						else if (f050801.STATUS == 2 || f050801.STATUS == 6)
						{
							var orderService = new OrderService(wmsTransaction);
              var result = orderService.PackageBoxDebit(f050801, currf055001.PACKAGE_BOX_NO, currf055001.PAST_NO, currf055001.WORKSTATION_CODE, f060701.BOX_CODE, 
                f060701.CREATE_TIME, "SORTER", f060701.SORTER_CODE);

              if (!result.IsSuccessed)
							{
								f060701.STATUS = "2";
								f060701.MSG_CONTENT = string.Format("物流單號{0}{1}", f060701.SHIP_CODE, result.Message);
							}
							else
							{
								f060701.STATUS = "1";
								f060701.MSG_CONTENT = string.Format("物流單號{0}稽核成功", f060701.SHIP_CODE);
								successCnt++;
								wmsTransaction.Complete();
							}
						}
					}
					messageList.Add(f060701.MSG_CONTENT);
				}
				catch(Exception ex)
				{
					messageList.Add(ex.Message);
					f060701.STATUS = "2";
					f060701.MSG_CONTENT = ex.Message?.Length > 255 ? ex.Message.Substring(0, 255) : ex.Message;
				}
				finally
				{
					f060701Repo.Update(f060701);
				}
			});

			int failCnt = executeDatas.Count - successCnt;
			res.MsgCode = "10005";
			res.MsgContent = string.Format(_tacService.GetMsg("10005"), "分揀出貨資訊回報", successCnt, failCnt, executeDatas.Count);
			res.Data = messageList;
			res.TotalCnt = executeDatas.Count;
			res.SuccessCnt = successCnt;
			res.FailureCnt = failCnt;

			return res;
		}
	}

}
