using LearnSqlite.Models;

namespace LearnSqlite.Services;

/// <summary>
/// חוזה (Interface) עבור שירות הנתונים של ארכיון גיבורי-העל.
/// מגדיר את כל פעולות ה-CRUD הא-סינכרוניות שיידרשו בעבודה מול SQLite:
/// שליפת כל הרשומות, שליפה לפי מזהה, סינון/חיפוש, הוספה, עדכון ומחיקה.
/// </summary>
public interface IHeroService
{
    /// <summary>
    /// שליפת כל גיבורי-העל מהמאגר (מקביל ל-SELECT * FROM Heroes).
    /// </summary>
    Task<List<HeroItem>> GetAllHeroesAsync();

    /// <summary>
    /// שליפת גיבור בודד לפי המזהה הייחודי שלו (מקביל ל-SELECT * FROM Heroes WHERE Id = @id).
    /// </summary>
    Task<HeroItem?> GetHeroByIdAsync(int id);

    /// <summary>
    /// חיפוש גיבורים לפי טקסט בשם או בזהות הסודית (מקביל ל-WHERE Name LIKE ...).
    /// </summary>
    Task<List<HeroItem>> SearchHeroesAsync(string searchText);

    /// <summary>
    /// הוספת רשומת גיבור חדשה למאגר (מקביל ל-INSERT INTO Heroes).
    /// מחזיר את מספר השורות שהושפעו (1 בהצלחה).
    /// </summary>
    Task<int> AddHeroAsync(HeroItem hero);

    /// <summary>
    /// עדכון רשומת גיבור קיימת במאגר (מקביל ל-UPDATE Heroes SET ... WHERE Id = @id).
    /// מחזיר את מספר השורות שהושפעו (1 בהצלחה).
    /// </summary>
    Task<int> UpdateHeroAsync(HeroItem hero);

    /// <summary>
    /// מחיקת רשומת גיבור לפי מזהה (מקביל ל-DELETE FROM Heroes WHERE Id = @id).
    /// מחזיר את מספר השורות שהושפעו (1 בהצלחה).
    /// </summary>
    Task<int> DeleteHeroAsync(int id);
}

