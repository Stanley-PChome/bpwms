using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F51;

namespace Wms3pl.Datas.Test.F51
{
	[TestClass]
	public class F510105RepositoryTest : BaseRepositoryTest
	{
		private F510105Repository _f510105Repo;
		public F510105RepositoryTest()
		{
			_f510105Repo = new F510105Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetData()
		{
			var data = _f510105Repo.Find(o => o.ID == 1);
		}

	}
}
