using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910402RepositoryTest : BaseRepositoryTest
    {
        private readonly F910402Repository _f910402Repository;
        public F910402RepositoryTest()
        {
            _f910402Repository = new F910402Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetF910402Reports()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700019";
            Output(new { dcCode, gupCode, custCode, quoteNo });
            var result = _f910402Repository.GetF910402Reports(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        [TestMethod]
        public void GetF910402Detail()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700019";
            Output(new { dcCode, gupCode, custCode, quoteNo });
            var result = _f910402Repository.GetF910402Detail(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        [TestMethod]
        public void DeleteNotInProcessIds()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020071300001";
            var processIds = new List<string>();            
            Output(new { dcCode, gupCode, custCode, quoteNo, processIds });
             _f910402Repository.DeleteNotInProcessIds(dcCode, gupCode, custCode, quoteNo, processIds);
            
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
