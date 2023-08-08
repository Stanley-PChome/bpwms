using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P11.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080803Service
	{
		private WmsTransaction _wmsTransaction;
		public P080803Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		
		public HomeDeliveryOrderDebitResult HomeDeliveryOrderDebit(string dcCode,string gupCode,string custCode,string pastNo)
		{
			pastNo = pastNo?.ToUpper();
			var f055001Repo = new F055001Repository(Schemas.CoreSchema,_wmsTransaction);
			var item = f055001Repo.GetHomeDeliveryOrderNumberData(dcCode, gupCode, custCode, pastNo);
			if (item == null)
				return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = "無此宅配單號" };

			if (item.STATUS == "1")
				return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = "宅單重複過刷" };

			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801 = f050801Repo.Find(x => x.DC_CODE == item.DC_CODE && x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.WMS_ORD_NO == item.WMS_ORD_NO);
			switch(f050801.STATUS)
			{
				case 9:
					return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = "訂單取消" };
				case 0:
				case 1:
					return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = "訂單未包裝" };
				case 5:
					return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = "訂單已出貨" };
				default:
					var orderSerivce = new OrderService(_wmsTransaction);
          var res = orderSerivce.PackageBoxDebit(f050801, item.PACKAGE_BOX_NO, item.PAST_NO, item.WORKSTATION_CODE, item.BOX_NUM, clientPc: Current.DeviceIp);
          if (!res.IsSuccessed)
						return new HomeDeliveryOrderDebitResult { IsSuccessed = false, Message = res.Message };

					item.TMPR_TYPE = f050801.TMPR_TYPE;
					return new HomeDeliveryOrderDebitResult { IsSuccessed = true, Message = "OK",Data = item };
			}
		}

	}
}
