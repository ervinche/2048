using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Infrastructure.Interfaces;

namespace Game2048.Game.Services
{
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
        /// Continues this instance.
        /// </summary>
        public void Continue()
        {
            browserManager.ClickControlClass("keep-playing-button");
        }

        /// <summary>
        /// Moves the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void Move(MoveDirection direction)
        {
            browserManager.SendKey((direction + 37).ToString());
        }

        #endregion
    }
}
