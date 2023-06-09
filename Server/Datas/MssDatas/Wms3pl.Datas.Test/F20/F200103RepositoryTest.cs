using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F20;
using System;
namespace Wms3pl.Datas.Test.F20
{
    [TestClass]
    public class F200103RepositoryTest123 : BaseRepositoryTest
    {
        private F200103Repository _F200103Repo;
        public F200103RepositoryTest123()
        {
            _F200103Repo = new F200103Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF200103Datas()
        {
            var r = _F200103Repo.GetF200103Datas("001","01","010001", "J2017030400001");
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
