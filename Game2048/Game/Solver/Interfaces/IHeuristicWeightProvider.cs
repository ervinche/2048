namespace Game2048.Game.Solver.Interfaces
{
    /// <summary>
    /// Heuristic weights provider.
    /// </summary>
    public interface IHeuristicWeightProvider
    {
        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <returns>Returns weights.</returns>
        HeuristicWeight GetWeight();
    }
}
