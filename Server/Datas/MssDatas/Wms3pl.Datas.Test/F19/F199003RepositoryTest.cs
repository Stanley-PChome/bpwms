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
    public class F199003RepositoryTest: BaseRepositoryTest
    {
        private F199003Repository _f199003Repo;
        public F199003RepositoryTest()
        {
            _f199003Repo = new F199003Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1980Datas()
        {
            #region Params
            var dcCode = "001";
            var accItemKindId = "01";
            var accKind = "B";
            var status = "0";
            #endregion

            _f199003Repo.GetShippingValuation(dcCode, accItemKindId, accKind, status);
        }
    }
}
