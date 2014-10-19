using System.Windows.Input;

namespace Game2048.Game.ViewModels.Interfaces
{
    public interface IGameViewModel
    {
        bool CanContinue { get; }
        bool CanFinish { get; }
        bool CanPause { get; }
        bool CanStart { get; }
        void Continue();
        ICommand ContinueCommand { get; }
        void Finish();
        ICommand FinishCommand { get; }
        void Initialize();
        void Pause();
        ICommand PauseCommand { get; }
        void Start();
        ICommand StartCommand { get; }
    }
}
