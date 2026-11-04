# HeroDetailsPage — מסך פרטי גיבור, עדכון ומחיקה (Read / Update / Delete)

## מטרת הפרק

- **מה הלומד יראה ויוכל לעשות:** קבלת מזהה רשומה (`HeroId`) מניווט ה-Shell באמצעות `IQueryAttributable`, שליפת הרשומה המלאה מהשירות (`GetHeroByIdAsync`), עריכת השדות ושמירתם (`UpdateHeroAsync`), או מחיקת הרשומה מהמאגר (`DeleteHeroAsync`).
- **עובדות הפרויקט הנוכחיות:** המסך ממומש בקבצים `LearnSqlite/Views/HeroDetailsPage.xaml`, `LearnSqlite/Views/HeroDetailsPage.xaml.cs` ו-`LearnSqlite/ViewModels/HeroDetailsViewModel.cs`.
- **מדוע זו אבן הדרך השלישית:** המסך משלים את מחזור ה-CRUD המלא (**Read by ID**, **Update**, **Delete**) ומדגים כיצד מפתח ראשי (`Id`) משמש לאיתור, עדכון ומחיקת שורה ספציפית בטבלה.

## מפת הדרך

| מצב נוכחי | אבן דרך זו | אבן הדרך הבאה | מושג מפתח |
| --- | --- | --- | --- |
| צפייה באוסף והוספת רשומה פועלות | שליפה לפי `HeroId`, עדכון (`Update`) ומחיקה (`Delete`) | מעבר לענף ה-SQLite (`SqliteHeroService`) | `IQueryAttributable` ועבודה מול מפתח ראשי (`Primary Key`) |

## מפת הבלוקים של המסך

![מודל הוראה חזותי ב-HTML של מסך HeroDetailsPage](../Images/HeroDetailsPage-overview.png)

> תמונה זו היא מודל הוראה חזותי ב-HTML (אינה צילום אמולטור). המימוש ב-.NET MAUI נבדק ומאומת בנפרד.

| בלוק | קובץ ושכבה | מטרה | ראיה / אימות |
| --- | --- | --- | --- |
| `identity-header` | `Views/HeroDetailsPage.xaml` | הצגת תמונת הגיבור, שמו ומזהה הרשומה (`#Id`) | טעינת הגיבור לפי `HeroId` בכניסה למסך |
| `edit-fields` | `Views/HeroDetailsPage.xaml` | טופס עריכת שדות הרשומה הקיימת | שינוי ערכים מעדכן את המאפיינים ב-ViewModel |
| `update-delete-actions` | `Views/HeroDetailsPage.xaml` | כפתורי "עדכן רשומה", "מחק רשומה" ו"חזרה ללא שמירה" | עדכון או מחיקה משתקפים מיד בחזרה לארכיון |

---

## בלוק 1 — קבלת `HeroId` בניווט וכרטיס זהות הרשומה (`identity-header`)

### מה משתנה
מממשים ב-`HeroDetailsViewModel` את הממשק `IQueryAttributable` כדי לקבל את הפרמטר `HeroId` שנשלח ממסך האוסף, שולפים את הגיבור באמצעות `_heroService.GetHeroByIdAsync(id)`, ומציגים בראש המסך את תמונתו ואת מזהה המפתח הראשי שלו (`RecordIdDisplay`).

### היכן הקוד יושב ומדוע
הכרטיס העליון יושב ב-`LearnSqlite/Views/HeroDetailsPage.xaml`, וקליטת פרמטר הניווט יושבת ב-`LearnSqlite/ViewModels/HeroDetailsViewModel.cs`.

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroDetailsPage.xaml -->
<Border StrokeShape="RoundRectangle 16"
        BackgroundColor="#0F172A"
        StrokeThickness="0"
        Padding="16">
    <Grid ColumnDefinitions="96,*"
          ColumnSpacing="14">
        <Border Grid.Column="0"
                StrokeShape="RoundRectangle 14"
                Stroke="#38BDF8"
                StrokeThickness="2"
                HeightRequest="96"
                WidthRequest="96"
                BackgroundColor="#1E293B">
            <Image Source="{Binding ImageUrl}"
                   Aspect="AspectFill"
                   HeightRequest="96"
                   WidthRequest="96" />
        </Border>

        <VerticalStackLayout Grid.Column="1"
                             Spacing="4"
                             VerticalOptions="Center">
            <Label Text="{Binding Name}"
                   FontSize="19"
                   FontAttributes="Bold"
                   TextColor="White" />
            <Label Text="{Binding AlterEgo}"
                   FontSize="14"
                   TextColor="#CBD5E1" />
            <Label Text="{Binding RecordIdDisplay}"
                   FontSize="12"
                   FontAttributes="Bold"
                   TextColor="#38BDF8" />
        </VerticalStackLayout>
    </Grid>
</Border>
```

```csharp
// מתוך LearnSqlite/ViewModels/HeroDetailsViewModel.cs
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
```

### מדוע זה עובד
כאשר מנווטים עם `Shell.Current.GoToAsync(nameof(HeroDetailsPage), navigationParams)`, תשתית ה-Shell בודקת האם ה-`BindingContext` של הדף מממש את `IQueryAttributable`. אם כן, היא מזריקה אליו אוטומטית את מילון הפרמטרים, וכך `LoadHeroByIdAsync` טוען את הרשומה המתאימה מהשירות.

![מודל HTML של בלוק 1 — כרטיס זהות ומפתח ראשי](../Images/HeroDetailsPage-block1-identity-header.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מדוע אנו מציגים את `HeroId` כתווית לקריאה בלבד (`Label`) בראש המסך ולא כשדה `Entry` שהמשתמש יכול לערוך? מה תפקידו של המפתח הראשי (`Primary Key`) בטבלת SQLite?

### אימות
- לחיצה על "פרטים" עבור באטמן מציגה בראש המסך: `מזהה רשומה (Primary Key): #1`.

---

## בלוק 2 — טופס עריכת שדות הרשומה (`edit-fields`)

### מה משתנה
מציגים את כל שדות הגיבור שנשלפו (`Name`, `AlterEgo`, `Publisher`, `PowerLevel`, `IsEliteMember`, `ImageUrl`) בתוך פקדי עריכה הדו-כיווניים (`Two-Way Binding`).

### היכן הקוד יושב ומדוע
הפקדים יושבים ב-`LearnSqlite/Views/HeroDetailsPage.xaml`, והמאפיינים יושבים ב-`LearnSqlite/ViewModels/HeroDetailsViewModel.cs`.

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroDetailsPage.xaml -->
<Border StrokeShape="RoundRectangle 14"
        BackgroundColor="White"
        Stroke="#CBD5E1"
        StrokeThickness="1"
        Padding="16">
    <VerticalStackLayout Spacing="12">
        <Label Text="שם גיבור-העל:"
               FontSize="13"
               FontAttributes="Bold"
               TextColor="#1E293B" />
        <Entry Text="{Binding Name}"
               Placeholder="שם הגיבור"
               TextColor="#0F172A" />

        <Label Text="זהות סודית / שם אמיתי:"
               FontSize="13"
               FontAttributes="Bold"
               TextColor="#1E293B" />
        <Entry Text="{Binding AlterEgo}"
               Placeholder="זהות סודית"
               TextColor="#0F172A" />

        <Label Text="{Binding PowerLevelDisplay}"
               FontSize="13"
               FontAttributes="Bold"
               TextColor="#0D9488" />
        <Slider Minimum="1"
                Maximum="100"
                Value="{Binding PowerLevel}"
                MinimumTrackColor="#0D9488"
                ThumbColor="#0F766E" />
    </VerticalStackLayout>
</Border>
```

### מדוע זה עובד
כאשר `LoadHeroByIdAsync` מעתיק את ערכי ה-`HeroItem` שנשלף אל מאפייני ה-ViewModel, כל הפקדים בטופס מתמלאים אוטומטית בערכים הנוכחיים של הרשומה. שינוי של כל שדה על ידי המשתמש מעדכן את ה-ViewModel בזיכרון, אך **עדיין לא משנה את המאגר** עד ללחיצה על "עדכן רשומה".

![מודל HTML של בלוק 2 — טופס עריכת רשומה](../Images/HeroDetailsPage-block2-edit-fields.png)

### נקודת בקרה לתלמיד

> [!TIP]
> אם המשתמש משנה את שם הגיבור בטופס ואז לוחץ על `⬅ חזרה לארכיון ללא שמירה`, מדוע השם המקורי בארכיון נשאר ללא שינוי?

### אימות
- כל שדות הטופס מתמלאים בנתוני הגיבור שנבחר מיד עם פתיחת המסך.

---

## בלוק 3 — פעולות עדכון (`Update`) ומחיקה (`Delete`) מול השירות (`update-delete-actions`)

### מה משתנה
מוסיפים שני כפתורי פעולה מרכזיים: כפתור **עדכן רשומה** (`UpdateHeroCommand`) הקורא ל-`_heroService.UpdateHeroAsync(updated)`, וכפתור **מחק רשומה** (`DeleteHeroCommand`) הקורא ל-`_heroService.DeleteHeroAsync(HeroId)`.

### היכן הקוד יושב ומדוע
הכפתורים יושבים ב-`LearnSqlite/Views/HeroDetailsPage.xaml`, ומתודות ה-Update וה-Delete יושבות ב-`LearnSqlite/ViewModels/HeroDetailsViewModel.cs`.

### שינוי הקוד ב-XAML וב-ViewModel

```xml
<!-- מתוך LearnSqlite/Views/HeroDetailsPage.xaml -->
<Grid ColumnDefinitions="*,*"
      ColumnSpacing="12">
    <Button Grid.Column="0"
            Text="💾 עדכן רשומה"
            Command="{Binding UpdateHeroCommand}"
            BackgroundColor="#2563EB"
            TextColor="White"
            FontAttributes="Bold"
            CornerRadius="12"
            HeightRequest="48" />

    <Button Grid.Column="1"
            Text="🗑️ מחק רשומה"
            Command="{Binding DeleteHeroCommand}"
            BackgroundColor="#DC2626"
            TextColor="White"
            FontAttributes="Bold"
            CornerRadius="12"
            HeightRequest="48" />
</Grid>
```

```csharp
// מתוך LearnSqlite/ViewModels/HeroDetailsViewModel.cs
public async Task UpdateHeroAsync()
{
    if (!CanUpdateHero())
    {
        return;
    }

    try
    {
        IsLoading = true;
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
        if (rowsAffected > 0 && Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("..");
        }
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
        int rowsAffected = await _heroService.DeleteHeroAsync(HeroId);
        if (rowsAffected > 0 && Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
    finally
    {
        IsLoading = false;
    }
}
```

### מדוע זה עובד
- ב-`UpdateHeroAsync`, אנו בונים אובייקט `HeroItem` הכולל את **`Id = HeroId`**. זהו פרט קריטי: ב-SQLite, פקודת `UpdateAsync(hero)` מאתרת את השורה לעדכון בטבלה אך ורק לפי ערך ה-`[PrimaryKey]` שלה (`Id`)!
- ב-`DeleteHeroAsync`, אנו מעבירים את `HeroId` אל `_heroService.DeleteHeroAsync(HeroId)` כדי למחוק את הרשומה המתאימה ולחזור למסך האוסף.

![מודל HTML של בלוק 3 — כפתורי עדכון ומחיקה](../Images/HeroDetailsPage-block3-update-delete-actions.png)

### נקודת בקרה לתלמיד

> [!TIP]
> מה היה קורה ב-`UpdateHeroAsync()` אילו היינו שוכחים להציב `Id = HeroId` בתוך האובייקט `updated` שאנו שולחים אל `_heroService.UpdateHeroAsync(updated)`?

### אימות
- שינוי `PowerLevel` של באטמן ל-99 ולחיצה על `💾 עדכן רשומה` מחזירה למסך האוסף ומציגה `⚡ עוצמה: 99/100`.
- לחיצה על `🗑️ מחק רשומה` מוחקת את הגיבור מהארכיון ומקטינה את המונה ב-1.

---

## רפלקציה והצעד הבא

- **החלטה ארכיטקטונית:** ריכזנו את שתי הפעולות התלויות במפתח ראשי קיים (`Update` ו-`Delete`) במסך הפרטים (`HeroDetailsPage`), כך שה הפרדה בין יצירת רשומה חדשה (`AddHeroPage` ללא `Id`) לבין טיפול ברשומה קיימת (`HeroDetailsPage` עם `HeroId`) ברורה לחלוטין לתלמיד.
- **שאלה למחשבה:** אילו שינויים נצטרך לבצע ב-`HeroesListViewModel`, `AddHeroViewModel` ו-`HeroDetailsViewModel` כאשר נחליף את `MockHeroService` ב-`SqliteHeroService`? (רמז: בדקו האם ה-ViewModels תלויים ב-`MockHeroService` או בממשק `IHeroService`!).
- **אתגר קטן:** הוסיפו כפתור "איפוס שינויים" במסך `HeroDetailsPage` הקורא מחדש ל-`LoadHeroByIdAsync(HeroId)` ומחזיר את השדות לערכם השמור במאגר.

## מקורות
- תיעוד רשמי של Microsoft Learn עבור `IQueryAttributable` ב-.NET MAUI Shell.

