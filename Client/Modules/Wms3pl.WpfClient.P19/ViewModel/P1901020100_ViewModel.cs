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
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Windows;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.P19.Services;
using System.IO;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public enum uploadType
	{
		Add,
		Edit
	}
	public partial class P1901020100_ViewModel : InputViewModelBase
	{
		public Action AddImage = delegate { };
		public Action UpdImage = delegate { };
		public uploadType UpType { get; set; }

		private string _itemCode;
		public string ItemCode { get { return _itemCode; } set { _itemCode = value; RaisePropertyChanged(); } }

		private string _imageNo;
		public string ImageNo { get { return _imageNo; } set { _imageNo = value; RaisePropertyChanged(); } }

		private string _gupCode;
		public string GupCode { get { return _gupCode; } set { _gupCode = value; RaisePropertyChanged(); } }
		private string _custCode;
		public string CustCode { get { return _custCode = Wms3plSession.Get<GlobalInfo>().CustCode; } set { Set(ref _custCode); } }

		private string _crtYear;
		public string CrtYear { get { return _crtYear; } set { _crtYear = value; RaisePropertyChanged(); } }

		#region 商品清單
		/// <summary>
		/// 商品主檔清單
		/// </summary>
		private List<F190207> _records;
		public List<F190207> Records { get { return _records; } set { _records = value; RaisePropertyChanged(); } }

		private F190207 _selectedData;
		public F190207 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _isHasPhoto = Visibility.Collapsed;
		public Visibility IsHasPhoto { get { return _isHasPhoto; } set { _isHasPhoto = value; RaisePropertyChanged(); } }
		#endregion

		#region Form - 圖檔名稱/ ShareFolder路徑
		private Int16 _maxImageNo = 0;
		public Int16 MaxImageNo { get { return _maxImageNo; } set { _maxImageNo = value; } }
		private string _fileName = string.Empty;
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}
		public bool IsImageUploaded
		{
			get { return _itemImageSource != null; }
		}
		private string _imagePathForDisplay = string.Empty;
		/// <summary>
		/// 取得圖檔路徑, 用來呈現在Image上
		/// 這裡不取資料庫裡的IMAGE_PATH, 因為位置可能變更, 所以還是抓APP SETTING的路徑為主
		/// </summary>
		public string ImagePathForDisplay
		{
			get { return _imagePathForDisplay; }
			set { _imagePathForDisplay = value; RaisePropertyChanged("ImagePathForDisplay"); RaisePropertyChanged("ItemImageSource"); RaisePropertyChanged("IsImageUploaded"); }
		}
	
		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				return _itemImageSource;
			}
			set
			{
				if (ItemCode == null || string.IsNullOrWhiteSpace(ItemCode)) _itemImageSource = null;
				else
				{
					if (string.IsNullOrWhiteSpace(ImageNo))
						_itemImageSource = null;
					else
						_itemImageSource = FileService.GetItemImage(GupCode, CustCode, ItemCode, short.Parse(ImageNo));
				}
				RaisePropertyChanged("ItemImageSource");
			}
		}
		#endregion


		public P1901020100_ViewModel()
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
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			if (Records == null) return;
			SelectedData = Records.FirstOrDefault();
			RefreshImage();
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxy = GetProxy<F19Entities>();
			var f190207s = proxy.F190207s.Where(x => x.GUP_CODE == GupCode && x.ITEM_CODE == ItemCode && x.CUST_CODE == CustCode).ToList();
			if (f190207s == null || !f190207s.Any())
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901020100_NoItemImage, Title = Resources.Resources.Information });
				IsHasPhoto = Visibility.Collapsed;
				MaxImageNo = 0;
				_itemImageSource = null;
				Records = null;
				ImagePathForDisplay = string.Empty;
				RaisePropertyChanged("IsImageUploaded");
				RaisePropertyChanged("ImagePathForDisplay");
				RaisePropertyChanged("ItemImageSource");
				return;
			}

			Records = f190207s.OrderBy(x => x.IMAGE_NO).ToList();
			IsHasPhoto = Visibility.Visible;
			MaxImageNo = Records.Max(x => x.IMAGE_NO);
		}
		#endregion Search

		#region 商品圖檔上傳
		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSaveImage(),
					() =>
					{
						// 如果ISUPLOAD = 0, 表示未上傳
						return UserOperateMode == OperateMode.Edit && !IsImageUploaded && SelectedData != null;
					},
					o =>
					{
						DoSearchComplete();
					}
				);
			}
		}
		/// <summary>
		/// 圖檔上傳, 並回傳檔案名稱 (ITEMCODE + NO)
		/// 圖檔上傳流程是先選擇檔案, 然後寫入資料庫, 再複製檔案到目的地
		/// </summary>
		/// <returns></returns>
		public bool DoSaveImage()
		{
			if (UpType == uploadType.Edit && SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901020100_ChooseImage, Title = Resources.Resources.Information });
				return false;
			}
			var OutMsg = string.Empty;
			short? imageNo = (UpType == uploadType.Add) ? (short?)null : SelectedData.IMAGE_NO;
			var result = FileService.UpLoadItemImage(_gupCode,CustCode,ItemCode,FileName,out OutMsg, imageNo);
			if (!result)
			{
				ShowMessage(new MessagesStruct() { Message = OutMsg, Title = Resources.Resources.Information });
				return false;
			}
			FileName = string.Empty;
			if (result) DoSearch();
			return result;
		}

		/// <summary>
		/// 上傳後, 或是載入時, 更新圖檔的顯示
		/// </summary>
		public void RefreshImage()
		{
			_itemImageSource = null;

			ImageNo = SelectedData.IMAGE_NO.ToString().PadLeft(3, '0');
			ItemImageSource = null;

			RaisePropertyChanged("IsImageUploaded");
			RaisePropertyChanged("ImagePathForDisplay");
			RaisePropertyChanged("ItemImageSource");
		}
		#endregion

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query,
					o => { AddImage(); }
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
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData !=null,
					o => { UpdImage(); }
					);
			}
		}

		private void DoEdit()
		{
			
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
					o => DoSearchComplete()
					);
			}
		}

		private void DoDelete()
		{
			var message = string.Empty;
			if (!FileService.DeleteItemImage(SelectedData.GUP_CODE, CustCode, SelectedData.ITEM_CODE, SelectedData.IMAGE_NO, out message))
				ShowWarningMessage(message);

			//重新查詢
			DoSearch();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save

		#region SetDefault
		public ICommand SetDefaultCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSetDefault(), () => (UserOperateMode == OperateMode.Query && SelectedData != null && !this.IsBusy),
					o => DoSearchComplete()
					);
			}
		}

		private void DoSetDefault()
		{
			var message = string.Empty;
			if (!FileService.SetDefaultItemImage(SelectedData.GUP_CODE, CustCode, SelectedData.ITEM_CODE, SelectedData.IMAGE_NO, out message))
				ShowWarningMessage(message);
			
			//重新查詢
			DoSearch();
		}
		#endregion SetDefault


	}
}
