using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon.Enums;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F15100101RepositoryTest: BaseRepositoryTest
    {
        private F15100101Repository _f15100101Repo;
        public F15100101RepositoryTest()
        {
            _f15100101Repo = new F15100101Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GenSCCode()
        {
            var dbKeyEnums = System.Enum.GetValues(typeof(DbKeyEnum)).Cast<DbKeyEnum>();
            var currKeyEnum = dbKeyEnums.Where(x => (int)x == 17).FirstOrDefault();
            var result = DbSchemaHelper.GenSCCode(currKeyEnum);
			      var encode = AesCryptor.Current.Encode(result);
			      var decode = AesCryptor.Current.Decode(encode);
		}
    }
}
