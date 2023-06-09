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
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Reflection;
using Wms3pl.WpfClient.P19.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using AutoMapper;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1920090000_ViewModel : InputViewModelBase
    {
        #region "Enum"
        public enum enumOpMode
        {
            /// <summary>
            /// 廠退
            /// </summary>
            F160203,
            /// <summary>
            /// 退貨
            /// </summary>
            F161203,

            None

        }
        #endregion

        #region "Field"
        public enumOpMode OpMode = enumOpMode.None;     
        public Action AddAction = delegate { };
        public Action EditAction = delegate { };
        public Action SearchAction = delegate { };
        #endregion

        #region "Property"

        private string _TYPEID = string.Empty;
        public string TYPEID
        {
            get
            { return this._TYPEID; }
            set
            {
                this._TYPEID = value;
                RaisePropertyChanged("TYPEID");
            }
        }

        private string _TYPENAME = string.Empty;
        public string TYPENAME
        {
            get
            { return this._TYPENAME; }
            set
            {
                this._TYPENAME = value;
                RaisePropertyChanged("TYPENAME");
            }
        }

        private ObservableCollection<F000904> _F000904S_RETURNTYPE;
        public ObservableCollection<F000904> F000904S_RETURNTYPE
        {
            get { return _F000904S_RETURNTYPE; }
            set
            {
                _F000904S_RETURNTYPE = value;               
                RaisePropertyChanged("F000904S_RETURNTYPE");
            }
        }


        private string _RETURNTYPE = string.Empty;
        public string RETURNTYPE
        {
            get
            {
                return this._RETURNTYPE;
            }
            set
            {
                this._RETURNTYPE = value;

                this.F160203_SelectedData = null;
                this.F160203_List = null;

                this.F161203_SelectedData = null;
                this.F161203_List = null;

                this.SetOpMode();
                this.SetGridVisible();
                RaisePropertyChanged("RETURNTYPE");
            }
        }
       
        /// <summary>
        /// 廠退類型設定檔
        /// </summary>
        private F160203 _F160203_SelectedData;
        public F160203 F160203_SelectedData
        {
            get { return _F160203_SelectedData; }
            set
            {
                _F160203_SelectedData = value;
                RaisePropertyChanged("F160203_SelectedData");
            }
        }
        private ObservableCollection<F160203> _F160203_List;
        public ObservableCollection<F160203> F160203_List
        {
            get
            {
                if (_F160203_List == null) _F160203_List = new ObservableCollection<F160203>();
                return _F160203_List;
            }
            set
            {
                _F160203_List = value;
                RaisePropertyChanged("F160203_List");
            }
        }
        private System.Windows.Visibility _F160203_List_Visible;
        public System.Windows.Visibility F160203_List_Visible
        {
            get { return _F160203_List_Visible; }
            set
            {

                _F160203_List_Visible = value;
                RaisePropertyChanged("F160203_List_Visible");
            }
        }

        /// <summary>
        /// 退貨類型設定檔
        /// </summary>
        private F161203 _F161203_SelectedData;
        public F161203 F161203_SelectedData
        {
            get { return _F161203_SelectedData; }
            set
            {
                _F161203_SelectedData = value;
                RaisePropertyChanged("F161203_SelectedData");
            }
        }

        private ObservableCollection<F161203> _F161203_List;
        public ObservableCollection<F161203> F161203_List
        {
            get
            {
                if (_F161203_List == null) _F161203_List = new ObservableCollection<F161203>();
                return _F161203_List;
            }
            set
            {
                _F161203_List = value;
                RaisePropertyChanged("F161203_List");
            }
        }
        private System.Windows.Visibility _F161203_List_Visible;
        public System.Windows.Visibility F161203_List_Visible
        {
            get { return _F161203_List_Visible; }
            set
            {

                _F161203_List_Visible = value;
                RaisePropertyChanged("F161203_List_Visible");
            }
        }

        #endregion

        #region "Constructor"
        public P1920090000_ViewModel()
        {
            if (!IsInDesignMode)
            {

                var F00Proxy = GetProxy<F00Entities>();
				this.F000904S_RETURNTYPE = GetBaseTableService.GetF000904List(FunctionCode, "P1920090000", "RETURN_TYPE").Select(o => new F000904 { NAME = o.Name, VALUE = o.Value }).ToObservableCollection(); 

                this.RETURNTYPE = this.F000904S_RETURNTYPE.First().VALUE;
                this.SetOpMode();
                this.SetGridVisible();

             
            }
        }
        #endregion


        #region "Method"

        public void SetGridVisible()
        {
            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    this.F160203_List_Visible = System.Windows.Visibility.Visible;
                    this.F161203_List_Visible = System.Windows.Visibility.Collapsed;
                    break;
                case enumOpMode.F161203:
                    this.F161203_List_Visible = System.Windows.Visibility.Visible;
                    this.F160203_List_Visible = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    this.F161203_List_Visible = System.Windows.Visibility.Collapsed;
                    this.F160203_List_Visible = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        public void SetOpMode()
        {
            if (enumOpMode.F160203.ToString().Contains(this.RETURNTYPE))
            {
                this.OpMode = enumOpMode.F160203;
            }
            else if (enumOpMode.F161203.ToString().Contains(this.RETURNTYPE))
            {
                this.OpMode = enumOpMode.F161203;
            }
            else
            {
                this.OpMode = enumOpMode.None;
            }
        }
        #endregion

       
        #region Data
       
       
        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => SearchComplate()
                    );
            }
        }

        private void DoSearch()
        {
            int intCount = 0;
            var F16Proxy = GetProxy<F16Entities>();

            this.F161203_SelectedData = null;
            this.F161203_List = null;
            this.F160203_SelectedData = null;
            this.F160203_List = null;

            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    F160203_List = F16Proxy.F160203s.ToObservableCollection();
                    if (!string.IsNullOrWhiteSpace(TYPEID))
                    {
                        F160203_List = F160203_List.Where(x => (x.RTN_VNR_TYPE_ID == TYPEID)).ToObservableCollection();
                    }
                    if (!string.IsNullOrWhiteSpace(TYPENAME))
                    {
                        F160203_List = F160203_List.Where(x => (x.RTN_VNR_TYPE_NAME.Contains(TYPENAME))).ToObservableCollection();
                    }
                    F160203_List = F160203_List.OrderBy(O => O.RTN_VNR_TYPE_ID).ToObservableCollection();
                    intCount = (this.F160203_List == null ? 0 : this.F160203_List.Count);
                    break;
                case enumOpMode.F161203:
                    F161203_List = F16Proxy.F161203s.ToObservableCollection();
                    if (!string.IsNullOrWhiteSpace(this.TYPEID))
                    {
                        F161203_List = F161203_List.Where(x => (x.RTN_TYPE_ID == TYPEID)).ToObservableCollection();
                    }
                    if (!string.IsNullOrWhiteSpace(this.TYPENAME))
                    {
                        F161203_List = F161203_List.Where(x => x.RTN_TYPE_NAME.Contains(this.TYPENAME)).ToObservableCollection();
                    }
                    F161203_List = F161203_List.OrderBy(O => O.RTN_TYPE_ID).ToObservableCollection();
                    intCount = (this.F161203_List == null ? 0 : this.F161203_List.Count);
                    break;
                default:
                    break;
            }

            if (intCount == 0)
            {
                ShowMessage(Messages.InfoNoData);
                return;
            }
        }

        private void SearchComplate()
        {
            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    if (this.F160203_List != null && this.F160203_List.Any())
                        this.F160203_SelectedData = this.F160203_List.First();
                    break;
                case enumOpMode.F161203:
                    if (this.F161203_List != null && this.F161203_List.Any())
                        this.F161203_SelectedData = this.F161203_List.First();
                    break;
                default:
                    break;
            }
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

        private void DoAddCompleted()
        {
            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    F160203 NewF160203 = new F160203() { CRT_DATE = DateTime.Now };
                    this.F160203_List.Add(NewF160203);
                    this.F160203_SelectedData = NewF160203;
                    RaisePropertyChanged("F160203_List");
                    break;
                case enumOpMode.F161203:
                    F161203 NewF161203 = new F161203() { CRT_DATE = DateTime.Now };
                    this.F161203_List.Add(NewF161203);
                    this.F161203_SelectedData = NewF161203;
                    RaisePropertyChanged("F161203_List");
                    break;
                default:
                    break;
            }
            AddAction();
            UserOperateMode = OperateMode.Add;
            //IsAdd = true;
        }

        //private bool _isAdd;
        //public bool IsAdd { get { return _isAdd; } set { _isAdd = value; RaisePropertyChanged("IsAdd"); } }


        private void DoAdd()
        {
            //執行新增動作
        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {

                //return CreateBusyAsyncCommand(
                //    o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DgList.Any()), o => EditComplate()
                //    );

                return CreateBusyAsyncCommand(
                   o => DoEdit(), () => UserOperateMode == OperateMode.Query && EditBefore(), o => EditComplate()
                   );
            }
        }
        private bool EditBefore()
        {
            bool blnEditCondition = false;
            object obj = null;


            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    obj = this.F160203_SelectedData;
                    if (this.F160203_List != null)
                        blnEditCondition = this.F160203_List.Any();
                    break;
                case enumOpMode.F161203:
                    obj = this.F161203_SelectedData;
                    if (this.F161203_List != null)
                        blnEditCondition = this.F161203_List.Any();
                    break;
                default:
                    break;
            }
            blnEditCondition = (obj != null && blnEditCondition);
            return blnEditCondition;
        }

        private void DoEdit()
        {
            //執行編輯動作
        }

        private void EditComplate()
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
                    o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => CancelComplate()
                    );
            }
        }

        private void DoCancel()
        {
            //執行取消動作
        }

        protected void ResetSelectedFirstData()
        {
            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    if (this.F160203_List.Any())
                        this.F160203_SelectedData = this.F160203_List.First();
                    break;
                case enumOpMode.F161203:
                    if (this.F161203_List.Any())
                        this.F161203_SelectedData = this.F161203_List.First();
                    break;
                default:
                    break;
            }
        }

        private void CancelComplate()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {

                DoSearch();

                UserOperateMode = OperateMode.Query;

                this.ResetSelectedFirstData();

                SearchAction();
            }
        }

        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                bool isDeleted = false;

                //return CreateBusyAsyncCommand(
                //    o => isDeleted = DoDelete(),
                //    () => UserOperateMode == OperateMode.Query && SelectedData != null,
                //    o => DelComplate(isDeleted)
                //    );

                return CreateBusyAsyncCommand(
                  o => isDeleted = DoDelete(), () => UserOperateMode == OperateMode.Query && DelBefore(), o => DelComplate(isDeleted)
                  );
            }
        }

        private bool DoDelete()
        {
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                var F16Proxy = GetProxy<F16Entities>();
                string ErrorMessage = string.Empty;
                var obj = (object)null;

                switch (this.OpMode)
                {
                    case enumOpMode.F160203:
                        var F160203Entity = F16Proxy.F160203s.Where(x => x.RTN_VNR_TYPE_ID == this.F160203_SelectedData.RTN_VNR_TYPE_ID).FirstOrDefault();
                        if (F160203Entity == null)
                            ErrorMessage = Properties.Resources.RTN_VNR_TYPE_ID;
                        else
                            obj = F160203Entity;
                        break;
                    case enumOpMode.F161203:
                        var F161203Entity = F16Proxy.F161203s.Where(x => x.RTN_TYPE_ID == this.F161203_SelectedData.RTN_TYPE_ID).FirstOrDefault();
                        if (F161203Entity == null)
                            ErrorMessage = Properties.Resources.RTN_TYPE_ID;
                        else
                            obj = F161203Entity;
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    DialogService.ShowMessage(string.Format(Properties.Resources.P1920090000_Deleted, ErrorMessage));
                    return false;
                }
              
                F16Proxy.DeleteObject(obj);
                F16Proxy.SaveChanges();
                ShowMessage(Messages.InfoDeleteSuccess);
                return true;
            }

            return false;
        }

        private bool DelBefore()
        {
            bool blnDeleteCondition = false;
            object obj = null;

            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    obj = this.F160203_SelectedData;
                    break;
                case enumOpMode.F161203:
                    obj = this.F161203_SelectedData;
                    break;
                default:
                    break;
            }
            blnDeleteCondition = (obj != null);
            return blnDeleteCondition;

        }

        private void DelComplate(bool isDeleted)
        {
            if (isDeleted)
            {
                DoSearch();

                this.ResetSelectedFirstData();

                SearchAction();
            }
        }

        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                bool isSaved = false;
                //return CreateBusyAsyncCommand(
                //    o => isSaved = DoSave(),
                //    () => UserOperateMode != OperateMode.Query && SelectedData != null,
                //    o => SaveComplate(isSaved)
                //    );
                return CreateBusyAsyncCommand(
                    o => isSaved = DoSave(), 
                    () => UserOperateMode != OperateMode.Query && SaveBefore(),
                    o => SaveComplate(isSaved)
                    );
            }
        }

        private bool SaveBefore()
        {
            bool blnSaveCondition = false;
            object obj = null;

            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    obj = this.F160203_SelectedData;
                    break;
                case enumOpMode.F161203:
                    obj = this.F161203_SelectedData;
                    break;
                default:
                    break;
            }
            blnSaveCondition = (obj != null);
            return blnSaveCondition;

        }

        private bool DoSave()
        {
            var F16Proxy = GetProxy<F16Entities>();
            string TypeID = string.Empty;
            string TypeName = string.Empty;
            string ErrorMessageContent = string.Empty;
            string UpdateTypeName = string.Empty;
            var obj = (object)null;

            F160203 F160203Entity = null;
            F161203 F161203Entity = null;

            switch (this.OpMode)
            {
                case enumOpMode.F160203:
                    TypeID = this.F160203_SelectedData.RTN_VNR_TYPE_ID;
                    TypeName = this.F160203_SelectedData.RTN_VNR_TYPE_NAME;
                    F160203Entity = F16Proxy.F160203s.Where(x => x.RTN_VNR_TYPE_ID == this.F160203_SelectedData.RTN_VNR_TYPE_ID).FirstOrDefault();
                  
                    obj = F160203Entity;
                    ErrorMessageContent = Properties.Resources.RTN_VNR_TYPE_ID;
                    break;
                case enumOpMode.F161203:
                    TypeID = this.F161203_SelectedData.RTN_TYPE_ID;
                    TypeName = this.F161203_SelectedData.RTN_TYPE_NAME;
                    F161203Entity = F16Proxy.F161203s.Where(x => x.RTN_TYPE_ID == this.F161203_SelectedData.RTN_TYPE_ID).FirstOrDefault();
                  
                    obj = F161203Entity;
                    ErrorMessageContent = Properties.Resources.RTN_TYPE_ID;
                    break;
                default:
                    break;
            }


            var error = GetEditableError(TypeID, TypeName);

            if (!string.IsNullOrEmpty(error))
            {
                DialogService.ShowMessage(error);
                return false;
            }

            if (UserOperateMode == OperateMode.Add)
            {
                if (obj != null)
                {
                    DialogService.ShowMessage(string.Format(Properties.Resources.P1920090000_Exist, ErrorMessageContent));
                    return false;
                }

                if (this.F160203_SelectedData != null)
                    F16Proxy.AddToF160203s(this.F160203_SelectedData);

                if (this.F161203_SelectedData != null)
                    F16Proxy.AddToF161203s(this.F161203_SelectedData);

            }
            else
            {
                if (obj == null)
                {
                    DialogService.ShowMessage(string.Format(Properties.Resources.P1920090000_Deleted, ErrorMessageContent));
                    return false;
                }

                if (F160203Entity != null)
                    F160203Entity.RTN_VNR_TYPE_NAME = this._F160203_SelectedData.RTN_VNR_TYPE_NAME;

                if (F161203Entity != null)
                    F161203Entity.RTN_TYPE_NAME = this._F161203_SelectedData.RTN_TYPE_NAME;

               
                F16Proxy.UpdateObject(obj);
            }

            F16Proxy.SaveChanges();           
            ShowMessage(Messages.Success);
            return true;
        }

        private void SaveComplate(bool isSaved)
        {
            if (isSaved)
            {
                UserOperateMode = OperateMode.Query;
                DoSearch();
                this.ResetSelectedFirstData();
                SearchAction();
            }
        }

        public string GetEditableError(string TypeID, string TypeName)
        {

            if (string.IsNullOrEmpty(TypeID))
                return Properties.Resources.P1920090000_InputTypeID;

            if (string.IsNullOrEmpty(TypeName))
                return Properties.Resources.P1920090000_TypeName;

            return string.Empty;
        }
        #endregion Save
    }
}
        #endregion