using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
  [TestClass]
  public class F060206RepositoryTest : BaseRepositoryTest
  {
    private F060206Repository _f060206Repo;
    public F060206RepositoryTest()
    {
      _f060206Repo = new F060206Repository(Schemas.CoreSchema);
    }

  }
}
