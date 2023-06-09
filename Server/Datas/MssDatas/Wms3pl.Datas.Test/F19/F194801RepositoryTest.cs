using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F194801RepositoryTest: BaseRepositoryTest
    {
        private F194801Repository _f194801Repo;
        public F194801RepositoryTest()
        {
            _f194801Repo = new F194801Repository(Schemas.CoreSchema);
        }
    }
}
