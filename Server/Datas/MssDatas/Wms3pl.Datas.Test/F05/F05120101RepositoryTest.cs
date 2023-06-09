using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05120101RepositoryTest : BaseRepositoryTest
    {
        private F05120101Repository _f05120101Repo;
        public F05120101RepositoryTest()
        {
            _f05120101Repo = new F05120101Repository(Schemas.CoreSchema);
        }
    }
}
