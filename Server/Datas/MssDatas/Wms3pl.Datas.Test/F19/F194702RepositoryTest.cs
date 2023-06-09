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
    public class F194702RepositoryTest: BaseRepositoryTest
    {
        private F194702Repository _f194702Repo;
        public F194702RepositoryTest()
        {
            _f194702Repo = new F194702Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF194702()
        {
            #region Params
            #endregion

            _f194702Repo.GetF194702();

        }
    }
}
