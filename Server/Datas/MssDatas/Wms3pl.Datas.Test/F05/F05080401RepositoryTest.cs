using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05080401RepositoryTest : BaseRepositoryTest
    {
        private F05080401Repository _f05080401Repo;
        public F05080401RepositoryTest()
        {
            _f05080401Repo = new F05080401Repository(Schemas.CoreSchema);
        }

       
    }
}
