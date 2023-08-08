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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P25WcfService;
using System.Security.Permissions;
using System.Windows;
using System.IO;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.P25.ViewModel
{
	public partial class P2503030000_ViewModel : InputViewModelBase
	{
		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		public P2503030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				QueryData = new F2501();
				//初始化執行時所需的值及資料	
				StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F2501", "STATUS", true);     //設定序號狀態
				QueryData.STATUS = "";                              //預設下拉全部
			}
		}

		#region 序號狀態參數
		private List<NameValuePair<string>> _statusList;
		public List<NameValuePair<string>> StatusList
		{
			get { return _statusList; }
			set
			{
				_statusList = value;
				RaisePropertyChanged("StatusList");
			}
		}
		#endregion

		#region 查詢條件Class參數
		private F2501 _queryData;
		public F2501 QueryData
		{
			get { return _queryData; }
			set
			{
				_queryData = value;
				RaisePropertyChanged("QueryData");
			}
		}
		#endregion

		#region GV查詢 DGList
		private SelectionList<F2501QueryData> _queryResultList;

		public SelectionList<F2501QueryData> QueryResultList
		{
			get { return _queryResultList; }
			set
			{
				Set(() => QueryResultList, ref _queryResultList, value);
			}
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}
		#endregion

		#region CheckAll
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
			if (QueryResultList != null)
			{
				foreach (var p in QueryResultList.Where(w => w.Item.STATUS == "A1" && w.Item.BUNDLE_SERIALLOC !="1"))
				{
					p.IsSelected = IsSelectedAll;
				}
			}
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
		private void DoSearch()
		{
			if (string.IsNullOrEmpty(QueryData.ITEM_CODE) && string.IsNullOrEmpty(QueryData.SERIAL_NO))
			{
				DialogService.ShowMessage("品號、序號 至少輸入一項查詢條件");
			}
			else
			{			   
        var wcfproxy = GetWcfProxy<wcf.P25WcfServiceClient>();       
          var DgQueryDataF250101 = wcfproxy.RunWcfMethod(w => w.Get2501QueryData(_gupCode, _custCode
          , string.Format("{0}", StringHelper.JoinSplitDistinct(QueryData.ITEM_CODE, ","))
           , string.Format("{0}", "")   
           , string.Format("{0}", "")
          , string.Format("{0}", StringHelper.JoinSplitDistinct(QueryData.SERIAL_NO, ","))       
          , string.Format("{0}", "")
          , string.Format("{0}", "")
          , string.Format("{0}", StringHelper.JoinSplitDistinct("", ","))
          , string.Format("{0}", string.IsNullOrEmpty(QueryData.STATUS) ? null : QueryData.STATUS)
          , string.Format("{0}", "")
          , string.Format("{0}", "")        
          , short.Parse("0")
          , string.Format("{0}", "")
          , string.Format("{0}", "")
          , string.Format("{0}", "")
          , string.Format("{0}", "")
          , string.Format("{0}", "")));      
       var f2501QueryData = ExDataMapper.MapCollection<wcf.F2501QueryData, ExDataServices.P25ExDataService.F2501QueryData>(DgQueryDataF250101).ToObservableCollection();


        if ((f2501QueryData == null || !f2501QueryData.Any()))
				{
					ShowMessage(Messages.InfoNoData);
				}

				QueryResultList = new SelectionList<F2501QueryData>(f2501QueryData, false);
			}
		}
		private void DoSearchCompleted()
		{

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
					o => DoDelete(), () => CanDelete()
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			var proxy = new wcf.P25WcfServiceClient();
			var deleteData = QueryResultList.Where(x => x.IsSelected && x.Item.STATUS == "A1" && x.Item.BUNDLE_SERIALLOC != "1").Select(s => s.Item.SERIAL_NO);
			if (!deleteData.ToList().Any())
			{
				ShowWarningMessage("請選擇刪除資料!!");
				return;
			}
      string err_msg = null;

       var BUNDLE_SERIALLOCs = QueryResultList.Where(x => x.IsSelected && x.Item.STATUS == "A1" && x.Item.BUNDLE_SERIALLOC=="1").Select(s => s.Item.ITEM_CODE).Distinct();
      if (BUNDLE_SERIALLOCs.ToList().Any())
      {    
       
        ShowWarningMessage(string.Format("此商品{0}為序號綁儲位商品，商品序號狀態為進貨，不可刪除!!", BUNDLE_SERIALLOCs.First()));
        return;
      }
      var Ststus_C1s = QueryResultList.Where(x => x.IsSelected && x.Item.STATUS == "C1" && string.IsNullOrWhiteSpace(x.Item.PO_NO) ).Select(s => s.Item.SERIAL_NO).Distinct();
      if (Ststus_C1s.ToList().Any())
      {
        ShowWarningMessage(string.Format("此商品序號{0}狀態為出貨，不可刪除!!", Ststus_C1s.First()));
        return;
      }
      var Ststus_D2s = QueryResultList.Where(x => x.IsSelected && x.Item.STATUS == "D2" && string.IsNullOrWhiteSpace(x.Item.PO_NO)).Select(s => s.Item.SERIAL_NO).Distinct();
      if (Ststus_D2s.ToList().Any())
      {
        ShowWarningMessage(string.Format("此商品序號{0}狀態為報廢，不可刪除!!", Ststus_D2s.First()));
        return;
      }
      var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteSerialNo(_gupCode, _custCode, deleteData.ToArray()));
			if (result.IsSuccessed)
			{				
				ShowMessage(Messages.DeleteSuccess);
        DoSearch();
			}
		}
    #endregion Delete

    private bool CanDelete()
    {
      if (QueryResultList != null)
      {
        var deleteData = QueryResultList.Where(x => x.IsSelected && x.Item.STATUS == "A1" && x.Item.BUNDLE_SERIALLOC != "1").Select(s => s.Item.SERIAL_NO);
                return deleteData.ToList().Any();
      }
      else
        return false;
    }

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

	}
}
