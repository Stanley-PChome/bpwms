using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Wms3pl.Common.Extensions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Helper;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Transaction.T05.ServiceEntites;

namespace Wms3pl.WebServices.Transaction.T05.Services
{
    /// <summary>
    /// 新配庫程式-單純只做訂單配庫不產揀貨單和出貨單改產生配庫後揀貨資料
    /// </summary>
    public partial class T050102Service
    {
        private WmsTransaction _wmsTransaction;
        private CommonService _commonService;
        private SharedService _sharedService;
        private StockService _stockService;
        private WmsLogHelper _wmsLogHelper;

        private F0500Repository _f0500Repo;
        private F050001Repository _f050001Repo;
        private F050002Repository _f050002Repo;
        private F050101Repository _f050101Repo;
        private F050301Repository _f050301Repo;
        private F050302Repository _f050302Repo;
        private F05030201Repository _f05030201Repo;
        private F050306Repository _f050306Repo;
        private F1913Repository _f1913Repo;
        private F190105Repository _f190105Repo;

        // 開始配庫訂單數
        private int _startAllocOrdCnt = 0;
        // 完成配庫訂單數
        private int _finishAllocOrdCnt = 0;
        // 配庫批次累積數
        private int _pickIndex = 0;
        // 訂單批次取得最大筆數
        private int _batchMaxCount = 1000;

        private List<ExecuteResult> _exeResults;
        private List<F198001> _f198001s;
        private List<F1905> _f1905s;
        private List<F050306> _f050306s;
        private List<F050301> _f050301s;
        private List<F190105> _f190105s;
        private List<F050004Ex> _f050004Exs;
        private List<ItemLimitValidDay> _itemLimitValidDays;
        private List<RetailCarPeriod> _retailCarPeriodList;
        /// <summary>
        /// 總庫已使用的指定序號<序號, 序號出現次數>
        /// </summary>
        private Dictionary<string, int> _usedAssignationSerials;

        /// <summary>
        /// 揀區已使用的指定序號<序號, 序號出現次數>
        /// </summary>
        private Dictionary<string, int> _pickUsedAssignationSerials;

        /// <summary>
        /// 各倉別商品缺貨數量清單
        /// </summary>
        private Dictionary<string, Dictionary<ItemMakeNoAndSerialNo, int>> _notEnoughWareHouses;
        /// <summary>
        /// 已配庫但未全配完的訂單明細List
        /// </summary>
        private List<AllotedPartOrdDetailInfo> _allotedPartOrdDetailInfos;

        /// <summary>
        /// 商品捕貨需求清單
        /// </summary>
        private List<ItemNeedQtyModel> _itemNeedQtyModels;

        public T050102Service(WmsTransaction wmsTransaction)
        {
            _wmsTransaction = wmsTransaction;
            if (_wmsTransaction != null)
                _wmsTransaction.UseBulkInsertFirst = true;
            _commonService = new CommonService();
            _sharedService = new SharedService(_wmsTransaction);
            _stockService = new StockService(_wmsTransaction);
            _wmsLogHelper = new WmsLogHelper();
            _exeResults = new List<ExecuteResult>();
            _allotedPartOrdDetailInfos = new List<AllotedPartOrdDetailInfo>();

            _f0500Repo = new F0500Repository(Schemas.CoreSchema);
            _f050001Repo = new F050001Repository(Schemas.CoreSchema);
            _f050002Repo = new F050002Repository(Schemas.CoreSchema);
            _f050101Repo = new F050101Repository(Schemas.CoreSchema);
            _f190105Repo = new F190105Repository(Schemas.CoreSchema);
            _f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
            _f050302Repo = new F050302Repository(Schemas.CoreSchema, _wmsTransaction);
            _f05030201Repo = new F05030201Repository(Schemas.CoreSchema, _wmsTransaction);
            _f050306Repo = new F050306Repository(Schemas.CoreSchema, _wmsTransaction);
            _f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
        }

        #region 配庫共用Cache

        /// <summary>
        /// 取得單據類型出貨批次產生參數設定
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        private List<F050004Ex> GetF050004Exs()
        {
            if (_f050004Exs == null)
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    _f050004Exs = new List<F050004Ex>();
                    var f050004Repo = new F050004Repository(Schemas.CoreSchema);
                    _f050004Exs = f050004Repo.GetF050004Exs(new List<string> { "O1", "O2", "OA" }).ToList();
                }
            }
            return _f050004Exs;
        }

        /// <summary>
        /// 取得出貨倉別型態資料
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        private F198001 GetF198001(string typeId)
        {
            if (_f198001s == null)
                _f198001s = new List<F198001>();
            var f198001 = _f198001s.FirstOrDefault(x => x.TYPE_ID == typeId);
            if (f198001 == null)
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    var f198001Repo = new F198001Repository(Schemas.CoreSchema, _wmsTransaction);
                    f198001 = f198001Repo.Find(x => x.TYPE_ID == typeId);
                    _f198001s.Add(f198001);
                }
            }
            return f198001;
        }

        /// <summary>
        /// 依門市取得品項效期天數
        /// </summary>
        /// <param name="f050301"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        private List<ItemLimitValidDay> GetItemLimitValidDays(string gupCode, string custCode, string reltailCode, List<string> itemCodes)
        {
            if (_itemLimitValidDays == null)
                _itemLimitValidDays = new List<ItemLimitValidDay>();

            var itemLimitValidDays = _itemLimitValidDays.Where(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.RETAIL_CODE == reltailCode && itemCodes.Contains(a.ITEM_CODE)).ToList();
            if (itemLimitValidDays == null || !itemLimitValidDays.Any())
            {
                var f191001Repo = new F191001Repository(Schemas.CoreSchema);
                itemLimitValidDays = f191001Repo.GeItemLimitValidDays(gupCode, custCode, reltailCode, itemCodes).ToList();
                _itemLimitValidDays.AddRange(itemLimitValidDays);
            }

            return itemLimitValidDays;
        }

        public List<RetailCarPeriod> GetRetailCarPeriods(string dcCode, string gupCode, string custCode, List<string> retailCodes)
        {
            var list = new List<RetailCarPeriod>();
            if (_retailCarPeriodList == null)
                _retailCarPeriodList = new List<RetailCarPeriod>();
            var datas = _retailCarPeriodList.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && retailCodes.Contains(x.RETAIL_CODE)).ToList();
            list.AddRange(datas);
            var existsRetails = datas.Select(x => x.RETAIL_CODE).Distinct().ToList();
            var noExistsRetails = retailCodes.Except(existsRetails).ToList();
            if (noExistsRetails.Any())
            {
                var repF194716 = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
                var retailCarPeriods = repF194716.GetRetailCarPeriods(dcCode, gupCode, custCode, noExistsRetails);
                _retailCarPeriodList.AddRange(retailCarPeriods);
                list.AddRange(retailCarPeriods);
            }
            return list;
        }

        private List<F1905> GetF1905s(string gupCode, string custCode, List<string> itemCodes)
        {
            if (_f1905s == null)
                _f1905s = new List<F1905>();
            var existItemCodes = _f1905s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).Select(x => x.ITEM_CODE).ToList();
            // 找出還沒有快取的F1905
            var newItemCodes = itemCodes.Except(existItemCodes).ToList(); ;
            if (newItemCodes.Any())
            {
                var f1905Rep = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);
                var f1905s = f1905Rep.InWithTrueAndCondition("ITEM_CODE", newItemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();
                _f1905s.AddRange(f1905s);
            }
            return _f1905s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCodes.Contains(x.ITEM_CODE)).ToList();
        }

        private List<F190105> GetCanAutoAllotF190105s()
        {
            if (_f190105s == null)
            {
                _f190105s = _f190105Repo.GetDatasByTrueAndCondition(x => x.OPEN_AUTO_ALLOC_STOCK == "1").ToList();
            }
            return _f190105s;
        }
        #endregion

        #region 配庫主流程

        /// <summary>
        /// 排程配庫
        /// </summary>
        /// <returns></returns>
        public IQueryable<ExecuteResult> AllotStocks()
        {
            //訂單池訂單配庫
            return AllotStocks(new List<string>(), true);
        }
        /// <summary>
        /// 手動挑單配庫
        /// </summary>
        /// <param name="ordNos"></param>
        /// <returns></returns>
        public IQueryable<ExecuteResult> AllotStocks(List<string> ordNos, string priorityCode)
        {
            return AllotStocks(ordNos, false, priorityCode);
        }
        /// <summary>
        /// 配庫主流程
        /// </summary>
        /// <param name="ordNos"></param>
        /// <param name="isAutoAllotStock"></param>
        /// <param name="priorityCode"></param>
        /// <returns></returns>
        private IQueryable<ExecuteResult> AllotStocks(List<string> ordNos, bool isAutoAllotStock, string priorityCode = null)
        {
            if (string.IsNullOrWhiteSpace(priorityCode))
                priorityCode = null;

            List<F050301> CanceledOrders = null;

            _wmsLogHelper.StartRecord(WmsLogProcType.AllotStock);
            _startAllocOrdCnt = 0;
            _finishAllocOrdCnt = 0;

			var limitDcList = new List<string>();
			// 自動配庫 檢查是否啟用配庫或是否符合配庫時間設定內
			if (isAutoAllotStock)
			{
        var autoAllotStockDcListRes = GetCanAutoAllotStockDcList();
        limitDcList = autoAllotStockDcListRes.CanAutoAllotStockDcList;

        if (!limitDcList.Any())
        {
          _exeResults.AddRange(autoAllotStockDcListRes.LogMsgs.Select(x => new ExecuteResult { IsSuccessed = false, Message = x }));
          foreach (var item in autoAllotStockDcListRes.LogMsgs)
          _wmsLogHelper.AddRecord(item);

          var msg = "沒有任何符合自動配庫時間且允續自動配庫的物流中心";
          _exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = msg });
          _wmsLogHelper.AddRecord(msg);
          _wmsLogHelper.StopRecord();
          return _exeResults.AsQueryable();
        }
      }
      else
			{
				if (!ordNos.Any())
				{
					var msg = "請勾選要配庫的訂單";
					_exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = msg });
					_wmsLogHelper.AddRecord(msg);
					_wmsLogHelper.StopRecord();
					return _exeResults.AsQueryable();
				}
			}

            var f050001s = new List<F050001>();
            var f050002s = new List<F050002>();

            bool checkstatus = false;
            bool hasOrderAllot = false;
            string allotBatchNo = string.Empty;
            try
            {
                //鎖定訂單&取得未配庫訂單資料
                var res = LockOrders(isAutoAllotStock, limitDcList, ordNos, ref f050001s, ref f050002s, ref allotBatchNo);
                if (!res.IsSuccessed)
                {
                    _exeResults.Add(res);
                    _wmsLogHelper.AddRecord(res.Message);
                    _wmsLogHelper.StopRecord();
                    return _exeResults.AsQueryable();
                }
                hasOrderAllot = f050001s.Any();

                if (!hasOrderAllot)
                {
                    _wmsLogHelper.AddRecord("無可配庫的訂單");
                }
                else
                {

                    // 將空字串的 MAKE_NO統一轉成Null
                    // 將空字串的 SERIAL_NO統一轉成NULL
                    if (f050002s != null)
                    {
                        f050002s.ForEach(a =>
                        {
                            if (string.IsNullOrEmpty(a.MAKE_NO))
                            {
                                a.MAKE_NO = null;
                            }
                            if (string.IsNullOrEmpty(a.SERIAL_NO))
                            {
                                a.SERIAL_NO = null;
                            }
                        });
                    }

                    _startAllocOrdCnt = f050001s.Count;
                    var isCheckResult = CheckOrder(ref f050001s);
                    if (!isCheckResult)
                        return _exeResults.AsQueryable();

                    _wmsLogHelper.AddRecord("檢查配庫商品鎖定狀態開始");
                    var itemList = f050002s.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
                    checkstatus = _stockService.CheckAllotStockStatus(isAutoAllotStock, allotBatchNo, itemList);
                    _wmsLogHelper.AddRecord("檢查配庫商品鎖定狀態結束");
                    if (!checkstatus)
                    {
                        var msg = "仍有程序正在配庫訂單所配庫商品，請稍待再配庫";
                        _exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = msg });
                        _wmsLogHelper.AddRecord(msg);
                        _wmsLogHelper.StopRecord();
                        return _exeResults.AsQueryable();
                    }
                    _wmsLogHelper.AddRecord("已更改配庫商品狀態為鎖定，配庫批次號" + allotBatchNo);


                    // 配庫訂單拆批次
                    var splitOrders = SplitBatchOrder(f050001s, f050002s);

                    _pickIndex = 0;
                    foreach (var splitOrder in splitOrders)
                    {
                        var dcCode = splitOrder.First().DC_CODE;
                        var gupCode = splitOrder.First().GUP_CODE;
                        var custCode = splitOrder.First().CUST_CODE;

                        var splitOrdNos = splitOrder.Select(x => x.ORD_NO).ToList();
                        var splitOrderDetails = f050002s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && splitOrdNos.Contains(x.ORD_NO)).ToList();
                        var splitOrderDetailExts = MergeBomItemList(dcCode, gupCode, custCode, ref splitOrderDetails);
                        // 區分有無批號商品訂單(有批號的訂單先配庫)
                        var splitHasMakeNoOrSerialNos = SplitOrdersByHasMakeNoOrHasSerialNo(splitOrder, splitOrderDetails);

                        foreach (var splitHasMakeNoOrSerialNoOrder in splitHasMakeNoOrSerialNos)
                        {
                            try
                            {
                                _usedAssignationSerials = new Dictionary<string, int>();
                                _pickUsedAssignationSerials = new Dictionary<string, int>();
                                _notEnoughWareHouses = new Dictionary<string, Dictionary<ItemMakeNoAndSerialNo, int>>();
                                _itemNeedQtyModels = new List<ItemNeedQtyModel>();
                                _f050306s = new List<F050306>();
                                _f050301s = new List<F050301>();
                                var splitHasMakeNoOrSerialNoOrdNos = splitHasMakeNoOrSerialNoOrder.Select(x => x.ORD_NO).ToList();
                                var splitHasMakeNoOrSerialNoOrderDetails = f050002s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && splitHasMakeNoOrSerialNoOrdNos.Contains(x.ORD_NO)).ToList();
                                var splitHasMakeNoOrSerialNoOrderExts = splitOrderDetailExts.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && splitHasMakeNoOrSerialNoOrdNos.Contains(x.ORD_NO)).ToList();
                                // 累加批次序號
                                _pickIndex++;
                                // 配庫處理
                                var result = AllotStockProcess(dcCode, gupCode, custCode, splitHasMakeNoOrSerialNoOrder, splitHasMakeNoOrSerialNoOrderDetails, splitHasMakeNoOrSerialNoOrderExts, isAutoAllotStock, priorityCode);
                                if (result)
                                {
                                    if (!isAutoAllotStock)
                                    {
                                        //若為手動挑單配庫則產生揀貨批次
                                        var result2 = _sharedService.CreatePick("01", _f050306s, false, _f050301s, !string.IsNullOrWhiteSpace(priorityCode));
                                        if (result2.IsSuccessed)
                                        {
                                            _wmsLogHelper.AddRecord("執行庫存異動 開始");
                                            _stockService.SaveChange();
                                            _wmsLogHelper.AddRecord("執行庫存異動 結束");
                                            _wmsLogHelper.AddRecord("執行db commit 開始");
                                            _wmsTransaction.Complete();
                                            _wmsLogHelper.AddRecord("執行db commit 結束");

                                            _wmsLogHelper.AddRecord("檢查訂單狀態 開始");
                                            var result3 = _sharedService.AfterCreatePickCheckOrder(_f050306s, out CanceledOrders);
                                            if (result3.IsSuccessed)
                                            {
                                                _wmsLogHelper.AddRecord("執行db commit 開始");
                                                _wmsTransaction.Complete();
                                                _wmsLogHelper.AddRecord("執行db commit 結束");
                                            }
                                            else
                                                _exeResults.Add(result3);
                                            _wmsLogHelper.AddRecord("檢查訂單狀態 結束");

                                        }
                                        else
                                            _exeResults.Add(result2);
                                    }
                                    else
                                    {
                                        _wmsLogHelper.AddRecord("執行db commit 開始");
                                        _wmsTransaction.Complete();
                                        _wmsLogHelper.AddRecord("執行db commit 結束");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //丟到訊息池
                                //配庫發生不可預期的錯誤!
                                AddLog(dcCode, gupCode, custCode, "AAM00001", _commonService.GetMsg("AAM00001"), true);
                                TxtLog(ex.Message);
                                throw new Exception("配庫發生例外", ex);
                            }
                        }
                        #region 產生補貨調撥單
                        if (_itemNeedQtyModels.Any())
                        {
                            // 相同條件 需求量加總起來
                            _itemNeedQtyModels = _itemNeedQtyModels.GroupBy(x => new { x.ItemCode, x.MakeNo, x.SerialNo }).Select(x => new ItemNeedQtyModel
                            {
                                ItemCode = x.Key.ItemCode,
                                MakeNo = x.Key.MakeNo,
                                SerialNo = x.Key.SerialNo,
                                NeedQty = x.Sum(y => y.NeedQty)
                            }).ToList();
                            var replenishService = new ReplenishService(_wmsTransaction);
                            var replenishProcessResult = replenishService.ReplenishProcess(ReplenishType.Manual, dcCode, gupCode, custCode, _itemNeedQtyModels, "3");
                            var msgList = new List<string>();

                            if (replenishProcessResult.ReturnAllocationList.Any(o => o.IsSuccessed == true))
                            {
                                _wmsTransaction.Complete();
                            }

                            foreach (var item in replenishProcessResult.ReturnAllocationList)
                            {
                                var msg = string.Format(_commonService.GetMsg("AAM00027"), _commonService.GetDc(dcCode).DC_NAME, _commonService.GetGup(gupCode).GUP_NAME, _commonService.GetCust(gupCode, custCode).CUST_NAME, item.No + Environment.NewLine + item.Message);
                                msgList.Add(msg);
                            }
                            AddLog(dcCode, gupCode, custCode, "AAM00027", string.Join(Environment.NewLine, msgList), true);
                        }

                        #endregion
                    }
                }
            }
            finally
            {
                if (hasOrderAllot)
                {
                    _wmsLogHelper.AddRecord("清除已配庫的訂單池訂單開始");
                    _f050002Repo.DeleteHasAllot();
                    _f050001Repo.DeleteHasAllot();
                    _wmsLogHelper.AddRecord("清除已配庫的訂單池訂單結束");

                    _wmsLogHelper.AddRecord("修改訂單池資料未配庫開始");
                    if (!string.IsNullOrWhiteSpace(allotBatchNo))
                    {
                        _f050001Repo.UnLockByAllotBatchNo(allotBatchNo);
                    }
                    _wmsLogHelper.AddRecord("修改訂單池資料未配庫結束");


                    // 更改配庫狀態為未配庫
                    if (checkstatus)
                    {
                        _wmsLogHelper.AddRecord("修改配庫狀態開始");
                        _stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
                        _wmsLogHelper.AddRecord("將配庫狀態改回待配庫");
                    }
                    else
                        _wmsLogHelper.AddRecord("仍有程序在配庫中，配庫狀態不須調整");

                    _wmsLogHelper.AddRecord("修改配庫狀態結束");
                }
            }
            var ReturnMessage = string.Format(Properties.Resources.AllocOrderMessage, _startAllocOrdCnt, _finishAllocOrdCnt);
            if ((CanceledOrders?.Count() ?? 0) > 0)
                ReturnMessage += $"(其中包含被LMS取消{CanceledOrders.Count()}筆)";
            if (!_exeResults.Any())
                _exeResults.Add(new ExecuteResult { IsSuccessed = true, Message = ReturnMessage });
            _wmsLogHelper.StopRecord();

            return _exeResults.AsQueryable();
        }
        #region TxtLog
        /// <summary>
        /// 紀錄Log至allotStockErrorLog.txt
        /// </summary>
        /// <param name="message"></param>
        private static void TxtLog(string message, bool isShowDatetime = true)
        {
            var dir = new DirectoryInfo(@"C:\PHWMS\ALLOTSTOCK\");
            if (!dir.Exists)
                dir.Create();

            var fileFullName = Path.Combine(dir.FullName, $"allotStockErrorLog.txt");

            using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
                sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
        }
        #endregion


        #endregion

		#region 取得符合自動配庫時間且開啟自動配庫的物流中心清單
		/// <summary>
		/// 取得符合自動配庫時間且開啟自動配庫的物流中心清單
		/// </summary>
		/// <returns></returns>
		private CanAutoAllotStockDcListRes GetCanAutoAllotStockDcList()
    {
      var logLists = new List<string>();
      var limitDcList = new List<string>();
      var f190106Repo = new F190106Repository(Schemas.CoreSchema);
      var f190105s = GetCanAutoAllotF190105s();
      if (f190105s.Any())
        logLists.Add("物流中心出貨指示設定可自動配庫物流中心：" + string.Join(",", f190105s.Select(x => x.DC_CODE)));
      else
        logLists.Add("物流中心出貨指示設定無可自動配庫物流中心");

      var nowTime = DateTime.Now;
      logLists.Add($"目前系統時間:{nowTime.ToString("HH:mm:ss")}");
      foreach (var f190105 in f190105s)
      {
        var f190106s = f190106Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f190105.DC_CODE && x.SCHEDULE_TYPE == "01").ToList();
        logLists.Add($"{f190105.DC_CODE}物流中心可配庫時間：{string.Join(", ", f190106s.Select(x => x.START_TIME + "-" + x.END_TIME + " 執行頻率:" + x.PERIOD))}");
        var haveAllotTime = false;
        foreach (var f190106 in f190106s)
        {
          var startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, int.Parse(f190106.START_TIME.Split(':')[0]), int.Parse(f190106.START_TIME.Split(':')[1]), 0);
          var endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, int.Parse(f190106.END_TIME.Split(':')[0]), int.Parse(f190106.END_TIME.Split(':')[1]), 0);
          if (endTime < startTime)
            endTime = endTime.AddDays(1);
          if (nowTime >= startTime && nowTime <= endTime)
          {
            var ts = nowTime - startTime;
            if (Math.Floor(ts.TotalMinutes) % f190106.PERIOD == 0)
            {
              limitDcList.Add(f190105.DC_CODE);
              haveAllotTime = true;
            }
            break;
          }
        }
        if (!haveAllotTime)
          logLists.Add($"{f190105.DC_CODE}物流中心無符合的可配庫時間");
      }
      return new CanAutoAllotStockDcListRes { LogMsgs = logLists, CanAutoAllotStockDcList = limitDcList };
    }

        #endregion

        #region 鎖定訂單&取得未配庫訂單資料

        private ExecuteResult LockOrders(bool isAutoAllotStock, List<string> limitDcList, List<string> ordNos, ref List<F050001> f050001s, ref List<F050002> f050002s, ref string allotBatchNo)
        {
            _wmsLogHelper.AddRecord("鎖定訂單池訂單開始");
            var f050001Repo = new F050001Repository(Schemas.CoreSchema);
            var f050002Repo = new F050002Repository(Schemas.CoreSchema);
            var f050001List = new List<F050001>();
            var strAllotBatchNo = string.Empty;
            var repeatOrdNos = new List<string>();
            var f190105s = new List<F190105>();
            var f050004Exs = new List<F050004Ex>();
            if (isAutoAllotStock)
            {
                f190105s = GetCanAutoAllotF190105s();

                // 自動配庫限制只能B2C訂單
                f050004Exs = GetF050004Exs().Where(x => x.TICKET_CLASS == "O2" && limitDcList.Contains(x.DC_CODE)).ToList();
            }
            var f5 = f050001Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }), () =>
                {
                    f050001Repo.LockF050001();
                    strAllotBatchNo = "BS" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var flag = "0";
                    if (isAutoAllotStock)
                    {
                        //_wmsLogHelper.AddRecord($"更新訂單池訂單配庫批次號{strAllotBatchNo}開始");
                        foreach (var f050004Ex in f050004Exs)
                        {
                            var custCodes = f190105s.Where(x => x.DC_CODE == f050004Ex.DC_CODE).FirstOrDefault()?.AUTO_ALLOT_CUST_LIST.Split(',').ToList();
                            if (custCodes != null && custCodes.Contains(f050004Ex.CUST_CODE))
                                f050001Repo.LockNonAllotOrderStatus(strAllotBatchNo, f050004Ex.TICKET_ID, f050004Ex.ORDER_LIMIT);
                        }
                        //_wmsLogHelper.AddRecord($"更新訂單池訂單配庫批次號{strAllotBatchNo}結束");

                        flag = "1";
                    }
                    else
                    {

                        #region 批次取得訂單池訂單
                        //_wmsLogHelper.AddRecord("取得訂單池訂單開始");
                        var batchOrders = new List<List<string>>();
                        if (ordNos.Count > _batchMaxCount)
                        {
                            var pages = ordNos.Count / _batchMaxCount + (ordNos.Count % _batchMaxCount > 0 ? 1 : 0);
                            for (var page = 0; page < pages; page++)
                                batchOrders.Add(ordNos.Skip(page * _batchMaxCount).Take(_batchMaxCount).ToList());
                        }
                        else
                            batchOrders.Add(ordNos);

                        using (var ts = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            foreach (var batchOrder in batchOrders)
                            {
                                f050001List.AddRange(f050001Repo.GetOrdersByOrdNos(batchOrder).ToList());
                            }
                        }
                        //_wmsLogHelper.AddRecord("取得訂單池訂單結束");
                        #endregion

                        //_wmsLogHelper.AddRecord("檢查指定訂單是否有正在配庫訂單開始");
                        repeatOrdNos.AddRange(f050001List.Where(x => x.PROC_FLAG == "1").Select(x => x.ORD_NO).ToList());
                        if (repeatOrdNos.Any())
                        {
                            flag = "0";
                            //_wmsLogHelper.AddRecord($"檢查指定訂單是否有正在配庫訂單結束-有正在配庫訂單共{repeatOrdNos.Count}筆");
                        }
                        else
                        {
                            //_wmsLogHelper.AddRecord($"檢查指定訂單是否有正在配庫訂單結束-無正在配庫訂單");

                            #region 依照單據類型篩選可配庫訂單符合該類型訂單上限
                            //_wmsLogHelper.AddRecord($"依照單據類型篩選可配庫訂單符合該類型訂單上限開始");
                            var group = f050001List.GroupBy(x => x.TICKET_ID).ToList();
                            var canAllotF050001List = new List<F050001>();
                            foreach (var g in group)
                            {
                                var f050004Ex = GetF050004Exs().FirstOrDefault(x => x.TICKET_ID == g.Key);
                                if (f050004Ex != null)
                                {
                                    canAllotF050001List.AddRange(g.Take(f050004Ex.ORDER_LIMIT));
                                }
                                else
                                    canAllotF050001List.AddRange(g);
                            }
                            f050001List = canAllotF050001List;
                            //_wmsLogHelper.AddRecord($"依照單據類型篩選可配庫訂單符合該類型訂單上限結束");
                            #endregion

                            #region 批次更新訂單池訂單配庫批次號
                            //_wmsLogHelper.AddRecord($"批次更新訂單池訂單配庫批次號{ strAllotBatchNo}開始");
                            batchOrders = new List<List<string>>();
                            if (f050001List.Count > _batchMaxCount)
                            {
                                var pages = f050001List.Count / _batchMaxCount + (f050001List.Count % _batchMaxCount > 0 ? 1 : 0);
                                for (var page = 0; page < pages; page++)
                                    batchOrders.Add(f050001List.Skip(page * _batchMaxCount).Take(_batchMaxCount).Select(x => x.ORD_NO).ToList());
                            }
                            else
                                batchOrders.Add(f050001List.Select(x => x.ORD_NO).ToList());

                            foreach (var batchOrder in batchOrders)
                            {
                                f050001Repo.LockNonAllotOrderStatusByOrdNos(strAllotBatchNo, batchOrder);
                            }
                            f050001List.ForEach(x => { x.ALLOT_BATCH_NO = strAllotBatchNo; });
                            //_wmsLogHelper.AddRecord($"批次更新訂單池訂單配庫批次號{ strAllotBatchNo}結束");
                            #endregion

                            flag = "1";
                        }
                    }
                    return new F050001 { PROC_FLAG = flag };
                });

            _wmsLogHelper.AddRecord("鎖定訂單池訂單結束");

            if (f5.PROC_FLAG == "1")
            {
                if (isAutoAllotStock)
                {
                    _wmsLogHelper.AddRecord("取得訂單池訂單開始");
                    f050001List = f050001Repo.GetOrdersByAllotBatchNo(strAllotBatchNo).ToList();
                    _wmsLogHelper.AddRecord("取得訂單池訂單結束");
                }


                // 取得訂單池主檔
                f050001s = f050001List;
                // 取得訂單池身擋
                _wmsLogHelper.AddRecord($"取得配庫批次號{strAllotBatchNo}訂單身擋開始");
                f050002s = f050002Repo.GetDatasByAllotBatchNo(strAllotBatchNo).ToList();
                _wmsLogHelper.AddRecord($"取得配庫批次號{strAllotBatchNo}訂單身擋結束");

                allotBatchNo = strAllotBatchNo;
                return new ExecuteResult(true);
            }
            else
            {
                return new ExecuteResult(false, string.Format("以下訂單正在配庫中，不可配庫{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, repeatOrdNos)));
            }

        }

        #endregion



        #region 檢查訂單

        /// <summary>
        /// 檢查訂單
        /// </summary>
        /// <param name="f050001s">訂單資料</param>
        private bool CheckOrder(ref List<F050001> f050001s)
        {
            #region 檢查訂單單據類別是否有設定

            _wmsLogHelper.AddRecord("檢查訂單單據類別是否有設定開始");
            if (f050001s.Any(x => x.TICKET_ID == 0))
            {
                var msgNo = "AAM00019";
                var zeroTicketIdOrdList = f050001s.Where(x => x.TICKET_ID == 0).Select(x => x.ORD_NO).ToList();
                var msgContent = string.Format(_commonService.GetMsg(msgNo), string.Join("、", zeroTicketIdOrdList));
                AddLog(f050001s.First().DC_CODE, f050001s.First().GUP_CODE, f050001s.First().CUST_CODE, msgNo, msgContent, true);
                _wmsLogHelper.AddRecord(msgContent);
                return false;
            }
            _wmsLogHelper.AddRecord("檢查訂單單據類別是否有設定結束");
            #endregion

            #region 檢查單據類型出貨批次產生參數是否有設定

            _wmsLogHelper.AddRecord("檢查單據類型出貨批次產生參數是否有設定開始");
            var f050001Gs = f050001s.GroupBy(x => new { x.TICKET_ID });
            foreach (var f050001G in f050001Gs)
            {
                var f050004 = GetF050004Exs().FirstOrDefault(x => x.TICKET_ID == f050001G.Key.TICKET_ID);
                if (f050004 == null)
                {
                    var ordNo = f050001G.Select(o => o.ORD_NO);
                    _exeResults.Add(new ExecuteResult { IsSuccessed = false, Message = string.Format("訂單編號:{0}{1}{0}出貨批次產生參數未設定", Environment.NewLine, string.Join("、" + Environment.NewLine, ordNo)) });
                    _wmsLogHelper.AddRecord(_exeResults.Last().Message);
                    return false;
                }
            }
            _wmsLogHelper.AddRecord("檢查單據類型出貨批次產生參數是否有設定結束");
            #endregion

            #region 設定未指定出貨倉別的訂單

            _wmsLogHelper.AddRecord("設定未指定出貨倉別的訂單開始");
            var f190002Repo = new F190002Repository(Schemas.CoreSchema);
            var noDirectTypeIdOrders = f050001s.Where(x => string.IsNullOrEmpty(x.TYPE_ID)).ToList();
            if (noDirectTypeIdOrders.Any())
            {
                var group = noDirectTypeIdOrders.GroupBy(x => x.TICKET_ID);
                var f190002s = f190002Repo.GetDatas(group.Select(x => x.Key).ToList()).ToList();
                foreach (var items in group)
                {
                    var f190002 = f190002s.FirstOrDefault(x => x.TICKET_ID == items.Key);
                    var typeId = "G";
                    if (f190002 != null)
                        typeId = f190002.WAREHOUSE_TYPE;
                    foreach (var f050001 in items)
                        f050001.TYPE_ID = typeId;
                }
                _f050001Repo.BulkUpdate(noDirectTypeIdOrders);
            }
            _wmsLogHelper.AddRecord("設定未指定出貨倉別的訂單結束");

            #endregion

            return true;
        }

        #endregion

        #region 非加工商品處理
        /// <summary>
        /// 將非加工使用中的組合商品拆成細項
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private List<F05030201> MergeBomItemList(string dcCode, string gupCode, string custCode, ref List<F050002> detail)
        {
            var noProcMergeItemSplitValue = _commonService.GetSysGlobalValue(dcCode, gupCode, custCode, "NoProcMergeItemSplit");
            if (noProcMergeItemSplitValue == "0")
            {
                _wmsLogHelper.AddRecord("貨主設定不將非加工使用中的組合商品拆成細項");
                return new List<F05030201>();
            }
            _wmsLogHelper.AddRecord("將非加工使用中的組合商品拆成細項開始");

            var f05030201List = new List<F05030201>();
            var f910102Repo = new F910102Repository(Schemas.CoreSchema);
            var group = detail.GroupBy(o => new { o.GUP_CODE, o.CUST_CODE });
            foreach (var item in group)
            {
                //取得BOM表組合商品明細
                var bomItemList = f910102Repo.GetBomItemDetailList(item.Key.GUP_CODE, item.Key.CUST_CODE, item.Select(o => o.ITEM_CODE).Distinct().ToList());
                //從訂單找出品號為組合商品品號
                var findDataInBoms = item.Where(o => bomItemList.Select(x => x.ITEM_CODE).Any(c => c == o.ITEM_CODE)).ToList();
                //將組合商品拆成細項 (組合C 拆成 A,B)
                foreach (var findItem in findDataInBoms)
                {
                    //找出此組合商品BOM表細項
                    var bomItems = bomItemList.Where(o => o.ITEM_CODE == findItem.ITEM_CODE).ToList();
                    //從BOM表細項產生或合併訂單明細及產生不加工的組合商品明細
                    foreach (var bomItem in bomItems)
                    {
                        var newItem = AutoMapper.Mapper.DynamicMap<F05030201>(findItem);
                        newItem.BOM_ITEM_CODE = findItem.ITEM_CODE;
                        newItem.ITEM_CODE = bomItem.MATERIAL_CODE;
                        newItem.BOM_QTY = bomItem.BOM_QTY;
                        //細項商品訂貨數 = 細項商品所需數量 * 組合C訂貨數
                        newItem.ORD_QTY = bomItem.BOM_QTY * findItem.ORD_QTY;
                        //找出此訂單是否有此細項商品
                        var ordItem = detail.FirstOrDefault(o => o.DC_CODE == findItem.DC_CODE && o.GUP_CODE == findItem.GUP_CODE && o.CUST_CODE == findItem.CUST_CODE && o.ORD_NO == findItem.ORD_NO && o.ITEM_CODE == bomItem.MATERIAL_CODE);
                        //存在就合併該商品數量 
                        if (ordItem != null)
                        {
                            ordItem.ORD_QTY += newItem.ORD_QTY;
                            newItem.ORD_SEQ = ordItem.ORD_SEQ;
                        }
                        //不存在就建立一筆新的訂單明細
                        else
                        {
                            ordItem = AutoMapper.Mapper.DynamicMap<F050002>(newItem);
                            ordItem.ORD_SEQ = (detail.Where(o => o.DC_CODE == findItem.DC_CODE && o.GUP_CODE == findItem.GUP_CODE && o.CUST_CODE == findItem.CUST_CODE && o.ORD_NO == findItem.ORD_NO).Max(o => int.Parse(o.ORD_SEQ)) + 1).ToString().PadLeft(findItem.ORD_SEQ.Length, '0');
                            newItem.ORD_SEQ = ordItem.ORD_SEQ;
                            detail.Add(ordItem);
                        }
                        //寫入貨主單據身檔(不加工的組合商品明細)
                        f05030201List.Add(newItem);
                    }
                    detail.Remove(findItem);
                }
            }
            _wmsLogHelper.AddRecord("將非加工使用中的組合商品拆成細項結束");

            return f05030201List;
        }

        #endregion

        #region 區分有無批號商品訂單
        private List<List<F050001>> SplitOrdersByHasMakeNoOrHasSerialNo(List<F050001> f050001s, List<F050002> f050002s)
        {
            var result = new List<List<F050001>>();
            var hasSerialNoOrdNos = f050002s.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)).Select(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ORD_NO }).Distinct().ToList();

            var hasSerialNoOrders = (from a in f050001s
                                     join b in hasSerialNoOrdNos on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ORD_NO } equals b
                                     select a).Distinct().ToList();

            var hasMakeNoOrdNos = f050002s.Where(a => !string.IsNullOrEmpty(a.MAKE_NO) && string.IsNullOrEmpty(a.SERIAL_NO)).Select(a => new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ORD_NO }).Distinct();

            var hasMakeNoOrders = (from a in f050001s
                                   join b in hasMakeNoOrdNos on new { a.DC_CODE, a.GUP_CODE, a.CUST_CODE, a.ORD_NO } equals b
                                   select a).Distinct().ToList();

            if (hasSerialNoOrders.Any())
            {
                result.Add(hasSerialNoOrders);
            }
            if (hasMakeNoOrders.Any())
            {
                result.Add(hasMakeNoOrders);
            }
            var noMakeNoOrders = f050001s.Except(hasSerialNoOrders).Except(hasMakeNoOrders).ToList();
            if (noMakeNoOrders.Any())
            {
                result.Add(noMakeNoOrders);
            }
            return result;
        }
        #endregion

        #region 配庫訂單分批次

        /// <summary>
        /// 配庫訂單分批次
        /// </summary>
        /// <param name="f050001s"></param>
        /// <param name="f050002s"></param>
        /// <param name="isTrialCalculation"></param>
        /// <returns></returns>
        private List<List<F050001>> SplitBatchOrder(List<F050001> f050001s, List<F050002> f050002s, bool isTrialCalculation = false)
        {
            _wmsLogHelper.AddRecord("配庫訂單分批次開始");
            var results = new List<List<F050001>>();
            var custOrders = f050001s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE });
            foreach (var custOrder in custOrders)
            {
                var f1909 = _commonService.GetCust(custOrder.Key.GUP_CODE, custOrder.Key.CUST_CODE);

                var custTicketOrders = custOrder.GroupBy(x => new { x.TICKET_ID });

                foreach (var f050001G in custTicketOrders)
                {
                    var ticketOrders = f050001G.ToList();
                    // 貨主是否允許預先配庫
                    if (f1909.ALLOW_ADVANCEDSTOCK == "0" && !isTrialCalculation)
                    {
                        // 依到貨日期過濾可出貨訂單
                        var f050004 = GetF050004Exs().First(x => x.TICKET_ID == f050001G.Key.TICKET_ID);
                        // 剔除不允許預先配庫訂單
                        var overTodayOrderNos = ticketOrders.Where(x => (x.ARRIVAL_DATE.HasValue && x.ARRIVAL_DATE.Value.AddDays(-1) > DateTime.Today) || (!x.ARRIVAL_DATE.HasValue && x.ORD_DATE.AddDays(f050004.DELV_DAY).AddDays(-1) > DateTime.Today)).ToList();
                        ticketOrders = ticketOrders.Except(overTodayOrderNos).ToList();
                        if (overTodayOrderNos.Any())
                        {
                            _exeResults.Add(new ExecuteResult { IsSuccessed = true, Message = string.Format("部分訂單配庫未完成，訂單指定到貨日為非今天或明天{0}訂單編號:{0}{1}", Environment.NewLine, string.Join("、" + Environment.NewLine, overTodayOrderNos.Select(x => x.ORD_NO))) });
                            _wmsLogHelper.AddRecord(_exeResults.Last().Message);
                        }
                    }

                    // 依照優先處理旗標(由大到小)、訂單編號(由小到大)排序產生批次
                    results.Add(ticketOrders.OrderByDescending(x => x.FAST_DEAL_TYPE).ThenBy(x => x.ORD_NO).ToList());
                }
            }
            _wmsLogHelper.AddRecord("配庫訂單分批次結束");

            return results;
        }

        #endregion

        #region Log紀錄
        /// <summary>
        /// Log紀錄
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="msgNo"></param>
        /// <param name="msgContent"></param>
        /// <param name="isNoTransaction"></param>
        /// <param name="isSuccess"></param>
        private void AddLog(string dcCode, string gupCode, string custCode, string msgNo, string msgContent, bool isNoTransaction = false, bool isSuccess = false, string targetType = "0")
        {
            _sharedService.AddMessagePool("9", dcCode, gupCode, custCode, msgNo, msgContent, "", targetType, targetType == "0" ? "AA" : custCode, isNoTransaction);
            _exeResults.Add(new ExecuteResult { IsSuccessed = isSuccess, Message = msgContent });
        }

        /// <summary>
        /// 總庫缺品Log
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        private void AddNotEnoughItemByWareHousesLog(string dcCode, string gupCode, string custCode)
        {
            _wmsLogHelper.AddRecord("產生總庫缺品Log開始");
            var f1901 = _commonService.GetDc(dcCode);
            var f1929 = _commonService.GetGup(gupCode);
            var f1909 = _commonService.GetCust(gupCode, custCode);
            var notEnoughMsgs = new List<string>();
            var notEnoughMsgsCust = new List<string>();
            var notEnoughMsgs2 = new List<string>();

            foreach (var notEnoughWareHouse in _notEnoughWareHouses)
            {
                var f198001 = GetF198001(notEnoughWareHouse.Key);
                var f1903s = _commonService.GetProductList(gupCode, custCode, notEnoughWareHouse.Value.Select(x => x.Key.ItemCode).ToList());
                foreach (var notEnoughItem in notEnoughWareHouse.Value)
                {
                    var f1903 = f1903s.FirstOrDefault(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.ITEM_CODE == notEnoughItem.Key.ItemCode);

                    // 指定序號總庫不足
                    if (!string.IsNullOrEmpty(notEnoughItem.Key.SerialNo))
                    {
                        //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}:{5}」序號:「{6}」總庫存不足數量:{7}
                        notEnoughMsgs.Add(string.Format(_commonService.GetMsg("AAM00028"),
                                        (f1901 == null) ? "" : f1901.DC_NAME,
                                        (f1929 == null) ? "" : f1929.GUP_NAME,
                                        (f1909 == null) ? "" : f1909.SHORT_NAME,
                                        (f198001 == null) ? "" : f198001.TYPE_NAME,
                                        notEnoughItem.Key.ItemCode,
                                        (f1903 == null) ? "" : f1903.ITEM_NAME,
                                        notEnoughItem.Key.SerialNo,
                                        notEnoughItem.Value));
                    }
                    else if (!string.IsNullOrEmpty(notEnoughItem.Key.MakeNo))
                    {
                        //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}:{5}」批號:「{6}」總庫存不足數量:{7}
                        notEnoughMsgs.Add(string.Format(_commonService.GetMsg("AAM00026"),
                                        (f1901 == null) ? "" : f1901.DC_NAME,
                                        (f1929 == null) ? "" : f1929.GUP_NAME,
                                        (f1909 == null) ? "" : f1909.SHORT_NAME,
                                        (f198001 == null) ? "" : f198001.TYPE_NAME,
                                        notEnoughItem.Key.ItemCode,
                                        (f1903 == null) ? "" : f1903.ITEM_NAME,
                                        notEnoughItem.Key.MakeNo,
                                        notEnoughItem.Value));
                    }
                    else
                    {
                        //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}:{5}」總庫存不足數量:{6}
                        notEnoughMsgs.Add(string.Format(_commonService.GetMsg("AAM00004"),
                                        (f1901 == null) ? "" : f1901.DC_NAME,
                                        (f1929 == null) ? "" : f1929.GUP_NAME,
                                        (f1909 == null) ? "" : f1909.SHORT_NAME,
                                        (f198001 == null) ? "" : f198001.TYPE_NAME,
                                        notEnoughItem.Key.ItemCode,
                                        (f1903 == null) ? "" : f1903.ITEM_NAME,
                                        notEnoughItem.Value));

                    }

                }
            }

            // 指定序號重複配庫，或者在這批次裡面重複配庫
            foreach (var usedAssignationSerial in _usedAssignationSerials.Where(x => x.Value > 1))
            {
                //物流中心:「{0}」業主:「{1}」貨主:「{2}」指定序號:「{3}」重複配庫
                notEnoughMsgs2.Add(string.Format(_commonService.GetMsg("AAM00005"),
                                                                                                                                                (f1901 == null) ? "" : f1901.DC_NAME,
                                                                                                                                                (f1929 == null) ? "" : f1929.GUP_NAME,
                                                                                                                                                (f1909 == null) ? "" : f1909.SHORT_NAME,
                                                                                                                                                usedAssignationSerial.Key));
            }

            if (notEnoughMsgs.Any())
                AddLog(dcCode, gupCode, custCode, "AAM00004", string.Join(Environment.NewLine, notEnoughMsgs), true);

            //if (notEnoughMsgsCust.Any())
            //	AddLog(dcCode, gupCode, custCode, "AAM00016", string.Join(Environment.NewLine, notEnoughMsgsCust), false, false, "1");

            if (notEnoughMsgs2.Any())
                AddLog(dcCode, gupCode, custCode, "AAM00005", string.Join(Environment.NewLine, notEnoughMsgs2), true);

            _wmsLogHelper.AddRecord("產生總庫缺品Log結束");

        }


        #endregion

        #region 配庫處理

        /// <summary>
        /// 配庫處理
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="f050001s"></param>
        /// <param name="f050002s"></param>
        /// <param name="f05030201s"></param>
        private bool AllotStockProcess(string dcCode, string gupCode, string custCode, List<F050001> f050001s, List<F050002> f050002s, List<F05030201> f05030201s, bool isAutoAllocStock, string priorityCode)
        {
            _wmsLogHelper.AddRecord("配庫處理開始，配庫總單數:" + f050001s.Count());

            //因為每次配庫的品項不一定相同，配庫前先初始化_itemLimitValidDays
            _itemLimitValidDays = new List<ItemLimitValidDay>();
            var ordType = f050001s.First().ORD_TYPE;
            var isB2B = ordType == "0";

            var f1909 = _commonService.GetCust(gupCode, custCode);
            //依倉別群組化訂單
            var f050001GTs = f050001s.GroupBy(a => new { a.TYPE_ID });

            foreach (var f050001GT in f050001GTs)
            {
                // 取得剛倉別訂單明細排除不出庫
                var f050002GT = f050002s.Where(x => f050001GT.Select(y => y.ORD_NO).Contains(x.ORD_NO) && x.NO_DELV == "0").ToList();
                // 倉別總庫檢查
                var notEnoughItems = CheckTotalStocks(dcCode, gupCode, custCode, f050001GT.Key.TYPE_ID, f050001GT.ToList(), f050002GT);
                // 不允許部分出貨 且商品總庫不足
                if (f1909.SPILT_OUTCHECK == "0" && notEnoughItems.Any())
                {
                    _notEnoughWareHouses.Add(f050001GT.Key.TYPE_ID, notEnoughItems);
                    // 釋放缺品訂單
                    var releaseOrdNos = ReleaseLackOrders(isB2B, f1909, notEnoughItems, f050001GT.ToList(), f050002GT);
                    f050001s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
                    f050002s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
                    f05030201s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
                    // 寫入總庫缺品紀錄
                    AddNotEnoughItemByWareHousesLog(dcCode, gupCode, custCode);
                }
            }

            //B2B若有商品缺貨 且貨主設定不允許B2B單張訂單出貨  全部回訂單池
            if (isB2B && f1909.ISB2B_ALONE_OUT == "0" && _notEnoughWareHouses.Any())
            {
                _wmsLogHelper.AddRecord("B2B且貨主設定不允許B2B單張訂單出貨");
                _wmsLogHelper.AddRecord("配庫處理結束");
                return false;
            }


            if (isB2B)
            {
                // 釋放B2B有條件缺品商品訂單
                var releaseOrdNos = ReleaseB2BConditionLackOrders(dcCode, gupCode, custCode, f1909, f050001s, f050002s);
                f050001s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
                f050002s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
                f05030201s.RemoveAll(x => releaseOrdNos.Contains(x.ORD_NO));
            }
            var f050301s = new List<F050301>();
            var f050302s = new List<F050302>();
            // 複製訂單池訂單產生已配庫訂單資料
            CopyOrderToAllocOrders(dcCode, gupCode, custCode, f050001s, f050002s, ref f050301s, ref f050302s, priorityCode);

            // 開始單據配庫
            var allotedStockOrders = ExecAllotStockOrders(dcCode, gupCode, custCode, f050301s, f050302s);
            if (allotedStockOrders == null || !allotedStockOrders.Any())
            {
                _wmsLogHelper.AddRecord("沒有任何已配庫成功訂單");
                _wmsLogHelper.AddRecord("配庫處理結束");
                return false;
            }


            // 已配到庫存的訂單
            var allotedOrdNos = allotedStockOrders.Select(x => x.F050302.ORD_NO).Distinct().ToList();

            _wmsLogHelper.AddRecord("產生配庫後揀貨資料");
            // 產生配庫後揀貨資料
            var f050306List = CreateF050306List(dcCode, gupCode, custCode, isB2B, f050301s, allotedStockOrders);

            _wmsLogHelper.AddRecord("扣除庫存 開始");
            // 扣除庫存
            var checkStock = _stockService.DeductStock(f050306List.Select(x => new OrderStockChange
            {
                DcCode = x.DC_CODE,
                GupCode = x.GUP_CODE,
                CustCode = x.CUST_CODE,
                WmsNo = x.WMS_NO,
                LocCode = x.PICK_LOC,
                ItemCode = x.ITEM_CODE,
                VaildDate = x.VALID_DATE,
                EnterDate = x.ENTER_DATE,
                MakeNo = x.MAKE_NO,
                SerialNo = x.SERIAL_NO,
                BoxCtrlNo = x.BOX_CTRL_NO,
                PalletCtrlNo = x.PALLET_CTRL_NO,
                VnrCode = x.VNR_CODE,
                Qty = x.B_PICK_QTY
            }).ToList());
            if (!checkStock.IsSuccessed)
            {
                var msgList = checkStock.Message.Split(Environment.NewLine.ToArray()).ToList();
                msgList.ForEach(msg =>
                {
                    _wmsLogHelper.AddRecord(msg);
                });
                // 商品庫存不足通知 訊息內容自訂
                AddLog(dcCode, gupCode, custCode, "AAM00016", checkStock.Message, true);
                _wmsLogHelper.AddRecord("扣除庫存 結束(庫存不足)");
                _wmsLogHelper.AddRecord("配庫處理結束");
                return false;
            }
            else
                _wmsLogHelper.AddRecord("扣除庫存 結束");

            if (isAutoAllocStock)
            {
                _wmsLogHelper.AddRecord("執行庫存異動 開始");
                _stockService.SaveChange();
                _wmsLogHelper.AddRecord("執行庫存異動 結束");
            }

            _wmsLogHelper.AddRecord("更新已配庫訂單狀態=1(已配庫) 開始");
            // 更新已配庫訂單狀態=1(已配庫)
            f050301s.ForEach(x =>
            {
                if (allotedOrdNos.Contains(x.ORD_NO))
                    x.PROC_FLAG = "1";
            });
            _wmsLogHelper.AddRecord("更新已配庫訂單狀態=1(已配庫) 結束");

            // 移除未配庫成功訂單
            _wmsLogHelper.AddRecord("移除未配庫訂單狀態=0(未配庫成功)的訂單 開始");
            var removeOrdNos = f050301s.Where(x => x.PROC_FLAG == "0").Select(x => x.ORD_NO).ToList();
            f050301s = f050301s.Where(x => !removeOrdNos.Contains(x.ORD_NO)).ToList();
            f050302s = f050302s.Where(x => !removeOrdNos.Contains(x.ORD_NO)).ToList();
            f05030201s = f05030201s.Where(x => !removeOrdNos.Contains(x.ORD_NO)).ToList();
            _wmsLogHelper.AddRecord("移除未配庫訂單狀態=0(未配庫成功)的訂單 結束");

            _wmsLogHelper.AddRecord("檢查是否有廠退出貨單 開始");
            var rtnShipWmsNos = f050301s.Where(x => x.SOURCE_TYPE == "13" && !string.IsNullOrWhiteSpace(x.SOURCE_NO)).Select(x => x.SOURCE_NO).ToList();
            if (rtnShipWmsNos.Any())
            {
                _wmsLogHelper.AddRecord("更新已配庫的廠退出貨單PROC_FLAG=1(已配庫) 開始");
                // 廠退出貨單，配庫後要將PROC_FLAG設為1(已配庫)
                var f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction);
                var f160204s = f160204Repo.GetDatasByRtnWmsNos(dcCode, gupCode, custCode, rtnShipWmsNos).ToList();
                f160204s.ForEach(x =>
                {
                    x.PROC_FLAG = "1"; //更新為已配庫
                });
                f160204Repo.BulkUpdate(f160204s);
                _wmsLogHelper.AddRecord("更新已配庫的廠退出貨單PROC_FLAG=1(已配庫) 結束");
            }
            _wmsLogHelper.AddRecord("檢查是否有廠退出貨單 結束");

            // 增加已配庫成功訂單數
            _finishAllocOrdCnt += allotedOrdNos.Count;

            _wmsLogHelper.AddRecord("整批寫入資料庫 開始");
            // 整批寫入資料庫
            _f050301Repo.BulkInsert(f050301s);
            _f050302Repo.BulkInsert(f050302s);
            _f05030201Repo.BulkInsert(f05030201s);
            if (isAutoAllocStock)
                _f050306Repo.BulkInsert(f050306List, "ID");
            else
            {
                _f050306s.AddRange(f050306List);
                _f050301s.AddRange(f050301s);
            }


            _wmsLogHelper.AddRecord("整批寫入資料庫 結束");

            _wmsLogHelper.AddRecord("配庫處理結束");
            return true;
        }

        #region 總庫檢查
        /// <summary>
        /// 倉別總庫檢查
        /// </summary>
        /// <param name="TypeId"></param>
        /// <param name="f050001s"></param>
        /// <param name="f050002s"></param>
        /// <returns></returns>
        private Dictionary<ItemMakeNoAndSerialNo, int> CheckTotalStocks(string dcCode, string gupCode, string custCode, string TypeId, List<F050001> f050001s, List<F050002> f050002s)
        {
            _wmsLogHelper.AddRecord(string.Format("[DcCode:{0} GupCode:{1} CustCode:{2} 倉庫型態:{3}]總庫檢查開始", dcCode, gupCode, custCode, TypeId));

            _wmsLogHelper.AddRecord("取得商品總庫存開始");
            var itemCodes = f050002s.Select(x => x.ITEM_CODE).Distinct().ToList();
            var stocks = GetItemMakeNoAndSerialNoTotalStockQtyWithVirtual(dcCode, gupCode, custCode, TypeId, itemCodes);
            _wmsLogHelper.AddRecord("取得商品總庫存結束");

            var makeNoUseQtys = new Dictionary<ItemMakeNo, int>();
            var notEnoughItems = new Dictionary<ItemMakeNoAndSerialNo, int>();
            // 群組條件: 如果訂單明細有指定序號 就不看批號
            var groupItems = f050002s.GroupBy(x => new { SerialNo = x.SERIAL_NO, MakeNo = !string.IsNullOrWhiteSpace(x.SERIAL_NO) ? null : x.MAKE_NO, ItemCode = x.ITEM_CODE }).OrderBy(x => x.Key.ItemCode).ThenByDescending(x => x.Key.SerialNo).ThenByDescending(x => x.Key.MakeNo);

            foreach (var item in groupItems)
            {
                if (!makeNoUseQtys.Keys.Any(x => x.ItemCode == item.Key.ItemCode && x.MakeNo == item.Key.MakeNo))
                {
                    makeNoUseQtys.Add(new ItemMakeNo { ItemCode = item.Key.ItemCode, MakeNo = item.Key.MakeNo }, 0);
                }

                var itemMakeNoAndSerialNo = new ItemMakeNoAndSerialNo { ItemCode = item.Key.ItemCode, MakeNo = item.Key.MakeNo, SerialNo = item.Key.SerialNo };
                _wmsLogHelper.AddRecord(string.Format("計算商品{0} {1} {2} 總庫檢查開始", item.Key.ItemCode, string.IsNullOrWhiteSpace(item.Key.MakeNo) ? "" : "批號:" + item.Key.MakeNo, string.IsNullOrWhiteSpace(item.Key.SerialNo) ? "" : "序號:" + item.Key.SerialNo));
                // 取得商品總庫存數 (訂單MakeNo為null會取出包含商品MakeNo != null的數量)
                int itemStockQty = 0;

                // 指定序號 就只看序號即可
                if (!string.IsNullOrEmpty(item.Key.SerialNo))
                {
                    itemStockQty = stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode && x.SERIAL_NO == item.Key.SerialNo).Sum(x => x.QTY);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.Key.MakeNo))
                    {
                        itemStockQty = stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode && x.MAKE_NO == item.Key.MakeNo).Sum(x => x.QTY);
                        // 指定批號庫存要扣除指定序號被使用的批號庫存
                        itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.Key.ItemCode && x.Key.MakeNo == item.Key.MakeNo).Sum(x => x.Value);
                    }
                    else
                    {
                        itemStockQty = stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode).Sum(x => x.QTY);
                        // 無批號明細因不論是否為有批號品項都可撿，因此須扣除已被有批號明細使用的數量
                        itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.Key.ItemCode).Sum(x => x.Value);
                    }
                }

                // 取得商品總訂購數量
                var orderQty = item.Sum(x => x.ORD_QTY);

                var makeNoUseQty = orderQty;
                if (itemStockQty < orderQty)
                { //庫存不足
                    notEnoughItems.Add(itemMakeNoAndSerialNo, orderQty - itemStockQty);
                    //MakeNo != null的商品部分，若不足以分配給所有的MakeNo != null的訂單，則所有商品都會被分配掉，所以 makeNoUseQty = itemStockQty
                    makeNoUseQty = itemStockQty;
                }
                else  //庫存足夠
                {
                    // 這裡是紀錄指定序號重複使用次數，並且將不足的品項+1
                    var serialNos = item.Where(x => !string.IsNullOrEmpty(x.SERIAL_NO)).Select(x => x.SERIAL_NO).ToList();
                    foreach (var sn in serialNos)
                    {
                        // 使用該序號次數 +1
                        if (!_usedAssignationSerials.ContainsKey(sn))
                            _usedAssignationSerials.Add(sn, 1);
                        else
                            _usedAssignationSerials[sn]++;

                        // 檢查指定序號有無重複配庫
                        if (_usedAssignationSerials[sn] == 1)
                            continue;
                        else
                        {
                            // 若重複配庫則將該序號商品不足數 +1
                            var existingKey = notEnoughItems.Keys.FirstOrDefault(k => k.ItemCode == itemMakeNoAndSerialNo.ItemCode && k.SerialNo == itemMakeNoAndSerialNo.SerialNo);
                            if (existingKey == null)
                            {
                                notEnoughItems.Add(itemMakeNoAndSerialNo, 1);
                            }
                            else
                                notEnoughItems[existingKey]++;
                        }
                    }
                }
                // 訂單明細有指定序號
                if (!string.IsNullOrEmpty(item.Key.SerialNo))
                {
                    // 有配到庫存
                    if (makeNoUseQty > 0)
                    {
                        //抓取該指定序號庫存
                        var stock = stocks.First(x => x.ITEM_CODE == item.Key.ItemCode && x.SERIAL_NO == item.Key.SerialNo);
                        // 用庫存的批號找之前有無該批號的已使用紀錄，如果有就已使用該批號數量+指定序號配庫數量 否則新增一筆已使用批號紀錄，數量為指定序號配庫數量
                        var findMakeNoUseQty = makeNoUseQtys.FirstOrDefault(x => x.Key.ItemCode == stock.ITEM_CODE && x.Key.MakeNo == stock.MAKE_NO);
                        if (findMakeNoUseQty.Equals(default(KeyValuePair<ItemMakeNo, int>)))
                        {
                            makeNoUseQtys.Add(new ItemMakeNo { ItemCode = stock.ITEM_CODE, MakeNo = stock.MAKE_NO }, makeNoUseQty);
                        }
                        else
                            makeNoUseQtys[findMakeNoUseQty.Key] += makeNoUseQty;
                    }
                }
                // 訂單明細有指定批號
                else if (!string.IsNullOrEmpty(item.Key.MakeNo))
                {
                    makeNoUseQtys[makeNoUseQtys.Keys.First(x => x.ItemCode == item.Key.ItemCode && x.MakeNo == item.Key.MakeNo)] += makeNoUseQty;
                }
                _wmsLogHelper.AddRecord(string.Format("計算商品{0} {1}總庫檢查結束", item.Key.ItemCode, string.IsNullOrEmpty(item.Key.MakeNo) ? "" : "批號:" + item.Key.MakeNo));

            }
            _wmsLogHelper.AddRecord(string.Format("[DcCode:{0} GupCode:{1} CustCode:{2} 倉庫型態:{3}]總庫檢查結束", dcCode, gupCode, custCode, TypeId));
            return notEnoughItems;
        }

        private List<ItemMakeNoTotalStockQty> GetItemMakeNoAndSerialNoTotalStockQtyWithVirtual(string dcCode, string gupCode, string custCode, string warehouseType, List<string> itemCodes)
        {
            var f1913Repo = new F1913Repository(Schemas.CoreSchema);
            var list = new List<ItemMakeNoTotalStockQty>();
            var maxBatchItemCnt = 200;
            var page = itemCodes.Count / maxBatchItemCnt + (itemCodes.Count % maxBatchItemCnt > 0 ? 1 : 0);
            for (var i = 0; i < page; i++)
            {
                var pageItemCodes = itemCodes.Skip(i * maxBatchItemCnt).Take(maxBatchItemCnt).ToList();
                var datas = f1913Repo.GetItemMakeNoAndSerialNoTotalStockQties(dcCode, gupCode, custCode, warehouseType, false, pageItemCodes).ToList();
                list.AddRange(datas);
            }
            return list;
        }

        /// <summary>
        /// 取得商品庫存數
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="warehouseType"></param>
        /// <param name="isGWithVirtual"></param>
        /// <param name="isIncludeResupply"></param>
        /// <returns></returns>
        private int GetItemStockQty(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, string makeNo, bool isGWithVirtual = true, bool isIncludeResupply = false)
        {
            var stock = 0;
            if (warehouseType == "G" && isGWithVirtual) //良品倉
            {
                //庫存需再加上補貨區及虛擬倉的庫存
                stock = _f1913Repo.GetItemStockWithVirtual(dcCode, gupCode, custCode, itemCode, warehouseType, false, makeNo);
            }
            else if (warehouseType == "G" && !isGWithVirtual && isIncludeResupply)
            {
                //庫存需再加上補貨區的庫存
                stock = _f1913Repo.GetItemStock(dcCode, gupCode, custCode, itemCode, warehouseType, true, makeNo);
            }
            else if (warehouseType == "D")
            {
                // 報廢倉，單純取得該商品存在於倉別的總庫存量，不會過濾效期
                stock = _f1913Repo.GetItemStock(dcCode, gupCode, custCode, itemCode, warehouseType, false, makeNo);
            }
            else
            {
                //庫存不包含補貨區及虛擬倉的庫存
                stock = _f1913Repo.GetItemStockWithoutResupply(dcCode, gupCode, custCode, itemCode, warehouseType, makeNo);
            }

            return stock;
        }

        #endregion

        #region 釋放缺品訂單
        /// <summary>
        /// 釋放缺品商品訂單
        /// </summary>
        /// <param name="notEnoughItems"></param>
        /// <param name="f050001s"></param>
        private List<string> ReleaseLackOrders(bool isB2B, F1909 f1909, Dictionary<ItemMakeNoAndSerialNo, int> notEnoughItems, List<F050001> f050001s, List<F050002> f050002s)
        {

            var releaseOrdNos = new List<string>();
            // B2C訂單或貨主設定允許B2B單張訂單出貨 移除庫存不足的訂單
            if (!isB2B || f1909.ISB2B_ALONE_OUT == "1")
            {
                _wmsLogHelper.AddRecord("釋放缺品商品訂單開始");
                var notEnoughItemsTmp = notEnoughItems.Select(a => new KeyValuePair<ItemMakeNoAndSerialNo, int>(a.Key, a.Value)).ToDictionary(a => a.Key, a => a.Value);

                #region 針對有指定序號缺貨的訂單進行釋放
                // 有指定序號缺貨清單
                var notEnoughItemSnList = notEnoughItemsTmp.Where(x => !string.IsNullOrWhiteSpace(x.Key.SerialNo)).Select(x => x.Key.SerialNo).ToList();
                if (notEnoughItemSnList.Any())
                {
                    // 有指定序號缺貨訂單編號清單
                    var noEnoughSnOrdNos = f050002s.Where(x => notEnoughItemSnList.Contains(x.SERIAL_NO)).Select(x => x.ORD_NO).Distinct().ToList();
                    if (noEnoughSnOrdNos.Any())
                    {
                        // 針對指定序號缺貨訂單先進行踢單(一單一品=>一單多品)
                        // 一單一品排序:品項數ASC,總訂購量ASC,訂單編號DESC
                        // 一單多品排序:品項數ASC,訂單編號DESC
                        var orders = (from o in noEnoughSnOrdNos
                                      join d in f050002s
                                      on new { ORD_NO = o } equals new { d.ORD_NO }
                                      select d).GroupBy(x => x.ORD_NO)
                                                    .Select(x => new { OrdNo = x.Key, ItemCnt = x.Select(y => y.ITEM_CODE).Distinct().Count(), Datas = x.ToList() })
                                                    .OrderBy(x => x.ItemCnt).ThenBy(x => x.ItemCnt == 1 ? x.Datas.Sum(y => y.ORD_QTY) : int.MaxValue).ThenByDescending(x => x.OrdNo).ToList();
                        foreach (var order in orders)
                        {
                            var isRelease = false;
                            foreach (var detail in order.Datas)
                            {
                                var notEnoughKey = string.IsNullOrWhiteSpace(detail.SERIAL_NO) ?
                                notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.ITEM_CODE && x.MakeNo == detail.MAKE_NO) :
                                notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.ITEM_CODE && x.SerialNo == detail.SERIAL_NO);
                                if (notEnoughKey != null)
                                {
                                    var notEnoughItemQty = notEnoughItemsTmp[notEnoughKey];
                                    if (notEnoughItemQty > 0)
                                    {
                                        if (notEnoughItemQty > detail.ORD_QTY)
                                        {
                                            notEnoughItemQty = notEnoughItemQty - detail.ORD_QTY;
                                            notEnoughItemsTmp[notEnoughKey] = notEnoughItemQty;
                                        }
                                        else
                                            notEnoughItemsTmp[notEnoughKey] = 0;

                                        isRelease = true;
                                    }
                                }
                            }
                            if (isRelease)
                                releaseOrdNos.Add(order.OrdNo);

                            if (notEnoughItemsTmp.All(x => x.Value == 0))
                                break;
                        }
                    }
                }

                #endregion

                #region 針對無指定序號缺貨訂單進行釋放

                if (notEnoughItemsTmp.Any(x => x.Value > 0))
                {
                    // 缺品清單
                    var notEnoughItemCodes = notEnoughItemsTmp.Where(x => string.IsNullOrWhiteSpace(x.Key.SerialNo) && x.Value > 0).Select(x => x.Key.ItemCode).Distinct().ToList();
                    // 取得有缺貨商品的訂單編號
                    var noEnoughItemOrdNos = (from o in f050001s
                                              join d in f050002s
                                              on o.ORD_NO equals d.ORD_NO
                                              join item in notEnoughItemCodes
                                              on d.ITEM_CODE equals item
                                              where !releaseOrdNos.Contains(o.ORD_NO)
                                              select o.ORD_NO).Distinct().ToList();

                    // 針對有缺貨訂單先進行踢單(一單一品=>一單多品)
                    // 一單一品排序:品項數ASC,總訂購量ASC,訂單編號DESC
                    // 一單多品排序:品項數ASC,訂單編號DESC
                    var orders = (from o in noEnoughItemOrdNos
                                  join d in f050002s
                                  on new { ORD_NO = o } equals new { d.ORD_NO }
                                  select d).GroupBy(x => x.ORD_NO)
                                                .Select(x => new { OrdNo = x.Key, ItemCnt = x.Select(y => y.ITEM_CODE).Distinct().Count(), Datas = x.ToList() })
                                                .OrderBy(x => x.ItemCnt).ThenBy(x => x.ItemCnt == 1 ? x.Datas.Sum(y => y.ORD_QTY) : int.MaxValue).ThenByDescending(x => x.OrdNo).ToList();

                    foreach (var order in orders)
                    {
                        var isRelease = false;
                        foreach (var detail in order.Datas)
                        {
                            var notEnoughKey = notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.ITEM_CODE && x.MakeNo == detail.MAKE_NO);

                            if (notEnoughKey != null)
                            {
                                var notEnoughItemQty = notEnoughItemsTmp[notEnoughKey];
                                if (notEnoughItemQty > 0)
                                {
                                    if (notEnoughItemQty > detail.ORD_QTY)
                                    {
                                        notEnoughItemQty = notEnoughItemQty - detail.ORD_QTY;
                                        notEnoughItemsTmp[notEnoughKey] = notEnoughItemQty;
                                    }
                                    else
                                        notEnoughItemsTmp[notEnoughKey] = 0;

                                    isRelease = true;
                                }
                            }
                        }
                        if (isRelease)
                            releaseOrdNos.Add(order.OrdNo);

                        if (notEnoughItemsTmp.All(x => x.Value == 0))
                            break;
                    }

                }

                #endregion

                _wmsLogHelper.AddRecord("釋放缺品商品訂單結束");
            }
            return releaseOrdNos;
        }

        /// <summary>
        ///  釋放B2B有條件缺品商品訂單
        /// </summary>
        /// <returns></returns>
        private List<string> ReleaseB2BConditionLackOrders(string dcCode, string gupCode, string custCode, F1909 f1909, List<F050001> f050001s, List<F050002> f050002s)
        {
            _wmsLogHelper.AddRecord("釋放B2B有條件缺品商品訂單開始");
            var releaseOrdNos = new List<string>();
            //貨主主檔設定判斷效期允出天數且不允許部分出貨
            if (f1909.ISALLOW_DELV_DAY == "1" && f1909.SPILT_OUTCHECK == "0")
            {
                _wmsLogHelper.AddRecord("不允許部分出貨且不符合商品效期允出天數 開始");
                var f1913Repo = new F1913Repository(Schemas.CoreSchema);
                var gwF050001s = f050001s.GroupBy(a => new { a.TYPE_ID });
                foreach (var gF050001 in gwF050001s)
                {
                    var wF050002s = (from a in f050002s
                                     join b in gF050001.AsEnumerable() on a.ORD_NO equals b.ORD_NO
                                     select a).ToList();
                    var itemCodes = (from a in wF050002s
                                     select a.ITEM_CODE).Distinct().ToList();
                    var pickLocPriorityInfos = f1913Repo.GetItemPickLocPriorityInfo(dcCode, gupCode, custCode, itemCodes, false, gF050001.Key.TYPE_ID).ToList();
                    foreach (var f050001 in gF050001)
                    {
                        var ordF050002s = f050002s.Where(a => a.ORD_NO == f050001.ORD_NO).ToList();
                        var tempPickLocPriorityInfos = new List<ItemLocPriorityInfo>();
                        pickLocPriorityInfos.ForEach((x) =>
                        {
                            ItemLocPriorityInfo y = new ItemLocPriorityInfo();
                            x.CloneProperties(y);
                            tempPickLocPriorityInfos.Add(y);
                        });

                        if (!CheckValidDate(f050001, ordF050002s, tempPickLocPriorityInfos))
                            releaseOrdNos.Add(f050001.ORD_NO);

                    }
                }
                _wmsLogHelper.AddRecord("不允許部分出貨且不符合商品效期允出天數 結束");
            }

            //允許部分出貨且依出車時段平均分配，若沒設出車時段則踢單
            if (f1909.SPILT_OUTCHECK == "1" && f1909.SPILT_OUTCHECKWAY == "1")
            {
                _wmsLogHelper.AddRecord("允許部分出貨且依出車時段平均分配，若沒設出車時段則踢單 開始");
                var noCarPeriodsOrds = ReleaseNoCarPeriodOrders(dcCode, gupCode, custCode, f050002s.ToList(), f050001s);
                releaseOrdNos.AddRange(noCarPeriodsOrds);
                _wmsLogHelper.AddRecord("允許部分出貨且依出車時段平均分配，若沒設出車時段則踢單 結束");
            }

            _wmsLogHelper.AddRecord("釋放B2B有條件缺品商品訂單結束");
            return releaseOrdNos;
        }

        /// <summary>
        /// 檢查訂單是否符合商品限制效期
        /// </summary>
        /// <param name="f050001">訂單主檔</param>
        /// <param name="f050002s">訂單明細</param>
        /// <returns></returns>
        private bool CheckValidDate(F050001 f050001, List<F050002> f050002s, List<ItemLocPriorityInfo> pickLocPriorityInfos)
        {
            var itemLimitValidDays = GetItemLimitValidDays(f050001.GUP_CODE, f050001.CUST_CODE, f050001.RETAIL_CODE, f050002s.Select(x => x.ITEM_CODE).Distinct().ToList()).ToList();
            var isEnougthQty = true;
            var message = new List<string>();
            if (itemLimitValidDays.Any())
            {
                var groupDetail = f050002s.GroupBy(x => x.ITEM_CODE);
                foreach (var detail in groupDetail)
                {
                    var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);

                    if (itemLimitValidDay != null)
                    {
                        var stockQty = pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).Sum(x => x.QTY);
                        var detailQty = detail.Sum(x => x.ORD_QTY);
                        //庫存數小於訂購數 此訂單不配庫
                        if (stockQty < detailQty)
                        {
                            //訂單編號:{0} 商品:{1} 符合允出天數的庫存不足
                            message.Add(string.Format(_commonService.GetMsg("AAM00024"), f050001.ORD_NO, detail.Key));
                            isEnougthQty = false;
                        }
                    }
                }
                if (message.Any())
                    AddLog(f050001.DC_CODE, f050001.GUP_CODE, f050001.CUST_CODE, "AAM00024", string.Join(Environment.NewLine, message), true);

                if (isEnougthQty)
                {
                    foreach (var detail in groupDetail)
                    {
                        var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);
                        var stocks = (itemLimitValidDay != null) ? pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).OrderBy(x => x.VALID_DATE) : pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key).OrderBy(x => x.VALID_DATE);
                        long detailQty = detail.Sum(x => x.ORD_QTY);
                        foreach (var st in stocks)
                        {
                            if (st.QTY < detailQty)
                            {
                                detailQty -= st.QTY;
                                st.QTY = 0;
                            }
                            else
                            {
                                st.QTY -= detailQty;
                                detailQty = 0;
                            }
                            if (detailQty == 0)
                                break;
                        }
                    }
                }
            }
            return isEnougthQty;
        }

        /// <summary>
        /// 釋放允許部分出貨但沒有設定門市出車時段B2B訂單 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="f050002s"></param>
        /// <param name="gF050001"></param>
        /// <returns></returns>
        private List<string> ReleaseNoCarPeriodOrders(string dcCode, string gupCode, string custCode, List<F050002> f050002s, List<F050001> gF050001)
        {
            var repF194716 = new F194716Repository(Schemas.CoreSchema, _wmsTransaction);
            var retailCodes = gF050001.AsEnumerable().Select(a => a.RETAIL_CODE).ToList();
            var retailCarPeriods = GetRetailCarPeriods(dcCode, gupCode, custCode, retailCodes);
            //允許部分出貨，依出貨時段
            var oOrdNos = from a in gF050001.AsEnumerable()
                          where !retailCarPeriods.Select(r => r.RETAIL_CODE).Contains(a.RETAIL_CODE)
                          select a.ORD_NO;

            return oOrdNos.ToList();
        }

        #endregion

        #region 複製訂單池訂單至配庫後訂單
        /// <summary>
        /// 複製訂單池訂單至配庫後訂單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="f050001s"></param>
        /// <param name="f050002s"></param>
        /// <param name="f050301s"></param>
        /// <param name="f050302s"></param>
        /// <returns></returns>
        private void CopyOrderToAllocOrders(string dcCode, string gupCode, string custCode, List<F050001> f050001s, List<F050002> f050002s, ref List<F050301> f050301s, ref List<F050302> f050302s, string priorityCode = null)
        {
            _wmsLogHelper.AddRecord("複製訂單池訂單至配庫後訂單 開始");
            var pickTempNo = DateTime.Now.ToString("yyMMddHHmmss") + "_" + _pickIndex.ToString("00");

            foreach (var f050001 in f050001s)
            {
                var f050301 = AutoMapper.Mapper.DynamicMap<F050301>(f050001);
                f050301.HELLO_LETTER = "0";
                f050301.PROC_FLAG = "0";
                f050301.PICK_TEMP_NO = pickTempNo;
                f050301.ORD_PROP = f050001.TRAN_CODE;
                //固定自取
                f050301.SELF_TAKE = "1";

                f050301.ORDER_CRT_DATE = f050001.CRT_DATE.Date;

                var ordF050302s = f050002s.Where(x => x.ORD_NO == f050001.ORD_NO).Select(x => AutoMapper.Mapper.DynamicMap<F050302>(x)).ToList();

                //總材積												 
                f050301.VOLUMN = GetTotalItemsVolumn(f050301.GUP_CODE, f050301.CUST_CODE, ordF050302s);
                //總重量
                f050301.WEIGHT = GetTotalItemsWeight(f050301.GUP_CODE, f050301.CUST_CODE, ordF050302s);

                // 指定到貨日未設定 預設D+1
                if (!f050301.ARRIVAL_DATE.HasValue)
                    f050301.ARRIVAL_DATE = DateTime.Today.AddDays(1);

                // 跨庫目的地名稱
                f050301.MOVE_OUT_TARGET = f050001.MOVE_OUT_TARGET;
                f050301.USER_DIRECT_PRIORITY_CODE = priorityCode;
                f050301.SUG_LOGISTIC_CODE = f050001.SUG_LOGISTIC_CODE;
                f050301.NP_FLAG = f050001.NP_FLAG;
                f050301s.Add(f050301);
                f050302s.AddRange(ordF050302s);
            }

            _wmsLogHelper.AddRecord("複製訂單池訂單至配庫後訂單 結束");

        }

        /// <summary>
        /// 取得訂單總材積
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="f050302s"></param>
        /// <returns></returns>
        private decimal GetTotalItemsVolumn(string gupCode, string custCode, List<F050302> f050302s)
        {
            var itemCodes = f050302s.Select(a => a.ITEM_CODE).Distinct().ToList();
            var f1905s = GetF1905s(gupCode, custCode, itemCodes);
            // 商品總體積
            var totalVolume = (from a in f1905s
                               join b in f050302s on a.ITEM_CODE equals b.ITEM_CODE
                               select new { a, b })
                                                                                                                                            .Sum(x => x.a.PACK_HIGHT * x.a.PACK_LENGTH * x.a.PACK_WIDTH * (x.b.ORD_QTY));
            return totalVolume;
        }

        /// <summary>
        /// 取得訂單總重量
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="f050302s"></param>
        /// <returns></returns>
        private decimal GetTotalItemsWeight(string gupCode, string custCode, List<F050302> f050302s)
        {
            var itemCodes = f050302s.Select(a => a.ITEM_CODE).Distinct().ToList();
            var f1905s = GetF1905s(gupCode, custCode, itemCodes);
            // 商品總重量
            var totalWeight = (from a in f1905s
                               join b in f050302s on a.ITEM_CODE equals b.ITEM_CODE
                               select new { a, b })
                                                                                                                                            .Sum(x => x.a.PACK_WEIGHT * (x.b.ORD_QTY));
            return totalWeight;
        }
        #endregion

        #region 單據配庫

        /// <summary>
        /// 執行單據配庫
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="isB2B"></param>
        /// <param name="sourceType"></param>
        /// <param name="f050301s"></param>
        /// <param name="f050302s"></param>
        private List<AllotedStockOrder> ExecAllotStockOrders(string dcCode, string gupCode, string custCode, List<F050301> f050301s, List<F050302> f050302s, bool isTrialCalculation = false, string calNo = null)
        {
            _wmsLogHelper.AddRecord("執行單據配庫 開始");
            var allotStockOrderDetails = (from m in f050301s
                                          join d in f050302s
                                          on new { m.DC_CODE, m.GUP_CODE, m.CUST_CODE, m.ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.ORD_NO }
                                          select new AllotStockOrderDetail
                                          {
                                              F050301 = m,
                                              F050302 = d,
                                              CarPeriod = "Z"
                                          }).ToList();
            if (!allotStockOrderDetails.Any())
                return new List<AllotedStockOrder>();

            var osap = new OrderStockCheckParam
            {
                DcCode = dcCode,
                GupCode = gupCode,
                CustCode = custCode,
                TicketId = f050301s.First().TICKET_ID,
                AllotStockOrderDetails = allotStockOrderDetails,
                IsTrialCalculation = isTrialCalculation,
                TrialCalculationNo = calNo
            };
            var orderStockCheckResult = NewOrderStockCheck(osap);
            var allotedStockOrders = NewOrderAllot(orderStockCheckResult);
            _wmsLogHelper.AddRecord("執行單據配庫 結束");
            return allotedStockOrders;
        }

        #region 新訂單庫存檢查

        /// <summary>
        /// 新訂單庫存檢查
        /// </summary>
        /// <param name="osap"></param>
        /// <returns></returns>
        public OrderStockCheckResult NewOrderStockCheck(OrderStockCheckParam osap)
        {
            _wmsLogHelper.AddRecord("訂單庫存檢查 開始");
            var result = new OrderStockCheckResult();
            result.DcCode = osap.DcCode;
            result.GupCode = osap.GupCode;
            result.CustCode = osap.CustCode;
            result.IsTrialCalculation = osap.IsTrialCalculation;
            result.TicketId = osap.TicketId;
            result.TrialCalculationNo = osap.TrialCalculationNo;
            result.ReleaseAllotStockOrderDetails = new List<AllotStockOrderDetail>();
            result.ItemNoEnougthList = new List<OrderItemOutOfStock>();
            result.CanAllotStockOrderDetails = new List<AllotStockOrderDetail>();
            //是否B2B訂單
            var isB2B = osap.AllotStockOrderDetails.First().F050301.ORD_TYPE == "0";
            result.IsB2B = isB2B;
            var f1909 = _commonService.GetCust(osap.GupCode, osap.CustCode);
            //if (result.IsTrialCalculation) // 試算時以允許部分出貨來計算
            //	f1909.SPILT_OUTCHECK = "1";
            result.F1909 = f1909;

            if (isB2B)
                //設定B2B訂單出貨時段
                SetCarPeriod(osap);

            //取得揀貨儲位庫存
            var pickLocStocks = GetPickLocStocks(osap);

            var gTypes = osap.AllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
            //依出貨倉別配庫
            foreach (var type in gTypes)
            {
                var typeList = type.ToList();
                //取得此出貨倉別揀貨庫存
                var typePickLocStocks = pickLocStocks[osap.DcCode + "_" + osap.GupCode + "_" + osap.CustCode + "_" + type.Key];
                //取得訂購商品在揀貨儲位總庫存缺貨資料
                var itemNoEnougthList = CheckItemTotalQtyEnough(typePickLocStocks, typeList, type.Key);

                //取得缺貨商品補貨區可補貨數量
                GetReSupplyStockQty(osap.DcCode, osap.GupCode, osap.CustCode, type.Key, ref itemNoEnougthList);
                //取得缺貨商品虛擬儲位可回復庫存
                GetVirtualStockQty(osap.DcCode, osap.GupCode, osap.CustCode, type.Key, ref itemNoEnougthList);

                itemNoEnougthList.ForEach(x =>
                {
                    result.ItemNoEnougthList.Add(AutoMapper.Mapper.DynamicMap<OrderItemOutOfStock>(x));
                });

                //釋放缺貨訂單(不允許部分出貨且B2C訂單或貨主允許B2B單張訂單出貨)
                if (f1909.SPILT_OUTCHECK == "0" && (!isB2B || f1909.ISB2B_ALONE_OUT == "1"))
                {
                    var releaseOrderNos = RelaseOutOfStockOrders(osap.DcCode, osap.GupCode, osap.CustCode, type.Key, ref itemNoEnougthList, ref typeList);
                    result.ReleaseAllotStockOrderDetails.AddRange(typeList.Where(x => releaseOrderNos.Contains(x.F050301.ORD_NO)));
                    typeList.RemoveAll(x => releaseOrderNos.Contains(x.F050301.ORD_NO));
                }

                result.CanAllotStockOrderDetails.AddRange(typeList);
            }
            result.PickLocStocks = pickLocStocks;
            //寫入庫存Log
            WriteStockLog(result);
            _wmsLogHelper.AddRecord("訂單庫存檢查 結束");
            return result;
        }


        #region 設定B2B訂單出貨時段
        /// <summary>
        /// 設定出貨時段
        /// </summary>
        /// <param name="osap"></param>
        private void SetCarPeriod(OrderStockCheckParam osap)
        {
            _wmsLogHelper.AddRecord("設定出貨時段 開始");

            var retailCarPeriods = GetRetailCarPeriods(osap.DcCode, osap.GupCode, osap.CustCode, osap.AllotStockOrderDetails.Select(x => x.F050301.RETAIL_CODE).Distinct().ToList());
            osap.AllotStockOrderDetails.ForEach(x =>
            {
                var retailCarPeriod = retailCarPeriods.FirstOrDefault(y => y.RETAIL_CODE == x.F050301.RETAIL_CODE);
                x.CarPeriod = retailCarPeriod == null ? "Z" : retailCarPeriod.CAR_PERIOD;
            });
            _wmsLogHelper.AddRecord("設定出貨時段 開始");

        }
        #endregion

        #region 釋放缺貨訂單
        /// <summary>
        /// 釋放缺貨訂單
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="typeId">出貨倉別代碼</param>
        /// <param name="itemNoEnoughList">缺貨清單</param>
        /// <param name="allotStockOrderDetails"></param>
        private List<string> RelaseOutOfStockOrders(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnoughList, ref List<AllotStockOrderDetail> allotStockOrderDetails)
        {
            _wmsLogHelper.AddRecord("釋放缺貨訂單 開始");
            var releaseOrdNos = new List<string>();
            //非配庫試算且有商品缺貨
            if (itemNoEnoughList.Any())
            {
                var notEnoughItemsTmp = itemNoEnoughList.GroupBy(x => new { x.ItemCode, x.MakeNo, x.SerialNo })
                    .Select(a => new KeyValuePair<ItemMakeNoAndSerialNo, List<OrderItemOutOfStock>>
                    (new ItemMakeNoAndSerialNo { ItemCode = a.Key.ItemCode, MakeNo = a.Key.MakeNo, SerialNo = a.Key.SerialNo }, a.ToList()))
                    .ToDictionary(a => a.Key, a => a.Value);

                #region 針對有指定序號缺貨的訂單進行釋放
                // 有指定序號缺貨清單
                var notEnoughItemSnList = notEnoughItemsTmp.Where(x => !string.IsNullOrWhiteSpace(x.Key.SerialNo)).Select(x => x.Key.SerialNo).ToList();
                if (notEnoughItemSnList.Any())
                {
                    // 有指定序號缺貨訂單編號清單
                    var noEnoughSnOrdNos = allotStockOrderDetails.Where(x => notEnoughItemSnList.Contains(x.F050302.SERIAL_NO)).Select(x => x.F050301.ORD_NO).Distinct().ToList();
                    if (noEnoughSnOrdNos.Any())
                    {
                        // 針對指定序號缺貨訂單先進行踢單(一單一品=>一單多品)
                        // 一單一品排序:品項數ASC,總訂購量ASC,訂單編號DESC
                        // 一單多品排序:品項數ASC,訂單編號DESC
                        var orders = (from o in noEnoughSnOrdNos
                                      join d in allotStockOrderDetails
                                      on new { ORD_NO = o } equals new { d.F050301.ORD_NO }
                                      select d).GroupBy(x => x.F050301.ORD_NO)
                                                    .Select(x => new { OrdNo = x.Key, ItemCnt = x.Select(y => y.F050302.ITEM_CODE).Distinct().Count(), Datas = x.ToList() })
                                                    .OrderBy(x => x.ItemCnt).ThenBy(x => x.ItemCnt == 1 ? x.Datas.Sum(y => y.F050302.ORD_QTY) : int.MaxValue).ThenByDescending(x => x.OrdNo).ToList();
                        foreach (var order in orders)
                        {
                            var isRelease = false;
                            foreach (var detail in order.Datas)
                            {
                                var notEnoughKey = string.IsNullOrWhiteSpace(detail.F050302.SERIAL_NO) ?
                                notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.F050302.ITEM_CODE && x.MakeNo == detail.F050302.MAKE_NO) :
                                notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.F050302.ITEM_CODE && x.SerialNo == detail.F050302.SERIAL_NO);
                                if (notEnoughKey != null)
                                {
                                    var notEnoughItemQty = notEnoughItemsTmp[notEnoughKey].Sum(x => x.OutStockQty);
                                    if (notEnoughItemQty > 0)
                                    {
                                        var releaseQty = detail.F050302.ORD_QTY;
                                        foreach (var notEnoughItem in notEnoughItemsTmp[notEnoughKey])
                                        {
                                            if (notEnoughItem.OutStockQty > releaseQty)
                                            {
                                                notEnoughItem.OutStockQty -= releaseQty;
                                                break;
                                            }
                                            else
                                            {
                                                releaseQty -= notEnoughItem.OutStockQty;
                                                notEnoughItem.OutStockQty = 0;
                                            }
                                        }
                                        isRelease = true;
                                    }
                                }
                            }
                            if (isRelease)
                                releaseOrdNos.Add(order.OrdNo);

                            if (notEnoughItemsTmp.All(x => x.Value.Sum(y => y.OutStockQty) == 0))
                                break;
                        }
                    }
                }

                #endregion

                #region 針對無指定序號缺貨訂單進行釋放

                if (notEnoughItemsTmp.Any(x => x.Value.Sum(y => y.OutStockQty) > 0))
                {
                    // 缺品清單
                    var notEnoughItemCodes = notEnoughItemsTmp.Where(x => string.IsNullOrWhiteSpace(x.Key.SerialNo) && x.Value.Sum(y => y.OutStockQty) > 0).Select(x => x.Key.ItemCode).Distinct().ToList();
                    // 取得有缺貨商品的訂單編號
                    var noEnoughItemOrdNos = (from d in allotStockOrderDetails
                                              join item in notEnoughItemCodes
                                              on d.F050302.ITEM_CODE equals item
                                              where !releaseOrdNos.Contains(d.F050301.ORD_NO)
                                              select d.F050301.ORD_NO).Distinct().ToList();

                    // 針對有缺貨訂單先進行踢單(一單一品=>一單多品)
                    // 一單一品排序:品項數ASC,總訂購量ASC,訂單編號DESC
                    // 一單多品排序:品項數ASC,訂單編號DESC
                    var orders = (from o in noEnoughItemOrdNos
                                  join d in allotStockOrderDetails
                                  on new { ORD_NO = o } equals new { d.F050301.ORD_NO }
                                  select d).GroupBy(x => x.F050301.ORD_NO)
                                                .Select(x => new { OrdNo = x.Key, ItemCnt = x.Select(y => y.F050302.ITEM_CODE).Distinct().Count(), Datas = x.ToList() })
                                                .OrderBy(x => x.ItemCnt).ThenBy(x => x.ItemCnt == 1 ? x.Datas.Sum(y => y.F050302.ORD_QTY) : int.MaxValue).ThenByDescending(x => x.OrdNo).ToList();

                    foreach (var order in orders)
                    {
                        var isRelease = false;
                        foreach (var detail in order.Datas)
                        {
                            var notEnoughKey = notEnoughItemsTmp.Keys.FirstOrDefault(x => x.ItemCode == detail.F050302.ITEM_CODE && x.MakeNo == detail.F050302.MAKE_NO);

                            if (notEnoughKey != null)
                            {
                                var notEnoughItemQty = notEnoughItemsTmp[notEnoughKey].Sum(x => x.OutStockQty);
                                if (notEnoughItemQty > 0)
                                {
                                    var releaseQty = detail.F050302.ORD_QTY;
                                    foreach (var notEnoughItem in notEnoughItemsTmp[notEnoughKey])
                                    {
                                        if (notEnoughItem.OutStockQty > releaseQty)
                                        {
                                            notEnoughItem.OutStockQty -= releaseQty;
                                            break;
                                        }
                                        else
                                        {
                                            releaseQty -= notEnoughItem.OutStockQty;
                                            notEnoughItem.OutStockQty = 0;
                                        }
                                    }
                                    isRelease = true;
                                }
                            }
                            if (isRelease)
                                releaseOrdNos.Add(order.OrdNo);

                            if (notEnoughItemsTmp.All(x => x.Value.Sum(y => y.OutStockQty) == 0))
                                break;
                        }

                    }
                }

                #endregion
            }
            _wmsLogHelper.AddRecord("釋放缺貨訂單 結束");
            return releaseOrdNos;
        }
        #endregion

        #region 取得各出貨倉別揀貨儲位數量
        private Dictionary<string, List<ItemLocPriorityInfo>> _pickLocStocks;
        /// <summary>
        /// 取得各出貨倉別揀貨儲位數量
        /// </summary>
        /// <param name="osap"></param>
        /// <returns></returns>
        private Dictionary<string, List<ItemLocPriorityInfo>> GetPickLocStocks(OrderStockCheckParam osap)
        {
            _wmsLogHelper.AddRecord("取得各出貨倉別揀貨儲位數量 開始");

            if (_pickLocStocks == null)
                _pickLocStocks = new Dictionary<string, List<ItemLocPriorityInfo>>();
            //依出貨倉別取得揀貨儲位庫存
            var gTypes = osap.AllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
            foreach (var type in gTypes)
            {
                var item = _pickLocStocks.FirstOrDefault(x => x.Key == osap.DcCode + "_" + osap.GupCode + "_" + osap.CustCode + "_" + type.Key);
                if (item.Equals(default(KeyValuePair<string, List<ItemLocPriorityInfo>>)))
                {
                    var pickLocPriorityInfos = _f1913Repo.GetItemPickLocPriorityInfo(osap.DcCode, osap.GupCode, osap.CustCode, new List<string>(), false, type.Key).Where(a => a.QTY > 0).ToList();
                    _pickLocStocks.Add(osap.DcCode + "_" + osap.GupCode + "_" + osap.CustCode + "_" + type.Key, pickLocPriorityInfos);
                }
            }
            _wmsLogHelper.AddRecord("取得各出貨倉別揀貨儲位數量 結束");
            return _pickLocStocks;
        }

        #endregion

        #region 取得各出貨倉別補貨儲位數量
        private Dictionary<string, List<ItemLocPriorityInfo>> _reSupplyLocStocks;
        /// <summary>
        /// 取得各出貨倉別補貨儲位數量
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="typeId">出貨倉別</param>
        /// <returns></returns>
        public List<ItemLocPriorityInfo> GetReSupplyLocStocks(string dcCode, string gupCode, string custCode, string typeId)
        {
            if (_reSupplyLocStocks == null)
                _reSupplyLocStocks = new Dictionary<string, List<ItemLocPriorityInfo>>();
            var item = _reSupplyLocStocks.FirstOrDefault(x => x.Key == dcCode + "_" + gupCode + "_" + custCode + "_" + typeId);
            if (item.Equals(default(KeyValuePair<string, List<ItemLocPriorityInfo>>)))
            {
                var pickLocPriorityInfos = _f1913Repo.GetItemResupplyLocPriorityInfo(dcCode, gupCode, custCode, new List<string>(), false, typeId).Where(a => a.QTY > 0).ToList();
                _reSupplyLocStocks.Add(dcCode + "_" + gupCode + "_" + custCode + "_" + typeId, pickLocPriorityInfos);
            }
            return _reSupplyLocStocks[dcCode + "_" + gupCode + "_" + custCode + "_" + typeId];
        }

        #endregion

        #region 取得訂購商品在揀貨儲位總庫存缺貨資料
        /// <summary>
        /// 取得訂購商品在揀貨儲位總庫存缺貨資料
        /// </summary>
        /// <param name="stocks">出貨倉別庫存</param>
        /// <param name="details">訂單明細</param>
        private List<OrderItemOutOfStock> CheckItemTotalQtyEnough(List<ItemLocPriorityInfo> stocks, List<AllotStockOrderDetail> details, string typeId)
        {
            _wmsLogHelper.AddRecord("取得訂購商品在揀貨儲位總庫存缺貨資料 開始");

            var makeNoUseQtys = new Dictionary<ItemMakeNo, int>();
            var ItemNoEnoughList = new List<OrderItemOutOfStock>();
            // 依ItemCode及MakeNo及SerialNo分群檢查庫存數[檢查順序=>指定序號、指定批號、品號]
            var gOrderItems = details.GroupBy(x => new { SerialNo = x.F050302.SERIAL_NO, MakeNo = !string.IsNullOrWhiteSpace(x.F050302.SERIAL_NO) ? null : x.F050302.MAKE_NO, ItemCode = x.F050302.ITEM_CODE }).OrderBy(x => x.Key.ItemCode).ThenByDescending(x => x.Key.SerialNo).ThenByDescending(x => x.Key.MakeNo);

            foreach (var item in gOrderItems)
            {
                if (!makeNoUseQtys.Keys.Any(x => x.ItemCode == item.Key.ItemCode && x.MakeNo == item.Key.MakeNo))
                {
                    makeNoUseQtys.Add(new ItemMakeNo { ItemCode = item.Key.ItemCode, MakeNo = item.Key.MakeNo }, 0);
                }

                int itemStockQty = 0;

                // 指定序號 就只看序號即可
                if (!string.IsNullOrEmpty(item.Key.SerialNo))
                {
                    itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode && x.SERIAL_NO == item.Key.SerialNo).Sum(x => x.QTY);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.Key.MakeNo))
                    {
                        itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode && x.MAKE_NO == item.Key.MakeNo).Sum(x => x.QTY);
                        // 指定批號庫存要扣除指定序號被使用的批號庫存
                        itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.Key.ItemCode && x.Key.MakeNo == item.Key.MakeNo).Sum(x => x.Value);
                    }
                    else
                    {
                        itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.Key.ItemCode).Sum(x => x.QTY);
                        // 無批號明細因不論是否為有批號品項都可撿，因此須扣除已被有批號明細使用的數量
                        itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.Key.ItemCode).Sum(x => x.Value);
                    }
                }

                var ordQty = item.Sum(x => x.F050302.ORD_QTY);
                var makeNoUseQty = ordQty;
                // 庫存不足
                if (ordQty > itemStockQty)
                {
                    ItemNoEnoughList.Add(new OrderItemOutOfStock { TypeId = typeId, ItemCode = item.Key.ItemCode, MakeNo = item.Key.MakeNo, SerialNo = item.Key.SerialNo, OrderQty = ordQty, PickStockQty = itemStockQty, OutStockQty = ordQty - itemStockQty });
                    //MakeNo != null的商品部分，若不足以分配給所有的MakeNo != null的訂單，則所有商品都會被分配掉，所以 makeNoUseQty = itemStockQty
                    makeNoUseQty = itemStockQty;
                }
                else //庫存足夠
                {
                    // 這裡是紀錄指定序號重複使用次數，並且將不足的品項+1
                    var serialNos = item.Where(x => !string.IsNullOrEmpty(x.F050302.SERIAL_NO)).Select(x => x.F050302.SERIAL_NO).ToList();
                    foreach (var sn in serialNos)
                    {
                        // 使用該序號次數 +1
                        if (!_pickUsedAssignationSerials.ContainsKey(sn))
                            _pickUsedAssignationSerials.Add(sn, 1);
                        else
                            _pickUsedAssignationSerials[sn]++;

                        // 檢查指定序號有無重複配庫
                        if (_pickUsedAssignationSerials[sn] == 1)
                            continue;
                        else
                        {
                            // 若重複配庫則將該序號商品不足數 +1
                            var existingKey = ItemNoEnoughList.FirstOrDefault(k => k.ItemCode == item.Key.ItemCode && k.SerialNo == item.Key.SerialNo);
                            if (existingKey == null)
                            {
                                ItemNoEnoughList.Add(new OrderItemOutOfStock { TypeId = typeId, ItemCode = item.Key.ItemCode, MakeNo = item.Key.MakeNo, SerialNo = item.Key.SerialNo, OrderQty = ordQty, PickStockQty = 0, OutStockQty = 1 });
                            }
                            else
                                existingKey.OutStockQty++;
                        }
                    }
                }
                // 訂單明細有指定序號
                if (!string.IsNullOrEmpty(item.Key.SerialNo))
                {
                    // 有配到庫存
                    if (makeNoUseQty > 0)
                    {
                        //抓取該指定序號庫存
                        var stock = stocks.First(x => x.ITEM_CODE == item.Key.ItemCode && x.SERIAL_NO == item.Key.SerialNo);
                        // 用庫存的批號找之前有無該批號的已使用紀錄，如果有就已使用該批號數量+指定序號配庫數量 否則新增一筆已使用批號紀錄，數量為指定序號配庫數量
                        var findMakeNoUseQty = makeNoUseQtys.FirstOrDefault(x => x.Key.ItemCode == stock.ITEM_CODE && x.Key.MakeNo == stock.MAKE_NO);
                        if (findMakeNoUseQty.Equals(default(KeyValuePair<ItemMakeNo, int>)))
                        {
                            makeNoUseQtys.Add(new ItemMakeNo { ItemCode = stock.ITEM_CODE, MakeNo = stock.MAKE_NO }, makeNoUseQty);
                        }
                        else
                            makeNoUseQtys[findMakeNoUseQty.Key] += makeNoUseQty;
                    }
                }
                // 訂單明細有指定批號
                else if (!string.IsNullOrEmpty(item.Key.MakeNo))
                {
                    makeNoUseQtys[makeNoUseQtys.Keys.First(x => x.ItemCode == item.Key.ItemCode && x.MakeNo == item.Key.MakeNo)] += makeNoUseQty;
                }
            }
            _wmsLogHelper.AddRecord("取得訂購商品在揀貨儲位總庫存缺貨資料 結束");

            return ItemNoEnoughList;
        }

        #endregion

        #region 取得缺貨商品補貨區庫存

        /// <summary>
        /// 取得缺貨商品補貨區庫存
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="typeId">出貨倉別代碼</param>
        /// <param name="itemNoEnougthList">缺貨清單</param>
        /// <returns></returns>
        private void GetReSupplyStockQty(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnougthList)
        {
            _wmsLogHelper.AddRecord("取得缺貨商品補貨區庫存 開始");

            if (itemNoEnougthList.Any())
            {
                var makeNoUseQtys = new Dictionary<ItemMakeNo, int>();
                var makeNoStockQtys = new Dictionary<ItemMakeNo, int>();
                var stocks = GetReSupplyLocStocks(dcCode, gupCode, custCode, typeId).Where(a => a.QTY > 0).ToList();
                itemNoEnougthList = itemNoEnougthList.OrderBy(x => x.ItemCode).ThenByDescending(x => x.SerialNo).ThenByDescending(x => x.MakeNo).ToList();

                foreach (var item in itemNoEnougthList)
                {
                    if (!makeNoUseQtys.Keys.Any(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo))
                    {
                        makeNoUseQtys.Add(new ItemMakeNo { ItemCode = item.ItemCode, MakeNo = item.MakeNo }, 0);
                    }
                    if (!makeNoStockQtys.Keys.Any(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo))
                    {
                        makeNoStockQtys.Add(new ItemMakeNo { ItemCode = item.ItemCode, MakeNo = item.MakeNo }, 0);
                    }

                    //商品補貨區數量
                    int itemStockQty = 0;
                    // 指定序號 就只看序號即可
                    if (!string.IsNullOrEmpty(item.SerialNo))
                    {
                        itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.ItemCode && x.SERIAL_NO == item.SerialNo).Sum(x => x.QTY);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.MakeNo))
                        {
                            itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.ItemCode && x.MAKE_NO == item.MakeNo).Sum(x => x.QTY);
                            // 指定批號庫存要扣除指定序號被使用的批號庫存
                            itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.ItemCode && x.Key.MakeNo == item.MakeNo).Sum(x => x.Value);
                        }
                        else
                        {
                            itemStockQty = (int)stocks.Where(x => x.ITEM_CODE == item.ItemCode).Sum(x => x.QTY);
                            // 無批號明細因不論是否為有批號品項都可撿，因此須扣除已被有批號明細使用的數量
                            itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.ItemCode).Sum(x => x.Value);
                        }
                    }
                    item.ReSupplyStockQty = itemStockQty;

                    var makeNoUseQty = item.OutStockQty;
                    if (item.OutStockQty >= itemStockQty)
                    {
                        makeNoUseQty = itemStockQty;
                    }
                    item.SuggestReSupplyStockQty = makeNoUseQty;


                    // 有指定序號
                    if (!string.IsNullOrEmpty(item.SerialNo))
                    {
                        // 有配到庫存
                        if (makeNoUseQty > 0)
                        {
                            //抓取該指定序號庫存
                            var stock = stocks.First(x => x.ITEM_CODE == item.ItemCode && x.SERIAL_NO == item.SerialNo);
                            // 用庫存的批號找之前有無該批號的已使用紀錄，如果有就已使用該批號數量+指定序號配庫數量 否則新增一筆已使用批號紀錄，數量為指定序號配庫數量
                            var findMakeNoUseQty = makeNoUseQtys.FirstOrDefault(x => x.Key.ItemCode == stock.ITEM_CODE && x.Key.MakeNo == stock.MAKE_NO);
                            if (findMakeNoUseQty.Equals(default(KeyValuePair<ItemMakeNo, int>)))
                            {
                                makeNoUseQtys.Add(new ItemMakeNo { ItemCode = stock.ITEM_CODE, MakeNo = stock.MAKE_NO }, makeNoUseQty);
                            }
                            else
                                makeNoUseQtys[findMakeNoUseQty.Key] += makeNoUseQty;
                        }
                    }
                    // 訂單明細有指定批號
                    else if (!string.IsNullOrEmpty(item.MakeNo))
                    {
                        makeNoUseQtys[makeNoUseQtys.Keys.First(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo)] += makeNoUseQty;
                    }
                }
            }
            _wmsLogHelper.AddRecord("取得缺貨商品補貨區庫存 結束");

        }

        #endregion

        #region 取得缺貨商品虛擬儲位可回復庫存
        /// <summary>
        /// 取得缺貨商品虛擬儲位可回復庫存
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="typeId">出貨倉別代碼</param>
        /// <param name="itemNoEnougthList">缺貨清單</param>
        /// <returns></returns>
        private void GetVirtualStockQty(string dcCode, string gupCode, string custCode, string typeId, ref List<OrderItemOutOfStock> itemNoEnougthList)
        {
            _wmsLogHelper.AddRecord("取得缺貨商品虛擬儲位可回復庫存 開始");
            if (itemNoEnougthList.Any())
            {
                var makeNoUseQtys = new Dictionary<ItemMakeNo, int>();
                var makeNoStockQtys = new Dictionary<ItemMakeNo, int>();
                var stocks = _f1913Repo.GetVirtualQtyItems(dcCode, gupCode, custCode, typeId, itemNoEnougthList.Where(x => x.OutStockQty > 0).Select(x => x.ItemCode).ToList()).ToList();
                itemNoEnougthList = itemNoEnougthList.OrderBy(x => x.ItemCode).ThenByDescending(x => x.SerialNo).ThenByDescending(x => x.MakeNo).ToList();

                foreach (var item in itemNoEnougthList)
                {
                    if (!makeNoUseQtys.Keys.Any(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo))
                    {
                        makeNoUseQtys.Add(new ItemMakeNo { ItemCode = item.ItemCode, MakeNo = item.MakeNo }, 0);
                    }
                    if (!makeNoStockQtys.Keys.Any(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo))
                    {
                        makeNoStockQtys.Add(new ItemMakeNo { ItemCode = item.ItemCode, MakeNo = item.MakeNo }, 0);
                    }

                    //商品補貨區數量
                    int itemStockQty = 0;
                    // 指定序號 就只看序號即可
                    if (!string.IsNullOrEmpty(item.SerialNo))
                    {
                        itemStockQty = (int)stocks.Where(x => x.ItemCode == item.ItemCode && x.SerialNo == item.SerialNo).Sum(x => x.Qty);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.MakeNo))
                        {
                            itemStockQty = (int)stocks.Where(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo).Sum(x => x.Qty);
                            // 指定批號庫存要扣除指定序號被使用的批號庫存
                            itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.ItemCode && x.Key.MakeNo == item.MakeNo).Sum(x => x.Value);
                        }
                        else
                        {
                            itemStockQty = (int)stocks.Where(x => x.ItemCode == item.ItemCode).Sum(x => x.Qty);
                            // 無批號明細因不論是否為有批號品項都可撿，因此須扣除已被有批號明細使用的數量
                            itemStockQty = itemStockQty - makeNoUseQtys.Where(x => x.Key.ItemCode == item.ItemCode).Sum(x => x.Value);
                        }
                    }
                    item.VirtualStockQty = itemStockQty;

                    var makeNoUseQty = item.OutStockQty - item.SuggestReSupplyStockQty;
                    if (item.OutStockQty - item.SuggestReSupplyStockQty >= itemStockQty)
                    {
                        makeNoUseQty = itemStockQty;
                    }
                    item.SuggestVirtualStockQty = makeNoUseQty;


                    // 有指定序號
                    if (!string.IsNullOrEmpty(item.SerialNo))
                    {
                        // 有配到庫存
                        if (makeNoUseQty > 0)
                        {
                            //抓取該指定序號庫存
                            var stock = stocks.First(x => x.ItemCode == item.ItemCode && x.SerialNo == item.SerialNo);
                            // 用庫存的批號找之前有無該批號的已使用紀錄，如果有就已使用該批號數量+指定序號配庫數量 否則新增一筆已使用批號紀錄，數量為指定序號配庫數量
                            var findMakeNoUseQty = makeNoUseQtys.FirstOrDefault(x => x.Key.ItemCode == stock.ItemCode && x.Key.MakeNo == stock.MakeNo);
                            if (findMakeNoUseQty.Equals(default(KeyValuePair<ItemMakeNo, int>)))
                            {
                                makeNoUseQtys.Add(new ItemMakeNo { ItemCode = stock.ItemCode, MakeNo = stock.MakeNo }, makeNoUseQty);
                            }
                            else
                                makeNoUseQtys[findMakeNoUseQty.Key] += makeNoUseQty;
                        }
                    }
                    // 訂單明細有指定批號
                    else if (!string.IsNullOrEmpty(item.MakeNo))
                    {
                        makeNoUseQtys[makeNoUseQtys.Keys.First(x => x.ItemCode == item.ItemCode && x.MakeNo == item.MakeNo)] += makeNoUseQty;
                    }
                }
            }
            _wmsLogHelper.AddRecord("取得缺貨商品虛擬儲位可回復庫存 結束");

        }

        #endregion

        #region 寫入庫存Log
        private void WriteStockLog(OrderStockCheckResult oscr)
        {
            _wmsLogHelper.AddRecord("寫入庫存Log 開始");
            var f1901 = _commonService.GetDc(oscr.DcCode);
            var f1929 = _commonService.GetGup(oscr.GupCode);
            var f1909 = _commonService.GetCust(oscr.GupCode, oscr.CustCode);
            if (oscr.IsTrialCalculation)
            {
                var f050805Repo = new F050805Repository(Schemas.CoreSchema, _wmsTransaction);
                var addF050805List = oscr.ItemNoEnougthList.GroupBy(a => new { a.ItemCode, a.MakeNo, a.SerialNo }).Select(x => new F050805
                {
                    DC_CODE = oscr.DcCode,
                    GUP_CODE = oscr.GupCode,
                    CUST_CODE = oscr.CustCode,
                    CAL_NO = oscr.TrialCalculationNo,
                    TYPE_ID = x.First().TypeId,
                    ITEM_CODE = x.First().ItemCode,
                    TTL_ORD_QTY = x.Sum(y => y.OrderQty),
                    TTL_PICK_STOCK_QTY = x.Sum(y => y.PickStockQty),
                    TTL_RESUPPLY_STOCK_QTY = x.Sum(y => y.ReSupplyStockQty),
                    TTL_VIRTUAL_STOCK_QTY = x.Sum(y => y.VirtualStockQty),
                    TTL_OUTSTOCK_QTY = x.Sum(y => y.OutStockQty),
                    SUG_RESUPPLY_STOCK_QTY = x.Sum(y => y.SuggestReSupplyStockQty),
                    SUG_VIRTUAL_STOCK_QTY = x.Sum(y => y.SuggestVirtualStockQty),
                    MAKE_NO = x.Key.MakeNo,
                    SERIAL_NO = x.Key.SerialNo
                });
                f050805Repo.BulkInsert(addF050805List, new string[] { "ID" });
            }
            else
            {
                var messageList = new Dictionary<string, List<string>>();

                #region 產生虛擬儲位回復Log
                var virtualItems = oscr.ItemNoEnougthList.Where(x => x.SuggestVirtualStockQty > 0).ToList();
                var virtualItemMsgList = new List<string>();
                foreach (var item in virtualItems)
                {
                    var f198001 = GetF198001(item.TypeId);
                    var f1903 = _commonService.GetProduct(oscr.GupCode, oscr.CustCode, item.ItemCode);
                    //物流中心:「{0}」業主:「{1}」貨主:「{2}」倉別:「{3}」商品:「{4}」需回復虛擬儲位數量:{5}
                    virtualItemMsgList.Add(string.Format(_commonService.GetMsg("AAM00012"),
                                                    (f1901 == null) ? "" : f1901.DC_NAME,
                                                    (f1929 == null) ? "" : f1929.GUP_NAME,
                                                    (f1909 == null) ? "" : f1909.SHORT_NAME,
                                                    (f198001 == null) ? "" : f198001.TYPE_NAME,
                                                    (f1903 == null) ? "" : f1903.ITEM_CODE + f1903.ITEM_NAME,
                                                    item.SuggestVirtualStockQty));
                }
                if (virtualItemMsgList.Any())
                    messageList.Add("AAM00012", virtualItemMsgList);
                #endregion

                #region 良品倉揀貨區庫存不足Log
                //不允許部分出貨
                if (oscr.F1909.SPILT_OUTCHECK == "0")
                {
                    var pickStockMsgList = new List<string>();
                    List<ItemNeedQtyModel> itemNeedQtyModels = new List<ItemNeedQtyModel>();

                    var pickItemNoEnoughList = oscr.ItemNoEnougthList.Where(x => x.TypeId == "G").ToList();
                    foreach (var item in pickItemNoEnoughList)
                    {
                        var f1903 = _commonService.GetProduct(oscr.GupCode, oscr.CustCode, item.ItemCode);
                        //物流中心:「{0}」業主:「{1}」貨主:「{2}」商品:「{3}」揀區庫存不足{4}，請執行配庫試算產生補貨調撥單
                        //pickStockMsgList.Add(string.Format(_commonService.GetMsg("AAM00025"),
                        //				(f1901 == null) ? "" : f1901.DC_NAME,
                        //				(f1929 == null) ? "" : f1929.GUP_NAME,
                        //				(f1909 == null) ? "" : f1909.SHORT_NAME,
                        //				(f1903 == null) ? "" : f1903.ITEM_CODE + f1903.ITEM_NAME
                        //				, item.OutStockQty));

                        _itemNeedQtyModels.Add(new ItemNeedQtyModel()
                        {
                            ItemCode = f1903.ITEM_CODE,
                            MakeNo = item.MakeNo,
                            SerialNo = item.SerialNo,
                            NeedQty = item.OutStockQty
                        });

                    }
                }
                #endregion

                foreach (var item in messageList)
                {
                    AddLog(oscr.DcCode, oscr.GupCode, oscr.CustCode, item.Key, string.Join(Environment.NewLine, item.Value), true);
                }
            }
            _wmsLogHelper.AddRecord("寫入庫存Log 結束");
        }

        #endregion

        #endregion

        #region 配庫
        /// <summary>
        /// 新訂單配庫
        /// </summary>
        /// <param name="oscr"></param>
        /// <returns></returns>
        public List<AllotedStockOrder> NewOrderAllot(OrderStockCheckResult oscr)
        {
            // 是否使用批號或序號配庫
            var useMakeNoOrSerialNo = false;
            _wmsLogHelper.AddRecord("訂單配庫 開始");
            var serialNoService = new SerialNoService();
            var allotedStockOrders = new List<AllotedStockOrder>();
            //非配庫試算且B2B揀貨儲位數量不足且或貨主設定不允許B2B單張訂單出貨 不配庫
            if (!oscr.IsTrialCalculation && oscr.ItemNoEnougthList.Any() && oscr.IsB2B && oscr.F1909.ISB2B_ALONE_OUT == "0")
                return allotedStockOrders;

            var gTypes = oscr.CanAllotStockOrderDetails.GroupBy(x => x.F050301.TYPE_ID);
            foreach (var type in gTypes)
            {
                var oOrdNos = new List<OrdNoQty>();
                List<OrdDetailPartItemQty> ordDetailPartItemQtys = null;
                //取得此倉別揀貨庫存資料
                var pickLocStocks = oscr.PickLocStocks[oscr.DcCode + "_" + oscr.GupCode + "_" + oscr.CustCode + "_" + type.Key];
                var f050301s = type.GroupBy(x => new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO }).Select(x => x.First().F050301).ToList();
                var f050302s = type.Select(x => x.F050302).ToList();
                //B2B訂單 有允出天數限制 允續部分出貨 庫存分配方式為平均分攤(***上方邏輯已使用批號，使用這段需再調整***)
                if (oscr.IsB2B && oscr.F1909.ISALLOW_DELV_DAY == "1" && oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "1")
                {
                    oOrdNos = SortOrderPartAllowedByCarPeriod(oscr.DcCode, oscr.GupCode, oscr.CustCode, type.ToList());
                    AddNoCarPeriodMessage(oscr.DcCode, oscr.GupCode, oscr.CustCode, type.ToList());
                    var tmpF050302s = f050302s.Select(a => AutoMapper.Mapper.DynamicMap<F050302>(a)).ToList();
                    AllotedEquallyDistributedAllowDelvDay(oOrdNos, f050301s, tmpF050302s, pickLocStocks, ref allotedStockOrders);
                }
                else
                {
                    if (oscr.F1909.SPILT_OUTCHECK == "0") //Robin 20180503 不允許部分出貨
                    {
                        //依有指定序號、總揀次多及訂單編號小排序做配庫
                        oOrdNos = SortOrderNotPartAllowed(f050302s, f050301s);
                        useMakeNoOrSerialNo = true;
                    }
                    //允許部分出貨，訂單先進先出 (***上方邏輯已使用批號，使用這段需再調整***)
                    else if (oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "0")
                    {
                        oOrdNos = SortOrderPartAllowedByOrdNo(f050301s);
                    }
                    // (***上方邏輯已使用批號，使用這段需再調整***)
                    else if (oscr.F1909.SPILT_OUTCHECK == "1" && oscr.F1909.SPILT_OUTCHECKWAY == "1")
                    {
                        oOrdNos = SortOrderPartAllowedByCarPeriod(oscr.DcCode, oscr.GupCode, oscr.CustCode, type.ToList());
                        AddNoCarPeriodMessage(oscr.DcCode, oscr.GupCode, oscr.CustCode, type.ToList());
                        ordDetailPartItemQtys = (from a in f050302s
                                                 group a by new { a.ORD_NO, a.ITEM_CODE } into g
                                                 let qty = g.Sum(s => s.ORD_QTY)
                                                 select new OrdDetailPartItemQty { OrdNo = g.Key.ORD_NO, ItemCode = g.Key.ITEM_CODE, PartItemQty = qty }).ToList();
                        CalcEquallyDistributed(oOrdNos, pickLocStocks, f050301s, f050302s, oscr.IsB2B, oscr.F1909, ref ordDetailPartItemQtys);
                    }
                    //暫存庫存資料檢核允出天數效期
                    var tempPickLocStocks = new List<ItemLocPriorityInfo>();
                    pickLocStocks.ForEach((x) =>
                    {
                        ItemLocPriorityInfo y = new ItemLocPriorityInfo();
                        x.CloneProperties(y);
                        tempPickLocStocks.Add(y);
                    });
                    if (oscr.IsB2B)
                    {
                        foreach (var f050301 in f050301s)
                        {
                            var ordF050302s = f050302s.Where(a => a.ORD_NO == f050301.ORD_NO).ToList();
                            //Robin 20180508 增加不允許部分出貨才需踢單
                            if (oscr.F1909.ISALLOW_DELV_DAY == "1" && !CheckValidDate(f050301, ordF050302s, tempPickLocStocks) && oscr.F1909.SPILT_OUTCHECK == "0")
                            {
                                oscr.ReleaseAllotStockOrderDetails.AddRange(type.Where(x => x.F050301.ORD_NO == f050301.ORD_NO).ToList());
                                oscr.CanAllotStockOrderDetails.RemoveAll(x => x.F050301.ORD_NO == f050301.ORD_NO);
                                oOrdNos.RemoveAll(a => a.OrdNo == f050301.ORD_NO);
                            }
                        }
                    }
                    if (useMakeNoOrSerialNo)
                    {
                        allotedStockOrders = AllotedStockOrderByMakeNoOrSerialNo(oscr, allotedStockOrders, type, oOrdNos, ordDetailPartItemQtys, pickLocStocks, f050301s, f050302s, tempPickLocStocks);
                    }
                    else
                    {
                        allotedStockOrders = AllotedStockOrderByOrder(oscr, allotedStockOrders, type, oOrdNos, ordDetailPartItemQtys, pickLocStocks, f050301s, f050302s, tempPickLocStocks);
                    }
                }
            }
            //產生訂單分配Log
            CreateOrderAllotLog(oscr, allotedStockOrders);
            _wmsLogHelper.AddRecord("訂單配庫 結束");

            return allotedStockOrders;
        }



        private List<AllotedStockOrder> AllotedStockOrderByMakeNoOrSerialNo(OrderStockCheckResult oscr, List<AllotedStockOrder> allotedStockOrders, IGrouping<string, AllotStockOrderDetail> type, List<OrdNoQty> oOrdNos, List<OrdDetailPartItemQty> ordDetailPartItemQtys, List<ItemLocPriorityInfo> pickLocStocks, List<F050301> f050301s, List<F050302> f050302s, List<ItemLocPriorityInfo> tempPickLocStocks)
        {
            var itemCodeMakeNoOrSerialNosGs = f050302s.GroupBy(a => new { a.ITEM_CODE, a.MAKE_NO, a.SERIAL_NO }).OrderBy(a => a.Key.ITEM_CODE).ThenByDescending(a => a.Key.SERIAL_NO).ThenByDescending(a => a.Key.MAKE_NO).ToList();


            foreach (var itemCodeMakeNosG in itemCodeMakeNoOrSerialNosGs)
            {
                var pickLocStocksG = pickLocStocks.Where(a => a.ITEM_CODE == itemCodeMakeNosG.Key.ITEM_CODE);
                if (!string.IsNullOrEmpty(itemCodeMakeNosG.Key.SERIAL_NO))
                    pickLocStocksG = pickLocStocksG.Where(a => a.SERIAL_NO == itemCodeMakeNosG.Key.SERIAL_NO);
                else if (!string.IsNullOrEmpty(itemCodeMakeNosG.Key.MAKE_NO))
                    pickLocStocksG = pickLocStocksG.Where(a => a.MAKE_NO == itemCodeMakeNosG.Key.MAKE_NO);

                foreach (var ordNoQty in oOrdNos)
                {
                    var ordF050302s = itemCodeMakeNosG.Where(a => a.ORD_NO == ordNoQty.OrdNo && a.ITEM_CODE == itemCodeMakeNosG.Key.ITEM_CODE);
                    if (!string.IsNullOrEmpty(itemCodeMakeNosG.Key.SERIAL_NO))
                        ordF050302s = ordF050302s.Where(a => a.SERIAL_NO == itemCodeMakeNosG.Key.SERIAL_NO);
                    else if (!string.IsNullOrEmpty(itemCodeMakeNosG.Key.MAKE_NO))
                        ordF050302s = ordF050302s.Where(a => a.MAKE_NO == itemCodeMakeNosG.Key.MAKE_NO);

                    foreach (var f050302 in ordF050302s)
                    {
                        var ordQty = f050302.ORD_QTY;
                        //允許部分出貨平均分攤，依分攤的數量進行配庫(***上方邏輯已使用批號，使用這段需再調整***)
                        if (ordDetailPartItemQtys != null && ordDetailPartItemQtys.Any())
                        {
                            var ordDetailPartItemQty = ordDetailPartItemQtys.Single(a => a.OrdNo == f050302.ORD_NO && a.ItemCode == f050302.ITEM_CODE);
                            if (ordQty > ordDetailPartItemQty.PartItemQty)
                                ordQty = ordDetailPartItemQty.PartItemQty;
                            else
                                ordDetailPartItemQty.PartItemQty = ordDetailPartItemQty.PartItemQty - ordQty;
                        }
                        var q = pickLocStocksG.Where(a => a.ITEM_CODE == f050302.ITEM_CODE && a.QTY > 0);
                        if (!string.IsNullOrEmpty(f050302.SERIAL_NO))
                            q = q.Where(a => a.SERIAL_NO == f050302.SERIAL_NO);
                        else if (!string.IsNullOrEmpty(itemCodeMakeNosG.Key.MAKE_NO))
                        {
                            q = q.Where(a => a.MAKE_NO == f050302.MAKE_NO);
                        }

                        //如果是B2B訂單且有找到商品限制效期天數則篩選效期必須大於等於今天日期+商品限制效期天數
                        if (oscr.IsB2B && _itemLimitValidDays != null)
                        {
                            var limitValidDay = _itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == f050302.ITEM_CODE);
                            if (limitValidDay != null)
                                q = q.Where(x => x.VALID_DATE >= DateTime.Today.AddDays(limitValidDay.DELIVERY_DAY));
                        }


                        var oItemPickLocStocks = q
                                .OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                                .ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
                                .ThenBy(a => a.CASE_NO).ThenBy(a => a.BATCH_NO).ThenBy(a => a.BOX_CTRL_NO).ThenBy(a => a.PALLET_CTRL_NO).ThenBy(a => a.MAKE_NO).ThenBy(a => a.SERIAL_NO);

                        //散裝
                        ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.Bulk, false, ref ordQty, ref allotedStockOrders);
                    }
                }
            }

            return allotedStockOrders;
        }

        private List<AllotedStockOrder> AllotedStockOrderByOrder(OrderStockCheckResult oscr, List<AllotedStockOrder> allotedStockOrders, IGrouping<string, AllotStockOrderDetail> type, List<OrdNoQty> oOrdNos, List<OrdDetailPartItemQty> ordDetailPartItemQtys, List<ItemLocPriorityInfo> pickLocStocks, List<F050301> f050301s, List<F050302> f050302s, List<ItemLocPriorityInfo> tempPickLocStocks)
        {
            foreach (var ordNoQty in oOrdNos)
            {
                var ordF050302s = f050302s.Where(a => a.ORD_NO == ordNoQty.OrdNo).ToList();
                foreach (var f050302 in ordF050302s)
                {
                    var ordQty = f050302.ORD_QTY;
                    //允許部分出貨平均分攤，依分攤的數量進行配庫
                    if (ordDetailPartItemQtys != null && ordDetailPartItemQtys.Any())
                    {
                        var ordDetailPartItemQty = ordDetailPartItemQtys.Single(a => a.OrdNo == f050302.ORD_NO && a.ItemCode == f050302.ITEM_CODE);
                        if (ordQty > ordDetailPartItemQty.PartItemQty)
                            ordQty = ordDetailPartItemQty.PartItemQty;
                        else
                            ordDetailPartItemQty.PartItemQty = ordDetailPartItemQty.PartItemQty - ordQty;
                    }
                    var q = pickLocStocks.Where(a => a.ITEM_CODE == f050302.ITEM_CODE && a.QTY > 0);
                    if (!string.IsNullOrEmpty(f050302.SERIAL_NO))
                        q = q.Where(a => a.SERIAL_NO == f050302.SERIAL_NO);

                    //如果是B2B訂單且有找到商品限制效期天數則篩選效期必須大於等於今天日期+商品限制效期天數
                    if (oscr.IsB2B && _itemLimitValidDays != null)
                    {
                        var limitValidDay = _itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == f050302.ITEM_CODE);
                        if (limitValidDay != null)
                            q = q.Where(x => x.VALID_DATE >= DateTime.Today.AddDays(limitValidDay.DELIVERY_DAY));
                    }


                    var oItemPickLocStocks = q
                            .OrderByDescending(a => a.ATYPE_CODE).ThenBy(a => a.VALID_DATE).ThenBy(a => a.ENTER_DATE)
                            .ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE).ThenBy(a => a.BOX_SERIAL)
                            .ThenBy(a => a.CASE_NO).ThenBy(a => a.BATCH_NO).ThenBy(a => a.BOX_CTRL_NO).ThenBy(a => a.PALLET_CTRL_NO).ThenBy(a => a.MAKE_NO);

                    //散裝
                    ShareQty(oItemPickLocStocks, f050302, 1, ShareUnitQtyType.Bulk, false, ref ordQty, ref allotedStockOrders);
                }
            }

            return allotedStockOrders;
        }

        #region 訂單檢核

        /// <summary>
        /// 檢查訂單是否符合商品限制效期
        /// </summary>
        /// <param name="f050001">訂單主檔</param>
        /// <param name="f050002s">訂單明細</param>
        /// <returns></returns>
        private bool CheckValidDate(F050301 f050301, List<F050302> f050302s, List<ItemLocPriorityInfo> pickLocPriorityInfos)
        {
            var itemLimitValidDays = GetItemLimitValidDays(f050301.GUP_CODE, f050301.CUST_CODE, f050301.RETAIL_CODE, f050302s.Select(x => x.ITEM_CODE).Distinct().ToList()).ToList();
            var isEnougthQty = true;
            var message = new List<string>();
            if (itemLimitValidDays.Any())
            {
                var groupDetail = f050302s.GroupBy(x => x.ITEM_CODE);
                foreach (var detail in groupDetail)
                {
                    var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);

                    if (itemLimitValidDay != null)
                    {
                        var stockQty = pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).Sum(x => x.QTY);
                        var detailQty = detail.Sum(x => x.ORD_QTY);
                        //庫存數小於訂購數 此訂單不配庫
                        if (stockQty < detailQty)
                        {
                            //訂單編號:{0} 商品:{1} 符合允出天數的庫存不足
                            message.Add(string.Format(_commonService.GetMsg("AAM00024"), f050301.ORD_NO, detail.Key));
                            isEnougthQty = false;
                        }
                    }
                }
                if (message.Any())
                    AddLog(f050301.DC_CODE, f050301.GUP_CODE, f050301.CUST_CODE, "AAM00024", string.Join(Environment.NewLine, message), true);

                if (isEnougthQty)
                {
                    foreach (var detail in groupDetail)
                    {
                        var itemLimitValidDay = itemLimitValidDays.FirstOrDefault(x => x.ITEM_CODE == detail.Key);
                        var stocks = (itemLimitValidDay != null) ? pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key && x.VALID_DATE >= DateTime.Today.AddDays(itemLimitValidDay.DELIVERY_DAY)).OrderBy(x => x.VALID_DATE) : pickLocPriorityInfos.Where(x => x.ITEM_CODE == detail.Key).OrderBy(x => x.VALID_DATE);
                        long detailQty = detail.Sum(x => x.ORD_QTY);
                        foreach (var st in stocks)
                        {
                            if (st.QTY < detailQty)
                            {
                                detailQty -= st.QTY;
                                st.QTY = 0;
                            }
                            else
                            {
                                st.QTY -= detailQty;
                                detailQty = 0;
                            }
                            if (detailQty == 0)
                                break;
                        }
                    }
                }
            }
            return isEnougthQty;
        }

        #endregion

        #region 分配庫存數
        /// <summary>
        /// 分配庫存數
        /// </summary>
        /// <param name="oItemPickLocStocks">商品揀貨庫存資料</param>
        /// <param name="f050302">訂單明細</param>
        /// <param name="unitQty">商品容積單位最小數量</param>
        /// <param name="shareUnitType">容積單位類型</param>
        /// <param name="isUnBoxing">是否拆盒拆箱</param>
        /// <param name="ordQty">訂購數量</param>
        /// <param name="allotedStockOrders">回傳訂單分配明細</param>
        private void ShareQty(IOrderedEnumerable<ItemLocPriorityInfo> oItemPickLocStocks, F050302 f050302, int unitQty, ShareUnitQtyType shareUnitType, bool isUnBoxing, ref int ordQty, ref List<AllotedStockOrder> allotedStockOrders)
        {
            if (unitQty <= 0 || ordQty < unitQty)
                return;
            //已分配的序號
            var sharedSerialNos = allotedStockOrders.Where(x => x.SerialNo != "0").Select(x => x.SerialNo).Distinct().ToList();
            var stocks = new List<IGrouping<string, ItemLocPriorityInfo>>();
            switch (shareUnitType)
            {
                case ShareUnitQtyType.Bulk:
                    // 排序方式: 儲區型態DESC,效期ASC,入庫日ASC,難易度DESC,水平距離ASC,儲位編號ASC
                    stocks = oItemPickLocStocks.Where(o => string.IsNullOrEmpty(o.CASE_NO) && string.IsNullOrEmpty(o.BOX_SERIAL) &&
                                                                                                                                             string.IsNullOrEmpty(o.BATCH_NO) && o.QTY > 0 &&
                                                                                                                                             sharedSerialNos.All(c => c != o.SERIAL_NO))
                                                                                             .OrderByDescending(a => a.ATYPE_CODE)
                                                                                             .ThenBy(a => a.VALID_DATE)
                                                                                             .ThenBy(a => a.ENTER_DATE)
                                                                                             .ThenByDescending(a => a.HANDY)
                                                                                             .ThenBy(a => a.HOR_DISTANCE)
                                                                                             .ThenBy(a => a.LOC_CODE)
                                                                                             .GroupBy(c => c.ATYPE_CODE + "|" + c.VALID_DATE.ToString("yyyy/MM/dd") + "|" + c.ENTER_DATE.ToString("yyyy/MM/dd") + "|" + c.HANDY + "|" + c.HOR_DISTANCE).ToList();
                    break;
            }
            foreach (var stock in stocks)
            {
                if (ordQty >= unitQty)
                {
                    if (((!isUnBoxing && stock.Sum(x => x.QTY) == unitQty) || isUnBoxing || shareUnitType == ShareUnitQtyType.Bulk))
                        CreateAllotedStockOrder(stock, f050302, ref ordQty, ref allotedStockOrders);
                }
                else
                    break;
            }
        }
        #endregion

        #region 建立配庫訂單結果
        /// <summary>
        /// 建立配庫訂單結果
        /// </summary>
        /// <param name="oItemPickLocPriorityInfos"></param>
        /// <param name="f050302"></param>
        /// <param name="ordQty"></param>
        /// <param name="allotedStockOrders"></param>
        private void CreateAllotedStockOrder(IEnumerable<ItemLocPriorityInfo> oItemPickLocPriorityInfos, F050302 f050302, ref int ordQty, ref List<AllotedStockOrder> allotedStockOrders)
        {
            foreach (var itemPickLocPriorityInfo in oItemPickLocPriorityInfos)
            {
                var isLocEnough = true;
                var allotQty = ordQty;
                if (itemPickLocPriorityInfo.QTY >= ordQty) //儲位數量足夠
                {
                    //扣除儲位數量
                    itemPickLocPriorityInfo.QTY -= ordQty;
                    ordQty = 0;
                }
                else //儲位數量不足
                {
                    allotQty = (int)itemPickLocPriorityInfo.QTY; //本儲位配庫的數量
                    ordQty -= (int)itemPickLocPriorityInfo.QTY; //剩餘未配庫的數量
                    itemPickLocPriorityInfo.QTY = 0; //儲位數量全數配庫，因此歸0
                    isLocEnough = false; //表示本儲位不足
                }
                var allotF050302 = AutoMapper.Mapper.DynamicMap<F050302>(f050302);
                allotF050302.ORD_QTY = allotQty;
                allotedStockOrders.Add(new AllotedStockOrder
                {
                    F050302 = allotF050302,
                    EnterDate = itemPickLocPriorityInfo.ENTER_DATE,
                    Floor = itemPickLocPriorityInfo.FLOOR,
                    LocCode = itemPickLocPriorityInfo.LOC_CODE,
                    SerialNo = itemPickLocPriorityInfo.SERIAL_NO,
                    TmprType = itemPickLocPriorityInfo.TMPR_TYPE,
                    ValidDate = itemPickLocPriorityInfo.VALID_DATE,
                    VnrCode = itemPickLocPriorityInfo.VNR_CODE,
                    BoxCtrlNo = itemPickLocPriorityInfo.BOX_CTRL_NO,
                    PalletCtrlNo = itemPickLocPriorityInfo.PALLET_CTRL_NO,
                    WarehouseId = itemPickLocPriorityInfo.WAREHOUSE_ID,
                    AreaCode = itemPickLocPriorityInfo.AREA_CODE,
                    MakeNo = itemPickLocPriorityInfo.MAKE_NO,
                    Orginal_OrdQty = f050302.ORD_QTY,
                    PickFloor = itemPickLocPriorityInfo.PICK_FLOOR,
                    DeviceType = itemPickLocPriorityInfo.DEVICE_TYPE,
                    PkArea = itemPickLocPriorityInfo.PK_AREA,
                    PKAreaName = itemPickLocPriorityInfo.PK_NAME,
                    WarehouseName = itemPickLocPriorityInfo.WAREHOUSE_NAME
                });
                if (isLocEnough) //儲位數量已滿足則跳開，若不足則找下一順序儲位配庫
                    break;
            }
        }

        #endregion

        #region 配庫訂單排序

        /// <summary>
        /// B2B訂單允許部分出貨-依出貨時段ASC訂單編號ASC排序
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="allotStockOrderDetails"></param>
        /// <returns></returns>
        private List<OrdNoQty> SortOrderPartAllowedByCarPeriod(string dcCode, string gupCode, string custCode, List<AllotStockOrderDetail> allotStockOrderDetails)
        {
            _wmsLogHelper.AddRecord("B2B訂單允許部分出貨-訂單依出貨時段ASC訂單編號ASC排序 開始");
            var data = allotStockOrderDetails.Select(x => new OrdNoQty { OrdNo = x.F050301.ORD_NO, CarPeriod = x.CarPeriod })
                    .Distinct().OrderBy(a => a.CarPeriod).ThenBy(a => a.OrdNo).ToList();
            _wmsLogHelper.AddRecord("B2B訂單允許部分出貨-訂單依出貨時段ASC訂單編號ASC排序 結束");
            return data;
        }

        /// <summary>
        /// 訂單不允許部分出貨-依有指定序號 DESC、總揀次多DESC、優先處理旗標DESC、訂單編號ASC排序
        /// </summary>
        /// <param name="f050302s"></param>
        /// <param name="gF050301"></param>
        /// <returns></returns>
        private List<OrdNoQty> SortOrderNotPartAllowed(List<F050302> f050302s, List<F050301> f050301s)
        {
            //訂單不允許部分出貨-依有指定序號 DESC、總揀次多DESC、優先處理旗標DESC、訂單編號ASC排序做配庫
            _wmsLogHelper.AddRecord("訂單不允許部分出貨-訂單依有指定序號 DESC、總揀次多DESC、優先處理旗標DESC、訂單編號ASC排序 開始");
            var oOrdNos = from a in f050301s
                          join b in f050302s on a.ORD_NO equals b.ORD_NO
                          group b by new { a.ORD_NO, a.FAST_DEAL_TYPE } into g
                          let sumOrdQty = g.Sum(s => s.ORD_QTY)
                          orderby g.Any(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO)) descending,
                                                          sumOrdQty descending,
                                                          g.Key.FAST_DEAL_TYPE descending,
                                                          g.Key.ORD_NO
                          select new OrdNoQty { OrdNo = g.Key.ORD_NO };
            _wmsLogHelper.AddRecord("訂單不允許部分出貨-訂單依有指定序號 DESC、總揀次多DESC、優先處理旗標DESC、訂單編號ASC排序 結束");
            return oOrdNos.ToList();
        }

        /// <summary>
        /// 訂單允許部分出貨且部分出貨方式選擇先進先出-訂單依訂單編號ASC排序
        /// </summary>
        /// <param name="f050302s"></param>
        /// <param name="gF050301"></param>
        /// <returns></returns>
        private List<OrdNoQty> SortOrderPartAllowedByOrdNo(List<F050301> f050301s)
        {
            _wmsLogHelper.AddRecord("訂單允許部分出貨且部分出貨方式選擇先進先出-訂單依訂單編號ASC排序 開始");
            //允許部分出貨，訂單先進先出
            var oOrdNos = from a in f050301s
                          orderby a.ORD_NO
                          select new OrdNoQty { OrdNo = a.ORD_NO };
            _wmsLogHelper.AddRecord("訂單允許部分出貨且部分出貨方式選擇先進先出-訂單依訂單編號ASC排序 結束");
            return oOrdNos.ToList();
        }
        #endregion

        #region 有允出天數限制的平均分攤配庫
        //概念
        //希望盡可能將可出的庫存配出去，因此希望最小效期的庫存盡量配給允許最小效期的訂單明細
        //庫存依效期分群組，短效期優先配
        //找出允許配此效期的訂單明細，取平均數分配(邏輯如沒有允出限制的平均分攤)
        //因訂單的各明細的允許最小效期在所有訂單中的排序未必一樣，所以要打散各群組(出車時段)的訂單明細來配庫
        private void AllotedEquallyDistributedAllowDelvDay(IEnumerable<OrdNoQty> oOrdNos, List<F050301> f050301s, List<F050302> details, List<ItemLocPriorityInfo> pickLocPriorityInfos, ref List<AllotedStockOrder> allotedStockOrders)
        {
            var itemLimitValidDays = new List<ItemLimitValidDay>();
            var gRetailF050301s = f050301s.GroupBy(a => a.RETAIL_CODE);
            foreach (var gRetailF050301 in gRetailF050301s)
            {
                var f050301First = gRetailF050301.First();
                var ordNos = gRetailF050301.Select(a => a.ORD_NO);
                var itemCodes = details.Where(a => ordNos.Contains(a.ORD_NO)).Select(a => a.ITEM_CODE).Distinct().ToList();
                itemLimitValidDays.AddRange(GetItemLimitValidDays(f050301First.GUP_CODE, f050301First.CUST_CODE, gRetailF050301.Key, itemCodes));
            }
            itemLimitValidDays = itemLimitValidDays.Distinct().ToList();

            //依出車時段分群訂單
            var gOrdNos = oOrdNos.GroupBy(a => a.CarPeriod).OrderBy(a => a.Key);
            foreach (var gOrdNo in gOrdNos)
            {
                var ordNos = gOrdNo.Select(a => a.OrdNo);
                //依品項分群訂單明細
                var gDetails = details.Where(a => ordNos.Contains(a.ORD_NO)).GroupBy(a => a.ITEM_CODE);
                foreach (var gDetail in gDetails)
                {

                    //先過濾此品項的庫存資料，再依儲區別將庫存分群排序(黃金先於一般)
                    var gATypePickLocPriorityInfos = pickLocPriorityInfos.Where(a => a.ITEM_CODE == gDetail.Key).GroupBy(a => a.ATYPE_CODE);
                    foreach (var gATypePickLocPriorityInfo in gATypePickLocPriorityInfos)
                    {
                        //不同的庫存群組視為另一次的配庫，所以將_allotedPartOrdDetailInfos清除重來
                        _allotedPartOrdDetailInfos.Clear();
                        //  再依效期分群
                        var validDatePickLocPriorityInfos = gATypePickLocPriorityInfo.Where(a => a.QTY > 0).GroupBy(a => a.VALID_DATE).OrderBy(a => a.Key);
                        foreach (var validDatePickLocPriorityInfo in validDatePickLocPriorityInfos)
                        {
                            //validDateDetails = 取得允許配此效期的訂單明細
                            var validDateDetails = (from a in gDetail
                                                    join b in f050301s on new { a.GUP_CODE, a.DC_CODE, a.CUST_CODE, a.ORD_NO } equals new { b.GUP_CODE, b.DC_CODE, b.CUST_CODE, b.ORD_NO }
                                                    join c in itemLimitValidDays on new { a.ITEM_CODE, b.RETAIL_CODE } equals new { c.ITEM_CODE, c.RETAIL_CODE } into j
                                                    from c in j.DefaultIfEmpty()
                                                    where (c == null || validDatePickLocPriorityInfo.Key >= DateTime.Today.AddDays(c.DELIVERY_DAY))
                                                                    && a.ORD_QTY > 0
                                                    select a).Distinct().ToList();
                            var sortValidDatePickLocPriorityInfos = validDatePickLocPriorityInfo.OrderBy(a => a.ENTER_DATE)
                                                                                                                            .ThenByDescending(a => a.HANDY).ThenBy(a => a.HOR_DISTANCE).ThenBy(a => a.LOC_CODE);
                            AllotedEquallyDistributedAllowDelvDayEmpl(validDateDetails, sortValidDatePickLocPriorityInfos, ref allotedStockOrders);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 有允出天數限制的平均分攤配庫實作
        /// 因會包含允許上一效期的訂單明細，允許上一效期的訂單明細已配部分數量
        /// 若排除已配部分數量的訂單明細所算得的平均數小於等於之前最小的部分配庫數量，則此次只配未配庫的部分(因為配給這些明細的數量最多跟先前配的部分數量一樣多)。
        /// 若平均數大於之前最小的部分配庫數量(表示應該要再配一些量給先前的明細)，將前已配的數量與這次可配的數量加總再取一次平均數(遞迴取得最終的平均數)，
        /// 取的的平均數在扣掉之前已配的數量進行配庫
        /// </summary>
        /// <param name="validDateDetails"></param>
        /// <param name="sortValidDatePickLocPriorityInfos"></param>
        /// <param name="allotedStockOrders"></param>
        private void AllotedEquallyDistributedAllowDelvDayEmpl(List<F050302> validDateDetails, IEnumerable<ItemLocPriorityInfo> sortValidDatePickLocPriorityInfos, ref List<AllotedStockOrder> allotedStockOrders)
        {
            if (validDateDetails.Count == 0)
                return;

            //計算可出庫存總數
            var stockSumQty = sortValidDatePickLocPriorityInfos.Sum(a => a.QTY);

            var itemAverageQty = 0;
            var modQty = 0;
            var tmpValidDateDetails = validDateDetails.Select(a => a).ToList();
            if (!_allotedPartOrdDetailInfos.Any())
            {
                //取可出數的平均數
                itemAverageQty = (int)(stockSumQty / tmpValidDateDetails.Count);
                //取餘數
                modQty = (int)(stockSumQty % tmpValidDateDetails.Count);
            }
            else
            {
                //計算平均數
                CalcAverageQty(validDateDetails, stockSumQty, out itemAverageQty, out modQty, out tmpValidDateDetails);
            }

            //取得少等於平均數的訂單明細，這些訂單明細可完全滿足，不須再計算數量
            var allotedOrdNos = _allotedPartOrdDetailInfos.Select(a => a.F050302.ORD_NO).ToList();
            var enoughOrdDetailPartItemQtys = tmpValidDateDetails.Where(a => a.ORD_QTY <= itemAverageQty && !allotedOrdNos.Contains(a.ORD_NO)).
                            Union(_allotedPartOrdDetailInfos.Where(a => a.F050302.ORD_QTY + a.AllotedQty <= itemAverageQty).Select(a => a.F050302)).ToList();
            if (enoughOrdDetailPartItemQtys.Any())
            {
                //對 enoughOrdDetailPartItemQtys 執行配庫
                //配庫中會扣除可配庫的庫存數量
                foreach (var f050302 in enoughOrdDetailPartItemQtys)
                {
                    var ordQty = f050302.ORD_QTY;
                    CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
                    //扣除這次配庫數(原則上此時ordQty應該為0)
                    f050302.ORD_QTY = ordQty;

                    var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.SingleOrDefault(a => a.F050302.Equals(f050302));
                    //由已配庫但未全配完的訂單明細List移除已完全配完的訂單明細
                    if (f050302.ORD_QTY == 0 && allotedOrdDetailInfo != null)
                        _allotedPartOrdDetailInfos.Remove(allotedOrdDetailInfo);
                }

                stockSumQty = stockSumQty - enoughOrdDetailPartItemQtys.Sum(a => a.ORD_QTY);

                //剩餘訂單須重新計算平均數做分攤
                var validDateDetails2 = tmpValidDateDetails.Where(a => !enoughOrdDetailPartItemQtys.Select(b => b.ORD_NO).Distinct().ToList().Contains(a.ORD_NO)).ToList();
                AllotedEquallyDistributedAllowDelvDayEmpl(validDateDetails2, sortValidDatePickLocPriorityInfos, ref allotedStockOrders);
            }
            else
            {
                //先分配給每張訂單明細平均數 執行配庫
                //配庫中會扣除可配庫的庫存數量
                foreach (var f050302 in tmpValidDateDetails)
                {
                    var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.SingleOrDefault(a => a.F050302.Equals(f050302));
                    //取的的平均數在扣掉之前已配的數量進行配庫
                    var ordQty = itemAverageQty - ((allotedOrdDetailInfo == null) ? 0 : allotedOrdDetailInfo.AllotedQty);
                    CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
                    //扣除這次配庫數(原則上此時ordQty應該為0)
                    f050302.ORD_QTY = f050302.ORD_QTY - itemAverageQty + ordQty;

                    if (allotedOrdDetailInfo == null)
                    {
                        allotedOrdDetailInfo = new AllotedPartOrdDetailInfo { AllotedQty = itemAverageQty - ordQty, F050302 = f050302 };
                        _allotedPartOrdDetailInfos.Add(allotedOrdDetailInfo);
                    }
                    else
                        allotedOrdDetailInfo.AllotedQty += itemAverageQty - ordQty;
                }

                //餘數的前幾筆各+1 (將剩餘數量平均分配在前幾個訂單)
                var validDateDetails2 = tmpValidDateDetails
                                .OrderBy(a => a.ORD_NO).Take(modQty).ToList();
                foreach (var f050302 in validDateDetails2)
                {
                    var ordQty = 1;
                    CreateAllotedStockOrder(sortValidDatePickLocPriorityInfos, f050302, ref ordQty, ref allotedStockOrders);
                    //扣除已配庫數(原則上此時ordQty應該為0)
                    f050302.ORD_QTY = f050302.ORD_QTY - 1 + ordQty;

                    var allotedOrdDetailInfo = _allotedPartOrdDetailInfos.Single(a => a.F050302.Equals(f050302));
                    //由已配庫但未全配完的訂單明細List移除已完全配完的訂單明細
                    if (f050302.ORD_QTY == 0)
                        _allotedPartOrdDetailInfos.Remove(allotedOrdDetailInfo);
                    else
                        allotedOrdDetailInfo.AllotedQty += 1 - ordQty;
                }
            }
        }

        /// <summary>
        /// 有允出天數限制的平均分攤配庫計算平均數
        /// 將前已配的數量與這次可配的數量加總再取一次平均數(遞迴取得最終的平均數)
        /// 若此次
        /// </summary>
        /// <param name="validDateDetails"></param>
        /// <param name="stockSumQty"></param>
        /// <param name="itemAverageQty"></param>
        /// <param name="modQty"></param>
        /// <param name="tmpValidDateDetails"></param>
        /// <param name="preMinAllotedQty"></param>
        private void CalcAverageQty(List<F050302> validDateDetails, long stockSumQty, out int itemAverageQty, out int modQty, out List<F050302> tmpValidDateDetails, int preMinAllotedQty = 0)
        {
            //先排除大於上一次遞迴的最小已配庫數(即取得未配庫及小於等於上一次遞迴的最小已配庫數)的訂單明細 (本次要計算的明細)
            var allotedOrdNos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty > preMinAllotedQty).Select(a => a.F050302.ORD_NO);
            tmpValidDateDetails = validDateDetails.Where(a => !allotedOrdNos.Contains(a.ORD_NO)).ToList();

            //由已配庫但未全配完的訂單明細List中，過濾小於等於上一次遞迴的最小已配庫數的訂單明細，計算已配庫數
            var tmpAllotedOrdDetailInfos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty <= preMinAllotedQty);
            var tmpAllotedQty = tmpAllotedOrdDetailInfos.Sum(a => a.AllotedQty);

            //將前已配的數量與這次可配的數量加總再取一次平均數
            itemAverageQty = 0;
            modQty = 0;
            if (tmpValidDateDetails.Any())
            {
                itemAverageQty = (int)((stockSumQty + tmpAllotedQty) / tmpValidDateDetails.Count);
                modQty = (int)((stockSumQty + tmpAllotedQty) % tmpValidDateDetails.Count);
            }

            var nowAllotedPartOrdDetailInfos = _allotedPartOrdDetailInfos.Where(a => a.AllotedQty > preMinAllotedQty);
            if (nowAllotedPartOrdDetailInfos.Any())
            {
                var minAllotedQty = nowAllotedPartOrdDetailInfos.Min(a => a.AllotedQty);
                //已無前面已配庫訂單明細或平均數小於等於tmpValidDateDetails前面已配庫訂單明細的最少量，則全配在tmpValidDateDetails中
                //若平均數大於於tmpValidDateDetails前面已配庫訂單明細的最少量，則須重新計算平均數
                if (!tmpValidDateDetails.Any() || itemAverageQty > minAllotedQty && allotedOrdNos.Any())
                {
                    //計算
                    CalcAverageQty(validDateDetails, stockSumQty, out itemAverageQty, out modQty, out tmpValidDateDetails, minAllotedQty);
                }
            }
        }
        #endregion 有允出天數限制的平均分攤配庫

        #region 計算平均分攤的分攤數量
        private void CalcEquallyDistributed(IEnumerable<OrdNoQty> oOrdNos, List<ItemLocPriorityInfo> pickLocPriorityInfos, List<F050301> f050301s, List<F050302> f050302s, bool isB2B, F1909 f1909, ref List<OrdDetailPartItemQty> ordDetailPartItemQtys)
        {
            //訂單依出車時段分群
            var gOrdNos = oOrdNos.GroupBy(a => a.CarPeriod).OrderBy(g => g.Key);
            //合計品項可出數量
            var itemSumQtys = (from a in pickLocPriorityInfos
                               group a by a.ITEM_CODE into g
                               let sumQty = g.Sum(s => s.QTY)
                               select new ItemSumQty { ITEM_CODE = g.Key, SumQty = sumQty }).ToList();


            foreach (var gOrdNo in gOrdNos)
            {
                var ordNos = gOrdNo.Select(a => a.OrdNo).ToList();
                //取得群組(出車時段)訂單的商品及數量
                var partOrdItemQtys = from a in f050302s
                                      join b in gOrdNo on a.ORD_NO equals b.OrdNo
                                      group a by new { a.ORD_NO, a.ITEM_CODE } into g
                                      let qty = g.Sum(s => s.ORD_QTY)
                                      select new { g.Key.ORD_NO, g.Key.ITEM_CODE, Qty = qty };
                //取得群組(出車時段)各商品的總數量
                var partOrdItemSumQtys = from a in partOrdItemQtys
                                         group a by a.ITEM_CODE into g
                                         let sumQry = g.Sum(s => s.Qty)
                                         select new ItemSumQty { ITEM_CODE = g.Key, SumQty = sumQry };

                //判斷各商品數量總數是否滿足
                foreach (var partOrdItemSumQty in partOrdItemSumQtys)
                {
                    var ordNos2 = f050302s.Where(a => a.ITEM_CODE == partOrdItemSumQty.ITEM_CODE).Select(a => a.ORD_NO).ToList();
                    var itemSumQty = itemSumQtys.SingleOrDefault(a => a.ITEM_CODE == partOrdItemSumQty.ITEM_CODE);
                    if (itemSumQty == null || itemSumQty.SumQty <= 0)
                    {
                        //此商品可出數量為0時，設定此群組此商品的數量為0
                        ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).ToList()
                                        .ForEach(a => a.PartItemQty = 0);
                    }
                    //大於表示全部滿足，可分配數扣除此群組總數
                    else if (itemSumQty.SumQty >= partOrdItemSumQty.SumQty)
                    {
                        itemSumQty.SumQty = itemSumQty.SumQty - ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).Sum(a => a.PartItemQty);
                    }
                    //當可出數量少於群組商品總數時須平均分攤
                    else if (itemSumQty.SumQty < partOrdItemSumQty.SumQty)
                    {
                        CalcEquallyDistributedEmpl(itemSumQty, ordNos2, partOrdItemSumQty, ref ordDetailPartItemQtys);
                    }
                }
            }
        }

        private void CalcEquallyDistributedEmpl(ItemSumQty itemSumQty, List<string> ordNos, ItemSumQty partOrdItemSumQty, ref List<OrdDetailPartItemQty> ordDetailPartItemQtys)
        {
            //取可出數的平均數
            var itemAverageQty = (int)((itemSumQty.SumQty ?? 0) / ordNos.Count);

            //取得少等於平均數的訂單商品，這些訂單的商品可完全滿足，不須再計算數量
            var enoughOrdDetailPartItemQtys = ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE && a.PartItemQty <= itemAverageQty);
            if (enoughOrdDetailPartItemQtys.Any())
            {
                itemSumQty.SumQty = itemSumQty.SumQty - enoughOrdDetailPartItemQtys.Sum(a => a.PartItemQty);

                //剩餘訂單須重新計算平均數做分攤
                var ordNos2 = ordNos.Where(a => !enoughOrdDetailPartItemQtys.Select(b => b.OrdNo).Distinct().ToList().Contains(a)).ToList();
                CalcEquallyDistributedEmpl(itemSumQty, ordNos2, partOrdItemSumQty, ref ordDetailPartItemQtys);
            }
            else
            {
                //先分配給每張訂單平均數
                ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE).ToList()
                                .ForEach(a => a.PartItemQty = itemAverageQty);

                //取餘數
                var modQty = (int)((itemSumQty.SumQty ?? 0) % ordNos.Count);
                //餘數的前幾筆各+1 (將剩餘數量平均分配在前幾個訂單)
                ordDetailPartItemQtys.Where(a => ordNos.Contains(a.OrdNo) && a.ItemCode == partOrdItemSumQty.ITEM_CODE)
                                .OrderBy(a => a.OrdNo).Take(modQty).ToList()
                                .ForEach(a => a.PartItemQty++);

                //因數量不足，已全數分攤，所以這邊歸0
                itemSumQty.SumQty = 0;
            }
        }
        #endregion

        #region 門市尚未設定出車時段Log

        /// <summary>
        /// 門市尚未設定出車時段Log
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="allotStockOrderDetails"></param>
        private void AddNoCarPeriodMessage(string dcCode, string gupCode, string custCode, List<AllotStockOrderDetail> allotStockOrderDetails)
        {
            _wmsLogHelper.AddRecord("門市尚未設定出車時段Log 開始");
            var noCarPeriodRetails = allotStockOrderDetails.Where(x => x.CarPeriod == "Z").GroupBy(x => x.F050301.RETAIL_CODE).Select(x => x.Key).ToList();
            if (noCarPeriodRetails.Any())
            {
                //丟到訊息池
                var f1901 = _commonService.GetDc(dcCode);
                var f1929 = _commonService.GetGup(gupCode);
                var f1909 = _commonService.GetCust(gupCode, custCode);

                //物流中心:「{0}」業主:「{1}」貨主:「{2}」門市:「{3}」尚未設定出車時段
                var msg = string.Format(_commonService.GetMsg("AAM00021"),
                                                                (f1901 == null) ? "" : f1901.DC_NAME,
                                                                (f1929 == null) ? "" : f1929.GUP_NAME,
                                                                (f1909 == null) ? "" : f1909.SHORT_NAME,
                                                                string.Join("、", noCarPeriodRetails));

                AddLog(dcCode, gupCode, custCode, "AAM00021", msg, true);
            }
            _wmsLogHelper.AddRecord("門市尚未設定出車時段Log 結束");
        }
        #endregion

        #region 產生訂單分配Log
        /// <summary>
        /// 產生訂單分配Log
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主</param>
        /// <param name="custCode">貨主</param>
        /// <param name="f050302s">原始訂單明細</param>
        /// <param name="allotedStockOrders">已分配訂單明細</param>
        /// <param name="isTrialCalculation">是否試算</param>
        private void CreateOrderAllotLog(OrderStockCheckResult oscr, List<AllotedStockOrder> allotedStockOrders)
        {
            _wmsLogHelper.AddRecord("產生訂單分配Log 開始");
            var messageList = new Dictionary<string, List<string>>();
            if (oscr.IsTrialCalculation)
            {
                var f05080501Repo = new F05080501Repository(Schemas.CoreSchema, _wmsTransaction);
                var f05080502Repo = new F05080502Repository(Schemas.CoreSchema, _wmsTransaction);
                var addF05080501List = new List<F05080501>();
                var addF05080502List = new List<F05080502>();

                #region 產生釋放訂單配庫試算紀錄
                var gRelaseAllotStockOrders = oscr.ReleaseAllotStockOrderDetails
                        .GroupBy(x => new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO });
                foreach (var item in gRelaseAllotStockOrders)
                {
                    var f05080501 = new F05080501
                    {
                        DC_CODE = item.Key.DC_CODE,
                        GUP_CODE = item.Key.GUP_CODE,
                        CUST_CODE = item.Key.CUST_CODE,
                        CAL_NO = oscr.TrialCalculationNo,
                        ORD_NO = item.Key.ORD_NO,
                        RESULT_CODE = "03"  //試算結果代碼:01:全件出貨  02:部分出貨 03:無法出貨
                    };
                    addF05080501List.Add(f05080501);
                    addF05080502List.AddRange(
                    item.Select(x => new F05080502
                    {
                        DC_CODE = x.F050302.DC_CODE,
                        GUP_CODE = x.F050302.GUP_CODE,
                        CUST_CODE = x.F050302.CUST_CODE,
                        CAL_NO = oscr.TrialCalculationNo,
                        ORD_NO = x.F050302.ORD_NO,
                        ORD_SEQ = x.F050302.ORD_SEQ,
                        ITEM_CODE = x.F050302.ITEM_CODE,
                        ORD_QTY = x.F050302.ORD_QTY,
                        ALLOT_QTY = 0
                    }).ToList());
                }
                #endregion

                #region 產生可配庫訂單試算紀錄
                var gCanAllotStockOrders = oscr.CanAllotStockOrderDetails
                        .GroupBy(x => new { x.F050301.DC_CODE, x.F050301.GUP_CODE, x.F050301.CUST_CODE, x.F050301.ORD_NO });
                foreach (var item in gCanAllotStockOrders)
                {
                    var f05080502s = item.Select(x => new F05080502
                    {
                        DC_CODE = x.F050302.DC_CODE,
                        GUP_CODE = x.F050302.GUP_CODE,
                        CUST_CODE = x.F050302.CUST_CODE,
                        CAL_NO = oscr.TrialCalculationNo,
                        ORD_NO = x.F050302.ORD_NO,
                        ORD_SEQ = x.F050302.ORD_SEQ,
                        ITEM_CODE = x.F050302.ITEM_CODE,
                        ORD_QTY = x.F050302.ORD_QTY,
                        ALLOT_QTY = allotedStockOrders.Where(y => y.F050302.DC_CODE == x.F050302.DC_CODE &&
                        y.F050302.GUP_CODE == x.F050302.GUP_CODE && y.F050302.CUST_CODE == x.F050302.CUST_CODE &&
                        y.F050302.ORD_NO == x.F050302.ORD_NO && y.F050302.ORD_SEQ == x.F050302.ORD_SEQ).Sum(z => z.F050302.ORD_QTY)
                    }).ToList();

                    var resultCode = string.Empty;
                    if (f05080502s.All(x => x.ALLOT_QTY == 0))
                        resultCode = "03"; //03:無法出貨
                    else if (f05080502s.Any(x => x.ORD_QTY != x.ALLOT_QTY))
                        resultCode = "02"; //02:部分出貨
                    else
                        resultCode = "01";//01:全件出貨 

                    if (resultCode != "01")
                    {
                        var f05080501 = new F05080501
                        {
                            DC_CODE = item.Key.DC_CODE,
                            GUP_CODE = item.Key.GUP_CODE,
                            CUST_CODE = item.Key.CUST_CODE,
                            CAL_NO = oscr.TrialCalculationNo,
                            ORD_NO = item.Key.ORD_NO,
                            RESULT_CODE = resultCode  //試算結果代碼:01:全件出貨  02:部分出貨 03:無法出貨
                        };
                        addF05080501List.Add(f05080501);
                        addF05080502List.AddRange(f05080502s.Where(x => x.ORD_QTY != x.ALLOT_QTY));
                    }
                }
                #endregion

                f05080501Repo.BulkInsert(addF05080501List);
                f05080502Repo.BulkInsert(addF05080502List);

            }
            else
            {
                //貨主允許部分出貨
                if (oscr.F1909.SPILT_OUTCHECK == "1")
                {
                    #region 產生配庫後訂單部分庫存不足商品
                    var f050302s = oscr.CanAllotStockOrderDetails.Select(x => x.F050302).ToList();
                    var unAllotOrderMsgList = new List<string>();
                    var orderItems = f050302s.Select(x => new { x.ORD_NO, x.ITEM_CODE, x.ORD_SEQ, x.ORD_QTY });
                    var allotedStockOrderItems = allotedStockOrders.Select(x => new { x.F050302.ORD_NO, x.F050302.ITEM_CODE, x.F050302.ORD_SEQ, x.F050302.ORD_QTY });
                    var hasNoStocksItem = from o in orderItems
                                          join a in allotedStockOrderItems
                                          on new { o.ORD_NO, o.ITEM_CODE, o.ORD_SEQ } equals new { a.ORD_NO, a.ITEM_CODE, a.ORD_SEQ } into g
                                          from a in g.DefaultIfEmpty()
                                          where a == null
                                          group o by new { o.ORD_NO, o.ITEM_CODE } into g2
                                          select new { g2.Key.ORD_NO, g2.Key.ITEM_CODE, OutOfStockQty = g2.Sum(x => x.ORD_QTY) };
                    foreach (var item in hasNoStocksItem)
                    {
                        //訂單「{0}」商品「{1}」訂購「{2}」庫存不足無法出貨
                        var message = string.Format(_commonService.GetMsg("AAM00022"), item.ORD_NO, item.ITEM_CODE, item.OutOfStockQty);
                        unAllotOrderMsgList.Add(message);
                    }
                    if (unAllotOrderMsgList.Any())
                        messageList.Add("AAM00022", unAllotOrderMsgList);
                    #endregion
                }

                foreach (var item in messageList)
                    AddLog(oscr.DcCode, oscr.GupCode, oscr.CustCode, item.Key, string.Join(Environment.NewLine, item.Value), true);
            }
            _wmsLogHelper.AddRecord("產生訂單分配Log 結束");
        }
        #endregion

        #endregion

        #endregion

        #region 建立配庫後揀貨資料F050306
        /// <summary>
        /// 建立配庫後揀貨資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="isB2b"></param>
        /// <param name="sourceType"></param>
        /// <param name="custCost"></param>
        /// <param name="fastDealType"></param>
        /// <param name="allotedStockOrders"></param>
        /// <returns></returns>
        public List<F050306> CreateF050306List(string dcCode, string gupCode, string custCode, bool isB2b, List<F050301> f050301s, List<AllotedStockOrder> allotedStockOrders)
        {
            var f160204repo = new F160204Repository(Schemas.CoreSchema);
            var f050306List = new List<F050306>();
            foreach (var allotStockOrder in allotedStockOrders)
            {
                var f050301 = f050301s.First(x => x.ORD_NO == allotStockOrder.F050302.ORD_NO);

                String RtnVnrCode = "";
                if (f050301.SOURCE_TYPE == "13")//廠退出貨
                    RtnVnrCode = f160204repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301.SOURCE_NO && x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode).First().VNR_CODE;


                var f050306 = new F050306
                {
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    ORD_TYPE = isB2b ? "0" : "1",
                    SOURCE = "01",
                    SOURCE_TYPE = f050301.SOURCE_TYPE,
                    FAST_DEAL_TYPE = !string.IsNullOrWhiteSpace(f050301.USER_DIRECT_PRIORITY_CODE) ? f050301.USER_DIRECT_PRIORITY_CODE : f050301.FAST_DEAL_TYPE,
                    CUST_COST = f050301.CUST_COST,
                    WMS_NO = allotStockOrder.F050302.ORD_NO,
                    WMS_SEQ = allotStockOrder.F050302.ORD_SEQ,
                    WAREHOUSE_ID = allotStockOrder.WarehouseId,
                    WH_TMPR_TYPE = allotStockOrder.TmprType,
                    PICK_LOC = allotStockOrder.LocCode,
                    PICK_FLOOR = allotStockOrder.PickFloor,
                    DEVICE_TYPE = allotStockOrder.DeviceType,
                    ITEM_CODE = allotStockOrder.F050302.ITEM_CODE,
                    B_PICK_QTY = allotStockOrder.F050302.ORD_QTY,
                    ENTER_DATE = allotStockOrder.EnterDate,
                    VALID_DATE = allotStockOrder.ValidDate,
                    MAKE_NO = allotStockOrder.MakeNo,
                    BOX_CTRL_NO = allotStockOrder.BoxCtrlNo,
                    PALLET_CTRL_NO = allotStockOrder.PalletCtrlNo,
                    VNR_CODE = allotStockOrder.VnrCode,
                    SERIAL_NO = allotStockOrder.SerialNo,
                    SUG_BOX_NO = f050301.SUG_BOX_NO,
                    MOVE_OUT_TARGET = f050301.MOVE_OUT_TARGET,
                    PACKING_TYPE = f050301.PACKING_TYPE,
                    RTN_VNR_CODE = RtnVnrCode,
                    PK_AREA = String.IsNullOrWhiteSpace(allotStockOrder.PkArea) ? allotStockOrder.WarehouseId : allotStockOrder.PkArea,
                    PK_AREA_NAME = String.IsNullOrWhiteSpace(allotStockOrder.PkArea) ? allotStockOrder.WarehouseName : allotStockOrder.PKAreaName,
                    ORDER_CRT_DATE = f050301.ORDER_CRT_DATE,
                    ORDER_PROC_TYPE = f050301.ORDER_PROC_TYPE,
                    ORDER_ZIP_CODE = f050301.ORDER_ZIP_CODE,
                    IS_NORTH_ORDER = f050301.IS_NORTH_ORDER,
                    SUG_LOGISTIC_CODE = f050301.SUG_LOGISTIC_CODE,
                    NP_FLAG = f050301.NP_FLAG,
                };
                f050306List.Add(f050306);
            }
            return f050306List;
        }

        #endregion

        #endregion

        #region 配庫試算
        #region 取得未配庫訂單資料
        /// <summary>
        /// 指定配庫訂單
        /// </summary>
        /// <param name="ordNos"></param>
        /// <param name="isTrialCalculation">是否配庫試算</param>
        /// <param name="f050001s"></param>
        /// <param name="f050002s"></param>
        private void GetDirectOrders(List<string> ordNos, bool isTrialCalculation, ref List<F050001> f050001s, ref List<F050002> f050002s)
        {
            _wmsLogHelper.AddRecord("取得訂單池訂單開始");
            if (!ordNos.Any())
                return;

            var batchOrders = new List<List<string>>();
            if (ordNos.Count > _batchMaxCount)
            {
                var pages = ordNos.Count / _batchMaxCount + (ordNos.Count % _batchMaxCount > 0 ? 1 : 0);
                for (var page = 0; page < pages; page++)
                    batchOrders.Add(ordNos.Skip(page * _batchMaxCount).Take(_batchMaxCount).ToList());
            }
            else
                batchOrders.Add(ordNos);

            foreach (var batchOrder in batchOrders)
            {
                f050001s.AddRange(_f050001Repo.GetNotAllotedDatas(batchOrder).ToList());
                f050002s.AddRange(_f050002Repo.GetNotAllotedDatas(batchOrder).ToList());
            }
            _wmsLogHelper.AddRecord("取得訂單池訂單結束");
        }
        #endregion
        /// <summary>
        /// 配庫試算
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="ordNos">訂單編號</param>
        /// <returns></returns>
        public ExecuteResult AllotStockTrialCalculation(string dcCode, string gupCode, string custCode, List<string> ordNos)
        {
            _wmsLogHelper = new WmsLogHelper();
            _wmsLogHelper.StartRecord(WmsLogProcType.AllotStockTC);
            var allotedStockOrderList = new List<AllotedStockOrder>();
            var beforeAllotStockOrderDetails = new List<AllotStockOrderDetail>();
            var f050001s = new List<F050001>();
            var f050002s = new List<F050002>();

            GetDirectOrders(ordNos, true, ref f050001s, ref f050002s);
            // 將空字串的 MAKE_NO統一轉成Null
            // 將空字串的 SERIAL_NO統一轉成NULL
            if (f050002s != null)
            {
                f050002s.ForEach(a =>
                {
                    if (string.IsNullOrEmpty(a.MAKE_NO))
                    {
                        a.MAKE_NO = null;
                    }
                    if (string.IsNullOrEmpty(a.SERIAL_NO))
                    {
                        a.SERIAL_NO = null;
                    }
                });
            }
            var splitOrders = SplitBatchOrder(f050001s, f050002s, true);
            var calNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var splitOrder in splitOrders)
            {
                var splitOrdNos = splitOrder.Select(x => x.ORD_NO).ToList();
                var splitOrderDetails = f050002s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && splitOrdNos.Contains(x.ORD_NO)).ToList();
                //排除明細設定不為不出貨
                splitOrderDetails = splitOrderDetails.Where(x => x.NO_DELV == "0").ToList();
                // 區分有無批號商品訂單(有批號的訂單先配庫)
                var splitHasMakeNoOrSerialNos = SplitOrdersByHasMakeNoOrHasSerialNo(splitOrder, splitOrderDetails);
                foreach (var splitHasMakeNoOrSerialNoOrder in splitHasMakeNoOrSerialNos)
                {

                    _pickUsedAssignationSerials = new Dictionary<string, int>();
                    var f050301s = new List<F050301>();
                    var f050302s = new List<F050302>();

                    var splitHasMakeNoOrSerialNoOrdNos = splitHasMakeNoOrSerialNoOrder.Select(x => x.ORD_NO).ToList();
                    var splitHasMakeNoOrSerialNoOrderDetails = f050002s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && splitHasMakeNoOrSerialNoOrdNos.Contains(x.ORD_NO)).ToList();

                    CopyOrderToAllocOrders(dcCode, gupCode, custCode, splitHasMakeNoOrSerialNoOrder, splitHasMakeNoOrSerialNoOrderDetails, ref f050301s, ref f050302s);

                    var allotDetails = (from m in f050301s
                                        join d in f050302s
                                        on new { m.DC_CODE, m.GUP_CODE, m.CUST_CODE, m.ORD_NO } equals new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.ORD_NO }
                                        select new AllotStockOrderDetail
                                        {
                                            F050301 = m,
                                            F050302 = d,
                                            CarPeriod = "Z"
                                        }).ToList();
                    beforeAllotStockOrderDetails.AddRange(allotDetails);

                    var allotedStockOrders = ExecAllotStockOrders(dcCode, gupCode, custCode, f050301s, f050302s, true, calNo);
                    allotedStockOrderList.AddRange(allotedStockOrders);
                }
            }

            CreateAllotStockTrialCalculationLog(dcCode, gupCode, custCode, calNo, beforeAllotStockOrderDetails, allotedStockOrderList);
            _wmsLogHelper.StopRecord();
            return new ExecuteResult(true, "", calNo);

        }

        /// <summary>
        /// 建立試算Log
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="calNo"></param>
        /// <param name="allotStockOrderDetails"></param>
        /// <param name="allotedStockOrderList"></param>
        private void CreateAllotStockTrialCalculationLog(string dcCode, string gupCode, string custCode, string calNo, List<AllotStockOrderDetail> beforeAllotStockOrderDetails, List<AllotedStockOrder> allotedStockOrderList)
        {
            var f05080503Repo = new F05080503Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05080504Repo = new F05080504Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05080505Repo = new F05080505Repository(Schemas.CoreSchema, _wmsTransaction);
            var f05080506Repo = new F05080506Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var f050301dcCode = beforeAllotStockOrderDetails.FirstOrDefault()?.F050301.DC_CODE;

            var f1980s = f1980Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f050301dcCode).ToList();
            #region 試算頭檔
            var beforeOrdNos = beforeAllotStockOrderDetails.Select(x => x.F050301.ORD_NO).Distinct().ToList();
            var afterOrdNos = allotedStockOrderList.Select(x => x.F050302.ORD_NO).Distinct().ToList();
            var beforeRetailCodes = beforeAllotStockOrderDetails.Where(x => !string.IsNullOrWhiteSpace(x.F050301.RETAIL_CODE)).Select(x => x.F050301.RETAIL_CODE).Distinct().ToList();
            var afterRetailCodes = beforeAllotStockOrderDetails.Where(x => !string.IsNullOrWhiteSpace(x.F050301.RETAIL_CODE) && afterOrdNos.Contains(x.F050301.ORD_NO)).Select(x => x.F050301.RETAIL_CODE).Distinct().ToList();
            var beforeItemCodes = beforeAllotStockOrderDetails.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
            var afterItemCodes = allotedStockOrderList.Select(x => x.F050302.ITEM_CODE).Distinct().ToList();
            var beforeDelvQty = beforeAllotStockOrderDetails.Select(x => x.F050302.ORD_QTY).Sum();
            var afterDelvQty = allotedStockOrderList.Select(x => x.F050302.ORD_QTY).Sum();

            var f05080503 = new F05080503
            {
                DC_CODE = dcCode,
                GUP_CODE = gupCode,
                CUST_CODE = custCode,
                CAL_NO = calNo,
                TTL_B_ORD_CNT = beforeOrdNos.Count,
                TTL_A_ORD_CNT = afterOrdNos.Count,
                TTL_B_RETAIL_CNT = beforeRetailCodes.Count,
                TTL_A_RETAIL_CNT = afterRetailCodes.Count,
                TTL_B_ITEM_CNT = beforeItemCodes.Count,
                TTL_A_ITEM_CNT = afterItemCodes.Count,
                TTL_B_DELV_QTY = beforeDelvQty,
                TTL_A_DELV_QTY = afterDelvQty,
                TTL_A_SHELF_CNT = 0
            };

            #endregion

            #region 試算各儲區出貨占比
            var addF05080504List = new List<F05080504>();
            var g = allotedStockOrderList.GroupBy(x => new { x.WarehouseId, x.AreaCode });
            foreach (var item in g)
            {
                var f05080504 = new F05080504
                {
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    CAL_NO = calNo,
                    WAREHOUSE_ID = item.Key.WarehouseId,
                    AREA_CODE = item.Key.AreaCode,
                    DELV_QTY = item.Sum(x => x.F050302.ORD_QTY),
                };
                f05080504.DELV_RATIO = (float)Math.Round(((float)f05080504.DELV_QTY / (float)f05080503.TTL_A_DELV_QTY), 2);
                addF05080504List.Add(f05080504);
            }

            //針對加總比例不足1 將最大比例加上差異
            var sumDelvRatio = addF05080504List.Sum(x => x.DELV_RATIO);
            if (addF05080504List.Any() && sumDelvRatio < 1)
            {
                var diff = 1 - sumDelvRatio;
                var item = addF05080504List.OrderByDescending(x => x.DELV_RATIO).First();
                item.DELV_RATIO += diff;
            }
            //針對加總比例大於1 將最大比例減掉差異
            else if (addF05080504List.Any() && sumDelvRatio > 1)
            {
                var diff = sumDelvRatio - 1;
                var item = addF05080504List.OrderByDescending(x => x.DELV_RATIO).First();
                item.DELV_RATIO -= diff;
            }
            #endregion

            var gupbeforeAllotStockOrderDetails = beforeAllotStockOrderDetails.GroupBy(
                    a => new
                    {
                        a.F050301.DC_CODE,
                        a.F050301.GUP_CODE,
                        a.F050301.CUST_CODE,
                        a.F050301.ORD_NO,
                        a.F050301.CUST_ORD_NO,
                        a.F050301.CUST_COST,
                        a.F050301.FAST_DEAL_TYPE,
                        a.F050301.MOVE_OUT_TARGET,
                    });

            var addF05080505 = new List<F05080505>();
            var addF05080506 = new List<F05080506>();
            foreach (var gupbeforeItem in gupbeforeAllotStockOrderDetails)
            {
                //把倉別編號WarehouseID轉成名稱
                //var allotedStockOrderListWarehouseIDs = allotedStockOrderList.Where(x => x.F050302.DC_CODE == groupItem.Key.DC_CODE && x.F050302.GUP_CODE == groupItem.Key.GUP_CODE && x.F050302.CUST_CODE == groupItem.Key.CUST_CODE && x.F050302.ORD_NO == groupItem.Key.ORD_NO).Select(x => new { x.F050302.DC_CODE, x.WarehouseId }).Distinct();
                var allotedStockOrderListWarehouseIDs = allotedStockOrderList.Where(x => x.F050302.DC_CODE == gupbeforeItem.Key.DC_CODE && x.F050302.GUP_CODE == gupbeforeItem.Key.GUP_CODE && x.F050302.CUST_CODE == gupbeforeItem.Key.CUST_CODE && x.F050302.ORD_NO == gupbeforeItem.Key.ORD_NO);


                var ConvertWarehouseIDToName = f1980s.Where(x => allotedStockOrderListWarehouseIDs.Any(a => a.F050302.DC_CODE == x.DC_CODE && a.WarehouseId == x.WAREHOUSE_ID)).Select(x => x.WAREHOUSE_NAME);

                addF05080505.Add(new F05080505
                {
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    CAL_NO = calNo,
                    ORD_NO = gupbeforeItem.Key.ORD_NO,
                    CUST_ORD_NO = gupbeforeItem.Key.CUST_ORD_NO,
                    CUST_COST = gupbeforeItem.Key.CUST_COST,
                    FAST_DEAL_TYPE = gupbeforeItem.Key.FAST_DEAL_TYPE,
                    MOVE_OUT_TARGET = gupbeforeItem.Key.MOVE_OUT_TARGET,
                    WAREHOUSE_INFO = String.Join(",", ConvertWarehouseIDToName),
                    IS_LACK_ORDER = gupbeforeItem.Sum(x => x.F050302.ORD_QTY) - allotedStockOrderListWarehouseIDs.Sum(x => x.F050302.ORD_QTY) > 0 ? "1" : "0",
                });

                var gupBeforeItemCode = gupbeforeItem.GroupBy(x => x.F050302.ITEM_CODE).Select(x => new { ITEM_CODE = x.Key, ORD_QTY = x.Sum(a => a.F050302.ORD_QTY) });
                foreach (var gupbeforeDetail in gupBeforeItemCode)
                {
                    var ConvertWarehouseIDToNameByITEM = new List<string>().AsEnumerable();
                    var allotedStockOrderListWarehouseIDByITEM = allotedStockOrderListWarehouseIDs.Where(x => x.F050302.ITEM_CODE == gupbeforeDetail.ITEM_CODE);

                    if (allotedStockOrderListWarehouseIDByITEM != null)
                        ConvertWarehouseIDToNameByITEM = from a in f1980s
                                                         join b in allotedStockOrderListWarehouseIDByITEM.Select(x => new { x.F050302.DC_CODE, WAREHOUSE_ID = x.WarehouseId }).Distinct()
                                                                on new { a.DC_CODE, a.WAREHOUSE_ID } equals new { b.DC_CODE, b.WAREHOUSE_ID }
                                                         select a.WAREHOUSE_NAME;
                    addF05080506.Add(new F05080506()
                    {
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        CAL_NO = calNo,
                        ORD_NO = gupbeforeItem.Key.ORD_NO,
                        ITEM_CODE = gupbeforeDetail.ITEM_CODE,
                        WAREHOUSE_INFO = String.Join(",", ConvertWarehouseIDToNameByITEM),
                        B_QTY = gupbeforeDetail.ORD_QTY,
                        A_QTY = allotedStockOrderListWarehouseIDByITEM?.Sum(x => x.F050302.ORD_QTY) ?? 0,
                        IS_LACK = gupbeforeDetail.ORD_QTY - (allotedStockOrderListWarehouseIDByITEM?.Sum(x => x.F050302.ORD_QTY) ?? 0) > 0 ? "1" : "0",
                    });
                }
            }


            f05080504Repo.BulkInsert(addF05080504List);
            f05080503Repo.Add(f05080503);
            f05080505Repo.BulkInsert(addF05080505);
            f05080506Repo.BulkInsert(addF05080506);
        }
        #endregion

    }
}
