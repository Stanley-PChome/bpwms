using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.Test.F15
{
    [TestClass]
    public class F1511RepositoryTest: BaseRepositoryTest
    {
        private F1511Repository _f1511Repo;
        public F1511RepositoryTest()
        {
            _f1511Repo = new F1511Repository(Schemas.CoreSchema);
        }

        [TestMethod]
        public void UpdateStatusCancel()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var ordNoList = new string[] { "P2019071900003" };
            #endregion

            _f1511Repo.UpdateStatusCancel(gupCode, custCode, dcCode, ordNoList);
        }

        [TestMethod]
        public void DeleteDatas()
        {
            #region Params
            var ordNo = "T2018090500022";
            var gupCode = "01";
            var custCode = "010002";
            var dcCode = "001";
            
            #endregion

            _f1511Repo.DeleteDatas(ordNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void UpdateDatasForCancel1()
        {
            #region Params
            var ordNo = "T2018090500023";
            var gupCode = "01";
            var custCode = "010002";
            var dcCode = "001";

            #endregion

            _f1511Repo.UpdateDatasForCancel(ordNo, gupCode, custCode, dcCode);
        }

        [TestMethod]
        public void GetDatas1()
        {
            #region Params
            var gupCode = "01";
            var custCode = "030001";
            var dcCode = "001";
            var ordNo = "P2019041900003";
            #endregion

            _f1511Repo.GetDatas(dcCode, gupCode, custCode, ordNo);
        }

        [TestMethod]
        public void DeleteData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var ordNo = "P2019041900003";
            var ordSeq = "0003";
            #endregion

            _f1511Repo.DeleteData(dcCode, gupCode, custCode, ordNo, ordSeq);
        }

        [TestMethod]
        public void UpdateData()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var ordNo = "P2019041900003";
            var ordSeq = "0002";
            var bPickQty = 11;
            var aPickQty = 11;
            var status = "1";
            #endregion

            _f1511Repo.UpdateData(dcCode, gupCode, custCode, ordNo, ordSeq, bPickQty,
                aPickQty, status);
        }

      

        [TestMethod]
        public void SetAlreadyDebitByWmsOrdNos()
        {
            #region Params
            var dcCode = "001";
            var gupCode = "01";
            var custCode = "030001";
            var wmsOrdNos = new List<string> { "P2019041900003" };
            #endregion

            _f1511Repo.SetAlreadyDebitByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos);
        }

       

        [TestMethod]
        public void BulkDeleteData()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var ordNos = new List<string> { "P2017022000013" };
            #endregion

            _f1511Repo.BulkDeleteData(gupCode, custCode, dcCode, ordNos);
        }

        [TestMethod]
        public void UpdateDatasForCancel2()
        {
            #region Params
            var gupCode = "01";
            var custCode = "010001";
            var dcCode = "001";
            var ordNos = new List<string> { "P2017022300002" };
            #endregion

            _f1511Repo.UpdateDatasForCancel(gupCode, custCode, dcCode, ordNos);
        }

				[TestMethod]
				public void GetF1511sByWmsOrdNo()
				{
					#region Params
					var dcCode = "001";
					var gupCode = "01";
					var custCode = "030001";
					var wmsOrdNos = new List<string> { "O2018051100019" };
					#endregion

					_f1511Repo.GetF1511sByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNos);
				}

				[TestMethod]
				public void GetDatas2()
				{
					#region Params
					var dcCode = "001";
					var gupCode = "01";
					var custCode = "030001";
					var orderNos = new List<string> { "P2019041900003" };
					#endregion

					_f1511Repo.GetDatas(dcCode, gupCode, custCode, orderNos);
				}
	}
}
