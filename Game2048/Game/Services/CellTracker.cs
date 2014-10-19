using Game2048.Game.Services.Interfaces;
using Game2048.Game.Solver.Interfaces;
using System;
using System.Collections.Generic;

namespace Game2048.Game.Services
{
    public class CellTracker : ICellTracker
    {
        private List<int> moves = new List<int>();
        private IGameSolver gameSolver;

        public CellTracker(IGameSolver gameSolver)
        {
            this.gameSolver = gameSolver;
        }        

        public void RegisterState(ulong gameStateValue)
        {            
            int max = gameSolver.GetMaxRank(gameStateValue);
            if (!moves.Contains(max))
            {
                moves.Add(max);
                if (max == 11)
                {
                    if (WinAction != null)
                    {
                        WinAction();
                    }
                }
                else
                {
                    if (NewCellAction != null)
                    {
                        NewCellAction(max);
                    }
                }
            }
        }

        public Action<int> NewCellAction
        {
            get;
            set;
        }

        public Action WinAction
        {
            get;
            set;
        }

        public void Initialize()
        {
            moves.Clear();
        }
    }
}
