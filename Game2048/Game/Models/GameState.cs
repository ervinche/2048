using Newtonsoft.Json;

namespace Game2048.Game.Models
{
    /// <summary>
    /// Class representing game state.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// gets or sets the grid.
        /// </summary>
        [JsonProperty("grid")]
        public Grid Grid { get; set; }

        /// <summary>
        /// gets or sets if game is over.
        /// </summary>
        [JsonProperty("over")]
        public bool Over { get; set; }

        /// <summary>
        /// gets or sets if game has been won.
        /// </summary>
        [JsonProperty("won")]
        public bool Won { get; set; }

        /// <summary>
        /// Gets or sets if game is in keeplaying state.
        /// </summary>
        [JsonProperty("keepPlaying")]
        public bool KeepPlaying { get; set; }
    }

    /// <summary>
    /// Class representing the grid.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Gets or ets the size of the grid.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        [JsonProperty("cells")]
        public Cell[][] Cells { get; set; }
    }

    /// <summary>
    /// Class representing the position.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        [JsonProperty("x")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        [JsonProperty("y")]
        public int Y { get; set; }
    }

    /// <summary>
    /// Class representing a cell.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Gets or sets the position of the cell.
        /// </summary>
        [JsonProperty("position")]
        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }
    }


}
