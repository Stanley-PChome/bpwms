using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050102RepositoryTest : BaseRepositoryTest
    {
        private F050102Repository _f050102Repo;
        public F050102RepositoryTest()
        {
            _f050102Repo = new F050102Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF050102ExDatas()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var ordNo = "S2017022200126";
            #endregion

            _f050102Repo.GetF050102ExDatas(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void BulkDelete()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNos = new List<string>{ "S2017022000067", "S2017022000066" } ;
            #endregion

            _f050102Repo.BulkDelete(dcCode, gupCode, custCode, ordNos);
        }

        [TestMethod]
        public void BulkDelete2()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var ordNo = "S2017022000006";
            var ordSeqs = new List<string> { "1" };
            #endregion

            _f050102Repo.BulkDelete(dcCode, gupCode, custCode, ordNo, ordSeqs);
        }

        [TestMethod]
        public void GetF050102Exs()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            #endregion

            _f050102Repo.GetF050102Exs(gupCode, custCode);
        }
    }
}
