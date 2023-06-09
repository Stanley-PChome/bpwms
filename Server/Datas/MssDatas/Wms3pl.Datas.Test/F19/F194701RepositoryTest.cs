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
    public class F194701RepositoryTest: BaseRepositoryTest
    {
        private F194701Repository _f194701Repo;
        public F194701RepositoryTest()
        {
            _f194701Repo = new F194701Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF194701WithF1934s()
        {
            #region Params
            var dcCode = "001";
            var allID = "711C2C";
            #endregion

            _f194701Repo.GetF194701WithF1934s(dcCode, allID);

        }

        [TestMethod]
        public void GetDelvTimeAreas()
        {
            #region Params
            var dcCode = "001";
            var allID = "HCT";
            var canFast = false;
            var delvTmpr = "A";
            var minDelvTime = "21:00";
            var maxDelvTime = "24:00";
            #endregion

            _f194701Repo.GetDelvTimeAreas(dcCode, allID, canFast, delvTmpr, minDelvTime, maxDelvTime);

        }

        [TestMethod]
        public void GetNewF194701WithF1934s()
        {
            #region Params
            var dcCode = "001";
            var allID = "HCT";
            #endregion

            _f194701Repo.GetNewF194701WithF1934s(dcCode, allID);
        }

        [TestMethod]
        public void GetF194701sByP700104()
        {
            #region Params
            var dcCode = "001";
            var allId = "HCT";
            var delvEffic = "01";
            var delvTmpr = "A";
            var zipCode = "623";
            var takeDate = Convert.ToDateTime("2020-09-26");
            var distrUse = "03";
            var intPastMax = 1;
            var intFastMax = 1;
            var takeTime = "21:00";
            #endregion

            _f194701Repo.GetF194701sByP700104(dcCode, allId, delvEffic, delvTmpr, zipCode, takeDate,
                distrUse, intPastMax,intFastMax, takeTime);
        }
    }
}
