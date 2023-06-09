using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUtility.Utilities
{
	public class ZipUtilities
	{

		public bool FileZip(List<string> fileList, string targetPath, string targetFileName, string filePassword)
		{
			try
			{


				//若不指定目的與目的檔名則取第一個 List 當作目的檔名
				if (!string.IsNullOrEmpty(fileList.FirstOrDefault()) && string.IsNullOrEmpty(targetPath))
				{
					targetPath = Path.GetDirectoryName(fileList.FirstOrDefault());
					targetFileName = Path.GetFileNameWithoutExtension(fileList.FirstOrDefault());
				}

				using (ZipFile dotZip = new ZipFile())
				{
					if (!string.IsNullOrEmpty(filePassword))
						dotZip.Password = filePassword;

					foreach (var item in fileList)
					{
						if (File.Exists(item.ToString()))
							dotZip.AddFile(item.ToString(), "");
					}

					dotZip.Save(string.Format(@"{0}.zip", Path.Combine(targetPath, targetFileName)));
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool fileUnZip(string filePath, string targetPath, string filePassword)
		{
			//解壓縮
			using (Ionic.Zip.ZipFile DotZip = Ionic.Zip.ZipFile.Read(filePath))
			{
				if (!string.IsNullOrEmpty(filePassword))
					DotZip.Password = filePassword;

				DotZip.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);  //解壓縮路徑  
				var result = DotZip.EntryFileNames.ToList();
			}

			return true;

		}
	}
}
