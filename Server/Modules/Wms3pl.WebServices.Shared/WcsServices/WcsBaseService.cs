using System;
using System.Collections.Generic;
using WebServices.WebApi.Common;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Schedule;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.Wcs.WcsApiConnectSetting;

namespace Wms3pl.WebServices.Shared.WcsService
{
	public class WcsBaseService
	{
		protected TransApiBaseService _tacService = new TransApiBaseService();
		protected WebApiConnect conn = new WebApiConnect();
		protected List<KeyValuePair<string, string>> httpHeaderList = new List<KeyValuePair<string, string>>();
		protected CommonService commonService;
		/// <summary>
		/// 將功能/API/排程產生的Json資料記錄於log中(0:否, 1:是)
		/// </summary>
		protected bool isSaveWcsData = false;
		public WcsBaseService()
		{
			commonService = new CommonService();
			conn = new WebApiConnect();
			conn.IsUseOAuth2 = false;
			conn.IsDisabledSSLValidate = false;
#if (Ph || Ph_A7)
			conn.IsUseSSL = true;
#endif
			conn.MillisecondsTimeout = 500;             // 最小間隔(毫秒)
			conn.Timeout = TimeSpan.FromSeconds(30);    // Time out超時設定 30秒
			httpHeaderList.Add(new KeyValuePair<string, string>("Authorization", WcsSetting.ApiAuthToken));

			// 將功能/API/排程產生的Json資料記錄於log中(0:否, 1:是)
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
			isSaveWcsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";
		}

		protected ApiResult WcsApiFunc<T>(T req, string url)
		{
			var res = new ApiResult { IsSuccessed = true };
			var result = conn.PostAsync<T, WcsResult>(string.Format("{0}{1}", WcsSetting.ApiUrl, url), req, httpHeaderList).GetAwaiter().GetResult();

			if (result == null)
				result = new WcsResult { Code = "99999", Msg = "回傳無資料" };

			if (conn.IsConnectSuccess == false)
			{
				res.IsSuccessed = false;
        if (conn.IsTimeout)
				{
					res.MsgCode = "99998";
					res.MsgContent = _tacService.GetMsg("99998");
				}
				else
				{
					res.MsgCode = "99999";
					res.MsgContent = _tacService.GetMsg("99999");
				}
			}
			else
			{
				res.IsSuccessed = result.Code == "200";
				res.MsgCode = result.Code;
				res.MsgContent = result.Msg;
				res.Data = result.Data;
			}
      res.OriAPIReturnMessage = conn.OriReturnValue;
			res.HttpCode = conn.HttpCode;
			res.HttpContent = conn.HttpContentValue;
			return res;
		}

		protected ApiResult WcsApiFuncTest<T>(T req, string url)
		{
			var res = new ApiResult { IsSuccessed = true };
			switch (url)
			{
#region 人員資訊
				case "User":
					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Input Error";
					//res.Data = new List<WcsUserResData>
					//{
					//    new WcsUserResData
					//    {
					//        UserId = "US20123654",
					//        ErrorColumn = "UserId"
					//    }
					//};

					// 派發AGV失敗
					res.IsSuccessed = false;
					res.MsgCode = "400-002";
					res.MsgContent = "Dispatch Error";
					res.Data = new List<WcsUserResData>
										{
												new WcsUserResData
												{
														UserId = "Tester26",
														errors = new List<WcsErrorModel>
														{
																new WcsErrorModel
																{
																		code = "GKP-IPS-E00041",
																		message = "Invalid status"
																}
														}
												}
										};
					break;
#endregion

#region 商品主檔同步
				case "ItemAsync":
					var random = new Random();
					var num = random.Next(0, 2);
					if (num == 0)
					{
						// 成功
						res.IsSuccessed = true;
						res.MsgCode = "200";
						res.MsgContent = "Success";
					}
					else if(num == 1)
					{
						if (random.Next(0, 2) == 0)
						{
							// 失敗
							res.IsSuccessed = false;
							res.MsgCode = "400";
							res.MsgContent = "Execute Error";
							res.Data = new List<WcsItemResData>
							{
								new WcsItemResData
								{
									SkuCode = "KK005",
									ErrorColumn = "SkuCode",
									errors = new List<WcsErrorModel>
									{
										new WcsErrorModel
										{
											code = "GKP-IPS-E00041",
											message = "Invalid status"
										}
									}
								}
							};
						}
						else
						{
							// 派發AGV失敗
							res.IsSuccessed = false;
							res.MsgCode = "400-002";
							res.MsgContent = "Dispatch Error";
							res.Data = new List<WcsItemResData>
											{
													new WcsItemResData
													{
															SkuCode = "KK005",
															errors = new List<WcsErrorModel>
															{
																	new WcsErrorModel
																	{
																			code = "GKP-IPS-E00041",
																			message = "Invalid status"
																	}
															}
													}
											};
						}

					}
					
					break;
#endregion

#region 商品序號主檔
				case "ItemSnAsync":

					// 成功
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";
					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Input Error";
					//res.Data = new List<WcsSnResData>
					//{
					//    new WcsSnResData
					//    {
					//        SnCode = "se123dsfa66d8f456d",
					//        ErrorColumn = "SkuCode"
					//    }
					//};

					// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsSnResData>
					//					{
					//							new WcsSnResData
					//							{
					//									SnCode = "se123dsfa66d8f456d",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
#endregion

#region 入庫任務
				case "Inbound":
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";

					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Execute Error";
					//res.Data = new List<WcsInboundResData>
					//{
					//    new WcsInboundResData {
					//        ReceiptCode = "T20210127000009",
					//        ErrorColumn = "ReceiptType"
					//    }
					//};

					// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsInboundResData>
					//					{
					//							new WcsInboundResData
					//							{
					//									ReceiptCode = "T20210127000009",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
        #endregion

        #region 入庫任務取消
        case "InboundCancel":
          var rn = new Random();
          var r = rn.Next(0, 3);
          if (r == 0) // 可成功取消
          {
            res.IsSuccessed = true;
            res.MsgCode = "200";
            res.MsgContent = "Success";
            res.Data = new WcsInboundCancelResData
            {
              ReceiptCode = (req as WcsInboundCancelReq).ReceiptCode,
              Status = r.ToString(),
              CompleteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
          }
          else if (r == 1) //不可成功取消
          {
            res.IsSuccessed = true;
            res.MsgCode = "200";
            res.MsgContent = "Success";
            res.Data = new WcsInboundCancelResData
            {
              ReceiptCode = (req as WcsInboundCancelReq).ReceiptCode,
              Status = r.ToString(),
              CompleteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
          }
          else
          {
            // 失敗
            res.IsSuccessed = false;
            res.MsgCode = "400-002";
            res.MsgContent = "Dispatch Error";
            res.Data = new List<WcsInboundCancelResData>
            {
              new WcsInboundCancelResData
              {
                ReceiptCode = (req as WcsInboundCancelReq).ReceiptCode,
                Status = "1",
                CompleteTime = DateTime.Now.ToString(),
                ErrorMsg = "收貨單不存在"
              }
            };
          }
          // 成功
          //res.IsSuccessed = true;
          //res.MsgCode = "200";
          //res.MsgContent = "Success";
          //res.Data = new List<WcsInboundCancelResData>
          //{
          //    new WcsInboundCancelResData {
          //        ReceiptCode = "T20210503000001",
          //        Status = 0,
          //        CompleteTime = Convert.ToDateTime("2021/05/03 09:21:02")
          //    }
          //};



          break;
#endregion

#region 出庫任務
				case "Outbound":
					// 成功
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";

					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Execute Error";
					//res.Data = new List<WcsOutboundResData>
					//{
					//    new WcsOutboundResData {
					//        OrderCode = "O20210503000001",
					//        ErrorColumn = "OrderCode"
					//    }
					//};

					// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsOutboundResData>
					//					{
					//							new WcsOutboundResData
					//							{
					//									OrderCode = "O20210503000001",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
#endregion

#region 出庫任務取消
				case "OutboundCancel":
          // 成功
          var rnByCancel = new Random();
          
          if (rnByCancel.Next(0, 2) == 0)
          {
            res.IsSuccessed = true;
            res.MsgCode = "200";
            res.MsgContent = "Success";
          }
          else
          {
            res.IsSuccessed = false;
            res.MsgCode = "400-002";
            res.MsgContent = "Dispatch Error";
            res.Data = new List<WcsOutboundCancelResData>
            {
              new WcsOutboundCancelResData
              {
                //OrderCode = " O20210503000001",
                OrderCode = (req as WcsOutboundCancelReq).OrderCode,
                Status = "1",
                //CompleteTime = Convert.ToDateTime("2021/05/03 09:21:02"),
                 CompleteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                ErrorMsg = "訂單已完成"
              }
            };
          }

          //res.Data = new List<WcsOutboundCancelResData>
          //{
          //    new WcsOutboundCancelResData {
          //        OrderCode = "O20210503000001",
          //        Status = 0,
          //        CompleteTime = Convert.ToDateTime("2021/05/03 09:21:02")
          //    }
          //};

          // 失敗
          //res.IsSuccessed = false;
          //res.MsgCode = "400-002";
          //res.MsgContent = "Dispatch Error";
          //res.Data = new List<WcsOutboundCancelResData>
          //					{
          //							new WcsOutboundCancelResData {
          //									OrderCode = " O20210503000001",
          //									Status = 1,
          //									CompleteTime = Convert.ToDateTime("2021/05/03 09:21:02"),
          //									ErrorMsg = "訂單已完成"
          //							}
          //					};
          break;
#endregion

#region 容器釋放
				case "Container":
					// 成功
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";

					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Execute Error";
					//res.Data = new List<WcsContainerResData>
					//{
					//		new WcsContainerResData {
					//				ContainerCode = "Z01",
					//				ErrorColumn = "ContainerCode"
					//		}
					//};

					// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsContainerResData>
					//					{
					//							new WcsContainerResData
					//							{
					//									ContainerCode = "Z01",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
#endregion

#region 盤點任務
				case "StockCheck":
					// 成功
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";

					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Execute Error";
					//res.Data = new List<WcsInventoryResData>
					//{
					//		new WcsInventoryResData {
					//				CheckCode = "I20210916000001",
					//				ErrorColumn = "ContainerCode"
					//		}
					//};

					//// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsInventoryResData>
					//					{
					//							new WcsInventoryResData
					//							{
					//									CheckCode = "I20210916000001",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
#endregion

#region 盤點調整任務
				case "StockCheckAdjustment":
					// 成功
					res.IsSuccessed = true;
					res.MsgCode = "200";
					res.MsgContent = "Success";

					// 失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-001";
					//res.MsgContent = "Execute Error";
					//res.Data = new List<WcsStockCheckAdjustmentResData>
					//{
					//		new WcsStockCheckAdjustmentResData {
					//				AdjustCode = "I20210916000001",
					//				ErrorColumn = "ContainerCode"
					//		}
					//};

					//// 派發AGV失敗
					//res.IsSuccessed = false;
					//res.MsgCode = "400-002";
					//res.MsgContent = "Dispatch Error";
					//res.Data = new List<WcsStockCheckAdjustmentResData>
					//					{
					//							new WcsStockCheckAdjustmentResData
					//							{
					//									AdjustCode = "I20210916000001",
					//									errors = new List<WcsErrorModel>
					//									{
					//											new WcsErrorModel
					//											{
					//													code = "GKP-IPS-E00041",
					//													message = "Invalid status"
					//											}
					//									}
					//							}
					//					};
					break;
                #endregion

                #region 集貨等待通知
                case "Collection":
                    // 成功
                    res.IsSuccessed = true;
                    res.MsgCode = "200";
                    res.MsgContent = "Success";
                    break;

                #endregion

				#region 工作站狀態同步
				case "Station":

                    if (new Random().Next(0, 5) == 0)
                    {
                        //失敗
                        res.IsSuccessed = false;
                        res.MsgCode = "400-001";
                        res.MsgContent = "Execute Error";
                        res.Data = new List<WcsStationResData>
                        {
                                new WcsStationResData {
                                        StationCode = "AG001",
                                        ErrorColumn = "StationCode"
                                }
                        };
                    }
                    else
                    {
                        res.IsSuccessed = true;
                        res.MsgCode = "200";
                        res.MsgContent = "Success";
                    }
					break;
					#endregion
			}
			return res;
		}

		/// <summary>
		/// 排程最大重派次數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public int GetMidApiRelmt()
		{
			return Convert.ToInt32(commonService.GetSysGlobalValue( "MIDAPIRELMT"));
		}

		/// <summary>
		/// 排程每次執行最大單據數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public int GetMidApiMisMax()
		{
			return Convert.ToInt32(commonService.GetSysGlobalValue( "MIDAPIMISMAX"));
		}
	}
}
