using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace UI.Models;

public enum StatusLevels
{
    Normal,
    Warning,
    Error
}

public class CurrentStatus : INotifyPropertyChanged
{
    public string Status
    {
        get => m_status;
        set => SetField(ref m_status, value, nameof(m_status));
    }

    public StatusLevels Level { get; set; } = StatusLevels.Normal;

    public override string ToString()
    {
        return Status;
    }

    private string m_status = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public static class StatusLevelExtension
{
    public static IBrush ToBrush(this StatusLevels level)
    {
        switch (level)
        {
            case StatusLevels.Normal:
                return Brushes.Black;
            case StatusLevels.Warning:
                return Brushes.Orange;
            case StatusLevels.Error:
                return Brushes.Red;
            default:
                return Brushes.Azure;
        }
    }
}

public static class Statuses
{
    public static void MovingTowards(this CurrentStatus status)
    {
        status.Status = $"Moving towards fixed points";
        status.Level = StatusLevels.Warning;
    }

    public static void Waiting(this CurrentStatus status)
    {
        status.Status = "Waiting user input";
        status.Level = StatusLevels.Normal;
    }

    public static void Connecting(this CurrentStatus status)
    {
        status.Status = "Connecting";
        status.Level = StatusLevels.Warning;
    }

    public static void MoveTowardsZero(this CurrentStatus status)
    {
        status.Status = $"Moving towards zero points";
        status.Level = StatusLevels.Warning;
    }
}