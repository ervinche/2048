using System.Windows;
using Game2048.Infrastructure.Interfaces;

namespace Game2048.Infrastructure
{
    /// <summary>
    /// Notifications service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="message">The notifications message.</param>
        public void ShowNotification(string message)
        {
            MessageBox.Show(message);
        }
    }
}
