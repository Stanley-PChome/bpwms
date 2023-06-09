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
    public class F1916RepositoryTest : BaseRepositoryTest
    {
        private F1916Repository _f1916Repo;
        public F1916RepositoryTest()
        {
            _f1916Repo = new F1916Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1916SearchData()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var bCode = "APPA";
            var clsName = "帽";
            #endregion

            var result = _f1916Repo.GetF1916SearchData(gupCode, custCode, bCode, clsName);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN";
            var bCode = "DNA";
            var custCodes = new List<string> { "030001" };
            #endregion

            var result = _f1916Repo.GetDatas(gupCode, aCode, bCode, custCodes);
        }

        [TestMethod]
        public void DeleteACodeByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "MM";
            var custCodes = new List<string> { "010001" };
            #endregion

            _f1916Repo.DeleteACodeByCustCodes(gupCode, aCode, custCodes);
        }

        [TestMethod]
        public void DeleteByCustCodes()
        {
            #region Params
            var gupCode = "01";
            var aCode = "DN1";
            var bCode = "HD1";
            var custCodes = new List<string> { "010001" };
            #endregion

            _f1916Repo.DeleteByCustCodes(gupCode, aCode, bCode, custCodes);
        }
    }
}
