using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.ServiceEntites
{
	public class KtjPostData
	{
		public string ID { get; set; }
		public string Address { get; set; }
	}
	public class KtjStation
	{
		public string ID { get; set; }
		public string Zip { get; set; }
		public string SID { get; set; }
		public string SNA { get; set; }
		public string SNO { get; set; }
	}
}
