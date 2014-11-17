namespace Game2048.Infrastructure.Interfaces
{
    /// <summary>
    /// Notification service.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="message">The notifications message.</param>
        void ShowNotification(string message);
    }
}
