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
    public class F510104RepositoryTest : BaseRepositoryTest
    {
        private F510104Repository _F510104Repo;
        public F510104RepositoryTest()
        {
            _F510104Repo = new F510104Repository(Schemas.CoreSchema);
        }
        
        [TestMethod]
        public void InsertVirtualByDate()
        {
            _F510104Repo.InsertVirtualByDate("12","10","010001",DateTime.Today.AddDays(-1));
        }

    }
}
