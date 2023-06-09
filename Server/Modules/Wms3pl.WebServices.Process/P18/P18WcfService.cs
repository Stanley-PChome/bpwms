using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P18.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P18WcfService
	{
		[OperationContract]
		public IQueryable<F1913> GetF1913Data(F1913 f1913s)
		{
			var srv = new P180101Service();
			var data = srv.GetF1913Data(f1913s);
			return data.AsQueryable();
		}
		[OperationContract]
        public ExecuteResult UpdateValidDateAndBatchNo(F1913 f1913s, DateTime newValidDate, string newMakeNo, Int64 NewQTY)
        {
            var wmsTransaction = new WmsTransaction();
			var srv = new P180101Service(wmsTransaction);
			var result = srv.UpdateValidDateAndBatchNo(f1913s, newValidDate, newMakeNo, NewQTY);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}

		[OperationContract]
		public ExecuteResult InsertF200101Data(string dcCode, string gupCode, string custCode, List<P180301ImportData> data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P180301Service(wmsTransaction);
			var result = srv.InsertF200101Data(dcCode,gupCode,custCode,data);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}

		#region 庫存異動處理
		[OperationContract]
		public ExecuteResult UpdateF191302(StockAbnormalData data)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P180201Service(wmsTransation);
			var result = service.UpdateF191302(data);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CreateInventory(string dcCode, List<StockAbnormalData> datas)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P180201Service(wmsTransation);
			return service.CreateInventory(dcCode, datas);
		}
		#endregion
	}
}
