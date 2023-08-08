using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810112Service
    {
        private WmsTransaction _wmsTransation;
        private CommonService _commonService;

        public P810112Service(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        /// <summary>
        /// 進貨收發-收貨確認
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult ConfirmRcvStockData(ConfirmRcvStockDataReq req, string gupCode)
        {
            var p81Service = new P81Service();
            var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransation);
            var f010202Repo = new F010202Repository(Schemas.CoreSchema);
            var f0202Repo = new F0202Repository(Schemas.CoreSchema, _wmsTransation);
            var f1903Repo = new F1903Repository(Schemas.CoreSchema);
            var f1946Repo = new F1946Repository(Schemas.CoreSchema);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var stowShelfAreaService = new StowShelfAreaService();
            var recvItemService = new RecvItemService();
            var apiInfoList = new List<string>();

            if (_commonService == null)
            {
              _commonService = new CommonService();
            }

            if(!string.IsNullOrWhiteSpace(req.StockNo))
                req.StockNo = req.StockNo.ToUpper();
            if (!string.IsNullOrWhiteSpace(req.WorkstationCode))
                req.WorkstationCode = req.WorkstationCode.ToUpper();

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

            // 檢核傳入進貨單號/貨主單號不得為空
            if (string.IsNullOrWhiteSpace(req.StockNo))
                return new ApiResult { IsSuccessed = false, MsgCode = "21201", MsgContent = p81Service.GetMsg("21201") };

            // 取得進倉單資料，檢核是否有無進倉單資料
            var f010201 = f010201Repo.FindDataByStockNoOrCustOrdNo(req.DcNo, gupCode, req.CustNo, req.StockNo);
            if (f010201 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "21202", MsgContent = p81Service.GetMsg("21202") };

            // 檢核該進倉單是否為跨併的進貨單
            if (f010201.CUST_COST == "MoveIn")
                return new ApiResult { IsSuccessed = false, MsgCode = "21203", MsgContent = p81Service.GetMsg("21203") };

            // 檢核該進倉單狀態是否可以進行收貨
            if (f010201.STATUS != "0")
                return new ApiResult { IsSuccessed = false, MsgCode = "21204", MsgContent = string.Format(p81Service.GetMsg("21204"), f010201.STOCK_NO, p81Service.GetTopicValueNameByVW("F010201", "STATUS", f010201.STATUS)) };

            // 取得進倉作業與影資系統整合設定(0:否、1:是)
            var videoCombinIn = _commonService.GetSysGlobalValue(req.DcNo, "VideoCombinIn");
            if (string.IsNullOrWhiteSpace(videoCombinIn))
                return new ApiResult { IsSuccessed = false, MsgCode = "21206", MsgContent = p81Service.GetMsg("21206") };

            // 若要與影資系統整合，工作站編號需必填
            if (videoCombinIn == "1")
            {
                if (string.IsNullOrWhiteSpace(req.WorkstationCode))
                    return new ApiResult { IsSuccessed = false, MsgCode = "21207", MsgContent = p81Service.GetMsg("21207") };
                else
                {
                    if (req.WorkstationCode.Length != 4)
                    {
                        return new ApiResult { IsSuccessed = false, MsgCode = "21209", MsgContent = p81Service.GetMsg("21209") };
                    }

                    var f1946s = f1946Repo.GetDatasByTrueAndCondition(o =>o.DC_CODE == req.DcNo && o.WORKSTATION_CODE == req.WorkstationCode);

                    if (!f1946s.Any())
                    {
                        return new ApiResult { IsSuccessed = false, MsgCode = "21210", MsgContent = string.Format(p81Service.GetMsg("21210"), req.WorkstationCode) };
                    } else if (!f1946s.FirstOrDefault().WORKSTATION_CODE.StartsWith("G"))
                    {
                        return new ApiResult { IsSuccessed = false, MsgCode = "21208", MsgContent = string.Format(p81Service.GetMsg("21208"), req.WorkstationCode) };
                    }
                }
            }
            #endregion

            var res = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005") };

            #region 呼叫上架倉別指示API
            // 取得進倉單明細F010202
            var f010202s = f010202Repo.GetDatasByTrueAndCondition(o =>
            o.DC_CODE == f010201.DC_CODE &&
            o.GUP_CODE == f010201.GUP_CODE &&
            o.CUST_CODE == f010201.CUST_CODE &&
            o.STOCK_NO == f010201.STOCK_NO);

            // 執行Lms上架倉別指示
            var custInNo = f010201.CUST_ORD_NO;
#if (DEBUG || TEST)
            if (string.IsNullOrWhiteSpace(custInNo))
                custInNo = f010201.STOCK_NO;
#endif
            var lmsRes = stowShelfAreaService.StowShelfAreaGuide(req.DcNo, gupCode, req.CustNo, "1" , custInNo, f010202s.Select(x => x.ITEM_CODE).Distinct().ToList());

            var data = new List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>();

            if (lmsRes.IsSuccessed)
                // A.請求成功(Http code = 200，Code = 200)，得到[上架倉別指示的回應資料]，IsSuccessed = true
                data = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>>(
                                    JsonConvert.SerializeObject(lmsRes.Data));
            else
                // B.請求失敗，(Http code = 200，Code != 200)，紀錄ApiInfo =”LMS回覆: ”+Msg +“，雖然無法取得上架倉別的指示，但仍然可視為收貨成功”，IsSuccessed = true
                // PS.雖然請求失敗，但仍然可以視為收貨成功
                apiInfoList.Add($"LMS回覆:{lmsRes.MsgContent} 雖然無法取得上架倉別的指示，但仍然可視為收貨成功");
            #endregion

            #region 更新F010201
            f010201.STATUS = "3"; // 已點收
            f010201Repo.Update(f010201);
            #endregion

            #region 新增F0202
            f0202Repo.Add(new F0202
            {
                DC_CODE = req.DcNo,
                GUP_CODE = gupCode,
                CUST_CODE = req.CustNo,
                ORDER_NO = f010201.STOCK_NO,
                VNR_CODE = f010201.VNR_CODE,
                CHECKIN_DATE = DateTime.Now,
                ORDER_WEIGHT = Convert.ToDecimal(0.00),
                BOX_QTY = 0
            });
            #endregion

            _wmsTransation.Complete();

            #region 結果回覆

            var f010202 = f010202s.First();

            var f1903 = f1903Repo.Find(o =>
            o.GUP_CODE == f010201.GUP_CODE &&
            o.CUST_CODE == f010201.CUST_CODE &&
            o.ITEM_CODE == f010202.ITEM_CODE);

            string warehouseId = string.Empty;
            string warehouseName = "無建議上架倉別";
            string warehouseIdType = "1";

            if (data != null && data.Any())
            {
                var item = data.Where(x => x.ItemCode == f010202.ITEM_CODE).FirstOrDefault();

                if (item != null)
                {
                    var f1980 = f1980Repo.Find(o =>
                    o.DC_CODE == f010201.DC_CODE &&
                    o.WAREHOUSE_ID == item.ShelfAreaCode);

                    if (f1980 != null)
                    {
                        warehouseId = f1980.WAREHOUSE_ID;
                        warehouseName = f1980.WAREHOUSE_NAME;
                        warehouseIdType = f1980.DEVICE_TYPE;
                    }
                }
            }

            var resData = new ConfirmRcvStockDataRes
            {
                StockNo = f010201.STOCK_NO,
                CustOrdNo = f010201.CUST_ORD_NO,
                FastPassType = f010201.FAST_PASS_TYPE,
                FastPassTypeName = p81Service.GetTopicValueNameByVW("F010201", "FAST_PASS_TYPE", f010201.FAST_PASS_TYPE),
                ItemCode = f010202.ITEM_CODE,
                CustItemCode = f1903.CUST_ITEM_CODE,
                ItemName = f1903.ITEM_NAME,
                WarehouseId = warehouseId,
                WarehouseName = warehouseName,
                WarehouseIdType = warehouseIdType
            };
            #endregion

            #region 呼叫Wcssr收單驗貨API
            if (videoCombinIn == "1")
            {
                var wcssrApiRes = recvItemService.RecvItemNotify(req.DcNo, gupCode, req.CustNo, new Datas.Shared.ApiEntities.RecvItemNotifyReq
                {
                    WhId = f010201.DC_CODE,
                    OrderNo = f010201.STOCK_NO,
                    WorkStationId = req.WorkstationCode,
                    TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                });

								if (!string.IsNullOrEmpty(wcssrApiRes.MsgContent))
									apiInfoList.Add(wcssrApiRes.MsgContent);

						}
            #endregion

            if (apiInfoList.Any())
                resData.ApiInfo = string.Join(Environment.NewLine, apiInfoList);

            res.Data = resData;

            return res;
        }

        /// <summary>
        /// 進貨收發-詳細資訊
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult RcvStockDetailData(RcvStockDetailDataReq req, string gupCode)
        {
            var p81Service = new P81Service();
            var f010201Repo = new F010201Repository(Schemas.CoreSchema);
            var f010202Repo = new F010202Repository(Schemas.CoreSchema);
            var f1903Repo = new F1903Repository(Schemas.CoreSchema);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var stowShelfAreaService = new StowShelfAreaService();
            var apiinfo = string.Empty;

            if (_commonService == null)
            {
              _commonService = new CommonService();
            }

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

            // 檢核傳入進貨單號不得為空
            if (string.IsNullOrWhiteSpace(req.StockNo))
                return new ApiResult { IsSuccessed = false, MsgCode = "21205", MsgContent = p81Service.GetMsg("21205") };

            // 取得進倉單資料，檢核是否有無進倉單資料
            var f010201 = f010201Repo.Find(o =>
            o.DC_CODE == req.DcNo &&
            o.GUP_CODE == gupCode &&
            o.CUST_CODE == req.CustNo &&
            o.STOCK_NO == req.StockNo);
            if (f010201 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "21202", MsgContent = p81Service.GetMsg("21202") };
            #endregion

            var res = new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = p81Service.GetMsg("10005") };

            #region 呼叫上架倉別指示API
            // 取得進倉單明細F010202
            var f010202s = f010202Repo.GetDatasByTrueAndCondition(o =>
            o.DC_CODE == f010201.DC_CODE &&
            o.GUP_CODE == f010201.GUP_CODE &&
            o.CUST_CODE == f010201.CUST_CODE &&
            o.STOCK_NO == f010201.STOCK_NO);

            var itemCodeList = f010202s.Select(x => x.ITEM_CODE).Distinct().ToList();

            // 執行Lms上架倉別指示
            var custInNo = f010201.CUST_ORD_NO;
#if (DEBUG || TEST)
            if (string.IsNullOrWhiteSpace(custInNo))
                custInNo = f010201.STOCK_NO;
#endif
            var lmsRes = stowShelfAreaService.StowShelfAreaGuide(req.DcNo, gupCode, req.CustNo, "1", custInNo, itemCodeList);

            var data = new List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>();

            if (lmsRes.IsSuccessed)
                // A.請求成功(Http code = 200，Code = 200)，得到[上架倉別指示的回應資料]，IsSuccessed = true
                data = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>>(
                                    JsonConvert.SerializeObject(lmsRes.Data));
            else
                // B.請求失敗，(Http code = 200，Code != 200)，紀錄ApiInfo =”LMS回覆: ”+Msg +“，雖然無法取得上架倉別的指示，但仍然可視為收貨成功”，IsSuccessed = true
                // PS.雖然請求失敗，但仍然可以視為收貨成功
                apiinfo = $"{lmsRes.MsgContent}，雖然無法取得上架倉別的指示，但仍然可視為收貨成功";
            #endregion

            #region 組回傳資料
            // 商品明細F1903
            var f1903s = _commonService.GetProductList(f010201.GUP_CODE, f010201.CUST_CODE, itemCodeList);

            var f1980s = f1980Repo.GetDatasByWarehouseId(f010201.DC_CODE, data.Select(x => x.ShelfAreaCode).Distinct().ToList());

            var lmsData = from A in data
                          join B in f1980s
                          on A.ShelfAreaCode equals B.WAREHOUSE_ID
                          select new
                          {
                              A.ItemCode,
                              WhId = B.WAREHOUSE_ID,
                              WhName = B.WAREHOUSE_NAME
                          };

            res.Data = new RcvStockDetailDataRes
            {
                StockNo = f010201.STOCK_NO,
                CustOrdNo = f010201.CUST_ORD_NO,
                DeliverDate = f010201.DELIVER_DATE,
                BookingInPeriod = p81Service.GetTopicValueNameByVW("F010201", "BOOKING_IN_PERIOD", f010201.BOOKING_IN_PERIOD),
                VnrCode = f010201.VNR_CODE,
                FastPassType = p81Service.GetTopicValueNameByVW("F010201", "FAST_PASS_TYPE", f010201.FAST_PASS_TYPE),
                ApiInfo = apiinfo,
                Detail = (from A in f010202s
                          join B in f1903s
                          on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                          join C in lmsData
                          on A.ITEM_CODE equals C.ItemCode into subC
                          from C in subC.DefaultIfEmpty()
                          select new RcvStockDetailDataDetail
                          {
                              ItemCode = A.ITEM_CODE,
                              ItemName = B.ITEM_NAME,
                              CustItemCode = B.CUST_ITEM_CODE,
                              WarehouseId = C == null ? string.Empty : C.WhId,
                              WarehouseName = C == null ? "無建議上架倉別" : C.WhName
                          }).ToList()
            };

            return res;
            #endregion

        }
    }
}
