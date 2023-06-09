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
    public class F1934RepositoryTest: BaseRepositoryTest
    {
        private F1934Repository _f1934Repo;
        public F1934RepositoryTest()
        {
            _f1934Repo = new F1934Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1934EXDatas()
        {
            #region Params
            #endregion

            _f1934Repo.GetF1934EXDatas();

        }

        [TestMethod]
        public void GetF1934Datas()
        {
            #region Params
            var COUDIV_ID = "01";
            #endregion

            _f1934Repo.GetF1934Datas(COUDIV_ID);

        }
    }
}
