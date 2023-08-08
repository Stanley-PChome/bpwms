using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F91;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F91
{
    [TestClass]
    public class F910209RepositoryTest : BaseRepositoryTest
    {
        private readonly F910209Repository _f910209Repository;
        public F910209RepositoryTest()
        {
            _f910209Repository = new F910209Repository(Schemas.CoreSchema);
        }

        private void Output(object obj)
        {
            Console.WriteLine($@"{JsonSerializer.Serialize(obj)}");
        }
    }
}
