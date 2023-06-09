using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P25.Services
{
	public partial class P250302Service
	{
		private WmsTransaction _wmsTransaction;
		public P250302Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P250302QueryItem> GetP250302QueryData(string gupCode, string custCode, string clientIp, string onlyPass)
		{
			var repF250106 = new F250106Repository(Schemas.CoreSchema);
			return repF250106.GetF250106Data(gupCode, custCode, clientIp, onlyPass);
		}

		public ExecuteResult CheckOldSerialNo(string gupCode, string custCode, string itemCode, string serialNo)
		{
			var serialNoService = new SerialNoService();
			return serialNoService.CheckSerialByInHouse(gupCode, custCode, itemCode, serialNo);
		}

		public void CheckNewSerialNo(string gupCode, string custCode, string itemCode, string oldSerialNo, string newSerialNo,
		  string clientIp, string userId, string userName)
		{
			var serialNoService = new SerialNoService();
			var result = serialNoService.SerialNoStatusCheckAll(gupCode, custCode, itemCode, newSerialNo, "A1");
			InsertF250106(result, oldSerialNo, clientIp, gupCode, custCode, userId, userName);
		}

		private void InsertF250106(SerialNoResult result, string oldSerialNo,
		  string clientIp, string gupCode, string custCode, string userId, string userName)
		{
			var repF250106 = new F250106Repository(Schemas.CoreSchema);
			var f250106 = new F250106
			{
				SERIAL_NO = oldSerialNo,
				ITEM_CODE = result.ItemCode,
				NEW_SERIAL_NO = result.SerialNo,
				SERIAL_STATUS = result.CurrentlyStatus,
				ISPASS = result.Checked ? "1" : "0",
				MESSAGE = result.Message,
				STATUS = "0",
				CLIENT_IP = clientIp,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CRT_STAFF = userId,
				CRT_NAME = userName,
				CRT_DATE = DateTime.Now
			};
			repF250106.Add(f250106);
		}

		private void InsertF250106(F250106Repository rep, string itemCode, string oldSerialNo, string newSerialNo, string serialStatus,
		  string isPass, string msg, string clientIp, string gupCode, string custCode, string userId, string userName)
		{
			var f250106 = new F250106
			{
				SERIAL_NO = oldSerialNo,
				ITEM_CODE = itemCode,
				NEW_SERIAL_NO = newSerialNo,
				SERIAL_STATUS = serialStatus,
				ISPASS = isPass,
				MESSAGE = msg,
				STATUS = "0",
				CLIENT_IP = clientIp,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CRT_STAFF = userId,
				CRT_NAME = userName,
				CRT_DATE = DateTime.Now
			};
			rep.Add(f250106);
		}

		//序號更換
		public ExecuteResult ChangeSerialNo(List<P250302QueryItem> listData, string dcCode)
		{
			var serialNoService = new SerialNoService(_wmsTransaction);
			var repF2501 = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var repF250106 = new F250106Repository(Schemas.CoreSchema, _wmsTransaction);
			var repF1913 = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var repF250101 = new F250101Repository(Schemas.CoreSchema, _wmsTransaction);

			var gupCode = listData.Select(x => x.GUP_CODE).FirstOrDefault();
			var custCode = listData.Select(x => x.CUST_CODE).FirstOrDefault();

			var f2501s = repF2501.AsForUpdate().InWithTrueAndCondition("SERIAL_NO", listData.Select(x => x.SERIAL_NO).ToList(), p => p.GUP_CODE == gupCode && p.CUST_CODE == custCode);

			foreach (var d in listData)
			{
				// 檢查舊序號是否存在序號主檔
				var f2501 = f2501s.FirstOrDefault(p => p.GUP_CODE == d.GUP_CODE && p.CUST_CODE == d.CUST_CODE && p.SERIAL_NO == d.SERIAL_NO);
				if (f2501 == null)
					return new ExecuteResult(false, Properties.Resources.P250302Service_SERIAL_NO + d.SERIAL_NO);

				// 檢查舊序號的狀態是否為在庫，若不是在庫的狀態，則不能換新序號
				if (f2501.STATUS != "A1")
					return new ExecuteResult(false, string.Format(Properties.Resources.P250302Service_SERIAL_NO_NOT_IN_STOCK, d.SERIAL_NO));

				var result = serialNoService.SerialNoStatusCheckAll(d.GUP_CODE, d.CUST_CODE, d.ITEM_CODE, d.NEW_SERIAL_NO, "A1");
				if (!string.IsNullOrWhiteSpace(result.Message))
					return new ExecuteResult(false, result.Message);

				// 更新F2501
				repF2501.UpdateNewSnByOldSn(d.GUP_CODE, d.CUST_CODE, d.SERIAL_NO, d.NEW_SERIAL_NO);
		
				//更新F250106
				var f250106 = repF250106.Find(p => p.LOG_SEQ == d.LOG_SEQ);
				if (f250106 != null)
				{
					f250106.STATUS = "1";
					f250106.UPD_NAME = d.UPD_NAME;
					f250106.UPD_STAFF = d.UPD_STAFF;
					f250106.UPD_DATE = DateTime.Now;
					repF250106.Update(f250106);
				}

				//更新F1913
				repF1913.UpdateF1913SerialNo(d.SERIAL_NO, d.NEW_SERIAL_NO, d.GUP_CODE, d.CUST_CODE, d.UPD_STAFF, d.UPD_NAME);
			}

			return new ExecuteResult(true);
		}

		public void CheckImportData(DataTable dt, string gupCode, string custCode, string clientIp, string userId, string userName)
		{
			//檢查Excel資料
			var duplicates = dt.AsEnumerable().GroupBy(r => r[1]).Where(gr => gr.Count() > 1).ToList();
			if (duplicates.Any())
				throw new Exception(string.Format(Properties.Resources.P250302Service_OldSerialNo_Duplicate, string.Join(", ", duplicates.Select(dupl => dupl.Key))));
			duplicates = dt.AsEnumerable().GroupBy(r => r[2]).Where(gr => gr.Count() > 1).ToList();
			if (duplicates.Any())
				throw new Exception(string.Format(Properties.Resources.P250302Service_NewSerialNo_Duplicate, string.Join(", ", duplicates.Select(dupl => dupl.Key))));

			var repF1903 = new F1903Repository(Schemas.CoreSchema);
			var repF250106 = new F250106Repository(Schemas.CoreSchema, _wmsTransaction);
			var serialNoService = new SerialNoService();
			var itemCodes = dt.AsEnumerable().Select(row => row.Field<string>(0)).Distinct();
			foreach (var itemCode in itemCodes)
			{
				var rows = from row in dt.AsEnumerable()
						   where row.Field<string>(0) == itemCode
						   select row;

				//檢查ItemCode
				var f1903 = repF1903.Find(p => p.ITEM_CODE == itemCode && p.GUP_CODE == gupCode && p.CUST_CODE == custCode);
				if (f1903 == null)
				{
					var msg = Properties.Resources.P250302Service_ITEM_CODE_NotFound;
					foreach (var r in rows)
					{
						InsertF250106(repF250106, r[0].ToString(), r[1].ToString(), r[2].ToString(), null, "0", msg, clientIp,
						  gupCode, custCode, userId, userName);
					}
					continue;
				}

				foreach (var r in rows)
				{
					var oldSerialNo = r[1].ToString();
					//檢查舊序號是否已存在刷讀資料
					var exists =
					  repF250106.Filter(p => p.SERIAL_NO == oldSerialNo && p.ISPASS == "1" && p.STATUS == "0").Any();
					if (exists)
					{
						InsertF250106(repF250106, r[0].ToString(), r[1].ToString(), r[2].ToString(), null, "0", Properties.Resources.P250302Service_ScanedData_Skip,
						  clientIp, gupCode, custCode, userId, userName);
						continue;
					}

					//檢查舊序號是否在庫
					var result = serialNoService.CheckSerialByInHouse(gupCode, custCode, itemCode, r[1].ToString());
					if (!result.IsSuccessed)
					{
						InsertF250106(repF250106, r[0].ToString(), r[1].ToString(), r[2].ToString(), null, "0", result.Message,
						  clientIp, gupCode, custCode, userId, userName);
						continue;
					}

					//檢查新序號
					var result2 = serialNoService.SerialNoStatusCheckAll(gupCode, custCode, itemCode, r[2].ToString(), "A1");
					InsertF250106(result2, r[1].ToString(), clientIp, gupCode, custCode, userId, userName);
				}
			}
		}
	}
}
