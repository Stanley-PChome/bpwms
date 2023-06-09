using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F06;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F06
{
    [TestClass]
    public class F060201RepositoryTest: BaseRepositoryTest
    {
        private F060201Repository _f060201Repo;
        public F060201RepositoryTest()
        {
            _f060201Repo = new F060201Repository(Schemas.CoreSchema);
        }
    }
}
