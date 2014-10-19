namespace Game2048.Game.Solver
{
    public class HeuristicWeight
    {
        public double Penalty { get; set; }

        public double MonoPower { get; set;}

        public double MonoWeight { get; set; }

        public double SumPower { get; set; }

        public double SumWeight { get; set; }

        public double MergeWeight { get; set; }

        public double EmptyWeight { get; set; }

        private const double SCORE_LOST_PENALTY = 200000;
        private const double SCORE_MONOTONICITY_POWER = 4.0f;
        private const double SCORE_MONOTONICITY_WEIGHT = 10.0f;
        private const double SCORE_SUM_POWER = 3.5f;
        private const double SCORE_SUM_WEIGHT = 11.9f;
        private const double SCORE_MERGES_WEIGHT = 760.0f;
        private const double SCORE_EMPTY_WEIGHT = 590f;
    }
}
