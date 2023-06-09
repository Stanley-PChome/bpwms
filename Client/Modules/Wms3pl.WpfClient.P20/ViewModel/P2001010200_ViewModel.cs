using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F20DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P20ExDataService;
using ex19 = Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P20ExDataService.ExecuteResult;
namespace Wms3pl.WpfClient.P20.ViewModel
{
	public partial class P2001010200_ViewModel : InputViewModelBase
	{
		public P2001010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

				UserOperateMode = OperateMode.Add;
				F1913Data = new F1913Data();
				F1913Data.WORK_TYPE = "0";
				F1913Data.BUNDLE_SERIALNO = "0";
				F1913Data.ISADD = true;
				F1913Data.GUP_CODE = GupCode;
				F1913Data.CUST_CODE = CustCode;
				F1913Data.ENTER_DATE = DateTime.Now.Date;
				F1913Data.VALID_DATE = DateTime.Parse("9999/12/31");
				F1913Data.VNR_CODE = "000000";
				F1913Data.BOX_CTRL_NO = "0";
				F1913Data.PALLET_CTRL_NO = "0";
				SetCauseList();
			}

		}

		#region Property

		public Action CancelClick = delegate { };
		public Action SaveClick = delegate { };
		public Action DgScrollIntoView = delegate { };
		public Action<string> SetErrorFocus = delegate { };
		public Action GetItemData = delegate { };
		public Action ClearItemData = delegate { };

		private F1913Data _f1913Data;
		public F1913Data F1913Data
		{
			get { return _f1913Data; }
			set
			{
				//_f1913Data = value;
				//RaisePropertyChanged("F1913Data");

				if (_f1913Data != null)
					_f1913Data.PropertyChanged -= F1913Data_PropertyChanged;
				Set(() => F1913Data, ref _f1913Data, value);
				if (_f1913Data != null)
					_f1913Data.PropertyChanged += F1913Data_PropertyChanged;
			}
		}
		private List<exShare.SerialNoResult> _serialNoResults;
		public List<exShare.SerialNoResult> SerialNoResults
		{
			get { return _serialNoResults; }
			set
			{
				_serialNoResults = value;
				RaisePropertyChanged("SerialNoResults");
			}
		}

		/// <summary>
		/// 外部所有暫存的序號
		/// </summary>
		public IEnumerable<exShare.SerialNoResult> OtherSerialNoResults { get; set; }

		#region 業主

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		#endregion

		#region 貨主

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		#endregion

		#region 物流中心

		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				_dcCode = value;
				RaisePropertyChanged("DcCode");
				SetWareHouse();
				F1913Data.DC_CODE = value;
			}
		}

		private string _dcName;

		public string DcName
		{
			get { return _dcName; }
			set
			{
				_dcName = value;
				RaisePropertyChanged("DcName");
			}
		}

		#endregion

		#region 倉別

		private List<NameValuePair<string>> _wareHouseList;

		public List<NameValuePair<string>> WareHouseList
		{
			get { return _wareHouseList; }
			set
			{
				_wareHouseList = value;
				RaisePropertyChanged("WareHouseList");
			}
		}

		#endregion

		#region 調整數量

		private int _adjustQty;

		public int AdjustQty
		{
			get { return _adjustQty; }
			set
			{
				_adjustQty = value;
				RaisePropertyChanged("AdjustQty");
				MustScanCount = F1913Data.BUNDLE_SERIALNO == "1" ? AdjustQty : 0;
			}
		}

        #endregion


        #region 異動原因List

        private List<NameValuePair<string>> _causeList;
		public List<NameValuePair<string>> CauseList
		{
			get { return _causeList; }
			set
			{
				_causeList = value;
				RaisePropertyChanged("CauseList");
			}
		}

		#endregion

		#region 刷讀序號

		private string _scanSerialNo;
		public string ScanSerialNo
		{
			get { return _scanSerialNo; }
			set
			{
				_scanSerialNo = value;
				RaisePropertyChanged("ScanSerialNo");
			}
		}
		#endregion

		#region 應刷數

		private int _mustScanCount;
		public int MustScanCount
		{
			get { return _mustScanCount; }
			set
			{
				_mustScanCount = value;
				RaisePropertyChanged("MustScanCount");
			}
		}

		#endregion

		#region 已刷數

		private int _scanCount;
		public int ScanCount
		{
			get { return _scanCount; }
			set
			{
				_scanCount = value;
				RaisePropertyChanged("ScanCount");
			}
		}

		#endregion

		#region 實收數

		private int _actualCount;
		public int ActualCount
		{
			get { return _actualCount; }
			set
			{
				_actualCount = value;
				RaisePropertyChanged("ActualCount");
			}
		}

		#endregion

		#region 錯誤數

		private int _falureCount;
		public int FalureCount
		{
			get { return _falureCount; }
			set
			{
				_falureCount = value;
				RaisePropertyChanged("FalureCount");
			}
		}

		#endregion

		#region grid

		private List<exShare.SerialNoResult> _dgList;

		public List<exShare.SerialNoResult> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private exShare.SerialNoResult _selectedData;
		public exShare.SerialNoResult SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
			}
		}
		#endregion

		#region 是否找到商品

		private F1913Data _tempF1913Data = null;

		private bool _hasFindSearchItem;
		public bool HasFindSearchItem
		{
			get { return _hasFindSearchItem; }
			set
			{
				Set(() => HasFindSearchItem, ref _hasFindSearchItem, value);

				if (F1913Data.ITEM_CODE == null)
					return;

				if (value)
				{
					// 若上一次是刷序號綁儲位商品，則提示 "更換商品將會清空序號，是否確認更換?"
					if (_tempF1913Data != null && _tempF1913Data.ITEM_CODE != F1913Data.ITEM_CODE
						&& DgList != null && DgList.Any())
					{
						var message = new MessagesStruct
						{
							Button = DialogButton.YesNo,
							Image = DialogImage.Question,
							Message = Properties.Resources.P2001010200_ViewModel_SureClearSerialNo_To_ChangeItem,
							Title = Resources.Resources.Information
						};
						if (ShowMessage(message) == DialogResponse.No)
						{
							// 先初始化商品搜尋控制項
							F1913Data.ITEM_CODE = null;
							// 還原資料
							F1913Data.ITEM_CODE = _tempF1913Data.ITEM_CODE;
							F1913Data.ITEM_NAME = _tempF1913Data.ITEM_NAME;
							F1913Data.ITEM_SPEC = _tempF1913Data.ITEM_SPEC;
							F1913Data.ITEM_SIZE = _tempF1913Data.ITEM_SIZE;
							F1913Data.ITEM_COLOR = _tempF1913Data.ITEM_COLOR;
							return;
						}
					}

					// 取得此商品是否為序號商品
					var proxy = GetProxy<F19Entities>();
					var item =
						proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.CUST_CODE == CustCode && o.ITEM_CODE == F1913Data.ITEM_CODE)
							.ToList()
							.FirstOrDefault();
					F1913Data.BUNDLE_SERIALNO = item != null ? item.BUNDLE_SERIALNO : "0";
					if (string.IsNullOrEmpty(F1913Data.BUNDLE_SERIALNO))
						F1913Data.BUNDLE_SERIALNO = "0";
					F1913Data.BUNDLE_SERIALLOC = item != null ? item.BUNDLE_SERIALLOC : "0";
					if (string.IsNullOrEmpty(F1913Data.BUNDLE_SERIALLOC))
						F1913Data.BUNDLE_SERIALLOC = "0";
					MustScanCount = F1913Data.BUNDLE_SERIALNO == "1" ? AdjustQty : 0;

					// 紀錄最後一次尋找的商品資料，只用於判斷更換品號，是否清除序號用
					_tempF1913Data = ExDataMapper.Clone(F1913Data);
				}
				else
				{
					_tempF1913Data = null;

					F1913Data.BUNDLE_SERIALNO = "0";
					F1913Data.BUNDLE_SERIALLOC = "0";
				}

				DgList = null;
				SelectedData = null;
				BindScanCount();

			}
		}

		#endregion
		#endregion


		private void BindScanCount()
		{
			if (DgList != null)
			{
				ScanCount = DgList.Count;
				ActualCount = DgList.Count(o => o.Checked);
				FalureCount = DgList.Count(o => !o.Checked);
			}
			else
			{
				ScanCount = 0;
				ActualCount = 0;
				FalureCount = 0;
			}
		}
		#region 下拉選單資料繫結

		private void SetWareHouse()
		{
			var proxy = GetProxy<F19Entities>();
			//var data = proxy.F1980s.Where(o => o.DC_CODE == DcCode).ToList();
			//WareHouseList = (from o in data
			//				 select new NameValuePair<string>
			//				 {
			//					 Name = o.WAREHOUSE_NAME,
			//					 Value = o.WAREHOUSE_ID
			//				 }).ToList();
			var proxyP19Ex = GetExProxy<ex19.P19ExDataSource>();
			WareHouseList = proxyP19Ex.CreateQuery<ex19.F1912WareHouseData>("GetCustWarehouseDatas")
										.AddQueryExOption("dcCode", DcCode)
										.AddQueryExOption("gupCode", GupCode)
										.AddQueryExOption("custCode", CustCode)
										.Where(x=>x.DEVICE_TYPE == "0")   // 只查詢出人工倉
										.Select(o => new NameValuePair<string>()
										{
											Name = o.WAREHOUSE_NAME,
											Value = o.WAREHOUSE_ID
										}).ToList();


			F1913Data.WAREHOUSE_ID = WareHouseList.Any() ? WareHouseList.First().Value : null;
		}
		private void SetCauseList()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1951s.Where(o => o.UCT_ID == "AI").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.CAUSE,
							Value = o.UCC_CODE
						}).ToList();
			CauseList = list;
			if (CauseList.Any())
				F1913Data.CAUSE = CauseList.First().Value;
		}
		#endregion

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return new RelayCommand(
					DoCancel, () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			CancelClick();
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(
					 DoSave, () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			GetItemData();
			if (!HasFindSearchItem)
			{
				ShowWarningMessage(Properties.Resources.P2001010200_ViewModel_SerialNoNotFound);
				return;
			}

			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Resources.Resources.Information
			};

			GetVnrName();

			var proxy = GetProxy<F19Entities>();

            F1913Data.LOC_CODE = LocCodeHelper.LocCodeConverter9(F1913Data.LOC_CODE);

            var item = proxy.F1912s.Where(o => o.DC_CODE == F1913Data.DC_CODE && o.LOC_CODE == F1913Data.LOC_CODE && o.WAREHOUSE_ID == F1913Data.WAREHOUSE_ID).ToList().FirstOrDefault();
			
			var msgResult =new DialogResponse();

			if (item == null)
			{
				message.Message = Properties.Resources.P2001010200_ViewModel_ItemInDCAndWarehouseIDNotFound;
				ShowMessage(message);
				SetErrorFocus("1");
			}
			else
			{
				#region 檢查儲位貨主是否相同
				string typeId = item.WAREHOUSE_ID.Substring(0, 1);
				var warehouseType = proxy.F198001s.Where(o => o.TYPE_ID == typeId).FirstOrDefault();
				//只允許儲位貨主為共用(0)或儲位貨主等於環境貨主使用
				if (item.CUST_CODE != "0" && item.CUST_CODE != CustCode)
				{
					message.Message = Properties.Resources.P2001010200_ViewModel_LocCodeCannotInsert;
					ShowMessage(message);
					return;
				}
				//儲位貨主=共用(0) 或 儲位貨主!=環境貨主 須檢核
				//儲位貨主=環境貨主 就不須檢核
				if (item.CUST_CODE == "0" || item.CUST_CODE != CustCode)
				{
					//該倉別允許此儲位可混不同貨主放商品
					if (warehouseType.LOC_MUSTSAME_NOWCUSTCODE == "0")
					{
						//NOW_CUST_CODE=0(無貨主使用) 或 NOW_CUST_CODE = 環境貨主 可新增 
						//NOW_CUST_CODE!=0(有貨主使用) 且 NOW_CUST_CODE !=環境貨主 要顯示詢問訊息
						if (item.NOW_CUST_CODE != "0"  && item.NOW_CUST_CODE != CustCode)
						{
							msgResult = ShowConfirmMessage(Properties.Resources.P2001010200_ViewModel_SelectedLocCode_DifferenceTo_ItemCustCode_IsSureToAdjust);
							if (msgResult != DialogResponse.Yes)
								return;
						}
					}
					//該倉別不允許此儲位可混不同貨主放商品
					else
					{
						//NOW_CUST_CODE=0(無貨主使用) 或 NOW_CUST_CODE = 環境貨主 可新增 
						//NOW_CUST_CODE!=0(有貨主使用) 且 NOW_CUST_CODE !=環境貨主 要顯示錯誤訊息
						if (item.NOW_CUST_CODE != "0" && item.NOW_CUST_CODE!= CustCode)
						{
							message.Message = Properties.Resources.P2001010200_ViewModel_LocCodeCannotInsert;
							ShowMessage(message);
							return;
						}

					}
				}
				
				#endregion

				F1913Data.DC_NAME = DcName;
				if (WareHouseList.Any())
					F1913Data.WAREHOUSE_NAME = WareHouseList.Find(o => o.Value == F1913Data.WAREHOUSE_ID).Name;
				if (CauseList.Any())
					F1913Data.CAUSENAME = CauseList.Find(o => o.Value == F1913Data.CAUSE).Name;
				if (F1913Data.CAUSE != "999")
					F1913Data.CAUSE_MEMO = "";

				F1913Data.ADJ_QTY_IN = AdjustQty;
				if (AdjustQty <= 0)
				{
					message.Message = Properties.Resources.P2001010200_ViewModel_AdjustQtyGreaterThanZero;
					ShowMessage(message);
				}
				else if (F1913Data.BUNDLE_SERIALNO == "1")
				{
					var list = DgList;
					if (list != null)
					{
						if (MustScanCount < list.Count(x => x.Checked))
						{
							message.Message = Properties.Resources.P2001010100_ViewModel_Checked_Over_MustScan;
							ShowMessage(message);
						}
					}
				}
				//儲位商品溫層檢查
				var proxyEx = GetExProxy<P20ExDataSource>();
				var result = proxyEx.CreateQuery<ExecuteResult>("CheckItemLocTmpr")
					.AddQueryExOption("dcCode", DcCode)
					.AddQueryExOption("gupCode", GupCode)
                    .AddQueryExOption("custCode",CustCode)
					.AddQueryExOption("itemCode", F1913Data.ITEM_CODE)
					.AddQueryExOption("locCode", LocCodeHelper.LocCodeConverter9(F1913Data.LOC_CODE)).ToList().First();

                //檢查批號是否為空，若為空值則跳提示(原本為若是批號為空，則填入0)
                //F1913Data.MAKE_NO = string.IsNullOrWhiteSpace(F1913Data.MAKE_NO) ? "0" : F1913Data.MAKE_NO;
                if(string.IsNullOrWhiteSpace(F1913Data.MAKE_NO))
                {
                    ShowWarningMessage(Properties.Resources.P2001010200_MakeNo_Required);
                    return;
                }
                

                if (!result.IsSuccessed)
				{
					message.Message = result.Message;
					ShowMessage(message);
				}

				if (message.Message.Length == 0)
				{
					SaveClick();
				}
			}
		}
		#endregion Save

		#region 刷讀序號

		public void AddSerialNo()
		{
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Title = Resources.Resources.Information
			};
			if (ScanSerialNo != null)
				ScanSerialNo = ScanSerialNo.Trim();
			if (string.IsNullOrEmpty(ScanSerialNo))
			{
				message.Message = Properties.Resources.P2001010100_ViewModel_SerailNo_Required;
				ShowMessage(message);
			}
			else
			{
				var focusSerialNo = ScanSerialNo;
				var list = DgList ?? new List<exShare.SerialNoResult>();
				if (list.Any(o => o.SerialNo == ScanSerialNo) || OtherSerialNoResults.Any(o => o.SerialNo == ScanSerialNo))
				{
					message.Message = Properties.Resources.P2001010100_ViewModel_SerailNo_Exist;
					ShowMessage(message);
				}
				else
				{
					var serialNoResults = CheckSerialNoIsPass(ScanSerialNo);
					if (!serialNoResults.First().Checked)
						list.Add(serialNoResults.First());
					else
					{
						list.AddRange(serialNoResults.Where(o => !list.Select(c => c.SerialNo).Contains(o.SerialNo)));
						focusSerialNo = list.Last().SerialNo;
					}

					DgList = list.ToList();
					BindScanCount();
				}
				if (DgList != null && DgList.Any())
					SelectedData = DgList.Find(o => o.SerialNo == focusSerialNo);
				ScanSerialNo = "";
				DgScrollIntoView();

			}
		}

		#endregion

		#region 清除序號

		public void DeleteSerialNo(exShare.SerialNoResult serialNoResult)
		{
			var list = DgList;
			if (list != null && list.Any())
			{
				var items = (!string.IsNullOrEmpty(serialNoResult.BatchNo)) ? list.Where(o => o.BatchNo == serialNoResult.BatchNo).ToList() : null;
				items = items ?? ((!string.IsNullOrEmpty(serialNoResult.BoxSerail)) ? list.Where(o => o.BoxSerail == serialNoResult.BoxSerail).ToList() : null);
				items = items ?? ((!string.IsNullOrEmpty(serialNoResult.CaseNo)) ? list.Where(o => o.BoxSerail == serialNoResult.CaseNo).ToList() : null);
				if (items == null)
					list.Remove(serialNoResult);
				else
				{
					for (int i = items.Count - 1; i >= 0; i--)
						list.Remove(items[i]);
				}
				DgList = list.ToList();
			}
			BindScanCount();
			if (DgList.Any())
				SelectedData = DgList.First();
			DgScrollIntoView();
		}

		#endregion

		#region 取得廠商名稱

		public void GetVnrName()
		{
			var proxy = GetProxy<F19Entities>();
			var item =
				proxy.F1908s.Where(o => o.GUP_CODE == GupCode && o.VNR_CODE == F1913Data.VNR_CODE)
					.ToList()
					.FirstOrDefault();
			F1913Data.VNR_NAME = (item != null) ? item.VNR_NAME : "";

		}

		#endregion

		#region 檢查序號

		private List<exShare.SerialNoResult> CheckSerialNoIsPass(string serialNo)
		{
			var proxyEx = GetExProxy<ShareExDataSource>();
			var serialNoResultList = proxyEx.CreateQuery<exShare.SerialNoResult>("CheckSerialNoFull")
				.AddQueryExOption("dcCode", F1913Data.DC_CODE)
				.AddQueryExOption("gupCode", F1913Data.GUP_CODE)
				.AddQueryExOption("custCode", F1913Data.CUST_CODE)
				.AddQueryExOption("itemCode", F1913Data.ITEM_CODE)
				.AddQueryExOption("serialNo", serialNo)
				.AddQueryExOption("status", "A1").ToList();
			return serialNoResultList;

		}
		#endregion

		private void F1913Data_PropertyChanged(object snder,System.ComponentModel.PropertyChangedEventArgs e)
		{
			_f1913Data.PropertyChanged -= F1913Data_PropertyChanged;
			switch (e.PropertyName)
			{
				case nameof(F1913Data.WAREHOUSE_ID):
					if (F1913Data.WAREHOUSE_ID != null)
					{
						ClearItemData();
					}
					break;
			}
			_f1913Data.PropertyChanged += F1913Data_PropertyChanged;
		}
	}
}
