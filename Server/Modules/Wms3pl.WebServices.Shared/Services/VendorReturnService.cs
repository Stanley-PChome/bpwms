using Wms3pl.Datas.F16;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
    public class VendorReturnService
    {
        private WmsTransaction _wmsTransaction;

        public VendorReturnService(WmsTransaction wmsTransaction)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 取消廠商退貨單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="rtnVnrNo"></param>
        /// <param name="procFlag"></param>
        /// <returns></returns>
        public bool CancelNotProcessVnrReturn(string dcCode, string gupCode, string custCode, string rtnVnrNo, string procFlag)
        {
            bool result = false;

            var f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);

            var data = f160201Repo.GetData(dcCode, gupCode, custCode, rtnVnrNo);

            if (data != null)
            {
                if (data.STATUS == "0" || data.STATUS == "3")
                {
                    var importFlag = procFlag == "D" ? "2" : "1";

                    // 取消廠退單
                    f160201Repo.CancelNotProcessVnrReturn(dcCode, gupCode, custCode, rtnVnrNo, importFlag);
                    result = true;
                }
            }

            return result;
        }
    }
}
