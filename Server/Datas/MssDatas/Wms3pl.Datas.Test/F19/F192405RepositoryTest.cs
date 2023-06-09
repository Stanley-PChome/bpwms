using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F192405RepositoryTest : BaseRepositoryTest
    {
        private F192405Repository _f192405Repo;
        public F192405RepositoryTest()
        {
            _f192405Repo = new F192405Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void Delete1()
        {
            #region Params
            var empId = "A12345";
            var scheduleId = "A1";
            #endregion

            _f192405Repo.Delete(empId, scheduleId);

        }
    }
}
