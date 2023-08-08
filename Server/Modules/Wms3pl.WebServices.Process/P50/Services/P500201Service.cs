using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P50.Services
{
	public partial class P500201Service
	{
		private WmsTransaction _wmsTransaction;
		public P500201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F500201ClearingData> GetF500201ClearingData(string gupCode, string custCode, string outSourceId, string clearingYearMonth)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema);
			return f500201Repo.GetF500201ClearingData(gupCode, custCode, outSourceId, clearingYearMonth);
		}

		/// <summary>
		/// 更新F500201列印時間
		/// </summary>
		private void UpdatePrintDate(F500201Repository f500201Repo, DateTime cntDate, string gupCode,string custCode)
		{			
			f500201Repo.SettlementPrint(Current.Staff, Current.StaffName, cntDate, gupCode, custCode);			
		}

		/// <summary>
		/// 請款總表
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="outSourceId"></param>
		/// <param name="cntDate"></param>
		/// <returns></returns>
		public IQueryable<RP7105100001> GetRp7105100001Data(string gupCode, string custCode, string outSourceId, DateTime cntDate)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema);
			UpdatePrintDate(f500201Repo, cntDate, gupCode,custCode);
			return f500201Repo.GetRp7105100001Data(gupCode, custCode, outSourceId, cntDate);
		}

		/// <summary>
		/// 出貨處理費明細表
		/// </summary>
		/// <param name="cntDate"></param>
		/// <param name="contractNo"></param>
		/// <returns></returns>
		public IQueryable<RP7105100002> GetRp7105100002Data(DateTime cntDate, string contractNo)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);			
			var reportList = f500201Repo.GetRp7105100002Data(cntDate, contractNo);
			var lastData = new RP7105100002();
			foreach (var data in reportList)
			{
				//將第一筆以外的金額歸0
				if (lastData.WMS_ORD_NO == data.WMS_ORD_NO)
				{
					data.FEE = 0;
					data.TOTAL_AMOUNT = 0;
				}
				lastData = data;
			}
			return reportList;			
		}

		/// <summary>
		/// 退貨處理費明細表
		/// </summary>
		/// <param name="cntDate"></param>
		/// <param name="contractNo"></param>
		/// <returns></returns>
		public IQueryable<RP7105100003> GetRp7105100003Data(DateTime cntDate, string contractNo)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);			
			var reportList = f500201Repo.GetRp7105100003Data(cntDate, contractNo);
			var lastData = new RP7105100003();
			foreach (var data in reportList)
			{
				//將第一筆以外的金額歸0
				if (lastData.WMS_NO == data.WMS_NO)
				{
					data.FEE = 0;
					data.PRICE = 0;
				}
				lastData = data;
			}
			return reportList;
		}

		/// <summary>
		/// 退貨驗收請款明細
		/// </summary>
		/// <param name="cntDate"></param>
		/// <param name="contractNo"></param>
		/// <returns></returns>
		public IQueryable<RP7105100004> GetRp7105100004Data(DateTime cntDate, string contractNo)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);
			var reportList = f500201Repo.GetRp7105100004Data(cntDate, contractNo);
			var lastData = new RP7105100004();
			foreach (var data in reportList)
			{
				//將第一筆以外的金額歸0
				if (lastData.WMS_NO == data.WMS_NO && lastData.ITEM_CODE == data.ITEM_CODE)
				{
					data.PRICE = 0;
					data.TOTAL = 0;
				}
				lastData = data;
			}
			return reportList;
		}

		/// <summary>
		/// 其他運費請款明細
		/// </summary>
		/// <param name="cntDate"></param>
		/// <param name="contractNo"></param>
		/// <returns></returns>
		public IQueryable<RP7105100005> GetRp7105100005Data(DateTime cntDate, string contractNo)
		{
			var f500201Repo = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);			
			return f500201Repo.GetRp7105100005Data(cntDate, contractNo);
		}

		public ExecuteResult SettlementClosing(string updStaff, string updStaffName, DateTime cntDate, string gupCode, string custCode)
		{
			var result = new ExecuteResult { IsSuccessed = true, Message = "" };
			var f500201Repo = new F500201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f199007Repo = new F199007Repository(Schemas.CoreSchema, _wmsTransaction);

			f500201Repo.SettlementClosing(updStaff, updStaffName, cntDate, gupCode, custCode);
			//更新專案計價項目狀態:結案
			f199007Repo.UpdateF199007Status(cntDate, gupCode, custCode);
			return result;
		}
	}
}
