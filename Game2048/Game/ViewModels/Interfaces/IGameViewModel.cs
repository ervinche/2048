using System.Windows.Input;

namespace Game2048.Game.ViewModels.Interfaces
{
    public interface IGameViewModel
    {
        void Initialize();
        void Start();
        void Pause();
        void Continue();
        void Finish();
        ICommand StartCommand { get; }
        bool CanStart { get; }
        ICommand PauseCommand { get; }
        bool CanPause { get; }
        ICommand ContinueCommand { get; }
        bool CanContinue { get; }
        ICommand FinishCommand { get; }
        bool CanFinish { get; }
    }
}
