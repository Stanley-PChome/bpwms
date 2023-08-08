using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.S19.Services;

namespace Wms3pl.WebServices.Schedule.S19
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class S19WcfService
	{
		[OperationContract]
		public ApiResult ExecUpdateLocVolumn(WmsScheduleParam param)
		{
			var service = new LocService(new WmsTransaction());
			return service.ExecUpdateLocVolumn(param);
		}
	}
}
