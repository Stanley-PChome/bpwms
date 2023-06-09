using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901040000_ViewModel : InputViewModelBase
	{

		public P1901040000_ViewModel()
		{
			if (!IsInDesignMode)
			{
        //初始化執行時所需的值及資料
        _ispierrecvpoint = "0";
        _isvendorreturn = "0";
        InitControls();
			}
		}

		private void InitControls()
		{
			SetDcList();
		}

		#region Property
		// 物流中心清單
		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { Set(() => DcList, ref _dcList, value); }
		}

		// 選擇的物流中心
		private string _selectedDc;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { Set(() => SelectedDc, ref _selectedDc, value); }
		}

    // 選擇的物流商編號
    private string _SelectedLogisticCode;
    public string SelectLogisticCode
    {
      get { return _SelectedLogisticCode; }
      set
      {
        var logisticCode = value?.ToString();
        Regex reg = new Regex(@"^[A-Za-z0-9+-]+$");
        Match match = reg.Match(logisticCode);

        if (string.IsNullOrWhiteSpace(logisticCode) || match.Success)
        {
          Set(() => SelectLogisticCode, ref _SelectedLogisticCode, value);
        }
      }
    }


    // 查詢結果清單
    private List<F0002> _dgList;
		public List<F0002> DgList
		{
			get { return _dgList; }
			set { Set(() => DgList, ref _dgList, value); }
		}

		// 選擇的物流中心(新增/編輯)
		private string _dcCode;
		public string DcCode
		{
			get { return _dcCode; }
			set { Set(() => DcCode, ref _dcCode, value); }
		}
    // 選擇碼頭收貨對點使用
    private string _isSelectpierrecvpoint;
    public string IsSelectedPierRecvPoint
    {
      get { return _isSelectpierrecvpoint; }
      set { Set(() => IsSelectedPierRecvPoint, ref _isSelectpierrecvpoint, value); }
    }

    // 選擇廠退出貨扣帳使用
    private string _isSelectedvendorreturn;
    public string IsSelectedVendorReturn
    {
      get { return _isSelectedvendorreturn; }
      set { Set(() => IsSelectedVendorReturn, ref _isSelectedvendorreturn, value); }
    }




    // 物流商編號(新增/編輯)
    private string _logisticCode;
		public string LogisticCode
		{
			get { return _logisticCode; }
			set {
				var logisticCode = value?.ToString();
				Regex reg = new Regex(@"^[A-Za-z0-9+-]+$");
				Match match = reg.Match(logisticCode);

				if (string.IsNullOrWhiteSpace(logisticCode) || match.Success)
				{
					Set(() => LogisticCode, ref _logisticCode, value);
				}
			}
		}

		// 物流商名稱(新增/編輯)
		private string _logisticName;
		public string LogisticName
		{
			get { return _logisticName; }
			set { Set(() => LogisticName, ref _logisticName, value); }
		}

		// 碼頭收貨對點使用(新增/編輯)
		private string _ispierrecvpoint;
		public string IsPierRecvPoint
		{
			get { return _ispierrecvpoint; }
			set { Set(() => IsPierRecvPoint, ref _ispierrecvpoint, value); }
		}

		// 廠退出貨扣帳使用(新增/編輯)
		private string _isvendorreturn;
		public string IsVendorReturn
		{
			get { return _isvendorreturn; }
			set { Set(() => IsVendorReturn, ref _isvendorreturn, value); }
		}

	

		private F0002 _selectedAddOrModifyF0002Data;
		public F0002 SelectedAddOrModifyF0002Data
        {
			get { return _selectedAddOrModifyF0002Data; }
			set { Set(() => SelectedAddOrModifyF0002Data, ref _selectedAddOrModifyF0002Data, value); }
		}
		#endregion

		#region Math
		// 取得物流中心清單
		public void SetDcList()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				SelectedDc = DcList.First().Value;
		}

		public void DoSearch()
		{
			var proxy = GetProxy<F00Entities>();
      var f0002s = proxy.F0002s.Where(x => x.DC_CODE == SelectedDc );
      if( !string.IsNullOrWhiteSpace(SelectLogisticCode))
      {
        f0002s = f0002s.Where(x => x.LOGISTIC_CODE == SelectLogisticCode);
      }    
      DgList = f0002s.ToList();
    
		}

		public void DoAdd()
		{
			DcCode = SelectedDc;
			UserOperateMode = OperateMode.Add;
		}

		public bool DoSave()
		{
      LogisticCode = LogisticCode.ToUpper();
      LogisticName = LogisticName.ToUpper();
			var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.InsertOrUpdateF0002(DcCode, LogisticCode, LogisticName, IsPierRecvPoint, IsVendorReturn, UserOperateMode.ToString()));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return false;
			}
			return true;
		}

		public void DoSaveCpmplete(bool isSuccess)
		{
			if (isSuccess)
			{
				UserOperateMode = OperateMode.Query;
				ClearInput();
				DoSearch();
			}
		}

		public void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			DcCode = SelectedAddOrModifyF0002Data.DC_CODE;
			LogisticCode = SelectedAddOrModifyF0002Data.LOGISTIC_CODE;
            LogisticName = SelectedAddOrModifyF0002Data.LOGISTIC_NAME;
			IsPierRecvPoint = SelectedAddOrModifyF0002Data.IS_PIER_RECV_POINT;
			IsVendorReturn = SelectedAddOrModifyF0002Data.IS_VENDOR_RETURN;
		}

		public void DoCancel()
		{
			UserOperateMode = OperateMode.Query;
			ClearInput();
		}

		public void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.DeleteF0002(SelectedAddOrModifyF0002Data.DC_CODE, SelectedAddOrModifyF0002Data.LOGISTIC_CODE));
				if (result.IsSuccessed)
				{
					ShowMessage(Messages.InfoDeleteSuccess);
				}
				else
				{
					DialogService.ShowMessage(result.Message);
				}
			}
		}

		public void DoDeleteComplete()
		{
			DoSearch();
		}

		public void ClearInput()
		{
			DcCode = "";
			LogisticCode = "";
			LogisticName = "";
			IsPierRecvPoint = "0";
			IsVendorReturn = "0";
		}
		#endregion

		#region ICommand
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query);
			}
		}

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query);
			}
		}

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedAddOrModifyF0002Data != null);
			}
		}

		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedAddOrModifyF0002Data != null,
					o=>DoDeleteComplete());
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit,
					o => DoSaveCpmplete(isSuccess));
			}
		}
		#endregion
	}
}