using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using sharewcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;


namespace Wms3pl.WpfClient.P02.ViewModel
{
    public partial class P0202060100_ViewModel : InputViewModelBase
    {
        public List<ItemBindContainerData> ItemBindContainerDataResult;

        //有多個地方會用到這幾個資料去查詢，從前端傳進來後把他保存起來
        string dcCode, gupCode, custCode, RTNo, RTSEQ;

        public ItemBindContainerData SendToP0202060300Data;
        public Action DoShowP0202060300 = delegate { };
        public Action<bool> DoExitWin = delegate { };
        public P0202060100_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                InitControls();
            }

        }

        private void InitControls()
        {
        }

        /// <summary>
        /// 讀取要分配的內容
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="RTNo"></param>
        /// <param name="RTSEQ"></param>
        /// <returns>執行結果 false=找不到資料</returns>
        public bool SetInitValue(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
        {
            //後面還有其他地方會用到這些資料去查詢，把他保存起來
            this.dcCode = dcCode;
            this.gupCode = gupCode;
            this.custCode = custCode;
            this.RTNo = RTNo;
            this.RTSEQ = RTSEQ;

            var exproxy = GetExProxy<P02ExDataSource>();
            ItemBindContainerDataResult = exproxy.CreateQuery<ItemBindContainerData>("GetItemBindContainerData")
                         .AddQueryExOption("dcCode", dcCode)
                         .AddQueryExOption("gupCode", gupCode)
                         .AddQueryExOption("custCode", custCode)
                         .AddQueryExOption("RTNo", RTNo)
                         .AddQueryExOption("RTSEQ", RTSEQ).ToList();

            if (!ItemBindContainerDataResult.Any())
                return false;
            ItemCode = ItemBindContainerDataResult.First().ITEM_CODE;
            ItemName = ItemBindContainerDataResult.First().ITEM_NAME;
            //只判斷揀區or補區的是否複驗來顯示畫面，正常來說這兩個區的這欄位應該要相同
            var OKItem = ItemBindContainerDataResult.Where(x => new string[] { "A", "C" }.Contains(x.TYPE_CODE)).FirstOrDefault();
            if (OKItem != null)
              DoubleCheckMsg = OKItem.NEED_DOUBLE_CHECK == 0 ? "不需要複驗" : "需要複驗";
            else
              //如果本次驗收商品全部都是不良品，那F0205中的TYPE_CODE只會有R，固定不用複驗，就直接給值
              DoubleCheckMsg = "不需要複驗";
            B_QTY = ItemBindContainerDataResult.Sum(x => x.B_QTY);
            A_QTY = ItemBindContainerDataResult.Sum(x => x.A_QTY ?? 0);

            //篩選出揀區資料
            var PickupData = ItemBindContainerDataResult.Where(x => x.TYPE_CODE.ToUpper() == "A").ToList();
            PickupTarWarehouseID = PickupData.FirstOrDefault()?.PICK_WARE_Name;
            PickupQty = PickupData.Sum(x => x.B_QTY - x.A_QTY) ?? 0;

            //篩選出補區資料
            var ReplenishData = ItemBindContainerDataResult.Where(x => x.TYPE_CODE.ToUpper() == "C").ToList();
            ReplenishTarWarehouseID = ReplenishData.FirstOrDefault()?.PICK_WARE_Name;
            ReplenishQTY = ReplenishData.Sum(x => x.B_QTY - x.A_QTY) ?? 0;

            //篩選出不良品區資料
            var NGData = ItemBindContainerDataResult.Where(x => x.TYPE_CODE.ToUpper() == "R").ToList();
            NGTarWarehouseID = NGData.FirstOrDefault()?.PICK_WARE_Name;
            NGQty = NGData.Sum(x => x.B_QTY - x.A_QTY) ?? 0;

            BindContainerList = exproxy.CreateQuery<BindContainerData>("GetBindContainerData")
                                 .AddQueryExOption("dcCode", dcCode)
                                 .AddQueryExOption("gupCode", gupCode)
                                 .AddQueryExOption("custCode", custCode)
                                 .AddQueryExOption("RTNo", RTNo)
                                 .AddQueryExOption("RTSEQ", RTSEQ)
                                .ToObservableCollection();

            return true;
        }

        #region 前端畫面屬性
        #region 品號
        private string _ItemCode;
        /// <summary>
        /// 品號
        /// </summary>
        public string ItemCode
        {
            get { return _ItemCode; }
            set { _ItemCode = value; RaisePropertyChanged(); }
        }
        #endregion 品號

        #region 是否需要複驗訊息
        private string _DoubleCheckMsg;
        /// <summary>
        /// 是否需要複驗訊息
        /// </summary>
        public string DoubleCheckMsg
        {
            get { return _DoubleCheckMsg; }
            set { _DoubleCheckMsg = value; RaisePropertyChanged(); }
        }
        #endregion 是否需要複驗訊息

        #region 總驗收數
        private int _B_QTY;
        /// <summary>
        /// 總驗收數
        /// </summary>
        public int B_QTY
        {
            get { return _B_QTY; }
            set { _B_QTY = value; RaisePropertyChanged(); }
        }
        #endregion 總驗收數

        #region 品名
        private string _ItemName;
        /// <summary>
        /// 品名
        /// </summary>
        public string ItemName
        {
            get { return _ItemName; }
            set { _ItemName = value; RaisePropertyChanged(); }
        }
        #endregion 品名

        #region 已分播數
        private int _A_QTY;
        /// <summary>
        /// 已分播數
        /// </summary>
        public int A_QTY
        {
            get { return _A_QTY; }
            set { _A_QTY = value; RaisePropertyChanged(); }
        }
        #endregion 已分播數

        #region 揀區上架倉別
        private string _PickupTarWarehouseID;
        /// <summary>
        /// 揀區上架倉別
        /// </summary>
        public string PickupTarWarehouseID
        {
            get { return _PickupTarWarehouseID; }
            set { _PickupTarWarehouseID = value; RaisePropertyChanged(); }
        }
        #endregion 上架倉別

        #region 揀區上架倉待分配數量
        private int _PickupQty;
        /// <summary>
        /// 揀區上架倉待分配數量
        /// </summary>
        public int PickupQty
        {
            get { return _PickupQty; }
            set { _PickupQty = value; RaisePropertyChanged(); }
        }
        #endregion 揀區上架倉待分配數量

        #region 補區上架倉別
        private string _ReplenTarWarehouseID;
        /// <summary>
        /// 補區上架倉別
        /// </summary>
        public string ReplenishTarWarehouseID
        {
            get { return _ReplenTarWarehouseID; }
            set { _ReplenTarWarehouseID = value; RaisePropertyChanged(); }
        }
        #endregion 補區上架倉別

        #region 補區上架倉待分配數量
        private int _ReplenQTY;
        /// <summary>
        /// 補區上架倉待分配數量
        /// </summary>
        public int ReplenishQTY
        {
            get { return _ReplenQTY; }
            set { _ReplenQTY = value; RaisePropertyChanged(); }
        }
        #endregion 補區上架倉待分配數量

        #region 不良品區上架倉別
        private string _NGTarWarehouseID;
        /// <summary>
        /// 不良品區上架倉別
        /// </summary>
        public string NGTarWarehouseID
        {
            get { return _NGTarWarehouseID; }
            set { _NGTarWarehouseID = value; RaisePropertyChanged(); }
        }
        #endregion 不良品區上架倉別

        #region 不良品區上架倉待分配數量
        private int _NGQty;
        /// <summary>
        /// 不良品區上架倉待分配數量
        /// </summary>
        public int NGQty
        {
            get { return _NGQty; }
            set { _NGQty = value; RaisePropertyChanged(); }
        }
        #endregion 不良品區上架倉待分配數量

        #region 綁定容器清單
        private ObservableCollection<BindContainerData> _BindContainerList;
        /// <summary>
        /// 綁定容器清單
        /// </summary>
        public ObservableCollection<BindContainerData> BindContainerList
        {
            get { return _BindContainerList; }
            set { _BindContainerList = value; RaisePropertyChanged(); }
        }
        #endregion 綁定容器清單

        #region 綁定容器清單所選項目
        private BindContainerData _SelectedBindContainer;
        /// <summary>
        /// 綁定容器清單所選項目
        /// </summary>
        public BindContainerData SelectedBindContainer
        {
            get { return _SelectedBindContainer; }
            set { _SelectedBindContainer = value; RaisePropertyChanged(); }
        }
        #endregion 綁定容器清單所選項目

        #region 全選按鈕
        private Boolean _IsCheckAll;
        public Boolean IsCheckAll
        {
            get { return _IsCheckAll; }
            set
            {
                _IsCheckAll = value;
                RaisePropertyChanged();
                foreach (var item in BindContainerList)
                    item.IsSelected = value;
                RaisePropertyChanged("BindContainerList");
            }
        }
        #endregion 全選按鈕

        #endregion 前端畫面屬性

        #region ICommand

        #region 關箱
        public ICommand CloseContainerCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { DoCloseContainer(); },
                    () => BindContainerList?.Count(x => x.IsSelected) > 0
                    );
            }
        }

        private void DoCloseContainer()
        {
            var shareProxy = GetWcfProxy<sharewcf.SharedWcfServiceClient>();
            var proxy = GetWcfProxy<wcf.P02WcfServiceClient>();

            var MessageList = new List<String>();

            var ExProxy = GetExProxy<P02ExDataSource>();
            var CheckAllContainerIsDone = proxy.RunWcfMethod(w => w.CheckAllContainerIsDone(dcCode, gupCode, custCode, RTNo, RTSEQ));
            //如果有一筆未完成，顯示訊息”必須先完成此驗收單容器綁定後再進行關箱”
            if (!CheckAllContainerIsDone)
            {
                ShowWarningMessage("必須先完成此驗收單容器綁定後再進行關箱");
                return;
            }
            //如果都完成，確認視窗，關箱後將不可已在調整此驗收單所有容器綁定數量，您是否確認要關箱?”
            if (ShowConfirmMessage("關箱後將不可以在調整此驗收單所有容器綁定數量，您是否確認要關箱?") != UILib.Services.DialogResponse.Yes)
                return;

            //(3)[AA]=取得勾選的容器F020501.ID
            //Foreach [BB] IN [AA]
            foreach (var item in BindContainerList.Where(x => x.IsSelected))
            {
                //(2-1)[CC]=呼叫[6.容器關箱共用服務,[BB]]
                var result = shareProxy.RunWcfMethod(w => w.ContainerCloseBoxWithRT(item.F020501_ID, item.RT_NO, item.RT_SEQ));
                if (!result.IsSuccessed)
                {
                    //(2-2)若[CC].IsSuccessed=false，加入訊息清單([容器:XXX]+[CC].Message)
                    MessageList.Add($"[容器:{item.CONTAINER_CODE}]{result.Message}");
                }
                else
                {
                    //(2-3)若[CC].IsSuccessed=true, 加入訊息清單([容器:XXX]+”關箱完成”)
                    MessageList.Add(String.Format("[容器:{0}]關箱完成", item.CONTAINER_CODE));
                    //(4)如果有一筆關箱完成，更新該驗收單所有F0205.STATUS=1(分播完成)
                    var tmpR = ExProxy.CreateQuery<ExecuteResult>("UpdataF0205StatusTo1")
                                             .AddQueryExOption("dcCode", dcCode)
                                             .AddQueryExOption("gupCode", gupCode)
                                             .AddQueryExOption("custCode", custCode)
                                             .AddQueryExOption("RTNo", RTNo)
                                             .AddQueryExOption("RTSeq", RTSEQ).ToList();
                }
                //檢查調撥單是否有異常，有的話要顯示
                if (!string.IsNullOrWhiteSpace(result.No))
                {
                    var allocMsg = proxy.RunWcfMethod(w => w.GetUnnormalAllocDatas(dcCode, gupCode, custCode, new[] { result.No })).FirstOrDefault();
                    if(!string.IsNullOrWhiteSpace(allocMsg))
                    MessageList.Add(allocMsg);
                }
            }
            //(5)顯示訊息清單用換行方式顯示在畫面上
            ShowInfoMessage(string.Join("\r\n", MessageList));
            //(6)重新整理UI=>綁定容器清單，重新取得驗收單所有F0205資料
            SetInitValue(dcCode, gupCode, custCode, RTNo, RTSEQ);
        }
        #endregion 關箱

        #region 揀區容器綁定
        public ICommand PickupAreaBindCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    //各區容器綁定[當該區預計分播數F0205有該區資料且F0205.B_QTY >0 AND F0205.STATUS=0按鈕才可以按]
                    () =>
                    {
                        if (ItemBindContainerDataResult == null)
                            return false;
                        var filter = ItemBindContainerDataResult?.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "A");
                        return filter?.B_QTY > 0 && filter?.STATUS == "0";
                    },
                    o =>
                    {
                        SendToP0202060300Data = ItemBindContainerDataResult.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "A");
                        DoShowP0202060300();
                    }
                    );
            }
        }

        #endregion 揀區容器綁定

        #region 補區容器綁定
        public ICommand ReplenishmentAreaBindCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    //各區容器綁定[當該區預計分播數F0205有該區資料且F0205.B_QTY >0 AND F0205.STATUS=0按鈕才可以按]
                    () =>
                    {
                        if (ItemBindContainerDataResult == null)
                            return false;
                        var filter = ItemBindContainerDataResult.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "C");
                        return filter?.B_QTY > 0 && filter?.STATUS == "0";
                    },
                    o =>
                    {
                        SendToP0202060300Data = ItemBindContainerDataResult.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "C");
                        DoShowP0202060300();
                    });
            }
        }

        #endregion 補區容器綁定

        #region 不良品區容器綁定
        public ICommand NGAreaBindCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    //各區容器綁定[當該區預計分播數F0205有該區資料且F0205.B_QTY >0 AND F0205.STATUS=0按鈕才可以按]
                    () =>
                    {
                        if (ItemBindContainerDataResult == null)
                            return false;
                        var filter = ItemBindContainerDataResult.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "R");
                        return filter?.B_QTY > 0 && filter?.STATUS == "0";
                    },
                    o =>
                    {
                        SendToP0202060300Data = ItemBindContainerDataResult.FirstOrDefault(x => x.TYPE_CODE.ToUpper() == "R");
                        DoShowP0202060300();
                    });
            }
        }

        #endregion 不良品區容器綁定

        #region 綁定完成
        public ICommand BindCompleteCommand
        {
            get
            {
                Boolean _IsSuccess = false;
                return CreateBusyAsyncCommand(
                    o => _IsSuccess = DoBindComplete(),
                    () => true,
                    o => { if (_IsSuccess) DoExitWin(true); }
                    );
            }
        }

        public Boolean DoBindComplete()
        {
            var ExProxy = GetExProxy<P02ExDataSource>();
            var result = ExProxy.CreateQuery<ExecuteResult>("SetContainerComplete")
                                            .AddQueryExOption("dcCode", dcCode)
                                            .AddQueryExOption("gupCode", gupCode)
                                            .AddQueryExOption("custCode", custCode)
                                            .AddQueryExOption("RTNo", RTNo)
                                            .AddQueryExOption("RTSEQ", RTSEQ).ToList();
            ShowResultMessage(result.FirstOrDefault());
            if (result.FirstOrDefault().IsSuccessed)
                return result.FirstOrDefault().IsSuccessed;

            SetInitValue(dcCode, gupCode, custCode, RTNo, RTSEQ);
            return false;
        }
        #endregion 綁定完成

        #endregion ICommand
    }
}
