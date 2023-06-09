using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060402RepositoryTest : BaseRepositoryTest
	{
		private F060402Repository _f060402Repo;
		public F060402RepositoryTest()
		{
			_f060402Repo = new F060402Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var datas = _f060402Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
