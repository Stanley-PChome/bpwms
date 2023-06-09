using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Services;
using System.Data.Objects;
using Wms3pl.WebServices.Process.P20.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160501Service
	{
		private WmsTransaction _wmsTransaction;
		private F160501Repository _f160501Repo = new F160501Repository(Schemas.CoreSchema);
		private F160502Repository _f160502Repo = new F160502Repository(Schemas.CoreSchema);
		private F160504Repository _f160504Repo = new F160504Repository(Schemas.CoreSchema);
		public P160501Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF160501s(F160501 f160501, List<F160502Data> detailData, List<F160502Data> serialData)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			//新增前先檢查虛擬商品序是與狀態是否正確
			// 1 .F160504  序號不得重覆
			// 2 .F2501 狀態檢查 使用共用 Function 出貨(A1)
			var errorStr = CheckF2501SerialStatus(serialData, "");

			if (!string.IsNullOrEmpty(errorStr))
			{
				result.Message = errorStr;
				result.IsSuccessed = false;
				return result;
			}

			//以下代表已通過檢查 , 進行新增動作
			var sharedService = new SharedService(_wmsTransaction);
			var destoryNo = sharedService.GetNewOrdCode("D");  //F000901 取銷毀編號
			f160501.DESTROY_NO = destoryNo;
			_f160501Repo.Add(f160501);

			//新增f160502 Detail 資料
			var srv160502 = new P160502Service(_wmsTransaction);
			var f160502result = srv160502.InsertF160502Detail(detailData, destoryNo);

			// 新增f160504 虛擬商品序號 資料
			var srv160504 = new P160504Service(_wmsTransaction);
			var f160504result = srv160504.InsertF160504Detail(serialData, destoryNo);

			//更新訂單池
			//var f050001result = F160501WithUpdteF050001(f160501, detailData, destoryNo);
			result.Message = destoryNo;
			result.IsSuccessed = true;
			result.Message = destoryNo;
			return result;
		}

		public ExecuteResult UpdateF160501s(F160501 f160501, List<F160502Data> detailData, List<F160502Data> serialData)
		{
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = new ExecuteResult { IsSuccessed = true };
			//新增前先檢查虛擬商品序是與狀態是否正確
			// 1 .F160504  序號不得重覆
			// 2 .F2501 狀態檢查 使用共用 Function 出貨(A1)
			var errorStr = CheckF2501SerialStatus(serialData, f160501.DESTROY_NO);

			if (!string.IsNullOrEmpty(errorStr))
			{
				result.Message = errorStr;
				result.IsSuccessed = false;
				return result;
			}

			//var f050001Entity = f050001Repo.Find(o => o.SOURCE_NO == f160501.DESTROY_NO
			//							&& o.DC_CODE == f160501.DC_CODE
			//							&& o.GUP_CODE == f160501.GUP_CODE
			//							&& o.CUST_CODE == f160501.CUST_CODE);
			//if (f050001Entity == null || f050001Entity.PROC_FLAG == "1")
			//{
			//	return new ExecuteResult(false, Properties.Resources.P160501Service_Destroy_No_Status_NeedPendingToModify);
			//}

			//更新主檔
			var f160501Repo = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160501Result = f160501Repo.UpdateF160501s(f160501);

			// 更新Detail
			if (f160501Result)
			{
				//新增前先刪除明細 F160502
				var f160502Repo = new F160502Repository(Schemas.CoreSchema, _wmsTransaction);
				f160502Repo.DeleteF160502s(f160501.DESTROY_NO);
				//新增明細
				var srv160502 = new P160502Service(_wmsTransaction);
				var f160502result = srv160502.InsertF160502Detail(detailData, f160501.DESTROY_NO);

				//刪除序號明細 F160504
				var f160504Repo = new F160504Repository(Schemas.CoreSchema, _wmsTransaction);
				f160504Repo.DeleteF160504s(f160501.DESTROY_NO);
				//新增序號明細 
				var srv160504 = new P160504Service(_wmsTransaction);
				var f160504result = srv160504.InsertF160504Detail(serialData, f160501.DESTROY_NO);

				//更新訂單池
				//刪除
				//var f050001Data = f050001Repo.Find(x => x.SOURCE_NO == f160501.DESTROY_NO);

				//if (f050001Data != null)
				//{
				//	f050001Repo.DeleteF050001(f050001Data.ORD_NO);
				//	f050002Repo.DeleteF050002(f050001Data.ORD_NO);
				//}
				////新增
				//var f050001result = F160501WithUpdteF050001(f160501, detailData, f160501.DESTROY_NO);

				//刪除派車記錄
				if (f160501.DISTR_CAR == "0")
				{
					var srv700102 = new P70.Services.P700102Service(_wmsTransaction);
					var f700102result = srv700102.DeleteDistrCarRecord(f160501.DESTROY_NO, f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE);
				}
			}

			return result;
		}


		public ExecuteResult DeleteF160501s(F160501 f160501)
		{
			var result = new ExecuteResult { IsSuccessed = true };


			//更新主檔
			var f160501Repo = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160501Result = f160501Repo.UpdateF160501Status(f160501.DESTROY_NO, "9", f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE);

			// 更新Detail
			if (f160501Result)
			{

				//刪除序號明細 F160504
				var f160504Repo = new F160504Repository(Schemas.CoreSchema, _wmsTransaction);
				f160504Repo.DeleteF160504s(f160501.DESTROY_NO);

				////刪除訂單池
				var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050001Data = f050001Repo.Find(x => x.SOURCE_NO == f160501.DESTROY_NO);

				if (f050001Data != null)
				{
					f050001Repo.DeleteF050001(f050001Data.ORD_NO);
					f050002Repo.DeleteF050002(f050001Data.ORD_NO);
				}

				//刪除派車記錄
				if (f160501.DISTR_CAR == "0")
				{
					var srv700102 = new P70.Services.P700102Service(_wmsTransaction);
					var f700102result = srv700102.DeleteDistrCarRecord(f160501.DESTROY_NO, f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE);
				}
			}

			return result;
		}

		#region 新增訂單池
		public bool F160501WithUpdteF050001(F160501 f160501, List<F160502Data> detailData, string destoryNo)
		{
			var sharedService = new SharedService(_wmsTransaction);
			// 抓取貨主檔(F1909)
			var f1909repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909Data = f1909repo.Find(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(f160501.GUP_CODE)
										&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f160501.CUST_CODE));

			// 抓取 Ticket_ID
			var ticketId = sharedService.GetTicketID(f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE, "OB");

			//新增出貨訂單池 F050001 F050002
			#region 新增 F050001 F050002 出貨資訊

			var ordNo = sharedService.GetNewOrdCode("S");  //系統訂單 F000901
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);

			F050001 f050001 = new F050001()
			{
				ORD_NO = ordNo,
				//CUST_ORD_NO = "",  //待確認			
				ORD_TYPE = "0",
				//RETAIL_CODE = "",
				ORD_DATE = DateTime.Today,
				PROC_FLAG = "0",
				CUST_NAME = f1909Data == null ? "" : f1909Data.CUST_NAME,
				SELF_TAKE = f160501.DISTR_CAR == "1" ? "0" : "1",   //待確認
				FRAGILE_LABEL = "0",
				GUARANTEE = "0",
				SA = "0",
				GENDER = "0",
				//AGE = "",
				//SA_QTY = "",
				TEL = f1909Data == null ? "" : f1909Data.ITEM_TEL,
				ADDRESS = f1909Data == null ? "" : f1909Data.ADDRESS,
				ORDER_BY = 0,
				CONSIGNEE = f1909Data == null ? "" : f1909Data.ITEM_CONTACT,
				ARRIVAL_DATE = f160501.DESTROY_DATE,
				TRAN_CODE = "OD",
				//SP_DELV = "",
				//CUST_COST = "",
				//BATCH_NO = "",
				//CHANNEL = "",
				POSM = "0",
				CONTACT = f1909Data == null ? "" : f1909Data.ITEM_CONTACT,
				CONTACT_TEL = f1909Data == null ? "" : f1909Data.ITEM_TEL,
				TEL_2 = f1909Data == null ? "" : f1909Data.ITEM_TEL,
				SPECIAL_BUS = f160501.DISTR_CAR == "1" ? "1" : "0",
				//ALL_ID = "",
				COLLECT = "0",
				//COLLECT_AMT = "",
				//MEMO = "",
				//SAP_MODULE = "",
				SOURCE_TYPE = "08",
				SOURCE_NO = destoryNo,
				GUP_CODE = f160501.GUP_CODE,
				CUST_CODE = f160501.CUST_CODE,
				DC_CODE = f160501.DC_CODE,
				TYPE_ID = "D",
				CAN_FAST = "0",
				TEL_1 = f1909Data == null ? "" : f1909Data.ITEM_TEL,
				//TEL_AREA = "",
				PRINT_RECEIPT = "0",
				//RECEIPT_NO = "",
				//ZIP_CODE = "",
				TICKET_ID = ticketId
			};

			f050001Repo.Add(f050001);

			//新增 F050002
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
			var recordCtn = 0;
			foreach (var items in detailData)
			{
				recordCtn += 1;
				F050002 f050002 = new F050002()
				{
					DC_CODE = f050001.DC_CODE,
					GUP_CODE = f050001.GUP_CODE,
					CUST_CODE = f050001.CUST_CODE,
					ORD_NO = ordNo,
					ORD_QTY = items.DESTROY_QTY,
					ORD_SEQ = recordCtn.ToString(),
					ITEM_CODE = items.ITEM_CODE
				};
				f050002Repo.Add(f050002);
			}
			#endregion


			return true;
		}
		#endregion

		#region 檢查 虛擬商品序號 F2501 狀態 & F160504 是否重複
		public string CheckF2501SerialStatus(List<F160502Data> serialData, string destory_no)
		{
			string errorStr = "";
			var tmp_ItemCode = "";
			var f160504repo = new F160504Repository(Schemas.CoreSchema);

			foreach (var item in serialData)
			{
				//檢查序號(呼叫共用Function)
				var serialNoService = new SerialNoService();
				var checkResult = serialNoService.SerialNoStatusCheck(item.GUP_CODE, item.CUST_CODE, item.ITEM_SERIALNO, "C1");

				if (!checkResult.Checked && tmp_ItemCode != item.ITEM_CODE)
				{
					tmp_ItemCode = item.ITEM_CODE;
					errorStr += checkResult.Message + string.Format(Properties.Resources.P160501Service_ITEM + "\n", item.ITEM_CODE);
				}

				var f160504Data = f160504repo.Find(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE
										&& x.CUST_CODE == item.CUST_CODE && x.SERIAL_NO == item.ITEM_SERIALNO);

				//修改模式 排除自已單據
				if (!string.IsNullOrEmpty(destory_no) && f160504Data != null)
				{
					if (f160504Data.DESTROY_NO == destory_no)
						f160504Data = null;
				}

				if (f160504Data != null)
				{
					errorStr += string.Format(Properties.Resources.P160501Service_Virtual_Item_DuplicateImport + "\n", item.ITEM_CODE, item.ITEM_SERIALNO);
				}
			}
			return errorStr;
		}
		#endregion


		public IQueryable<F160501Data> Get160501QueryData(
			string dcItem, string gupCode, string custCode, string destoryNo, string postingSDate
			, string postingEDate, string custOrdNo, string status, string ordNo, string crtSDate, string crtEDate)
		{
			//建立日期
			var coverCrtSDate = (string.IsNullOrEmpty(crtSDate)) ? ((DateTime?)null) : Convert.ToDateTime(crtSDate);
			var coverCrtEDate = (string.IsNullOrEmpty(crtEDate)) ? ((DateTime?)null) : Convert.ToDateTime(crtEDate);

			//過帳日期
			var coverPostingSDate = (string.IsNullOrEmpty(postingSDate)) ? ((DateTime?)null) : Convert.ToDateTime(postingEDate);
			var coverPostingEDate = (string.IsNullOrEmpty(postingEDate)) ? ((DateTime?)null) : Convert.ToDateTime(postingEDate);

			var repF160501 = new F160501Repository(Schemas.CoreSchema);
			return repF160501.Get160501QueryData(dcItem, gupCode, custCode, destoryNo, coverPostingSDate, coverPostingEDate, custOrdNo
			, status, ordNo, coverCrtSDate, coverCrtEDate);
		}


		public IQueryable<F160501Status> GetF160501Status(string dcCode, string gupCode, string custCode, string destoryNo)
		{
			var f160501Repo = new F160501Repository(Schemas.CoreSchema);
			return f160501Repo.GetF160501Status(dcCode,gupCode,custCode,destoryNo);
		}

		public IQueryable<F160502Data> GetF1913ScrapData(string dcCode, string gupCode, string custCode)
		{

			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			return f1913Repo.GetF1913ScrapData(dcCode, gupCode, custCode);
		}

		public ExecuteResult P160501Shipment(F160501Data f160501Data)
		{
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);

			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == f160501Data.GUP_CODE && x.CUST_CODE == f160501Data.CUST_CODE);
			var p200101Service = new P200101Service(_wmsTransaction);
			var f160501Repo = new F160501Repository(Schemas.CoreSchema);
			var f160501 = f160501Repo.Find(x => x.DC_CODE == f160501Data.DC_CODE && x.GUP_CODE == f160501Data.GUP_CODE && x.CUST_CODE == f160501Data.CUST_CODE && x.DESTROY_NO == f160501Data.DESTROY_NO);
			if (f160501.STATUS != "0")
				return new ExecuteResult(false, Properties.Resources.P160501Service_DestoryMustStatusInUnProcess);
			var f160502Repo = new F160502Repository(Schemas.CoreSchema);
			var detailData = f160502Repo.Get160502DetailData(f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE,f160501.DESTROY_NO).ToList();
			var f160504Repo = new F160504Repository(Schemas.CoreSchema);
			var serialData = f160504Repo.Get160504SerialData(f160501.DC_CODE, f160501.GUP_CODE, f160501.CUST_CODE, f160501.DESTROY_NO).ToList();
			var errorStr = CheckF2501SerialStatus(serialData, f160501.DESTROY_NO);
			if (!string.IsNullOrEmpty(errorStr))
				return new ExecuteResult(false, errorStr);

			var message = string.Empty;
			switch (f1909.DESTROY_TYPE)
			{
				case "0"://出貨模式
					var f050001Data = f050001Repo.Find(x => x.SOURCE_NO == f160501.DESTROY_NO);
					if (f050001Data != null)
					{
						f050001Repo.DeleteF050001(f050001Data.ORD_NO);
						f050002Repo.DeleteF050002(f050001Data.ORD_NO);
					}
					var f050001result = F160501WithUpdteF050001(f160501, detailData, f160501.DESTROY_NO);
					message = Properties.Resources.P060204Service_VnrRtnToShip;
					f160501.STATUS = "3";//處理中
					break;
				case "1"://直接扣帳
					var result = p200101Service.DestoryShipDebit(f160501, detailData);
					if (!result.IsSuccessed)
						return result;

					message = string.Format(Properties.Resources.P160501Service_DestoryShipDebit, result.No);
					f160501.POSTING_DATE = DateTime.Now;
					f160501.STATUS = "4";//結案
					break;
				default:
					break;
			}
			f160501Repo.Update(f160501);
			return new ExecuteResult(true, message);
		}
	}
}
