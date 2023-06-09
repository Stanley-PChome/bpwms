using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using System.Linq;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F19000101RepositoryTest : BaseRepositoryTest
    {
        private F19000101Repository _F19000101Repo;

        public F19000101RepositoryTest()
        {
            _F19000101Repo = new F19000101Repository(Schemas.CoreSchema);
        }
        [TestMethod]
        public void GetMilestones()
        {
            #region Params
            decimal ticketId = 123;
            #endregion
            var result2 = _F19000101Repo.GetMilestones(ticketId).ToList();
        }
    }
}
