using LearnSqlite.Services;
using LearnSqlite.ViewModels;

namespace LearnSqlite.Views;

public partial class HeroesListPage : ContentPage
{
    private readonly HeroesListViewModel _viewModel;

    public HeroesListPage()
    {
        InitializeComponent();

        // בשלב ה-SQLite (ללא DI) אנו מחליפים את MockHeroService ב-SqliteHeroService בלבד!
        // ה-ViewModel אינו משתנה כלל משום שהוא עובד מול הממשק IHeroService.
        _viewModel = new HeroesListViewModel(new SqliteHeroService());
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHeroesAsync();
    }
}

