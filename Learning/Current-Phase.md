# Current Phase

- **Feature:** ארכיון גיבורי-העל — מעבר מ-`MockHeroService` למסד נתונים מקומי `SQLite` (`SqliteHeroService`)
- **Phase number:** 2
- **Phase title:** חיבור מסד נתונים מקומי SQLite בענף `sqlite`
- **Primary concept:** `sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`, תכונות מיפוי `[Table]` / `[PrimaryKey, AutoIncrement]` / `[Ignore]`, אתחול א-סינכרוני עצל (`InitAsync`), והחלפת מימוש של `IHeroService` ללא שינוי ב-ViewModels
- **Workflow stage:** VERIFICATION_AUDITOR
- **Baton owner:** Agent (Teacher Mode -> Ready to switch back to Student Mode)
- **Approved plan:**
  1. יצירת תיקיית `docs` עם כל המדריכים וספר הלמידה.
  2. פתיחת ענף חדש `sqlite` מתוך `master`.
  3. הוספת חבילות `sqlite-net-pcl` ו-`SQLitePCLRaw.bundle_green` ל-`LearnSqlite.csproj`.
  4. הוספת תכונות SQLite למודל `HeroItem.cs`.
  5. מימוש `SqliteHeroService.cs` המממש את `IHeroService` כולל `InitAsync()` וזריעת נתוני פתיחה (Seeding).
  6. חיבור `SqliteHeroService` בשלושת המסכים (`HeroesListPage`, `AddHeroPage`, `HeroDetailsPage`).
  7. תיעוד שלב ה-SQLite במדריך שלב 4 (`04-SqliteDatabase-Guide.md`), הפקת ספר ה-PDF המעודכן, סנכרון תיקיית `docs`, והחזרת מצב העבודה ל-`MAXIMIZE STUDENT CAPABILITY`.
- **Verification:**
  - `dotnet build`
  - `Validate-Maui-Version.ps1`
  - `Validate-Maui-ObsoleteApis.ps1`
  - `Validate-Maui-XamlResources.ps1`
  - `Generate-Learning-Book-Pdf.py` + `MAUI-Learning-Book.validation.json`
- **Status:** COMPLETE
