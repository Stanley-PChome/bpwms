using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050103RepositoryTest : BaseRepositoryTest
    {
        private F050103Repository _f050103Repo;
        public F050103RepositoryTest()
        {
            _f050103Repo = new F050103Repository(Schemas.CoreSchema);
        }
    }
}
