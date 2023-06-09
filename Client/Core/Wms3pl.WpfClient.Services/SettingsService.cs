using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;

namespace Wms3pl.WpfClient.Services
{
  public partial class SettingsService : ISettingStorage
  {
    private const string SmartSettingsName = "SmartSettings";
		private const string LangSettingsName = "LangSettings";

		public void Save(Wms3plSettings settings, string accountName)
    {
      //http://msdn.microsoft.com/en-us/library/3ak841sy.aspx
      // Isolation storage 的位置放在 C:\Users\userName\AppData\Local\IsolatedStorage\ 下
      var f = IsolatedStorageFile.GetUserStoreForAssembly();
      var filename = string.Format("{0}_{1}", accountName, SmartSettingsName);
      using (var stream = new IsolatedStorageFileStream(filename, FileMode.Create, f))
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, settings);
      }
    }

    public Wms3plSettings Load(string accountName)
    {
      var f = IsolatedStorageFile.GetUserStoreForAssembly();
      var filename = string.Format("{0}_{1}", accountName, SmartSettingsName);
      using (var stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, f))
      {
        try
        {
          var formatter = new BinaryFormatter();
          return formatter.Deserialize(stream) as Wms3plSettings;
        }
        catch
        {
          return new Wms3plSettings();
        }
      }
    }

		public void SaveDefaultLang(string lang)
		{
			var f = IsolatedStorageFile.GetUserStoreForAssembly();
			var filename = LangSettingsName;
			using (var stream = new IsolatedStorageFileStream(filename, FileMode.Create, f))
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, lang);
			}
		}

		public string LoadDefaultLang()
		{
			var f = IsolatedStorageFile.GetUserStoreForAssembly();
			var filename = LangSettingsName;
			using (var stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, f))
			{
				try
				{
					var formatter = new BinaryFormatter();
					return formatter.Deserialize(stream) as string;
				}
				catch
				{
					return string.Empty;
				}
			}
		}
	}
}