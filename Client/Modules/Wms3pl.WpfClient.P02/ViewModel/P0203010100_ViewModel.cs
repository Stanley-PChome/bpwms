using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0203010100_ViewModel : InputViewModelBase
	{
		public P0203010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.CurrentUserInfo.Account;
			}

		}

		#region Property

		private readonly string _userId;
		public bool IsChangedData;
		public Action ExitClick = delegate { };
		public Action ClosedSuccessClick = delegate { };
		private F1510Data _f1510Data { get; set; }
		public string _status;

		#region 商品編號

		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				_itemCode = value;
				RaisePropertyChanged("ItemCode");
			}
		}

		#endregion

		#region 商品名稱

		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				_itemName = value;
				RaisePropertyChanged("ItemName");
			}
		}

		#endregion

		public string _validDate;
		public string ValidDate
		{
			get { return _validDate; }
			set { Set(ref _validDate, value); }
		}

		#region 總上架數

		private int _totalQty;

		public int TotalQty
		{
			get { return _totalQty; }
			set
			{
				_totalQty = value;
				RaisePropertyChanged("TotalQty");
			}
		}

		#endregion

		#region 上架儲位

		private string _locCode;

		public string LocCode
		{
			get { return _locCode; }
			set
			{
				_locCode = value;
				RaisePropertyChanged("LocCode");
			}
		}

		#endregion

		#region  上架數量

		private string _locQty;

		public string LocQty
		{
			get { return _locQty; }
			set
			{
				_locQty = value;
				RaisePropertyChanged("LocQty");
			}
		}

		#endregion

		#region 調撥單商品上架儲位資料 Grid List

		private List<F1510ItemLocData> _f1510ItemLocDatas;

		public List<F1510ItemLocData> F1510ItemLocDatas
		{
			get { return _f1510ItemLocDatas; }
			set
			{
				_f1510ItemLocDatas = value;
				RaisePropertyChanged("F1510ItemLocDatas");
			}
		}

		private F1510ItemLocData _selectedF1510ItemLocData;

		public F1510ItemLocData SelectedF1510ItemLocData
		{
			get { return _selectedF1510ItemLocData; }
			set
			{
				_selectedF1510ItemLocData = value;
				RaisePropertyChanged("SelectedF1510ItemLocData");
			}
		}


		#endregion

		private string _makeNo;
		public string MakeNo
		{
			get { return _makeNo; }
			set { Set(ref _makeNo, value); }
		}

		#endregion


		#region 資料繫結

		public void Bind(F1510Data f1510Data, string status)
		{
			_f1510Data = f1510Data;
			ItemCode = f1510Data.ITEM_CODE;
			ItemName = f1510Data.ITEM_NAME;
			ValidDate = f1510Data.VALID_DATE.Value.ToString("yyyy/MM/dd");
			MakeNo = f1510Data.MAKE_NO;
			_status = status;
			SearchCommand.Execute(null);

		}

		#endregion

		#region 儲位檢查

		public bool LocCodeCheck()
		{
			bool isCheckOk = true;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			var item = F1510ItemLocDatas.FirstOrDefault(o => o.TAR_LOC_CODE == LocCode);
			if (item != null)
				message.Message = Properties.Resources.P0203010100_LocExist;
			else
			{
				var proxyEx = GetExProxy<P02ExDataSource>();
        var result = proxyEx.CreateQuery<ExecuteResult>("CheckLocCode")
          .AddQueryExOption("locCode", LocCode.Replace("-", string.Empty))
          .AddQueryExOption("tarDcCode", _f1510Data.TAR_DC_CODE)
          .AddQueryExOption("tarWareHouseId", _f1510Data.TAR_WAREHOUSE_ID)
          .AddQueryExOption("itemCode", _f1510Data.ITEM_CODE)
          .AddQueryExOption("validDate", _f1510Data.VALID_DATE?.ToString() ?? null)
          .ToList();
				var rtnResult = result.First();
				if (!rtnResult.IsSuccessed)
					message.Message = rtnResult.Message;
			}
			if (message.Message.Length > 0)
			{
				ShowMessage(message);
				isCheckOk = false;
			}
			return isCheckOk;
		}

		#endregion

		#region 數量檢查

		public bool CheckLocQty()
		{
			bool isCheckOk = false;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			int qty = 0;
			if (string.IsNullOrEmpty(LocQty))
				message.Message = Properties.Resources.P0203010100_LocQtyIsNull;
			else if (!int.TryParse(LocQty, out qty))
				message.Message = Properties.Resources.P0203010100_LocQtyFormatError;
			else if (qty <= 0)
				message.Message = Properties.Resources.P0203010100_LocQtyZero;
			else
				isCheckOk = true;

			if (message.Message.Length > 0)
				ShowMessage(message);
			return isCheckOk;
		}

		#endregion

		#region 新增資料

		public void AddF1510Data()
		{
			IsChangedData = true;
			var proxy = GetProxy<F19Entities>();
			var locItem = proxy.F1912s.Where(o => o.DC_CODE == _f1510Data.DC_CODE && o.LOC_CODE == LocCode.Replace("-", "")).First();
			var wareHouseItem = proxy.F1980s.Where(o => o.WAREHOUSE_ID == locItem.WAREHOUSE_ID && o.DC_CODE == locItem.DC_CODE).First();
			var item = new F1510ItemLocData
			{
				ChangeStatus = "Insert",
				ALLOCATION_DATE = _f1510Data.ALLOCATION_DATE,
				ALLOCATION_NO = _f1510Data.ALLOCATION_NO,
				ITEM_CODE = _f1510Data.ITEM_CODE,
				ITEM_NAME = _f1510Data.ITEM_NAME,
				WAREHOUSE_ID = wareHouseItem.WAREHOUSE_ID,
				WAREHOUSE_NAME = wareHouseItem.WAREHOUSE_NAME,
				SUG_LOC_CODE = "",//此時無法知道他扣哪一個建議儲位 由後端決定
				TAR_LOC_CODE = LocCode,
				ORGINAL_QTY = 0,
				QTY = int.Parse(LocQty),
				DC_CODE = _f1510Data.DC_CODE,
				GUP_CODE = _f1510Data.GUP_CODE,
				CUST_CODE = _f1510Data.CUST_CODE,
				VALID_DATE = _f1510Data.VALID_DATE.Value
			};
			F1510ItemLocDatas.Add(item);
			F1510ItemLocDatas = F1510ItemLocDatas.OrderBy(o => o.TAR_LOC_CODE).ToList();

		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxyEx = GetExProxy<P02ExDataSource>();
			F1510ItemLocDatas = proxyEx.CreateQuery<F1510ItemLocData>("GetF1510ItemLocDatas")
					 .AddQueryExOption("tarDcCode", _f1510Data.TAR_DC_CODE)
					 .AddQueryExOption("gupCode", _f1510Data.GUP_CODE)
					 .AddQueryExOption("custCode", _f1510Data.CUST_CODE)
					 .AddQueryExOption("allocationNo", _f1510Data.ALLOCATION_NO)
					 .AddQueryExOption("status", _status)
					 .AddQueryExOption("itemCode", _f1510Data.ITEM_CODE)
					 .AddQueryExOption("validDate", _f1510Data.VALID_DATE)
					 .AddQueryExOption("srcLocCode", _f1510Data.SRC_LOC_CODE)
					 .AddQueryExOption("makeNo", _f1510Data.MAKE_NO)
					 .ToList();
		}
		private void DoSearchComplete()
		{
			if (F1510ItemLocDatas != null && F1510ItemLocDatas.Any())
			{
				SelectedF1510ItemLocData = F1510ItemLocDatas.First();
				TotalQty = F1510ItemLocDatas.Select(o => o.QTY).Sum();
			}
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return new RelayCommand(
					 DoCancel, () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//IsChangedData = F1510ItemLocDatas.Any(o => o.ORGINAL_QTY != o.QTY || o.ChangeStatus!="Normal");
			//執行取消動作
			ExitClick();
		}
		#endregion Cancel

		#region Save

		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode == OperateMode.Query && _f1510ItemLocDatas != null && _f1510ItemLocDatas.Select(o => o.QTY).Sum() == _totalQty,
					c => DoSaveComplete()
					);
			}
		}

		public bool IsSaveOk;
		public void DoSave()
		{
			//執行確認儲存動作
			IsSaveOk = true;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			if (F1510ItemLocDatas.Any(o => o.QTY < 0))
				message.Message = Properties.Resources.P0203010100_ItemLocQtyZero;
			else if (F1510ItemLocDatas.Select(o => o.QTY).Sum() != TotalQty)
				message.Message = Properties.Resources.P0203010100_ItemLocQtyNotEqualTotalQty;
			else
			{
				IsChangedData = F1510ItemLocDatas.Any(o => o.ORGINAL_QTY != o.QTY || o.ChangeStatus != "Normal");
				if (IsChangedData)
				{
					var data = ExDataMapper.MapCollection<F1510ItemLocData, wcf.F1510ItemLocData>(F1510ItemLocDatas).ToList();
                    data.ForEach(o => { o.TAR_LOC_CODE = o.TAR_LOC_CODE.Replace("-", ""); });
                    var wcfF1510Data = ExDataMapper.Map<F1510Data, wcf.F1510Data>(_f1510Data);
					var proxyWcf = new wcf.P02WcfServiceClient();
					var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.InsertOrUpdateF1510LocItemData(wcfF1510Data, data.ToArray()));
					if (result.IsSuccessed)
					{
						IsChangedData = false;
						ShowMessage(Messages.InfoUpdateSuccess);
					}
					else
					{
						IsSaveOk = false;
						var error = Messages.ErrorUpdateFailed;
						error.Message += Environment.NewLine + result.Message;
						ShowMessage(error);
					}
				}
				else
					ShowMessage(Messages.WarningNotModified);
			}
			if (message.Message.Length > 0)
			{
				IsSaveOk = false;
				ShowMessage(message);
			}
		}

		private void DoSaveComplete()
		{
			if (IsSaveOk)
				ClosedSuccessClick();
		}
		#endregion Save
	}
}
