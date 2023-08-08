using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Lms.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
    public class ReplenishService
    {
        private WmsTransaction _wmsTransaction;
        public ReplenishService(WmsTransaction wmsTransaction = null)
        {
            _wmsTransaction = wmsTransaction;
        }

        #region Cache
        /// <summary>
        /// Cache_商品未上架資料清單
        /// </summary>
        private List<AllocDetailByReplenish> _allocDetailCacheList;
        /// <summary>
        /// Cache_商品補貨區庫存清單
        /// </summary>
        private List<ReplensihModel> _replenishStockList;
        #endregion

        #region Get data for cache (Method)
        /// <summary>
        /// 取得補貨調撥明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="makeNo"></param>
        /// <returns></returns>
        private List<AllocDetailByReplenish> GetAllocDetailListByReplenish(string dcCode, string gupCode, string custCode, string itemCode, string makeNo, string serialNo)
        {
            #region 找Cache
            if (_allocDetailCacheList == null)
                _allocDetailCacheList = new List<AllocDetailByReplenish>();

            var datas = _allocDetailCacheList.Where(x => x.ItemCode == itemCode);

            if (!datas.Any())
            {
                #region 找資料庫
                var f151002Repo = new F151002Repository(Schemas.CoreSchema);
                datas = f151002Repo.GetDataByReplenish(dcCode, gupCode, custCode, itemCode);

                if (datas.Any())
                    _allocDetailCacheList.AddRange(datas);
				#endregion
			}

			if (!string.IsNullOrWhiteSpace(serialNo))
				datas = datas.Where(x => x.SerialNo == serialNo);
			else if (!string.IsNullOrWhiteSpace(makeNo))
				datas = datas.Where(x => x.MakeNo == makeNo);

			return datas.Where(x => x.TarQty > 0).ToList();
            #endregion
        }

        /// <summary>
        /// 取得商品補貨區庫存清單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="makeNo"></param>
        /// <returns></returns>
		private List<ReplensihModel> GetReplenishStockData(string dcCode, string gupCode, string custCode, string itemCode, string makeNo, string serialNo)
        {
            var f1913Repo = new F1913Repository(Schemas.CoreSchema);
            if (_replenishStockList == null)
                _replenishStockList = new List<ReplensihModel>();

			if (!string.IsNullOrWhiteSpace(serialNo))
			{
				#region 找Cache
				var datas = _replenishStockList.Where(x => x.ItemCode == itemCode && x.SerialNo == serialNo && x.ReplensihQty > 0).ToList();
				if (datas.Any())
					return datas;
				#endregion

				#region 找資料庫
				var data = f1913Repo.GetReplensihData(dcCode, gupCode, custCode, itemCode, null, serialNo).ToList();
				if (data.Any())
					_replenishStockList.AddRange(data);
				return data;
				#endregion
			}
			else if (!string.IsNullOrWhiteSpace(makeNo))
            {
                #region 找Cache
                var datas = _replenishStockList.Where(x => x.ItemCode == itemCode && x.MakeNo == makeNo && x.ReplensihQty > 0).ToList();
                if (datas.Any())
                    return datas;
                #endregion

                #region 找資料庫
                var data = f1913Repo.GetReplensihData(dcCode, gupCode, custCode, itemCode, makeNo, null).ToList();
                if (data.Any())
                    _replenishStockList.AddRange(data);
                return data;
                #endregion
            }
            else
            {
                #region 找資料庫
                var returnList = new List<ReplensihModel>();

                var currStock = _replenishStockList.Where(x => x.ItemCode == itemCode).ToList();

                returnList.AddRange(currStock);

                var existData = f1913Repo.GetReplensihData(dcCode, gupCode, custCode, itemCode, null, null).ToList();

                if(currStock.Any())
                    existData = existData.Where(x => !currStock.Select(z => z.MakeNo).Contains(x.MakeNo)).ToList();

                if (existData.Any())
                {
                    returnList.AddRange(existData);
                    _replenishStockList.AddRange(existData);
                }
                
                return returnList;
                #endregion
            }

        }
        #endregion

        #region New
        /// <summary>
        /// 每日補貨排程
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public ApiResult DailyReplenish(string dcCode, string gupCode, string custCode)
        {
            var res = new ApiResult();
            var f050801Rep = new F050801Repository(Schemas.CoreSchema);
            var stockService = new StockService();
			      var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			      // 找出補貨區有庫存，且商品揀區庫存小於商品補貨安全庫存數的需補貨的商品
			      var needReplenishItemCodes = f1913Repo.GetNeedReplenishItemCodes(dcCode,gupCode,custCode).ToList();


						// 取得90天每日商品出貨平均數
			      var replensihStockData = f050801Rep.GetReplensihStockData(dcCode, gupCode, custCode, 90).ToList();
			      replensihStockData = replensihStockData.Where(x=> needReplenishItemCodes.Contains(x.ITEM_CODE)).ToList();

						var isPass = false;
            var proRes = new ReplenishProcessRes();
			      string allotBatchNo=string.Empty;
            try
            {
								var itemCodes = replensihStockData.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
								allotBatchNo = "BR" + DateTime.Now.ToString("yyyyMMddHHmmss");

								isPass = stockService.CheckAllotStockStatus(true, allotBatchNo,itemCodes);

                // 如果[A]為false 回傳訊息”系統正在配庫中，請稍後在試”，如果[A]為true往下執行
                if (!isPass)
                    res = new ApiResult { IsSuccessed = false, Data = new List<ApiResponse> { new ApiResponse { MsgContent = "仍有程序正在配庫補貨單所配庫商品，請稍待再配庫" } } };
                else
                {
                    // [B] = 呼叫ReplenishProcess
                    proRes = ReplenishProcess(ReplenishType.Daily, dcCode, gupCode, custCode, replensihStockData.Select(x => new ItemNeedQtyModel
                    {
                        ItemCode = x.ITEM_CODE,
                        NeedQty = x.RESULT_QTY,
						SerialNo = x.SERIAL_NO
                    }).ToList(), "2");
                }
            }
            finally
            {
                if (isPass)
                    stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
            }

            if (proRes.ReturnAllocationList != null && proRes.ReturnAllocationList.Any())
                res = new ApiResult
                {
                    IsSuccessed = true,
                    MsgContent = "執行成功",
                    Data = proRes.ReturnAllocationList.GroupBy(x => x.Message).Select(x => new ApiResponse { MsgContent = $"{x.Key}\n{string.Join(x.Key == "該商品庫存不足" ? "、" : "；", x.Select(z => z.No)) }" })
                };

            return res;
        }

        /// <summary>
        /// 手動補貨
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="ids">F050805.ID清單</param>
        /// <returns></returns>
        public ExecuteResult ManualReplenish(string dcCode, string gupCode, string custCode, List<decimal> ids)
        {
            var res = new ExecuteResult(true);
            var f050801Rep = new F050801Repository(Schemas.CoreSchema);
            var stockService = new StockService();

            // 取得需要手動補貨商品資料By F050805.ID
            var replensihStockData = f050801Rep.GetReplensihStockData(dcCode, gupCode, custCode, ids: ids).ToList();

            var isPass = false;
            var proRes = new ReplenishProcessRes();
						string allotBatchNo=string.Empty;
            try
            {
								var itemCodes = replensihStockData.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				        allotBatchNo = "BR" + DateTime.Now.ToString("yyyyMMddHHmmss");

								isPass = stockService.CheckAllotStockStatus(true, allotBatchNo,itemCodes);

                // 如果[A]為false 回傳訊息”系統正在配庫中，請稍後在試”，如果[A]為true往下執行
                if (!isPass)
                    res = new ExecuteResult { IsSuccessed = false, Message = "仍有程序正在配庫補貨單所配庫商品，請稍待再配庫" };
                else
                {
                    // [B] = 呼叫ReplenishProcess
                    proRes = ReplenishProcess(ReplenishType.Manual, dcCode, gupCode, custCode, replensihStockData.Select(x => new ItemNeedQtyModel
                    {
                        ItemCode = x.ITEM_CODE,
                        NeedQty = x.RESULT_QTY,
                        MakeNo = x.MAKE_NO,
						SerialNo = x.SERIAL_NO
                    }).ToList(), "3");
                }
            }
            finally
            {
                if (isPass)
                    stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
            }

            if (proRes.ReturnAllocationList != null && proRes.ReturnAllocationList.Any())
                res = new ExecuteResult
                {
                    IsSuccessed = true,
                    Message = string.Join("\n", 
                    proRes.ReturnAllocationList.GroupBy(x => x.Message).Select(x => $"{x.Key}\n{string.Join((x.Key == "該商品庫存不足" ? "、" : "；"), x.Select(z => z.No)) }").ToList())
                };

            return res;
        }

        /// <summary>
        /// 補貨處理
        /// </summary>
        /// <param name="replenishType">進入方式</param>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemNeedQtyList">商品需求量清單</param>
        /// <param name="allocationTypeCode">調撥單類型</param>
        public ReplenishProcessRes ReplenishProcess(ReplenishType replenishType, string dcCode, string gupCode, string custCode, List<ItemNeedQtyModel> itemNeedQtyList, string allocationTypeCode)
        {
            var result = new ReplenishProcessRes();
            result.ItemSuggetReplenishList = GetItemSuggetReplenishQty(dcCode, gupCode, custCode, itemNeedQtyList);
            result.ReturnAllocationList = CreateReplenishAllocation(replenishType, dcCode, gupCode, custCode, result.ItemSuggetReplenishList, allocationTypeCode);
            return result;
        }

        /// <summary>
        /// 取得商品補貨建議量
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemNeedQtyList">商品需求量清單</param>
        /// <returns></returns>
        public List<ItemNeedQtyModel> GetItemSuggetReplenishQty(string dcCode, string gupCode, string custCode, List<ItemNeedQtyModel> itemNeedQtyList)
        {
            var commonService = new CommonService();
            var itemService = new ItemService();

			// 商品需求量清單依照品號批號排序，若商品有指定序號優先分配，若有指定批號再分配
			itemNeedQtyList = itemNeedQtyList.OrderBy(x => x.ItemCode).OrderByDescending(x => x.SerialNo).OrderByDescending(x => x.MakeNo).ToList();

			// 一箱總Pcs
			var unitQtyList = itemService.GetSysItemUnitQtyList(gupCode, custCode, itemNeedQtyList.Select(x => x.ItemCode).Distinct().ToList(), Datas.Shared.Enums.SysUnit.Case);

            itemNeedQtyList.ForEach(item =>
            {
                // 取得商品補貨安全庫存量
                var pickSaveQty = commonService.GetProduct(gupCode, custCode, item.ItemCode).PICK_SAVE_QTY;

                // 若商品補貨安全庫存量則替換
                if (pickSaveQty > item.NeedQty)
                    item.NeedQty = pickSaveQty;

                // [B] = 取得未上架完成的補貨調撥單該商品清單(F151002 WHERE IN(SELECT ALLOCATION_NO WHERE F151001.MEMO = ‘每日補貨區調撥’ OR F151001.MEMO = ‘配庫補貨區調撥’ &STATUS != 5 & 9))[欄位: 調撥單號，品號，批號，數量]，請做cache
                var allocDetailData = GetAllocDetailListByReplenish(dcCode, gupCode, custCode, item.ItemCode, item.MakeNo, item.SerialNo);

                // [C] = 從[B]取得商品未上架數量(排除調撥單號加總)(SUM(TAR_QTY) GROUP BY品號，批號)，若有指定批號，增加批號篩選
                var sumTarQty = allocDetailData.Sum(x => x.TarQty);

                // 如果[C] >= A1.NeedQty，從[C]清單扣除A1.NeedQty，A1.SuggestReplenishQty = 0，AllocationNos =[B]的調撥單號，不往下處理跳至下一筆項目。
                if (sumTarQty >= item.NeedQty)
                {
                    var needQty = item.NeedQty;

                    foreach (var currAllocDetail in allocDetailData)
                    {
                        if (needQty > 0)
                        {
                            // AllocationNos =[B]的調撥單號
                            item.AllocationNos.Add(currAllocDetail.AllocNo);

                            if (needQty >= currAllocDetail.TarQty)
                            {
                                needQty -= currAllocDetail.TarQty;
                                currAllocDetail.TarQty = 0;
                            }
                            else
                            {
                                currAllocDetail.TarQty -= needQty;
                                needQty = 0;
                                break;
                            }
                        }
                    }

                    item.SuggestReplenishQty = 0;
                }
                else
                {
                    // A1.NeedQty = A1.NeedQty - SUM([C].QTY)
                    item.NeedQty -= sumTarQty;
                    // 從[C]清單數量都設為0，往下處理
                    allocDetailData.ForEach(currAllocDetail => { currAllocDetail.TarQty = 0; });

                    // [D] = 取得商品補貨區庫存清單(REPLENSIH_QTY)[欄位: 品號、批號、數量]，請做cache
                    var replenishStockData = GetReplenishStockData(dcCode, gupCode, custCode, item.ItemCode, item.MakeNo, item.SerialNo);

                    // [E] = 從[D]取得商品可補貨區數量((REPLENSIH_QTY))，若有指定批號，增加批號篩選
                    var sumReplensihQty = replenishStockData.Sum(x => x.ReplensihQty);

                    // 如果[E] <= A1.NeedQty 則 A1. SuggestReplenishQty =[E]，將[E]的數量都設為0，不往下處理跳至下一筆項目
                    if (sumReplensihQty <= item.NeedQty)
                    {
                        item.SuggestReplenishQty = sumReplensihQty;
                        replenishStockData.ForEach(currAllocDetail => { currAllocDetail.ReplensihQty = 0; });
                    }
                    else
                    {
                        long qty = 0;

                        // 計算需補貨的安全數量
                        if (item.NeedQty > 0)
                        {
                            // [G] = 取得商品一箱總數
                            var unitQty = unitQtyList.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                            long boxTotalPcs = unitQty == null ? 0 : unitQty.Qty;

                            // [F] = [G] > [F] THEN[G] ELSE([F] /[G])*[G]
                            if (boxTotalPcs > 0)
                                // 若最大數大過箱最大PCS就算箱的倍數，若沒有大過箱最大PCS就用一箱PCS數
                                qty = boxTotalPcs > item.NeedQty ? boxTotalPcs : boxTotalPcs * Convert.ToInt32(Math.Ceiling(Convert.ToDouble(item.NeedQty) / boxTotalPcs));
                            else
                                // 如果沒有箱PCS直接帶入最大數
                                qty = item.NeedQty;
                        }

                        // 如果[E]<=[F] A.SuggestReplenishQty =[E] ELSE A. SuggestReplenishQty = [F];
                        var suggestReplenishQty = sumReplensihQty <= qty ? sumReplensihQty : Convert.ToInt32(qty);
                        item.SuggestReplenishQty = suggestReplenishQty;

                        // [E]=[E] - A.SuggestReplenishQty;
                        foreach (var currStock in replenishStockData)
                        {
                            if (suggestReplenishQty > 0)
                            {
                                if (suggestReplenishQty >= currStock.ReplensihQty)
                                {
                                    suggestReplenishQty -= currStock.ReplensihQty;
                                    currStock.ReplensihQty = 0;
                                }
                                else
                                {
                                    currStock.ReplensihQty -= suggestReplenishQty;
                                    suggestReplenishQty = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            });

            return itemNeedQtyList;
        }

        /// <summary>
        /// 建立補貨調撥單
        /// </summary>
        /// <param name="replenishType">進入方式</param>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemSuggetReplenishList">計算後商品需求量清單</param>
        /// <param name="allocationTypeCode">調撥單類型</param>
        /// <returns></returns>
        public List<ExecuteResult> CreateReplenishAllocation(ReplenishType replenishType, string dcCode, string gupCode, string custCode, List<ItemNeedQtyModel> itemSuggetReplenishList, string allocationTypeCode)
        {
            var sharedSrv = new SharedService(_wmsTransaction);
            var stowShelfAreaService = new StowShelfAreaService(_wmsTransaction);
            var res = new List<ExecuteResult>();

            // 補貨調撥單調整為一品一張調撥單，若有指定批號，則為一品一批號一張調撥單。
            foreach (var item in itemSuggetReplenishList)
			{
				var inStr = string.IsNullOrWhiteSpace(item.SerialNo) ? (string.IsNullOrEmpty(item.MakeNo) ? string.Empty : $",批號:{item.MakeNo}") : $",序號:{item.SerialNo}";

				if (item.SuggestReplenishQty > 0)
                {
                    string tarWarehouseId = null;

                    // 呼叫LmsApi上架倉別指示
                    var lmsRes = stowShelfAreaService.StowShelfAreaGuide(dcCode, gupCode, custCode, "2", null, new List<string> { item.ItemCode });

                    if (lmsRes.IsSuccessed)
                    {
                        var lmsResData = JsonConvert.DeserializeObject<List<StowShelfAreaGuideData>>(JsonConvert.SerializeObject(lmsRes.Data));
                        if (lmsResData != null && lmsResData.Any())
                            tarWarehouseId = lmsResData.First().ShelfAreaCode;
                    }
                    else
                    {
                        res.Add(new ExecuteResult { IsSuccessed = true, Message = "無法產生調撥單，因取不到上架指示倉別", No = $"[品號:{item.ItemCode}{inStr}]" });
                        continue;
                    }

                    var allocationParam = new NewAllocationItemParam
                    {
                        IsCheckExecStatus = false,
                        AllocationType = AllocationType.Both,
                        GupCode = gupCode,
                        CustCode = custCode,
                        SrcDcCode = dcCode,
                        IsExpendDate = true,
                        isIncludeResupply = false,
                        ReturnStocks = new List<F1913>(),
                        SrcStockFilterDetails = new List<StockFilter>
                        {
                            new StockFilter
                            {
                                ItemCode = item.ItemCode,
                                Qty = item.SuggestReplenishQty,
                                MakeNos = string.IsNullOrWhiteSpace(item.MakeNo) ? new List<string>() : new List<string>{ item.MakeNo },
								SerialNos = string.IsNullOrWhiteSpace(item.SerialNo) ? new List<string>() : new List<string>{ item.SerialNo }
							}
                        },
                        SrcWarehouseType = "G",
                        TarDcCode = dcCode,
                        TarWarehouseId = tarWarehouseId,
                        ATypeCode = "C", //補貨區,
                        Memo = replenishType == ReplenishType.Daily ? "每日補貨區調撥" : "配庫補貨區調撥",
                        AllocationTypeCode = allocationTypeCode
                    };

                    var result = sharedSrv.CreateOrUpdateAllocation(allocationParam);
                    if (result.Result.IsSuccessed)
                    {
                        var result1 = sharedSrv.BulkInsertAllocation(result.AllocationList, result.StockList);
                        res.Add(new ExecuteResult { IsSuccessed = true, Message = "產生補貨調撥單號", No = $"[品號:{item.ItemCode}{inStr}] 調撥單號:{string.Join(",", result.AllocationList.Select(x => x.Master.ALLOCATION_NO).ToList())}" });
                    }
                    else
                    {
                        res.Add(result.Result);
                    }
                }
                else
                {
                    if (item.AllocationNos.Any())
                        res.Add(new ExecuteResult { IsSuccessed = false, Message = "尚有該商品補貨調撥單未完成上架", No = $"[品號:{item.ItemCode}{inStr}] 調撥單號:{string.Join(",", item.AllocationNos.Distinct().ToList())}" });
                    else
                        res.Add(new ExecuteResult { IsSuccessed = false, Message = "該商品庫存不足", No = $"[品號:{item.ItemCode}{inStr}]" });
                }
            }

            return res;
        }
        #endregion
    }
}
