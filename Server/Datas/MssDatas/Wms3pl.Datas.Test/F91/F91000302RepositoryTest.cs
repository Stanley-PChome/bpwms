using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F91000302RepositoryTest : BaseRepositoryTest
    {
        private readonly F91000302Repository _f91000302Repository;
        public F91000302RepositoryTest()
        {
            _f91000302Repository = new F91000302Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetAccUnitList()
        {
            string itemTypeId = "001";
            Output(new { itemTypeId });
            var result = _f91000302Repository.GetAccUnitList(itemTypeId);
            Output(result);
        }

        [TestMethod]
        public void GetF91000302Data()
        {
            string itemTypeId = "001";
            string accUnit = "";
            string accUnitName = "";
            Output(new { itemTypeId, accUnit, accUnitName });
            var result = _f91000302Repository.GetF91000302Data(itemTypeId, accUnit, accUnitName);
            Output(result);
        }

        [TestMethod]
        public void GetItemUnit()
        {
            #region Params
            var itemUnit = "15";
            #endregion

            _f91000302Repository.GetItemUnit(itemUnit);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
