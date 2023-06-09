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
    public class F510102RepositoryTest : BaseRepositoryTest
    {
        private F510102Repository _F510102Repo;
        public F510102RepositoryTest()
        {
            _F510102Repo = new F510102Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void InsertStockByDate()
        {
             _F510102Repo.InsertStockByDate(DateTime.Today);
        }
        [TestMethod]
        public void GetLocSettleQty()
        {
            //2018-05-08 00:00:00
            //2018-05-09 00:00:00
            //2018-05-10 00:00:00
            var r = _F510102Repo.GetLocSettleQty("001","01","010001",DateTime.Parse("2018-05-08"));
            Trace.Write(JsonSerializer.Serialize(r));
        }

    }
}
