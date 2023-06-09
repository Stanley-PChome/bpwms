using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test
{
    [TestClass]
    public class F02020102RepositoryTest : BaseRepositoryTest
    {
        public F02020102RepositoryTest()
        {
        }

        [TestMethod]
        public void Delete()
        {
            var repo = new F02020102Repository(Schemas.CoreSchema);
            //var r = repo.Delete("001", "01","030001","pchsNo", "rtNo");
            repo.Delete("", "", "", "pchsNo", "rtNo");
        }
    }
}
