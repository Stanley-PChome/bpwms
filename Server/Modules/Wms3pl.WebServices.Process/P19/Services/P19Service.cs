using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P19Service
	{
		private WmsTransaction _wmsTransaction;
		public P19Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 範例用，以後移除
		public void Update1(string gupCode)
		{
			//var f1929Repository = new F1929Repository(Schemas.CoreSchema, _wmsTransaction);
			//var f1929 = f1929Repository.Find(a => a.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode) && a.CUST_CODE == "010001");
			//f1929.GUP_NAME += "A";
			//f1929Repository.Update(f1929);

			//f1929Repository.Add(new F1929 {
			//	GUP_CODE = "55",
			//	GUP_NAME = "Test",
			//	CUST_CODE = "010001"
			//});

			//可能有其他 Service 中已有的商業邏輯可直接引用
			//var p01Service = new P01Service(_wmsTransaction);
			//p01Service.Update1(gupCode);
		}

		public void Update2(string gupCode)
		{
			var f1929Repository = new F1929Repository(Schemas.CoreSchema, _wmsTransaction);
			f1929Repository.UpdateName(gupCode, "B");
			f1929Repository.UpdateName2(gupCode, "A");
			//f1929Repository.UpdateName("55", "B");
		}
		#endregion 範例用，以後移除
	}
}
