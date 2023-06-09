using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F151004RepositoryTest: BaseRepositoryTest
    {
        private F151004Repository _f151004Repo;
        public F151004RepositoryTest()
        {
            _f151004Repo = new F151004Repository(Schemas.CoreSchema);
        }
    }
}
