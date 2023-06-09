using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F20;
using System;
namespace Wms3pl.Datas.Test.F20
{
    [TestClass]
    public class F200102RepositoryTest : BaseRepositoryTest
    {
        private F200102Repository _F200102Repo;
        public F200102RepositoryTest()
        {
            _F200102Repo = new F200102Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF200102Datas()
        {
            var r = _F200102Repo.GetF200102Datas("001","01","010001", "J2017022000001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF200102DatasNotF050801F050301()
        {
            var r = _F200102Repo.GetF200102DatasNotF050801F050301("001"
                , "01", "010001", "J2017030600006", new System.Collections.Generic.List<string>() { "9" });
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
