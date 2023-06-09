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
    public partial class F161302RepositoryTest : BaseRepositoryTest
    {
        private F161302Repository _F161302Repo;
        public F161302RepositoryTest()
        {
            _F161302Repo = new F161302Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetCheckExist()
        {
            var r = _F161302Repo.GetCheckExist("001", "", "","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
