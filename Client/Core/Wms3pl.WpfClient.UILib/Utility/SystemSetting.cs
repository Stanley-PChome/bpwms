using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib.Utility
{
	public class SystemSetting
	{
		public static void ApplySetting(string lang = null)
		{
			var info = Wms3plSession.Get<GlobalInfo>();
			if (info != null && string.IsNullOrEmpty(lang))
			{
				lang = info.Lang;
			}
			else if (info != null && info.Lang != lang)
			{
				info.Lang = lang;
				SetSystemLang(lang);
			}


			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(lang);
			CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lang);

			Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
		}

		public static string GetSystemLang()
		{
			//因取Default Lang尚未登入，所以不知UserId，所以只適合存在檔案，不適合存在DB
			//因此直接使用SettingsService而不透過DI用ISettingStorage
			var settingsService = new SettingsService();
			var lang = settingsService.LoadDefaultLang();
			if (string.IsNullOrEmpty(lang))
				lang = CultureInfo.CurrentCulture.Name;

			if (lang.ToUpper() == "ZH-HANT" || lang.ToUpper() =="ZH-TW")
				lang = "zh-TW";
			else if (lang.ToUpper() == "ZH-HANS" || lang.ToUpper() == "ZH-CN")
				lang = "zh-CN";

			var info = Wms3plSession.Get<GlobalInfo>();
			if (info == null) info = new GlobalInfo();
			info.Lang = lang;
			Wms3plSession.Set<GlobalInfo>(info);

			return lang;
		}

		private static void SetSystemLang(string lang)
		{
			//因取Default Lang尚未登入，所以不知UserId，所以只適合存在檔案，不適合存在DB
			//因此直接使用SettingsService而不透過DI用ISettingStorage
			var settingsService = new SettingsService();
			settingsService.SaveDefaultLang(lang);
		}
	}
}
