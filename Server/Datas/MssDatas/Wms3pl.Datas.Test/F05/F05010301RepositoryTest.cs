using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05010301RepositoryTest : BaseRepositoryTest
    {
        private F05010301Repository _f05010301Repo;
        public F05010301RepositoryTest()
        {
            _f05010301Repo = new F05010301Repository(Schemas.CoreSchema);
        }
    }
}
