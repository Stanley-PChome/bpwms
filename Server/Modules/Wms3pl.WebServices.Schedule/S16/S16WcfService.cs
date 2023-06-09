using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.S16.Services;

namespace Wms3pl.WebServices.Schedule.S16
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class S16WcfService
	{
		[OperationContract]
		public ExecuteResult ExecReturnAllot(OrderAllotParam orderAllotParam)
		{
			var wmsTransaction = new WmsTransaction();
			var returnAllotService = new ReturnAllotService(wmsTransaction);
			var result = returnAllotService.ExecReturnAllot(orderAllotParam);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
	}
}
