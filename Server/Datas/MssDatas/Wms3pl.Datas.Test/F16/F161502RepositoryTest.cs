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
    public partial class F161502RepositoryTest : BaseRepositoryTest
    {
        private F161502Repository _F161502Repo;
        public F161502RepositoryTest()
        {
            _F161502Repo = new F161502Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void DeleteGatherDataDetails()
        {
            _F161502Repo.DeleteGatherDataDetails("001", new List<string>() { "", "" });
            //Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
