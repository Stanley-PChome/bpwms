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
using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
//using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using System.Data;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.IO;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030300_ViewModel : InputViewModelBase
	{
		public Action AfterUpload = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();

			}
		}

		private void InitControls()
		{
			UserOperateMode = OperateMode.Edit;
		}

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
		}
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
		}
		#endregion
		#region Form - 單號
		private string _purchaseNo = string.Empty;
		public string PurchaseNo
		{
			get { return _purchaseNo; }
			set { _purchaseNo = value; }
		}
		private string _rtNo = string.Empty;
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; }
		}
		#endregion
		#region Data - 檔案清單
		private ObservableCollection<SelectionItem<FileUploadData>> _dgList = new ObservableCollection<SelectionItem<FileUploadData>>();
		public ObservableCollection<SelectionItem<FileUploadData>> DgList
		{
			get { return _dgList; }
			set { _dgList = value; RaisePropertyChanged("DgList"); }
		}
		private SelectionItem<FileUploadData> _selectedItem;
		public SelectionItem<FileUploadData> SelectedItem
		{
			get { return _selectedItem; }
			set { _selectedItem = value; }
		}
		public string ShareFolderItemFiles
		{
			get { return FileHelper.GetShareFolderPath(new string[] { "RT", SelectedDc, this._gupCode, this._custCode, RtNo.Substring(0, 4), RtNo }); }
		}

		/// <summary>
		/// 將選到的檔案放到這裡, 然後DoUpload時才去複製檔案及寫入資料庫
		/// </summary>
		public List<string> SelectedFiles { get; set; }
		#endregion
		#endregion

		#region Command
		#region Upload
		private bool _isUploadSuccess = false;
		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isUploadSuccess = DoUpload(),
					() => DgList.Any(x => x.IsSelected == true && x.Item.SELECTED_COUNT > 0),
					o => DoAfterUpload()
				);
			}
		}
		private void DoAfterUpload()
		{
			// 成功: 關閉視窗, 失敗: 更新畫面資料
			if (_isUploadSuccess)
			{
				AfterUpload();
			}
			else
			{
				SearchCommand.Execute(null);
			}

			_isUploadSuccess = false;
		}
		/// <summary>
		/// 檔案上傳
		/// 1. 寫入資料庫, 取得檔案名稱
		/// 2. 複製檔案
		/// Memo: 寫入時, 上傳一筆就寫入一筆記錄
		/// </summary>
		public bool DoUpload()
		{
			if (ShowMessage(Messages.WarningBeforeUploadRtFiles) == DialogResponse.No) return false;
			var proxy = GetProxy<F02Entities>();
			var proxyF19 = GetProxy<F19Entities>();
			var proxyEx = GetExProxy<P02ExDataSource>();

			// 複製檔案到新檔案, 同時到資料庫寫入新編號 (檔案名稱)
			// 1. 商品圖檔之外的檔案
			// 取得最新的序號
			var tmp = proxy.F02020105s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == this._gupCode
				&& x.CUST_CODE == this._custCode && x.RT_NO == RtNo).OrderByDescending(x => x.UPLOAD_NO).FirstOrDefault();

			short fileNo = (tmp != null) ? (short)tmp.UPLOAD_NO : (short)0;


			var f02020105s = DgList.Where(x => x.IsSelected == true && !string.IsNullOrWhiteSpace(x.Item.SELECTED_FILES) && x.Item.UPLOAD_TYPE != "00")
									.SelectMany(p =>
									{
										return p.Item.SELECTED_FILES.Split('|')
												.Where(q => !string.IsNullOrWhiteSpace(q))
												.Select(q =>
												{
													fileNo++;
													string newFileName = initfolderItemFiles + p.Item.UPLOAD_NAME + "_" + RtNo + "_" + fileNo + ".jpg";

													return new F02020105
													{
														DC_CODE = SelectedDc,
														GUP_CODE = this._gupCode,
														CUST_CODE = this._custCode,
														PURCHASE_NO = PurchaseNo,
														RT_NO = RtNo,
														UPLOAD_NO = fileNo,
														UPLOAD_C_PATH = q,
														UPLOAD_S_PATH = newFileName,
														UPLOAD_TYPE = p.Item.UPLOAD_TYPE,
													};
												});
									})
									.ToList();

			var existsFile = f02020105s.FirstOrDefault(x => File.Exists(x.UPLOAD_S_PATH));
			if (existsFile != null)
			{
				ShowWarningMessage(Properties.Resources.P0202030300_UploadFileExist);
				return false;
			}

			foreach (var item in f02020105s)
			{
				File.Copy(item.UPLOAD_C_PATH, item.UPLOAD_S_PATH, false);
			}

			foreach (var item in f02020105s)
			{
				proxy.AddToF02020105s(item);
				proxy.SaveChanges();
			}

			//// 2. 商品圖檔
			foreach (var p in DgList.Where(x => x.IsSelected == true && !string.IsNullOrWhiteSpace(x.Item.SELECTED_FILES) && x.Item.UPLOAD_TYPE == "00"))
			{
				var message = string.Empty;
				if (FileService.UpLoadItemImage(_gupCode, _custCode, p.Item.ITEM_CODE, p.Item.SELECTED_FILES, out message))
				{
					// 更新F02020101的檔案上傳狀態
					var tmp2 = proxy.F02020101s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode
						&& x.PURCHASE_NO == PurchaseNo && x.ITEM_CODE == p.Item.ITEM_CODE).ToList();
					tmp2.ForEach(x =>
					{
						x.ISUPLOAD = "1";
						proxy.UpdateObject(x);
					});

					// 更新F020201的檔案上傳狀態
					var tmp3 = proxy.F020201s.Where(x => x.GUP_CODE == _gupCode && x.ITEM_CODE == p.Item.ITEM_CODE).ToList();
					tmp3.ForEach(x =>
					{
						x.ISUPLOAD = "1";
						proxy.UpdateObject(x);
					});
					proxy.SaveChanges();
				}
				else
				{
					ShowWarningMessage(message);
					break;
				}
					
			}

			////3.更新狀態
			var result = proxyEx.CreateQuery<ExecuteResult>("UpdateStatusByAfterUploadFile")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
				.AddQueryOption("purchaseNo", string.Format("'{0}'", PurchaseNo))
				.AddQueryOption("rtNo", string.Format("'{0}'", RtNo))
				.AddQueryOption("inCludeAllItems", string.Format("'{0}'", "1"))
				.ToList();
			if (result.First().IsSuccessed)
			{
				ShowMessage(Messages.InfoFileUploaded);
				return true;
			}
			ShowMessage(Messages.WarningFileUploadedFailure);
			return false;
		}

		#endregion
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						DoSearch();
					},
					() => true,
					o =>
					{
					}
				);
			}
		}
		private bool DoSearch()
		{
			var proxy = GetExProxy<P02ExDataSource>();
			var result = proxy.CreateQuery<FileUploadData>("GetFileUploadSetting")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
				.AddQueryOption("purchaseNo", string.Format("'{0}'", PurchaseNo))
				.AddQueryOption("rtNo", string.Format("'{0}'", RtNo))
				.AddQueryOption("inCludeAllItems", string.Format("'{0}'", "0"))
				.ToList();
			DgList = result.ToSelectionList().ToObservableCollection();
			return true;
		}
		#endregion
		#endregion

		#region 取得新路徑 && 檔案大小
		private string _initfolderItemFiles;

		public string initfolderItemFiles
		{
			get { return _initfolderItemFiles; }
			set { _initfolderItemFiles = value; }
		}
		private int _fileSize;

		public int fileSize
		{
			get { return _fileSize; }
			set { _fileSize = value; }
		}

		public void GetPath()
		{
			//抓gup_code=0、cust_code=0 的新增至新dc、新貨主 =>共用
			var proxy = GetProxy<F19Entities>();
			var size = proxy.F190906s.Where(x => x.DC_CODE == SelectedDc && (x.GUP_CODE == this._gupCode || x.GUP_CODE == "0")
				&& (x.CUST_CODE == this._custCode || x.CUST_CODE == "0") && x.IMG_KEY == "Acceptance").OrderBy(a => a.GUP_CODE).OrderBy(o => o.CUST_CODE).FirstOrDefault();
			fileSize = Convert.ToInt16(size.IMG_SIZE);

			var tmpFilePath = proxy.F190907s.Where(x => x.DC_CODE == SelectedDc && (x.GUP_CODE == this._gupCode || x.GUP_CODE == "0")
				&& (x.CUST_CODE == this._custCode || x.CUST_CODE == "0") && x.PATH_KEY == "AcceptanceFile").OrderBy(a => a.GUP_CODE).OrderBy(o => o.CUST_CODE).FirstOrDefault();
			initfolderItemFiles = tmpFilePath.PATH_ROOT;
			if (!Directory.Exists(initfolderItemFiles))
				Directory.CreateDirectory(initfolderItemFiles);
		}

		#endregion
	}
}
