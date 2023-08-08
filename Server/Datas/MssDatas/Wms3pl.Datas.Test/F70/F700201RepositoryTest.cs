using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F70;

namespace Wms3pl.Datas.Test.F70
{
	[TestClass]
	public class F700201RepositoryTest : BaseRepositoryTest
	{
		private F700201Repository _F700201Repo;
		public F700201RepositoryTest()
		{
			_F700201Repo = new F700201Repository(Schemas.CoreSchema);
		}
	}
}
