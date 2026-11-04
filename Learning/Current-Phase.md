# Current Phase

- **Feature:** בניית פרויקט הבסיס (ענף `master`) ללמידת SQLite ב-.NET MAUI — ארכיון גיבורי-העל (`LearnSqlite`)
- **Phase number:** 1
- **Phase title:** תשתית MVVM מלאה (3 מסכים ב-AppShell, ממשק שירות CRUD א-סינכרוני ומימוש Mockup בזיכרון ללא DI)
- **Primary concept:** הפרדת שכבות (Model, Service Interface + Mockup, ViewModel, View) וניווט Shell רב-מסכי כהכנה להחלפת השירות ב-SQLite
- **Workflow stage:** VERIFICATION_AUDITOR
- **Baton owner:** Instructor / Agent (Mode temporarily set to `MAXIMIZE CODE GENERATED` for teacher baseline build, then restored to `MAXIMIZE STUDENT CAPABILITY`)
- **Student decisions so far:**
  - נושא הפרויקט: ארכיון גיבורי-העל (`HeroItem`) עם תמונות חיות ותקינות ממאגר אינטרנטי (`Superhero API CDN`).
  - 3 מסכים ב-`AppShell`: מסך צפייה באוסף (`HeroesListPage`), מסך הוספה (`AddHeroPage`), ומסך פרטי/עדכון/מחיקת רשומה (`HeroDetailsPage`).
  - ארכיטקטורת MVVM לפי כללי ה-Guardrails, ללא DI בשלב זה, עם ממשק `IHeroService` המכיל את כל מתודות ה-CRUD הא-סינכרוניות ומימוש `MockHeroService`.
- **Verification:**
  - `dotnet build` עובר בהצלחה מלאה עם 0 אזהרות ו-0 שגיאות.
  - כל סקריפטי האימות (`Validate-Maui-Version`, `Validate-Maui-ObsoleteApis`, `Validate-Maui-XamlResources`, `Validate-Learning-Book`) עוברים בסטטוס `PASS`.
- **Status:** COMPLETE
