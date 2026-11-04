using SQLite;

namespace LearnSqlite.Models;

/// <summary>
/// מודל הנתונים המייצג רשומת גיבור-על בטבלת Heroes במסד הנתונים SQLite.
/// </summary>
[Table("Heroes")]
public class HeroItem
{
    /// <summary>
    /// מזהה ייחודי של הרשומה — מפתח ראשי (PrimaryKey) עם מספור אוטומטי (AutoIncrement) ב-SQLite.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// שם גיבור-העל (למשל: באטמן, ספיידרמן).
    /// </summary>
    [MaxLength(100), NotNull]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// זהות סודית / שם אמיתי (למשל: ברוס ויין, פיטר פארקר).
    /// </summary>
    [MaxLength(100)]
    public string AlterEgo { get; set; } = string.Empty;

    /// <summary>
    /// הוצאה לאור / יקום קומיקס (Marvel / DC).
    /// </summary>
    [MaxLength(50)]
    public string Publisher { get; set; } = "Marvel";

    /// <summary>
    /// דירוג עוצמה מספרי (1 עד 100) — מדגים שמירה וסינון של שדה מספרי (INTEGER).
    /// </summary>
    public int PowerLevel { get; set; } = 75;

    /// <summary>
    /// האם חבר בצוות עלית (הנוקמים / ליגת הצדק) — מדגים שדה בוליאני.
    /// </summary>
    public bool IsEliteMember { get; set; }

    /// <summary>
    /// כתובת תמונה ישירה מהאינטרנט (URL תקני ממאגר Superhero API).
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// מאפיין עזר מחושב לתצוגה נקייה ב-XAML של דירוג העוצמה (מתעלמים ממנו ביצירת עמודות בטבלה).
    /// </summary>
    [Ignore]
    public string PowerBadgeText => $"⚡ עוצמה: {PowerLevel}/100";

    /// <summary>
    /// מאפיין עזר מחושב לתצוגת סטטוס השייכות לצוות (מתעלמים ממנו ביצירת עמודות בטבלה).
    /// </summary>
    [Ignore]
    public string EliteStatusText => IsEliteMember ? "⭐ חבר צוות עלית" : "🛡️ פועל עצמאית";
}

