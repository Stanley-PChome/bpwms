using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810103Service
	{
		protected WmsTransaction _wmsTransation;
		public P810103Service(WmsTransaction wmsTransation = null)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 調撥單據查詢
		/// </summary>
		/// <param name="getAllocReq"></param>
		/// <returns></returns>
		public ApiResult GetAlloc(GetAllocReq getAllocReq, string gupCode)
		{
			var p81Service = new P81Service();
			var f02020107Repo = new F02020107Repository(Schemas.CoreSchema);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

            // 傳入參數轉大寫
            if (!string.IsNullOrWhiteSpace(getAllocReq.WmsNo))
                getAllocReq.WmsNo = getAllocReq.WmsNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(getAllocReq.ItemNo))
                getAllocReq.ItemNo = getAllocReq.ItemNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(getAllocReq.PalletNo))
                getAllocReq.PalletNo = getAllocReq.PalletNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(getAllocReq.SerialNo))
                getAllocReq.SerialNo = getAllocReq.SerialNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(getAllocReq.ContainerCode))
                getAllocReq.ContainerCode = getAllocReq.ContainerCode.ToUpper();

            #region 資料檢核

            // 帳號檢核
            var accData = p81Service.CheckAcc(getAllocReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(getAllocReq.FuncNo, getAllocReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(getAllocReq.CustNo, getAllocReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(getAllocReq.DcNo, getAllocReq.AccNo);

			// 

			// 單據類別
			List<string> allocTypeList = new List<string> { "01", "02", "03" };

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(getAllocReq.FuncNo) ||
					string.IsNullOrWhiteSpace(getAllocReq.AccNo) ||
					string.IsNullOrWhiteSpace(getAllocReq.DcNo) ||
					string.IsNullOrWhiteSpace(getAllocReq.CustNo) ||
					string.IsNullOrWhiteSpace(getAllocReq.AllocType) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0 ||
					!allocTypeList.Contains(getAllocReq.AllocType))
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}

			#endregion

			#region 資料處理
			List<string> allocationNos = new List<string>();

			if (result.IsSuccessed)
			{
				if (!string.IsNullOrWhiteSpace(getAllocReq.WmsNo) && !getAllocReq.WmsNo.StartsWith("T"))
				{
					// 若 WmsNo is not null 且 WmsNo 第一碼不是T，走資料處理1
					allocationNos = f02020107Repo.GetAllocationNoDatas(getAllocReq.DcNo, getAllocReq.CustNo, gupCode, getAllocReq.WmsNo).ToList();

					// 若撈取資料失敗，則回傳失敗訊息代碼20351(取得調撥單據資料失敗)
					if (allocationNos.Count == 0)
					{
						result = new ApiResult { IsSuccessed = false, MsgCode = "20351", MsgContent = p81Service.GetMsg("20351") };
					}
				}
				else if (!string.IsNullOrWhiteSpace(getAllocReq.WmsNo))
				{
					allocationNos.Add(getAllocReq.WmsNo);
				}

				if (!string.IsNullOrWhiteSpace(getAllocReq.ContainerCode))
				{
					var allocNos = p81Service.GetWmsNoByContainerCode(getAllocReq.ContainerCode).Select(x => x.WMS_NO).ToList();
					if (allocNos.Any())
						allocationNos.AddRange(allocNos);
					else
						result = new ApiResult { IsSuccessed = false, MsgCode = "20353", MsgContent = string.Format(p81Service.GetMsg("20353"), getAllocReq.ContainerCode) };
				}
			}

			if (result.IsSuccessed)
			{
                if (getAllocReq.AllocType == "01")// 進倉上架
                {
                    result.Data = f151001Repo.GetP810103DataByInbound(
                            getAllocReq.DcNo,
                            getAllocReq.CustNo,
                            gupCode,
                            getAllocReq.AllocDate,
                            getAllocReq.WmsNo,
                            getAllocReq.ItemNo,
                            getAllocReq.PalletNo,
                            getAllocReq.SerialNo,
                            allocationNos);
                }
                else // 調撥下架、調撥上架
                {
                    result.Data = f151001Repo.GetP810103Data(
                            getAllocReq.DcNo,
                            getAllocReq.CustNo,
                            gupCode,
                            getAllocReq.AllocType,
                            getAllocReq.AllocDate,
                            getAllocReq.WmsNo,
                            getAllocReq.ItemNo,
                            getAllocReq.PalletNo,
                            getAllocReq.SerialNo,
                            allocationNos);
                }
			}

			#endregion

			return result;
		}

		/// <summary>
		/// 調撥單據檢核
		/// </summary>
		/// <param name="postAllocCheckReq"></param>
		/// <returns></returns>
		public ApiResult PostAllocCheck(Datas.Shared.Pda.Entitues.PostAllocCheckReq postAllocCheckReq, string gupCode)
		{
			P81Service p81Service = new P81Service();
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);

			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(postAllocCheckReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(postAllocCheckReq.FuncNo, postAllocCheckReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(postAllocCheckReq.CustNo, postAllocCheckReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(postAllocCheckReq.DcNo, postAllocCheckReq.AccNo);

			// 單據類別
			List<string> allocTypeList = new List<string> { "01", "02", "03" };

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(postAllocCheckReq.FuncNo) ||
					string.IsNullOrWhiteSpace(postAllocCheckReq.AccNo) ||
					string.IsNullOrWhiteSpace(postAllocCheckReq.DcNo) ||
					string.IsNullOrWhiteSpace(postAllocCheckReq.CustNo) ||
					string.IsNullOrWhiteSpace(postAllocCheckReq.AllocNo) ||
					string.IsNullOrWhiteSpace(postAllocCheckReq.AllocType) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0 ||
					!allocTypeList.Contains(postAllocCheckReq.AllocType))
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}

			#endregion

			#region 資料處理
			if (result.IsSuccessed)
			{
				// 取得調撥單主檔資料
				F151001 data = f151001Repo.GetSingleData(
						postAllocCheckReq.DcNo,
						postAllocCheckReq.CustNo,
						gupCode,
						postAllocCheckReq.AllocNo);

				if (data != null)
				{
					// 預設先不通過
					result = new ApiResult { IsSuccessed = false, MsgCode = "20070", MsgContent = p81Service.GetMsg("20070") };

					// 2.若AllocType in (01, 03)
					if (postAllocCheckReq.AllocType == "01" || postAllocCheckReq.AllocType == "03")
					{
						//   若 status = 3(已下架處理)，then單據檢核通過
						//   若 status = 4(上架處理中)
						//   若upd_staff = AccNo， then單據檢核通過
						//   Else，&取得訊息內容[30001, upd_name]”(更換人員訊息)
						//   若 status = 5(上架完成)，then & 取得訊息內容[20066](單據已完成)，單據檢核不通過
						if (data.STATUS == "3")
						{
							result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };
						}
						else if (data.STATUS == "4")
						{
							if (data.TAR_MOVE_STAFF == postAllocCheckReq.AccNo)
							{
								result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };
							}
							else
							{
								result = new ApiResult { IsSuccessed = false, MsgCode = "30001", MsgContent = p81Service.GetMsg("30001").Replace("(%s)", !string.IsNullOrWhiteSpace(data.TAR_MOVE_NAME) ? $"({data.TAR_MOVE_NAME})" : string.Empty) };
							}
						}
						else if (data.STATUS == "5")
						{
							result = new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = p81Service.GetMsg("20066") };
						}
						else
						{
							result = new ApiResult { IsSuccessed = false, MsgCode = "20070", MsgContent = p81Service.GetMsg("20070") };
						}
					}
					// 3.若AllocType in (02)
					else if (postAllocCheckReq.AllocType == "02")
					{
						//   若 status = 0(待處理)，then單據檢核通過
						//   若 status = 2(下架處理中)，
						//   若upd_staff = AccNo， then單據檢核通過
						//   Else，&取得訊息內容[30001, upd_name]”(更換人員訊息)
						//   若 status = 3(已下架處理)，then & 取得訊息內容[20066](單據已完成)，單據檢核不通過
						if (data.STATUS == "0")
						{
							result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };
						}
						else if (data.STATUS == "2")
						{
							if (data.SRC_MOVE_STAFF == postAllocCheckReq.AccNo)
							{
								result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };
							}
							else
							{
								result = new ApiResult { IsSuccessed = false, MsgCode = "30001", MsgContent = p81Service.GetMsg("30001").Replace("(%s)", !string.IsNullOrWhiteSpace(data.SRC_MOVE_NAME) ? $"({data.SRC_MOVE_NAME})" : string.Empty) };
							}
						}
						else if (data.STATUS == "3")
						{
							result = new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = p81Service.GetMsg("20066") };
						}
						else
						{
							result = new ApiResult { IsSuccessed = false, MsgCode = "20070", MsgContent = p81Service.GetMsg("20070") };
						}
					}
				}
				else
				{
					result = new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = p81Service.GetMsg("20064") };
				}
			}
			#endregion

			return result;
		}

		/// <summary>
		/// 調撥單據更新
		/// </summary>
		/// <param name="postAllocUpdateReq"></param>
		/// <returns></returns>
		public ApiResult PostAllocUpdate(PostAllocUpdateReq postAllocUpdateReq, string gupCode)
		{
			P81Service p81Service = new P81Service();
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);

			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10002", MsgContent = p81Service.GetMsg("10002") };

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(postAllocUpdateReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(postAllocUpdateReq.FuncNo, postAllocUpdateReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(postAllocUpdateReq.CustNo, postAllocUpdateReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(postAllocUpdateReq.DcNo, postAllocUpdateReq.AccNo);

			// 單據類別
			List<string> allocTypeList = new List<string> { "01", "02", "03" };

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(postAllocUpdateReq.FuncNo) ||
					string.IsNullOrWhiteSpace(postAllocUpdateReq.AccNo) ||
					string.IsNullOrWhiteSpace(postAllocUpdateReq.DcNo) ||
					string.IsNullOrWhiteSpace(postAllocUpdateReq.CustNo) ||
					string.IsNullOrWhiteSpace(postAllocUpdateReq.AllocNo) ||
					string.IsNullOrWhiteSpace(postAllocUpdateReq.AllocType) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0 ||
					!allocTypeList.Contains(postAllocUpdateReq.AllocType))
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}

			// 取得調撥單主檔資料
			F151001 data = f151001Repo.GetSingleData(
					postAllocUpdateReq.DcNo,
					postAllocUpdateReq.CustNo,
					gupCode,
					postAllocUpdateReq.AllocNo);

			if (result.IsSuccessed && data == null)
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = p81Service.GetMsg("20064") };
			}

			if (result.IsSuccessed && data != null && data.STATUS == "9")
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20065", MsgContent = p81Service.GetMsg("20065") };
			}

			if (result.IsSuccessed && data != null && data.STATUS == "5")
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = p81Service.GetMsg("20066") };
			}
			#endregion

			#region 資料處理
			if (result.IsSuccessed)
			{
				// & 取得人員名稱
				string empName = p81Service.GetEmpName(postAllocUpdateReq.AccNo);

				// 如果調撥狀態為0(待處理)、1(已列印調撥單)、2(下架處理中)
				if (postAllocUpdateReq.AllocType == "02" &&
						(data.STATUS == "0" || data.STATUS == "1" || data.STATUS == "2"))
				{
					// 更新下架人員編號[登入者帳號]、下架人名[登入者名稱]、
					// 狀態[2]、LOCK_STATUS[1]
					f151001Repo.PostAllocUpdate(
							postAllocUpdateReq.DcNo,
							postAllocUpdateReq.CustNo,
							gupCode,
							postAllocUpdateReq.AllocNo,
							null,
							null,
							postAllocUpdateReq.AccNo,
							empName,
							"2",
							"1");
				}
				// 如果調撥狀態為3(已下架處理)、4(上架處理中)
				else if ((postAllocUpdateReq.AllocType == "01" || postAllocUpdateReq.AllocType == "03") &&
						(data.STATUS == "3" || data.STATUS == "4"))
				{
					// 更新上架人員編號[登入者帳號]、上架人名[登入者名稱]、
					// 狀態[4]、LOCK_STATUS[3]
					f151001Repo.PostAllocUpdate(
							postAllocUpdateReq.DcNo,
							postAllocUpdateReq.CustNo,
							gupCode,
							postAllocUpdateReq.AllocNo,
							postAllocUpdateReq.AccNo,
							empName,
							null,
							null,
							"4",
							"3");
				}
				else
				{
					result = new ApiResult { IsSuccessed = false, MsgCode = "20070", MsgContent = p81Service.GetMsg("20070") };
				}
			}
			#endregion

			return result;
		}

		/// <summary>
		/// 調撥明細查詢
		/// </summary>
		/// <param name="getAllocReq"></param>
		/// <returns></returns>
		public virtual ApiResult GetAllocDetail(GetAllocDetailReq getAllocDetailReq, string gupCode)
		{
			P81Service p81Service = new P81Service();
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);

			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };
			GetAllocDetailRes resData = new GetAllocDetailRes();
			resData.DetailList = new List<GetAllocDetail>();

			#region 資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(getAllocDetailReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(getAllocDetailReq.FuncNo, getAllocDetailReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(getAllocDetailReq.CustNo, getAllocDetailReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(getAllocDetailReq.DcNo, getAllocDetailReq.AccNo);

			// 單據類別
			List<string> allocTypeList = new List<string> { "01", "02", "03" };

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(getAllocDetailReq.FuncNo) ||
					string.IsNullOrWhiteSpace(getAllocDetailReq.AccNo) ||
					string.IsNullOrWhiteSpace(getAllocDetailReq.DcNo) ||
					string.IsNullOrWhiteSpace(getAllocDetailReq.CustNo) ||
					string.IsNullOrWhiteSpace(getAllocDetailReq.AllocNo) ||
					string.IsNullOrWhiteSpace(getAllocDetailReq.AllocType) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0 ||
					!allocTypeList.Contains(getAllocDetailReq.AllocType))
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}

			#endregion

			#region 資料處理

			if (result.IsSuccessed)
			{
				// 取得資料處理1 Data
				var detailList = f151002Repo.GetAllocDetail(
								getAllocDetailReq.DcNo,
								getAllocDetailReq.CustNo,
								gupCode,
								getAllocDetailReq.AllocNo,
								getAllocDetailReq.AllocType
						);

				if (detailList.Any())
				{
					// 取得調撥路線編號
					var routeDataList = p81Service.GetRouteList(detailList.Select(x => new GetRouteListReq
					{
						No = x.AllocNo,
						Seq = x.AllocSeq,
						LocCode = x.SugLoc
					}).ToList());

					var itemService = new ItemService();

					// 商品單位階層
					//var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, getAllocDetailReq.CustNo, detailList.Select(x => new Datas.Shared.Entities.ItemCodeQtyModel { ItemCode = x.ItemNo, Qty = x.Qty }).ToList());

					foreach (var item in detailList)
					{
						//var currRef = itemPackageRefs.Where(z => z.ItemCode == item.ItemNo).FirstOrDefault();

						resData.DetailList.Add(new GetAllocDetail
						{
							AllocNo = item.AllocNo,
							AllocSeq = item.AllocSeq.ToString(),
							Route = routeDataList.Where(z => item.AllocNo == z.No && item.AllocSeq == z.Seq && item.SugLoc == z.LocCode).SingleOrDefault().Route,
							ItemNo = item.ItemNo,
							WhName = item.WhName,
							SugLoc = item.SugLoc,
							ValidDate = item.ValidDate,
							EnterDate = item.EnterDate,
							MkNo = item.MkNo,
							Sn = item.Sn,
							Qty = item.Qty,
							ActQty = item.ActQty,
							PalletNo = item.PalletNo,
							//PackRef = currRef == null ? null : currRef.PackageRef,
							EanCode1 = item.EanCode1
						});
					}

					List<string> rtNo = null;

					var itemNoArray = detailList.Select(x => x.ItemNo).Distinct().ToList();
					if (getAllocDetailReq.AllocType.Contains("01"))
					{
						// (1) 驗收單號 = Select RT_NO from F02020107 by dc_code、gup_code、cust_code and allocation_no = AllocNo
						// (2) 商品序號資料 = &取得商品序號清單[CustNo, F151002[item_code清單], 驗收單號]
						var f02020107Repo = new F02020107Repository(Schemas.CoreSchema);
						rtNo = f02020107Repo.GetRtNo(getAllocDetailReq.DcNo, getAllocDetailReq.CustNo, gupCode, getAllocDetailReq.AllocNo).ToList();

						resData.ItemSnList = p81Service.GetSnList(getAllocDetailReq.DcNo, getAllocDetailReq.CustNo, gupCode, itemNoArray, rtNo).ToList();
					}
					else
					{
						var snList = detailList.Where(x => !string.IsNullOrWhiteSpace(x.Sn)).Select(x => x.Sn).ToList();
						resData.ItemSnList = p81Service.GetSnList(getAllocDetailReq.DcNo, getAllocDetailReq.CustNo, gupCode, itemNoArray, null, snList).ToList();
					}
				}

				result.Data = resData;
			}

			#endregion

			return result;
		}

		/// <summary>
		/// 調撥確認
		/// </summary>
		/// <param name="postAllocConfirmReq"></param>
		/// <returns></returns>
		public ApiResult PostAllocConfirm(PostAllocConfirmReq postAllocConfirmReq, string gupCode)
		{
			P81Service p81Service = new P81Service(_wmsTransation);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransation);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransation);
			var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransation);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransation);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransation);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f191204Repo = new F191204Repository(Schemas.CoreSchema, _wmsTransation);
			var sharedService = new SharedService(_wmsTransation);

			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

            // 傳入參數轉大寫
            if (!string.IsNullOrWhiteSpace(postAllocConfirmReq.ActLoc))
                postAllocConfirmReq.ActLoc = postAllocConfirmReq.ActLoc.ToUpper();

            #region 資料檢核

            // 帳號檢核
            var accData = p81Service.CheckAcc(postAllocConfirmReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(postAllocConfirmReq.FuncNo, postAllocConfirmReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(postAllocConfirmReq.CustNo, postAllocConfirmReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(postAllocConfirmReq.DcNo, postAllocConfirmReq.AccNo);

			// 單據類別
			List<string> allocTypeList = new List<string> { "01", "02", "03" };

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(postAllocConfirmReq.FuncNo) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.AccNo) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.DcNo) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.CustNo) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.AllocNo) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.AllocSeq) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.ActLoc) ||
					string.IsNullOrWhiteSpace(postAllocConfirmReq.AllocType) ||
					!allocTypeList.Contains(postAllocConfirmReq.AllocType) ||
					!p81Service.CheckIsNum(postAllocConfirmReq.AllocSeq) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}

            // 實際儲位轉大寫
            postAllocConfirmReq.ActLoc = postAllocConfirmReq.ActLoc.ToUpper();

            F151001 f151001Data = new F151001();
			// 取得多筆調撥單明細資料
			IQueryable<F151002> f151002Datas = f151002Repo.AsForUpdate().GetDatas(postAllocConfirmReq.DcNo, gupCode, postAllocConfirmReq.CustNo, postAllocConfirmReq.AllocNo).OrderBy(x => x.ALLOCATION_SEQ);
			F151002 f151002Data = new F151002();
			if (result.IsSuccessed)
			{
				// 取得調撥單主檔資料
				f151001Data = f151001Repo.Find(o => o.DC_CODE == postAllocConfirmReq.DcNo &&
																						o.GUP_CODE == gupCode &&
																						o.CUST_CODE == postAllocConfirmReq.CustNo &&
																						o.ALLOCATION_NO == postAllocConfirmReq.AllocNo);

				// 取得一筆調撥單明細資料
				f151002Data = f151002Datas.Where(x => x.ALLOCATION_SEQ == Convert.ToInt16(postAllocConfirmReq.AllocSeq)).SingleOrDefault();
			}

			// 檢核調撥單是否存在
			if (result.IsSuccessed && f151001Data == null)
				result = new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = p81Service.GetMsg("20064") };

			// 檢核調撥單狀態是否刪除
			if (result.IsSuccessed && f151001Data.STATUS == "9")
				result = new ApiResult { IsSuccessed = false, MsgCode = "20065", MsgContent = p81Service.GetMsg("20065") };

			// 檢核調撥單狀態是否結案
			if (result.IsSuccessed && f151001Data.STATUS == "5")
				result = new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = p81Service.GetMsg("20066") };

			// 單據明細不存在
			if (result.IsSuccessed && f151002Data == null)
				result = new ApiResult { IsSuccessed = false, MsgCode = "20352", MsgContent = p81Service.GetMsg("20352") };

			// 檢核數量
			if (result.IsSuccessed && postAllocConfirmReq.ActQty < 0)
				result = new ApiResult { IsSuccessed = false, MsgCode = "20059", MsgContent = p81Service.GetMsg("20059") };

			// 檢核儲位是否凍結，False代表檢核不通過
			bool checkLocFreeze = p81Service.CheckLocFreeze(postAllocConfirmReq.DcNo, postAllocConfirmReq.ActLoc, postAllocConfirmReq.AllocType);

			// 下架
			if (result.IsSuccessed && postAllocConfirmReq.AllocType == "02")
			{

				// 檢核調撥單是否其他作業人員使用
				if (result.IsSuccessed && !string.IsNullOrWhiteSpace(f151001Data.SRC_MOVE_STAFF) && f151001Data.SRC_MOVE_STAFF != postAllocConfirmReq.AccNo)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20061", MsgContent = p81Service.GetMsg("20061") };

				// 如果狀態=1 下架:回傳&取得訊息內容[20067]
				if (result.IsSuccessed && f151002Data.STATUS == "1")
					result = new ApiResult { IsSuccessed = false, MsgCode = "20067", MsgContent = p81Service.GetMsg("20067") };

				// 下架:如果數量超過應下架數-已下架數，則回傳&取得訊息內容[20063]
				if (result.IsSuccessed && postAllocConfirmReq.ActQty > (f151002Data.SRC_QTY - f151002Data.A_SRC_QTY))
					result = new ApiResult { IsSuccessed = false, MsgCode = "20063", MsgContent = p81Service.GetMsg("20063") };

				// 檢核是否有凍結，False代表檢核不通過
				if (result.IsSuccessed && !checkLocFreeze)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20062", MsgContent = p81Service.GetMsg("20062") };
			}
			// 上架
			else if (result.IsSuccessed && postAllocConfirmReq.AllocType == "01" || postAllocConfirmReq.AllocType == "03")
			{
				// 檢核調撥單是否其他作業人員使用
				if (result.IsSuccessed && !string.IsNullOrWhiteSpace(f151001Data.TAR_MOVE_STAFF) && f151001Data.TAR_MOVE_STAFF != postAllocConfirmReq.AccNo)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20061", MsgContent = p81Service.GetMsg("20061") };

				// 如果狀態=2 上架:回傳&取得訊息內容[20068]
				if (result.IsSuccessed && f151002Data.STATUS == "2")
					result = new ApiResult { IsSuccessed = false, MsgCode = "20068", MsgContent = p81Service.GetMsg("20068") };

				// 檢核上架儲位是否存在、是否為上架倉
				if (result.IsSuccessed)
				{
					var f1912 = f1912Repo.Find(o => o.DC_CODE == postAllocConfirmReq.DcNo && o.LOC_CODE == postAllocConfirmReq.ActLoc);
					if (f1912 == null)
						result = new ApiResult { IsSuccessed = false, MsgCode = "20354", MsgContent = string.Format(p81Service.GetMsg("20354"), postAllocConfirmReq.ActLoc) };
					else
					{
						if (f1912.WAREHOUSE_ID != f151001Data.TAR_WAREHOUSE_ID)
						{
							var f1980 = f1980Repo.Find(o => o.DC_CODE == postAllocConfirmReq.DcNo && o.WAREHOUSE_ID == f151001Data.TAR_WAREHOUSE_ID);
							result = new ApiResult { IsSuccessed = false, MsgCode = "20355", MsgContent = string.Format(p81Service.GetMsg("20355"), postAllocConfirmReq.ActLoc, f1980.WAREHOUSE_NAME) };
						}
					}
				}

				// 上架:如果數量超過應上架數-已上架數，則回傳&取得訊息內容[20060]
				if (result.IsSuccessed && postAllocConfirmReq.ActQty > (f151002Data.TAR_QTY - f151002Data.A_TAR_QTY))
					result = new ApiResult { IsSuccessed = false, MsgCode = "20060", MsgContent = p81Service.GetMsg("20060") };

				// 檢核是否有凍結，False代表檢核不通過
				if (result.IsSuccessed && !checkLocFreeze)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = p81Service.GetMsg("20054") };
			}

			// 檢核儲位是否登入者有權限
			if (result.IsSuccessed && !p81Service.CheckActLoc(postAllocConfirmReq.AccNo, postAllocConfirmReq.DcNo, postAllocConfirmReq.ActLoc))
				result = new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = p81Service.GetMsg("20055") };

			// 檢核儲位是否非此貨主儲位
			var f1912Datas = f1912Repo.GetF1912Datas(postAllocConfirmReq.DcNo, gupCode, postAllocConfirmReq.CustNo, postAllocConfirmReq.ActLoc);
			foreach (var item in f1912Datas)
			{
				if (item.CUST_CODE != postAllocConfirmReq.CustNo && item.CUST_CODE != "0")
					result = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = p81Service.GetMsg("20056") };
				if (item.NOW_CUST_CODE != postAllocConfirmReq.CustNo && item.NOW_CUST_CODE != "0")
					result = new ApiResult { IsSuccessed = false, MsgCode = "20074", MsgContent = p81Service.GetMsg("20074") };
			}

			// 檢查商品上架儲位溫層(針對上架才處理)
			if (result.IsSuccessed && (postAllocConfirmReq.AllocType == "01" || postAllocConfirmReq.AllocType == "03"))
			{
				// 取得儲位溫層 => F1912 + F1980 + VW_F000904_LANG(TOPIC = F1980, SUBTOPIC = TMPR_TYPE, VALUE = F1980.TMPR_TYPE, LANG = Current.Lang)=>F1980.TMPR_TYPE
				var f1912Tmpr = f1912Repo.GetTmprTypeData(postAllocConfirmReq.DcNo, postAllocConfirmReq.ActLoc);
				if (result.IsSuccessed && f1912Tmpr == null)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20073", MsgContent = string.Format(p81Service.GetMsg("20073"), postAllocConfirmReq.ActLoc) };

				// 取得商品溫層 => F1903.TMPR_TYPE + VW_F000904_LANG(TOPIC = F1903, SUBTOPIC = TMPR_TYPE, VALUE = F1903.TMPR_TYPE, LANG = Current.Lang)
				TmprTypeModel f1903Tmpr = null;
				if (result.IsSuccessed)
					f1903Tmpr = f1903Repo.GetTmprTypeData(gupCode, f151002Data.ITEM_CODE);

				if (result.IsSuccessed && f1903Tmpr == null)
					result = new ApiResult { IsSuccessed = false, MsgCode = "20072", MsgContent = string.Format(p81Service.GetMsg("20072"), f151002Data.ITEM_CODE) };

				// 商品溫層轉儲位溫層，共用方法: sharedService.GetWareHouseTmprByItemTmpr
				string newTmpr = p81Service.GetWareHouseTmprByItemTmpr(f1903Tmpr.TmprType);

				// 比較儲位溫層 = 商品溫層轉儲位溫層 如果不相同則回傳 & 取得訊息內容[20071](儲位, 儲位溫層名稱, 品號, 商品溫層名稱)
				if (!newTmpr.Split(',').Contains(f1912Tmpr.TmprType))
				{
					//"儲位{0}溫層{1}不符合商品{2}溫層{3}"
					result = new ApiResult
					{
						IsSuccessed = false,
						MsgCode = "20071",
						MsgContent = string.Format(p81Service.GetMsg("20071"),
							postAllocConfirmReq.ActLoc, f1912Tmpr.TmprTypeName, f151002Data.ITEM_CODE, f1903Tmpr.TmprTypeName)
					};
				}

				// 檢核商品是否混批
				if (result.IsSuccessed && !p81Service.CheckItemMixBatch(postAllocConfirmReq.DcNo, gupCode, postAllocConfirmReq.CustNo, f151002Data.ITEM_CODE, postAllocConfirmReq.ActLoc, f151002Data.VALID_DATE.ToString("yyyy/MM/dd")))
					result = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = p81Service.GetMsg("20057") };

				// 檢核商品是否混品
				if (result.IsSuccessed && !p81Service.CheckItemMixLoc(postAllocConfirmReq.DcNo, gupCode, postAllocConfirmReq.CustNo, f151002Data.ITEM_CODE, postAllocConfirmReq.ActLoc))
					result = new ApiResult { IsSuccessed = false, MsgCode = "20058", MsgContent = p81Service.GetMsg("20058") };
			}
			#endregion

			#region 資料處理
			if (result.IsSuccessed)
			{
				var param = new AllocationConfirmParam
				{
					DcCode = postAllocConfirmReq.DcNo,
					GupCode = gupCode,
					CustCode = postAllocConfirmReq.CustNo,
					AllocNo = postAllocConfirmReq.AllocNo,
					Operator = postAllocConfirmReq.AccNo,
					Details = new List<AllocationConfirmDetail>
					{
						new AllocationConfirmDetail{ Seq = f151002Data.ALLOCATION_SEQ, TarLocCode = postAllocConfirmReq.ActLoc, Qty = postAllocConfirmReq.ActQty }
					}
				};

        sharedService.AllocationConfirm(param);

				_wmsTransation.Complete();

				// 下架
				if (postAllocConfirmReq.AllocType == "02")
					result = new ApiResult { IsSuccessed = true, MsgCode = "10003", MsgContent = p81Service.GetMsg("10003") };

				// 上架
				else if (postAllocConfirmReq.AllocType == "01" || postAllocConfirmReq.AllocType == "03")
					result = new ApiResult { IsSuccessed = true, MsgCode = "10004", MsgContent = p81Service.GetMsg("10004") };
			}
			#endregion

			return result;
		}
	}
}
