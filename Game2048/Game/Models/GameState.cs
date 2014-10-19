using Newtonsoft.Json;

namespace Game2048.Game.Models
{
    public class GameState
    {
        [JsonProperty("grid")]
        public Grid Grid { get; set; }

        [JsonProperty("over")]
        public bool Over { get; set; }

        [JsonProperty("won")]
        public bool Won { get; set; }

        [JsonProperty("keepPlaying")]
        public bool KeepPlaying { get; set; }
    }

    public class Grid
    {

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("cells")]
        public Cell[][] Cells { get; set; }
    }

    public class Position
    {

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }
    }


    public class Cell
    {

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }


}
