using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050303RepositoryTest : BaseRepositoryTest
    {
        private F050303Repository _f050303Repo;
        public F050303RepositoryTest()
        {
            _f050303Repo = new F050303Repository(Schemas.CoreSchema);
        }
    }
}
