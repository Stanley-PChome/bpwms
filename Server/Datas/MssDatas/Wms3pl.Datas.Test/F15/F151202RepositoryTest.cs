using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.Datas.Test.F15
{
   [TestClass]
    public class F151202RepositoryTest : BaseRepositoryTest
    {
        private F151202Repository _f151202Repo;
        public F151202RepositoryTest()
        {
            _f151202Repo = new F151202Repository(Schemas.CoreSchema);
        }

    }
}
