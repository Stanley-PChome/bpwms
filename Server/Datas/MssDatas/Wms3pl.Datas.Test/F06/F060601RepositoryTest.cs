using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060601RepositoryTest : BaseRepositoryTest
	{
		private F060601Repository _f060601Repo;
		public F060601RepositoryTest()
		{
			_f060601Repo = new F060601Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			var data = _f060601Repo.Find(o => o.ID == 1);
		}
	}
}
