using System;
using System.Collections.Generic;
using Game2048.Game.Services.Interfaces;
using Game2048.Game.Solver.Interfaces;

namespace Game2048.Game.Services
{
    /// <summary>
    /// Cell tracker service.
    /// </summary>
    public class CellTracker : ICellTracker
    {
        #region Fields

        private List<int> moves = new List<int>();
        private IGameSolver gameSolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CellTracker"/> class.
        /// </summary>
        /// <param name="gameSolver">The game solver.</param>
        public CellTracker(IGameSolver gameSolver)
        {
            this.gameSolver = gameSolver;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the new cell action.
        /// </summary>
        /// <value>
        /// The new cell action.
        /// </value>
        public Action<int> NewCellAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the win action.
        /// </summary>
        /// <value>
        /// The win action.
        /// </value>
        public Action WinAction
        {
            get;
            set;
        }

        #endregion        

        #region Methods

        /// <summary>
        /// Initializes the cell tracker.
        /// </summary>
        public void Initialize()
        {
            moves.Clear();
        }

        /// <summary>
        /// Registers game state.
        /// </summary>
        /// <param name="gameStateValue">The game state.</param>
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

        #endregion
    }
}
