using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
	public class OutWarehouseContainerReceiptService : BaseService
	{
		private WmsTransaction _wmsTransaction;
		private F151001Repository _f151001Repo;
		private F151002Repository _f151002Repo;
		private F1511Repository _f1511Repo;
		private F151003Repository _f151003Repo;
		private SharedService _sharedService;
		private ContainerService _containerService;
		/// <summary>
		/// 出庫結果回傳更新(按箱) 入口點
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult OutWarehouseContainerReceipt(WcsImportReq req)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			List<ApiResponse> data = new List<ApiResponse>();

			res = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSSH_F009005, req.DcCode, req.GupCode, req.GupCode, "OutboundContainerConfirm", req, () =>
			{
				CommonService commonService = new CommonService();
				var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
				dcCustList.ForEach(item =>
							{
								var result = OutboundContainerConfirm(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
								data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
							});
				res.Data = JsonConvert.SerializeObject(data);
				return res;
			}, true);

			return res;
		}

		/// <summary>
		/// 資料處理1
		/// </summary>
		/// <returns></returns>
		private ApiResult OutboundContainerConfirm(string dcCode, string gupCode, string custCode)
		{
			ApiResult res = new ApiResult { IsSuccessed = true };
			_wmsTransaction = new WmsTransaction();

			_f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			_f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			_f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			_f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			_sharedService = new SharedService(_wmsTransaction);
			_containerService = new ContainerService(_wmsTransaction);

			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f060207Repo = new F060207Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060207s = f060207Repo.GetDatasByNoProcess(dcCode, gupCode, custCode).ToList();
			int successCnt = 0;

			foreach (var f060207 in f060207s)
			{
				var warehouseDeviceType = f1980Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f060207.DC_CODE && x.WAREHOUSE_ID == f060207.WAREHOUSE_ID).FirstOrDefault()?.DEVICE_TYPE;
				if (warehouseDeviceType != "3") // 倉庫類型<>板進箱出倉(3)
				{
					f060207.STATUS = "3";
					f060207.MSG_CONTENT = "倉庫類型非板進箱出倉不處理此容器";
				}
				else // 倉庫類型=板進箱出倉(3)
				{
					var result = ProcessAllocation(f060207);
					if (result.IsSuccessed)
					{
						f060207.STATUS = "1";
						successCnt++;
					}
					else
					{
						f060207.STATUS = "2";
						f060207.MSG_CONTENT = result.Message;
					}
				}
				f060207Repo.Update(f060207);
				_wmsTransaction.Complete();
			}

			int failCnt = f060207s.Count - successCnt;
			res.MsgCode = "10005";

			res.MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫結果回傳(按箱)", successCnt, failCnt, f060207s.Count);
			res.TotalCnt = f060207s.Count;
			res.SuccessCnt = successCnt;
			res.FailureCnt = failCnt;

			return res;
		}


		#region 板進箱出倉容器處理

		/// <summary>
		/// 容器單據處理(板進箱出倉處理)
		/// </summary>
		/// <param name="f060207"></param>
		/// <returns></returns>
		private ExecuteResult ProcessAllocation(F060207 f060207)
		{
			var f06020701Repo = new F06020701Repository(Schemas.CoreSchema, _wmsTransaction);
			var containerDetails = f06020701Repo.GetContainerDetails(f060207.ID).ToList();

			var f1924Repo = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1924 = f1924Repo.Find(x => x.EMP_ID == f060207.OPERATOR && x.ISDELETED == "0");
			var empId = f1924 == null ? f060207.OPERATOR : f1924.EMP_ID;
			var empName = f1924 == null ? "支援人員" : f1924.EMP_NAME;
			// 依調撥單號排序
			var groupAllocations = containerDetails.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO }).OrderBy(x => x.Key.ALLOCATION_NO).ToList();
			var containerF151001s = new List<F151001>();
			var containerF151002s = new List<F151002>();
			var containerF1511s = new List<F1511>();

			#region Step1 檢查容器單據、單據明細

			var result = ContainerOrderCheck(ref containerF151001s, ref containerF151002s, ref containerF1511s, ref containerDetails);
			if (!result.IsSuccessed)
				return result;

			#endregion

			#region Step2 更新容器內下架調撥單

			ContainerOrderDownProcess(empId, empName, containerF151001s, containerF151002s, containerF1511s, ref containerDetails);

			#endregion

			#region Step3 產生容器調撥上架單
			var allotResult = ContainerUpProcess(f060207, empId, empName, containerDetails);
			if (!allotResult.Result.IsSuccessed)
				return allotResult.Result;
			#endregion

			var newAllocationNo = allotResult.AllocationList.First().Master.ALLOCATION_NO;

			#region Step4 產生容器資料

			var containerResult = CreateContainer(f060207, newAllocationNo, containerDetails);

			//更新調撥單容器ID
			allotResult.AllocationList.First().Master.F0701_ID = containerResult.f0701_ID;
			#endregion

			var bulkResult = _sharedService.BulkInsertAllocation(allotResult.AllocationList, allotResult.StockList, true);
			if (!bulkResult.IsSuccessed)
				return bulkResult;

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 容器內單據檢核
		/// </summary>
		/// <param name="containerF151001s"></param>
		/// <param name="containerF151002s"></param>
		/// <param name="containerF1511s"></param>
		/// <param name="containerDetails"></param>
		/// <returns></returns>
		private ExecuteResult ContainerOrderCheck(ref List<F151001> containerF151001s, ref List<F151002> containerF151002s, ref List<F1511> containerF1511s, ref List<ContainerDetail> containerDetails)
		{
			// 依調撥單號排序
			var groupAllocations = containerDetails.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO }).OrderBy(x => x.Key.ALLOCATION_NO).ToList();
			var isContainerCheckSuccess = true;
			var msg = string.Empty;
			foreach (var allocation in groupAllocations)
			{
				// 取得調撥頭檔
				var f151001 = _f151001Repo.Find(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ALLOCATION_NO == allocation.Key.ALLOCATION_NO);
				// 取得調撥身擋
				var f151002s = _f151002Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ALLOCATION_NO == allocation.Key.ALLOCATION_NO).ToList();
				var f1511s = _f1511Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ORDER_NO == allocation.Key.ALLOCATION_NO).ToList();
				if (f151001.STATUS == "9") // 已取消
				{
					// 23002 [單號{0}]單據己刪除
					msg = string.Format(_tacService.GetMsg("23002"), f151001.ALLOCATION_NO);
					isContainerCheckSuccess = false;
					break;
				}
				if (f151001.STATUS == "5") // 已結案
				{
					// 23003 [單號{0}]單據已結案
					msg = string.Format(_tacService.GetMsg("23003"), f151001.ALLOCATION_NO);
					isContainerCheckSuccess = false;
					break;
				}
				foreach (var allocationDetail in allocation)
				{
					var f151002 = f151002s.First(x => x.ALLOCATION_SEQ == allocationDetail.ALLOCATION_SEQ);
					if (f151002.STATUS == "2" || allocationDetail.QTY > f151002.SRC_QTY - f151002.A_SRC_QTY)
					{
						// 20047 [單號{0}]{1}超過下架數
						msg = string.Format(_tacService.GetMsg("20047"), f151001.ALLOCATION_NO, f151002.ITEM_CODE);
						isContainerCheckSuccess = false;
						break;
					}
				}
				containerF151001s.Add(f151001);
				containerF151002s.AddRange(f151002s);
				containerF1511s.AddRange(f1511s);
			}
			if (!isContainerCheckSuccess)
				return new ExecuteResult(false, msg);
			else
				return new ExecuteResult(true);
		}

		/// <summary>
		/// 容器內單據下架
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="allocationNo"></param>
		/// <param name="empId"></param>
		/// <param name="empName"></param>
		/// <param name="allocation"></param>
		/// <param name="containerF151001s"></param>
		/// <param name="containerF151002s"></param>
		/// <param name="containerF1511s"></param>
		private void ContainerOrderDownProcess(string empId, string empName, List<F151001> containerF151001s, List<F151002> containerF151002s, List<F1511> containerF1511s, ref List<ContainerDetail> containerDetails)
		{
			// 依調撥單號排序
			var groupAllocations = containerDetails.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ALLOCATION_NO }).OrderBy(x => x.Key.ALLOCATION_NO).ToList();
			foreach (var allocation in groupAllocations)
			{
				//此容器是否為該單據最後一箱(只要該單據有一筆明細設為最後一箱就認定他為最後一箱)
				var isLastBox = allocation.Max(x => x.ISLASTCONTAINER) == 1;
				var updF151002s = new List<F151002>();
				var updF1511s = new List<F1511>();
				var addF151003s = new List<F151003>();
				// 取得調撥頭檔
				var f151001 = containerF151001s.First(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ALLOCATION_NO == allocation.Key.ALLOCATION_NO);
				// 取得調撥身擋
				var f151002s = containerF151002s.Where(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ALLOCATION_NO == allocation.Key.ALLOCATION_NO).ToList();
				var f1511s = containerF1511s.Where(x => x.DC_CODE == allocation.Key.DC_CODE && x.GUP_CODE == allocation.Key.GUP_CODE && x.CUST_CODE == allocation.Key.CUST_CODE && x.ORDER_NO == allocation.Key.ALLOCATION_NO).ToList();

				foreach (var allocationDetail in allocation)
				{
					var f151002 = f151002s.First(x => x.ALLOCATION_SEQ == allocationDetail.ALLOCATION_SEQ);
					f151002.A_SRC_QTY += allocationDetail.QTY;
					f151002.SRC_STAFF = empId;
					f151002.SRC_NAME = empName;
					f151002.SRC_DATE = allocationDetail.COMPLETE_TIME;
					if (f151002.SRC_QTY == f151002.A_SRC_QTY)
						f151002.STATUS = "2";
					var f1511 = f1511s.First(x => x.ORDER_SEQ == allocationDetail.ALLOCATION_SEQ.ToString());
					f1511.A_PICK_QTY += allocationDetail.QTY;
					if (f1511.B_PICK_QTY == f1511.A_PICK_QTY)
						f1511.STATUS = "2";
					updF151002s.Add(f151002);
					updF1511s.Add(f1511);
					allocationDetail.F151002 = f151002;
				}
				if (isLastBox) //此單據最後一箱，進行單據結案並產生調撥下架缺貨紀錄
				{
					var notFinishDetails = f151002s.Where(x => x.STATUS == "0").ToList();
					foreach (var detail in notFinishDetails)
					{
						var lackQty = detail.SRC_QTY - detail.A_SRC_QTY;
						addF151003s.Add(new F151003
						{
							DC_CODE = detail.DC_CODE,
							GUP_CODE = detail.GUP_CODE,
							CUST_CODE = detail.CUST_CODE,
							ALLOCATION_NO = detail.ALLOCATION_NO,
							ALLOCATION_SEQ = detail.ALLOCATION_SEQ,
							LACK_QTY = (int)lackQty,
							ITEM_CODE = detail.ITEM_CODE,
							MOVE_QTY = (int)detail.SRC_QTY,
							LACK_TYPE = "0",
							REASON = "001",
							STATUS = "0",
						});
						detail.STATUS = "2";
						var f1511 = f1511s.First(x => x.ORDER_SEQ == detail.ALLOCATION_SEQ.ToString());
						f1511.STATUS = "2";
					}
					f151001.STATUS = "5";
					f151001.LOCK_STATUS = "4";
					f151001.POSTING_DATE = DateTime.Now;
					_f151001Repo.Update(f151001);
					_f151002Repo.BulkUpdate(f151002s);
					_f1511Repo.BulkUpdate(f1511s);
					_f151003Repo.BulkInsert(addF151003s, "LACK_SEQ");
					// 新增F191303 庫存跨倉移動紀錄表
					_sharedService.CrtSpanWhMoveLogByAlloc(f151001, f151002s);
					// 更新儲位容積
					_sharedService.UpdateAllocationLocVolumn(f151001, f151002s);

				}
				else
				{
					if (!f151001.SRC_START_DATE.HasValue)
					{
						f151001.SRC_START_DATE = allocation.Min(x => x.COMPLETE_TIME);
						f151001.SRC_MOVE_STAFF = empId;
						f151001.SRC_MOVE_NAME = empName;
						_f151001Repo.Update(f151001);
					}
					_f151002Repo.BulkUpdate(updF151002s);
					_f1511Repo.BulkUpdate(updF1511s);
				}
			}
		}

		/// <summary>
		/// 容器產生純上架調撥單
		/// </summary>
		/// <param name="f060207"></param>
		/// <param name="empId"></param>
		/// <param name="empName"></param>
		/// <param name="containerDetails"></param>
		private ReturnNewAllocationResult ContainerUpProcess(F060207 f060207, string empId, string empName, List<ContainerDetail> containerDetails)
		{

			// 取得第一筆容器明細的上架倉庫編號
			var targetWarehouseId = containerDetails.First().PRE_TAR_WAREHOUSE_ID;
			var allotParam = new NewAllocationItemParam
			{
				TarDcCode = f060207.DC_CODE,
				GupCode = f060207.GUP_CODE,
				CustCode = f060207.CUST_CODE,
				AllocationType = AllocationType.NoSource,
				AllocationTypeCode = "6",
				ATypeCode = "A",
				ContainerCode = f060207.CONTAINERCODE.ToUpper(),
				IsExpendDate = true,
				NotAllowSeparateLoc = true,
				ReturnStocks = new List<F1913>(),
				TarWarehouseId = targetWarehouseId,
				isIncludeResupply = false, //建議儲位不含補貨區儲位
				StockDetails = containerDetails.Select((x, index) => new StockDetail
				{
					DataId = index,
					TarDcCode = x.DC_CODE,
					GupCode = x.GUP_CODE,
					CustCode = x.CUST_CODE,
					ItemCode = x.ITEM_CODE,
					ValidDate = x.F151002.VALID_DATE,
					EnterDate = x.F151002.ENTER_DATE,
					VnrCode = x.F151002.VNR_CODE,
					MAKE_NO = x.F151002.MAKE_NO,
					SerialNo = string.IsNullOrEmpty(x.F151002.SERIAL_NO) ? "0" : x.F151002.SERIAL_NO,
					Qty = x.QTY,
					SourceType = "17",
					SourceNo = x.F151002.ALLOCATION_NO,
					BoxCtrlNo = x.F151002.BOX_CTRL_NO,
					PalletCtrlNo = x.F151002.PALLET_CTRL_NO,
					BinCode = x.BIN_CODE
				}).ToList()
			};
			return _sharedService.CreateOrUpdateAllocation(allotParam, false);
		}

		/// <summary>
		/// 產生容器資料
		/// </summary>
		/// <param name="f060207"></param>
		/// <param name="allocationNo"></param>
		/// <param name="containerDetails"></param>
		/// <returns></returns>
		private ContainerExecuteResult CreateContainer(F060207 f060207, string allocationNo, List<ContainerDetail> containerDetails)
		{
			var containerParams = containerDetails
				.Select(x => new CreateContainerParam
				{
					DC_CODE = x.DC_CODE,
					GUP_CODE = x.GUP_CODE,
					CUST_CODE = x.CUST_CODE,
					WAREHOUSE_ID = x.PRE_TAR_WAREHOUSE_ID,
					CONTAINER_CODE = f060207.CONTAINERCODE.ToUpper(),
					CONTAINER_TYPE = "0",
					WMS_NO = allocationNo,
					WMS_TYPE = "T",
					ITEM_CODE = x.ITEM_CODE,
					VALID_DATE =x.F151002.VALID_DATE,
					MAKE_NO = x.F151002.MAKE_NO,
					QTY = x.QTY,
					BIN_CODE = x.BIN_CODE,
					SERIAL_NO_LIST = x.SERIALNUMLIST
				}).ToList();
			var containerExecuteResult = _containerService.CreateContainer(containerParams);
			return containerExecuteResult.First();
		}
		#endregion

	}


}
