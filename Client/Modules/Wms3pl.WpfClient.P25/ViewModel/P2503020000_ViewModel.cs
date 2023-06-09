using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P25ExDataService.ExecuteResult;
using wcf = Wms3pl.WpfClient.ExDataServices.P25WcfService;

namespace Wms3pl.WpfClient.P25.ViewModel
{
  public partial class P2503020000_ViewModel : InputViewModelBase
  {
    private string _userId;
    private string _userName;
    private string _gupCode;
    private string _custCode;
    private string _clientIp;
    public Action SetDefaultFocus = delegate { };
    public P2503020000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_clientIp = Wms3plSession.Get<GlobalInfo>().ClientIp;
        InitControls();
        SetDefaultFocus();
      }

    }
    private void InitControls()
    {
    }

    #region 查詢條件

    //商品編號
    private string _itemCode;
    public string ItemCode
    {
      get { return _itemCode; }
      set
      {
        if (_itemCode == value) return;
        Set(() => ItemCode, ref _itemCode, value);
      }
    }

    //商品名稱
    private string _itemName;
    public string ItemName
    {
      get { return _itemName; }
      set
      {
        if (_itemName == value) return;
        Set(() => ItemName, ref _itemName, value);
      }
    }

    //舊序號
    private string _serialNoOld;
    public string SerialNoOld
    {
      get { return _serialNoOld; }
      set
      {
        if (_serialNoOld == value) return;
        Set(() => SerialNoOld, ref _serialNoOld, value);
      }
    }

    //新序號
    private string _serialNoNew;
    public string SerialNoNew
    {
      get { return _serialNoNew; }
      set
      {
        if (_serialNoNew == value) return;
        Set(() => SerialNoNew, ref _serialNoNew, value);
      }
    }

    private bool _onlyShowPass;
    public bool OnlyShowPass
    {
      get { return _onlyShowPass; }
      set
      {
        _onlyShowPass = value;
        DoSearch();
        RaisePropertyChanged("OnlyShowPass");
      }
    }

    #endregion

    #region 查詢結果

    //商品編號檢核結果
    private string _itemCodeCheckMsg;
    public string ItemCodeCheckMsg
    {
      get { return _itemCodeCheckMsg; }
      set
      {
        if (_itemCodeCheckMsg == value) return;
        Set(() => ItemCodeCheckMsg, ref _itemCodeCheckMsg, value);
      }
    }

    //更換數量
    private string _changeCount = "0";
    public string ChangeCount
    {
      get { return _changeCount; }
      set
      {
        if (_changeCount == value) return;
        Set(() => ChangeCount, ref _changeCount, value);
      }
    }

    private ObservableCollection<P250302QueryItem> _queryData;
    public ObservableCollection<P250302QueryItem> QueryData
    {
      get { return _queryData; }
      set
      {
        if (_queryData == value) return;
        Set(() => QueryData, ref _queryData, value);
      }
    }

    //選取的Data
    private P250302QueryItem _selectedQueryData;
    public P250302QueryItem SelectedQueryData
    {
      get { return _selectedQueryData; }
      set
      {
        _selectedQueryData = value;

        RaisePropertyChanged("SelectedQueryData");
      }
    }

    #endregion

    #region 刷讀檢核

    public bool CheckItemCode()
    {
      ItemCodeCheckMsg = null;
      SerialNoOld = null;
      SerialNoNew = null;
      ItemName = null;

      var proxy = GetProxy<F19Entities>();
      var f1903 =
        proxy.F1903s.Where(p => p.ITEM_CODE.ToLower() == ItemCode.ToLower() && p.CUST_CODE == _custCode && p.GUP_CODE == _gupCode)
          .FirstOrDefault();
      if (f1903 == null)
      {
        ItemCodeCheckMsg = Properties.Resources.P2503020000_ViewModel_ItemCodeNotExist;
        PlaySoundHelper.Oo();
        return false;
      }

      ItemCode = f1903.ITEM_CODE;
      ItemName = f1903.ITEM_NAME;
      return true;
    }

    public bool CheckOldSerialNo()
    {
      SerialNoNew = null;
      CanInputNewSerialNo = false;

			if (CheckSerialNoIsExistByF250106(SerialNoOld))
			{
				DialogService.ShowMessage(string.Format(Properties.Resources.P2503020000_ViewModel_ScanData_Duplicate_Old0, SerialNoOld));
				return false;
			}

			var proxyEx = GetExProxy<P25ExDataSource>();
      var result = proxyEx.CreateQuery<ExecuteResult>("CheckOldSerialNo")
        .AddQueryExOption("gupCode", _gupCode)
        .AddQueryExOption("custCode", _custCode)
        .AddQueryExOption("itemCode", ItemCode)
        .AddQueryExOption("serialNo", SerialNoOld)
        .ToList();

      var msg = string.Empty;
      if (result.FirstOrDefault() == null)
        msg = Properties.Resources.P2503020000_ViewModel_OldSerialNoCheckError;
      else if (!result.FirstOrDefault().IsSuccessed)
        msg = result.FirstOrDefault().Message;

      if (!string.IsNullOrWhiteSpace(msg))
      {
        DialogService.ShowMessage(msg);
        return false;
      }

      CanInputNewSerialNo = true;
      return true;
    }

    private bool _canInputNewSerialNo;
    public bool CanInputNewSerialNo
    {
      get { return _canInputNewSerialNo; }
      set
      {
        if (_canInputNewSerialNo == value) return;
        Set(() => CanInputNewSerialNo, ref _canInputNewSerialNo, value);
      }
    }

    public void CheckNewSerialNo()
    {
      if (SerialNoNew == SerialNoOld)
      {
        DialogService.ShowMessage(Properties.Resources.P2503020000_ViewModel_SerialNo_New_Old_TheSame);
        return;
      }

			// 檢查新序號是否存在於F2501
			var proxyEntities = GetProxy<F25Entities>();
			var f2501 = proxyEntities.F2501s.Where(x =>
			x.GUP_CODE == _gupCode &&
			x.CUST_CODE == _custCode &&
			x.SERIAL_NO.ToLower() == SerialNoNew.ToLower()).FirstOrDefault();
			if (f2501 != null)
			{
				DialogService.ShowMessage(Properties.Resources.P2503020000_ViewModel_NewSerialNoIsExist);
				return;
			}

			if (CheckSerialNoIsExistByF250106(SerialNoNew))
      {
        DialogService.ShowMessage(string.Format(Properties.Resources.P2503020000_ViewModel_ScanData_Duplicate_New0, SerialNoNew));
        return;
      }

      var proxy = new ExDataServices.P25WcfService.P25WcfServiceClient();
      RunWcfMethod(proxy.InnerChannel,
        () =>
          proxy.CheckNewSerialNo(_gupCode, _custCode, ItemCode, SerialNoOld, SerialNoNew, _clientIp, _userId, _userName));

      SerialNoNew = null;
      CanInputNewSerialNo = false;
      SerialNoOld = null;
      ItemName = null;
      ItemCode = null;

      DoSearch();
      SetDefaultFocus();
    }
    #endregion

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => DoSearchCompleted()
          );
      }
    }

    private void DoSearchCompleted()
    {
      if (QueryData != null && !QueryData.Any())
        ShowMessage(Messages.InfoNoData);
    }

    private void DoSearch()
    {
      SelectedQueryData = null;
      QueryData = null;

      var proxyEx = GetExProxy<P25ExDataSource>();
      QueryData = proxyEx.CreateQuery<P250302QueryItem>("GetP250302QueryData")
                .AddQueryExOption("gupCode", _gupCode)
                .AddQueryExOption("custCode", _custCode)
                .AddQueryExOption("clientIp", _clientIp)
                .AddQueryExOption("onlyPass", OnlyShowPass ? "1" : "0")
                .ToObservableCollection();

      ChangeCount = QueryData.Count(p => p.ISPASS == "1").ToString();
    }
    #endregion Search

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedQueryData != null
          );
      }
    }

    private void DoDelete()
    {
      if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
        return;

      var proxy = GetProxy<F25Entities>();
      var f250106 = (from p in proxy.F250106s
                     where p.LOG_SEQ == SelectedQueryData.LOG_SEQ
                     select p).SingleOrDefault();
      if (f250106 != null)
      {
        f250106.STATUS = "9";
      }
      proxy.UpdateObject(f250106);
      proxy.SaveChanges();

      DoSearch();
      ShowMessage(Messages.DeleteSuccess);
    }
    #endregion Delete

    #region Import
    public ICommand ImportCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoImport(), () => UserOperateMode == OperateMode.Query
          );
      }
    }

    private void DoImport()
    {
      var dlg = new Microsoft.Win32.OpenFileDialog
      {
        DefaultExt = ".xls",
        Filter = "excel files (*.xls,*.xlsx)|*.xls*"
      };

      if (dlg.ShowDialog() != true)
        return;

      var fullFilePath = dlg.FileName;
      var errorMeg = string.Empty;
      var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg, -1);
      if (string.IsNullOrWhiteSpace(excelTable.TableName))
        excelTable.TableName = "TableName1";

      var proxy = new ExDataServices.P25WcfService.P25WcfServiceClient();
      var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
        () => proxy.CheckImportData(excelTable, _gupCode, _custCode, _clientIp, _userId, _userName));

      if (!result.IsSuccessed)
      {
        DialogService.ShowMessage(result.Message);
        return;
      }

      DoSearch();
    }

    #endregion

    #region 序號更換
    public ICommand ChangeCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoChange(),
          () =>
            UserOperateMode == OperateMode.Query && QueryData != null && QueryData.Any(p => p.ISPASS == "1")
          );
      }
    }

    private void DoChange()
    {
      if (
        ShowMessage(new MessagesStruct()
        {
          Button = DialogButton.YesNo,
          Image = DialogImage.Warning,
          Message = string.Format(Properties.Resources.P2503020000_ViewModel_ExpectChangeCount, ChangeCount),
          Title = Resources.Resources.Information
        }) != DialogResponse.Yes)
        return;

      var listData =
        ExDataMapper.MapCollection<P250302QueryItem, wcf.P250302QueryItem>(QueryData.Where(p => p.ISPASS == "1"))
          .ToArray();
      var proxy = new ExDataServices.P25WcfService.P25WcfServiceClient();
      var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
        () =>
          proxy.ChangeSerialNo(listData, string.Empty));

      if (!result.IsSuccessed)
      {
        DialogService.ShowMessage(result.Message);
        return;
      }

      DoSearch();
      DialogService.ShowMessage(Properties.Resources.P2503020000_ViewModel_ChangeCount);
    }

		#endregion

		/// <summary>
		/// 用SerialNo於F250106取得是否有資料
		/// </summary>
		/// <param name="sn"></param>
		/// <returns></returns>
		private bool CheckSerialNoIsExistByF250106(string sn)
		{
			var proxy = GetProxy<F25Entities>();
			var f250106 = proxy.F250106s.Where(x =>
			x.GUP_CODE == _gupCode &&
			x.CUST_CODE == _custCode &&
			x.ITEM_CODE == ItemCode &&
			x.STATUS == "0" &&
			x.ISPASS == "1" &&
			x.SERIAL_NO.ToLower() == sn.ToLower() &&
			x.CLIENT_IP == _clientIp).FirstOrDefault();
			return f250106 != null;
		}
	}
}
