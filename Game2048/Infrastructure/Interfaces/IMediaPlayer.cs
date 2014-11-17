namespace Game2048.Infrastructure.Interfaces
{
    /// <summary>
    /// Media Player Service.
    /// </summary>
    public interface IMediaPlayer
    {
        /// <summary>
        /// Plays the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        void Play(string file);        
    }
}
