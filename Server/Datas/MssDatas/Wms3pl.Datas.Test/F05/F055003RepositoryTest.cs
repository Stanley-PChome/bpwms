using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F055003RepositoryTest : BaseRepositoryTest
    {
        private F055003Repository _f055003Repo;
        public F055003RepositoryTest()
        {
            _f055003Repo = new F055003Repository(Schemas.CoreSchema);
        }

    }
}
