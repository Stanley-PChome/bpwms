using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F05.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public class P050901Service
	{
		private WmsTransaction _wmsTransaction;
		public P050901Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		//揀貨列表
		public IQueryable<F051206PickList> GetF051206PickLists(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string editType)
		{
			var Repo = new F051206Repository(Schemas.CoreSchema);
			return Repo.GetF051206PickLists(dcCode, gupCode, custCode, delvDate, pickTime, editType);
		}
		//調撥列表
		public IQueryable<F051206AllocationList> GetF051206AllocationLists(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string editType)
		{
			var Repo = new F051206Repository(Schemas.CoreSchema);
			return Repo.GetF051206AllocationLists(dcCode, gupCode, custCode, delvDate, pickTime, editType);
		}
		//缺貨明細
		public IQueryable<F051206LackList> GetF051206LackLists(string PICK_ORD_NO, string WMS_ORD_NO, string editType)
		{
			var Repo = new F051206Repository(Schemas.CoreSchema);
			return Repo.GetF051206LackLists(PICK_ORD_NO, WMS_ORD_NO, editType);
		}
	}
}