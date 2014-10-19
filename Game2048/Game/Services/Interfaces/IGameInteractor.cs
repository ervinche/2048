using Game2048.Game.Models;

namespace Game2048.Game.Services.Interfaces
{
    public interface IGameInteractor
    {        
        void Continue();

        void Move(MoveDirection direction);

    }
}
