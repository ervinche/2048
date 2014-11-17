using Game2048.Game.Models;

namespace Game2048.Game.Solver.Interfaces
{
    /// <summary>
    /// Game solver.
    /// </summary>
    public interface IGameSolver
    {
        /// <summary>
        /// Initializes the heuristics.
        /// </summary>
        void InitHeuristics();

        /// <summary>
        /// Gets the best move.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        MoveDirection GetBestMove(ulong board);

        /// <summary>
        /// Gets the maximum rank.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        byte GetMaxRank(ulong board);

        /// <summary>
        /// Sets the depth of the search tree.
        /// </summary>
        /// <param name="depth"></param>
        void SetDepth(int depth);
    }
}
