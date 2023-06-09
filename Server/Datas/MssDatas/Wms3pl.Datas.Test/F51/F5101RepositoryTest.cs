using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F51;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F51
{
    [TestClass]
    public class F5101RepositoryTest : BaseRepositoryTest
    {
        private F5101Repository _F5101Repo;
        public F5101RepositoryTest()
        {
            _F5101Repo = new F5101Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void GetLastLocQty()
        {
            var r = _F5101Repo.GetLastLocQty("001","01","010001",DateTime.Parse("2018/05/08"));
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void DeleteByDate()
        {
            _F5101Repo.DeleteByDate(DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
