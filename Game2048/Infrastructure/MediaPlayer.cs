using Game2048.Infrastructure.Interfaces;

namespace Game2048.infrastructure
{
    public class MediaPlayer : IMediaPlayer
    {
        /// <summary>
        /// Plays the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void Play(string file)
        {
            new System.Media.SoundPlayer(file).Play();
        }
    }
}
