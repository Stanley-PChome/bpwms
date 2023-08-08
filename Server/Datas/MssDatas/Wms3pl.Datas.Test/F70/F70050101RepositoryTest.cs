using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F70;

namespace Wms3pl.Datas.Test.F70
{
	[TestClass]
	public class F70050101RepositoryTest : BaseRepositoryTest
	{
		private F70050101Repository _F70050101Repo;
		public F70050101RepositoryTest()
		{
			_F70050101Repo = new F70050101Repository(Schemas.CoreSchema);
		}
	}
}