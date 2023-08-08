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
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Services;
using p19Wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using p91Wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;

namespace Wms3pl.WpfClient.UcLib.ViewModel
{
  public partial class UcDevice_ViewModel : InputViewModelBase
  {
    public Action OnSaved = delegate { };
    public Action OnFocus = delegate { };
    public UcDevice_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;

        this.GetCommonData();
        this.InitialValue();

      }

    }

    #region 資料連結
    private string _userId { get; set; }
    private string _userName { get; set; }
    private string _custCode { get; set; }
    private string _gupCode { get; set; }

    #region Form - DC
    private string _selectedDc { get; set; }
    public string SelectedDc
    {
      get { return _selectedDc; }
      set {
        if (SelectedDc != null)
          check_WORKSTATION_CODE();
        _selectedDc = value; RaisePropertyChanged("SelectedDC");
        //init_detail();
        //SearchCommand.Execute(null);
        DoSearch();
      }
    }

    private string _clientIp { get; set; }
    #endregion

    #region Data - 秤重機清單/ 印表機清單/ 標籤機清單
    private F910501 _data;
    public F910501 Data
    {
      get { return _data; }
      set { Set(() => Data, ref _data, value); }
    }

    private List<NameValuePair<string>> _weightingList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> WeightingList
    {
      get { return _weightingList; }
      set {
        if(_weightingList != null)
        {

        }
        _weightingList = value; RaisePropertyChanged("WeightingList"); }
    }

    private List<NameValuePair<string>> _printerList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> PrinterList
    {
      get { return _printerList; }
      set { _printerList = value; RaisePropertyChanged("PrinterList"); }
    }

    private List<NameValuePair<string>> _labelingList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> LabelingList
    {
      get { return _labelingList; }
      set { _labelingList = value; RaisePropertyChanged("LabelingList"); }
    }

    private List<NameValuePair<string>> _matrixList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> MatrixList
    {
      get { return _matrixList; }
      set { _matrixList = value; RaisePropertyChanged("MatrixList"); }
    }

    private List<NameValuePair<string>> _allWorkstationTypeList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> AllWorkstationTypeList
    {
      get { return _allWorkstationTypeList; }
      set { _allWorkstationTypeList = value; RaisePropertyChanged("AllWorkstationTypeList"); }
    }

    private List<NameValuePair<string>> _workstationTypeList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> WorkstationTypeList
    {
      get { return _workstationTypeList; }
      set { _workstationTypeList = value; RaisePropertyChanged("WorkstationTypeList"); }
    }

    private List<NameValuePair<string>> _workstationGroupList = new List<NameValuePair<string>>();
    public List<NameValuePair<string>> WorkstationGroupList
    {
      get { return _workstationGroupList; }
      set { _workstationGroupList = value; RaisePropertyChanged("WorkstationGroupList"); }
    }

    private List<F1946> _workstationList { get; set; }

    #endregion


    #region
    private List<NameValuePair<string>> _dcList;

    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set
      {
        Set(() => DcList, ref _dcList, value);
      }
    }
    #endregion

    #region 工作站類型
    private string _workStationType;
    public string WorkStationType
    {
      get { return _workStationType; }
      set
      {
        Set(() => WorkStationType, ref _workStationType, value);
      }
    }
    #endregion

    #region 工作站群組
    private string _workStationGroup;
    public string WorkStationGroup
    {
      get { return _workStationGroup; }
      set
      {
        Set(() => WorkStationGroup, ref _workStationGroup, value);
        if (string.IsNullOrWhiteSpace(value))
        {
          WorkStationType = string.IsNullOrWhiteSpace(WorkStationGroup) ? string.Empty : WorkstationTypeList.FirstOrDefault()?.Value;
        }
        else if (value == "B")
        {
          var typeList = new List<string> { "PA1", "PA2", "PACK" };
          WorkstationTypeList = AllWorkstationTypeList.Where(x => typeList.Contains(x.Value)).ToList();
        }
        else
        {
          WorkstationTypeList = AllWorkstationTypeList.Where(x => x.Value.StartsWith(value)).ToList();
        }
      }
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
          o => DoSearch(),
          () => UserOperateMode == OperateMode.Query,
          o => DoSearchCompleted()
          );
      }
    }

    public void DoSearch()
    {
      var proxy = GetProxy<F91Entities>();
      var result =
        proxy.F910501s.Where(x => x.DC_CODE == SelectedDc && x.DEVICE_IP == _clientIp)
          .FirstOrDefault();
      Data = (result ?? new F910501());

      WorkStationGroup = Data.WORKSTATION_GROUP;
      WorkStationType = Data.WORKSTATION_TYPE;
    }

    private void DoSearchCompleted()
    {
    }
    #endregion Search

    #region Save
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => true,
          o => {
            DoSave();
            DoSaveComplete();
          }
        );
      }
    }

    public ICommand CheckCommand
    {
      get
      {
        var isSuccess = false;
        return CreateBusyAsyncCommand<string>(o =>
        {
          DispatcherAction(() =>
          {
            isSuccess = CheckWorkStationCode(o.ToString());
          });
        },
        o => true);
      }
    }

    string Validate()
    {
      if (string.IsNullOrEmpty(SelectedDc))
      {
        return "請選擇物流中心";
      }
      if (!string.IsNullOrEmpty(Data.LABELING))
      {
        if (!LabelingList.Any(x => x.Value == Data.LABELING))
          return string.Format("{0}沒有安裝，請重新選擇快速標籤機!", Data.LABELING);
      }

      if (!string.IsNullOrEmpty(Data.MATRIX_PRINTER))
      {
        if (!MatrixList.Any(x => x.Value == Data.MATRIX_PRINTER))
          return string.Format("{0}沒有安裝，請重新選擇印表機2", Data.MATRIX_PRINTER);
      }

      if (!string.IsNullOrEmpty(Data.PRINTER))
      {
        if (!PrinterList.Any(x => x.Value == Data.PRINTER))
          return string.Format("{0}沒有安裝，請重新選擇印表機1", Data.PRINTER);
      }
      if (!string.IsNullOrWhiteSpace(Data.WORKSTATION_CODE))
      {
        if (string.IsNullOrWhiteSpace(WorkStationGroup))
        {
          return "工作站群組必填";
        }
        if (string.IsNullOrWhiteSpace(WorkStationType))
        {
          return "工作站類型必填";
        }
        if (Data.WORKSTATION_CODE.Length != 4)
        {
          return "工作站編號錯誤，必須輸入4碼";
        }
        if (WorkStationGroup != Data.WORKSTATION_CODE.Substring(0, 1))
        {
          return string.Format("工作站號碼的第一碼應該是{0}", WorkStationGroup);
        }

      }



      //// 檢查工作站類型和工作站編號
      //if (!string.IsNullOrWhiteSpace(WorkStationType))
      //{
      //	var proxyF19 = GetProxy<F19Entities>();
      //	var f1946s = proxyF19.F1946s.Where(x => x.DC_CODE == SelectedDc &&
      //	x.WORKSTATION_TYPE == WorkStationType);

      //	if (!string.IsNullOrWhiteSpace(Data.WORKSTATION_CODE))
      //	{
      //		var existWorkstationCode = f1946s.Where(x => x.WORKSTATION_CODE == Data.WORKSTATION_CODE).FirstOrDefault();
      //		if (existWorkstationCode == null)
      //		{
      //			return string.Format($"工作站類型或工作站編號不存在!");
      //		}
      //	}

      //}
      //else
      //{
      //	return string.Format($"工作站類型或工作站編號不存在!");
      //}

      return string.Empty;
    }

    public bool _isSaved;
    private bool _canClose;
    public void DoSave()
    {
      Data.WORKSTATION_CODE = Data.WORKSTATION_CODE?.ToUpper();
      var p91WcfServiceProxy = new p91Wcf.P91WcfServiceClient();

      _isSaved = false;
      _canClose = true;

      var proxy = GetProxy<F91Entities>();
      var error = Validate();

      if (!string.IsNullOrEmpty(error))
      {
        _canClose = false;
        ShowWarningMessage(error);
        return;
      }

      if (!string.IsNullOrWhiteSpace(Data.WORKSTATION_CODE))
      {
        var f19Proxy = GetProxy<F19Entities>();
        _workstationList = f19Proxy.F1946s.Where(x => x.DC_CODE == SelectedDc).ToList();

        var newStation = _workstationList.Where(o => o.WORKSTATION_CODE == Data.WORKSTATION_CODE).FirstOrDefault();

        if (newStation != null)
        {
          WorkStationGroup = newStation.WORKSTATION_GROUP;
          WorkStationType = newStation.WORKSTATION_TYPE;
        }
        else
        {
          DialogService.ShowMessage(string.Format("工作站編號 {0} 未設定，請通知資訊管理人員新增工作站編號", Data.WORKSTATION_CODE));
          WorkStationGroup = "";
          OnFocus();
          return;
        }
      }

      RunWcfMethod(p91WcfServiceProxy.InnerChannel, () => p91WcfServiceProxy.AddOrUpdateUcDeviceSetting(
        new wcf.DeviceData {
          DC_CODE = SelectedDc,
          DEVICE_IP = _clientIp,
          LABELING = Data.LABELING,
          PRINTER = Data.PRINTER,
          MATRIX_PRINTER = Data.MATRIX_PRINTER,
          WORKSTATION_CODE = Data.WORKSTATION_CODE,
          WORKSTATION_TYPE = WorkStationType,
          WORKSTATION_GROUP = WorkStationGroup
        }, string.IsNullOrWhiteSpace(Data.WORKSTATION_CODE) ? null : new wcf.WorkstationData
        {
          DC_CODE = SelectedDc,
          WORKSTATION_GROUP = WorkStationGroup,
          WORKSTATION_TYPE = WorkStationType,
          WORKSTATION_CODE = Data.WORKSTATION_CODE,
          STATUS = "0"
        }));
      _isSaved = true;
    }

    public void DoSaveComplete()
    {
      if (_isSaved)
        PlaySoundHelper.Scan();
      else
        PlaySoundHelper.Oo();

      if (_canClose)
        OnSaved();
    }
    #endregion

    //public ICommand TestingCommand
    //{
    //	get
    //	{
    //		return new RelayCommand(() =>
    //		{
    //			var f910501 = Data;

    //			var videoServerHelper = new VideoServerHelper();
    //			var result = videoServerHelper.ConnectVideoServer(f910501.VIDEO_URL, f910501.VIDEO_ERROR == "1", f910501.VIDEO_NO, _userId, _userName);
    //			if (!result.IsSuccessed)
    //			{
    //				ShowWarningMessage(result.Message);
    //				return;
    //			}

    //			result = videoServerHelper.VideoShowItemByPastNo("ITEM_CODE", "ITEM_NAME", 1, "PAST_NO");
    //			if (!result.IsSuccessed)
    //			{
    //				ShowWarningMessage(result.Message);
    //			}

    //			result = videoServerHelper.VideoEndSession("RSI_ORD_NO", "RETAIL_CODE", "");
    //			if (!result.IsSuccessed)
    //			{
    //				ShowWarningMessage(result.Message);
    //			}

    //		});
    //	}
    //}

    #region 取得共用資料
    private void GetCommonData()
    {
      IsBusy = true;
      PrinterList = CommonDataHelper.PrinterList();
      WeightingList = CommonDataHelper.WeightingList();
      LabelingList = CommonDataHelper.LabelingList();
      MatrixList = CommonDataHelper.PrinterList();

      IsBusy = false;
    }
    private void InitialValue()
    {
      _clientIp = Wms3plSession.Get<GlobalInfo>().ClientIp;
      DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      if (DcList.Any())
        SelectedDc = DcList.First().Value;

      AllWorkstationTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "TYPE");

      WorkstationGroupList = GetBaseTableService.GetF000904List(FunctionCode, "F1946", "GROUP");
      WorkstationGroupList.Insert(0, new NameValuePair<string> { Name = "", Value = "" });
    }

    public void check_WORKSTATION_CODE()
    {
      var proxy = GetProxy<F91Entities>();
      var result = proxy.F910501s.Where(x => x.DC_CODE == Data.DC_CODE && x.DEVICE_IP == _clientIp && x.WORKSTATION_CODE == Data.WORKSTATION_CODE
      && x.PRINTER == Data.PRINTER && x.LABELING==Data.LABELING && x.MATRIX_PRINTER == Data.MATRIX_PRINTER )
          .FirstOrDefault();
   
      if (result == null)
      {
        var dr = DialogService.ShowMessage("您是否要儲存此物流中心設定?", "訊息", UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
        if (dr == UILib.Services.DialogResponse.Yes)
          DoSave();
      }
    }

    public bool CheckWorkStationCode(string workStationCode)
    {
      if (workStationCode != null && !string.IsNullOrWhiteSpace(workStationCode))
      {
        workStationCode = workStationCode.ToUpper();
        Data.WORKSTATION_CODE = workStationCode;
        
        if (workStationCode.Length != 4)
        {
          DialogService.ShowMessage("工作站編號錯誤，必須輸入4碼");
          WorkStationGroup = "";
          OnFocus();
          return false;
        }
        else
        {
          if (_workstationList == null)
          {
            var proxy = GetProxy<F19Entities>();
            _workstationList = proxy.F1946s.Where(x => x.DC_CODE == SelectedDc).ToList();
          }

          var newStation = _workstationList.Where(o => o.WORKSTATION_CODE == workStationCode).FirstOrDefault();

          if (newStation != null)
          {
            WorkStationGroup = newStation.WORKSTATION_GROUP;
            WorkStationType = newStation.WORKSTATION_TYPE;
          }
          else
          {
            DialogService.ShowMessage(string.Format("工作站編號 {0} 未設定，請通知資訊管理人員新增工作站編號", workStationCode));
            WorkStationGroup = "";
            OnFocus();
            return false;
          }
        }
      }
      
      return true;
    }

		#endregion
		#endregion

	}
}
