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
    public class F1952RepositoryTest: BaseRepositoryTest
    {
        private F1952Repository _f1952Repo;
        public F1952RepositoryTest()
        {
            _f1952Repo = new F1952Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1952Ex()
        {
            #region Params
            var empId = "kayee001";
            #endregion

            _f1952Repo.GetF1952Ex(empId);
        }

        [TestMethod]
        public void ValidateUser()
        {
            #region Params
            var AccNo = "000001";
            #endregion

            _f1952Repo.ValidateUser(AccNo);
        }
    }
}
