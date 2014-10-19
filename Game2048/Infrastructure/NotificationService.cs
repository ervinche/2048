using Game2048.Infrastructure.Interfaces;
using System.Windows;

namespace Game2048.Infrastructure
{
    public class NotificationService : INotificationService
    {
        public void ShowNotification(string message)
        {
            MessageBox.Show(message);
        }
    }
}
