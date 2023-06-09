using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F050301RepositoryPart3Test : BaseRepositoryTest
    {
        private F050301Repository _f050301Repo;
        public F050301RepositoryPart3Test()
        {
            _f050301Repo = new F050301Repository(Schemas.CoreSchema);
        }
    }
}
