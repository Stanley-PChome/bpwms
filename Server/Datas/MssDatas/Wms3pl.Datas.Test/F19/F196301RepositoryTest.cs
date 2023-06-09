using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.Datas.Test.F19
{
    [TestClass]
    public  class F196301RepositoryTest: BaseRepositoryTest
    {
        private F196301Repository _f196301Repo;
        public F196301RepositoryTest()
        {
            _f196301Repo = new F196301Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void DeleteLocIn()
        {
            #region Params
            var workgroupId = 1;
            var dcCode = "001";
            var locList = new List<string>{ "10H000011", "10H000012" };
            #endregion

            _f196301Repo.DeleteLocIn(workgroupId, dcCode, locList);
        }

    [TestMethod]
    public void DeleteLocIn_2()
    {
      #region Params
      var workgroupId = 1;
      var dcCode = "001";
      //var locList = new List<string> { "10H000011", "10H000012" };
      List<F1912LocData> locList= new List<F1912LocData> {
          new F1912LocData
          {
              DC_CODE = "12",
              AREA_CODE = "",
              LOC_CODE = "Q00001"
          }
    };
      #endregion

      _f196301Repo.DeleteLocIn_2(workgroupId, dcCode, locList);
    }

    [TestMethod]
        public void GetNonAllowedF1912LocDatas1()
        {
            #region Params
            var dcCode = "001";
            var warehouseId = "G03";
            var floor = "1";
            var beginLocCode = "10A010101";
            var endLocCode = "ZZ123";
            var workId = "1";
            #endregion

            _f196301Repo.GetNonAllowedF1912LocDatas(dcCode, warehouseId, floor, beginLocCode, endLocCode, workId);
        }

        [TestMethod]
        public void GetNonAllowedF1912LocDatas2()
        {
            #region Params
            var dcCode = "001";
            var warehouseId = "G03";
            var floor = "1";
            var beginLocCode = "10A010101";
            var endLocCode = "ZZ123";
            var excludeLocCodes =new List<string>{ "10C020404", "10C020405" };
            #endregion

            _f196301Repo.GetNonAllowedF1912LocDatas(dcCode, warehouseId, floor, beginLocCode, endLocCode,    excludeLocCodes);
        }

        [TestMethod]
        public void GetAllowedF1912LocDatas()
        {
            #region Params
            var workId = "001";
            #endregion

            _f196301Repo.GetAllowedF1912LocDatas(workId);
        }
    }
}
