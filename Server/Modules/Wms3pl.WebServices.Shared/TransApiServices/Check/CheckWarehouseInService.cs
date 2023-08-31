using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;


namespace Wms3pl.WebServices.Shared.TransApiServices.Check
{
    public class CheckWarehouseInService
    {
        private TransApiBaseService tacService = new TransApiBaseService();
        private WmsTransaction _wmsTransaction = new WmsTransaction();

    /// <summary>
    /// 檢查ProcFlag
    /// </summary>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    /// <returns></returns>
    public void CheckProcFlag(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
    {
      List<string> procFlags = new List<string> { "0", "D", "U" };
      if (!procFlags.Contains(warehouseIns.ProcFlag))
        res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20961", MsgContent = string.Format(tacService.GetMsg("20961"), warehouseIns.CustInNo) });
    }

    /// <summary>
    /// 檢查刪除狀態貨主單號是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="thirdPartOrdersList"></param>
    /// <param name="warehouseIns"></param>
    /// <returns></returns>
    public bool CheckCustExistForThirdPart(List<ApiResponse> res, List<ThirdPartOrders> thirdPartOrdersList, PostCreateWarehouseInsModel warehouseIns, string custCode)
    {
      var isAddF075101 = false;
      var f075101Repo = new F075101Repository(Schemas.CoreSchema);
      var currCustData = thirdPartOrdersList.Where(x => x.CUST_ORD_NO == warehouseIns.CustInNo);
      if (warehouseIns.ProcFlag == "D" || warehouseIns.ProcFlag == "U")
      {
        if (!currCustData.Any())
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), warehouseIns.CustInNo) });
        else
          f075101Repo.DelF075101ByKey(custCode, warehouseIns.CustInNo);
      }
      else if (warehouseIns.ProcFlag == "0")
      {
        if (currCustData.Any())
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), warehouseIns.CustInNo) });
        else
        {
          #region 新增進倉單匯入控管紀錄表
          var f075101Res = f075101Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
          () =>
          {
            var lockF075101 = f075101Repo.LockF075101();
            //var f075101 = f075101Repo.Find(o => o.CUST_CODE == custCode && o.CUST_ORD_NO == warehouseIns.CustInNo, isForUpdate: true, isByCache: false);
            var f075101s = f075101Repo.GetF075101Data(custCode, warehouseIns.CustInNo);
            var f075101 = (F075101)f075101s;       
            if (f075101 == null)
            {
              f075101 = new F075101 { CUST_CODE = custCode, CUST_ORD_NO = warehouseIns.CustInNo };
              f075101Repo.Add(f075101);
              isAddF075101 = true;
            }
            else
            {
              f075101 = null; // 代表F075101已存在資料
            }
            return f075101;
          });
          if (f075101Res == null)// 代表F075101已存在資料
            res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), warehouseIns.CustInNo) });
          #endregion
        }
      }

      return isAddF075101;
    }

    /// <summary>
    /// 檢查廠商編號
    /// </summary>
    /// <param name="res"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="warehouseIns"></param>
    /// <returns></returns>
    public void CheckVnrExist(List<ApiResponse> res, List<F1908> vnrList, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
        {
            if (warehouseIns.ProcFlag == "0")
            {
                // 是否存在於[廠商資料清單]，若不存在否則回傳 & 訊息內容[20851,< 參數4 >.CustInNo,< 參數4 >.VnrCode]
                var vnrData = vnrList.Where(x => x.VNR_CODE == warehouseIns.VnrCode &&
                                                                                 x.GUP_CODE == gupCode &&
                                                                                 x.CUST_CODE == custCode);
                if (!vnrData.Any())
                {
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20851", MsgContent = string.Format(tacService.GetMsg("20851"), warehouseIns.CustInNo, warehouseIns.VnrCode) });
                }
            }
        }

        /// <summary>
        /// 檢查交易類型
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        public void CheckTranExist(List<ApiResponse> res, List<F000903> tranCodeList, PostCreateWarehouseInsModel warehouseIns)
        {
            if (warehouseIns.ProcFlag == "0")
            {
                // 是否存在於[交易類型資料清單],若不存在否則回傳 & 訊息內容[20852,< 參數4 >.CustInNo,< 參數4 >.TranCode]
                var tranData = tranCodeList.Where(x => x.ORD_PROP == warehouseIns.InProp);
                if (!tranData.Any())
                {
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20852", MsgContent = string.Format(tacService.GetMsg("20852"), warehouseIns.CustInNo, warehouseIns.InProp) });
                }
            }
        }

        /// <summary>
        /// 檢查貨主單號是否存在
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="warehouseIns"></param>
        public void CheckCustExist(ref List<F010201> f010201List, ref List<F010202> f010202List, string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
        {
            // 檢查貨主單號是否存在
            // [找到的WMS單]=檢查<參數4>.CustInNo是否存在於[貨主單號已產生WMS訂單] 中找CUST_ORD_NO =< 參數4 >.CustInNo
            var delF010201 = f010201List.Where(x => x.DC_CODE == dcCode &&
                                                                                            x.GUP_CODE == gupCode &&
                                                                                            x.CUST_CODE == custCode &&
                                                                                            x.CUST_ORD_NO == warehouseIns.CustInNo).SingleOrDefault();

            if (delF010201 != null)
            {
                // 移除之前Cache已新增的進倉單、進倉單明細
                f010201List.Remove(delF010201);

                var delF010202List = f010202List.Where(x => x.DC_CODE == dcCode &&
                                                                                                        x.GUP_CODE == gupCode &&
                                                                                                        x.CUST_CODE == custCode &&
                                                                                                        x.STOCK_NO == delF010201.STOCK_NO).ToList();

                f010202List = f010202List.Except(delF010202List).ToList();
            }
        }

        /// <summary>
        /// 檢查貨主自訂義分類
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        public void CheckCustCost(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
        {
            List<string> custCost = new List<string> { "In", "MoveIn" };
            if (warehouseIns.ProcFlag == "0" && (!string.IsNullOrWhiteSpace(warehouseIns.CustCost) && !custCost.Contains(warehouseIns.CustCost)))
                res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20091", MsgContent = string.Format(tacService.GetMsg("20091"), warehouseIns.CustInNo) });
        }

    /// <summary>
    /// 檢查快速通關分類
    /// </summary>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    public void CheckFastPassType(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
    {
      List<string> fastPassType = new List<string> { "1", "2", "3" };

      if ((warehouseIns.ProcFlag == "0" || warehouseIns.ProcFlag == "U") && !fastPassType.Contains(warehouseIns.FastPassType))
        res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20101", MsgContent = string.Format(tacService.GetMsg("20101"), warehouseIns.CustInNo) });
    }

    /// <summary>
    /// 檢查預定進倉時段
    /// </summary>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    public void CheckBoookingInPeriod(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
        {
            List<string> boookingInPeriod = new List<string> { "0", "1" };

            if (warehouseIns.ProcFlag == "0" && !boookingInPeriod.Contains(warehouseIns.BoookingInPeriod))
                res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20102", MsgContent = string.Format(tacService.GetMsg("20102"), warehouseIns.CustInNo) });
        }

        /// <summary>
        /// 檢查明細筆數
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        public void CheckDetailCnt(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
        {
            // 如果<參數4>.Status<> D且檢查明細是否為null或明細筆數是否為0，若是則回傳&訊息內容[20071, <參數4>.CustInNo]
            if (warehouseIns.ProcFlag == "0" && (warehouseIns.WarehouseInDetails == null || (warehouseIns.WarehouseInDetails != null && !warehouseIns.WarehouseInDetails.Any())))
                res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20071", MsgContent = string.Format(tacService.GetMsg("20071"), warehouseIns.CustInNo) });
        }

        /// <summary>
        /// 檢核項次必須大於0，且同一張單據內的序號不可重複
        /// </summary>
        /// <param name="res"></param>
        /// <param name="returns"></param>
        public void CheckDetailSeq(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
        {
            if (warehouseIns.ProcFlag == "0" && warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any())
            {
                if (warehouseIns.WarehouseInDetails.Where(x => string.IsNullOrWhiteSpace(x.ItemSeq)).Any() ||
                        warehouseIns.WarehouseInDetails.Count > warehouseIns.WarehouseInDetails.Select(x => x.ItemSeq).Distinct().Count())
                {
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20070", MsgContent = tacService.GetMsg("20070") });
                }
            }
        }

        /// <summary>
        /// 檢查明細進倉數量
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        public void CheckDetailQty(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
        {
            if (warehouseIns.ProcFlag == "0" && warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any())
            {
                var itemSeqList = warehouseIns.WarehouseInDetails.Where(x => x.InQty <= 0).Select(x => x.ItemSeq).Distinct().ToList();
                if (itemSeqList.Any())
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20105", MsgContent = string.Format(tacService.GetMsg("20105"), warehouseIns.CustInNo, string.Join("、", itemSeqList)) });
            }
        }

		/// <summary>
		/// 鑒察採購單號
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		public void CheckDetailPoNoRepeat(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns)
		{
            if (warehouseIns.ProcFlag == "0")
            {
                var f010201Repo = new F010201Repository(Schemas.CoreSchema);
                var f010201s = f010201Repo.GetDatasByPoNo(warehouseIns.PoNo);
                if (f010201s.Any())
                {
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "23064", MsgContent = string.Format(tacService.GetMsg("23064"), warehouseIns.PoNo) });
                }
            }
		}

		/// <summary>
		/// 檢查容器進倉數量
		/// </summary>
		/// <param name="res"></param>
		/// <param name="warehouseIns"></param>
		public void CheckContainerDatas(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns, PostCreateWarehouseInDetailModel detail)
		{
			if (detail.ContainerDatas == null || (detail.ContainerDatas != null && !detail.ContainerDatas.Any()))
				res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20104", MsgContent = string.Format(tacService.GetMsg("20104"), warehouseIns.CustInNo) });
		}

        /// <summary>
        /// 檢查容器進倉數量
        /// </summary>
        /// <param name="res"></param>
        /// <param name="warehouseIns"></param>
        public void CheckContainerQty(List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns, PostCreateWarehouseInDetailModel detail)
        {
            if (detail.ContainerDatas != null && detail.ContainerDatas.Any())
            {
                var containerCodeList = detail.ContainerDatas.Where(x => x.InQty <= 0).Select(x => x.ContainerCode).Distinct().ToList();
                if (containerCodeList.Any())
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20103", MsgContent = string.Format(tacService.GetMsg("20103"), warehouseIns.CustInNo, detail.ItemSeq, string.Join("、", containerCodeList)) });

                if (detail.InQty != detail.ContainerDatas.Sum(x => x.InQty))
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20106", MsgContent = string.Format(tacService.GetMsg("20106"), warehouseIns.CustInNo, detail.ItemSeq) });
            }
        }

    /// <summary>
    /// 檢查容器序號清單
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="res"></param>
    /// <param name="warehouseIns"></param>
    /// <param name="detail"></param>
    /// <param name="thirdPartContainerSnList"></param>
    /// <param name="IsCheckSerialQty">是否檢查容器與序號數量相符</param>
    public void CheckContainerSnList(string dcCode, string gupCode, string custCode, List<ApiResponse> res, PostCreateWarehouseInsModel warehouseIns, PostCreateWarehouseInDetailModel detail, List<string> thirdPartContainerSnList, Boolean IsCheckSerialQty)
    {
      var serialNoService = new SerialNoService();
      // 檢核是否序號清單為空
      var emptyData = detail.ContainerDatas.Where(x => x.SnList == null || (x.SnList != null && !x.SnList.Any()));
      if (emptyData.Any())
        res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20107", MsgContent = string.Format(tacService.GetMsg("20107"), warehouseIns.CustInNo, detail.ItemSeq, string.Join("、", emptyData.Select(x => x.ContainerCode).Distinct().ToList())) });
      else
      {
        // 該明細的容器序號清單
        var currDetailContainerSnList = detail.ContainerDatas.Where(x => x.SnList != null).SelectMany(x => x.SnList);

        // 檢核數量是否符合序號數量
        var notEqualContainerCodes = detail.ContainerDatas.Where(x => x.InQty != x.SnList.Count).Select(x => x.ContainerCode).Distinct().ToList();
        if (notEqualContainerCodes.Any() && IsCheckSerialQty)
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20108", MsgContent = string.Format(tacService.GetMsg("20108"), warehouseIns.CustInNo, detail.ItemSeq, string.Join("、", notEqualContainerCodes)) });
        else
        {
          // 檢核明細SnList是否跟Container.SnList是否一致
          if (detail.SnList != null && detail.SnList.Any() && IsCheckSerialQty)
          {
            var mergeSnList = new List<string>();
            mergeSnList.AddRange(detail.SnList.Distinct());
            mergeSnList.AddRange(currDetailContainerSnList.Distinct());

            // 有差異的序號
            var diffSnList = mergeSnList.GroupBy(x => x).Where(x => x.Count() < 2).Select(x => x.Key).ToList();

            if (diffSnList.Any())
              res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20125", MsgContent = string.Format(tacService.GetMsg("20125"), warehouseIns.CustInNo, detail.ItemSeq, string.Join("、", diffSnList)) });
          }
        }

        // 檢核是否有送重複的序號
        var snRepeatContainerCodes = currDetailContainerSnList.GroupBy(sn => sn).Select(z => new { Sn = z.Key, Cnt = z.Count() }).Where(z => z.Cnt > 1).Select(x => x.Sn).ToList();
        if (snRepeatContainerCodes.Any())
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20109", MsgContent = string.Format(tacService.GetMsg("20109"), warehouseIns.CustInNo, detail.ItemSeq, string.Join("、", snRepeatContainerCodes)) });

        // 檢核序號清單的序號是否已存在進倉序號明細檔
        var existSns = detail.ContainerDatas.Select(x => new
        {
          x.ContainerCode,
          IsExistSns = x.SnList.Intersect(thirdPartContainerSnList).ToList()
        }).Where(x => x.IsExistSns.Any()).GroupBy(x => x.ContainerCode).Select(x => new
        {
          ContainerCode = x.Key,
          SnList = x.SelectMany(z => z.IsExistSns).Distinct().ToList()
        }).ToList();
        existSns.ForEach(existSn =>
        {
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20096", MsgContent = string.Format(tacService.GetMsg("20096"), warehouseIns.CustInNo, $"{detail.ItemCode}容器{existSn.ContainerCode}", string.Join("、", existSn.SnList)) });
        });

        // 檢核大量序號、盒號、儲值卡盒號、箱號檢核
        //var checkLargeSnRes = serialNoService.CheckLargeSerialNoFull(dcCode, gupCode, custCode, detail.ItemCode, detail.ContainerDatas.SelectMany(x => x.SnList).Distinct().ToArray(), "A1");
        //CheckItemLargeSerialWithBeforeInWarehouse
        var checkLargeSnRes = serialNoService.CheckItemLargeSerialWithBeforeInWarehouse( gupCode, custCode, detail.ItemCode, detail.ContainerDatas.SelectMany(x => x.SnList).Distinct().ToList());
        var errorSns = checkLargeSnRes.Where(x => !x.Checked);
        if (errorSns.Any())
        {
          var errorSnList = errorSns.GroupBy(x => x.Message).Select(x => new { Msg = x.Key, SnList = x.Select(z => z.SerialNo).ToList() }).ToList();
          errorSnList.ForEach(errorSn =>
          {
            res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20110", MsgContent = string.Format(tacService.GetMsg("20110"), warehouseIns.CustInNo, detail.ItemSeq, errorSn.Msg, string.Join("、", errorSn.SnList)) });
          });
        }
      }
    }

    /// <summary>
    /// 檢查資料是否完整
    /// </summary>
    /// <param name="f010201"></param>
    /// <param name="f010202List"></param>
    /// <returns></returns>
    public void CheckWarehouseInsData(List<ApiResponse> res, string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns, List<CommonProduct> f1903s)
    {
      if (warehouseIns.ProcFlag == "0")
      {
        CommonService commonService = new CommonService();
        TransApiBaseService tacService = new TransApiBaseService();

        var itemCodeList = warehouseIns.WarehouseInDetails.Select(x => x.ItemCode).Distinct().ToList();

        // 2.[商品資料] = 取得商品資料[< 參數1 >.GUP_CODE,< 參數1 >.CUST_CODE, DISTINCT<參數2>.ITEM_CODE]
        var productList = commonService.GetProductList(gupCode, custCode, itemCodeList);

        // 3.[差異品號清單] = DISTINCT<參數2>.ITEM_CODE 比較[商品資料].ITEM_CODE 是否有差異(訂單品號無商品主檔資料)
        var differentData = itemCodeList.Except(productList.Select(x => x.ITEM_CODE));

        // IF[差異品號清單].Any() then
        if (differentData.Any())
        {
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20086", MsgContent = string.Format(tacService.GetMsg("20086"), warehouseIns.CustInNo, string.Join("、", differentData)) });
        }

        // 3.[停售品號清單] = 檢查[商品資料]的停售日期(STOP_DATE) <= 系統日(DateTime.Today)
        var cessationOfSaleList = productList.Where(x => x.STOP_DATE <= DateTime.Today).Select(x => x.ITEM_CODE);

        // IF[停售品號清單].Any() then
        if (cessationOfSaleList.Any())
        {
          res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20087", MsgContent = string.Format(tacService.GetMsg("20087"), warehouseIns.CustInNo, string.Join("、", cessationOfSaleList)) });
        }

        //序號檢查
        //var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
        var f020302Repo = new F020302Repository(Schemas.CoreSchema);

        SerialNoService serialNoService = new SerialNoService();

        foreach (var warehouseInDetail in warehouseIns.WarehouseInDetails)
        {

          var f1903 = f1903s.Where(x => x.ITEM_CODE == warehouseInDetail.ItemCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode).FirstOrDefault();

          if (f1903 != null)
          {
            //如果是跨庫入+有序號+但該品項非序號商品，就不檢查數量與序號是否相符
            var IsCheckSerialQty = !(warehouseIns.CustCost == "MoveIn" && warehouseIns.WarehouseInDetails.Any(a => a.ContainerDatas.Any(b => b.SnList?.Any() ?? false)) && f1903.BUNDLE_SERIALNO != "1");

            // 為序號商品才做序號檢查
            if (f1903.BUNDLE_SERIALNO == "1" || (warehouseIns.CustCost == "MoveIn" && warehouseIns.WarehouseInDetails.Any(a => a.ContainerDatas.Any(b => b.SnList?.Any() ?? false))))
            {
              if (f1903.BUNDLE_SERIALNO == "1" && warehouseIns.CustCost == "MoveIn" && (warehouseInDetail.SnList == null || (warehouseInDetail.SnList != null && !warehouseInDetail.SnList.Any())))
                res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20111", MsgContent = string.Format(tacService.GetMsg("20111"), warehouseIns.CustInNo, warehouseInDetail.ItemCode) });

              // 序號商品SnList不能為空
              if (warehouseInDetail.SnList != null && warehouseInDetail.SnList.Any())
              {
                if (warehouseInDetail.SnList.Count() != warehouseInDetail.InQty && IsCheckSerialQty)
                  res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20112", MsgContent = string.Format(tacService.GetMsg("20112"), warehouseIns.CustInNo, warehouseInDetail.ItemSeq) });

                //檢查張單同商品序號是否重複
                var repeatSns = warehouseInDetail.SnList.GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase).Select(x => new { Sn = x.Key, Cnt = x.Count() }).Where(x => x.Cnt > 1).Select(x => x.Sn).ToList();
                if (repeatSns.Any())
                  res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20096", MsgContent = string.Format(tacService.GetMsg("20096"), warehouseIns.CustInNo, warehouseInDetail.ItemCode, string.Join("、", repeatSns)) });

                //大量序號、盒號、儲值卡盒號、箱號檢核
                //var checkLargeSnRes = serialNoService.CheckLargeSerialNoFull(dcCode, gupCode, custCode, warehouseInDetail.ItemCode, warehouseInDetail.SnList.Distinct().ToArray(), "A1");
                var checkLargeSnRes = serialNoService.CheckItemLargeSerialWithBeforeInWarehouse( gupCode, custCode, warehouseInDetail.ItemCode, warehouseInDetail.SnList.Distinct().ToList());
                var errorSns = checkLargeSnRes.Where(x => !x.Checked);
                if (errorSns.Any())
                {
                  var errorSnList = errorSns.GroupBy(x => x.Message).Select(x => new { Msg = x.Key, SnList = x.Select(z => z.SerialNo).ToList() }).ToList();
                  errorSnList.ForEach(errorSn =>
                  {
                    res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20094", MsgContent = string.Format(tacService.GetMsg("20094"), warehouseIns.CustInNo, warehouseInDetail.ItemCode, string.Join("、", errorSn.SnList), errorSn.Msg) });
                  });
                }

                // 檢核在進倉序號明細檔是否有有相同序號
                var serailRepeatQuery = f020302Repo.InWithTrueAndCondition("SERIAL_NO", warehouseInDetail.SnList.ToList(),
                        x => x.DC_CODE == dcCode
                 && x.GUP_CODE == gupCode
                 && x.CUST_CODE == custCode
                 && x.ITEM_CODE == warehouseInDetail.ItemCode
                 && x.STATUS == "0");
                if (serailRepeatQuery.Any())
                {
                  var snList = serailRepeatQuery.Select(x => x.SERIAL_NO).Distinct().ToList();
                  res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20096", MsgContent = string.Format(tacService.GetMsg("20096"), warehouseIns.CustInNo, warehouseInDetail.ItemCode, string.Join("、", snList)) });
                }
              }
            }
            else if (f1903.BUNDLE_SERIALNO == "0" && warehouseInDetail.SnList != null && warehouseInDetail.SnList.Any())
              // 非序號商品檢查SnList必須為空
              res.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20095", MsgContent = string.Format(tacService.GetMsg("20095"), warehouseIns.CustInNo, warehouseInDetail.ItemCode) });
          }
        }
      }
    }

  }
}