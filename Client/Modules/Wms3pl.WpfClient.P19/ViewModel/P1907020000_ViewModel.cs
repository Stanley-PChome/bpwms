using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P19.ViewModel
{
  public partial class P1907020000_ViewModel : InputViewModelBase
  {
		private string _gupCode;
		private string _custCode;
    private readonly F19Entities _proxy;
    private CancellationTokenSource _cancellationSource;
    public P1907020000_ViewModel()
    {
      if (!IsInDesignMode)
      {
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
        _proxy = GetProxy<F19Entities>();
        InitControls();
      }

    }

    private void InitControls()
    {
      GetQueryGroupList();
      if (QueryGroupList.Any())
        SelectedQueryGroup = QueryGroupList.First().Value;

	    SetComboBoxSource();

    }

	  public void SetComboBoxSource()
	  {
			var proxy = new ExDataServices.P19WcfService.P19WcfServiceClient();

			var list = new Dictionary<string, List<NameValuePair<string>>>();
			var queryResult = RunWcfMethod(proxy.InnerChannel,
					() => proxy.GetQueryDataByCombo());

			var data = Wms3plSession.Get<GlobalInfo>().DcGupCustDatas;
			foreach (var item in queryResult)
		  {
					var ds = item.Value;
					var listSub = (from KeyValuePair<string,string> row in item.Value select new NameValuePair<string> { Name = row.Value, Value = row.Key }).ToList();
				if (item.Key == "4") //業主
					list.Add(item.Key, listSub.Where(x => data.Any(y=> y.GupCode == x.Value)).ToList());
				else if (item.Key == "5") //貨主
					list.Add(item.Key, listSub.Where(x => data.Any(y => y.CustCode == x.Value)).ToList());
				else
					list.Add(item.Key, listSub);
			}
			AllIdListDict = list;
	  }

		private Dictionary<string, List<NameValuePair<string>>> _allIdListDict;

		public Dictionary<string, List<NameValuePair<string>>> AllIdListDict
		{
			get { return _allIdListDict; }
			set
			{
				if (_allIdListDict == value)
					return;
				Set(() => AllIdListDict, ref _allIdListDict, value);
			}
		}






    #region 查詢類別
    private string _selectedQueryGroup;
    public string SelectedQueryGroup
    {
      get { return _selectedQueryGroup; }
      set
      {
        if (_selectedQueryGroup == value) return;
        Set(() => SelectedQueryGroup, ref _selectedQueryGroup, value);
	      GetQueryItemList();
      }
    }

    private List<NameValuePair<string>> _queryGroupList;
    public List<NameValuePair<string>> QueryGroupList
    {
      get { return _queryGroupList; }
      set
      {
        if (_queryGroupList == value) return;
        Set(() => QueryGroupList, ref _queryGroupList, value);
      }
    }
    private void GetQueryGroupList()
    {
			QueryGroupList = GetBaseTableService.GetF000904List(FunctionCode, "F190701", "QGROUP");
			QueryGroupList.Insert(0,new NameValuePair<string>{ Name = Resources.Resources.All, Value = string.Empty });
    }
    #endregion

    #region 查詢項目
    private F190701 _selectedQueryItem;
    public F190701 SelectedQueryItem
    {
      get
      {
        return _selectedQueryItem;
      }
      set
      {
        _selectedQueryItem = value;
        ClearData();
        GetQueryParamsList();
        RaisePropertyChanged("SelectedQueryItem");
      }
    }


    private List<F190701> _queryItemList;
    public List<F190701> QueryItemList
    {
      get { return _queryItemList; }
      set
      {
        if (_queryItemList == value) return;
        Set(() => QueryItemList, ref _queryItemList, value);
      }
    }

    private void GetQueryItemList()
    {
      if (Wms3plSession.Get<UserInfo>().Account.ToLower() == "wms")
      {
        QueryItemList = (from p in _proxy.F190701s
                         where (string.IsNullOrWhiteSpace(SelectedQueryGroup) || p.QGROUP == SelectedQueryGroup)
                         orderby p.NAME
                         select p).ToList();
      }
      else
      {
        QueryItemList = _proxy.CreateQuery<F190701>("GetQueryListByEmpId")
          .AddQueryOption("empId", string.Format("'{0}'", Wms3plSession.Get<UserInfo>().Account))
          .AddQueryOption("qGroup", string.Format("'{0}'", SelectedQueryGroup))
          .ToList();
      }
		}
		#endregion

		#region
		private AddedPropertyItem<F190703, object> _selectedQueryParam;
		public AddedPropertyItem<F190703, object> SelectedQueryParam
		{
			get { return _selectedQueryParam; }
			set {
				if (_selectedQueryParam != null)
					_selectedQueryParam.PropertyChanged -= SelectedQueryParam_PropertyChanged; //先移除PropertyChanged的Event Handler，避免重複呼叫
				Set(() => SelectedQueryParam, ref _selectedQueryParam, value);
				if (_selectedQueryParam != null)
					_selectedQueryParam.PropertyChanged += SelectedQueryParam_PropertyChanged; //增加PropertyChanged 的Event Handler

			}
		}
		#endregion

		#region 參數Grid
		private AddedPropertyItemList<F190703, object> _queryParamsList;
    public AddedPropertyItemList<F190703, object> QueryParamsList
    {
      get { return _queryParamsList; }
      set
      {
        if (_queryParamsList == value) return;
        Set(() => QueryParamsList, ref _queryParamsList, value);
      }
    }

    private void GetQueryParamsList()
    {
      if (SelectedQueryItem == null)
        return;

      var results = (from p in _proxy.F190703s
                     where p.QID == SelectedQueryItem.QID
                     orderby p.SEQ
                     select p).ToList();
      QueryParamsList = results.ToAddedPropertyItemList<F190703, object>();
			foreach(var item in QueryParamsList)
			{
				if (item.Item.PTYPE == "2")
				{
					item.AddedProp = AllIdListDict[item.Item.FUN_ID.ToString()].FirstOrDefault()?.Value;
				}
			}
      ShowQueryParam = (QueryParamsList != null && QueryParamsList.Any());
    }
    #endregion

    private bool _showQueryParam;
    /// <summary>
    /// 是否顯示參數Grid
    /// </summary>
    public bool ShowQueryParam
    {
      get { return _showQueryParam; }
      set
      {
        if (_showQueryParam == value) return;
        Set(() => ShowQueryParam, ref _showQueryParam, value);
      }
    }

    private bool _showQueryResult;
    /// <summary>
    /// 是否顯示查詢結果Grid
    /// </summary>
    public bool ShowQueryResult
    {
      get { return _showQueryResult; }
      set
      {
        if (_showQueryResult == value) return;
        Set(() => ShowQueryResult, ref _showQueryResult, value);
      }
    }

    private DataTable _data;
    /// <summary>
    /// 查詢結果
    /// </summary>
    public DataTable Data
    {
      get { return _data; }
      set
      {
        _data = value;
        ShowQueryResult = _data != null;
        RaisePropertyChanged("Data");
				//Set(() => Data, ref _data, value);
      }
    }

    private TimeSpan _timeSpan;
    public TimeSpan TimeSpan
    {
      get { return _timeSpan; }
      set
      {
        if (_timeSpan == value) return;
        Set(() => TimeSpan, ref _timeSpan, value);
      }
    }

    private void ClearData()
    {
      Data = null;
      QueryParamsList = null;
      ShowQueryParam = false;
    }

    #region Search
    private bool IsParamsIsValid()
    {
      if (QueryParamsList == null) return true;
      var q = QueryParamsList.Any(p => p.AddedProp == null || string.IsNullOrWhiteSpace((string)p.AddedProp));
      return !q;
    }

    public ICommand SearchCommand
    {
      get
      {
	      return CreateBusyAsyncCommand(
		      o => DoSearch(),
		      () => UserOperateMode == OperateMode.Query && SelectedQueryItem != null,
		      null, null, DoSearchCheck
		      );
      }
    }

    private bool _canSearch;

    private void DoSearchCheck()
    {
      _canSearch = false;
      Data = null;
      if (SelectedQueryItem.FUN_ID == null)
      {
        DialogService.ShowMessage(Properties.Resources.P1907020000_QueryFailByFuncIDNull);
        return;
      }
      _canSearch = true;
    }

    private void DoSearch()
    {
      if (!_canSearch) return;

      var watch = Stopwatch.StartNew();
      var proxy = new ExDataServices.P19WcfService.P19WcfServiceClient();

      var listParams = new List<object>();
      foreach (var p in QueryParamsList)
      {
        if (p.Item.PTYPE == "1")
        {
          var format = p.Item.FORMAT;
          format = format.Replace("YYYY", "yyyy").Replace("mm", "MM").Replace("DD", "dd");
          if (string.IsNullOrWhiteSpace(format)) format = "yyyy/MM/dd";
					listParams.Add(p.AddedProp == null || p.AddedProp == "" ? p.AddedProp : Convert.ToDateTime(p.AddedProp).ToString(format));
        }
        else
          listParams.Add(p.AddedProp);
      }

      var queryResult = RunWcfMethod(proxy.InnerChannel,
        () => proxy.GetQueryData(SelectedQueryItem.QID, listParams.ToArray()));

      if (queryResult.Item1 == null)
      {
        watch.Stop();
        DialogService.ShowMessage(Properties.Resources.P1907020000_QueryFail + queryResult.Item2);
        return;
      }

      var ds = queryResult.Item1;

      //置換欄位名稱
      var columnNames = GetBaseTableService.GetF000904List(FunctionCode, "P190702", "COLUMN_NAME");
      foreach (DataColumn column in ds.Tables[0].Columns)
      {
        var newColumName = columnNames.SingleOrDefault(p => p.Value.ToUpper() == column.ColumnName.ToUpper());
        if (newColumName != null)
          column.ColumnName = newColumName.Name;
      }

      Data = ds.Tables[0];
      watch.Stop();
      TimeSpan = watch.Elapsed;
    }
    #endregion Search

    #region Cancel
    private ICommand _cancelCommand;
    public ICommand CancelCommand
    {
      get
      {
        if (_cancelCommand == null)
        {
          _cancelCommand = new RelayCommand(() =>
          {
            _cancellationSource.Cancel();
            IsBusy = false;
          },
            () => IsBusy);
        }
        return _cancelCommand;
      }
    }
    #endregion Cancel

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

    #region Export
    public ICommand ExportCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoExport(), () => Data != null && Data.Rows.Count > 0
          );
      }
    }

    private void DoExport()
    {
      var saveFileDialog = new SaveFileDialog
      {
        DefaultExt = ".xlsx",
        Filter = "Excel (.xlsx)|*.xlsx",
        RestoreDirectory = true,
        OverwritePrompt = true,
        Title = Properties.Resources.P1905060000_DestSavePath,
        FileName = SelectedQueryItem.NAME
      };

      var isShowOk = saveFileDialog.ShowDialog();
      if (isShowOk != true) return;

      var excelExportService = new ExcelExportService();
      excelExportService.CreateNewSheet(SelectedQueryItem.NAME);
      var excelReportDataSource = new ExcelExportReportSource();
      excelReportDataSource.Data = Data;
      excelExportService.AddExportReportSource(excelReportDataSource);

      bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
          Path.GetFileName(saveFileDialog.FileName));

      DialogService.ShowMessage(isExportSuccess ? Properties.Resources.P1907020000_ExportQuertResult : Properties.Resources.P1907020000_ExportQueryResultFail);

    }
    #endregion

    #region ExportCsv
    public ICommand ExportCsvCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoExportCsv(), () => Data != null && Data.Rows.Count > 0
          );
      }
    }

    private void DoExportCsv()
    {
      var saveFileDialog = new SaveFileDialog
      {
        DefaultExt = "csv",
        Filter = "save files (*.csv)|*.csv",
        RestoreDirectory = true,
        OverwritePrompt = true,
        Title = Properties.Resources.P1905060000_DestSavePath,
        FileName = SelectedQueryItem.NAME
      };

      var isShowOk = saveFileDialog.ShowDialog();
      if (isShowOk != true) return;

      var f = new FileInfo(saveFileDialog.FileName);
      var isExportSuccess = true;
      try
      {
        Data.ExportDataAsCSVString(f.Directory + "/" + f.Name);
      }
      catch (Exception)
      {
        isExportSuccess = false;
      }

      DialogService.ShowMessage(isExportSuccess ? Properties.Resources.P1907020000_ExportQuertResult : Properties.Resources.P1907020000_ExportQueryResultFail);
    }
    #endregion

    #region ExportXml
    public ICommand ExportXmlCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoExportXml(), () => Data != null && Data.Rows.Count > 0
          );
      }
    }

    private void DoExportXml()
    {
      var saveFileDialog = new SaveFileDialog
      {
        DefaultExt = "xml",
        Filter = "save files (*.xml)|*.xml",
        RestoreDirectory = true,
        OverwritePrompt = true,
        Title = Properties.Resources.P1905060000_DestSavePath,
        FileName = SelectedQueryItem.NAME
      };

      var isShowOk = saveFileDialog.ShowDialog();
      if (isShowOk != true) return;

      var f = new FileInfo(saveFileDialog.FileName);
      var isExportSuccess = true;
      try
      {
        Data.DataSet.WriteXml(f.Directory + "/" + f.Name);
      }
      catch (Exception)
      {
        isExportSuccess = false;
      }

      DialogService.ShowMessage(isExportSuccess ? Properties.Resources.P1907020000_ExportQuertResult : Properties.Resources.P1907020000_ExportQueryResultFail);
    }
    #endregion

    #region Copy
    public ICommand CopyCommand
    {
      get
      {
        return new RelayCommand(
          () =>
          {
            IsBusy = true;
            try
            {
              DoCopy();
            }
            catch (Exception ex)
            {
              Exception = ex;
              IsBusy = false;
            }
            IsBusy = false;
          },
          () => !IsBusy && Data != null && Data.Rows.Count > 0);
      }
    }

    private void DoCopy()
    {
      var clipboardString = Data.ExportDataAsExcelString();
      Clipboard.Clear();
      Clipboard.SetData(DataFormats.Text, clipboardString);
      DialogService.ShowMessage(Properties.Resources.P1907020000_CopyComplete);
    }
		#endregion

		private void SelectedQueryParam_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//var item = sender as AddedPropertyItem<F190703, object>;
			//if(item.Item.FUN_ID == 5)
			//{
			//	var list = AllIdListDict;
			//	list["9"].Clear();
			//	AllIdListDict = list;
			//	QueryParamsList = QueryParamsList;
			//}
		}
	}

  public class QueryParamTemplateSelector : DataTemplateSelector
  {
    public DataTemplate TextBoxTemplate { get; set; }

    public DataTemplate DatePickerTemplate { get; set; }

    /// <summary>
    /// 顯示 ComboBox，但以 LIST_SQLID 的值當成 xml 顯示 ComboBox
    /// </summary>
    public DataTemplate Combo1Template { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var addedPropertyItem = item as AddedPropertyItem<F190703, object>;

      if (addedPropertyItem != null)
      {
        if (addedPropertyItem.Item.PTYPE == "0")
          return TextBoxTemplate;
        else if (addedPropertyItem.Item.PTYPE == "1")
          return DatePickerTemplate;
        //TODO
		else if (addedPropertyItem.Item.PTYPE == "2")
			return Combo1Template;
        else
          return TextBoxTemplate;
      }
      else
        return null;
    }
	

	}
}
