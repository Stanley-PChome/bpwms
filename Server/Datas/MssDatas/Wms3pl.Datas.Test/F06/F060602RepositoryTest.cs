using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060602RepositoryTest : BaseRepositoryTest
	{
		private F060602Repository _f060602Repo;
		public F060602RepositoryTest()
		{
			_f060602Repo = new F060602Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var data = _f060602Repo.Find(o => o.ID == 1);
		}
	}
}
