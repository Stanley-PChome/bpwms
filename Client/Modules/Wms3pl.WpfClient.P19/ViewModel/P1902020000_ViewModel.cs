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

using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1902020000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private bool _isInit = true;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };

		#region 查詢條件
		#region Form - 原因類別編號
		private string _txtuct_ID;

		public string txtUct_ID
		{
			get{return _txtuct_ID;}
			set
			{
				_txtuct_ID=value;
				RaisePropertyChanged("txtUct_ID");
			}
		}
		#endregion

		#region Form - 原因類別名稱
		private string _txttype_Desc;

		public string txtType_Desc
		{
			get { return _txttype_Desc; }
			set
			{
				_txttype_Desc = value;
				RaisePropertyChanged("txtType_Desc");
			}
		}
		#endregion
		#endregion

		#region Data 原因類別檔清單
		private ObservableCollection<F1950> _dgList;
		public ObservableCollection<F1950> DgList
		{
			get { return _dgList; }
			set { _dgList = value; RaisePropertyChanged("DgList"); }
		}
		#endregion

		#region Grid資料選取
		private F1950 _selectedData;

		public F1950 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");

				if (value != null && !string.IsNullOrWhiteSpace(value.UCT_ID))
				{
					_lastSelectedUctId = value.UCT_ID;
				}
			}
			
		}

		private string _lastSelectedUctId = string.Empty;
		#endregion
		#endregion

		#region 函式
		public P1902020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料				
				InitControls();
			}
		}

		private void InitControls()
		{
			SearchCommand.Execute(null);
			_isInit = false;
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(txtUct_ID, txtType_Desc), 
					() => UserOperateMode == OperateMode.Query, o => SearchComplate()
					);
			}
		}

		private void DoSearch(string uctId, string typeDesc)
		{
			//執行查詢動作		
			F19Entities proxy = GetProxy<F19Entities>();
            var q = proxy.F1950s.AsQueryable();
            if (!String.IsNullOrEmpty(uctId))
            {
                q = q.Where(x => x.UCT_ID.Equals(uctId));
            }
            if (!String.IsNullOrEmpty(typeDesc))
            {
                q = q.Where(x => x.TYPE_DESC.Contains(typeDesc));
            }
            var F1950List = q.OrderBy(x => x.UCT_ID).ToList();
			
			DgList = F1950List.ToObservableCollection();
			if ((DgList == null || !DgList.Any()) && !_isInit)
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
		}

		private void SearchComplate()
		{
			if (DgList != null && DgList.Any())
				SelectedData = DgList.First();
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query,o => AddComplate()
					);
			}
		}

		private void DoAdd()
		{
			//執行新增動作
		}

		private void AddComplate()
		{
			F1950 newItem = new F1950();
			DgList.Add(newItem);
			SelectedData = newItem;
			//RaisePropertyChanged("DgList");
			AddAction();
			UserOperateMode = OperateMode.Add;
			IsAdd = true;
		}

		private bool _isAdd;
		public bool IsAdd { get { return _isAdd; } set { _isAdd = value; RaisePropertyChanged("IsAdd"); } }
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DgList.Any()), o => EditComplate()
					);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
		}

		private void EditComplate()
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
				bool isCanceled = true ;
				return CreateBusyAsyncCommand(
					o =>
					{
						isCanceled = true;

						if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
						{
							isCanceled = false;
							return;
						}

						DoCancel();
					}, 
					() => UserOperateMode != OperateMode.Query,
					p => CancelComplate(isCanceled)
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			DoSearch(string.Empty, string.Empty);
		}

		private void CancelComplate(bool isCanceled)
		{
			if (isCanceled)
			{
				UserOperateMode = OperateMode.Query;
				SelectedData = DgList.Where(item => item.UCT_ID == _lastSelectedUctId).FirstOrDefault();
				SearchAction();
			}

		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				bool isDeleted = false;
				return CreateBusyAsyncCommand(
					o => isDeleted = DoDelete(), 
					() => UserOperateMode == OperateMode.Query && SelectedData != null,
					o => DelComplate(isDeleted)
					);
			}
		}

		private bool DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				F19Entities proxy = GetProxy<F19Entities>();
			    var f1950Entity =	proxy.F1950s.Where(item => item.UCT_ID == SelectedData.UCT_ID).FirstOrDefault();
				if (f1950Entity == null)
				{
					DialogService.ShowMessage(Properties.Resources.P1902020000_Deleted);
					return false;
				}
				
				proxy.DeleteObject(f1950Entity);
				proxy.SaveChanges();
				ShowMessage(Messages.InfoDeleteSuccess);
				DoSearch(txtUct_ID, txtType_Desc);
				return true;
			}
			return false;
		}

		private void DelComplate(bool isDeleted)
		{
			if (isDeleted)
				SearchAction();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = true;
				return CreateBusyAsyncCommand(
					o =>
						{
							isSaved = true;

							var error = GetEditableError(SelectedData);
							if (!string.IsNullOrEmpty(error))
							{
								isSaved = false;
								DialogService.ShowMessage(error);
								return;
							}

							isSaved = DoSave();
						}, 
					() => UserOperateMode != OperateMode.Query,
					o => SaveComplate(isSaved)
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作

			var proxy = GetProxy<F19Entities>();
			if (UserOperateMode == OperateMode.Add)
			{
				proxy.AddToF1950s(SelectedData);
			}
			else
			{
				var f1950Entity = proxy.F1950s.Where(item => item.UCT_ID == SelectedData.UCT_ID).FirstOrDefault();
				if (f1950Entity == null)
				{
					DialogService.ShowMessage(Properties.Resources.P1902020000_Deleted);
					return false;
				}

				f1950Entity.TYPE_DESC = SelectedData.TYPE_DESC;
				proxy.UpdateObject(f1950Entity);
			}

			proxy.SaveChanges();
			ShowMessage(Messages.Success);

			DoSearch(SelectedData.UCT_ID, string.Empty);
			return true;

		}


		private void SaveComplate(bool isSaved)
		{
			if (isSaved)
			{
				UserOperateMode = OperateMode.Query;
				SelectedData = DgList.FirstOrDefault();
				SearchAction();
			}
			
		}

		public string GetEditableError(F1950 e)
		{
			if (e == null)
				return Properties.Resources.P1902020000_ChooseItemYet;

			if (string.IsNullOrEmpty(e.UCT_ID))
				return Properties.Resources.P1902020000_InputReasonTypeNo;

			if (e.UCT_ID.Length > 4)
				return Properties.Resources.P1902020000_ReasonTypeNoLength4;

			if (!ValidateHelper.IsMatchAZaz09(e.UCT_ID))
				return Properties.Resources.P1902020000_ReasonTypeNo_ValidateCNWord;

			if (!string.IsNullOrEmpty(e.TYPE_DESC) && e.TYPE_DESC.Length > 40)
				return Properties.Resources.P1902020000_ReasonCommentLength40;

			if (UserOperateMode == OperateMode.Add)
			{
				var proxy = GetProxy<F19Entities>();
				var f1950Entity = proxy.F1950s.Where(item => item.UCT_ID == e.UCT_ID).FirstOrDefault();
				if (f1950Entity != null)
				{
					return Properties.Resources.P1902020000_ReasonTypeNoDuplicate;
				}
			}

			return string.Empty;
		}

		#endregion Save

		public Action OnCopyPasePreData = delegate { };
		public ICommand CopyPasePreData
		{
			get
			{
				return new RelayCommand(
					() => OnCopyPasePreData()
				);
			}
		}
		#endregion
	}
}
