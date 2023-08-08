using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
	[TestClass]
	public class F140106RepositoryTest : BaseRepositoryTest
	{
		private readonly F140106Repository _f140106Repository;
		public F140106RepositoryTest()
		{
			_f140106Repository = new F140106Repository(Schemas.CoreSchema);
		}
	}
}
