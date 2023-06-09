using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
	public class ShipDebitService
	{
		private int _errorCnt;
		private List<string> _msgList;
		public ShipDebitService()
		{

		}

		public ApiResult ExecShipOrderDebit()
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var canDebitShipOrders = f050801Repo.GetCanDebitShipOrders().ToList();
			if (!canDebitShipOrders.Any())
				return new ApiResult { IsSuccessed = true, MsgCode = "200", MsgContent = "無需扣帳出貨單資料，不需處理" };

			var res = ApiLogHelper.CreateApiLogInfo("0", "0", "0", "ShipDebit", canDebitShipOrders, () =>
				 {
					 _errorCnt = 0;
					 _msgList = new List<string>();
					 foreach(var canDebitShipOrder in canDebitShipOrders)
					 {
						 ShipOrderDebit(canDebitShipOrder);
					 }
					 
					 return new ApiResult
					 {
						 IsSuccessed = _errorCnt == 0,
						 MsgCode = _errorCnt == 0 ? "200" : "99999",
						 MsgContent = string.Join(Environment.NewLine, _msgList)
					 };
				 }, true);
			return res;
		}

		public void ShipOrderDebit(CanDebitShipOrder canDebitShipOrder)
		{
			try
			{
				var wmsTransaton = new WmsTransaction();
				var orderService = new OrderService(wmsTransaton);
				var result = orderService.OneShipOrderDebit(canDebitShipOrder.DC_CODE, canDebitShipOrder.GUP_CODE, canDebitShipOrder.CUST_CODE, canDebitShipOrder.WMS_ORD_NO);
				if (!result.IsSuccessed)
				{
					_errorCnt++;
					_msgList.Add(string.Format("出貨單:{0} 出貨扣帳失敗，原因:{1}", canDebitShipOrder.WMS_ORD_NO, result.Message));
				}
				else
				{
					wmsTransaton.Complete();
					_msgList.Add(string.Format("出貨單:{0} 出貨扣帳成功", canDebitShipOrder.WMS_ORD_NO));
				}
			}
			catch (Exception ex)
			{
				_errorCnt++;
				_msgList.Add(string.Format("出貨單:{0} 出貨扣帳失敗，原因:{1}", canDebitShipOrder.WMS_ORD_NO,ex.Message));
			}
		}
	}
}
