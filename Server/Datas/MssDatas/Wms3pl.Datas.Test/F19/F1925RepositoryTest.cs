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
    public class F1925RepositoryTest: BaseRepositoryTest
    {
        private F1925Repository _f1925Repo;
        public F1925RepositoryTest()
        {
            _f1925Repo = new F1925Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1925Datas()
        {
            #region Params
            var DEP_ID = "A001";
            var DEP_NAME = "人";
            #endregion

            _f1925Repo.GetF1925Datas(DEP_ID, DEP_NAME);

        }
    }
}
