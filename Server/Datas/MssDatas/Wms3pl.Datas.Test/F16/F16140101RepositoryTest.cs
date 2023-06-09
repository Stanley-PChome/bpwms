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
    public partial class F16140101RepositoryTest : BaseRepositoryTest
    {
        private F16140101Repository _F16140101Repo;
        public F16140101RepositoryTest()
        {
            _F16140101Repo = new F16140101Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void DeleteByNoPostSerial()
        {
            _F16140101Repo.DeleteByNoPostSerial("", "", "", "","","");
        }
        [TestMethod]
        public void GetSerialItems()
        {
            var r=_F16140101Repo.GetSerialItems("001", "01", "010001", "R2017060200001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
