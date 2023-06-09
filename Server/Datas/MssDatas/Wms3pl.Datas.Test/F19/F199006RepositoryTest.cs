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
    public class F199006RepositoryTest: BaseRepositoryTest
    {
        private F199006Repository _f199006Repo;
        public F199006RepositoryTest()
        {
            _f199006Repo = new F199006Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF199006s()
        {
            #region Params
            var dcCode = "001";
            var accItemName = "出貨";
            var status = "0";
            #endregion

            _f199006Repo.GetF199006s(dcCode, accItemName, status);
        }
    }
}
