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
    public class F194716RepositoryTest : BaseRepositoryTest
    {
        private F194716Repository _f194716Repo;
        public F194716RepositoryTest()
        {
            _f194716Repo = new F194716Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatasByAllId()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var retailCodes = new List<string> { "12123123" };
            #endregion

            _f194716Repo.GetRetailCarPeriods(dcCode, gupCode, custCode, retailCodes);
        }

        [TestMethod]
        public void GetF194716Datas()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var carPeriod = "321313";
            var delvNo = "3213";
            var carGup = "42131";
            var retailCode = "131241";
            #endregion

            _f194716Repo.GetF194716Datas(gupCode, custCode, dcCode, carPeriod, delvNo, carGup, retailCode);
        }

        [TestMethod]
        public void UpdateHasKey()
        {
            #region Params
            var data = new F194716
            {
                DC_CODE = "001",
                GUP_CODE = "01",
                CUST_CODE = "030001",
                DELV_NO = "1",
                CAR_PERIOD = "1",
                CAR_GUP = "1",
                DRIVER_ID = "1",
                DRIVER_NAME = "1",
                EXTRA_FEE = 1,
                CRT_STAFF = "1",
                CRT_NAME = "1",
                CRT_DATE = Convert.ToDateTime("2019-09-28"),
                UPD_STAFF = "1",
                UPD_NAME = "1",
                UPD_DATE = Convert.ToDateTime("2019-09-28"),
                REGION_FEE = 1,
                OIL_FEE = 1,
                OVERTIME_FEE = 1,
                PACK_FIELD = "1"

            };
            #endregion

            _f194716Repo.UpdateHasKey(data);
        }
    }
}
