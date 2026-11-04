namespace LearnSqlite.Models;

/// <summary>
/// מודל הנתונים המייצג רשומת גיבור-על בארכיון.
/// בשלב הבסיס (Mockup) המחלקה משמשת כ-POCO רגיל בזיכרון.
/// בשלב ה-SQLite נהפוך מחלקה זו לטבלה במסד הנתונים באמצעות תכונות [Table] ו-[PrimaryKey, AutoIncrement].
/// </summary>
public class HeroItem
{
    /// <summary>
    /// מזהה ייחודי של הרשומה (ישמש כמפתח ראשי ב-SQLite).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// שם גיבור-העל (למשל: באטמן, ספיידרמן).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// זהות סודית / שם אמיתי (למשל: ברוס ויין, פיטר פארקר).
    /// </summary>
    public string AlterEgo { get; set; } = string.Empty;

    /// <summary>
    /// הוצאה לאור / יקום קומיקס (Marvel / DC).
    /// </summary>
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
    /// כתובת תמונה ישירה מהאינטרנט (URL תקני ממאגר Superhero API CDN).
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// מאפיין עזר מחושב לתצוגה נקייה ב-XAML של דירוג העוצמה.
    /// </summary>
    public string PowerBadgeText => $"⚡ עוצמה: {PowerLevel}/100";

    /// <summary>
    /// מאפיין עזר מחושב לתצוגת סטטוס השייכות לצוות.
    /// </summary>
    public string EliteStatusText => IsEliteMember ? "⭐ חבר צוות עלית" : "🛡️ פועל עצמאית";
}

