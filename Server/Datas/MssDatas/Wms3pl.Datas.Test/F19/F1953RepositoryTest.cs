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
    public class F1953RepositoryTest: BaseRepositoryTest
    {
        private F1953Repository _f1953Repo;
        public F1953RepositoryTest()
        {
            _f1953Repo = new F1953Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetFunctionShowInfos()
        {
            #region Params
            var account = "000002";
            #endregion

            _f1953Repo.GetFunctionShowInfos(account);
        }

        [TestMethod]
        public void GetF1953Data()
        {
            #region Params
            #endregion

            _f1953Repo.GetF1953Data();
        }
    }
}
