using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.Common;
using System.IO;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.Services
{
	public static class FileService
	{
		static F19Entities _proxyF19;
		static List<F190907> _tempF190907;
		static List<F190207> _tempF190207;
	  
		/// <summary>
		/// 取得貨主路徑設定資料
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="patchKey"></param>
		/// <returns></returns>
		private static F190907 GetF190907(string gupCode,string custCode,string patchKey)
		{
			if (_tempF190907 == null)
				_tempF190907 = new List<F190907>();
			var item = _tempF190907.FirstOrDefault(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PATH_KEY == patchKey);
			if (item == null)
			{
				item = _proxyF19.F190907s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PATH_KEY == patchKey).FirstOrDefault();
				if (item != null)
					_tempF190907.Add(item);
			}
			return item;
		}

		/// <summary>
		/// 連線資料夾
		/// </summary>
		/// <param name="f190907"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private static bool ConnetFolder(F190907 f190907, out string message)
		{
			switch (f190907.FILESOURCE_TYPE)
			{
				case "0": //本機路徑
					break;
				case "1": //網路磁碟機
					var diskPartition = f190907.PATH_ROOT.Split(':')[0];
					if (string.IsNullOrWhiteSpace(f190907.NETWORKPATH))
					{
						message = Properties.Resources.NotSetNetWorkDiskPath;
						return false;
					}
					if (string.IsNullOrWhiteSpace(f190907.NETWORKACCOUNT))
					{
						message = Properties.Resources.NotSetNetWorkDiskAccount;
						return false;
					}
					if (string.IsNullOrWhiteSpace(f190907.NETWORKPASSWORD))
					{
						message = Properties.Resources.NotSetNetWorkDiskPassword;
						return false;
					}
					//刪除網路磁碟連線
					if (!ProcessHelper.ExecuteCommand(string.Format("if exist {0}: net use {0}: /delete /Y", diskPartition)))
					{
						message = Properties.Resources.DeleteNetWorkDiskFailure;
						return false;
					}
					//建立網路磁碟連線
					if (!ProcessHelper.ExecuteCommand(string.Format("net use {0}: {1} {2} /user:{3}", diskPartition, f190907.NETWORKPATH, f190907.NETWORKPASSWORD, f190907.NETWORKACCOUNT)))
					{
						message = Properties.Resources.CreateNetWorkDistFailure;
						return false;
					}
					break;
			}
			if (!Directory.Exists(f190907.PATH_ROOT))
			{
				message = String.Format(Properties.Resources.PathNotExists, f190907.PATH_ROOT);
				return false;
			}
			message = string.Empty;
			return true;
		}

		#region 商品圖檔
		
		/// <summary>
		/// 取得商品圖檔路徑資料
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="imageNo"></param>
		/// <returns></returns>
		private static F190207 GetF190207(string gupCode, string itemCode,short imageNo, string custCode)
		{
			if (_tempF190207 == null)
				_tempF190207 = new List<F190207>();
			var item = _tempF190207.FirstOrDefault(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.IMAGE_NO == imageNo && x.CUST_CODE == custCode);
			if (item == null)
			{
				item = _proxyF19.F190207s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.IMAGE_NO == imageNo && x.CUST_CODE == custCode).FirstOrDefault();
				if (item != null)
					_tempF190207.AddRange(_tempF190207.Except(new List<F190207> { item }));
			}
			return item;
		}
		
		/// <summary>
		/// 取得貨主商品圖檔共用資料夾路徑
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public static string GetCustItemImageShareFolderPath(string gupCode,string custCode)
		{
			if (_proxyF19 == null)
				_proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, "FileService");
			var f190907 = GetF190907(gupCode, custCode, "ItemImagePath");
			if (f190907 == null)
				return string.Empty;
			if (string.IsNullOrWhiteSpace(f190907.PATH_ROOT))
				return string.Empty;
			var message = string.Empty;
			if (!ConnetFolder(f190907, out message))
				return string.Empty;
			return Path.Combine(f190907.PATH_ROOT, custCode);
		}

		/// <summary>
		/// 上傳商品圖檔
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="sourceFilePath">來源檔案路徑</param>
		/// <param name="message">回傳訊息</param>
		/// <param name="imageNo">商品圖檔流水號(null:代表新增)</param>
		/// <returns></returns>
		public static bool UpLoadItemImage(string gupCode,string custCode,string itemCode,  string sourceFilePath, out string message, short? imageNo = null)
		{
			if(_proxyF19 == null)
				_proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, "FileService");

			var f190907 = GetF190907( gupCode, custCode, "ItemImagePath");
			if (f190907 == null)
			{
				message = Properties.Resources.CustNotSetItemImagePath;
				return false;
			}
			if (string.IsNullOrWhiteSpace(f190907.PATH_ROOT))
			{
				message = Properties.Resources.ItemImageNotSetRootFolder;
				return false;
			}
			if (!ConnetFolder(f190907, out message))
				return false;
			var sourceFileInfo = new FileInfo(sourceFilePath);
			var itemDir = Path.Combine(f190907.PATH_ROOT, custCode, itemCode);
			if (!Directory.Exists(itemDir))
				Directory.CreateDirectory(itemDir);
			if (imageNo == null)
			{
				var datas = _proxyF19.F190207s.Where(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode).ToList();
				imageNo =  datas.Any() ? datas.Max(x => x.IMAGE_NO) : (short)0;
				imageNo += 1;
			}
			var serial = imageNo.ToString().PadLeft(3, '0');
			var itemFileName = Path.Combine(itemDir, string.Format("{0}{1}{2}", itemCode, serial, sourceFileInfo.Extension));
			File.Copy(sourceFilePath, itemFileName,true);
			var f190207 = GetF190207(gupCode, itemCode, imageNo??1, custCode);
			if(f190207 == null)
			{
				_proxyF19.AddToF190207s(new F190207
				{
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ITEM_CODE = itemCode,
					IMAGE_NO = imageNo ?? 1,
					IMAGE_PATH = itemFileName,
				});
			}
			else
			{
				f190207.IMAGE_PATH = itemFileName;
				_proxyF19.UpdateObject(f190207);
			}
			_proxyF19.SaveChanges();

			message = string.Empty;
			return true;
		}

		/// <summary>
		/// 取得商品圖檔
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="imageNo">商品圖檔流水號</param>
		/// <returns></returns>
		public static BitmapImage GetItemImage(string gupCode,string custCode, string itemCode, short imageNo = 1)
		{
			if (_proxyF19 == null)
				_proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, "FileService");

			var f190907 = GetF190907(gupCode, custCode, "ItemImagePath");
			if (f190907 == null)
				return null;
			string message = string.Empty;
			if (!ConnetFolder(f190907, out message))
				return null;
			var f190207 = GetF190207(gupCode, itemCode, imageNo, custCode);
			if (f190207 == null)
				return null;

			if (string.IsNullOrWhiteSpace(f190207.IMAGE_PATH) || !File.Exists(f190207.IMAGE_PATH))
				return null;
			return WmsImpersonation.Run<BitmapImage>(() =>
			{
				using (var filestream = new FileStream(f190207.IMAGE_PATH, FileMode.Open))
				{
					if (filestream.Length == 0) return null;

					var image = new BitmapImage();
					image.BeginInit();
					image.StreamSource = filestream;
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.EndInit();
					return image;
				}
			});

		}

		/// <summary>
		/// 刪除圖檔
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="imageNo">商品圖檔流水號(null:代表全部)</param>
		/// <param name="message">回傳訊息</param>
		/// <returns></returns>
		public static bool DeleteItemImage(string gupCode,string custCode,string itemCode,short? imageNo,out string message)
		{
			if (_proxyF19 == null)
				_proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, "FileService");
			var f190907 = GetF190907(gupCode, custCode, "ItemImagePath");
			if (f190907 == null)
			{
				message = Properties.Resources.CustNotSetItemImagePath;
				return false;
			}
			if (string.IsNullOrWhiteSpace(f190907.PATH_ROOT))
			{
				message = Properties.Resources.ItemImageNotSetRootFolder;
				return false;
			}
			if (!ConnetFolder(f190907, out message))
				return false;
			if(imageNo != null)
			{
				var f190207 = GetF190207(gupCode, itemCode, imageNo.Value, custCode);
				if (f190207 != null)
				{
					if (File.Exists(f190207.IMAGE_PATH))
						File.Delete(f190207.IMAGE_PATH);

					if (f190207.IMAGE_NO == 1)
					{
						var second = _proxyF19.F190207s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode && x.IMAGE_NO != imageNo).OrderBy(x => x.IMAGE_NO).FirstOrDefault();
						if (second != null)
						{
							var temp = _tempF190207.FirstOrDefault(x => x.GUP_CODE == second.GUP_CODE && x.ITEM_CODE == second.ITEM_CODE && x.IMAGE_NO == second.IMAGE_NO && x.CUST_CODE == second.CUST_CODE);
							if (temp != null)
								_tempF190207.Remove(f190207);
							var newFileNamePath = f190207.IMAGE_PATH.Replace(f190207.IMAGE_PATH.Split('.').Last(), second.IMAGE_PATH.Split('.').Last());
							if (File.Exists(second.IMAGE_PATH))
								File.Move(second.IMAGE_PATH, newFileNamePath);
							_proxyF19.DeleteObject(second);
							f190207.IMAGE_PATH = newFileNamePath;
							_proxyF19.UpdateObject(f190207);
						}
						else
						{
							_proxyF19.DeleteObject(f190207);
							_tempF190207.Remove(f190207);
						}
					}
					else
					{
						_proxyF19.DeleteObject(f190207);
						_tempF190207.Remove(f190207);
					}

					_proxyF19.SaveChanges();
				}
			}
			else
			{
				var datas = _proxyF19.F190207s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode).ToList();
				foreach (var item in datas)
					_proxyF19.DeleteObject(item);
				_tempF190207.RemoveAll(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode && x.CUST_CODE == custCode);
				_proxyF19.SaveChanges();
				var itemDir = Path.Combine(f190907.PATH_ROOT, custCode, itemCode);
				if (Directory.Exists(itemDir))
				{

					Directory.Delete(itemDir,true);
				}
			}

			message = string.Empty;
			return true;
		}
		/// <summary>
		/// 設定指定商品圖檔流水號為預設圖檔
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCode">品號</param>
		/// <param name="imageNo">商品圖檔流水號</param>
		/// <param name="message">回傳訊息</param>
		/// <returns></returns>
		public static bool SetDefaultItemImage(string gupCode,string custCode,string itemCode,short imageNo,out string message)
		{
			if (_proxyF19 == null)
				_proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false, "FileService");
			var f190907 = GetF190907(gupCode, custCode, "ItemImagePath");
			if (f190907 == null)
			{
				message = Properties.Resources.CustNotSetItemImagePath;
				return false;
			}
			if (string.IsNullOrWhiteSpace(f190907.PATH_ROOT))
			{
				message = Properties.Resources.ItemImageNotSetRootFolder;
				return false;
			}
			if (!ConnetFolder(f190907, out message))
				return false;
			var current = GetF190207(gupCode, itemCode, imageNo, custCode);
			//當這個已經是預設圖檔 就不異動
			if(current.IMAGE_NO == 1)
			{
				message = string.Empty;
				return true;
			}
			var itemDir = Path.Combine(f190907.PATH_ROOT, custCode, itemCode);
			//將第一筆流水號移到最後
			var first = GetF190207(gupCode, itemCode, 1, custCode);
			var datas = _proxyF19.F190207s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == itemCode).ToList();
			var newImageNo = datas.Any() ? datas.Max(x => x.IMAGE_NO) : (short)0;
			newImageNo += 1;
			var newImagePath = string.Format("{0}{1}.{2}", itemCode, newImageNo.ToString().PadLeft(3, '0'), first.IMAGE_PATH.Split('.').Last());
			newImagePath = Path.Combine(itemDir, newImagePath);
			_proxyF19.AddToF190207s(new F190207
			{
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ITEM_CODE = itemCode,
				IMAGE_NO = newImageNo,
				IMAGE_PATH = newImagePath,
			});
			if(File.Exists(first.IMAGE_PATH))
				File.Move(first.IMAGE_PATH, newImagePath);

			//將current指派的圖檔給此第一筆
			var newFileNamePath = first.IMAGE_PATH.Replace(first.IMAGE_PATH.Split('.').Last(), current.IMAGE_PATH.Split('.').Last());
			if (File.Exists(current.IMAGE_PATH))
				File.Move(current.IMAGE_PATH, newFileNamePath);
			first.IMAGE_PATH = newFileNamePath;
			_proxyF19.UpdateObject(first);
			_proxyF19.DeleteObject(current);
			_tempF190207.Remove(current);
			_proxyF19.SaveChanges();

			message = string.Empty;
			return true;
		}
		#endregion

	}
}
