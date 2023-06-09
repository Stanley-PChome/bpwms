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
	public partial class P1902030000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private readonly F19Entities _proxy;		
		private bool _isInit = true;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };

		#region 查詢條件
		#region Form - 原因編號
		private string _txtUCC_CODE;

		public string txtUCC_CODE
		{
			get { return _txtUCC_CODE; }
			set
			{
				_txtUCC_CODE = value;
				RaisePropertyChanged("txtUCC_CODE");
			}
		}
		#endregion

		#region Form - 原因名稱
		private string _txtCAUSE;

		public string txtCAUSE
		{
			get { return _txtCAUSE; }
			set
			{
				_txtCAUSE = value;
				RaisePropertyChanged("txtCAUSE");
			}
		}
		#endregion
		#region Form - 原因類別編號
		private string _cbUctId;

		public string cbUctId
		{
			get { return _cbUctId; }
			set
			{
				_cbUctId = value;
				RaisePropertyChanged("cbUctId");
			}
		}
		#endregion
		#endregion

		#endregion

		public P1902030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料				
				_proxy = GetProxy<F19Entities>();

				//原因類別下拉清單
				F1950S = _proxy.F1950s.ToObservableCollection();
				F1950S.Insert(0, new F1950());
				//
				InitControls();
			}
		}

		private void InitControls()
		{
			SearchCommand.Execute(null);
			_isInit = false;
		}

		#region Data
		#region Data 原因檔清單
		private ObservableCollection<F1951> _dgList;
		public ObservableCollection<F1951> DgList
		{
			get { return _dgList; }
			set { _dgList = value; RaisePropertyChanged("DgList"); }
		}
		#endregion

		#region Data 原因檔清單
		private ObservableCollection<F1951> _olddgList;
		public ObservableCollection<F1951> OldDgList
		{
			get { return _olddgList; }
			set { _olddgList = value; RaisePropertyChanged("OldDgList"); }
		}
		#endregion

		#region Grid資料選取
		private F1951 _selectedData;

		public F1951 SelectedData
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

					if (value != null && !string.IsNullOrEmpty(value.UCT_ID) && !string.IsNullOrEmpty(value.UCC_CODE))
					{
						_uccCode = value.UCC_CODE;
						_uctId = value.UCT_ID;
					}
				}
			}
		}

		private string _uccCode = null;
		private string _uctId = null;
		#endregion
		/// <summary>
		/// 用在下拉原因類別
		/// </summary>
		private ObservableCollection<F1950> _f1950S;
		public ObservableCollection<F1950> F1950S
		{
			get { return _f1950S; }
			set { _f1950S = value; IsEnableEdit = value.Any(); RaisePropertyChanged("F1950S"); }
		}
		#endregion

		public bool IsEnableEdit
		{
			get;
			set;
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(txtUCC_CODE, cbUctId, txtCAUSE), () => UserOperateMode == OperateMode.Query, o => SearchComplate()
					);
			}
		}

		private void DoSearch(string uccCode, string uctId, string cause)
		{
			//執行查詢動
			var proxy = GetProxy<F19Entities>();
            var F1951datas = proxy.F1951s.Select(c => c);
            if (!string.IsNullOrEmpty(uctId))
            {
                F1951datas = F1951datas.Where(c => c.UCT_ID.Equals(uctId));
            }
            if (!string.IsNullOrEmpty(cause))
            {
                F1951datas = F1951datas.Where(c => c.CAUSE.Contains(cause));
            }
            if (!string.IsNullOrEmpty(uccCode))
            {
                F1951datas = F1951datas.Where(c => c.UCC_CODE.Equals(uccCode));
            }
            var F1951List = F1951datas.OrderBy(s => s.UCC_CODE).ToList();
            DgList = F1951List.ToObservableCollection();
			OldDgList = F1951List.AsQueryable().ToObservableCollection();
			RaisePropertyChanged("OldDgList");
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
					o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddCompleted()
					);
			}
		}

		private void DoAddCompleted()
		{
			F1951 newItem = new F1951();
			DgList.Add(newItem);
			SelectedData = newItem;
			//RaisePropertyChanged("DgList");
			AddAction();
			UserOperateMode = OperateMode.Add;
			IsAdd = true;
		}

		private bool _isAdd;
		public bool IsAdd { get { return _isAdd; } set { _isAdd = value; RaisePropertyChanged("IsAdd"); } }

		private void DoAdd()
		{
			//執行新增動作
		}
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
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => CancelComplate()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}

		private void CancelComplate()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//var resetSelectedData = false;
				//if (UserOperateMode == OperateMode.Add && DgList.Any())
				//{
				//	DgList.RemoveAt(DgList.Count - 1);
				//	resetSelectedData = true;
				//}

				DoSearch(txtUCC_CODE, cbUctId, txtCAUSE);
				

				UserOperateMode = OperateMode.Query;
				if ( DgList.Any())
					SelectedData = DgList.FirstOrDefault(item=>item.UCC_CODE == _uccCode && item.UCT_ID == _uctId);
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
				//執行刪除動作
				var proxy = GetProxy<F19Entities>();
				var f1951Entity = proxy.F1951s.Where(item => item.UCC_CODE == SelectedData.UCC_CODE && item.UCT_ID == SelectedData.UCT_ID).FirstOrDefault();

				if (f1951Entity == null)
				{
					DialogService.ShowMessage(Properties.Resources.P1902030000_ReasonTypeNoDeleted);
					return false;
				}

				proxy.DeleteObject(f1951Entity);
				proxy.SaveChanges();
				ShowMessage(Messages.InfoDeleteSuccess);
				return true;
			}

			return false;
		}

		private void DelComplate(bool isDeleted)
		{
			if (isDeleted)
			{
				DoSearch(txtUCC_CODE, cbUctId, txtCAUSE);
				if (DgList != null && DgList.Any())
					SelectedData = DgList.First();
				SearchAction();
			}
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(),
					() => UserOperateMode != OperateMode.Query && SelectedData != null,
					o => SaveComplate(isSaved)
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			ExDataMapper.Trim(SelectedData);
			var error = GetEditableError(SelectedData);
			if (!string.IsNullOrEmpty(error))
			{
				DialogService.ShowMessage(error);
				return false;
			}

			var proxy = GetProxy<F19Entities>();
			var f1951Entity = proxy.F1951s.Where(item => item.UCC_CODE == SelectedData.UCC_CODE && item.UCT_ID == SelectedData.UCT_ID).FirstOrDefault();

			if (UserOperateMode == OperateMode.Add)
			{
				if (f1951Entity != null)
				{
					DialogService.ShowMessage(Properties.Resources.P1902030000_ReasonTypeNoExist);
					return false;
				}
				proxy.AddToF1951s(SelectedData);
			}
			else
			{
				if (f1951Entity == null)
				{
					DialogService.ShowMessage(Properties.Resources.P1902030000_ReasonTypeNoDeleted);
					return false;
				}
				f1951Entity.CAUSE = SelectedData.CAUSE;
				proxy.UpdateObject(f1951Entity);
			}

			proxy.SaveChanges();
			ShowMessage(Messages.Success);
			return true;
		}

		private void SaveComplate(bool isSaved)
		{
			if (isSaved)
			{
				UserOperateMode = OperateMode.Query;

				DoSearch(SelectedData.UCC_CODE, SelectedData.UCT_ID, string.Empty);
				SelectedData = DgList.FirstOrDefault();
				SearchAction();
			}
		}

		public string GetEditableError(F1951 e)
		{
			if (e == null)
				return Properties.Resources.P1902030000_ChooseItem;

			if (string.IsNullOrEmpty(e.UCC_CODE))
				return Properties.Resources.P1902020000_InputReasonTypeNo;

			if (!ValidateHelper.IsMatchAZaz09(e.UCC_CODE))
				return Properties.Resources.P1902030000_ReasonNo_ValidateCNWord;

			if (string.IsNullOrEmpty(e.UCT_ID))
				return Properties.Resources.P1902030000_ChooseReasonNo;

			if (!ValidateHelper.IsMatchAZaz09(e.UCT_ID))
				return Properties.Resources.P1902020000_ReasonTypeNo_ValidateCNWord;

			if (string.IsNullOrEmpty(e.CAUSE))
				return Properties.Resources.P1902030000_InputReason;

			if (e.CAUSE.Length > 40)
				return Properties.Resources.P1902030000_ReasonLength40;

			return string.Empty;
		}
		#endregion Save
	}
}
