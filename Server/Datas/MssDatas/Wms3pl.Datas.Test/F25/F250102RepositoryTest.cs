using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F25
{
    [TestClass]
    public class F250102RepositoryTest : BaseRepositoryTest
    {
        private readonly F250102Repository _f250102Repository;
        public F250102RepositoryTest()
        {
            _f250102Repository = new F250102Repository(Schemas.CoreSchema);
        }

       

        

       
    }
}
