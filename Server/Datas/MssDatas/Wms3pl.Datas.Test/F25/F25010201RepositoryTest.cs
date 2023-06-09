using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F25010201RepositoryTest : BaseRepositoryTest
    {
        private readonly F25010201Repository _f25010201Repository;
        public F25010201RepositoryTest()
        {
            _f25010201Repository = new F25010201Repository(Schemas.CoreSchema);
        }

       
    }
}
