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
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;


namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0201040000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private bool _saveResult = true;
		public Action OnAddFocus = delegate { };
		public Action OnEditFocus = delegate { };

		public P0201040000_ViewModel()
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
			GetDcCodes();
			GetPierList();
		}

		#region 資料連結/ 頁面參數

		#region Form - 物流中心
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
				GetPierList();
				RaisePropertyChanged("SelectDcCode");
			}
		}
		#endregion

		#region Form - DataGrid選取
		private F020104 _selectData;

		public F020104 SelectData
		{
			get { return _selectData; }
			set
			{
				_selectData = value;
				RaisePropertyChanged("SelectData");
			}
		}
		#endregion

		#region Form - 查詢日期
		private DateTime? _selectedStartDate;
		public DateTime? SelectedStartDate
		{
			get { return _selectedStartDate; }
			set { _selectedStartDate = value; RaisePropertyChanged("SelectedStartDate"); }
		}

		private DateTime? _selectedEndDate;
		public DateTime? SelectedEndDate
		{
			get { return _selectedEndDate; }
			set { _selectedEndDate = value; RaisePropertyChanged("SelectedEndDate"); }
		}
		#endregion

		#region Form - 新增編輯日期
		public DateTime? EditStartDate
		{
			get { return SelectData.BEGIN_DATE; }
			set
			{
				if (value.HasValue)
				{
					SelectData.BEGIN_DATE = value.Value;
				}
				else
				{
					SelectData.BEGIN_DATE = DateTime.Now;
				}

				RaisePropertyChanged("EditStartDate");
			}
		}

		public DateTime? EditEndDate
		{
			get { return SelectData.END_DATE; }
			set
			{
				if (value.HasValue)
				{
					SelectData.END_DATE = value.Value;
				}
				else
				{
					SelectData.END_DATE = DateTime.Now;
				}

				RaisePropertyChanged("EditEndDate");
			}
		}
		#endregion

		#region Form - 碼頭
		private ObservableCollection<NameValuePair<string>> _pierList;
		/// <summary>
		/// 碼頭清單. 依據所選擇的物流中心而改變.
		/// </summary>
		public ObservableCollection<NameValuePair<string>> PierList
		{
			get { return _pierList; }
			set { _pierList = value; RaisePropertyChanged("PierList"); }
		}
		#endregion

		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
		}
		private List<NameValuePair<string>> _dcList;
		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set { _dcList = value; RaisePropertyChanged("DcList"); }
		}
		#endregion

		#region Form - 暫存區號,進貨,出貨
		public Int16? Area
		{
			get { return SelectData.TEMP_AREA; }
			set
			{
				if (value.HasValue)
				{
					SelectData.TEMP_AREA = value.Value;
				}
				else
				{
					SelectData.TEMP_AREA = 0;
				}

				RaisePropertyChanged();
			}
		}

		private bool _allowIn;
		public bool AllowIn
		{
			get { return _allowIn; }
			set
			{
				_allowIn = value;
				RaisePropertyChanged();
			}
		}

		private bool _allowOut;
		public bool AllowOut
		{
			get { return _allowOut; }
			set
			{
				_allowOut = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Data - 查詢結果
		private List<F020104> _dgItemSource;
		public List<F020104> DgItemSource
		{
			get { return _dgItemSource; }
			set
			{
				_dgItemSource = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#endregion

		#region 函式
		private void ControlClear()
		{
			SelectedStartDate = null;
			SelectedEndDate = null;
			EditStartDate = null;
			EditEndDate = null;
			Area = null;
			AllowIn = false;
			AllowOut = false;
            SelectData = null;
        }
		/// <summary>
		/// 取得物流中心DeCodeList
		/// </summary>
		private void GetDcCodes()
		{
			if (DcCodes != null) DcCodes.Clear();
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
			{
				SelectDcCode = DcCodes.First().Value;
			}
		}

		/// <summary>
		/// 取得碼頭編號PierList
		/// </summary>
		private void GetPierList()
		{
			PierList = PiersService.PiersList(FunctionCode, SelectDcCode).ToObservableCollection();
            RaisePropertyChanged("PierList");
			//if (PierList.Any())
			//	SelectData.PIER_CODE = PierList.FirstOrDefault().Value;
		}

		/// <summary>
		/// 查詢碼頭清單. 依照所選取的DC_CODE
		/// 主要搜尋F1981, 若F020104有資料, 則該碼頭依F020104的設定區間
		/// </summary>
		private void PiersChange(string dcCode, string pierCode)
		{
			// 只有在新增模式下才做連動
			if (UserOperateMode != OperateMode.Add) return;
			List<F1981> selPier = PiersService.PiersList(FunctionCode, dcCode, pierCode);
			if (selPier.Any() && (UserOperateMode == OperateMode.Add))
			{
				Area = selPier.FirstOrDefault().TEMP_AREA;
				AllowIn = selPier.FirstOrDefault().ALLOW_IN.Equals("1");
				AllowOut = selPier.FirstOrDefault().ALLOW_OUT.Equals("1");
			}
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
				);
			}
		}

		private void DoSearch()
		{
			//先清空DataGrid的資料
			if (DgItemSource != null)
			{
				DgItemSource.Clear();
				DgItemSource = null;
			}
			//RaisePropertyChanged("DgItemSource");
			//執行查詢動作
			var proxy = GetProxy<F02Entities>();
			var qry = (from a in proxy.F020104s
					   where a.DC_CODE.Equals(SelectDcCode)
					   select a).ToList();
			if (qry.Any())
			{
				DgItemSource = qry.Where(x => (SelectedStartDate == null || x.BEGIN_DATE.Date >= new DateTime(SelectedStartDate.Value.Year, SelectedStartDate.Value.Month, SelectedStartDate.Value.Day))
					&& (SelectedEndDate == null || x.END_DATE.Date <= new DateTime(SelectedEndDate.Value.Year, SelectedEndDate.Value.Month, SelectedEndDate.Value.Day))
					//&& (SelectData.PIER_CODE == null || x.PIER_CODE == SelectData.PIER_CODE)
					//&& (Area == null || x.TEMP_AREA == (int.TryParse(Area,out result) ? result:result))
					//&& (SelectData.PIER_CODE == null || x.ALLOW_IN == (AllowIn ? "1" : "0"))
					//&& (SelectData.PIER_CODE == null ||x.ALLOW_OUT == (AllowOut ? "1" : "0"))
					).ToList();
			}
			RaisePropertyChanged();
			if (DgItemSource != null)
			{
				if (!DgItemSource.Any())
					ShowMessage(Messages.InfoNoData);
			}
			else
				ShowMessage(Messages.InfoNoData);
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
			// 如果有變更, 或是有新增時, 先確認是否繼續操作
			UserOperateMode = OperateMode.Add;

			SelectData = new F020104();
			SelectData.BEGIN_DATE = DateTime.Now;
			SelectData.END_DATE = DateTime.Now;
			SelectData.ALLOW_OUT = "0";
			SelectData.ALLOW_IN = "0";
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectData != null
				);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			if (SelectData != null)
			{
				AllowIn = SelectData.ALLOW_IN.Equals("1");
				AllowOut = SelectData.ALLOW_OUT.Equals("1");
			}

			UserOperateMode = OperateMode.Edit;
			OnEditFocus();
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
			if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes) return;
          
            //執行取消動作
            UserOperateMode = OperateMode.Query;
            ControlClear();
			GetPierList();
            DoSearch();
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectData != null
				);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			var msg = Messages.WarningBeforeDelete;
			msg.Message = Properties.Resources.P0201010000_DelData;
			if (ShowMessage(msg) != DialogResponse.Yes) return;
			//執行刪除動作
			var proxy = GetModifyQueryProxy<F02Entities>();
			var pier = (from a in proxy.F020104s
						where a.DC_CODE.Equals(SelectData.DC_CODE)
								&& a.PIER_CODE.Equals(SelectData.PIER_CODE)
								&& a.BEGIN_DATE == SelectData.BEGIN_DATE
								&& a.END_DATE == SelectData.END_DATE
						select a).FirstOrDefault();
			if (pier == null)
			{
				DialogService.ShowMessage(Properties.Resources.P0201040000_pierIsNull);
				return;
			}
			else
			{
				proxy.DeleteObject(pier);
			}
			proxy.SaveChanges();
			DoSearch();
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
					o =>
					{
						if (_saveResult)
						{
							ShowMessage(Messages.Success);
						}
					}
				);
			}
		}

		private void DoSave()
		{
			_saveResult = false;
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;
			var errorMsg = DoCheckData();
			_saveResult = string.IsNullOrEmpty(errorMsg);

			if (_saveResult)
			{
				if (UserOperateMode == OperateMode.Add) DoSaveInsert();
				if (UserOperateMode == OperateMode.Edit) DoSaveUpdate();
			}
			else
				ShowWarningMessage(errorMsg);
		}


		/// <summary>
		/// 更新時的寫入動作
		/// </summary>
		private void DoSaveUpdate()
		{
			var proxy = GetProxy<F02Entities>();
			var pier = (from a in proxy.F020104s
						where a.DC_CODE.Equals(SelectData.DC_CODE)
							   && a.PIER_CODE.Equals(SelectData.PIER_CODE)
							   && a.BEGIN_DATE == new DateTime(SelectData.BEGIN_DATE.Year, SelectData.BEGIN_DATE.Month, SelectData.BEGIN_DATE.Day)
							   && a.END_DATE == new DateTime(SelectData.END_DATE.Year, SelectData.END_DATE.Month, SelectData.END_DATE.Day)
						select a).FirstOrDefault();
			if (pier != null)
			{
				pier.TEMP_AREA = Area.Value;

				if (String.IsNullOrEmpty(SelectData.ALLOW_IN))
					SelectData.ALLOW_IN = "0";

				if (String.IsNullOrEmpty(SelectData.ALLOW_OUT))
					SelectData.ALLOW_OUT = "0";

				pier.ALLOW_IN = SelectData.ALLOW_IN;//AllowIn ? "1" : "0";
				pier.ALLOW_OUT = SelectData.ALLOW_OUT;//AllowOut ? "1" : "0";
				pier.UPD_DATE = DateTime.Now;
				pier.UPD_STAFF = Wms3plSession.CurrentUserInfo.Account;
				proxy.UpdateObject(pier);
				proxy.SaveChanges();
			}

			UserOperateMode = OperateMode.Query;
			DoSearch();
			_saveResult = true;
		}

		/// <summary>
		/// 新增時的寫入動作
		/// </summary>
		/// <returns></returns>
		private void DoSaveInsert()
		{
			F020104 NewData = new F020104();
			NewData.BEGIN_DATE = EditStartDate.Value.Date;
			NewData.END_DATE = EditEndDate.Value.Date;
			NewData.DC_CODE = SelectDcCode;
			NewData.PIER_CODE = SelectData.PIER_CODE;
			NewData.TEMP_AREA = SelectData.TEMP_AREA;

			if (String.IsNullOrEmpty(SelectData.ALLOW_IN))
				SelectData.ALLOW_IN = "0";

			if (String.IsNullOrEmpty(SelectData.ALLOW_OUT))
				SelectData.ALLOW_OUT = "0";

			NewData.ALLOW_IN = SelectData.ALLOW_IN; //AllowIn ? "1" : "0";
			NewData.ALLOW_OUT = SelectData.ALLOW_OUT;// ? "1" : "0";
			NewData.CRT_DATE = DateTime.Now;
			NewData.UPD_DATE = DateTime.Now;
			NewData.CRT_STAFF = this._userId;
			NewData.UPD_STAFF = this._userId;

			var proxy = GetProxy<F02Entities>();
			proxy.AddToF020104s(NewData);
			proxy.SaveChanges();

			UserOperateMode = OperateMode.Query;
			DoSearch();
			_saveResult = true;
		}

		/// <summary>
		/// 檢查必要資料是否都有填入
		/// </summary>
		/// <returns></returns>
		private string DoCheckData()
		{
			if (string.IsNullOrEmpty(SelectDcCode)) return Properties.Resources.P0201040000_SelectDcCode;
			if (string.IsNullOrEmpty(SelectData.PIER_CODE)) return Properties.Resources.P0201040000_PierCodeIsNull;

			if (Area == null)
			{
				return Properties.Resources.P0201040000_AreaIsNull;
			}

			if (Area < 1)
			{
				return Properties.Resources.P0201040000_AreaError;
			}

			if (String.IsNullOrEmpty(SelectData.ALLOW_IN) && String.IsNullOrEmpty(SelectData.ALLOW_OUT))
			{
				return Properties.Resources.P0201040000_SelectAllowInAndAllowOut;
			}

			if (SelectData.ALLOW_IN.Equals("0") && SelectData.ALLOW_OUT.Equals("0"))
			{
				return Properties.Resources.P0201040000_SelectAllowInAndAllowOut;
			}

			if (UserOperateMode == OperateMode.Add && IsDataExist(SelectData.PIER_CODE))
			{
				return Properties.Resources.P0201040000_PierCodeError;
			}

			return string.Empty;
		}
		#endregion Save

		#endregion

		private bool IsDataExist(string pierCode)
		{
			bool result = false;

			var proxy = GetProxy<F02Entities>();
			var qry = (from a in proxy.F020104s
					   where a.DC_CODE.Equals(SelectDcCode)
					   select a).ToList();

			if (qry.Any())
			{
				List<F020104> data =
					qry.Where(x => (x.BEGIN_DATE.Date >= new DateTime(SelectData.BEGIN_DATE.Year, SelectData.BEGIN_DATE.Month, SelectData.BEGIN_DATE.Day))
								&& (x.END_DATE.Date <= new DateTime(SelectData.END_DATE.Year, SelectData.END_DATE.Month, SelectData.END_DATE.Day))
								&& (x.PIER_CODE == pierCode)).ToList();

				if (data != null && data.Count > 0)
				{
					result = true;
				}
			}

			if (!result)
			{
				var qry2 = (from a in proxy.F020103s
							where a.DC_CODE.Equals(SelectDcCode)
							select a).ToList();

				if (qry2.Any())
				{
					List<F020103> data2 =
						qry2.Where(x => (x.ARRIVE_DATE.Date >= new DateTime(SelectData.BEGIN_DATE.Year, SelectData.BEGIN_DATE.Month, SelectData.BEGIN_DATE.Day))
									&& (x.ARRIVE_DATE.Date <= new DateTime(SelectData.END_DATE.Year, SelectData.END_DATE.Month, SelectData.END_DATE.Day))
									&& (x.PIER_CODE == pierCode)).ToList();

					if (data2 != null && data2.Count > 0)
					{
						result = true;
					}
				}
			}

			return result;
		}

	}
}
