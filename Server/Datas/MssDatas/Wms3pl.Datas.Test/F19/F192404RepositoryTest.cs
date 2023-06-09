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
    public class F192404RepositoryTest : BaseRepositoryTest
    {
        private F192404Repository _f192404Repo;
        public F192404RepositoryTest()
        {
            _f192404Repo = new F192404Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF192404Datas()
        {
            #region Params
            var DC_CODE = "001";
            #endregion

            _f192404Repo.GetF192404Datas(DC_CODE);
        }
    }
}
