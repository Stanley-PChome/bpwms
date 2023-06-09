using AutoMapper;
using CrystalDecisions.Shared.Json;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.ExDataServices.P18ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using exshare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P18WcfService;

namespace Wms3pl.WpfClient.P18.ViewModel
{
	public partial class P1802010000_ViewModel : InputViewModelBase
	{
		public P1802010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				BegCrtDate = DateTime.Today;
				EndCrtDate = DateTime.Today;
				SetDcList();
				SetSrcTypeList();
				SetProcFlagList();
				SetOperateMode(OperateMode.Query);
			}
		}

		#region Property

		private readonly string _userId;
		private readonly string _userName;
		public Action OnDcCodeChanged = delegate { };// 物流中心改變時，顯示 Device

		#region 查詢結果CheckBox控制項是否Enabled
		private bool _checkBoxIsEnabled;

		public bool CheckBoxIsEnabled
		{
			get { return _checkBoxIsEnabled; }
			set { _checkBoxIsEnabled = value; RaisePropertyChanged("CheckBoxIsEnabled"); }
		}
		#endregion

		#region 處理方式控制項是否Enabled
		private bool _processingMethodIsEnabled;

		public bool ProcessingMethodIsEnabled
		{
			get { return _processingMethodIsEnabled; }
			set { _processingMethodIsEnabled = value; RaisePropertyChanged("ProcessingMethodIsEnabled"); }
		}
		#endregion

		#region 業主

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		#endregion

		#region 貨主

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#endregion

		#region 物流中心

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

		private string _SelectedDcCode;

		public string SelectedDcCode
		{
			get { return _SelectedDcCode; }
			set
			{
				_SelectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				if (value != null)
					OnDcCodeChanged();
			}
		}

		#endregion

		#region 建立日期-起

		private DateTime? _begCrtDate;

		public DateTime? BegCrtDate
		{
			get { return _begCrtDate; }
			set
			{
				_begCrtDate = value;
				RaisePropertyChanged("BegCrtDate");
			}
		}

		#endregion

		#region 建立日期-迄

		private DateTime? _endCrtDate;

		public DateTime? EndCrtDate
		{
			get { return _endCrtDate; }
			set
			{
				_endCrtDate = value;
				RaisePropertyChanged("EndCrtDate");
			}
		}
		#endregion

		#region 來源類型

		private List<NameValuePair<string>> _srcTypeList;
		public List<NameValuePair<string>> SrcTypeList
		{
			get { return _srcTypeList; }
			set
			{
				_srcTypeList = value;
				RaisePropertyChanged("SrcTypeList");
			}
		}

		private string _selectedSrcType;

		public string SelectedSrcType
		{
			get { return _selectedSrcType; }
			set
			{
				_selectedSrcType = value;
				RaisePropertyChanged("SelectedSrcType");
			}
		}

		#endregion

		#region 來源單號

		private string _srcWmsNo;

		public string SrcWmsNo
		{
			get { return _srcWmsNo; }
			set
			{
				_srcWmsNo = value;
				RaisePropertyChanged("SrcWmsNo");
			}
		}

		#endregion

		#region 處理方式

		private List<NameValuePair<string>> _procFlagList;
		public List<NameValuePair<string>> ProcFlagList
		{
			get { return _procFlagList; }
			set
			{
				_procFlagList = value;
				RaisePropertyChanged("ProcFlagList");
			}
		}

		private string _selectedProcFlag;

		public string SelectedProcFlag
		{
			get { return _selectedProcFlag; }
			set
			{
				_selectedProcFlag = value;
				RaisePropertyChanged("SelectedProcFlag");
			}
		}

		#endregion

		#region 調撥單號

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
				_itemCode = value;
				RaisePropertyChanged("ItemCode");
			}
		}
		#endregion

		#region 處理方式_(編輯)
		private List<NameValuePair<string>> _editProcFlagListTmp;
		public List<NameValuePair<string>> EditProcFlagListTmp
		{
			get { return _editProcFlagListTmp; }
			set
			{
				_editProcFlagListTmp = value;
			}
		}

		private List<NameValuePair<string>> _editProcFlagList;
		public List<NameValuePair<string>> EditProcFlagList
		{
			get { return _editProcFlagList; }
			set
			{
				_editProcFlagList = value;
				RaisePropertyChanged("EditProcFlagList");
			}
		}
		#endregion

		#region Grid資料繫結資料
		private ObservableCollection<StockAbnormalData> _stockAbnormalData;

		public ObservableCollection<StockAbnormalData> StockAbnormalData
		{
			get { return _stockAbnormalData; }
			set
			{
				_stockAbnormalData = value;
				RaisePropertyChanged("StockAbnormalData");
			}
		}

		private StockAbnormalData _selectedStockAbnormalData;

		public StockAbnormalData SelectedStockAbnormalData
		{
			get { return _selectedStockAbnormalData; }
			set
			{
				if (_selectedStockAbnormalData == value)
					return;

				_selectedStockAbnormalData = Mapper.Map<StockAbnormalData>(value);
				RaisePropertyChanged("SelectedStockAbnormalData");
			}
		}

		private StockAbnormalData _editStockAbnormalData;

		public StockAbnormalData EditStockAbnormalData
		{
			get { return _editStockAbnormalData; }
			set
			{
				if (_editStockAbnormalData == value)
					return;

				_editStockAbnormalData = value;
				RaisePropertyChanged("EditStockAbnormalData");
			}
		}

		#endregion

		#region CheckAll
		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}

		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			if (StockAbnormalData == null) return;
			foreach (var p in StockAbnormalData)
				p.IsSelected = IsSelectedAll;
		}
		#endregion

		#region 顯示/隱藏查詢

		private bool _isShowQuery = true;
		public bool IsShowQuery
		{
			get { return _isShowQuery; }
			set
			{
				_isShowQuery = value;
				RaisePropertyChanged("IsShowQuery");
			}
		}

		#endregion

		#region 顯示/隱藏查詢結果

		private bool _isShowQueryResult = true;
		public bool IsShowQueryResult
		{
			get { return _isShowQueryResult; }
			set
			{
				_isShowQueryResult = value;
				RaisePropertyChanged("IsShowQueryResult");
			}
		}

		#endregion

		#region 顯示/隱藏處理方式

		private bool _isShowProcessingMethod = true;
		public bool IsShowProcessingMethod
		{
			get { return _isShowProcessingMethod; }
			set
			{
				_isShowProcessingMethod = value;
				RaisePropertyChanged("IsShowProcessingMethod");
			}
		}

		#endregion

		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		private void SetSrcTypeList()
		{
			SrcTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F191302", "SRC_TYPE", true);
			SelectedSrcType = SrcTypeList.First().Value;
		}

		private void SetProcFlagList()
		{
			ProcFlagList = GetBaseTableService.GetF000904List(FunctionCode, "F191302", "PROC_FLAG", true);
			SelectedProcFlag = ProcFlagList.First().Value;
			EditProcFlagListTmp = ProcFlagList.Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList();
		}
		#endregion

		void SetOperateMode(OperateMode operateMode)
		{
			UserOperateMode = operateMode;
			ProcessingMethodIsEnabled = UserOperateMode == OperateMode.Edit;
			CheckBoxIsEnabled = UserOperateMode == OperateMode.Query;
		}

		#region 產生盤點單
		public ICommand GenerateInventoryCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoGenerateInventory(), () => UserOperateMode == OperateMode.Query && (StockAbnormalData != null && StockAbnormalData.Where(x => x.IsSelected).Any()), c => DoGenerateInventoryComplete()
					);
			}
		}

		private void DoGenerateInventory()
		{
			var selectedData = StockAbnormalData.Where(x => x.IsSelected).ToList();

			var param = ExDataMapper.MapCollection<StockAbnormalData, wcf.StockAbnormalData>(selectedData);

			var proxy = new wcf.P18WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.CreateInventory(SelectedDcCode, param.ToArray()));

      if (result.IsSuccessed)
      {
        var message = new MessagesStruct
        {
          Button = DialogButton.OK,
          Image = DialogImage.Information,
          Message = string.Format(Properties.Resources.P1802010000_InsertSuccess, result.Message),
          Title = Resources.Resources.Information
        };
        ShowMessage(message);
      }
      else
      {
        //如有多行訊息，切分為20個顯示一個
        var splitMessageCnt = 20;
        var splitMessage = result.Message.Split(new[] { "\r\n" }, StringSplitOptions.None);
        var splitMessagePages = Math.Ceiling(splitMessage.Count() / (decimal)splitMessageCnt);
        for (int i = 0; i < splitMessagePages; i++)
          ShowWarningMessage(Properties.Resources.P1802010000_InsertFail + Environment.NewLine + string.Join("\r\n", splitMessage.Skip(i * splitMessageCnt).Take(splitMessageCnt)));
      }
    }

		private void DoGenerateInventoryComplete()
		{
		}

		#endregion Generate inventory

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
			var proxyEx = GetExProxy<P18ExDataSource>();
			StockAbnormalData = proxyEx.CreateQuery<StockAbnormalData>("GetStockAbnormalData")
									.AddQueryExOption("dcCode", SelectedDcCode)
									.AddQueryExOption("gupCode", GupCode)
									.AddQueryExOption("custCode", CustCode)
									.AddQueryExOption("begCrtDate", BegCrtDate)
									.AddQueryExOption("endCrtDate", EndCrtDate)
									.AddQueryExOption("srcType", SelectedSrcType)
									.AddQueryExOption("srcWmsNo", SrcWmsNo)
									.AddQueryExOption("procFlag", SelectedProcFlag)
									.AddQueryExOption("allocationNo", AllocationNo)
									.AddQueryExOption("itemCode", ItemCode)
									.ToObservableCollection();
		}

		private void DoSearchComplete()
		{
			IsSelectedAll = false;
			if (StockAbnormalData.Any())
				SelectedStockAbnormalData = StockAbnormalData.First();
			else
				ShowMessage(Messages.InfoNoData);
		}

		#endregion Search

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedStockAbnormalData != null && SelectedStockAbnormalData.PROC_FLAG == "0",
					c => DoEditComplete()
					);
			}
		}

		private void DoEdit()
		{
			EditStockAbnormalData = ExDataMapper.Map<StockAbnormalData, StockAbnormalData>(SelectedStockAbnormalData);
			
			if (EditStockAbnormalData.SRC_TYPE == "0")
				EditProcFlagList = EditProcFlagListTmp;
			else if (EditStockAbnormalData.SRC_TYPE == "1")
                EditProcFlagList = EditProcFlagListTmp.Where(x => x.Value == "0" || x.Value == "2").ToList();
            else if (EditStockAbnormalData.SRC_TYPE == "2")
                EditProcFlagList = EditProcFlagListTmp.Where(x => new[] { "0", "1", "2" }.Contains(x.Value)).ToList();

            SetOperateMode(OperateMode.Edit);
		}

		private void DoEditComplete()
		{
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					c => DoCancelComplete()
					);
			}
		}

		private void DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
				return;

			//執行取消動作
			SetOperateMode(OperateMode.Query);
			SelectedProcFlag = "";
			DoSearch();
		}

		private void DoCancelComplete()
		{
			DoSearchComplete();
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query && SelectedStockAbnormalData != null,
					c => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			var editStockAbnormalData = ExDataMapper.Map<StockAbnormalData, wcf.StockAbnormalData>(EditStockAbnormalData);

			// 資料無異動
			if (editStockAbnormalData.PROC_FLAG == SelectedStockAbnormalData.PROC_FLAG &&
				editStockAbnormalData.QTY == SelectedStockAbnormalData.QTY &&
				editStockAbnormalData.MEMO == SelectedStockAbnormalData.MEMO)
			{
				ShowWarningMessage(Properties.Resources.InfoNotModified);
				return;
			}

			// 處理方式若為代處理，不可改變處理數量
			if (editStockAbnormalData.PROC_FLAG == "0" && editStockAbnormalData.QTY != SelectedStockAbnormalData.QTY)
			{
				ShowWarningMessage(Properties.Resources.P1802010000_QtyNotChange);
				return;
			}

			// 處理數量不得超過異常數量
			if (editStockAbnormalData.PROC_FLAG != "0" && editStockAbnormalData.QTY > SelectedStockAbnormalData.QTY)
			{
				ShowWarningMessage(Properties.Resources.P1802010000_QtyError);
				return;
			}

			// 因為只處理部分異常數量，本明細將拆成兩筆，請確認?
			if (editStockAbnormalData.PROC_FLAG != "0" && editStockAbnormalData.QTY < SelectedStockAbnormalData.QTY)
			{
				if (ShowConfirmMessage(Properties.Resources.P1802010000_IsDivide) != DialogResponse.Yes)
					return;
			}

			var proxy = new wcf.P18WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
				() => proxy.UpdateF191302(editStockAbnormalData));

			if (!result.IsSuccessed)
				DialogService.ShowMessage(result.Message);
			else
			{
				ShowMessage(Messages.InfoUpdateSuccess);
				SetOperateMode(OperateMode.Query);
				SelectedStockAbnormalData = null;
				SearchCommand.Execute(null);
			}
		}

		private void DoSaveComplete()
		{
		}
		#endregion Save
	}
}