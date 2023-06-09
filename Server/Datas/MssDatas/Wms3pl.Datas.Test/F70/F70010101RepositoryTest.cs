using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F70;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F70
{
    [TestClass]
    public class F70010101RepositoryTest : BaseRepositoryTest
    {
        private F70010101Repository _F70010101Repo;
        public F70010101RepositoryTest()
        {
            _F70010101Repo = new F70010101Repository(Schemas.CoreSchema);
        }
        //[TestMethod]
        //public void InsertStockByDate()
        //{
        //    _F70010101Repo.InsertStockByDate(DateTime.Today);
        //}

        //[TestMethod]
        //public void GetLocSettleQty()
        //{
        //    //2018-05-08 00:00:00
        //    //2018-05-09 00:00:00
        //    //2018-05-10 00:00:00
        //    var r = _F70010101Repo.GetLocSettleQty("001", "01", "010001", DateTime.Parse("2018-05-08"));
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}

    }
}
