using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public partial class F010201RepositoryPart2Test : BaseRepositoryTest
    {
        private F010202Repository _f010202Repo;
        public F010201RepositoryPart2Test()
        {
            _f010202Repo = new F010202Repository(Schemas.CoreSchema);
        }
    }
}
