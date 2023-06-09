using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CrystalDecisions.Shared.Json;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Services;
using Wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0203010200_ViewModel : InputViewModelBase
	{
		public P0203010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.CurrentUserInfo.Account;
			}

		}
		#region Property
		private readonly string _userId;
		public Action ExitClick = delegate { };
		public Action ClosedSuccessClick = delegate { };
		//public Action ShowSetItem = delegate { };
		public Action SetDefaultFocus = delegate { };
		public bool IsChangeData;

		private List<F1510BundleSerialLocData> _tempF1510BundleSerialLocDatas;

		private string _dcCode;
		private string _gupCode;
		public string _custCode;
		private string _tarDcCode;
		private string _tarWarehouseId;
    private DateTime? _validDate;
		private string _srcDcCode;
		private string _srcWarehouseId;
		public string ImportFilePath { get; set; }

		#region 調撥單日期

		private DateTime _allocationDate;

		public DateTime AllocationDate
		{
			get { return _allocationDate; }
			set
			{
				_allocationDate = value;
				RaisePropertyChanged("AllocationDate");
			}
		}

		#endregion

		#region 調撥單編號

		private string _allocationNo;

		public string AllocationNo
		{
			get { return _allocationNo; }
			set
			{
				_allocationNo = value;
				RaisePropertyChanged("AllocationNo");
			}
		}

		#endregion


		#region 品號
		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				if (_itemCode == value)
					return;
				Set(() => ItemCode, ref _itemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				if (_itemName == value)
					return;
				Set(() => ItemName, ref _itemName, value);
			}
		}
		#endregion

		#region 序號/箱號

		private string _serialNoOrBoxNo;

		public string SerialNoOrBoxNo
		{
			get { return _serialNoOrBoxNo; }
			set
			{
				_serialNoOrBoxNo = value;
				RaisePropertyChanged("SerialNoOrBoxNo");
			}
		}

		#endregion

		#region 儲位

		private string _locCode;

		public string LocCode
		{
			get { return _locCode; }
			set
			{
				_locCode = value;
				RaisePropertyChanged("LocCode");
			}
		}

		#endregion

		#region 應刷讀數

		private int _requiredCount;

		public int RequiredCount
		{
			get { return _requiredCount; }
			set
			{
				_requiredCount = value;
				RaisePropertyChanged("RequiredCount");
			}
		}

		#endregion

		#region 已刷讀數

		private int _checkedCount;

		public int CheckedCount
		{
			get { return _checkedCount; }
			set
			{
				_checkedCount = value;
				RaisePropertyChanged("CheckedCount");
			}
		}

		#endregion


		#region 調撥單序號刷讀資料 Grid List

		private List<F15100101Data> _f15100101Datas;

		public List<F15100101Data> F15100101Datas
		{
			get { return _f15100101Datas; }
			set
			{
				_f15100101Datas = value;
				RaisePropertyChanged("F15100101Datas");
			}
		}

		private F15100101Data _selectedF15100101Data;

		public F15100101Data SelectedF15100101Data
		{
			get { return _selectedF15100101Data; }
			set
			{
				_selectedF15100101Data = value;
				RaisePropertyChanged("SelectedF15100101Data");
			}
		}


		#endregion
		#endregion

		#region 資料繫結

		public void Bind(string dcCode, string gupCode, string custCode, DateTime allocationDate, string allocationNo, string tarDcCode, string tarWarehouseId,DateTime? validDate)
		{
			_gupCode = gupCode;
			_custCode = custCode;
			_dcCode = dcCode;
			_tarDcCode = tarDcCode;
			_tarWarehouseId = tarWarehouseId;
			_validDate = validDate;
			AllocationDate = allocationDate;
			AllocationNo = allocationNo;
			var proxy = GetProxy<F15Entities>();
			var item = proxy.F151001s.Where(o=>o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo).First();
			_srcDcCode = item.SRC_DC_CODE;
			_srcWarehouseId = item.SRC_WAREHOUSE_ID;
			SearchCommand.Execute(null);
		}
		#endregion

		#region 序號找商品檢查
		/// <summary>
		/// 序號找商品檢查
		/// </summary>
		/// <returns></returns>
		public bool SerialNoOrBoxNoItemCodeCheck()
		{
			var proxyEx = GetExProxy<ShareExDataSource>();

			var getSerialItemResult = proxyEx.CreateQuery<SerialNoResult>("GetSerialItem")
				.AddQueryExOption("dcCode",  _dcCode)
				.AddQueryExOption("gupCode",  _gupCode)
				.AddQueryExOption("custCode",  _custCode)
				.AddQueryExOption("barCode",  SerialNoOrBoxNo)
				.AddQueryExOption("isCombinCheck","1").ToList();
			var item = getSerialItemResult.First();
			if (item.Checked)
			{
				ItemCode = item.ItemCode;
				ItemName = item.ItemName;
			}
			else
			{
				ShowWarningMessage(item.Message);
			}
			return item.Checked;
		}

		#endregion
		#region 序號檢查



		public bool SerialNoOrBoxNoCheck()
		{
			var isCheckOk = true;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			var proxyEx = GetExProxy<ShareExDataSource>();
			var result = proxyEx.CreateQuery<SerialNoResult>("CheckSerialNoFull")
				.AddQueryExOption("dcCode",  _dcCode)
				.AddQueryExOption("gupCode",  _gupCode)
				.AddQueryExOption("custCode",  _custCode)
				.AddQueryExOption("itemCode",  ItemCode)
				.AddQueryExOption("serialNo",  SerialNoOrBoxNo)
				.AddQueryExOption("status",  !string.IsNullOrEmpty(_tarWarehouseId) ? "A1" : "C1")
				.AddQueryExOption("ignoreCheckOfStatus", "A1,C1,D2")
				.AddQueryExOption("isCombinCheck","1")
				;
			var rtnResult = result.First();
			if (!rtnResult.Checked)
				message.Message = rtnResult.Message;

			if (message.Message.Length > 0)
			{
				ShowMessage(message);
				isCheckOk = false;
			}
			return isCheckOk;
		}

		#endregion

		#region 儲位檢查

		public bool LocCodeCheck()
		{
            LocCode = LocCodeHelper.LocCodeConverter9(LocCode);
            var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			if (string.IsNullOrWhiteSpace(LocCode))
			{
				ShowWarningMessage(Properties.Resources.P0203010200_LocIsNull);
				return false;
			}
			var proxyEx = GetExProxy<P02ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("CheckLocCode")
				.AddQueryExOption("tarDcCode", !string.IsNullOrEmpty(_tarWarehouseId) ? _tarDcCode : _srcDcCode)
				.AddQueryExOption("tarWareHouseId",  !string.IsNullOrEmpty(_tarWarehouseId) ? _tarWarehouseId : _srcWarehouseId)
				.AddQueryExOption("locCode",  LocCode)
				.AddQueryExOption("itemCode",  ItemCode)
        .AddQueryExOption("validDate", _validDate.HasValue ? _validDate.Value.ToString() : null);
      var rtnResult = result.First();
			if (!rtnResult.IsSuccessed)
				message.Message = rtnResult.Message;

			if (message.Message.Length > 0)
			{
				ShowMessage(message);
				return false;
			}
			return true;
		}

		#endregion

		#region Method

		/// <summary>
		/// 計算應刷讀數
		/// </summary>
		private void CountRequiredAndCheckedCount()
		{
			var proxyEx = GetExProxy<P02ExDataSource>();
			var data = proxyEx.CreateQuery<AllocationBundleSerialLocCount>("GetAllocationBundleSerialLocCount")
				 .AddQueryExOption("dcCode",  _dcCode)
				 .AddQueryExOption("gupCode",  _gupCode)
				 .AddQueryExOption("custCode",  _custCode)
				 .AddQueryExOption("allocationNo",  AllocationNo)
				 .ToList().First();
			RequiredCount = data.RequiredQty;
			CheckedCount = data.CheckSerialNoQty;
		}

	


		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			var proxyEx = GetExProxy<P02ExDataSource>();
			F15100101Datas = proxyEx.CreateQuery<F15100101Data>("GetF15100101Data")
				 .AddQueryExOption("dcCode",  _dcCode)
				 .AddQueryExOption("gupCode",  _gupCode)
				 .AddQueryExOption("custCode",  _custCode)
				 .AddQueryExOption("allocationNo",  AllocationNo)
				 .ToList();
		}

		private void DoSearchComplete()
		{
			if (F15100101Datas.Any())
				SelectedF15100101Data = F15100101Datas.First();
			CountRequiredAndCheckedCount();
		}
		#endregion Search

		#region Exit
		public ICommand ExitCommand
		{
			get
			{
				return new RelayCommand(
					 DoExit, () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoExit()
		{
			//執行離開動作
			ExitClick();
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedF15100101Data != null && SelectedF15100101Data.ISPASS=="1",
					c => DoDeleteComplete()
					);
			}
		}

		private void DoDelete()
		{
			IsSaveOk = true;
			//執行刪除動作
			var proxyEx = GetExProxy<P02ExDataSource>();
			var result = proxyEx.CreateQuery<ExecuteResult>("DeleteF1510BundleSerialLocData")
				.AddQueryExOption("dcCode",  SelectedF15100101Data.DC_CODE)
				.AddQueryExOption("gupCode",  SelectedF15100101Data.GUP_CODE)
				.AddQueryExOption("custCode",  SelectedF15100101Data.CUST_CODE)
				.AddQueryExOption("allocationNo",  SelectedF15100101Data.ALLOCATION_NO)
				.AddQueryExOption("itemCode",  SelectedF15100101Data.ITEM_CODE)
				.AddQueryExOption("serialNo",  SelectedF15100101Data.SERIAL_NO)
				.ToList();
			if (result[0].IsSuccessed)
			{
				IsChangeData = true;
				SetDefaultFocus();
				ShowMessage(Messages.InfoUpdateSuccess);
			}
			else
			{
				var error = Messages.ErrorUpdateFailed;
				error.Message += Environment.NewLine + result[0].Message;
				ShowMessage(error);
				IsSaveOk = false;
			}

		}

		private void DoDeleteComplete()
		{
			if (IsSaveOk)
				SearchCommand.Execute(null);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Query, c => DoSaveComplete()
					);
			}
		}

		public bool IsSaveOk;
		public void DoSave()
		{
			//執行確認儲存動作
			IsSaveOk = true;
			UserOperateMode = OperateMode.Query;

			if (!string.IsNullOrEmpty(SerialNoOrBoxNo))
			{
				IsSaveOk = SerialNoOrBoxNoItemCodeCheck();
				//if (!IsSaveOk)
				//{
				//	if (string.IsNullOrEmpty(ItemCode))
				//	{
				//		IsSaveOk = false;
				//		ShowSetItem();
				//		return;
				//	}
				//	IsSaveOk = true;
				//}
			}
			if (IsSaveOk)
				IsSaveOk = LocCodeCheck();
			if (IsSaveOk)
			{
				if (!string.IsNullOrEmpty(SerialNoOrBoxNo) && !string.IsNullOrEmpty(LocCode))
				{
					var proxyEx = GetExProxy<P02ExDataSource>();
					var result = proxyEx.CreateQuery<ExecuteResult>("InsertOrUpdateF1510BundleSerialLocData")
						.AddQueryExOption("dcCode",  _dcCode)
						.AddQueryExOption("gupCode",  _gupCode)
						.AddQueryExOption("custCode",  _custCode)
						.AddQueryExOption("allocationNo",  AllocationNo)
						.AddQueryExOption("serialorBoxNo",  SerialNoOrBoxNo)
						.AddQueryExOption("locCode",  LocCode)
						.AddQueryExOption("itemCode",  ItemCode).ToList();
					if (result[0].IsSuccessed)
					{
						IsChangeData = true;
						ShowMessage(Messages.InfoUpdateSuccess);
						SerialNoOrBoxNo = "";
						LocCode = "";
						SetDefaultFocus();
					}
					else
					{
						var error = Messages.ErrorUpdateFailed.Message;
						error += Environment.NewLine + result[0].Message;
						ShowWarningMessage(error);
						IsSaveOk = false;
					}
					DoSearch();
					DoSearchComplete();
				}
			}
		}

		private void DoSaveComplete()
		{
			if (IsSaveOk)
				ClosedSuccessClick();
		}
		#endregion Save

		#region ExcelImportItem
		public ICommand ExcelImportItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoExcelImportItem(), () => UserOperateMode == OperateMode.Query, c => DoExcelImportItemComplete()
						);
			}
		}

		private void DoExcelImportItem()
		{
			var errorMeg = string.Empty;
			var excelTable = DataTableHelper.ReadExcelDataTable(ImportFilePath, ref errorMeg, -1);
			if (!string.IsNullOrEmpty(errorMeg))
			{
				ImportFilePath = null;
				ShowWarningMessage(errorMeg);
				return;
			}
			
			if (excelTable.Columns.Count == 0 || excelTable.Rows.Count == 0)
			{
				ImportFilePath = null;
				ShowWarningMessage(Properties.Resources.P0203010200_ExcelDataIsNull);
				return;
			}
			if (excelTable.Columns.Count < 2)
			{
				ImportFilePath = null;
				ShowWarningMessage(Properties.Resources.P0203010200_ExcelFormatError);
				return;
			}
			var list = (from DataRow dataRow in excelTable.Rows select new wcf.ImportBundleSerialLoc {ROWNUM = excelTable.Rows.IndexOf(dataRow), SERIAL_NO = dataRow[0].ToString(), LOC_CODE = LocCodeHelper.LocCodeConverter9(dataRow[1].ToString())}).ToList();
			var proxyWcf = new wcf.P02WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.ImportSerialNoLoc(_dcCode, _gupCode, _custCode, _allocationNo, list.ToArray()));
			ShowMessage(new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Information,
				Message = result.Message,
				Title = Properties.Resources.Message
			});
			IsChangeData = true;
		}

		private void DoExcelImportItemComplete()
		{
			SearchCommand.Execute(null);
		}
		#endregion

	}
}
