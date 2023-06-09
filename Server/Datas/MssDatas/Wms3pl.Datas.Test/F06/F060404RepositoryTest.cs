using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060404RepositoryTest : BaseRepositoryTest
	{
		private F060404Repository _f060404Repo;
		public F060404RepositoryTest()
		{
			_f060404Repo = new F060404Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var datas = _f060404Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
