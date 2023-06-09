using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F0500RepositoryTest : BaseRepositoryTest
    {
        private F0500Repository _f0500Repo;
        public F0500RepositoryTest()
        {
            _f0500Repo = new F0500Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetDatas()
        {
            _f0500Repo.GetDatas();
        }
    }
}
