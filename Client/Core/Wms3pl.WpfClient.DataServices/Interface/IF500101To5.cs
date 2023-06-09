using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.DataServices.Interface
{
	public interface IF500101To5
	{
		string DC_CODE { get; set; }

		string GUP_CODE { get; set; }

		string CUST_CODE { get; set; }

		string STATUS { get; set; }

		DateTime ENABLE_DATE { get; set; }

		DateTime DISABLE_DATE { get; set; }

		string QUOTE_NO { get; set; }

		string ACC_ITEM_NAME { get; set; }

		string ACC_UNIT { get; set; }
		
	}
}
