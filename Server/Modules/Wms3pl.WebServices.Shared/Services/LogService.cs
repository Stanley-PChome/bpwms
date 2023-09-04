using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared.Services
{
    public class LogService
    {
        private bool _isSetLog;
        private string _logFilePath;
        private string _fileName;

        private CommonService _CommonService;
        public CommonService CommonService
        {
			    get { return _CommonService == null ? _CommonService = new CommonService() : _CommonService; }
			    set { _CommonService = value; }
		    }

        public LogService(string fileName, CommonService commonService = null)
        {
            CommonService = commonService;
            _isSetLog = CommonService.GetSysGlobalValue("IsWriteLog") == "1";
						_logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
            _fileName = fileName;
        }

        #region Log
        public async Task Log(string message, bool isShowDatetime = true)
        {
            if (_isSetLog)
            {
                if (!Directory.Exists(_logFilePath))
                    Directory.CreateDirectory(_logFilePath);

                var fileFullName = Path.Combine(_logFilePath, $"{_fileName}.txt");

                using (var sw = new StreamWriter(fileFullName, true, Encoding.GetEncoding(950))) //BIG5
                   await sw.WriteLineAsync(string.Format("{0} {1}", isShowDatetime ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.FFF") : string.Empty, message));
            }
        }

        public string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            if (_isSetLog)
            {
                string dateDiff = null;
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = " 耗時 " + ts.Seconds.ToString() + "." + ts.Milliseconds.ToString().PadLeft(3, '0') + "毫秒";
                return dateDiff;
            }
            return null;
        }
        #endregion
    }
}
