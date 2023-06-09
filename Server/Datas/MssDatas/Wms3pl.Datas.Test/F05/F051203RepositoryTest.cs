using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F051203RepositoryTest : BaseRepositoryTest
	{
		private F051203Repository _f051203Repo;
		public F051203RepositoryTest()
		{
			_f051203Repo = new F051203Repository(Schemas.CoreSchema);
		}
		[TestMethod]
		public void GetBatchPickDetail()
		{
			#region Params
			var dcNo = "001";
			var gupCode = "01";
			var custNo = "030001";
			var pickOrdNo = "P2018072700006";
			#endregion

			_f051203Repo.GetBatchPickDetail(dcNo, gupCode, custNo, pickOrdNo);
		}
	}
}
