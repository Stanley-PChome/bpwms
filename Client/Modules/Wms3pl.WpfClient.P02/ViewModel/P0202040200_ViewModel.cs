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
using Wms3pl.WpfClient.DataServices.F02DataService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202040200_ViewModel : InputViewModelBase
	{

		public P0202040200_ViewModel()
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

		private string _rtNo;
		public string RtNo
		{
			get { return _rtNo; }
			set
			{
				_rtNo = value;
				RaisePropertyChanged("RtNo");
			}
		}
		private string _purchaseNo;
		public string PurchaseNo
		{
			get { return _purchaseNo; }
			set
			{
				_purchaseNo = value;
				RaisePropertyChanged("PurchaseNo");
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

				_selectedData = value;
				FILE_SOURCE = value.FILE_PATH;
				RaisePropertyChanged("FILE_SOURCE");
				RaisePropertyChanged("ItemImageSource");

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


		private void DoSearch()
		{
			string tmpImagePath = string.Empty;
			char[] delimit = new char[] { '\\' };
			List<FILE_INFO> list = ExistData();
			if (list.Any())
			{
				string[] tmpfilePath = list.FirstOrDefault().FILE_NAME.Split(delimit);
				tmpImagePath = list.FirstOrDefault().FILE_NAME.Replace(tmpfilePath.Last().ToString(), " ");

				if (System.IO.Directory.Exists(tmpImagePath.Trim()))
				{
					List<FILE_INFO> tempDataList =new List<FILE_INFO>();
					foreach (var f in list)
					{
						string fileName = f.FILE_NAME.Split(delimit).Last();
						var fileData = new FILE_INFO
						{
							FILE_NAME = fileName,
							FILE_PATH = f.FILE_PATH
						};
						tempDataList.Add(fileData);
					}
					DataList = tempDataList;
					//DataList = list;
					SelectedData = DataList.FirstOrDefault();
				}
			}
		}
		#endregion Search
		private List<FILE_INFO> ExistData()
		{
			var proxy = GetProxy<F02Entities>();
			List<F02020106> tmpF02020106 = proxy.F02020106s.ToList();
			List<F02020105> tmpF02020105 = proxy.F02020105s.Where(a => a.DC_CODE == DC_CODE && a.GUP_CODE == GUP_CODE && a.PURCHASE_NO == PurchaseNo && a.RT_NO == RtNo).ToList();
			var tmpFileData = (from i in tmpF02020105
							   join j in tmpF02020106 on i.UPLOAD_TYPE equals j.UPLOAD_TYPE
							   select new FILE_INFO()
							   {
								   FILE_NAME = i.UPLOAD_S_PATH,
								   FILE_PATH = i.UPLOAD_S_PATH
							   }).ToList();
			return tmpFileData;
		}
	}
}
