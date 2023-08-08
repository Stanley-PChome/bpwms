using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810115Service
    {
        private WmsTransaction _wmsTransation;
        private P81Service _p81Service;
        public P810115Service(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
            _p81Service = new P81Service();
        }

    /// <summary>
    /// 容器查詢
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult GetContainerInfo(GetContainerInfoReq req, string gupCode)
    {
      var commonService = new CommonService();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);

      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

      // 檢核人員功能權限
      var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 傳入的參數驗證
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
              string.IsNullOrWhiteSpace(req.AccNo) ||
              string.IsNullOrWhiteSpace(req.DcNo) ||
              string.IsNullOrWhiteSpace(req.CustNo) ||
              !accData.Any() ||
              accFunctionCount == 0 ||
              accCustCount == 0 ||
              accDcCount == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

      // 檢核傳入容器條碼不得為空
      if (string.IsNullOrWhiteSpace(req.ContainerCode))
        return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };

      // 容器條碼轉大寫
      req.ContainerCode = req.ContainerCode.ToUpper();

      //1.[A1] = 取得容器資料頭檔 資料表: F0701 條件: CONTAINER_CODE = 傳入參數.ContainerCode
      var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == req.ContainerCode).OrderByDescending(x => x.ID).FirstOrDefault();

      //2.如果[A1] = null，回傳訊息[false, 容器條碼不存在]
      if (f0701 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "21002", MsgContent = _p81Service.GetMsg("21002") };
      #endregion

      #region 資料處理
      if (f0701.CONTAINER_TYPE == "0")// 單一型容器
      {
        var f070101Repo = new F070101Repository(Schemas.CoreSchema);
        var f070102Repo = new F070102Repository(Schemas.CoreSchema);
        F020501 f020501 = null;

        #region 單一型容器檢核
        //1.  [A2] = 取得容器單據頭檔 資料表: F070101 條件: F0701.ID = [A1].ID
        var f070101 = f070101Repo.GetDataByF0701Id(f0701.ID);

        //2.如果[A2] = null，回傳訊息[false, 此容器條碼等待單據綁定中，請稍後再查詢]
        //若F070101有資料，只是WMS_NO=NULL(剛綁完容器關箱)，還是接著取容器資料
        if (f070101 == null || string.IsNullOrWhiteSpace(f070101.WMS_NO))
        {
          var f020501Repo = new F020501Repository(Schemas.CoreSchema);
          f020501 = f020501Repo.GetDatasByF0701idAndExcepStatus(f0701.ID, new string[] { "0", "9" });
          if (f020501 == null)
          {
            return new ApiResult { IsSuccessed = false, MsgCode = "21403", MsgContent = _p81Service.GetMsg("21403") };
          }
        }
        #endregion

        #region 單一容器資料處理
        var apiResult = GetContainerSingle(f070101.DC_CODE, f070101.GUP_CODE, f070101.CUST_CODE, f070101.WMS_NO, f0701.CONTAINER_CODE, f020501);
        if (!apiResult.IsSuccessed)
          return apiResult;

        var containerInfo = JsonConvert.DeserializeObject<GetContainerInfoRes>(
                            JsonConvert.SerializeObject(apiResult.Data));
        containerInfo.NotifyFontColor = GetNotifyFontColor(containerInfo.Notify1);
        #endregion

        #region 取得容器明細資料
        var f070102s = f070102Repo.GetDatasByTrueAndCondition(o => o.F070101_ID == f070101.ID);
        var f1903s = commonService.GetProductList(gupCode, req.CustNo, f070102s.Select(x => x.ITEM_CODE).Distinct().ToList());
        containerInfo.Detail = (from A in f070102s
                                join B in f1903s
                                on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                                select new { A.BIN_CODE, A.ITEM_CODE, B.CUST_ITEM_CODE, B.ITEM_NAME, A.QTY })
                                  .GroupBy(x => x.BIN_CODE)
                                  .OrderBy(x => x.Key)
                                  .Select(x => new GetContainerInfoDetailModel
                                  {
                                    BinCode = string.IsNullOrWhiteSpace(x.Key) ? "無" : x.Key,
                                    ItemDetail = x.GroupBy(z => new { z.ITEM_CODE, z.ITEM_NAME, z.CUST_ITEM_CODE }).Select(z => new GetContainerInfoItemDetailModel
                                    {
                                      ItemCode = z.Key.ITEM_CODE,
                                      ItemName = z.Key.ITEM_NAME,
                                      CustItemCode = z.Key.CUST_ITEM_CODE,
                                      Qty = z.Sum(q => q.QTY)
                                    }).ToList()
                                  }).ToList();
        #endregion

        return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = containerInfo };
      }
      else if (f0701.CONTAINER_TYPE == "2")// 混和型容器
        return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = _p81Service.GetMsg("10001"), Data = GetMixContainer(f0701) };
      else
        return new ApiResult { IsSuccessed = false, MsgCode = "21402", MsgContent = _p81Service.GetMsg("21402") };
      #endregion
    }

    /// <summary>
    /// 重點提示1顯示顏色
    /// </summary>
    /// <param name="notify1"></param>
    /// <returns></returns>
    private string GetNotifyFontColor(string notify1)
        {
            switch (notify1)
            {
                case "異常區":
                    return "#FFFF0000";// 紅色
                default:
                    return "#FF000000";// 黑色
            }
        }

        /// <summary>
        /// 取得單一型容器資料
        /// </summary>
        /// <param name="apiResult"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsNo"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        private ApiResult GetContainerSingle(string dcCode, string gupCode, string custCode, string wmsNo, string containerCode, F020501 f020501=null)
        {
            var result = new GetContainerInfoRes();
            string status = null;
            string notify1 = null;
            string notify2 = null;

            if (string.IsNullOrWhiteSpace(wmsNo))
            {
              switch (f020501.STATUS)
              {
                case "1":
                  status = "已關箱待複驗";
                  break;
                case "2":
                  status = "可上架";
                  break;
                case "3":
                  status = "不可上架";
                  break;
                case "4":
                  status = "上架移動中";
                  break;
                case "5":
                  status = "移動完成";
                  break;
                case "6":
                  status = "上架完成";
                  break;
              }

            result = new GetContainerInfoRes
            {
              ContainerCode = containerCode,
              WmsNo = wmsNo,
              Status = status,
              Notify1 = "驗收區",
              Notify2 = ""
            };

             return new ApiResult { IsSuccessed = true, Data = result };
            }

            switch (wmsNo.Substring(0, 1))
            {
                case "O"://出貨單
                    var f050801Repo = new F050801Repository(Schemas.CoreSchema);
                    var f051301Repo = new F051301Repository(Schemas.CoreSchema);

                    // [SS] =  SELECT STATUS FROM F050801 WHERE DC_CODE = [< 參數1 > AND GUP_CODE = < 參數2 >  AND CUST_CODE = < 參數3 > AND WMS_ORD_NO = < 參數4 >
                    var f050801 = f050801Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsNo);

                    // QQ
                    var f051301 = f051301Repo.GetContainerSingleByOrd(dcCode, gupCode, custCode, wmsNo);

                    if (f051301 != null)
                    {
                        status = f051301.STATUS == "1" ? (f050801.STATUS == 9 ? "訂單取消" : "待包裝") : f051301.STATUS_NAME;
                        notify1 = f051301.STATUS == "1" && f050801.STATUS == 9 ? "異常區" : f051301.NEXT_STEP_NAME;
                        if (f051301.NEXT_STEP == "2")
                            notify2 = f051301.COLLECTION_CODE;
                    }
                    else
                    {
                        status = f050801.STATUS == 9 ? "訂單取消" : "待包裝";
                        notify1 = f050801.STATUS == 9 ? "異常區" : "包裝站";
                    }

                    result = new GetContainerInfoRes
                    {
                        ContainerCode = containerCode,
                        WmsNo = wmsNo,
                        Status = status,
                        Notify1 = notify1,
                        Notify2 = notify2
                    };
                    break;
                case "P"://揀貨單
                    var f051201Repo = new F051201Repository(Schemas.CoreSchema);

                    var f051201 = f051201Repo.GetContainerSingleByPick(dcCode, gupCode, custCode, wmsNo);

                    // 快速補揀單
                    if (f051201.PICK_TYPE == "5" && !string.IsNullOrWhiteSpace(f051201.MERGE_NO))
                    {
                        if (f051201.MERGE_NO.StartsWith("O"))// 呼叫#單一容器資料處理(出貨單)
                            return GetContainerSingle(dcCode, gupCode, custCode, f051201.MERGE_NO, containerCode);
                        else if (f051201.MERGE_NO.StartsWith("P"))// 呼叫#特殊結構揀貨單資料處理
                            return new ApiResult { IsSuccessed = true, Data = GetSpecialPick(dcCode, gupCode, custCode, f051201.MERGE_NO, containerCode) };
                    }

                    // 人工倉單一揀貨單
                    if (f051201.PICK_TYPE == "0" && !string.IsNullOrWhiteSpace(f051201.MERGE_NO))
                        return GetContainerSingle(dcCode, gupCode, custCode, f051201.MERGE_NO, containerCode);// 呼叫#單一容器資料處理(出貨單)

                    // 人工倉批量揀貨
                    if (f051201.PICK_TYPE == "1" || f051201.PICK_TYPE == "3")
                    {
                        status = "待分貨";
                        notify1 = f051201.NEXT_STEP_NAME;
                    }

                    // 特殊結構訂單
                    if (f051201.PICK_TYPE == "4" || f051201.PICK_TYPE == "7" || f051201.PICK_TYPE == "8")
                        return new ApiResult { IsSuccessed = true, Data = GetSpecialPick(dcCode, gupCode, custCode, f051201.MERGE_NO, containerCode) };

                    // 如果F051201.NEXT_STPE = 6(調撥場)
                    if (f051201.NEXT_STEP == "6")
                    {
                        status = "需稽核出庫";
                        notify1 = f051201.NEXT_STEP_NAME;
                    }

                    result = new GetContainerInfoRes
                    {
                        ContainerCode = containerCode,
                        WmsNo = wmsNo,
                        Status = status,
                        Notify1 = notify1,
                        Notify2 = notify2
                    };
                    break;
                case "T"://調撥單
                    var f151001Repo = new F151001Repository(Schemas.CoreSchema);

                    var f151001 = f151001Repo.GetContainerSingleByAlloc(dcCode, gupCode, custCode, wmsNo);

                    result = new GetContainerInfoRes
                    {
                        ContainerCode = containerCode,
                        WmsNo = wmsNo,
                        Status = f151001.STATUS_NAME,
                        Notify1 = "目的倉",
                        Notify2 = f151001.TAR_WAREHOUSE_ID
                    };
                    break;
            }
            return new ApiResult { IsSuccessed = true, Data = result };
        }

        /// <summary>
        /// 特殊結構揀貨單資料處理
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="mergeNo"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        private GetContainerInfoRes GetSpecialPick(string dcCode, string gupCode, string custCode, string mergeNo, string containerCode)
        {
            var result = new GetContainerInfoRes { WmsNo = mergeNo };
            var f051301Repo = new F051301Repository(Schemas.CoreSchema);
            var f051301 = f051301Repo.GetContainerSingleByOrd(dcCode, gupCode, custCode, mergeNo);

            result.ContainerCode = containerCode;
            result.WmsNo = mergeNo;

            if (f051301 == null)
            {
                result.Status = "待包裝";
                result.Notify1 = "包裝站";
            }
            else
            {
                result.Status = f051301.STATUS == "1" ? "待包裝" : f051301.STATUS_NAME;
                result.Notify1 = f051301.NEXT_STEP_NAME;
                if (f051301.NEXT_STEP == "2")
                    result.Notify2 = f051301.COLLECTION_CODE;
            }

            return result;
        }

        /// <summary>
        /// 混和型容器資料處理
        /// </summary>
        /// <param name="f0701"></param>
        /// <returns></returns>
        private GetContainerInfoRes GetMixContainer(F0701 f0701)
        {
            var f070104Repo = new F070104Repository(Schemas.CoreSchema);
            var f070104s = f070104Repo.GetCollectionMixDatas(f0701.ID);

            var firstF070104 = f070104s.FirstOrDefault();

            var containerInfo = new GetContainerInfoRes
            {
                ContainerCode = f0701.CONTAINER_CODE,
                WmsNo = f0701.CONTAINER_CODE
            };

            if (firstF070104.WMS_NO.StartsWith("P"))
            {
                containerInfo.Status = "需稽核出庫";
                containerInfo.Notify1 = "調撥場";
            }
            else if (firstF070104.WMS_NO.StartsWith("A"))
            {
                containerInfo.Status = firstF070104.STATUS == "0" ? "跨庫進貨待驗收" : "跨庫進貨待上架";
                containerInfo.Notify1 = "驗收區";
            }

            containerInfo.Detail = new List<GetContainerInfoDetailModel>
                {
                    new GetContainerInfoDetailModel
                    {
                        BinCode = "無",
                        ItemDetail = f070104s.Select(x => new GetContainerInfoItemDetailModel
                        {
                            ItemCode = x.ITEM_CODE,
                            CustItemCode = x.CUST_ITEM_CODE,
                            ItemName = x.ITEM_NAME,
                            Qty = x.QTY
                        }).ToList()
                    }
                };

            containerInfo.NotifyFontColor = GetNotifyFontColor(containerInfo.Notify1);

            return containerInfo;
        }
    }
}
