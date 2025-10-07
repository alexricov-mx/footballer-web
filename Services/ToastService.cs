using Microsoft.AspNetCore.Components;

namespace FootballerWeb.Services;

public class ToastService
{
    public event Action<ToastMessage>? OnToastAdded;
    public event Action<string>? OnToastRemoved;

    public void ShowSuccess(string message, string title = "Éxito", int duration = 5000)
    {
        var toast = new ToastMessage
        {
            Id = Guid.NewGuid().ToString(),
            Message = message,
            Title = title,
            Type = ToastType.Success,
            Duration = duration
        };

        OnToastAdded?.Invoke(toast);
    }

    public void ShowError(string message, string title = "Error", int duration = 8000)
    {
        var toast = new ToastMessage
        {
            Id = Guid.NewGuid().ToString(),
            Message = message,
            Title = title,
            Type = ToastType.Error,
            Duration = duration
        };

        OnToastAdded?.Invoke(toast);
    }

    public void ShowWarning(string message, string title = "Advertencia", int duration = 6000)
    {
        var toast = new ToastMessage
        {
            Id = Guid.NewGuid().ToString(),
            Message = message,
            Title = title,
            Type = ToastType.Warning,
            Duration = duration
        };

        OnToastAdded?.Invoke(toast);
    }

    public void ShowInfo(string message, string title = "Información", int duration = 5000)
    {
        var toast = new ToastMessage
        {
            Id = Guid.NewGuid().ToString(),
            Message = message,
            Title = title,
            Type = ToastType.Info,
            Duration = duration
        };

        OnToastAdded?.Invoke(toast);
    }

    public void RemoveToast(string id)
    {
        OnToastRemoved?.Invoke(id);
    }
}

public class ToastMessage
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ToastType Type { get; set; } = ToastType.Info;
    public int Duration { get; set; } = 5000;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}