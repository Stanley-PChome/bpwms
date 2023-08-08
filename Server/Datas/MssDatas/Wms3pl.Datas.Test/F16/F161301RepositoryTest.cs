using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Wms3pl.WebServices.DataCommon;
using System.Diagnostics;
using Wms3pl.Datas.F16;
using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Test.F16
{
    [TestClass]
    public partial class F161301RepositoryTest : BaseRepositoryTest
    {
        private F161301Repository _F161301Repo;
        public F161301RepositoryTest()
        {
            _F161301Repo = new F161301Repository(Schemas.CoreSchema);
        }
    }
}
