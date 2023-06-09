using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public class F1952_HISTORYRepositoryTest : BaseRepositoryTest
    {
        private F1952_HISTORYRepository _f1952HistoryRepo;
        public F1952_HISTORYRepositoryTest()
        {
            _f1952HistoryRepo = new F1952_HISTORYRepository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetLastFrequencyF1952History()
        {
            #region Params
            var empId = "kayee001";
            var unrepeatableFrequency = 1;
            #endregion

            _f1952HistoryRepo.GetLastFrequencyF1952History(empId, unrepeatableFrequency);
        }
    }
}
