using Game2048.Game.Models;

namespace Game2048.Game.Services.Interfaces
{
    public interface IGameStateRetriever
    {
        ulong TransformStateValue(GameState gameState);

        GameState GetState();
    }
}
