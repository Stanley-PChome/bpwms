﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wms3pl.WpfClient.ExDataServices.S16WcfService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OrderAllotParam", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.Entities")]
    [System.SerializableAttribute()]
    public partial class OrderAllotParam : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CustCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DcCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExecEDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExecSDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GupCodeField;
        
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
        public string CustCode {
            get {
                return this.CustCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.CustCodeField, value) != true)) {
                    this.CustCodeField = value;
                    this.RaisePropertyChanged("CustCode");
                }
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
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExecEDate {
            get {
                return this.ExecEDateField;
            }
            set {
                if ((object.ReferenceEquals(this.ExecEDateField, value) != true)) {
                    this.ExecEDateField = value;
                    this.RaisePropertyChanged("ExecEDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExecSDate {
            get {
                return this.ExecSDateField;
            }
            set {
                if ((object.ReferenceEquals(this.ExecSDateField, value) != true)) {
                    this.ExecSDateField = value;
                    this.RaisePropertyChanged("ExecSDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GupCode {
            get {
                return this.GupCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.GupCodeField, value) != true)) {
                    this.GupCodeField = value;
                    this.RaisePropertyChanged("GupCode");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ExecuteResult", Namespace="http://schemas.datacontract.org/2004/07/Wms3pl.Datas.Shared.Entities")]
    [System.SerializableAttribute()]
    public partial class ExecuteResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NoField;
        
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
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string No {
            get {
                return this.NoField;
            }
            set {
                if ((object.ReferenceEquals(this.NoField, value) != true)) {
                    this.NoField = value;
                    this.RaisePropertyChanged("No");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="S16WcfService.S16WcfService")]
    public interface S16WcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S16WcfService/ExecReturnAllot", ReplyAction="http://tempuri.org/S16WcfService/ExecReturnAllotResponse")]
        Wms3pl.WpfClient.ExDataServices.S16WcfService.ExecuteResult ExecReturnAllot(Wms3pl.WpfClient.ExDataServices.S16WcfService.OrderAllotParam orderAllotParam);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/S16WcfService/ExecReturnAllot", ReplyAction="http://tempuri.org/S16WcfService/ExecReturnAllotResponse")]
        System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S16WcfService.ExecuteResult> ExecReturnAllotAsync(Wms3pl.WpfClient.ExDataServices.S16WcfService.OrderAllotParam orderAllotParam);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface S16WcfServiceChannel : Wms3pl.WpfClient.ExDataServices.S16WcfService.S16WcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class S16WcfServiceClient : System.ServiceModel.ClientBase<Wms3pl.WpfClient.ExDataServices.S16WcfService.S16WcfService>, Wms3pl.WpfClient.ExDataServices.S16WcfService.S16WcfService {
        
        public S16WcfServiceClient() {
        }
        
        public S16WcfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public S16WcfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S16WcfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public S16WcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Wms3pl.WpfClient.ExDataServices.S16WcfService.ExecuteResult ExecReturnAllot(Wms3pl.WpfClient.ExDataServices.S16WcfService.OrderAllotParam orderAllotParam) {
            return base.Channel.ExecReturnAllot(orderAllotParam);
        }
        
        public System.Threading.Tasks.Task<Wms3pl.WpfClient.ExDataServices.S16WcfService.ExecuteResult> ExecReturnAllotAsync(Wms3pl.WpfClient.ExDataServices.S16WcfService.OrderAllotParam orderAllotParam) {
            return base.Channel.ExecReturnAllotAsync(orderAllotParam);
        }
    }
}
