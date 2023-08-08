using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wms3pl.WpfClient.UILib
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. The default return value for the
	/// CanExecute method is 'true'.
	/// </summary>
	public class AsyncRelayCommand : ICommand
	{
		#region Fields

		readonly Action<object> _execute;
		readonly Action<object> _finallyAction;
		readonly Action<object> _completed;
		readonly Func<object, bool> _preFunc;
		readonly Func<object, bool> _canExecute;

		#endregion // Fields

		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public AsyncRelayCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="execute">�D�P�B���ʧ@</param>
		/// <param name="canExecute">���ʧ@�O�_�i�H����</param>
		/// <param name="finallyAction">����D�P�B���ʧ@�� finally ���n����ʧ@</param>
		/// <param name="completed">���\�᪺�ʧ@�C�q�`�O��sUI</param>
		/// <param name="preFunc">����D�P�B�ʧ@�e���ʧ@�C�q�`���ǳƫD�P�B�ʧ@�ҥΪ����</param>
		public AsyncRelayCommand(Action<object> execute, Func<object, bool> canExecute, Action<object> finallyAction = null,
			Action<object> completed = null,
			Func<object, bool> preFunc = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_finallyAction = finallyAction;
			_completed = completed;
			_preFunc = preFunc;
			_canExecute = canExecute;
		}

		#endregion // Constructors

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameters)
		{
			return _canExecute == null ? true : _canExecute(parameters);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameters)
		{
			ExecuteAsync(parameters);
		}

		private async void ExecuteAsync(object parameters)
		{
			try
			{
				var preActionResult = true;
				if (_preFunc != null)
					preActionResult = _preFunc(parameters);
				if (!preActionResult)
					return;
				await Task.Run(() => _execute(parameters));
				if (_completed != null)
					_completed(parameters);
			}
			finally
			{
				if (_finallyAction != null)
					_finallyAction(parameters);
			}
		}

		#endregion // ICommand Members
	}

	public class AsyncRelayCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> _execute;
		readonly Action<T> _finallyAction;
		readonly Action<T> _completed;
		readonly Func<T, bool> _preFunc;
		readonly Func<T, bool> _canExecute;

		#endregion // Fields

		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public AsyncRelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="execute">�D�P�B���ʧ@</param>
		/// <param name="canExecute">���ʧ@�O�_�i�H����</param>
		/// <param name="finallyAction">����D�P�B���ʧ@�� finally ���n����ʧ@</param>
		/// <param name="completed">���\�᪺�ʧ@�C�q�`�O��sUI</param>
		/// <param name="preFunc">����D�P�B�ʧ@�e���ʧ@�C�q�`���ǳƫD�P�B�ʧ@�ҥΪ����</param>
		public AsyncRelayCommand(Action<T> execute, Func<T, bool> canExecute, Action<T> finallyAction = null,
			Action<T> completed = null,
			Func<T, bool> preFunc = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_finallyAction = finallyAction;
			_completed = completed;
			_preFunc = preFunc;
			_canExecute = canExecute;
		}

		#endregion // Constructors

		#region ICommand Members

		[DebuggerStepThrough]
		public bool CanExecute(object parameters)
		{
			return _canExecute == null ? true : _canExecute((T)parameters);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameters)
		{
			ExecuteAsync((T)parameters);
		}

		private async void ExecuteAsync(T parameters)
		{
			try
			{
				var preActionResult = true;
				if (_preFunc != null)
					preActionResult = _preFunc(parameters);
				if (!preActionResult)
					return;
				await Task.Run(() => _execute(parameters));
				if (_completed != null)
					_completed(parameters);
			}
			finally
			{
				if (_finallyAction != null)
					_finallyAction(parameters);
			}
		}

		#endregion // ICommand Members
	}
}
