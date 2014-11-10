using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;

namespace Game2048.Game.Services
{
    public class GameStateRetriever : IGameStateRetriever
    {
        #region Fields

        private IBrowserManager browserManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateRetriever"/> class.
        /// </summary>
        /// <param name="browserManager">The browser manager.</param>
        public GameStateRetriever(IBrowserManager browserManager)
        {
            this.browserManager = browserManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Transforms the state value.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        /// <returns></returns>
        public ulong TransformStateValue(GameState gameState)
        {
            if (gameState == null)
            {
                return 0;
            }
            ulong boardValue = 0;
            int i, j;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    Cell cellState = gameState.Grid.Cells[j][i];
                    ulong cell = 0;
                    if (cellState != null)
                    {
                        cell = (ulong)Math.Log((double)cellState.Value, 2);
                    }
                    boardValue = boardValue | cell << (i * 4 + j) * 4;
                }

            }            
            return boardValue;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public GameState GetState()
        {            
            string state = browserManager.ExecuteScript("localStorage['gameState']") as string;
            if (state == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<GameState>(state);
        }

        #endregion
    }
}
