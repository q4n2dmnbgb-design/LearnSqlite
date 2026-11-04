using System.Windows.Input;
using LearnSqlite.Models;
using LearnSqlite.Services;

namespace LearnSqlite.ViewModels;

/// <summary>
/// ViewModel עבור מסך הוספת גיבור חדש (AddHeroPage).
/// מדגים קליטת נתונים בטופס Two-Way Binding, אימות קלט (Validation) באמצעות CanExecute,
/// וקריאה לפעולת ה-Create בשירות הנתונים (AddHeroAsync).
/// </summary>
public class AddHeroViewModel : BaseViewModel
{
    private readonly IHeroService _heroService;
    private string _name = string.Empty;
    private string _alterEgo = string.Empty;
    private string _publisher = "Marvel";
    private int _powerLevel = 80;
    private bool _isEliteMember = true;
    private string _imageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/149-captain-america.jpg";

    public List<string> AvailablePublishers { get; } = new() { "Marvel", "DC", "Independent" };

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }
    }

    public string AlterEgo
    {
        get => _alterEgo;
        set
        {
            if (_alterEgo != value)
            {
                _alterEgo = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }
    }

    public string Publisher
    {
        get => _publisher;
        set
        {
            if (_publisher != value)
            {
                _publisher = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }
    }

    public int PowerLevel
    {
        get => _powerLevel;
        set
        {
            if (_powerLevel != value)
            {
                _powerLevel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PowerLevelDisplay));
                RefreshCommands();
            }
        }
    }

    public string PowerLevelDisplay => $"דירוג עוצמה נבחר: {PowerLevel}/100";

    public bool IsEliteMember
    {
        get => _isEliteMember;
        set
        {
            if (_isEliteMember != value)
            {
                _isEliteMember = value;
                OnPropertyChanged();
            }
        }
    }

    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (_imageUrl != value)
            {
                _imageUrl = value;
                OnPropertyChanged();
                RefreshCommands();
            }
        }
    }

    public ICommand SaveHeroCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand UsePresetHeroImageCommand { get; }

    public AddHeroViewModel(IHeroService heroService)
    {
        _heroService = heroService;
        Title = "הוספת גיבור-על חדש";

        SaveHeroCommand = new Command(
            execute: async () => await SaveHeroAsync(),
            canExecute: CanSaveHero);

        CancelCommand = new Command(
            execute: async () => await CancelAsync(),
            canExecute: () => IsNotLoading);

        UsePresetHeroImageCommand = new Command<string>(
            execute: ApplyPresetHeroImage,
            canExecute: (_) => IsNotLoading);
    }

    public bool CanSaveHero()
    {
        return IsNotLoading &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(AlterEgo) &&
               !string.IsNullOrWhiteSpace(ImageUrl) &&
               PowerLevel >= 1 &&
               PowerLevel <= 100;
    }

    protected override void OnLoadingStateChanged()
    {
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        ((Command)SaveHeroCommand).ChangeCanExecute();
        ((Command)CancelCommand).ChangeCanExecute();
        ((Command<string>)UsePresetHeroImageCommand).ChangeCanExecute();
    }

    private void ApplyPresetHeroImage(string? presetKey)
    {
        ImageUrl = presetKey switch
        {
            "Wolverine" => "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/717-wolverine.jpg",
            "Thor" => "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/659-thor.jpg",
            "GreenLantern" => "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/306-hal-jordan.jpg",
            _ => "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/149-captain-america.jpg"
        };
    }

    public async Task SaveHeroAsync()
    {
        if (!CanSaveHero())
        {
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = string.Empty;

            var newHero = new HeroItem
            {
                Name = Name.Trim(),
                AlterEgo = AlterEgo.Trim(),
                Publisher = string.IsNullOrWhiteSpace(Publisher) ? "Marvel" : Publisher.Trim(),
                PowerLevel = PowerLevel,
                IsEliteMember = IsEliteMember,
                ImageUrl = ImageUrl.Trim()
            };

            int rowsAffected = await _heroService.AddHeroAsync(newHero);
            if (rowsAffected > 0)
            {
                StatusMessage = "הגיבור נשמר בהצלחה!";
                if (Shell.Current is not null)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                HasError = true;
                StatusMessage = "שמירת הגיבור נכשלה.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"שגיאה בשמירת הגיבור: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CancelAsync()
    {
        if (Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
