using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F05
{
    [TestClass]
    public class F055002RepositoryTest : BaseRepositoryTest
    {
        private F055002Repository _f055002Repo;
        public F055002RepositoryTest()
        {
            _f055002Repo = new F055002Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void GetLatestPackageBoxSeq()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017080100001";
            short packageBoxNo = 1;
            #endregion

            _f055002Repo.GetLatestPackageBoxSeq(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
        }

        [TestMethod]
        public void GetPackageBoxSeqsByWmsOrdNos()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNos = new List<string> { "O2017022000005", "O2017022000006" };
            #endregion

            //_f055002Repo.GetPackageBoxSeqsByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos);
        }

        [TestMethod]
        public void GetDeliveryData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000005";
            short packageBoxNo = 1;
            var itemCode = "BB010701";
            #endregion

            _f055002Repo.GetDeliveryData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo, itemCode);
        }

        [TestMethod]
        public void GetQuantityOfDeliveryInfo()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000005";
            var itemCode = "BB010701";
            short packageBoxNo = 1;
            #endregion

            _f055002Repo.GetQuantityOfDeliveryInfo(dcCode, gupCode, custCode, wmsOrdNo, itemCode, packageBoxNo);
        }

        [TestMethod]
        public void GetDeliveryReport()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000005";
            short? packageBoxNo = 1;
            #endregion

            _f055002Repo.GetDeliveryReport(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
        }

        [TestMethod]
        public void GetF055002WithF2501s()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017022000005";
            #endregion

            _f055002Repo.GetF055002WithF2501s(dcCode, gupCode, custCode, wmsOrdNo);
        }

        [TestMethod]
        public void GetF055002WithGridLog()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "010001";
            var wmsOrdNo = "O2017030700023";
            #endregion

            _f055002Repo.GetF055002WithGridLog(dcCode, gupCode, custCode, wmsOrdNo);
		}

		[TestMethod]
		public void GetDatasByOrdSeqs()
		{
			#region Params
			var dcCode = "001";
			var gupCode = "01";
			var custCode = "010001";
			var ordNo = "S20230101000001";
			var ordSeqs = new List<string> { "1", "2" };
			#endregion

			_f055002Repo.GetDatasByOrdSeqs(dcCode, gupCode, custCode, ordNo, ordSeqs);
		}

	}
}
