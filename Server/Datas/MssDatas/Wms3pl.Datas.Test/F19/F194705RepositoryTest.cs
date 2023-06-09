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
    public class F194705RepositoryTest: BaseRepositoryTest
    {
        private F194705Repository _f194705Repo;
        public F194705RepositoryTest()
        {
            _f194705Repo = new F194705Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetRoutes1()
        {
            #region Params
            var zipCodes = new List<string> { "100", "103", "104" };
            var allId = "711";
            var dcCode = "001";
            #endregion

            _f194705Repo.GetRoutes(zipCodes, allId, dcCode);

        }

        [TestMethod]
        public void GetRoutes2()
        {
            #region Params
            var allId = "711";
            var dcCode = "001";
            #endregion

            _f194705Repo.GetRoutes(allId, dcCode);

        }
    }
}
