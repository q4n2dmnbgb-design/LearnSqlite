using LearnSqlite.Services;
using LearnSqlite.ViewModels;

namespace LearnSqlite.Views;

public partial class AddHeroPage : ContentPage
{
    public AddHeroPage()
    {
        InitializeComponent();
        BindingContext = new AddHeroViewModel(new MockHeroService());
    }
}

