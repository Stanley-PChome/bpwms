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
    public class F1933RepositoryTest: BaseRepositoryTest
    {
        private F1933Repository _f1933Repo;
        public F1933RepositoryTest()
        {
            _f1933Repo = new F1933Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var COUDIV_ID = "01";
            #endregion

            _f1933Repo.GetF1933Datas(COUDIV_ID);

        }

    }
}
