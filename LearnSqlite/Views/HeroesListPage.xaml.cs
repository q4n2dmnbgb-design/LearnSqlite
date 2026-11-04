using LearnSqlite.Services;
using LearnSqlite.ViewModels;

namespace LearnSqlite.Views;

public partial class HeroesListPage : ContentPage
{
    private readonly HeroesListViewModel _viewModel;

    public HeroesListPage()
    {
        InitializeComponent();

        // בשלב הבסיס (ללא DI) אנו יוצרים את שירות ה-Mockup ומעבירים אותו ל-ViewModel
        _viewModel = new HeroesListViewModel(new MockHeroService());
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHeroesAsync();
    }
}

