using System.Windows.Input;
using LearnSqlite.Models;
using LearnSqlite.Services;

namespace LearnSqlite.ViewModels;

/// <summary>
/// ViewModel עבור מסך פרטי גיבור, עריכה ומחיקה (HeroDetailsPage).
/// מממש את ממשק IQueryAttributable לקבלת מזהה הרשומה (HeroId) מניווט ה-Shell,
/// ומדגים את פעולות ה-Read-By-Id, Update ו-Delete מול השירות.
/// </summary>
public class HeroDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IHeroService _heroService;
    private int _heroId;
    private string _name = string.Empty;
    private string _alterEgo = string.Empty;
    private string _publisher = "Marvel";
    private int _powerLevel = 75;
    private bool _isEliteMember;
    private string _imageUrl = string.Empty;

    public List<string> AvailablePublishers { get; } = new() { "Marvel", "DC", "Independent" };

    public int HeroId
    {
        get => _heroId;
        set
        {
            if (_heroId != value)
            {
                _heroId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecordIdDisplay));
                RefreshCommands();
            }
        }
    }

    public string RecordIdDisplay => $"מזהה רשומה (Primary Key): #{HeroId}";

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

    public string PowerLevelDisplay => $"דירוג עוצמה: {PowerLevel}/100";

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

    public ICommand UpdateHeroCommand { get; }
    public ICommand DeleteHeroCommand { get; }
    public ICommand GoBackCommand { get; }

    public HeroDetailsViewModel(IHeroService heroService)
    {
        _heroService = heroService;
        Title = "פרטי ועדכון גיבור-על";

        UpdateHeroCommand = new Command(
            execute: async () => await UpdateHeroAsync(),
            canExecute: CanUpdateHero);

        DeleteHeroCommand = new Command(
            execute: async () => await DeleteHeroAsync(),
            canExecute: CanDeleteHero);

        GoBackCommand = new Command(
            execute: async () => await GoBackAsync(),
            canExecute: () => IsNotLoading);
    }

    /// <summary>
    /// מתודה של ממשק IQueryAttributable המופעלת אוטומטית על ידי Shell בעת כניסה למסך עם פרמטרים.
    /// </summary>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("HeroId", out object? rawId))
        {
            if (rawId is int intId)
            {
                await LoadHeroByIdAsync(intId);
            }
            else if (int.TryParse(rawId?.ToString(), out int parsedId))
            {
                await LoadHeroByIdAsync(parsedId);
            }
        }
    }

    public bool CanUpdateHero()
    {
        return IsNotLoading &&
               HeroId > 0 &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(AlterEgo) &&
               !string.IsNullOrWhiteSpace(ImageUrl) &&
               PowerLevel >= 1 &&
               PowerLevel <= 100;
    }

    public bool CanDeleteHero()
    {
        return IsNotLoading && HeroId > 0;
    }

    protected override void OnLoadingStateChanged()
    {
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        ((Command)UpdateHeroCommand).ChangeCanExecute();
        ((Command)DeleteHeroCommand).ChangeCanExecute();
        ((Command)GoBackCommand).ChangeCanExecute();
    }

    public async Task LoadHeroByIdAsync(int id)
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = string.Empty;

            HeroItem? hero = await _heroService.GetHeroByIdAsync(id);
            if (hero is null)
            {
                HasError = true;
                StatusMessage = $"לא נמצאה רשומת גיבור עם מזהה #{id}.";
                return;
            }

            HeroId = hero.Id;
            Name = hero.Name;
            AlterEgo = hero.AlterEgo;
            Publisher = hero.Publisher;
            PowerLevel = hero.PowerLevel;
            IsEliteMember = hero.IsEliteMember;
            ImageUrl = hero.ImageUrl;
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"שגיאה בטעינת פרטי הגיבור: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task UpdateHeroAsync()
    {
        if (!CanUpdateHero())
        {
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = string.Empty;

            var updated = new HeroItem
            {
                Id = HeroId,
                Name = Name.Trim(),
                AlterEgo = AlterEgo.Trim(),
                Publisher = Publisher.Trim(),
                PowerLevel = PowerLevel,
                IsEliteMember = IsEliteMember,
                ImageUrl = ImageUrl.Trim()
            };

            int rowsAffected = await _heroService.UpdateHeroAsync(updated);
            if (rowsAffected > 0)
            {
                StatusMessage = "השינויים נשמרו בהצלחה!";
                if (Shell.Current is not null)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                HasError = true;
                StatusMessage = "לא נמצאה רשומה לעדכון.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"שגיאה בעדכון הרשומה: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task DeleteHeroAsync()
    {
        if (!CanDeleteHero())
        {
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = string.Empty;

            int rowsAffected = await _heroService.DeleteHeroAsync(HeroId);
            if (rowsAffected > 0)
            {
                StatusMessage = "הרשומה נמחקה בהצלחה.";
                if (Shell.Current is not null)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            else
            {
                HasError = true;
                StatusMessage = "מחיקת הרשומה נכשלה.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"שגיאה במחיקת הרשומה: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task GoBackAsync()
    {
        if (Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

