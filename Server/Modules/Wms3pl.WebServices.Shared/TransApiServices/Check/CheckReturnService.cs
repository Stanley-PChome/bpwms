using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
    public class CheckReturnService
    {
        private TransApiBaseService tacService = new TransApiBaseService();

        /// <summary>
        /// 檢查ProcFlag
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        /// <returns></returns>
        public void CheckProcFlag(List<ApiResponse> res, PostCreateReturnsModel returns)
        {
            List<string> procFlags = new List<string> { "0", "D" };
            if (!procFlags.Contains(returns.ProcFlag))
                res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20961", MsgContent = string.Format(tacService.GetMsg("20961"), returns.CustReturnNo) });
        }

        /// <summary>
        /// 檢查刪除狀態貨主單號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="thirdPartOrdersList"></param>
        /// <param name="warehouseIns"></param>
        /// <returns></returns>
        public void CheckCustExistForThirdPart(List<ApiResponse> res, List<ThirdPartReturns> thirdPartReturnsList, PostCreateReturnsModel returns)
        {
            var currCustData = thirdPartReturnsList.Where(x => x.CUST_ORD_NO == returns.CustReturnNo);
            if (returns.ProcFlag == "D" && !currCustData.Any())
                res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), returns.CustReturnNo) });
            else if (returns.ProcFlag == "0" && currCustData.Any())
                res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), returns.CustReturnNo) });
        }

        /// <summary>
        /// 檢核單據類型是否正確
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckTypeExist(List<ApiResponse> res, PostCreateReturnsModel returns)
        {
            if (returns.ProcFlag == "0")
            {
                List<string> typeList = new List<string> { "0", "1" };

                if (!typeList.Contains(returns.Type))
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20951", MsgContent = string.Format(tacService.GetMsg("20951"), returns.CustReturnNo) });
                }
            }
        }

        /// <summary>
        /// 檢核出貨單號是否存在
        /// </summary>
        /// <param name="data"></param>
        /// <param name="wmsOrdNoList"></param>
        /// <param name="returns"></param>
        public void CheckWmsOrdNoExist(List<ApiResponse> res, List<string> wmsOrdNoList, PostCreateReturnsModel returns)
        {
            if (returns.ProcFlag == "0" && !wmsOrdNoList.Contains(returns.WmsOrdNo))
            {
                res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20061", MsgContent = string.Format(tacService.GetMsg("20061"), returns.WmsOrdNo) });
            }
        }

        /// <summary>
        /// 檢查貨主單號是否存在
        /// </summary>
        /// <param name="res"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="warehouseIns"></param>
        public void CheckCustExist(ref List<F161201> f161201List, ref List<F161202> f161202List, string dcCode, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            // 檢查貨主單號是否存在
            // [找到的WMS單]=檢查<參數4>.CustInNo是否存在於[貨主單號已產生WMS訂單] 中找CUST_ORD_NO =< 參數4 >.CustInNo
            var delF161201 = f161201List.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.CUST_ORD_NO == returns.CustReturnNo).SingleOrDefault();
            if (delF161201 != null)
            {
                // 移除之前Cache已新增的進倉單、進倉單明細
                f161201List.Remove(delF161201);

                var delF161202List = f161202List.Where(x => x.DC_CODE == dcCode &&
                                                            x.GUP_CODE == gupCode &&
                                                            x.CUST_CODE == custCode &&
                                                            x.RETURN_NO == delF161201.RETURN_NO).ToList();

                f161202List = f161202List.Except(delF161202List).ToList();
            }
        }

        /// <summary>
        /// 檢查門市編號
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckReturnCustCode(List<ApiResponse> res, PostCreateReturnsModel returns, List<F1910> retailList)
        {
            if (returns.ProcFlag == "0")
            {
                var retailData = retailList.Where(x => x.RETAIL_CODE == returns.ReturnCustCode);
                if (returns.Type == "0" && !retailData.Any())
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20960", MsgContent = string.Format(tacService.GetMsg("20960"), returns.CustReturnNo, returns.ReturnCustCode) });
                }
            }
        }

        /// <summary>
        /// 檢查退貨類型
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckReturnType(List<ApiResponse> res, PostCreateReturnsModel returns, List<F161203> returnTypeList)
        {
            if (returns.ProcFlag == "0")
            {
                if (!returnTypeList.Select(x => x.RTN_TYPE_ID).Contains(returns.ReturnType))
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20958", MsgContent = string.Format(tacService.GetMsg("20958"), returns.CustReturnNo, returns.ReturnType) });
                }
            }
        }

        /// <summary>
        /// 檢查退貨原因
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckReturnCause(List<ApiResponse> res, PostCreateReturnsModel returns, List<F1951> returnCauseList)
        {
            if (returns.ProcFlag == "0")
            {
                if (!returnCauseList.Select(x => x.UCC_CODE).Contains(returns.ReturnCause))
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20959", MsgContent = string.Format(tacService.GetMsg("20959"), returns.CustReturnNo, returns.ReturnCause) });
                }
            }
        }


        /// <summary>
        /// 檢查明細筆數
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckDetailCnt(List<ApiResponse> res, PostCreateReturnsModel returns)
        {
            if (returns.ProcFlag == "0" && (returns.ReturnDetails == null || (returns.ReturnDetails != null && returns.ReturnDetails.Count == 0)))
            {
                res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20071", MsgContent = string.Format(tacService.GetMsg("20071"), returns.CustReturnNo) });
            }
        }

        /// <summary>
        /// 檢核項次必須大於0，且同一張單據內的序號不可重複
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckDetailSeq(List<ApiResponse> res, PostCreateReturnsModel returns)
        {
            if (returns.ProcFlag == "0" && returns.ReturnDetails != null && returns.ReturnDetails.Count > 0)
            {
                if (returns.ReturnDetails.Where(x => string.IsNullOrWhiteSpace(x.ItemSeq)).Any() ||
                    returns.ReturnDetails.Count > returns.ReturnDetails.Select(x => x.ItemSeq).Distinct().Count())
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20070", MsgContent = tacService.GetMsg("20070") });
                }
            }
        }

        /// <summary>
        /// 檢查明細進倉數量
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckDetailQty(List<ApiResponse> res, PostCreateReturnsModel returns)
        {
            if (returns.ProcFlag == "0" && returns.ReturnDetails != null && returns.ReturnDetails.Count > 0)
            {
                var itemSeqList = returns.ReturnDetails.Where(x => x.Qty <= 0).Select(x => x.ItemSeq).Distinct().ToList();

                if (itemSeqList.Count > 0)
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20060", MsgContent = string.Format(tacService.GetMsg("20060"), returns.CustReturnNo, string.Join("、", itemSeqList)) });
                }
            }
        }

        /// <summary>
        /// 檢查資料是否完整
        /// </summary>
        /// <param name="res"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returns"></param>
        /// <param name="retailList"></param>
        /// <param name="returnTypeList"></param>
        /// <param name="returnCauseList"></param>
        public void CheckReturnData(List<ApiResponse> res, string gupCode, string custCode, PostCreateReturnsModel returns)
        {
            if (returns.ReturnDetails.Any() && returns.ProcFlag == "0")
            {
                CommonService commonService = new CommonService();
                TransApiBaseService tacService = new TransApiBaseService();

                var itemCodeList = returns.ReturnDetails.Select(x => x.ItemCode).Distinct().ToList();

                // [商品資料] = 取得商品資料[< 參數1 >.GUP_CODE,< 參數1 >.CUST_CODE, DISTINCT<參數2>.ITEM_CODE]
                var productList = commonService.GetProductList(gupCode, custCode, itemCodeList);

                // [差異品號清單] = DISTINCT<參數2>.ITEM_CODE 比較[商品資料].ITEM_CODE 是否有差異(訂單品號無商品主檔資料)
                var differentData = itemCodeList.Except(productList.Select(x => x.ITEM_CODE));

                #region 檢查差異品號
                if (differentData.Any())
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20086", MsgContent = string.Format(tacService.GetMsg("20086"), returns.CustReturnNo, string.Join("、", differentData)) });
                }
                #endregion

                #region 檢查停售品號
                // [停售品號清單] = 檢查[商品資料]的停售日期(STOP_DATE) <= 系統日(DateTime.Today)
                var cessationOfSaleList = productList.Where(x => x.STOP_DATE <= DateTime.Today).Select(x => x.ITEM_CODE);

                // IF[停售品號清單].Any() then
                if (cessationOfSaleList.Any())
                {
                    res.Add(new ApiResponse { No = returns.CustReturnNo, MsgCode = "20087", MsgContent = string.Format(tacService.GetMsg("20087"), returns.CustReturnNo, string.Join("、", cessationOfSaleList)) });
                }
                #endregion
            }
        }
    }
}
