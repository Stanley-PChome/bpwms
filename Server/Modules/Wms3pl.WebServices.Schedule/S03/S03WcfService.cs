using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Schedule.S03.Services;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Schedule.S03
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class S03WcfService
	{
		[OperationContract]
		public F0020 GetF0020Data(string msgNo)
		{
			var service = new S0301Service();
			return service.GetF0020(msgNo);
		}

		[OperationContract]
		public IQueryable<OrderIsProblem> GetOrderIsProblem(DateTime selectDate)
		{
			var service = new S0301Service();
			return service.GetOrderIsProblem(selectDate);
		}

		[OperationContract]
		public IQueryable<ExceedPickFinishTime> GetExceedPickFinishTimeDatas(DateTime selectDate)
		{
			var service = new S0301Service();
			return service.GetExceedPickFinishTimeDatas(selectDate);
		}

		[OperationContract]
		public ExecuteResult InsertSchMessage()
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new S0301Service(wmsTransaction);
			var result = srv.InsertSchMessage();
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

	}
}
