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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1902060000_ViewModel : InputViewModelBase
	{
		private readonly F19Entities _proxy;
		private string _userId;
		private string _userName;
		private bool isValid;
		private F1903 isExistItem;
		private F1903 f1903;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };

		public P1902060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_proxy = GetProxy<F19Entities>();
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				CheckItemList = _proxy.F1983s.OrderBy(x => x.CHECK_NO).ToObservableCollection();
				CheckItemTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F190206", "CHECK_TYPE");
				InitControls();
			}
		}

		private void InitControls()
		{
			if (!string.IsNullOrWhiteSpace(ITEM_CODE))
			{
				SearchCommand.Execute(null);
			}
		}

		/// <summary>
		/// 每次搜尋後，會記憶搜尋的 ItemCode，用於新增與刪除
		/// </summary>
		private string _selectedItemCode;

		private string _itemCode = string.Empty;
		public string ITEM_CODE
		{
			get
			{
				return _itemCode ?? (_itemCode = string.Empty);
			}
			set
			{
				Set(() => ITEM_CODE, ref _itemCode, value);
			}
		}

		private string _custCode;
		public string CUST_CODE
		{
			get { return _custCode; }
			set
			{
				_custCode = value;
				RaisePropertyChanged();
			}
		}

		private string _checkType_before;
		public string CHECKTYPE_BEFORE
		{
			get { return _checkType_before; }
			set
			{
				_checkType_before = value;
				RaisePropertyChanged();
			}
		}

		private string _checkNo_before;
		public string CHECKNO_BEFORE
		{
			get { return _checkNo_before; }
			set
			{
				_checkNo_before = value;
				RaisePropertyChanged();
			}
		}

		private string _gupCode;
		public string GUP_CODE
		{
			get { return _gupCode; }
			set
			{
				_gupCode = value;
				RaisePropertyChanged();
			}
		}

		#region 檢驗項目下拉選單選項

		private List<NameValuePair<string>> _checkItemTypeList;

		public List<NameValuePair<string>> CheckItemTypeList
		{
			get { return _checkItemTypeList; }
			set
			{
				Set(() => CheckItemTypeList, ref _checkItemTypeList, value);
			}
		}

		private ObservableCollection<F1983> _checkItemList;
		public ObservableCollection<F1983> CheckItemList
		{
			get { return _checkItemList; }
			set { _checkItemList = value; RaisePropertyChanged("CheckItemList"); }
		}

		#endregion

		private List<F190206> _DataList;
		public List<F190206> DataList
		{
			get { return _DataList; }
			set
			{
				_DataList = value;
				RaisePropertyChanged();
			}
		}

		private F190206 _selectedData;

		public F190206 SelectedData
		{
			get { return _selectedData; }
			set
			{
				if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
				{
					//ShowMessage(Properties.Resources.P1901090100_UnSelectableStatus);
					return;
				}
				else
				{
					_selectedData = value;
					RaisePropertyChanged("SelectedData");
				}
			}
		}

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
			//執行查詢動
			DataList = new List<F190206>();
			//if (IsValidItemCode(ITEM_CODE) == false)
			//{
			//	DialogService.ShowMessage(Properties.Resources.P1902060000_Item_OnlyCNWord);
			//	return;
			//}
			ITEM_CODE = ITEM_CODE.Trim();

			isExistItem = _proxy.F1903s.Where(x => x.ITEM_CODE.Equals(ITEM_CODE) && x.GUP_CODE.Equals(GUP_CODE) && x.CUST_CODE == CUST_CODE).FirstOrDefault();
			if (isExistItem == null)
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
			else
			{
				_selectedItemCode = ITEM_CODE;

				f1903 = _proxy.F1903s.Where(x => x.ITEM_CODE.Equals(ITEM_CODE) && x.GUP_CODE.Equals(GUP_CODE) && x.CUST_CODE.Equals(CUST_CODE)).FirstOrDefault();
				if (f1903 == null)
				{
					ShowMessage(Messages.InfoNoData);
					return;
				}
			}

			DataList = _proxy.F190206s.Where(x => x.GUP_CODE.Equals(GUP_CODE) && x.CUST_CODE.Equals(CUST_CODE) && x.ITEM_CODE.Equals(ITEM_CODE)).OrderBy(x => x.ITEM_CODE).ToList();
			if (DataList == null || !DataList.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
		}

		private void DoSearchCompleted()
		{
			if (DataList == null || !DataList.Any()) return;
			SelectedData = DataList.FirstOrDefault();
			SearchAction();
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query && isExistItem != null,
					o => DoAddCompleted()
					);
			}
		}

		private void DoAdd()
		{
		}

		private void DoAddCompleted()
		{
			if (string.IsNullOrWhiteSpace(ITEM_CODE))
			{
				DialogService.ShowMessage(Properties.Resources.P1902060000_InputItemCode);
				return;
			}

			ITEM_CODE = _selectedItemCode;

			F190206 newItem = new F190206();
			DataList.Add(newItem);
			DataList = DataList.ToList();
			SelectedData = newItem;
			AddAction();
			UserOperateMode = OperateMode.Add;
		}


		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DataList.Any()), o => DoEditCompleted()
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
		}

		private void DoEditCompleted()
		{
			if (SelectedData != null)
			{
				CHECKNO_BEFORE = SelectedData.CHECK_NO;
				CHECKTYPE_BEFORE = SelectedData.CHECK_TYPE;
			}
			EditAction();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
									o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => DoCancelCompleted()
									);
			}
		}

		private void DoCancel()
		{
		}

		private void DoCancelCompleted()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				DoSearch();
				SearchAction();
				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
					o => DoDeleteCompleted()
					);
			}
		}

		private void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

			var pier = _proxy.F190206s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.ITEM_CODE.Equals(ITEM_CODE) && x.CHECK_NO.Equals(SelectedData.CHECK_NO) && x.CHECK_TYPE.Equals(SelectedData.CHECK_TYPE)).ToList().FirstOrDefault();

			if (pier == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1902060000_NoItemCheckData);
				return;
			}
			else
			{
				_proxy.DeleteObject(pier);
			}
			_proxy.SaveChanges();
			ShowMessage(Messages.Success);
		}
		private void DoDeleteCompleted()
		{
			UserOperateMode = OperateMode.Query;
			SearchCommand.Execute(null);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query, p => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;

			isValid = true;
			if (string.IsNullOrWhiteSpace(SelectedData.CHECK_TYPE))
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1902060000_InputCheckItemType);
				return;
			}
			if (string.IsNullOrEmpty(SelectedData.CHECK_NO))
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1902060000_InputCheckItem);
				return;
			}
			if (UserOperateMode == OperateMode.Add)
			{
				var pier = _proxy.F190206s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.ITEM_CODE.Equals(ITEM_CODE) && x.CHECK_NO.Equals(SelectedData.CHECK_NO) && x.CHECK_TYPE.Equals(SelectedData.CHECK_TYPE)).Count();
				if (pier != 0)
				{
					isValid = false;
					DialogService.ShowMessage(Properties.Resources.P1902060000_CheckItemDuplicate);
					return;
				}
			} if (UserOperateMode == OperateMode.Edit)
			{
				if (!CHECKTYPE_BEFORE.Equals(SelectedData.CHECK_TYPE) || !CHECKNO_BEFORE.Equals(SelectedData.CHECK_NO))
				{
					var pier = _proxy.F190206s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.ITEM_CODE.Equals(ITEM_CODE) && x.CHECK_NO.Equals(SelectedData.CHECK_NO) && x.CHECK_TYPE.Equals(SelectedData.CHECK_TYPE)).Count();
					if (pier != 0)
					{
						isValid = false;
						DialogService.ShowMessage(Properties.Resources.P1902060000_CheckItemDuplicate);
						return;
					}
				}
			}
			//執行確認儲存動作
			if (UserOperateMode == OperateMode.Add)
				DoSaveAdd();
			else if (UserOperateMode == OperateMode.Edit)
				DoSaveEdit();
			UserOperateMode = OperateMode.Query;

		}

		private void DoSaveAdd()
		{
			var f190206 = new F190206();
			f190206.ITEM_CODE = ITEM_CODE;
			f190206.CHECK_TYPE = SelectedData.CHECK_TYPE;
			f190206.CHECK_NO = SelectedData.CHECK_NO; ;
			f190206.CUST_CODE = _custCode;
			f190206.GUP_CODE = _gupCode;
			_proxy.AddToF190206s(f190206);
			_proxy.SaveChanges();
			ShowMessage(Messages.Success2);
		}

		private void DoSaveEdit()
		{
			var updProxy = GetProxy<F19Entities>();
			var updData = updProxy.F190206s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.ITEM_CODE.Equals(ITEM_CODE) && x.CHECK_NO.Equals(CHECKNO_BEFORE) && x.CHECK_TYPE.Equals(CHECKTYPE_BEFORE)).FirstOrDefault();
			updData.CHECK_TYPE = SelectedData.CHECK_TYPE;
			updData.CHECK_NO = SelectedData.CHECK_NO;
			updData.UPD_STAFF = _userId;
			updData.UPD_NAME = _userName;
			updData.UPD_DATE = DateTime.Now;
			updProxy.UpdateObject(updData);
			updProxy.SaveChanges();
			ShowMessage(Messages.Success2);
		}

		private void DoSaveCompleted()
		{
			if (isValid == true)
			{
				UserOperateMode = OperateMode.Query;
				SearchCommand.Execute(null);
			}

		}
		#endregion Save

		public bool IsValidItemCode(String itemCode)
		{
			if (string.IsNullOrEmpty(itemCode))
				return false;
			else
			{
				System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex("^[A-Za-z0-9]+$");
				return reg1.IsMatch(itemCode);
			}
		}

	}
}
