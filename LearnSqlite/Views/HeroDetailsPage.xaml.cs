using LearnSqlite.Services;
using LearnSqlite.ViewModels;

namespace LearnSqlite.Views;

public partial class HeroDetailsPage : ContentPage
{
    public HeroDetailsPage()
    {
        InitializeComponent();
        BindingContext = new HeroDetailsViewModel(new MockHeroService());
    }
}

