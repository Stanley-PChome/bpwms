﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wms3pl.WpfClient.ExDataServices.S19WcfService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ExecUpdateLocVolumnParam", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.Entities")]
    [System.SerializableAttribute()]
    public partial class ExecUpdateLocVolumnParam : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DcCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DcCode {
            get {
                return this.DcCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.DcCodeField, value) != true)) {
                    this.DcCodeField = value;
                    this.RaisePropertyChanged("DcCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ApiResult", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.ApiEntities")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Wms3pl.WpfClient.ExDataServices.S19WcfService.ExecUpdateLocVolumnParam))]
    public partial class ApiResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FailureCntField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int InsertCntField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MsgCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MsgContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SuccessCntField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TotalCntField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int UpdateCntField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FailureCnt {
            get {
                return this.FailureCntField;
            }
            set {
                if ((this.FailureCntField.Equals(value) != true)) {
                    this.FailureCntField = value;
                    this.RaisePropertyChanged("FailureCnt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int InsertCnt {
            get {
                return this.InsertCntField;
            }
            set {
                if ((this.InsertCntField.Equals(value) != true)) {
                    this.InsertCntField = value;
                    this.RaisePropertyChanged("InsertCnt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSuccessed {
            get {
                return this.IsSuccessedField;
            }
            set {
                if ((this.IsSuccessedField.Equals(value) != true)) {
                    this.IsSuccessedField = value;
                    this.RaisePropertyChanged("IsSuccessed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MsgCode {
            get {
                return this.MsgCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.MsgCodeField, value) != true)) {
                    this.MsgCodeField = value;
                    this.RaisePropertyChanged("MsgCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MsgContent {
            get {
                return this.MsgContentField;
            }
            set {
                if ((object.ReferenceEquals(this.MsgContentField, value) != true)) {
                    this.MsgContentField = value;
                    this.RaisePropertyChanged("MsgContent");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SuccessCnt {
            get {
                return this.SuccessCntField;
            }
            set {
                if ((this.SuccessCntField.Equals(value) != true)) {
                    this.SuccessCntField = value;
                    this.RaisePropertyChanged("SuccessCnt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalCnt {
            get {
                return this.TotalCntField;
            }
            set {
                if ((this.TotalCntField.Equals(value) != true)) {
                    this.TotalCntField = value;
                    this.RaisePropertyChanged("TotalCnt");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int UpdateCnt {
            get {
                return this.UpdateCntField;
            }
            set {
                if ((this.UpdateCntField.Equals(value) != true)) {
                    this.UpdateCntField = value;
                    this.RaisePropertyChanged("UpdateCnt");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="S19WcfService.S19WcfService")]
    public interface S19WcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S19WcfService/ExecUpdateLocVolumn", ReplyAction="http://tempuri.org/S19WcfService/ExecUpdateLocVolumnResponse")]
        Wms3pl.WpfClient.ExDataServices.S19WcfService.ApiResult ExecUpdateLocVolumn(Wms3pl.WpfClient.ExDataServices.S19WcfService.ExecUpdateLocVolumnParam param);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S19WcfService/ExecUpdateLocVolumn", ReplyAction="http://tempuri.org/S19WcfService/ExecUpdateLocVolumnResponse")]
        System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S19WcfService.ApiResult> ExecUpdateLocVolumnAsync(Wms3pl.WpfClient.ExDataServices.S19WcfService.ExecUpdateLocVolumnParam param);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface S19WcfServiceChannel : Wms3pl.WpfClient.ExDataServices.S19WcfService.S19WcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class S19WcfServiceClient : System.ServiceModel.ClientBase<Wms3pl.WpfClient.ExDataServices.S19WcfService.S19WcfService>, Wms3pl.WpfClient.ExDataServices.S19WcfService.S19WcfService {
        
        public S19WcfServiceClient() {
        }
        
        public S19WcfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public S19WcfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S19WcfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S19WcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Wms3pl.WpfClient.ExDataServices.S19WcfService.ApiResult ExecUpdateLocVolumn(Wms3pl.WpfClient.ExDataServices.S19WcfService.ExecUpdateLocVolumnParam param) {
            return base.Channel.ExecUpdateLocVolumn(param);
        }
        
        public System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S19WcfService.ApiResult> ExecUpdateLocVolumnAsync(Wms3pl.WpfClient.ExDataServices.S19WcfService.ExecUpdateLocVolumnParam param) {
            return base.Channel.ExecUpdateLocVolumnAsync(param);
        }
    }
}
