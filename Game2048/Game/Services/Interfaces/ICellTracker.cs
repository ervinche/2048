using System;

namespace Game2048.Game.Services.Interfaces
{
    /// <summary>
    /// Cell tracker.
    /// </summary>
    public interface ICellTracker
    {
        /// <summary>
        /// Gets or sets the new cell action.
        /// </summary>
        /// <value>
        /// The new cell action.
        /// </value>
        Action<int> NewCellAction
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
        Action WinAction
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the cell tracker.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Registers game state.
        /// </summary>
        /// <param name="gameStateValue">The game state.</param>
        void RegisterState(ulong gameStateValue);
    }
}
