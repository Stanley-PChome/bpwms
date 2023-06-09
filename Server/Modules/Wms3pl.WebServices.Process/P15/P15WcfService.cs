using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P02.Services;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using ExecuteResult = Wms3pl.Datas.Shared.Entities.ExecuteResult;

namespace Wms3pl.WebServices.Process.P15.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P15WcfService
	{


		#region P150201 - 調撥單維護
		[OperationContract]
		public ExecuteResult UpdateF1510Data(string tarDcCode, string gupCode, string custCode, string allocationNo, List<F1510Data> datas)
		{

			var wmsTransaction = new WmsTransaction();
			var srv = new P020301Service(wmsTransaction);
			var result = srv.UpdateF1510Data(tarDcCode, gupCode, custCode, allocationNo, datas, true);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult InsertOrUpdateF151001Datas(F151001 masterData, List<F151001DetailDatas> detailData, string userId, string userName)
		{
			var LogName = masterData.ALLOCATION_NO;
			if (string.IsNullOrWhiteSpace(LogName))
				LogName = DateTime.Now.ToString("yyyyMMddHHmmss");

			Log(LogName, "調撥單更新開始");
			var wmsTransaction = new WmsTransaction();
			var sharedSrv = new SharedService(wmsTransaction);
			var sharedService = new SharedService(wmsTransaction);
			F1980Repository f1980Repo = new F1980Repository(Schemas.CoreSchema);
			if (!string.IsNullOrWhiteSpace(masterData.TAR_WAREHOUSE_ID))
			{
				Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查儲位溫層開始");
				foreach (var item in detailData)
				{
					//檢查商品上架儲位溫層
					var message = sharedService.CheckItemLocTmpr(masterData.TAR_DC_CODE, masterData.GUP_CODE, item.ITEM_CODE, masterData.CUST_CODE,
						item.TAR_LOC_CODE);
					if (!string.IsNullOrEmpty(message))
						return new ExecuteResult { IsSuccessed = false, Message = message };
				}
				Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查儲位溫層結束");
			}

			// 非自動倉 要檢核混品
			if(!f1980Repo.CheckAutoWarehouse(masterData.DC_CODE, masterData.TAR_WAREHOUSE_ID))
			{
				Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查混批混品開始");

				var checkMixRes = sharedService.CheckItemMixLoc(detailData.Select(x => new CheckItemTarLocAndParamsMixLoc
				{
					DcCode = x.DC_CODE,
					GupCode = x.GUP_CODE,
					CustCode = x.CUST_CODE,
					ItemCode = x.ITEM_CODE,
					TarLocCode = x.TAR_LOC_CODE
				}).ToList());
				if (!checkMixRes.IsSuccessed)
				{
					Log(LogName, checkMixRes.Message);
					Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查混批混品結束");
					return checkMixRes;
				}

        var checkMixItemRes = sharedService.CheckItemMixBatch(detailData.GroupBy(x=>new { x.CUST_CODE, x.GUP_CODE, x.ITEM_CODE, x.TAR_LOC_CODE }).Select(x => new CheckItemTarLocMixLoc
        {
          CustCode=x.Key.CUST_CODE,
          GupCode=x.Key.GUP_CODE,
          ItemCode=x.Key.ITEM_CODE,
          TarLocCode=x.Key.TAR_LOC_CODE,
          CountValidDate=x.Select(o=>o.VALID_DATE).Distinct().Count()
        }).ToList());
        if(!checkMixItemRes.IsSuccessed)
        {
          Log(LogName, checkMixItemRes.Message);
          Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查混批混品結束");
          return checkMixItemRes;
        }

        var checkStockMixItemRes = sharedService.CheckStockItemMixBatch(detailData
          .GroupBy(x => new { x.DC_CODE, x.CUST_CODE, x.GUP_CODE, x.ITEM_CODE, x.TAR_LOC_CODE, x.VALID_DATE })
          .Select(x => new CheckStockItemTarLocMixLoc
          {
            DcCode = x.Key.DC_CODE,
            CustCode = x.Key.CUST_CODE,
            GupCode = x.Key.GUP_CODE,
            ItemCode = x.Key.ITEM_CODE,
            TarLocCode = x.Key.TAR_LOC_CODE,
            ValidDate = x.Key.VALID_DATE,
          }).ToList());
        if (!checkStockMixItemRes.IsSuccessed)
        {
          Log(LogName, checkStockMixItemRes.Message);
          Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查混批混品結束");
          return checkStockMixItemRes;
        }

        Log(LogName, $"UI上人員選擇上架倉別[{ masterData.TAR_WAREHOUSE_ID ?? "" }]，檢查混批混品結束");
      }
			Log(LogName, "取得DB調撥單開始");
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			var f151001 = f151001Repo.Find(x => x.CUST_CODE == EntityFunctions.AsNonUnicode(masterData.CUST_CODE)
																&& x.GUP_CODE == EntityFunctions.AsNonUnicode(masterData.GUP_CODE)
																&& x.DC_CODE == EntityFunctions.AsNonUnicode(masterData.DC_CODE)
																&& x.ALLOCATION_NO == EntityFunctions.AsNonUnicode(masterData.ALLOCATION_NO));

			Log(LogName, "取得DB調撥單結束");

			Log(LogName, $"檢查是否為純下架單，UI人員調整的上架倉別為[{ masterData.TAR_WAREHOUSE_ID ?? "" }]");
			if (f151001 != null && string.IsNullOrWhiteSpace(masterData.TAR_WAREHOUSE_ID)) //純下架只更新Memo和調撥日期
			{
				f151001.ALLOCATION_DATE = masterData.ALLOCATION_DATE;
				f151001.MEMO = masterData.MEMO;
				f151001Repo.Update(f151001);
				wmsTransaction.Complete();
				Log(LogName, "純下架單只更新調撥日期與備註 DBCommit");
				return new ExecuteResult(true);
			}
			
			//調撥單狀態為3or4只更新上架物流中心、上架倉別與上架儲位
			if (f151001 != null && (f151001.STATUS == "3" || f151001.STATUS == "4" ||
        (f151001.STATUS == "8" && !string.IsNullOrWhiteSpace(f151001.SOURCE_NO) && f151001.SOURCE_TYPE == "04") ||
        (f151001.STATUS == "8" && !string.IsNullOrWhiteSpace(f151001.CONTAINER_CODE) && f151001.ALLOCATION_TYPE == "4")))
      {
				Log(LogName, "DB調撥單狀態[" + f151001.STATUS + "]為已下架處理或上架處理中或異常");
				// 複製一份現有資料庫調撥單資料
				Log(LogName, "備份原調撥單資料開始");
				var oldF151001 = JsonConvert.DeserializeObject<F151001>(JsonConvert.SerializeObject(f151001));
				Log(LogName, "備份原調撥單資料結束");

				f151001.ALLOCATION_DATE = masterData.ALLOCATION_DATE;
				f151001.MEMO = masterData.MEMO;
				f151001.TAR_DC_CODE = masterData.TAR_DC_CODE;
				f151001.TAR_WAREHOUSE_ID = masterData.TAR_WAREHOUSE_ID;

        if (f151001.STATUS == "8")
        {

          //SOURCE_NO!=null && SOURCE_TYPE == "04" 為商品檢驗異常處理
          //CONTAINER_CODE!=null && ALLOCATION_TYPE == "4" 為商品檢驗與容器綁定異常處理
          if ((!string.IsNullOrWhiteSpace(masterData.SOURCE_NO) && masterData.SOURCE_TYPE == "04") ||
            (!string.IsNullOrWhiteSpace(masterData.CONTAINER_CODE) && masterData.ALLOCATION_TYPE == "4"))
          {
            f151001.STATUS = "3";
            f151001.LOCK_STATUS = "2";
          }
          else
          {
            f151001.STATUS = "0";
            f151001.LOCK_STATUS = "0";
          }
					Log(LogName, "DB調撥單狀態[8]更新回新狀態[" + f151001.STATUS + "]");
				}

				Log(LogName, "更新調撥單明細開始");
				var f151002Repo = new F151002Repository(Schemas.CoreSchema, wmsTransaction);
				var data = f151002Repo.AsForUpdate()
					.GetDatas(f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO)
					.ToList();
				foreach (var detail in detailData)
				{
					var sugLocCode = !string.IsNullOrWhiteSpace(detail.SUG_LOC_CODE) ? detail.SUG_LOC_CODE : "000000000";
					var items = data.Where(o => o.ITEM_CODE == detail.ITEM_CODE && o.SUG_LOC_CODE == sugLocCode && o.PALLET_CTRL_NO == detail.PALLET_CTRL_NO &&
												o.BOX_CTRL_NO == detail.BOX_CTRL_NO && o.MAKE_NO == detail.MAKE_NO && o.VALID_DATE == detail.VALID_DATE &&
												o.ENTER_DATE == detail.ENTER_DATE && o.SRC_LOC_CODE == detail.SRC_LOC_CODE);
					foreach (var f151002 in items)
					{
            f151002.SUG_LOC_CODE = detail.TAR_LOC_CODE;
						f151002.TAR_LOC_CODE = detail.TAR_LOC_CODE;
            if (string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID)) { f151002.SRC_LOC_CODE = detail.TAR_LOC_CODE; }
						f151002Repo.Update(f151002);
					}
				}
				Log(LogName, "更新調撥單明細結束");

        if (f151001.STATUS == "3")
        {
          //修正如果原目的倉與新目的倉相同時(就是人員沒有變更目的倉)，請不要下發入庫取消任務與派新的入庫任務
          if (f151001.TAR_WAREHOUSE_ID != oldF151001.TAR_WAREHOUSE_ID)
          {
            Log(LogName, "DB調撥單狀態=3，下發入庫任務開始");
            // 如果新設定的目的倉為自動倉，下發入庫任務
            int insertCnt = sharedService.CreateInBoundJob(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.TAR_WAREHOUSE_ID);
            Log(LogName, "DB調撥單狀態=3，下發入庫任務結束");

            Log(LogName, $"備份調撥單上架倉別為[{ oldF151001.TAR_WAREHOUSE_ID ?? "" }]，產生入庫取消任務開始");
            if (!string.IsNullOrEmpty(oldF151001.TAR_WAREHOUSE_ID))
              // 如果原調撥單目的倉為自動倉，下發入庫任務取消
              sharedService.CreateInBoundCancelJob(oldF151001.TAR_DC_CODE, oldF151001.GUP_CODE, oldF151001.CUST_CODE, oldF151001.ALLOCATION_NO, oldF151001.TAR_WAREHOUSE_ID, insertCnt);
            Log(LogName, $"備份調撥單上架倉別為[{ oldF151001.TAR_WAREHOUSE_ID ?? "" }]，產生入庫取消任務結束");
          }
        }

				if(f151001.STATUS =="4")
				{
          //修正如果原目的倉與新目的倉相同時(就是人員沒有變更目的倉)，請不要下發入庫取消任務與派新的入庫任務
          if (f151001.TAR_WAREHOUSE_ID != oldF151001.TAR_WAREHOUSE_ID)
          {
            Log(LogName, $"DB調撥單狀態=4，備份上架倉別為[{ oldF151001.TAR_WAREHOUSE_ID ?? "" }]");
            if (!string.IsNullOrEmpty(oldF151001.TAR_WAREHOUSE_ID))
            {
              Log(LogName, $"DB調撥單狀態=4，即時取消入庫任務開始，備份上架倉別為[{ oldF151001.TAR_WAREHOUSE_ID ?? "" }]");
              // 原目的倉要發送任務取消，若接到取消成功，單據狀態改為3
              var isCancel = sharedService.InboundCancel(oldF151001.DC_CODE, oldF151001.GUP_CODE, oldF151001.CUST_CODE, oldF151001.ALLOCATION_NO, oldF151001.TAR_WAREHOUSE_ID);
              if (isCancel.IsSuccessed)
              {

                f151001.STATUS = "3";
                f151001.LOCK_STATUS = "2";
                f151001Repo.Update(f151001);
                Log(LogName, "DB調撥單狀態=4，即時取消入庫任務成功，更新調撥單為已下架處理");

                Log(LogName, $"DB調撥單狀態=4，產生新目的倉[{ f151001.TAR_WAREHOUSE_ID ?? "" }]入庫任務開始");

                // 如果新設定的目的倉為自動倉，下發入庫任務
                sharedService.CreateInBoundJob(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.TAR_WAREHOUSE_ID);
                Log(LogName, $"DB調撥單狀態=4，產生新目的倉[{ f151001.TAR_WAREHOUSE_ID ?? "" }]入庫任務結束");
              }
              else
              {
                if (isCancel.MsgCode == "99990")
                {
                  //如果F060101.STATUS=1，顯示訊息為"目前系統正在執行派發入庫任務，請稍後在修改"
                  Log(LogName, isCancel.MsgContent);
                  return new ExecuteResult { IsSuccessed = false, Message = isCancel.MsgContent };
                }
                else
                {
                  Log(LogName, $"自動倉已開始作業，不可變更\r\n[WCS 入庫指示取消] {isCancel.MsgContent}");
                  return new ExecuteResult { IsSuccessed = false, Message = $"自動倉已開始作業，不可變更\r\n[WCS 入庫指示取消] {isCancel.MsgContent}" };
                }
              }
            }
            else
            {
              Log(LogName, $"DB調撥單狀態=4，備份上架倉別為[{ oldF151001.TAR_WAREHOUSE_ID ?? "" }]，不下發取消任務，更新調撥單狀態為以下架處理");
              f151001.STATUS = "3";
              f151001.LOCK_STATUS = "2";
              f151001Repo.Update(f151001);
              Log(LogName, $"DB調撥單狀態=4，產生新目的倉[{ f151001.TAR_WAREHOUSE_ID ?? "" }]入庫任務開始");
              // 如果新設定的目的倉為自動倉，下發入庫任務
              sharedService.CreateInBoundJob(f151001.TAR_DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.TAR_WAREHOUSE_ID);
              Log(LogName, $"DB調撥單狀態=4，產生新目的倉[{ f151001.TAR_WAREHOUSE_ID ?? "" }]入庫任務結束");
            }
          }
				}
				f151001Repo.Update(f151001);
				wmsTransaction.Complete();
				Log(LogName, "DB Commit");
				Log(LogName, "調撥單更新結束");
				return new ExecuteResult(true);
			}

			Log(LogName, "檢查DB調撥單狀態[" + masterData.STATUS + "]，執行配庫鎖定，重建調撥單開始");

			// 建立/修改調撥單時，如果調撥單狀態=0 or 1 or 8，要進行配庫鎖定，再呼叫建立調撥單，建立完成調撥單後再將配庫鎖定解鎖
			var isPass = false;
            var stockService = new StockService();
            var returnNewAllocationResult = new ReturnNewAllocationResult();
            var lockStatus = new List<string> { "0", "1", "8" };
			      string allotBatchNo=string.Empty;
            try
            {
				        var itemCodes = detailData.Select(x => new ItemKey { DcCode =x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
								allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
				        Log(LogName, "配庫鎖定開始");
                if ((masterData != null && lockStatus.Contains(masterData.STATUS)) ||
                    (f151001 != null && lockStatus.Contains(f151001.STATUS)))
                    isPass = stockService.CheckAllotStockStatus(false, allotBatchNo,itemCodes);
                
                if (!isPass)
								{
									Log(LogName, "仍有程序正在配庫調撥單所配庫商品，請稍待再配庫");
									return new ExecuteResult { IsSuccessed = false, Message = "仍有程序正在配庫調撥單所配庫商品，請稍待再配庫" };
								}
                else
                {
                    var newAllocationItemParam = new NewAllocationItemParam
                    {
                        GupCode = masterData.GUP_CODE,
                        CustCode = masterData.CUST_CODE,
                        AllocationDate = masterData.ALLOCATION_DATE,
                        PostingDate = masterData.POSTING_DATE,
                        SourceType = masterData.SOURCE_TYPE,
                        SourceNo = masterData.SOURCE_NO,
                        Memo = masterData.MEMO,
                        SendCar = masterData.SEND_CAR == "1",
                        IsExpendDate = masterData.ISEXPENDDATE == "1",
                        SrcDcCode = masterData.SRC_DC_CODE,
                        SrcWarehouseId = masterData.SRC_WAREHOUSE_ID,
                        TarDcCode = masterData.TAR_DC_CODE,
                        TarWarehouseId = masterData.TAR_WAREHOUSE_ID,
                        AllocationType = AllocationType.Both,
                        IsDeleteOrginalAllocation = f151001 != null,
                        OrginalAllocationNo = f151001 != null ? f151001.ALLOCATION_NO : "",
                        SrcStockFilterDetails = detailData.Select((x, index) => new StockFilter
                        {
                            DataId = index,
                            ItemCode = x.ITEM_CODE,
                            LocCode = x.SRC_LOC_CODE,
                            Qty = (decimal)x.SRC_QTY,
                            ValidDates = x.VALID_DATE.HasValue ? new List<DateTime> { x.VALID_DATE.Value } : new List<DateTime>(),
                            EnterDates = x.ENTER_DATE.HasValue ? new List<DateTime> { x.ENTER_DATE.Value } : new List<DateTime>(),
                            BoxCtrlNos = !string.IsNullOrWhiteSpace(x.BOX_CTRL_NO) ? new List<string> { x.BOX_CTRL_NO } : new List<string>(),
                            PalletCtrlNos = !string.IsNullOrWhiteSpace(x.PALLET_CTRL_NO) ? new List<string> { x.PALLET_CTRL_NO } : new List<string>(),
                            VnrCodes = !string.IsNullOrWhiteSpace(x.VnrCode) ? new List<string> { x.VnrCode } : new List<string>(),
                            SerialNos = !string.IsNullOrWhiteSpace(x.SerialNo) && x.SerialNo != "0" ? new List<string> { x.SerialNo } : new List<string>(),
                            isAllowExpiredItem = true,
                            MakeNos = !string.IsNullOrWhiteSpace(x.MAKE_NO) ? new List<string> { x.MAKE_NO } : new List<string>()
                        }).ToList(),
                        SrcLocMapTarLocs = detailData.Select((x, index) => new SrcLocMapTarLoc
                        {
                            DataId = index,
                            ItemCode = x.ITEM_CODE,
                            SrcLocCode = x.SRC_LOC_CODE,
                            TarLocCode = x.TAR_LOC_CODE,
                            TarVnrCode = x.TarVnrCode,

                        }).ToList(),
                        IsCheckExecStatus = !isPass
                    };
										Log(LogName, "呼叫調撥單共用函數-建立or重建調撥單");
                    returnNewAllocationResult = sharedService.CreateOrUpdateAllocation(newAllocationItemParam);
                    if (returnNewAllocationResult.Result.IsSuccessed)
                    {
                        sharedService.BulkInsertAllocation(returnNewAllocationResult.AllocationList, returnNewAllocationResult.StockList);
                        wmsTransaction.Complete();
						          if(LogName!=masterData.ALLOCATION_NO)
							          	Log(LogName, $"建立調撥單{returnNewAllocationResult.AllocationList.First().Master.ALLOCATION_NO}成功，DBCommit");
											else
												Log(LogName, "重建調撥單成功，DBCommit");
										}
										else
												Log(LogName, "重建or重建調撥單失敗，"+ returnNewAllocationResult.Result.Message);
										}
            }
            finally
            {
								if (isPass)
								{
									stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
									Log(LogName, "配庫鎖定結束");
								}
			}
			Log(LogName, "調撥單新增or更新結束");
			return returnNewAllocationResult.Result;
		}

        [OperationContract]
        public ExecuteResult DeleteF151001Datas(P1502010000Data f151001)
        {
            var service = new P150201Service(new WmsTransaction());
            return service.DeleteF151001Datas(f151001);
        }

        [OperationContract]
        public F151001ReportDataByTicket GetF151001ReportDataByTicket(P1502010000Data f151001)
        {
            var service = new P150201Service();
            return service.GetF151001ReportDataByTicket(f151001);
        }
        #endregion


        #region 呼叫共用function 來更新 來源單據狀態 (調撥)
        [OperationContract]
		public ExecuteResult UpdateSourceNoStatus(F151001 f151001)
		{
			// 呼叫共用function 來更新 來源單據狀態
			var wmsTransaction = new WmsTransaction();
			var sharedSrv = new SharedService(wmsTransaction);
			ExecuteResult result = new ExecuteResult { IsSuccessed = true };

			var dataResult = sharedSrv.UpdateSourceNoStatus(SourceType.Allocation, f151001.DC_CODE, f151001.GUP_CODE, f151001.CUST_CODE, f151001.ALLOCATION_NO, f151001.STATUS);

			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion


		#region 序號收集
		[OperationContract]
		public ExecuteResult ImportSerialNo(string dcCode, string gupCode, string custCode, string itemCode, List<SerialNoResult> serialNoResultList, string changeStatus, string wmsNo = null, string vnrCode = null, DateTime? validDate = null)
		{
			var wmsTransaction = new WmsTransaction();
			var serialNoService = new SerialNoService(wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
			var addF2501List = new List<F2501>();
			var updF2501List = new List<F2501>();
			var delSnList = new List<string>();
			foreach (var serialNoResult in serialNoResultList)
			{
				var snResults = serialNoService.CheckSerialNoFull(dcCode, gupCode, custCode, itemCode, serialNoResult.SerialNo, changeStatus);
				if (snResults.First().Checked)
				{
					foreach (var snResult in snResults)
					{
						var result = serialNoService.UpdateSerialNoFull(ref addF2501List,ref updF2501List,ref delSnList,dcCode, gupCode, custCode, changeStatus, snResult, wmsNo, vnrCode,
							validDate);
						if (!result.IsSuccessed)
							return result;
					}
				}
				else
					return new ExecuteResult { IsSuccessed = false, Message = snResults.First().Message };
			}
			if (delSnList.Any())
				f2501Repo.DeleteBySnList(gupCode, custCode, delSnList);
			if (addF2501List.Any())
				f2501Repo.BulkInsert(addF2501List);
			if (updF2501List.Any())
				f2501Repo.BulkUpdate(updF2501List);
			wmsTransaction.Complete();
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		#endregion

		#region Import 資料
		[OperationContract]
		public ExecuteResult ImportF150201Data(string gupCode, string custCode
											 , string fileName, List<F150201ImportData> importData)
		{

			var wmsTransaction = new WmsTransaction();
			var srv = new P150201Service(wmsTransaction);
			var result = srv.ImportF150201Data(gupCode, custCode, fileName, importData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;

		}
		#endregion


		[OperationContract]
		public AddAllocationSuggestLocResult GetSuggestLocByF1913WithF1912MovedList(F151001 master, List<F1913WithF1912Moved> f1913WithF1912Moveds)
		{
			var srv = new P150201Service();
			return srv.GetSuggestLocByF1913WithF1912MovedList(master, f1913WithF1912Moveds);
		}


		[OperationContract]
    public ExecuteResult FinishedOffShelf(string dcCode, string gupCode, string custCode, string allocationNo)
    {
      WmsTransaction wmsTransaction = new WmsTransaction();
      var srv = new P150201Service(wmsTransaction);
      var result = srv.FinishedOffShelf(dcCode, gupCode, custCode, allocationNo);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    [OperationContract]
    public ExecuteResult FinishedOffShelfWithLack(string dcCode, string gupCode, string custCode, string allocationNo, List<P1502010500Data> p1502010500Data)
    {
      WmsTransaction wmsTransaction = new WmsTransaction();
      var srv = new P150201Service(wmsTransaction);
      var result = srv.FinishedOffShelfWithLack(dcCode, gupCode, custCode, allocationNo, p1502010500Data);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

		public void Log(string allocationNo,string message, bool isShowDatetime = true)
		{
			var commonService = new CommonService();
			var isSetLog = commonService.GetSysGlobalValue("IsWriteLog") == "1";
			if(isSetLog)
			{
				var _logFilePath = ConfigurationManager.AppSettings["LogFilePath"] + "P150201\\";
				if (!Directory.Exists(_logFilePath))
					Directory.CreateDirectory(_logFilePath);

				var fileFullName = Path.Combine(_logFilePath, $"{allocationNo}.txt");

				try
				{
					using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
						sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
				}
				catch (Exception)
				{
					fileFullName = Path.Combine(_logFilePath, $"{allocationNo}_{Guid.NewGuid()}.txt");
					using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
						sw.WriteLine(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") : string.Empty, message));
				}
			}
		}
	}
}

