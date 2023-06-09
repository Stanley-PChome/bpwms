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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
    public partial class P7105060000_ViewModel : InputViewModelBase
    {
        private string _userId;
        private string _userName;
        private string _gupCode;
        private string _custCode;
        private bool _isSave;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };
        public Action DeleteAction = delegate { };
        public Action CancelAction = delegate { };
        public Action CollapsedQryResultAction = delegate { };

        public P7105060000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                _userId = Wms3plSession.Get<UserInfo>().Account;
                _userName = Wms3plSession.Get<UserInfo>().AccountName;
                _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
                _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
                InitControls();
            }
        }

        private void InitControls()
        {
            GET_DC_LIST();
            GET_ACC_UNIT_LIST();
            GET_IN_TAX_LIST();
            GET_DELV_ACC_TYPE_LIST();
            GET_STATUS_LIST();
            STATUS = "";
        }

        #region 物流中心
        private string _dccode;
        public string DC_CODE
        {
            get { return _dccode; }
            set
            {
                _dccode = value;
                RaisePropertyChanged("DC_CODE");
            }
        }

        private List<NameValuePair<string>> _dclist;
        public List<NameValuePair<string>> DC_LIST
        {
            get { return _dclist; }
            set
            {
                _dclist = value;
                RaisePropertyChanged("DCLIST");
            }
        }
        private void GET_DC_LIST()
        {
			var dcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			dcList.Insert(0, new NameValuePair<string>(Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, "000"));
			DC_LIST = dcList;
            if (DC_LIST.Any())
                DC_CODE = DC_LIST.First().Value;

        }
        #endregion

        #region 計價項目
        private string _accitemname;
        public string ACC_ITEM_NAME
        {
            get { return _accitemname; }
            set
            {
                _accitemname = value;
                RaisePropertyChanged("ACC_ITEM_NAME");
            }
        }
        #endregion

        #region 計價單位
        private List<NameValuePair<string>> _accunitlist;
        public List<NameValuePair<string>> ACC_UNIT_LIST
        {
            get { return _accunitlist; }
            set
            {
                _accunitlist = value;
                RaisePropertyChanged("ACC_UNIT_LIST");
            }
        }
        private void GET_ACC_UNIT_LIST()
        {

            var proxyF91 = GetProxy<F91Entities>();
            var result = proxyF91.F91000302s.Where(x => x.ITEM_TYPE_ID == "006")
          .Select(x => new NameValuePair<string>() { Value = x.ACC_UNIT, Name = x.ACC_UNIT_NAME }).ToList();
            ACC_UNIT_LIST = result;
        }
        #endregion

        #region 稅別
        private List<NameValuePair<string>> _intaxlist;
        public List<NameValuePair<string>> IN_TAX_LIST
        {
            get { return _intaxlist; }
            set
            {
                _intaxlist = value;
                RaisePropertyChanged("IN_TAX_LIST");
            }
        }
        private void GET_IN_TAX_LIST()
        {

            IN_TAX_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F199006", "IN_TAX");
        }
        #endregion

        #region 配送計價類別
        private List<NameValuePair<string>> _delvacctypelist;
        public List<NameValuePair<string>> DELV_ACC_TYPE_LIST
        {
            get { return _delvacctypelist; }
            set
            {
                _delvacctypelist = value;
                RaisePropertyChanged("DELV_ACC_TYPE_LIST");
            }
        }
        private void GET_DELV_ACC_TYPE_LIST()
        {

            DELV_ACC_TYPE_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F199006", "DELV_ACC_TYPE");
        }
        #endregion

        #region STATUS
        private string _status;
        public string STATUS
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("STATUS");
            }
        }

        private List<NameValuePair<string>> _statuslist;
        public List<NameValuePair<string>> STATUS_LIST
        {
            get { return _statuslist; }
            set
            {
                _statuslist = value;
                RaisePropertyChanged("STATUS_LIST");
            }
        }
        private void GET_STATUS_LIST()
        {

            STATUS_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F199006", "STATUS");
            STATUS_LIST.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
        }
        #endregion

        #region F199006(Ex) Series
        private ObservableCollection<F199006> _dgf199006;
        public ObservableCollection<F199006> dgF199006
        {
            get { return _dgf199006; }
            set
            {
                _dgf199006 = value;
                RaisePropertyChanged("dgF199006");
            }
        }

        private F199006 _selectedf199006;
        public F199006 Selected_F199006
        {
            get { return _selectedf199006; }
            set
            {
                _selectedf199006 = value;
                RaisePropertyChanged("Selected_F199006");
            }
        }

        private F199006 _f199006Datas;
        public F199006 F199006Datas
        {
            get { return _f199006Datas; }
            set
            {
                _f199006Datas = value;
                RaisePropertyChanged("F199006Datas");
            }
        }
        #endregion

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoSearch(),
                  () => UserOperateMode == OperateMode.Query,
                  o => DoSearchCompleted()
                  );
            }
        }


        private void DoSearch()
        {
            dgF199006 = new ObservableCollection<F199006>();
            var proxy = GetProxy<F19Entities>();
            dgF199006 = proxy.CreateQuery<F199006>("GetF199006s")
                                                 .AddQueryOption("dcCode", string.Format("'{0}'", DC_CODE))
                                                 .AddQueryOption("accItemName", string.Format("'{0}'", ACC_ITEM_NAME))
                                                 .AddQueryOption("status", string.Format("'{0}'", STATUS))
                                                 .ToObservableCollection();
        }

        private void DoSearchCompleted()
        {
            if (dgF199006 == null || dgF199006.Count == 0)
            {
                ClearData();
                DialogService.ShowMessage(Resources.Resources.InfoNoData);
            }
            else
            {
                SearchAction();
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
            F199006Datas = new F199006();
            //RemoveItemFromList();
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoEdit(),
                  () => UserOperateMode == OperateMode.Query && Selected_F199006 != null && Selected_F199006.STATUS == "0"
                  );
            }
        }

        private void DoEdit()
        {
            UserOperateMode = OperateMode.Edit;
            F199006Datas = ExDataMapper.Clone(Selected_F199006);
        }
        #endregion Edit

        #region Cancel
        public ICommand CancelCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoCancel(),
                  () => UserOperateMode != OperateMode.Query,
                  o => DoCancelCompleted()
                  );
            }
        }

        private void DoCancel()
        {
            //執行取消動作
            UserOperateMode = OperateMode.Query;
           // AddItemToList();
        }

        private void DoCancelCompleted()
        {
            if (F199006Datas != null)
                F199006Datas = null;
            CancelAction();
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoDelete(),
                  () => UserOperateMode == OperateMode.Query && Selected_F199006 != null && Selected_F199006.STATUS == "0",
                  o => DoDeleteCompleted()
                  );
            }
        }

        private void DoDelete()
        {
            _isSave = false;
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                F199006 f199006 = new F199006();
                //f199006 = ExDataMapper.Map<F199006, F199006>(Selected_F199006);
                f199006 = ExDataMapper.Clone(Selected_F199006);
                f199006.UPD_STAFF = _userId;
                f199006.UPD_DATE = DateTime.Now;
                f199006.UPD_NAME = _userName;

                var proxy = new wcf.P71WcfServiceClient();
                var wcfF199006= ExDataMapper.Map<F199006, wcf.F199006>(f199006);
                var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF199006(wcfF199006));

                if (result.IsSuccessed)
                {
                    ShowMessage(Messages.InfoDeleteSuccess);
                    _isSave = true;
                }
                else
                {
                    _isSave = false;
                    ShowMessage(Messages.ErrorUpdateFailed);
                }
            }
        }

        private void DoDeleteCompleted()
        {
            if (_isSave)
            {
                DC_CODE = Selected_F199006.DC_CODE;
                ACC_ITEM_NAME = Selected_F199006.ACC_ITEM_NAME;
                STATUS = "9";
                ClearData();
                SearchCommand.Execute(null);
                UserOperateMode = OperateMode.Query;
            }
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoSave(),
                  () => UserOperateMode != OperateMode.Query,
                  o => DoSaveCompleted()
                  );
            }
        }

        private void DoSave()
        {
            if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
            {
                _isSave = false;
                if (F199006Datas.ACC_NUM <= 0)
                {
                    DialogService.ShowMessage(Properties.Resources.P7105060000_ViewModel_ACC_NUM_LessThanOne);
                    return;
                }
                if (UserOperateMode == OperateMode.Add)
                {
                    #region SaveAdd
                    F199006 f199006 = new F199006();
                    //f199001 = ExDataMapper.Map<F199001Ex, F199001>(F199006Datas);
                    f199006 = ExDataMapper.Clone(F199006Datas);
                    f199006.CRT_STAFF = _userId;
                    f199006.CRT_DATE = DateTime.Now;
                    f199006.CRT_NAME = _userName;

                    var proxy = new wcf.P71WcfServiceClient();
                    var wcfF199006 = ExDataMapper.Map<F199006, wcf.F199006>(f199006);
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF199006(wcfF199006));

                    if (result.IsSuccessed)
                    {
                        ShowMessage(Messages.InfoAddSuccess);
                        _isSave = true;
                    }
                    else
                    {
                        _isSave = false;
												DialogService.ShowMessage(result.Message);
                    }
                    #endregion
                }
                else
                {
                    #region SaveEdit
                    F199006 f199006 = new F199006();
                    //f199001 = ExDataMapper.Map<F199001Ex, F199001>(F199006Datas);
                    f199006 = ExDataMapper.Clone(F199006Datas);
                    f199006.UPD_STAFF = _userId;
                    f199006.UPD_DATE = DateTime.Now;
                    f199006.UPD_NAME = _userName;

                    var proxy = new wcf.P71WcfServiceClient();
                    var wcfF199006 = ExDataMapper.Map<F199006, wcf.F199006>(f199006);
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF199006(wcfF199006));

                    if (result.IsSuccessed)
                    {
                        ShowMessage(Messages.InfoUpdateSuccess);
                        _isSave = true;
                    }
                    else
                    {
                        _isSave = false;
                        ShowMessage(Messages.ErrorUpdateFailed);
                    }
                    #endregion
                }
            }
        }

        private void DoSaveCompleted()
        {
            if (_isSave)
            {
                DC_CODE = F199006Datas.DC_CODE;
                ACC_ITEM_NAME = F199006Datas.ACC_ITEM_NAME;
                STATUS = "";
                ClearData();
                SearchCommand.Execute(null);
                UserOperateMode = OperateMode.Query;
            }
        }
        #endregion Save

        #region ClearData
        private void ClearData()
        {
            if (dgF199006 != null)
                dgF199006 = null;
            if (Selected_F199006 != null)
                Selected_F199006 = null;
            if (F199006Datas != null)
                F199006Datas = null;
        }
        #endregion





    }
}
