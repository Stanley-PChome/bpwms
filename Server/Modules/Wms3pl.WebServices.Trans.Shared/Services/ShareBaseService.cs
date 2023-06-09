using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Trans.Shared.Services
{
    /// <summary>
    /// Base共用服務
    /// </summary>
    public class ShareBaseService
    {
        protected WmsTransaction _wmsTransation;
        protected string _dcCode;
        protected string _gupCode;
        protected string _custCode;

        //Repo
        protected F1910Repository _f1910Repo;
        protected F1903Repository _f1903Repo;
        protected F1908Repository _f1908Repo;
        private F0020Repository _f0020Repo;
        protected F1909Repository _f1909Repo;

        //Temp 暫存檢查重複資料庫存取
        protected List<F1910> _tempF1910List;
        protected List<F1903> _tempF1903List;
        protected List<F1908> _tempF1908List;
        protected List<F1909> _tempF1909List;
        protected List<F0020> _tempF0020List;

        //Log
        protected List<KeyValuePair<string, string>> _addLogList;

        //Sevices
        protected SharedService _sharedService;


        public ShareBaseService(string dcCode, string gupCode, string custCode, WmsTransaction wmsTransation = null)
        {
            _dcCode = dcCode;
            _gupCode = gupCode;
            _custCode = custCode;
            _wmsTransation = wmsTransation;
        }
        /// <summary>
        /// 取得貨主資料
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        public F1909 GetF1909(string gupCode, string custCode)
        {
            if (_tempF1909List == null)
                _tempF1909List = new List<F1909>();
            var f1909 = _tempF1909List.ToList().FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
            if (f1909 == null)
            {
                if (_f1909Repo == null)
                    _f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransation);

                f1909 = _f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).FirstOrDefault();
                if (f1909 != null)
                    _tempF1909List.Add(f1909);
            }
            return f1909;
        }
        /// <summary>
        /// 取得門市資料
        /// </summary>
        /// <param name="retailCode">門市編號</param>
        /// <param name="channel">通路編號</param>
        /// <returns>回傳門市資料</returns>
        public F1910 GetF1910(string retailCode)
        {
            var f1909 = GetF1909(_gupCode, _custCode);
            var custCode = _custCode;
            if (f1909 != null && f1909.ALLOWGUP_RETAILSHARE == "1")
                custCode = "0";

            if (_f1910Repo == null)
                _f1910Repo = new F1910Repository(Schemas.CoreSchema, _wmsTransation);

            if (_tempF1910List == null)
                _tempF1910List = new List<F1910>();

            var f1910 = _tempF1910List.FirstOrDefault(x => x.GUP_CODE == _gupCode && x.CUST_CODE == custCode && x.RETAIL_CODE == retailCode);
            if (f1910 == null)
            {
                f1910 = _f1910Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == _gupCode && x.CUST_CODE == custCode && x.RETAIL_CODE == retailCode).FirstOrDefault();
                if (f1910 != null)
                    _tempF1910List.Add(f1910);
            }
            return f1910;
        }

        /// <summary>
        /// 取得商品主檔清單
        /// </summary>
        /// <param name="itemCodes">品號清單</param>
        /// <returns>回傳商品主檔清單</returns>
        public List<F1903> GetF1903s(List<string> itemCodes)
        {
            if (_f1903Repo == null)
                _f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransation);

            if (_tempF1903List == null)
                _tempF1903List = new List<F1903>();

            var returnF1903s = _tempF1903List.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && itemCodes.Any(y => y == x.ITEM_CODE)).ToList();

            var noExistItemCodes = itemCodes.Except(_tempF1903List.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode).Select(x => x.ITEM_CODE)).ToList();
            if (noExistItemCodes.Any())
            {
                var f1903s = _f1903Repo.GetDatasByItems(_gupCode, _custCode, noExistItemCodes);
                returnF1903s.AddRange(f1903s);
                _tempF1903List.AddRange(f1903s);
            }
            return returnF1903s;
        }

        /// <summary>
        /// 取得廠商資料
        /// </summary>
        /// <param name="vnrCode">廠商編號</param>
        /// <returns>回傳廠商資料</returns>
        public F1908 GetF1908(string vnrCode)
        {

            var f1909 = GetF1909(_gupCode, _custCode);
            var custCode = _custCode;
            if (f1909 != null && f1909.ALLOWGUP_VNRSHARE == "1")
                custCode = "0";

            if (_f1908Repo == null)
                _f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransation);

            if (_tempF1908List == null)
                _tempF1908List = new List<F1908>();
            var f1908 = _tempF1908List.FirstOrDefault(x => x.GUP_CODE == _gupCode && x.CUST_CODE == custCode && x.VNR_CODE == vnrCode);
            if (f1908 == null)
            {
                f1908 = _f1908Repo.GetEnabledVnrData(_gupCode, custCode, vnrCode);
                if (f1908 != null)
                    _tempF1908List.Add(f1908);
            }
            return f1908;
        }

        /// <summary>
        /// 取得行事曆預設訊息
        /// </summary>
        /// <param name="msgNo">訊息編號</param>
        /// <returns></returns>
        public F0020 GetF0020(string msgNo)
        {
            if (_f0020Repo == null)
                _f0020Repo = new F0020Repository(Schemas.CoreSchema);

            if (_tempF0020List == null)
                _tempF0020List = new List<F0020>();

            var f0020 = _tempF0020List.FirstOrDefault(x => x.MSG_NO == msgNo);
            if (f0020 == null)
            {
                var f0020List = _f0020Repo.GetDatasBymsgNoKeyword(msgNo.Substring(0, 2)).ToList();
                _tempF0020List.AddRange(f0020List.Except(_tempF0020List));
                f0020 = _tempF0020List.FirstOrDefault(x => x.MSG_NO == msgNo);
            }
            return f0020;
        }

        /// <summary>
        /// 寫入Log紀錄
        /// </summary>
        /// <param name="msgNo">訊息編號</param>
        /// <param name="message">訊息內容</param>
        public void InsertLog(string msgNo, string message)
        {
            if (_addLogList == null)
                _addLogList = new List<KeyValuePair<string, string>>();
            _addLogList.Add(new KeyValuePair<string, string>(msgNo, message));
        }

        /// <summary>
        /// 取得所有Log訊息
        /// </summary>
        /// <returns></returns>
        public string GetAllLogMessage()
        {
            if (_addLogList == null)
                return string.Empty;
            else
                return string.Join(Environment.NewLine, _addLogList.Select(x => string.Format("{0}:{1}", x.Key, x.Value)));
        }
        /// <summary>
        /// 儲存Log
        /// </summary>
        protected void SaveLogToDb()
        {
            if (_sharedService == null)
                _sharedService = new SharedService(_wmsTransation);

            var group = _addLogList.GroupBy(x => x.Key);
            foreach (var item in group)
                _sharedService.AddMessagePool("9", _dcCode, _gupCode, _custCode, item.Key, string.Join(Environment.NewLine, item.Select(x => x.Value)), "", "0", "BP");
        }

    }
}
