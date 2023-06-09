using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F05500102RepositoryTest : BaseRepositoryTest
    {
        private F05500102Repository _f05500102Repo;
        public F05500102RepositoryTest()
        {
            _f05500102Repo = new F05500102Repository(Schemas.CoreSchema);
        }
    }
}
