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
    public class F1917RepositoryTest: BaseRepositoryTest
    {
        private F1917Repository _f1917Repo;
        public F1917RepositoryTest()
        {
            _f1917Repo = new F1917Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1917SearchData()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var cCode = "0501";
            var clsName = "材";
            #endregion

            var result = _f1917Repo.GetF1917SearchData(gupCode, custCode, cCode, clsName);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN";
            var bCode = "HD";
            var cCode = "F001";
            var custCodes = new List<string> { "010001"};
            #endregion

            var result = _f1917Repo.GetDatas(gupCode, aCode, bCode, cCode, custCodes);
        }

        [TestMethod]
        public void DeleteACodeByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN";
            var custCodes = new List<string> { "030099", "030098" };
            #endregion

            _f1917Repo.DeleteACodeByCustCodes(gupCode, aCode, custCodes);
        }

        [TestMethod]
        public void DeleteABCodeByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "HF";
            var bCode = "HHF";
            var custCodes = new List<string> { "030001", "030002" };
            #endregion

            _f1917Repo.DeleteABCodeByCustCodes(gupCode, aCode, bCode, custCodes);
        }

        [TestMethod]
        public void DeleteByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "SN";
            var bCode = "SN";
            var cCode = "SN";
            var custCodes = new List<string> { "010099", "010098" };
            #endregion

            _f1917Repo.DeleteByCustCodes(gupCode, aCode, bCode, cCode, custCodes);
        }
    }
}
