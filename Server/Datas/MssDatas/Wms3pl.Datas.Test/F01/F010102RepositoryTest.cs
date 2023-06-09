using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public class F010102RepositoryTest : BaseRepositoryTest
    {
        private F010102Repository _f010102Repo;
        public F010102RepositoryTest()
        {
            _f010102Repo = new F010102Repository(Schemas.CoreSchema);
        }

        /// <summary>
        /// 查詢採購單內的明細
        /// </summary>
        [TestMethod]
        public void GetF010102Datas()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var shopNo = "ZP2019090900001";
            #endregion

            _f010102Repo.GetF010102Datas(dcCode, gupCode, custCode, shopNo);
        }

        /// <summary>
        /// 刪除採購單內的明細
        /// </summary>
        [TestMethod]
        public void DeleteShopDetail()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var shopNo = "ZP2020090100002";
            #endregion

            _f010102Repo.DeleteShopDetail(dcCode, gupCode, custCode, shopNo);
        }
    }
}
