using Wms3pl.Datas.F16;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
    public class ReturnService
    {
        private WmsTransaction _wmsTransaction;

        public ReturnService(WmsTransaction wmsTransaction)
        {
            _wmsTransaction = wmsTransaction;
        }

        /// <summary>
        /// 取消客戶退貨單
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="returnNo"></param>
        /// <param name="procFlag"></param>
        /// <returns></returns>
        public bool CancelNotProcessReturn(string dcCode, string gupCode, string custCode, string returnNo, string procFlag)
        {
            bool result = false;

            var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);

            var data = f161201Repo.GetData(dcCode, gupCode, custCode, returnNo);

            if (data != null)
            {
                if (data.STATUS == "0" || data.STATUS == "3")
                {
                    var importFlag = procFlag == "D" ? "2" : "1";

                    // 取消客戶退貨單
                    f161201Repo.CancelNotProcessReturn(dcCode, gupCode, custCode, returnNo, importFlag);
                    result = true;
                }
            }

            return result;
        }
    }
}
