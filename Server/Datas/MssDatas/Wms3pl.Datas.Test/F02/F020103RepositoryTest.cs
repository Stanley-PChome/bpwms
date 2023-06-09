using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F02;
using System;

namespace Wms3pl.Datas.Test.F02
{
    [TestClass]
    public class F020103RepositoryTest : BaseRepositoryTest
    {
        private F020103Repository _F020103Repo;
        public F020103RepositoryTest()
        {
            _F020103Repo = new F020103Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Find()
        {
            //SELECT*
            //  FROM F020103 Purchase
            // WHERE ARRIVE_DATE = TO_DATE('2017/5/31', 'yyyy/MM/dd')
            //       AND SERIAL_NO = '1'
            //       AND PURCHASE_NO = 'A2017053100002'
            //       AND DC_CODE = '001'
            //       AND GUP_CODE = '01'
            //       AND CUST_CODE = '010002'
            //string date, int serialNo, string purchaseNo, string dcCode, string gupCode, string custCode
            var repo = new F020103Repository(Schemas.CoreSchema);
            var r = repo.Find("2017/5/31", 1, "A2017053100002", "001", "01", "010002");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetF020103Detail()
        {
            var repo = new F020103Repository(Schemas.CoreSchema);
            var r = repo.GetF020103Detail(DateTime.Parse("2017/5/31"), "1944", "001", "venderCodeA2017053100002", "030001", "01");
            Trace.Write(JsonSerializer.Serialize(r));
        }
        [TestMethod]
        public void GetNewId()
        {
            var repo = new F020103Repository(Schemas.CoreSchema);
            var r = repo.GetNewId("001","01", "030002", "pchNo", DateTime.Now);
            Trace.Write(JsonSerializer.Serialize(r));
        }
    }
}
