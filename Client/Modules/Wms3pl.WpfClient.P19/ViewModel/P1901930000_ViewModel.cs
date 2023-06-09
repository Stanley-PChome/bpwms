using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901930000_ViewModel : InputViewModelBase
	{

		public P1901930000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			SetDcList();
		}

		#region Property
		// 物流中心清單
		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { Set(() => DcList, ref _dcList, value); }
		}

		// 選擇的物流中心
		private string _selectedDc;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { Set(() => SelectedDc, ref _selectedDc, value); }
		}

		// 查詢結果清單
		private List<F194501> _dgList;
		public List<F194501> DgList
		{
			get { return _dgList; }
			set { Set(() => DgList, ref _dgList, value); }
		}

		// 選擇的物流中心(新增/編輯)
		private string _dcCode;
		public string DcCode
		{
			get { return _dcCode; }
			set { Set(() => DcCode, ref _dcCode, value); }
		}

		// 集貨格料架類型(新增/編輯)
		private string _cellType;
		public string CellType
		{
			get { return _cellType; }
			set {
				var cellType = value?.ToString();
				Regex reg = new Regex(@"^[A-Za-z0-9+-]+$");
				Match match = reg.Match(cellType);

				if (string.IsNullOrWhiteSpace(cellType) || match.Success)
				{
					Set(() => CellType, ref _cellType, value);
				}
			}
		}

		// 集貨格料架名稱(新增/編輯)
		private string _cellName;
		public string CellName
		{
			get { return _cellName; }
			set { Set(() => CellName, ref _cellName, value); }
		}

		// 長(新增/編輯)
		private int _length;
		public int Length
		{
			get { return _length; }
			set { Set(() => Length, ref _length, value); }
		}

		// 寬(新增/編輯)
		private int _depth;
		public int Depth
		{
			get { return _depth; }
			set { Set(() => Depth, ref _depth, value); }
		}

		// 高(新增/編輯)
		private int _heigth;
		public int Heigth
		{
			get { return _heigth; }
			set { Set(() => Heigth, ref _heigth, value); }
		}

		// 容積率(新增/編輯)
		private string _volumeRate;
		public string VolumeRate
		{
			get { return _volumeRate; }
			set { Set(() => VolumeRate, ref _volumeRate, value); }
		}

		private F194501 _selectedAddOrModifyF010202Data;
		public F194501 SelectedAddOrModifyF010202Data
		{
			get { return _selectedAddOrModifyF010202Data; }
			set { Set(() => SelectedAddOrModifyF010202Data, ref _selectedAddOrModifyF010202Data, value); }
		}
		#endregion

		#region Math
		// 取得物流中心清單
		public void SetDcList()
		{
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcList.Any())
				SelectedDc = DcList.First().Value;
		}

		public void DoSearch()
		{
			var proxy = GetProxy<F19Entities>();
			DgList = proxy.F194501s.Where(x => x.DC_CODE == SelectedDc).ToList();
		}

		public void DoAdd()
		{
			DcCode = SelectedDc;
			UserOperateMode = OperateMode.Add;
		}

		public bool DoSave()
		{
			CellType = CellType.ToUpper();
			var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.InsertOrUpdateF194501(DcCode, CellType, CellName, Length, Depth, Heigth, VolumeRate, UserOperateMode.ToString()));
			if (!result.IsSuccessed)
			{
				ShowWarningMessage(result.Message);
				return false;
			}
			return true;
		}

		public void DoSaveCpmplete(bool isSuccess)
		{
			if (isSuccess)
			{
				UserOperateMode = OperateMode.Query;
				ClearInput();
				DoSearch();
			}
		}

		public void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			DcCode = SelectedAddOrModifyF010202Data.DC_CODE;
			CellType = SelectedAddOrModifyF010202Data.CELL_TYPE;
			CellName = SelectedAddOrModifyF010202Data.CELL_NAME;
			Length = SelectedAddOrModifyF010202Data.LENGTH;
			Depth = SelectedAddOrModifyF010202Data.DEPTH;
			Heigth = SelectedAddOrModifyF010202Data.HEIGHT;
			VolumeRate = SelectedAddOrModifyF010202Data.VOLUME_RATE.ToString();
		}

		public void DoCancel()
		{
			UserOperateMode = OperateMode.Query;
			ClearInput();
		}

		public void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var proxy = GetWcfProxy<wcf.P19WcfServiceClient>();
				var result = proxy.RunWcfMethod(w => w.DeleteF194501(SelectedAddOrModifyF010202Data.DC_CODE, SelectedAddOrModifyF010202Data.CELL_TYPE));
				if (result.IsSuccessed)
				{
					ShowMessage(Messages.InfoDeleteSuccess);
				}
				else
				{
					DialogService.ShowMessage(result.Message);
				}
			}
		}

		public void DoDeleteComplete()
		{
			DoSearch();
		}

		public void ClearInput()
		{
			DcCode = "";
			CellType = "";
			CellName = "";
			Length = 0;
			Depth = 0;
			Heigth = 0;
			VolumeRate = "";
		}
		#endregion

		#region ICommand
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query);
			}
		}

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query);
			}
		}

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedAddOrModifyF010202Data != null);
			}
		}

		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedAddOrModifyF010202Data != null,
					o=>DoDeleteComplete());
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode == OperateMode.Add || UserOperateMode == OperateMode.Edit,
					o => DoSaveCpmplete(isSuccess));
			}
		}
		#endregion
	}
}