using System.Data.Services;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Process.P08.ExDataSources;
using Wms3pl.WebServices.Process.P08.Services;

namespace Wms3pl.WebServices.Process.P08
{
    public partial class P08ExDataService : DataService<P08ExDataSource>
	{
		//黑貓拖運單相關資訊
		[WebGet]
		public F055001Data GetEgsData(string dcCode, string gupCode, string custCode, string wms_ord_no, string address,string custAddress)
		{
			var srv = new P080701Service();
			var result = srv.GetEgsData(dcCode, gupCode, custCode, wms_ord_no, address, custAddress);
			return result;
		}
		
	}
}
