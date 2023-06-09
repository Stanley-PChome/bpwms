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
	public partial class P1901290000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		private bool isValid;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public P1901290000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;

				InitControls();
			}

		}

		private void InitControls()
		{
			SearchCommand.Execute(null);
		}

		#region Gup
		/// <summary>
		/// Gup 業主清單
		/// </summary>
		private List<F1929> _DataList;
		public List<F1929> DataList
		{
			get { return _DataList; }
			set
			{
				_DataList = value;
				RaisePropertyChanged();
			}
		}
		/// <summary>
		/// 存放選擇的Gup資訊
		/// </summary>
		private F1929 _selectedData;
		public F1929 SelectedData
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
		#endregion

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

		private void DoSearch()
		{
			var proxy = GetProxy<F19Entities>();
			DataList = proxy.F1929s.OrderBy(x => x.GUP_CODE).ToList();
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
					o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddCompleted()
					);
			}
		}

		private void DoAdd()
		{

		}
		private void DoAddCompleted()
		{
			F1929 newItem = new F1929();
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
			EditAction();
			UserOperateMode = OperateMode.Edit;
		}

		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{

			get
			{
				bool isCancel = false;
				return CreateBusyAsyncCommand(
									o => isCancel = DoCancel(), 
									() => UserOperateMode != OperateMode.Query,
									p => DoCancelCompleted(isCancel)
									);
			}
		}

		private bool DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				DoSearch();
				return true;
			}

			return false;
		}

		private void DoCancelCompleted(bool isCancel)
		{
			if (isCancel)
			{
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
			// 0.確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			// 1.檢查該筆資料是否存在
			var proxy = GetProxy<F19Entities>();
			var f1929s = proxy.F1929s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE).AsQueryable().ToList();
			bool isExist = (f1929s != null && f1929s.Count() > 0);
			// 1.1 不存在
			if (!isExist)
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return;
			}
			// 2.判斷F1909是否有指定該業主
			var f1909s = proxy.F1909s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE).AsQueryable().ToList();
			// 2.1 F1909指定該業主
			if (f1909s != null && f1909s.Count() > 0)
			{
				ShowMessage(Messages.WarningCannotDeleteGup);
				return;
			}
			// 3. 刪除
			var f1929 = f1929s.FirstOrDefault();
			proxy.DeleteObject(f1929);
			// 4.存檔
			proxy.SaveChanges();
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
			ExDataMapper.Trim(SelectedData);

			isValid = true;
			
			var error = GetEditableError(SelectedData);
			if (!string.IsNullOrWhiteSpace(error))
			{
				isValid = false;
				DialogService.ShowMessage(error);
				return;
			}

			//執行確認儲存動作
			if (UserOperateMode == OperateMode.Add)
				DoSaveAdd();
			else if (UserOperateMode == OperateMode.Edit)
				DoSaveEdit();
			UserOperateMode = OperateMode.Query;
		}

		string GetEditableError(F1929 e)
		{
			var proxy = GetProxy<F19Entities>();

			if ( string.IsNullOrWhiteSpace(e.GUP_CODE))
			{
				return Properties.Resources.P1901290000_GUP_CODENull;
			}

			if (!ValidateHelper.IsMatchAZaz09(e.GUP_CODE))
			{
				return Properties.Resources.P1901290000_GUP_CODE_ValidateCNWord;
			}

			if ( string.IsNullOrWhiteSpace(e.GUP_NAME))
			{
				return Properties.Resources.P1901290000_GUP_NAMENull;
			}

			if (string.IsNullOrWhiteSpace(e.SYS_GUP_CODE))
			{
				return Properties.Resources.P1901290000_SYS_GUP_CODENull;
			}

			if (!ValidateHelper.IsMatchAZaz09(e.SYS_GUP_CODE))
			{
				return Properties.Resources.P1901290000_SYS_GUP_CODE_ValidateCNWord;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (proxy.F1929s.Where(item => item.GUP_CODE == e.GUP_CODE || item.SYS_GUP_CODE == e.SYS_GUP_CODE).Count() > 0)
				{
					return Properties.Resources.P1901290000_GUP_CODE_SYS_GUP_CODE_Duplicate;
				}
			}
			else
			{
				if (proxy.F1929s.Where(item => item.GUP_CODE == e.GUP_CODE).Count() == 0)
				{
					return Properties.Resources.P1901290000_GupCodeNotExist;
				}

				if (proxy.F1929s.Where(item => item.GUP_CODE != e.GUP_CODE && item.SYS_GUP_CODE == e.SYS_GUP_CODE).Count() > 0)
				{
					return Properties.Resources.P1901290000_SYS_GUP_CODE_Duplicate;
				}
			}

			return string.Empty;
		}

		private void DoSaveAdd()
		{
			var proxy = GetModifyQueryProxy<F19Entities>();
			proxy.AddToF1929s(Mapper.DynamicMap<F1929>(SelectedData));
			proxy.SaveChanges();
			ShowMessage(Messages.InfoAddSuccess);
		}

		private void DoSaveEdit()
		{
			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1929 = proxy.F1929s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE).First();
			f1929.GUP_NAME = SelectedData.GUP_NAME;
			f1929.SYS_GUP_CODE = SelectedData.SYS_GUP_CODE;
			proxy.UpdateObject(f1929);
			proxy.SaveChanges();
			ShowMessage(Messages.InfoUpdateSuccess);
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
	}
}
