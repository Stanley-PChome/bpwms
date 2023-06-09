using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F1915RepositoryTest: BaseRepositoryTest
    {
        private F1915Repository _f1915Repo;
        public F1915RepositoryTest()
        {
            _f1915Repo = new F1915Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1915SearchData()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var aCode = "Z001";
            var clsName = "耗";
            #endregion

            var result = _f1915Repo.GetF1915SearchData(gupCode, custCode, aCode, clsName);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN";
            var custCodes = new List<string> { "030001" };
            #endregion

            var result = _f1915Repo.GetDatas(gupCode, aCode, custCodes);
        }

        [TestMethod]
        public void DeleteByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN";
            var custCodes = new List<string> { "030099" };
            #endregion

            _f1915Repo.DeleteByCustCodes(gupCode, aCode, custCodes);
        }
    }
}
