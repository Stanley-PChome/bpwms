using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F250101RepositoryTest : BaseRepositoryTest
    {
        private readonly F250101Repository _f250101Repository;
        public F250101RepositoryTest()
        {
            _f250101Repository = new F250101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetP2502QueryDatas()
        {
            string gupCode = "01";
            string custCode = "030001";
            var itemCode = new List<string>();
            var serialNo = new List<string>();
            string batchNo = "";
            string cellNum = "";
            string poNo = "";
            var wmsNo = new List<string>();
            string status = "";
            string retailCode = "";
            Int16? combinNo = null;
            string crtName = "";
            DateTime? updSDate = null;
            DateTime? updEDate = null;
            string boxSerial = "";
            string itemType = "";
            Console.WriteLine($@"{JsonSerializer.Serialize(new { gupCode, custCode,
                itemCode, serialNo, batchNo, cellNum, poNo, wmsNo
            , status, retailCode, combinNo, crtName, updSDate
            , updEDate, boxSerial, itemType })}");
            var result = _f250101Repository.GetP2502QueryDatas(gupCode, custCode,
              itemCode.ToArray(), serialNo.ToArray(), batchNo, cellNum, poNo, wmsNo.ToArray()
             , status, retailCode, combinNo, crtName, updSDate
             , updEDate, boxSerial, itemType);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
