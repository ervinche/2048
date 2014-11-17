using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Game2048.Infrastructure
{
    /// <summary>
    /// Relay command
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The action to execute</param>
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Evaluats execution state.
        /// </summary>
        /// <param name="parameter">The parrameter.</param>
        /// <returns>Returns true if the command can be executed.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Adds or removes the CanExecuteChanged event.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the command with a parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Updates execution state.
        /// </summary>
        public void UpdateCanExecuteState()
        {
            CommandManager.InvalidateRequerySuggested();
        }


        #endregion
    }

}