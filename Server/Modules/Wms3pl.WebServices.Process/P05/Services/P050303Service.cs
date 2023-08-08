using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P050303Service
	{
		private WmsTransaction _wmsTransaction;
    public P050303Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

	  public IQueryable<P050303QueryItem> GetP050303SearchData(string gupCode, string custCode, string dcCode,
		DateTime? delvDateBegin, DateTime? delvDateEnd, string ordNo, string custOrdNo, 
		string wmsOrdNo, string status, string consignNo,string itemCode)
	  {
	    var repF050801 = new F050801Repository(Schemas.CoreSchema);
	    return repF050801.GetP050303SearchData(gupCode, custCode, dcCode, delvDateBegin, delvDateEnd, ordNo, custOrdNo,
		  wmsOrdNo, status, consignNo, itemCode);
	  }


	  public IQueryable<F055002WithGridLog> GetF055002WithGridLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
	  {
		  var repF055002 = new F055002Repository(Schemas.CoreSchema);
		  return repF055002.GetF055002WithGridLog(dcCode,gupCode, custCode , wmsOrdNo);
	  }


	}
}