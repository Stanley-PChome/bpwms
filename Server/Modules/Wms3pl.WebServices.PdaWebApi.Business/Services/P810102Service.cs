using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Common.Security;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public partial class P810102Service
	{
		private WmsTransaction _wmsTransation;
		public P810102Service(WmsTransaction wmsTransation = null)
		{
			_wmsTransation = wmsTransation;

		}
		public ApiResult PostLogout(PostLogoutReq postLogoutReq)
		{
			var p81Service = new P81Service();
			var f0050Repo = new F0050Repository(Schemas.CoreSchema);

			// 帳號檢核
			var checkAcc = p81Service.CheckAcc(postLogoutReq.AccNo).FirstOrDefault();

			// 檢核人員功能權限
			var checkAccFunction = p81Service.CheckAccFunction(postLogoutReq.FuncNo, postLogoutReq.AccNo);

			// 檢查參數資料是否為空值
			if (!string.IsNullOrWhiteSpace(postLogoutReq.FuncNo) ||
					!string.IsNullOrWhiteSpace(postLogoutReq.AccNo) ||
					!string.IsNullOrWhiteSpace(postLogoutReq.DevCode) ||
					checkAcc != null ||
					checkAccFunction > 0)
			{
				// 新增 使用者登出紀錄insert F0050 
				f0050Repo.Add(new F0050
				{
					LOG_DATE = DateTime.Now,
					MACHINE = postLogoutReq.DevCode,
					MESSAGE = "fixed code",
					FUN_ID = postLogoutReq.FuncNo,
					FUNCTION_NAME = p81Service.GetFunName(postLogoutReq.FuncNo),
					CRT_STAFF = Current.Staff,
					CRT_NAME = Current.StaffName
				});

				// 刪除登入紀錄[帳號、裝置驗證碼]
				p81Service.DeleteLoginLog(postLogoutReq.AccNo, postLogoutReq.DevCode);

				return new ApiResult { IsSuccessed = true, MsgCode = "10102", MsgContent = p81Service.GetMsg("10102") };
			}
			else
			{
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };
			}
		}

		public ApiResult PostLogin(PostLoginReq postLoginReq, string scCode)
		{
			var p81Service = new P81Service();
			var f192401Repo = new F192401Repository(Schemas.CoreSchema);
			var f192402Repo = new F192402Repository(Schemas.CoreSchema);
			var f0050Repo = new F0050Repository(Schemas.CoreSchema);
			var f0070Repo = new F0070Repository(Schemas.CoreSchema);
			var f192403Repo = new F192403Repository(Schemas.CoreSchema);
			var f1924Repo = new F1924Repository(Schemas.CoreSchema);
			var f1952Repo = new F1952Repository(Schemas.CoreSchema);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var f1945Repo = new F1945Repository(Schemas.CoreSchema);
			var f194501Repo = new F194501Repository(Schemas.CoreSchema);
			var f056001Repo = new F056001Repository(Schemas.CoreSchema);
			var f1955Repo = new F1955Repository(Schemas.CoreSchema);
      var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var commonService = new CommonService();

			// 檢核功能編號是否等於空值
			// 檢核帳號是否等於空值
			// 檢核密碼是否等於空值
			// 檢核裝置驗證碼是否等於空值
			// 檢核機器號碼是否等於空值
			if (string.IsNullOrWhiteSpace(postLoginReq.FuncNo) ||
				 string.IsNullOrWhiteSpace(postLoginReq.AccNo) ||
				 string.IsNullOrWhiteSpace(postLoginReq.Pwd) ||
				 string.IsNullOrWhiteSpace(postLoginReq.DevCode) ||
				 string.IsNullOrWhiteSpace(postLoginReq.McCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "20151", MsgContent = p81Service.GetMsg("20151") };

			// 帳號密碼檢核
			var user = f1952Repo.ValidateUser(postLoginReq.AccNo);
			if (user == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20151", MsgContent = p81Service.GetMsg("20151") };

			var compareHash = CryptoUtility.CompareHash(postLoginReq.Pwd, user.Password);
			if (!compareHash)
				return new ApiResult { IsSuccessed = false, MsgCode = "20151", MsgContent = p81Service.GetMsg("20151") };

			// 檢核帳號是否已登入在其他裝置
			var checkLoginLog = p81Service.CheckLoginLog(postLoginReq.AccNo, postLoginReq.McCode);
			if (checkLoginLog)
				return new ApiResult { IsSuccessed = false, MsgCode = "20154", MsgContent = p81Service.GetMsg("20154") };

			// 新增登入紀錄
			p81Service.InsertLoginLog(postLoginReq.McCode, postLoginReq.AccNo, postLoginReq.DevCode);

			// INSERT F0050
			f0050Repo.Add(new F0050
			{
				LOG_DATE = DateTime.Now,
				MACHINE = postLoginReq.DevCode,
				MESSAGE = "登入 (fixed code)",
				FUN_ID = postLoginReq.FuncNo,
				FUNCTION_NAME = p81Service.GetFunName(postLoginReq.FuncNo),
				CRT_STAFF = Current.Staff,
				CRT_NAME = Current.StaffName
			});

			// PDA是否同步
			var pdaSync = commonService.GetSysGlobalValue("PdaSync");
			var isSync = string.IsNullOrWhiteSpace(pdaSync) ? "0" : pdaSync;

			// 取得人員功能清單
			IQueryable<FuncList> getUserFuncList = f192401Repo.GetUserFuncList(postLoginReq.AccNo, isSync);

			// 取得人員物流中心清單
			IQueryable<DcList> getUserDcList = f192402Repo.GetUserDcList(postLoginReq.AccNo);

			// 取得人員業主清單
			IQueryable<GupList> getUserGupList = f192402Repo.GetUserGupList(postLoginReq.AccNo);

			// 取得人員貨主清單
			IQueryable<CustList> getUserCustList = f192402Repo.GetUserCustList(postLoginReq.AccNo);

			// 取得人員資料
			UserInfo getUserInfo = f1924Repo.GetUserInfo(postLoginReq.AccNo);

			getUserInfo.SCCode = scCode;

			// 取得人員倉別資料
			IQueryable<WarehouseInfo> getUserWarehouse = f192403Repo.GetUserWarehouse(postLoginReq.AccNo);

			// 取得集貨場資料
			IQueryable<CollectionInfo> getUserCollectionList = f1945Repo.GetCollectionCode(postLoginReq.AccNo);

			// 取得集貨場儲格資料
			IQueryable<CellInfo> getUserCollList = f194501Repo.GetCellType(postLoginReq.AccNo);

			// 取得取得紙箱工作站樓層
			IQueryable<CartonWorkStationFloorInfo> getCartonWorkStationFloor = f056001Repo.GetCartonWorkStationFloor();
			
			// 取得便利倉資料
			IQueryable<ConvenientInfo> getConvenientList = f1955Repo.GetConvenient(postLoginReq.AccNo);

			// 取得APK版本號
			string getVersionNo = f0003Repo.GetVersionNo();

			//取得ApkUrl
			string getApkUrl = f0003Repo.GetApkUrl();

      //取得跨庫調撥驗收入自動倉清單
      List<MoveInAutoWarehouseInfo> getUserMoveInAutoWarehouse = new List<MoveInAutoWarehouseInfo>();
      foreach (var item in getUserDcList.ToList())
      {
        var MoveInAutoWhIdSetup = commonService.GetSysGlobalValue(item.DcNo, "MoveInAutoWhId");
        if (string.IsNullOrWhiteSpace(MoveInAutoWhIdSetup))
          continue;

        var f1980s = f1980Repo.GetDatas(item.DcNo, MoveInAutoWhIdSetup.Split(',').ToList()).ToList();
        if (f1980s != null && f1980s.Any())
          getUserMoveInAutoWarehouse.AddRange(
            f1980s.Select(x => new MoveInAutoWarehouseInfo
            {
              DcNo = item.DcNo,
              WhName = x.WAREHOUSE_NAME,
              WhNo = x.WAREHOUSE_ID
            }));
      }

      PostLoginRes postLoginRes = new PostLoginRes()
      {
        FuncList = getUserFuncList.ToList(),
        DcList = getUserDcList.ToList(),
        GupList = getUserGupList.ToList(),
        CustList = getUserCustList.ToList(),
        UserInfo = getUserInfo,
        WarehouseInfo = getUserWarehouse.ToList(),
        PdaInfo = new PdaInfo { VersionNo = getVersionNo, ApkUrl = getApkUrl, IsSync = isSync },
        CollectionInfo = getUserCollectionList.ToList(),
        CellInfo = getUserCollList.ToList(),
        CartonWorkStationFloorInfo = getCartonWorkStationFloor.ToList(),
        ConvenientInfo = getConvenientList.ToList(),
        MoveInAutoWarehouseInfo = getUserMoveInAutoWarehouse
      };

			return new ApiResult { IsSuccessed = true, MsgCode = "10101", MsgContent = p81Service.GetMsg("MsgContent"), Data = postLoginRes };
		}
	}
}