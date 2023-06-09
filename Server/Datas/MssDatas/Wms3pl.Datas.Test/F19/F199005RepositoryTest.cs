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
    public class F199005RepositoryTest : BaseRepositoryTest
    {
        private F199005Repository _f199003Repo;
        public F199005RepositoryTest()
        {
            _f199003Repo = new F199005Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF199005()
        {
            #region Params
            var dcCode = "001";
            var accItemKindId = "01";
            var logiType = "01";
            var taxType = "0";
            var accKind = "D";
            var isSpecialCar = "0";
            var status = "0";
            #endregion

            _f199003Repo.GetF199005(dcCode, accItemKindId, logiType, taxType,
                                    accKind, isSpecialCar, status);
        }
    }
}
