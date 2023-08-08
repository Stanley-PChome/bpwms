using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F00
{
    [TestClass]
    public class F000904RepositoryTest: BaseRepositoryTest
    {
        private F000904Repository _f000904Repo;

        public F000904RepositoryTest()
        {
            _f000904Repo = new F000904Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetTopicValueName()
        {
            #region Params
            var topic = "F195401";
            var subTopic = "SUB_CATEGORY";
            var value = "D02";
            #endregion

            _f000904Repo.GetTopicValueName(topic, subTopic, value);
        }
    }
}
