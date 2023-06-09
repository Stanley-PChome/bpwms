using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public enum UploadType
	{
		Add,
		Edit
	}
	public class P0809010200_ViewModel : InputViewModelBase
	{
	
		public Action<bool> DoUpload =  delegate { };
		private bool _isUploadSuccess { get; set; }
		#region property


		#region 最大可上傳數量
		private int _maxUploadCount;

		public int MaxUploadCount
		{
			get { return _maxUploadCount; }
			set
			{
				if (_maxUploadCount == value)
					return;
				Set(() => MaxUploadCount, ref _maxUploadCount, value);
			}
		}
		#endregion

		#region 欲上傳檔案清單
		private List<string> _filePathList;

		public List<string> FilePathList
		{
			get { return _filePathList; }
			set
			{
				if (_filePathList == value)
					return;
				Set(() => FilePathList, ref _filePathList, value);
			}
		}
		#endregion

		#region 物流中心
		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				if (_dcCode == value)
					return;
				Set(() => DcCode, ref _dcCode, value);
			}
		}
		#endregion

		#region 業主
		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				if (_gupCode == value)
					return;
				Set(() => GupCode, ref _gupCode, value);
			}
		}
		#endregion

		#region 貨主
		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				if (_custCode == value)
					return;
				Set(() => CustCode, ref _custCode, value);
			}
		}
		#endregion

		#region 批次日期
		private DateTime _delvDate;

		public DateTime DelvDate
		{
			get { return _delvDate; }
			set
			{
				if (_delvDate == value)
					return;
				Set(() => DelvDate, ref _delvDate, value);
			}
		}
		#endregion

		#region 出車時間
		private string _takeTime;

		public string TakeTime
		{
			get { return _takeTime; }
			set
			{
				if (_takeTime == value)
					return;
				Set(() => TakeTime, ref _takeTime, value);
			}
		}
		#endregion


		#region 配送商
		private string _allId;

		public string AllId
		{
			get { return _allId; }
			set
			{
				if (_allId == value)
					return;
				Set(() => AllId, ref _allId, value);
			}
		}
		#endregion
		

		#region 上傳類型
		private UploadType _uploadType;

		public UploadType UploadType
		{
			get { return _uploadType; }
			set
			{
				if (_uploadType == value)
					return;
				Set(() => UploadType, ref _uploadType, value);
			}
		}
		#endregion

		#region 檔案路徑
		private string _fileSource;

		public string FileSource
		{
			get { return _fileSource; }
			set
			{
				if (_fileSource == value)
					return;
				Set(() => FileSource, ref _fileSource, value);
			}
		}
		#endregion

		#region 預設路徑
		public string ShareFolderItemFiles
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "CarSeal", DcCode, GupCode, CustCode, DelvDate.Year.ToString() }); }
		}
		public string ImagePath
		{
			get { return ShareFolderItemFiles + DelvDate.ToString("yyyyMMdd") + TakeTime.Replace(":", ""); }
		}
		#endregion

		#region 圖檔

		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				if (string.IsNullOrWhiteSpace(FileSource) || !System.IO.File.Exists(FileSource))
				{
					_itemImageSource = null;
					return _itemImageSource;
				}
				var image = new BitmapImage();
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = new Uri(FileSource);
				image.EndInit();
				_itemImageSource = image;
				return _itemImageSource;

			}
		}

		#endregion


		#region 已上傳檔案清單
		private List<P0809010100_ViewModel.FILE_INFO> _dataList;

		public List<P0809010100_ViewModel.FILE_INFO> DataList
		{
			get { return _dataList; }
			set
			{
				if (_dataList == value)
					return;
				Set(() => DataList, ref _dataList, value);
			}
		}
		#endregion

		#region 目前選取的檔案
		private P0809010100_ViewModel.FILE_INFO _selectedData;

		public P0809010100_ViewModel.FILE_INFO SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData == value)
					return;
				Set(() => SelectedData, ref _selectedData, value);
				IsHasPhoto = _selectedData != null;
				FileSource = IsHasPhoto ? _selectedData.FILE_PATH : "";
				RaisePropertyChanged("FileSource");
				RaisePropertyChanged("ItemImageSource");
			}
		}
		#endregion


		#region 是否有圖檔
		private bool _isHasPhoto;

		public bool IsHasPhoto
		{
			get { return _isHasPhoto; }
			set
			{
				if (_isHasPhoto == value)
					return;
				Set(() => IsHasPhoto, ref _isHasPhoto, value);
			}
		}
		#endregion
		
		

		#endregion
		public P0809010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			if (Directory.Exists(ImagePath))
			{
				var tempDataList = Directory.GetFiles(ImagePath).Select(f => new P0809010100_ViewModel.FILE_INFO
				{
					FILE_NAME = Path.GetFileName(f), FILE_PATH = f
				}).ToList();
				DataList = tempDataList.OrderBy(o=>o.FILE_NAME).ToList();
				if (DataList.Any())
					SelectedData = DataList.FirstOrDefault();
			}
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query && DataList == null || DataList.Count<=MaxUploadCount,
					c => DoUpload(true)
					);
			}
		}

		private void DoAdd()
		{
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData!=null,
					c => DoUpload(false)
					);
			}
		}

		private void DoEdit()
		{
		}
		#endregion Edit

		#region Upload
		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isUploadSuccess =DoSaveImage(), () => true,
					c =>
					{
						if (_isUploadSuccess)
						{
							if (UploadType == UploadType.Edit)
							{
								var fileName = SelectedData.FILE_NAME;
								DoSearch();
								SelectedData = DataList.First(o => o.FILE_NAME == fileName);
							}
							else
							{
								var oldlist = DataList ?? new List<P0809010100_ViewModel.FILE_INFO>();
								DoSearch();
								var newList = DataList ?? new List<P0809010100_ViewModel.FILE_INFO>();
								SelectedData = newList[oldlist.Count];
							}
						}
							
					});
			}
		}

		private bool DoSaveImage()
		{
			_isUploadSuccess = false;

			if (!Directory.Exists(ImagePath))
				Directory.CreateDirectory(ImagePath);
			short fileNo;
			if (UploadType == UploadType.Add)
			{
				var list = DataList ?? new List<P0809010100_ViewModel.FILE_INFO>();
				fileNo = (list.Any()) ? short.Parse(list.OrderBy(o => o.FILE_NAME).Last().FILE_NAME.Split('_')[1].Replace(".jpg","")) : (short)0;
				fileNo++;
				string header = DateTime.Now.ToString("HHmmss");
				foreach (var p in FilePathList)
				{
					var newFileName = Path.Combine(ImagePath, string.Format("{0}_{1}_01.jpg", header, fileNo.ToString("00")));
					File.Copy(p, newFileName, true);
					fileNo++;
				}
			}
			else
			{
				var dr = DialogService.ShowMessage(Properties.Resources.P0809010000_ImageDirectoryExistMsg, WpfClient.Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
				if (dr == UILib.Services.DialogResponse.No)
					return false;
				var fileArray = SelectedData.FILE_NAME.Replace(".jpg", "").Split('_');
				var value = int.Parse(fileArray[2]) + 1;
				if (value >= 100)
					value = 1;
				fileArray[2] = value.ToString("00");
				var newFileName = Path.Combine(ImagePath, string.Join("_", fileArray)+".jpg");
				File.Delete(Path.Combine(ImagePath,SelectedData.FILE_NAME));
				File.Copy(FilePathList.First(), newFileName, true);
				SelectedData.FILE_NAME = string.Join("_", fileArray) + ".jpg";
			}

			var proxyEx = GetExProxy<P08ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateIsSeal")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("delvDate", DelvDate.ToString("yyyy/MM/dd"))
				.AddQueryExOption("takeTime", TakeTime)
				.AddQueryExOption("allId", AllId).ToList();
			if (result.First().IsSuccessed)
			{
				DialogService.ShowMessage(Properties.Resources.P0809010000_UpdateIsSealSuccess);
			}
			else
			{
				ShowWarningMessage(Properties.Resources.P0809010000_UpdateIsSealFail);
			}
			return true;
		}
		#endregion Upload

	}
}
