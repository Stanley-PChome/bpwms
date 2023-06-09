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
    public class F195402RepositoryTest: BaseRepositoryTest
    {
        private F195402Repository _f195402Repo;
        public F195402RepositoryTest()
        {
            _f195402Repo = new F195402Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetNewMenuCode()
        {
            #region Params
            #endregion

            _f195402Repo.GetNewMenuCode();
        }
    }
}
