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
    public class F1929RepositoryTest: BaseRepositoryTest
    {
        private F1929Repository _f1929Repo;
        public F1929RepositoryTest()
        {
            _f1929Repo = new F1929Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            #endregion

            _f1929Repo.GetDatas();

        }

        [TestMethod]
        public void GetF1929WithF1909Tests()
        {
            #region Params
            var gupCode = "01";
            #endregion

            _f1929Repo.GetF1929WithF1909Tests(gupCode);

        }

        [TestMethod]
        public void UpdateName()
        {
            #region Params
            var gupCode = "01";
            var subName = "01";
            #endregion

            _f1929Repo.UpdateName(gupCode, subName);

        }

        [TestMethod]
        public void UpdateName2()
        {
            #region Params
            var gupCode = "01";
            var subName = "01";
            #endregion

            _f1929Repo.UpdateName2(gupCode, subName);

        }

        [TestMethod]
        public void UpdateName3()
        {
            #region Params
            var gupCode = "01";
            var subName = "台灣航空貨運";
            #endregion

            _f1929Repo.UpdateName3(gupCode, subName);

        }
    }
}
