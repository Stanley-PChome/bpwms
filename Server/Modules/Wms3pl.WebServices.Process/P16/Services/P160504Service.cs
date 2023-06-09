using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Services;
using System.Data.Objects;
namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160504Service
	{
		private WmsTransaction _wmsTransaction;
		private F160504Repository _f160504Repo = new F160504Repository(Schemas.CoreSchema);
		public P160504Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F160502Data> Get160504SerialData(string dcCode, string gupCode, string custCode, string destoryNo)
		{
			var repF160504 = new F160504Repository(Schemas.CoreSchema);
			return repF160504.Get160504SerialData(dcCode,gupCode,custCode,destoryNo);

		}

		public bool InsertF160504Detail(List<F160502Data> serialData, string destoryNo)
		{
			var repF160504 = new F160504Repository(Schemas.CoreSchema,_wmsTransaction);
			Int16 recordCtn = 0;
			foreach (var items in serialData)
			{
				recordCtn += 1;
				F160504 f160504 = new F160504
				{
					DESTROY_NO = destoryNo,
					SERIAL_SEQ = recordCtn,
					ITEM_CODE = items.ITEM_CODE,
					SERIAL_NO = items.ITEM_SERIALNO,
					DC_CODE = items.DC_CODE,
					GUP_CODE = items.GUP_CODE,
					CUST_CODE = items.CUST_CODE
				};
				repF160504.Add(f160504);
			}
			return true;
		}
	}


}
