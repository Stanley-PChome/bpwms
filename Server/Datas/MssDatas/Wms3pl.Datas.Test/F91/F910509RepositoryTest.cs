using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
	[TestClass]
	public class F910509RepositoryTest : BaseRepositoryTest
	{
		private readonly F910509Repository _f910509Repository;
		public F910509RepositoryTest()
		{
			_f910509Repository = new F910509Repository(Schemas.CoreSchema);
		}
	}
}
