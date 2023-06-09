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
    public class F19470801RepositoryTest: BaseRepositoryTest
    {
        private F19470801Repository _f19470801Repo;
        public F19470801RepositoryTest()
        {
            _f19470801Repo = new F19470801Repository(Schemas.CoreSchema);
        }
    }
}
