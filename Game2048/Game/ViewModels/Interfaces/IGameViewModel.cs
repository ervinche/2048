using System.Windows.Input;

namespace Game2048.Game.ViewModels.Interfaces
{
    /// <summary>
    /// Game view model.
    /// </summary>
    public interface IGameViewModel
    {
        /// <summary>
        /// Initializes the game.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts the game.
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the game.
        /// </summary>
        void Pause();

        /// <summary>
        /// Continues the game.
        /// </summary>
        void Continue();

        /// <summary>
        /// Finishes the game.
        /// </summary>
        void Finish();

        /// <summary>
        /// Start command.
        /// </summary>
        ICommand StartCommand { get; }

        /// <summary>
        /// Gets if start command can be executed.
        /// </summary>
        bool CanStart { get; }

        /// <summary>
        /// Pause command.
        /// </summary>
        ICommand PauseCommand { get; }

        /// <summary>
        /// Gets if pause command can be executed.
        /// </summary>
        bool CanPause { get; }

        /// <summary>
        /// Continue command.
        /// </summary>
        ICommand ContinueCommand { get; }

        /// <summary>
        /// Gets if continue command can be executed.
        /// </summary>
        bool CanContinue { get; }

        /// <summary>
        /// Finish command.
        /// </summary>
        ICommand FinishCommand { get; }

        /// <summary>
        /// Gets if finish command can be executed.
        /// </summary>
        bool CanFinish { get; }
    }
}
