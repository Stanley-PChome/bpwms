using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Process.P16.ServiceEntites
{

	[DataContract]
	[Serializable]
	public class ItemRtnQty
	{
		[DataMember]
		public string ItemCode { get; set; }
		[DataMember]
		public long RtnQty { get; set; }
		[DataMember]
		public string MakeNo { get; set; }
	}

	
}
