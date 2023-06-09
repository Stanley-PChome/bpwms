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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1920190000_ViewModel : InputViewModelBase
	{
		private string _userId;
		private string _userName;
		private string _gupCode;
		private string _custCode;
		private readonly F19Entities _proxy;
		private bool _isValid;
		public Action AddAction = delegate { };
		public Action EditAction = delegate { };
		public Action SearchAction = delegate { };
		public P1920190000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_proxy = GetProxy<F19Entities>();
				InitControls();
				
			}

		}

		private void InitControls()
		{
			GetClassTypeList();
			if (ClassTypeList.Any())
				SelectedClassType = ClassTypeList[0].Value;

			GetClassTypeAList();
			GetClassTypeBList();
		}

		#region 類別
		private List<NameValuePair<string>> _classTypeList;

		public List<NameValuePair<string>> ClassTypeList
		{
			get { return _classTypeList; }
			set
			{
				Set(() => ClassTypeList, ref _classTypeList, value);
				//if (_classTypeList == value) return;
				//Set(() => ClassTypeList, ref _classTypeList, value);
			}
		}

		private string _selectedClassType;

		public string SelectedClassType
		{
			get { return _selectedClassType; }
			set
			{
				switch (value)
				{
					case "A":
						SelectedDataA = null;
						DataListA = null;
						break;
					case "B":
						GetClassTypeAList();
						SelectedDataB = null;
						DataListB = null;
						break;
					case "C":
						GetClassTypeAList();
						GetClassTypeBList();
						SelectedDataC = null;
						DataListC = null;
						break;
				}
				_selectedClassType = value;
				RaisePropertyChanged("SelectedClassType");
			}
		}

		private void GetClassTypeList()
		{
			ClassTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P1920190000", "CLASS_TYPE");
		}
		#endregion

		#region 分類編號
		private string _classCode = string.Empty;

		public string ClassCode
		{
			get { return _classCode; }
			set
			{
				if (_classCode == value) return;
				Set(() => ClassCode, ref _classCode, value);
			}
		}
		#endregion

		#region 分類名稱
		private string _className = string.Empty;

		public string ClassName
		{
			get { return _className; }
			set
			{
				if (_className == value) return;
				Set(() => ClassName, ref _className, value);
			}
		}
		#endregion

		#region 大分類
		private List<NameValuePair<string>> _classTypeAList= new List<NameValuePair<string>>();

		public List<NameValuePair<string>> ClassTypeAList
		{
			get { return _classTypeAList; }
			set
			{
				Set(() => ClassTypeAList, ref _classTypeAList, value);

				//if (_classTypeAList == value) return;
				//Set(() => ClassTypeAList, ref _classTypeAList, value);
			}
		}

		public void GetClassTypeAList()
		{
			var qry = (from p in _proxy.F1915s
					   where p.GUP_CODE == _gupCode && p.CUST_CODE == _custCode
					   select new NameValuePair<string>()
					   {
						   Value = p.ACODE,
						   Name = p.CLA_NAME
					   }).ToList().OrderBy(p => p.Value).ToList();

			ClassTypeAList = qry;
		}

		#endregion

		#region 中分類
		private Dictionary<string, List<NameValuePair<string>>> _classTypeBList;

		public Dictionary<string, List<NameValuePair<string>>> ClassTypeBList
		{
			get { return _classTypeBList; }
			set
			{
				if (_classTypeBList == value) return;
				Set(() => ClassTypeBList, ref _classTypeBList, value);
			}
		}


		public void GetClassTypeBList()
		{
			var proxy = GetProxy<F19Entities>();
			var groups = (from p in proxy.F1916s
						  where p.GUP_CODE == _gupCode && p.CUST_CODE == _custCode
						  select p).ToList().GroupBy(item => item.ACODE);
			 ClassTypeBList = groups.ToDictionary(g => g.Key, g => g.GroupBy(item => new { item.BCODE, item.CLA_NAME })
			  .Select(p => new NameValuePair<string>() { Value = p.Key.BCODE, Name = p.Key.CLA_NAME }).ToList());
			
		}
		#endregion



		#region 貨主大分類資料
		private ObservableCollection<P192019Item> _dataListA;

		public ObservableCollection<P192019Item> DataListA
		{
			get { return _dataListA; }
			set
			{
				Set(() => DataListA, ref _dataListA, value);
			}
		}
		#endregion


		#region 選取的貨主大分類
		private P192019Item _selectedDataA;

		public P192019Item SelectedDataA
		{
			get { return _selectedDataA; }
			set
			{
				Set(() => SelectedDataA, ref _selectedDataA, value);
			}
		}
		#endregion


		#region 貨主中分類資料
		private ObservableCollection<P192019Item> _dataListB;

		public ObservableCollection<P192019Item> DataListB
		{
			get { return _dataListB; }
			set
			{
				Set(() => DataListB, ref _dataListB, value);
			}
		}
		#endregion

		#region 選取的貨主中分類
		private P192019Item _selectedDataB;

		public P192019Item SelectedDataB
		{
			get { return _selectedDataB; }
			set
			{
				Set(() => SelectedDataB, ref _selectedDataB, value);
			}
		}
		#endregion

		#region 貨主小分類資料
		private ObservableCollection<P192019Item> _dataListC;

		public ObservableCollection<P192019Item> DataListC
		{
			get { return _dataListC; }
			set
			{
				Set(() => DataListC, ref _dataListC, value);
			}
		}
		#endregion

		#region 選取的貨主大分類
		private P192019Item _selectedDataC;

		public P192019Item SelectedDataC
		{
			get { return _selectedDataC; }
			set
			{
				Set(() => SelectedDataC, ref _selectedDataC, value);
			}
		}
		#endregion


		
		//private P192019Item _selectedData;

		//public P192019Item SelectedData
		//{
		//	get { return _selectedData; }
		//	set
		//	{
		//		if (_selectedData != null && (UserOperateMode == OperateMode.Edit))
		//			return;

		//		_selectedData = value;
		//		RaisePropertyChanged("SelectedData");
		//	}
		//}


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
			var proxyEx = GetExProxy<P19ExDataSource>();
			var datas = proxyEx.GetP192019SearchData(_gupCode,_custCode, ClassCode.Trim(), ClassName.Trim(), SelectedClassType).ToList();
			switch (SelectedClassType)
			{
				case "A":
					DataListA = datas.ToObservableCollection();
					break;
				case "B":
					DataListB = datas.ToObservableCollection();
					break;
				case "C":
					DataListC = datas.ToObservableCollection();
					break;
			}
		}

		private void DoSearchCompleted()
		{
			switch (SelectedClassType)
			{
				case "A":
					if(!DataListA.Any() && UserOperateMode == OperateMode.Query)
						ShowMessage(Messages.InfoNoData);
					SelectedDataA = DataListA.FirstOrDefault();
					break;
				case "B":
					if (!DataListB.Any() && UserOperateMode == OperateMode.Query)
						ShowMessage(Messages.InfoNoData);
					SelectedDataB = DataListB.FirstOrDefault();
					break;
				case "C":
					if (!DataListC.Any() && UserOperateMode == OperateMode.Query)
						ShowMessage(Messages.InfoNoData);
					SelectedDataC = DataListC.FirstOrDefault();
					break;
			}
			if(UserOperateMode == OperateMode.Query)
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
			var newItem = new P192019Item();
			newItem.CRT_DATE = DateTime.Now;
			switch (SelectedClassType)
			{
				case "A":
					if (DataListA == null)
						DataListA = new ObservableCollection<P192019Item>();
					DataListA.Add(newItem);
					SelectedDataA = DataListA.Last();
					break;
				case "B":
					if (DataListB == null)
						DataListB = new ObservableCollection<P192019Item>();
					DataListB.Add(newItem);
					SelectedDataB = DataListB.Last();
					break;
				case "C":
					if (DataListC == null)
						DataListC = new ObservableCollection<P192019Item>();
					DataListC.Add(newItem);
					SelectedDataC = DataListC.Last();
					break;
			}
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
				  o => DoEdit(),
				  () => UserOperateMode == OperateMode.Query && 
					((SelectedClassType=="A" && DataListA!=null && DataListA.Any() && SelectedDataA!=null) ||
					(SelectedClassType == "B" && DataListB != null && DataListB.Any() && SelectedDataB != null) ||
					(SelectedClassType == "C" && DataListC != null && DataListC.Any() && SelectedDataC != null)),
				  o => DoEditCompleted()
				  );
			}
		}

		private void DoEdit()
		{

		}

		private void DoEditCompleted()
		{
			EditAction();
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

		#region Cancel
		private P192019Item _oldSelectedData;
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoCancel(), () => UserOperateMode != OperateMode.Query, o => DoCancelCompleted(), null, ()=>{
						if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
							return;
					}
				  );
			}
		}

		private void DoCancel()
		{
			_oldSelectedData = null;
			if(UserOperateMode == OperateMode.Edit)
			{
				switch (SelectedClassType)
				{
					case "A":
						_oldSelectedData = SelectedDataA;
						break;
					case "B":
						_oldSelectedData = SelectedDataB;
						break;
					case "C":
						_oldSelectedData = SelectedDataC;
						break;
				}
			}
			DoSearch();
			
		}

		private void DoCancelCompleted()
		{
			DoSearchCompleted();
			if(UserOperateMode == OperateMode.Edit)
			{
				switch (SelectedClassType)
				{
					case "A":
						SelectedDataA = DataListA.FirstOrDefault(x=> x.ACODE == _oldSelectedData.ACODE);
						break;
					case "B":
						SelectedDataB = DataListB.FirstOrDefault(x => x.ACODE == _oldSelectedData.ACODE && x.BCODE == _oldSelectedData.BCODE);
						break;
					case "C":
						SelectedDataC = DataListC.FirstOrDefault(x => x.ACODE == _oldSelectedData.ACODE &&x.BCODE == _oldSelectedData.BCODE && x.CCODE == _oldSelectedData.CCODE);
						break;
				}
			}
			UserOperateMode = OperateMode.Query;
			SearchAction();
		}

		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoDelete(), () => UserOperateMode == OperateMode.Query && 
					((SelectedClassType =="A" && SelectedDataA!=null) ||
					(SelectedClassType == "B" && SelectedDataB != null) ||
					(SelectedClassType == "C" && SelectedDataC != null)),
				  o => DoDeleteCompleted()
				  );
			}
		}

		private void DoDelete()
		{
			_isValid = false;
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
				return;
			var proxy = GetProxy<F19Entities>();
			var custData = proxy.F1909s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode).FirstOrDefault();
			if (custData != null && custData.ALLOWGUP_ITEMCATEGORYSHARE == "1")
			{
				var classTypeName = ClassTypeList.First(x => x.Value == SelectedClassType).Name;
				if (ShowConfirmMessage(Properties.Resources.P1920190000_WillDeleteOtherSharingCustSet) == DialogResponse.No)
					return;
			}

			var claName = string.Empty;
			P192019Item selectedData = new P192019Item();
			switch (_selectedClassType)
			{
				case "A":
					claName = SelectedDataA.ANAME;
					selectedData = SelectedDataA;
					break;
				case "B":
					claName = SelectedDataB.BNAME;
					selectedData = SelectedDataB;
					break;
				case "C":
					claName = SelectedDataC.CNAME;
					selectedData = SelectedDataC;
					break;
			}
			var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			var result = wcfProxy.RunWcfMethod(w => w.DeleteCls(new wcf.P192019Data { GupCode = _gupCode, CustCode = _custCode, ACode = selectedData.ACODE, BCode = selectedData.BCODE, CCode = selectedData.CCODE, CheckPercent = string.IsNullOrEmpty(selectedData.CHECK_PERCENT) ? new decimal?() : decimal.Parse(selectedData.CHECK_PERCENT), ClaName = claName, ClaType = SelectedClassType }));
			
			if(result.IsSuccessed)
			{
				DoSearch();
				_isValid = true;
			}
			else
				ShowWarningMessage(result.Message);
			
		}

		private void DoDeleteCompleted()
		{
			if (!_isValid) return;
			DoSearchCompleted();
			UserOperateMode = OperateMode.Query;
			ShowMessage(Messages.DeleteSuccess);
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
				  o => DoSave(), () => UserOperateMode != OperateMode.Query, o => DoSaveCompleted()
				  );
			}
		}

	
		private void DoSave()
		{
			_isValid = false;

			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
				return;

			var proxy = GetProxy<F19Entities>();
			var custData = proxy.F1909s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode).FirstOrDefault();
			if(custData!=null && custData.ALLOWGUP_ITEMCATEGORYSHARE == "1")
			{
				var classTypeName = ClassTypeList.First(x => x.Value == SelectedClassType).Name;
				if (ShowConfirmMessage(Properties.Resources.P1920190000_WillInsertOrUpdateOtherSharingCustSameSet) == DialogResponse.No)
					return;
			}
			P192019Item selectedData = new P192019Item();
			var claName = string.Empty;
			switch(_selectedClassType)
			{
				case "A":
					if (string.IsNullOrWhiteSpace(SelectedDataA.ACODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputACODE);
						return;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataA.ANAME))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputANAME);
						return;
					}
					selectedData = SelectedDataA;
					claName = SelectedDataA.ANAME;
					break;
				case "B":
					if (string.IsNullOrWhiteSpace(SelectedDataB.ACODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_SelectACODE);
						return;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataB.BCODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputBCODE);
						return;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataB.BNAME))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputBNAME);
						return;
					}
					selectedData = SelectedDataB;
					claName = SelectedDataB.BNAME;
					break;
				case "C":
					if (string.IsNullOrWhiteSpace(SelectedDataC.ACODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_SelectACODE);
						return;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataC.BCODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_SelectBCODE);
						return;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataC.CCODE))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputCCODE);
						return ;
					}

					if (string.IsNullOrWhiteSpace(SelectedDataC.CNAME))
					{
						DialogService.ShowMessage(Properties.Resources.P1920190000_InputCNAME);
						return;
					}
					selectedData = SelectedDataC;
					claName = SelectedDataC.CNAME;
					break;
			}
			if (!ValidCheckPercent())
				return;

			switch (_selectedClassType)
			{
				case "A":
					break;
				case "B":
					selectedData = SelectedDataB;
					break;
				case "C":
					selectedData = SelectedDataC;
					break;
			}

			var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
			var result = wcfProxy.RunWcfMethod(w => w.InsertOrUpdateCls(new wcf.P192019Data {GupCode = _gupCode,CustCode = _custCode,ACode = selectedData.ACODE,BCode = selectedData.BCODE,CCode = selectedData.CCODE,CheckPercent = string.IsNullOrEmpty(selectedData.CHECK_PERCENT) ? new decimal?() : decimal.Parse(selectedData.CHECK_PERCENT),ClaName = claName,ClaType = SelectedClassType },UserOperateMode == OperateMode.Add));
				ShowResultMessage(result);
			if (result.IsSuccessed)
			{
				DoSearch();
				switch (_selectedClassType)
				{
					case "A":
						SelectedDataA = DataListA.FirstOrDefault(x => x.ACODE == selectedData.ACODE);
						break;
					case "B":
						SelectedDataB = DataListB.FirstOrDefault(x => x.ACODE == selectedData.ACODE && x.BCODE == selectedData.BCODE);
						break;
					case "C":
						SelectedDataC = DataListC.FirstOrDefault(x => x.ACODE == selectedData.ACODE && x.BCODE == selectedData.BCODE && x.CCODE == selectedData.CCODE);
						break;
				}
				_isValid = true;
			}
		}

		private void DoSaveCompleted()
		{
			
			if (!_isValid) return;
			UserOperateMode = OperateMode.Query;
			SearchAction();
		}

		private bool ValidCheckPercent()
		{
			var checkPercent = string.Empty;
			switch (_selectedClassType)
			{
				case "A":
					checkPercent = SelectedDataA?.CHECK_PERCENT?.Trim();
					break;
				case "B":
					checkPercent = SelectedDataB?.CHECK_PERCENT?.Trim();
					break;
				case "C":
					checkPercent = SelectedDataC?.CHECK_PERCENT?.Trim();
					break;
			}
			if (string.IsNullOrWhiteSpace(checkPercent))
			{
				//DialogService.ShowMessage(Properties.Resources.P1920190000_InputCheckPercent);
				//return false;
				//SelectedData.CHECK_PERCENT = "0";
				return true;
			}

			var msg = Properties.Resources.P1920190000_dCheckPercentChecking;
			decimal dCheckPercent;
			if (!decimal.TryParse(checkPercent, out dCheckPercent))
			{
				DialogService.ShowMessage(msg);
				return false;
			}

			if (dCheckPercent < 0)
			{
				DialogService.ShowMessage(msg);
				return false;
			}

			if (checkPercent.Replace(".", "").Length >= 14)
			{
				DialogService.ShowMessage(msg);
				return false;
			}
           
            if (Convert.ToDecimal(checkPercent) > 100)
            {
                DialogService.ShowMessage(msg);
                return false;
            }

            var arrTmp = checkPercent.Split('.');
			if (arrTmp[0].Length > 3)
			{
				DialogService.ShowMessage(msg);
				return false;
			}

			if (arrTmp.Length > 1 && arrTmp[1].Length > 11)
			{
				switch (_selectedClassType)
				{
					case "A":
						SelectedDataA.CHECK_PERCENT = arrTmp[0] + "." + arrTmp[1].Substring(0, 11);
						break;
					case "B":
						SelectedDataB.CHECK_PERCENT = arrTmp[0] + "." + arrTmp[1].Substring(0,11);
						break;
					case "C":
						SelectedDataC.CHECK_PERCENT = arrTmp[0] + "." + arrTmp[1].Substring(0, 11);
						break;
				}
			}
			return true;
		}

		
		#endregion Save
	}

}
