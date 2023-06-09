using System.Collections.Generic;
using WebServices.WebApi.Common;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Shared.Wcssr.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.TransApiServices;
using System;

namespace Wms3pl.WebServices.Shared.Wcssr.Services
{
	public enum WcssrApiFun
	{
		/// <summary>
		/// 配箱資訊同步
		/// </summary>
		Distributing,
		/// <summary>
		/// 封箱資訊同步
		/// </summary>
		Sealing,
		/// <summary>
		/// 收單驗貨上架
		/// </summary>
		TakingExamining
	}

	public class WcssrBaseService
	{
		public WebApiConnect conn = new WebApiConnect();
		public List<KeyValuePair<string, string>> httpHeaderList = new List<KeyValuePair<string, string>>();
		public WcssrBaseService()
		{
			conn = new WebApiConnect();
			conn.IsUseOAuth2 = false;
			conn.IsDisabledSSLValidate = false;
#if (Ph || Ph_A7)
			conn.IsUseSSL = true;
#endif
			conn.Timeout = TimeSpan.FromSeconds(30);    // Time out超時設定 30秒
			httpHeaderList.Add(new KeyValuePair<string, string>("Authorization", WcssrSetting.ApiAuthToken));
		}

		/// <summary>
		/// 檢查api連線是否成功
		/// </summary>
		/// <returns></returns>
		public ApiResult CheckConn()
		{
			var res = new ApiResult { IsSuccessed = conn.IsConnectSuccess };
			var tacService = new TransApiBaseService();

            if (conn.IsConnectSuccess == false)
            {
                if (conn.IsTimeout == true)
                {
                    res.MsgCode = "99998";
                    res.MsgContent = $"[WCSSR]{tacService.GetMsg("99998")}";
                }
                else
                {
                    res.MsgCode = "99999";
                    res.MsgContent = $"[WCSSR]{tacService.GetMsg("99999")}";
                }
            }
            else
            {
                res.MsgCode = "201";
            }
						res.OriAPIReturnMessage = conn.OriReturnValue;
						res.HttpCode = conn.HttpCode;
						res.HttpContent = conn.HttpContentValue;
						return res;
        }

		/// <summary>
		/// 呼叫WcssrApi
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="req"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public T2 WcssrApiFunc<T1, T2>(T1 req, string url)
		{
			return conn.PostAsync<T1, T2>($"{WcssrSetting.ApiUrl}{url}", req, httpHeaderList).GetAwaiter().GetResult();
		}

		public object WcssrApiFuncTest<T>(T req, WcssrApiFun wcssrApiFun, List<dynamic> sourceDatas = null)
		{
			switch (wcssrApiFun)
			{
				case WcssrApiFun.TakingExamining: // 收單驗貨上架
					conn.IsConnectSuccess = true;
					var param = (RecvItemNotifyReq)Convert.ChangeType(req, typeof(RecvItemNotifyReq));
					var random = new Random();

					if (random.Next(5) % 5 == 0)// 五分之一失敗
					{
						if (random.Next(5) % 5 == 0) // 五分之一失敗
						{
							conn.IsConnectSuccess = false;
							return null;
						}
						else
							return new RecvItemNotifyRes
							{
								Code = 400,
								ErrorData = new List<OrderErrorData>
															{
																	new OrderErrorData
																	{
																			OrderNo = param.OrderNo,
																			ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤一：CCTV向指定NVR Server請求失敗或是其他例外錯誤",
																			ErrorColumn = new List<string> { "WhId", "WorkStationId" }
																	},
																	new OrderErrorData
																	{
																			OrderNo = param.OrderNo,
																			ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤二：CCTV向指定NVR Server發生其他例外錯誤",
																			ErrorColumn = new List<string> { "SkuId" }
																	},

															}

							};
					}
					else// 成功
					{
						return new RecvItemNotifyRes
						{
							Code = 201
						};
					}
				case WcssrApiFun.Distributing: // 配箱資訊同步
					conn.IsConnectSuccess = true;
					var paramByDistributing = (DistibuteInfoAsyncReq)Convert.ChangeType(req, typeof(DistibuteInfoAsyncReq));
					var randomByDistributing = new Random();

					if (randomByDistributing.Next(5) % 5 == 0)// 五分之一失敗
					{
						if (randomByDistributing.Next(5) % 5 == 0) // 五分之一失敗
						{
							conn.IsConnectSuccess = false;
							return null;
						}
						else
							return new DistibuteInfoAsyncRes
							{
								Code = 400,
								ErrorData = new List<OutboundErrorData>
														{
																new OutboundErrorData
																{
																		 OutboundNo = paramByDistributing.OutboundNo,
																		 ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤一：CCTV向指定NVR Server請求失敗或是其他例外錯誤",
																		 ErrorColumn = new List<string> { "WhId", "WorkStationId" }
																},
																new OutboundErrorData
																{
																		OutboundNo =paramByDistributing.OutboundNo,
																		ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤二：CCTV向指定NVR Server發生其他例外錯誤",
																		ErrorColumn = new List<string> { "OperationUserId" }
																},
														},
							};
					}
					else// 成功
					{
						return new DistibuteInfoAsyncRes
						{
							Code = 201,
						};
					}
				case WcssrApiFun.Sealing: // 封箱資訊同步
					conn.IsConnectSuccess = true;
					var paramBySealing = (SealingInfoAsyncReq)Convert.ChangeType(req, typeof(SealingInfoAsyncReq));
					var randomBySealing = new Random();

					if (randomBySealing.Next(5) % 5 == 0)// 五分之一失敗
					{
						if (randomBySealing.Next(5) % 5 == 0) // 五分之一失敗
						{
							conn.IsConnectSuccess = false;
							return null;
						}
						else
							return new SealingInfoAsyncRes
							{
								Code = 400,
								ErrorData = new List<OutboundErrorData>
														{
																new OutboundErrorData
																{
																		OutboundNo = paramBySealing.OutboundNo,
																		ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤一：CCTV向指定NVR Server請求失敗或是其他例外錯誤",
																		ErrorColumn = new List<string> { "ShipNo", "WorkStationId" }
																},
																new OutboundErrorData
																{
																		OutboundNo = paramBySealing.OutboundNo,
																		ErrorMsg = "Transfer Fail: 往後轉送請求失敗。錯誤二：CCTV向指定NVR Server發生其他例外錯誤",
																		ErrorColumn = new List<string> { "OperationUserId"}
																}
														}
							};
					}
					else// 成功
					{
						return new SealingInfoAsyncRes
						{
							Code = 201,
						};
					}
				default:
					return new object();
			}
		}
	}
}
