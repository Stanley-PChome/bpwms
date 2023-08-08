using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P20.Services;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Process.P14.Services
{
	public partial class P140103Service
	{
		private WmsTransaction _wmsTransaction;
		public P140103Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F140106QueryData> GetF140106QueryData(string dcCode, string gupCode, string custCode, DateTime? inventoryDateS, DateTime? inventoryDateE, string inventoryNo, string procWmsNo, string itemCode, string checkTool)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			return f140101Repo.GetF140106QueryData(dcCode, gupCode, custCode, inventoryDateS, inventoryDateE, inventoryNo, procWmsNo, itemCode, checkTool);
		}

		public IQueryable<F1913Data> GetF140106QueryDetailData(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			return f140101Repo.GetF140106QueryDetailData(dcCode, gupCode, custCode,inventoryNo);

		}


		public IQueryable<ImportInventorySerial> CheckImorImportInventorySerial(string dcCode, string gupCode, string custCode,
			string inventoryNo, List<ImportInventorySerial> importInventorySerials)
		{
			if (!importInventorySerials.Any())
				return new List<ImportInventorySerial>().AsQueryable();
			var list = new List<ImportInventorySerial>();
			var f140104Repo = new F140104Repository(Schemas.CoreSchema);
			var resultData = f140104Repo.GetInventoryDiffLocItemQties(dcCode, gupCode, custCode, inventoryNo).Where(o => o.BUNDLE_SERIALLOC == "1");
			var groupByImport = (from o in importInventorySerials
									 group o by new {o.LOC_CODE,o.ITEM_CODE} into g 
									 select new { g.Key.LOC_CODE,g.Key.ITEM_CODE,SerialCount = g.Count(),data=g}).ToList();
			var groupByresult = (from o in resultData
													 select new { o.LOC_CODE,o.ITEM_CODE, SerialCount = o.INVENTORY_QTY }).ToList();
            //var requestItem = groupByresult.Where(o=> !groupByImport.Any(c=> c.LOC_CODE == o.LOC_CODE && c.ITEM_CODE == o.ITEM_CODE)).ToList();
            //var noRequestItem = groupByImport.Where(o => !groupByresult.Any(c => c.LOC_CODE == o.LOC_CODE && c.ITEM_CODE == o.ITEM_CODE)).ToList();
            //if (requestItem.Any() || noRequestItem.Any())
            //{
            //	var message = requestItem.Any() ? Properties.Resources.P140103Service_RequestItem + string.Join(",", requestItem.Select(o => o.ITEM_CODE).Distinct().ToArray()) + Properties.Resources.P140103Service_RequestItemCode : "";
            //	if (message.Any() && noRequestItem.Any())
            //		message += Environment.NewLine;
            //	message += noRequestItem.Any() ? Properties.Resources.P140103Service_NoRequestItem + string.Join(",", noRequestItem.Select(o => o.ITEM_CODE).Distinct().ToArray()) : "";
            //	list.Add(new ImportInventorySerial { IsSuccess = false, Message = message });
            //	return list.AsQueryable();
            //}
            var noRequestItem = groupByImport.Where(o => !groupByresult.Any(c => c.LOC_CODE == o.LOC_CODE && c.ITEM_CODE == o.ITEM_CODE)).ToList();
            if (noRequestItem.Any())
            {
                var message = noRequestItem.Any() ? Properties.Resources.P140103Service_NoRequestItem + string.Join(",", noRequestItem.Select(o => o.ITEM_CODE).Distinct().ToArray()) : "";
                list.Add(new ImportInventorySerial { IsSuccess = false, Message = message });
                return list.AsQueryable();
            }

            var noEnough = groupByresult.Where(o => !groupByImport.Any(c => c.LOC_CODE == o.LOC_CODE && c.ITEM_CODE == o.ITEM_CODE && c.SerialCount == o.SerialCount)).ToList();
			if (noEnough.Any())
			{
				var message = Properties.Resources.P140103Service_NoEnough + string.Join(",", noEnough.Select(o => o.ITEM_CODE).Distinct().ToArray()) + Properties.Resources.P140103Service_NoEnoughItemCode;
				list.Add(new ImportInventorySerial { IsSuccess = false, Message = message });
				return list.AsQueryable();
			}
			var f14010101Repo = new F14010101Repository(Schemas.CoreSchema);
			var data = f14010101Repo.GetDatasByHasSerialNo(dcCode, gupCode, custCode, inventoryNo).ToList();
			var serialNoService = new SerialNoService();
			foreach (var importData in groupByImport)
			{
				var item = resultData.First(o => o.LOC_CODE == importData.LOC_CODE && o.ITEM_CODE == importData.ITEM_CODE);
				var serialData = data.Where(o => o.LOC_CODE == importData.LOC_CODE && o.ITEM_CODE == importData.ITEM_CODE).Select(o=>o.SERIAL_NO).ToList();
				var addSerial = new List<string>();
				if (item.WORK_TYPE == "0") //盤盈
				{
					if(!serialData.All(o => importData.data.Any(c => c.SERIAL_NO == o))) //必須匯入的序號都要在此盤點單內 否則回傳訊息
					{
						list.Clear();
						list.Add(new ImportInventorySerial { IsSuccess = false, Message = Properties.Resources.P140103Service_ITEM_CODE+importData.ITEM_CODE+Properties.Resources.P140103Service_InventorySurplus_NeedSaveItemCodeInInventoryList });
						return list.AsQueryable();
					}
					addSerial.AddRange(importData.data.Where(o=> serialData.All(c=> c != o.SERIAL_NO)).Select(o=>o.SERIAL_NO));
					
				}
				if (item.WORK_TYPE == "1" && !importData.data.All(o => serialData.Any(c => c == o.SERIAL_NO))) //盤損
				{
					list.Clear();
					list.Add(new ImportInventorySerial { IsSuccess = false, Message = Properties.Resources.P140103Service_ITEM_CODE + importData.ITEM_CODE + Properties.Resources.P140103Service_InventoryLoss_NeedSaveItemCodeInInventoryList });
					return list.AsQueryable();
				}
				var query = serialNoService.CheckLargeSerialNoFull(dcCode, gupCode, custCode, importData.ITEM_CODE,
										addSerial.ToArray(), "A1");

				var failure = query.FirstOrDefault(x => !x.Checked);
				if (failure != null)
				{
					list.Clear();
					list.Add(new ImportInventorySerial { IsSuccess = false, Message = failure.Message });
					return list.AsQueryable();
				}
				list.AddRange(importData.data);
			}
			foreach (var importInventorySerial in list)
				importInventorySerial.IsSuccess = true;
			return list.AsQueryable();
		}

		public IQueryable<F1913Data> GetInventoryDetailData(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransaction);
			var isSec = f140105Repo.GetDatasByTrueAndCondition(o=>o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.INVENTORY_NO == inventoryNo).Any();
			return f140101Repo.GetInventoryDetailData(dcCode, gupCode, custCode, inventoryNo, isSec);
		}

		public IQueryable<InventoryDoc> GetInventoryDoc(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			var f191302Repo = new F191302Repository(Schemas.CoreSchema);
			var excludeWmsNos = f191302Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SRC_WMS_NO == inventoryNo)
				.Where(x => !string.IsNullOrWhiteSpace(x.PROC_WMS_NO))
				.Select(x => x.PROC_WMS_NO)
				.Distinct()
				.ToList();
			return f140101Repo.GetInventoryDoc(dcCode, gupCode, custCode, inventoryNo, excludeWmsNos);
		}

		//盤點過帳
		public ExecuteResult UpdateF140101PostingStauts(string dcCode, string gupCode, string custCode, string inventoryNo, List<ImportInventorySerial> importInventorySerials)
		{
			var check = CheckImorImportInventorySerial(dcCode, gupCode, custCode, inventoryNo, importInventorySerials);
			if(check.Any() && !check.First().IsSuccess)
				return new ExecuteResult(false,check.First().Message);

			//Step:
			//1 更新主檔 Status / 過帳日期
			//2 新增 F140107 (盤盈損)		
			//3 新增調整單 & F1913 庫存量 ; 若盤盈不可回沖時 :是更新或新增 管理倉庫存
			//4 若盤盈: 盤盈可回沖: 產生調撥單 

			var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f140107Repo = new F140107Repository(Schemas.CoreSchema, _wmsTransaction);
			var p200101Srv = new P200101Service(_wmsTransaction);
			var sharedSrv = new SharedService(_wmsTransaction);
			var result = new ExecuteResult { IsSuccessed = false };

			//抓取 1913 相關資料
			var resultData = f140101Repo.GetF140106QueryDetailData(dcCode, gupCode, custCode, inventoryNo);
			// 先找貨主的儲位 -> 若無，找業主共用的儲位 -> 若無，找DC共用的儲位 取管理倉第一個倉位
			var firstLoc = sharedSrv.GetSuggestLocByFirst(gupCode, custCode, dcCode, 0, "M", "A", "");

			#region Step:1 更新 F140101 主檔 Status & 過帳日期

			var f140101Item = f140101Repo.Find(o => o.INVENTORY_NO == inventoryNo && o.DC_CODE == dcCode
												&& o.GUP_CODE == gupCode && o.CUST_CODE == custCode);
			if (f140101Item != null)
			{
				f140101Item.STATUS = "5";
				f140101Item.POSTING_DATE = DateTime.Now;
				f140101Repo.Update(f140101Item);
				result.IsSuccessed = true;
			}
			#endregion

			//盤點有差異才做以下動作
			if (resultData != null && resultData.Any())
			{
				#region Step:2 新增 F140107
				var addF140107List = new List<F140107>();
				foreach (var item in resultData)
				{
					F140107 f140107 = new F140107
					{
						INVENTORY_NO = inventoryNo,
						WAREHOUSE_ID = item.WAREHOUSE_ID,
						LOC_CODE = item.LOC_CODE,
						ITEM_CODE = item.ITEM_CODE,
						VALID_DATE = item.VALID_DATE,
						ENTER_DATE = item.ENTER_DATE,
						PROFIT_QTY = item.WORK_TYPE == "0" ? (int)item.ADJ_QTY_IN : 0, // 調入=盤盈
						LOSS_QTY = item.WORK_TYPE == "1" ? (int)item.ADJ_QTY_OUT : 0, // 調出=盤損
						FLUSHBACK = item.IS_FLUSHBACK,
						DC_CODE = item.DC_CODE,
						GUP_CODE = item.GUP_CODE,
						CUST_CODE = item.CUST_CODE,
						BOX_CTRL_NO = item.BOX_CTRL_NO,
						PALLET_CTRL_NO = item.PALLET_CTRL_NO,
						MAKE_NO = item.MAKE_NO
					};
					addF140107List.Add(f140107);
				}
				f140107Repo.BulkInsert(addF140107List);
				#endregion

				#region Step:3 新增調整單 與 更新 F1913
				var f14010101Repo = new F14010101Repository(Schemas.CoreSchema);
				var existSerialData = f14010101Repo.GetDatasByHasSerialNo(f140101Item.DC_CODE, f140101Item.GUP_CODE,
					f140101Item.CUST_CODE, f140101Item.INVENTORY_NO).ToList();
				var wcfDataDetail = new List<KeyValuePair<int, SerialNoResult[]>>();
				var datas = new List<F1913Data>();
				if (resultData.Any())
				{
					if (resultData.Any(o=>o.WORK_TYPE=="0" && o.IS_FLUSHBACK=="0") && (firstLoc == null || firstLoc.F1912 == null))
					{
						result.Message = Properties.Resources.P140103Service_LocNotFound;
						result.IsSuccessed = false;
						return result;
					}
					foreach (var item in resultData)
					{
						//取得此次盤點單此儲位此商品此效期此入庫日的序號
						var itemSerial = existSerialData.Where(o=>o.LOC_CODE == item.LOC_CODE && o.ITEM_CODE == item.ITEM_CODE && o.VALID_DATE == item.VALID_DATE && o.ENTER_DATE == item.ENTER_DATE && o.BOX_CTRL_NO == item.BOX_CTRL_NO && o.PALLET_CTRL_NO == item.PALLET_CTRL_NO && o.MAKE_NO == item.MAKE_NO).Select(c=>c.SERIAL_NO).ToList();
						//取得匯入此盤點單序號
						var importSerial = importInventorySerials.Where(o=>o.LOC_CODE == item.LOC_CODE && o.ITEM_CODE == item.ITEM_CODE).Select(c=>c.SERIAL_NO).ToList();
						//取得此盤點單此儲位此商品無差異的序號
						var okSerial = existSerialData.Where(o => o.LOC_CODE == item.LOC_CODE && o.ITEM_CODE == item.ITEM_CODE && !resultData.Any(c=>c.LOC_CODE == o.LOC_CODE && c.ITEM_CODE == o.ITEM_CODE && c.VALID_DATE == o.VALID_DATE && c.ENTER_DATE == o.ENTER_DATE && c.BOX_CTRL_NO == o.BOX_CTRL_NO && c.PALLET_CTRL_NO == o.PALLET_CTRL_NO && o.MAKE_NO == item.MAKE_NO)).Select(c=>c.SERIAL_NO).ToList();
						importSerial = importSerial.Except(okSerial).ToList();

						//盤盈
						if (item.WORK_TYPE == "0")
						{
							//盤盈新增序號
							var addSerial = importSerial.Except(itemSerial);
							if (addSerial.Any())
							{
								var serialData = addSerial.Select(serial => new SerialNoResult
								{
									SerialNo = serial,
									ItemCode = item.ITEM_CODE,
									Checked = true,
									boxCtrlNo = item.BOX_CTRL_NO,
									palletCtrlNo = item.PALLET_CTRL_NO,
									MakeNo = item.MAKE_NO
								}).ToList();
								var itemKeyValue = new KeyValuePair<int, SerialNoResult[]>(item.ROWNUM, serialData.ToArray());
								wcfDataDetail.Add(itemKeyValue);
							}
							//若有盤盈不回沖時 更新 F1913儲位變更為 管理儲位
							if (item.IS_FLUSHBACK == "0")
							{
								// 更新 倉別/儲位為管理類型
								item.LOC_CODE = firstLoc.F1912.LOC_CODE;
								item.WAREHOUSE_ID = firstLoc.F1912.WAREHOUSE_ID;
								var findItem = datas.FirstOrDefault(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.LOC_CODE == item.LOC_CODE && x.ITEM_CODE == item.ITEM_CODE && x.VALID_DATE == item.VALID_DATE && x.ENTER_DATE == item.ENTER_DATE && x.VNR_CODE == item.VNR_CODE && x.BOX_CTRL_NO == item.BOX_CTRL_NO && x.PALLET_CTRL_NO == item.PALLET_CTRL_NO && x.MAKE_NO == item.MAKE_NO);
								if (findItem == null)
									datas.Add(item);
								else
								{
									findItem.ADJ_QTY_IN += item.ADJ_QTY_IN;
									findItem.ADJ_QTY_OUT += item.ADJ_QTY_OUT;
								}
									
							}
							else
								datas.Add(item);
						}
						else //盤損
						{
							var removeSerial = itemSerial.Except(importSerial);
							if (removeSerial.Any())
							{
								var serialData = removeSerial.Select(serial => new SerialNoResult
								{
									SerialNo = serial,
									ItemCode = item.ITEM_CODE,
									Checked = true,
									boxCtrlNo = item.BOX_CTRL_NO,
									palletCtrlNo = item.PALLET_CTRL_NO,
									MakeNo = item.MAKE_NO
								}).ToList();
								var itemKeyValue = new KeyValuePair<int, SerialNoResult[]>(item.ROWNUM, serialData.ToArray());
								wcfDataDetail.Add(itemKeyValue);
							}
							datas.Add(item);
						}
					}
				}

				// 呼叫共用 Function 產生調整單 & 更新 F1913
				result = p200101Srv.InsertP200101ByAdjustType1(datas, wcfDataDetail.ToArray(), "2");

				#endregion
			}
			return result;

		}
	}
}
