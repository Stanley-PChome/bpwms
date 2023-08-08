using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.P02.Services
{
    public static class SerialService
    {
        /// <summary>
        /// 取得進倉時要對應的Status狀態
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
			public static string GetStatusMessageForInWarehouse(string status, string functionCode)
        {
            if (string.IsNullOrEmpty(status) || status == "C1")
            {
                // Status為空或"C1", 表示為合格的序號
                return "";
            }
            else if (status == "00")
            {
                // Status為"00"表示無此序號
                return "無此序號";
            }
            else if (status != "C1")
            {
                // 已出貨, 已退貨等狀態
							return "序號狀態錯誤: " + GetStatusMessage(status, functionCode);
            }
            return "未知";
        }

        /// <summary>
        /// 依狀態編碼傳回說明文字
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
				public static string GetStatusMessage(string status, string functionCode)
        {
					var proxy = ConfigurationHelper.GetProxy<F00Entities>(false, functionCode);
					var data = proxy.F000904s.Where(x => x.TOPIC == "F2501" && x.SUBTOPIC == "STATUS" && x.VALUE == status && x.ISUSAGE == "1").FirstOrDefault();
					return (data == null ? string.Empty : data.NAME);
				}

				/// <summary>
				/// 判斷序號狀態是否符合. 需依照是否有進倉單號來判斷.
				/// </summary>
				/// <param name="status"></param>
				/// <param name="isHavingOrderNo"></param>
				/// <returns></returns>
				public static string GetStatusMessageForP91010301(string status, bool isHavingOrderNo, string functionCode)
				{
					status = status.ToUpper();

					if (isHavingOrderNo == false)
					{
						// 無進倉單號時, 狀態須為A1進貨
						if (status == "A1")
							return "";
						return "序號狀態錯誤: " + GetStatusMessage(status, functionCode);
					}
					else
					{
						// 有進倉單號時, 狀態須為A1進貨或是無此序號檔
						if (status == "A1" || status == "00")
							return "";
						return "序號狀態錯誤: " + GetStatusMessage(status, functionCode);
					}
				}
    }
}
