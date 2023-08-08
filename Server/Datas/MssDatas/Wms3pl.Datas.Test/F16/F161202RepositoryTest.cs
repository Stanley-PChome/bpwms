using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F16;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F16
{
	[TestClass]
	public partial class F161202RepositoryTest : BaseRepositoryTest
	{
		private F161202Repository _F161202Repo;
		public F161202RepositoryTest()
		{
			_F161202Repo = new F161202Repository(Schemas.CoreSchema);
		}
		[TestMethod]
		public void GetF161202ReturnDetails()
		{
			var r = _F161202Repo.GetF161202ReturnDetails("001", "01", "010001", "R2017060200001");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetReturnItems()
		{
			var r = _F161202Repo.GetReturnItems("001", "01", "010001", "", "", "R2017060200001", "R2017060200001", "", "");
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetDatasByDc()
		{
			var r = _F161202Repo.GetDatasByDc("001", "R8", DateTime.Parse("2017-06-02"));
			Trace.Write(JsonSerializer.Serialize(r));
		}
		[TestMethod]
		public void GetAllotReturnData()
		{
			var r = _F161202Repo.GetAllotReturnData("001", "01", "010001");
			Trace.Write(JsonSerializer.Serialize(r));
		}
	}
}
