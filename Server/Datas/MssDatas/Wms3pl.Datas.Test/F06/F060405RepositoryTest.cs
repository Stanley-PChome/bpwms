using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060405RepositoryTest : BaseRepositoryTest
	{
		private F060405Repository _f060405Repo;
		public F060405RepositoryTest()
		{
			_f060405Repo = new F060405Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var datas =_f060405Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
