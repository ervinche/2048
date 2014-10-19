using Game2048.Game.Models;

namespace Game2048.Game.Solver.Interfaces
{
    public interface IGameSolver
    {
        void InitHeuristics();

        MoveDirection GetBestMove(ulong board);

        byte GetMaxRank(ulong board);

        void SetDepth(int depth);
    }
}
