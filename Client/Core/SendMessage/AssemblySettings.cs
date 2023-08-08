using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SendMessage
{
	public class AssemblySettings
	{
		private KeyValueConfigurationCollection _settings;

		public AssemblySettings(Assembly asmb)
		{
			LoadSettings(asmb);
		}

		private void LoadSettings(Assembly asmb)
		{
			var config = ConfigurationManager.OpenExeConfiguration(asmb.Location);

			var section = (config.GetSection("appSettings") as AppSettingsSection);
			_settings = section.Settings;
		}

		public string this[string key]
		{
			get
			{
				return _settings[key].Value;
			}
		}
	}
}
