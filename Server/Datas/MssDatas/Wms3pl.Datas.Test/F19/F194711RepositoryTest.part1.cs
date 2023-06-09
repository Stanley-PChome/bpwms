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
    public class F194711RepositoryTest: BaseRepositoryTest
    {
        private F194711Repository _f194711Repo;
        public F194711RepositoryTest()
        {
            _f194711Repo = new F194711Repository(Schemas.CoreSchema);
        }
    }
}
