using System.Linq;
using Wms3pl.WpfClient.Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.Datas.Schedule;

namespace Wms3pl.WpfClient.Services
{
	public partial class DbSettingsService : ISettingStorage
	{
		public DbSettingsService()
		{
			_proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "DbSettings");
		}

        F19Entities _proxy;

		public void Save(Wms3plSettings settings, string accountName)
		{
			var q = (from i in _proxy.PREFERENCEs
							 where i.EMP_ID == accountName
							 select i).FirstOrDefault();

			var formatter = new BinaryFormatter();
			var ms = new MemoryStream();
			formatter.Serialize(ms, settings);

			if (q == null)
			{
				//Insert
				var userpreference = new PREFERENCE()
				{
					EMP_ID = accountName,
					DATA = ms.ToArray()
				};

				_proxy.AddToPREFERENCEs(userpreference);
			}
			else
			{
				//Update
				q.DATA = ms.ToArray();
				_proxy.UpdateObject(q);
			}
			_proxy.SaveChanges();
		}



		public Wms3plSettings Load(string accountName)
		{
			var q = (from i in _proxy.PREFERENCEs
							 where i.EMP_ID == accountName
							 select i).FirstOrDefault();
			if (q != null)
			{
				var formatter = new BinaryFormatter();
				var ms = new MemoryStream(q.DATA);
				return formatter.Deserialize(ms) as Wms3plSettings;
			}
			else
			{
				return new Wms3plSettings();
			}

		}

		public void SaveDefaultLang(string lang)
		{

		}

		public string LoadDefaultLang()
		{
			return string.Empty;
		}
	}
}
