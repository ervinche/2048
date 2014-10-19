using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Infrastructure.Interfaces;

namespace Game2048.Game.Services
{
    public class GameInteractor : IGameInteractor
    {
        private IBrowserManager browserManager;

        public GameInteractor(IBrowserManager browserManager)
        {
            this.browserManager = browserManager;
        }        

        public void Continue()
        {
            browserManager.ClickControlClass("keep-playing-button");
        }

        public void Move(MoveDirection direction)
        {
            browserManager.SendKey((direction + 37).ToString());
        }
    }
}
