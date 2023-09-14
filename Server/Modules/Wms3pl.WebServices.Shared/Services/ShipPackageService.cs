using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.WcsServices;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.Shared.Services
{
	public class ShipPackageService
	{
		#region Repository

		private F055007Repository _f055007Repo;
		public F055007Repository F055007Repo
		{
			get { return _f055007Repo == null ? _f055007Repo = new F055007Repository(Schemas.CoreSchema, _wmsTransaction) : _f055007Repo; }
			set { _f055007Repo = value; }
		}

		private F05500101Repository _f05500101RepoNoTrans;
		public F05500101Repository F05500101RepoNoTrans
		{
			get { return _f05500101RepoNoTrans == null ? _f05500101RepoNoTrans = new F05500101Repository(Schemas.CoreSchema) : _f05500101RepoNoTrans; }
			set { _f05500101RepoNoTrans = value; }
		}

		private F05500101Repository _f05500101Repo;
		public F05500101Repository F05500101Repo
		{
			get { return _f05500101Repo == null ? _f05500101Repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction) : _f05500101Repo; }
			set { _f05500101Repo = value; }
		}

		private F050801Repository _f050801Repo;
		public F050801Repository F050801Repo
		{
			get { return _f050801Repo == null ? _f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction) : _f050801Repo; }
			set { _f050801Repo = value; }
		}

		private F050801Repository _f050801RepoNoTrans;
		public F050801Repository F050801RepoNoTrans
		{
			get { return _f050801RepoNoTrans == null ? _f050801RepoNoTrans = new F050801Repository(Schemas.CoreSchema) : _f050801RepoNoTrans; }
			set { _f050801RepoNoTrans = value; }
		}

		private F055001Repository _f055001Repo;
		public F055001Repository F055001Repo
		{
			get { return _f055001Repo == null ? _f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction) : _f055001Repo; }
			set { _f055001Repo = value; }
		}

		private F055001Repository _f055001RepoNoTrans;
		public F055001Repository F055001RepoNoTrans
		{
			get { return _f055001RepoNoTrans == null ? _f055001RepoNoTrans = new F055001Repository(Schemas.CoreSchema) : _f055001RepoNoTrans; }
			set { _f055001RepoNoTrans = value; }
		}

		private F050301Repository _f050301Repo;
		public F050301Repository F050301Repo
		{
			get { return _f050301Repo == null ? _f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction) : _f050301Repo; }
			set { _f050301Repo = value; }
		}

		private F051301Repository _f051301Repo;
		public F051301Repository F051301Repo
		{
			get { return _f051301Repo == null ? _f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction) : _f051301Repo; }
			set { _f051301Repo = value; }
		}

		private F060208Repository _f060208Repo;
		public F060208Repository F060208Repo
		{
			get { return _f060208Repo == null ? _f060208Repo = new F060208Repository(Schemas.CoreSchema, _wmsTransaction) : _f060208Repo; }
			set { _f060208Repo = value; }
		}

		private F060208Repository _f060208RepoNoTrans;
		public F060208Repository F060208RepoNoTrans
		{
			get { return _f060208RepoNoTrans == null ? _f060208RepoNoTrans = new F060208Repository(Schemas.CoreSchema) : _f060208RepoNoTrans; }
			set { _f060208RepoNoTrans = value; }
		}

		private F160204Repository _f160204Repo;
		public F160204Repository F160204Repo
		{
			get { return _f160204Repo == null ? _f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction) : _f160204Repo; }
			set { _f160204Repo = value; }
		}

		private F056001Repository _f056001Repo;
		public F056001Repository F056001Repo
		{
			get { return _f056001Repo == null ? _f056001Repo = new F056001Repository(Schemas.CoreSchema, _wmsTransaction) : _f056001Repo; }
			set { _f056001Repo = value; }
		}

		private F05030101Repository _f05030101RepoNoTrans;
		public F05030101Repository F05030101RepoNoTrans
		{
			get { return _f05030101RepoNoTrans == null ? _f05030101RepoNoTrans = new F05030101Repository(Schemas.CoreSchema) : _f05030101RepoNoTrans; }
			set { _f05030101RepoNoTrans = value; }
		}

		private F055002Repository _f055002Repo;
		public F055002Repository F055002Repo
		{
			get { return _f055002Repo == null ? _f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction) : _f055002Repo; }
			set { _f055002Repo = value; }
		}

		private F050104Repository _f050104RepoNoTrans;
		public F050104Repository F050104RepoNoTrans
		{
			get { return _f050104RepoNoTrans == null ? _f050104RepoNoTrans = new F050104Repository(Schemas.CoreSchema) : _f050104RepoNoTrans; }
			set { _f050104RepoNoTrans = value; }
		}

		private F05030201Repository _f05030201RepoNoTrans;
		public F05030201Repository F05030201RepoNoTrans
		{
			get { return _f05030201RepoNoTrans == null ? _f05030201RepoNoTrans = new F05030201Repository(Schemas.CoreSchema) : _f05030201RepoNoTrans; }
			set { _f05030201RepoNoTrans = value; }
		}

		private F160202Repository _f160202RepoNoTrans;
		public F160202Repository F160202RepoNoTrans
		{
			get { return _f160202RepoNoTrans == null ? _f160202RepoNoTrans = new F160202Repository(Schemas.CoreSchema) : _f160202RepoNoTrans; }
			set { _f160202RepoNoTrans = value; }
		}

		private F1951Repository _f1951RepoNoTrans;
		public F1951Repository F1951RepoNoTrans
		{
			get { return _f1951RepoNoTrans == null ? _f1951RepoNoTrans = new F1951Repository(Schemas.CoreSchema) : _f1951RepoNoTrans; }
			set { _f1951RepoNoTrans = value; }
		}

		private F1908Repository _f1908RepoNoTrans;
		public F1908Repository F1908RepoNoTrans
		{
			get { return _f1908RepoNoTrans == null ? _f1908RepoNoTrans = new F1908Repository(Schemas.CoreSchema) : _f1908RepoNoTrans; }
			set { _f1908RepoNoTrans = value; }
		}

		#endregion Repository


		private WmsTransaction _wmsTransaction;
		private CommonService _commonService;
		public CommonService CommonService
		{
			get
			{
				if (_commonService == null)
					_commonService = new CommonService();
				return _commonService;
			}
		}
		public ShipPackageService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 出貨容器條碼檢核
		/// </summary>
		/// <param name="req"></param>
		public CheckShipContainerCodeRes CheckShipContainerCode(CheckShipContainerCodeReq req)
		{
			var logService = new LogService("ShipPackage_" + DateTime.Now.ToString("yyyyMMdd"));
			logService.Log("出貨容器條碼檢核 開始");
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);

			F050801 f050801 = null;

			// 檢查ContainerCode是否有值，若無值或null，回傳訊息[false,請刷讀容器條碼]
			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = "請刷讀容器條碼" } };

			// <參數4>如果有值，請先轉成大寫
			req.ContainerCode = req.ContainerCode.ToUpper();

			// 如果[CC]第一碼是O，取得出貨單資料
			if (req.ContainerCode.Substring(0, 1) == "O")
			{
				f050801 = F050801Repo.Find(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.ContainerCode);
				if (f050801 != null)
				{
					return new CheckShipContainerCodeRes
					{
						Result = new ExecuteResult { IsSuccessed = true },
						DcCode = f050801.DC_CODE,
						GupCode = f050801.GUP_CODE,
						CustCode = f050801.CUST_CODE,
						WmsNo = f050801.WMS_ORD_NO,
						IsSpecialOrder = false,
						PickType = null
					};
				}
				logService.Log("輸入為O單，取得出貨單資料完成");
			}

			// 檢查容器條碼是否存在[F0701]
			var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == req.ContainerCode && o.CONTAINER_TYPE == "0").FirstOrDefault();
			if (f0701 == null)
				return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = $"容器{req.ContainerCode}不存在" } };

			// 取得容器綁定的單號[F070101]
			var f070101 = f070101Repo.GetDatasByTrueAndCondition(o => o.F0701_ID == f0701.ID).FirstOrDefault();

			logService.Log("找到容器" + req.ContainerCode + "對應單號" + f070101.WMS_NO);
			// 如果[B].WMS_NO第一碼非O或P則回傳訊息[false,您刷讀的容器條碼非出貨使用的容器]
			var firstYardWmsNo = f070101.WMS_NO?.Substring(0, 1);
			var WmsNo = f070101.WMS_NO;
			if (firstYardWmsNo != "O" && firstYardWmsNo != "P")
				return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = "您刷讀的容器條碼非出貨使用的容器" } };

			F051201 f051201 = null;
			// 如果[B].WMS_NO第一碼為P
			if (firstYardWmsNo == "P")
			{
				f051201 = f051201Repo.Find(o => o.DC_CODE == f070101.DC_CODE && o.GUP_CODE == f070101.GUP_CODE && o.CUST_CODE == f070101.CUST_CODE && o.PICK_ORD_NO == f070101.WMS_NO);

				// 如果揀貨單[C]不是特殊結構揀貨單=>[C].PICK_TYPE NOT IN(4,7,8)，則回傳訊息[false,您刷讀的容器條碼非出貨使用的容器]
				var pickTypes = new List<string> { "4", "7", "8" };
				if (!pickTypes.Contains(f051201.PICK_TYPE))
					return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = "您刷讀的容器條碼非出貨使用的容器" } };
				logService.Log("檢查是否完成集貨");

				// [E]=檢查是否完成集貨 資料表: F051301 條件:DC_CODE = [B].DC_CODE GUP_CODE = [B].GUP_CODE CUST_CODE = [B].CUST_CODE WMS_NO = [B].WMS_NO STATUS NOT IN(1)
				var f051301s = F051301Repo.GetDataByChkShip(f070101.DC_CODE, f070101.GUP_CODE, f070101.CUST_CODE, f070101.WMS_NO);

				// 如果[E]有資料，回傳訊息[false,您刷讀的容器條碼尚未完成集貨，不可出貨]
				if (f051301s.Any())
					return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = "您刷讀的容器條碼尚未完成集貨，不可出貨" } };
			}

			// 如果<參數5> = 2(包裝線包裝站)
			if (req.ShipMode == "2")
			{
				logService.Log("取得包裝線容器是否已抵達 開始");

				var isExist = F060208Repo.CheckIsArrival(f070101.DC_CODE, f070101.GUP_CODE, f070101.CUST_CODE, req.ContainerCode, f070101.WMS_NO);
				// [D] =0，則回傳訊息[false,您刷讀的容器未回報抵達工作站]
				if (!isExist)
					return new CheckShipContainerCodeRes { Result = new ExecuteResult { IsSuccessed = false, Message = "您刷讀的容器未回報抵達工作站" } };
				logService.Log("取得包裝線容器是否已抵達 結束");
			}

			logService.Log("出貨容器條碼檢核 結束");
			// 回傳結果
			return new CheckShipContainerCodeRes
			{
				Result = new ExecuteResult { IsSuccessed = true },
				DcCode = f070101.DC_CODE,
				GupCode = f070101.GUP_CODE,
				CustCode = f070101.CUST_CODE,
				WmsNo = f070101.WMS_NO,
				IsSpecialOrder = firstYardWmsNo == "P",
				PickType = firstYardWmsNo == "P" && f051201 != null ? f051201.PICK_TYPE : null
			};
		}

		/// <summary>
		/// 查詢與檢核出貨單資訊
		/// </summary>
		/// <param name="req"></param>
		public SearchAndCheckWmsOrderInfoRes SearchAndCheckWmsOrderInfo(SearchAndCheckWmsOrderInfoReq req)
		{
			var logService = new LogService("ShipPackage_" + DateTime.Now.ToString("yyyyMMdd"));
			logService.Log("查詢與檢核出貨單資訊 開始");

			var distibuteService = new DistibuteService();
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051206Repo = new F051206Repository(Schemas.CoreSchema);
			var f05120601Repo = new F05120601Repository(Schemas.CoreSchema);
			var f052903Repo = new F052903Repository(Schemas.CoreSchema);
			var f050101Repo = new F050101Repository(Schemas.CoreSchema);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);

			if (_commonService == null)
			{
				_commonService = new CommonService();
			}
			logService.Log("取得出貨單資料 開始");
			#region 取得出貨單資料
			var f050801 = F050801Repo.GetF050801(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
			//var f050801 = F050801Repo.AsForUpdate().Find(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo);
			if (f050801 == null)
				return new SearchAndCheckWmsOrderInfoRes { Result = new ExecuteResult { IsSuccessed = false, Message = "出貨單不存在" } };

			if (f050801.CUST_COST == "MoveOut")
				return new SearchAndCheckWmsOrderInfoRes { Result = new ExecuteResult { IsSuccessed = false, Message = "跨庫出貨單不允許進行出貨包裝，請至跨庫整箱出貨/新稽核出庫處理" } };

			int boxCnt = 0;
			if (req.ShipMode == "1")// 單人包裝站
									// 取得箱數[Count(DISTINCT CONTAINER_CODE) 資料表: F070101 條件: DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND WMS_NO = < 參數4 >
				boxCnt = f070101Repo.GetData(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).Count();
			else if (req.ShipMode == "2")// 包裝線包裝站
										 // 取得箱數[TOP 1 F060208.BOX_TOTAL 資料表: F060208 條件: DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND ORI_ORDER_CODE = < 參數4 > AND STATUS <> 9 AND POSITION_CODE = TARGET_POS_CODE 排序: ORDER BY CRT_DATE DESC
				boxCnt = F060208Repo.GetTop1BoxTotal(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, new List<int> { 9 });
			var result = new SearchAndCheckWmsOrderInfoRes
			{
				DcCode = f050801.DC_CODE,
				GupCode = f050801.GUP_CODE,
				CustCode = f050801.CUST_CODE,
				WmsOrdNo = f050801.WMS_ORD_NO,
				Status = f050801.STATUS,
				DelvDate = f050801.DELV_DATE,
				PickTime = f050801.PICK_TIME,
				IsPackCheck = f050801.ISPACKCHECK == 0 ? "否" : "是",
				SugBoxNo = f050801.SUG_BOX_NO,
				BoxCnt = boxCnt,
				OrderType = f050801.SOURCE_TYPE == "13" ? "02" : "01",
				PackingType = f050801.PACKING_TYPE,
				ItemList = f050802Repo.GetDatasByShipPackage(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO).ToList(),
				CartonItemList = f1903Repo.GetF1903sByCarton(f050801.GUP_CODE, f050801.CUST_CODE, "1").Select(x => new BoxItemModel
				{
					ItemCode = x.ITEM_CODE,
					ItemName = x.ITEM_NAME
				}).ToList()
			};
			#endregion
			logService.Log("取得出貨單資料 結束");
			logService.Log("檢核 開始");
      #region 檢核
      // 檢查訂單是否取消
      var f050101 = f050101Repo.GetDataByWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, f050801.WMS_ORD_NO);
      if (f050101 != null && f050101.STATUS == "9")
        if (req.ShipMode == "2")
          return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此訂單已取消，請將容器移至異常區，請手動按下取消到站紀錄" }, true);
        else
          return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此訂單已取消" });
      // 檢查出貨單據狀態
      if (f050801.STATUS == 9)// 如果[A].STATUS =9 回傳訊息[false,此出貨單已取消]
				if (req.ShipMode == "2")
				{
					F060208RepoNoTrans.UpdateProcFlag(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, 3, new List<int> { 9 });
					return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單已取消，請將容器移至異常區，請手動按下取消到站紀錄" }, true);
				}
				else
					return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單已取消" });
			else if (f050801.STATUS == 2) // 如果[A].STATUS =2 回傳訊息[false,此出貨單已包裝完成]
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單已包裝完成" });
			else if (f050801.STATUS == 5) // 如果[A].STATUS =5 回傳訊息[false,此出貨單已出貨]
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單已出貨" });
			else if (f050801.STATUS == 6) // 如果[A].STATUS =6 回傳訊息[false,6	單據狀態	已扣帳]
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單已扣帳" });

			// 如果[A].SHIP_MODE=1，回傳訊息[false,此出貨單必須到原出貨包裝功能出貨]
			if (f050801.SHIP_MODE == "1")
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單必須到原出貨包裝功能出貨" });


			// 檢查出貨單是否揀貨完成
			var f051202s = f051202Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.PICK_STATUS == "0");

			// 檢查出貨單是否揀貨完成
			if (f051202s.Any())
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "此出貨單尚未完成揀貨" });

			// [MM] = 檢查出貨單是否有缺貨未確認或貨主回覆訂單被取消[F051206]s
			var f051206s = f051206Repo.GetDatasByWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).ToList();

			// 如果[MM]有任何一筆RETURN_FLAG=2 顯示訊息[false,貨主回覆訂單取消，不可出貨]
			if (f051206s.Any(x => x.RETURN_FLAG == "2"))
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "貨主回覆訂單取消，不可出貨" });

			// 如果[MM]有任何一筆 RETURN_FLAG 是空白或是NULL，顯示訊息[false, 缺貨待確認，不可包裝]
			if (f051206s.Any(x => string.IsNullOrWhiteSpace(x.RETURN_FLAG)))
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "缺貨待確認，不可包裝" });



			var f05120601s = f05120601Repo.GetDatasByWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).ToList();
			if (f05120601s.Any())
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "尚有揀缺未配庫資料，不可包裝" });


			// [TT]=檢查是否出貨單未完成分貨
			var f052903s = f052903Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.STATUS == "0");

			// 如果[TT]有存在，顯示訊息[false,該出貨單尚未完成分貨，不可出貨]
			if (f052903s.Any())
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "該出貨單尚未完成分貨，不可出貨" });


			// [SS]=檢查是否為未完成集貨出貨單
			var f051301 = F051301Repo.Find(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_NO == req.WmsOrdNo);

      // 如果[SS]存在，但[SS].STATUS !=1，則顯示訊息[false,該出貨單尚未完成集貨，不可出貨]
      if (f051301 != null && f051301.STATUS != "1")
      {
        if (req.ShipMode == "1") //單人包裝站
          return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "該出貨單尚未完成集貨，不可出貨" });
        else if (req.ShipMode == "2" && f051301.COLLECTION_POSITION == "0") //包裝線包裝站
          return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = "該出貨單尚未完成集貨，不可出貨" });
      }

			// 更新出貨單出貨模式為2
			var checkPackageModeResult = CheckPackageMode(f050801, "2");
			if (!checkPackageModeResult.IsSuccessed)
				return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = false, Message = checkPackageModeResult.MsgContent });
			#endregion
			logService.Log("檢核 結束");

			logService.Log("呼叫WcssrApi配箱資訊同步 開始");

			#region 呼叫WcssrApi配箱資訊同步
			// 包裝作業與影資系統整合(0:否、1:是)
			var f0003ByVCIP = _commonService.GetSysGlobalValue(req.DcCode, "VideoCombinInPack");
			var videoCombinInPack = f0003ByVCIP != null && f0003ByVCIP == "0" ? "0" : "1";

			// IF [B] ==1 (啟用)
			if (videoCombinInPack == "1")
			{
				// WcssrApi配箱資訊同步
				var wcssrApiRes = distibuteService.DistibuteInfoAsync(req.DcCode, req.GupCode, req.CustCode, new DistibuteInfoAsyncReq
				{
					WhId = req.DcCode,
					OutboundNo = req.WmsOrdNo,
					WorkStationId = req.WorkStationId,
					OperationUserId = Current.Staff,
					TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
				});
			}
			#endregion

			logService.Log("呼叫WcssrApi配箱資訊同步 結束");
			logService.Log("資料處理 開始");
			#region 資料處理
			// ShipMode= 2(包裝線包裝站)，更新容器位置回報將狀態等於等待中更新為已完成 (1) 更新F060208.PROC_FLAG = 2條件 DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND ORI_ORDER_CODE = < 參數4 > AND STATUS = 0
			if (req.ShipMode == "2")
				F060208Repo.UpdateProcFlag(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, 2);


			if (f050801.STATUS == 0)
				LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "開始包裝", 0, null);

      F050801Repo.UpdatePackStart(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, f050801.PACK_START_TIME, req.NoSpecReprots, req.CloseByBoxno);

      // 新增訂單回檔紀錄[F050305.STATUS=2]，如果已存在就不新增
      InsertF050305Data(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, "2", "0", req.WorkStationId);
			logService.Log("資料處理 結束");
			logService.Log("DB Commit 開始");
			_wmsTransaction.Complete();
			logService.Log("DB Commit 結束");
			logService.Log("查詢與檢核出貨單資訊 結束");

			// 回傳結果
			return SearchAndCheckWmsOrderInfoReturn(result, new ExecuteResult { IsSuccessed = true });
			#endregion



		}

		/// <summary>
		/// 查詢出貨商品包裝明細
		/// </summary>
		/// <param name="req"></param>
		public List<SearchWmsOrderPackingDetailRes> SearchWmsOrderPackingDetail(SearchWmsOrderPackingDetailReq req)
		{
			return F055002Repo.SearchWmsOrderPackingDetail(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).ToList();
		}

		/// <summary>
		/// 查詢出貨單刷讀紀錄
		/// </summary>
		/// <param name="req"></param>
		public IQueryable<SearchWmsOrderScanLogRes> SearchWmsOrderScanLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			#region 資料處理
			return F05500101Repo.GetSearchWmsOrderScanLog(dcCode, gupCode, custCode, wmsOrdNo);
			#endregion
		}

		/// <summary>
		/// 刷讀商品條碼
		/// </summary>
		/// <param name="req"></param>
		public ScanItemBarcodeRes ScanItemBarcode(ScanItemBarcodeReq req)
		{
			var logService = new LogService("ShipPackage_" + DateTime.Now.ToString("yyyyMMdd"));
			logService.Log("刷讀商品條碼 開始");

			var itemService = new ItemService();
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);

			logService.Log("檢核 開始");
			#region 檢核
			// 檢查<參數7>是否有值，若無值或null，回傳訊息[false,請刷讀商品條碼,null,null,false]
			if (string.IsNullOrWhiteSpace(req.BarCode))
				return new ScanItemBarcodeRes { IsSuccessed = false, Message = "請刷讀商品條碼" };

			// 檢查<參數8>是否<=0，若是則回傳訊息[false,數量必須大於0,null,null,false]
			if (req.Qty <= 0)
				return new ScanItemBarcodeRes { IsSuccessed = false, Message = "數量必須大於0" };

			// <參數7>如果有值，請先轉成大寫
			req.BarCode = req.BarCode.ToUpper();

			// 檢查是否有原箱商品未關箱 資料表: F055001 條件: DC_CODE = < 參數1 > and GUP_CODE =< 參數2 > and CUST_CODE =< 參數3 > and WMS_ORD_NO =< 參數4 > andIS_CLOSED = 0 and IS_ORIBOX = 1
			var f055001sByNotClosed = F055001Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.IS_CLOSED == "0" && o.IS_ORIBOX == "1");

			// 如果[B]有資料，回傳訊息(false,尚有原箱商品未正常關箱，請手動執行關箱後再繼續作業。,null,null,false)
			if (f055001sByNotClosed.Any())
				return new ScanItemBarcodeRes { IsSuccessed = false, Message = "尚有原箱商品未正常關箱，請手動執行關箱後再繼續作業。" };

			// [D] = 檢查刷讀的商品是否為出貨紙箱 資料表: <參數11> 條件: ITEM_CODE =[A] 
			if (req.BoxItemList.Any(x => x.ItemCode == req.BarCode))// 如果[D]有資料
			{
				// <參數9> = 02(廠退出貨)
				// a.新增F05500101紀錄 ITEM_CODE =[A], ISPASS = 0, MESSAGE = 廠退出貨不可刷入紙箱條碼 
				// b.回傳訊息[false, 廠退出貨不可刷入紙箱條碼, null, null, false]
				if (req.OrdType == "02")
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.BarCode, null, null, "0", "廠退出貨不可刷入紙箱條碼", 0, req.BarCode);

				// [D1]= 檢查是否有非原箱未關箱包裝頭檔 資料表: F055001 條件: DC_CODE = < 參數1 > GUP_CODE =< 參數2 > CUST_CODE =< 參數3 > WMS_ORD_NO =< 參數4 > IS_CLOSED = 0  IS_ORIBOX = 0
				var f055001 = F055001Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.IS_CLOSED == "0" && o.IS_ORIBOX == "0").FirstOrDefault();

				if (f055001 == null)
				{
					// 如果[D1]無資料
					// a.新增F05500101紀錄 ITEM_CODE =[A], ISPASS = 0, MESSAGE = 無未關箱資料，不可刷出貨紙箱條碼
					// b.回傳訊息[false, 無未關箱資料，不可刷出貨紙箱條碼, null, null, false]
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.BarCode, null, null, "0", "無未關箱資料，不可刷出貨紙箱條碼", 0, req.BarCode);
				}
				else
				{
					// 如果[D1]有資料
					// (1)[D1].BOX_NUM =[A]
					// (2)新增F05500101紀錄 ITEM_CODE = [A], ISPASS = 1, MESSAGE =[D2].Message。
					// (3)回傳訊息[true,[D2].Message, null, [D1].PACKAGE_BOX_NO, true]
					f055001.BOX_NUM = req.BarCode;
					f055001.ORG_BOX_NUM = req.BarCode;
					F055001Repo.Update(f055001);
					_wmsTransaction.Complete();
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.BarCode, null, null, "1", "找到紙箱", 0, req.BarCode, new ScanItemBarcodeRes { IsCloseBox = true });
				}
			}

			// [C] = <參數10> 篩選ItemCode=[A] OR EanCode1 =[A] OR EanCode2 =[A] OR EanCode3 =[A](要轉大寫比對)
			var itemCodes = req.ItemList.Where(x =>
			x.ItemCode.ToUpper() == req.BarCode ||
			x.EanCode1.ToUpper() == req.BarCode ||
			x.EanCode2.ToUpper() == req.BarCode ||
			x.EanCode3.ToUpper() == req.BarCode).Select(x => x.ItemCode).ToList();

			F2501 f2501 = null;

			// (1) 如果[C].Count=0
			if (!itemCodes.Any())
			{
				// a. [D] = 呼叫ItemService.FindItems(<參數2>,<參數3>,[A])
				itemCodes = itemService.FindItems(req.GupCode, req.CustCode, req.BarCode, ref f2501);

				if (!itemCodes.Any())
				{
					// b.如果[D] 無任何資料，
					// (b-1)新增F05500101紀錄 ITEM_CODE = [A], ISPASS = 0, MESSAGE = 找不到商品，請確認商品條碼是否輸入錯誤
					// (b-2)回傳訊息(false, 找不到商品，請確認商品條碼是否輸入錯誤,null,null,false)
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, req.BarCode, null, "0", "找不到商品，請確認商品條碼是否輸入錯誤", 0, req.BarCode);
				}
				else
				{
					// c.如果[D] 有任何資料
					// (c-1) [C]= <參數10> 是否有存在[D] 的品號
					// (c-2) 如果[C] 不存在
					// (c-2-1)新增F05500101紀錄 ITEM_CODE = [A], ISPASS = 0, MESSAGE = 找不到商品，請確認商品條碼是否輸入錯誤
					// (c-2-2)回傳訊息(false, 找不到商品，請確認商品條碼是否輸入錯誤,null,null,false)
					if (!req.ItemList.Any(x => itemCodes.Contains(x.ItemCode)))
						return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, f2501 != null ? itemCodes.First() : req.BarCode, f2501 != null ? req.BarCode : null, null, "0", "非此出貨單出貨商品", 0, req.BarCode);
				}
			}

			// [E] = 呼叫查詢出貨商品包裝明細
			var details = SearchWmsOrderPackingDetail(new SearchWmsOrderPackingDetailReq
			{
				DcCode = req.DcCode,
				GupCode = req.GupCode,
				CustCode = req.CustCode,
				WmsOrdNo = req.WmsOrdNo
			});

			// [E1]=篩選[E]資料，條件[E].ItemCode in([C])
			details = details.Where(x => itemCodes.Contains(x.ItemCode)).ToList();

			SearchWmsOrderPackingDetailRes detail = null;

			// 若[E1]無資料，
			// (1) 新增F05500101紀錄 ITEM_CODE = [A], ISPASS = 0, MESSAGE = 非此出貨單出貨商品
			// (2) 回傳訊息(false, 非此出貨單出貨商品, null, null, false)
			if (!details.Any())
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.BarCode, null, null, "0", "非此出貨單出貨商品", 0, req.BarCode);
			else // 若[E1]有資料
			{
				// [E1].Count>1 且[< 參數8 >] > 1，
				// a.新增F05500101紀錄 ITEM_CODE = [A], ISPASS = 0, MESSAGE = 此商品條碼找到多個商品，不可調整數量，請逐筆過刷
				// b.回傳訊息(false, 此商品條碼找到多個商品，不可調整數量，請逐筆過刷,null,null,false)
				if (details.Count > 1 && req.Qty > 1)
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.BarCode, null, null, "0", "此商品條碼找到多個商品，不可調整數量，請逐筆過刷", 0, req.BarCode);

				if (req.Action == "01" && req.Qty == 1)
					detail = details.Where(x => x.DiffQty >= req.Qty).OrderByDescending(x => x.PackageQty).OrderBy(x => x.DiffQty).FirstOrDefault();
				else
					detail = details.Where(x => (x.DiffQty + x.PackageQty) >= req.Qty).OrderByDescending(x => x.PackageQty).OrderBy(x => x.DiffQty).FirstOrDefault();

				if (detail == null)
				{
					var firstDetail = details.OrderByDescending(x => x.DiffQty).FirstOrDefault();
					return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, f2501 == null ? details.First().ItemCode : null, f2501 == null ? null : req.BarCode, null, "0", "已超過商品出貨數", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = firstDetail == null ? null : firstDetail.ItemCode });
				}
				else
				{
					if (req.Action == "02" && req.Qty <= detail.PackageQty)
						return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, f2501 == null ? details.First().ItemCode : null, f2501 == null ? null : req.BarCode, null, "0", $"調整數量必須超過商品已包裝數{detail.PackageQty}", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = detail.ItemCode });
				}
			}

			// [F]=取得商品主檔 資料表: F1903 條件:GUP_CODE = < 參數2 > CUST_CODE =< 參數3 > ITEM_CODE =[E2].ItemCode
			var item = req.ItemList.Where(o => o.ItemCode == detail.ItemCode).First();

			// [F4]=商品序號[C].F2501!=null then [C].F2501.SERIAL_NO Else 空白 END
			var serialNo = f2501 != null ? f2501.SERIAL_NO : string.Empty;
			var status = f2501 != null ? f2501.STATUS : string.Empty;
			var wmsNo = f2501?.WMS_NO;

			// 如果商品為原箱商品[F2]=1，但數量[<參數8>]大於1
			// (1)新增F05500101紀錄 ITEM_CODE =[F3], SERIAL_NO =[F4], ISPASS = 0, MESSAGE = 原箱商品不可調整數量
			// (2)回傳訊息(false, 原箱商品不可調整數量,[F3], null, false)
			if (item.IsOriItem == "1" && req.Qty > 1)
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "原箱商品不可調整數量", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });

			// 如果商品為序號商品[F1] = 1，但數量[< 參數8 >]大於1
			// (1)新增F05500101紀錄 ITEM_CODE =[F3], SERIAL_NO =[F4], ISPASS = 0, MESSAGE = 序號商品不可調整數量
			// (2)回傳訊息(false, 序號商品不可調整數量,[F3], null, false)
			if (item.BundleSerialNo == "1" && req.Qty > 1)
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "序號商品不可調整數量", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });

			// 如果商品為序號商品[F1] = 1，[F4] 為空白
			// (1)新增F05500101紀錄 ITEM_CODE =[F3], SERIAL_NO =[F4], ISPASS = 0, MESSAGE = 序號商品必須刷讀序號
			// (2)回傳訊息(false, 序號商品必須刷讀序號, [F3] ,null,false)
			if (item.BundleSerialNo == "1" && string.IsNullOrWhiteSpace(serialNo))
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "序號商品必須刷讀序號", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });


			// 如果商品為序號商品[F1] = 1，[C].F2501!=null、[C].F2501.STATUS=C1
			// (1)新增F05500101紀錄 ITEM_CODE =[F3], SERIAL_NO =[F4], ISPASS = 0, MESSAGE = 您刷讀的商品序號已出庫，不可出貨
			// (2)回傳訊息(false, 您刷讀的商品序號已出庫，不可出貨, [F3] ,null,false)
			//if (item.BundleSerialNo == "1" && f2501 != null && f2501.STATUS == "C1")
			if (f2501 != null && f2501.STATUS == "C1")
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "您刷讀的商品序號已出庫，不可出貨", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });

			if (f2501 != null && f2501.STATUS == "D2")
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "您刷讀的商品序號已報廢，不可出貨", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });


			// No.2091 若為不良品序號(F2501.ACTIVATED=1) & 為客戶訂單(F050801.SOURCE_TYPE is NULL 或空白)，不可出貨
			var f050801 = F050801Repo.GetData(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);
			if (f2501 != null && f2501.ACTIVATED == "1" && string.IsNullOrWhiteSpace(f050801.SOURCE_TYPE))
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "0", "此為不良品序號，不可以出貨", 0, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode });

			// 如果商品為序號綁儲位商品，且人員刷讀的序號非本出貨單指定的序號
			var serialNoList = f050802Repo.GetDatasByHasSerial(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).Select(o => o.SERIAL_NO).ToList();
			if (item.BundleSerialLoc == "1" && !serialNoList.Contains(f2501.SERIAL_NO))
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, f2501 != null ? itemCodes.First() : req.BarCode, f2501 != null ? req.BarCode : null, null, "0", string.Format("刷讀序號{0}非此商品指定出貨序號", req.BarCode), 0, req.BarCode);
			#endregion

			#region 資料處理
			var addF055002List = new List<F055002>();
			var updF055002List = new List<F055002>();
			var insertRes = true;

			// [G]=取得未關箱的包裝頭檔F055001
			var f055001ByNotClosed = F055001Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.IS_CLOSED == "0" && o.IS_ORIBOX == "0").FirstOrDefault();

			short packageBoxNo = 0;

			if (req.Qty > 1 && detail.PackageQty >= 1 && req.Qty > detail.PackageQty)
				req.Qty -= detail.PackageQty;

			// 如果是一般商品[F1]=0 AND 非原箱商品[F2]=0
			if ((item.BundleSerialNo == "0" && string.IsNullOrEmpty(serialNo)) && item.IsOriItem == "0")
			{
				if (f055001ByNotClosed == null)
				{
					packageBoxNo = F055001Repo.GetNewPackageBoxNo(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);

					// 如果[G]不存在，新增F055001、新增F055002
					var f055001 = new F055001
					{
						WMS_ORD_NO = req.WmsOrdNo,
						PACKAGE_BOX_NO = packageBoxNo,
						DELV_DATE = req.DelvDate,
						PICK_TIME = req.PickTime,
						PRINT_FLAG = 0,
						PRINT_DATE = null,
						BOX_NUM = null,
						PAST_NO = null,
						DC_CODE = req.DcCode,
						GUP_CODE = req.GupCode,
						CUST_CODE = req.CustCode,
						PACKAGE_STAFF = Current.Staff,
						PACKAGE_NAME = Current.StaffName,
						STATUS = "0",
						AUDIT_DATE = null,
						WEIGHT = null,
						AUDIT_STAFF = null,
						AUDIT_NAME = null,
						IS_CLOSED = "0",
						IS_ORIBOX = "0"
					};
					F055001Repo.Add(f055001);

					// 新增 or 修改 F055002
					insertRes = InsertOrUpdateF055002(f055001, item.ItemCode, req.Qty, serialNo, ref addF055002List, ref updF055002List, req.WorkstationId);
					if (!insertRes)
						return new ScanItemBarcodeRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
				}
				else
				{
					packageBoxNo = f055001ByNotClosed.PACKAGE_BOX_NO;

					// 如果[G]存在，新增F055002
					insertRes = InsertOrUpdateF055002(f055001ByNotClosed, item.ItemCode, req.Qty, serialNo, ref addF055002List, ref updF055002List, req.WorkstationId);
					if (!insertRes)
						return new ScanItemBarcodeRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
				}

				addF055002List.ForEach(f055002 => { F055002Repo.Add(f055002); });
				updF055002List.ForEach(f055002 => { F055002Repo.Update(f055002); });
				_wmsTransaction.Complete();

				// 新增F05500101 ITEM_CODE =[F3],SERIAL_NO =[F4],ISPASS = 1,MESSAGE = 空白
				// 回傳訊息[true, null,[F3], F055001.PACKAGE_BOX_NO, false]
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "1", null, packageBoxNo, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode, PackageBoxNo = packageBoxNo });
			}
			// 如果是序號商品[F1]=1 AND 非原箱商品[F2]=0
			else if ((item.BundleSerialNo == "1" || !string.IsNullOrEmpty(serialNo)) && item.IsOriItem == "0")
			{
				// 更新F2501.STATUS=C1
				f2501.STATUS = "C1";
				f2501.WMS_NO = f050801.WMS_ORD_NO;
				f2501.ORD_PROP = f050801.ORD_PROP;
				f2501Repo.Update(f2501);

				// 如果[G]不存在，新增F055001、新增F055002
				if (f055001ByNotClosed == null)
				{
					packageBoxNo = F055001Repo.GetNewPackageBoxNo(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);

					// 如果[G]不存在，新增F055001、新增F055002
					var f055001 = new F055001
					{
						WMS_ORD_NO = req.WmsOrdNo,
						PACKAGE_BOX_NO = packageBoxNo,
						DELV_DATE = req.DelvDate,
						PICK_TIME = req.PickTime,
						PRINT_FLAG = 0,
						PRINT_DATE = null,
						BOX_NUM = null,
						PAST_NO = null,
						DC_CODE = req.DcCode,
						GUP_CODE = req.GupCode,
						CUST_CODE = req.CustCode,
						PACKAGE_STAFF = Current.Staff,
						PACKAGE_NAME = Current.StaffName,
						STATUS = "0",
						AUDIT_DATE = null,
						WEIGHT = null,
						AUDIT_STAFF = null,
						AUDIT_NAME = null,
						IS_CLOSED = "0",
						IS_ORIBOX = "0"
					};
					F055001Repo.Add(f055001);

					// 新增 or 修改 F055002
					insertRes = InsertOrUpdateF055002(f055001, item.ItemCode, req.Qty, serialNo, ref addF055002List, ref updF055002List, req.WorkstationId);
					if (!insertRes)
						return new ScanItemBarcodeRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
				}
				else
				{
					packageBoxNo = f055001ByNotClosed.PACKAGE_BOX_NO;

					// 如果[G]存在，新增F055002
					insertRes = InsertOrUpdateF055002(f055001ByNotClosed, item.ItemCode, req.Qty, serialNo, ref addF055002List, ref updF055002List, req.WorkstationId);
					if (!insertRes)
						return new ScanItemBarcodeRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
				}

				addF055002List.ForEach(f055002 => { F055002Repo.Add(f055002); });
				updF055002List.ForEach(f055002 => { F055002Repo.Update(f055002); });
				logService.Log("資料處理 結束");

				logService.Log("db commit 開始");
				_wmsTransaction.Complete();
				logService.Log("db commit 結束");
				logService.Log("刷讀商品條碼 結束");

				// 新增F05500101 ITEM_CODE =[F3],SERIAL_NO =[F4],ISPASS = 1,MESSAGE = 空白
				// 回傳訊息[true, null,[F3], F055001.PACKAGE_BOX_NO, false]
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "1", null, packageBoxNo, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode, PackageBoxNo = packageBoxNo }, wmsNo);
			}
			// 如果是原箱商品[F2]=1
			else if (item.IsOriItem == "1")
			{
				// 如果[F4]有值，更新F2501.STATUS=C1
				if (!string.IsNullOrWhiteSpace(serialNo))
				{
					f2501.STATUS = "C1";
					f2501Repo.Update(f2501);
				}

				packageBoxNo = F055001Repo.GetNewPackageBoxNo(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);

				// 新增F055001
				var f055001 = new F055001
				{
					WMS_ORD_NO = req.WmsOrdNo,
					PACKAGE_BOX_NO = packageBoxNo,
					DELV_DATE = req.DelvDate,
					PICK_TIME = req.PickTime,
					PRINT_FLAG = 1,
					PRINT_DATE = DateTime.Now,
					BOX_NUM = "ORI",
					PAST_NO = null,
					DC_CODE = req.DcCode,
					GUP_CODE = req.GupCode,
					CUST_CODE = req.CustCode,
					PACKAGE_STAFF = Current.Staff,
					PACKAGE_NAME = Current.StaffName,
					STATUS = "0",
					AUDIT_DATE = null,
					WEIGHT = null,
					AUDIT_STAFF = null,
					AUDIT_NAME = null,
					IS_CLOSED = "0",
					IS_ORIBOX = "1"
				};
				F055001Repo.Add(f055001);

				// 新增 or 修改 F055002
				insertRes = InsertOrUpdateF055002(f055001, item.ItemCode, req.Qty, serialNo, ref addF055002List, ref updF055002List, req.WorkstationId);
				if (!insertRes)
					return new ScanItemBarcodeRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
				addF055002List.ForEach(f055002 => { F055002Repo.Add(f055002); });
				updF055002List.ForEach(f055002 => { F055002Repo.Update(f055002); });
				logService.Log("資料處理 結束");

				logService.Log("db commit 開始");
				_wmsTransaction.Complete();
				logService.Log("db commit 結束");

				logService.Log("刷讀商品條碼 結束");

				// 新增F05500101 ITEM_CODE =[F3], SERIAL_NO =[F4], ISPASS = 1, MESSAGE = 空白
				// 回傳訊息[true, null,[F3], F055001.PACKAGE_BOX_NO, True]
				return LogF05500101AndReturn(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, item.ItemCode, serialNo, status, "1", null, packageBoxNo, req.BarCode, new ScanItemBarcodeRes { ItemCode = item.ItemCode, PackageBoxNo = packageBoxNo, IsCloseBox = true });
			}
			#endregion

			return new ScanItemBarcodeRes();
		}

		/// <summary>
		/// 關箱處理
		/// </summary>
		/// <param name="req"></param>
		public CloseShipBoxRes CloseShipBox(CloseShipBoxReq req)
		{
			var logService = new LogService("ShipPackage_" + DateTime.Now.ToString("yyyyMMdd"), CommonService);
			logService.Log("關箱處理開始" + req.WmsOrdNo);
			var distibuteService = new DistibuteService(null, CommonService);
			var containerService = new ContainerService(_wmsTransaction);
			var printService = new PrintService();
			var consignService = new Lms.Services.ConsignService(_wmsTransaction);
			var f055007List = new List<F055007>();

			#region 資料處理

			logService.Log("取得出貨單未關箱資料開始");

			F055001 f055001 = null;
			if (req.PackageBoxNo == null)// <參數5>=null
			{
				// (1)[BB] = 取得出貨單未關箱資料(原箱優先關箱) 資料表: F055001 條件: DC_CODE = < 參數1 > AND GUP_CODE = < 參數2 > CUST_CODE = < 參數3 > AND WMS_ORD_NO =< 參數4 > IS_CLOSED = 0 ORDER BY IS_ORIBOX DESC
				f055001 = F055001Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo && o.IS_CLOSED == "0").OrderByDescending(x => x.IS_ORIBOX).FirstOrDefault();

				if (f055001 == null)
					return new CloseShipBoxRes { IsSuccessed = false, Message = "無任何需關箱的紙箱" };

				// (2) < 參數5 > = [BB].PACKAGE_BOX_NO
				req.PackageBoxNo = f055001.PACKAGE_BOX_NO;
			}
			else// <參數5>!=null
			{
				// [BB] = 取得出貨單箱資料 資料表:F055001 條件: DC_CODE = <參數1> AND GUP_CODE = < 參數2 > CUST_CODE = < 參數3 > AND WMS_ORD_NO=<參數4> AND PACKAGE_BOX_NO =< 參數5 >
				f055001 = F055001Repo.GetData(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, Convert.ToInt16(req.PackageBoxNo));

				if (f055001 == null)
					return new CloseShipBoxRes { IsSuccessed = false, Message = $"指定箱序{req.PackageBoxNo}找不到可關箱的紙箱" };
			}

			//把前端的工作編號寫入
			f055001.WORKSTATION_CODE = req.WorkStationId;
			f055001.CLOSEBOX_WORKSTATION_CODE = SwitchToBoxWorksatationCode(req.PackageMode == "02", req.WorkStationId);
			f055001.PACK_CLIENT_PC = Current.DeviceIp;
			f055001.NO_SPEC_REPROTS = req.PackageMode == "02" ? "1" : "0";
			f055001.CLOSE_BY_BOXNO = req.IsScanBox;
			f055001.CLOSEBOX_TIME = DateTime.Now;
			logService.Log("取得貨主單號開始");
			// [CC]=取得貨主單號[F050301.CUST_ORD_NO] 取第一筆 資料表: F050301 + F05030101 條件: F05030101.DC_CODE = < 參數1 > F05030101.GUP_CODE = < 參數2 > F05030101.CUST_CODE = < 參數3 > F05030101.WMS_ORD_NO = < 參數4 >
			var custOrdNo = F050301Repo.GetFstCustOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);

			// 如果 < 參數7 >= 01(一般出貨) AND < 參數9 >= 1(需刷讀紙箱) and F055001.BOX_NUM是空白或NULL(沒有紙箱編號) 回傳訊息[false, 請刷讀紙箱條碼進行關箱]
			if (req.OrdType == "01" && req.IsScanBox == "1" && string.IsNullOrWhiteSpace(f055001.BOX_NUM))
				return new CloseShipBoxRes { IsSuccessed = false, Message = "請刷讀紙箱條碼進行關箱" };

			// 如果 < 參數7 >= 01(一般出貨) AND < 參數9 >= 0(需刷讀紙箱) and [BB].BOX_NUM 空的時 回傳訊息[false, 無建議箱號，無法自動關箱，請刷讀紙箱條碼進行關箱]
			if (req.OrdType == "01" && req.IsScanBox == "0" && string.IsNullOrWhiteSpace(f055001.BOX_NUM) && string.IsNullOrWhiteSpace(req.SugBoxNo))
			{
				logService.Log("無建議箱號，依系統邏輯取得建議箱號");
				var getMinBoxs = F056001Repo.GetCloseShipSysBox(req.DcCode, req.GupCode, req.CustCode, req.WorkStationId).ToList();
				//資料不存在時，預設為 24H-01
				if (!getMinBoxs.Any())
					f055001.BOX_NUM = "24H-01";
				else
					f055001.BOX_NUM = getMinBoxs.First().BOX_CODE;
				f055001.ORG_BOX_NUM = f055001.BOX_NUM;
			}

			// <參數7>=01(一般出貨) AND [BB].PAST_NO = NULL OR 空白
			if (req.OrdType == "01" && string.IsNullOrWhiteSpace(f055001.PAST_NO))
			{
				if (req.PrintBoxSettingParam.isGetShipOrder)
				{
					logService.Log("呼叫商品出貨申請宅配單號開始");
					// [A]=呼叫LMS API 商品出貨申請宅配單號
					var lmsApiRes = consignService.ApplyConsign(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, f055001.PACKAGE_BOX_NO, req.IsScanBox, req.SugBoxNo, f055001);

					// 呼叫失敗: 回傳結果[false,[[A].MsgCode] +[A].MsgContent,null]
					if (!lmsApiRes.IsSuccessed)
						return new CloseShipBoxRes { IsSuccessed = false, Message = $"{lmsApiRes.Message}\r\n呼叫LMS申請宅配單失敗，請執行<手動關箱>" };

					logService.Log("呼叫商品出貨申請宅配單號結束");
					if (req.PackageMode == "01")
					{
						var f0003ByVCIP = CommonService.GetSysGlobalValue(req.DcCode, "VideoCombinInPack");
						var videoCombinInPack = f0003ByVCIP != null && f0003ByVCIP == "0" ? "0" : "1";
						if (videoCombinInPack == "1")
						{
							logService.Log("呼叫WcssrApi封箱資訊同步開始");
							// WcssrApi封箱資訊同步
							var wcssrApiRes = distibuteService.SealingInfoAsync(req.DcCode, req.GupCode, req.CustCode, new SealingInfoAsyncReq
							{
								WhId = req.DcCode,
								OutboundNo = req.WmsOrdNo,
								WorkStationId = req.WorkStationId,
								ShipNo = lmsApiRes.No,
								OperationUserId = Current.Staff,
								TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
							});
							logService.Log("呼叫WcssrApi封箱資訊同步結束");
						}
					}

				}
				else
				{
					f055001.PRINT_FLAG = 1;
					f055001.PRINT_DATE = DateTime.Now;
					f055001.IS_CLOSED = "1";
					if (string.IsNullOrWhiteSpace(f055001.BOX_NUM))
						f055001.BOX_NUM = req.SugBoxNo;
					F055001Repo.Update(f055001);
				}
				logService.Log("扣除紙箱庫存開始");
				// 扣除紙箱庫存  參考P050801Service.UpdateBoxStock
				UpdateBoxStock(f055001.DC_CODE, f055001.GUP_CODE, f055001.CUST_CODE, req.IsScanBox == "0" ? req.SugBoxNo : f055001.BOX_NUM);
				logService.Log("扣除紙箱庫存結束");

			}

			// 刪除出貨包裝箱列印報表[F055007]
			F055007Repo.DeleteByPackageBoxNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, Convert.ToInt16(req.PackageBoxNo));

			var reportSeq = 1;

			if (req.OrdType == "01")// <參數7> = 01(一般出貨)
			{
				// [LL]=1則新增報表清單[F055007] ([BB],[CC],[DD],01,箱明細,NULL,1)
				if (req.PrintBoxSettingParam.isPrintBoxDetail == "1")
				{
					logService.Log("報表清單新增箱明細");
					f055007List.Add(InsertF055007(f055001, custOrdNo, "01", "箱明細", null, "1", ref reportSeq));

				}

				// [PP]=1，則新增報表清單[F055007] ([BB],[CC],[DD],02,一般出貨小白標,NULL)
				if (req.PrintBoxSettingParam.isPrintShipLittleLabel == "1")
				{
					logService.Log("報表清單新增一般出貨小白標");
					f055007List.Add(InsertF055007(f055001, custOrdNo, "02", "一般出貨小白標", null, "3", ref reportSeq));
				}

			}
			else if (req.OrdType == "02")// <參數7>=02(廠退出貨)
			{

				// (1)更新[BB] PRINT_FLAG = 1, PRINT_DATE = 系統時間, IS_CLOSED = 1 WHERE DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > WMS_ORD_NO =< 參數4 > AND PACKAGE_BOX_NO = < 參數5 >
				f055001.PRINT_FLAG = 1;
				f055001.PRINT_DATE = DateTime.Now;
				f055001.IS_CLOSED = "1";
				F055001Repo.Update(f055001);

				// a.[RR] = 1則新增報表清單 => 廠退出貨小白標[F055007]([BB],[CC],[DD],03, 廠退出貨小白標,NULL)
				if (req.PrintBoxSettingParam.isPrintRtnShipLittleLabel == "1")
				{
					logService.Log("報表清單新增廠退出貨小白標");
					f055007List.Add(InsertF055007(f055001, custOrdNo, "03", "廠退出貨小白標", null, "1", ref reportSeq));
				}
			}

			// <參數8> =01 and <參數7> = 01(一般出貨)
			if (req.PackageMode == "01" && req.OrdType == "01")
			{
				var strPackageBoxNo = Convert.ToString(req.PackageBoxNo).PadLeft(3, '0');
				logService.Log("呼叫LMS API取得訂單出貨列印清單 開始");
				// [MM]=呼叫LMS API取得訂單出貨列印清單
				var lmsRes = printService.GetShipPrintList(req.DcCode, req.GupCode, req.CustCode, custOrdNo, strPackageBoxNo);

				if (lmsRes.IsSuccessed)
				{
					var data = JsonConvert.DeserializeObject<List<PrintJobListRes>>(JsonConvert.SerializeObject(lmsRes.Data));
					data.ForEach(item =>
					{
#if (DEBUG || TEST)
			  //string url = $"{item.Url.Replace("https", "http").Replace("wmsext-dev.agroup.tw", "lmsdev.agroup.tw")}12|PI2208160006724|001";
			  string url = item.Url;
#else
            string url = $"{item.Url}{req.DcCode}|{custOrdNo}|{strPackageBoxNo}";
#endif

			  // [GG]= [MM].Url LIKE ‘%/ShipOrder/%’  OR [MM].Name=宅配單Then 2 ELSE 1
			  var printerNo = url.Contains("/ShipOrder/") || item.Name == "宅配單" ? "2" : "1";

			  //新增報表清單[F055007] ([BB],[CC],[DD],[EE],[FF],[GG])，API會回傳多筆，所以要寫多筆
			  f055007List.Add(InsertF055007(f055001, custOrdNo, "04", item.Name, url, printerNo, ref reportSeq));
					});
					logService.Log("呼叫LMS API取得訂單出貨列印清單 結束");

				}
				else
				{
					// [MM]呼叫失敗，回傳結果[false,[[MM].MsgCode] +[MM].MsgContent,null]
					return new CloseShipBoxRes { IsSuccessed = false, Message = $"[LMS取得訂單出貨列印清單][{lmsRes.MsgCode}]{lmsRes.MsgContent}\r\n取得LMS訂單出貨列印清單失敗，請執行<手動關箱>" };
				}
			}

			// [PP]= null (單據完成，還有未關箱的箱序)
			Int16? packageBoxNo = null;
			logService.Log("呼叫查詢出貨商品包裝明細 開始");
			// [FF] = 呼叫查詢出貨商品包裝明細
			var detail = SearchWmsOrderPackingDetail(new SearchWmsOrderPackingDetailReq
			{
				DcCode = req.DcCode,
				GupCode = req.GupCode,
				CustCode = req.CustCode,
				WmsOrdNo = req.WmsOrdNo
			});
			logService.Log("呼叫查詢出貨商品包裝明細 結束");

			string msg = string.Empty;
			bool packingFinish = false;
			// 如果[FF]所有出貨明細都無差異[DiffQty ==0]，則
			if (detail.All(x => x.DiffQty == 0))
			{

				// [KK]=檢查是否還有紙箱未關箱且非原箱且非本次關箱箱序 資料表: F055001 條件: DC_CODE = < 參數1 > AND GUP_CODE = < 參數2 > CUST_CODE = < 參數3 > AND WMS_ORD_NO =< 參數4 > IS_CLOSED = 0 AND IS_ORIBOX = false AND PACKAGE_BOX_NO!= < 參數5 >
				var f055001ExcludePackageBoxNo = F055001Repo.GetDataExdulePackageBoxNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, "0", "0", Convert.ToInt16(req.PackageBoxNo));

				if (f055001ExcludePackageBoxNo != null)
				{
					packageBoxNo = f055001ExcludePackageBoxNo.PACKAGE_BOX_NO;
				}
				else
				{
					// a.更新出貨單狀態為已稽核[F050801.STATUS = 2]，出貨單列印註記 = 1[F050801.PRINT_FLAG = 1]
					F050801Repo.Update(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, 2, "1");
					logService.Log("更新出貨單狀態為已稽核 結束");
					// b.釋放容器 呼叫containerService.DelContainer 
					containerService.DelContainer(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
					logService.Log("刪除容器 結束");
					// (d - 3).< 參數7 >= 02(廠退出貨) 更新F060204.PROC_FLAG = 2
					// 資料表: F160204 + F050301 + F05030101 條件: F05030101.DC_CODE =< 參數1 > F05030101.GUP_CODE =< 參數2 > F05030101.CUST_CODE =< 參數3 > F05030101.WMS_ORD_NO =< 參數4 >F050301.SOURCE_NO = F060204.RTN_WMS_NO
					if (req.OrdType == "02")
					{
						F160204Repo.UpdateProcFlag(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, "2");
						logService.Log("更新廠退Flag 結束");
					}
					// b.[SS] = 包裝完成
					msg = "包裝完成";

					if (req.ShipMode == "2")
					{  // 更新F060208.PROC_FLAG = 3 條件 DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND ORI_ORDER_CODE = < 參數4 > AND STATUS not in (9)
						F060208Repo.UpdateProcFlag(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, 3, new List<int> { 9 });
						logService.Log("更新包裝線到站紀錄PROC_FLAG =3 結束");
					}
					// 刪除F051301
					F051301Repo.DeleteWmsNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
					logService.Log("刪除F051301 結束");

					packingFinish = true;
				}
			}
			else // 如果[FF]出貨明細有任何一筆有差異，[SS] = 加箱完成
			{
				msg = req.IsAppendBox ? "加箱完成" : "關箱完成";
			}
			_wmsTransaction.Complete(true);
			logService.Log("DB Commit 結束");

      if (req.IsAppendBox)
        LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "人員按下加箱", 0, null);
      if (req.IsManualCloseBox)
        LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "人員按下手動關箱", 0, null);

      if (packingFinish)
			{
				LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "包裝完成", 0, null);
				logService.Log("寫入包裝完成紀錄 結束");
			}
			#endregion

			#region 回傳結果
			// F055007清單
			var reportList = f055007List.Select(x => new ShipPackageReportModel
			{
				PackageBoxNo = x.PACKAGE_BOX_NO,
				CustOrdNo = x.CUST_ORD_NO,
				ReportCode = x.REPORT_CODE,
				ReportName = x.LMS_NAME,
				ReportUrl = x.LMS_URL,
				PrinterNo = x.PRINTER_NO,
				ReportSeq = x.REPORT_SEQ,
				ISPRINTED = x.ISPRINTED,
				PRINT_TIME = x.PRINT_TIME
			}).ToList();

			if (req.OrdType == "01") // 一般出貨
			{
				// <參數7> = 01(一般出貨) AND [LL]=1 AND [PQ]=1
				if (req.PrintBoxSettingParam.isPrintBoxDetail == "1" && req.PrintBoxSettingParam.isPrintShipLittleLabel == "1")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}{(string.IsNullOrWhiteSpace(msg) ? string.Empty : "，")}請將箱明細放入箱中，並將小白標貼於箱外。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };
				// <參數7> = 01(一般出貨) AND [LL]=1 AND [PQ]=0
				if (req.PrintBoxSettingParam.isPrintBoxDetail == "1" && req.PrintBoxSettingParam.isPrintShipLittleLabel == "0")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}{(string.IsNullOrWhiteSpace(msg) ? string.Empty : "，")}請將箱明細放入箱中。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };

				// <參數7> = 01(一般出貨) AND [LL]=0 AND [PQ]=1
				if (req.PrintBoxSettingParam.isPrintBoxDetail == "0" && req.PrintBoxSettingParam.isPrintShipLittleLabel == "1")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}{(string.IsNullOrWhiteSpace(msg) ? string.Empty : "，")}請將小白標貼於箱外。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };
				// <參數7> = 01(一般出貨) AND [LL]=0 AND [PQ]=0 
				if (req.PrintBoxSettingParam.isPrintBoxDetail == "0" && req.PrintBoxSettingParam.isPrintShipLittleLabel == "0")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };
			}
			else // 廠退出貨
			{
				// <參數7> = 02(廠退出貨) AND [RR]=1
				if (req.PrintBoxSettingParam.isPrintRtnShipLittleLabel == "1")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}{(string.IsNullOrWhiteSpace(msg) ? string.Empty : "，")}請將小白標貼於箱外。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };
				// <參數7> = 02(廠退出貨) AND [RR]=1
				if (req.PrintBoxSettingParam.isPrintRtnShipLittleLabel == "0")
					return new CloseShipBoxRes { IsSuccessed = true, Message = $"{msg}。", LastPackageBoxNo = packageBoxNo, ReportList = reportList };
			}

			return new CloseShipBoxRes();
			#endregion
		}

		/// <summary>
		/// 使用出貨單容器資料產生箱明細
		/// </summary>
		/// <param name="req"></param>
		public UseShipContainerToBoxDetailRes UseShipContainerToBoxDetail(UseShipContainerToBoxDetailReq req)
		{
			var f070102Repo = new F070102Repository(Schemas.CoreSchema);

			LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "人員按下包裝完成", 0, null);

			//如果 < 參數8 >= 2(包裝線包裝站)
			if (req.ShipMode == "2")
			{
				//(1)[T] = 取得總箱數[TOP 1 F060208.BOX_TOTAL 資料表: F060208 條件: DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND ORI_ORDER_CODE = < 參數4 > AND PROC_FLAG not in (3, 9) AND POSITION_CODE = TARGET_POS_CODE 排序: ORDER BY CRT_DATE DESC
				var boxTotal = F060208Repo.GetTop1BoxTotal(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, new List<int> { 3, 9 });

				//(2) [B] = 取得已抵達箱數[Count(*)] 資料表: F060208 條件: DC_CODE = < 參數1 > AND GUP_CODE =< 參數2 > AND CUST_CODE =< 參數3 > AND ORI_ORDER_CODE = < 參數4 > AND PROC_FLAG not in (3, 9) AND POSITION_CODE = TARGET_POS_CODE
				var boxCnt = F060208Repo.GetDataCnt(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, new List<int> { 3, 9 });

				//(3) 如果[T] !=[B]，回傳訊息[false, 尚有周轉箱未抵達工作站，不可包裝完成]
				//(4) 如果[T] =[B]，往下執行
				if (boxTotal != boxCnt)
					return new UseShipContainerToBoxDetailRes { IsSuccessed = false, Message = "尚有周轉箱未抵達工作站，不可包裝完成" };
			}

			// [SS] = 檢查出貨單是否有一筆箱頭檔 資料表: F055001 條件:DC_CODE =< 參數1 > AND GUP_CODE = < 參數2 > AND CUST_CODE = < 參數3 > AND WMS_ORD_NO =< 參數4 >
			var f055001s = F055001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_ORD_NO == req.WmsOrdNo);

			// 如果[SS]有資料:
			if (f055001s.Any())
				return new UseShipContainerToBoxDetailRes { IsSuccessed = true };

			// [K]=取得容器明細[F070102]
			var containerItemData = f070102Repo.GetContainerItemData(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).ToList();

			// 檢查是否有原箱商品、序號商品，若有一筆，回傳訊息[false,出貨明細中有原箱商品或序號商品，請刷讀所有商品]。
			if (containerItemData.Any(x => x.ALLOWORDITEM == "1" || x.BUNDLE_SERIALNO == "1"))
				return new UseShipContainerToBoxDetailRes { IsSuccessed = false, Message = "出貨明細中有原箱商品或序號商品，請刷讀所有商品" };

			#region Insert F055001
			// 取得該出貨單最大PACKAGE_BOX_NO +1
			var packageBoxNo = F055001Repo.GetNewPackageBoxNo(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);

			var f055001 = new F055001
			{
				WMS_ORD_NO = req.WmsOrdNo,
				PACKAGE_BOX_NO = packageBoxNo,
				DELV_DATE = req.DelvDate,
				PICK_TIME = req.PickTime,
				PRINT_FLAG = null,
				PRINT_DATE = null,
				BOX_NUM = req.SubBoxNo,
				PAST_NO = null,
				DC_CODE = req.DcCode,
				GUP_CODE = req.GupCode,
				CUST_CODE = req.CustCode,
				PACKAGE_STAFF = Current.Staff,
				PACKAGE_NAME = Current.StaffName,
				STATUS = "0",
				AUDIT_DATE = null,
				WEIGHT = null,
				AUDIT_STAFF = null,
				AUDIT_NAME = null,
				IS_CLOSED = "0",
				IS_ORIBOX = "0"
			};
			F055001Repo.Add(f055001);
			#endregion

			#region Insert Or Update F055002
			var addF055002List = new List<F055002>();
			var updF055002List = new List<F055002>();
			var insertRes = true;
			req.WorkstationId = SwitchToBoxWorksatationCode(req.NoSpecReprots == "1", req.WorkstationId);
			foreach (var item in containerItemData)
			{
				insertRes = InsertOrUpdateF055002(f055001, item.ITEM_CODE, item.QTY, null, ref addF055002List, ref updF055002List, req.WorkstationId);
				if (!insertRes)
					break;
			}
			if (!insertRes)
				return new UseShipContainerToBoxDetailRes { IsSuccessed = false, Message = "容器商品數量超過出貨數" };
			addF055002List.ForEach(f055002 => { F055002Repo.Add(f055002); });
			updF055002List.ForEach(f055002 => { F055002Repo.Update(f055002); });
			#endregion

			_wmsTransaction.Complete();

			return new UseShipContainerToBoxDetailRes { IsSuccessed = true, PackageBoxNo = packageBoxNo };
		}

		/// <summary>
		/// 取消包裝
		/// </summary>
		/// <param name="req"></param>
		public CancelShipOrderRes CancelShipOrder(CancelShipOrderReq req)
		{
			var consignService = new Lms.Services.ConsignService();
			var sharedService = new SharedService(_wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055007Repo = new F055007Repository(Schemas.CoreSchema, _wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f19471201Repo = new F19471201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f194712Repo = new F194712Repository(Schemas.CoreSchema);
			var f194704Repo = new F194704Repository(Schemas.CoreSchema);

			// [SS]=檢查出貨單狀態是否已出貨 資料表:F050801 條件: DC_CODE = < 參數1 > GUP_CODE =< 參數2 > CUST_CDOE =< 參數3 >WMS_ORD_NO =< 參數4 >
			var f050801 = F050801Repo.GetData(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);
			// 如果[SS].STATUS = 5 回傳訊息[false, 此出貨單已出貨，部可取消包裝]
			if (f050801.STATUS == 5)
				return new CancelShipOrderRes { IsSuccessed = false, Message = "此出貨單已出貨，不可取消包裝" };
			else if (f050801.STATUS == 6)  //6	單據狀態	已扣帳
				return new CancelShipOrderRes { IsSuccessed = false, Message = "此出貨單已扣帳，不可取消包裝" };

			#region 呼叫LmsApi取消宅配單
			// [YY]=取得已關箱包裝頭檔 資料表:F055001 條件: DC_CODE= <參數1> GUP_CODE=<參數2> CUST_CDOE=<參數3> WMS_ORD_NO=<參數4> AND IS_CLOSED = 1
			var f055001Data = F055001Repo.GetDatas(req.WmsOrdNo, req.GupCode, req.CustCode, req.DcCode);

			if (f055001Data.Any(x => x.IS_CLOSED == "1" && !string.IsNullOrWhiteSpace(x.PAST_NO)))
			{
				// 呼叫LmsApi取消宅配單
				var lmsRes = consignService.CancelConsign(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);

				// 如果[A]失敗，回傳訊息[false,[A].Message+換行+取消申請LMS宅配單失敗，請手動執行<取消包裝>。]
				if (!lmsRes.IsSuccessed)
					return new CancelShipOrderRes { IsSuccessed = false, Message = $"{lmsRes.Message}\r\n取消申請LMS宅配單失敗，請手動執行<取消包裝>。" };
			}
			#endregion

			#region 刪除F055002
			F055002Repo.Delete(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo);
			#endregion

			#region 回復包材庫存
			var shareResult = sharedService.ReturnBoxQty(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
			if (!shareResult.IsSuccessed)
				return new CancelShipOrderRes { IsSuccessed = false, Message = shareResult.Message };
			#endregion

			#region 取消包裝 要刪除加箱的託運單 要復原加箱託運單
			if (f050801.SELF_TAKE == "1")
			{
				var f050901s = f050901Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_NO == req.WmsOrdNo).ToList();

				if (f050901s != null && f050901s.Any())
					f050901Repo.SqlBulkDeleteForAnyCondition(f050901s, "F050901", new List<string> { "DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_NO" });
			}
			#endregion

			#region 刪除 F055001
			F055001Repo.Delete(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo);
			#endregion

			#region 修改 F2501
			UpdateF2501(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
			#endregion

			#region 刪除 F05500101
			//f05500101Repo.Delete(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo);
			LogF05500101(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, null, null, null, "1", "人員取消包裝", 0, null, flag: "9");
			#endregion

			#region 刪除F055007
			f055007Repo.Delete(o => o.DC_CODE == req.DcCode && o.GUP_CODE == req.GupCode && o.CUST_CODE == req.CustCode && o.WMS_ORD_NO == req.WmsOrdNo);
			#endregion

			#region 修改 F050801
			// 若出貨單狀態為已取消則不可更新為待處理(0)
      int? isPackCheck = null;

			if (f050801 != null && f050801.STATUS != 9)
			{
				var CheckPackageModeResult = CheckPackageMode(f050801, "0");
				if (!CheckPackageModeResult.IsSuccessed)
					return new CancelShipOrderRes { IsSuccessed = false, Message = CheckPackageModeResult.MsgContent };

				f050801 = CheckPackageModeResult.f050801;

        if (f050801.STATUS == 2)
          isPackCheck = 1;
				else if (f050801.ISPACKCHECK == 2)
          isPackCheck = 0;
			}

			F050801Repo.UpdateOrderUnpacked(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, isPackCheck);
			#endregion

			#region 出貨包裝_取消廠退單
			if (f050801.SOURCE_TYPE == "13")
			{
				var f050301s = F050301Repo.GetF050301ForWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);
				// 取得廠退單出貨單資料
				var f160204s = F160204Repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301s.First().SOURCE_NO).OrderBy(x => x.RTN_WMS_NO);
				// 更新f160204.PROC_FLAG
				foreach (var item in f160204s)
				{
					item.PROC_FLAG = "1";
				}
				F160204Repo.BulkUpdate(f160204s);
			}
			#endregion

			#region 將此出貨單的F05500101.FLAG設為9
			UpdateF05500101Flag(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, "9");
			#endregion 將此出貨單的F05500101.FLAG設為9

			_wmsTransaction.Complete();

			return new CancelShipOrderRes { IsSuccessed = true, Status = f050801.STATUS };
		}

		//可考慮將P080701Service.UpdateF2501改用這邊的共用函數，以便維護
		/// <summary>
		/// 取消包裝-還原商品序號狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		public void UpdateF2501(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f2501repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var upF2501Data = new List<F2501>();
			var statusDatas = F05500101Repo.GetAllDataByShipPackageService(dcCode, gupCode, custCode, wmsOrdCode).ToList();
			var serialNos = (from a in statusDatas select a.SERIAL_NO).ToList();
			var f2501Data = CommonService.GetItemSerialList(gupCode, custCode, serialNos);
			foreach (var s in statusDatas)
			{
				var exist = f2501Data.FirstOrDefault(o => o.SERIAL_NO == s.SERIAL_NO);
				if (exist != null)
				{
					exist.STATUS = s.STATUS;
					exist.IS_ASYNC = "N";  //#2149 1. 當訂單做虛擬儲位回復時，若已經揀貨完成自動倉已經回覆序號資料過，請將該出貨單相關的序號，都改成IS_ASYNC = N
					if (!string.IsNullOrWhiteSpace(s.ORG_SERIAL_WMS_NO))
						exist.WMS_NO = s.ORG_SERIAL_WMS_NO;
					upF2501Data.Add(exist);
				}
			}
			f2501repo.BulkUpdate(upF2501Data);
		}

		/// <summary>
		/// 取得出貨單所有出貨箱要列印報表清單
		/// </summary>
		/// <param name="req"></param>
		public List<ShipPackageReportModel> SearchShipReportList(SearchShipReportListReq req)
		{
			var f055007Repo = new F055007Repository(Schemas.CoreSchema);

			#region 資料處理
			return f055007Repo.GetDataByWmsOrCustOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo).ToList();
			#endregion
		}

		private SearchAndCheckWmsOrderInfoRes SearchAndCheckWmsOrderInfoReturn(SearchAndCheckWmsOrderInfoRes res, ExecuteResult result, Boolean? IsCancelOrder = null)
		{
			res.Result = result;
			if (IsCancelOrder ?? false)
				res.IsCancelOrder = IsCancelOrder.Value;
			return res;
		}

		/// <summary>
		/// 產生新增或更新出貨包裝身擋明細
		/// </summary>
		/// <param name="f055001"></param>
		/// <param name="itemCode"></param>
		/// <param name="addQty"></param>
		/// <param name="serialNo"></param>
		/// <param name="addF055002List"></param>
		/// <param name="updF055002List"></param>
		private bool InsertOrUpdateF055002(F055001 f055001, string itemCode, int addQty, string serialNo, ref List<F055002> addF055002List, ref List<F055002> updF055002List,
		  string WorkstationCode)
		{
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);

			var itemShipPackageNoAllotOrders = f05030202Repo.GetItemShipPackageNoAllotOrder(f055001.DC_CODE, f055001.GUP_CODE, f055001.CUST_CODE, f055001.WMS_ORD_NO, itemCode).ToList();

			if (!string.IsNullOrEmpty(serialNo) && itemShipPackageNoAllotOrders.Any(x => x.SERIAL_NO == serialNo))
				itemShipPackageNoAllotOrders = itemShipPackageNoAllotOrders.Where(x => x.SERIAL_NO == serialNo).ToList();

			var f055002s = F055002Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f055001.DC_CODE && x.GUP_CODE == f055001.GUP_CODE && x.CUST_CODE == f055001.CUST_CODE && x.WMS_ORD_NO == f055001.WMS_ORD_NO && x.PACKAGE_BOX_NO == f055001.PACKAGE_BOX_NO && x.ITEM_CODE == itemCode).ToList();

			do
			{
				var item = itemShipPackageNoAllotOrders.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO) && x.SERIAL_NO == serialNo);

				if (item == null)
					item = itemShipPackageNoAllotOrders.FirstOrDefault(x => x.B_DELV_QTY - x.PACKAGE_QTY > 0);

				if (item == null)
					return false;

				var itemQty = item.B_DELV_QTY - item.PACKAGE_QTY;
				var allotQty = addQty;

				if (itemQty >= addQty)
				{
					item.PACKAGE_QTY += addQty;
					addQty = 0;
				}
				else
				{
					item.PACKAGE_QTY += itemQty;
					allotQty = itemQty;
					addQty -= itemQty;
				}

				if (string.IsNullOrWhiteSpace(serialNo))
				{
					var findF055002 = f055002s.FirstOrDefault(x => x.ORD_NO == item.ORD_NO && x.ORD_SEQ == item.ORD_SEQ && string.IsNullOrWhiteSpace(x.SERIAL_NO));
					if (findF055002 != null)
					{
						findF055002.PACKAGE_QTY += allotQty;
						updF055002List.Add(findF055002);
						continue;
					}
				}

				addF055002List.Add(new F055002
				{
					WMS_ORD_NO = f055001.WMS_ORD_NO,
					DC_CODE = f055001.DC_CODE,
					GUP_CODE = f055001.GUP_CODE,
					CUST_CODE = f055001.CUST_CODE,
					PACKAGE_BOX_NO = f055001.PACKAGE_BOX_NO,
					PACKAGE_BOX_SEQ = GetF055002NextSeq(f055001),
					PACKAGE_QTY = allotQty,
					CLIENT_PC = Current.DeviceIp,
					ITEM_CODE = itemCode,
					SERIAL_NO = serialNo,
					ORD_NO = item.ORD_NO,
					ORD_SEQ = item.ORD_SEQ,
					WORKSTATION_CODE = WorkstationCode
				});
			}
			while (addQty > 0);
			return true;
		}

		private int? f055002MaxSeq = null;
		/// <summary>
		/// 取得包裝身擋明細新的SEQ
		/// </summary>
		/// <param name="f055001"></param>
		/// <returns></returns>
		private int GetF055002NextSeq(F055001 f055001)
		{
			if (f055002MaxSeq == null)
			{
				var maxSeq = F055002Repo.Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(f055001.WMS_ORD_NO)
													 && x.PACKAGE_BOX_NO == f055001.PACKAGE_BOX_NO
													 && x.DC_CODE == EntityFunctions.AsNonUnicode(f055001.DC_CODE)
													 && x.GUP_CODE == EntityFunctions.AsNonUnicode(f055001.GUP_CODE)
													 && x.CUST_CODE == EntityFunctions.AsNonUnicode(f055001.CUST_CODE))
					.Max(x => (int?)x.PACKAGE_BOX_SEQ);
				f055002MaxSeq = maxSeq.HasValue ? maxSeq.Value + 1 : 1;
				return f055002MaxSeq.Value;
			}
			f055002MaxSeq++;
			return f055002MaxSeq.Value;

		}

		/// <summary>
		/// 刷讀Log並回傳刷讀商品條碼失敗物件
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <param name="isPass"></param>
		/// <param name="message"></param>
		/// <param name="packageBoxNo"></param>
		/// <returns></returns>
		private ScanItemBarcodeRes LogF05500101AndReturn(string dcCode, string gupCode, string custCode, string wmsOrdNo, string itemCode, string serialNo, string status, string isPass, string message, short packageBoxNo, string scanCode, ScanItemBarcodeRes res = null, string OrgSerialWmsNo = null)
		{
			LogF05500101(dcCode, gupCode, custCode, wmsOrdNo, itemCode, serialNo, status, isPass, message, packageBoxNo, scanCode, OrgSerialWmsNo: OrgSerialWmsNo);
			if (res == null)
				res = new ScanItemBarcodeRes();
			res.IsSuccessed = isPass == "1";
			res.Message = message;

			return res;
		}

		/// <summary>
		/// 刷讀Log
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <param name="isPass"></param>
		/// <param name="message"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="scanCode"></param>
		/// <param name="logSeq"></param>
		public void LogF05500101(string dcCode, string gupCode, string custCode, string wmsOrdNo, string itemCode, string serialNo, string status, string isPass, string message, short packageBoxNo, string scanCode, int? logSeq = null, string flag = "0", string OrgSerialWmsNo = null)
		{
			// 若沒有指定 Seq 則自動找下一個序號
			if (!logSeq.HasValue)
				logSeq = F05500101RepoNoTrans.GetNextLogSeq(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);

			F05500101RepoNoTrans.Add(new F05500101
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				WMS_ORD_NO = wmsOrdNo,
				PACKAGE_STAFF = Current.Staff,
				PACKAGE_NAME = Current.StaffName,
				ITEM_CODE = itemCode,
				SERIAL_NO = serialNo,
				STATUS = status,
				ISPASS = isPass,
				MESSAGE = message,
				PACKAGE_BOX_NO = packageBoxNo,
				LOG_SEQ = logSeq.Value,
				SCAN_CODE = scanCode,
				FLAG = flag,
				ORG_SERIAL_WMS_NO = OrgSerialWmsNo
			});
		}

		/// <summary>
		/// 更新紙箱庫存
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="boxNum"></param>
		/// <returns></returns>
		public ExecuteResult UpdateBoxStock(string dcCode, string gupCode, string custCode, string boxNum)
		{
			if (boxNum == "ORI" || string.IsNullOrWhiteSpace(boxNum)) // 原箱不更新紙箱庫存
				return new ExecuteResult { IsSuccessed = true };

			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913 = f1913Repo.GetF1913BoxKeyColumn(dcCode, gupCode, custCode, boxNum);

			if (f1913 == null)
			{
				var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction); ;
				var f1912 = f1912Repo.GetDatas(dcCode, "S", gupCode, custCode).FirstOrDefault();
				if (f1912 == null)
					f1912 = f1912Repo.GetDatas(dcCode, "S", gupCode).FirstOrDefault();
				if (f1912 == null)
					f1912 = f1912Repo.GetDatas(dcCode, "S").FirstOrDefault();
				// 無耗材倉不扣庫存
				if (f1912 == null)
					return new ExecuteResult(true);

				f1913Repo.Add(new F1913 {
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          ENTER_DATE = DateTime.Today,
          VALID_DATE = Convert.ToDateTime("9999/12/31"),
          ITEM_CODE = boxNum,
          LOC_CODE = f1912.LOC_CODE,
          QTY = -1,
          MAKE_NO = "0",
          VNR_CODE = "000000",
          BOX_CTRL_NO = "0",
          PALLET_CTRL_NO = "0",
          SERIAL_NO = "0"
        });
			}
			else
			{
				f1913Repo.UpdateBoxStock(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, f1913.VALID_DATE, f1913.ENTER_DATE,
          f1913.MAKE_NO, f1913.SERIAL_NO, f1913.VNR_CODE, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO);
			}

			return new ExecuteResult { IsSuccessed = true };
		}

		/// <summary>
		/// 新增F055007
		/// </summary>
		/// <param name="f055001"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="reportCode"></param>
		/// <param name="lmsName"></param>
		/// <param name="lmsUrl"></param>
		/// <param name="printerNo"></param>
		/// <param name="reportSeq"></param>
		/// <returns></returns>
		private F055007 InsertF055007(F055001 f055001, string custOrdNo, string reportCode, string lmsName, string lmsUrl, string printerNo, ref int reportSeq)
		{
			var f055007 = new F055007
			{
				DC_CODE = f055001.DC_CODE,
				GUP_CODE = f055001.GUP_CODE,
				CUST_CODE = f055001.CUST_CODE,
				WMS_ORD_NO = f055001.WMS_ORD_NO,
				PACKAGE_BOX_NO = f055001.PACKAGE_BOX_NO,
				CUST_ORD_NO = custOrdNo,
				REPORT_CODE = reportCode,
				LMS_NAME = lmsName,
				LMS_URL = lmsUrl,
				PRINTER_NO = printerNo,
				REPORT_SEQ = reportSeq
			};

			F055007Repo.Add(f055007);
			reportSeq++;

			return f055007;
		}

		/// <summary>
		/// 取得箱明細報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GetBoxDetailReportRes GetBoxDetailReport(GetBoxDetailReportReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var result = new GetBoxDetailReportRes();
			var BoxHeader = new BoxHeaderData();

			// 箱明細頭檔
			result.BoxHeader = F05030101RepoNoTrans.GetBoxHeaderData(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);

			// 箱明細身檔
			var boxDetail = F055002Repo.GetDeliveryReport(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.PackageBoxNo).ToList();

			// 取得服務型商品
			var f050104s = F050104RepoNoTrans.GetDatas(req.DcCode, req.GupCode, req.CustCode, boxDetail.Select(x => x.OrdNo).ToList())
			  .GroupBy(x => x.ITEM_CODE)
			  .Select(x => new { ItemCode = x.Key, Services = x.Select(z => new { Code = z.SERVICE_ITEM_CODE, Name = z.SERVICE_ITEM_NAME }).ToList() });
			if (!f050104s.Any())
			{
				result.BoxDetail = boxDetail.Select(AutoMapper.Mapper.DynamicMap<BoxDetailData>).ToList();
			}
			else
			{
				int rowNum = 0;
				boxDetail.ForEach(item =>
				{
					if (result.BoxDetail == null)
						result.BoxDetail = new List<BoxDetailData>();

					rowNum++;
					item.ROWNUM = rowNum;
					result.BoxDetail.Add(AutoMapper.Mapper.DynamicMap<BoxDetailData>(item));
					var currF050104 = f050104s.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
					if (currF050104 != null)
					{
						currF050104.Services.ForEach(service =>
				{
						rowNum++;
						result.BoxDetail.Add(new BoxDetailData
						{
							ROWNUM = rowNum,
							PackageBoxNo = item.PackageBoxNo,
							CUST_ITEM_CODE = service.Code,
							ItemName = service.Name
						});
					});
					}
				});
			}

			// 2023/02/13 Scott 目前客戶無非加工組合商品，先暫時註解，等客戶有需要再來解開註解
			//以下為將明細商品轉換成非加工組合商品
			//var BomOrderDetail = F05030201RepoNoTrans.GetDeliveryReportByBomItem(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, req.PackageBoxNo).ToList();
			//if (BomOrderDetail.Any())
			//{
			//  var detail = F05030201RepoNoTrans.GetDatasByWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, new List<string> { req.WmsOrdNo }).ToList();
			//  foreach (var item in BomOrderDetail)
			//  {
			//    var bomItemDetail = detail.Where(o => o.BOM_ITEM_CODE == item.ItemCode).ToList();
			//    var resultDetail = result.BoxDetail.Where(x => bomItemDetail.Any(y => y.ITEM_CODE == x.ItemCode) && x.PackageBoxNo == item.PackageBoxNo).ToList();
			//    //組合品項數必須存在出貨單中
			//    if (resultDetail.Count == bomItemDetail.Count)
			//    {
			//      //計算每個商品訂貨數可以組合成幾個c
			//      var countItemC = new List<int>();
			//      foreach (var item3 in bomItemDetail)
			//      {
			//        var resultItem = resultDetail.FirstOrDefault(o => o.ItemCode == item3.ITEM_CODE);
			//        var orderQty = resultItem.PackQty;
			//        //如果訂貨數>= 組合C該商品數量 則訂貨數等於組合C該商品數量
			//        if (orderQty - item3.ORD_QTY >= 0)
			//          orderQty = item3.ORD_QTY;
			//        //此商品可產生幾個組合C
			//        countItemC.Add(orderQty / item3.BOM_QTY);
			//      }
			//      //取最小可組合c的數量
			//      var minC = countItemC.Min();
			//      if (minC >= item.PackQty)
			//        minC = item.PackQty;
			//      if (minC > 0) //有組合c數量
			//      {
			//        foreach (var item2 in bomItemDetail)
			//        {
			//          var resultItem = resultDetail.FirstOrDefault(o => o.ItemCode == item2.ITEM_CODE);
			//          resultItem.PackQty -= item2.BOM_QTY * minC;
			//          if (resultItem.PackQty == 0)
			//            result.BoxDetail.Remove(resultItem);
			//        }
			//        item.PackQty = minC;
			//        item.ROWNUM = result.BoxDetail.Count + 1;
			//        result.BoxDetail.Add(AutoMapper.Mapper.DynamicMap<BoxDetailData>(item));
			//      }
			//    }
			//  }
			//}
			return result;
		}

		/// <summary>
		/// 取得一般出貨小白標報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GetShipLittleLabelReportRes GetShipLittleLabelReport(GetShipLittleLabelReportReq req)
		{
			var boxLittleLabelDetail = new List<Box>();
			var f050301 = F050301Repo.GetDataByWmsOrdNo(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo);

			boxLittleLabelDetail.Add(new Box
			{
				BoxBarCode = f050301.CUST_ORD_NO + "|" + req.PackageBoxNo.ToString().PadLeft(3, '0'),
				BoxCode = f050301.CUST_ORD_NO + "|" + req.PackageBoxNo.ToString().PadLeft(3, '0')
			});

			return new GetShipLittleLabelReportRes
			{
				BoxLittleLabelDetail = boxLittleLabelDetail
			};
		}

		/// <summary>
		/// 取得廠退出貨小白標報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GetRtnShipLittleLabelReportRes GetRtnShipLittleLabelReport(GetRtnShipLittleLabelReportReq req)
		{
			// 取得訂單編號
			var ordNo = F05030101RepoNoTrans.GetDatasByTrueAndCondition(x => x.WMS_ORD_NO == req.WmsOrdNo).FirstOrDefault()?.ORD_NO;
			// 取得貨主單據頭檔
			var f050301 = F050301Repo.GetDatasByTrueAndCondition(x => x.ORD_NO == ordNo).FirstOrDefault();
			// 取得廠退單出貨單資料
			var f160204s = F160204Repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301.SOURCE_NO).OrderBy(x => x.RTN_WMS_NO);
			// 取得廠退原因
			var rtnVnrCause = F160202RepoNoTrans.GetDatasByTrueAndCondition(x => x.RTN_VNR_NO == f160204s.First().RTN_VNR_NO).FirstOrDefault()?.RTN_VNR_CAUSE;
			var cause = F1951RepoNoTrans.GetDatasByTrueAndCondition(x => x.UCC_CODE == rtnVnrCause && x.UCT_ID == "RV").FirstOrDefault().CAUSE;
			var getRtnShipLittleLabelReportRes = new BoxRtnLittleLabel
			{
				VNR_CODE = f160204s.FirstOrDefault().VNR_CODE,
				VNR_NAME = F1908RepoNoTrans.GetDatasByTrueAndCondition(x => x.VNR_CODE == f160204s.FirstOrDefault().VNR_CODE).FirstOrDefault()?.VNR_NAME,
				SOURCE_NO = f050301.SOURCE_NO,
				CAUSE = cause
			};

			// 更新f055001.BOX_DOC
			var f055001 = F055001RepoNoTrans.GetDatasByTrueAndCondition(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode
			&& x.CUST_CODE == req.CustCode && x.WMS_ORD_NO == req.WmsOrdNo).OrderByDescending(x => x.PACKAGE_BOX_NO).FirstOrDefault();
			f055001.BOX_DOC = f050301.SOURCE_NO;

			F055001RepoNoTrans.Update(f055001);
			var result = new GetRtnShipLittleLabelReportRes();
			result.BoxRtnLittleLabelDetail = new List<BoxRtnLittleLabel> { getRtnShipLittleLabelReportRes };
			return result;
		}

		/// <summary>
		/// 取得LMS出貨列印檔案
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GeShipFileRes GeShipFile(GeShipFileReq req)
		{
			var service = new ShipFileService();
			var shipFileData = service.ShipFile(req.DcCode, req.GupCode, req.CustCode, req.Url, req.Reprint);
			var reuslt = new GeShipFileRes
			{
				IsSuccessed = shipFileData.MsgCode == "201" ? true : false,
				Message = shipFileData.MsgCode == "201" ? null : (string.IsNullOrWhiteSpace(shipFileData.MsgContent) ? null : $"[{shipFileData.MsgCode}]{shipFileData.MsgContent}"),
				ContentType = shipFileData.Data?.GetType().GetProperty("ContentType").GetValue(shipFileData.Data)?.ToString(),
				FileBytes = (byte[])(shipFileData.Data?.GetType().GetProperty("FileBytes").GetValue(shipFileData.Data))
			};
			return reuslt;
		}

		/// <summary>
		/// 包裝站開站/關站紀錄
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public SetPackageStationStatusLogRes SetPackageStationStatusLog(SetPackageStationStatusLogReq req)
		{
			var f077102Repo = new F077102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910501Repo = new F910501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = new SetPackageStationStatusLogRes();

			f077102Repo.Add(new F077102
			{
				DC_CODE = req.DcCode,
				EMP_ID = Current.Staff,
				WORK_TYPE = "1",
				WORKSTATION_CODE = req.WorkstationCode,
				WORKING_TIME = DateTime.Now,
				STATUS = req.Status
			});
			if (req.Status == "1")
			{
				f910501Repo.Update(req.DcCode, req.DeviceIp, req.NoSpecReport, req.CloseByBoxNo);
				f1946Repo.Update("1", req.DcCode, req.WorkstationCode);
			}
			if (req.Status == "0")
			{
				f1946Repo.Update("0", req.DcCode, req.WorkstationCode);
			}

			return new SetPackageStationStatusLogRes { IsSuccessed = true };
		}

		public SetPackageLineStationStatusRes SetPackageLineStationStatus(SetPackageLineStationStatusReq req)
		{
			var f1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910501Repo = new F910501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f077102Repo = new F077102Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = new SetPackageLineStationStatusRes();
			var wcsWorkStationServices = new WcsWorkStationServices();
			var wcsStationReq = new WcsStationReq
			{
				OwnerCode = req.CustCode,
				StationTotal = 1,
				StationList = new List<WcsStationModel> {
		  new WcsStationModel
		  {
			StationCode = req.WorkstationCode,
			StationType = req.WorkstationType,
			Status = Convert.ToInt32(req.Status),
		  }
		}
			};
			var apiResult = wcsWorkStationServices.Station(wcsStationReq, req.DcCode);
			if (apiResult.IsSuccessed && req.Status == "1")
			{
				f1946Repo.Update("1", req.DcCode, req.WorkstationCode);
				f910501Repo.Update(req.DcCode, req.DeviceIp, req.NoSpecReports, req.CloseByBoxno);
			}
			if (apiResult.IsSuccessed && req.Status == "4")
			{
				f1946Repo.Update("1", req.DcCode, req.WorkstationCode);
			}
			if (apiResult.IsSuccessed && req.Status == "2")
			{
				f1946Repo.Update(req.Status, req.DcCode, req.WorkstationCode);
			}
			if (apiResult.IsSuccessed && req.Status == "0")
			{
				f1946Repo.Update(req.HasUndone ? "3" : "0", req.DcCode, req.WorkstationCode);

				if (!req.HasUndone)
				{
					f077102Repo.Add(new F077102
					{
						DC_CODE = req.DcCode,
						EMP_ID = Current.Staff,
						WORK_TYPE = "2",
						WORKSTATION_CODE = req.WorkstationCode,
						WORKING_TIME = DateTime.Now,
						STATUS = req.Status
					});
				}
			}
			result.IsSuccessed = apiResult.IsSuccessed;
			result.Message = $"{apiResult.MsgCode} {apiResult.MsgContent}";
			return result;
		}

		/// <summary>
		/// 變更出貨單為所有商品都需過刷
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ChangeShipPackCheckRes ChangeShipPackCheck(ChangeShipPackCheckReq req)
		{
			var result = new ChangeShipPackCheckRes();
			F050801RepoNoTrans.UpdateIsPackCheck(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo, "2");
			return new ChangeShipPackCheckRes { IsSuccessed = true };
		}

		/// <summary>
		/// 取得出貨單容器資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GetWorkStataionShipDataRes GetWorkStataionShipData(GetWorkStataionShipDataReq req)
		{
			var procFlagList = new List<int> { 0, 1 };
			var f060208s = F060208Repo.GetWorkStataionShipData(req.DcCode, req.workstationCode).ToList();

			var waiting = f060208s.Where(x => x.PROC_FLAG == 0).ToList();
			var arrived = f060208s.Except(waiting).ToList();

			return new GetWorkStataionShipDataRes
			{
				IsArrialContainer = arrived.Any() ? true : false,
				ArrivalContainerCode = arrived.Any() ? arrived.OrderBy(x => x.CREATE_TIME).FirstOrDefault()?.CONTAINER_CODE : null,
				ArrivalWmsNo = arrived.Any() ? arrived.FirstOrDefault()?.ORI_ORDER_CODE : null,
				WaitWmsOrderCnt = waiting.Select(x => x.ORI_ORDER_CODE).Distinct().Count(),
				WaitContainerCnt = waiting.Select(x => x.CONTAINER_CODE).Distinct().Count(),
			};
		}


		/// <summary>
		/// 取得出貨單容器資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public GetShipLogisticBoxRes GetShipLogisticBox(GetShipLogisticBoxReq req)
		{
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			GetShipLogisticBoxRes getShipLogisticBoxRes = new GetShipLogisticBoxRes();
			getShipLogisticBoxRes.IsSuccessed = true;
			getShipLogisticBoxRes.Datas = new List<GetShipLogisticBoxData>();
			getShipLogisticBoxRes.Datas.AddRange(f070101Repo.GetShipLogisticBox(req.DcCode, req.GupCode, req.CustCode, req.WmsOrdNo)
				.Select(x => new GetShipLogisticBoxData() { ContainerCode = x }));
			return getShipLogisticBoxRes;
		}

		public void LogPausePacking(string dcCode, string gupCode, string custCode, string ContainerCode)
		{
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);

			if (ContainerCode.Substring(0, 1) == "O")
			{
				LogF05500101(dcCode, gupCode, custCode, ContainerCode, null, null, null, "1", "暫停包裝", 0, null);
				return;
			}

			var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == ContainerCode && o.CONTAINER_TYPE == "0").FirstOrDefault();
			if (f0701 == null)
				return;

			var f070101 = f070101Repo.GetDatasByTrueAndCondition(o => o.F0701_ID == f0701.ID).FirstOrDefault();
			var firstYardWmsNo = f070101.WMS_NO.Substring(0, 1);
			if (firstYardWmsNo == "O")
				LogF05500101(dcCode, gupCode, custCode, f070101.WMS_NO, null, null, null, "1", "暫停包裝", 0, null);
		}

		public void UpdateShipReportList(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<ShipPackageReportModel> shipPackageReportModels)
		{
			if (!shipPackageReportModels.Any())
				return;

			var logSeq = F05500101RepoNoTrans.GetNextLogSeq(dcCode, gupCode, custCode, wmsOrdNo, shipPackageReportModels.First().PackageBoxNo);
			foreach (var item in shipPackageReportModels)
			{
				F05500101RepoNoTrans.Add(new F05500101
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					WMS_ORD_NO = wmsOrdNo,
					PACKAGE_STAFF = Current.Staff,
					PACKAGE_NAME = Current.StaffName,
					ITEM_CODE = null,
					SERIAL_NO = null,
					STATUS = null,
					ISPASS = "1",
					MESSAGE = "人員開始列印" + item.ReportName,
					PACKAGE_BOX_NO = item.PackageBoxNo,
					LOG_SEQ = logSeq,
					SCAN_CODE = null,
					FLAG = "0",
					ORG_SERIAL_WMS_NO = null,
					CRT_DATE = item.START_PRINT_TIME.Value,
					CRT_STAFF = Current.Staff,
					CRT_NAME = Current.StaffName,
					UPD_DATE = DateTime.Now,
					UPD_STAFF = Current.Staff,
					UPD_NAME = Current.StaffName
				}, true);
				logSeq++;

				F05500101RepoNoTrans.Add(new F05500101
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					WMS_ORD_NO = wmsOrdNo,
					PACKAGE_STAFF = Current.Staff,
					PACKAGE_NAME = Current.StaffName,
					ITEM_CODE = null,
					SERIAL_NO = null,
					STATUS = null,
					ISPASS = "1",
					MESSAGE = "人員結束列印" + item.ReportName,
					PACKAGE_BOX_NO = item.PackageBoxNo,
					LOG_SEQ = logSeq,
					SCAN_CODE = null,
					FLAG = "0",
					ORG_SERIAL_WMS_NO = null,
					CRT_DATE = item.PRINT_TIME.Value,
					CRT_STAFF = Current.Staff,
					CRT_NAME = Current.StaffName,
					UPD_DATE = DateTime.Now,
					UPD_STAFF = Current.Staff,
					UPD_NAME = Current.StaffName
				}, true);
				logSeq++;
			}
		}

		/// <summary>
		/// 確認包裝模式，通過的話會更新F050801.SHIP_MODE
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="shipMode"></param>
		/// <returns></returns>
		public CheckPackageModeResult CheckPackageMode(F050801 f050801, string shipMode)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			CheckPackageModeResult result = new CheckPackageModeResult() { IsSuccessed = true };
			//若SHIP_MODE IS NULL OR 0 OR 傳入參數=0，更新F050801.SHIP_MODE=傳入參數，COMMIT,可出貨

			//若SHIP_MODE=1 AND SHIP_MODE<>傳入參數，回傳[人員已在出貨包裝處理，不可在此出貨]
			if (f050801.SHIP_MODE == "1" && f050801.SHIP_MODE != shipMode && shipMode != "0")
				return new CheckPackageModeResult() { IsSuccessed = false, MsgCode = "20872", MsgContent = tacService.GetMsg("20872") };
			//若SHIP_MODE=2 AND SHIP_MODE<>傳入參數，回傳[人員已在單人包裝站/包裝線包裝站處理，不可在此出貨]
			else if (f050801.SHIP_MODE == "2" && f050801.SHIP_MODE != shipMode && shipMode != "0")
				return new CheckPackageModeResult() { IsSuccessed = false, MsgCode = "20873", MsgContent = tacService.GetMsg("20873") };
			//若SHIP_MODE=3 AND SHIP_MODE<>傳入參數，回傳[人員已在外部包裝站處理，不可在此出貨]
			else if (f050801.SHIP_MODE == "3" && f050801.SHIP_MODE != shipMode && shipMode != "0")
				return new CheckPackageModeResult() { IsSuccessed = false, MsgCode = "20874", MsgContent = tacService.GetMsg("20874") };
			//若SHIP_MODE=4 AND SHIP_MODE<>傳入參數，回傳[人員已在稽核出庫處理，不可在此出貨]
			else if (f050801.SHIP_MODE == "4" && f050801.SHIP_MODE != shipMode && shipMode != "0")
				return new CheckPackageModeResult() { IsSuccessed = false, MsgCode = "20875", MsgContent = tacService.GetMsg("20875") };
			else
				f050801 = F050801RepoNoTrans.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
					 () =>
					 {
						 var lockF050801 = F050801RepoNoTrans.LockF050801();
						 var dbF050801 = F050801RepoNoTrans.Find(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.WMS_ORD_NO == f050801.WMS_ORD_NO, isByCache: false);
						 //若SHIP_MODE IS NULL OR 0 OR 傳入參數=0，更新F050801.SHIP_MODE=傳入參數，COMMIT,可出貨
						 if (string.IsNullOrWhiteSpace(dbF050801.SHIP_MODE) || dbF050801.SHIP_MODE == "0" || shipMode == "0")
						 {
							 F050801RepoNoTrans.UpdateShipMode(dbF050801.DC_CODE, dbF050801.GUP_CODE, dbF050801.CUST_CODE, dbF050801.WMS_ORD_NO, shipMode);
							 dbF050801.SHIP_MODE = shipMode;
						 }
						 return dbF050801;
					 });
			// 如果DB 無法修改SHIP_MODE，代表已經有其人功能變更，要再判斷一次
			if (f050801.SHIP_MODE != shipMode && shipMode != "0")
				return CheckPackageMode(f050801, shipMode);

			result.f050801 = f050801;
			return result;
		}

		/// <summary>
		/// 更新刷讀紀錄Flag
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="WmsOrdNo"></param>
		/// <param name="Flag"></param>
		/// <returns></returns>
		public ExecuteResult UpdateF05500101Flag(string dcCode, string gupCode, string custCode, string WmsOrdNo, string Flag)
		{
			F05500101Repo.UpdateFields(new { FLAG = Flag }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == WmsOrdNo);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 寫入訂單回檔記錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNos"></param>
		/// <param name="status"></param>
		/// <param name="procFlag"></param>
		/// <param name="WorkStationId"></param>
		public void InsertF050305Data(string dcCode, string gupCode, string custCode, string wmsOrdNo, string status, string procFlag, string WorkStationId)
		{
			var f050305Repo = new F050305Repository(Schemas.CoreSchema, _wmsTransaction);
			var insertF050305 = F05030101RepoNoTrans.GetOrderRtnInsertDatas(dcCode, gupCode, custCode, status, new List<string> { wmsOrdNo }).ToList().FirstOrDefault();
			if (insertF050305 != null)
			{
				insertF050305.WORKSTATION_CODE = WorkStationId;
				insertF050305.PROC_FLAG = procFlag;
				f050305Repo.Add(insertF050305);
			}
		}

		/// <summary>
		/// 取消到站紀錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="containerCode"></param>
		public ExecuteResult CancelArrivalRecord(string dcCode, string gupCode, string custCode, string wmsNo, string containerCode = null)
		{
			var result = new ExecuteResult(true);
			var f060208s = F060208Repo.GetDatasByWmsNoAndContainerCode(dcCode, gupCode, custCode, wmsNo, containerCode).ToList();
			if (f060208s.Any())
			{
				F060208Repo.UpdateProcFlag(dcCode, gupCode, custCode, wmsNo, containerCode);
			}
			return result;
		}

		/// <summary>
		/// 將工作站編號替換為配箱站編號
		/// </summary>
		/// <param name="CloseByBoxno"></param>
		/// <param name="WorkstationCode"></param>
		/// <returns></returns>
		private string SwitchToBoxWorksatationCode(Boolean IsNoSpecReprots, string WorkstationCode)
		{
			//前端傳回工作站設定的配箱站與封箱站分開的值=1，則配箱工作站編號第一碼變更為S存入此欄，否則為配箱工作站編號
			return IsNoSpecReprots ? WorkstationCode.Remove(0, 1).Insert(0, "S") : WorkstationCode;
		}

		public PrintBoxSettingParam GetPrintBoxSetting(string dcCode, string gupCode, string custCode, string ShipMode)
		{
			var param = new PrintBoxSettingParam
			{
				isPrintBoxDetail = "0",
				isPrintShipLittleLabel = "0",
				isPrintRtnShipLittleLabel = "0",
				isGetShipOrder = false,
			};

			if (ShipMode == "1")
			{
				// [LL] = 單人包裝站取得是否列印箱明細
				var f0003ByLL = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "IsPrintBoxDetail_SinglePack");
				param.isPrintBoxDetail = f0003ByLL != null && f0003ByLL == "0" ? "0" : "1";
				// [PP] = 單人包裝站是否列印出貨小白標
				var f0003ByPP = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "IsPrintShipLittleLabel_SinglePack");
				param.isPrintShipLittleLabel = f0003ByPP != null && f0003ByPP == "0" ? "0" : "1";
			}
			else if (ShipMode == "2")
			{
				// [LL] = 包裝線包裝站取得是否列印箱明細
				var f0003ByLL = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "IsPrintBoxDetail_PackLine");
				param.isPrintBoxDetail = f0003ByLL != null && f0003ByLL == "0" ? "0" : "1";
				// [PQ] = 包裝線包裝站是否列印出貨小白標
				var f0003ByPQ = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "IsPrintShipLittleLabel_PackLine");
				param.isPrintShipLittleLabel = f0003ByPQ != null && f0003ByPQ == "0" ? "0" : "1";
			}

			// 取得是否取得宅配單
			var f0003BySO = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "GetShipOrder");
			param.isGetShipOrder = f0003BySO != null && f0003BySO == "0" ? false : true;

			// [RR] = 取得是否列印廠退出貨小白標
			var f0003ByRR = CommonService.GetSysGlobalValue(dcCode, gupCode, custCode, "IsPrintRtnShipLittleLabel");
			param.isPrintRtnShipLittleLabel = f0003ByRR != null && f0003ByRR == "0" ? "0" : "1";

			return param;
		}

	}
}
