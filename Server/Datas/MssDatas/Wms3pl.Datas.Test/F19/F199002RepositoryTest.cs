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
    public class F199002RepositoryTest: BaseRepositoryTest
    {
        private F199002Repository _f199002Repo;
        public F199002RepositoryTest()
        {
            _f199002Repo = new F199002Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1980Datas()
        {
            #region Params
            var dcCode = "001";
            var accItemKindId = "01";
            var OrdType = "A";
            var accKind = "A";
            var accUnit = "02";
            var status = "0";
            #endregion

            _f199002Repo.GetJobValuation(dcCode, accItemKindId, OrdType, accKind, accUnit, status);
        }
    }
}
