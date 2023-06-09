using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910502RepositoryTest : BaseRepositoryTest
    {
        private readonly F910502Repository _f910502Repository;
        public F910502RepositoryTest()
        {
            _f910502Repository = new F910502Repository(Schemas.CoreSchema);
        }


        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
