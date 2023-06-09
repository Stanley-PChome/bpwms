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
    public partial class P7105010000_ViewModel : InputViewModelBase
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

        public P7105010000_ViewModel()
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
            GET_LOC_TYPE_ID_LIST();
            GET_LOC_TYPE_INFO_LIST();
            GET_TMPR_TYPE_LIST();
            GET_IN_TAX_LIST();
            GET_ACC_UNIT_LIST();
            GET_ACC_ITEM_KIND_ID_LIST();
            GET_STATUS_LIST();
            LOC_TYPE_ID = "";
            TMPR_TYPE = "";
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
			var data = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
			data.Insert(0, new NameValuePair<string>(Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, "000"));
			DC_LIST = data;
            if (DC_LIST.Any())
                DC_CODE = DC_LIST.First().Value;

        }
        #endregion

        #region 儲位型態
        private string _loctypeid;
        public string LOC_TYPE_ID
        {
            get { return _loctypeid; }
            set
            {
                _loctypeid = value;
                RaisePropertyChanged("LOC_TYPE_ID");
            }
        }

        private List<NameValuePair<string>> _loctypeidlist;
        public List<NameValuePair<string>> LOC_TYPE_ID_LIST
        {
            get { return _loctypeidlist; }
            set
            {
                _loctypeidlist = value;
                RaisePropertyChanged("LOC_TYPE_ID_LIST");
            }
        }
        private void GET_LOC_TYPE_ID_LIST()
        {
            var proxyF19 = GetProxy<F19Entities>();
            var result = proxyF19.F1942s.OrderBy(x => x.LOC_TYPE_ID)
          .Select(x => new NameValuePair<string>() { Value = x.LOC_TYPE_ID, Name = x.LOC_TYPE_NAME }).ToList();
            result.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
            LOC_TYPE_ID_LIST = result;
        }
        #endregion

        #region 儲位IFNO
        private List<F1942> _loctypeinfo;
        public List<F1942> LOC_TYPE_INFO_LIST
        {
            get { return _loctypeinfo; }
            set
            {
                _loctypeinfo = value;
                RaisePropertyChanged("LOC_TYPE_INFO_LIST");
            }
        }
        private void GET_LOC_TYPE_INFO_LIST()
        {
            var proxyF19 = GetProxy<F19Entities>();
            var result = proxyF19.F1942s.OrderBy(x => x.LOC_TYPE_ID).AsQueryable().ToList();
            LOC_TYPE_INFO_LIST = result;
        }
        #endregion

        #region 溫層
        private string _tmprtype;
        public string TMPR_TYPE
        {
            get { return _tmprtype; }
            set
            {
                _tmprtype = value;
                RaisePropertyChanged("TMPR_TYPE");
            }
        }

        private List<NameValuePair<string>> _tmprtypelist;
        public List<NameValuePair<string>> TMPR_TYPE_LIST
        {
            get { return _tmprtypelist; }
            set
            {
                _tmprtypelist = value;
                RaisePropertyChanged("TMPR_TYPE_LIST");
            }
        }
        private void GET_TMPR_TYPE_LIST()
        {
            TMPR_TYPE_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F1980", "TMPR_TYPE", true);
        }
        #endregion

        #region 稅別
        private string _intax;
        public string IN_TAX
        {
            get { return _intax; }
            set
            {
                _intax = value;
                RaisePropertyChanged("IN_TAX");
            }
        }

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

            IN_TAX_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F199001", "IN_TAX");
        }
        #endregion

        #region 計價單位
        private string _accunit;
        public string ACC_UNIT
        {
            get { return _accunit; }
            set
            {
                _accunit = value;
                RaisePropertyChanged("ACC_UNIT");
            }
        }

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
            var result = proxyF91.F91000302s.Where(x => x.ITEM_TYPE_ID == "002")
          .Select(x => new NameValuePair<string>() { Value = x.ACC_UNIT, Name = x.ACC_UNIT_NAME }).ToList();
            ACC_UNIT_LIST = result;
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

            STATUS_LIST = GetBaseTableService.GetF000904List(FunctionCode, "F199001", "STATUS");
            STATUS_LIST.Insert(0, new NameValuePair<string>() { Value = "", Name = Resources.Resources.All });
        }
        #endregion

        #region 計價項目
        private string _accitemkindid;
        public string ACC_ITEM_KIND_ID
        {
            get { return _accitemkindid; }
            set
            {
                _accitemkindid = value;
                RaisePropertyChanged("ACC_ITEM_KIND_ID");
            }
        }

        private List<NameValuePair<string>> _accitemkindidlist;
        public List<NameValuePair<string>> ACC_ITEM_KIND_ID_LIST
        {
            get { return _accitemkindidlist; }
            set
            {
                _accitemkindidlist = value;
                RaisePropertyChanged("ACC_ITEM_KIND_ID_LIST");
            }
        }
        private void GET_ACC_ITEM_KIND_ID_LIST()
        {
            var proxy = GetExProxy<P71ExDataSource>();
            var results = proxy.CreateQuery<F91000301Data>("GetAccItemKinds")
                                                 .AddQueryExOption("itemTypeId", "002")
                                                 .ToList()
                                                 .Select(x => new NameValuePair<string>()
                                                 {
                                                     Name = x.ACC_ITEM_KIND_NAME,
                                                     Value = x.ACC_ITEM_KIND_ID
                                                 }
                                                 ).ToList();
            ACC_ITEM_KIND_ID_LIST = results;
        }
        #endregion

        #region F199001(Ex) Series
        private ObservableCollection<F199001Ex> _dgf199001exs;
        public ObservableCollection<F199001Ex> dgF199001Exs
        {
            get { return _dgf199001exs; }
            set
            {
                _dgf199001exs = value;
                RaisePropertyChanged("dgF199001Exs");
            }
        }

        private F199001Ex _selectedf199001ex;
        public F199001Ex Selected_F199001Ex
        {
            get { return _selectedf199001ex; }
            set
            {
                _selectedf199001ex = value;
                RaisePropertyChanged("Selected_F199001Ex");
            }
        }

        private F199001Ex _f199001Datas;
        public F199001Ex F199001Datas
        {
            get { return _f199001Datas; }
            set
            {
                _f199001Datas = value;
                RaisePropertyChanged("F199001Datas");
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
            //ClearData();
            dgF199001Exs = new ObservableCollection<F199001Ex>();
            var proxyP71 = GetExProxy<P71ExDataSource>();
            dgF199001Exs = proxyP71.CreateQuery<F199001Ex>("GetF199001Exs")
                                                        .AddQueryOption("dcCode", string.Format("'{0}'", DC_CODE))
                                                        .AddQueryOption("locTypeID", string.Format("'{0}'", LOC_TYPE_ID))
                                                        .AddQueryOption("tmprType", string.Format("'{0}'", TMPR_TYPE))
                                                        .AddQueryOption("status", string.Format("'{0}'", STATUS))
                                                        .ToObservableCollection();
        }

        private void DoSearchCompleted()
        {
            if (dgF199001Exs == null || dgF199001Exs.Count == 0)
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
            F199001Datas = new F199001Ex();
            RemoveItemFromList();
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                  o => DoEdit(),
                  () => UserOperateMode == OperateMode.Query && Selected_F199001Ex != null && Selected_F199001Ex.STATUS == "0"
                  );
            }
        }

        private void DoEdit()
        {
            UserOperateMode = OperateMode.Edit;
            F199001Datas = ExDataMapper.Clone(Selected_F199001Ex);
            RemoveItemFromList();
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
            AddItemToList();
        }

        private void DoCancelCompleted()
        {
            if (F199001Datas != null)
                F199001Datas = null;
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
                  () => UserOperateMode == OperateMode.Query && Selected_F199001Ex != null && Selected_F199001Ex.STATUS == "0",
                  o => DoDeleteCompleted()
                  );
            }
        }

        private void DoDelete()
        {
            _isSave = false;
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                F199001 f199001 = new F199001();
                f199001 = ExDataMapper.Map<F199001Ex, F199001>(Selected_F199001Ex);
                f199001.UPD_STAFF = _userId;
                f199001.UPD_DATE = DateTime.Now;
                f199001.UPD_NAME = _userName;

                var proxy = new wcf.P71WcfServiceClient();
                var wcfF199001 = ExDataMapper.Map<F199001, wcf.F199001>(f199001);
                var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.DeleteF199001(wcfF199001));

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
                DC_CODE = Selected_F199001Ex.DC_CODE;
                LOC_TYPE_ID = Selected_F199001Ex.LOC_TYPE_ID;
                TMPR_TYPE = Selected_F199001Ex.TMPR_TYPE;
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
                if (UserOperateMode == OperateMode.Add)
                {
                    #region SaveAdd
                    F199001 f199001 = new F199001();
                    f199001 = ExDataMapper.Map<F199001Ex, F199001>(F199001Datas);
                    f199001.CRT_STAFF = _userId;
                    f199001.CRT_DATE = DateTime.Now;
                    f199001.CRT_NAME = _userName;

                    var proxy = new wcf.P71WcfServiceClient();
                    var wcfF199001 = ExDataMapper.Map<F199001, wcf.F199001>(f199001);
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.InsertF199001(wcfF199001));

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
                    F199001 f199001 = new F199001();
                    f199001 = ExDataMapper.Map<F199001Ex, F199001>(F199001Datas);
                    f199001.UPD_STAFF = _userId;
                    f199001.UPD_DATE = DateTime.Now;
                    f199001.UPD_NAME = _userName;

                    var proxy = new wcf.P71WcfServiceClient();
                    var wcfF199001 = ExDataMapper.Map<F199001, wcf.F199001>(f199001);
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateF199001(wcfF199001));

                    if (result.IsSuccessed)
                    {
                        ShowMessage(Messages.InfoUpdateSuccess);
                        _isSave = true;
                    }
                    else
                    {
                        _isSave = false;
	                    ShowWarningMessage(result.Message);
                    }
                    #endregion
                }
            }
        }

        private void DoSaveCompleted()
        {
            if (_isSave)
            {
                AddItemToList();
                DC_CODE = F199001Datas.DC_CODE;
                LOC_TYPE_ID = F199001Datas.LOC_TYPE_ID;
                TMPR_TYPE = F199001Datas.TMPR_TYPE;
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
            if (dgF199001Exs != null)
                dgF199001Exs = null;
            if (Selected_F199001Ex != null)
                Selected_F199001Ex = null;
            if (F199001Datas != null)
                F199001Datas = null;
        }
        #endregion

        #region RemoveItemFromList
        private void RemoveItemFromList()
        {
            if (LOC_TYPE_ID_LIST != null)
                LOC_TYPE_ID_LIST.RemoveAt(0);
            if (TMPR_TYPE_LIST != null)
                TMPR_TYPE_LIST.RemoveAt(0);
        }
        #endregion

        #region AddItemToList
        private void AddItemToList()
        {
            if (LOC_TYPE_ID_LIST != null)
                LOC_TYPE_ID_LIST.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
            if (TMPR_TYPE_LIST != null)
                TMPR_TYPE_LIST.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
        }
        #endregion

        #region 取得儲位長寬高
        public void Get_Loc_Info(string loc_type_id)
        {
            var f1942 = LOC_TYPE_INFO_LIST.Where(x => x.LOC_TYPE_ID == loc_type_id).FirstOrDefault();
            if (f1942 != null && F199001Datas != null)
            {
                F199001Datas.LENGTH = (short)f1942.LENGTH;
                F199001Datas.WEIGHT = (decimal)f1942.WEIGHT;
                F199001Datas.HEIGHT = (short)f1942.HEIGHT;
            }
        }
        #endregion

    }
}
