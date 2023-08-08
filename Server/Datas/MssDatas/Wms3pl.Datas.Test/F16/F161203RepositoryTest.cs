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
    public partial class F161203RepositoryTest : BaseRepositoryTest
    {
        private F161203Repository _F161203Repo;
        public F161203RepositoryTest()
        {
            _F161203Repo = new F161203Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetALLF161203()
        {
            var r = _F161203Repo.GetALLF161203();
            Trace.Write(JsonSerializer.Serialize(r));
        }

    }
}
