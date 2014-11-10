namespace Game2048.Game.Solver
{
    public class HeuristicWeight
    {
        #region Properties

        /// <summary>
        /// Gets or sets the penalty.
        /// </summary>
        /// <value>
        /// The penalty.
        /// </value>
        public double Penalty { get; set; }

        /// <summary>
        /// Gets or sets the mono power.
        /// </summary>
        /// <value>
        /// The mono power.
        /// </value>
        public double MonoPower { get; set;}

        /// <summary>
        /// Gets or sets the mono weight.
        /// </summary>
        /// <value>
        /// The mono weight.
        /// </value>
        public double MonoWeight { get; set; }

        /// <summary>
        /// Gets or sets the sum power.
        /// </summary>
        /// <value>
        /// The sum power.
        /// </value>
        public double SumPower { get; set; }

        /// <summary>
        /// Gets or sets the sum weight.
        /// </summary>
        /// <value>
        /// The sum weight.
        /// </value>
        public double SumWeight { get; set; }

        /// <summary>
        /// Gets or sets the merge weight.
        /// </summary>
        /// <value>
        /// The merge weight.
        /// </value>
        public double MergeWeight { get; set; }

        /// <summary>
        /// Gets or sets the empty weight.
        /// </summary>
        /// <value>
        /// The empty weight.
        /// </value>
        public double EmptyWeight { get; set; }

        #endregion
    }
}
