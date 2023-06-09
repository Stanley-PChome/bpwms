using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810103CustomService : P810103Service
    {
        public P810103CustomService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        /// <summary>
        /// 調撥明細查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override ApiResult GetAllocDetail(GetAllocDetailReq req, string gupCode)
        {
            var p81Service = new P81Service();
            var f151002Repo = new F151002Repository(Schemas.CoreSchema);

            #region 資料檢核

            // 帳號檢核
            var accData = p81Service.CheckAcc(req.AccNo);

            // 檢核人員功能權限
            var accFunctionCount = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

            // 檢核人員貨主權限
            var accCustCount = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

            // 檢核人員物流中心權限
            var accDcCount = p81Service.CheckAccDc(req.DcNo, req.AccNo);

            // 單據類別
            var allocTypeList = new List<string> { "01", "02", "03" };

            // 傳入的參數驗證
            if (string.IsNullOrWhiteSpace(req.FuncNo) ||
                    string.IsNullOrWhiteSpace(req.AccNo) ||
                    string.IsNullOrWhiteSpace(req.DcNo) ||
                    string.IsNullOrWhiteSpace(req.CustNo) ||
                    string.IsNullOrWhiteSpace(req.AllocNo) ||
                    string.IsNullOrWhiteSpace(req.AllocType) ||
                    !accData.Any() ||
                    accFunctionCount == 0 ||
                    accCustCount == 0 ||
                    accDcCount == 0 ||
                    !allocTypeList.Contains(req.AllocType))
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            #endregion

            #region 資料處理
            var result = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

            var resData = new GetAllocDetailRes();

            // 取得資料處理1 Data
            var detailList = f151002Repo.GetAllocDetailAllCol(
                            req.DcNo,
                            req.CustNo,
                            gupCode,
                            req.AllocNo,
                            req.AllocType);

            if (detailList.Any())
            {
                #region Item
                // 取得調撥路線編號
                var routeDataList = p81Service.GetRouteList(detailList.Select(x => new GetRouteListReq
                {
                    No = x.AllocNo,
                    Seq = Convert.ToInt32(x.AllocSeq),
                    LocCode = x.SugLoc
                }).ToList());


                resData.DetailList = (from A in detailList
                                      join B in routeDataList
                                      on new { No = A.AllocNo, Seq = Convert.ToInt32(A.AllocSeq), LocCode = A.SugLoc } equals new { B.No, B.Seq, B.LocCode }
                                      select new GetAllocDetail
                                      {
                                          AllocNo = A.AllocNo,
                                          AllocSeq = A.AllocSeq,
                                          ItemNo = A.ItemNo,
                                          WhName = A.WhName,
                                          SugLoc = A.SugLoc,
                                          ValidDate = A.ValidDate,
                                          EnterDate = A.EnterDate,
                                          MkNo = A.MkNo,
                                          Sn = A.Sn,
                                          Qty = A.Qty,
                                          ActQty = A.ActQty,
                                          PalletNo = A.PalletNo,
                                          EanCode1 = A.EanCode1,
                                          Unit = A.Unit,
                                          ProductName = A.ProductName,
                                          ProductSize = A.ProductSize,
                                          ProductColor = A.ProductColor,
                                          ProductSpec = A.ProductSpec,
                                          Barcode1 = A.Barcode1,
                                          Barcode2 = A.Barcode2,
                                          Barcode3 = A.Barcode3,
                                          Weight = A.Weight,
                                          BoxQty = A.BoxQty,
                                          Route = B.Route,
                                          CustNo = A.CustNo
                                      }).ToList();
                #endregion

                #region ItemSn
                List<string> rtNo = null;

                var itemNoArray = detailList.Select(x => x.ItemNo).Distinct().ToList();
                if (req.AllocType.Contains("01"))
                {
                    // (1) 驗收單號 = Select RT_NO from F02020107 by dc_code、gup_code、cust_code and allocation_no = AllocNo
                    // (2) 商品序號資料 = &取得商品序號清單[CustNo, F151002[item_code清單], 驗收單號]
                    var f02020107Repo = new F02020107Repository(Schemas.CoreSchema);
                    rtNo = f02020107Repo.GetRtNo(req.DcNo, req.CustNo, gupCode, req.AllocNo).ToList();

                    resData.ItemSnList = p81Service.GetSnList(req.DcNo, req.CustNo, gupCode, itemNoArray, rtNo).ToList();
                }
                else
                {
                    var snList = detailList.Where(x => !string.IsNullOrWhiteSpace(x.Sn)).Select(x => x.Sn).ToList();
                    resData.ItemSnList = p81Service.GetSnList(req.DcNo, req.CustNo, gupCode, itemNoArray, null, snList).ToList();
                }
                #endregion
            }

            result.Data = resData;

            #endregion

            return result;
        }
    }
}
