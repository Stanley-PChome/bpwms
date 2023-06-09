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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using F19 = Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;

using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using Wms3pl.WpfClient.DataServices.F19DataService;
namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010100_ViewModel : InputViewModelBase
	{
		public Action actionAfterCreatePickTicket = delegate { };

		public P9101010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		#region Data - Base Data/ 物料清單/ 倉別類型/ 還有庫存的倉別/ 庫存數/ 揀料商品規格
		private F1903 _f1903Data;
		public F1903 F1903Data
		{
			get { return _f1903Data; }
			set { _f1903Data = value; RaisePropertyChanged("F1903Data"); }
		}
		private F910201 _baseData;
		public F910201 BaseData
		{
			get { return _baseData; }
			set
			{
				_baseData = value;
				if (_baseData != null && _baseData.STATUS == "0")
				{
					DoSearchBaseBomQty();
					DoSearchMaterial();
					DoSearchAvailableStocks();
					//DoRefreshProcessQty(); // 雖然在DoSearchMaterial時會算一次數量, 但那時的BaseBomQty還沒有值, 所以這裡再重抓一次
				}
				RaisePropertyChanged("BaseData");
			}
		}

		private List<ProcessItem> _materialList = new List<ProcessItem>();
		public List<ProcessItem> MaterialList
		{
			get { return _materialList; }
			set { _materialList = value; RaisePropertyChanged("MaterialList"); }
		}

		private ProcessItem _selectedMaterial;
		public ProcessItem SelectedMaterial
		{
			get { return _selectedMaterial; }
			set
			{
				_selectedMaterial = value;
				DoSearchItemDetail();
				DoSearchAvailableStocks(); // 改變商品後就去抓可用的庫存
				DoSearchWarehouseType();
				DoSearchWarehouse();
				DoRefreshProcessQty();
				RaisePropertyChanged("SelectedMaterial");
			}
		}

		private List<StockData> _availableStockList = new List<StockData>();
		public List<StockData> AvailableStockList
		{
			get { return _availableStockList; }
			set { _availableStockList = value; RaisePropertyChanged("AvailableStockList"); }
		}

		private List<NameValuePair<string>> _f198001List = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> F198001List
		{
			get { return _f198001List; }
			set { _f198001List = value; RaisePropertyChanged("F198001List"); }
		}
		private string _selectedF198001 = string.Empty;
		public string SelectedF198001
		{
			get { return _selectedF198001; }
			set { _selectedF198001 = value; DoSearchWarehouse(); RaisePropertyChanged("SelectedF198001"); }
		}

		private List<NameValuePair<string>> _f1980List = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> F1980List
		{
			get { return _f1980List; }
			set { _f1980List = value; RaisePropertyChanged("F1980List"); }
		}
		private string _selectedF1980 = string.Empty;
		public string SelectedF1980
		{
			get { return _selectedF1980; }
			set { _selectedF1980 = value; DoRefreshStockQty(); RaisePropertyChanged("SelectedF1980"); }
		}

		private decimal _stockQty = 0;
		public decimal StockQty
		{
			get { return _stockQty; }
			set { _stockQty = value; RaisePropertyChanged("StockQty"); }
		}

		private string _pickNo = string.Empty;
		public string PickNo
		{
			get { return _pickNo; }
			set { _pickNo = value; RaisePropertyChanged("PickNo"); }
		}
		#endregion
		#region Form - 加工料數/ 揀料數/ 揀料資料
		private decimal _processQty = 0;
		public decimal ProcessQty
		{
			get { return _processQty; }
			set { _processQty = value; RaisePropertyChanged("ProcessQty"); }
		}

		private decimal _aProcessQty = 0;
		public decimal AProcessQty
		{
			get { return _aProcessQty; }
			set { _aProcessQty = value; RaisePropertyChanged("AProcessQty"); }
		}

		private ObservableCollection<PickData> _bomData = new ObservableCollection<PickData>();
		public ObservableCollection<PickData> BomData
		{
			get { return _bomData; }
			set { _bomData = value; RaisePropertyChanged("BomData"); }
		}
		#endregion
		#region Data - 選到的揀料資料/ 每個物料所需的基本組合數
		private PickData _selectedBomData;
		public PickData SelectedBomData
		{
			get { return _selectedBomData; }
			set
			{
				_selectedBomData = value;
				RaisePropertyChanged("SelectedBomData");
				if (value != null) AProcessQty = value.A_PROCESS_QTY;
			}
		}

		private List<F910102> _baseBomQty = new List<F910102>();
		public List<F910102> BaseBomQty
		{
			get { return _baseBomQty; }
			set { _baseBomQty = value; RaisePropertyChanged("BaseBomQty"); }
		}
		#endregion
		#endregion

		#region Command
		#region 尋找物料清單
		public ICommand SearchMaterialCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchMaterial(),
					() => true
				);
			}
		}

		/// <summary>
		/// 取得物料清單
		/// </summary>
		private void DoSearchMaterial()
		{
			IsBusy = true;
			var proxyEx = GetExProxy<P91ExDataSource>();

			// 抓出物料清單
			MaterialList = proxyEx.CreateQuery<ProcessItem>("GetMaterialList")
					.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
					.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
					.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
					.AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO))
					.ToList();

			SelectedMaterial = MaterialList.FirstOrDefault();

			IsBusy = false;
		}
		#endregion

		#region 尋找揀料的倉別類型/ 倉別
		private void DoSearchAvailableStocks()
		{
			IsBusy = true;
			var proxy = GetExProxy<P91ExDataSource>();
			/* 原本是抓出該成品所有料件的庫存, 改為選擇料件後才去抓該料件的庫存
			var tmp = proxy.CreateQuery<StockData>("GetStockData")
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("processNo", string.Format("'{0}'", BaseData.PROCESS_NO))
				.ToList();
			*/
			var tmp = proxy.CreateQuery<StockData>("GetStockData2")
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", BaseData.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", BaseData.CUST_CODE))
				.AddQueryOption("itemCode", string.Format("'{0}'", SelectedMaterial.ITEM_CODE))
				.ToList();

			AvailableStockList = tmp;

			IsBusy = false;
		}

		/// <summary>
		/// 依照所需要的物料, 取得基本需要的組合數
		/// 這裡要依是否有ITEM_CODE_BOM來判斷要不要抓組合商品
		/// </summary>
		private void DoSearchBaseBomQty()
		{
			IsBusy = true;
			var proxy = GetProxy<F91Entities>();
			// 依ITEM_CODE/ ITEM_CODE_BOM來決定要顯示哪個清單 
			if (string.IsNullOrWhiteSpace(BaseData.ITEM_CODE_BOM))
			{
				BaseBomQty = new List<F910102>() { new F910102() { BOM_NO = "", BOM_QTY = 1, CUST_CODE = "", GUP_CODE = "", COMBIN_ORDER = 0, MATERIAL_CODE = BaseData.ITEM_CODE } };
			}
			else
			{

				var f910101 = proxy.F910101s.Where(a => a.GUP_CODE == BaseData.GUP_CODE && a.CUST_CODE == BaseData.CUST_CODE && a.BOM_NO == BaseData.ITEM_CODE_BOM).SingleOrDefault();
				if (f910101.BOM_TYPE == "0") //Bom為組合時才需取組合的物料
				{
					var tmp = proxy.F910102s.Where(x => x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE && x.BOM_NO == BaseData.ITEM_CODE_BOM).ToList();
					BaseBomQty = tmp;
				}
				else //Bom為拆解時流通加工單的物料即BaseData.ITEM_CODE
				{
					BaseBomQty = new List<F910102>() { new F910102() { BOM_NO = BaseData.ITEM_CODE_BOM, BOM_QTY = 1, CUST_CODE = BaseData.GUP_CODE, GUP_CODE = BaseData.CUST_CODE, COMBIN_ORDER = 0, MATERIAL_CODE = BaseData.ITEM_CODE } };
				}

			}
			string itemCode = (string.IsNullOrWhiteSpace(BaseData.ITEM_CODE_BOM) ? BaseData.ITEM_CODE : BaseData.ITEM_CODE_BOM);

			IsBusy = false;
		}

		/// <summary>
		/// 切換揀料商品時, 同時取得商品規格
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		private void DoSearchItemDetail()
		{
			//var proxy = GetProxy<F19Entities>();
			//var data = proxy.F1903s.Where(x => x.GUP_CODE == BaseData.GUP_CODE && x.ITEM_CODE == SelectedMaterial.ITEM_CODE).FirstOrDefault();
			//if (data == null)
			//    F1903Data = data;
			//else
			//    F1903Data = null;
		}

		/// <summary>
		/// 依照選擇的商品來顯示有庫存的倉別類型
		/// </summary>
		private void DoSearchWarehouseType()
		{
			if (SelectedMaterial == null) return;
			IsBusy = true;
			_selectedF198001 = string.Empty;
			F198001List = AvailableStockList.GroupBy(x => new { TYPE_NAME = x.TYPE_NAME, WAREHOUSE_TYPE = x.WAREHOUSE_TYPE }).Select(x => new NameValuePair<string>()
			{
				Name = x.Key.TYPE_NAME,
				Value = x.Key.WAREHOUSE_TYPE
			}).OrderBy(x => x.Value).ToList();
			if (F198001List.Any())
				SelectedF198001 = F198001List.FirstOrDefault().Value;
			IsBusy = false;
		}

		/// <summary>
		/// 依照選擇的倉別類型來顯示倉別
		/// </summary>
		private void DoSearchWarehouse()
		{
			IsBusy = true;
			_selectedF1980 = string.Empty;
			F1980List = AvailableStockList.Where(x => x.WAREHOUSE_TYPE == SelectedF198001).GroupBy(x => new { WAREHOUSE_NAME = x.WAREHOUSE_NAME, WAREHOUSE_ID = x.WAREHOUSE_ID }).Select(x => new NameValuePair<string>()
			{
				Name = x.Key.WAREHOUSE_NAME,
				Value = x.Key.WAREHOUSE_ID
			}).OrderBy(x => x.Value).ToList();

			if (F1980List.Any())
				SelectedF1980 = F1980List.FirstOrDefault().Value;
			IsBusy = false;
		}

		/// <summary>
		/// 依照選擇的倉別來顯示庫存
		/// 因為取出的資料為ITEM + LOC + WAREHOUSE, 而顯示時是依Warehouse來顯示庫存數, 所以要再做group by
		/// </summary>
		private void DoRefreshStockQty()
		{
			IsBusy = true;
			_stockQty = 0;
			// 原本有判斷MATERIAL CODE, 因已改為只抓該商品的庫存, 所以不用再判斷MATERIAL CODE
			var tmp = AvailableStockList.Where(x => x.WAREHOUSE_TYPE == SelectedF198001 && x.WAREHOUSE_ID == SelectedF1980).GroupBy(x => x.WAREHOUSE_ID).Select(x => new { WAREHOUSE_ID = x.Key, QTY = x.Sum(y => y.QTY) }).FirstOrDefault();
			if (tmp != null)
				StockQty = tmp.QTY ?? 0;
			AProcessQty = StockQty;
			IsBusy = false;
		}

		/// <summary>
		/// 選了物料後, 計算出該物料所需的總數
		/// </summary>
		private void DoRefreshProcessQty()
		{
			var tmp = BaseBomQty.Find(x => x.MATERIAL_CODE == SelectedMaterial.ITEM_CODE);
			ProcessQty = 0;
			if (tmp != null)
				ProcessQty = (tmp.BOM_QTY * BaseData.PROCESS_QTY);
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => SelectedMaterial != null && SelectedF1980 != null && SelectedF198001 != null && AProcessQty > 0
					);
			}
		}

		private bool DoCheck()
		{
			if (AProcessQty > StockQty)
			{
				DialogService.ShowMessage(Properties.Resources.P9101010100_ViewModel_PickQtyMustLessthanStockQty);
				return false;
			}
			return true;
		}
		private void DoAdd()
		{
			if (!DoCheck()) return;
			var tmp = BomData.ToList();
			// 檢查同一個倉別同一個料號是否已經存在, 如果是的話就做更新
			var tmp2 = tmp.Where(x => x.ITEM_CODE == SelectedMaterial.ITEM_CODE && x.WAREHOUSE_ID == SelectedF1980 && x.WAREHOUSE_TYPE == SelectedF198001).FirstOrDefault();
			if (tmp2 != null)
			{
				tmp2.A_PROCESS_QTY = Convert.ToInt32(AProcessQty);
				tmp2.QTY = Convert.ToInt32(StockQty);
			}
			else
			{
				//執行新增動作
				var newBomData = new PickData()
				{
					ITEM_CODE = SelectedMaterial.ITEM_CODE,
					ITEM_NAME = SelectedMaterial.ITEM_NAME,
					ITEM_COLOR = SelectedMaterial.ITEM_COLOR,
					ITEM_SIZE = SelectedMaterial.ITEM_SIZE,
					ITEM_SPEC = SelectedMaterial.ITEM_SPEC,
					A_PROCESS_QTY = Convert.ToInt32(AProcessQty),
					QTY = Convert.ToInt32(StockQty),
					TYPE_NAME = F198001List.Where(x => x.Value == SelectedF198001).FirstOrDefault().Name,
					WAREHOUSE_TYPE = SelectedF198001,
					WAREHOUSE_ID = SelectedF1980,
					WAREHOUSE_NAME = F1980List.Where(x => x.Value == SelectedF1980).FirstOrDefault().Name
				};
				tmp.Insert(0, newBomData);
			}
			BomData = tmp.ToObservableCollection();
			SelectedBomData = BomData.FirstOrDefault();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => SelectedBomData != null
					);
			}
		}

		private void DoEdit()
		{
			if (!DoCheck()) return;
			//執行編輯動作
			if (SelectedBomData != null) SelectedBomData.A_PROCESS_QTY = Convert.ToInt32(AProcessQty);
			var tmp = BomData.ToList();
			var data = tmp.Find(x => x.ITEM_CODE == SelectedBomData.ITEM_CODE && x.WAREHOUSE_ID == SelectedBomData.WAREHOUSE_ID && x.WAREHOUSE_TYPE == SelectedBomData.WAREHOUSE_TYPE);
			data.A_PROCESS_QTY = SelectedBomData.A_PROCESS_QTY;
			BomData = tmp.ToObservableCollection();
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel()
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => SelectedBomData != null && BomData.Any(),
					o => DoDeleteComplete()
				);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			var tmp = BomData.ToList();
			var tmp1 = tmp.Find(x => x.ITEM_CODE == SelectedBomData.ITEM_CODE && x.WAREHOUSE_TYPE == SelectedBomData.WAREHOUSE_TYPE && x.WAREHOUSE_ID == SelectedBomData.WAREHOUSE_ID);
			tmp.Remove(tmp1);
			BomData = tmp.ToObservableCollection();
		}
		private void DoDeleteComplete()
		{
			SelectedBomData = BomData.FirstOrDefault();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => BaseData != null && BaseData.STATUS == "0"
					);
			}
		}

		private void DoSave()
		{
			var f910201 = FindByKey<F910201>(BaseData);
			if (f910201.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.P9101010100_ViewModel_ProcessStatusCantPick);
				BaseData = f910201;
				return;
			}

			//檢查揀料數與流通加工單加工數是否符合
			if (!CheckPickItemProcessQty())
			{
				ShowWarningMessage(Properties.Resources.P9101010100_ViewModel_PickQty);
				return;
			}
			_isPrintPickTicket = false;
			//執行確認儲存動作
			#region 以下邏輯改到SERVER去做
			/*
			var data = new List<BomQtyData>();
			foreach (var p in BomData)
			{
				// 如果該料號/倉庫只有一個儲位+Valid Date
				if (tmpAvailableStockList.Where(x => x.ITEM_CODE == p.ITEM_CODE && x.WAREHOUSE_ID == p.WAREHOUSE_ID && x.WAREHOUSE_TYPE == p.WAREHOUSE_TYPE).GroupBy(x => new { x.LOC_CODE, x.VALID_DATE }).Count() == 1)
				{
					#region 該料號/倉庫只有一個儲位+Valid Date
					var tmp = tmpAvailableStockList.Where(x => x.ITEM_CODE == p.ITEM_CODE && x.WAREHOUSE_ID == p.WAREHOUSE_ID && x.WAREHOUSE_TYPE == p.WAREHOUSE_TYPE);
					int qty = p.QTY; // 存放需要的量
					foreach(var q in tmp) {
						int needQty = 0;
						if (p.QTY > q.QTY) {
							// 當需要的量大於可供給的量時, 計算剩下要的數量 (將需要的量扣掉可供給的量)
							// Memo: 原則上這裡不應該出現此種狀況. 或是有此狀況時應該要跳訊息.
							qty = (int)(qty - q.QTY);
							needQty = (int)q.QTY; // 將可供給的量記下來, 等下寫資料用
						}else {
							// 當可供給的量很夠時, 將需要的量記下來, 等下寫資料用
							needQty = p.QTY;
						}
						// 將該儲位加到要傳送的資料去
						data.Add(new BomQtyData()
						{
							LOC_CODE = q.LOC_CODE,
							VALID_DATE = q.VALID_DATE,
							DC_CODE = BaseData.DC_CODE,
							GUP_CODE = BaseData.GUP_CODE,
							CUST_CODE = BaseData.CUST_CODE,
							NEED_QTY = needQty,
							AVAILABLE_QTY = needQty,
							ITEM_CODE = p.ITEM_CODE,
							ITEM_CODE_BOM = p.ITEM_CODE,
							PROCESS_NO = ""
						});
					}
					#endregion
				}
				else
				{
					// 如果該料號/倉庫有一個以上的儲位+Valid Date
					// 此時要針對每一個料號來跑迴圈, 每次迴圈要計算此儲位可出貨數是否足夠
					int needQty = 0;
					needQty = p.A_PROCESS_QTY; // 因為會對每個倉庫設定不同需求量, 所以直接抓設定好的需求量
					foreach (var q in tmpAvailableStockList.Where(x => x.ITEM_CODE == p.ITEM_CODE && x.WAREHOUSE_ID == p.WAREHOUSE_ID && x.WAREHOUSE_TYPE == p.WAREHOUSE_TYPE).OrderByDescending(x => x.QTY))
					{
						int availableQty;
						if (needQty > q.QTY)
						{
							// 如果供給小於需求, 此時用完供給後, 要更新needQty, 然後繼續下一個可用的儲位
							// 當下一個儲位供給量夠時, 就會跑到else裡
							availableQty = (int)q.QTY;
						}
						else
						{
							// 如果供給大於需求, 直接新增一筆, 然後跳到下一個料號
							availableQty = needQty;
						}
						needQty = needQty - availableQty;

						data.Add(new BomQtyData()
						{
							LOC_CODE = q.LOC_CODE,
							VALID_DATE = q.VALID_DATE,
							DC_CODE = BaseData.DC_CODE,
							GUP_CODE = BaseData.GUP_CODE,
							CUST_CODE = BaseData.CUST_CODE,
							NEED_QTY = availableQty,
							AVAILABLE_QTY = availableQty,
							ITEM_CODE = p.ITEM_CODE,
							ITEM_CODE_BOM = p.ITEM_CODE,
							PROCESS_NO = ""
						});

						// 當需求都滿足了, 就跳到下一個料號
						if (needQty <= 0) break;
					}
				}
			}
			*/
			#endregion

			// 呼叫WCF寫入資料
			var proxyWcf = new wcf.P91WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.CreateP910205(BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.PROCESS_NO, BomData.Select(AutoMapper.Mapper.DynamicMap<wcf.PickData>).ToArray()));
			if (result == null || result.IsSuccessed == false)
			{
				DialogService.ShowMessage(Messages.Failed.Message + Environment.NewLine + result.Message);

				//ShowMessage(Messages.Failed);
				return;
			}
			if (!string.IsNullOrEmpty(result.Message))
			{
				PickNo = result.Message;
				DispatcherAction(() => actionAfterCreatePickTicket());
			}
		}

		private bool _isPrintPickTicket = false;
		private void DoPickCommandComplete()
		{
			actionAfterCreatePickTicket();
		}

		private bool CheckPickItemProcessQty()
		{
			foreach (var baseBom in BaseBomQty)
			{
				var pickProcessQty = BomData.Where(a => a.ITEM_CODE == baseBom.MATERIAL_CODE).Sum(a => a.A_PROCESS_QTY);
				var processQty = baseBom.BOM_QTY * BaseData.PROCESS_QTY;
				if (pickProcessQty != processQty)
					return false;
			}
			return true;
		}
		#endregion Save

		#endregion

		//public class PickData
		//{
		//	public string ITEM_CODE { get; set; }
		//	public string ITEM_NAME { get; set; }
		//	public string ITEM_SIZE { get; set; }
		//	public string ITEM_COLOR { get; set; }
		//	public string ITEM_SPEC { get; set; }
		//	public string WAREHOUSE_TYPE { get; set; }
		//	public string TYPE_NAME { get; set; }
		//	public string WAREHOUSE_ID { get; set; }
		//	public string WAREHOUSE_NAME { get; set; }
		//	public int QTY { get; set; }
		//	public int A_PROCESS_QTY { get; set; }
		//}
	}
}
