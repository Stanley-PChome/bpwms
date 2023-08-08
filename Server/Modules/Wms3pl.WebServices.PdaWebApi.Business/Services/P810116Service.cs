using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810116Service
	{
		private WmsTransaction _wmsTransation;
		public P810116Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		/// <summary>
		/// 廠退便利倉-進場檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult VnrConvenientBookin(VnrConvenientBookinReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var f051801Repo = new F051801Repository(Schemas.CoreSchema);
			var f051802Repo = new F051802Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var data = new VnrConvenientBookinRes { VnrWmsNo = req.RtnWmsNo };

			#region 資料檢核

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
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 便利商編號是否為空
			if (string.IsNullOrWhiteSpace(req.ConvenientCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21601", MsgContent = p81Service.GetMsg("21601") };

			// 廠退出貨單號是否為空
			if (string.IsNullOrWhiteSpace(req.RtnWmsNo))
				return new ApiResult { IsSuccessed = false, MsgCode = "21602", MsgContent = p81Service.GetMsg("21602") };

			// 便利商編號、廠退出貨單號轉大寫
			req.ConvenientCode = req.ConvenientCode.ToUpper();
			req.RtnWmsNo = req.RtnWmsNo.ToUpper();

			// 取得廠退出貨明細第一筆廠商編號
			var f160204 = f160204Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.RTN_WMS_NO == req.RtnWmsNo).FirstOrDefault();
			if (f160204 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21603", MsgContent = p81Service.GetMsg("21603") };

			// 取得第一筆的出貨單號F05030101.WMS_ORD_NO
			var wmsOrdNo = f05030101Repo.GetFirstWmsOrdNoBySourceNo(req.DcNo, gupCode, req.CustNo, req.RtnWmsNo);
			if (string.IsNullOrWhiteSpace(wmsOrdNo))
				return new ApiResult { IsSuccessed = false, MsgCode = "21604", MsgContent = p81Service.GetMsg("21604") };

			// 取得出貨單
			var f050801 = f050801Repo.Find(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.WMS_ORD_NO == wmsOrdNo);
			if (f050801 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21603", MsgContent = p81Service.GetMsg("21603") };

			// 檢核出貨單狀態
			switch (f050801.STATUS)
			{
				case 0:
					return new ApiResult { IsSuccessed = false, MsgCode = "21605", MsgContent = p81Service.GetMsg("21605") };
				case 5:
					return new ApiResult { IsSuccessed = false, MsgCode = "21606", MsgContent = p81Service.GetMsg("21606") };
				case 9:
					return new ApiResult { IsSuccessed = false, MsgCode = "21607", MsgContent = p81Service.GetMsg("21607") };
				case 2: // 2 往下執行
					break;
				default:
					return new ApiResult { IsSuccessed = false, MsgCode = "21608", MsgContent = string.Format(p81Service.GetMsg("21608"), p81Service.GetTopicValueName("F050801", "STATUS", f050801.STATUS.ToString())) };
			}

			// [E] = 取得廠退便利倉是否已有此廠退出貨單放入
			var f051802s = f051802Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.WMS_NO == req.RtnWmsNo);

			// 如果[E] 有資料 顯示訊息[此廠退出貨單已入場廠退便利倉，不可入場]
			if (f051802s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21609", MsgContent = p81Service.GetMsg("21609") };
			#endregion

			#region 資料處理
			// [F] = 取得廠退便利倉是否已有此廠商使用中的儲位
			var f051801s = f051801Repo.GetDataByConvenientCodeWithVnrCode(req.DcNo, gupCode, req.CustNo, req.ConvenientCode, f160204.VNR_CODE);
			if (f051801s.Any())
			{
				// 如果[F]有資料 [S]=回傳[F] 所有的CELL_CODE [H] = NULL
				data.CellList = f051801s.Select(x => new VnrConvenientBookinCellModel { CellCode = x.CELL_CODE }).ToList();
			}
			else
			{
				//如果[F]無資料
				//	[G] = 取得廠退便利倉第一筆空儲位(要做lock機制)
				//				資料表: F051801
				//				 條件:  F051801.DC_CODE = 傳入的DcNo F051801.CONVENIENT_CODE = 傳入的ConvenientCode  AND STATUS = 0
				//ORDER BY CELL_CODE
				// 找到建議上架儲位
				//(1) 集貨格資料 = 撈top(1) F051401 by cell_type = F051301.cell_type + status = 0(空儲位) + collection_code = F051301.collection_code + dc_code = F051301.dc_code for update
				var f051801Res = f051801Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				() =>
				{
					// [G] = 取得廠退便利倉第一筆空儲位(要做lock機制)
					//			資料表: F051801
					//			 條件:  F051801.DC_CODE = 傳入的DcNo F051801.CONVENIENT_CODE = 傳入的ConvenientCode  AND STATUS = 0
					//ORDER BY CELL_CODE
					var lockF051801 = f051801Repo.LockF051801();
					var updF051801 = f051801Repo.AsForUpdate().GetDatasByTrueAndCondition(x =>
					x.DC_CODE == req.DcNo &&
					x.CONVENIENT_CODE == req.ConvenientCode &&
					x.STATUS == "0").OrderBy(x => x.CELL_CODE).FirstOrDefault();
					if (updF051801 == null)
						return null;
					else
					{
						//如果[G] 有資料
						//更新[G].STATUS = 1,
						// [G].VNR_CODE =[B]
						updF051801.STATUS = "1";
						updF051801.VNR_CODE = f160204.VNR_CODE;
						updF051801.GUP_CODE = gupCode;
						updF051801.CUST_CODE = req.CustNo;
						f051801Repo.Update(updF051801);
						//	[S] = 回傳[G].CELL_CODE
						//	[H] = [G].CELL_CODE
						data.CellList = new List<VnrConvenientBookinCellModel> { new VnrConvenientBookinCellModel { CellCode = updF051801.CELL_CODE } };
						data.BookinCellCode = updF051801.CELL_CODE;

						return updF051801;
					}
				});

				//	如果[G] 無資料 顯示訊息[空儲格不足，無法入場]
				if (f051801Res == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21610", MsgContent = p81Service.GetMsg("21610") };
			}

			// [B1] = 取得廠商名稱F1908.VNR_NAME WHERE GUP_CODE = [A] AND CUST_CODE=傳入的CustNo AND VNR_CODE=[B]
			var f1908 = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.VNR_CODE == f160204.VNR_CODE);
			data.VnrCode = f160204.VNR_CODE;
			data.VnrName = f1908 == null ? string.Empty : f1908.VNR_NAME;

			return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = data };
			#endregion
		}

		/// <summary>
		/// 廠退便利倉-進場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult VnrConvenientBookinConfirm(VnrConvenientBookinConfirmReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051801Repo = new F051801Repository(Schemas.CoreSchema, _wmsTransation);
			var f051802Repo = new F051802Repository(Schemas.CoreSchema, _wmsTransation);
			var f051803Repo = new F051803Repository(Schemas.CoreSchema, _wmsTransation);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);

			#region 資料檢核

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
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 便利商編號是否為空
			if (string.IsNullOrWhiteSpace(req.ConvenientCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21601", MsgContent = p81Service.GetMsg("21601") };

			// 廠退出貨單號是否為空
			if (string.IsNullOrWhiteSpace(req.RtnWmsNo))
				return new ApiResult { IsSuccessed = false, MsgCode = "21602", MsgContent = p81Service.GetMsg("21602") };

			// 廠商編號是否為空
			if (string.IsNullOrWhiteSpace(req.VnrCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21611", MsgContent = p81Service.GetMsg("21611") };

			// 便利倉儲格編號是否為空
			if (string.IsNullOrWhiteSpace(req.CellCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21612", MsgContent = p81Service.GetMsg("21612") };

			// 便利商編號、廠退出貨單號、廠商編號、便利倉儲格編號轉大寫
			req.ConvenientCode = req.ConvenientCode.ToUpper();
			req.RtnWmsNo = req.RtnWmsNo.ToUpper();
			req.VnrCode = req.VnrCode.ToUpper();
			req.CellCode = req.CellCode.ToUpper();
			if (!string.IsNullOrWhiteSpace(req.BookinCellCode))
				req.BookinCellCode = req.BookinCellCode.ToUpper();

			// [B] = 取得便利倉儲格資料 資料表: F051801 條件: DC_CODE = 傳入的DcNo AND CONVENIENT_CODE = 傳入的ConvenientCode AND CELL_CODE = 傳入的CellCode
			var f051801 = f051801Repo.AsForUpdate().Find(o => o.DC_CODE == req.DcNo && o.CONVENIENT_CODE == req.ConvenientCode && o.CELL_CODE == req.CellCode);
			// IF [B] 不存在，回傳訊息[便利倉儲格編號不存在]
			if (f051801 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21613", MsgContent = p81Service.GetMsg("21613") };

			// F[B]存在且[B].STATUS IN(1, 2)且VNR_CODE != 傳入的廠商編號，回傳[便利倉儲格已被XXX廠商使用，不可放入]
			var statusList = new List<string> { "1", "2" };
			if (statusList.Contains(f051801.STATUS) && f051801.VNR_CODE != req.VnrCode)
			{
				var f1908 = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.VNR_CODE == f051801.VNR_CODE);
				return new ApiResult { IsSuccessed = false, MsgCode = "21614", MsgContent = string.Format(p81Service.GetMsg("21614"), f1908 == null ? string.Empty : f1908.VNR_NAME) };
			}
			#endregion

			#region 資料處理
			// IF [B]存在且[B].STATUS=0 OR B.VNR_CODE = 傳入的廠商編號 往下執行
			if (f051801.STATUS == "0" || f051801.VNR_CODE == req.VnrCode)
			{
				#region 更新 F051801
				// IF [B].STATUS=0,更新[B].STATUS=2,VNR_CODE=傳入的廠商編號
				if (f051801.STATUS == "0" || f051801.STATUS == "1")
				{
					f051801.STATUS = "2";
					f051801.VNR_CODE = req.VnrCode;
					f051801.GUP_CODE = gupCode;
					f051801.CUST_CODE = req.CustNo;
					f051801Repo.Update(f051801);
				}
				#endregion

				// 5.	[C] = 取得廠退出貨單號對應的貨主單號[F160201.CUST_ORD_NO]
				//資料表: F160204 + F160201
				//條件: DC_CODE = 傳入的DcNo AND GUP_CODE =[A] AND CUST_CODE = 傳入的CustNo AND RTN_WMS_NO = 傳入的廠退出貨單號
				var custOrdNos = f160204Repo.GetCustOrdNosByRtnWmsNo(req.DcNo, gupCode, req.CustNo, req.RtnWmsNo);
				if (custOrdNos.Any())
				{
					#region 新增 F051802
					f051802Repo.BulkInsert(custOrdNos.Select(custOrdNo => new F051802
					{
						DC_CODE = req.DcNo,
						CONVENIENT_CODE = req.ConvenientCode,
						CELL_CODE = req.CellCode,
						VNR_CODE = req.VnrCode,
						GUP_CODE = gupCode,
						CUST_CODE = req.CustNo,
						WMS_NO = req.RtnWmsNo,
						CUST_ORD_NO = custOrdNo
					}).ToList());
					#endregion

					#region 新增 F051803 
					f051803Repo.BulkInsert(custOrdNos.Select(custOrdNo => new F051803
					{
						DC_CODE = req.DcNo,
						CONVENIENT_CODE = req.ConvenientCode,
						CELL_CODE = req.CellCode,
						VNR_CODE = req.VnrCode,
						GUP_CODE = gupCode,
						CUST_CODE = req.CustNo,
						WMS_NO = req.RtnWmsNo,
						CUST_ORD_NO = custOrdNo,
						STATUS = "1"
					}).ToList());
					#endregion
				}

				// F051801.STATUS=0,VNR_CODE=NULL,GUP_CODE=NULL,CUST_CODE=NULL WHERE DC_CODE = 傳入的DcNo and CELL_CODE <>傳入的CellCode AND STATUS=1
				f051801Repo.UpdateStatusByScheduledExcludeCellCode(req.DcNo, req.CellCode);

				_wmsTransation.Complete();
			}
			#endregion

			#region 準備訊息
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = p81Service.GetMsg("10020") };
			#endregion
		}

		/// <summary>
		/// 廠退便利倉-出場查詢
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult VnrConvenientOutQuery(VnrConvenientOutQueryReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051802Repo = new F051802Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);

			#region 資料檢核

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
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 便利商編號是否為空
			if (string.IsNullOrWhiteSpace(req.ConvenientCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21601", MsgContent = p81Service.GetMsg("21601") };

			// 廠商編號是否為空
			if (string.IsNullOrWhiteSpace(req.VnrCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21611", MsgContent = p81Service.GetMsg("21611") };

			// 便利倉編號、廠商編號轉大寫
			req.ConvenientCode = req.ConvenientCode.ToUpper();
			req.VnrCode = req.VnrCode.ToUpper();

			// 檢核廠商編號是否存在
			var f1908 = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.VNR_CODE == req.VnrCode);
			if (f1908 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21616", MsgContent = p81Service.GetMsg("21616") };

			// [B] = 取得廠商便利倉儲格資料
			// 資料表: F051802
			// 條件: DC_CODE = 傳入的DcNo AND CONVENIENT_CODE = 傳入的ConvenientCode AND VNR_CODE = 傳入的VnrCode AND GUP_CODE = [A] AND CUST_CODE = 傳入的CustNo
			var f051802s = f051802Repo.GetDetailData(req.DcNo, req.ConvenientCode, req.VnrCode, gupCode, req.CustNo);
			#endregion

			#region 資料處理
			var result = new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10001",
				MsgContent = p81Service.GetMsg("10001"),
				Data = new VnrConvenientOutQueryRes
				{
					Info = f051802s.Any() ? null : string.Format(p81Service.GetMsg("21615"), f1908.VNR_NAME),
					VnrCode = req.VnrCode,
					VnrName = f1908.VNR_NAME,
					CellList = f051802s.GroupBy(x => x.CELL_CODE).Select(x => new VnrConvenientOutQueryCellModel
					{
						CellCode = x.Key,
						WmsNoList = x.Where(z => !string.IsNullOrWhiteSpace(z.WMS_NO)).GroupBy(z => z.WMS_NO).Select(z => new VnrConvenientOutQueryVnrWmsNoModel
						{
							VnrWmsNo = z.Key
						}).OrderBy(z => z.VnrWmsNo).ToList()
					}).ToList()
				}
			};
			return result;
			#endregion
		}

		/// <summary>
		/// 廠退便利倉-出場儲格明細
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult VnrConvenientOutCellDetail(VnrConvenientOutCellDetailReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051801Repo = new F051801Repository(Schemas.CoreSchema);
			var f051802Repo = new F051802Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);

			#region 資料檢核

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
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 便利商編號是否為空
			if (string.IsNullOrWhiteSpace(req.ConvenientCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21601", MsgContent = p81Service.GetMsg("21601") };

			// 儲格編號是否為空
			if (string.IsNullOrWhiteSpace(req.CellCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21617", MsgContent = p81Service.GetMsg("21617") };

			// 廠商編號是否為空
			if (string.IsNullOrWhiteSpace(req.VnrCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21611", MsgContent = p81Service.GetMsg("21611") };

			// 便利倉編號、廠商編號轉大寫
			req.ConvenientCode = req.ConvenientCode.ToUpper();
			req.CellCode = req.CellCode.ToUpper();

			// 檢核廠商編號是否存在
			var f1908 = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.VNR_CODE == req.VnrCode);
			if (f1908 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21616", MsgContent = p81Service.GetMsg("21616") };

			//[B] = 取得廠商便利倉儲格資料
			//資料表: F051802
			//條件: DC_CODE = 傳入的DcNo
			//AND CONVENIENT_CODE = 傳入的ConvenientCode
			//AND CELL_CODE = 傳入的CellCode
			var f051802s = f051802Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.CONVENIENT_CODE == req.ConvenientCode && o.CELL_CODE == req.CellCode);
			// 如果[B]無資料，顯示訊息[無此廠商XXXX便利倉入場資料]
			if (!f051802s.Any())
			{
				//更新F051801.STATUS = 0 
				//WHHERE DC_CODE = 傳入的DcNo 
				//	AND CONVENIENT_CODE = 傳入的ConvenientCode 
				//	AND CELL_CODE = 傳入的CellCode(釋放此儲格)
				f051801Repo.UpdateStatusByScheduled(req.DcNo, req.ConvenientCode, req.CellCode, null, "0");
				//return new ApiResult { IsSuccessed = false, MsgCode = "21618", MsgContent = p81Service.GetMsg("21618") };
			}
			#endregion

			#region 資料處理
			return new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10001",
				MsgContent = p81Service.GetMsg("10001"),
				Data = new VnrConvenientOutCellDetailRes
				{
					VnrCode = req.VnrCode,
					VnrName = f1908.VNR_NAME,
					CellCode = req.CellCode,
					Info = f051802s.Any() ? null : p81Service.GetMsg("21618"),
					WmsNoList = f051802s.Select(x => new VnrConvenientOutCellDetailWmsNoModel
					{
						VnrWmsNo = x.WMS_NO,
						CustOrdNo = x.CUST_ORD_NO
					}).OrderBy(z => z.VnrWmsNo).ThenBy(z => z.CustOrdNo).ToList()
				}
			};
			#endregion
		}

		/// <summary>
		/// 廠退便利倉-出場確認
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult VnrConvenientOutConfirm(VnrConvenientOutConfirmReq req, string gupCode)
		{
			var p81Service = new P81Service();
			var f051801Repo = new F051801Repository(Schemas.CoreSchema);
			var f051802Repo = new F051802Repository(Schemas.CoreSchema);
			var f051803Repo = new F051803Repository(Schemas.CoreSchema, _wmsTransation);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);

			#region 資料檢核

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
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

			// 便利商編號是否為空
			if (string.IsNullOrWhiteSpace(req.ConvenientCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21601", MsgContent = p81Service.GetMsg("21601") };

			// 儲格編號是否為空
			if (string.IsNullOrWhiteSpace(req.CellCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21617", MsgContent = p81Service.GetMsg("21617") };

			// 廠退出貨單號是否為空
			if (string.IsNullOrWhiteSpace(req.RtnWmsNo))
				return new ApiResult { IsSuccessed = false, MsgCode = "21602", MsgContent = p81Service.GetMsg("21602") };

			// 檢核廠商編號是否存在
			var f1908 = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == req.CustNo && o.VNR_CODE == req.VnrCode);
			if (f1908 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21616", MsgContent = p81Service.GetMsg("21616") };

			// 便利倉編號、廠商編號轉大寫
			req.ConvenientCode = req.ConvenientCode.ToUpper();
			req.CellCode = req.CellCode.ToUpper();
			req.RtnWmsNo = req.RtnWmsNo.ToUpper();

			//[B] = 取得廠商便利倉儲格資料
			//資料表: F051802
			//條件: DC_CODE = 傳入的DcNo AND CONVENIENT_CODE = 傳入的ConvenientCode AND CELL_CODE = 傳入的CellCode AND RTN_WMS_NO = 傳入的RtnWmsNo
			var f051802s = f051802Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == req.DcNo && o.CONVENIENT_CODE == req.ConvenientCode && o.CELL_CODE == req.CellCode && o.WMS_NO == req.RtnWmsNo);
			// 如果[B]無資料，顯示訊息[此儲格無此廠退出貨單號]
			if (!f051802s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21619", MsgContent = p81Service.GetMsg("21619") };

			#endregion

			#region 資料處理
			// 由[B]產生新增F051803，STATUS=2(出場)
			f051803Repo.BulkInsert(f051802s.Select(x => new F051803
			{
				DC_CODE = x.DC_CODE,
				CONVENIENT_CODE = x.CONVENIENT_CODE,
				CELL_CODE = x.CELL_CODE,
				VNR_CODE = x.VNR_CODE,
				GUP_CODE = x.GUP_CODE,
				CUST_CODE = x.CUST_CODE,
				WMS_NO = x.WMS_NO,
				CUST_ORD_NO = x.CUST_ORD_NO,
				STATUS = "2"
			}).ToList());

			// 刪除F051802
			f051802Repo.DeleteByRtnWmsNo(req.DcNo, req.ConvenientCode, req.CellCode, req.RtnWmsNo);

			//[C] =檢查此儲格是否還有放入其他廠退出貨單
			//資料表: F051802
			// 條件: DC_CODE = 傳入的DcNo
			//AND CONVENIENT_CODE = 傳入的ConvenientCode
			//AND CELL_CODE = 傳入的CellCode
			//AND RTN_WMS_NO<> 傳入的RtnWmsNo
			var f051802sByNotWmsNo = f051802Repo.GetDataByNotRtnWmsNo(req.DcNo, gupCode, req.CustNo, req.ConvenientCode, req.CellCode, req.RtnWmsNo);

			var data = new VnrConvenientOutConfirmRes
			{
				VnrCode = req.VnrCode,
				VnrName = f1908.VNR_NAME
			};

			if (f051802sByNotWmsNo.Any())
			{
				data.IsReleaseCell = false;
			}
			else
			{
				// 如果[C]無資料，釋放儲格
				f051801Repo.UpdateStatusByScheduled(req.DcNo, req.ConvenientCode, req.CellCode, null, "0");
				data.IsReleaseCell = true;
			}
			data.Info = p81Service.GetMsg("10021");

			_wmsTransation.Complete();

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005"), Data = data };
			#endregion
		}
	}
}
