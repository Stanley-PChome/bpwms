using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Test.F19
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class F194712RepositoryTest: BaseRepositoryTest
    {
        private F194712Repository _f194712Repo;
        public F194712RepositoryTest()
        {
            _f194712Repo = new F194712Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetSettings()
        {
            #region Params
            var param = new AutoGenConsignParam
            {
                Channel = "00",
                AllId = "TCAT",
                ConsignType = "B",
                CustomerId = "1265635401",
                IsTest = "1",
            };
            #endregion

            _f194712Repo.GetSettings(param);
        }

        [TestMethod]
        public void Get()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var channel = "00";
            var allId = "TCAT";
            var consignType = "A";
            #endregion

            _f194712Repo.Get(dcCode, gupCode, custCode, channel,
                allId, consignType);
        }
    }
}
