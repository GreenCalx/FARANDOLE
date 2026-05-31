using System.Collections.Generic;

/// Tracks how many of each projectile type a pot still needs.
/// Auto-load order is Almond before Water (see NextNeeded).
public class PotFill
{
    static readonly ProjectileType[] Order = { ProjectileType.Almond, ProjectileType.Water };

    readonly Dictionary<ProjectileType, int> required = new();
    readonly Dictionary<ProjectileType, int> current = new();

    public PotFill(IReadOnlyDictionary<ProjectileType, int> requirements)
    {
        foreach (var kv in requirements)
        {
            required[kv.Key] = kv.Value;
            current[kv.Key] = 0;
        }
    }

    public static PotFill Simple()
        => new PotFill(new Dictionary<ProjectileType, int> { { ProjectileType.Almond, 1 } });

    public static PotFill Watered()
        => new PotFill(new Dictionary<ProjectileType, int>
           { { ProjectileType.Almond, 1 }, { ProjectileType.Water, 1 } });

    public bool Accepts(ProjectileType t)
        => required.TryGetValue(t, out int need) && current[t] < need;

    public bool TryAdd(ProjectileType t)
    {
        if (!Accepts(t)) return false;
        current[t]++;
        return true;
    }

    public ProjectileType? NextNeeded()
    {
        foreach (var t in Order)
            if (Accepts(t)) return t;
        return null;
    }

    public bool IsFertilized
    {
        get
        {
            foreach (var kv in required)
                if (current[kv.Key] < kv.Value) return false;
            return true;
        }
    }
}
