using Game2048.Infrastructure.Interfaces;

namespace Game2048.infrastructure
{
    public class MediaPlayer : IMediaPlayer
    {
        public void Play(string file)
        {
            new System.Media.SoundPlayer(file).Play();
        }
    }
}
