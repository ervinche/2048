using Game2048.Game.Models;

namespace Game2048.Game.Services.Interfaces
{
    /// <summary>
    /// Game interactor.
    /// </summary>
    public interface IGameInteractor
    {        
        /// <summary>
        /// Continues the game.
        /// </summary>
        void Continue();

        /// <summary>
        /// Moves into a direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        void Move(MoveDirection direction);

    }
}
