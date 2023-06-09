using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
	[TestClass]
	public class F05000301RepositoryTest : BaseRepositoryTest
	{
		private F05000301Repository _f05000301Repo;
		public F05000301RepositoryTest()
		{
			_f05000301Repo = new F05000301Repository(Schemas.CoreSchema);
		}
	}
}
