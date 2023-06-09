using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
	[TestClass]
	public class F060403RepositoryTest : BaseRepositoryTest
	{
		private F060403Repository _f060403Repo;
		public F060403RepositoryTest()
		{
			_f060403Repo = new F060403Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatas()
		{
			_f060403Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == "12").ToList();
		}
	}
}
