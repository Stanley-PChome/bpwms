using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Seagull.BarTender.Print;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Common;
using System.IO;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.LabelPrinter.Bartender
{
	public class LabelPrintHelper
	{
		private string bartenderIp = FileHelper.BartenderLicenseIp;
		private static Engine _btEngine;
		private static List<LabelFormatDocument> _btFormatDocs = new List<LabelFormatDocument>();
		private string _functionCode;

		public LabelPrintHelper(string functionCode)
		{
			_functionCode = functionCode;
		}
		public static void IniLabelPrint()
		{
			if (_btEngine == null)
			{
				_btEngine = new Engine();
				_btEngine.Start();
			}
		}
		
		public static void Dispose()
		{
			if (_btEngine != null)
			{
				_btEngine.Stop();
				_btEngine = null;
			}			
		}

		private ExecuteResult GetLabelingName(string dcCode,ref string deviceName)
		{
			var result = new ExecuteResult() { IsSuccessed = true };
			var proxy = ConfigurationHelper.GetProxy<F91Entities>(false, _functionCode);
			var device = proxy.F910501s.Where(
				n =>
					(n.DC_CODE == dcCode || string.IsNullOrEmpty(dcCode)) &&
					n.DEVICE_IP == Wms3plSession.Get<GlobalInfo>().ClientIp).FirstOrDefault();
			if (device != null && !string.IsNullOrEmpty(device.LABELING))
			{
				deviceName = device.LABELING;
			}
			else
			{
				result.IsSuccessed = false;
				result.Message = "查無已設定標籤機";
			}
			return result;
		}

		/// <summary>
		/// 列印標籤(無指定標籤機)
		/// </summary>
		/// <param name="labelItem">單個標籤物件</param>
		/// <param name="dcCode">物流中心(可為空值)</param>
		/// <param name="page">列印張數</param>
		/// <returns></returns>
		public ExecuteResult DoPrintNoDevice(LableItem labelItem, string dcCode, int page = 1)
		{
			var deviceName = string.Empty;
			var result = GetLabelingName(dcCode, ref deviceName);
			if (result.IsSuccessed)
				result = DoPrint(labelItem, deviceName, page);
			return result;
		}

		/// <summary>
		/// 列印標籤(無指定標籤機)
		/// </summary>
		/// <param name="labelItem">標籤物件集合</param>
		/// <param name="dcCode">物流中心(可為空值)</param>
		/// <param name="page">列印張數</param>
		/// <returns></returns>
		public ExecuteResult DoPrintNoDevice(List<LableItem> labelItem, string dcCode, int page = 1)
		{
			var deviceName = string.Empty;
			var result = GetLabelingName(dcCode,ref deviceName);
			if (result.IsSuccessed)
				result = DoPrint(labelItem, deviceName, page);
		
			return result;
		}

		/// <summary>
		/// 列印標籤	
		/// </summary>
		/// <param name="labelItem">單個標籤物件</param>
		/// <param name="printerName">標籤機名稱</param>
		/// <param name="page">列印張數</param>
		/// <returns></returns>
		public ExecuteResult DoPrint(LableItem labelItem, string printerName, int page = 1)
		{
			var labelItems = new List<LableItem>(){labelItem};
			return DoPrint(labelItems, printerName, page);
		}


		/// <summary>
		/// 列印標籤	
		/// </summary>
		/// <param name="labelItems">多個標籤物件</param>
		/// <param name="printerName">標籤機名稱</param>
		/// <param name="page">列印張數</param>
		/// <returns></returns>
		public ExecuteResult DoPrint(List<LableItem> labelItems, string printerName, int page = 1)
		{
			StartLog();
			var result = DoOpenFile(labelItems);
			if (!result.IsSuccessed)
				return result;

			foreach (var label in labelItems)
			{
				var btFormat = _btFormatDocs.First(n => n.Title == label.LableCode + ".btw");
				if (btFormat == null)
				{
					result.Message = "查無標籤檔案";
					result.IsSuccessed = false;
					return result;
				}
				//設定列印張數
				btFormat.PrintSetup.IdenticalCopiesOfLabel = page;

				//設定License
				btFormat.PrintSetup.PrintToFileLicense = bartenderIp;

				//設定條碼機
				btFormat.PrintSetup.PrinterName = printerName;

				//傳值到標籤檔內
				var printDataList = TransPrintData(label);
				foreach (var data in printDataList)
				{
					if (btFormat.SubStrings.Any(n => n.Name == data.Name) && !string.IsNullOrWhiteSpace(data.Value))
						btFormat.SubStrings[data.Name].Value = data.Value;
					else if (btFormat.SubStrings.Any(n => n.Name == data.Name))
						btFormat.SubStrings[data.Name].Value = string.Empty;
				}
				RecordLog("StartPrint");
				// Print the label 
				var resultPrint = btFormat.Print();
				RecordLog("EndPrint");
				if (resultPrint != Result.Success)
				{
					result.Message = "列印回傳失敗";
					result.IsSuccessed = false;
					return result;
				}
			}

			return result;
		}

		public ExecuteResult DoOpenFile(List<LableItem> labelItems)
		{
			var result = new ExecuteResult() {IsSuccessed = true};
			
			foreach (var label in labelItems)
			{
				if (label == null || string.IsNullOrEmpty(label.LableCode))
				{
					result.Message = "無標籤設定檔資料";
					result.IsSuccessed = false;
					return result;
				}

				//取得列印標籤格式檔路徑
				var mainDirectory = FileHelper.ShareFolderLabel;
#if DEBUG				
				mainDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Label");
#endif				

				//列印	 檔名為 LabelCode	
				var fileName = string.Format("{0}.btw", label.LableCode);
				//標籤不重複開啟
				if (_btFormatDocs.Any(n=>n.Title == fileName))
					break;

				//標籤檔存在才呼叫列印
				var filePath = Path.Combine(mainDirectory, fileName);				
				if (!File.Exists(filePath))
				{
					result.Message = "無標籤檔";
					result.IsSuccessed = false;
					return result;
				}

				//載入標籤檔
				IniLabelPrint();
				_btFormatDocs = _btFormatDocs ?? new List<LabelFormatDocument>();
				_btFormatDocs.Add(_btEngine.Documents.Open(filePath));
				RecordLog(filePath);
			}
			return result;
		}

		private IEnumerable<NameValuePair<string>> TransPrintData(LableItem labelItems)
		{
			foreach (var item in labelItems.GetType().GetProperties())
			{
				//設定 Key Value
				if (item.PropertyType == typeof (List<string>))
				{
					//list盒號
					var listStrig = (List<string>) item.GetValue(labelItems, null);
					if (listStrig == null)
						continue;

					var i = 1;
					foreach (var s in listStrig)
					{
						yield return new NameValuePair<string>()
						{
							Name = item.Name + i.ToString().PadLeft(2, '0'),
							Value = s
						};
						i++;
					}
				}
				else
				{
					yield return new NameValuePair<string>()
					{
						Name = item.Name,
						Value = (item.GetValue(labelItems) == null) ? "" : item.GetValue(labelItems).ToString()
					};
				}
			}
		}

		static Stopwatch _sw = new Stopwatch();

		[Conditional("DEBUG")]
		private void StartLog()
		{
			_sw.Reset();
			_sw = Stopwatch.StartNew();
		}

		[Conditional("DEBUG")]
		private void RecordLog(string memo)
		{
			_sw.Stop();
			var ms = _sw.ElapsedMilliseconds;
			Debug.WriteLine(@"{0}:花費 {1}毫秒", memo, ms);
			_sw.Start();
		}

		[Conditional("DEBUG")]
		private void StopLog()
		{
			_sw.Stop();
			_sw.Reset();
		}
	}
}
