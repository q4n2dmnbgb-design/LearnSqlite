using System.Collections.ObjectModel;
using System.Windows.Input;
using LearnSqlite.Models;
using LearnSqlite.Services;
using LearnSqlite.Views;

namespace LearnSqlite.ViewModels;

/// <summary>
/// ViewModel עבור מסך הצפייה באוסף גיבורי-העל (HeroesListPage).
/// אחראי על שליפת הרשימה מהשירות, סינון/חיפוש, וניווט למסך הוספה או למסך פרטי/עדכון רשומה.
/// </summary>
public class HeroesListViewModel : BaseViewModel
{
    private readonly IHeroService _heroService;
    private string _searchText = string.Empty;
    private int _totalCount;

    public ObservableCollection<HeroItem> Heroes { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (_totalCount != value)
            {
                _totalCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SummaryText));
            }
        }
    }

    public string SummaryText => $"סה\"כ גיבורים מוצגים בארכיון: {TotalCount}";

    public ICommand LoadHeroesCommand { get; }
    public ICommand SearchHeroesCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand GoToAddHeroCommand { get; }
    public ICommand OpenHeroDetailsCommand { get; }

    public HeroesListViewModel(IHeroService heroService)
    {
        _heroService = heroService;
        Title = "ארכיון גיבורי-העל";

        LoadHeroesCommand = new Command(
            execute: async () => await LoadHeroesAsync(),
            canExecute: () => IsNotLoading);

        SearchHeroesCommand = new Command(
            execute: async () => await SearchHeroesAsync(),
            canExecute: () => IsNotLoading);

        ClearSearchCommand = new Command(
            execute: async () => await ClearSearchAsync(),
            canExecute: () => IsNotLoading);

        GoToAddHeroCommand = new Command(
            execute: async () => await GoToAddHeroAsync(),
            canExecute: () => IsNotLoading);

        OpenHeroDetailsCommand = new Command<HeroItem>(
            execute: async (hero) => await OpenHeroDetailsAsync(hero),
            canExecute: (hero) => IsNotLoading && hero is not null);
    }

    protected override void OnLoadingStateChanged()
    {
        ((Command)LoadHeroesCommand).ChangeCanExecute();
        ((Command)SearchHeroesCommand).ChangeCanExecute();
        ((Command)ClearSearchCommand).ChangeCanExecute();
        ((Command)GoToAddHeroCommand).ChangeCanExecute();
        ((Command<HeroItem>)OpenHeroDetailsCommand).ChangeCanExecute();
    }

    public async Task LoadHeroesAsync()
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

            List<HeroItem> items = string.IsNullOrWhiteSpace(SearchText)
                ? await _heroService.GetAllHeroesAsync()
                : await _heroService.SearchHeroesAsync(SearchText);

            Heroes.Clear();
            foreach (HeroItem item in items)
            {
                Heroes.Add(item);
            }

            TotalCount = Heroes.Count;
        }
        catch (Exception ex)
        {
            HasError = true;
            StatusMessage = $"שגיאה בטעינת הארכיון: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchHeroesAsync()
    {
        await LoadHeroesAsync();
    }

    public async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        await LoadHeroesAsync();
    }

    private async Task GoToAddHeroAsync()
    {
        if (Shell.Current is not null)
        {
            await Shell.Current.GoToAsync(nameof(AddHeroPage));
        }
    }

    private async Task OpenHeroDetailsAsync(HeroItem? hero)
    {
        if (hero is null || Shell.Current is null)
        {
            return;
        }

        var navigationParams = new Dictionary<string, object>
        {
            { "HeroId", hero.Id }
        };

        await Shell.Current.GoToAsync(nameof(HeroDetailsPage), navigationParams);
    }
}
