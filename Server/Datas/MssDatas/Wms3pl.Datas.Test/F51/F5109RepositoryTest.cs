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
    public class F5109RepositoryTest : BaseRepositoryTest
    {
        private F5109Repository _F5109Repo;
        public F5109RepositoryTest()
        {
            _F5109Repo = new F5109Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteByDate()
        {
            _F5109Repo.DeleteByDate(DateTime.Today);
            //Trace.Write(JsonSerializer.Serialize(r));
        }

        //[TestMethod]
        //public void GetSettleMonFee()
        //{
        //    var r = _F5109Repo.GetSettleMonFee(DateTime.Today,"");
        //    Trace.Write(JsonSerializer.Serialize(r));
        //}
    }
}
