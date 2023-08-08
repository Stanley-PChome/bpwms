using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F51;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;

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

    [TestMethod]
    public void GetDatasByCalDate()
    {
      var data = _f510105Repo.GetDatasByCalDate("12", "10", "010001", Convert.ToDateTime("2023-04-25")).ToList();
      Trace.WriteLine(JsonConvert.SerializeObject(data));
    }

    [TestMethod]
    public void GetDatasEqualCalDate()
    {
      var data = _f510105Repo.GetDatasEqualCalDate("12", "10", "010001", "2022/07/10").ToList();
      Trace.WriteLine(JsonConvert.SerializeObject(data));
      //var data0 = _f510105Repo.GetDatasEqualCalDate0("12", "10", "010001", "2022/07/10").ToList();
      //Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(data0));
    }

    [TestMethod]
    public void GetAddDatasByStockSnapshot()
    {
      var data = _f510105Repo.GetAddDatasByStockSnapshot("12", "10", "010001", DateTime.Today).OrderBy(x => x.LOC_CODE).ThenBy(x => x.ITEM_CODE).ThenBy(x => x.MAKE_NO).ThenBy(x => x.VALID_DATE).ToList();
      Trace.WriteLine(JsonConvert.SerializeObject(data));
      //var data0 = _f510105Repo.GetAddDatasByStockSnapshot0("12", "10", "010001", DateTime.Today).OrderBy(x => x.LOC_CODE).ThenBy(x => x.ITEM_CODE).ThenBy(x => x.MAKE_NO).ThenBy(x => x.VALID_DATE).ToList();
      //Trace.WriteLine(JsonConvert.SerializeObject(data0));
      //Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(data0));
    }

  }
}
