using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060401RepositoryTest : BaseRepositoryTest
	{
		private F060401Repository _f060401Repo;
		public F060401RepositoryTest()
		{
			_f060401Repo = new F060401Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetData()
		{
			var datas = _f060401Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
