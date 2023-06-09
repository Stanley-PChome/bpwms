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
    public class F1980RepositoryTest: BaseRepositoryTest
    {
        private F1980Repository _f1980Repo;
        public F1980RepositoryTest()
        {
            _f1980Repo = new F1980Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetF1980Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var typeId = "G";
            var account = "wms";
            #endregion

            _f1980Repo.GetF1980Datas(dcCode, null, custCode, null, account);
        }

        [TestMethod]
        public void GetF1980CheckDatas()
        {
            #region Params
            var dcCode = "001";
            #endregion

            _f1980Repo.GetF1980CheckDatas(dcCode);
        }

        [TestMethod]
        public void GetUserWarehouse()
        {
            #region Params
            var userId = "wms";
            var gupCode = "01";
            var custCode = "030001";
            #endregion

            _f1980Repo.GetUserWarehouse(userId, gupCode, custCode);
        }

        [TestMethod]
        public void GetFirstData()
        {
            #region Params
            var dcCode = "001";
            var warehouseType = "G";
            #endregion

            _f1980Repo.GetFirstData(dcCode, warehouseType);
        }

        [TestMethod]
        public void GetInventoryWareHouses()
        {
            #region Params
            var dcCode = "001";
            var wareHouseType = "G";
            var tool = "123";
            #endregion

            _f1980Repo.GetInventoryWareHouses(dcCode, wareHouseType, tool);
        }

        [TestMethod]
        public void GetF1980ByLocCode()
        {
            #region Params
            var dcCode = "001";
            var locCode = "10C020404";
            #endregion

            _f1980Repo.GetF1980ByLocCode(dcCode, locCode);
        }

        [TestMethod]
        public void GetWareHouseTmprTypeByLocCode()
        {
            #region Params
            var dcCode = "001";
            var locCodes = new List<string> { "10C020404", "10C020405" };
            #endregion

            _f1980Repo.GetWareHouseTmprTypeByLocCode(dcCode, locCodes);
        }

        [TestMethod]
        public void GetDatas()
        {
            #region Params
            var dcCode = "001";
            var warehouseIds = new List<string> { "G07"};
            #endregion

            _f1980Repo.GetDatas(dcCode, warehouseIds);
        }

        [TestMethod]
        public void GetWareHouseTmprTypeByLocCodes()
        {
            #region Params
            var dcCodes = new List<string> { "001" };
            var locCodes = new List<string> { "10H000004", "10H000005" };
            #endregion

            _f1980Repo.GetWareHouseTmprTypeByLocCodes( dcCodes, locCodes);
        }

        [TestMethod]
        public void GetWareHouseTmprTypeByWareHouse()
        {
            #region Params
            var dcCode ="001";
            var warehouseId = "G07";
            #endregion

            _f1980Repo.GetWareHouseTmprTypeByWareHouse(dcCode, warehouseId);
        }
    }
}
