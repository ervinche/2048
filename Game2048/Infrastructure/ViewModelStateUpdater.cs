using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Game2048.Infrastructure.Interfaces;

namespace Game2048.Infrastructure
{
    /// <summary>
    /// View model state updater class
    /// </summary>
    public class ViewModelStateUpdater : IViewModelStateUpdater
    {
        /// <summary>
        /// Updates the states.
        /// </summary>
        public void UpdateStates()
        {
            CommandManager.InvalidateRequerySuggested();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
        }
    }
}
