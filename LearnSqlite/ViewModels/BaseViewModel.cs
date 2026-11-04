using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LearnSqlite.ViewModels;

/// <summary>
/// מחלקת בסיס לכל ה-ViewModels בפרויקט.
/// מממשת את ממשק INotifyPropertyChanged עם Setters מפורשים וברורים לתלמיד,
/// ומספקת ניהול מצב טעינה (IsLoading / IsNotLoading) והודעות משוב/שגיאה.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    private string _title = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _hasError;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// האם מתבצעת כעת פעולה א-סינכרונית ברקע (מחובר ל-ActivityIndicator ב-XAML).
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
                OnLoadingStateChanged();
            }
        }
    }

    /// <summary>
    /// מאפיין משלים לנוחות עבודה ב-XAML ובתנאי CanExecute.
    /// </summary>
    public bool IsNotLoading => !IsLoading;

    /// <summary>
    /// כותרת המסך הנוכחי.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// הודעת משוב או שגיאה למשתמש.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public bool HasError
    {
        get => _hasError;
        set
        {
            if (_hasError != value)
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// מאפשר למחלקות יורשות לרענן מצב פקודות (CanExecute) כאשר מצב הטעינה משתנה.
    /// </summary>
    protected virtual void OnLoadingStateChanged()
    {
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

