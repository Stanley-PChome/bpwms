
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P70.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P70WcfService
	{

		#region DeleteF700101

		[OperationContract]
		public ExecuteResult DeleteF700101ByDistrCarNo(string distrCarNo, string dcCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P700104Service(wmsTransaction);
			var result = srv.DeleteF700101ByDistrCarNo(distrCarNo, dcCode);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		#endregion

		[OperationContract]
		public decimal GetTableSeqId(string tableSeqId)
		{
			var result = SharedService.GetTableSeqId(tableSeqId);
			return result;
		}
	}
}

