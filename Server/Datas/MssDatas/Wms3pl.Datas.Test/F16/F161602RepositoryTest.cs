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
    public partial class F161602RepositoryTest : BaseRepositoryTest
    {
        private F161602Repository _F161602Repo;
        public F161602RepositoryTest()
        {
            _F161602Repo = new F161602Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF161602Exs()
        {
            var r =_F161602Repo.GetF161602Exs("001", "01", "020003","");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void Delete()
        {
            //給予錯誤資料避免珍刪除
            _F161602Repo.Delete("A001", "A01","", "");
        }
    }
}
