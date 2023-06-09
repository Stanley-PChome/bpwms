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
    public partial class F16140201RepositoryTest : BaseRepositoryTest
    {
        private F16140201Repository _F16140201Repo;
        public F16140201RepositoryTest()
        {
            _F16140201Repo = new F16140201Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetReturnDetailSummary()
        {
            var r = _F16140201Repo.GetReturnDetailSummary("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
