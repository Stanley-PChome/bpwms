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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using System.Windows;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Ex = Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using AutoMapper;


namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101050000_ViewModel : InputViewModelBase
	{
		public P7101050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}

		}
		#region **Prop
		private string _searchLocTypeId;
		public string SearchLocTypeId
		{
			get { return _searchLocTypeId; }
			set { _searchLocTypeId = value; RaisePropertyChanged("SearchLocTypeId"); }
		}

		private List<F1942> _reportList = new List<F1942>();
		/// <summary>
		/// 料架資料
		/// </summary>
		public List<F1942> ReportList
		{
			get { return _reportList; }
			set { _reportList = value; RaisePropertyChanged("ReportList"); }
		}

		private F1942 _selectedListItem;
		public F1942 SelectedListItem
		{
			get { return _selectedListItem; }
			set
			{
				if (_selectedListItem == value) return;
				_selectedListItem = value;
				SetItem(_selectedListItem);
				RaisePropertyChanged("SelectedListItem");
			}
		}
		/// <summary>
		/// 存放當前的料架資料
		/// </summary>
		private F1942 _currentRecord;
		public F1942 CurrentRecord
		{
			get { return _currentRecord; }
			set { _currentRecord = value; RaisePropertyChanged("CurrentRecord"); }
		}
		/// <summary>
		/// 原始料架資料
		/// </summary>
		private F1942 _orgRecord;
		public F1942 OrgRecord { get { return _orgRecord; } set { _orgRecord = value; } }
		/// <summary>
		/// 存檔料架資料
		/// </summary>
		private F1942 _newRecord;
		public F1942 NewRecord { get { return _newRecord; } set { _newRecord = value; } }

		public List<NameValuePair<string>> handyList { get; set; }

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
							o => DoSearch(), () => UserOperateMode == OperateMode.Query,
							o => SelectedListItem = (ReportList != null) ? ReportList.FirstOrDefault() : null
				  );
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxy = GetProxy<F19Entities>();

			ReportList = proxy.F1942s.Where(x => x.LOC_TYPE_ID.Equals(SearchLocTypeId) || string.IsNullOrEmpty(SearchLocTypeId))
		.OrderBy(x => x.LOC_TYPE_ID).ToList();

			if (ReportList == null || !ReportList.Any())
			{
				ShowMessage(Messages.InfoNoData);
				SelectedListItem = null;
				return;
			}
		}
		#endregion Search

		#region Add
		public Action OnFocusAdd = delegate { };
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
			// 如果有變更, 或是有新增時, 先確認是否繼續操作
			//if (ReportList.Any() && ConfirmToUpdate() == DialogResponse.Cancel) return;

			UserOperateMode = OperateMode.Add;
			//執行新增動作
			SetItem(new F1942());
			OnFocusAdd();
		}
		#endregion Add

		#region Edit
		public Action OnFocusEdit = delegate { };
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && (SelectedListItem != null)
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			OnFocusEdit();
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
			if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
			{
				return;
			}

			var item = SelectedListItem;
			SetItem(item);
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoDelete(), () => UserOperateMode == OperateMode.Query && (SelectedListItem != null),
							o => DoSaveComplete()
				  );
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			DeleteObj();
		}
		private void DeleteObj()
		{
			if (SelectedListItem == null) return;
			var proxyEx = GetExProxy<P71ExDataSource>();
			var result = proxyEx.CreateQuery<Ex.ExecuteResult>("Delete710105")
					.AddQueryOption("locTypeId", string.Format("'{0}'", SelectedListItem.LOC_TYPE_ID))
					.ToList();
			ShowMessage(result);
			// 刪除成功時重新載入資料
			NewRecord = (result.FirstOrDefault().IsSuccessed) ? ReportList.FirstOrDefault() : null;
			proxyEx = null;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
							o => DoSave(), () => UserOperateMode != OperateMode.Query,
							c => DoSaveComplete()
				  );
			}
		}

		private void DoSaveComplete()
		{
			//存檔失敗不重查
			if (ReportList.Any() && NewRecord == null) return;
			//重新查詢並指定至新增或修改之該筆資料
			DoSearch();
			SelectedListItem = (NewRecord != null) ? ReportList.Find(x => x.LOC_TYPE_ID == NewRecord.LOC_TYPE_ID) : ReportList.FirstOrDefault();
		}

		private void DoSave()
		{
			//執行確認儲存動作
			if (SaveObj())
				UserOperateMode = OperateMode.Query;
		}
		private bool SaveObj()
		{
			//檢查必填欄位
			if (!IsValid()) 
				return false;
			//資料儲存
			var locType = new F1942Ex()
			{
				LOC_TYPE_ID = CurrentRecord.LOC_TYPE_ID,
				LOC_TYPE_NAME = CurrentRecord.LOC_TYPE_NAME,
				LENGTH = CurrentRecord.LENGTH,
				DEPTH = CurrentRecord.DEPTH,
				HEIGHT = CurrentRecord.HEIGHT,
				WEIGHT = (decimal)CurrentRecord.WEIGHT,
				CRT_STAFF = Wms3plSession.CurrentUserInfo.Account,
				CRT_DATE = DateTime.Now,
				VOLUME_RATE = CurrentRecord.VOLUME_RATE,
				HANDY = CurrentRecord.HANDY
			};
			var proxy = new wcf.P71WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.SaveP710105(locType, (UserOperateMode == OperateMode.Add)));
			if (result.IsSuccessed)
			{
                ShowResultMessage(result);
				NewRecord = CurrentRecord;
				return true;
			}
            ShowResultMessage(result);
			NewRecord = null;
			return false;
		}
		#endregion Save

		#region **Func
		private void Init()
		{
			this.SearchCommand.Execute(null);
			handyList = new List<NameValuePair<string>>() 
										{  
											new NameValuePair<string>(){ Name = "1", Value = "1"},
											new NameValuePair<string>(){ Name = "2", Value = "2"},
											new NameValuePair<string>(){ Name = "3", Value = "3"}
										};
		}

		private void SetItem(F1942 item = null)
		{
			var tmp = (item ?? ReportList.FirstOrDefault());
			// 先清空資料
			CurrentRecord = null;
			// 設定要顯示的資料
			if (tmp != null)
				CurrentRecord = Mapper.DynamicMap<F1942>(tmp);

			// 將原始資料備份起來, 以備做資料是否有編輯的檢查
			if (CurrentRecord != null)
				OrgRecord = Mapper.DynamicMap<F1942>(CurrentRecord);
		}



		private bool IsValid()
		{
			StringBuilder MsgSb = new StringBuilder();
			if (string.IsNullOrEmpty(CurrentRecord.LOC_TYPE_ID)) MsgSb.Append(Properties.Resources.P7101050000_ViewModel_LOC_TYPE_ID_Reequired).Append(Environment.NewLine);
			if (string.IsNullOrEmpty(CurrentRecord.LOC_TYPE_NAME)) MsgSb.Append(Properties.Resources.P7101050000_ViewModel_LOC_TYPE_NAME_Required).Append(Environment.NewLine);
			string Msg = MsgSb.ToString();
			if (!string.IsNullOrEmpty(Msg))
			{
				DialogService.ShowMessage(Msg);
				return false;
			}

			if (!ValidateHelper.IsMatchAZaz09(CurrentRecord.LOC_TYPE_ID))
			{
				DialogService.ShowMessage(Properties.Resources.P7101050000_ViewModel_LOC_TYPE_ID_CNWordOnly);
				return false;
			}


			return true;
		}

		private DialogResponse ConfirmToUpdate()
		{
			DialogResponse dr = DialogResponse.Yes;
			DataModifyType dmt = IsDataModified();
			if (dmt == DataModifyType.Modified || dmt == DataModifyType.New)
			{
				dr = ShowMessage(Messages.WarningBeforeAdd);
				if (dr == DialogResponse.Yes)
				{
					// 佔測到有修改, 並且同意先儲存時, 要做儲存資料的動作
					DoSave();
					//DoSearch();
				}
			}
			return dr;
		}
		/// <summary>
		/// 操作之前先檢查資料是否有被更改, 以及是否被刪除
		/// </summary>
		/// <returns></returns>
		private DataModifyType IsDataModified()
		{
			if (UserOperateMode == OperateMode.Add) return DataModifyType.New;
			if (CurrentRecord.LOC_TYPE_ID != OrgRecord.LOC_TYPE_ID ||
					CurrentRecord.LOC_TYPE_NAME != OrgRecord.LOC_TYPE_NAME ||
					CurrentRecord.HEIGHT != OrgRecord.HEIGHT || CurrentRecord.LENGTH != OrgRecord.LENGTH ||
					CurrentRecord.DEPTH != OrgRecord.DEPTH || CurrentRecord.WEIGHT != OrgRecord.WEIGHT ||
					CurrentRecord.VOLUME_RATE != OrgRecord.VOLUME_RATE || CurrentRecord.HANDY != OrgRecord.HANDY)
				return DataModifyType.Modified;

			return DataModifyType.NotModified;
		}

		#endregion
	}
	public enum DataModifyType
	{
		Deleted, Modified, NotModified, New
	}
}
