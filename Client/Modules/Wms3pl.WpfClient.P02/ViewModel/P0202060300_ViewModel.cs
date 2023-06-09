using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.Datas.F02;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using sharewcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
    public partial class P0202060300_ViewModel : InputViewModelBase
    {
        public Action DoFocusContanerCode = delegate { };
        public Action DoCloseAction = delegate { };

        /// <summary>
        /// 資料傳進來後還會用到，把他先存起來
        /// </summary>
        F0205 _f0205;

        public P0202060300_ViewModel()
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

        public void SetInitValue(F0205 f0205)
        {
            if (_f0205 == null)
            {
                _f0205 = f0205;

                switch (f0205.TYPE_CODE)
                {
                    case "A":
                        FunctionHeader = "請刷入容器條碼(揀區)";
                        ContainerCodeHintMsg = "容器條碼/容器分格條碼";
                        colBindCodeVisible = "Visible";
                        break;
                    case "C":
                        FunctionHeader = "請刷入容器條碼(補區)";
                        ContainerCodeHintMsg = "容器條碼";
                        colBindCodeVisible = "Collapsed";
                        break;
                    case "R":
                        FunctionHeader = "請刷入容器條碼(不良品區)";
                        ContainerCodeHintMsg = "容器條碼";
                        colBindCodeVisible = "Collapsed";
                        break;
                    default:
                        ShowWarningMessage("無法辨識的區域");
                        break;
                }
            }
            DoFocusContanerCode();
            ContainerCode = "";
            UpdateRemainingQTY();

            var exproxy = GetExProxy<P02ExDataSource>();
            BindContainerList = exproxy.CreateQuery<AreaContainerData>("GetAreaContainerData")
                                 .AddQueryExOption("dcCode", f0205.DC_CODE)
                                 .AddQueryExOption("gupCode", f0205.GUP_CODE)
                                 .AddQueryExOption("custCode", f0205.CUST_CODE)
                                 .AddQueryExOption("typeCode", f0205.TYPE_CODE)
                                 .AddQueryExOption("RTNo", f0205.RT_NO)
                                 .AddQueryExOption("RTSEQ", f0205.RT_SEQ).ToObservableCollection();


        }

        public void UpdateMemoryF0205A_QTY()
        {
            _f0205.A_QTY = BindContainerList.Sum(x => x.QTY);
            UpdateRemainingQTY();
        }

        private void UpdateRemainingQTY()
        {
            ItemQty = _f0205.B_QTY - (_f0205.A_QTY ?? 0);
            if (ItemQty < 0)
                ItemQty = 0;
        }

        /// <summary>
        /// 給UI判斷是否有需要檢查輸入數量小於1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean IsNeedCheckQTY(int value)
        {
            var tmpQTY = _f0205.B_QTY - (_f0205.A_QTY ?? 0);
            return ItemQty > 0;
        }

        #region 前端畫面屬性

        #region 表頭名稱
        private string _FunctionHeader;
        /// <summary>
        /// 表頭名稱
        /// </summary>
        public string FunctionHeader
        {
            get { return _FunctionHeader; }
            set { _FunctionHeader = value; RaisePropertyChanged(); }
        }
        #endregion 表頭名稱

        #region 綁定容器編號
        private string _ContainerCode;
        /// <summary>
        /// 綁定容器編號
        /// </summary>
        public string ContainerCode
        {
            get { return _ContainerCode; }
            set { _ContainerCode = value; RaisePropertyChanged(); }
        }
        #endregion 綁定容器編號

        #region 放入數量
        private int _ItemQty;
        /// <summary>
        /// 放入數量
        /// </summary>
        public int ItemQty
        {
            get { return _ItemQty; }
            set { _ItemQty = value; RaisePropertyChanged(); }
        }
        #endregion 放入數量

        #region 資料清單
        private ObservableCollection<AreaContainerData> _BindContainerList;
        /// <summary>
        /// 資料清單
        /// </summary>
        public ObservableCollection<AreaContainerData> BindContainerList
        {
            get { return _BindContainerList; }
            set { _BindContainerList = value; RaisePropertyChanged(); }
        }
        #endregion 資料清單

        #region 資料清單所選項目
        private AreaContainerData _SelectedBindContainer;
        /// <summary>
        /// 資料清單所選項目
        /// </summary>
        public AreaContainerData SelectedBindContainer
        {
            get { return _SelectedBindContainer; }
            set { _SelectedBindContainer = value; RaisePropertyChanged(); }
        }
        #endregion 資料清單所選項目

        #region 容器分隔條碼Visable狀態
        private String _colBindCodeVisible;
        /// <summary>
        /// 容器分隔條碼Visable狀態
        /// </summary>
        public String colBindCodeVisible
        {
            get { return _colBindCodeVisible; }
            set { _colBindCodeVisible = value; RaisePropertyChanged(); }
        }

        #endregion

        #region txtContainerCode(容器)提示輸入內容文字
        private String _ContainerCodeHintMsg = "容器條碼/容器分格條碼";
        /// <summary>
        /// txtContainerCode(容器)提示輸入內容文字
        /// </summary>
        public String ContainerCodeHintMsg
        {
            get { return _ContainerCodeHintMsg; }
            set { _ContainerCodeHintMsg = value; RaisePropertyChanged(); }
        }

        #endregion txtContainerCode(容器)提示輸入內容文字

        #endregion 前端畫面屬性

        #region ICommand

        #region 確認按鈕
        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => _f0205.B_QTY - _f0205.A_QTY > 0,
                    o => DoAdd()
                    );
            }
        }


        private void DoAdd()
        {
            string msg = "";
            if (String.IsNullOrWhiteSpace(ContainerCode))
                msg = "請輸入容器條碼";
            if (ItemQty <= 0)
                msg = "放入數量必須大於0";
            //檢查是否有重複輸入容器
            //檢查輸入的內容是否有分格符號
            if (ContainerCode.IndexOf('-') != -1)
            {
                if (BindContainerList.Any(x => x.BIN_CODE == ContainerCode))
                    msg = "此容器已存在清單中";
            }
            else
            {
                if (BindContainerList.Any(x => x.CONTAINER_CODE == ContainerCode))
                    msg = "此容器已存在清單中";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ShowWarningMessage(msg);
                DoFocusContanerCode();
                return;
            }
            IsBusy = true;

            var proxy = GetWcfProxy<wcf.P02WcfServiceClient>();
            var result = proxy.RunWcfMethod(x => x.AddF020501(ContainerCode, ItemQty, _f0205.Map<F0205, wcf.F0205>()));
            if (!result.IsSuccessed)
            {
                IsBusy = false;
                ShowWarningMessage(result.Message);
                if (result.NeedFocuseContanerCode)
                    DoFocusContanerCode();
                return;
            }

            var shareproxy = GetWcfProxy<sharewcf.SharedWcfServiceClient>();
            var containerResult = shareproxy.RunWcfMethod(x => x.CheckContainer(ContainerCode));

            BindContainerList.Add(new AreaContainerData()
            {
                ID = result.F020502_ID,
                F020501_ID = result.F020501_ID,
                DC_CODE = _f0205.DC_CODE,
                GUP_CODE = _f0205.GUP_CODE,
                CUST_CODE = _f0205.CUST_CODE,
                STOCK_NO = _f0205.STOCK_NO,
                STOCK_SEQ = _f0205.STOCK_SEQ,
                RT_NO = _f0205.RT_NO,
                RT_SEQ = _f0205.RT_SEQ,
                TYPE_CODE = _f0205.TYPE_CODE,
                CONTAINER_CODE = containerResult.ContainerCode,
                MCONTAINER_CODE = containerResult.ContainerCode,
                BIN_CODE = containerResult.BinCode,
                QTY = ItemQty
            });
            BindContainerList = BindContainerList;
            ContainerCode = "";
            UpdateMemoryF0205A_QTY();
            IsBusy = false;
            DoFocusContanerCode();
        }

        #endregion 確認按鈕

        #region 刪除按鈕
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => { },
                        () => true,
                        o => DoDelete());
            }
        }

        private void DoDelete()
        {

            //(1)提示”是否要釋放該容器XXX商品綁定?” 若選否，回到畫面並Focus容器條碼若選是往下執行
            if (ShowConfirmMessage($"是否要釋放該容器{SelectedBindContainer.CONTAINER_CODE}商品綁定?") != UILib.Services.DialogResponse.Yes)
            {
                DoFocusContanerCode();
                return;
            }
            var proxy = GetWcfProxy<wcf.P02WcfServiceClient>();
            var result = proxy.RunWcfMethod(x => x.DeleteContainerBindData(SelectedBindContainer.Map<AreaContainerData, wcf.AreaContainerData>()));
            ShowResultMessage(result);
            if (!result.IsSuccessed)
                return;

            DoFocusContanerCode();
            BindContainerList.Remove(SelectedBindContainer);
            BindContainerList = BindContainerList;
            UpdateMemoryF0205A_QTY();
        }
        #endregion 刪除按鈕

        #region 離開按鈕
        public ICommand ExitCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => { },
                        () => BindContainerList.Any(x => x.QTY > 0) || !BindContainerList.Any(),
                        o => DoExit()
              );
            }
        }

        private void DoExit()
        {
            String msg = "";
            if (BindContainerList.Any(x => x.QTY <= 0))
                msg = "放入數量必須大於0";
            // (1)檢查該區放入數量加總是否超過預計分播數，若超過，顯示訊息”您已綁定容器總放入數量超過該區預計分播數量XXX，請調整放入數量”
            var A_QTY = BindContainerList.Sum(x => x.QTY);
            if (_f0205.B_QTY < A_QTY)
                msg = $"您已綁定容器總放入數量超過該區預計分播數量{_f0205.B_QTY}，請調整放入數量";


            if (!string.IsNullOrEmpty(msg))
            {
                ShowWarningMessage(msg);
                return;
            }

            var UpdateData = BindContainerList.MapCollection<AreaContainerData, wcf.AreaContainerData>().ToArray();
            var proxy = GetWcfProxy<wcf.P02WcfServiceClient>();
            var result = proxy.RunWcfMethod(x => x.UpdateContainerBindData(UpdateData));
            DoCloseAction();
        }
        #endregion 離開按鈕

        #endregion
    }
}
