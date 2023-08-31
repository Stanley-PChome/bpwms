using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared.TransApiServices
{
    public class TransApiBaseService
    {
        #region Service

        private CommonService _commonService;
        public CommonService CommonService
        {
            get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
            set { _commonService = value; }
        }

        #endregion Service

        /// <summary>
        /// 檢核資料筆數
        /// </summary>
        /// <param name="totalCnt">總筆數</param>
        /// <param name="cnt">資料筆數</param>
        /// <returns></returns>
        public bool CheckDataCount(int totalCnt, int cnt)
        {
            return totalCnt == cnt;
        }

        /// <summary>
        /// 取得出貨單號
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNos"></param>
        /// <returns></returns>
        public List<string> GetWmsOrderData(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
        {
            var f050801Repo = new F050801Repository(Schemas.CoreSchema);
            return f050801Repo.GetDatas(dcCode, gupCode, custCode, wmsOrdNos).Select(x => x.WMS_ORD_NO).ToList();
        }

        /// <summary>
        /// 檢核出貨單號是否存在
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="wmsOrdNo">出貨單號</param>
        /// <returns></returns>
        public bool CheckWmsOrderExist(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var f050801Repo = new F050801Repository(Schemas.CoreSchema);
            var data = f050801Repo.GetData(wmsOrdNo, gupCode, custCode, dcCode);
            return data != null;
        }

        /// <summary>
        /// 取得訊息內容
        /// </summary>
        /// <param name="msgNo">訊息代碼</param>
        /// <returns></returns>
        public string GetMsg(string msgNo)
        {
            return CommonService.GetMsg($"API{msgNo}");
        }
    }
}
