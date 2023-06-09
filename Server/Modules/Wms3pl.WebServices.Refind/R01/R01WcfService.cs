
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F70;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Refind.R01.Services;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.Schedule.ReplenishStock.Services;

namespace Wms3pl.WebServices.Refind.R01
{	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class R01WcfService
	{

		[OperationContract]
		public ApiResult GetReplensihStock(BatchTransApiOrdersReq req)
		{
			var srv = new ReplenishStockService();
            return srv.ProcessApiDatas_Order(req);
		}


		/// <param name="baseDay1"> : 商品已在黃金揀貨區，過去X天以來 累計出貨數量低於30pcs。</param>
		/// <param name="baseDay2"> : 商品已在黃金揀貨區，過去X天以來 累計出貨次數小於等於三次。</param>
		[OperationContract]
		public IQueryable<SchF700501Data> GetSchOrderData(int baseDay1 = 7, int baseDay2 = 14)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new R010101Service(wmsTransaction);
			var result = srv.GetSchOrderData(baseDay1, baseDay2);
			wmsTransaction.Complete();
			return result;
		}


		/// <param name="schRunDay">(排程為一週執行一次 , 故設 7)</param>
		/// <param name="baseDay">過去 x 個月以來，平均出貨間隔天數<=3天</param>
		/// <param name="avgOrders">間隔天數<=3天。 </param>
		[OperationContract]
		public IQueryable<SchF700501Data> GetSchOrderNormalData(int schRunDay = 7, int baseDay = 90, int avgOrders = 3)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new R010101Service(wmsTransaction);
			var result = srv.GetSchOrderNormalData(schRunDay, baseDay, avgOrders);
			wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public IQueryable<SchF700501Data> GetSchOrderAllData(int baseDay = 90)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new R010101Service(wmsTransaction);
			var result = srv.GetSchOrderAllData(baseDay);
			wmsTransaction.Complete();
			return result;
		}



		[OperationContract]
		public ExecuteResult InsertF700501(F700501 f700501)
		{
			Current.DefaultStaff = "Refine";
			Current.DefaultStaffName = "Refine";
			var wmsTransaction = new WmsTransaction();
			var srv = new R010101Service(wmsTransaction);
			var result = srv.InsertF700501(f700501);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}


	}
}
