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
using Microsoft.Win32;
using LINQtoCSV;
using System.IO;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.Services;
using System.Data;
using Wms3pl.WpfClient.UILib.Services;
using Wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0802010100_ViewModel : InputViewModelBase
	{

		#region Property
		#region file
		private string _fileFullPath;
		public string FileFullPath
		{
			get { return _fileFullPath; }
			set
			{
				_fileFullPath = value;
				RaisePropertyChanged();
			}
		}

		private string _filePath;
		public string FilePath
		{
			get { return _filePath; }
			set
			{
				_filePath = value;
				RaisePropertyChanged();
			}
		}
		private string _fileName;
		public string FileName { get { return _fileName; } set { _fileName = value; } }
		#endregion
		#region dc,gup,cust..
		private string _selectedDc = string.Empty;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
			}
		}
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		#endregion
		#region chose
		#region 匯入分貨表
		private bool _isInport = false;
		public bool IsInport { get { return _isInport; } set { _isInport = value; RaisePropertyChanged("IsInport"); } }
		#endregion
		#region 產生分貨表
		private bool _isGenerate = true;
		public bool IsGenerate { get { return _isGenerate; } set { _isGenerate = value; RaisePropertyChanged("IsGenerate"); } }
		#endregion
		#endregion
		#region 退貨查詢條件
		#region 退貨單建立起日期
		private DateTime _selectedStartDt = DateTime.Today;
		public DateTime SelectedStartDt
		{
			get { return _selectedStartDt; }
			set { _selectedStartDt = value; RaisePropertyChanged("SelectedStartDt"); }
		}
		#endregion;
		#region 退貨單建立迄日期
		private DateTime _selectedEndDt = DateTime.Today;
		public DateTime SelectedEndDt
		{
			get { return _selectedEndDt; }
			set { _selectedEndDt = value; RaisePropertyChanged("SelectedEndDt"); }
		}
		#endregion
		#region 退貨起單號
		private string _returnNoStart;
		public string ReturnNoStart { get { return _returnNoStart; } set { _returnNoStart = value; RaisePropertyChanged("ReturnNoStart"); } }
		#endregion
		#region 退貨迄單號
		private string _returnNoEnd;
		public string ReturnNoEnd { get { return _returnNoEnd; } set { _returnNoEnd = value; RaisePropertyChanged("ReturnNoEnd"); } }
		#endregion
		#region 退貨品號
		private string _returnItemCode;
		public string ReturnItemCode { get { return _returnItemCode; } set { _returnItemCode = value; RaisePropertyChanged("ReturnItemCode"); } }
		#endregion
		#region 退貨品名
		private string _returnItemName;
		public string ReturnItemName { get { return _returnItemName; } set { _returnItemName = value; RaisePropertyChanged("ReturnItemName"); } }
		#endregion
		#endregion
		#region 退貨查詢結果
		#region 是否全選

		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				_isCheckAll = value;
				CheckSelectedAll(_isCheckAll);
				RaisePropertyChanged("IsCheckAll");
			}
		}
		#endregion
		#endregion
		#region 退貨選單 Grid List

		private SelectionList<F161202SelectedData> _f161202SelectedDatas;

		public SelectionList<F161202SelectedData> F161202SelectedDatas
		{
			get { return _f161202SelectedDatas; }
			set
			{
				_f161202SelectedDatas = value;
				RaisePropertyChanged("F161202SelectedDatas");
			}
		}

		private SelectionItem<F161202SelectedData> _selectedF161202SelectedData;

		public SelectionItem<F161202SelectedData> SelectedF161202SelectedData
		{
			get { return _selectedF161202SelectedData; }
			set
			{
				_selectedF161202SelectedData = value;
				RaisePropertyChanged("SelectedF161202SelectedData");
			}
		}
		#endregion

		#region 彙總單號查詢條件
		#region 建立起日期
		private DateTime _selectedGatherStartDt = DateTime.Today;
		public DateTime SelectedGatherStartDt
		{
			get { return _selectedGatherStartDt; }
			set { _selectedGatherStartDt = value; RaisePropertyChanged("SelectedGatherStartDt"); }
		}
		#endregion;
		#region 建立迄日期
		private DateTime _selectedGatherEndDt = DateTime.Today;
		public DateTime SelectedGatherEndDt
		{
			get { return _selectedGatherEndDt; }
			set { _selectedGatherEndDt = value; RaisePropertyChanged("SelectedGatherEndDt"); }
		}
		#endregion
		#region 彙總單號起
		private string _gatherNoStart;
		public string GatherNoStart { get { return _gatherNoStart; } set { _gatherNoStart = value; RaisePropertyChanged("GatherNoStart"); } }
		#endregion
		#region 彙總單號迄
		private string _gatherNoEnd;
		public string GatherNoEnd { get { return _gatherNoEnd; } set { _gatherNoEnd = value; RaisePropertyChanged("GatherNoEnd"); } }
		#endregion
		#region 檔名
		private string _searchFileName;
		public string SearchFileName { get { return _searchFileName; } set { _searchFileName = value; RaisePropertyChanged("SearchFileName"); } }
		#endregion
		#endregion
		#region 彙總單 Grid List

		private SelectionList<F161501> _f161501SelectedDatas;

		public SelectionList<F161501> F161501SelectedDatas
		{
			get { return _f161501SelectedDatas; }
			set
			{
				_f161501SelectedDatas = value;
				RaisePropertyChanged("F161501SelectedDatas");
			}
		}

		private SelectionItem<F161501> _selectedF161501SelectedData;

		public SelectionItem<F161501> SelectedF161501SelectedData
		{
			get { return _selectedF161501SelectedData; }
			set
			{
				_selectedF161501SelectedData = value;
				RaisePropertyChanged("SelectedF161501SelectedData");
			}
		}
		#endregion
		#region 是否全選

		private bool _isGatherCheckAll;

		public bool IsGatherCheckAll
		{
			get { return _isGatherCheckAll; }
			set
			{
				_isGatherCheckAll = value;
				CheckGatherSelectedAll(_isGatherCheckAll);
				RaisePropertyChanged("IsGatherCheckAll");
			}
		}
		#endregion
		#endregion

		#region Function

		public P0802010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region Grid 全選
		public void CheckSelectedAll(bool isChecked)
		{
			foreach (var f161202SelectedData in F161202SelectedDatas)
				f161202SelectedData.IsSelected = isChecked;
		}

		public void CheckGatherSelectedAll(bool isChecked)
		{
			foreach (var f161501SelectedData in F161501SelectedDatas)
				f161501SelectedData.IsSelected = isChecked;
		}
		#endregion
		#endregion

		#region Command

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
			//執行查詢動
			DoSearchF161202SelectedData();
		}
		private void DoSearchF161202SelectedData()
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			F161202SelectedDatas = proxyEx.CreateQuery<F161202SelectedData>("GetReturnItems")
														.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
														.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
														.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
														.AddQueryOption("returnDateStart", string.Format("'{0}'", SelectedStartDt.ToString("yyyy/MM/dd")))
														.AddQueryOption("returnDateEnd", string.Format("'{0}'", SelectedEndDt.ToString("yyyy/MM/dd")))
														.AddQueryOption("returnNoStart", string.Format("'{0}'", ReturnNoStart))
														.AddQueryOption("returnNoEnd", string.Format("'{0}'", ReturnNoEnd))
														.AddQueryOption("itemCode", string.Format("'{0}'", ReturnItemCode))
														.AddQueryOption("itemName", string.Format("'{0}'", ReturnItemName)).ToSelectionList();

			if (F161202SelectedDatas != null && F161202SelectedDatas.Any())
				SelectedF161202SelectedData = F161202SelectedDatas.FirstOrDefault();
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query || true
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
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query || CanImport() || CanGenerate()
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			if (IsInport)
			{
				if (CanImport())
					DoImport();
			}
			else
			{
				if (F161202SelectedDatas.Where(x => x.IsSelected == true).Any())
					DoGenerated();
				else
					ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0802010100_NoSelectData, Title = WpfClient.Resources.Resources.Information });
			}
			UserOperateMode = OperateMode.Query;
		}

		private void DoGenerated()
		{
			var data = new List<wcf.F161502>();
			//取得彙總單流水號
			var GatherNo = GetGatherNo();

			var SelectedDatas = F161202SelectedDatas.Where(x => x.IsSelected).ToList();
			data = SelectedDatas.Select(y =>
							new wcf.F161502()
							{
								GATHER_NOk__BackingField = GatherNo,
								CUST_CODEk__BackingField = _custCode,
								GUP_CODEk__BackingField = _gupCode,
								GATHER_SEQk__BackingField = Convert.ToString(F161202SelectedDatas.IndexOf(y)).PadLeft(3, '0'),
								ITEM_CODEk__BackingField = Convert.ToString(y.Item.ITEM_CODE),
								ITEM_NAMEk__BackingField = Convert.ToString(y.Item.ITEM_NAME),
								RTN_QTYk__BackingField = Convert.ToString(y.Item.RTN_QTY),
								DC_CODEk__BackingField = SelectedDc
							}).ToList();

			var msg = DoInsertSummaryReport(data, GatherNo);
			ShowMessage(msg);
		}

		private void DoImport()
		{
			//執行匯入動作
			var data = new List<wcf.F161502>();
			var msg = new MessagesStruct();
			var subFileName = Path.GetExtension(FileFullPath);
			//取得彙總單流水號
			var gatherNo = GetGatherNo();
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(FileFullPath, ref errorMeg);
			if (excelTable != null)
			{
				try
				{
					data = (from row in excelTable.AsEnumerable()
									where !string.IsNullOrEmpty(Convert.ToString(row[3]))
						select new wcf.F161502()
						{
							GATHER_NOk__BackingField = gatherNo,
							CUST_CODEk__BackingField = Convert.ToString(row[0]),
							GUP_CODEk__BackingField = Convert.ToString(row[1]),
							GATHER_SEQk__BackingField = Convert.ToString(row[2]).PadLeft(3, '0'),
							ITEM_CODEk__BackingField = Convert.ToString(row[3]),
							ITEM_NAMEk__BackingField = Convert.ToString(row[4]),
							RTN_QTYk__BackingField = Convert.ToString(row[5]),
							DC_CODEk__BackingField = SelectedDc
						}).ToList();
				}
				catch (Exception ex)
				{
					msg = Messages.ErrorImportFailed;
					msg.Message = msg.Message + Environment.NewLine + Properties.Resources.P0802010100_FileFormatError + Environment.NewLine + ex.Message;
					ShowMessage(msg);
					return;
				}
			}
			else if (string.IsNullOrWhiteSpace(errorMeg))
			{
				msg = new MessagesStruct() { Message = Properties.Resources.P0802010100_NoData, Button = DialogButton.OK, Image = DialogImage.Warning, Title = WpfClient.Resources.Resources.Information };
				ShowMessage(msg);
			}
			else
			{
				msg = Messages.ErrorImportFailed;
				msg.Message = errorMeg;
				ShowMessage(msg);
				return;
			}

			msg = DoInsertSummaryReport(data, gatherNo, FileName);
			ShowMessage(msg);

			#region 改寫讀取方式，以判斷EXCEL是否已被開啟或鎖定(20150613)-暫時保留
			/*
			using (FileStream file = new FileStream(FileFullPath, FileMode.Open, FileAccess.Read))
			{
				byte[] bytes = new byte[file.Length];
				file.Read(bytes, 0, (int)file.Length);
				file.Position = 0;
				//ms.Write(bytes, 0, (int)file.Length);

				DataTable excelTable = null;
				try
				{
					if (subFileName == ".xlsx")
						excelTable = DataTableExtension3.RenderDataTableFromExcelFor2007(file, 0, 0);
					else if (subFileName == ".xls")
						excelTable = DataTableExtension.RenderDataTableFromExcel(file, 0, 0);
					else
					{
						var result = new ExDataServices.P19ExDataService.ExecuteResult();
						result.IsSuccessed = false;
						result.Message = Properties.Resources.P0802010100_FileFormatError;
						msg = Messages.ErrorImportFailed;
						msg.Message = msg.Message + Environment.NewLine + result.Message;
						ShowMessage(msg);
						return;
					}

					data = (from row in excelTable.AsEnumerable()
									select new wcf.F161502()
									{
										GATHER_NOk__BackingField = GatherNo,
										CUST_CODEk__BackingField = Convert.ToString(row[0]),
										GUP_CODEk__BackingField = Convert.ToString(row[1]),
										GATHER_SEQk__BackingField = Convert.ToString(row[2]).PadLeft(3, '0'),
										ITEM_CODEk__BackingField = Convert.ToString(row[3]),
										ITEM_NAMEk__BackingField = Convert.ToString(row[4]),
										RTN_QTYk__BackingField = Convert.ToString(row[5]),
										DC_CODEk__BackingField = SelectedDc
									}).ToList();
					file.Close();
				}
				catch (Exception ex)
				{
					iscsvOk = false;
					var result = new ExDataServices.P19ExDataService.ExecuteResult();
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P0802010100_FileFormatError + Environment.NewLine + ex.Message;
					msg = Messages.ErrorImportFailed;
					msg.Message = msg.Message + Environment.NewLine + result.Message;
				}
			}
			if (iscsvOk)
			{
				msg = DoInsertSummaryReport(data, GatherNo, FileName);
			}
			ShowMessage(msg);
			*/
			#endregion


		}

		private MessagesStruct DoInsertSummaryReport(List<Wcf.F161502> data, string GatherNo, string FileName = null)
		{
			var proxy = GetProxy<F16Entities>();
			var proxywcf = new wcf.P08WcfServiceClient();
			var msg = new MessagesStruct();
			var itemCount = 0;
			var result = RunWcfMethod<wcf.ExecuteResult>(
					proxywcf.InnerChannel,
					() => proxywcf.ImpoortP161502(data.ToArray(), FileName));
			if (result.IsSuccessed)
			{
				var f161502s = proxy.F161502s.Where(x => x.DC_CODE == SelectedDc && x.GATHER_NO == GatherNo).ToList();
				if (f161502s != null && f161502s.Any())
					itemCount = f161502s.Count;
				msg = new MessagesStruct() { Message = string.Format(Properties.Resources.P0802010100_GatherNoItemCount, GatherNo, itemCount), Title = WpfClient.Resources.Resources.Information };
			}
			else
			{
				msg = Messages.ErrorAddFailed;
				msg.Message = msg.Message + Environment.NewLine + result.Message;
			}
			return msg;
		}



		private bool CanImport()
		{
			bool hasFile = false;
			if (!string.IsNullOrEmpty(_fileFullPath))
			{
				var file = new FileInfo(_fileFullPath);
				hasFile = file.Exists;
			}
			return hasFile;
		}

		private bool CanGenerate()
		{
			if (F161202SelectedDatas == null || !F161202SelectedDatas.Any()) return false;
			return true;
		}

		private string GetGatherNo()
		{
			var proxy = new Wcf.P08WcfServiceClient();
			var gatherNo = RunWcfMethod<string>(proxy.InnerChannel,
					() => proxy.GetPintBarCode("ZR"));
			return gatherNo;
		}
		#endregion Save

		#region OpenFileDialog

		public ICommand OpenFileDialogCommand
		{
			get
			{


				return new RelayCommand(
					 () => OpenFileDialogMethod(),
					 () => UserOperateMode == OperateMode.Query
					);
			}
		}

		public void OpenFileDialogMethod()
		{
			var win = new OpenFileDialog { InitialDirectory = string.IsNullOrEmpty(FilePath) ? @"C:\" : FilePath };
			if (!win.CheckPathExists)
				win.InitialDirectory = @"C:\";

			win.Multiselect = false;
			win.DefaultExt = ".xlsx";
			win.Filter = "xlsx files (.xlsx; .xls)|*.xlsx; *.xls";
			win.FilterIndex = 1;
			win.RestoreDirectory = true;
			var result = win.ShowDialog();
			if (result == true)
			{
				FileFullPath = win.FileName;
				FilePath = win.FileName.Substring(0, win.FileName.LastIndexOf("\\"));
				FileName = win.SafeFileName;
			}

		}

		#endregion

		#region SearchGatherCommand

		public ICommand SearchGatherCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchGather(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearchGather()
		{
			//執行查詢動
			DoSearchGatherData();
		}
		private void DoSearchGatherData()
		{
			var proxyEx = GetExProxy<P08ExDataSource>();
			F161501SelectedDatas = proxyEx.CreateQuery<F161501>("GetGatherItems")
														.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
														.AddQueryOption("gatherDateStart", string.Format("'{0}'", SelectedGatherStartDt.ToString("yyyy/MM/dd")))
														.AddQueryOption("gatherDateEnd", string.Format("'{0}'", SelectedGatherEndDt.ToString("yyyy/MM/dd")))
														.AddQueryOption("gatherNoStart", string.Format("'{0}'", GatherNoStart))
														.AddQueryOption("gatherNoEnd", string.Format("'{0}'", GatherNoEnd))
														.AddQueryOption("fileName", string.Format("'{0}'", SearchFileName)).ToSelectionList();

			if (F161501SelectedDatas != null && F161501SelectedDatas.Any())
				SelectedF161501SelectedData = F161501SelectedDatas.FirstOrDefault();
		}
		#endregion

		#region DeleteGather
		public ICommand DeleteGatherCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDeleteGather(), () => UserOperateMode == OperateMode.Query && (F161501SelectedDatas != null && F161501SelectedDatas.Any())
					);
			}
		}

		private void DoDeleteGather()
		{
			//執行刪除動作
			var msg = new MessagesStruct() { Message = "", Title = WpfClient.Resources.Resources.Information };
			var SelectedDatas = F161501SelectedDatas.Where(x => x.IsSelected).ToList();
			if (SelectedDatas == null || !SelectedDatas.Any())
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0802010100_NoSelectData, Title = WpfClient.Resources.Resources.Information });
				return;
			}
			var strGatherNo = string.Join(",", SelectedDatas.Select(x => x.Item.GATHER_NO).ToList().ToArray());

			var proxyEx = GetExProxy<P08ExDataSource>();
			var results = proxyEx.CreateQuery<ExecuteResult>("DoDelGatherData")
						.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
						.AddQueryOption("gatherNos", string.Format("'{0}'", strGatherNo))
						.ToList();

			if (results != null || results.Any())
			{
				if (results.FirstOrDefault().IsSuccessed)
					msg.Message = WpfClient.Resources.Resources.InfoDeleteSuccess;
				else
					msg.Message = Properties.Resources.P0802010100_DeletedFail;
			}
			ShowMessage(msg);
			DoSearchGather();
		}
		#endregion DeleteGather
		#endregion


	}
}
