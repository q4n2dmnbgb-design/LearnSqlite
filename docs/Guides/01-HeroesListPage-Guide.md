# HeroesListPage — מסך צפייה באוסף, חיפוש וניווט

## מטרת הפרק

- **מה הלומד יראה ויוכל לעשות:** צפייה ברשימת גיבורי-העל מתוך השירות בפקד `CollectionView`, סינון גיבורים לפי טקסט חופשי (שם, זהות סודית או יקום), מעבר למסך הוספת גיבור חדש, ומעבר למסך פרטים/עריכה של גיבור נבחר.
- **עובדות הפרויקט הנוכחיות:** המסך ממומש בקבצים `LearnSqlite/Views/HeroesListPage.xaml`, `LearnSqlite/Views/HeroesListPage.xaml.cs` ו-`LearnSqlite/ViewModels/HeroesListViewModel.cs`.
- **מדוע זו אבן הדרך הראשונה:** זהו מסך הבית הראשי ב-`AppShell.xaml` המדגים את פעולת ה-**Read** (`GetAllHeroesAsync` ו-`SearchHeroesAsync`) ב-CRUD.

## מפת הדרך

| מצב נוכחי | אבן דרך זו | אבן הדרך הבאה | מושג מפתח |
| --- | --- | --- | --- |
| מודל ושירות `MockHeroService` מוכנים | הצגת האוסף ב-`CollectionView` וסינון | מסך הוספת גיבור (`AddHeroPage`) | `ObservableCollection<HeroItem>` ו-`RelativeSource` |

## מפת הבלוקים של המסך

![מודל הוראה חזותי ב-HTML של מסך HeroesListPage](../Images/HeroesListPage-overview.png)

> תמונה זו היא מודל הוראה חזותי ב-HTML (אינה צילום אמולטור). המימוש ב-.NET MAUI נבדק ומאומת בנפרד.

| בלוק | קובץ ושכבה | מטרה | ראיה / אימות |
| --- | --- | --- | --- |
| `header-banner` | `Views/HeroesListPage.xaml` | הצגת כותרת, סיכום כמות (`SummaryText`) וכפתור הוספה | ניווט ל-`AddHeroPage` בלחיצה |
| `search-filter` | `Views/HeroesListPage.xaml` | שדה קלט לחיפוש וכפתורי "חפש" ו"נקה" | סינון הרשימה דרך `SearchHeroesCommand` |
| `collection-list` | `Views/HeroesListPage.xaml` | הצגת כרטיסי הגיבורים ב-`CollectionView` עם תמונה מהאינטרנט | הצגת 6 גיבורי הבסיס וכפתור "פרטים" |
| `status-feedback` | `Views/HeroesListPage.xaml` | חיווי טעינה (`ActivityIndicator`) והודעות שגיאה | מופעל בעת `IsLoading = true` |

---

## בלוק 1 — באנר כותרת עליון וניווט להוספה (`header-banner`)

### מה משתנה
מוסיפים בחלקו העליון של המסך כרטיס כותרת המציג את סך הגיבורים המוצגים בארכיון וכפתור בולט למעבר למסך הוספת גיבור חדש.

### היכן הקוד יושב ומדוע
התצוגה יושבת ב-`LearnSqlite/Views/HeroesListPage.xaml` (שכבת ה-View), בעוד שהטקסט המחושב `SummaryText` ופקודת הניווט `GoToAddHeroCommand` יושבים ב-`LearnSqlite/ViewModels/HeroesListViewModel.cs` (שכבת ה-ViewModel).

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroesListPage.xaml -->
<Border Grid.Row="0"
        StrokeShape="RoundRectangle 16"
        BackgroundColor="#1E3A8A"
        StrokeThickness="0"
        Padding="16">
    <Grid ColumnDefinitions="*,Auto"
          ColumnSpacing="12">
        <VerticalStackLayout Grid.Column="0"
                             Spacing="4">
            <Label Text="🦸 ארכיון גיבורי-העל"
                   FontSize="20"
                   FontAttributes="Bold"
                   TextColor="White" />
            <Label Text="{Binding SummaryText}"
                   FontSize="13"
                   TextColor="#BFDBFE" />
        </VerticalStackLayout>

        <Button Grid.Column="1"
                Text="+ גיבור חדש"
                Command="{Binding GoToAddHeroCommand}"
                BackgroundColor="#2563EB"
                TextColor="White"
                FontAttributes="Bold"
                CornerRadius="10"
                VerticalOptions="Center" />
    </Grid>
</Border>
```

```csharp
// מתוך LearnSqlite/ViewModels/HeroesListViewModel.cs
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

private async Task GoToAddHeroAsync()
{
    if (Shell.Current is not null)
    {
        await Shell.Current.GoToAsync(nameof(AddHeroPage));
    }
}
```

### מדוע זה עובד
כאשר `TotalCount` מתעדכן בסיום טעינת הרשימה, הוא מפעיל `OnPropertyChanged(nameof(SummaryText))` והתווית בבאנר מתעדכנת מיד. כפתור ההוספה מחובר ל-`GoToAddHeroCommand` המוגן על ידי `CanExecute: () => IsNotLoading`.

![מודל HTML של בלוק 1 — באנר כותרת](../Images/HeroesListPage-block1-header-banner.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מדוע קראנו ל-`OnPropertyChanged(nameof(SummaryText))` בתוך ה-`set` של `TotalCount`? מה היה קורה לתווית במסך אם היינו קוראים רק ל-`OnPropertyChanged()` עבור `TotalCount`?

### אימות
- בעת עליית המסך מוצג הכיתוב: `סה"כ גיבורים מוצגים בארכיון: 6`.
- לחיצה על `+ גיבור חדש` מנווטת למסך `AddHeroPage`.

---

## בלוק 2 — סרגל חיפוש וסינון רשומות (`search-filter`)

### מה משתנה
מוסיפים תיבת חיפוש (`Entry`) המקושרת ב-Two-Way Binding למאפיין `SearchText`, לצד כפתור "חפש" וכפתור "נקה".

### היכן הקוד יושב ומדוע
הקלט והכפתורים נמצאים ב-`LearnSqlite/Views/HeroesListPage.xaml`, והלוגיקה שקוראת ל-`_heroService.SearchHeroesAsync(SearchText)` נמצאת ב-`LearnSqlite/ViewModels/HeroesListViewModel.cs`.

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroesListPage.xaml -->
<Border Grid.Row="1"
        StrokeShape="RoundRectangle 12"
        BackgroundColor="White"
        Stroke="#CBD5E1"
        StrokeThickness="1"
        Padding="10">
    <Grid ColumnDefinitions="*,Auto,Auto"
          ColumnSpacing="8">
        <Entry Grid.Column="0"
               Placeholder="חפש לפי שם, זהות סודית או יקום (Marvel/DC)..."
               Text="{Binding SearchText}"
               TextColor="#0F172A"
               PlaceholderColor="#64748B" />

        <Button Grid.Column="1"
                Text="חפש"
                Command="{Binding SearchHeroesCommand}"
                BackgroundColor="#0284C7"
                TextColor="White"
                CornerRadius="8" />

        <Button Grid.Column="2"
                Text="נקה"
                Command="{Binding ClearSearchCommand}"
                BackgroundColor="#E2E8F0"
                TextColor="#1E293B"
                CornerRadius="8" />
    </Grid>
</Border>
```

```csharp
// מתוך LearnSqlite/ViewModels/HeroesListViewModel.cs
public async Task ClearSearchAsync()
{
    SearchText = string.Empty;
    await LoadHeroesAsync();
}
```

### מדוע זה עובד
פקודת `SearchHeroesCommand` מפעילה את `LoadHeroesAsync()`, שבודקת האם `SearchText` ריק: אם הוא ריק נשלפים כל הגיבורים (`GetAllHeroesAsync`), ואם הוזן טקסט מופעלת מתודת הסינון בשירות (`SearchHeroesAsync`). בענף ה-SQLite מתודה זו תתורגם לשאילתת `WHERE` במסד הנתונים!

![מודל HTML של בלוק 2 — סרגל חיפוש](../Images/HeroesListPage-block2-search-filter.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מדוע עדיף לבצע את הסינון בתוך השירות (`IHeroService.SearchHeroesAsync`) ולא לסנן רק ברשימה המקומית בתוך ה-ViewModel? חישבו מה יקרה כשנעבור למסד נתונים SQLite עם אלפי רשומות.

### אימות
- הקלדת `DC` ולחיצה על "חפש" מציגה רק את גיבורי DC ומעדכנת את המונה ל-4.
- לחיצה על "נקה" מאפסת את תיבת הטקסט ומחזירה את כל 6 הגיבורים.

---

## בלוק 3 — רשימת הגיבורים (`CollectionView`) וניווט לפרטים (`collection-list`)

### מה משתנה
מציגים את אוסף הגיבורים (`ObservableCollection<HeroItem>`) באמצעות `CollectionView` עם תבנית כרטיס (`DataTemplate`) הכוללת תמונה מהאינטרנט, פרטי הגיבור וכפתור "פרטים ⬅" המעביר את הגיבור הנבחר למסך הפרטים.

### היכן הקוד יושב ומדוע
תבנית הכרטיס יושבת ב-`LearnSqlite/Views/HeroesListPage.xaml` עם `x:DataType="models:HeroItem"`, ופקודת המעבר `OpenHeroDetailsCommand` יושבת ב-`HeroesListViewModel.cs`.

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroesListPage.xaml -->
<CollectionView Grid.Row="2"
                ItemsSource="{Binding Heroes}"
                SelectionMode="None">
    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:HeroItem">
            <Border StrokeShape="RoundRectangle 14"
                    BackgroundColor="White"
                    Stroke="#E2E8F0"
                    StrokeThickness="1"
                    Padding="12"
                    Margin="0,0,0,10">
                <Grid ColumnDefinitions="72,*,Auto"
                      ColumnSpacing="12">
                    <Border Grid.Column="0"
                            StrokeShape="RoundRectangle 12"
                            StrokeThickness="0"
                            HeightRequest="72"
                            WidthRequest="72"
                            BackgroundColor="#E2E8F0">
                        <Image Source="{Binding ImageUrl}"
                               Aspect="AspectFill"
                               HeightRequest="72"
                               WidthRequest="72" />
                    </Border>

                    <VerticalStackLayout Grid.Column="1"
                                         Spacing="3"
                                         VerticalOptions="Center">
                        <Label Text="{Binding Name}"
                               FontSize="16"
                               FontAttributes="Bold"
                               TextColor="#0F172A" />
                        <Label Text="{Binding AlterEgo}"
                               FontSize="13"
                               TextColor="#475569" />
                    </VerticalStackLayout>

                    <Button Grid.Column="2"
                            Text="פרטים ⬅"
                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:HeroesListViewModel}}, Path=OpenHeroDetailsCommand}"
                            CommandParameter="{Binding .}"
                            BackgroundColor="#EFF6FF"
                            TextColor="#1D4ED8"
                            FontAttributes="Bold"
                            CornerRadius="10"
                            VerticalOptions="Center" />
                </Grid>
            </Border>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

```csharp
// מתוך LearnSqlite/ViewModels/HeroesListViewModel.cs
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
```

### מדוע זה עובד
בתוך `DataTemplate`, הקשר הנתונים (`BindingContext`) הוא אובייקט בודד מסוג `HeroItem`. לכן, כדי שכפתור ה"פרטים" יפעיל פקודה שנמצאת ב-`HeroesListViewModel`, אנו משתמשים ב-`RelativeSource AncestorType={x:Type vm:HeroesListViewModel}` ומעבירים את הפריט הנוכחי דרך `CommandParameter="{Binding .}"`.
בנוסף, אנו מעבירים בניווט רק את המזהה `HeroId` (ולא את האובייקט כולו) כדי שמסך הפרטים ישלוף את הרשומה העדכנית מהשירות באמצעות `GetHeroByIdAsync(id)` — הרגל קריטי לעבודה נכונה עם SQLite!

![מודל HTML של בלוק 3 — רשימת הגיבורים ב-CollectionView](../Images/HeroesListPage-block3-collection-list.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מדוע העברנו בניווט למסך הפרטים את המזהה `HeroId` (`int`) במקום להעביר את כל האובייקט `HeroItem`? איזו מתודה ב-`IHeroService` אנחנו מתרגלים בזכות הבחירה הזו?

### אימות
- לחיצה על כפתור `פרטים ⬅` בכרטיס של באטמן מנווטת אל `HeroDetailsPage` ומעבירה `HeroId = 1`.

---

## בלוק 4 — חיווי טעינה (`ActivityIndicator`) וטעינה אוטומטית ב-`OnAppearing` (`status-feedback`)

### מה משתנה
מוסיפים בתחתית המסך פקד `ActivityIndicator` ותווית שגיאה/משוב, ומפעילים את `LoadHeroesAsync()` מתוך `OnAppearing()` בקובץ ה-Code-Behind של המסך.

### היכן הקוד יושב ומדוע
החיווי הוויזואלי יושב ב-`HeroesListPage.xaml`, וקריאת המחזוריות של המסך יושבת ב-`HeroesListPage.xaml.cs` כיוון ש-`OnAppearing` הוא אירוע מחזור חיים של ה-`ContentPage`.

### שינוי הקוד ב-XAML וב-Code-Behind

```xml
<!-- מתוך LearnSqlite/Views/HeroesListPage.xaml -->
<VerticalStackLayout Grid.Row="3"
                     Spacing="6">
    <ActivityIndicator IsRunning="{Binding IsLoading}"
                       IsVisible="{Binding IsLoading}"
                       Color="#2563EB"
                       HorizontalOptions="Center" />

    <Label Text="{Binding StatusMessage}"
           IsVisible="{Binding HasStatusMessage}"
           TextColor="#DC2626"
           FontSize="13"
           HorizontalOptions="Center" />
</VerticalStackLayout>
```

```csharp
// מתוך LearnSqlite/Views/HeroesListPage.xaml.cs
protected override async void OnAppearing()
{
    base.OnAppearing();
    await _viewModel.LoadHeroesAsync();
}
```

### מדוע זה עובד
הפעלת `LoadHeroesAsync()` בתוך `OnAppearing()` (ולא בתוך הבנאי של ה-ViewModel!) מבטיחה שני יתרונות עצומים:
1. איננו חוסמים את יצירת המסך בקריאות א-סינכרוניות בבנאי.
2. בכל פעם שהמשתמש חוזר ממסך ההוספה (`AddHeroPage`) או ממסך העדכון/מחיקה (`HeroDetailsPage`), מתודת `OnAppearing` מופעלת אוטומטית ומרעננת את הרשימה עם הנתונים העדכניים!

![מודל HTML של בלוק 4 — חיווי טעינה ומשוב](../Images/HeroesListPage-block4-status-feedback.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מה היה קורה אילו היינו קוראים ל-`LoadHeroesAsync()` רק פעם אחת בבנאי של המסך, ולא ב-`OnAppearing()`, לאחר שהמשתמש הוסיף גיבור חדש במסך ההוספה וחזר אחורה?

### אימות
- חזרה ממסך הוספה או ממסך עריכה מרעננת מיד את הרשימה ב-`HeroesListPage`.

---

## רפלקציה והצעד הבא

- **החלטה ארכיטקטונית:** בחרנו להעביר `HeroId` בניווט ולרענן את הרשימה ב-`OnAppearing` כדי שכל מסך יעבוד ישירות מול שירות הנתונים (`IHeroService`), בדיוק כפי שיעבוד מול טבלת SQLite.
- **שאלה למחשבה:** מדוע `Heroes` מוגדר כ-`ObservableCollection<HeroItem>` ולא כ-`List<HeroItem>` רגיל?
- **אתגר קטן:** הוסיפו ב-`HeroesListViewModel` מאפיין מחושב המציג כמה מתוך הגיבורים המוצגים הם חברי צוות עלית (`IsEliteMember == true`).

## מקורות
- תיעוד רשמי של Microsoft Learn עבור `.NET MAUI Shell Navigation` ו-`CollectionView`.

