using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ConsoleUtility.Utilities
{
	public class FtpUtility
	{
		public string Ip { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string RootPath { get; set; }
		/// <summary>
		/// 當下執行完的成功與否狀態
		/// </summary>
		public bool Status { get; set; }

		private int _timeout;
		/// <summary>
		/// 連線逾時時間(In Second)
		/// </summary>
		public int Timeout
		{
			get { return _timeout * 1000; }//FtpWebRequest採用毫秒，取得時轉換
			set { _timeout = value; }
		}

		private const char EndSlash = '/';
		/// <summary>
		/// ftp操作的整段流程
		/// </summary>
		public StringBuilder Logger { get; set; }
		public string ErrorMessage { get; set; }

		public bool UsePassive { get; set; }
		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="ftpIp">FTP的IP(ex: ftp://127.0.0.1:21/) </param>
		/// <param name="userName">登入帳號</param>
		/// <param name="password">登入密碼</param>
		/// <param name="rootPath">網址下指定資料夾  (通常多帳戶共用FTP，會在FTP網址底下切分資料夾做區隔)</param>
		/// <param name="timeout">連線逾時時間  (注意FTP Server有可能也有設定)</param>
		public FtpUtility(string ftpIp, string userName, string password, string rootPath = "", int timeout = 15, bool usePassive = false)
		{
			Ip = ftpIp;  
			UserName = userName;
			Password = password;
			RootPath = string.IsNullOrEmpty(rootPath) ? string.Empty : rootPath.TrimEnd(EndSlash) + EndSlash;
			Timeout = timeout;
			UsePassive = usePassive;
			Logger = new StringBuilder();
		}

		/// <summary>
		/// Format : "ftp://{ip}/"
		/// ex : ftp://127.0.0.1:21/FtpFolder/
		/// </summary>
		/// <param name="ip"></param>
		/// <returns></returns>
		public string FormatIp(string ip)
		{
			var ftpStart = string.Format("ftp://{0}{1}", ip, EndSlash);
			return ftpStart;
		}

		private Uri GetFtpIp()
		{
			return new Uri(Ip + RootPath);
		}


		/// <summary>
		/// 連線FTP
		/// </summary>
		/// <param name="ftpIp"></param>
		/// <param name="method">請用WebRequestMethods.Ftp</param>
		/// <returns></returns>
		public FtpWebRequest OpenRequest(Uri ftpIp, string method)
		{
			var ftpRequest = (FtpWebRequest)WebRequest.Create(ftpIp);
			ftpRequest.UseBinary = true;
			ftpRequest.Credentials = new NetworkCredential(UserName, Password);
			ftpRequest.Method = method;
			ftpRequest.Proxy = null;
			ftpRequest.KeepAlive = true;
			ftpRequest.UsePassive = UsePassive;
			return ftpRequest;
		}


		/// <summary>
		/// 取得FTP上面的檔案與文件夾
		/// ex: "123456.txt"、"AFolder"
		/// </summary>
		/// <param name="directory">指定FTP下的指定資料夾(ex: @"BFolder/CFolder/")</param>
		/// <returns></returns>
		public List<string> GetFileList(string directory = "")
		{
			var result = new List<string>();
			try
			{
				var uri = new Uri(GetFtpIp(), directory);
				Logger.AppendLine(string.Format("開始取得FTP({0})上的資料夾與檔案...", uri.AbsoluteUri));
				var request = OpenRequest(uri, WebRequestMethods.Ftp.ListDirectory);
				var response = request.GetResponse();
				var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

				string line = reader.ReadLine();
				while (line != null)
				{
					//只取檔名 
					//drwx------ 1 user group              0 Dec 08 09:56 新增資料夾
					//-rwx------ 1 user group             83 Dec 07 14:37 ITEM_ALERT-01-DEC-15 1400.csv
					if (string.IsNullOrWhiteSpace(line) == false)
					{
						var splitName = line.Split(':');

						if (splitName.Count() > 1)
						{
							result.Add(splitName[1].Substring(3));
						}
						else
						{
							result.Add(line);
						}
					}
					line = reader.ReadLine();
				}

				reader.Close();
				response.Close();
				Status = true;
				Logger.AppendLine(string.Format("取得成功！檔案明細: {0}", string.Join("、", result.ToArray())));
				return result;
			}
			catch (Exception ex)
			{
				Status = false;
				ErrorMessage = ex.Message;
				Logger.AppendLine("取得FTP的文件發生錯誤：" + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 取得檔案大小
		/// </summary>
		public long GetFileSize(string file)
		{
			try
			{
				var uri = new Uri(GetFtpIp(), file);
				Logger.AppendLine(string.Format("開始取得FTP檔案({0})大小...", uri.AbsoluteUri));
				var request = OpenRequest(uri, WebRequestMethods.Ftp.GetFileSize);
				var length = (long)request.GetResponse().ContentLength;
				Status = true;
				Logger.AppendLine(string.Format("取得成功，FTP檔案大小為{0}", length));
				return length;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				Logger.AppendLine("取得FTP檔案大小Error：" + Environment.NewLine + ex.Message);
				Status = false;
				return -1;
			}
		}

		/// <summary>
		/// 檢查FTP上是否已經有相同檔名
		/// </summary>
		public bool CheckFileExist(string folderPath, string fileName)
		{
			var uri = new Uri(GetFtpIp(), folderPath);
			Logger.AppendLine(string.Format("開始檢查FTP({0})上的資料夾與檔案是否存在...", uri.AbsoluteUri));
			var allFile = GetFileList(folderPath);
			var checkExist = allFile != null && allFile.Any(afile => fileName == afile);
			Logger.AppendLine(string.Format("檢查完成，檔案{0}存在", checkExist ? string.Empty : "不"));
			return checkExist;
		}

		/// <summary>
		/// FTP上傳
		/// </summary>
		/// <param name="localFilePath">本地端檔案路徑(要上傳的檔案)</param>
		/// <param name="newFileName">指定的新檔名(若無，則以原檔案名稱為主)</param>
		/// <param name="bufferSize">指定緩衝大小</param>
		public bool Upload(string localFilePath, string newFileName = "", int bufferSize = 4096)
		{
			try
			{
				var fileInfo = new FileInfo(localFilePath);
				var uploadFileName = (string.IsNullOrEmpty(newFileName) ? fileInfo.Name : newFileName);
				//while (CheckFileExist(uploadFileName))//檢查檔案存在則迴圈，重新產生新檔名(避免重複問題)
				//{
				//  uploadFileName = 
				//}
				var uri = new Uri(GetFtpIp() + uploadFileName);
				Logger.AppendLine(string.Format("開始上傳FTP檔案({0})...", uri.AbsoluteUri));
				var request = OpenRequest(uri, WebRequestMethods.Ftp.UploadFile);
				request.ContentLength = fileInfo.Length;
				byte[] buff = new byte[bufferSize];
				using (var fs = fileInfo.OpenRead())
				{
					using (var stream = request.GetRequestStream())
					{
						var contentLength = fs.Read(buff, 0, bufferSize);
						while (contentLength != 0)
						{
							stream.Write(buff, 0, contentLength);
							contentLength = fs.Read(buff, 0, bufferSize);
						}
					}
				}
				Logger.AppendLine("上傳FTP檔案成功");
				Status = true;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				Logger.AppendLine("上傳FTP檔案Error：" + ex.Message);
				Status = false;
			}
			return Status;
		}

		/// <summary>
		/// FTP下載
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="saveFilePath"></param>
		/// <param name="newFileName">Download 後變更檔名</param>
		/// <param name="bufferSize"></param>
		public bool Download(string fileName, string saveFilePath, string newFileName = "", int bufferSize = 4096)
		{
			try
			{
				var downloadFileName = (string.IsNullOrEmpty(newFileName) ? fileName : newFileName);
				var saveFullFilePath = Path.Combine(saveFilePath, downloadFileName);
				using (var outputStream = new FileStream(saveFullFilePath, FileMode.Create))
				{
					var uri = new Uri(GetFtpIp() + fileName);
					Logger.AppendLine(string.Format("開始下載FTP檔案({0})...", uri.AbsoluteUri));
					var request = OpenRequest(uri, WebRequestMethods.Ftp.DownloadFile);
					using (var response = (FtpWebResponse)request.GetResponse())
					{
						using (var ftpStream = response.GetResponseStream())
						{
							byte[] buffer = new byte[bufferSize];
							var readCount = ftpStream.Read(buffer, 0, bufferSize);
							while (readCount > 0)
							{
								outputStream.Write(buffer, 0, readCount);
								readCount = ftpStream.Read(buffer, 0, bufferSize);
							}
						}
					}
					Logger.AppendLine(string.Format("下載FTP檔案完成 {0}", saveFullFilePath));
				}
				return true;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				Logger.AppendLine("下載FTP檔案Error：" + ex.Message);
				return false;
			}
		}

		/// <summary>
		/// FTP 裡檔案搬移
		/// </summary>
		/// <param name="fileName">原始FTP 檔案名稱</param>
		/// <param name="targetPath">移至目的目錄位置 ex /FtpBackup/xxx.xlsx  (最前面加 / 根目錄)</param>
		/// /// <param name="newFileName">原始FTP 檔案</param>
		public bool MoveFtpFile(string fileName, string targetPath, string newFileName = null)
		{
			try
			{
				var moveFileName = string.IsNullOrEmpty(newFileName) ? fileName : newFileName;
				if (CheckFileExist(targetPath, moveFileName))
				{
					Logger.AppendLine("搬移FTP檔案失敗，放置位置已經存在相同檔案名稱");
					return false;
				}

				var uri = new Uri(GetFtpIp(), fileName);
				var request = OpenRequest(uri, WebRequestMethods.Ftp.Rename);
				request.RenameTo = targetPath + moveFileName;
				using (var response = request.GetResponse())
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				Logger.AppendLine("搬移FTP檔案Error：" + ex.Message);
				return false;
			}
		}

		public List<string> GetWindowFtpFileList(string directory="", List<string> filter = null)
		{
			var result = new List<string>();
			try
			{
				var uri = new Uri(GetFtpIp(), directory);
				Logger.AppendLine(string.Format("開始取得FTP({0})上的資料夾與檔案...", uri.AbsoluteUri));
				var request = OpenRequest(uri, WebRequestMethods.Ftp.ListDirectory);
				var response = request.GetResponse();
				var reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
				string line = reader.ReadLine();
				while (line != null)
				{
					if(filter==null || !filter.Any())
						result.Add(line);
					else
					{
						var isFind = true;
						foreach(var item in filter)
						{
							if (line.ToUpper().IndexOf(item.ToUpper()) == -1)
								isFind = false;
						}
						if (isFind)
							result.Add(line);
					}

					line = reader.ReadLine();
				}
				reader.Close();
				response.Close();
				Status = true;
				Logger.AppendLine(string.Format("取得成功！檔案明細: {0}", string.Join("、", result.ToArray())));
			}
			catch (Exception ex)
			{
				Status = false;
				ErrorMessage = ex.Message;
				Logger.AppendLine("取得FTP的文件發生錯誤：" + ex.Message);
			}
			return result;
		}

		public bool DeleteFile(string fileName)
		{
			try
			{
				var uri = new Uri(GetFtpIp(), fileName);
				var request = OpenRequest(uri, WebRequestMethods.Ftp.DeleteFile);
				Logger.AppendLine(string.Format("開始刪除FTP({0})上的檔案...", uri.AbsoluteUri));
				using (var response = request.GetResponse())
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
				Logger.AppendLine("刪除FTP檔案Error：" + ex.Message);
				return false;
			}
		}


	}

}
