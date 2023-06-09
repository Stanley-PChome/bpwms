using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F055004RepositoryTest : BaseRepositoryTest
	{
		private F055004Repository _f055004Repo;
		public F055004RepositoryTest()
		{
			_f055004Repo = new F055004Repository(Schemas.CoreSchema);
		}

		[TestMethod]
		public void GetDatasById()
		{
			#region Params
			var id = 1;
			#endregion

			var result = _f055004Repo.GetDatasById(id);
		}
	}
}
