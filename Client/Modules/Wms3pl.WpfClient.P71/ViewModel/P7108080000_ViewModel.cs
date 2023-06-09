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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;

namespace Wms3pl.WpfClient.P71.ViewModel
{
  public partial class P7108080000_ViewModel : InputViewModelBase
  {
    public P7108080000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
				InitControls();
      }

    }

		#region Properties,Fields..
		private string _gupCode;
		private string _custCode;

		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{
				_selectedDc = value;
				if (SearchDelvDate != null && !string.IsNullOrEmpty(SearchPickTime)) SearchCommand.Execute(null);
			}
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList.OrderBy(x => x.Value).ToList(); }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion

		#region 查詢條件
		/// <summary>
		/// 批次日期
		/// </summary>
		private DateTime _searchDelvDate;
		public DateTime SearchDelvDate
		{
			get { return _searchDelvDate; }
			set
			{
				_searchDelvDate = value;
				if (value != null) DoSearchPickTime();
				RaisePropertyChanged("SearchDelvDate");
			}
		}
		/// <summary>
		/// 批次時段
		/// </summary>
		private string _searchPickTime;
		public string SearchPickTime
		{
			get { return _searchPickTime; }
			set
			{
				_searchPickTime = value;
				RaisePropertyChanged("SearchPickTime");
			}
		}
		#endregion

		#region 揀貨時間
		/// <summary>
		/// 揀貨時間清單
		/// </summary>
		private List<NameValuePair<string>> _pickTimeList;
		public List<NameValuePair<string>> PickTimeList { get { return _pickTimeList; } set { _pickTimeList = value; RaisePropertyChanged("PickTimeList"); } }
		#endregion

		#region 訂單處理進度清單
		private List<F051201Progress> _dgList;

		public List<F051201Progress> DgList
		{
			get { return _dgList; }
			set
			{
				if (_dgList == value)
					return;
				Set(() => DgList, ref _dgList, value);
			}
		}
		#endregion
		#region 選擇的訂單處理進度
		private F051201Progress _selectedData;

		public F051201Progress SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData == value)
					return;
				SearchDetailCommand.Execute(null);
				Set(() => SelectedData, ref _selectedData, value);
			}
		}
		#endregion


		#region 選擇的訂單明細清單
		private List<F050301ProgressData> _dgDetailList;

		public List<F050301ProgressData> DgDetailList
		{
			get { return _dgDetailList; }
			set
			{
				if (_dgDetailList == value)
					return;
				Set(() => DgDetailList, ref _dgDetailList, value);
			}
		}
		#endregion
		
		
		#endregion

		#region Functions
		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
			SearchDelvDate = DateTime.Today;
		}
		private void DoSearchPickTime()
		{
			var proxy = GetProxy<F05Entities>();
			var results = proxy.F0513s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode &&
															(x.DELV_DATE != null && x.DELV_DATE == SearchDelvDate.Date))
												 .Select(x => new NameValuePair<string> { Name = x.PICK_TIME, Value = x.PICK_TIME })
												 .AsQueryable().ToList();
			if (results == null || !results.Any())
			{
				ShowMessage(Messages.PickTimeNoData);
				SetClear();
				return;
			}
			PickTimeList = results;
		}
		private void SetClear()
		{
			PickTimeList = null;
			SearchPickTime = string.Empty;
		}
		#endregion

		#region Commands
		#region Search
		public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchCompleted()
          );
      }
    }

		private void DoSearchCompleted()
		{
			if (DgList != null && DgList.Any())
				SelectedData = DgList.FirstOrDefault();
		}

    private void DoSearch()
    {
      //執行查詢動
			//0. 檢查必填欄位
			if (!isSearchValid()) return;
			//1. 取得合約主檔資料
			DgList = GetF051201ProgressDatas();
    }

		private List<F051201Progress> GetF051201ProgressDatas()
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F051201Progress>("GetOrderProcessProgress")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
				.AddQueryOption("pickTime", string.Format("'{0}'", SearchPickTime))
				.AddQueryOption("delvDate", string.Format("'{0}'", SearchDelvDate.ToString("yyyy/MM/dd")))
				.ToList();
			if (results == null || !results.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return null;
			}
			return results;
		}

		private bool isSearchValid()
		{
			if (string.IsNullOrEmpty(SelectedDc))
			{
				if (DcList != null && DcList.Any())
					SelectedDc = DcList.First().Value;
				//ShowMessage(new MessagesStruct() { Message = Properties.Resources.P7105020000_ViewModel_SelectDC, Title = Resources.Resources.Information });
				//return false;
			}
			if (SearchDelvDate == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P7108080000_ViewModel_SelectBatchDate, Title = Resources.Resources.Information });
				return false;
			}
			if (string.IsNullOrEmpty(SearchPickTime))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P7108080000_ViewModel_SelectBatchTime, Title = Resources.Resources.Information });
				return false;
			}
			return true;
		}
    #endregion Search

		#region SearchDetail
		public ICommand SearchDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchDetail(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearchDetail()
		{
			//執行查詢動
			if (SelectedData != null)
				DgDetailList = GetF050301ProgressDatas();
		}

		private List<F050301ProgressData> GetF050301ProgressDatas()
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var results = proxy.CreateQuery<F050301ProgressData>("GetProgressData")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
				.AddQueryOption("pickTime", string.Format("'{0}'", SearchPickTime))
				.AddQueryOption("delvDate", string.Format("'{0}'", SearchDelvDate.ToString("yyyy/MM/dd")))
				.AddQueryOption("pickOrdNo", string.Format("'{0}'", SelectedData.PICK_ORD_NO))
				.ToList();
			if (results == null || !results.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return null;
			}
			return results;
		}

		#endregion SearchDetail

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
		#endregion
	}
}
