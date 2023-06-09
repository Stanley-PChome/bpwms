using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Entities
{
	[Serializable]
	[DataContract]
	[DataServiceKey("FileName")]

	public class DataImportFile
	{
		[DataMember]
		/// <summary>
		/// 檔案名稱(不含副檔名)
		/// </summary>
		public string FileName { get; set; }
		[DataMember]
		/// <summary>
		/// 檔案副檔名
		/// </summary>
		public string FileExtension { get; set; }
		[DataMember]
		/// <summary>
		/// 檔案內容
		/// </summary>
		public List<string> FileContent { get; set; }
		/// <summary>
		/// 檔案名稱前置詞
		/// </summary>
		public string FileStartTag {
			get {
				if(!string.IsNullOrEmpty(FileName))	
				{
					var startTag = string.Empty;	
					foreach(char c in FileName)
					{
						if (char.IsNumber(c))
							break;
						startTag += c;
					}
					return startTag;
				}
				return string.Empty;
			}
		}
		/// <summary>
		/// 檔案名稱(含副檔名)
		/// </summary>
		public string FileFullName { get { return string.Format("{0}{1}", FileName, FileExtension); } }
	}

	
}
