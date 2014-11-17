using Game2048.Game.Solver.Interfaces;
using System.Configuration;

namespace Game2048.Game.Solver
{
    /// <summary>
    /// Heuristic weights provider.
    /// </summary>
    public class HeuristicWeightProvider : IHeuristicWeightProvider
    {
        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <returns>Returns weights.</returns>
        public HeuristicWeight GetWeight()
        {
            return new HeuristicWeight()
            {
                Penalty = double.Parse(ConfigurationManager.AppSettings["Penalty"]),
                MonoPower = double.Parse(ConfigurationManager.AppSettings["MonoPower"]),
                MonoWeight = double.Parse(ConfigurationManager.AppSettings["MonoWeight"]),
                SumPower = double.Parse(ConfigurationManager.AppSettings["SumPower"]),
                SumWeight = double.Parse(ConfigurationManager.AppSettings["SumWeight"]),
                MergeWeight = double.Parse(ConfigurationManager.AppSettings["MergeWeight"]),
                EmptyWeight = double.Parse(ConfigurationManager.AppSettings["EmptyWeight"])
            };
        }
    }
}
