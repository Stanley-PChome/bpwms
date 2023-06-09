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
    public class F1928RepositoryTest : BaseRepositoryTest
    {
        private F1928Repository _f1928Repo;
        public F1928RepositoryTest()
        {
            _f1928Repo = new F1928Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetFirstData()
        {
            #region Params
            #endregion

            _f1928Repo.GetFirstData();

        }
    }
}
