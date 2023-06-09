using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202050000_ViewModel : InputViewModelBase
	{
        public Action ExcelImport = delegate { };
        public P0202050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}

		}

		private void Init()
		{
			var userInfo = Wms3plSession.Get<UserInfo>();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();

			//_account = userInfo.Account;
			//_accountName = userInfo.AccountName;

			DcList = globalInfo.DcCodeList;
			if (DcList.Any())
				SelectDcCode = DcList.Select(x => x.Value).FirstOrDefault();

			ImportStartDate = DateTime.Today;
			ImportEndDate = DateTime.Today;
		}

		#region Property

		#region DC

		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		private string _selectedDcCode;

		public string SelectDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				_selectedDcCode = value;
				RaisePropertyChanged("SelectDcCode");
			}
		}

		#endregion

		#region 匯入日期起迄
		private DateTime _importStartDate;
		public DateTime ImportStartDate
		{
			get { return _importStartDate; }
			set
			{
				_importStartDate = value;
				RaisePropertyChanged("ImportStartDate");
			}
		}

		private DateTime _importEndDate;
		public DateTime ImportEndDate
		{
			get { return _importEndDate; }
			set
			{
				_importEndDate = value;
				RaisePropertyChanged("ImportEndDate");
			}
		}
		#endregion

		#region PO單號
		private string _poNo;

		public string PoNo
		{
			get { return _poNo; }
			set
			{
				if (_poNo == value)
					return;
				_poNo = value;
				RaisePropertyChanged("PoNo");
			}
		}
		#endregion

		#region Main
		private P020205Main _selectMainData;

		public P020205Main SelectMainData
		{
			get { return _selectMainData; }
			set
			{
				if (_selectMainData == value)
					return;
				_selectMainData = value;
				ReSearchDetail();
				RaisePropertyChanged("SelectMainData");
			}
		}

		private List<P020205Main> _mainData;

		public List<P020205Main> MainData
		{
			get { return _mainData; }
			set
			{
				if (_mainData == value)
					return;
				_mainData = value;
				RaisePropertyChanged("MainData");
			}
		}
		#endregion

		#region Detail
		private P020205Detail _selectDetailData;

		public P020205Detail SelectDetailData
		{
			get { return _selectDetailData; }
			set
			{
				if (_selectDetailData == value)
					return;
				_selectDetailData = value;
				RaisePropertyChanged("SelectDetailData");
			}
		}

		private List<P020205Detail> _detailData;

		public List<P020205Detail> DetailData
		{
			get { return _detailData; }
			set
			{
				if (_detailData == value)
					return;
				_detailData = value;
				RaisePropertyChanged("DetailData");
			}
		}
		#endregion

		#endregion

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
			//執行查詢動作
			MainData = GetMainData();
			if (MainData == null || !MainData.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
			SelectMainData = null;
			DetailData = null;
			SelectDetailData = null;
		}
        #endregion Search

        #region 貨主

        public string CustCode
        {
            get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
        }

        #endregion


        #region Import

        public Action ImportDataAction = delegate { };

        #region 匯入檔案路徑參數

        private string _importFilePath;

        public string ImportFilePath
        {
            get { return _importFilePath; }
            set
            {
                _importFilePath = value;
                RaisePropertyChanged("ImportFilePath");
            }
        }
        #endregion

        public ICommand ImportCommand
		{
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        ExcelImport();
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        DoImportCommand.Execute(null);
                    });
                });
            }
   //         get
			//{
			//	return CreateBusyAsyncCommand(
			//		o => ImportDataAction(),
			//		() => UserOperateMode == OperateMode.Query
			//		);
			//}
		}

        public ICommand DoImportCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { Import(); },
                    () => UserOperateMode == OperateMode.Query
                    );
            }
        }

        public void Import()
		{
			var fullFilePath = ImportFilePath;
			var maxShowMessage = 6;

			var msg = new MessagesStruct()
			{
				Button = DialogButton.OK,
				Image = DialogImage.Information,
				Message = Properties.Resources.P0202050000_ImportSuccess,
				Title = Properties.Resources.Message
			};
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg, 2);
			if (excelTable != null)
			{
				//讀取Excel 欄位
				try
				{
					var sysCustCode = GetF1909SysCustCode();
					var fileName = Path.GetFileNameWithoutExtension(fullFilePath);
					var checkImport = true;
					var message = string.Empty;

					var importData = (from col in excelTable.AsEnumerable()
									  let validDate = DateTimeHelper.ConversionDate(col[5].ToString())
									  select new Wcf.P020205Detail
									  {
										  FILE_NAME = fileName,
										  SYS_CUST_CODE = Convert.ToString(col[0]),
										  PO_NO = Convert.ToString(col[1]),
										  ITEM_CODE = Convert.ToString(col[2]),
                      SERIAL_NO = Convert.ToString(col[3])?.ToUpper(),
                      SERIAL_LEN = Convert.ToInt16(col[4]),
										  VALID_DATE = validDate
									  }).ToList();


					var checkSysCustCode =
						(from i in importData
						 let check = String.Compare(i.SYS_CUST_CODE, sysCustCode, StringComparison.OrdinalIgnoreCase) != 0
						 where check
						 select i);

					if (checkSysCustCode.Any())
					{
						ShowWarningMessage(Properties.Resources.P0202050000_SysCustCodeError);
						return;
					}

					var errorCount = 0;
					foreach (var p in importData.Distinct())
					{
						var result = DoCheckSerialNo(p.ITEM_CODE.Trim(), p.SERIAL_NO.Trim());
						if (!result.Checked)
						{
							errorCount++;
							checkImport = false;
							message += result.Message + Environment.NewLine;

							if (errorCount >= maxShowMessage)//避免資料過多，畫面被充滿
							{
								message += "......";
								break;
							}
						}
					}
					message = message.TrimEnd('、');
					if (checkImport)
					{
						var importResult = ImportSave(importData);
						if (importResult.IsSuccessed)
						{
							ShowMessage(msg);
							ReSearchDetail();
						}
						else
						{
							ShowWarningMessage(string.Format(Properties.Resources.P0202050000_ImportFail, Environment.NewLine, importResult.Message));
						}
					}
					else
					{
						var tipMessage = (message.IndexOf(Properties.Resources.P0202030600_GetC1TipMessageCheck) != -1 && message.IndexOf("C1") != -1) ? Properties.Resources.P0202050000_TipMessage : "";
						ShowWarningMessage(string.Format(Properties.Resources.P0202050000_ImportFail2, Environment.NewLine, message, tipMessage));
					}
				}
				catch
				{
					ShowWarningMessage(Properties.Resources.P0202050000_ImportFormatError);
				}
			}
			else if (string.IsNullOrWhiteSpace(errorMeg))
			{
				msg = new MessagesStruct() { Message = Properties.Resources.P0202030600_DataIsNull, Button = DialogButton.OKCancel, Image = DialogImage.Warning, Title = Properties.Resources.Message };
				ShowMessage(msg);
			}
			else
			{
				msg = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = errorMeg, Title = Properties.Resources.Message };
				ShowMessage(msg);
			}

		}

		public ExecuteResult ImportSave(List<Wcf.P020205Detail> importData)
		{
			var proxy = new Wcf.P02WcfServiceClient();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var result = RunWcfMethod<Wcf.ExecuteResult>(proxy.InnerChannel
						, () => proxy.InserF020301AndF020302(SelectDcCode, globalInfo.GupCode, globalInfo.CustCode, importData.ToArray()));
			return new ExecuteResult { IsSuccessed = result.IsSuccessed, Message = result.Message };
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
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectMainData != null
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			var proxy = GetWcfProxy<Wcf.P02WcfServiceClient>();
			var result = proxy.RunWcfMethod(x => x.DeleteP020205(SelectMainData.Map<P020205Main, Wcf.P020205Main>()));
			ShowResultMessage(result);

			if (result.IsSuccessed)
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

		#region Data
		public List<P020205Main> GetMainData()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P02ExDataSource>();
			return proxyEx.CreateQuery<P020205Main>("GetJincangNoFileMain")
				.AddQueryExOption("dcCode", SelectDcCode)
				.AddQueryExOption("gupCode", globalInfo.GupCode)
				.AddQueryExOption("custCode", globalInfo.CustCode)
				.AddQueryExOption("importStartDate", ImportStartDate)
				.AddQueryExOption("importEndDate", ImportEndDate)
				.AddQueryExOption("poNo", PoNo)
				.ToList();
		}

		public List<P020205Detail> GetDetailData()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<P02ExDataSource>();
			return proxyEx.CreateQuery<P020205Detail>("GetJincangNoFileDetail")
				.AddQueryExOption("dcCode", SelectMainData.DC_CODE)
				.AddQueryExOption("gupCode", SelectMainData.GUP_CODE)
				.AddQueryExOption("custCode", SelectMainData.CUST_CODE)
				.AddQueryExOption("fileName", SelectMainData.FILE_NAME)
				.AddQueryExOption("poNo", PoNo)
				.ToList();
		}

		private void ReSearchDetail()
		{
			if (SelectMainData != null)
			{
				DetailData = GetDetailData();
			}
			SelectDetailData = null;
		}

		public SerialNoResult DoCheckSerialNo(string itemCode, string serialNo, string status = "A1")
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var proxyEx = GetExProxy<ShareExDataSource>();
			var result = proxyEx.CreateQuery<SerialNoResult>("CheckSerialNoFull")
				.AddQueryExOption("dcCode", SelectDcCode)
				.AddQueryExOption("gupCode", globalInfo.GupCode)
				.AddQueryExOption("custCode", globalInfo.CustCode)
				.AddQueryExOption("itemCode", itemCode)
				.AddQueryExOption("serialNo", serialNo)
				.AddQueryExOption("status", status).ToList();

			return result.FirstOrDefault();
		}

		private string GetF1909SysCustCode()
		{
			var proxy = GetProxy<F19Entities>();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var f1909Data = proxy.F1909s.Where(o => o.GUP_CODE == globalInfo.GupCode && o.CUST_CODE == globalInfo.CustCode).FirstOrDefault();
			if (f1909Data != null)
				return f1909Data.SYS_CUST_CODE;

			return string.Empty;

		}
		#endregion
	}
}
