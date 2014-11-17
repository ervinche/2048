using Game2048.Game.Models;

namespace Game2048.Game.Services.Interfaces
{
    /// <summary>
    /// Game state retriever.
    /// </summary>
    public interface IGameStateRetriever
    {
        /// <summary>
        /// Transforms game state value.
        /// </summary>
        /// <param name="gameState">The game state.</param>
        /// <returns>Returns game state value represented as long.</returns>
        ulong TransformStateValue(GameState gameState);

        /// <summary>
        /// Gets the state of the game.
        /// </summary>
        /// <returns>Returns the game state.</returns>
        GameState GetState();
    }
}
