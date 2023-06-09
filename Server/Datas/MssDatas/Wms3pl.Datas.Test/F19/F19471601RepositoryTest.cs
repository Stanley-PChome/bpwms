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
    public class F19471601RepositoryTest: BaseRepositoryTest
    {
        private F19471601Repository _f19471601Repo;
        public F19471601RepositoryTest()
        {
            _f19471601Repo = new F19471601Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatasByAllId()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var delvNo = "cdcs";
            #endregion

            _f19471601Repo.GetF19471601Datas(gupCode, custCode, dcCode, delvNo);
        }
    }
}
