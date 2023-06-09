using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F14
{
    [TestClass]
    public class F140113RepositoryTest: BaseRepositoryTest
    {
        private readonly F140113Repository _f140113Repository;
        public F140113RepositoryTest()
        {
            _f140113Repository = new F140113Repository(Schemas.CoreSchema);
        }
    }
}
