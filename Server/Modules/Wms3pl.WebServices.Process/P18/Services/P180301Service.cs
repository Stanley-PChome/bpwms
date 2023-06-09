using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P18.Services
{
	public partial class P180301Service
	{
		private WmsTransaction _wmsTransaction;
		public P180301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 建立異動單更新庫存作業
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="datas"></param>
		/// <returns></returns>
		public ExecuteResult InsertF200101Data(string dcCode, string gupCode, string custCode, List<P180301ImportData> datas)
		{
			var sharedService = new SharedService(_wmsTransaction);
			return sharedService.InsertF200101Data(dcCode, gupCode, custCode, datas);
		}
	}
}
