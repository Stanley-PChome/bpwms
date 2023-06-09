
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.Edm;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.GoogleMapAddress;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F16;
using System.Text.RegularExpressions;

namespace Wms3pl.WebServices.Process.P20.Services
{
	public partial class P200101Service
	{
		private WmsTransaction _wmsTransaction;

		public P200101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F200101Data> GetF200101Datas(string dcCode, string gupCode, string custCode, string adjustNo,
			string adjustType, string workType, string begAdjustDate, string endAdjustDate)
		{
			var f200101Repo = new F200101Repository(Schemas.CoreSchema);
			return f200101Repo.GetF200101Datas(dcCode, gupCode, custCode, adjustNo, adjustType, workType, begAdjustDate, endAdjustDate);
		}

		public IQueryable<F200101Data> GetF200101DatasByAdjustType1Or2(string dcCode, string gupCode, string custCode, string adjustNo,
			string adjustType, string workType, string begAdjustDate, string endAdjustDate)
		{
			var f200101Repo = new F200101Repository(Schemas.CoreSchema);
			return f200101Repo.GetF200101DatasByAdjustType1Or2(dcCode, gupCode, custCode, adjustNo, adjustType, workType, begAdjustDate, endAdjustDate);
		}

		#region 訂單調整
		public IQueryable<F200102Data> GetF200102Datas(string dcCode, string gupCode, string custCode, string adjustNo, string workType)
		{
			var f200102Repo = new F200102Repository(Schemas.CoreSchema);
			var f200102 = f200102Repo.GetF200102Datas(dcCode, gupCode, custCode, adjustNo);
			if (workType != "5")
				return f200102;
			else
			{
				var adjustSql = f200102.Select(o => o.ADJUST_SEQ.ToString()).ToList();
				var notAllotStock = f200102Repo.GetF200102DatasNotF050801F050301(dcCode, gupCode, custCode, adjustNo, adjustSql);
				var allData = f200102.ToList().Union(notAllotStock);
				return allData.AsQueryable();
			}
		}

		public IQueryable<F050301Data> GetF050301Datas(string dcCode, string gupCode, string custCode, string delvDate,
			string pickTime, string custOrdNo, string itemCode, string consignee, string ordNo, string workType)
		{
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			return f050301Repo.GetF050301Datas(dcCode, gupCode, custCode, delvDate, pickTime, custOrdNo, itemCode, consignee, ordNo, workType);
		}

		public IQueryable<F0513Data> GetF0513Datas(string dcCode, string gupCode, string custCode, string delvDate)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema);
			return f0513Repo.GetF0513Datas(dcCode, gupCode, custCode, delvDate);
		}

		public ExecuteResult BeforeInsertP200101ByAdjustType0Check(List<F050301Data> f050301Datas, string workType)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f200102Repo = new F200102Repository(Schemas.CoreSchema);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var adjustList = new List<string>();
			var notCancelOrdNos = new List<string>();// 不可取消的訂單清單
			var notCancelStatusList = new List<decimal> { 2, 5, 6 };// 不可取消的F050801.STATUS 2(已包裝)或5(已裝車)或6(已扣帳)

			foreach (var f050301Data in f050301Datas)
			{
				var f200102Item =
					f200102Repo.Filter(
						o =>
							o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE &&
							o.WORK_TYPE == workType && o.ORD_NO == f050301Data.ORD_NO && o.STATUS != "9").FirstOrDefault();
				if (f200102Item != null)
					adjustList.Add(f200102Item.ADJUST_NO);

				#region 檢核是否有出貨單狀態為[F050801.STATUS] = 2(已包裝)或5(已裝車)或6(已扣帳)，若有提示訊息[訂單XXX(多筆用逗點分隔)，已包裝或已出貨不可取消]，若已經在稽核出庫作業也不可取消
				var ordStatusData = f050301Repo.GetOrdNoStatusData(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO);
				if (ordStatusData.Where(x => notCancelStatusList.Contains(x.STATUS)).Any())
					notCancelOrdNos.Add(f050301Data.ORD_NO);
				else
				{
					var hasAudit = f051202Repo.AnyWmsOrdIntAudit(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO);
					if (hasAudit)
					{
						notCancelOrdNos.Add(f050301Data.ORD_NO);
					}
				}
				#endregion


			}
			if (adjustList.Count > 0)
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P200101Service_HasSameWorkTypeAdjustNo + Environment.NewLine + Properties.Resources.P200101Service_AdjustNo + string.Join("、", adjustList.Distinct().ToArray());
			}
			else
			{
				if (notCancelOrdNos.Any())
				{
					result.IsSuccessed = false;
					result.Message = string.Format(Properties.Resources.P200101Service_NotCancelOrdNos, string.Join("、", notCancelOrdNos.Distinct().ToArray()));
				}
				else
				{
					//workType = 5 [修改出貨地址]需更改配庫前核准後訂單所以不作此判斷

					if (workType == "0" || workType == "3")
					{
						var tmpf050301Datas = f050301Datas.Select(AutoMapper.Mapper.DynamicMap<F050301Data>).ToList();
						foreach (var f050301Data in tmpf050301Datas)
						{
							var f050301WmsNoData = f050301Repo.GetF050301WmsNoData(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO).ToList();
							var relationNoCancelOrdList = new List<string>();
							foreach (var item in f050301WmsNoData)
							{
								var f050301NewData = f050301Datas.Where(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE
																		&& o.ORD_NO == item.ORD_NO).FirstOrDefault();
								//未在原本 f050301Datas 時, 加入item 置 f050301Datas
								if (f050301NewData == null && relationNoCancelOrdList.All(c => c != item.ORD_NO))
								{
									relationNoCancelOrdList.Add(item.ORD_NO);
								}
							}
							if (relationNoCancelOrdList.Any())
							{
								result.IsSuccessed = false;
								for (int i = 0; i < relationNoCancelOrdList.Count; i++)
									if (i % 2 == 0 && i != relationNoCancelOrdList.Count - 1)
										relationNoCancelOrdList[i] = relationNoCancelOrdList[i] + Environment.NewLine;
								result.Message += f050301Data.ORD_NO + Properties.Resources.P200101Service_RelationNoOrd_NeedCheck + Environment.NewLine + string.Join("、", relationNoCancelOrdList.ToArray()) + Environment.NewLine;
							}
						}
					}
					else if (workType == "1" || workType == "2") //1產生新批次 2修改配送資訊 =>檢查是否已開始揀貨 如果已揀貨不允許調整
					{
						var isStartPick = false;
						foreach (var f050301Data in f050301Datas)
						{
							var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
							var items =
								f05030101Repo.Filter(
									o => o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE && o.ORD_NO == f050301Data.ORD_NO).ToList();
							foreach (var f05030101 in items)
							{
								isStartPick = f051202Repo.Filter(
								 o =>
									 o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
									 o.WMS_ORD_NO == f05030101.WMS_ORD_NO && (o.PICK_STATUS == "1" || o.A_PICK_QTY > 0)).Any();
								if (isStartPick)
									break;
							}
							if (isStartPick)
								break;
						}
						if (isStartPick)
						{
							result.IsSuccessed = false;
							result.Message = Properties.Resources.P200101Service_OrdisStartPick;
						}
					}
					else if (workType == "4") //自取 如果原訂單為已自取 則不可以新增
					{

						foreach (var f050301Data in f050301Datas)
						{
							var f050301Item =
								f050301Repo.Find(
									o =>
										o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE &&
										o.ORD_NO == f050301Data.ORD_NO);
							if (f050301Item.SELF_TAKE == "1")
							{
								result.IsSuccessed = false;
								result.Message = Properties.Resources.P200101Service_OrdIisSelfTakeCannotAdjust;
								break;
							}
						}

					}
				}
			}

			return result;
		}

		public ExecuteResult InsertP200101ByAdjustType0(List<F050301Data> f050301Datas, string workType, string allId, string allTime, string address, string newDcCode,
			string cause, string causeMemo)
		{
            var orderService = new OrderService(_wmsTransaction);

            var firstF050301Data = f050301Datas.First();

            return orderService.CancelAllocStockOrder(
                firstF050301Data.DC_CODE, 
                firstF050301Data.GUP_CODE, 
                firstF050301Data.CUST_CODE, 
                f050301Datas.Select(x => x.ORD_NO).ToList(),
                workType,
                allId,
                allTime,
                address,
                newDcCode,
                cause,
                causeMemo);
		}

		/// <summary>
		/// 將還沒揀貨的數量回復至庫存儲位中
		/// </summary>
		/// <param name="f050301"></param>
		/// <returns></returns>
		public ExecuteResult RestoreNonPickF051202ToF1913(string dcCode, string gupCode, string custCode, List<string> ordList)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202s = f05030101Repo.GetNonPickF051202ByOrdNo(dcCode, gupCode, custCode, ordList);
			// 取得虛擬儲位
			var f1511s = f1511Repo.GetDatasByF051202(dcCode, gupCode, custCode, f051202s);
			// 取得庫存資料
			var f1913s = f1913Repo.GetDatasForF051202s(f051202s);
			var updF1913s = new List<F1913>();

			var datas = (from C in f051202s
									join A in f1511s
									on new { NO = C.PICK_ORD_NO, SEQ = C.PICK_ORD_SEQ } equals new { NO = A.ORDER_NO, SEQ = A.ORDER_SEQ }
									select new{ F051202 = C, F1511 = A }).ToList();

			foreach (var data in datas)
			{
				if (data.F1511.STATUS == "0")
				{
					// F1913 的序號預設都是 0 ，F051202 的序號預設都是 NULL，這樣可讓兩邊比較相等
					if (string.IsNullOrEmpty(data.F051202.SERIAL_NO))
						data.F051202.SERIAL_NO = "0";

					bool isAddUpdData = false;
					F1913 f1913 = null;
					if (updF1913s.Any())
					{
						f1913 = updF1913s.Where(x => x.LOC_CODE == data.F051202.PICK_LOC
												&& x.ITEM_CODE == data.F051202.ITEM_CODE
												&& x.SERIAL_NO == data.F051202.SERIAL_NO
												&& x.VALID_DATE == data.F051202.VALID_DATE
												&& x.ENTER_DATE == data.F051202.ENTER_DATE
												&& x.DC_CODE == data.F051202.DC_CODE
												&& x.GUP_CODE == data.F051202.GUP_CODE
												&& x.CUST_CODE == data.F051202.CUST_CODE
												&& x.VNR_CODE == data.F051202.VNR_CODE
												&& x.BOX_CTRL_NO == data.F051202.BOX_CTRL_NO
												&& x.PALLET_CTRL_NO == data.F051202.PALLET_CTRL_NO
												&& x.MAKE_NO == data.F051202.MAKE_NO).FirstOrDefault();
					}

					if(f1913 == null)
					{
						f1913 = f1913s.Where(x => x.LOC_CODE == data.F051202.PICK_LOC
						&& x.ITEM_CODE == data.F051202.ITEM_CODE
						&& x.SERIAL_NO == data.F051202.SERIAL_NO
						&& x.VALID_DATE == data.F051202.VALID_DATE
						&& x.ENTER_DATE == data.F051202.ENTER_DATE
						&& x.DC_CODE == data.F051202.DC_CODE
						&& x.GUP_CODE == data.F051202.GUP_CODE
						&& x.CUST_CODE == data.F051202.CUST_CODE
						&& x.VNR_CODE == data.F051202.VNR_CODE
						&& x.BOX_CTRL_NO == data.F051202.BOX_CTRL_NO
						&& x.PALLET_CTRL_NO == data.F051202.PALLET_CTRL_NO
						&& x.MAKE_NO == data.F051202.MAKE_NO).FirstOrDefault();
						isAddUpdData = true;
					}

					// 將尚未揀完的直接還回庫存，若已揀的則用虛擬儲位回復產生調撥單還庫存
					var restoreQty = data.F051202.B_PICK_QTY - data.F051202.A_PICK_QTY;
					if (f1913 == null)
					{
						// 這裡會有可能進來嗎? 不確定如果庫存為0後，資料是否會有被刪除的可能，這邊就防止這種事情發生。
						f1913 = new F1913
						{
							LOC_CODE = data.F051202.PICK_LOC,
							ITEM_CODE = data.F051202.ITEM_CODE,
							SERIAL_NO = data.F051202.SERIAL_NO,
							VALID_DATE = data.F051202.VALID_DATE,
							ENTER_DATE = data.F051202.ENTER_DATE,
							DC_CODE = data.F051202.DC_CODE,
							GUP_CODE = data.F051202.GUP_CODE,
							CUST_CODE = data.F051202.CUST_CODE,
							VNR_CODE = data.F051202.VNR_CODE,
							QTY = restoreQty,
							MAKE_NO = data.F051202.MAKE_NO,
							BOX_CTRL_NO = data.F051202.BOX_CTRL_NO,
							PALLET_CTRL_NO = data.F051202.PALLET_CTRL_NO
						};
						f1913Repo.Add(f1913);
					}
					else
					{
						f1913.QTY += restoreQty;
						if(isAddUpdData)
							updF1913s.Add(f1913);
					}

					// 更新揀貨明細改為9
					data.F051202.PICK_STATUS = "9";
					f051202Repo.Update(data.F051202);
				}
			}

			if (updF1913s.Any())
				f1913Repo.BulkUpdate(updF1913s);

			// 找出需要更新的F051201
			var pickOrdNos = f051202s.GroupBy(x => x.PICK_ORD_NO)
				.Select(x => new { PickOrdNo = x.Key, IsUpd = x.All(z => z.PICK_STATUS == "9") })
				.Where(x => x.IsUpd)
				.Select(x => x.PickOrdNo)
				.ToList();

			// 更新揀貨單改為9
			if (pickOrdNos.Any())
			{
				var f051201s = f051201Repo.GetDatas(dcCode, gupCode, custCode, pickOrdNos).ToList();
				f051201s.ForEach(f051201 => { f051201.PICK_STATUS = 9; });
				f051201Repo.BulkUpdate(f051201s);
			}

			return new ExecuteResult(true);
		}

		public ExecuteResult UpdateP200101ByAdjustTye0(string dcCode, string gupCode, string custCode, string adjustNo, int adjustSeq, string workType, string address, string newDcCode,
			string cause, string causeMemo)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f200102Repo = new F200102Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f200102Repo.Filter(
				o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo && o.ADJUST_SEQ == adjustSeq).FirstOrDefault();
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050301Item = f050301Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == item.ORD_NO);
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			if (!f050301Repo.IsOrderNotPackage(dcCode, gupCode, custCode, item.ORD_NO))
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P200101Service_OrdIsOrderNotPackage_CannotEdit;
				return result;
			}
			if (workType == "3")
			{
				var f050001Item = f050001Repo.GetF050001sByNoProcFlag9(item.GUP_CODE, item.CUST_CODE, item.NEW_ORD_NO);
				if (f050001Item.PROC_FLAG != "0")
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P200101Service_OrdIsTransfered_CannotEdit;
					return result;
				}
			}
			switch (workType)
			{
				case "0": //訂單取消
					item.CAUSE = cause;
					item.CAUSE_MEMO = causeMemo;
					break;
				case "1": //產生新批次
					item.CAUSE = cause;
					item.CAUSE_MEMO = causeMemo;
					break;
				case "2": //修改配送資訊
					item.ADDRESS = address;
					item.CAUSE = cause;
					item.CAUSE_MEMO = causeMemo;
					f050301Item.ADDRESS = address;
					f050301Repo.Update(f050301Item);
					break;
				case "3": //修改出貨物流中心
					item.NEW_DC_CODE = newDcCode;
					item.CAUSE = cause;
					item.CAUSE_MEMO = causeMemo;
					var f050001Item =
						f050001Repo.Find(
							o => o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE &&
								o.ORD_NO == item.NEW_ORD_NO);
					f050001Item.DC_CODE = newDcCode;
					f050001Repo.Update(f050001Item);
					break;
				case "4": //自取
				case "5": //修改出貨地址
					item.CAUSE = cause;
					item.CAUSE_MEMO = causeMemo;
					break;
			}
			f200102Repo.Update(item);
			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200101Item = f200101Repo.Find(
				o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo);
			f200101Item.UPD_DATE = DateTime.Now;
			f200101Repo.Update(f200101Item);

			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };
			return result;
		}

		public ExecuteResult DeleteP200101ByAdjustType0(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var f200102Repo = new F200102Repository(Schemas.CoreSchema, _wmsTransaction);
			var items = f200102Repo.Filter(
			o =>
				o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
				o.STATUS != "9").ToList();
			foreach (var f200102 in items)
			{
				var result = DeleteP200101DetailByAdjustType0(dcCode, gupCode, custCode, f200102.ADJUST_NO, f200102.ADJUST_SEQ);
				if (!result.IsSuccessed)
					return result;
			}

			// 復原為非自選的訂單
			var f0530101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F05030101> f05030101s = new List<F05030101>();
			foreach (var ordNo in items.Select(x => x.ORD_NO))
			{
				f05030101s.AddRange(f0530101Repo.GetMergerOrders(dcCode, gupCode, custCode, ordNo));
			}
			UndoF700102WithF050901(dcCode, gupCode, custCode, f05030101s.Select(x => x.WMS_ORD_NO).Distinct().ToList());



			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200101Item = f200101Repo.Find(
				o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo);
			f200101Item.STATUS = "9";
			f200101Repo.Update(f200101Item);

			return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult DeleteP200101DetailByAdjustType0Selected(string dcCode, string gupCode, string custCode, string adjustNo,
			int adjustSeq)
		{
			var f200102Repo = new F200102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200102Entity = f200102Repo.Find(
				o =>
					o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
					o.ADJUST_SEQ == adjustSeq);

			// 復原為非自選的訂單
			var f0530101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			List<F05030101> f05030101s = f0530101Repo.GetMergerOrders(dcCode, gupCode, custCode, f200102Entity.ORD_NO).ToList();
			UndoF700102WithF050901(dcCode, gupCode, custCode, f05030101s.Select(x => x.WMS_ORD_NO).Distinct().ToList());

			var ordNods = f05030101s.Select(x => x.ORD_NO).Distinct().ToList();


			var items = f200102Repo.Filter(
			o =>
				o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
				ordNods.Contains(o.ORD_NO) &&
				o.STATUS != "9").ToList();

			foreach (var f200102 in items)
			{
				var result = DeleteP200101DetailByAdjustType0(dcCode, gupCode, custCode, f200102.ADJUST_NO, f200102.ADJUST_SEQ);
				if (!result.IsSuccessed)
					return result;
			}

			//檢查是否已為最後一筆刪除資料 如果是 要將主檔設定狀態為刪除
			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);


			var seqForDeleteList = items.Select(x => x.ADJUST_SEQ).ToList();
			var q = f200102Repo.Filter(
				o =>
					o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
					o.STATUS != "9" && !seqForDeleteList.Contains(o.ADJUST_SEQ));
			if (!q.Any())
			{
				var f200101Item = f200101Repo.Find(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo);
				f200101Item.STATUS = "9";
				f200101Repo.Update(f200101Item);
			}


			return new ExecuteResult(true);
		}

		public ExecuteResult DeleteP200101DetailByAdjustType0(string dcCode, string gupCode, string custCode, string adjustNo,
			int adjustSeq)
		{
			var result = new ExecuteResult { IsSuccessed = true };

			var f200102Repo = new F200102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f200102Repo.Find(
				o =>
					o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
					o.ADJUST_SEQ == adjustSeq);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);

			var f050301Item = f050301Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == item.ORD_NO);
			if (!f050301Repo.IsOrderNotPackage(dcCode, gupCode, custCode, item.ORD_NO))
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P200101Service_OrdIsOrderNotPackage_CannotDelete;
				return result;
			}
			switch (item.WORK_TYPE)
			{
				case "0": //訂單取消
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P200101Service_WORK_TYPEIsCancel_CannotDelete;
					//item.STATUS = "9";
					//f050301Item.PROC_FLAG = item.ORG_STATUS;
					//f050301Repo.Update(f050301Item);
					break;
				case "1": //產生新批次
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P200101Service_WORK_TYPEIsGenNewBatch_CannotDelete;
					break;
				case "2": //修改配送資訊
					item.STATUS = "9";
					UpdateWorkType1Or2(dcCode, gupCode, custCode, item.ORD_NO, item.ORG_PICK_TIME);
					f050301Item.ADDRESS = item.ORG_ADDRESS;
					f050301Repo.Update(f050301Item);
					break;
				case "3": //修改出貨物流中心
					item.STATUS = "9";
					//var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050001Item = f050001Repo.AsForUpdate().GetF050001sByNoProcFlag9(item.GUP_CODE, item.CUST_CODE, item.NEW_ORD_NO);
					if (f050001Item.PROC_FLAG == "0")
					{
						f050001Item.PROC_FLAG = "9";
						f050001Repo.Update(f050001Item);
						f050301Item.PROC_FLAG = item.ORG_STATUS;
						f050301Repo.Update(f050301Item);
					}
					else
					{
						result.IsSuccessed = false;
						result.Message = Properties.Resources.P200101Service_OrdIsTransfered_CannotDelete;
					}
					break;
				case "4": //自取
					item.STATUS = "9";
					f050301Item.SELF_TAKE = "0";
					f050301Repo.Update(f050301Item);
					var f050801s = UpdateWorkType4(dcCode, gupCode, custCode, item.ORD_NO, selfTake: "0", printPass: "1");
					break;
				case "5":
					item.STATUS = "9";
					ChangeAddress(false, item.ORG_ADDRESS.Split(',')[1], item.ORG_ADDRESS.Split(',')[0], f050301Item, item, f050301Repo);
					break;
			}
			if (result.IsSuccessed)
			{
				f200102Repo.Update(item);
			}
			return result;
		}

		/// <summary>
		/// 更新地址
		/// </summary>
		/// <param name="isAdd">是否為新增調整單</param>
		/// <param name="address">更新的地址</param>
		/// <param name="zipCode">更新的郵遞區號</param>
		/// <param name="f050301Item">原始F050301資料</param>
		/// <param name="item">調整單明細資料</param>
		/// <param name="f050301Repo">F050301Repository</param>
		private void ChangeAddress(bool isAdd, string address, string zipCode, F050301 f050301Item, F200102 item, F050301Repository f050301Repo)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Rep = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			if (f050301Item != null)
			{
				//已配庫未出貨包裝
				var f05030101 = f05030101Rep.Find(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE && o.ORD_NO == item.ORD_NO);
				if (f05030101 != null)
				{
					//已配庫未出貨包裝資料還須更新派車單明細地址郵遞區號
					var f700102 = f700102Repo.Find(o => o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE && o.WMS_NO == f05030101.WMS_ORD_NO && o.ZIP_CODE == f050301Item.ZIP_CODE && o.ADDRESS == f050301Item.ADDRESS);
					if (f700102 != null)
					{
						f700102.ADDRESS = address;
						f700102.ZIP_CODE = zipCode;
						f700102Repo.Update(f700102);
					}
				}
				//新增F200102異動調整單明細+更新F050301
				if (isAdd)
				{
					item.ORG_ADDRESS = f050301Item.ZIP_CODE + "," + f050301Item.ADDRESS;
					item.ADDRESS = zipCode + "," + address;
				}
				f050301Item.ADDRESS = address;
				f050301Item.ZIP_CODE = zipCode;
				f050301Repo.Update(f050301Item);
			}
			else
			{
				//未配庫已核准
				var f050001Item = f050001Repo.Find(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE && o.ORD_NO == item.ORD_NO);
				if (f050001Item != null)
				{
					//F200102 調整單明細 沒有郵遞區號所以郵遞區號串在地址欄位用，分隔
					if (isAdd)
					{
						item.ORG_ADDRESS = f050001Item.ZIP_CODE + "," + f050001Item.ADDRESS;
						item.ADDRESS = zipCode + "," + address;
					}
					f050001Item.ADDRESS = address;
					f050001Item.ZIP_CODE = zipCode;
					f050001Repo.Update(f050001Item);
				}
			}
		}


		private void UpdateWorkType1Or2(string dcCode, string gupCode, string custCode, string ordNo, string newPickTime)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var items =
				f05030101Repo.Filter(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052901Repo = new F052901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			foreach (var f05030101 in items)
			{
				var f050801Item = f050801Repo.Filter(
					o =>
						o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
						o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f050801Item != null)
				{
					f050801Item.PICK_TIME = newPickTime;
					f050801Repo.Update(f050801Item);
				}
				var f052901Item =
					f052901Repo.Filter(
						o =>
							o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
							o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f052901Item != null)
				{
					f052901Item.PICK_TIME = newPickTime;
					f052901Repo.Update(f052901Item);
				}

				var f055001Item =
					f055001Repo.Filter(
						o =>
							o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
							o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f055001Item != null)
				{
					f055001Item.PICK_TIME = newPickTime;
					f055001Repo.Update(f055001Item);
				}

				var pickOrdNoList =
					f051202Repo.Filter(
						o =>
							o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
							o.WMS_ORD_NO == f05030101.WMS_ORD_NO).Select(o => o.PICK_ORD_NO).Distinct().ToList();
				foreach (var pickOrdNo in pickOrdNoList)
				{
					var f051201Item =
						f051201Repo.Filter(
							o =>
								o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
								o.PICK_ORD_NO == pickOrdNo).FirstOrDefault();
					if (f051201Item != null)
					{
						f051201Item.PICK_TIME = newPickTime;
						f051201Repo.Update(f051201Item);
					}
				}

			}

		}

		private string UpdateWorkType3(string dcCode, string gupCode, string custCode, string ordNo, string newDcCode, F050301 f050301)
		{
			var sharedService = new SharedService(_wmsTransaction);
			var newOrdNo = sharedService.GetNewOrdCode("S");
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001 = new F050001();
			f050001.GUP_CODE = f050301.GUP_CODE;
			f050001.CUST_CODE = f050301.CUST_CODE;
			f050001.CUST_ORD_NO = f050301.CUST_ORD_NO;
			f050001.ORD_TYPE = f050301.ORD_TYPE;
			f050001.RETAIL_CODE = f050301.RETAIL_CODE;
			f050001.ORD_DATE = f050301.ORD_DATE;
			f050001.PROC_FLAG = "0";
			f050001.CUST_NAME = f050301.CUST_NAME;
			f050001.SELF_TAKE = f050301.SELF_TAKE;
			f050001.FRAGILE_LABEL = f050301.FRAGILE_LABEL;
			f050001.GUARANTEE = f050301.GUARANTEE;
			f050001.SA = f050301.SA;
			f050001.GENDER = f050301.GENDER;
			f050001.AGE = f050301.AGE;
			f050001.SA_QTY = f050301.SA_QTY;
			f050001.SA_CHECK_QTY = f050301.SA_CHECK_QTY;
			f050001.TEL = f050301.TEL;
			f050001.ADDRESS = f050301.ADDRESS;
			f050001.ORDER_BY = f050301.ORDER_BY;
			f050001.CONSIGNEE = f050301.CONSIGNEE;
			f050001.ARRIVAL_DATE = f050301.ARRIVAL_DATE;
			f050001.TRAN_CODE = f050301.TRAN_CODE;
			f050001.SP_DELV = f050301.SP_DELV;
			f050001.CUST_COST = f050301.CUST_COST;
			f050001.BATCH_NO = f050301.BATCH_NO;
			f050001.CHANNEL = f050301.CHANNEL;
			f050001.POSM = f050301.POSM;
			f050001.CONTACT = f050301.CONTACT;
			f050001.CONTACT_TEL = f050301.CONTACT_TEL;
			f050001.TEL_2 = f050301.TEL_2;
			f050001.SPECIAL_BUS = f050301.SPECIAL_BUS;
			f050001.ALL_ID = f050301.ALL_ID;
			f050001.COLLECT = f050301.COLLECT;
			f050001.COLLECT_AMT = f050301.COLLECT_AMT;
			f050001.MEMO = f050301.MEMO;
			f050001.SAP_MODULE = f050301.SAP_MODULE;
			f050001.SOURCE_TYPE = f050301.SOURCE_TYPE;
			f050001.SOURCE_NO = f050301.SOURCE_NO;
			f050001.TYPE_ID = f050301.TYPE_ID;
			f050001.CAN_FAST = f050301.CAN_FAST;
			f050001.TEL_1 = f050301.TEL_1;
			f050001.TEL_AREA = f050301.TEL_AREA;
			f050001.PRINT_RECEIPT = f050301.PRINT_RECEIPT;
			f050001.RECEIPT_NO = f050301.RECEIPT_NO;
			f050001.ZIP_CODE = f050301.ZIP_CODE;
			f050001.TICKET_ID = f050301.TICKET_ID;
			f050001.ORD_NO = newOrdNo;
			f050001.DC_CODE = newDcCode;
			f050001Repo.Add(f050001);
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			f050101Repo.Add(CreateF050101(f050301, newOrdNo));
			var f050302Repo = new F050302Repository(Schemas.CoreSchema);
			var items =
				f050302Repo.Filter(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f050302 in items)
			{
				var f050002 = new F050002();
				f050002.ORD_SEQ = f050302.ORD_SEQ;
				f050002.CUST_CODE = f050302.CUST_CODE;
				f050002.GUP_CODE = f050302.GUP_CODE;
				f050002.ITEM_CODE = f050302.ITEM_CODE;
				f050002.ORD_QTY = f050302.ORD_QTY;
				f050002.SERIAL_NO = f050302.SERIAL_NO;
				f050002.DC_CODE = newDcCode;
				f050002.ORD_NO = newOrdNo;
				f050002Repo.Add(f050002);
				f050102Repo.Add(CreateF050102(f050302, newOrdNo));
			}

			return newOrdNo;
		}

		private F050101 CreateF050101(F050301 f050301, string newOrdNo)
		{
			var f050101 = new F050101
			{
				ORD_NO = newOrdNo,
				CUST_ORD_NO = f050301.CUST_ORD_NO,
				ORD_TYPE = f050301.ORD_TYPE,
				RETAIL_CODE = f050301.RETAIL_CODE,
				ORD_DATE = f050301.ORD_DATE,
				STATUS = "1",
				CUST_NAME = f050301.CUST_NAME,
				SELF_TAKE = f050301.SELF_TAKE,
				FRAGILE_LABEL = f050301.FRAGILE_LABEL,
				GUARANTEE = f050301.GUARANTEE,
				SA = f050301.SA,
				GENDER = f050301.GENDER,
				AGE = f050301.AGE,
				SA_QTY = f050301.SA_QTY,
				SA_CHECK_QTY = f050301.SA_CHECK_QTY,
				TEL = f050301.TEL,
				ADDRESS = f050301.ADDRESS,
				CONSIGNEE = f050301.CONSIGNEE,
				ARRIVAL_DATE = f050301.ARRIVAL_DATE ?? DateTime.Now,
				TRAN_CODE = f050301.TRAN_CODE,
				SP_DELV = f050301.SP_DELV,
				CUST_COST = f050301.CUST_COST,
				BATCH_NO = f050301.BATCH_NO,
				CHANNEL = f050301.CHANNEL,
				POSM = f050301.POSM,
				CONTACT = f050301.CONTACT,
				CONTACT_TEL = f050301.CONTACT_TEL,
				TEL_2 = f050301.TEL_2,
				SPECIAL_BUS = f050301.SPECIAL_BUS,
				ALL_ID = f050301.ALL_ID,
				COLLECT = f050301.COLLECT,
				COLLECT_AMT = f050301.COLLECT_AMT,
				MEMO = f050301.MEMO,
				GUP_CODE = f050301.GUP_CODE,
				CUST_CODE = f050301.CUST_CODE,
				DC_CODE = f050301.DC_CODE,
				TYPE_ID = f050301.TYPE_ID,
				CAN_FAST = f050301.CAN_FAST,
				TEL_1 = f050301.TEL_1,
				TEL_AREA = f050301.TEL_AREA,
				PRINT_RECEIPT = f050301.PRINT_RECEIPT,
				RECEIPT_NO = f050301.RECEIPT_NO,
				RECEIPT_NO_HELP = f050301.RECEIPT_NO_HELP,
				//RECEIPT_TITLE="",
				//RECEIPT_ADDRESS ="",
				//BUSINESS_NO = "",
				//DISTR_CAR_NO = "",
				HAVE_ITEM_INVO = f050301.HAVE_ITEM_INVO,
				NP_FLAG = f050301.NP_FLAG,
				EXTENSION_A = f050301.EXTENSION_A,
				EXTENSION_B = f050301.EXTENSION_B,
				EXTENSION_C = f050301.EXTENSION_C,
				EXTENSION_D = f050301.EXTENSION_D,
				EXTENSION_E = f050301.EXTENSION_E,
				ROUND_PIECE = "0"
			};
			return f050101;
		}
		private F050102 CreateF050102(F050302 f050302, string newOrdNo)
		{
			var f050102 = new F050102
			{
				ORD_NO = newOrdNo,
				ORD_SEQ = f050302.ORD_SEQ,
				ITEM_CODE = f050302.ITEM_CODE,
				ORD_QTY = f050302.ORD_QTY,
				SERIAL_NO = f050302.SERIAL_NO,
				DC_CODE = f050302.DC_CODE,
				GUP_CODE = f050302.GUP_CODE,
				CUST_CODE = f050302.CUST_CODE,
				NO_DELV = f050302.NO_DELV
			};
			return f050102;
		}

		private List<F050801> UpdateWorkType4(string dcCode, string gupCode, string custCode, string ordNo, string selfTake, string printPass)
		{
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050101 =
				f050101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo);
			f050101.SELF_TAKE = selfTake;
			f050101Repo.Update(f050101);
			var f050801s = new List<F050801>();

			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var items =
				f05030101Repo.Filter(
					o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f05030101 in items)
			{
				var f050801Item = f050801Repo.Filter(
					o =>
						o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
						o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f050801Item != null)
				{
					f050801Item.SELF_TAKE = selfTake;
					f050801Item.PRINT_PASS = printPass;
					f050801Item.NO_LOADING = selfTake;
					f050801Repo.Update(f050801Item);
					f050801s.Add(f050801Item);
				}
			}

			return f050801s;
		}

		/// <summary>
		/// 若為自取的話，就將派車單與託運單暫時備份起來
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		void DeleteF700102WithF050901(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010201Repo = new F20010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010202Repo = new F20010202Repository(Schemas.CoreSchema, _wmsTransaction);

			var f700102s = f700102Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
			var f050901s = f050901Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			f20010201Repo.BulkInsert(f700102s.Select(AutoMapper.Mapper.DynamicMap<F20010201>));
			f20010202Repo.BulkInsert(f050901s.Select(AutoMapper.Mapper.DynamicMap<F20010202>));

			foreach (var f700102 in f700102s)
			{
				f700102Repo.Delete(x => x.WMS_NO == f700102.WMS_NO && x.DC_CODE == f700102.DC_CODE && x.GUP_CODE == f700102.GUP_CODE && x.CUST_CODE == f700102.CUST_CODE);
			}

			foreach (var f050901 in f050901s)
			{
				f050901Repo.Delete(x => x.WMS_NO == f050901.WMS_NO && x.DC_CODE == f050901.DC_CODE && x.GUP_CODE == f050901.GUP_CODE && x.CUST_CODE == f050901.CUST_CODE);
			}
		}

		void UndoF700102WithF050901(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010201Repo = new F20010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010202Repo = new F20010202Repository(Schemas.CoreSchema, _wmsTransaction);

			var f20010201s = f20010201Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
			var f20010202s = f20010202Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			f700102Repo.BulkInsert(f20010201s.Select(AutoMapper.Mapper.DynamicMap<F700102>));
			f050901Repo.BulkInsert(f20010202s.Select(AutoMapper.Mapper.DynamicMap<F050901>));

			foreach (var f20010201 in f20010201s)
			{
				f20010201Repo.Delete(x => x.WMS_NO == f20010201.WMS_NO && x.DC_CODE == f20010201.DC_CODE && x.GUP_CODE == f20010201.GUP_CODE && x.CUST_CODE == f20010201.CUST_CODE);
			}

			foreach (var f20010202 in f20010202s)
			{
				f20010202Repo.Delete(x => x.WMS_NO == f20010202.WMS_NO && x.DC_CODE == f20010202.DC_CODE && x.GUP_CODE == f20010202.GUP_CODE && x.CUST_CODE == f20010202.CUST_CODE);
			}
		}

		#endregion

		public IQueryable<F200103Data> GetF200103Datas(string dcCode, string gupCode, string custCode, string adjustNo)
		{
			var f200103Repo = new F200103Repository(Schemas.CoreSchema);
			return f200103Repo.GetF200103Datas(dcCode, gupCode, custCode, adjustNo);
		}

		public IQueryable<F1913Data> GetF1913Datas(string dcCode, string gupCode, string custCode, string warehouseId,
			string itemCode, string itemName)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			return f1913Repo.GetF1913Datas(dcCode, gupCode, custCode, warehouseId, itemCode, itemName);
		}

		public ExecuteResult InsertP200101ByAdjustType1(List<F1913Data> f1913Datas, KeyValuePair<int, SerialNoResult[]>[] serialNoResults, string adjustType = "1")
		{
			if(f1913Datas.Any())
			{
				var adjustOrderService = new AdjustOrderService(_wmsTransaction);
				foreach (var item in serialNoResults)
					for (int i = 0; i < item.Value.Count(); i++)
						item.Value[i].SerialNo = item.Value[i].SerialNo?.ToUpper();

				var stockService = new StockService();
				bool isPass = false;
				var allotBatchNo = "BJ" + DateTime.Now.ToString("yyyyMMddHHmmss");
				var list = new List<AdjustOrderParam>();

				try
				{
					var itemCodes = f1913Datas.Where(x => x.WORK_TYPE == "1").Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
					if (itemCodes.Any())
					{
						isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
						if (!isPass)
							return new ExecuteResult(false, "仍有程序正在配庫調整單調出商品，請稍後再試");
					}

					var groupCust = f1913Datas.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }).ToList();
					foreach(var cust in groupCust)
					{
						var adjustStockDetailList = cust.Select((x) =>
							new AdjustStockDetail
							{
								LocCode = x.LOC_CODE,
								ItemCode = x.ITEM_CODE,
								ValidDate = x.VALID_DATE,
								EnterDate = x.ENTER_DATE,
								MakeNo = x.MAKE_NO,
								PalletCtrlNo = x.PALLET_CTRL_NO,
								BoxCtrlNo = x.BOX_CTRL_NO,
								WarehouseId = x.WAREHOUSE_ID,
								Cause = x.CAUSE,
								CasueMemo = x.CAUSE_MEMO,
								WORK_TYPE = x.WORK_TYPE,
								AdjQty = (x.WORK_TYPE == "0") ? x.ADJ_QTY_IN.Value : x.ADJ_QTY_OUT.Value,
								SerialNoList = serialNoResults.FirstOrDefault(y => y.Key == x.ROWNUM).Value?.Select(z => z.SerialNo).ToList()
							}).ToList();

						var adjustOrderParam = new AdjustOrderParam
						{
							DcCode = cust.Key.DC_CODE,
							GupCode = cust.Key.GUP_CODE,
							CustCode = cust.Key.CUST_CODE,
							AdjustType = adjustOrderService.GetAdjustTypeByValue(adjustType),
							CheckSerialItem = true,
							WorkType = null,
							AdjustStockDetails = adjustStockDetailList,
						};
						list.Add(adjustOrderParam); 
					}

					var result = adjustOrderService.CreateAdjustOrder(list);
					if (result.IsSuccessed)
						result.Message = result.No;
					return result;
				}
				finally
				{
					if (isPass)
						stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
				}
			}
			return new ExecuteResult(false, "無調整明細，建立調整單失敗");
		}

		public ImportF1913DataResult ImportF1913DataItems(string dcCode, string gupCode, string custCode, List<F1913DataImport> importF1913Datas)
		{
			var failDataItems = new List<F1913DataImport>();
			var returnList = new List<F1913Data>();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var f1901Repo = new F1901Repository(Schemas.CoreSchema);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema);

      importF1913Datas.ForEach(x => x.SERIAL_NO?.ToUpper());

      var itemCodes = importF1913Datas.Where(x => !string.IsNullOrWhiteSpace(x.ITEM_CODE)).Select(x => x.ITEM_CODE).Distinct().ToList();
			var items = f1903Repo.GetDatasByItems(gupCode, custCode, itemCodes).ToList();
			var locs = f1912Repo.GetDatasByLocCodes(dcCode, gupCode, custCode, importF1913Datas.Where(x => !string.IsNullOrWhiteSpace(x.LOC_CODE)).Select(x => x.LOC_CODE).Distinct().ToList()).ToList();
			var warehouses = f1980Repo.GetDatas(dcCode, locs.Select(x => x.WAREHOUSE_ID).Distinct().ToList());

			List<F1913> stockCach = new List<F1913>();
			List<F1913DataImport> addList = new List<F1913DataImport>();

			int i = 1;
			foreach (var dataItem in importF1913Datas)
			{
				var item = items.FirstOrDefault(x => x.ITEM_CODE == dataItem.ITEM_CODE);
				string errMessage = string.Empty;
				if (!CheckData(dcCode, gupCode, custCode, item, locs, dataItem, stockCach, addList, ref errMessage))
				{
					failDataItems.Add(new F1913DataImport()
					{
						ROWNUM = i,
						DC_CODE = dataItem.DC_CODE,
						LOC_CODE = dataItem.LOC_CODE,
						ITEM_CODE = dataItem.ITEM_CODE,
						VALID_DATE = dataItem.VALID_DATE_FORMAT,
						ENTER_DATE = dataItem.ENTER_DATE_FORMAT,
						MAKE_NO = dataItem.MAKE_NO,
						ADJ_QTY_IN = dataItem.ADJ_QTY_IN,
						ADJ_QTY_OUT = dataItem.ADJ_QTY_OUT,
						CAUSE = dataItem.CAUSE,
						SERIAL_NO = dataItem.SERIAL_NO,
						FailMessage = errMessage
					});
					i++;
					continue;
				}
				addList.Add(dataItem);


				var loc = locs.First(x => x.LOC_CODE == dataItem.LOC_CODE);
				var warehouse = warehouses.First(x => x.WAREHOUSE_ID == loc.WAREHOUSE_ID);
				var vnr = f1908Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.VNR_CODE == "000000").FirstOrDefault();

				var validDate = DateTime.Parse(dataItem.VALID_DATE);
				var enterDate = string.IsNullOrWhiteSpace(dataItem.ENTER_DATE) ? DateTime.Today : DateTime.Parse(dataItem.ENTER_DATE);

				var makeNo = string.IsNullOrWhiteSpace(dataItem.MAKE_NO) ? "0" : dataItem.MAKE_NO;
				var dcName = f1901Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dataItem.DC_CODE).FirstOrDefault().DC_NAME;

				List<F1913> stockTemp = new List<F1913>();
				List<F1913> stock = new List<F1913>();
				long stockCont = 0;
				if (stockTemp.Any())
				{
					stock = stockTemp.Where(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
					o.ITEM_CODE == dataItem.ITEM_CODE && o.LOC_CODE == dataItem.LOC_CODE && o.VALID_DATE == validDate && o.MAKE_NO == makeNo &&
					o.ENTER_DATE == enterDate).ToList();
				}
				if (!stock.Any())
				{
					stock = f1913Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
					o.ITEM_CODE == dataItem.ITEM_CODE && o.LOC_CODE == dataItem.LOC_CODE && o.VALID_DATE == validDate && o.MAKE_NO == makeNo &&
					o.ENTER_DATE == enterDate).ToList();
					stockCach.AddRange(stock);
				}

				stockCont = stock.Sum(o => o.QTY);
				
				var addF1913 = new F1913Data()
				{
					DC_CODE = dataItem.DC_CODE,
					DC_NAME = dcName,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					LOC_CODE = loc.LOC_CODE,
					ITEM_CODE = item.ITEM_CODE,
					ITEM_NAME = item.ITEM_NAME,
					ITEM_COLOR = item.ITEM_COLOR,
					ITEM_SIZE = item.ITEM_SIZE,
					ITEM_SPEC = item.ITEM_SPEC,
					VALID_DATE = validDate,
					ENTER_DATE = enterDate,
					CAUSE_MEMO = dataItem.CAUSE,
					CAUSE = dataItem.CAUSE,
					CAUSENAME = f1951Repo.GetDatasByTrueAndCondition(x => x.UCT_ID == "AI" && x.UCC_CODE == dataItem.CAUSE).FirstOrDefault()?.CAUSE,
					VNR_CODE = "000000",
					VNR_NAME = vnr == null ? "" : vnr.VNR_NAME,
					WAREHOUSE_ID = warehouse.WAREHOUSE_ID,
					WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME,
					IsSelected = true,
					ADJ_QTY_IN = string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_IN) ? 0 : Convert.ToInt32(dataItem.ADJ_QTY_IN),
					ADJ_QTY_OUT = string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_OUT) ? 0 : Convert.ToInt32(dataItem.ADJ_QTY_OUT),
					QTY = stockCont,
					BOX_CTRL_NO = "0",
					PALLET_CTRL_NO = "0",
					MAKE_NO = makeNo,
					SERIAL_NO = dataItem.SERIAL_NO,
					WORK_TYPE = string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_IN) || dataItem.ADJ_QTY_IN == "0" ? "1" : "0",
					BUNDLE_SERIALLOC = item.BUNDLE_SERIALLOC,
					BUNDLE_SERIALNO = item.BUNDLE_SERIALNO
				};
				returnList.Add(addF1913);

			}

			return new ImportF1913DataResult
			{
				Result = new ExecuteResult(true),
				F1913DataItems = returnList,
				F1913DataFailItems = failDataItems
			};
		}

		/// <summary>
		/// 資料檢查
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="items"></param>
		/// <param name="locs"></param>
		/// <param name="dataItem"></param>
		/// <param name="errMessage"></param>
		/// <returns></returns>
		private bool CheckData(string dcCode, string gupCode, string custCode, F1903 f1903, List<F1912> locs, F1913DataImport dataItem, List<F1913> stockCach, List<F1913DataImport> addList, ref string errMessage)
		{
			bool checkResult = true;
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var serialNoService = new SerialNoService(_wmsTransaction);

			//驗證DC
			if (string.IsNullOrWhiteSpace(dataItem.DC_CODE))
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_DCCodeNULL);
				checkResult = false;
			}
			else
			{
				if (dataItem.DC_CODE != dcCode)
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_DCCodeErr);
					checkResult = false;
				}
			}

			//驗證儲位
			var findLoc = locs.FirstOrDefault(x => x.LOC_CODE == dataItem.LOC_CODE);
			if (string.IsNullOrWhiteSpace(dataItem.LOC_CODE))
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_LocNULL);
				checkResult = false;
			}
			else
			{
				if (findLoc == null)
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_LocCodeNotExit);
					checkResult = false;
				}
			}

			//驗證品號
			//var findItem = items.FirstOrDefault(x => x.ITEM_CODE == dataItem.ITEM_CODE);
			if (string.IsNullOrWhiteSpace(dataItem.ITEM_CODE))
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ItemCodeNULL);
				checkResult = false;
			}
			else
			{
				if (f1903 == null)
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ItemCodeNotExit);
					checkResult = false;
				}
			}

			//驗證效期
			if (string.IsNullOrWhiteSpace(dataItem.VALID_DATE))
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ValidDateNULL);
				checkResult = false;
			}
			else
			{
				DateTime vdate;
				var notDate = DateTime.TryParse(dataItem.VALID_DATE, out vdate) ? null : dataItem.VALID_DATE;
				if (notDate != null)
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ValidDateFormaErr);
					checkResult = false;
				}
				//修正日期格式顯示的問題
				//補上此段Code的原因是防止匯出時出現日期格是為"31-十二月-9999"的情況
				//假設其它欄位有錯誤，日期正確時，會把日期格是轉成字串
				dataItem.VALID_DATE_FORMAT = string.IsNullOrWhiteSpace(notDate) ? vdate.ToString("yyyy/MM/dd") : notDate;
			}

			//驗證異動原因
			if (string.IsNullOrWhiteSpace(dataItem.CAUSE) || f1951Repo.GetDatasByTrueAndCondition(x => x.UCT_ID == "AI" && x.UCC_CODE == dataItem.CAUSE)?.FirstOrDefault() == null)
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_NotExistCause);
				checkResult = false;
			}

			//驗證入庫日
			if (!string.IsNullOrWhiteSpace(dataItem.ENTER_DATE))
			{
				DateTime edate;
				var notDate = DateTime.TryParse(dataItem.ENTER_DATE, out edate) ? null : dataItem.ENTER_DATE;
				if (notDate != null)
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_EnterDateFormaErr);
					checkResult = false;
				}
				//修正日期格式顯示的問題
				//補上此段Code的原因是防止匯出時出現日期格是為"31-十二月-9999"的情況
				//假設其它欄位有錯誤，日期正確時，會把日期格是轉成字串
				dataItem.ENTER_DATE_FORMAT = string.IsNullOrWhiteSpace(notDate) ? edate.ToString("yyyy/MM/dd") : notDate;
			}

            //驗證批號是否有填
            if (string.IsNullOrWhiteSpace(dataItem.MAKE_NO))
            {
                errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_MakeNoNULL);
                checkResult = false;
            }
            else
            {
                //檢核批號長度不可超過40
                if (dataItem.MAKE_NO.Length > 40)
                {
                    errMessage = MessageJoin(errMessage, "批號長度不可超過40碼");
                    checkResult = false;
                }
            }
            

            //驗證是自動倉F1980.DEVICE_TYPE != 0(自動倉)，不可調整商品庫存
            if (f1980Repo.GetF1980ByLocCode(dcCode, dataItem.LOC_CODE)?.DEVICE_TYPE != "0")
			{
				errMessage = MessageJoin(errMessage, string.Format(Properties.Resources.P2001010000_IsAutoWarehourse, dataItem.LOC_CODE));
				checkResult = false;
			}

			string adjQtyType = string.Empty; // 判斷是調出或是調入
																				//驗證調入調出數
			int adQtyIn = 0;
			long adQtyOut = 0;
			bool isAdd = true;
			if (string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_IN) && string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_OUT))
			{
				errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyInOutNoExist);
				checkResult = false;
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_IN) && (string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_OUT) || dataItem.ADJ_QTY_OUT == "0"))
				{
					var notQty = int.TryParse(dataItem.ADJ_QTY_IN, out adQtyIn) ? null : dataItem.ADJ_QTY_IN;
					if (notQty != null)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyInFormaErr);
						checkResult = false;
					}
					//判斷調出數是否小於0
					if (adQtyIn < 0)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyInLessZeroErr);
						checkResult = false;
					}
					isAdd = true;
					adjQtyType = "A1";

				}
				else if (!string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_OUT) && (string.IsNullOrWhiteSpace(dataItem.ADJ_QTY_IN) || dataItem.ADJ_QTY_IN == "0"))
				{
					var notQty = long.TryParse(dataItem.ADJ_QTY_OUT, out adQtyOut) ? null : dataItem.ADJ_QTY_OUT;
					if (notQty != null)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyOutFormaErr);
						checkResult = false;
					}
					//判斷調出數是否小於0
					if (adQtyOut < 0)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyOutLessZeroErr);
						checkResult = false;
					}
					isAdd = false;
					adjQtyType = "C1";
				}
				else
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ADJQtyInOutNoExist);
					checkResult = false;
				}
			}

      // 驗證序號
      if (f1903 != null && f1903.BUNDLE_SERIALNO == "1" && !string.IsNullOrWhiteSpace(dataItem.SERIAL_NO))
      {
        if (checkResult)
				{
					var serialNoData = Regex.Replace(dataItem.SERIAL_NO, @"\s", "").Trim('|').Split('|').ToList();

					// 驗證序號是否重複
					bool isRepeat = serialNoData.GroupBy(i => i).Where(g => g.Count() > 1).Count() > 0;
					if (isRepeat)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_SerialNoIsRepeat);
						checkResult = false;
					}

					//驗證序號數是否和調出或調入數相符
					int adjQtyIn = 0;
					int adjQtyOut = 0;
					int.TryParse(dataItem.ADJ_QTY_IN, out adjQtyIn);
					int.TryParse(dataItem.ADJ_QTY_OUT, out adjQtyOut);
					if ((serialNoData.Count() == adjQtyIn) == (serialNoData.Count() == adjQtyOut))
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_SerialNoCountError);
						checkResult = false;
					}

					foreach (var serialNoItem in serialNoData)
					{
						var serialNoResultList = serialNoService.CheckSerialNoFull(dcCode,
					 gupCode,
					 custCode,
					 dataItem.ITEM_CODE,
					 serialNoItem,
					 adjQtyType);
						if (serialNoResultList.First().Checked == false)
						{
							errMessage = MessageJoin(errMessage, string.Format(Properties.Resources.P2001010000_SerialNoError, serialNoItem, serialNoResultList.First().Message));
							checkResult = false;
						}
					}
				}
			}


			//驗證是否有重複的品項
			if (addList.Any())
			{
				if (addList.Where(o => o.DC_CODE == dataItem.DC_CODE && o.ITEM_CODE == dataItem.ITEM_CODE &&
				o.LOC_CODE == dataItem.LOC_CODE && o.VALID_DATE == dataItem.VALID_DATE &&
				o.ENTER_DATE == dataItem.ENTER_DATE && o.MAKE_NO == dataItem.MAKE_NO && o.SERIAL_NO == dataItem.SERIAL_NO).Any())
				{
					errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_ItemRepeat);
					checkResult = false;
				}
			}

			//調入不用算庫存
			if (checkResult && isAdd)
			{
				return checkResult;
			}

			//驗證庫存
			if (checkResult && !isAdd)
			{
				string makeno = string.IsNullOrWhiteSpace(dataItem.MAKE_NO) ? "0" : dataItem.MAKE_NO;
				DateTime edate = string.IsNullOrWhiteSpace(dataItem.ENTER_DATE) ? DateTime.Now : Convert.ToDateTime(dataItem.ENTER_DATE);
				DateTime vdate = Convert.ToDateTime(dataItem.VALID_DATE);

				var stockCachTmp = stockCach.Where(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
				o.ITEM_CODE == dataItem.ITEM_CODE && o.LOC_CODE == dataItem.LOC_CODE && o.VALID_DATE == vdate && o.MAKE_NO == makeno &&
				o.ENTER_DATE == edate).ToList();

				if (stockCachTmp.Any())
				{
					foreach (var item in stockCachTmp)
					{
						if (item.QTY - adQtyOut > 0)
							item.QTY = item.QTY - adQtyOut;
						else
						{
							adQtyOut = adQtyOut - item.QTY;
							item.QTY = 0;
						}
					}
				}
				else
				{
					var stock = f1913Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
					o.ITEM_CODE == dataItem.ITEM_CODE && o.LOC_CODE == dataItem.LOC_CODE && o.VALID_DATE == vdate && o.MAKE_NO == makeno &&
					o.ENTER_DATE == edate).ToList();

					foreach (var item in stock)
					{
						if (item.QTY - adQtyOut > 0)
						{
							var tmpItemQty = item.QTY;
							item.QTY = item.QTY - adQtyOut;
							adQtyOut -= tmpItemQty;
						}
						else
						{
							adQtyOut = adQtyOut - item.QTY;
							item.QTY = 0;
						}
					}
					stockCach.AddRange(stock);

					//庫存不足
					if (adQtyOut > 0)
					{
						errMessage = MessageJoin(errMessage, Properties.Resources.P2001010000_StockNoQTY);
						checkResult = false;
					}
				}
			}
			return checkResult;
		}

		private string MessageJoin(string message, string addMessage)
		{
			message = string.Format("{0}{1}", string.IsNullOrEmpty(message) ? "" : (message + ","), addMessage);
			return message;
		}

		public ExecuteResult UpdateP200101ByAdjustType1(string dcCode, string gupCode, string custCode, string adjustNo,
	int adjustSeq, string cause, string causeMemo)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f200103Repo = new F200103Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f200103Repo.Find(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo &&
						o.ADJUST_SEQ == adjustSeq);
			item.CAUSE = cause;
			item.CAUSE_MEMO = causeMemo;
			f200103Repo.Update(item);
			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200101Item = f200101Repo.Find(
				o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ADJUST_NO == adjustNo);
			f200101Item.UPD_DATE = DateTime.Now;
			f200101Repo.Update(f200101Item);
			return result;
		}


		#region 廠退退倉庫存調整
		public ExecuteResult VnrReturnShipDebit(List<F160204> f160204s)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var dcCode = f160204s.First().DC_CODE;
			var gupCode = f160204s.First().GUP_CODE;
			var custCode = f160204s.First().CUST_CODE;
			var wmsNo = f160204s.First().RTN_WMS_NO;
			var stockService = new StockService();
			bool isPass = false;
			var allotBatchNo = "BJ" + DateTime.Now.ToString("yyyyMMddHHmmss");
			var adjustStockDetailList = new List<AdjustStockDetail>();
			var adjustOrderService = new AdjustOrderService(_wmsTransaction);
			try
			{
				var itemCodes = f160204s.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫調整單調出商品，請稍後再試");

				var group = f160204s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE, x.TYPE_ID });
				foreach(var g in group)
				{
					var rtnQty = g.Sum(x => x.RTN_WMS_QTY);
					var stocks = f1913Repo.GetItemPickLocPriorityInfo(g.Key.DC_CODE, g.Key.GUP_CODE, g.Key.CUST_CODE, new List<string> { g.Key.ITEM_CODE }, false, g.Key.TYPE_ID)
															.Where(x => x.QTY > 0)
															.OrderByDescending(a => a.ATYPE_CODE)
															.ThenBy(a => a.VALID_DATE)
															.ThenBy(a => a.ENTER_DATE)
															.ThenByDescending(a => a.HANDY)
															.ThenBy(a => a.HOR_DISTANCE)
															.ThenBy(a => a.LOC_CODE).ToList();
					foreach (var stock in stocks)
					{
						var qty = stock.QTY >= rtnQty ? rtnQty : (int)stock.QTY;
						var detail = new AdjustStockDetail
						{
							LocCode = stock.LOC_CODE,
							ItemCode = stock.ITEM_CODE,
							ValidDate = stock.VALID_DATE,
							EnterDate = stock.ENTER_DATE,
							MakeNo = stock.MAKE_NO,
							PalletCtrlNo = stock.PALLET_CTRL_NO,
							BoxCtrlNo = stock.BOX_CTRL_NO,
							WarehouseId = stock.WAREHOUSE_ID,
							Cause = "999",
							CasueMemo = "廠退直接扣帳",
							WORK_TYPE = "1",
							AdjQty = qty,
						};
						adjustStockDetailList.Add(detail);
						rtnQty -= qty;
						if (rtnQty <= 0)
							break;
					}
					if(rtnQty > 0 )
					{
						return new ExecuteResult(false, $"商品{g.Key.ITEM_CODE}揀區庫存不足，無法直接廠退扣帳");
					}
				}
					var adjustOrderParam = new AdjustOrderParam
					{
						DcCode = dcCode,
						GupCode = gupCode,
						CustCode = custCode,
						AdjustType = adjustOrderService.GetAdjustTypeByValue("1"),
						CheckSerialItem = false,
						WorkType = null,
						AdjustStockDetails = adjustStockDetailList,
						SourceType ="13",
						SourceNo = wmsNo
					};
				var result = adjustOrderService.CreateAdjustOrder(adjustOrderParam);
				return result;
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}
		}
		#endregion

		#region 銷毀退倉庫存調整
		public ExecuteResult DestoryShipDebit(F160501 f160501, List<F160502Data> detailData)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			var dcCode = f160501.DC_CODE;
			var gupCode = f160501.GUP_CODE;
			var custCode = f160501.CUST_CODE;
			var wmsNo = f160501.DESTROY_NO;
			var stockService = new StockService();
			bool isPass = false;
			var allotBatchNo = "BJ" + DateTime.Now.ToString("yyyyMMddHHmmss");
			var adjustStockDetailList = new List<AdjustStockDetail>();
			var adjustOrderService = new AdjustOrderService(_wmsTransaction);
			try
			{
				var itemCodes = detailData.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).Distinct().ToList();
				isPass = stockService.CheckAllotStockStatus(false, allotBatchNo, itemCodes);
				if (!isPass)
					return new ExecuteResult(false, "仍有程序正在配庫調整單調出商品，請稍後再試");

				var group = detailData.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.ITEM_CODE});
				foreach (var g in group)
				{
					var rtnQty = g.Sum(x => x.DESTROY_QTY);
					var stocks = f1913Repo.GetItemPickLocPriorityInfo(g.Key.DC_CODE, g.Key.GUP_CODE, g.Key.CUST_CODE, new List<string> { g.Key.ITEM_CODE }, false, "D")
															.Where(x => x.QTY > 0)
															.OrderByDescending(a => a.ATYPE_CODE)
															.ThenBy(a => a.VALID_DATE)
															.ThenBy(a => a.ENTER_DATE)
															.ThenByDescending(a => a.HANDY)
															.ThenBy(a => a.HOR_DISTANCE)
															.ThenBy(a => a.LOC_CODE).ToList();
					foreach (var stock in stocks)
					{
						var qty = stock.QTY >= rtnQty ? rtnQty : (int)stock.QTY;
						var detail = new AdjustStockDetail
						{
							LocCode = stock.LOC_CODE,
							ItemCode = stock.ITEM_CODE,
							ValidDate = stock.VALID_DATE,
							EnterDate = stock.ENTER_DATE,
							MakeNo = stock.MAKE_NO,
							PalletCtrlNo = stock.PALLET_CTRL_NO,
							BoxCtrlNo = stock.BOX_CTRL_NO,
							WarehouseId = stock.WAREHOUSE_ID,
							Cause = "999",
							CasueMemo = "銷毀直接扣帳",
							WORK_TYPE = "1",
							AdjQty = qty,
						};
						adjustStockDetailList.Add(detail);
						rtnQty -= qty;
						if (rtnQty <= 0)
							break;
					}
					if (rtnQty > 0)
					{
						return new ExecuteResult(false, $"商品{g.Key.ITEM_CODE}庫存不足，無法直接銷毀扣帳");
					}
				}
				var adjustOrderParam = new AdjustOrderParam
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode = custCode,
					AdjustType = adjustOrderService.GetAdjustTypeByValue("1"),
					CheckSerialItem = false,
					WorkType = null,
					AdjustStockDetails = adjustStockDetailList,
					SourceType = "08",
					SourceNo = wmsNo
				};
				var result = adjustOrderService.CreateAdjustOrder(adjustOrderParam);
				return result;
			}
			finally
			{
				if (isPass)
					stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
			}
		}
		#endregion
	}
}

