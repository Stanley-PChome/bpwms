using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
  [TestClass]
  public class F0532RepositoryTest : BaseRepositoryTest
  {
    private F0532Repository _f0532Repo;
    public F0532RepositoryTest()
    {
      _f0532Repo = new F0532Repository(Schemas.CoreSchema);
    }

    [TestMethod]
    public void GetF0532Ex()
    {
      #region Params
      string dcCode = "12";
      DateTime startDate = Convert.ToDateTime("2023-04-20");
      DateTime endDate = Convert.ToDateTime("2023-04-20");
      string status = "";
      string outContainerCode = "";
      #endregion

      //Trace.WriteLine(JsonConvert.SerializeObject(_f0532Repo.GetF0532Ex(dcCode, startDate, endDate, status, outContainerCode)));
    }

  }
}
