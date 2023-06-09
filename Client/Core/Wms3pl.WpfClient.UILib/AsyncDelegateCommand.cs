using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib
{
  public class AsyncDelegateCommand : ICommand
  {
    BackgroundWorker _worker = new BackgroundWorker();
    Func<bool> _canExecute;

    public Action PreAction = delegate { };
    public Action<Exception> ErrorAction = delegate { };

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="action">非同步的動作</param>
    /// <param name="canExecute">此動作是否可以執行</param>
    /// <param name="completed">成功後的動作。通常是更新UI</param>
    /// <param name="error">失敗後的動作。</param>
    /// <param name="preAction">執行非同步動作前的動作。通常為準備非同步動作所用的資料</param>
    public AsyncDelegateCommand(Action<object> action,
                                Func<bool> canExecute = null,
                                Action<object> completed = null,
                                Action<Exception> error = null,
      Action preAction = null)
    {

      if (preAction != null) PreAction = preAction;
      if (error != null) ErrorAction = error;
      
      _worker.DoWork += (s, e) =>
      {
        CommandManager.InvalidateRequerySuggested();
        action(e.Argument);
      };

      _worker.RunWorkerCompleted += (s, e) =>
      {
        if (completed != null && e.Error == null)
          completed(e.Result);

				if (error != null && e.Error != null)
				{
					error(e.Error);
				}

				if (e.Error != null)
				{
					ErrorHandleHelper.HandleException(e.Error);
				}

        CommandManager.InvalidateRequerySuggested();
      };

      _canExecute = canExecute;
    }


    /// <summary>
    /// 取消正在執行的動作
    /// </summary>
    public void Cancel()
    {
      if (_worker.IsBusy)
        _worker.CancelAsync();
    }

    public bool IsBusy
    {
      get { return _worker.IsBusy; }
    }

    /// <summary>
    /// 如果正在執行的話，會回傳 false
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object parameter)
    {
      return (_canExecute == null) ?
              !(_worker.IsBusy) : !(_worker.IsBusy)
                  && _canExecute();
    }

    /// <summary>
    /// for thread safe
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// 開始執行
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object parameter)
    {
      try
      {
        if (PreAction != null) PreAction();
      }
      catch (Exception ex)
      {
				ErrorHandleHelper.HandleException(ex);
        ErrorAction(ex);
      }

      if (!_worker.IsBusy)
        _worker.RunWorkerAsync(parameter);
    }


  }
}
