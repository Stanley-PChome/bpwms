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
    public class F198001RepositoryTest: BaseRepositoryTest
    {
        private F198001Repository _f198001Repo;
        public F198001RepositoryTest()
        {
            _f198001Repo = new F198001Repository(Schemas.CoreSchema);
        }

      
    }
}
