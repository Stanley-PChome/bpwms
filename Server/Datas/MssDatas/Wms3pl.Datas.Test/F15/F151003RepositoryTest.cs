using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F151003RepositoryTest: BaseRepositoryTest
    {
        private F151003Repository _f151003Repo;
        public F151003RepositoryTest()
        {
            _f151003Repo = new F151003Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF151003sByLackType()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var allocationNo = "test";
            short allocationSeq = 1;
            var itemCode = "test";
            var lackType = "test";
            #endregion

            _f151003Repo.GetF151003sByLackType(dcCode, gupCode, custCode, 
                allocationNo, allocationSeq, itemCode, lackType);
        }
    }
}
