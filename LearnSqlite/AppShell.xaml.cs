using LearnSqlite.Views;

namespace LearnSqlite;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // רישום נתיבי הניווט (Routes) עבור מסך ההוספה ומסך פרטי/עדכון הרשומה
        Routing.RegisterRoute(nameof(AddHeroPage), typeof(AddHeroPage));
        Routing.RegisterRoute(nameof(HeroDetailsPage), typeof(HeroDetailsPage));
    }
}
