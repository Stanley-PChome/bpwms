using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	public class HomeDeliveryResultsReq
	{
		public string DcCode { get; set; }
		public string CustCode { get; set; }
		public List<HomeDeliveryNo> Data { get; set; }
	}

	public class HomeDeliveryNo
	{
		public string WmsNo { get; set; }
		public string TransportCode { get; set; }
		public string ShipmentTime { get; set; }
	}
	
}
