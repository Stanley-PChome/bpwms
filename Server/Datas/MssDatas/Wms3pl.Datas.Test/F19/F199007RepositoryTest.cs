using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F199007RepositoryTest: BaseRepositoryTest
    {
        private F199007Repository _f199007Repo;
        public F199007RepositoryTest()
        {
            _f199007Repo = new F199007Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetProjectValuation()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var creDateS = Convert.ToDateTime("2020-07-07");
            var creDateE = Convert.ToDateTime("2020-07-08");
            var accProjectNo = "ZD2020070700001";
            var enableD = Convert.ToDateTime("2020-07-01");
            var disableD = Convert.ToDateTime("2020-07-10");
            var quoteNo = "TTS200707011";
            var status = "2";
            var accProjectName = "7/1-8專案報價";

            #endregion

            _f199007Repo.GetProjectValuation(dcCode, gupCode, custCode, creDateS, creDateE,
            accProjectNo, enableD, disableD, quoteNo, status, accProjectName);
        }

        [TestMethod]
        public void GetQuoteDatas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var quotes = new List<string> { "ZD2020070700001" };

            #endregion

            _f199007Repo.GetQuoteDatas(dcCode, gupCode, custCode,  quotes);
        }

        [TestMethod]
        public void UpdateF199007Status()
        {
            #region Params
            var cntDate = Convert.ToDateTime("2020-10-05");
            var gupCode = "01";
            var custCode = "010001";

            #endregion

            _f199007Repo.UpdateF199007Status(cntDate, gupCode, custCode);
        }
    }
}
