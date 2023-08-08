using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910403RepositoryTest : BaseRepositoryTest
    {
        private readonly F910403Repository _f910403Repository;
        public F910403RepositoryTest()
        {
            _f910403Repository = new F910403Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF910403DataByQuoteNo()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700020";
            Output(new { dcCode, gupCode , custCode, quoteNo });
            var result = _f910403Repository.GetF910403DataByQuoteNo(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        [TestMethod]
        public void GetF910403Reports()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700020";
            Output(new { dcCode, gupCode, custCode, quoteNo });
            var result = _f910403Repository.GetF910403Reports(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        [TestMethod]
        public void GetF910403Detail()
        {
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700020";
            Output(new { dcCode, gupCode, custCode, quoteNo });
            var result = _f910403Repository.GetF910403Detail(dcCode, gupCode, custCode, quoteNo);
            Output(result);
        }

        [TestMethod]
        public void DeleteNotItemCodeList()
        {
            
            string dcCode = "001";
            string gupCode = "01";
            string custCode = "010001";
            string quoteNo = "Q2020070700020";
            var itemCodeList = new List<string>();
            Output(new { dcCode, gupCode, custCode, quoteNo, itemCodeList });
            _f910403Repository.DeleteNotItemCodeList(dcCode, gupCode, custCode, quoteNo, itemCodeList);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
