using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace ScreenManager.Helper
{
    public static class Notification
    {
        public static void Notify(Window window, string msg, NotificationType type)
        {
            var notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: window,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10
                    );

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            switch (type)
            {
                case NotificationType.Success:
                    notifier.ShowSuccess(msg);
                    break;
                case NotificationType.Warning:
                    notifier.ShowWarning(msg);
                    break;
                case NotificationType.Error:
                    notifier.ShowError(msg);
                    break;
                case NotificationType.Info:
                    notifier.ShowInformation(msg);
                    break;
            }
        }
    }

    public enum NotificationType
    {
        Success,
        Warning,
        Error,
        Info
    }
}
