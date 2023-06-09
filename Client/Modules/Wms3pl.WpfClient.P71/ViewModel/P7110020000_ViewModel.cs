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
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7110020000_ViewModel : InputViewModelBase
	{      
        #region Property

        #region UI 連動 binding
        private bool _searchResultIsExpanded;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged("SearchResultIsExpanded");
			}
		}
		#endregion

		#region 物流中心 業主 貨主清單

		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged();
				if (_dcList != null && _dcList.Any())
				{
					SelectedDcCode = _dcList.First().Value;
				}
			}
		}

		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				_selectedDcCode = value;
				RaisePropertyChanged();

				if (value != null && SelectedDcCode != null)
				{
					SetSearchGupList(value);
					if (UserOperateMode == OperateMode.Query)
						SelectedGupCode = SelectedGupList.FirstOrDefault().Value;

					//清空單據清單資料
					setClearData();
				}
			}
		}

		private List<NameValuePair<string>> _selectedGupList;

		public List<NameValuePair<string>> SelectedGupList
		{
			get { return _selectedGupList; }
			set
			{
				_selectedGupList = value;
				RaisePropertyChanged();
			}
		}


		private string _selectedGupCode;

		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged();

				if (value != null && SelectedDcCode != null)
				{
					SetSearchCustList(SelectedDcCode, value);
					if (UserOperateMode == OperateMode.Query)
						SelectedCustCode = SelectedCustList.FirstOrDefault().Value;

					//清空單據清單資料
					setClearData();
				}
			}
		}


		private List<NameValuePair<string>> _selectedCustList;

		public List<NameValuePair<string>> SelectedCustList
		{
			get { return _selectedCustList; }
			set
			{
				_selectedCustList = value;
				RaisePropertyChanged();
			}
		}

		private string _selectedCustCode;

		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged();
				//清空單據清單資料
				setClearData();
			}
		}

		#endregion

		#region 單據清單
		private List<F190002Data> _dgList;
		public List<F190002Data> DgList { get { return _dgList; } set { _dgList = value; RaisePropertyChanged("DgList"); } }

		private F190002Data _selectedData;
		public F190002Data SelectedData { get { return _selectedData; } 
			set {
				_selectedData = value;
				if (_selectedData != null)
					SelectedDataId = _selectedData.TICKET_ID;
				RaisePropertyChanged(); 
			} 
		}

		private decimal SelectedDataId { get; set; }
		#endregion


		#endregion

		#region Function

		public P7110020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SetSearchGupList(SelectedDcCode);
				SetSearchCustList(SelectedDcCode,SelectedGupCode);
                //動態生成用的方法 寫入也要改動態(Model要手動加
                //SetDataGridViewData();

            }

		}

        /// <summary>
        /// 取得DataGrid顯示的倉別欄位
        /// </summary>
        /// <returns></returns>
        public List<F198001> SetDataGridViewData()
        {
            var proxy = GetProxy<F19Entities>();
            var f198001 = proxy.F198001s.ToList();
            //排除進貨站存倉
            f198001.Remove(f198001.Where(o => o.TYPE_ID == "I").FirstOrDefault());
            return f198001;
        }

		#region 清空單據清單資料
		private void setClearData()
		{
			DgList = null;
			SelectedData = null;
		}
		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		public void SetSearchGupList(string dcCode)
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(dcCode);

			gupList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
			SelectedGupList = gupList;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		/// <param name="gupCode">業主</param>
		public void SetSearchCustList(string dcCode, string gupCode)
		{
			if (gupCode == null)
				return;

			var custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(dcCode, gupCode);

			custList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
			SelectedCustList = custList;
		}

		#endregion

		#endregion

		#region Command

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
			//指定SelectedData
			if (DgList != null && DgList.Any())
				SelectedData = DgList.FirstOrDefault();

			SearchResultIsExpanded = (DgList != null && DgList.Any());
		}

		private void DoSearch()
		{
			//執行查詢動
			var dcCode = SelectedDcCode;
			var gupCode = (SelectedGupCode == "-1") ? string.Empty : SelectedGupCode;
			var custCode = (SelectedCustCode == "-1") ? string.Empty : SelectedCustCode;

			var proxy = GetExProxy<P71ExDataSource>();
			var query = proxy.CreateQuery<F190002Data>("GetF190002Data")
							 .AddQueryExOption("dcCode", dcCode)
							 .AddQueryExOption("gupCode", gupCode)
							 .AddQueryExOption("custCode", custCode);

			DgList = query.ToList();

			if (DgList == null || !DgList.Any())
			{
				ShowMessage(Messages.InfoNoData);
				SelectedData = null;
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
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null
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
			if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
				return;
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
					o => DoSave(), () => UserOperateMode != OperateMode.Query,
					o => DoSaveComplete()
					);
			}
		}

		private void DoSaveComplete()
		{
			if (DgList != null && DgList.Any())
				SelectedData = DgList.Where(x => x.TICKET_ID == SelectedDataId).FirstOrDefault();
		}

		private void DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) != UILib.Services.DialogResponse.Yes)
				return;

			//執行確認儲存動作

			SetF190002WAREHOUSE(SelectedData.SWAREHOUSE, "S");
			SetF190002WAREHOUSE(SelectedData.TWAREHOUSE, "T");
			SetF190002WAREHOUSE(SelectedData.OWAREHOUSE, "O");
			SetF190002WAREHOUSE(SelectedData.BWAREHOUSE, "B");
			SetF190002WAREHOUSE(SelectedData.GWAREHOUSE, "G");
			SetF190002WAREHOUSE(SelectedData.NWAREHOUSE, "N");
			SetF190002WAREHOUSE(SelectedData.WWAREHOUSE, "W");
			SetF190002WAREHOUSE(SelectedData.RWAREHOUSE, "R");
			SetF190002WAREHOUSE(SelectedData.DWAREHOUSE, "D");
			SetF190002WAREHOUSE(SelectedData.MWAREHOUSE, "M");
            SetF190002WAREHOUSE(SelectedData.DWAREHOUSE, "U");
            SetF190002WAREHOUSE(SelectedData.MWAREHOUSE, "V");

            ShowMessage(Messages.Success);
			UserOperateMode = OperateMode.Query;
		}

		private void SetF190002WAREHOUSE(decimal isChecked,string type)
		{
			var proxy = GetProxy<F19Entities>();
			var f190002 = proxy.F190002s.Where(x => x.TICKET_ID == SelectedData.TICKET_ID && x.WAREHOUSE_TYPE == type).FirstOrDefault();
			if (isChecked == 1 && f190002 == null)
			{
				F190002 addF190002 = new F190002()
				{
					TICKET_ID = SelectedData.TICKET_ID,
					WAREHOUSE_TYPE = type,
					CUST_CODE = SelectedData.CUST_CODE,
					GUP_CODE = SelectedData.GUP_CODE,
					DC_CODE = SelectedData.DC_CODE
				};
				proxy.AddToF190002s(addF190002);
				proxy.SaveChanges();
			}
			else if (isChecked == 0 && f190002 != null)
			{
				proxy.DeleteObject(f190002);
				proxy.SaveChanges();
			}
		}
		#endregion Save

		#endregion
	}
}
