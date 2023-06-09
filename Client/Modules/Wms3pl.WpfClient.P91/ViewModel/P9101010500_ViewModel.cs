using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.Services;
using System.IO;
using System.Data;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010500_ViewModel : InputViewModelBase
	{
		public Action<PrintType, List<P91010105Report>> OnPrint;

		public P9101010500_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				UserOperateMode = OperateMode.Edit;
			}
		}

		public void InitDatas()
		{
			SetProcessItems();
			DoSearch();
		}

		public Action ActionBeforeImportData = delegate { };

		#region 資料連結
		private F910201 _baseData;
		public F910201 BaseData
		{
			get { return _baseData; }
			set
			{
				_baseData = value;
				RaisePropertyChanged("BaseData");
			}
		}

		private F1903 _itemData;
		public F1903 ItemData
		{
			get { return _itemData; }
			set { _itemData = value; RaisePropertyChanged("ItemData"); }
		}

		#region 檔案上傳 file
		private string _fullPath;
		public string FullPath { get { return _fullPath; } set { _fullPath = value; } }
		private string _filePath;
		public string FilePath { get { return _filePath; } set { _filePath = value; } }
		private string _fileName;
		public string FileName { get { return _fileName; } set { _fileName = value; } }
		#endregion


		private bool _showNotBackQty;
		/// <summary>
		/// 拆解商品不顯示未回倉數
		/// </summary>
		public bool ShowNotBackQty
		{
			get { return _showNotBackQty; }
			set
			{
				Set(() => ShowNotBackQty, ref _showNotBackQty, value);
			}
		}


		private int? _notBackQty = null;
		/// <summary>
		/// 未回倉數
		/// </summary>
		public int? NotBackQty
		{
			get { return _notBackQty; }
			set { _notBackQty = value; RaisePropertyChanged("NotBackQty"); }
		}
		private int? _goodBackQty = null;
		/// <summary>
		/// 良品已回倉數
		/// </summary>
		public int? GoodBackQty
		{
			get { return _goodBackQty; }
			set { _goodBackQty = value; RaisePropertyChanged("GoodBackQty"); }
		}
		private int? _badBackQty = null;
		/// <summary>
		/// 損壞已回倉數
		/// </summary>
		public int? BadBackQty
		{
			get { return _badBackQty; }
			set { _badBackQty = value; RaisePropertyChanged("BadBackQty"); }
		}

		private List<BackData> _historyList = new List<BackData>();
		/// <summary>
		/// 回倉歷史明細
		/// </summary>
		public List<BackData> HistoryList
		{
			get { return _historyList; }
			set { _historyList = value; RaisePropertyChanged("HistoryList"); }
		}

        private ObservableCollection<BackData> _newTmpList = new ObservableCollection<BackData>();
        /// <summary>
        /// 回倉品項明細
        /// </summary>
        public ObservableCollection<BackData> UiNewTmpList
        {
            get { return _newTmpList.ToObservableCollection(); }
            set { _newTmpList = value; }
        }

        private ObservableCollection<BackData> _newList = new ObservableCollection<BackData>();
		/// <summary>
		/// 回倉品項明細
		/// </summary>
		public ObservableCollection<BackData> UiNewList
		{
			get { return _newList.ToObservableCollection(); }
            set { _newList = value; RaisePropertyChanged("UiNewList"); }
        }

		private BackData _selectedDetail;
		/// <summary>
		/// 選取到的回倉品項明細
		/// </summary>
		public BackData SelectedDetail
		{
			get { return _selectedDetail; }
			set { _selectedDetail = value; RaisePropertyChanged("SelectedDetail"); }
		}

		private ObservableCollection<ProcessItem> _processItems = new ObservableCollection<ProcessItem>();
		public ObservableCollection<ProcessItem> ProcessItems
		{
			get { return _processItems; }
			set { _processItems = value; RaisePropertyChanged("ProcessItems"); }
		}

		private ProcessItem _selectedProcessItem;
		public ProcessItem SelectedProcessItem
		{
			get { return _selectedProcessItem; }
			set { _selectedProcessItem = value; RaisePropertyChanged("SelectedProcessItem"); }
		}

		private int _newGoodBackQty = 0;
		/// <summary>
		/// 要新增的良品回倉數
		/// </summary>
		public int NewGoodBackQty
		{
			get { return _newGoodBackQty; }
			set { _newGoodBackQty = value; RaisePropertyChanged("NewGoodBackQty"); }
		}

		private int _newBreakBackQty = 0;
		/// <summary>
		/// 要新增的損壞回倉數
		/// </summary>
		public int NewBreakBackQty
		{
			get { return _newBreakBackQty; }
			set { _newBreakBackQty = value; RaisePropertyChanged("NewBreakBackQty"); }
		}

		private Visibility _newBreakBackQtyIsEnabled;
		public Visibility NewBreakBackQtyIsEnabled
		{
			get { return _newBreakBackQtyIsEnabled; }
			set { _newBreakBackQtyIsEnabled = value; RaisePropertyChanged("NewBreakBackQtyIsEnabled"); }
		}

		private List<string> _goodSerialNos = new List<string>();
		public List<string> GoodSerialNos
		{
			get { return _goodSerialNos; }
			set
			{
				Set(() => GoodSerialNos, ref _goodSerialNos, value);
			}
		}

		private List<string> _breakSerialNos = new List<string>();
		public List<string> BreakSerialNos
		{
			get { return _breakSerialNos; }
			set
			{
				Set(() => BreakSerialNos, ref _breakSerialNos, value);
			}
		}


		public ObservableCollection<NameValuePair<string>> ItemTypes
		{
			get
			{
				return new ObservableCollection<NameValuePair<string>> { 
				new NameValuePair<string> { Name = Properties.Resources.P9101010500_ViewModel_Product, Value = "0"},				
				new NameValuePair<string> { Name = Properties.Resources.P9101010500_ViewModel_Source, Value = "1"}
			};
			}
		}

		private string _selectedItemType = "0";
		public string SelectedItemType
		{
			get { return _selectedItemType; }
			set
			{
				Set(() => SelectedItemType, ref _selectedItemType, value);
				SetProcessItems();
			}
		}

		public void SetProcessItems()
		{
			var exMethodName = "GetFinishItemList";
			if (_selectedItemType == "1")
			{
				exMethodName = "GetMaterialList";
			}
			
			var proxyEx = GetExProxy<P91ExDataSource>();
			var tmpMaterialList = proxyEx.CreateQuery<ProcessItem>(exMethodName)
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO))
				.ToList();
			ProcessItems = tmpMaterialList.ToObservableCollection();
			SelectedProcessItem = ProcessItems.FirstOrDefault();
			ClearEditData();
			NewBreakBackQtyIsEnabled = _selectedItemType == "0" ? Visibility.Visible : Visibility.Collapsed;
		}

		private void ClearEditData()
		{
			NewGoodBackQty = 0;
			NewBreakBackQty = 0;

		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Edit
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			// 1. 查詢加工單的品項資訊, 及完成數等統計資料
			var proxy = GetProxy<F19Entities>();
			ItemData =
				proxy.F1903s.Where(x => x.GUP_CODE == BaseData.GUP_CODE && x.ITEM_CODE == BaseData.ITEM_CODE && x.CUST_CODE == BaseData.CUST_CODE).FirstOrDefault();

			var proxyEx = GetExProxy<P91ExDataSource>();
            // 3. 查詢已設定的回倉明細
            _newList = proxyEx.CreateQuery<BackData>("GetBackListForP9101010500")
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO))
				.ToObservableCollection();

            _newTmpList = _newList.Select(x => AutoMapper.Mapper.DynamicMap<BackData>(x)).ToObservableCollection();

            

            // 4. 查詢歷史回倉明細
            HistoryList = proxyEx.CreateQuery<BackData>("GetHistoryListForP9101010500")
                .AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
                .AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
                .AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
                .AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO)).ToList();
            SelectedDetail = UiNewList.FirstOrDefault();

			var proxyF91 = GetProxy<F91Entities>();
			var bomData =
				proxyF91.F910101s.Where(
					n => n.GUP_CODE == _baseData.GUP_CODE && n.CUST_CODE == _baseData.CUST_CODE && n.BOM_NO == (_baseData.ITEM_CODE_BOM??""))
					.FirstOrDefault();

			//拆解商品則隱藏未回倉數
			ShowNotBackQty = !(bomData != null && bomData.BOM_TYPE == "1");

			GoodBackQty = (int)HistoryList.Where(n => n.ITEM_CODE == _baseData.ITEM_CODE).Sum(n => n.GOOD_BACK_QTY);
			BadBackQty = (int)HistoryList.Where(n => n.ITEM_CODE == _baseData.ITEM_CODE).Sum(n => n.BREAK_BACK_QTY);
			NotBackQty = BaseData.PROCESS_QTY - GoodBackQty - BadBackQty;
		}

		#endregion Search

		#region Add
		/// <summary>
		/// 新增/修改前檢查填的資料正不正確
		/// </summary>
		/// <returns></returns>
		private bool DoCheck()
		{
			if ((NewGoodBackQty == 0 && NewBreakBackQty == 0) || NewGoodBackQty < 0 || NewBreakBackQty < 0)
			{
				DialogService.ShowMessage(Properties.Resources.P9101010500_ViewModel_QtyIncorrect);
				return false;
			}

            // 數量加總超出加工數
            if ((NewGoodBackQty + NewBreakBackQty + UiNewList.Sum(x => x.GOOD_BACK_QTY + x.BREAK_BACK_QTY)) > BaseData.PROCESS_QTY)
            {
                DialogService.ShowMessage(Properties.Resources.P9101010500_ViewModel_QtyBeyond);
                return false;
            }

            return true;
		}

		private bool IsModified()
		{
			if (_newList.Any(x => x.OperateStatus != 0)) return true;
			return false;
		}

		public ICommand AddCommand
		{
			get
			{
				return new RelayCommand(
					() => DoAdd(),
					() => UserOperateMode == OperateMode.Edit && 
                    SelectedProcessItem != null && 
                    NewGoodBackQty >= 0 && NewBreakBackQty >= 0 && 
                    (NewGoodBackQty > 0 || NewBreakBackQty > 0) &&
                    ((HistoryList.Sum(x => x.A_TAR_QTY) + UiNewList.Sum(x => x.GOOD_BACK_QTY + x.BREAK_BACK_QTY)) < BaseData.PROCESS_QTY)
                );
			}
		}

		private void DoAdd()
		{
			//UserOperateMode = OperateMode.Add;
			//執行新增動作
			if (!DoCheck()) return;
			var item = _newList.Where(a => a.ITEM_CODE == SelectedProcessItem.ITEM_CODE && a.BACK_ITEM_TYPE == SelectedItemType).FirstOrDefault();
            var itemTmp = _newTmpList.Where(a => a.ITEM_CODE == SelectedProcessItem.ITEM_CODE && a.BACK_ITEM_TYPE == SelectedItemType).FirstOrDefault();
            if (item != null)
            {
                //_removeList.Add(item);
                if (item.OperateStatus == 3)
                {
                    item.GOOD_BACK_QTY = NewGoodBackQty;
                    item.BREAK_BACK_QTY = NewBreakBackQty;
                    itemTmp.GOOD_BACK_QTY = NewGoodBackQty;
                    itemTmp.BREAK_BACK_QTY = NewBreakBackQty;
                }
                else
                {
                    item.GOOD_BACK_QTY += NewGoodBackQty;
                    item.BREAK_BACK_QTY += NewBreakBackQty;
                    itemTmp.GOOD_BACK_QTY += NewGoodBackQty;
                    itemTmp.BREAK_BACK_QTY += NewBreakBackQty;
                }

                if (item.BACK_NO != 0) //BACK_NO!=0表示為先前資料庫中的資料，所以須改為修改狀態，BACK_NO=0則仍為新增改為新增狀態
                {
                    item.OperateStatus = 2;
                    itemTmp.OperateStatus = 2;
                }    
                else
                {
                    item.OperateStatus = 1;
                    itemTmp.OperateStatus = 1;
                }    
            }
            else
            {
                _newList.Add(new BackData()
                {
                    BACK_NO = 0,
                    BREAK_BACK_QTY = NewBreakBackQty,
                    GOOD_BACK_QTY = NewGoodBackQty,
                    ITEM_CODE = SelectedProcessItem.ITEM_CODE,
                    ITEM_NAME = SelectedProcessItem.ITEM_NAME,
                    CRT_NAME = Wms3plSession.CurrentUserInfo.AccountName,
                    CRT_DATE = DateTime.Now,
                    BACK_ITEM_TYPE = SelectedItemType,
                    OperateStatus = 1
                });

                _newTmpList.Add(new BackData()
                {
                    BACK_NO = 0,
                    BREAK_BACK_QTY = NewBreakBackQty,
                    GOOD_BACK_QTY = NewGoodBackQty,
                    ITEM_CODE = SelectedProcessItem.ITEM_CODE,
                    ITEM_NAME = SelectedProcessItem.ITEM_NAME,
                    CRT_NAME = Wms3plSession.CurrentUserInfo.AccountName,
                    CRT_DATE = DateTime.Now,
                    BACK_ITEM_TYPE = SelectedItemType,
                    OperateStatus = 1
                });
            }
				
			RaisePropertyChanged("UiNewList");
			SelectedDetail = UiNewList.LastOrDefault();
		}
		#endregion Add

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Edit
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
					o => DoDelete(), () => UserOperateMode == OperateMode.Edit && SelectedDetail != null && SelectedDetail.A_TAR_QTY == 0
					);
			}
		}

		//private List<BackData> _removeList = new List<BackData>();
		private void DoDelete()
		{
            if (SelectedDetail.BACK_NO > 0)
            {
                UiNewList = UiNewList.Where(x => x.BACK_NO != SelectedDetail.BACK_NO).ToObservableCollection();
                var tmpData = UiNewTmpList.Where(x => x.BACK_NO == SelectedDetail.BACK_NO).FirstOrDefault();
                if (tmpData != null)
                    tmpData.OperateStatus = 3;
            }
            else
            {
                UiNewList = UiNewList.Where(x => x.ITEM_CODE == SelectedDetail.ITEM_CODE && x.BACK_ITEM_TYPE != SelectedDetail.BACK_ITEM_TYPE).ToObservableCollection();
                UiNewTmpList = UiNewTmpList.Where(x => x.ITEM_CODE == SelectedDetail.ITEM_CODE && x.BACK_ITEM_TYPE != SelectedDetail.BACK_ITEM_TYPE).ToObservableCollection();
            }

            RaisePropertyChanged("UiNewList");
			SelectedDetail = UiNewList.FirstOrDefault();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query
				);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			if (!IsModified())
			{
				ShowMessage(Messages.WarningNotModified);
				return;
			}

			DoSaveModified();
		}

		private bool DoSaveModified()
		{
			var msg = DialogService.ShowMessage(Properties.Resources.P9101010500_ViewModel_IsSaveBackDetail, Resources.Resources.Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Information);
			if (msg == UILib.Services.DialogResponse.No)
				return false;

            // 取得差異的資料
            var editData = (from a in _newTmpList.Where(a => a.OperateStatus == 2)
							select a)
										 .Select(x => AutoMapper.Mapper.DynamicMap<wcf.BackData>(x)).ToArray(); // 取得修改的資料
			var newData = (from a in _newTmpList.Where(a => a.OperateStatus == 1)
						   select a)
										 .Select(x => AutoMapper.Mapper.DynamicMap<wcf.BackData>(x)).ToArray();// 取得新增的資料
			var removeData = (from a in _newTmpList.Where(a => a.OperateStatus == 3 && a.BACK_NO != 0) //BACK_NO=0表示非先前資料庫中的資料，不需做刪除
							  select a)
												.Select(x => AutoMapper.Mapper.DynamicMap<wcf.BackData>(x)).ToArray();// 取得刪除的資料 (不包含新增的資料)

			// 儲存資料的流程
			// 1. 如果是回倉明細關聯的調撥單尚未上架，狀態為3，則需先將調撥單刪除，產生新的調撥單與上架回倉關聯檔
			// 2. 如果之前建立的調撥單已經上架，則產生新的調撥單與Insert關聯單
			// 3. 產生調撥單請使用Share專案共用的產生調撥單Function，預設狀態為3
			var proxy = new wcf.P91WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.CreateUpdateBackDataForP91010105(BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.PROCESS_NO, newData, removeData, editData, GoodSerialNos.ToArray(), BreakSerialNos.ToArray()));
			if (result.IsSuccessed == true)
			{
				DoSearch();
				// 結束時提示更新已完成

				DialogService.ShowMessage(string.Format(Properties.Resources.P9101010500_ViewModel_EditSuccess
										, !string.IsNullOrEmpty(result.Message)
											? string.Format(Properties.Resources.P9101010500_ViewModel_TransferNo, result.Message)
											: ""));
			}
			else
			{
				ShowResultMessage(result);
			}

			return result.IsSuccessed;
		}
		#endregion Save


		#region ImportExcelCommand
		public ICommand ImportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
						ActionBeforeImportData();
					},
					() => UiNewList != null && UiNewList.Any()
				);
			}
		}
		public void DoImportData()
		{
			GoodSerialNos.Clear();
			BreakSerialNos.Clear();

			var subFileName = Path.GetExtension(FullPath);
			DataTable excelTable = null;
			try
			{
				using (FileStream file = new FileStream(FullPath, FileMode.Open, FileAccess.Read))
				{
					byte[] bytes = new byte[file.Length];
					file.Read(bytes, 0, (int)file.Length);
					file.Position = 0;

					if (subFileName == ".xlsx")
						excelTable = DataTableExtension3.RenderDataTableFromExcelFor2007(file, 0, -1, 2);
					else if (subFileName == ".xls")
						excelTable = DataTableExtension.RenderDataTableFromExcel(file, 0, -1, 2);
				}
			}
			catch (Exception ex)
			{
				var errorMsg = ErrorHandleHelper.GetCustomErrorCodeDescription(ex, Properties.Resources.P9101010500xamlcs_ImportFail, true);
				ShowWarningMessage(errorMsg);
			}

			var data = (from a in excelTable.AsEnumerable()
						select new
						{
							SerialNo = Convert.ToString(a[0]),
							IsNG = (a.ItemArray.Length < 2) ? false : (Convert.ToString(a[1]).ToUpper() == "NG")
						}).ToList();
			// 沒有資料時直接跳出
			if (!data.Any()) return;

			// 如果第一列不是數字, 直接跳出
			int tmpCount;
			if (!int.TryParse(data.First().SerialNo, out tmpCount))
			{
				ShowWarningMessage(Properties.Resources.P9101010500_ViewModel_FirstNeedTotalCount);
				return;
			}
			if (tmpCount != data.Skip(1).Count(a => !string.IsNullOrEmpty(a.SerialNo)))
			{
				ShowWarningMessage(Properties.Resources.P9101010500_ViewModel_TotalCountNotCorrect);
				return;
			}

			GoodSerialNos = data.Skip(1).Where(a => !a.IsNG).Select(a => a.SerialNo).Distinct().ToList();
			BreakSerialNos = data.Skip(1).Where(a => a.IsNG).Select(a => a.SerialNo).Distinct().ToList();
			ShowWarningMessage(Properties.Resources.P9101010500_ViewModel_SerialNoImportSuccess);
		}
		#endregion


		private ICommand _printCommand;

		/// <summary>
		/// Gets the PrintCommand.
		/// </summary>
		public ICommand PrintCommand
		{
			get
			{
				PrintType printType = PrintType.Preview;
				bool canPrint = true;
				List<P91010105Report> reportData = null;
				return _printCommand
				?? (_printCommand = CreateBusyAsyncCommand(o =>
				{
					printType = (PrintType)o;
					canPrint = true;
					reportData = null;

					// 點選《預覽》或《列印》，系統檢查是否有修改回倉明細，是則提示【回倉明細已修改，是否儲存？】，是則更新上架回倉明細與重新產生調撥單資料，更新完成提示【資料已更新】
					if (IsModified())
					{
						canPrint = DoSaveModified();
					}

					// 後即預覽或列印當前尚未完成回倉的回倉明細調撥單。
					if (canPrint)
					{
						var proxy = GetExProxy<P91ExDataSource>();
						reportData = proxy.GetP91010105Reports(BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.PROCESS_NO).ToList();
					}
				},
				() => true,
				o =>
				{
					if (!canPrint)
						return;

					if (reportData == null || !reportData.Any())
					{
						ShowWarningMessage(Properties.Resources.P9101010500_ViewModel_NoEnoughWarehouse);
						return;
					}

					OnPrint(printType, reportData);
				}));
			}
		}

		#endregion
	}
}
