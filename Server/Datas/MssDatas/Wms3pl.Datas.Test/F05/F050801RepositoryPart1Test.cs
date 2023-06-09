using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050801RepositoryPart1Test : BaseRepositoryTest
    {
        private F050801Repository _f050801Repo;
        public F050801RepositoryPart1Test()
        {
            _f050801Repo = new F050801Repository(Schemas.CoreSchema);
        }
    }
}
