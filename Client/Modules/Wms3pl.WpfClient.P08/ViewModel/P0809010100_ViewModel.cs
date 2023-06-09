using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using System.Windows.Media.Imaging;
using System.IO;


namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0809010100_ViewModel : InputViewModelBase
	{

		public P0809010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				GetDcCodes();
			}
		}
		#region 物流中心
		private List<NameValuePair<string>> _dcCodes;
		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				DataList = null;
				RaisePropertyChanged();
			}
		}
		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}
		#endregion
		#region 變數

		public class FILE_INFO
		{
			public string FILE_NAME { get; set; }
			public string FILE_PATH { get; set; }
		}

		private string _gupCode;
		public string GUP_CODE
		{
			get { return _gupCode; }
			set
			{
				_gupCode = value;
				RaisePropertyChanged();
			}
		}
		private string _custCode;
		public string CUST_CODE
		{
			get { return _custCode; }
			set
			{
				_custCode = value;
				RaisePropertyChanged();
			}
		}
		private string _dcCode;
		public string DC_CODE
		{
			get { return _dcCode; }
			set
			{
				_dcCode = value;
				RaisePropertyChanged();
			}
		}
		private DateTime _delvDate;
		public DateTime DELV_DATE
		{
			get { return _delvDate; }
			set
			{
				_delvDate = value;
				RaisePropertyChanged();
			}
		}
		private string _checkoutTime;
		public string CHECKOUT_TIME
		{
			get { return _checkoutTime; }
			set
			{
				_checkoutTime = value;
				RaisePropertyChanged();
			}
		}
		private string _pickTime;
		public string PICK_TIME
		{
			get { return _pickTime; }
			set
			{
				_pickTime = value;
				RaisePropertyChanged();
			}
		}
		private string _fileSource;
		public string FILE_SOURCE
		{
			get { return _fileSource; }
			set
			{
				_fileSource = value;
				RaisePropertyChanged("FILE_SOURCE");
			}
		}
		private List<FILE_INFO> _dataList;
		public List<FILE_INFO> DataList { get { return _dataList; } set { _dataList = value; RaisePropertyChanged("DataList"); } }

		private FILE_INFO _selectedData;
		public FILE_INFO SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
				{
					//ShowMessage("編輯狀態不可選取!");
					return;
				}
				else
				{
					_selectedData = value;
					FILE_SOURCE = value.FILE_PATH;
					RaisePropertyChanged("FILE_SOURCE");
					RaisePropertyChanged("ItemImageSource");
				}
			}
		}
		#endregion

		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				if (string.IsNullOrWhiteSpace(FILE_SOURCE) || !System.IO.File.Exists(FILE_SOURCE))
				{
					_itemImageSource = null;
					return _itemImageSource;
				}
				//if (_itemImageSource == null)
				//{
					BitmapImage image = new BitmapImage();
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.UriSource = new Uri(FILE_SOURCE);
					image.EndInit();
					_itemImageSource = image;
				//}
				return _itemImageSource;

			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{ 
					}
					);
			}
		}
		public string ShareFolderItemFiles
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "CarSeal", DC_CODE, GUP_CODE, CUST_CODE, DELV_DATE.Year.ToString() }); }
		}
		public string ImagePath
		{
			get { return ShareFolderItemFiles + DELV_DATE.ToString("yyyyMMdd") + CHECKOUT_TIME.Replace(":", ""); }
		}

		private void DoSearch()
		{
			if (System.IO.Directory.Exists(ImagePath))
			{
				List<FILE_INFO> tempDataList = new List<FILE_INFO>();
				foreach (var f in System.IO.Directory.GetFiles(ImagePath))
				{
					var fileData = new FILE_INFO
					{
						FILE_NAME = Path.GetFileName(f),
						FILE_PATH = f
					};
					tempDataList.Add(fileData);
				}
				DataList = tempDataList;
				if (tempDataList != null && tempDataList.Count != 0)
				{
					DataList = tempDataList;
					SelectedData = tempDataList.FirstOrDefault();
				}
				
			}
		}
		#endregion Search


	}
}
