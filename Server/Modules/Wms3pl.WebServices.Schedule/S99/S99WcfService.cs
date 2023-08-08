using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Schedule.S99.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.S99
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class S99WcfService
	{

		#region 排程-訊息發送

		[OperationContract]
		public IQueryable<WMSMessage> GetWmsMessages()
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new S9901Service(wmsTransaction);
			var result = srv.GetWmsMessages();
			return result;
		}

		[OperationContract]
		public void SetMessageStatus(List<WMSMessage> wmsMessages)
		{

			var wmsTransaction = new WmsTransaction();

			var srv = new S9901Service(wmsTransaction);
			srv.SetMessageStatus(wmsMessages);
			wmsTransaction.Complete();
		}
		#endregion 排程-訊息發送
	}
}