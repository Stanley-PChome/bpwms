using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class SignalRWcfService
	{
		[OperationContract]
		public ExecuteResult SendMessage(string messageContent)
		{
			var s = Schemas.CoreSchema;
			var server = new LoginService();
			if (s == null) //選擇全部
				server.SendMessageByAllUser(messageContent);
			else
				server.SendMessageByDBUser(messageContent);
			return new ExecuteResult(true);
		}
	}
}
