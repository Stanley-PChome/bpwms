using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05000201RepositoryTest : BaseRepositoryTest
    {
        private F05000201Repository _f05000201Repo;
        public F05000201RepositoryTest()
        {
            _f05000201Repo = new F05000201Repository(Schemas.CoreSchema);
        }
    }
}
