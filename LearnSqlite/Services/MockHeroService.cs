using LearnSqlite.Models;

namespace LearnSqlite.Services;

/// <summary>
/// מימוש דמה (Mockup) של ממשק IHeroService השומר את הנתונים ברשימה סטטית בזיכרון.
/// מאפשר לבנות ולבדוק את כל 3 המסכים וזרימת ה-MVVM לפני התקנת חבילת SQLite.
/// </summary>
public class MockHeroService : IHeroService
{
    /// <summary>
    /// רשימה סטטית משותפת בזיכרון כדי שכל המסכים יראו את אותם הנתונים גם ללא DI.
    /// כל כתובות ה-ImageUrl נבדקו ומפנות לשרת ה-CDN הציבורי של מאגר Superhero-API.
    /// </summary>
    private static readonly List<HeroItem> _heroes = new()
    {
        new HeroItem
        {
            Id = 1,
            Name = "באטמן (Batman)",
            AlterEgo = "ברוס ויין",
            Publisher = "DC",
            PowerLevel = 88,
            IsEliteMember = true,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/70-batman.jpg"
        },
        new HeroItem
        {
            Id = 2,
            Name = "ספיידרמן (Spider-Man)",
            AlterEgo = "פיטר פארקר",
            Publisher = "Marvel",
            PowerLevel = 85,
            IsEliteMember = true,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/620-spider-man.jpg"
        },
        new HeroItem
        {
            Id = 3,
            Name = "איירון מן (Iron Man)",
            AlterEgo = "טוני סטארק",
            Publisher = "Marvel",
            PowerLevel = 92,
            IsEliteMember = true,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/346-iron-man.jpg"
        },
        new HeroItem
        {
            Id = 4,
            Name = "וונדר וומן (Wonder Woman)",
            AlterEgo = "דיאנה פרינס",
            Publisher = "DC",
            PowerLevel = 95,
            IsEliteMember = true,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/720-wonder-woman.jpg"
        },
        new HeroItem
        {
            Id = 5,
            Name = "סופרמן (Superman)",
            AlterEgo = "קלארק קנט",
            Publisher = "DC",
            PowerLevel = 99,
            IsEliteMember = true,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/644-superman.jpg"
        },
        new HeroItem
        {
            Id = 6,
            Name = "הפלאש (The Flash)",
            AlterEgo = "בארי אלן",
            Publisher = "DC",
            PowerLevel = 90,
            IsEliteMember = false,
            ImageUrl = "https://cdn.jsdelivr.net/gh/akabab/superhero-api@0.3.0/api/images/md/263-flash.jpg"
        }
    };

    private static int _nextId = 7;

    public async Task<List<HeroItem>> GetAllHeroesAsync()
    {
        await Task.Delay(150);
        return _heroes
            .Select(CloneHero)
            .ToList();
    }

    public async Task<HeroItem?> GetHeroByIdAsync(int id)
    {
        await Task.Delay(100);
        var found = _heroes.FirstOrDefault(h => h.Id == id);
        return found is null ? null : CloneHero(found);
    }

    public async Task<List<HeroItem>> SearchHeroesAsync(string searchText)
    {
        await Task.Delay(100);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return await GetAllHeroesAsync();
        }

        string term = searchText.Trim();
        return _heroes
            .Where(h =>
                h.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                h.AlterEgo.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                h.Publisher.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(CloneHero)
            .ToList();
    }

    public async Task<int> AddHeroAsync(HeroItem hero)
    {
        await Task.Delay(150);
        var copy = CloneHero(hero);
        copy.Id = _nextId++;
        _heroes.Add(copy);
        hero.Id = copy.Id;
        return 1;
    }

    public async Task<int> UpdateHeroAsync(HeroItem hero)
    {
        await Task.Delay(150);
        var existing = _heroes.FirstOrDefault(h => h.Id == hero.Id);
        if (existing is null)
        {
            return 0;
        }

        existing.Name = hero.Name;
        existing.AlterEgo = hero.AlterEgo;
        existing.Publisher = hero.Publisher;
        existing.PowerLevel = hero.PowerLevel;
        existing.IsEliteMember = hero.IsEliteMember;
        existing.ImageUrl = hero.ImageUrl;
        return 1;
    }

    public async Task<int> DeleteHeroAsync(int id)
    {
        await Task.Delay(150);
        var existing = _heroes.FirstOrDefault(h => h.Id == id);
        if (existing is null)
        {
            return 0;
        }

        _heroes.Remove(existing);
        return 1;
    }

    private static HeroItem CloneHero(HeroItem source)
    {
        return new HeroItem
        {
            Id = source.Id,
            Name = source.Name,
            AlterEgo = source.AlterEgo,
            Publisher = source.Publisher,
            PowerLevel = source.PowerLevel,
            IsEliteMember = source.IsEliteMember,
            ImageUrl = source.ImageUrl
        };
    }
}
