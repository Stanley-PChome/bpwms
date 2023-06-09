using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F19471201RepositoryTest: BaseRepositoryTest
    {
        private F19471201Repository _f19471201Repo;
        public F19471201RepositoryTest()
        {
            _f19471201Repo = new F19471201Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allId = "TCAT";
            var channel = "00";
            var consignType = "B";
            var isUsed = "0";
            var exceptConsignNo = new List<string> { "620003368755", "620003368766" };
            #endregion

            _f19471201Repo.GetData(dcCode, gupCode, custCode, allId, channel, consignType,
                isUsed, exceptConsignNo);

        }

        [TestMethod]
        public void UpDataForIsused()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var channel = "00";
            var allId = "TCAT";
            var consignType = "A";
            var exceptConsignNo = new List<string> { "905073307100" };
            #endregion

            _f19471201Repo.UpDataForIsused(dcCode, gupCode, custCode, channel, allId, consignType, exceptConsignNo);

        }
    }
}
