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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.P19.ViewModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
   

    public partial class P1920030000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        private readonly F91Entities _proxy;
        private string _userId;
        private string _userName;
        private bool isValid;
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };

        #endregion

        public P1920030000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                _proxy = GetProxy<F91Entities>();
                _userId = Wms3plSession.Get<UserInfo>().Account;
                _userName = Wms3plSession.Get<UserInfo>().AccountName;
                InitControls();
            }
        }

        private void InitControls()
        {
            //GetDcCodes();
            //SearchCommand.Execute(null);
        }

        //private void GetDcCodes()
        //{
        //    DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
        //    if (DcCodes.Any())
        //    {
        //        SelectDcCode = DcCodes.First().Value;
        //    }
        //}

        private List<NameValuePair<string>> _dcCodes;

        public List<NameValuePair<string>> DcCodes
        {
            get { return _dcCodes; }
            set
            {
                _dcCodes = value;
                RaisePropertyChanged();
            }
        }

        //private string _selectDcCode;

        //public string SelectDcCode
        //{
        //    get { return _selectDcCode; }
        //    set
        //    {
        //        _selectDcCode = value;
        //        SelectedData = null;
        //        DataList = null;
        //        //DoSearch();
        //        //SearchCommand.Execute(null);
        //        RaisePropertyChanged("SelectDcCode");
        //    }
        //}

        private string _ITEMTYPEID = string.Empty;
        //[Required(ErrorMessage = Resources.Resources.Required_ErrorMessage, AllowEmptyStrings = false)]
        public string ITEMTYPEID
        {
            get { return _ITEMTYPEID; }
            set
            {
                _ITEMTYPEID = value;
                RaisePropertyChanged("ITEMTYPEID");
            }
        }

        private string _ITEMTYPE = string.Empty;
        public string ITEMTYPE
        {
            get { return _ITEMTYPE; }
            set
            {
                _ITEMTYPE = value;
                RaisePropertyChanged("ITEMTYPE");
            }
        }

        private List<F910003> _DataList;
        public List<F910003> DataList
        {
            get
            {
                if (_DataList == null) _DataList = new List<F910003>();
                return _DataList;
            }
            set
            {
                _DataList = value;
                RaisePropertyChanged("DataList");
            }
        }

        private F910003 _selectedData;

        public F910003 SelectedData
        {
            get { return _selectedData; }
            set
            {
                //if (_selectedData != null && (UserOperateMode == OperateMode.Edit || UserOperateMode == OperateMode.Add))
                if (_selectedData != null && (UserOperateMode == OperateMode.Edit))
                {
                    //ShowMessage(Properties.Resources.P1901090100_UnSelectableStatus);
                    return;
                }
                else
                {
                    _selectedData = value;
                    RaisePropertyChanged("SelectedData");
                }
            }
        }

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
            var proxy = GetProxy<F91Entities>();
            this.DataList = (proxy.CreateQuery<F910003>("GetF910003sByITEMTYPEIDANDITEMTYPE")
                 .AddQueryOption("ITEM_TYPE_ID", "'" + this._ITEMTYPEID + "'")
                 .AddQueryOption("ITEM_TYPE", "'" + this._ITEMTYPE + "'")).ToList();

            if (DataList == null || DataList.Count.Equals(0))
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }
        }

        private void DoSearchCompleted()
        {
            if (DataList == null || !DataList.Any()) return;
            SelectedData = DataList.FirstOrDefault();
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
            F910003 newItem = new F910003();
            newItem.CRT_DATE = DateTime.Now;
            DataList.Add(newItem);
            DataList = DataList.ToList();
            SelectedData = newItem;
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
                    o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DataList.Any()), o => DoEditCompleted()
                    );
            }
        }

        private void DoEdit()
        {
            //執行編輯動作
        }

        private void DoEditCompleted()
        {
            EditAction();
            UserOperateMode = OperateMode.Edit;
        }
        #endregion Edit

        #region Cancel
        public ICommand CancelCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                                    o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => DoCancelCompleted()
                                    );
            }
        }

        private void DoCancel()
        {
           
        }

        private void DoCancelCompleted()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
                var resetSelectedData = false;
                if (UserOperateMode == OperateMode.Add && DataList.Any())
                {
                    //DataList.RemoveAt(DataList.Count - 1);
                    resetSelectedData = true;
                }

                UserOperateMode = OperateMode.Query;
                if (resetSelectedData && DataList.Any())
                    SelectedData = DataList.First();
                SearchAction();

                DoSearch();
            }
            else
            {
                if (UserOperateMode == OperateMode.Edit)
                    DoEditCompleted();
                else
                {
                    //UserOperateMode = OperateMode.Query;
                    //DoAddCompleted();
                }

            }
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
                    o => DoDeleteCompleted()
                    );
            }
        }

        private void DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;

            var pier = (from a in _proxy.F910003s
                        where a.ITEM_TYPE_ID.Equals(SelectedData.ITEM_TYPE_ID) 
                        select a).FirstOrDefault();
            if (pier == null)
            {
                DialogService.ShowMessage(Properties.Resources.P1920030000_QuoteItemTypeDataNull);
                return;
            }
            else
            {
                _proxy.DeleteObject(pier);
            }
            _proxy.SaveChanges();
            ShowMessage(Messages.DeleteSuccess);
        }

        private void DoDeleteCompleted()
        {
            UserOperateMode = OperateMode.Query;
            SearchCommand.Execute(null);
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSave(), () => UserOperateMode != OperateMode.Query, p => DoSaveCompleted()
                    );
            }
        }

        private void DoSave()
        {
            isValid = true;

            //
            if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
            {
                isValid = false;
                return;
            }

            if (string.IsNullOrEmpty(SelectedData.ITEM_TYPE_ID) || string.IsNullOrWhiteSpace(SelectedData.ITEM_TYPE_ID))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920030000_InputAccUnitNo);
                return;
            }

           
            //if (IsValidPireCode(SelectedData.UNIT_ID) == false)
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_PierValidateCNWord);
            //    return;
            //}

            if (UserOperateMode == OperateMode.Add)
            {
                var pier = _proxy.F910003s.Where(x => x.ITEM_TYPE_ID.ToLower().Equals(SelectedData.ITEM_TYPE_ID.ToLower())).AsQueryable().ToList().Count();
                if (pier != 0)
                {
                    isValid = false;
                    DialogService.ShowMessage(Properties.Resources.P1920030000_ItemTypeNo_Duplicate);
                    return;
                }
            }

            if (string.IsNullOrEmpty(SelectedData.ITEM_TYPE) || string.IsNullOrWhiteSpace(SelectedData.ITEM_TYPE))
            {
                isValid = false;
                DialogService.ShowMessage(Properties.Resources.P1920020000_InputItemType);
                return;
            }
            //if (IsValidTempArea(SelectedData.TEMP_AREA.ToString()) == false)
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustGreaterThanZero);
            //    return;
            //}
            //if (SelectedData.TEMP_AREA < 1)
            //{
            //	isValid = false;
            //	DialogService.ShowMessage(Properties.Resources.P1901250000_TmpMustPositive);
            //	return;				
            //}
            //if ((SelectedData.ALLOW_IN == "0" && SelectedData.ALLOW_OUT == "0") || (SelectedData.ALLOW_IN == null && SelectedData.ALLOW_OUT == null))
            //{
            //    isValid = false;
            //    DialogService.ShowMessage(Properties.Resources.P1901250000_AtleastCheckOne);
            //    return;
            //}
            //執行確認儲存動作
            if (UserOperateMode == OperateMode.Add)
                DoSaveAdd();
            else if (UserOperateMode == OperateMode.Edit)
                DoSaveEdit();
        }

        private void DoSaveAdd()
        {
            var F910003 = new F910003();
            //var _AllowIn = SelectedData.ALLOW_IN;
            //var _AllowOut = SelectedData.ALLOW_OUT;
            //if (_AllowIn == null) { _AllowIn = "0"; }
            //if (_AllowOut == null) { _AllowOut = "0"; }
            F910003.ITEM_TYPE_ID = SelectedData.ITEM_TYPE_ID;
            F910003.ITEM_TYPE = SelectedData.ITEM_TYPE.Trim();

            _proxy.AddToF910003s(F910003);
            _proxy.SaveChanges();
            ShowMessage(Messages.Success);
        }

        private void DoSaveEdit()
        {
            var F910003s = _proxy.F910003s.Where(x => x.ITEM_TYPE_ID == SelectedData.ITEM_TYPE_ID).AsQueryable().ToList();
            var F910003 = F910003s.FirstOrDefault();

            if (F910003 != null)
            {
                F910003.ITEM_TYPE = SelectedData.ITEM_TYPE;
                F910003.UPD_STAFF = _userId;
                F910003.UPD_NAME = _userName;
                F910003.UPD_DATE = DateTime.Now;
                _proxy.UpdateObject(F910003);
                _proxy.SaveChanges();
                ShowMessage(Messages.Success);
            }
            else
            {
                ShowMessage(Messages.Failed);
            }
        }

        private void DoSaveCompleted()
        {
            if (isValid == true)
            {
                UserOperateMode = OperateMode.Query;
                SearchCommand.Execute(null);
            }
        }
        #endregion Save

        public bool IsValidPireCode(String pireCode)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg1.IsMatch(pireCode);
        }
        public bool IsValidTempArea(String tempArea)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
            return reg1.IsMatch(tempArea);
        }

    }
}
