using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;

namespace Wms3pl.WebServices.Schedule.S16.Services
{
	public class ReturnAllotService
	{
		private WmsTransaction _wmsTransation;
		public ReturnAllotService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		public ExecuteResult ExecReturnAllot(OrderAllotParam orderAllotParam)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransation);
			var f161202Repo = new F161202Repository(Schemas.CoreSchema, _wmsTransation);
			var f16120201Repo = new F16120201Repository(Schemas.CoreSchema, _wmsTransation);
			var f161402Repo = new F161402Repository(Schemas.CoreSchema);
			var movedData = f161402Repo.GetTransReturnData(orderAllotParam.DcCode, orderAllotParam.GupCode, orderAllotParam.CustCode).ToList();
			var f161202s = f161202Repo.GetAllotReturnData(orderAllotParam.DcCode, orderAllotParam.GupCode, orderAllotParam.CustCode);
			var insertF16120201 = new List<F16120201>();
			foreach (var moved in movedData)
			{
				var f161202TransData = f161202s.Where(o => o.DC_CODE == moved.DC_CODE && o.GUP_CODE == moved.GUP_CODE && o.CUST_CODE == moved.CUST_CODE && o.RETURN_NO == moved.RETURN_NO && o.ITEM_CODE == moved.ITEM_CODE);
				foreach (var f161202 in f161202TransData)
				{
					var aRtnQty = 0;
					if (moved.MOVED_QTY >= f161202.RTN_QTY)
					{
						aRtnQty = f161202.RTN_QTY;
						moved.MOVED_QTY -= f161202.RTN_QTY;
					}
					else if (moved.MOVED_QTY > 0 && moved.MOVED_QTY < f161202.RTN_QTY)
					{
						aRtnQty = moved.MOVED_QTY;
						moved.MOVED_QTY = 0;
					}

					insertF16120201.Add(
						new F16120201
						{
							DC_CODE = orderAllotParam.DcCode,
							GUP_CODE = orderAllotParam.GupCode,
							CUST_CODE = orderAllotParam.CustCode,
							ITEM_CODE = f161202.ITEM_CODE,
							RETURN_NO = f161202.RETURN_NO,
							RETURN_SEQ = f161202.RETURN_SEQ,
							RTN_QTY = f161202.RTN_QTY,
							A_RTN_QTY = aRtnQty
						});
				}
			}

			f16120201Repo.BulkInsert(insertF16120201);

			var f161201s = f161201Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == orderAllotParam.DcCode && o.CUST_CODE == orderAllotParam.CustCode && o.GUP_CODE == orderAllotParam.GupCode && o.STATUS == "2" && o.PROC_FLAG == "0").ToList();
			f161201s.ForEach(o => o.PROC_FLAG = "1");
			f161201Repo.BulkUpdate(f161201s);
			return new ExecuteResult(true);
		}
	}
}
