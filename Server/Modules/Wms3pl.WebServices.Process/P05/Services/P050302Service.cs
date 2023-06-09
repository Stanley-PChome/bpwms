
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F01;
using Wms3pl.WebServices.Process.P01.Services;
using Wms3pl.Datas.F00;
using System.Reflection;
using Wms3pl.Datas.F07;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050302Service
	{
		private WmsTransaction _wmsTransaction;
		public P050302Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		//public IQueryable<F051201Data> GetF051201Datas(string dcCode, string gupCode, string custCode, string delvDate,
		//	string isPrinted)
		//{
		//	var f051201Repo = new F051201Repository(Schemas.CoreSchema);
		//	return f051201Repo.GetF051201Datas(dcCode, gupCode, custCode, delvDate, isPrinted,"1");
		//}

		#region 取得訂單資料
		public IQueryable<F050101Ex> GetF050101ExDatas(string gupCode, string custCode, string dcCode, string ordDateFrom, string ordDateTo, string ordNo, string arriveDateFrom,
			string arriveDateTo, string custOrdNo, string status, string retailCode, string custName, string wmsOrdNo, string pastNo, string address, string channel, string delvType, string allId,string moveOutTarget)
		{
			DateTime? ordDateFromDate = null;
			DateTime? ordDateToDate = null;
			DateTime? arriveDateFromDate = null;
			DateTime? arriveDateToDate = null;
			DateTime date;

			if (DateTime.TryParse(ordDateFrom, out date))
				ordDateFromDate = date;
			if (DateTime.TryParse(ordDateTo, out date))
				ordDateToDate = date;
			if (DateTime.TryParse(arriveDateFrom, out date))
				arriveDateFromDate = date;
			if (DateTime.TryParse(arriveDateTo, out date))
				arriveDateToDate = date;

			var f050101Repo = new F050101Repository(Schemas.CoreSchema);
			return f050101Repo.GetF050101ExDatas(gupCode,
												custCode,
												dcCode,
												ordDateFromDate,
												ordDateToDate,
												ordNo,
												arriveDateFromDate,
												arriveDateToDate,
												custOrdNo,
												status,
												retailCode,
												custName,
												wmsOrdNo,
												pastNo,
												address,
												channel,
												delvType,
												allId,
												moveOutTarget);
		}

		public IQueryable<F050102Ex> GetF050102ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f050102Repo = new F050102Repository(Schemas.CoreSchema);
			return f050102Repo.GetF050102ExDatas(dcCode, gupCode, custCode, ordNo);
		}


		public IQueryable<F050102WithF050801> GetF050102WithF050801s(string gupCode, string custCode, string dcCode, string wmsordno)
		{
			var f050101Repo = new F050101Repository(Schemas.CoreSchema);
			return f050101Repo.GetF050102WithF050801s(gupCode, custCode, dcCode, wmsordno);
		}

		public IQueryable<P05030201BasicData> GetP05030201BasicData(string gupCode, string custCode, string dcCode, string wmsOrdNo, string ordNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetP05030201BasicData(gupCode, custCode, dcCode, wmsOrdNo, ordNo);
		}

		/// <summary>
		/// 訂單維護寫入訂單池的TicketId取法規則
		/// </summary>
		/// <param name="SP_DELV"></param>
		/// <param name="ORD_TYPE"></param>
		/// <returns></returns>
		private string GetOrderTicketClass(string SP_DELV, string ORD_TYPE)
		{
			// 參考 Table
			// O幾 F000906 
			// SP_DELV, ORD_TYPE F000904

			// 特殊出貨若選擇互賣，則直接回傳 內部交易/互賣出貨
			if (SP_DELV == "02")
			{
				return "O5";
			}

			if (ORD_TYPE == "0")
			{
				// 若訂單類型為 B2B，且特殊出貨選員購，則回傳員購單、二手單出貨(B2B)，反之回傳門市出貨(B2B)
				return (SP_DELV == "04") ? "O3" : "O1";
			}
			else
			{
				// 若訂單類型為 B2C，且特殊出貨選員購，則回傳員購單、二手單出貨(B2C)，反之回傳消費者出貨(B2C)
				return (SP_DELV == "04") ? "O4" : "O2";
			}
		}

		public IQueryable<F1924> GetEmpHasAuth(string empID)
		{
			var f1924Repo = new F1924Repository(Schemas.CoreSchema);
			return f1924Repo.GetEmpHasAuth(empID);
		}

		public IQueryable<F050304AddEService> GetF050304ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f050304Repo = new F050304Repository(Schemas.CoreSchema);
			return f050304Repo.GetF050304ExDatas(dcCode, gupCode, custCode, ordNo);
		}

		#endregion

		#region 建立訂單
		public ExecuteResult InsertF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			var ord_no = sharedService.GetNewOrdCode("S");

			bool isDcAddress = (f050101.SELF_TAKE == "1") ? false : sharedService.IsMatchAddressIsDCAddress(f050101.DC_CODE, f050101.ADDRESS);
			if (isDcAddress)
			{
				f050101.SELF_TAKE = "1";
				f050101.SP_DELV = "02"; // 02:互賣
			}

			#region 訂單主檔
			f050101.ORD_NO = ord_no;
			f050101Repo.Add(f050101);
			#endregion

			#region 訂單明細
			InsertOrUpdateF050102(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, f050101.ORD_NO, f050102Exs);
			#endregion

			#region 進倉單
			InsertOrUpdateF010201(f050101, f050102Exs);
			#endregion

			result.No = ord_no;
			result.Message = string.Format("訂單單號：{0} 新增成功{1}", ord_no, (isDcAddress ? "\n與物流中心相同地址時，強制改為自取與互賣!" : ""));
			return result;
		}

		#endregion

		#region Excel匯入訂單
		public ExecuteResult InsertExcelOrder(List<F050101> f050101s,List<F050102Excel> f050102s)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var sharedService = new SharedService(_wmsTransaction);
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1947Repo = new F1947Repository(Schemas.CoreSchema);
			var ord_nos = sharedService.GetNewOrdStackCodes("S", f050101s.Count());
			List<F050102> f050201List = new List<F050102>();
			foreach (var f050101 in f050101s)
			{
				var f1947 = f1947Repo.Find(o=>o.DC_CODE == f050101.DC_CODE && o.ALL_COMP == f050101.ALL_ID);
				var ord_no = ord_nos.Pop();
				f050101.ORD_NO = ord_no;
				f050101.ALL_ID = f1947?.ALL_ID;
				var details = f050102s.Where(o => o.CUST_ORD_NO == f050101.CUST_ORD_NO).Select(AutoMapper.Mapper.DynamicMap<F050102>).ToList() ;
				details.ForEach(o=>o.ORD_NO = ord_no);
				f050201List.AddRange(details);
			}
			f050101Repo.BulkInsert(f050101s);
			f050102Repo.BulkInsert(f050201List);
			return result;
		}
		#endregion

		#region 更新訂單
		public ExecuteResult UpdateF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs, F050304 f050304)
		{
			var result = new ExecuteResult { IsSuccessed = true };

			#region 訂單主檔
			var isDcAddress = false;
			UpdateF050101(f050101,false,ref isDcAddress);
			#endregion

			#region 訂單明細
			InsertOrUpdateF050102(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, f050101.ORD_NO, f050102Exs);
			#endregion

			#region 進倉單
			InsertOrUpdateF010201(f050101, f050102Exs);
			#endregion

			result.IsSuccessed = true;
			result.Message = string.Format("訂單單號：{0} 更新成功{1}", f050101.ORD_NO, (isDcAddress ? "\n與物流中心相同地址時，強制改為自取與互賣!" : ""));
			return result;
		}

		#endregion

		#region  刪除訂單

		public ExecuteResult DeleteF050101(F050101 f050101)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f05101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f075102Repo = new F075102Repository(Schemas.CoreSchema, _wmsTransaction);
			F050101 f050101Data = f05101Repo.Find(x => x.GUP_CODE == f050101.GUP_CODE && x.CUST_CODE == f050101.CUST_CODE && x.DC_CODE == f050101.DC_CODE && x.ORD_NO == f050101.ORD_NO);
			if (f050101.CVS_TAKE == "1")
			{

				var f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
				f050304Repo.DeleteF050304(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, f050101.ORD_NO);
			}
			if (f050101Data != null)
			{
				//已產生訂單池 要刪除
				if (f050101Data.STATUS == "1")
				{
					var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
					f050001Repo.DeleteF050001(f050101.ORD_NO);
					f050002Repo.DeleteF050002(f050101.ORD_NO);
				}
				f050101Data.STATUS = "9";
				f05101Repo.Update(f050101Data);
			}
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f050301Repo.Find(
					o =>
						o.DC_CODE == f050101.DC_CODE && o.GUP_CODE == f050101.GUP_CODE && o.CUST_CODE == f050101.CUST_CODE &&
						o.ORD_NO == f050101.ORD_NO);
			if (item != null)
			{
				item.PROC_FLAG = "9";
				f050301Repo.Update(item);
			}
			//進倉單
			DeleteF010201(f050101);

			// 將 F075102 該貨主訂單的訂單刪除
			f075102Repo.DelF075102ByKey(f050101.CUST_CODE, f050101.CUST_ORD_NO);

			result.IsSuccessed = true;
			return result;
		}

		#endregion

		#region 進倉單
		private void InsertOrUpdateF010201(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs)
		{
			if (f050101.SP_DELV == "02")
			{
				ChkF1908(f050101.GUP_CODE, f050101.CUST_CODE, f050101.CRT_STAFF, f050101.CRT_NAME);

				var p010201Service = new P010201Service(_wmsTransaction);
				string stockNo = "";
				var f010201 = p010201Service.GetF010201SourceNo(f050101.ORD_NO).FirstOrDefault();
				if (f010201 != null)
					stockNo = f010201.STOCK_NO;
				var f010201Data = new F010201Data()
				{
					STOCK_NO = stockNo,
					STOCK_DATE = DateTime.Now.Date,
					SHOP_DATE = f050101.ORD_DATE,
					DELIVER_DATE = f050101.ARRIVAL_DATE,
					SOURCE_TYPE = "01",
					SOURCE_NO = f050101.ORD_NO,
					ORD_PROP = "A1",
					VNR_CODE = f050101.CUST_CODE,
					CUST_ORD_NO = f050101.CUST_ORD_NO,
					CUST_COST = f050101.CUST_COST,
					STATUS = "0",
					MEMO = f050101.MEMO,
					DC_CODE = f050101.DC_CODE,
					GUP_CODE = f050101.GUP_CODE,
					CUST_CODE = f050101.CUST_CODE,
					CRT_NAME = f050101.CRT_NAME
				};

				if (!string.IsNullOrEmpty(stockNo))
				{
					var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
					f010202Repo.DeleteF010202(stockNo, f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE);
				}

				var f010202Datas = new List<F010202Data>();
				short seq = 0;
				foreach (var item in f050102Exs)
				{
					seq++;
					var f010202Data = new F010202Data()
					{
						STOCK_SEQ = seq,
						DC_CODE = item.DC_CODE,
						GUP_CODE = item.GUP_CODE,
						CUST_CODE = item.CUST_CODE,
						ITEM_CODE = item.ITEM_CODE,
						STOCK_QTY = item.ORD_QTY,
						ChangeFlag = "A"
					};
					f010202Datas.Add(f010202Data);
				}

				p010201Service.InsertOrUpdateP010201(f010201Data, f010202Datas);
			}
			else
				DeleteF010201(f050101);
		}

		private void DeleteF010201(F050101 f050101)
		{
			var p010201Service = new P010201Service(_wmsTransaction);
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010201 = f010201Repo.GetF010201SourceNo(f050101.ORD_NO).FirstOrDefault();
			if (f010201 != null)
			{
				p010201Service.DeleteP010201FromDB(f010201.STOCK_NO, f050101.GUP_CODE, f050101.CUST_CODE, f050101.DC_CODE);
			}
		}

		public void ChkF1908(string gupCode, string custCode, string userID, string userName)
		{
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1908 = f1908Repo.Find(x => x.VNR_CODE == custCode && x.GUP_CODE == gupCode);
			if (f1908 == null)
			{
				var f1909 = f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
				if (f1909 != null)
				{
					// f1908 = ExDataMapper.Map<F1909, F1908>(f1909);
					f1908 = new F1908()
					{
						GUP_CODE = f1909.GUP_CODE,
						VNR_CODE = f1909.CUST_CODE,
						VNR_NAME = f1909.CUST_NAME,
						STATUS = "0",
						UNI_FORM = f1909.UNI_FORM,
						BOSS = f1909.BOSS,
						TEL = f1909.TEL,
						// ZIP = f1909.INVO_ZIP,
						ADDRESS = f1909.ADDRESS,
						ITEM_CONTACT = f1909.ITEM_CONTACT,
						ITEM_TEL = f1909.ITEM_TEL,
						ITEM_CEL = f1909.ITEM_CEL,
						ITEM_MAIL = f1909.ITEM_MAIL,
						BILL_CONTACT = f1909.BILL_CONTACT,
						BILL_TEL = f1909.BILL_TEL,
						BILL_CEL = f1909.BILL_CEL,
						BILL_MAIL = f1909.BILL_MAIL,
						INV_ZIP = f1909.INVO_ZIP,
						INV_ADDRESS = f1909.INVO_ADDRESS,
						TAX_TYPE = f1909.TAX_TYPE,
						CURRENCY = f1909.CURRENCY,
						PAY_FACTOR = f1909.PAY_FACTOR,
						PAY_TYPE = f1909.PAY_TYPE,
						BANK_CODE = f1909.BANK_CODE,
						BANK_NAME = f1909.BANK_NAME,
						BANK_ACCOUNT = f1909.BANK_ACCOUNT,
						CRT_DATE = DateTime.Now,
						CRT_NAME = userName,
						CRT_STAFF = userID
					};
					f1908Repo.Add(f1908);
				}
			}
		}

		#endregion


		#region 若選擇超取,則執行insertF050304

		public ExecuteResult InsertF050304(F050304 f050304)
		{
			F050304Repository _f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = new ExecuteResult { IsSuccessed = true };
			var wmsTransaction = new WmsTransaction();
			var consignService = new ConsignService(wmsTransaction);
			var tmpResult = consignService.ValidData(f050304);
			if (!tmpResult.IsSuccessed) return tmpResult;
			_f050304Repo.Add(f050304);
			result.IsSuccessed = true;
			return result;
		}
		#endregion

		#region 若原始資料為超取,編輯後為非超取,則執行DeleteF050304
		public ExecuteResult DeleteF050304(F050304 f050304)
		{
			F050304Repository _f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = new ExecuteResult { IsSuccessed = true };
			_f050304Repo.Delete(x => x.DC_CODE == f050304.DC_CODE && x.GUP_CODE == f050304.GUP_CODE && x.CUST_CODE == f050304.CUST_CODE && x.ORD_NO == f050304.ORD_NO);
			result.IsSuccessed = true;
			return result;
		}
		#endregion

		#region 若原始資料為超取,編輯後為非超取,則執行UpdateF050304
		public ExecuteResult UpdateF050304(F050304 f050304)
		{
			F050304Repository _f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = new ExecuteResult { IsSuccessed = true };
			_f050304Repo.UpdateF050304(f050304);
			result.IsSuccessed = true;
			return result;
		}
		#endregion


		#region 更新訂單配送門市資料
		public ExecuteResult UpdateOrderDelvRetail(string dcCode, string gupCode, string custCode, string ordNo, string retailCode, string retailName)
		{
			var f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05010103Repo = new F05010103Repository(Schemas.CoreSchema, _wmsTransaction);
			var logData = f05010103Repo.GetDatasByOrdNo(dcCode, gupCode, custCode, ordNo, "1");
			var item = f050304Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo).FirstOrDefault();
			if (item == null)
				return new ExecuteResult(false, "找不到此訂單超取設定檔資料");

			var f05010103 = new F05010103
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ORD_NO = ordNo,
				DELV_RETAILCODE = retailCode,
				DELV_RETAILNAME = retailName,
				TYPE = "1",
				CRT_DATE = DateTime.Now,
				CRT_STAFF = Current.Staff,
				CRT_NAME = Current.StaffName
			};
			if (logData.Any())
				f05010103Repo.Add(f05010103, new string[] { "LOG_ID" });
			else
			{
				var preRecord = new F05010103
				{
					DC_CODE = item.DC_CODE,
					GUP_CODE = item.GUP_CODE,
					CUST_CODE = item.CUST_CODE,
					ORD_NO = item.ORD_NO,
					DELV_RETAILCODE = item.DELV_RETAILCODE,
					DELV_RETAILNAME = item.DELV_RETAILNAME,
					TYPE = "1",
					CRT_DATE = item.CRT_DATE,
					CRT_STAFF = item.CRT_STAFF,
					CRT_NAME = item.CRT_NAME
				};
				f05010103Repo.Insert(preRecord);
				f05010103Repo.Add(f05010103);
			}
			item.DELV_RETAILCODE = retailCode;
			item.DELV_RETAILNAME = retailName;
			f050304Repo.Update(item);
			return new ExecuteResult(true);
		}
		#endregion

		#region 核准
		public ExecuteResult ApproveF050101(F050101 f050101, ObservableCollection<F050102Ex> f050102Exs)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var sharedService = new SharedService(_wmsTransaction);
			#region 更新訂單主檔
			var isDcAddress = false;
			UpdateF050101(f050101, true,ref isDcAddress);

			#endregion

			#region 更新訂單明細
			InsertOrUpdateF050102(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, f050101.ORD_NO, f050102Exs);
			#endregion

			#region 寫入F050001與F050002
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);

			// get ticket_id

			decimal ticket_id = sharedService.GetTicketID(f050101.DC_CODE, f050101.GUP_CODE, f050101.CUST_CODE, GetOrderTicketClass(f050101.SP_DELV, f050101.ORD_TYPE));

			//刪除原訂單的訂單池資料
			f050001Repo.DeleteF050001(f050101.ORD_NO);
			f050002Repo.DeleteF050002(f050101.ORD_NO);

			#region 一般新增訂單池動作
			//insert into F050001
			F050001 f050001 = new F050001();
			f050001.ORD_NO = f050101.ORD_NO;
			f050001.CUST_ORD_NO = f050101.CUST_ORD_NO;
			// f050001.DELV_DATE
			f050001.ORD_TYPE = f050101.ORD_TYPE;
			f050001.RETAIL_CODE = f050101.RETAIL_CODE;
			f050001.ORD_DATE = (DateTime)f050101.ORD_DATE;
			//f050001.STATUS
			f050001.CUST_NAME = f050101.CUST_NAME;
			f050001.SELF_TAKE = f050101.SELF_TAKE;
			f050001.FRAGILE_LABEL = f050101.FRAGILE_LABEL;
			f050001.GUARANTEE = f050101.GUARANTEE;
			f050001.SA = f050101.SA;
			f050001.GENDER = f050101.GENDER;
			f050001.AGE = f050101.AGE;
			f050001.SA_QTY = f050101.SA_QTY;
			f050001.SA_CHECK_QTY = f050101.SA_CHECK_QTY;
			f050001.TEL = f050101.TEL;
			f050001.ADDRESS = f050101.ADDRESS;
			//f050001.ORDER_BY = f050101.ORDER_BY;
			f050001.CONSIGNEE = f050101.CONSIGNEE;
			f050001.ARRIVAL_DATE = f050101.ARRIVAL_DATE;
			f050001.TRAN_CODE = f050101.TRAN_CODE;
			f050001.SP_DELV = f050101.SP_DELV;
			f050001.CUST_COST = f050101.CUST_COST;
			f050001.BATCH_NO = f050101.BATCH_NO;
			f050001.CHANNEL = f050101.CHANNEL;
			f050001.POSM = f050101.POSM;
			f050001.CONTACT = f050101.CONTACT;
			f050001.CONTACT_TEL = f050101.CONTACT_TEL;
			f050001.TEL_2 = f050101.TEL_2;
			f050001.SPECIAL_BUS = f050101.SPECIAL_BUS;
			f050001.ALL_ID = f050101.ALL_ID;
			f050001.COLLECT = f050101.COLLECT;
			f050001.COLLECT_AMT = f050101.COLLECT_AMT;
			f050001.MEMO = f050101.MEMO;
			//f050001.SAP_MODULE=f050101.ORD_NO;
			//f050001.SOURCE_TYPE=f050101.ORD_NO;
			//f050001.SOURCE_NO=f050101.ORD_NO;
			f050001.GUP_CODE = f050101.GUP_CODE;
			f050001.CUST_CODE = f050101.CUST_CODE;
			f050001.DC_CODE = f050101.DC_CODE;
			f050001.CRT_STAFF = f050101.CRT_STAFF;
			f050001.CRT_DATE = f050101.CRT_DATE;
			f050001.CRT_NAME = f050101.CRT_NAME;
			f050001.TYPE_ID = f050101.TYPE_ID;
			f050001.CAN_FAST = f050101.CAN_FAST;
			f050001.TEL_1 = f050101.TEL_1;
			f050001.TEL_AREA = f050101.TEL_AREA;
			f050001.PRINT_RECEIPT = f050101.PRINT_RECEIPT;
			f050001.RECEIPT_NO = f050101.RECEIPT_NO;
			//f050001.ZIP_CODE = f050101.ZIP_CODE;
			f050001.TICKET_ID = ticket_id;
			f050001.CVS_TAKE = f050101.CVS_TAKE;
			f050001.SUBCHANNEL = f050101.SUBCHANNEL;
			f050001.FOREIGN_WMSNO = f050101.FOREIGN_WMSNO;
			f050001.FOREIGN_CUSTCODE = f050101.FOREIGN_CUSTCODE;
			f050001.ROUND_PIECE = f050101.ROUND_PIECE;
			f050001.FAST_DEAL_TYPE = f050101.FAST_DEAL_TYPE;
			f050001.SUG_BOX_NO = f050101.SUG_BOX_NO;
			f050001.MOVE_OUT_TARGET = f050101.MOVE_OUT_TARGET;
			f050001.PACKING_TYPE = f050101.PACKING_TYPE;
			f050001.ISPACKCHECK = f050101.ISPACKCHECK;
			f050001Repo.Add(f050001);

			//insert into F050002
			foreach (var d in f050102Exs)
			{
				F050002 f050002 = new F050002()
				{
					ORD_NO = d.ORD_NO,
					ORD_SEQ = d.ORD_SEQ,
					ITEM_CODE = d.ITEM_CODE,
					ORD_QTY = d.ORD_QTY,
					SERIAL_NO = d.SERIAL_NO,
					DC_CODE = d.DC_CODE,
					GUP_CODE = d.GUP_CODE,
					CUST_CODE = d.CUST_CODE,
					CRT_DATE = d.CRT_DATE,
					CRT_STAFF = d.CRT_STAFF,
					CRT_NAME = d.CRT_NAME,
					NO_DELV = d.NO_DELV,
					MAKE_NO = d.MAKE_NO
				};
				f050002Repo.Add(f050002);
			}
			#endregion
			
			#endregion

			#region 進倉單
			InsertOrUpdateF010201(f050101, f050102Exs);
			#endregion

			result.IsSuccessed = true;
			result.Message = string.Format("訂單單號：{0} 核准成功{1}", f050101.ORD_NO, (isDcAddress ? "\n與物流中心相同地址時，強制改為自取與互賣!" : ""));
			return result;
		}

		#endregion



		#region 批次核准
		/// <summary>
		/// 批次核准
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <returns></returns>
		public ExecuteResult BatchApprove(string gupCode, string custCode)
		{
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050304Repo = new F050304Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);

			var f050101s = f050101Repo.AsForUpdate().GetDatasByUnApprove(gupCode, custCode).ToList();
			var f050102Exs = f050102Repo.GetF050102Exs(gupCode, custCode).ToList();
			var orderNos = f050101s.Select(x => x.ORD_NO).ToList();
			var f050304s = f050304Repo.GetDatas(gupCode, custCode, orderNos).ToList();
			var successCount = 0;
			var messageList = new List<string>();
			var f050301s = f050301Repo.GetF050301DataByOrdNos(gupCode, custCode, f050101s.Select(o=>o.ORD_NO).ToList()).ToList();
			foreach (var f050101 in f050101s)
			{
				var curF050102s = f050102Exs.Where(x => x.DC_CODE == f050101.DC_CODE && x.GUP_CODE == f050101.GUP_CODE && x.CUST_CODE == f050101.CUST_CODE && x.ORD_NO == f050101.ORD_NO).ToList();
				var curF050304 = f050304s.Where(x => x.DC_CODE == f050101.DC_CODE && x.GUP_CODE == f050101.GUP_CODE && x.CUST_CODE == f050101.CUST_CODE && x.ORD_NO == f050101.ORD_NO).FirstOrDefault();
				
				var checkResult = Validation(f050101, curF050102s, curF050304, f050301s);
				if (!checkResult.IsSuccessed)
				{
					messageList.Add(string.Format("訂單編號:{0} {1}", f050101.ORD_NO, checkResult.Message));
					continue;
				}


				var result = ApproveF050101(f050101, new ObservableCollection<F050102Ex>(curF050102s));
				if (result.IsSuccessed)
				{
					successCount++;
				}
				else
				{
					messageList.Add(result.Message);
				}
			}
			messageList.Insert(0, string.Format("已成功核准訂單{0}筆，失敗{1}筆，共{2}筆", successCount, f050101s.Count() - successCount, f050101s.Count()));
			return new ExecuteResult(true, string.Join(Environment.NewLine, messageList));
		}

		#endregion

		#region 取得Dc資料
		private List<F1901> _dcList;
		/// <summary>
		/// 取得Dc資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <returns></returns>
		private F1901 GetDc(string dcCode)
		{
			if (_dcList == null)
				_dcList = new List<F1901>();
			var dc = _dcList.FirstOrDefault(x => x.DC_CODE == dcCode);
			if (dc != null)
				return dc;
			else
			{
				var f1901Repo = new F1901Repository(Schemas.CoreSchema);
				dc = f1901Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode).FirstOrDefault();
				if (dc != null)
					_dcList.Add(dc);
				return dc;
			}
		}

		#endregion

		#region 取得商品資料

		private List<F1903> _itemList;
		/// <summary>
		/// 取得商品
		/// </summary>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="itemCode">品號</param>
		/// <returns></returns>
		private F1903 GetItem(string gupCode,string custCode,string itemCode)
		{
			if (_itemList == null)
				_itemList = new List<F1903>();
			var item = _itemList.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode);
			if (item != null)
				return item;
			else
			{
				var f1903Repo = new F1903Repository(Schemas.CoreSchema);
				item = f1903Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode).FirstOrDefault();
				if (item != null)
					_itemList.Add(item);
				return item;
			}
		}

		#endregion

		#region 訂單檢核
		/// <summary>
		/// 訂單檢核
		/// </summary>
		/// <param name="f050101">訂單主檔</param>
		/// <param name="f050102s">訂單明細檔</param>
		/// <returns></returns>
		private ExecuteResult Validation(F050101 f050101, List<F050102Ex> f050102s, F050304 f050304,List<F050301> f050301s)
		{
			var dc = GetDc(f050101.DC_CODE);
			var errorMessage = new List<string>();
			//0.檢查是否已存在貨主單據檔
			if (f050301s.FirstOrDefault(o => o.ORD_NO == f050101.ORD_NO && o.DC_CODE == dc.DC_CODE) != null)
				errorMessage.Add("已存在貨主單據檔");

			//訂單主檔檢查
			//1.檢查出貨倉別必填
			if (string.IsNullOrEmpty(f050101.TYPE_ID))
				errorMessage.Add("出貨倉別未設定");

			//2.指定到貨日期不能大於訂單日7天
			TimeSpan s = new TimeSpan(f050101.ARRIVAL_DATE.Ticks - f050101.ORD_DATE.Value.Ticks);
			if (s.TotalDays > 7)
				errorMessage.Add("指定到貨日期不能大於訂單日7天");

			//3.訂單類型為B2B，客戶代號必填
			if (f050101.ORD_TYPE == "0" && string.IsNullOrEmpty(f050101.RETAIL_CODE))
				errorMessage.Add("訂單類型為B2B，客戶代號必填");

			//4.有客戶代號但訂單類型不是B2B，請重新確認
			if (f050101.ORD_TYPE != "0" && !string.IsNullOrEmpty(f050101.RETAIL_CODE))
				errorMessage.Add("有客戶代號但訂單類型不是B2B，請重新確認");

			//5.POSM包裝量更新為 YES，批次號必填
			if (f050101.POSM == "1" && string.IsNullOrEmpty(f050101.BATCH_NO))
				errorMessage.Add("POSM包裝量更新為 YES，批次號必填");

			//6.特殊出貨為互賣， 出貨單地址必須是物流中心地址
			if (f050101.SP_DELV == "02" && dc != null && dc.ADDRESS != f050101.ADDRESS)
				errorMessage.Add("特殊出貨為互賣， 出貨單地址必須是物流中心地址");

			//7.出貨地址必填
			if (string.IsNullOrWhiteSpace(f050101.ADDRESS))
				errorMessage.Add("出貨地址必填");

			//8.年齡格式錯誤
			if (f050101.AGE.HasValue && f050101.AGE.Value > 999)
				errorMessage.Add("年齡格式錯誤");

			//9.是否代收、代收金額
			if (f050101.COLLECT == "0" && f050101.COLLECT_AMT.HasValue && f050101.COLLECT_AMT != 0)
				errorMessage.Add("是否代收選擇NO，代收金額請等於0");
			if (f050101.COLLECT == "1" && f050101.COLLECT_AMT <= 0)
				errorMessage.Add("是否代收選擇YES，代收金額請大於0");

			//10.客戶名稱必填
			if (string.IsNullOrWhiteSpace(f050101.CUST_NAME))
				errorMessage.Add("客戶名稱必填");

			//11.非自取訂單 配送商必填
			if (f050101.SELF_TAKE == "0" && string.IsNullOrWhiteSpace(f050101.ALL_ID))
				errorMessage.Add("配送商必填");

			//13.超取檢核
			if (f050101.CVS_TAKE == "1")
			{
				if (f050304 == null)
					errorMessage.Add("外部託運單號資料未建立");
				else
				{
					if (string.IsNullOrWhiteSpace(f050304.DELV_RETAILCODE))
						errorMessage.Add("超取訂單配送門市編號必填");
					if (string.IsNullOrWhiteSpace(f050304.DELV_RETAILNAME))
						errorMessage.Add("超取訂單配送門市名稱必填");
					//沒有客戶單號(代表手動建立訂單)
					if (string.IsNullOrWhiteSpace(f050101.CUST_ORD_NO))
					{
						if (string.IsNullOrWhiteSpace(f050304.CONSIGN_NO))
							errorMessage.Add("超取訂單配送編號必填");
						if (string.IsNullOrWhiteSpace(f050304.ESERVICE))
							errorMessage.Add("超取訂單服務商必填");
					}
				}
			}

			//14.狀態為待處理時 檢核是否內容還有"請輸入"字樣
			if (f050101.STATUS == "0")
			{
				foreach (PropertyInfo pi in typeof(F050101).GetProperties())
				{
					if (pi.CanRead && pi.CanWrite && pi.PropertyType == typeof(string))
					{
						var value = (string)pi.GetValue(f050101);
						if (value != null && value == "請輸入")
						{
							errorMessage.Add("部分資料值為[請輸入]，請填入正確資料");
							break;
						}
					}
				}
			}

			//訂單明細檢查
			//1.檢查訂單商品明細中訂購數量是否小於等於零
			if (f050102s.Any(x => x.ORD_QTY <= 0))
				errorMessage.Add("商品訂單數量要大於0");

			//2.狀態為待處理時 
			//a.檢核明細品號是否還有"XXX"開頭品號
			//b.檢核明細品號是否存在
			//c.檢核明細品號是否已停售
			if (f050101.STATUS == "0")
			{
				foreach(var f050102 in f050102s)
				{
					if (f050102.ITEM_CODE.StartsWith("XXX"))
						errorMessage.Add(string.Format("匯入的訂單品號不正確，不正確的品號 暫時用{0}取代，請修改為正確的品號", f050102.ITEM_CODE));
					else
					{
						var item = GetItem(f050102.GUP_CODE, f050102.CUST_CODE, f050102.ITEM_CODE);
						if (item == null)
							errorMessage.Add(string.Format("品號{0}商品不存在", f050102.ITEM_CODE));
						else
						{
							if(item.STOP_DATE.HasValue && item.STOP_DATE <= DateTime.Today)
								errorMessage.Add(string.Format("品號{0}商品已停售", f050102.ITEM_CODE));
						}
					}
				}
			}

			if (errorMessage.Any())
				return new ExecuteResult(false, string.Format("資料有誤，{0}", string.Join(";", errorMessage)));
			return new ExecuteResult(true);
		}

		#endregion

		#region 更新訂單主檔
		private ExecuteResult UpdateF050101(F050101 f050101,bool isApprove,ref bool isDcAddress)
		{
			var sharedService = new SharedService(_wmsTransaction);
			var f05101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			F050101 f050101Data = f05101Repo.Find(x => x.GUP_CODE == f050101.GUP_CODE && x.CUST_CODE == f050101.CUST_CODE && x.DC_CODE == f050101.DC_CODE && x.ORD_NO == f050101.ORD_NO);
			if (f050101Data == null)
				return new ExecuteResult(false, "訂單不存在!");
			var srv = new P050302Service(_wmsTransaction);

			f050101Data.DC_CODE = f050101.DC_CODE;
			f050101Data.ARRIVAL_DATE = f050101.ARRIVAL_DATE;
			f050101Data.RETAIL_CODE = f050101.RETAIL_CODE;
			f050101Data.CUST_NAME = f050101.CUST_NAME;
			f050101Data.TRAN_CODE = f050101.TRAN_CODE;
			f050101Data.SP_DELV = f050101.SP_DELV;
			f050101Data.ORD_DATE = f050101.ORD_DATE;
			f050101Data.CUST_COST = f050101.CUST_COST;
			f050101Data.BATCH_NO = f050101.BATCH_NO;
			f050101Data.ORD_TYPE = f050101.ORD_TYPE;
			f050101Data.CHANNEL = f050101.CHANNEL;
			f050101Data.POSM = f050101.POSM;
			f050101Data.CONTACT = f050101.CONTACT;
			f050101Data.CONTACT_TEL = f050101.CONTACT_TEL;
			f050101Data.CONSIGNEE = f050101.CONSIGNEE;
			f050101Data.TEL_AREA = f050101.TEL_AREA;
			f050101Data.TEL = f050101.TEL;
			f050101Data.TEL_1 = f050101.TEL_1;
			f050101Data.TEL_2 = f050101.TEL_2;
			f050101Data.ADDRESS = f050101.ADDRESS;
			f050101Data.GENDER = f050101.GENDER;
			f050101Data.AGE = f050101.AGE;
			f050101Data.SELF_TAKE = f050101.SELF_TAKE;
			f050101Data.SPECIAL_BUS = f050101.SPECIAL_BUS;
			f050101Data.ALL_ID = f050101.ALL_ID;
			f050101Data.COLLECT = f050101.COLLECT;
			f050101Data.COLLECT_AMT = f050101.COLLECT_AMT;
			f050101Data.SA = f050101.SA;
			f050101Data.SA_QTY = f050101.SA_QTY;
			f050101Data.SA_CHECK_QTY = f050101.SA_CHECK_QTY;
			f050101Data.TYPE_ID = f050101.TYPE_ID;
			f050101Data.CAN_FAST = f050101.CAN_FAST;
			f050101Data.MEMO = f050101.MEMO;
			f050101Data.UPD_DATE = f050101.UPD_DATE;
			f050101Data.UPD_STAFF = f050101.UPD_STAFF;
			f050101Data.UPD_NAME = f050101.UPD_NAME;
			f050101Data.CVS_TAKE = f050101.CVS_TAKE;
			f050101Data.SUBCHANNEL = f050101.SUBCHANNEL;
			f050101Data.CUST_ORD_NO = f050101.CUST_ORD_NO;
			f050101Data.ROUND_PIECE = f050101.ROUND_PIECE;
			f050101Data.MOVE_OUT_TARGET = f050101.MOVE_OUT_TARGET;
			f050101Data.PACKING_TYPE = f050101.PACKING_TYPE;
			f050101Data.SUG_BOX_NO = f050101.SUG_BOX_NO;
			f050101Data.ISPACKCHECK = f050101.ISPACKCHECK;
			isDcAddress = (f050101Data.SELF_TAKE == "1") ? false : sharedService.IsMatchAddressIsDCAddress(f050101Data.DC_CODE, f050101Data.ADDRESS);
			if (isDcAddress)
			{
				f050101Data.SELF_TAKE = "1";
				f050101Data.SP_DELV = "02"; // 02:互賣
			}

			if (isApprove)
				f050101Data.STATUS = "1";

			f05101Repo.Update(f050101Data);
			return new ExecuteResult(true);
		}

		#endregion

		#region  新增/更新訂單明細
		/// <summary>
		/// 新增/更新訂單明細
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="ordNo">訂單編號</param>
		/// <param name="f050102Exs">訂單明細</param>
		private void InsertOrUpdateF050102(string dcCode,string gupCode,string custCode,string ordNo,ObservableCollection<F050102Ex> f050102Exs)
		{
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102s = f050102Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORD_NO == ordNo).ToList();
		

			//本次訂單明細序號(非空值序號)
			var f050102ExOrdSeqs = f050102Exs.Where(x=> !string.IsNullOrWhiteSpace(x.ORD_SEQ)).Select(x => x.ORD_SEQ);
			//資料庫訂單明細序號
			var f050102OrdSeqs = f050102s.Select(x => x.ORD_SEQ);

			//本次要新增的明細(本次訂單明細序號是空值 或 資料庫訂單明細序號在本次訂單明細序號找不到)
			var addF050102Exs = f050102Exs.Where(x => string.IsNullOrWhiteSpace(x.ORD_SEQ) || f050102OrdSeqs.All(y=> y != x.ORD_SEQ));

			//要從資料庫刪除的明細(資料庫訂單明細序號在本次訂單明細序號找不到)
			var delF050102s = f050102s.Where(x => f050102ExOrdSeqs.All(o => o != x.ORD_SEQ));

			//要從資料庫更新的明細(資料庫訂單明細序號在本次訂單明細序號存在)
			var updF050102s = f050102s.Where(x => f050102ExOrdSeqs.Any(y => y == x.ORD_SEQ));

			var ordSeq = "0";
			if (updF050102s.Any())
				ordSeq = updF050102s.Max(x => x.ORD_SEQ);

			//Step1 刪除資料庫訂單明細中本次訂單明細不存在資料
			if(delF050102s.Any())
				f050102Repo.BulkDelete(dcCode, gupCode, custCode, ordNo, delF050102s.Select(x => x.ORD_SEQ).ToList());

			//Step2 更新資料庫訂單明細資料(由本次訂單明細調整)
			var bulkUpdateF050102s = new List<F050102>();
			foreach(var f050102 in updF050102s)
			{
				var item = f050102Exs.FirstOrDefault(x => x.ORD_SEQ == f050102.ORD_SEQ);
				if(item !=null && (item.ITEM_CODE!= f050102.ITEM_CODE || item.ORD_QTY!= f050102.ORD_QTY || item.NO_DELV!= f050102.NO_DELV|| item.MAKE_NO != f050102.MAKE_NO))
				{
					f050102.ITEM_CODE = item.ITEM_CODE;
					f050102.ORD_QTY = item.ORD_QTY;
					f050102.NO_DELV = item.NO_DELV;
					f050102.MAKE_NO = item.MAKE_NO;
					bulkUpdateF050102s.Add(f050102);
				}
			}
			if (bulkUpdateF050102s.Any())
				f050102Repo.BulkUpdate(bulkUpdateF050102s);

			var bulkInsertF050102s = new List<F050102>();
			//Step3 新增訂單明細
			foreach (var f050102Ex in addF050102Exs)
			{
				ordSeq = (int.Parse(ordSeq) + 1).ToString().PadLeft(ordSeq.Length, '0');
				f050102Ex.ORD_SEQ = ordSeq;
				var f050102 = new F050102
				{
					CUST_CODE = custCode,
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					ITEM_CODE = f050102Ex.ITEM_CODE,
					ORD_NO = ordNo,
					ORD_QTY = f050102Ex.ORD_QTY,
					ORD_SEQ = ordSeq,
					SERIAL_NO = f050102Ex.SERIAL_NO,
					NO_DELV = f050102Ex.NO_DELV,
					MAKE_NO = f050102Ex.MAKE_NO
				};
				bulkInsertF050102s.Add(f050102);
			}
			if (bulkInsertF050102s.Any())
				f050102Repo.BulkInsert(bulkInsertF050102s);
		}

		#endregion
	}
}

