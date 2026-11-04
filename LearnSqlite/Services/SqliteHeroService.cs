using LearnSqlite.Models;
using SQLite;

namespace LearnSqlite.Services;

/// <summary>
/// מימוש מלא של ממשק IHeroService מול מסד נתונים מקומי מסוג SQLite באמצעות sqlite-net-pcl.
/// מחליף את MockHeroService תוך שמירה מלאה על החוזה (Interface) כך שה-ViewModels וה-Views אינם משתנים.
/// </summary>
public class SqliteHeroService : IHeroService
{
    /// <summary>
    /// שם קובץ מסד הנתונים המקומי שיישמר בתיקיית הנתונים של האפליקציה במכשיר.
    /// </summary>
    public const string DatabaseFilename = "HeroesArchive.db3";

    /// <summary>
    /// דגלי הפתיחה המומלצים ל-SQLite ב-.NET MAUI:
    /// קריאה/כתיבה, יצירת הקובץ אם אינו קיים, ושימוש ב-SharedCache לביצועים ובטיחות ב-Multi-threading.
    /// </summary>
    public const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    /// <summary>
    /// הנתיב המלא לקובץ ה-SQLite במערכת הקבצים של הפלטפורמה (Android / iOS / Windows).
    /// </summary>
    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    /// <summary>
    /// חיבור א-סינכרוני משותף לקובץ מסד הנתונים.
    /// מוגדר כ-static כדי שגם ללא Dependency Injection כל המסכים יעבדו מול חיבור יחיד ומסונכרן.
    /// </summary>
    private static SQLiteAsyncConnection? _database;

    /// <summary>
    /// מנעול א-סינכרוני המונע מצב מרוץ (Race Condition) בעת האתחול הראשוני של הטבלה.
    /// </summary>
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    /// <summary>
    /// הבנאי ריק בכוונה — לפי כללי ה-Guardrails של .NET MAUI אין לבצע פעולות קלט/פלט או מסד נתונים בבנאי.
    /// </summary>
    public SqliteHeroService()
    {
    }

    /// <summary>
    /// אתחול עצל (Lazy Async Initialization):
    /// פותח את החיבור לקובץ ה-SQLite, יוצר את טבלת Heroes אם אינה קיימת,
    /// וזורע (Seed) נתוני פתיחה בפעם הראשונה שהאפליקציה רצה.
    /// </summary>
    private async Task InitAsync()
    {
        if (_database is not null)
        {
            return;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_database is not null)
            {
                return;
            }

            _database = new SQLiteAsyncConnection(DatabasePath, Flags);

            // יצירת הטבלה לפי התכונות ([Table], [PrimaryKey], [AutoIncrement]) שהוגדרו ב-HeroItem
            await _database.CreateTableAsync<HeroItem>();

            // אם הטבלה ריקה לחלוטין (הפעלה ראשונה של האפליקציה), נזריע 6 גיבורי-על התחלתיים
            int existingCount = await _database.Table<HeroItem>().CountAsync();
            if (existingCount == 0)
            {
                var starterHeroes = GetSeedHeroes();
                await _database.InsertAllAsync(starterHeroes);
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// שליפת כל גיבורי-העל מטבלת Heroes (SELECT * FROM Heroes).
    /// </summary>
    public async Task<List<HeroItem>> GetAllHeroesAsync()
    {
        await InitAsync();
        return await _database!
            .Table<HeroItem>()
            .OrderByDescending(h => h.PowerLevel)
            .ToListAsync();
    }

    /// <summary>
    /// שליפת גיבור בודד לפי המפתח הראשי שלו (SELECT * FROM Heroes WHERE Id = @id).
    /// </summary>
    public async Task<HeroItem?> GetHeroByIdAsync(int id)
    {
        await InitAsync();
        return await _database!
            .Table<HeroItem>()
            .Where(h => h.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// חיפוש גיבורים במסד הנתונים לפי טקסט בשם, בזהות הסודית או ביקום הקומיקס.
    /// </summary>
    public async Task<List<HeroItem>> SearchHeroesAsync(string searchText)
    {
        await InitAsync();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return await GetAllHeroesAsync();
        }

        string term = searchText.Trim().ToLowerInvariant();

        // שימוש ב-Where על הטבלה ב-SQLite מתורגם לשאילתת SQL עם LIKE
        return await _database!
            .Table<HeroItem>()
            .Where(h =>
                h.Name.ToLower().Contains(term) ||
                h.AlterEgo.ToLower().Contains(term) ||
                h.Publisher.ToLower().Contains(term))
            .OrderByDescending(h => h.PowerLevel)
            .ToListAsync();
    }

    /// <summary>
    /// הוספת רשומת גיבור חדשה לטבלה (INSERT INTO Heroes).
    /// לאחר ההוספה, SQLite מעדכן אוטומטית את המאפיין hero.Id במזהה החדש שנוצר (AutoIncrement).
    /// </summary>
    public async Task<int> AddHeroAsync(HeroItem hero)
    {
        await InitAsync();
        return await _database!.InsertAsync(hero);
    }

    /// <summary>
    /// עדכון רשומת גיבור קיימת בטבלה לפי ה-PrimaryKey שלה (UPDATE Heroes SET ... WHERE Id = @id).
    /// </summary>
    public async Task<int> UpdateHeroAsync(HeroItem hero)
    {
        await InitAsync();
        return await _database!.UpdateAsync(hero);
    }

    /// <summary>
    /// מחיקת רשומת גיבור מהטבלה לפי המפתח הראשי שלה (DELETE FROM Heroes WHERE Id = @id).
    /// </summary>
    public async Task<int> DeleteHeroAsync(int id)
    {
        await InitAsync();
        return await _database!.DeleteAsync<HeroItem>(id);
    }

    /// <summary>
    /// רשימת גיבורי-העל ההתחלתיים שנזרעים לטבלה בהפעלה הראשונה בלבד.
    /// </summary>
    private static List<HeroItem> GetSeedHeroes()
    {
        return new List<HeroItem>
        {
            new HeroItem
            {
                Name = "באטמן (Batman)",
                AlterEgo = "ברוס ויין",
                Publisher = "DC",
                PowerLevel = 88,
                IsEliteMember = true,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/70-batman.jpg"
            },
            new HeroItem
            {
                Name = "ספיידרמן (Spider-Man)",
                AlterEgo = "פיטר פארקר",
                Publisher = "Marvel",
                PowerLevel = 85,
                IsEliteMember = true,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/620-spider-man.jpg"
            },
            new HeroItem
            {
                Name = "איירון מן (Iron Man)",
                AlterEgo = "טוני סטארק",
                Publisher = "Marvel",
                PowerLevel = 92,
                IsEliteMember = true,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/346-iron-man.jpg"
            },
            new HeroItem
            {
                Name = "וונדר וומן (Wonder Woman)",
                AlterEgo = "דיאנה פרינס",
                Publisher = "DC",
                PowerLevel = 95,
                IsEliteMember = true,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/720-wonder-woman.jpg"
            },
            new HeroItem
            {
                Name = "סופרמן (Superman)",
                AlterEgo = "קלארק קנט",
                Publisher = "DC",
                PowerLevel = 99,
                IsEliteMember = true,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/644-superman.jpg"
            },
            new HeroItem
            {
                Name = "הפלאש (The Flash)",
                AlterEgo = "בארי אלן",
                Publisher = "DC",
                PowerLevel = 90,
                IsEliteMember = false,
                ImageUrl = "https://raw.githubusercontent.com/akabab/superhero-api/0.3.0/api/images/md/263-flash.jpg"
            }
        };
    }
}

