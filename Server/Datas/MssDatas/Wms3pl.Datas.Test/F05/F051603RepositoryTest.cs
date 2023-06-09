using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F051603RepositoryTest : BaseRepositoryTest
    {
        private F051603Repository _f051603Repo;
        public F051603RepositoryTest()
        {
            _f051603Repo = new F051603Repository(Schemas.CoreSchema);
        }
    }
}
