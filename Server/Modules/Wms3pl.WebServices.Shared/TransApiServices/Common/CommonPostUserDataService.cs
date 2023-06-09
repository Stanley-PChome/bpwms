using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Common.Security;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	/// <summary>
	/// 出貨更換物流商
	/// </summary>
	public class CommonPostUserDataService
	{
		private WmsTransaction _wmsTransation;

		public CommonPostUserDataService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region Private Property
		//工作群組存在F1953
		private List<string> _existF1953 = new List<string>();
		//工作群組不存在F1953
		private List<string> _notExistF1953 = new List<string>();
		//人員類型不存在F1925
		private List<string> _notExistF1925 = new List<string>();
		#endregion

		public ApiResult RecevieApiDatas(PostUserDataReq req)
		{
			CheckUserService ctaService = new CheckUserService();
			TransApiBaseService tacService = new TransApiBaseService();
			SharedService sharedService = new SharedService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			#region 資料檢核1

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
			{
				return res;
			}

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckOwnerCode(ref res, req);
			if (!res.IsSuccessed)
			{
				return res;
			}

			// 檢核資料總筆數與資料筆數
			int reqTotal = req.UserTotal != null ? Convert.ToInt32(req.UserTotal) : 0;
			if (req.UserList == null || (req.UserList != null && !tacService.CheckDataCount(req.UserTotal.Value, req.UserList.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.UserList.Count) };

			#endregion

			#region 資料處理
			res = ProcessApiDatas(req, req.DcCode, gupCode, req.OwnerCode);
			#endregion

			return res;
		}





		#region 資料處理
		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="req">api資料</param>
		/// <param name="gupCode">業主編號</param>
		/// <returns></returns>
		private ApiResult ProcessApiDatas(PostUserDataReq req, string dcCode, string gupCode, string custCode)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();
			List<F1924> addF1924 = new List<F1924>();
			List<F192401> addF192401 = new List<F192401>();
			List<F192402> addF192402 = new List<F192402>();
			List<F192403> addF192403 = new List<F192403>();
			List<F1952> addF1952 = new List<F1952>();
			List<F1952_HISTORY> addF1952History = new List<F1952_HISTORY>();
			List<F060301> addF060301 = new List<F060301>();
			List<F1924> updF1924 = new List<F1924>();
			List<F192401> updF192401 = new List<F192401>();

			var f1924Repo = new F1924Repository(Schemas.CoreSchema, _wmsTransation);
			var f192401Repo = new F192401Repository(Schemas.CoreSchema, _wmsTransation);
			var f192402Repo = new F192402Repository(Schemas.CoreSchema, _wmsTransation);
			var f1925Repo = new F1925Repository(Schemas.CoreSchema, _wmsTransation);
			var f192403Repo = new F192403Repository(Schemas.CoreSchema, _wmsTransation);
			var f1952Repo = new F1952Repository(Schemas.CoreSchema, _wmsTransation);
			var f1952HistroyRepo = new F1952_HISTORYRepository(Schemas.CoreSchema, _wmsTransation);
			var f060301Repo = new F060301Repository(Schemas.CoreSchema, _wmsTransation);
			var fF1963Repo = new F1963Repository(Schemas.CoreSchema, _wmsTransation);

			foreach (var user in req.UserList)
			{
				var res1 = CheckUser(gupCode, custCode, user);
				if (!res1.IsSuccessed)
				{
					return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = res1.Data };
				}
				else
				{
					// 檢查人員帳號是否存在
					var f1924 = f1924Repo.GetF1924ByEmpId(user.UserId).FirstOrDefault();
					if (f1924 == null)
					{
						// 新增人員主檔
						addF1924.Add(CreateF1924(user.UserId, user.UserName, user.Status.Value, user.UserType));
						// 新增人員所屬工作群組
						if (!string.IsNullOrWhiteSpace(user.UserGroup))
						{
							_existF1953.ForEach(grpId =>
							{
								var existUserGroup = user.UserGroup.Split(',').ToList().Any(x => x.ToString() == grpId);
								if (existUserGroup)
								{
									addF192401.Add(CreateF192401(user.UserId, grpId));
								}
							});
						}
						// 新增人員操作貨主檔
						addF192402.Add(CreateF192402(user.UserId, dcCode, gupCode, custCode));
						// 人員所屬作業群組
						if(fF1963Repo.GetDatasByTrueAndCondition(x=>x.WORK_ID == 1).Any())
						{
							addF192403.Add(CreateF192403(user.UserId));
						}
						// 新增人員入密碼檔
						addF1952.Add(CreateF1952(user.UserId, user.UserPw));
						// 新增人員密碼修改歷史紀錄
						addF1952History.Add(CreateF1952_HISTORY(user.UserId, user.UserPw));
					}
					else
					{
						// 更新人員主檔
						string status = user.Status == 1 ? "0" : "1";
						f1924Repo.updateF1924(user.UserId, user.UserName, status, user.UserType);
					}

					// 新增人員主檔派發作業
					addF060301.Add(CreateF060301(dcCode, user.UserId, user.Status.Value));
				}
			}

			f1924Repo.BulkInsert(addF1924);
			f192401Repo.BulkInsert(addF192401);
			f192402Repo.BulkInsert(addF192402);
			f192403Repo.BulkInsert(addF192403);
			f1952Repo.BulkInsert(addF1952);
			f1952HistroyRepo.BulkInsert(addF1952History);
			f060301Repo.BulkInsert(addF060301);

			_wmsTransation.Complete();

			var msgContent = string.Empty;
			if (_notExistF1925.Any())
			{
				var msg = string.Format(tacService.GetMsg("20001"), string.Join(",", _notExistF1925));
				msgContent += string.IsNullOrWhiteSpace(msgContent) ? msg : $";{msg}";
			}

			if (_notExistF1953.Any())
			{
				var msg = string.Format(tacService.GetMsg("20002"), string.Join(",", _notExistF1953));
				msgContent += string.IsNullOrWhiteSpace(msgContent) ? msg : $";{msg}";
			}

			res.IsSuccessed = !data.Any();
			res.MsgCode = data.Any() ? "400-001" : "200";
			res.MsgContent = msgContent;
			res.Data = data.Any() ? data : null;
			return res;
        }
		
		#endregion

		private ApiResult CheckUser(string gupCode, string custCode, PostUserDataResult userData)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(gupCode, custCode, userData).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(gupCode, custCode, userData).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(gupCode, custCode, userData).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(gupCode, custCode, userData).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(gupCode, custCode, userData).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}

		#region Protected 檢核

		protected ApiResult CheckDefaultSetting(string gupCode, string custCode, PostUserDataResult userData)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="returns">客戶退貨單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string gupCode, string custCode, PostUserDataResult userData)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 定義需檢核欄位、必填、型態、長度
			// 帳號列表
			List<ApiCkeckColumnModel> checkColumnList = new List<ApiCkeckColumnModel>();


			checkColumnList = new List<ApiCkeckColumnModel>
			{
				new ApiCkeckColumnModel{  Name = "UserId",          Type = typeof(string),   MaxLength = 32,  Nullable = false },
				new ApiCkeckColumnModel{  Name = "UserName",        Type = typeof(string),   MaxLength = 32,   Nullable = false },
				new ApiCkeckColumnModel{  Name = "UserPw",          Type = typeof(string),   MaxLength = 64,   Nullable = false },
				new ApiCkeckColumnModel{  Name = "Status",          Type = typeof(int),   MaxLength = 64,   Nullable = false },
				new ApiCkeckColumnModel{  Name = "UserType",        Type = typeof(string),   MaxLength = 5,   Nullable = false },
				new ApiCkeckColumnModel{  Name = "UserGroup",       Type = typeof(string),   MaxLength = 100,   Nullable = true }
			};

			#endregion

			#region 檢查帳號列表必填、最大長度
			List<string> returnIsNullList = new List<string>();
			List<ApiCkeckColumnModel> returnIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			checkColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(userData, column.Name))
					{
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(tacService.GetMsg("20034").Split('，')[0], column.Name), UserId = userData.UserId });
					}
						//returnIsNullList.Add(column.Name);
					
				}

				// 最大長度
				if (column.MaxLength > 0)
				{
					if (!DataCheckHelper.CheckDataMaxLength(userData, column.Name, column.MaxLength))
					{
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(tacService.GetMsg("20035").Split('，')[0], column.Name),UserId = userData.UserId });
					
					}
						//returnIsExceedMaxLenthList.Add(column);
				}
			});
			
			//// 必填訊息
			//if (returnIsNullList.Any())
			//{
			//	data.Add(new ApiResponse { No = userData.UserId, MsgCode = "20020", MsgContent = string.Format(tacService.GetMsg("20020"), userData.UserId, string.Join("、", returnIsNullList) ) });
			//}

			//// 最大長度訊息
			//if (returnIsExceedMaxLenthList.Any())
			//{
			//	List<string> errorMsgList = returnIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

			//	string errorMsg = string.Join("、", errorMsgList);

			//	data.Add(new ApiResponse { No = userData.UserId, MsgCode = "20021", MsgContent = string.Format(tacService.GetMsg("20021"), userData.UserId, string.Join("、", errorMsg)) });
			//}
			#endregion



			res.Data = data;

			return res;
		}


		protected ApiResult CheckCustomColumnType(string gupCode, string custCode, PostUserDataResult userData)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		protected ApiResult CheckCommonColumnData(string gupCode, string custCode, PostUserDataResult userData)
		{
			CheckUserService cuService = new CheckUserService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 主檔欄位資料檢核
			// 檢查UserType是否存在F1925.DEP_ID
			cuService.CheckUserType(data, userData, _notExistF1925);
			// 檢查UserGroup是否存在F1953.GRP_ID
			cuService.CheckUserGroup(data, _existF1953,_notExistF1953, userData);
			
			#endregion

			res.Data = data;

			return res;
		}

		protected ApiResult CheckCustomColumnValue( string gupCode, string custCode, PostUserDataResult userData)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion


		protected F1924 CreateF1924(string empId,string empName, int status, string userType)
		{
			return new F1924
			{
				EMP_ID = empId,
				EMP_NAME = empName,
				ISDELETED = status == 0 ? "1" : "0",
				PACKAGE_UNLOCK = "0",
				ISCOMMON = "0",
				DEP_ID = userType,
				MENUSTYLE = "0",
			};
		}

		protected F192401 CreateF192401(string userId, string userGroup)
		{
			return new F192401
			{
				EMP_ID = userId,
				GRP_ID = Convert.ToDecimal(userGroup)
			};
		}

		protected F192402 CreateF192402(string userId,string dcCode,string gupCode, string custCode)
		{
			return new F192402
			{
				EMP_ID = userId,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
			};
		}

		protected F192403 CreateF192403(string userId)
		{
			return new F192403
			{
				EMP_ID = userId,
				WORK_ID = 1
			};
		}

		protected F1952 CreateF1952(string userId,string userPw)
		{
			return new F1952
			{
				EMP_ID = userId,
				PASSWORD = CryptoUtility.GetHashString(userPw),
				LAST_PASSWORD_CHANGED_DATE = DateTime.Now,
				FAILED_PASSWORD_ATTEMPT_COUNT = 0,
				STATUS = 0
			};
		}

		protected F1952_HISTORY CreateF1952_HISTORY(string userId, string userPw)
		{
			return new F1952_HISTORY
			{
				EMP_ID = userId,
				PASSWORD = CryptoUtility.GetHashString(userPw)
			};
		}

		protected F060301 CreateF060301(string dcCode,string userId,int status)
		{
			return new F060301
			{
				DC_CODE = dcCode,
				WAREHOUSE_ID = "ALL",
				EMP_ID = userId,
				CMD_TYPE = status == 0 ? "2" : "1",
				STATUS = "0",
				RESENT_CNT = 0
			};
		}
	}
}
