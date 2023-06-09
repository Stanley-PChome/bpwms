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
    public class F194707RepositoryTest: BaseRepositoryTest
    {
        private F194707Repository _f194707Repo;
        public F194707RepositoryTest()
        {
            _f194707Repo = new F194707Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetP710507SearchData()
        {
            #region Params
            var dcCode = "001";
            var allId = "TCAT";
            var accKind = "D";
            var inTax = "0";
            var logiType = "01";
            var custType = "0";
            var status = "0";
            #endregion

            _f194707Repo.GetP710507SearchData(dcCode, allId, accKind,
          inTax, logiType, custType, status);

        }

        [TestMethod]
        public void GetQuoteDatas()
        {
            #region Params
            #endregion

            _f194707Repo.GetQuoteDatas();

        }

        [TestMethod]
        public void GetF194707WithF19470801s()
        {
            #region Params

            var dcCode = "001";
            var allIds = new string[] { "TCAT", "711" };
            var zipCodes = new string[] { "743" };

            #endregion

            _f194707Repo.GetF194707WithF19470801s(dcCode,  allIds, zipCodes);

        }
    }
}
