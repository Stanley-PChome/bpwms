using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib.Controls;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.UILib
{
  public class Wms3plPage: Page
  {
    private bool _hasBeenLoaded;

    private Function _function;
    private Wms3plContainer _Wms3plContainer;

    public Wms3plPage()
    {
      //套用 User Control 的 style
      if (!DesignerProperties.GetIsInDesignMode(this))
        Style = FindResource("PageStyle") as Style;

      Loaded += WindowLoaded;
      var classTypeName = this.GetType().Name.Substring(1);
      _function = FormService.GetFunctionFromSession(classTypeName);

      //_Wms3plContainer = new Wms3plContainer(_function);
      var container = new UnityContainer();
      var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
      section.Configure(container, "");
      _dialogService = container.Resolve<IDialogService>(
        new ParameterOverride("View", this)
        );

    }

    public Wms3plPage(Function function)
      : this()
    {
      _function = function;
      //_Wms3plContainer = new Wms3plContainer(_function);
    }


    public Function Function
    {
      get { return _function; }
      set
      {
        _function = value;
        if (_function != null)
          SetFunctionAndTitle();
      }
    }

    private void SetFunctionAndTitle()
    {
      var master = FindName("Master") as BasicMaster;
      if (master != null)
      {
        master.FunctionId = _function.Id;
        master.FunctionName = _function.Name;
				master.PositionName = ConfigurationManager.AppSettings["PositionName"].ToString();
				if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
					master.VersionNo = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
				else
					master.VersionNo  = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			
      }

      SetTitle();
    }

    //由Function 製作 title 的路徑
    private void SetTitle()
    {
      var titles = new List<string>();
      var currentFuntion = _function;
      do
      {
        titles.Add(currentFuntion.Name);
        currentFuntion = currentFuntion.Parent;
      } while (currentFuntion != null);
      titles.Reverse();
      this.Title = string.Join(" \\ ", titles.ToArray());
    }


    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      if (!DesignerProperties.GetIsInDesignMode(this))
      {
        if (!_hasBeenLoaded)
        {
          _hasBeenLoaded = true;
          _Wms3plContainer = new Wms3plContainer(Function);
          
          if (_function != null)
          {
            //check PermissionService.CheckPermission
            var needCheck = (bool) GetValue(PermissionService.CheckPermissionProperty);
            if (needCheck) CheckPermission();
            SetFunctionAndTitle();
          }
        }
      }
    }

    /// <summary>
    /// 檢查使用者是否可以使用該Window
    /// </summary>
    /// <returns>true: 可以使用</returns>
    private void CheckPermission()
    {
      bool userCanExecute = _Wms3plContainer.CheckPermission();
      if (!userCanExecute)
      {
        MessageBox.Show("您並沒有權限使用該功能，請向管理員連絡。");
        IsEnabled = false;
      }
    }

    public void LogUsage(string message)
    {
      _Wms3plContainer.LogUsage(message);
    }


    /// <summary>
    /// </summary>
    /// <param name = "action"></param>
    /// <param name = "clearMessage">是否清除訊息</param>
    protected void SmartAction(Action action, bool clearMessage = false)
    {
      try
      {
        action();
        if (clearMessage) ClearMessage();
      }
      catch (Exception e)
      {
				ErrorHandleHelper.HandleException(e);
        //ShowError(e);
      }
    }

    protected void AsyncAction(Action<object> action, Action<object> completed, bool clearMessage = false)
    {
      var command = new AsyncDelegateCommand(action,
                                             () => true, (o) =>
                                                           {
                                                             completed(o);
                                                             if (clearMessage) ClearMessage();
                                                           }, e =>
                                                                {
																																	ErrorHandleHelper.HandleException(e);
                                                                  //ShowError(e);
                                                                });
      command.Execute(null);
    }


    protected void ClearMessage()
    {
      //this.ShowMessage("", false);
    }

    protected object OpenWindow(string functionId, params object[] parameters)
    {
      var formService = new FormService();
      return formService.AddFunctionForm(functionId, this.Parent as Window, parameters);
    }

    protected void FocusElement(string name)
    {
      var control = (FindName(name) as Control);
      if (control != null) control.Focus();
    }

    private IDialogService _dialogService;
    public IDialogService DialogService
    {

      get
      {
        return _dialogService;
      }
    }
    
  }

  public static class Wms3plPageExtension
  {
    public static void ShowStatusMessage(this Wms3plPage userControl, string message)
    {
      Messenger.Default.Send<NotificationMessage<string>>(
        new NotificationMessage<string>(userControl, message, "Show"));
    }
  }
}