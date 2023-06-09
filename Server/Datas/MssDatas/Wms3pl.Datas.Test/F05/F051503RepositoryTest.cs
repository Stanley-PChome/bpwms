using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051503RepositoryTest : BaseRepositoryTest
    {
        private F051503Repository _f051503Repo;
        public F051503RepositoryTest()
        {
            _f051503Repo = new F051503Repository(Schemas.CoreSchema);
        }
    }
}
