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
	public partial class P1901250000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private bool isValid;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };

		#endregion

		public P1901250000_ViewModel()
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
			GetDcCodes();
			SearchCommand.Execute(null);
		}

		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
			{
				SelectDcCode = DcCodes.First().Value;
			}
		}

		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged();
			}
		}

		private string _selectDcCode;

		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				SelectedData = null;
				DataList = null;
				//DoSearch();
				SearchCommand.Execute(null);
				RaisePropertyChanged("SelectDcCode");
			}
		}

		private List<F1981> _DataList;
		public List<F1981> DataList
		{
			get { return _DataList; }
			set
			{
				_DataList = value;
				RaisePropertyChanged();
			}
		}

		private F1981 _selectedData;

		public F1981 SelectedData
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
					o => DoSearch(), () => UserOperateMode == OperateMode.Query,
					o => DoSearchCompleted()
					);
			}
		}

		private void DoSearch()
		{
			var proxy = GetProxy<F19Entities>();
			DataList = proxy.F1981s.Where(x => x.DC_CODE == SelectDcCode).ToList();
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
			F1981 newItem = new F1981();
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
				UserOperateMode = OperateMode.Query;
				return true;
			}

			return false;
		}

		private void DoCancelCompleted(bool isCancel)
		{
			if (isCancel)
			{
				SearchAction();
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

			var proxy = GetProxy<F19Entities>();
			var pier = (from a in proxy.F1981s
						where a.DC_CODE.Equals(SelectedData.DC_CODE) && a.PIER_CODE.Equals(SelectedData.PIER_CODE)
						select a).FirstOrDefault();
			if (pier == null)
			{
				DialogService.ShowMessage(Properties.Resources.P1901250000_PierDataNotFound);
				return;
			}
			else
			{
				proxy.DeleteObject(pier);
			}
			proxy.SaveChanges();
			ShowMessage(Messages.DeleteSuccess);
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
			isValid = true;

			ExDataMapper.Trim(SelectedData);
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
			{
				isValid = false;
				return;
			}

			if (string.IsNullOrEmpty(SelectedData.PIER_CODE) || string.IsNullOrWhiteSpace(SelectedData.PIER_CODE))
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1901250000_PierNull);
				return;
			}
			if (IsValidPireCode(SelectedData.PIER_CODE) == false)
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1901250000_PierValidateCNWord);
				return;
			}
			if (UserOperateMode == OperateMode.Add)
			{
				var proxy = GetProxy<F19Entities>();
				var pier = proxy.F1981s.Where(x => x.DC_CODE.Equals(SelectDcCode) && x.PIER_CODE.Equals(SelectedData.PIER_CODE)).AsQueryable().ToList().Count();
				if (pier != 0)
				{
					isValid = false;
					DialogService.ShowMessage(Properties.Resources.P1901250000_PierDuplicate);
					return;
				}
			}
			if (IsValidTempArea(SelectedData.TEMP_AREA.ToString()) == false)
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustGreaterThanZero);
				return;
			}
			//if (SelectedData.TEMP_AREA < 1)
			//{
			//	isValid = false;
			//	DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustPositive);
			//	return;				
			//}
			if ((SelectedData.ALLOW_IN == "0" && SelectedData.ALLOW_OUT == "0") || (SelectedData.ALLOW_IN == null && SelectedData.ALLOW_OUT == null))
			{
				isValid = false;
				DialogService.ShowMessage(Properties.Resources.P1901250000_AtleastCheckOne);
				return;
			}
			//執行確認儲存動作
			if (UserOperateMode == OperateMode.Add)
				DoSaveAdd();
			else if (UserOperateMode == OperateMode.Edit)
				DoSaveEdit();
		}

		private void DoSaveAdd()
		{
			var f1981 = new F1981();
			var _AllowIn = SelectedData.ALLOW_IN;
			var _AllowOut = SelectedData.ALLOW_OUT;
			if (_AllowIn == null) { _AllowIn = "0"; }
			if (_AllowOut == null) { _AllowOut = "0"; }
			f1981.DC_CODE = SelectDcCode;
			f1981.PIER_CODE = SelectedData.PIER_CODE.Trim();
			f1981.TEMP_AREA = SelectedData.TEMP_AREA;
			f1981.ALLOW_IN = _AllowIn;
			f1981.ALLOW_OUT = _AllowOut;
			var proxy = GetProxy<F19Entities>();
			proxy.AddToF1981s(f1981);
			proxy.SaveChanges();
			ShowMessage(Messages.Success);
		}

		private void DoSaveEdit()
		{
			var proxy = GetProxy<F19Entities>();
			var f1981s = proxy.F1981s.Where(x => x.PIER_CODE == SelectedData.PIER_CODE).AsQueryable().ToList();
			var f1981 = f1981s.FirstOrDefault();
			f1981.TEMP_AREA = SelectedData.TEMP_AREA;
			f1981.ALLOW_IN = SelectedData.ALLOW_IN;
			f1981.ALLOW_OUT = SelectedData.ALLOW_OUT;
			f1981.UPD_STAFF = _userId;
			f1981.UPD_NAME = _userName;
			f1981.UPD_DATE = DateTime.Now;
			proxy.UpdateObject(f1981);
			proxy.SaveChanges();
			ShowMessage(Messages.Success);
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

		public bool IsValidPireCode(String pireCode)
		{
			System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
			return reg1.IsMatch(pireCode);
		}
		public bool IsValidTempArea(String tempArea)
		{
			System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
			return reg1.IsMatch(tempArea);
		}

	}
}
