using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P16.ServiceEntites;
using Wms3pl.WebServices.Process.P20.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160202Service
	{
		private WmsTransaction _wmsTransaction;
		public P160202Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult ValidateVendorRtnStack(string dcCode, string gupCode, string custCode,string typeId, IEnumerable<ItemRtnQty> itemRtnQtys)
		{
			var result = true;
			var messages = new List<string>();
			var message = string.Empty;
			var repF1909 = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909 = repF1909.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode);
			var repF198001 = new F198001Repository(Schemas.CoreSchema);
			var typeName = repF198001.GetDatasByTrueAndCondition(x => x.TYPE_ID == typeId).FirstOrDefault()?.TYPE_NAME ?? "";
			var stocks = new List<ItemRtnQty>();
			var repF1913 = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			// 排序 有指定的批號先配，再配沒指定的批號
			itemRtnQtys = itemRtnQtys.OrderBy(x=>x.ItemCode).ThenByDescending(x => !string.IsNullOrWhiteSpace(x.MakeNo)).ThenByDescending(x => x.MakeNo);
			foreach (var itemRtnQty in itemRtnQtys)
			{
				var itemStocks = new List<ItemRtnQty>();
				if (!stocks.Any(x=> x.ItemCode == itemRtnQty.ItemCode))
				{
					itemStocks = repF1913.GetItemLocStock(dcCode, gupCode, custCode, itemRtnQty.ItemCode, typeId, isForIn: false, isAllowExpiredItem: false, aTypeCode: "A")
						.GroupBy(x => new { x.ITEM_CODE, x.MAKE_NO }).Select(x => new ItemRtnQty
						{
							ItemCode = x.Key.ITEM_CODE,
							MakeNo = x.Key.MAKE_NO,
							RtnQty = x.Sum(y=> y.QTY)
						}).ToList();
					stocks.AddRange(itemStocks);
				}
				else
				{
					itemStocks = stocks.Where(x => x.ItemCode == itemRtnQty.ItemCode).ToList();
				}
				var sumQty = string.IsNullOrWhiteSpace(itemRtnQty.MakeNo) ? itemStocks.Sum(x=> x.RtnQty) : itemStocks.Where(x=> x.MakeNo == itemRtnQty.MakeNo).Sum(x=> x.RtnQty);
				if (itemRtnQty.RtnQty > sumQty)
				{
					if (f1909.SPILT_VENDER_ORD == "0")
						result = false;
					if (string.IsNullOrWhiteSpace(itemRtnQty.MakeNo))
						messages.Add(string.Format(Properties.Resources.P160202Service_ItemStockNotEnough, itemRtnQty.ItemCode, itemRtnQty.RtnQty, itemRtnQty.RtnQty - sumQty));
					else
						messages.Add(string.Format(Properties.Resources.P160202Service_ItemMakeNoStockNotEnough, itemRtnQty.ItemCode, itemRtnQty.MakeNo, itemRtnQty.RtnQty, itemRtnQty.RtnQty - sumQty));
				}
				else
				{
					if (string.IsNullOrWhiteSpace(itemRtnQty.MakeNo))
					{
						var qty = itemRtnQty.RtnQty;
						itemStocks = itemStocks.Where(x => x.RtnQty > 0).ToList();
						foreach (var item in itemStocks)
						{
							if (item.RtnQty >= qty)
							{
								item.RtnQty -= qty;
								qty = 0;
								break;
							}
							else
							{
								qty -= item.RtnQty;
								item.RtnQty = 0;
							}
						}
					}
					else
					{
						var item = itemStocks.FirstOrDefault(x => x.MakeNo == itemRtnQty.MakeNo && x.RtnQty >0);
						if(item != null)
							item.RtnQty = (itemRtnQty.RtnQty >= item.RtnQty) ? 0 : item.RtnQty - itemRtnQty.RtnQty;
					}
				}
			}
			if (messages.Any())
			{
				var subMsg = "\r\n" + Properties.Resources.P160202Service_Gen_VRN_RTN_Fail;
				if (f1909.SPILT_VENDER_ORD == "1")
					subMsg = "\r\n" + Properties.Resources.P160202Service_Is_Gen_VNR_RTN;
				message = string.Join("\r\n", messages) + subMsg;
			}

			return new ExecuteResult { IsSuccessed = result, Message = message };
		}

		public ExecuteResult InsertF160204(F160204[] f160204s)
		{
			var sharedService = new SharedService();
			var f160204Repo = new F160204Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160202Repo = new F160202Repository(Schemas.CoreSchema, _wmsTransaction);
			var newOrdCode = sharedService.GetNewOrdCode("ZO");

			short seq = 0;

			foreach (var item in f160204s)
			{
				seq++;
				item.RTN_WMS_NO = newOrdCode;
				item.RTN_WMS_SEQ = seq;
				item.RTN_WMS_DATE = DateTime.Now;
				item.RTN_VNR_SEQ = item.RTN_VNR_SEQ;
				item.PROC_FLAG = "0";
				f160204Repo.Add(item);
			}

			return new ExecuteResult() { IsSuccessed = true, Message = newOrdCode };//"已新增廠退出貨單\n"
		}

		public ExecuteResult UpdateF160202ByF160204(string returnWmsNo, F160204[] f160204s)
		{
			var firstF160204 = f160204s.First();
			var dcCode = firstF160204.DC_CODE;
			var gupCode = firstF160204.GUP_CODE;
			var custCode = firstF160204.CUST_CODE;
			var f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160202Repo = new F160202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			var group = f160204s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.RTN_VNR_NO });
			var f160201List = new List<F160201>();
			var f160202List = new List<F160202>();
			foreach (var g in group)
			{
				var f160201 = f160201Repo.Find(x => x.DC_CODE == g.Key.DC_CODE && x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE && x.RTN_VNR_NO == g.Key.RTN_VNR_NO);
				if (f160201.STATUS != "0" && f160201.STATUS != "1")
					return new ExecuteResult(false, Properties.Resources.P160204Service_VNR_RTN_Status_CannotAddNew);
				var f160202s = f160202Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == g.Key.DC_CODE && x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE && x.RTN_VNR_NO == g.Key.RTN_VNR_NO).ToList();
				foreach (var item in g)
				{
					var qty = item.RTN_WMS_QTY; //本次要廠退出貨數量
					var f160202 = f160202s.First(x => x.RTN_VNR_SEQ == item.RTN_VNR_SEQ);
					if (f160202.RTN_VNR_QTY - f160202.RTN_WMS_QTY - qty >= 0)
					{
						f160202.RTN_WMS_QTY += qty;
					}
					else
					{
						return new ExecuteResult{ IsSuccessed = false, Message = string.Format(Properties.Resources.P160202Service_RtnWmsQtyOverRtnQty, f160202.RTN_VNR_NO, f160202.RTN_VNR_SEQ, qty, f160202.RTN_VNR_QTY,f160202.RTN_WMS_QTY)};
					}
				}
				f160202List.AddRange(f160202s);
				if (f160202s.Any(x => x.RTN_VNR_QTY - x.RTN_WMS_QTY > 0)) //是否所有明細都已出貨
				{
					f160201.STATUS = "1"; //處理中
				}
				else
				{
					//貨主選擇直接扣帳 且所有明細都已出貨就直接結案 (選擇出貨模式會再出貨完成後更新)
					f160201.STATUS = f1909.VNR_RTN_TYPE == "0" ? "1" : "2";
				}
				f160201List.Add(f160201);
			}
			f160201Repo.BulkUpdate(f160201List);
			f160202Repo.BulkUpdate(f160202List);
		
			var p200101Service = new P200101Service(_wmsTransaction);
			var message = string.Empty;
			switch (f1909.VNR_RTN_TYPE)
			{
				case "0"://出貨模式
					#region 寫入F050001、F050002
					var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050305Repo = new F050305Repository(Schemas.CoreSchema, _wmsTransaction);
					var f1908Repo = new F1908Repository(Schemas.CoreSchema);
					string showNewOrdCode = Properties.Resources.P160204Service_NewOrdCode + Environment.NewLine;
					var f1908 = f1908Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.VNR_CODE == firstF160204.VNR_CODE).First();

					#region 新增F050001
					var sharedService = new SharedService();
					var newOrdCode = sharedService.GetNewOrdCode("S");
					var f160201 = f160201List.First(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RTN_VNR_NO == firstF160204.RTN_VNR_NO);
					showNewOrdCode += showNewOrdCode.Last() == '\n' ? newOrdCode : ',' + newOrdCode;

					// 取最小的貨主單號
					var minCustOrdNo = f160204s.Where(x => !string.IsNullOrWhiteSpace(x.CUST_ORD_NO)).Min(x => x.CUST_ORD_NO);
					var custOrdNo = minCustOrdNo == null ? f160204s.Select(x => x.RTN_VNR_NO).Min(x => x) : minCustOrdNo;

					var f050001 = new F050001
					{
						ORD_NO = newOrdCode,
						ORD_TYPE = "0",
						ORD_DATE = DateTime.Today,
						CUST_NAME = f1908.VNR_NAME,
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						TICKET_ID = sharedService.GetTicketID(dcCode, gupCode, custCode, "OA"),
						TYPE_ID = firstF160204.TYPE_ID,
						SELF_TAKE = "1",
						TRAN_CODE = firstF160204.ORD_PROP,
						SOURCE_NO = returnWmsNo,
						SOURCE_TYPE = "13",
						CONTACT = f160201.ITEM_CONTACT ?? Properties.Resources.P160204Service_NoAssign,
						CONSIGNEE = f160201.ITEM_CONTACT ?? Properties.Resources.P160204Service_NoAssign,
						CONTACT_TEL = f160201.ITEM_TEL ?? "0000000000",
						TEL = f160201.ITEM_TEL ?? "0000000000",
						ADDRESS = f160201.ADDRESS,
						ALL_ID = null,
						SPECIAL_BUS = "0",
						CUST_ORD_NO = custOrdNo,
            ISPACKCHECK = 1
					};
					f050001Repo.Add(f050001);
					#endregion

					#region 新增F050305
					//新增F050305
					f050305Repo.Add(new F050305
					{
						DC_CODE = f050001.DC_CODE,
						GUP_CODE = f050001.GUP_CODE,
						CUST_CODE = f050001.CUST_CODE,
						ORD_NO = f050001.ORD_NO,
						SOURCE_NO = f050001.SOURCE_NO,
						SOURCE_TYPE = f050001.SOURCE_TYPE,
						STATUS = "0",
						PROC_FLAG = "0"
					});
					#endregion

					#region 新增F050002
					f050002Repo.BulkInsert(f160204s.Select((x, index) => new F050002
					{
						ORD_NO = newOrdCode,
						ORD_SEQ = (index +1).ToString(),
						ITEM_CODE = x.ITEM_CODE,
						ORD_QTY = x.RTN_WMS_QTY,
						DC_CODE = x.DC_CODE,
						GUP_CODE = x.GUP_CODE,
						CUST_CODE = x.CUST_CODE,
						WELCOME_LETTER = "0",
						NO_DELV = "0"
					}).ToList());
					#endregion

					#endregion
					message = Properties.Resources.P060204Service_VnrRtnToShip + Environment.NewLine + showNewOrdCode;
					break;
				case "1"://直接扣帳
					var result = p200101Service.VnrReturnShipDebit(f160204s.ToList());
					if (!result.IsSuccessed)
						return result;

					message = string.Format(Properties.Resources.P160204Service_VnrRtnStockDebit, result.No);
					break;
				default:
					break;
			}
			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160204Service_Insert_VNR_RTNN + returnWmsNo + Environment.NewLine + message };//已更新廠退明細\n
		}
	}
}

