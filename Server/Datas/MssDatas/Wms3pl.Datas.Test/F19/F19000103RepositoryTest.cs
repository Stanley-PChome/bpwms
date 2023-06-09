using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Data.SqlClient;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public  class F19000103RepositoryTest : BaseRepositoryTest
    {
        private F19000103Repository _F19000103Repo;

        public F19000103RepositoryTest()
        {
            _F19000103Repo = new F19000103Repository(Schemas.CoreSchema);
        }

        [TestMethod]
		public void GetExistsTicketAllMilestoneNo()
		{
            #region Params
            var topic = "F195401";
            var subTopic = "SUB_CATEGORY";
            #endregion

            var result = _F19000103Repo.GetExistsTicketAllMilestoneNo().ToList();
        }
	}
}
