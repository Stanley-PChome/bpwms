using System;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810114Service
	{
		private WmsTransaction _wmsTransation;
		public P810114Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 上架移動-容器查詢
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult GetApprovedData(GetApprovedDataReq req, string gupCode)
		{
			var containerService = new ContainerService();
			var commonService = new CommonService();
			var p81Service = new P81Service();
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f020501Repo = new F020501Repository(Schemas.CoreSchema);
			var f020503Repo = new F020503Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var data = new GetApprovedDataRes();
			var res = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

			#region 基本資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
							string.IsNullOrWhiteSpace(req.AccNo) ||
							string.IsNullOrWhiteSpace(req.DcNo) ||
							string.IsNullOrWhiteSpace(req.CustNo) ||
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			#endregion

			#region 組移動中容器清單
			data.MovingListData = f020503Repo.GetData(req.DcNo, gupCode, req.CustNo, req.AccNo, "0")
					.OrderByDescending(x => x.CRT_DATE)
					.Select(x => new GetApprovedMovingData
					{
						WarehouseId = x.PICK_WARE_ID,
						ContainerCode = x.CONTAINER_CODE
					}).ToList();
			data.MovingNum = data.MovingListData.Count;
			#endregion

			#region 檢查容器狀態、容器資料
			if (!string.IsNullOrWhiteSpace(req.ContainerCode))
			{
				req.ContainerCode = req.ContainerCode.ToUpper();

				// 檢查容器狀態
				var chkCtnRes = containerService.CheckContainer(req.ContainerCode);
				if (!chkCtnRes.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "21300", MsgContent = chkCtnRes.Message };

				if (!string.IsNullOrWhiteSpace(chkCtnRes.BinCode))
					return new ApiResult { IsSuccessed = false, MsgCode = "21324", MsgContent = p81Service.GetMsg("21324") };

				// 檢核F0701是否存在
				var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_TYPE == "0" && o.CONTAINER_CODE == chkCtnRes.ContainerCode).FirstOrDefault();
				if (f0701 == null)
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21325");
					res.Data = data;
					return res;
				}

				// 取得驗收容器上架檔資料，檢核是否有無驗收容器上架檔資料
				var f020501 = f020501Repo.GetDataByF0701Id(req.DcNo, gupCode, req.CustNo, f0701.ID);
				if (f020501 == null)
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21325");
					res.Data = data;
					return res;
				}

				data.F020501_ID = f020501.ID;
				data.WarehouseId = f020501.PICK_WARE_ID;
				var f1980 = f1980Repo.Find(o => o.DC_CODE == f020501.DC_CODE && o.WAREHOUSE_ID == f020501.PICK_WARE_ID);
				if (f1980 != null)
					data.WarehouseName = f1980.WAREHOUSE_NAME;

				// 檢核驗收容器上架檔狀態
				if (f020501.STATUS == "0") // 開箱中
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21326");
					res.Data = data;
					return res;
				}
				if (f020501.STATUS == "1") // 已關箱待複驗
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21327");
					res.Data = data;
					return res;
				}
				else if (f020501.STATUS == "2") // 可上架
				{
					data.MoveFlag = "1"; // 可上架
					data.ApiInfo = "可上架";
					res.Data = data;
					return res;
				}
				else if (f020501.STATUS == "3") // 不可上架
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21305");
					res.Data = data;
					return res;
				}
				else if (f020501.STATUS == "4") // 上架移動中
				{
					data.MoveFlag = "2"; // 到達上架處
					data.ApiInfo = "可上架";
					res.Data = data;
					return res;
				}
				else if (f020501.STATUS == "5") // 移動完成
				{
					data.MoveFlag = "0"; // 不可上架
					data.ApiInfo = p81Service.GetMsg("21328");
					res.Data = data;
					return res;
				}
			}
			#endregion

			res.Data = data;
			return res;
		}

		/// <summary>
		/// 上架移動-容器移動
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult MoveContainer(MoveContainerReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransation);
			var f020503Repo = new F020503Repository(Schemas.CoreSchema, _wmsTransation);
			var f077101Repo = new F077101Repository(Schemas.CoreSchema, _wmsTransation);
			var res = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005") };

			#region 基本資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
							string.IsNullOrWhiteSpace(req.AccNo) ||
							string.IsNullOrWhiteSpace(req.DcNo) ||
							string.IsNullOrWhiteSpace(req.CustNo) ||
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 檢核驗收容器資料對應的資料是否為空
			if (req.F020501_ID == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = p81Service.GetMsg("21306") };

			// 取得驗收資料中的容器
			var f020501 = f020501Repo.AsForUpdate().Find(o => o.ID == req.F020501_ID);
			if (f020501 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = p81Service.GetMsg("21306") };
			#endregion

			#region 資料處理
			// 1.更新驗收資料中的容器狀態 更新F020501.status = 4(上架移動中) by ID = F020501_ID
			f020501.STATUS = "4";// 上架移動中
			f020501Repo.Update(f020501);

			// 2.將移動中的容器清單寫入 容器等待上架明細檔 新增 F020503
			f020503Repo.Add(new F020503
			{
				F020501_ID = f020501.ID,
				PICK_WARE_ID = f020501.PICK_WARE_ID,
				CONTAINER_CODE = f020501.CONTAINER_CODE,
				EMP_ID = req.AccNo,
				STATUS = "0"
			});

			//3.新增移動開始時間 新增F077101
			f077101Repo.Add(new F077101
			{
				DC_CODE = f020501.DC_CODE,
				EMP_ID = req.AccNo,
				WORK_TYPE = "2",
				REF_ID = f020501.ID,
				WORKING_TIME = DateTime.Now,
				STATUS = "0"
			});

			_wmsTransation.Complete();

			//4.重撈一次帶上架明細檔 撈F020503 by EMP_ID = [AccNo] + STATUS = 0(移動中)[多筆]
      var f020503s = f020503Repo.GetData(req.DcNo, gupCode, req.CustNo, req.AccNo, "0").ToList();
      res.Data = new MoveContainerRes
			{
				MovingNum = f020503s.Count,
				MovingListData = f020503s.OrderByDescending(z => z.CRT_DATE).Select(z => new GetApprovedMovingData
				{
					WarehouseId = z.PICK_WARE_ID,
					ContainerCode = z.CONTAINER_CODE
				}).ToList()
			};
			#endregion

			return res;
		}

		/// <summary>
		/// 上架移動-移動完成
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult MoveCompleted(MoveCompletedReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransation);
			var f020503Repo = new F020503Repository(Schemas.CoreSchema, _wmsTransation);
			var f077101Repo = new F077101Repository(Schemas.CoreSchema, _wmsTransation);
			var res = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005") };

			#region 基本資料檢核

			// 帳號檢核
			var accData = p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
							string.IsNullOrWhiteSpace(req.AccNo) ||
							string.IsNullOrWhiteSpace(req.DcNo) ||
							string.IsNullOrWhiteSpace(req.CustNo) ||
							!accData.Any() ||
							accFunctionCount == 0 ||
							accCustCount == 0 ||
							accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 檢核驗收容器資料對應的資料是否為空
			if (req.F020501_ID == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = p81Service.GetMsg("21306") };

			// 取得驗收資料中的容器
			var f020501 = f020501Repo.AsForUpdate().Find(o => o.ID == req.F020501_ID);
			if (f020501 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21306", MsgContent = p81Service.GetMsg("21306") };
			#endregion

			#region 資料處理
			// 1.更新驗收資料中的容器狀態 更新F020501.status = 5(移動完成) by ID = F020501_ID
			f020501.STATUS = "5";// 移動完成
			f020501Repo.Update(f020501);

			// 2.將移動中的容器清單寫入 容器等待上架明細檔 新增 F020503
			var updF020503s = f020503Repo.GetDatasByTrueAndCondition(o => o.F020501_ID == f020501.ID).ToList();
			updF020503s.ForEach(o => { o.STATUS = "1"; });// 移動完成 
			f020503Repo.BulkUpdate(updF020503s);

			//3.新增移動開始時間 新增F077101
			f077101Repo.Add(new F077101
			{
				DC_CODE = f020501.DC_CODE,
				EMP_ID = req.AccNo,
				WORK_TYPE = "2",
				REF_ID = f020501.ID,
				WORKING_TIME = DateTime.Now,
				STATUS = "1"
			});

			_wmsTransation.Complete();

      //4.重撈一次帶上架明細檔 撈F020503 by EMP_ID = [AccNo] + STATUS = 0(移動中)[多筆]
      var f020503s = f020503Repo.GetData(req.DcNo, gupCode, req.CustNo, req.AccNo, "0").ToList();
      res.Data = new MoveContainerRes
			{
				MovingNum = f020503s.Count,
				MovingListData = f020503s.OrderByDescending(z => z.CRT_DATE).Select(z => new GetApprovedMovingData
				{
					WarehouseId = z.PICK_WARE_ID,
					ContainerCode = z.CONTAINER_CODE
				}).ToList()
			};
			#endregion

			return res;
		}
	}
}
