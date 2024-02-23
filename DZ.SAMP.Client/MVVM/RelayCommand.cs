using System;
using System.Diagnostics;
using System.Windows.Input;

namespace DZ.SAMP.Client.MVVM
{
    public class RelayCommand : RelayCommand<object>
    {
        #region Constructors

        public RelayCommand(Action<object> execute)
            : base(execute, null)
        {
        }

        public RelayCommand(Action execute)
            : base(o => execute(), null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : base(o => execute(), o => canExecute())
        {
        }
        #endregion // Constructors
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        protected RelayCommand() { }

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public virtual void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion // ICommand Members
    }
}
