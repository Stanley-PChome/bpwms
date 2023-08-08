using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P21.Services
{
	public partial class P211602Service
	{
		private WmsTransaction _wmsTransaction;
		public P211602Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 任務派發查詢
		public IQueryable<TaskDispatchData> GetF060101Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var f060101Repo = new F060101Repository(Schemas.CoreSchema);
			var f060101s = f060101Repo.GetF060101Data(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums).ToList();
			f060101s.ForEach(w => {
				w.ENABLE = f060101s.GroupBy(x => x.WMS_NO).Select(x => x.Max(y => y.DOC_ID)).Contains(w.DOC_ID) ? true : false;
				w.ENABLE = w.ENABLE == true && (w.STATUS == "F" || w.STATUS == "T") ? true : false;
			});
			return f060101s.AsQueryable();
		}

		public IQueryable<TaskDispatchData> GetF060201Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			var f060201s = f060201Repo.GetF060201Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums).ToList();
			f060201s.ForEach(w => {
				w.ENABLE = f060201s.GroupBy(x => x.WMS_NO).Select(x => x.Max(y => y.DOC_ID)).Contains(w.DOC_ID) ? true : false;
				w.ENABLE = w.ENABLE == true && (w.STATUS == "F" || w.STATUS == "T") ? true : false;
			});
			return f060201s.AsQueryable();
		}

		public IQueryable<TaskDispatchData> GetF060401Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var f060401Repo = new F060401Repository(Schemas.CoreSchema);
			var f060401s = f060401Repo.GetF060401Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums).ToList();
			f060401s.ForEach(w => {
				w.ENABLE = f060401s.GroupBy(x => x.WMS_NO).Select(x => x.Max(y => y.DOC_ID)).Contains(w.DOC_ID) ? true : false;
				w.ENABLE = w.ENABLE == true && (w.STATUS == "F" || w.STATUS == "T") ? true : false;
			});
			return f060401s.AsQueryable();
		}

		public IQueryable<TaskDispatchData> GetF060404Datas(string dcCode, string gupCode, string custCode, DateTime? beginCreateDate, DateTime? endCreateDate, string status, List<string> docNums, List<string> taskNums)
		{
			var f060404Repo = new F060404Repository(Schemas.CoreSchema);
			var f060404s = f060404Repo.GetF060404Datas(dcCode, gupCode, custCode, beginCreateDate, endCreateDate, status, docNums, taskNums).ToList();
			f060404s.ForEach(w => {
				w.ENABLE = f060404s.GroupBy(x => x.WMS_NO).Select(x => x.Max(y => y.DOC_ID)).Contains(w.DOC_ID) ? true : false;
				w.ENABLE = w.ENABLE == true && (w.STATUS == "F" || w.STATUS == "T") ? true : false;
			});
			return f060404s.AsQueryable();
		}
		public ExecuteResult OriginalOrderRedistribution(string schedule,List<string> dcoIds)
		{
			var f060101Repo = new F060101Repository(Schemas.CoreSchema);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			var f060401Repo = new F060401Repository(Schemas.CoreSchema);
			var f060404Repo = new F060404Repository(Schemas.CoreSchema);
			var f060703Repo = new F060703Repository(Schemas.CoreSchema);

			if (dcoIds == null || !dcoIds.Any() )
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請選擇派發任務" };
			}
			if (dcoIds.Count > 1)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "只能選擇一個派發任務" };
			}
			switch (schedule)
			{
				case "F060101":
					var f060101 = f060101Repo.Find(x => x.DOC_ID == dcoIds.FirstOrDefault());
					f060101Repo.UpdateF060101(dcoIds);
					f060703Repo.Add(new F060703 {
						DC_CODE = f060101.DC_CODE,
						GUP_CODE = f060101.GUP_CODE,
						CUST_CODE = f060101.CUST_CODE,
						ORI_DOC_ID = f060101.DOC_ID,
						ORI_TABLE = schedule,
						ORI_STATUS = f060101.STATUS,
						ORI_CMD_TYPE = f060101.CMD_TYPE,
						ORI_PROC_DATE = f060101.PROC_DATE,
						ORI_MESSAGE = f060101.MESSAGE
					});
					break;
				case "F060201":
					var f060201 = f060201Repo.Find(x => x.DOC_ID == dcoIds.FirstOrDefault());
					f060201Repo.UpdateF060201(dcoIds);
					f060703Repo.Add(new F060703
					{
						DC_CODE = f060201.DC_CODE,
						GUP_CODE = f060201.GUP_CODE,
						CUST_CODE = f060201.CUST_CODE,
						ORI_DOC_ID = f060201.DOC_ID,
						ORI_TABLE = schedule,
						ORI_STATUS = f060201.STATUS,
						ORI_CMD_TYPE = f060201.CMD_TYPE,
						ORI_PROC_DATE = f060201.PROC_DATE,
						ORI_MESSAGE = f060201.MESSAGE
					});
					break;
				case "F060401":
					var f060401 = f060401Repo.Find(x => x.DOC_ID == dcoIds.FirstOrDefault());
					f060401Repo.UpdateF060401(dcoIds);
					f060703Repo.Add(new F060703
					{
						DC_CODE = f060401.DC_CODE,
						GUP_CODE = f060401.GUP_CODE,
						CUST_CODE = f060401.CUST_CODE,
						ORI_DOC_ID = f060401.DOC_ID,
						ORI_TABLE = schedule,
						ORI_STATUS = f060401.STATUS,
						ORI_CMD_TYPE = f060401.CMD_TYPE,
						ORI_PROC_DATE = f060401.PROC_DATE,
						ORI_MESSAGE = f060401.MESSAGE
					});
					break;
				case "F060404":
					var f060404 = f060404Repo.Find(x => x.DOC_ID == dcoIds.FirstOrDefault());
					f060404Repo.UpdateF060404(dcoIds);
					f060703Repo.Add(new F060703
					{
						DC_CODE = f060404.DC_CODE,
						GUP_CODE = f060404.GUP_CODE,
						CUST_CODE = f060404.CUST_CODE,
						ORI_DOC_ID = f060404.DOC_ID,
						ORI_TABLE = schedule,
						ORI_STATUS = f060404.STATUS,
						ORI_CMD_TYPE = f060404.CMD_TYPE,
						ORI_PROC_DATE = f060404.PROC_DATE,
						ORI_MESSAGE = f060404.MESSAGE
					});
					break;
				default:
					break;
			}
			return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult DoAssignNewTasks(string schedule, List<string> docIds )
		{
			var wmsNos = new List<string>();
			if (string.IsNullOrWhiteSpace(schedule))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請選擇排程名稱" };
			}
			if(!docIds.Any()|| docIds == null)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請選擇任務單號" };
			}
			switch (schedule)
			{
				case "F060101":
					var f060101Repo = new F060101Repository(Schemas.CoreSchema);
					wmsNos = f060101Repo.GetWmsNoByWmsNo(docIds).ToList();
					var f060101s = f060101Repo.GetMaxDocId(wmsNos);
					List<F060101> newF060101s = new List<F060101>();
					foreach (var f060101 in f060101s)
					{
						var docId = string.Empty;
						if (f060101.DOC_ID == f060101.WMS_NO)
						{
							docId = f060101.DOC_ID + "01";
						}
						else
						{
							docId = f060101.WMS_NO.Substring(0, 15) + (Convert.ToInt32(f060101.DOC_ID.Substring(f060101.DOC_ID.Length - 2, 2)) + 1).ToString().PadLeft(2,'0');
						}
						newF060101s.Add(new F060101
						{
							DOC_ID = docId,
							DC_CODE = f060101.DC_CODE,
							GUP_CODE = f060101.GUP_CODE,
							CUST_CODE = f060101.CUST_CODE,
							WAREHOUSE_ID = f060101.WAREHOUSE_ID,
							WMS_NO = f060101.WMS_NO,
							CMD_TYPE = f060101.CMD_TYPE,
							STATUS = "0",
							PROC_DATE = null,
							MESSAGE = null,
							RESENT_CNT = 0,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName,
						});
					}
					f060101Repo.BulkInsert(newF060101s);
					break;
				case "F060201":
					var f060201Repo = new F060201Repository(Schemas.CoreSchema);
					wmsNos = f060201Repo.GetWmsNoByWmsNo(docIds).ToList();
					var f060201s = f060201Repo.GetMaxDocId(wmsNos);
					List<F060201> newF060201s = new List<F060201>();
					foreach (var f060201 in f060201s)
					{
						var docId = string.Empty;
						if (f060201.DOC_ID == f060201.WMS_NO)
						{
							docId = f060201.DOC_ID + "01";
						}
						else
						{
							docId = f060201.WMS_NO.Substring(0, 15) + (Convert.ToInt32(f060201.DOC_ID.Substring(f060201.DOC_ID.Length - 2, 2)) + 1).ToString().PadLeft(2, '0');
						}
						newF060201s.Add(new F060201
						{
							DOC_ID = docId,
							DC_CODE = f060201.DC_CODE,
							GUP_CODE = f060201.GUP_CODE,
							CUST_CODE = f060201.CUST_CODE,
							WAREHOUSE_ID = f060201.WAREHOUSE_ID,
							WMS_NO = f060201.WMS_NO,
							PICK_NO = f060201.PICK_NO,
							CMD_TYPE = f060201.CMD_TYPE,
							STATUS = "0",
							PROC_DATE = null,
							MESSAGE = null,
							RESENT_CNT = 0,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName,
						});
					}
					f060201Repo.BulkInsert(newF060201s);
					break;
				case "F060401":
					var f060401Repo = new F060401Repository(Schemas.CoreSchema);
					wmsNos = f060401Repo.GetWmsNoByWmsNo(docIds).ToList();
					var f060401s = f060401Repo.GetMaxDocId(wmsNos);
					List<F060401> newF060401s = new List<F060401>();
					foreach (var f060401 in f060401s)
					{
						var docId = string.Empty;
						if (f060401.DOC_ID == f060401.WMS_NO)
						{
							docId = f060401.DOC_ID + "01";
						}
						else
						{
							docId = f060401.WMS_NO.Substring(0,15) + (Convert.ToInt32(f060401.DOC_ID.Substring(f060401.DOC_ID.Length - 2, 2)) + 1).ToString().PadLeft(2, '0');
						}
						newF060401s.Add(new F060401
						{
							DOC_ID = docId,
							DC_CODE = f060401.DC_CODE,
							GUP_CODE = f060401.GUP_CODE,
							CUST_CODE = f060401.CUST_CODE,
							WAREHOUSE_ID = f060401.WAREHOUSE_ID,
							WMS_NO = f060401.WMS_NO,
							CMD_TYPE = f060401.CMD_TYPE,
							STATUS = "0",
							PROC_DATE = null,
							ISSECOND = f060401.ISSECOND,
							MESSAGE = null,
							RESENT_CNT = 0,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName,
						});
					}
					f060401Repo.BulkInsert(newF060401s);
					break;
				case "F060404":
					var f060404Repo = new F060404Repository(Schemas.CoreSchema);
					wmsNos = f060404Repo.GetWmsNoByWmsNo(docIds).ToList();
					var f060404s = f060404Repo.GetMaxDocId(wmsNos);
					List<F060404> newF060404s = new List<F060404>();
					foreach (var f060404 in f060404s)
					{
						var docId = string.Empty;
						if (f060404.DOC_ID.Substring(1) == f060404.WMS_NO)
						{
							docId = f060404.DOC_ID + "01";
						}
						else
						{
							docId = $"{f060404.DOC_ID.Substring(0,1)}{f060404.WMS_NO.Substring(0, 15) + (Convert.ToInt32(f060404.DOC_ID.Substring(f060404.DOC_ID.Length - 2, 2)) + 1).ToString().PadLeft(2, '0')}";
						}
						newF060404s.Add(new F060404
						{
							DOC_ID = docId,
							DC_CODE = f060404.DC_CODE,
							GUP_CODE = f060404.GUP_CODE,
							CUST_CODE = f060404.CUST_CODE,
							WAREHOUSE_ID = f060404.WAREHOUSE_ID,
							WMS_NO = f060404.WMS_NO,
							CMD_TYPE = f060404.CMD_TYPE,
							STATUS = "0",
							PROC_DATE = null,
							MESSAGE = null,
							RESENT_CNT = 0,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName,
							CHECK_CODE = f060404.CHECK_CODE
						});
					}
					f060404Repo.BulkInsert(newF060404s);
					break;
				default:
					break;
			}


			return new ExecuteResult(true);
		}
		
		#endregion
	}
}

