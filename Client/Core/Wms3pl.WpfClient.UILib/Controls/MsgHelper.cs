using System;

namespace Wms3pl.WpfClient.UILib.Controls
{
    public static class MsgHelper
    {
        /// <summary>
        /// 將\r\n或\n轉為換行
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string ConvertNewLine(string msg)
        {
            msg = msg.Replace("\\r\\n", Environment.NewLine);
            msg = msg.Replace("\\n", Environment.NewLine);
            return msg;
        }
    }
}
