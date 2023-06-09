using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060406RepositoryTest : BaseRepositoryTest
	{
		private F060406Repository _f060406Repo;
		public F060406RepositoryTest()
		{
			_f060406Repo = new F060406Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var datas = _f060406Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
