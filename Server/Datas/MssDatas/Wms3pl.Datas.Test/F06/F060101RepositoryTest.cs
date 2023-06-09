using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
    [TestClass]
    public class F060101RepositoryTest: BaseRepositoryTest
    {
        private F060101Repository _f060101Repo;
        public F060101RepositoryTest()
        {
            _f060101Repo = new F060101Repository(Schemas.CoreSchema);
        }
    }
}
