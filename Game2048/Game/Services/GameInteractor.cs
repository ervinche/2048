using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Infrastructure.Interfaces;

namespace Game2048.Game.Services
{
    /// <summary>
    /// Game interactor service.
    /// </summary>
    public class GameInteractor : IGameInteractor
    {
        #region Fields

        private IBrowserManager browserManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameInteractor"/> class.
        /// </summary>
        /// <param name="browserManager">The browser manager.</param>
        public GameInteractor(IBrowserManager browserManager)
        {
            this.browserManager = browserManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Continues the game.
        /// </summary>
        public void Continue()
        {
            browserManager.ClickControlClass("keep-playing-button");
        }

        /// <summary>
        /// Moves into a direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        public void Move(MoveDirection direction)
        {
            browserManager.SendKey((direction + 37).ToString());
        }

        #endregion
    }
}
