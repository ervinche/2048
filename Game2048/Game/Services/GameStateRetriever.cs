using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;

namespace Game2048.Game.Services
{
    public class GameStateRetriever : IGameStateRetriever
    {
        private IBrowserManager browserManager;

        public GameStateRetriever(IBrowserManager browserManager)
        {
            this.browserManager = browserManager;
        }

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

        public GameState GetState()
        {            
            string state = browserManager.ExecuteScript("localStorage['gameState']") as string;
            if (state == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<GameState>(state);

        }
    }
}
