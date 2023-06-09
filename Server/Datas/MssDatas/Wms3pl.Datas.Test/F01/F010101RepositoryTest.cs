using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;

namespace Wms3pl.Datas.Test.F01
{
    [TestClass]
    public class F010101RepositoryTest : BaseRepositoryTest
    {
        private F010101Repository _f010101Repo;
        public F010101RepositoryTest()
        {
            _f010101Repo = new F010101Repository(Schemas.CoreSchema);
        }

        /// <summary>
        /// 取得採購單資料
        /// </summary>
        [TestMethod]
        public void GetF010101ShopNoList()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var shopDateS = "2017/01/01";
            var shopDateE = "2020/01/01";

            // 以下可空
            var shopNo = "ZP2019090900001";
            var vnrCode = "成心科技";
            var vnrName = "成心科";
            var itemCode = "ISMKPPSU0101";
            var custOrdNo = "123";
            var status = "";
            #endregion

            var result = _f010101Repo.GetF010101ShopNoList(dcCode, gupCode, custCode, shopDateS, shopDateE, shopNo
                , vnrCode, vnrName, itemCode, custOrdNo, status);
        }

        /// <summary>
        /// 取得採購單資料
        /// </summary>
        [TestMethod]
        public void GetF000904Data()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var shopNo = "ZP2020090100001";
            #endregion

            var result = _f010101Repo.GetF010101Datas(dcCode, gupCode, custCode, shopNo);
        }

        /// <summary>
        /// 取得採購單報表資料
        /// </summary>
        [TestMethod]
        public void GetF010101Reports()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var shopNo = "ZP2019090900001";
            #endregion

            var result = _f010101Repo.GetF010101Reports(dcCode, gupCode, custCode, shopNo);
        }
    }
}
