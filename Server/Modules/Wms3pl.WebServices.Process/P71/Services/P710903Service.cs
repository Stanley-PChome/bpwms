
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710903Service
	{
		private WmsTransaction _wmsTransaction;
		public P710903Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 建立預設的貨主單據(所有不同的物流中心 x (固定業主、貨主) x 單據類型 x 單據類別 的所有里程碑)
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public ExecuteResult CreateDefaultTicketMilestoneNo(string gupCode, string custCode)
		{
			// 由於 F190001 的 PK 是由 Trigger 產生，所以這裡必須即時得到 PK 才能去新增里程碑資料
			var f1901Repo = new F1901Repository(Schemas.CoreSchema);
			var f19000103Repo = new F19000103Repository(Schemas.CoreSchema);
			var f190001Repo = new F190001Repository(Schemas.CoreSchema);
			var f19000101Repo = new F19000101Repository(Schemas.CoreSchema, _wmsTransaction);

			// 取得所有物流中心與里程碑資料
			var dcCodeList = f1901Repo.GetAllDcCode().ToList();
			var milestoneNoList = f19000103Repo.GetExistsTicketAllMilestoneNo().ToList();

			// 新增不同的物流中心 x (固定業主、貨主) x 單據類型 x 單據類別 的所有里程碑
			var groups = from item in milestoneNoList
						 group item by new { item.TICKET_TYPE, item.TICKET_CLASS, item.TICKET_CLASS_NAME }
							 into g
						 select g;

			foreach (var dcCode in dcCodeList)
			{
				foreach (var g in groups)
				{
					var f190001Entity = f190001Repo.Find(item => item.DC_CODE == dcCode
															&& item.GUP_CODE == gupCode
															&& item.CUST_CODE == custCode
															&& item.TICKET_TYPE == g.Key.TICKET_TYPE
															&& item.TICKET_CLASS == g.Key.TICKET_CLASS
															, isForUpdate: false, isByCache: false);
					// 若尚未新增該貨主新單據，則新增預設單據主檔
					if (f190001Entity == null)
					{
						f190001Repo.Add(new F190001
						{
							TICKET_NAME = g.Key.TICKET_CLASS_NAME,
							TICKET_TYPE = g.Key.TICKET_TYPE,
							TICKET_CLASS = g.Key.TICKET_CLASS,
							DC_CODE = dcCode,
							GUP_CODE = gupCode,
							CUST_CODE = custCode,
							SHIPPING_ASSIGN = "0",
							FAST_DELIVER = "0",
							PRIORITY = 0
						});

						// 取得 TICKET_ID
						f190001Entity = f190001Repo.Find(item => item.DC_CODE == dcCode
															&& item.GUP_CODE == gupCode
															&& item.CUST_CODE == custCode
															&& item.TICKET_TYPE == g.Key.TICKET_TYPE
															&& item.TICKET_CLASS == g.Key.TICKET_CLASS
														, isForUpdate: false, isByCache: false);
					}

					// 若貨主單據的里程碑資料已存在，則不新增預設的里程碑資料
					if (f19000101Repo.Filter(item => item.TICKET_ID == f190001Entity.TICKET_ID).Any())
						continue;

					// 新增里程碑資料
					foreach (var f19000103WithF000906 in g)
					{
						foreach (var p in typeof(F19000103WithF000906).GetProperties())
						{
							if (!p.Name.StartsWith("MILESTONE_NO"))
							{
								continue;
							}

							var sortNo = p.Name.Substring(p.Name.Length - 1);
							var milestoneNo = Convert.ToString(p.GetValue(f19000103WithF000906));

							if (string.IsNullOrWhiteSpace(milestoneNo))
							{
								continue;
							}

							f19000101Repo.Add(new F19000101
							{
								TICKET_ID = f190001Entity.TICKET_ID,
								SORT_NO = sortNo,
								MILESTONE_NO = milestoneNo
							});
						}
					}
				}
			}

			return new ExecuteResult(true); // , string.Format("已新增{0}筆貨主單據", groups.Count() * dcCodeList.Count)
		}

		public ExecuteResult InsertOrUpdateP710903(F1909 f1909, F190902[] f190902s, bool isAdd)
		{
			if (f1909 == null)
				return new ExecuteResult(false, "參數不能為空");

			if (f1909.DM == "1" && (f190902s == null || !f190902s.Any()))
				return new ExecuteResult(false, "DM至少需要一個項目");


			//系統產生F1909 貨主編號
			if (isAdd)
				f1909.CUST_CODE = GetCustCode(f1909.GUP_CODE);

			var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190902Repo = new F190902Repository(Schemas.CoreSchema, _wmsTransaction);

			var f1909Entity = f1909Repo.Find(item => item.GUP_CODE == f1909.GUP_CODE
																	&& item.CUST_CODE == f1909.CUST_CODE);

			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var isHasData = f1903Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == f1909.GUP_CODE && o.CUST_CODE == f1909.CUST_CODE).Any(c => c.SND_TYPE != "9");
		

			// 直接新增或修改資料
			if (f1909Entity == null)
			{
				f1909.STATUS = "0";
				f1909Repo.Add(f1909);
			}
			else
			{
				f1909Entity.CUST_NAME = f1909.CUST_NAME;
				f1909Entity.SHORT_NAME = f1909.SHORT_NAME;
				f1909Entity.BOSS = f1909.BOSS;
				f1909Entity.CONTACT = f1909.CONTACT;
				f1909Entity.TEL = f1909.TEL;
				f1909Entity.ADDRESS = f1909.ADDRESS;
				f1909Entity.UNI_FORM = f1909.UNI_FORM;
				f1909Entity.ITEM_CEL = f1909.ITEM_CEL;
				f1909Entity.ITEM_CONTACT = f1909.ITEM_CONTACT;
				f1909Entity.ITEM_MAIL = f1909.ITEM_MAIL;
				f1909Entity.ITEM_TEL = f1909.ITEM_TEL;
				f1909Entity.BILL_CEL = f1909.BILL_CEL;
				f1909Entity.BILL_CONTACT = f1909.BILL_CONTACT;
				f1909Entity.BILL_MAIL = f1909.BILL_MAIL;
				f1909Entity.BILL_TEL = f1909.BILL_TEL;
				f1909Entity.CURRENCY = f1909.CURRENCY;
				f1909Entity.PAY_FACTOR = f1909.PAY_FACTOR;
				f1909Entity.PAY_TYPE = f1909.PAY_TYPE;
				f1909Entity.BANK_ACCOUNT = f1909.BANK_ACCOUNT;
				f1909Entity.BANK_CODE = f1909.BANK_CODE;
				f1909Entity.BANK_NAME = f1909.BANK_NAME;
				f1909Entity.ORDER_ADDRESS = f1909.ORDER_ADDRESS;
				f1909Entity.MIX_LOC_BATCH = f1909.MIX_LOC_BATCH;
				f1909Entity.MIX_LOC_ITEM = f1909.MIX_LOC_ITEM;
				f1909Entity.MIX_SERIAL_NO = f1909.MIX_SERIAL_NO;
				f1909Entity.DC_TRANSFER = f1909.DC_TRANSFER;
				f1909Entity.BOUNDLE_SERIALLOC = f1909.BOUNDLE_SERIALLOC;
				f1909Entity.RTN_DC_CODE = f1909.RTN_DC_CODE;
				f1909Entity.FLUSHBACK = f1909.FLUSHBACK;
				f1909Entity.SAM_ITEM = f1909.SAM_ITEM;
				f1909Entity.INSIDER_TRADING = f1909.INSIDER_TRADING;
				f1909Entity.INSIDER_TRADING_LIM = f1909.INSIDER_TRADING_LIM;
				f1909Entity.AUTO_GEN_RTN = f1909.AUTO_GEN_RTN;
				f1909Entity.SPILT_ORDER = f1909.SPILT_ORDER;
				f1909Entity.SPILT_ORDER_LIM = f1909.SPILT_ORDER_LIM;
				f1909Entity.B2C_CAN_LACK = f1909.B2C_CAN_LACK;
				f1909Entity.CAN_FAST = f1909.CAN_FAST;
				f1909Entity.INSTEAD_INVO = f1909.INSTEAD_INVO;
				f1909Entity.SPILT_INCHECK = f1909.SPILT_INCHECK;
				f1909Entity.SPECIAL_IN = f1909.SPECIAL_IN;
				f1909Entity.CHECK_PERCENT = f1909.CHECK_PERCENT;
				f1909Entity.NEED_SEAL = f1909.NEED_SEAL;
				f1909Entity.DM = f1909.DM;
				f1909Entity.RIBBON = f1909.RIBBON;
				f1909Entity.RIBBON_BEGIN_DATE = f1909.RIBBON_BEGIN_DATE;
				f1909Entity.RIBBON_END_DATE = f1909.RIBBON_END_DATE;
				f1909Entity.CUST_BOX = f1909.CUST_BOX;
				f1909Entity.SP_BOX = f1909.SP_BOX;
				f1909Entity.SP_BOX_CODE = f1909.SP_BOX_CODE;
				f1909Entity.SPBOX_BEGIN_DATE = f1909.SPBOX_BEGIN_DATE;
				f1909Entity.SPBOX_END_DATE = f1909.SPBOX_END_DATE;
				f1909Entity.TAX_TYPE = f1909.TAX_TYPE;
				f1909Entity.SYS_CUST_CODE = f1909.SYS_CUST_CODE;
				f1909Entity.GUPSHARE = f1909.GUPSHARE;
				f1909Entity.CUST_MEMO = f1909.CUST_MEMO;
				f1909Entity.ISPRINTDELV = f1909.ISPRINTDELV;
				f1909Entity.ISPRINTDELVDTL = f1909.ISPRINTDELVDTL;
				f1909Entity.ISPICKLOCFIRST = f1909.ISPICKLOCFIRST;
				f1909Entity.ISOUTOFSTOCKRECV = f1909.ISOUTOFSTOCKRECV;
				f1909Entity.ISDELV_NOLOADING = f1909.ISDELV_NOLOADING;
				f1909Entity.ISPRINT_SELFTAKE = f1909.ISPRINT_SELFTAKE;
				f1909Entity.SELFTAKE_CHECKCODE = f1909.SELFTAKE_CHECKCODE;
				//2017/6/9 新增欄位 MANAGER_LOCK. ISPICKSHOWCUSTNAME
				f1909Entity.MANAGER_LOCK = f1909.MANAGER_LOCK;
				f1909Entity.ISPICKSHOWCUSTNAME = f1909.ISPICKSHOWCUSTNAME;
				f1909Entity.ISBACK_DISTR = f1909.ISBACK_DISTR;
				f1909Entity.ISUPLOADFILE = f1909.ISUPLOADFILE;
				f1909Entity.IS_ORDDATE_TODAY = f1909.IS_ORDDATE_TODAY;
				f1909Entity.ALLOWREPEAT_ITEMBARCODE = f1909.ALLOWREPEAT_ITEMBARCODE;
				f1909Entity.ALLOWGUP_ITEMCATEGORYSHARE = f1909.ALLOWGUP_ITEMCATEGORYSHARE;
				f1909Entity.ALLOWGUP_VNRSHARE = f1909.ALLOWGUP_VNRSHARE;
				f1909Entity.ALLOWGUP_RETAILSHARE = f1909.ALLOWGUP_RETAILSHARE;
				f1909Entity.ALLOWRT_SPECIALBUY = f1909.ALLOWRT_SPECIALBUY;
				f1909Entity.ALLOW_NOPRINTPICKORDER = f1909.ALLOW_NOPRINTPICKORDER;
				f1909Entity.ALLOW_NOSHIPPACKAGE = f1909.ALLOW_NOSHIPPACKAGE;
				f1909Entity.ALLOW_ADDBOXNOCHECK = f1909.ALLOW_ADDBOXNOCHECK;
				f1909Entity.ISALLOCATIONSHOWVALIDDATE = f1909.ISALLOCATIONSHOWVALIDDATE;
				f1909Entity.ISPICKSHOWVALIDDATE = f1909.ISPICKSHOWVALIDDATE;
                //2018/5/11 新增欄位 
                f1909Entity.CAL_CUFT = f1909.CAL_CUFT;
                f1909Entity.CUFT_FACTOR = f1909.CUFT_FACTOR;
                f1909Entity.CUFT_BLUK = f1909.CUFT_BLUK;
                f1909Entity.PRINT_TYPE = f1909.PRINT_TYPE;
                f1909Entity.SHOW_UNIT_TRANS = f1909.SHOW_UNIT_TRANS;
                f1909Entity.ISLATEST_VALID_DATE = f1909.ISLATEST_VALID_DATE;
                f1909Entity.ISB2B_ALONE_OUT = f1909.ISB2B_ALONE_OUT;
                f1909Entity.ISALLOW_DELV_DAY = f1909.ISALLOW_DELV_DAY;
                f1909Entity.ZIP_CODE = f1909.ZIP_CODE;
                f1909Entity.SPILT_OUTCHECK = f1909.SPILT_OUTCHECK;
                f1909Entity.SPILT_OUTCHECKWAY = f1909.SPILT_OUTCHECKWAY;
                f1909Entity.ISDELV_LOADING_CHECKCODE = f1909.ISDELV_LOADING_CHECKCODE;
                f1909Entity.IS_SINGLEBOXCHECK = f1909.IS_SINGLEBOXCHECK;
                //2018/12/22 新增欄位
                f1909Entity.SHARED_FOLDER = f1909.SHARED_FOLDER;
                f1909Entity.ISSHIFTITEM = f1909.ISSHIFTITEM;
                f1909Entity.SHIFTITEMCODE = f1909.SHIFTITEMCODE;
                f1909Entity.PACKCOUNT_MAX_UNIT = f1909.PACKCOUNT_MAX_UNIT;
                f1909Entity.NEED_ITEMSPEC = f1909.NEED_ITEMSPEC;
                f1909Entity.IS_PRINT_INSTOCKPALLETSTICKER = f1909.IS_PRINT_INSTOCKPALLETSTICKER;
                f1909Entity.IS_QUICK_CHECK = f1909.IS_QUICK_CHECK;
                f1909Entity.INSTOCKAUTOCLOSED = f1909.INSTOCKAUTOCLOSED;
                f1909Entity.ALLOWOUTSHIPDETLOG = f1909.ALLOWOUTSHIPDETLOG;
                f1909Entity.SPILT_VENDER_ORD = f1909.SPILT_VENDER_ORD;
                f1909Entity.CHG_VENDER_ORD = f1909.CHG_VENDER_ORD;
                f1909Entity.ALLOW_ADVANCEDSTOCK = f1909.ALLOW_ADVANCEDSTOCK;
                f1909Entity.PRINT2PDF = f1909.PRINT2PDF;
								f1909Entity.DESTROY_TYPE = f1909.DESTROY_TYPE;		//報廢銷毀方式
								f1909Entity.VNR_RTN_TYPE = f1909.VNR_RTN_TYPE;		//廠退方式
				        f1909Entity.SHOW_QTY = f1909.SHOW_QTY;
                //2021/12/07 新增欄位
                f1909Entity.SHOW_MESSAGE = f1909.SHOW_MESSAGE;
				f1909Entity.ALLOW_EDIT_BOX_QTY = f1909.ALLOW_EDIT_BOX_QTY;
        //f1909Entity.SUGGEST_LOC_TYPE = f1909.SUGGEST_LOC_TYPE;
        //f1909Entity.ALLOW_CANCEL_LACKORD = f1909.ALLOW_CANCEL_LACKORD;
        //f1909Entity.ALLOCATIONCHANGVALIDATE = f1909.ALLOCATIONCHANGVALIDATE;
        //f1909Entity.ALLOCATIONCHANGMAKENO = f1909.ALLOCATIONCHANGMAKENO;
				f1909Entity.ALLOW_WAREHOUSEIN_CLOSED = f1909.ALLOW_WAREHOUSEIN_CLOSED;
        f1909Entity.IS_EXECIMMEDIATEITEM = f1909.IS_EXECIMMEDIATEITEM;
        f1909Entity.ALLOW_ORDER_NO_DELV = f1909.ALLOW_ORDER_NO_DELV;
        f1909Entity.ALLOW_ORDER_LACKSHIP = f1909.ALLOW_ORDER_LACKSHIP;

        f1909Repo.Update(f1909Entity);

        f190902Repo.Delete(item => item.GUP_CODE == f1909.GUP_CODE
																&& item.CUST_CODE == f1909.CUST_CODE);
			}

			if (f1909.DM == "1")
			{
				int seq = 0;
				foreach (var item in f190902s)
				{
					seq++;
					item.DM_SEQ = seq;
					item.GUP_CODE = f1909.GUP_CODE;
					item.CUST_CODE = f1909.CUST_CODE;
					f190902Repo.Add(item);
				}
			}

			return new ExecuteResult(true);
		}

		public ExecuteResult DeleteP710903(string gupCode, string custCode)
		{
			var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);

			var f1909 = f1909Repo.Find(item => item.GUP_CODE == gupCode
																	&& item.CUST_CODE == custCode);

			if (f1909 == null)
				return new ExecuteResult(false, "此貨主不存在，已無法刪除");

			if (f1909.STATUS != "0")
				return new ExecuteResult(false, "此此貨主狀態已無法刪除");

			f1909.STATUS = "9";
			f1909Repo.Update(f1909);
			return new ExecuteResult(true, "已刪除");
		}

		public string GetCustCode(string gupCode)
		{
		  // 編碼規則: 業主編號+四碼流水號

			var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909Item = f1909Repo.Filter(o => o.GUP_CODE == gupCode).OrderByDescending(o => o.CUST_CODE).FirstOrDefault();

			if (f1909Item == null)
			{
				return string.Format("{0}0001", gupCode);
			}
			else
			{
				var secondCode = int.Parse(f1909Item.CUST_CODE.Substring(2))+1;
				return string.Format("{0}{1}", gupCode, secondCode.ToString().PadLeft(4,'0'));
			}
		}
	}
}

