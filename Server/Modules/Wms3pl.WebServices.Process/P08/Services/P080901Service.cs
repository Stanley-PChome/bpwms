
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080901Service
	{
		private WmsTransaction _wmsTransaction;
		public P080901Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult UpdateCarNo(string dcCode, string gupCode, string custCode, DateTime takeDate,
			string takeTime, string allId, string carNoA, string carNoB, string carNoC, string needSeal)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var datas = f700102Repo.AsForUpdate().GetF700102List(dcCode, gupCode, custCode, takeDate, takeTime, allId);
			foreach (var f700102 in datas)
			{
				f700102.CAR_NO_A = carNoA;
				f700102.CAR_NO_B = carNoB;
				f700102.CAR_NO_C = carNoC;
				f700102Repo.Update(f700102);
			}
			if (needSeal != "1")
			{
				var wmsNoList = datas.Select(o => o.WMS_NO);
				var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
				var f050801Datas = f050801Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, takeDate, "6");
				f050801Datas = f050801Datas.Where(o => wmsNoList.Contains(o.WMS_ORD_NO));
				var sharedService = new SharedService(_wmsTransaction);
				foreach (var f050801Data in f050801Datas)
				{
					f050801Data.STATUS = 5;
					f050801Data.INCAR_DATE = DateTime.Now;
					f050801Data.INCAR_NAME = Current.StaffName;
					f050801Data.INCAR_STAFF = Current.Staff;
					f050801Repo.Update(f050801Data);
					sharedService.UpdateSourceNoStatus(SourceType.Order, f050801Data.DC_CODE, f050801Data.GUP_CODE
									 , f050801Data.CUST_CODE, f050801Data.WMS_ORD_NO, f050801Data.STATUS.ToString());
				}
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		public ExecuteResult UpdateIsSeal(string dcCode, string gupCode, string custCode, DateTime takeDate,
			string takeTime, string allId)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var datas = f700102Repo.AsForUpdate().GetF700102List(dcCode, gupCode, custCode, takeDate, takeTime, allId);
			foreach (var f700102 in datas)
			{
				f700102.ISSEAL = "1";
				f700102Repo.Update(f700102);
			}
			var wmsNoList = datas.Select(o => o.WMS_NO);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Datas = f050801Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, takeDate, "6");
			f050801Datas = f050801Datas.Where(o => wmsNoList.Contains(o.WMS_ORD_NO));
			var sharedService = new SharedService(_wmsTransaction);
			foreach (var f050801Data in f050801Datas)
			{
				f050801Data.STATUS = 5;
				f050801Data.INCAR_DATE = DateTime.Now;
				f050801Repo.Update(f050801Data);
				sharedService.UpdateSourceNoStatus(SourceType.Order, f050801Data.DC_CODE, f050801Data.GUP_CODE
									, f050801Data.CUST_CODE, f050801Data.WMS_ORD_NO, f050801Data.STATUS.ToString());
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		/// <summary>
		/// 從該取件日期、出車時段、配送商的派車明細，往派車主檔找派車明細的所有出貨單，當所有出貨單都為已裝車狀態已上，就更新派車單狀態為結案，
		/// 若只有部分已裝車，則改為處理中。
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="takeDate"></param>
		/// <param name="takeTime"></param>
		/// <param name="allId"></param>
		public void UpdateDistrCarStatus(string dcCode, string gupCode, string custCode, DateTime takeDate,
			string takeTime, string allId)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);

			var f700102s = f700102Repo.GetF700102List(dcCode, gupCode, custCode, takeDate, takeTime, allId);
			foreach (var f700102 in f700102s)
			{
				var f050801s = f050801Repo.GetF050801sByDistrCarNo(f700102.DC_CODE, f700102.DISTR_CAR_NO);

				// 當所有出貨單都已經裝車後(狀態為已扣帳)，就能更新派車單狀態為已結案
				// 若只有部分已裝車(狀態為已扣帳)，則改為處理中
				var status = string.Empty;
				if (f050801s.All(x => x.STATUS == 5 || x.STATUS == 6)) //全部已扣帳
					status = "2";
				else if (f050801s.Any(x => x.STATUS == 5 || x.STATUS == 6)) //部分已扣帳
					status = "1";

				if (!string.IsNullOrEmpty(status))
				{
					var f700101 = f700101Repo.Find(x => x.DC_CODE == f700102.DC_CODE && x.DISTR_CAR_NO == f700102.DISTR_CAR_NO);
					f700101.STATUS = status;
					f700101Repo.Update(f700101);
				}
			}
		}
	
	}
}

