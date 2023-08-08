using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.Lms.WebApiConnectSetting;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.ForeignWebApi.Business.LmsServices
{
    public class ExportOrderService : LmsBaseService
    {
        private WmsTransaction _wmsTransaction;

        public ExportOrderService(WmsTransaction wmsTransation)
        {
            _wmsTransaction = wmsTransation;
        }

        /// <summary>
        /// 匯出訂單出貨結果
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        public ApiResult ExportOrderResults(string dcCode, string gupCode, string custCode)
        {
            ApiResult res = new ApiResult { IsSuccessed = true };
            List<ApiResponse> data = new List<ApiResponse>();
            var f050305Repo = new F050305Repository(Schemas.CoreSchema);
            TransApiBaseService tacService = new TransApiBaseService();

            var commonService = new CommonService();
            var outputJsonInLog = commonService.GetSysGlobalValue("OutputJsonInLog");
            bool isSaveWmsData = string.IsNullOrWhiteSpace(outputJsonInLog) ? false : outputJsonInLog == "1";

      #region 取得 訂單出貨未回檔資料
      //var f050305s = f050305Repo.GetDatasForExport(dcCode, gupCode, custCode).ToList();
      var f050305s = f050305Repo.GetDatasForExport_Sql(dcCode, gupCode, custCode).ToList();
      #endregion

      f050305s.ForEach(f050305 =>
            {
                SaleOrderReplyReq req = new SaleOrderReplyReq { DcCode = dcCode, CustCode = custCode };

                #region 取得 WarehouseOuts資料
                req.WarehouseOuts = GetWarehouseOuts(f050305);
                #endregion

                // 取得 WarehouseOutDetails資料(STATUS=5才撈取資料)
                // 取得 RcvDatas資料(STATUS= 5 才撈取資料)
                if (f050305.STATUS == "5" && req.WarehouseOuts.Any())
                {
                    req.WarehouseOuts[0].WarehouseOutDetails = GetWarehouseOutDetails(f050305);
                    req.WarehouseOuts[0].Packages = GetPackageDatas(f050305);
                }

                #region 訂單出貨結果回傳
                // 新增API Log[< 參數1 >,< 參數2 >,< 參數3 >, LmsApi_ ExportODResult, 傳入F050305, &LmsApiFunc, false]
                ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ExportODResult", new { LmsApiUrl = LmsSetting.ApiUrl + "OutboundOrder/Reply", LmsToken = LmsSetting.ApiAuthToken, WmsCondition = f050305, LmsData = isSaveWmsData ? req : null }, () =>
             {
                 ApiResult result = new ApiResult();
#if (DEBUG)
                 result = LmsApiFuncTest(req, "OutboundOrder/Reply");
#else
         result = LmsApiFunc(req, "OutboundOrder/Reply");
#endif
                 if (result != null)
                 {
                     if (result.IsSuccessed)
                     {
                         #region 更新處理狀態
                         f050305Repo.UpdateProcFlag(f050305.ID,"1");
                         #endregion
                     }
                     else
                     {
                         data.Add(new ApiResponse
                         {
                             No = $"ORD_NO：{f050305.ORD_NO} ",
                             MsgCode = result.MsgCode,
                             MsgContent = result.MsgContent
                         });

                         res.FailureCnt++;
                     }
                 }

                 return result;
             }, false);
                #endregion
            });

            if (data.Any())
                res.Data = data;

            res.IsSuccessed = res.FailureCnt == 0;
            res.TotalCnt = f050305s.Count;
            res.MsgCode = "10005";
            res.MsgContent = string.Format(tacService.GetMsg("10005"),
                    "訂單出貨結果回傳", res.TotalCnt - res.FailureCnt, res.FailureCnt, res.TotalCnt);

            return res;
        }

        private List<SaleOrderReplyWarehouseOut> GetWarehouseOuts(F050305 f050305)
        {
            var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
            var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);

            var f050101s = f050101Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
                                                                                                                        o.GUP_CODE == f050305.GUP_CODE &&
                                                                                                                        o.CUST_CODE == f050305.CUST_CODE &&
                                                                                                                        o.ORD_NO == f050305.ORD_NO);

            var wmsOrdNos = f05030101Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
                                                                                                                        o.GUP_CODE == f050305.GUP_CODE &&
                                                                                                                        o.CUST_CODE == f050305.CUST_CODE &&
                                                                                                                        o.ORD_NO == f050305.ORD_NO).Select(x => x.WMS_ORD_NO).ToList();

            var f055001s = f055001Repo.GetDatas(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos);

            var res = f050101s.Select(x => new SaleOrderReplyWarehouseOut
            {
                CustOrdNo = x.CUST_ORD_NO,
                OrderType = x.CUST_COST !="MoveOut" ? "SO" : "OTO",
                Status = f050305.STATUS,
                WorkTime = f050305.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss"),
                TotalBoxNum = f055001s.Count()
            }).ToList();

            return res;
        }

        private List<SaleOrderReplyWarehouseOutDetail> GetWarehouseOutDetails(F050305 f050305)
        {
            var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);

            var f050102s = f050102Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
                                                                                                                                 o.GUP_CODE == f050305.GUP_CODE &&
                                                                                                                                 o.CUST_CODE == f050305.CUST_CODE &&
                                                                                                                                 o.ORD_NO == f050305.ORD_NO);

            var f05030202s = f05030202Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
                                                                                                                                 o.GUP_CODE == f050305.GUP_CODE &&
                                                                                                                                 o.CUST_CODE == f050305.CUST_CODE &&
                                                                                                                                 o.ORD_NO == f050305.ORD_NO);


            var res = (from A in f050102s
                       join B in f05030202s
                       on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO, A.ORD_SEQ } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO, B.ORD_SEQ } into subB
                       from B in subB.DefaultIfEmpty()
                       select new
                       {
                           F050102 = A,
                           F05030202 = B
                       }).GroupBy(x => new { x.F050102.ORD_NO, x.F050102.ORD_SEQ, x.F050102.ITEM_CODE }).Select(x => new SaleOrderReplyWarehouseOutDetail
                       {
                           ItemSeq = x.Key.ORD_SEQ,
                           ItemCode = x.Key.ITEM_CODE,
                           ActQty = x.Where(z => z.F05030202 != null).Any() ? Convert.ToInt32(x.Sum(z => z.F05030202.A_DELV_QTY)) : 0
                       }).ToList();

            return res;
        }



        private List<SaleOrderReplyPackage> GetPackageDatas(F050305 f050305)
        {
            var f055004Repo = new F055004Repository(Schemas.CoreSchema);
            var exportService = new ExportService();

            var f055004s = f055004Repo.GetDatasByTrueAndCondition(o =>
            o.DC_CODE == f050305.DC_CODE &&
            o.GUP_CODE == f050305.GUP_CODE &&
            o.CUST_CODE == f050305.CUST_CODE &&
            o.ORD_NO == f050305.ORD_NO).ToList();

            var f055004List = f055004s.Any() ? f055004s : exportService.CreateF055004(f050305);
            return CreatePackageDatas(f050305, f055004List);
        }

        private List<SaleOrderReplyPackage> CreatePackageDatas(F050305 f050305, List<F055004> f055004List)
        {
            var f050101repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
            var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);

            var wmsOrdNos = f05030101Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == f050305.DC_CODE &&
                                                                                                                        o.GUP_CODE == f050305.GUP_CODE &&
                                                                                                                        o.CUST_CODE == f050305.CUST_CODE &&
                                                                                                                        o.ORD_NO == f050305.ORD_NO).Select(x => x.WMS_ORD_NO).ToList();

            var f055001s = f055001Repo.GetDatas(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos);

            var f055002s = f055002Repo.GetPackageBoxSeqsByWmsOrdNos(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos);

            var f050901s = f050901Repo.GetDatasByWmsOrdNos(f050305.DC_CODE, f050305.GUP_CODE, f050305.CUST_CODE, wmsOrdNos);

            var f055004s = f055004List.GroupBy(x => new { x.ITEM_CODE, x.ORD_SEQ, x.MAKE_NO, x.BOX_NO, x.VALID_DATE }).Select(x => new
            {
                x.Key.ITEM_CODE,
                ORD_SEQ = x.Key.ORD_SEQ.ToString(),
                x.Key.MAKE_NO,
                x.Key.BOX_NO,
                QTY = x.Sum(z => z.QTY),
                x.Key.VALID_DATE
            });

            var datas = from A in f055001s
                        join B in f055002s
                        on new { A.WMS_ORD_NO, A.PACKAGE_BOX_NO } equals new { B.WMS_ORD_NO, B.PACKAGE_BOX_NO }
                        join C in f050901s
                        on new { CONSIGN_NO = A.PAST_NO } equals new { C.CONSIGN_NO } into g
                        from C in g.DefaultIfEmpty()
                        select new { F055001 = A, F055002 = B, F050901 = C };


            var res = datas.GroupBy(x => new { x.F055001, x.F050901 })
                                         .Select(package => new SaleOrderReplyPackage // Package層
                                         {
                                             WmsNo = package.Key.F055001.WMS_ORD_NO,
                                             BoxNo = package.Key.F055001.PACKAGE_BOX_NO,
                                             BoxNum = package.Key.F055001.BOX_NUM,
                                             TransportCode = package.Key.F055001.PAST_NO,
                                             TransportProvider = package.Key.F050901 != null ? package.Key.F050901.DELIVID_SEQ_NAME : string.Empty,
                                             ShipmentTime = package.Key.F055001.AUDIT_DATE == null ? null : Convert.ToDateTime(package.Key.F055001.AUDIT_DATE).ToString("yyyy/MM/dd HH:mm:ss"),
                                             Details = package.GroupBy(z => new { z.F055002.ORD_SEQ, z.F055002.ITEM_CODE, z.F055002.PACKAGE_BOX_NO })
                                                                     .OrderBy(z => z.Key.ORD_SEQ).Select(detail => new SaleOrderReplyPackageDetail // Detail層
                                                                     {
                                                                         ItemSeq = detail.Key.ORD_SEQ,
                                                                         ItemCode = detail.Key.ITEM_CODE,
                                                                         OutQty = Convert.ToInt32(detail.Sum(y => y.F055002.PACKAGE_QTY)),
                                                                         MakeNoDetails = f055004s.Where(z => z.ITEM_CODE == detail.Key.ITEM_CODE && z.ORD_SEQ == detail.Key.ORD_SEQ && z.BOX_NO == detail.Key.PACKAGE_BOX_NO.ToString()).Select(makeNoDetail => new SaleOrderReplyPackageMakeNoDetail
                                                                         {
                                                                             ValidDate = makeNoDetail.VALID_DATE.HasValue 
                                                                                 ? makeNoDetail.VALID_DATE.Value.ToString("yyyy/MM/dd") 
                                                                                 : "",
                                                                             MakeNo = makeNoDetail.MAKE_NO,
                                                                             MakeNoQty = makeNoDetail.QTY,
                                                                             SnList = detail.Where(x => !string.IsNullOrWhiteSpace(x.F055002.SERIAL_NO)).Select(x => x.F055002.SERIAL_NO).ToList()
                                                                         }).ToList()
                                                                     }).ToList()
                                         }).ToList();

            return res;
        }
    }
}
