using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
	public class CheckVendorReturnService
	{
		private TransApiBaseService tacService = new TransApiBaseService();

		/// <summary>
		/// 檢查ProcFlag
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		public void CheckProcFlag(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns)
		{
			List<string> procFlags = new List<string> { "0", "D" };
			if (!procFlags.Contains(vnrReturns.ProcFlag))
				res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20961", MsgContent = string.Format(tacService.GetMsg("20961"), vnrReturns.CustVnrRetrunNo) });
		}

		/// <summary>
		/// 檢查刪除狀態貨主單號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="thirdPartVnrReturnsList"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		public bool CheckCustExistForThirdPart(List<ApiResponse> res, List<ThirdPartVendorReturns> thirdPartVnrReturnsList, PostCreateVendorReturnsModel vnrReturns, string custCode)
		{
			var isAddF075103 = false;
			var f075103Repo = new F075103Repository(Schemas.CoreSchema);
			var currCustData = thirdPartVnrReturnsList.Where(x => x.CUST_ORD_NO == vnrReturns.CustVnrRetrunNo);
			if (vnrReturns.ProcFlag == "D")
			{
				if (!currCustData.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), vnrReturns.CustVnrRetrunNo) });
				else
					f075103Repo.DelF075103ByKey(custCode, vnrReturns.CustVnrRetrunNo);
			}
			else if (vnrReturns.ProcFlag == "0")
			{
				if (currCustData.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), vnrReturns.CustVnrRetrunNo) });
				else
				{
					#region 新增廠退單匯入控管紀錄表
					var f075103Res = f075103Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
						new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
					() =>
					{
						var lockF075103 = f075103Repo.LockF075103();
						var f075103 = f075103Repo.Find(o => o.CUST_CODE == custCode && o.CUST_ORD_NO == vnrReturns.CustVnrRetrunNo, isForUpdate: true, isByCache: false);
						if (f075103 == null)
						{
							f075103 = new F075103 { CUST_CODE = custCode, CUST_ORD_NO = vnrReturns.CustVnrRetrunNo };
							f075103Repo.Add(f075103);
							isAddF075103 = true;
						}
						else
						{
							f075103 = null; // 代表F075103已存在資料
						}
						return f075103;
					});
					if (f075103Res == null)// 代表F075103已存在資料
						res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), vnrReturns.CustVnrRetrunNo) });
					#endregion
				}
			}

			return isAddF075103;
		}

		/// <summary>
		/// 檢查廠商編號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrCodeList"></param>
		/// <param name="vnrReturns"></param>
		public void CheckVnrCodeExist(List<ApiResponse> res, List<F1908> vnrCodeList, PostCreateVendorReturnsModel vnrReturns)
		{
			if (vnrReturns.ProcFlag == "0" && !vnrCodeList.Select(x => x.VNR_CODE).Contains(vnrReturns.SupCode))
				res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "21057", MsgContent = string.Format(tacService.GetMsg("21057"), vnrReturns.CustVnrRetrunNo, vnrReturns.SupCode) });
		}

		/// <summary>
		/// 檢查出貨倉別是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrCodeList"></param>
		/// <param name="vnrReturns"></param>
		public void CheckTypeId(List<ApiResponse> res, List<string> typeIdList, PostCreateVendorReturnsModel vnrReturns)
		{
			if (vnrReturns.ProcFlag == "0" && !typeIdList.Contains(vnrReturns.TypeId))
				res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20759", MsgContent = string.Format(tacService.GetMsg("20759"), vnrReturns.CustVnrRetrunNo, vnrReturns.TypeId) });
		}


		/// <summary>
		/// 檢查配送方式是否正確
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrCodeList"></param>
		/// <param name="vnrReturns"></param>
		public void CheckDeliveryWay(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns)
		{
			var deliveryWay = new List<string> { "0", "1" };
			if (vnrReturns.ProcFlag == "0" && !string.IsNullOrWhiteSpace(vnrReturns.DeliveryWay) && !deliveryWay.Contains(vnrReturns.DeliveryWay))
				res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), vnrReturns.CustVnrRetrunNo, "配送方式") });
		}

		/// <summary>
		/// 檢查貨主單號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="vnrReturns"></param>
		public void CheckCustExist(ref List<F160201> f160201List, ref List<F160202> f160202List, string dcCode, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns)
		{
			// 檢查貨主單號是否存在
			// [找到的WMS單]=檢查<參數4>.CustInNo是否存在於[貨主單號已產生WMS訂單] 中找CUST_ORD_NO =< 參數4 >.CustInNo
			var delF160201 = f160201List.Where(x => x.DC_CODE == dcCode &&
																							x.GUP_CODE == gupCode &&
																							x.CUST_CODE == custCode &&
																							x.CUST_ORD_NO == vnrReturns.CustVnrRetrunNo).SingleOrDefault();
			if (delF160201 != null)
			{
				// 移除之前Cache已新增的進倉單、進倉單明細
				f160201List.Remove(delF160201);

				var delF160202List = f160202List.Where(x => x.DC_CODE == dcCode &&
																										x.GUP_CODE == gupCode &&
																										x.CUST_CODE == custCode &&
																										x.RTN_VNR_NO == delF160201.RTN_VNR_NO).ToList();

				f160202List = f160202List.Except(delF160202List).ToList();
			}
		}

		/// <summary>
		/// 檢查明細筆數
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrReturns"></param>
		public void CheckDetailCnt(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns)
		{
			if (vnrReturns.ProcFlag == "0" && (vnrReturns.VnrReturnDetails == null || (vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Count == 0)))
				res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20071", MsgContent = string.Format(tacService.GetMsg("20071"), vnrReturns.CustVnrRetrunNo) });
		}

		/// <summary>
		/// 檢核項次必須大於0，且同一張單據內的序號不可重複
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrReturns"></param>
		public void CheckDetailSeq(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns)
		{
			if (vnrReturns.ProcFlag == "0" && vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Count > 0)
			{
				if (vnrReturns.VnrReturnDetails.Where(x => string.IsNullOrWhiteSpace(x.ItemSeq)).Any() ||
						vnrReturns.VnrReturnDetails.Count > vnrReturns.VnrReturnDetails.Select(x => x.ItemSeq).Distinct().Count())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20070", MsgContent = tacService.GetMsg("20070") });
			}
		}

		/// <summary>
		/// 檢查明細進倉數量
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrReturns"></param>
		public void CheckDetailQty(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns)
		{
			if (vnrReturns.ProcFlag == "0" && vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Count > 0)
			{
				var itemSeqList = vnrReturns.VnrReturnDetails.Where(x => x.ReturnQty <= 0).Select(x => x.ItemSeq).Distinct().ToList();
				if (itemSeqList.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20060", MsgContent = string.Format(tacService.GetMsg("20060"), vnrReturns.CustVnrRetrunNo, string.Join("、", itemSeqList)) });
			}
		}

		/// <summary>
		/// 檢查明細廠退原因
		/// </summary>
		/// <param name="res"></param>
		/// <param name="vnrReturns"></param>
		/// <param name="vnrReturnCauseList"></param>
		public void CheckVnrReturnCause(List<ApiResponse> res, PostCreateVendorReturnsModel vnrReturns, List<string> vnrReturnCauseList)
		{
			if (vnrReturns.ProcFlag == "0" && vnrReturns.VnrReturnDetails != null && vnrReturns.VnrReturnDetails.Count > 0)
			{
				var itemSeqList = vnrReturns.VnrReturnDetails.Where(x => !vnrReturnCauseList.Contains(x.VnrReturnCause)).Select(x => x.ItemSeq).Distinct().ToList();
				if (itemSeqList.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20100", MsgContent = string.Format(tacService.GetMsg("20100"), vnrReturns.CustVnrRetrunNo, string.Join("、", itemSeqList)) });
			}
		}

		/// <summary>
		/// 檢查資料是否完整
		/// </summary>
		/// <param name="res"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="vnrReturns"></param>
		/// <param name="vnrReturnTypeList"></param>
		public void CheckReturnData(List<ApiResponse> res, string gupCode, string custCode, PostCreateVendorReturnsModel vnrReturns, List<F160203> vnrReturnTypeList)
		{
			CommonService commonService = new CommonService();
			TransApiBaseService tacService = new TransApiBaseService();

			if (vnrReturns.VnrReturnDetails.Any() && vnrReturns.ProcFlag == "0")
			{
				var itemCodeList = vnrReturns.VnrReturnDetails.Select(x => x.ItemCode).Distinct().ToList();

				// [商品資料] = 取得商品資料[< 參數1 >.GUP_CODE,< 參數1 >.CUST_CODE, DISTINCT<參數2>.ITEM_CODE]
				var productList = commonService.GetProductList(gupCode, custCode, itemCodeList);

				// [差異品號清單] = DISTINCT<參數2>.ITEM_CODE 比較[商品資料].ITEM_CODE 是否有差異(訂單品號無商品主檔資料)
				var differentData = itemCodeList.Except(productList.Select(x => x.ITEM_CODE));

				#region 檢查退貨類型

				if (!vnrReturnTypeList.Select(x => x.RTN_VNR_TYPE_ID).Contains(vnrReturns.VnrReturnType))
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20088", MsgContent = string.Format(tacService.GetMsg("20088"), vnrReturns.CustVnrRetrunNo) });

				#endregion

				#region 檢查差異品號
				if (differentData.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20086", MsgContent = string.Format(tacService.GetMsg("20086"), vnrReturns.CustVnrRetrunNo, string.Join("、", differentData)) });

				#endregion

				#region 檢查停售品號
				// [停售品號清單] = 檢查[商品資料]的停售日期(STOP_DATE) <= 系統日(DateTime.Today)
				var cessationOfSaleList = productList.Where(x => x.STOP_DATE <= DateTime.Today).Select(x => x.ITEM_CODE);

				// IF[停售品號清單].Any() then
				if (cessationOfSaleList.Any())
					res.Add(new ApiResponse { No = vnrReturns.CustVnrRetrunNo, MsgCode = "20087", MsgContent = string.Format(tacService.GetMsg("20087"), vnrReturns.CustVnrRetrunNo, string.Join("、", cessationOfSaleList)) });

				#endregion
			}
		}
	}
}
