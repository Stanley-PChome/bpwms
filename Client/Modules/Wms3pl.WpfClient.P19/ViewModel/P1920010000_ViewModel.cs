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
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1920010000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		public P1920010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			SearchCommand.Execute(null);
		}

		#region CheckItem
		/// <summary>
		/// 檢驗項目清單
		/// </summary>
		private List<F1983> _checkItemList;
		public List<F1983> CheckItemList { get { return _checkItemList; } set { _checkItemList = value; RaisePropertyChanged("CheckItemList"); } }
		/// <summary>
		/// 存放選擇的檢驗項目資訊
		/// </summary>
		private F1983 _selectData;
		public F1983 SelectData { get { return _selectData; } set { _selectData = value; RaisePropertyChanged("SelectData"); } }
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchComplete()
					);
			}
		}

		private void DoSearchComplete()
		{
			if (CheckItemList == null || !CheckItemList.Any()) return;
			SelectData = CheckItemList.FirstOrDefault();
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxy = GetProxy<F19Entities>();
			CheckItemList = proxy.F1983s.OrderBy(x => x.CHECK_NO).ToList();
			if (CheckItemList == null ||! CheckItemList.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
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
			SelectData = new F1983();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectData != null && CheckItemList.Any())
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
					o => DoCancel(), () => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			//重查
			DoSearch();
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			// 0.確認是否刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			// 1.檢核資料是否存在
			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1983s = proxy.F1983s.Where(x => x.CHECK_NO == SelectData.CHECK_NO).AsQueryable().ToList();
			var isExist = (f1983s != null && f1983s.Count() > 0);
			// 1.1 不存在
			if (!isExist)
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return;
			}
			// 2.判斷是否有關連到F190206(不可刪除)
			var f190206s = proxy.F190206s.Where(x => x.CHECK_NO == SelectData.CHECK_NO).AsQueryable().ToList();
			if (f190206s != null && f190206s.Any())
			{
				ShowMessage(Messages.WarningCannotDeleteCheckItem);
				return;
			}
			// 3.刪除
			var f1983 = f1983s.FirstOrDefault();
			proxy.DeleteObject(f1983);
			// 4.存檔
			proxy.SaveChanges();
			// 5.重查
			DoSearch();
			// 6.顯示成功訊息
			ShowMessage(Messages.Success);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			// 0.檢核必填
			if (!isValid()) return;
			// 1.查看資料是否存在
			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1983s = proxy.F1983s.Where(x => x.CHECK_NO == SelectData.CHECK_NO).AsQueryable().ToList();
			var isExist = (f1983s != null && f1983s.Count() > 0);
			if (UserOperateMode == OperateMode.Add)
			{
				// 2.新增
				// 2.1 已存在
				if (isExist)
				{
					ShowMessage(Messages.WarningExist);
					return;
				}
				SelectData = SetBasicInfo(SelectData);
				proxy.AddToF1983s(SelectData);
			}
			else
			{
				// 3.更新
				// 3.1 不存在
				if (!isExist)
				{
					ShowMessage(Messages.WarningBeenDeleted);
					return;
				}
				var f1983 = f1983s.FirstOrDefault();
				f1983 = SetBasicInfo(f1983);
				f1983.CHECK_NO = SelectData.CHECK_NO;
				f1983.CHECK_NAME = SelectData.CHECK_NAME;
				proxy.UpdateObject(f1983);
			}
			// 4.存檔
			proxy.SaveChanges();
			// 5.重查
			DoSearch();
			// 6.顯示成功訊息
			ShowMessage(Messages.Success);
			UserOperateMode = OperateMode.Query;
		}

		private F1983 SetBasicInfo(F1983 obj)
		{
			if (UserOperateMode == OperateMode.Add)
			{
				obj.CRT_STAFF = _userId;
				obj.CRT_NAME = _userName;
				obj.CRT_DATE = DateTime.Now;
			}
			obj.UPD_STAFF = _userId;
			obj.UPD_NAME = _userName;
			obj.UPD_DATE = DateTime.Now;
			return obj;
		}

		private bool isValid()
		{
			if (string.IsNullOrEmpty(SelectData.CHECK_NO))
			{
				ShowMessage(Messages.WarningNoCheckNo);
				return false;
			}
			if (string .IsNullOrEmpty(SelectData.CHECK_NAME))
			{
				ShowMessage(Messages.WarningNoCheckName);
				return false;
			}
			return true;
		}
		private void DoSaveComplete()
		{
			//指定至該筆資料
			if (CheckItemList == null || !CheckItemList.Any()) return;
			if (SelectData == null || !CheckItemList.Any(x => x.CHECK_NO == SelectData.CHECK_NO))
				SelectData = CheckItemList.FirstOrDefault();
			else
				SelectData = CheckItemList.Find(x => x.CHECK_NO == SelectData.CHECK_NO);
		}
		#endregion Save
	}
}
