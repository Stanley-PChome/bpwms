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
    /// �غc�l
    /// </summary>
    /// <param name="action">�D�P�B���ʧ@</param>
    /// <param name="canExecute">���ʧ@�O�_�i�H����</param>
    /// <param name="completed">���\�᪺�ʧ@�C�q�`�O��sUI</param>
    /// <param name="error">���ѫ᪺�ʧ@�C</param>
    /// <param name="preAction">����D�P�B�ʧ@�e���ʧ@�C�q�`���ǳƫD�P�B�ʧ@�ҥΪ����</param>
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
    /// �������b���檺�ʧ@
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
    /// �p�G���b���檺�ܡA�|�^�� false
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
    /// �}�l����
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
