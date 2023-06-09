using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
    /// <summary>
    /// 跨庫出貨配庫補貨排程
    /// </summary>
    public class AllocateReplenishmentService
    {
        private WmsTransaction _wmsTransation;

        public AllocateReplenishmentService(WmsTransaction wmsTransation = null)
        {
            _wmsTransation = wmsTransation;
        }

        /// <summary>
        /// 進入點
        /// </summary>
        /// <returns></returns>
        public ApiResult AllocateReplenishment(WmsScheduleParam req)
        {
            CommonService commonService = new CommonService();
            var f050001Repo = new F050001Repository(Schemas.CoreSchema);
            var f050002Repo = new F050002Repository(Schemas.CoreSchema);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema);
            var stockService = new StockService();
            var msgList = new List<string>();

            // [A]=取得物流中心服務的貨主資料
            var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
            foreach (var dcCust in dcCustList)
            {
                msgList.Add("物流中心服務貨主 : [DC_CODE : " + dcCust.DC_CODE + " GUP_CODE : " + dcCust.GUP_CODE + " CUST_CODE : " + dcCust.CUST_CODE + "]");

                // [B]=從訂單池取得未配庫跨庫出貨訂單商品訂購數
                var moveOutOrdQty = f050001Repo.GetMoveOutOrdItemOty(dcCust.DC_CODE, dcCust.GUP_CODE, dcCust.CUST_CODE).ToList();

                if (moveOutOrdQty.Any())
                {
                    var checkResult = ApiLogHelper.CreateApiLogInfo(dcCust.DC_CODE, dcCust.GUP_CODE, dcCust.CUST_CODE, "MoveOutReplenshSchedule", new object { }, () =>
                    {
                        var wmsTransaction = new WmsTransaction();
                        var replenishService = new ReplenishService(wmsTransaction);

                        // [C]=取得商品各區庫存數(若品號超過500筆，請分批取得)
                        var itemList = moveOutOrdQty.Select(o => o.ITEM_CODE).Distinct().ToList();
                        var stockByLoc = f1913Repo.GetF1913ByLocStauts(dcCust.DC_CODE, dcCust.GUP_CODE, dcCust.CUST_CODE, itemList);

                        var replenishList = new List<ItemNeedQtyModel>();

                        // Foreach [B1] IN [B]
                        foreach (var ord in moveOutOrdQty)
                        {
                            // [D1]=取得商品揀區庫存
                            var availableStock = stockByLoc.Where(o => o.DC_CODE == ord.DC_CODE &&
                                                                       o.GUP_CODE == ord.GUP_CODE &&
                                                                       o.CUST_CODE == ord.CUST_CODE &&
                                                                       o.ITEM_CODE == ord.ITEM_CODE &&
                                                                       new[] { "A","B" }.Contains(o.ATYPE_CODE));

                            if (!string.IsNullOrWhiteSpace(ord.MAKE_NO))
                            {
                                availableStock = availableStock.Where(o => o.MAKE_NO == ord.MAKE_NO);
                            }

                            // 取得商品揀區庫存數量
                            var availableQty = availableStock.Sum(o => o.QTY);
                        
                            // 分配商品揀區庫存給訂單數量
                            foreach (var compare in availableStock)
                            {
                              // 該商品是否還有訂單數量未分配到庫存
                              if (ord.ORD_QTY > 0)
                              {
                                // 是否有指定批號
                                if (!string.IsNullOrWhiteSpace(ord.MAKE_NO))
                                { 
                                  if (compare.ITEM_CODE == ord.ITEM_CODE && compare.MAKE_NO == ord.MAKE_NO && (compare.ATYPE_CODE == "A" || compare.ATYPE_CODE == "B") && compare.QTY > 0)
                                  {
                                    // 揀區庫存 >= 商品訂單數量，不用補貨
                                    if (compare.QTY >= ord.ORD_QTY)
                                    {
                                      compare.QTY -= ord.ORD_QTY;
                                      ord.ORD_QTY = 0;
                                      msgList.Add("品號：" + ord.ITEM_CODE + " 批號：" + ord.MAKE_NO + " 揀區數量足夠，不產生補貨調撥單");
                                    }
                                    else
                                    {
                                      // 揀區庫存 < 商品訂單數量，訂單數量扣除揀區庫存數量得出要補貨數量
                                      ord.ORD_QTY = Convert.ToInt32(ord.ORD_QTY - compare.QTY);
                                      compare.QTY = 0;
                                    }
                                  }
                                  else
                                  {
                                    continue;
                                  }
                                }
                                else
                                {
                                  if (compare.ITEM_CODE == ord.ITEM_CODE && (compare.ATYPE_CODE == "A" || compare.ATYPE_CODE == "B") && compare.QTY > 0)
                                  {
                                    // 揀區庫存 >= 商品訂單數量，不用補貨
                                    if (compare.QTY >= ord.ORD_QTY)
                                    {
                                      compare.QTY -= ord.ORD_QTY;
                                      ord.ORD_QTY = 0;
                                      msgList.Add("品號：" + ord.ITEM_CODE + " 批號：" + ord.MAKE_NO + " 揀區數量足夠，不產生補貨調撥單");
                                    }
                                    else
                                    {
                                      // 揀區庫存 < 商品訂單數量，訂單數量扣除揀區庫存數量得出要補貨數量
                                      ord.ORD_QTY = Convert.ToInt32(ord.ORD_QTY - compare.QTY);
                                      compare.QTY = 0;
                                    }
                                  }
                                  else
                                  {
                                    continue;
                                  }
                            }
                              }
                              else
                              {
                                break;
                              }
                            }

                            // [D2]=取得商品補區庫存
                            var replenishmentStock = stockByLoc.Where(o => o.DC_CODE == ord.DC_CODE &&
                                                                      o.GUP_CODE == ord.GUP_CODE &&
                                                                      o.CUST_CODE == ord.CUST_CODE &&
                                                                      o.ITEM_CODE == ord.ITEM_CODE &&
                                                                      o.ATYPE_CODE == "C");

                            if (!string.IsNullOrWhiteSpace(ord.MAKE_NO))
                            {
                                replenishmentStock = replenishmentStock.Where(o => o.MAKE_NO == ord.MAKE_NO);
                            }

                            // 取得商品補區庫存數量
                            var replenishmentQty = replenishmentStock.Sum(o => o.QTY);

                            // 如果商品補區庫存數[D2] <= 0
                            if (replenishmentQty <= 0 && ord.ORD_QTY > 0)
                            {
                                msgList.Add("品號：" + ord.ITEM_CODE + " 批號：" + ord.MAKE_NO + " 補區無庫存數量，不產生補貨調撥單");
                                continue;
                            }
                            else if (ord.ORD_QTY > 0)
                            {
                                // [F]=加入需要補貨清單裡(ITEM_CODE,MAKE_NO,[E])
                                replenishList.Add(
                                    new ItemNeedQtyModel
                                    {
                                        ItemCode = ord.ITEM_CODE,
                                        MakeNo = ord.MAKE_NO,
                                        NeedQty = ord.ORD_QTY
                                    }
                                );
                                msgList.Add("品號：" + ord.ITEM_CODE + " 批號：" + ord.MAKE_NO + " 需補數量：" + ord.ORD_QTY);
                            }
                        }

                        var isPass = false;
                        var result = new ReplenishProcessRes();
											  string allotBatchNo=string.Empty;
                        try
                        {
												var itemCodes = replenishList.Select(x => new ItemKey { DcCode = dcCust.DC_CODE, GupCode = dcCust.GUP_CODE, CustCode = dcCust.CUST_CODE, ItemCode = x.ItemCode }).Distinct().ToList();
												allotBatchNo = "BC" + DateTime.Now.ToString("yyyyMMddHHmmss");
												// 檢查系統是否在配庫中
												isPass = stockService.CheckAllotStockStatus(true, allotBatchNo,itemCodes);

                          if (!isPass)
                          {
                            return new ApiResult { IsSuccessed = false, MsgContent = "仍有程序正在配庫跨庫調撥補貨所配庫商品，請稍待再配庫" };
                          }
                          else
                          {
                            // [G]=呼叫補貨共用函數
                            result = replenishService.ReplenishProcess(ReplenishType.Manual, dcCust.DC_CODE, dcCust.GUP_CODE, dcCust.CUST_CODE, replenishList, "3");
                            
                            // 只要有一筆成功就需要commit
                            if (result.ReturnAllocationList.Any(o => o.IsSuccessed == true))
                            {
                              wmsTransaction.Complete();
                            }
                          }
                        }
                        finally
                        {
                          if (isPass)
                            stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
                        }

                        // 回傳[G]. ReturnAllocationList，訊息用換行符號分隔
                        msgList.Add("=======================================================================");
                        foreach (var returnAllocation in result.ReturnAllocationList)
                        {
                            msgList.Add(string.Join("\r\n", returnAllocation.Message));
                            msgList.Add(string.Join("\r\n", returnAllocation.No));
                        }

                        return new ApiResult { IsSuccessed = true };
                    }, true);

                    if (!checkResult.IsSuccessed)
                    {
                      return checkResult;
                    }
                }
                else
                {
                    // 如果[B]無資料，回傳[無跨庫出貨訂單資料，不需處理]
                    msgList.Add("無跨庫出貨訂單資料，不需處理");
                }
            }

            return new ApiResult { IsSuccessed = true, MsgContent = string.Join(Environment.NewLine, msgList)};
        }
    }
}