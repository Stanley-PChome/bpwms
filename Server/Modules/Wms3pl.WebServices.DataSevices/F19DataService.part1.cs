using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	public partial class F19DataService : DataServiceBase<Wms3plDbContext>
	{
		[WebGet]
		public IQueryable<F1903> GetF1912s1(string gupCode, string custCode, string itemCodes, string itemName,string itemSpec, string lType)
		{
			var repo = new F1903Repository(Schemas.CoreSchema);
			return repo.GetF1912s1(gupCode, custCode, itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), itemName,itemSpec, lType);
		} 
	}
}
