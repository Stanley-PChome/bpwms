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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
  public partial class P1907010000_ViewModel : InputViewModelBase
  {
    private string _userId;
    private string _userName;
    private readonly F19Entities _proxy;
    public P1907010000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        _proxy = GetProxy<F19Entities>();
        InitControls();
      }

    }

    private void InitControls()
    {
      GetQueries();

      GetGroupList();
      if (GroupList.Any())
        SelectedGroupItem = GroupList.First();
    }

    private F1953 _selectedGroupItem;
    public F1953 SelectedGroupItem
    {
      get
      {
        return _selectedGroupItem;
      }
      set
      {
        _selectedGroupItem = value;
        SetSelectedQueryByGroup();
        RaisePropertyChanged("SelectedGroupItem");
      }
    }

    /// <summary>
    /// 工作群組清單
    /// </summary>
    private List<F1953> _groupList;
    public List<F1953> GroupList
    {
      get { return _groupList; }
      set
      {
        if (_groupList == value) return;
        Set(() => GroupList, ref _groupList, value);
      }
    }

    private void GetGroupList()
    {
      GroupList = _proxy.F1953s.Where(x => x.ISDELETED == "0")
        .OrderBy(x => x.GRP_ID).ToList();
    }

    private SelectionList<F190701> _queries;
    public SelectionList<F190701> Queries
    {
      get { return _queries; }
      set
      {
        if (_queries == value) return;
        Set(() => Queries, ref _queries, value);
      }
    }

    private void GetQueries()
    {
      Queries = _proxy.F190701s.OrderBy(x => x.NAME).ToSelectionList();
    }

    private void SetSelectedQueryByGroup()
    {
      var results = _proxy.CreateQuery<F190701>("GetQueryListByGroupId")
        .AddQueryOption("gid", string.Format("'{0}'", SelectedGroupItem.GRP_ID))
        .ToList().Select(x => x.QID).ToList();
      foreach (var q in Queries)
        q.IsSelected = results.Contains(q.Item.QID);
      IsCheckedAll = (Queries.Count == results.Count);
    }

    private bool _isCheckedAll;
    public bool IsCheckedAll
    {
      get { return _isCheckedAll; }
      set
      {
        if (_isCheckedAll == value) return;
        Set(() => IsCheckedAll, ref _isCheckedAll, value);
      }
    }

    public ICommand CheckedAllCommand
    {
      get
      {
        return new RelayCommand(CheckedAll, () => (Queries != null && Queries.Any() && SelectedGroupItem != null));
      }
    }

    private void CheckedAll()
    {
      foreach (var item in Queries)
      {
        item.IsSelected = IsCheckedAll;
      }
    }

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => false
          );
      }
    }

    private void DoSearch()
    {

    }
    #endregion Search

    #region Save
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSave(), () => (Queries != null && Queries.Any() && SelectedGroupItem != null)
          );
      }
    }

    private void DoSave()
    {
      if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
        return;

      var listQid = Queries.Where(x => x.IsSelected).Select(x => x.Item.QID.ToString()).ToList();

      var proxyEx = new ExDataServices.P19WcfService.P19WcfServiceClient();
      wcf.ExecuteResult result = RunWcfMethod<wcf.ExecuteResult>(proxyEx.InnerChannel,
        () => proxyEx.SaveData(SelectedGroupItem.GRP_ID.ToString(), _userId, _userName, listQid.ToArray()));

      if (!result.IsSuccessed)
      {
        ShowWarningMessage(string.IsNullOrWhiteSpace(result.Message) ? Messages.Failed.Message : result.Message);
        return;
      }
      ShowMessage(Messages.Success);

      UserOperateMode = OperateMode.Query;
    }
    #endregion Save
  }
}
