using Game2048.Infrastructure.Interfaces;
using System.Windows;

namespace Game2048.Infrastructure
{
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ShowNotification(string message)
        {
            MessageBox.Show(message);
        }
    }
}
