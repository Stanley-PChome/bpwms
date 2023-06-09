
using System;
using System.Linq;
using System.Collections.Generic;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Process.P11.Services;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.Process.P08.Services;

namespace Wms3pl.WebServices.Process.P06.Services
{
	public partial class P060101Service
	{
		private WmsTransaction _wmsTransaction;
		public P060101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F050801WmsOrdNo> GetF050801ByDelvPickTime(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string custOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801ByDelvPickTime(dcCode, gupCode, custCode, delvDate, pickTime, custOrdNo);
		}

		/// <summary>
		/// 確認批次扣帳
		/// </summary>
		/// <param name="f0513Batchs">勾選的批次日期與批次時段資訊</param>
		/// <param name="f050801WmsOrdNos">若單純要針對某些出貨單做扣帳，則傳入這個出貨單號集合(只做過濾出貨單號用)</param>
		public ExecuteResult ConfirmBatchDebit(List<F0513> f0513Batchs, List<string> wmsOrdNos)
		{
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);

			var orderService = new OrderService(_wmsTransaction);
			var batchDelvList = new List<BatchDelv>();
			List<F0513> updF0513s;
			List<F05030202> updF05030202s;
			List<F050801> updF050801s;
			List<F050802> updF050802s;
			List<F1511> updF1511s;
			ExecuteResult result;
			if (wmsOrdNos != null && wmsOrdNos.Any())
			{
				var f0513 = f0513Batchs.First();
				result = orderService.MultiShipOrderDebit(f0513.DC_CODE, f0513.GUP_CODE, f0513.CUST_CODE, wmsOrdNos, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
			}
			else
			{
				batchDelvList.AddRange(f0513Batchs.Select(x => new BatchDelv { DC_CODE = x.DC_CODE, GUP_CODE = x.GUP_CODE, CUST_CODE = x.CUST_CODE, DELV_DATE = x.DELV_DATE, PICK_TIME = x.PICK_TIME, WmsOrders = null }));
				result = orderService.DoBatchDebit(batchDelvList, out updF050801s, out updF050802s, out updF0513s, out updF05030202s, out updF1511s);
			}

			if (result.IsSuccessed)
			{
				if (updF050801s.Any())
					f050801Repo.BulkUpdate(updF050801s);

				if (updF050802s.Any())
					f050802Repo.BulkUpdate(updF050802s);

				if (updF0513s.Any())
					f0513Repo.BulkUpdate(updF0513s);

				if (updF05030202s.Any())
					f05030202Repo.BulkUpdate(updF05030202s);

				if (updF1511s.Any())
					f1511Repo.BulkUpdate(updF1511s);

			}

			return result;
		}

	}
}
