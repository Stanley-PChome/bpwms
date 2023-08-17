using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;

using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0806130000_ViewModel : InputViewModelBase
	{

		#region Constructor 
		public Action<bool> DGBtnSwitch = delegate { };
		public Action OnSearchEmpIDComplete = delegate { };
		public Action OnSearchOrderNoComplete = delegate { };

		public P0806130000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				SetDcList();
				ModelDataSet(ModelType.OrigionMode);
				InitialObject(true);
			}
		}

		#endregion

		#region 物流中心
		/// <summary>
		/// 物流中心combobox變數
		/// </summary>
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				if (_dcList == value)
					return;
				Set(() => DcList, ref _dcList, value);
			}
		}

		/// <summary>
		/// 選取的物流中心物件
		/// </summary>
		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				if (_selectedDcCode == value)
					return;
				Set(() => SelectedDcCode, ref _selectedDcCode, value);

			}
		}

		/// <summary>
		/// 取得物流中心combobox資料的方法
		/// </summary>
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}

		#endregion

		#region Property 環境變數
		private readonly string _userId;
		private readonly string _userName;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		private DateTime _crtDate = DateTime.Today;
		public DateTime CrtDate
		{
			get { return _crtDate; }
			set { _crtDate = value; RaisePropertyChanged("CrtDate"); }
		}
		#endregion

		#region 工號、名稱

		/// <summary>
		/// 工號變數
		/// </summary>
		private string _empID;

		public string EmpID
		{
			get { return _empID; }
			set
			{
				_empID = value;
				RaisePropertyChanged("EmpID");
			}
		}

		/// <summary>
		/// 工號名稱變數
		/// </summary>
		private string _empName;

		public string EmpName
		{
			get { return _empName; }
			set
			{
				_empName = value;
				RaisePropertyChanged("EmpName");
			}
		}

		#endregion

		#region 查詢用工號、揀貨單TextBox屬性

		/// <summary>
		/// 查詢工號的屬性
		/// </summary>
		private string _empIDSearch;

		public string EmpIDSearch
		{
			get { return _empIDSearch; }
			set
			{
				_empIDSearch = value;
				RaisePropertyChanged("EmpIDSearch");
			}
		}

		/// <summary>
		/// 查詢揀貨單用的屬性
		/// </summary>

		private string _orderNOSearch;

		public string OrderNOSearch
		{
			get { return _orderNOSearch; }
			set
			{
				_orderNOSearch = value;
				RaisePropertyChanged("OrderNOSearch");
			}
		}

		//private string _labelText;
		//public string LabelText
		//{
		//    get { return _labelText; }
		//    set
		//    {
		//        _labelText = value;
		//        RaisePropertyChanged("LabelText");
		//    }
		//}


		#endregion

		#region 揀貨單號、狀態
		/// <summary>
		/// 揀貨單號的textbox變數
		/// </summary>
		private string _orderNO;

		public string OrderNO
		{
			get { return _orderNO; }
			set
			{
				_orderNO = value;
				RaisePropertyChanged("OrderNO");
			}
		}

		#endregion

		#region 模組切換用物件
		/// <summary>
		/// 模組用的物件
		/// </summary>
		private ModelSetting _modelSet = new ModelSetting()
		{
			LabEmpID = "Visible",
			LabOrderNO = "Visible",
			LabEmpIDBrush = "Collapsed",
			LabOrderNOBrush = "Collapsed",
			TxbEmpID = "Collapsed",
			TxbEmpIDEnable = false,
			TxbOrderNo = "Collapsed",
			TxbOrderNoEnable = false,
			CobDC = true,
			BtnEmpIDBind = "Visible",
			BtnBindComplete = "Collapsed",
			BtnBindCompleteEnable = false,
			BtnExit = "Visible",
			BtnCancel = "Collapsed",
			BtnEmpIDComplete = "Visible",
			BtnEmpIDCompleteEnable = true,
			BtnPickComplete = "Collapsed",
			GDBtnDelete = "Hidden",
			TxbEmpIDSearch = "Visible",
			TxbOrderNoSearch = "Visible",
			BtnSearchEnable = true
		};

		public ModelSetting ModelSet
		{
			get { return _modelSet; }
			set
			{
				_modelSet = value;
				RaisePropertyChanged("ModelSet");
			}
		}

		#endregion

		public ICommand AddGDAction
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => Check(),
						() => !string.IsNullOrWhiteSpace(OrderNO) && !string.IsNullOrWhiteSpace(EmpID),
						o =>
						{
							DispatcherAction(() =>
											{
									if (SetEmpIDInfo())
										SetOrderNOInfo();
								});

						}
						);
			}

		}
		public void Check() { }

		#region Grid資料物件

		/// <summary>
		/// DataGrid的資料包
		/// </summary>
		private ObservableCollection<F0011BindData> _empOrderList;

		public ObservableCollection<F0011BindData> EmpOrderList
		{
			get { return _empOrderList; }
			set
			{
				_empOrderList = value;
				RaisePropertyChanged("EmpOrderList");
			}
		}

		private F0011BindData _selectedF0011BindData;

		public F0011BindData SelectedF0011BindData
		{
			get { return _selectedF0011BindData; }
			set
			{
				_selectedF0011BindData = value;
				RaisePropertyChanged("SelectedF0011BindData");
			}
		}

		#endregion

		#region 取得工號名稱_查詢
		public void GetSearchEmpName()
		{
			var proxy = GetProxy<F19Entities>();
			var empData = proxy.F1924s.Where(o => o.EMP_ID == EmpIDSearch && o.ISDELETED == "0").FirstOrDefault();
			EmpName = empData == null ? string.Empty : empData.EMP_NAME;
		}
		#endregion


		#region 工號、揀貨單刷讀

		/// <summary>
		/// 檢驗輸入的員工ID
		/// </summary>
		/// <returns></returns>
		public bool SetEmpIDInfo()
		{
			bool isFind = false;
			var proxy = GetProxy<F19Entities>();
			var empData = proxy.F1924s.Where(o => o.EMP_ID == EmpID && o.ISDELETED == "0").ToList();

			switch (ModelSet.Type)
			{
				case ModelType.BindMode:
					if (IsEmpIDExist(EmpID))
					{
						EmpID = empData.FirstOrDefault().EMP_ID;
						EmpName = empData.FirstOrDefault().EMP_NAME;
						OnSearchOrderNoComplete();
						isFind = true;
					}
					else
					{
						//工號不正確
						EmpName = Properties.Resources.P0806130000_EmpIDError;
						OnSearchEmpIDComplete();
						isFind = false;
					}
					break;
				case ModelType.CompleteMode:

					if (IsEmpIDExist(EmpID))
					{
						EmpOrderList.Clear();
						EmpID = empData.FirstOrDefault().EMP_ID;
						EmpName = empData.FirstOrDefault().EMP_NAME;
						var proxyP08Ex = GetExProxy<P08ExDataSource>();
						var resultData = proxyP08Ex.CreateQuery<F0011BindData>("GetP0806130000Detail")
								.AddQueryExOption("dcCode", SelectedDcCode)
								.AddQueryExOption("gupCode", _gupCode)
								.AddQueryExOption("custCode", _custCode)
								.AddQueryExOption("empID", EmpID)
								.AddQueryExOption("status", "0").ToList();
						BindData(resultData);
						RaisePropertyChanged("EmpOrderList");
						OnSearchOrderNoComplete();
						isFind = true;
					}
					else
					{
						//工號不正確
						EmpName = Properties.Resources.P0806130000_EmpIDError;
						OrderNO = null;
						EmpOrderList = new ObservableCollection<F0011BindData>();
						OnSearchEmpIDComplete();
						isFind = false;
					}
					break;
			}

			return isFind;
		}

		/// <summary>
		/// 檢核工號是否存在
		/// </summary>
		/// <param name="empID"></param>
		/// <returns></returns>
		private bool IsEmpIDExist(string empID)
		{
			bool result = false;
			var proxy = GetProxy<F19Entities>();
			var empData = proxy.F1924s.Where(o => o.EMP_ID == empID && o.ISDELETED == "0").ToList();
			if (empData.Any())
			{
				if (IsAuthorization(empData.FirstOrDefault(), SelectedDcCode))
					result = true;
				else
					result = false;
			}
			else
				result = false;

			return result;
		}

		/// <summary>
		/// 檢驗員工在該DC是否有授權
		/// </summary>
		/// <param name="f1924"></param>
		/// <param name="selectedDcCode"></param>
		/// <returns></returns>
		private bool IsAuthorization(F1924 f1924, string selectedDcCode)
		{
			bool result = false;
			var proxy = GetProxy<F19Entities>();
			var empAuthorization = proxy.F192402s.Where(o => o.EMP_ID == f1924.EMP_ID && o.DC_CODE == selectedDcCode).ToList();
			if (empAuthorization.Any())
				result = true;
			else
				result = false;

			return result;
		}

		private void BindData(List<F0011BindData> resultData)
		{
			EmpOrderList = new ObservableCollection<F0011BindData>();
			if (resultData.Any())
			{
				foreach (var item in resultData)
				{
					EmpOrderList.Add(new F0011BindData()
					{
						CLOSE_DATE = item.CLOSE_DATE,
						CRT_DATE = item.CRT_DATE,
						CRT_NAME = item.CRT_NAME,
						CUST_CODE = item.CUST_CODE,
						DC_CODE = item.DC_CODE,
						EMP_ID = item.EMP_ID,
						EMP_NAME = item.EMP_NAME,
						GUP_CODE = item.GUP_CODE,
						ID = item.ID,
						ORDER_NO = item.ORDER_NO,
						PICK_STATUS = item.PICK_STATUS,
						ROWNUM = item.ROWNUM,
						START_DATE = item.START_DATE,
						STATUS = item.STATUS,
						UPD_DATE = item.UPD_DATE,
						UPD_NAME = item.UPD_NAME
					});
				}
			}
		}

		private void ResetDataGridPickComplete(ObservableCollection<F0011BindData> resultData)
		{
			List<F0011BindData> result = new List<F0011BindData>();

			foreach (var item in resultData)
			{
				item.CLOSE_DATE = DateTime.Now;
				item.STATUS = "1";
			}
		}

		/// <summary>
		/// 檢驗輸入的揀貨單號
		/// </summary>
		/// <returns></returns>
		public bool SetOrderNOInfo()
		{
			OrderNO = OrderNO?.Trim().ToUpper();
			bool isFind = false;
			List<F0011BindData> repeactEmpOrderList = EmpOrderList.Where(x => x.EMP_ID == EmpID &&
																			x.ORDER_NO == OrderNO &&
																			x.STATUS == "0" &&
																			x.DC_CODE == SelectedDcCode &&
																			x.GUP_CODE == _gupCode &&
																			x.CUST_CODE == _custCode).ToList();

            //檢核單號是否有重複綁定
			if (repeactEmpOrderList.Any())
			{
				DialogService.ShowMessage(Properties.Resources.P0806130000_RepeactItem);
				OnSearchOrderNoComplete();
				isFind = false;
				return isFind;
			}


			// 檢驗單號、工號
			var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.CheckOrderNo(SelectedDcCode, _gupCode, _custCode, OrderNO, EmpID));

			if (result.IsSuccessed)
			{
				EmpOrderList.Add(new F0011BindData()
				{
					EMP_ID = EmpID,
					EMP_NAME = EmpName,
					ORDER_NO = OrderNO,
					PICK_STATUS = Properties.Resources.P0806130000_StatusList_0,
					STATUS = "0",
					DC_CODE = SelectedDcCode,
					GUP_CODE = _gupCode,
					CUST_CODE = _custCode,
					CRT_NAME = _userName
				});

				RaisePropertyChanged("EmpOrderList");
				OnSearchOrderNoComplete();
				isFind = true;
			}
			else
			{
				ShowWarningMessage(result.Message);
				OnSearchOrderNoComplete();
				isFind = false;
			}

			return isFind;
		}

		#endregion

		#region 按鈕功能

		#region 查詢

		/// <summary>
		/// 查詢
		/// </summary>
		public ICommand EmpOrdBindSerachCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSerach(), () => true, o => { DGBtnSwitch(true); }
						);
			}
		}

		/// <summary>
		/// 按下查詢的方法
		/// </summary>
		public void DoSerach()
		{
			
			//GetSearchEmpName();
			if(!string.IsNullOrWhiteSpace(EmpIDSearch))
			{
				EmpIDSearch = EmpIDSearch?.Trim();
				var proxy = GetProxy<F19Entities>();
				var empData = proxy.F1924s.Where(o => o.EMP_ID == EmpIDSearch && o.ISDELETED == "0").ToList();
				if (empData == null || !empData.Any())
				{
					ShowWarningMessage(Properties.Resources.P0806130000_EmpIDError);
					return;
				}
				else
				{
					EmpIDSearch = empData.First().EMP_ID;
					EmpName = empData.First().EMP_NAME;
				}
			}
			else
			{
				EmpName = string.Empty;
			}

			if (!string.IsNullOrWhiteSpace(OrderNOSearch))
			{
				OrderNOSearch = OrderNOSearch?.Trim().ToUpper();
			}

				EmpOrderList = new ObservableCollection<F0011BindData>();
			var proxyP08Ex = GetExProxy<P08ExDataSource>();
			var resultData = proxyP08Ex.CreateQuery<F0011BindData>("GetP0806130000SearchData")
					.AddQueryExOption("dcCode", SelectedDcCode)
					.AddQueryExOption("gupCode", _gupCode)
					.AddQueryExOption("custCode", _custCode)
					.AddQueryExOption("empID", EmpIDSearch)
					.AddQueryExOption("orderNo", OrderNOSearch)
					.AddQueryExOption("crtDate", _crtDate).ToList();
			if (resultData.Any())
			{
				BindData(resultData);
				RaisePropertyChanged("EmpOrderList");
			}
			else
				ShowWarningMessage(Properties.Resources.P0806130000_SearchErr);
		}

		#endregion

		#region 工號綁定

		/// <summary>
		/// 按下工號綁定
		/// </summary>
		public ICommand EmpBindCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEmpBind(), () => true, o => { DGBtnSwitch(false);  }
						);
			}
		}

		/// <summary>
		/// 按下工號綁定的方法
		/// </summary>
		public void DoEmpBind()
		{
			InitialObject(true);
			ModelDataSet(ModelType.BindMode);
		}

		#endregion

		#region 綁定完成

		/// <summary>
		/// 綁定完成
		/// </summary>

		public ICommand BindCompleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(

						o => DoBindComplete(),()=>true, o => { DGBtnSwitch(true); }
						);
			}
		}

		/// <summary>
		/// 按下綁定完成的方法
		/// </summary>
		private void DoBindComplete()
		{
			if (EmpOrderList.Any())
			{
				var proxy08 = new wcf.P08WcfServiceClient();
				var wcfData = ExDataMapper.MapCollection<F0011BindData, wcf.F0011BindData>(EmpOrderList);
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy08.InnerChannel,
								() => proxy08.BindComplete(SelectedDcCode, _gupCode, _custCode, wcfData.ToArray()));

				RaisePropertyChanged("EmpOrderList");
        if (result.IsSuccessed)
        {
          ShowInfoMessage(Properties.Resources.P0806130000_SaveSucess);
          //完成後回到初始畫面
          ModelDataSet(ModelType.OrigionMode);
          InitialObject(true);
        }
        else
        {
          ShowWarningMessage(result.Message);

          #region 把已經取消的揀貨單從清單中移除
          var IsCancelOrder = string.IsNullOrWhiteSpace(result.No) ? Array.Empty<string>() : result.No.Split(',');
          EmpOrderList = EmpOrderList.Where(x => !IsCancelOrder.Contains(x.ORDER_NO)).ToObservableCollection();
          #endregion 把已經取消的揀貨單從清單中移除
        }

      }
			else
			{
				//請刷讀要綁定的揀貨單
				ShowWarningMessage(Properties.Resources.P0806130000_PleaseRushOrderNo);
			}
		}

		#endregion

		#region 工號完成

		/// <summary>
		/// 按下工號完成
		/// </summary>
		public ICommand EmpIDCompleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEmpIDComplete(), () => true, o => { DGBtnSwitch(false); }
						);
			}
		}

		/// <summary>
		/// 按下工號完成
		/// </summary>
		public void DoEmpIDComplete()
		{
			InitialObject(true);
			ModelDataSet(ModelType.CompleteMode);
		}

		#endregion

		#region Delete

		/// <summary>
		/// 刪除綁定的資料
		/// </summary>
		/// <param name="btn"></param>
		public void DoDelete(Button btn)
		{
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
			{
				return;
			}

			var x = btn.DataContext;

      if (ModelSet.Type == ModelType.BindMode)
      {
        EmpOrderList.Remove(EmpOrderList.Where(o => o.ORDER_NO == ((F0011BindData)x).ORDER_NO).FirstOrDefault());
      }
      else
      {
			  // 檢驗單號、工號
			  var proxy = GetWcfProxy<wcf.P08WcfServiceClient>();
			  var result = proxy.RunWcfMethod(w => w.DeleteEmpPickBind(((F0011BindData)x).ID.Value));
			  if (result.IsSuccessed)
				  //刪除成功
				  ShowInfoMessage(Properties.Resources.P0806130000_DeleteSucess);
			  else
				  ShowWarningMessage(result.Message);

			  EmpOrdBindSerachCommand.Execute(null);
      }
		}

		#endregion Delete

		#region 揀貨完成

		/// <summary>
		/// 揀貨完成
		/// </summary>
		public ICommand PickCompleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoPickComplete(),()=>true, o => { DGBtnSwitch(true); }
						);
			}
		}

		/// <summary>
		/// 揀貨完成的方法
		/// </summary>
		private void DoPickComplete()
		{
			if (EmpOrderList.Any())
			{
        var proxyP08Ex = GetExProxy<P08ExDataSource>();
				var proxy = new wcf.P08WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
								() => proxy.UpdateP0806130000Data(SelectedDcCode, _gupCode, _custCode, ExDataMapper.MapCollection<F0011BindData, wcf.F0011BindData>(EmpOrderList).ToArray()));

        if (result.IsSuccessed)
				{
          if (!string.IsNullOrWhiteSpace(result.Message))
            ShowInfoMessage(result.Message);

          //揀貨完成
          ResetDataGridPickComplete(EmpOrderList);
					ShowInfoMessage(Properties.Resources.P0806130000_PickSucess);

					//完成後回到初始畫面
					ModelDataSet(ModelType.OrigionMode);
					InitialObject(true);
				}
				else
					ShowWarningMessage(result.Message);
			}
			else
			{
				//無須更新的單據
				ShowWarningMessage(Properties.Resources.P0806130000_DontUpdateOrder);
			}
		}

		#endregion

		#region 取消

		/// <summary>
		/// 取消
		/// </summary>
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCancel(),()=>true, o => { DGBtnSwitch(true); }
						);
			}
		}

		/// <summary>
		/// 按下取消的方法
		/// </summary>
		private void DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
			{
				return;
			}
			InitialObject(true);
			ModelDataSet(ModelType.OrigionMode);
		}

		#endregion

		#endregion

		/// <summary>
		/// 初始化物件
		/// </summary>
		public void InitialObject(bool isEmpNameInit)
		{
			EmpID = null;
			if (isEmpNameInit)
				EmpName = null;
			OrderNO = null;
			EmpOrderList = new ObservableCollection<F0011BindData>();
			EmpIDSearch = null;
			OrderNOSearch = null;
		}

		#region 切換模組

		/// <summary>
		/// 模組類型
		/// </summary>
		public enum ModelType
		{
			/// <summary>
			/// 原始模組
			/// </summary>
			OrigionMode = 0,
			/// <summary>
			/// 工號綁定模組
			/// </summary>
			BindMode = 1,
			/// <summary>
			/// 工號完成模組
			/// </summary>
			CompleteMode = 2
		}
		public class ModelSetting
		{
			public ModelType Type { get; set; }
			/// <summary>
			/// 工號Lable開關
			/// </summary>
			public string LabEmpID { get; set; }
			/// <summary>
			/// 揀貨單號Lable開關
			/// </summary>
			public string LabOrderNO { get; set; }
			/// <summary>
			/// 請刷入工號Lable開關
			/// </summary>
			public string LabEmpIDBrush { get; set; }
			/// <summary>
			/// 請刷入揀貨單號Lable開關
			/// </summary>
			public string LabOrderNOBrush { get; set; }
			/// <summary>
			/// 工號TextBox的顯示開關
			/// </summary>
			public string TxbEmpID { get; set; }
			/// <summary>
			/// 工號TextBox的Enable開關
			/// </summary>
			public bool TxbEmpIDEnable { get; set; }
			/// <summary>
			/// 揀貨單號TextBox的顯示開關
			/// </summary>
			public string TxbOrderNo { get; set; }
			/// <summary>
			/// 揀貨單號TextBox的Enable開關
			/// </summary>
			public bool TxbOrderNoEnable { get; set; }
			/// <summary>
			/// 物流中心ComboBox的Enable開關
			/// </summary>
			public bool CobDC { get; set; }
			/// <summary>
			/// 工號綁定Button開關
			/// </summary>
			public string BtnEmpIDBind { get; set; }
			/// <summary>
			/// 綁定完成Button開關
			/// </summary>
			public string BtnBindComplete { get; set; }
			/// <summary>
			/// 綁定完成Button的Enable開關
			/// </summary>
			public bool BtnBindCompleteEnable { get; set; }
			/// <summary>
			/// 離開Button開關
			/// </summary>
			public string BtnExit { get; set; }
			/// <summary>
			/// 取消Button開關
			/// </summary>
			public string BtnCancel { get; set; }
			/// <summary>
			/// 工號完成Button開關
			/// </summary>
			public string BtnEmpIDComplete { get; set; }
			/// <summary>
			/// 工號完成Button的Enable開關
			/// </summary>
			public bool BtnEmpIDCompleteEnable { get; set; }
			/// <summary>
			/// 揀貨完成Button開關
			/// </summary>
			public string BtnPickComplete { get; set; }
			/// <summary>
			/// Grid裡面的Button開關
			/// </summary>
			public string GDBtnDelete { get; set; }
			/// <summary>
			/// 工號查詢的TextBox開關
			/// </summary>
			public string TxbEmpIDSearch { get; set; }
			/// <summary>
			/// 揀貨單查詢的TextBox開關
			/// </summary>
			public string TxbOrderNoSearch { get; set; }
			/// <summary>
			/// 查詢Button的Enable開關
			/// </summary>
			public bool BtnSearchEnable { get; set; }
		}

		/// <summary>
		/// 模組設定
		/// </summary>
		/// <param name="mt"></param>
		public void ModelDataSet(ModelType mt)
		{
			switch (mt)
			{
				case ModelType.OrigionMode:
					ModelSet.LabEmpID = "Visible";
					ModelSet.LabOrderNO = "Visible";
					ModelSet.LabEmpIDBrush = "Collapsed";
					ModelSet.LabOrderNOBrush = "Collapsed";
					ModelSet.TxbEmpID = "Collapsed";
					ModelSet.TxbEmpIDEnable = false;
					ModelSet.TxbOrderNo = "Collapsed";
					ModelSet.TxbOrderNoEnable = false;
					ModelSet.CobDC = true;
					ModelSet.BtnEmpIDBind = "Visible";
					ModelSet.BtnBindComplete = "Collapsed";
					ModelSet.BtnBindCompleteEnable = false;
					ModelSet.BtnExit = "Visible";
					ModelSet.BtnCancel = "Collapsed";
					ModelSet.BtnEmpIDComplete = "Visible";
					ModelSet.BtnEmpIDCompleteEnable = true;
					ModelSet.BtnPickComplete = "Collapsed";
					ModelSet.GDBtnDelete = "Hidden";
					ModelSet.Type = ModelType.OrigionMode;
					ModelSet.TxbEmpIDSearch = "Visible";
					ModelSet.TxbOrderNoSearch = "Visible";
					ModelSet.BtnSearchEnable = true;
					// LabelText = Properties.Resources.P0806130000_Lab_EMP_ID;
					break;
				case ModelType.BindMode:
					ModelSet.LabEmpID = "Collapsed";
					ModelSet.LabOrderNO = "Collapsed";
					ModelSet.LabEmpIDBrush = "Visible";
					ModelSet.LabOrderNOBrush = "Visible";
					ModelSet.TxbEmpID = "Visible";
					ModelSet.TxbEmpIDEnable = true;
					ModelSet.TxbOrderNo = "Visible";
					ModelSet.TxbOrderNoEnable = true;
					ModelSet.CobDC = false;
					ModelSet.BtnEmpIDBind = "Collapsed";
					ModelSet.BtnBindComplete = "Visible";
					ModelSet.BtnBindCompleteEnable = true;
					ModelSet.BtnExit = "Collapsed";
					ModelSet.BtnCancel = "Visible";
					ModelSet.BtnEmpIDComplete = "Visible";
					ModelSet.BtnEmpIDCompleteEnable = false;
					ModelSet.BtnPickComplete = "Collapsed";
					ModelSet.GDBtnDelete = "Hidden";
					ModelSet.Type = ModelType.BindMode;
					ModelSet.TxbEmpIDSearch = "Collapsed";
					ModelSet.TxbOrderNoSearch = "Collapsed";
					ModelSet.BtnSearchEnable = false;
					// LabelText = Properties.Resources.P0806130000_Lab_EMP_ID_Brush;
					break;
				case ModelType.CompleteMode:
					ModelSet.LabEmpID = "Collapsed";
					ModelSet.LabOrderNO = "Collapsed";
					ModelSet.LabEmpIDBrush = "Visible";
					ModelSet.LabOrderNOBrush = "Visible";
					ModelSet.TxbEmpID = "Visible";
					ModelSet.TxbEmpIDEnable = true;
					ModelSet.TxbOrderNo = "Visible";
					ModelSet.TxbOrderNoEnable = false;
					ModelSet.CobDC = false;
					ModelSet.BtnEmpIDBind = "Collapsed";
					ModelSet.BtnBindComplete = "Visible";
					ModelSet.BtnBindCompleteEnable = false;
					ModelSet.BtnExit = "Collapsed";
					ModelSet.BtnCancel = "Visible";
					ModelSet.BtnEmpIDComplete = "Collapsed";
					ModelSet.BtnEmpIDCompleteEnable = true;
					ModelSet.BtnPickComplete = "Visible";
					ModelSet.GDBtnDelete = "Visible";
					ModelSet.Type = ModelType.CompleteMode;
					ModelSet.TxbEmpIDSearch = "Collapsed";
					ModelSet.TxbOrderNoSearch = "Collapsed";
					ModelSet.BtnSearchEnable = false;
					//LabelText = Properties.Resources.P0806130000_Lab_EMP_ID_Brush;
					break;
				default:
					break;
			}
			RaisePropertyChanged("ModelSet");
		}

		#endregion

	}
}
