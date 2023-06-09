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
    public class F192401RepositoryTest: BaseRepositoryTest
    {
        private F192401Repository _f192401Repo;
        public F192401RepositoryTest()
        {
            _f192401Repo = new F192401Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Delete1()
        {
            #region Params
            var empId = "000002";
            var groupId = 55;
            #endregion

            _f192401Repo.Delete(empId, groupId);
        }

        [TestMethod]
        public void Delete2()
        {
            #region Params
            var groupId = 55;
            #endregion

            _f192401Repo.Delete(groupId);
        }

        [TestMethod]
        public void CheckAccFunction()
        {
            #region Params
            var funcNo = "BP1901020101";
            var accNo = "A12345";
            #endregion

            _f192401Repo.CheckAccFunction(funcNo, accNo);
        }
    }
}
