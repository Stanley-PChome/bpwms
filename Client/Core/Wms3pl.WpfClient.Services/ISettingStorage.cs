using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.WpfClient.Services
{
  public interface ISettingStorage
  {
    void Save(Wms3plSettings settings, string accountName);
    Wms3plSettings Load(string accountName);
		void SaveDefaultLang(string lang);
		string LoadDefaultLang();
	}
}
