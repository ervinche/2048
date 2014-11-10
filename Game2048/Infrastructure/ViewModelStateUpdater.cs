using Game2048.Infrastructure.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Game2048.Infrastructure
{
    public class ViewModelStateUpdater : IViewModelStateUpdater
    {
        /// <summary>
        /// Updates the states.
        /// </summary>
        public void UpdateStates()
        {            
            CommandManager.InvalidateRequerySuggested();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));            
        }
    }
}
