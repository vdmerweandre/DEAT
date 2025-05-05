using Microsoft.AspNetCore.Components;

namespace DEAT.AdminUI.Services
{
    public class NotificationService
    {
        public event Action<string, string, NotificationType>? OnShow;
        public event Action? OnHide;

        public void ShowSuccess(string message, string? title = null)
        {
            OnShow?.Invoke(message, title ?? "Success", NotificationType.Success);
        }

        public void ShowError(string message, string? title = null)
        {
            OnShow?.Invoke(message, title ?? "Error", NotificationType.Error);
        }

        public void ShowWarning(string message, string? title = null)
        {
            OnShow?.Invoke(message, title ?? "Warning", NotificationType.Warning);
        }

        public void ShowInfo(string message, string? title = null)
        {
            OnShow?.Invoke(message, title ?? "Information", NotificationType.Info);
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }
    }

    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }
} 