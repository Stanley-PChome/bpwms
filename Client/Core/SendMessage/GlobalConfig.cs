using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SendMessage
{
	public class GlobalConfig
	{
		/*暫時保留*/
		/*注意: 一般發行專案時，參考的DLL的Config是不會一起發布，此處必須手動處理其參考Config的檔案至發行資料夾下*/
		/*註記: 此處不寫成靜態，是因為可能一次需要大量讀取app資訊，此時會浪費效能在反覆讀取dll資料 */
		private AssemblySettings _settings = new AssemblySettings(Assembly.GetExecutingAssembly());

		/// <summary>
		/// 簡訊Service網址
		/// </summary>
		public string SmsServiceLink
		{
			get
			{
				return _settings["SMSServiceLink"];
			}
		}


	}
}
