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
    public class F1909RepositoryTest: BaseRepositoryTest
    {
        private F1909Repository _f1909Repo;
        public F1909RepositoryTest()
        {
            _f1909Repo = new F1909Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetGupCode()
        {
            #region Params
            var custCode = "010001";
            #endregion

            _f1909Repo.GetGupCode(custCode);
        }
    }
}
