using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public partial class F19DataService : DataServiceBase<Wms3plDbContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F19");
            config.SetEntitySetAccessRule("PREFERENCEs", EntitySetRights.All);
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("GetUserFunctions", ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }


        /// <summary>
        /// 取得有權限的配送商主檔
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1947> GetAllowedF1947s(string dcCode, string gupCode, string custCode)
        {
            var repo = new F1947Repository(Schemas.CoreSchema);
            return repo.GetAllowedF1947s(dcCode, gupCode, custCode);
        }

        /// <summary>
        /// 取得是否為紙箱品項
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="isCarton"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1903> GetF1903sByCarton(string gupCode, string custCode, string isCarton)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetF1903sByCarton(gupCode, custCode, isCarton);
        }

        [WebGet]
        public IQueryable<F1925> GetF1925sByDEPIDANDDEPNAME(string DEP_ID, string DEP_NAME)
        {
            var F1925Repository = new F1925Repository(Schemas.CoreSchema);
            return F1925Repository.GetF1925Datas(DEP_ID, DEP_NAME);
        }

        [WebGet]
        public IQueryable<F192404> GetF192404sByDCCODE(string DC_CODE)
        {
            var F192404Repository = new F192404Repository(Schemas.CoreSchema);
            return F192404Repository.GetF192404Datas(DC_CODE);
        }

        [WebGet]
        public IQueryable<F1933> GetF1933sByCOUDIVID(string COUDIVID)
        {
            var F1933Repository = new F1933Repository(Schemas.CoreSchema);
            return F1933Repository.GetF1933Datas(COUDIVID);
        }

        [WebGet]
        public IQueryable<F1934> GetF1934sByCOUDIVID(string COUDIVID)
        {
            var F1934Repository = new F1934Repository(Schemas.CoreSchema);
            return F1934Repository.GetF1934Datas(COUDIVID);
        }

        /// <summary>
        /// 取出貨單配送商
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        [WebGet]
        public F1947 GetAllIdByWmsOrdNo(string wmsOrdNo, string gupCode, string custCode, string dcCode)
        {
            var repo = new F1947Repository(Schemas.CoreSchema);
            return repo.GetAllIdByWmsOrdNo(wmsOrdNo, gupCode, custCode, dcCode);
        }

        [WebGet]
        public IQueryable<F1905> GetCartonSize(string gupCode, string custCode, string searchCode)
        {
            var repo = new F1905Repository(Schemas.CoreSchema);
            return repo.GetCartonSize(gupCode, custCode, searchCode);
        }

        [WebGet]
        public IQueryable<F1903> GetCartonItem(string gupCode, string custCode, string searchCode)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetCartonItem(gupCode, custCode, searchCode);
        }

        /// <summary>
        /// 取得已過濾人員權限的廠商主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="vnrCode"></param>
        /// <param name="vnrName"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1908> GetAllowedF1908s(string gupCode, string vnrCode, string vnrName, string custCode)
        {
            var repo = new F1908Repository(Schemas.CoreSchema);
            return repo.GetAllowedF1908s(gupCode, vnrCode, vnrName, custCode);
        }

        /// <summary>
        /// 取得已過濾人員權限的廠商主檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="vnrCode"></param>
        /// <param name="vnrName"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F1910> GetAllowedF1910s(string gupCode, string custCode, string retailCode, string retailName)
        {
            var repo = new F1910Repository(Schemas.CoreSchema);
            return repo.GetAllowedF1910s(gupCode, custCode, retailCode, retailName);
        }

        #region GetF199006s
        [WebGet]
        public IQueryable<F199006> GetF199006s(string dcCode, string accItemName, string status)
        {
            var repo = new F199006Repository(Schemas.CoreSchema);
            return repo.GetF199006s(dcCode, accItemName, status);
        }
        #endregion

        [WebGet]
        public IQueryable<F1903> GetF1912s(string gupCode, string custCode, string itemCode, string itemName)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetF1912s(gupCode, custCode, itemCode, itemName);
        }

        [WebGet]
        public IQueryable<F1909> GetF1909ByDc(string dcCode)
        {
            var repo = new F1909Repository(Schemas.CoreSchema);
            return repo.GetDatasByDc(dcCode);
        }

        [WebGet]
        public IQueryable<F1903> GetF1903sByItemName(string gupCode, string custCode, string itemName, int itemSearchMaximum)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetF1903sByItemName(gupCode, custCode, itemName, itemSearchMaximum);
        }

        [WebGet]
        public IQueryable<F1903> GetF1903sByItemCode(string gupCode, string itemCode, string custCode, string account)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetF1903sByItemCode(gupCode, itemCode, custCode, account);
        }

        [WebGet]
        public IQueryable<F1909> GetF1909Datas(string gupCode, string custName, string account)
        {
            var repo = new F1909Repository(Schemas.CoreSchema);
            return repo.GetF1909Datas(gupCode, custName, account);
        }

        [WebGet]
        public IQueryable<F190701> GetQueryListByGroupId(string gid)
        {
            var rep = new F190701Repository(Schemas.CoreSchema);
            var result = rep.GetQueryListByGroupId(gid);
            return result;
        }

        [WebGet]
        public IQueryable<F190701> GetQueryListByEmpId(string empId, string qGroup)
        {
            var rep = new F190701Repository(Schemas.CoreSchema);
            var result = rep.GetQueryListByEmpId(empId, qGroup);
            return result;
        }

        [WebGet]
        public IQueryable<F1903> GetF1903sByItemCodes(string gupCode, string custCode, string itemCodes)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            var itemCodeList = itemCodes.Split(',').ToList();
            return repo.InWithTrueAndCondition("ITEM_CODE", itemCodeList, x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
        }

        [WebGet]
        public IQueryable<F194716> GetF194716Datas(string gupCode, string custCode, string dcCode, string carPeriod, string delvNo, string carGup, string retailCode)
        {
            var repo = new F194716Repository(Schemas.CoreSchema);
            return repo.GetF194716Datas(gupCode, custCode, dcCode, carPeriod, delvNo, carGup, retailCode);
        }
        [WebGet]
        public IQueryable<F1919> GetDatasByCanToShip(string dcCode, string gupCode, string custCode)
        {
            var f1919Repo = new F1919Repository(Schemas.CoreSchema);
            return f1919Repo.GetDatasByCanToShip(dcCode, gupCode, custCode);
        }

        [WebGet]
        public IQueryable<F1903> GetF1903(string gupCode, string custCode, string itemCodes, string itemName, string itemSpec, string lType, string oriVnrCode)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
      return repo.GetF1903(gupCode, custCode, itemCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(), itemName, itemSpec, lType, oriVnrCode);
    }

        [WebGet]
        public IQueryable<F1903> GetF1903sBySerialNo(string gupCode, string custCode, string serialNo)
        {
            var repo = new F1903Repository(Schemas.CoreSchema);
            return repo.GetF1903sBySerialNo(gupCode, custCode, serialNo);
        }

        [WebGet]
        public IQueryable<F1980> GetAutoWarehourse(string dcCode)
        {
            var repo = new F1980Repository(Schemas.CoreSchema);
            return repo.GetAutoWarehourse(dcCode);
        }

        [WebGet]
        public IQueryable<F191206> GetF191206sResult(string dcCode, string pickFloor, string pkArea = null)
        {
            var repo = new F191206Repository(Schemas.CoreSchema);
            return repo.GetF191206sResult(dcCode, pickFloor, pkArea);
        }

        [WebGet]
        public IQueryable<F19120601> GetF19120601sSelectItem(string dcCode, string pkArea)
        {
            var repo = new F19120601Repository(Schemas.CoreSchema);
            return repo.GetF19120601sSelectItem(dcCode, pkArea);
        }

        [WebGet]
        public IQueryable<F19120602> GetF19120602s(string dcCode, string pkArea)
        {
            var repo = new F19120602Repository(Schemas.CoreSchema);
            return repo.GetF19120602s(dcCode, pkArea);
        }

		[WebGet]
		public IQueryable<F1946> GetWorkstationList(string dcCode, string workstationGroup, string workstationType, string workstationCode, string status)
		{
			var repo = new F1946Repository(Schemas.CoreSchema);
			return repo.GetWorkstationList(dcCode, workstationGroup, workstationType, workstationCode, status);
		}

        [WebGet]
        public IQueryable<F194501> GetF194501ByDcCode(String dcCode)
        {
            var repo = new F194501Repository(Schemas.CoreSchema);
            return repo.GetF194501ByDcCode(dcCode);
        }

    }
}
