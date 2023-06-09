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
    public class F1919RepositoryTest : BaseRepositoryTest
    {
        private F1919Repository _f1919Repo;
        public F1919RepositoryTest()
        {
            _f1919Repo = new F1919Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1919Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var warehourseId = "G07";
            var areaCode = "A13";
            #endregion

            var result = _f1919Repo.GetF1919Datas(dcCode, gupCode, custCode, warehourseId, areaCode);
        }

        [TestMethod]
        public void P710101Delete()
        {
            #region Params
            var dcCode = "001";
            var warehouseId = "G07";
            var areaCode = new List<string>{ "A14","A15" };
            #endregion

           _f1919Repo.P710101Delete( dcCode, warehouseId, areaCode);
        }

        [TestMethod]
        public void GetDatasByCanToShip()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f1919Repo.GetDatasByCanToShip(dcCode, gupCode, custCode);
        }
    }
}
