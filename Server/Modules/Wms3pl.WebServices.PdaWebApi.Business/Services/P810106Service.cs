using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P08.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810106Service
	{
		private WmsTransaction _wmsTransation;
		private P81Service _p81Service;
		private SharedService _sharedService;
		public P810106Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
			_p81Service = new P81Service(_wmsTransation);
			_sharedService = new SharedService(_wmsTransation);

		}

		/// <summary>
		/// 揀貨基本共用檢核
		/// </summary>
		/// <param name="pickBaseReq"></param>
		/// <returns></returns>
		private ApiResult PickBaseCheck(PickBaseReq pickBaseReq)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001") };

			// 帳號檢核
			var accData = _p81Service.CheckAcc(pickBaseReq.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = _p81Service.CheckAccFunction(pickBaseReq.FuncNo, pickBaseReq.AccNo);

			// 檢核人員貨主權限
			var accCustCount = _p81Service.CheckAccCustCode(pickBaseReq.CustNo, pickBaseReq.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = _p81Service.CheckAccDc(pickBaseReq.DcNo, pickBaseReq.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(pickBaseReq.FuncNo) ||
					string.IsNullOrWhiteSpace(pickBaseReq.AccNo) ||
					string.IsNullOrWhiteSpace(pickBaseReq.DcNo) ||
					string.IsNullOrWhiteSpace(pickBaseReq.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0 ||
					(!string.IsNullOrWhiteSpace(pickBaseReq.WmsNo) && !pickBaseReq.WmsNo.StartsWith("P")))
			{
				result = new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };
			}
			return result;
		}

		/// <summary>
		/// 取得PDA 揀貨明細
		/// </summary>
		/// <param name="pickBaseReq"></param>
		/// <param name="gupCode"></param>
		/// <param name="lastGetPickDate"></param>
		/// <returns></returns>
		private GetPickDetailRes GetPickDetail(PickBaseReq pickBaseReq, string gupCode, DateTime? lastGetPickDate)
		{
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema);

			GetPickDetailRes resData = new GetPickDetailRes();
			resData.DetailList = new List<GetPickDetailDetail>();
			resData.ItemSnList = new List<GetAllocItemSn>();

			var f051201 = _sharedService.GetF051201(pickBaseReq.DcNo, gupCode, pickBaseReq.CustNo, pickBaseReq.WmsNo);

			// 單一揀貨
			if (f051201.SPLIT_TYPE == "03")
			{
                // 1.資料表:F051202 + F1912 + F1980
                // 2.條件:DC_CODE = 傳入的物流中心編號 AND GUP_CODE = 業主編號 AND CUST_CODE = 傳入的貨主編號 AND PICK_ORD_NO = 傳入的單號 AND PICK_STATUS = 0
                // 3.WmsNo = 揀貨單號,WmsSeq = 揀貨明細序號,PickNo = 揀貨單號
                if (pickBaseReq.IsSync == "1")
                    resData.DetailList = f051202Repo.GetSinglePickDetail(pickBaseReq.DcNo, gupCode, pickBaseReq.CustNo, pickBaseReq.WmsNo).ToList();
                else
                    resData.DetailList = f051202Repo.GetSinglePickDetailAllCol(pickBaseReq.DcNo, gupCode, pickBaseReq.CustNo, pickBaseReq.WmsNo).ToList();

                if (lastGetPickDate.HasValue)
					resData.DetailList = resData.DetailList.Where(x => x.CrtDate > lastGetPickDate.Value).ToList();
			}
			// 批量揀貨
			else
			{
                // 1.資料表:F051203 + F1912 + F1980
                // 2.條件:DC_CODE = 傳入的物流中心編號 AND GUP_CODE = 業主編號 AND CUST_CODE = 傳入的貨主編號 AND PICK_ORD_NO = 傳入的單號 AND PICK_STATUS = 0
                // 3.WmsNo = 揀貨單號,WmsSeq = 揀貨明細序號,PickNo = 揀貨單號
                if (pickBaseReq.IsSync == "1")
                    resData.DetailList = f051203Repo.GetBatchPickDetail(pickBaseReq.DcNo, gupCode, pickBaseReq.CustNo, pickBaseReq.WmsNo).ToList();
                else
                    resData.DetailList = f051203Repo.GetBatchPickDetailAllCol(pickBaseReq.DcNo, gupCode, pickBaseReq.CustNo, pickBaseReq.WmsNo).ToList();

                if (lastGetPickDate.HasValue)
					resData.DetailList = resData.DetailList.Where(x => x.CrtDate > lastGetPickDate.Value).ToList();
			}

			if (resData.DetailList.Count > 0)
			{
				// 取得調撥路線編號
				var routeDataList = _p81Service.GetRouteList(resData.DetailList.Select(x => new GetRouteListReq
				{
					No = x.WmsNo,
					Seq = Convert.ToInt32(x.WmsSeq),
					LocCode = x.Loc
				}).ToList());

				// 取得包裝參考
				var itemService = new ItemService();
				var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, pickBaseReq.CustNo, resData.DetailList.Select(x => new Datas.Shared.Entities.ItemCodeQtyModel { ItemCode = x.ItemNo, Qty = x.ShipQty }).ToList());

				resData.DetailList.ForEach(x =>
				{
					var currRef = itemPackageRefs.Where(z => z.ItemCode == x.ItemNo).FirstOrDefault();

					x.PackRef = currRef == null ? null : currRef.PackageRef;
					// x.Route = routeDataList.Where(z => x.WmsNo.Equals(z.No) && Convert.ToInt32(x.WmsSeq) == z.Seq && x.Loc == z.LocCode).SingleOrDefault().Route;
				});

				var snList = resData.DetailList.Where(x => !string.IsNullOrWhiteSpace(x.Sn) && x.Sn != "0").Select(x => x.Sn).ToList();
				// 取得商品序號清單
				resData.ItemSnList = _p81Service.GetSnList(
						pickBaseReq.DcNo,
						pickBaseReq.CustNo,
						gupCode,
						resData.DetailList.Select(x => x.ItemNo).ToList(),
						null,
						snList
						).ToList();
			}

			return resData;
		}

		/// <summary>
		/// 揀貨單據查詢
		/// </summary>
		/// <param name="getPickReq"></param>
		/// <returns></returns>
		public ApiResult GetPick(GetPickReq getPickReq, string gupCode)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001") };

			#region 資料檢核

			var checkPickBase = PickBaseCheck(getPickReq);
			if (!checkPickBase.IsSuccessed)
				return checkPickBase;

			// 單據類別
			List<string> modeList = new List<string> { "01", "02" };

			// 傳入的參數驗證 
			if (!modeList.Contains(getPickReq.Mode))
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			#endregion

			#region 資料處理


			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);


			// 取得人員名稱
			string empName = _p81Service.GetEmpName(getPickReq.AccNo);

			if (getPickReq.Mode.Equals("01"))
			{
				// 作業模式=01(批量揀貨) 走資料處理1+資料處理2
				result.Data = f051201Repo.GetPdaBatchPick(
						getPickReq.Mode,
						getPickReq.DcNo,
						gupCode,
						getPickReq.CustNo,
						getPickReq.WmsNo,
						getPickReq.ShipDate,
						getPickReq.AccNo);
			}
			else if (getPickReq.Mode.Equals("02"))
			{
				// 作業模式=02(單一揀貨) 走資料處理3
				result.Data = f051201Repo.GetPdaSinglePick(
						getPickReq.Mode,
						getPickReq.DcNo,
						gupCode,
						getPickReq.CustNo,
						getPickReq.WmsNo,
						getPickReq.ShipDate,
						getPickReq.AccNo);
			}


			#endregion

			return result;
		}

		/// <summary>
		/// 揀貨明細查詢
		/// </summary>
		/// <param name="getPickDetailReq"></param>
		/// <returns></returns>
		public ApiResult GetPickDetail(GetPickDetailReq getPickDetailReq, string gupCode)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001") };

			#region 資料檢核

			var checkPickBase = PickBaseCheck(getPickDetailReq);
			if (!checkPickBase.IsSuccessed)
				return checkPickBase;

			#endregion

			#region 資料處理

			result.Data = GetPickDetail(getPickDetailReq, gupCode, null);

			#endregion

			return result;
		}

		/// <summary>
		/// 揀貨單據檢核
		/// </summary>
		/// <param name="postPickCheckReq"></param>
		/// <returns></returns>
		public ApiResult PostPickCheck(PostPickCheckReq postPickCheckReq, string gupCode)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001") };

			#region 資料檢核

			var checkPickBase = PickBaseCheck(postPickCheckReq);
			if (!checkPickBase.IsSuccessed)
				return checkPickBase;

			#endregion

			#region 資料處理

			var f051201 = _sharedService.GetF051201(postPickCheckReq.DcNo, gupCode, postPickCheckReq.CustNo, postPickCheckReq.WmsNo);

			//  檢查單據是否存在(F051201 = null) 不存在回傳 & 取得訊息內容[20064]
			if (f051201 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = _p81Service.GetMsg("20064") };

			// 檢查單據是否取消(F051201.STATUS = 9) 回傳 & 取得訊息內容[20065]
			if (f051201.PICK_STATUS == 9)
				return new ApiResult { IsSuccessed = false, MsgCode = "20065", MsgContent = _p81Service.GetMsg("20065") };

			// 檢查單據是否已完成揀貨 F051201.STATUS = 2 回傳 & 取得訊息內容[20066]
			if (f051201.PICK_STATUS == 2)
				return new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = _p81Service.GetMsg("20066") };

			// 檢核單據是否有其人作業人員揀貨中
			if (f051201.PICK_STAFF != null && f051201.PICK_STAFF != postPickCheckReq.AccNo)
				return new ApiResult { IsSuccessed = false, MsgCode = "30001", MsgContent = _p81Service.GetMsg("30001").Replace("(%s)", $"({f051201.PICK_NAME})") };

      //檢查訂單是否已被取消，被取消的話就自動回庫
      var checkIsOrderCancel = _sharedService.CheckIfAllOrdersCanceledByPickNoList(postPickCheckReq.DcNo, gupCode, postPickCheckReq.CustNo, new[] { postPickCheckReq.WmsNo }.ToList());
      if (checkIsOrderCancel.Any())
      {
        if (_wmsTransation != null)
          _wmsTransation.Complete();
        return new ApiResult { IsSuccessed = false, MsgCode = "20460", MsgContent = _p81Service.GetMsg("20460") };
      }
      #endregion

      return result;
		}

		/// <summary>
		/// 揀貨單據更新
		/// </summary>
		/// <param name="postPickUpdateReq"></param>
		/// <returns></returns>
		public ApiResult PostPickUpdate(PostPickUpdateReq postPickUpdateReq, string gupCode)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10002", MsgContent = _p81Service.GetMsg("10002") };

			#region 資料檢核

			var checkPickBase = PickBaseCheck(postPickUpdateReq);
			if (!checkPickBase.IsSuccessed)
				return checkPickBase;

			var f051201 = _sharedService.GetF051201(postPickUpdateReq.DcNo, gupCode, postPickUpdateReq.CustNo, postPickUpdateReq.WmsNo);

			//  檢查單據是否存在(F051201 = null) 不存在回傳 & 取得訊息內容[20064]
			if (f051201 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = _p81Service.GetMsg("20064") };

			// 檢查單據是否取消(F051201.STATUS = 9) 回傳 & 取得訊息內容[20065]
			if (f051201.PICK_STATUS == 9)
				return new ApiResult { IsSuccessed = false, MsgCode = "20065", MsgContent = _p81Service.GetMsg("20065") };

			// 檢查單據是否已完成揀貨 F051201.STATUS = 2 回傳 & 取得訊息內容[20066]
			if (f051201.PICK_STATUS == 2)
				return new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = _p81Service.GetMsg("20066") };


			#endregion

			#region 資料處理

			_sharedService.StartPick(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, postPickUpdateReq.AccNo,new List<string> { f051201.PICK_ORD_NO });

			#endregion

			_wmsTransation.Complete();

			return result;
		}

		/// <summary>
		/// 揀貨完成確認
		/// </summary>
		/// <param name="postPickConfirmReq"></param>
		/// <returns></returns>
		public ApiResult PostPickConfirm(PostPickConfirmReq postPickConfirmReq, string gupCode)
		{
			ApiResult result = new ApiResult { IsSuccessed = true, MsgCode = "10002", MsgContent = _p81Service.GetMsg("10002") };
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransation);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransation);
			var f1945Repo = new F1945Repository(Schemas.CoreSchema);
			var f051301Repo = new F051301Repository(Schemas.CoreSchema);
			F051203 f051203;

			#region 資料檢核

			var checkResult = PickBaseCheck(postPickConfirmReq);
			if (!checkResult.IsSuccessed)
				return checkResult;

			if (postPickConfirmReq.ActQty < 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20453", MsgContent = _p81Service.GetMsg("20453") };

			var f051201 = _sharedService.GetF051201(postPickConfirmReq.DcNo, gupCode, postPickConfirmReq.CustNo, postPickConfirmReq.WmsNo);

			//  檢查單據是否存在(F051201 = null) 不存在回傳 & 取得訊息內容[20064]
			if (f051201 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20064", MsgContent = _p81Service.GetMsg("20064") };

			// 檢查單據是否取消(F051201.STATUS = 9) 回傳 & 取得訊息內容[20065]
			if (f051201.PICK_STATUS == 9)
				return new ApiResult { IsSuccessed = false, MsgCode = "20065", MsgContent = _p81Service.GetMsg("20065") };

			// 檢查單據是否已完成揀貨 F051201.STATUS = 2 回傳 & 取得訊息內容[20066]
			if (f051201.PICK_STATUS == 2)
				return new ApiResult { IsSuccessed = false, MsgCode = "20066", MsgContent = _p81Service.GetMsg("20066") };

			var f051202 = f051202Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == postPickConfirmReq.DcNo && x.GUP_CODE == gupCode &&
			x.CUST_CODE == postPickConfirmReq.CustNo && x.PICK_ORD_NO == postPickConfirmReq.WmsNo && x.PICK_ORD_SEQ == postPickConfirmReq.WmsSeq).FirstOrDefault();
			
			// 單一揀貨
			if (f051201.SPLIT_TYPE == "03")
			{

				// 檢查揀貨明細是否存在，若已不存在回傳&取得訊息內容[20459]
				if (f051202 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "20459", MsgContent = _p81Service.GetMsg("20459") };

				// 檢查揀貨明細是否已完成(PICK_STATUS = 1)，若已完成回傳 & 取得訊息內容[20457]
				if (f051202.PICK_STATUS == "1")
					return new ApiResult { IsSuccessed = false, MsgCode = "20457", MsgContent = string.Format(_p81Service.GetMsg("20457"), f051202.UPD_NAME) };

				// 檢查揀貨明細是否已取消(PICK_STATUS=9)，若已取消回傳&取得訊息內容[20458]
				if (f051202.PICK_STATUS == "9")
					return new ApiResult { IsSuccessed = false, MsgCode = "20458", MsgContent = _p81Service.GetMsg("20458") };

				// 檢查實際揀貨數量是否超過 F051202.A_PICK_QTY + 傳入的實際揀貨數量 > F051202.B_PICK_QTY 如果超過回傳 & 取得訊息內容[20455]
				if ((f051202.A_PICK_QTY + postPickConfirmReq.ActQty) > f051202.B_PICK_QTY)
					return new ApiResult { IsSuccessed = false, MsgCode = "20455", MsgContent = _p81Service.GetMsg("20455") };
			}
			// 批量揀貨
			else
			{
				f051203 = f051203Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == postPickConfirmReq.DcNo && x.GUP_CODE == gupCode &&
				x.CUST_CODE == postPickConfirmReq.CustNo && x.PICK_ORD_NO == postPickConfirmReq.WmsNo && x.TTL_PICK_SEQ == postPickConfirmReq.WmsSeq).FirstOrDefault();

				// 檢查揀貨明細是否存在，若已不存在回傳&取得訊息內容[20459]
				if (f051203 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "20459", MsgContent = _p81Service.GetMsg("20459") };

				// 檢查揀貨明細是否已完成(PICK_STATUS = 1)，若已完成回傳 & 取得訊息內容[20457]
				if (f051203.PICK_STATUS == "1")
					return new ApiResult { IsSuccessed = false, MsgCode = "20457", MsgContent = string.Format(_p81Service.GetMsg("20457"), f051203.UPD_NAME) };

				// 檢查揀貨明細是否已取消(PICK_STATUS=9)，若已取消回傳&取得訊息內容[20458]
				if (f051203.PICK_STATUS == "9")
					return new ApiResult { IsSuccessed = false, MsgCode = "20458", MsgContent = _p81Service.GetMsg("20458") };

				// 檢查實際揀貨數量是否超過 F051203.A_PICK_QTY + 傳入的實際揀貨數量 > F051203.B_PICK_QTY 如果超過回傳 & 取得訊息內容[20455]
				if ((f051203.A_PICK_QTY + postPickConfirmReq.ActQty) > f051203.B_PICK_QTY)
					return new ApiResult { IsSuccessed = false, MsgCode = "20455", MsgContent = _p81Service.GetMsg("20455") };

			}

			#endregion

			#region 資料處理

			var lastGetDetailDate = f051201.UPD_DATE;

			// 揀貨單處理
			_sharedService.PickConfirm(new PickConfirmParam
			{
				DcCode = postPickConfirmReq.DcNo,
				GupCode = gupCode,
				CustCode = postPickConfirmReq.CustNo,
				PickNo = postPickConfirmReq.WmsNo,
				EmpId = postPickConfirmReq.AccNo,
				Details = new List<PickConfirmDetail> {
					new PickConfirmDetail
					{
						Seq = postPickConfirmReq.WmsSeq,
						Qty = postPickConfirmReq.ActQty
					}
				}
			});

			#endregion

			_wmsTransation.Complete();

			//如果有新的揀貨明細回傳給PDA 讓人員接續揀貨
			result.Data = GetPickDetail(postPickConfirmReq, gupCode, lastGetDetailDate);

			if (f051201.PICK_STATUS == 2)
			{
				result.IsSuccessed = true;
				result.MsgCode = "10009";

				if(f051201.SPLIT_TYPE == "03") // 單一揀貨
				{
					var nextStepData = f051301Repo.GetCollectionNameByWmsNo(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.SPLIT_CODE);
					if (nextStepData.NEXT_STEP == "2")
					{
						result.MsgContent = string.Format(_p81Service.GetMsg("10009"), nextStepData.NEXT_STEP_NAME) + nextStepData.COLLECTION_NAME;
					}
					else
					{
						result.MsgContent = string.Format(_p81Service.GetMsg("10009"), _p81Service.GetTopicValueName("F051201", "NEXT_STEP", f051201.NEXT_STEP));
					}
				}
				else
				{
					result.MsgContent = string.Format(_p81Service.GetMsg("10009"), _p81Service.GetTopicValueName("F051201", "NEXT_STEP", f051201.NEXT_STEP));
				}
			}

			return result;
		}
	}
}
