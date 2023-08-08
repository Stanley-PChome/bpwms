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
	public partial class P160502Service
	{
		private WmsTransaction _wmsTransaction;
		private F160502Repository _f160502Repo = new F160502Repository(Schemas.CoreSchema);
		public P160502Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F160502Data> Get160502DetailData(string dcCode,string gupCode,string custCode,string destoryNo)
		{
			var repF160502 = new F160502Repository(Schemas.CoreSchema);
			return repF160502.Get160502DetailData(dcCode,gupCode,custCode, destoryNo);			

		}

		public bool InsertF160502Detail(List<F160502Data> detailData, string destoryNo)
		{
			var repF160502 = new F160502Repository(Schemas.CoreSchema , _wmsTransaction);
			Int16 recordCtn = 0;
			foreach (var items in detailData)
			{
				recordCtn += 1;
				F160502 f160502 = new F160502
				{
					DESTROY_NO = destoryNo,
					DESTROY_SEQ = recordCtn,
					ITEM_CODE = items.ITEM_CODE,
					DESTROY_QTY = items.DESTROY_QTY,
					DC_CODE = items.DC_CODE,
					GUP_CODE = items.GUP_CODE,
					CUST_CODE = items.CUST_CODE
				};
				repF160502.Add(f160502);
			}
			return true;
		}

	}
}
