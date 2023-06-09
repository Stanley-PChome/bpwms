
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.Process.P05.Services
{
	public partial class P05010101Service
	{
		private WmsTransaction _wmsTransaction;
		public P05010101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public bool CheckF05010101SelectAll(List<string> ordNos)
		{
			//有資料代表 :有未選到的小單訂單編號
			var f05010101Rep = new F05010101Repository(Schemas.CoreSchema);			
			var result = f05010101Rep.CheckF05010101SelectAll(ordNos);

			if (result != null && result.Count() > 0)
			{
				return false;
			}
			return true;

		}
	}
}
