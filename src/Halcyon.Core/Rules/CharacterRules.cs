using Halcyon.Core.Model;

namespace Halcyon.Core.Rules;

public static class CharacterRules
{
    public static readonly int[] DieSteps = [4, 6, 8, 10, 12];

    // ---------------------------------------------------------------- Scores

    /// <summary>Roll bonus on a score from powers (Free of fate, Made for purpose, Logic core, Quick).</summary>
    public static int ScoreBonus(IEnumerable<CharacterPower> powers, Score score)
    {
        int bonus = 0;
        foreach (var p in powers)
        {
            var def = Catalog.FindPower(p.PowerId);
            if (def is null) continue;
            if (def.ScoreBonus == score) bonus++;
            if (def.Choice == ChoiceKind.Score && string.Equals(p.Choice, Catalog.ScoreName(score), StringComparison.OrdinalIgnoreCase)) bonus++;
        }
        return bonus;
    }

    public static int ScoreBonus(Character c, Score score) => ScoreBonus(c.Powers, score);

    /// <summary>Checks a die assignment against the two legal arrays and class restrictions.</summary>
    public static List<string> ValidateScores(IReadOnlyDictionary<Score, int> dice, ClassDef? cls)
    {
        var errors = new List<string>();
        var sorted = dice.Values.OrderByDescending(v => v).ToArray();
        if (dice.Count != 5)
            errors.Add("Assign a die to all five scores.");
        else if (!Catalog.ScoreArrays.Any(a => a.SequenceEqual(sorted)))
            errors.Add("Dice must be d12, d10, d8, d6, d6 or d12, d10, d8, d8, d4.");
        if (cls?.RequiresD4 == true && !dice.Values.Contains(4))
            errors.Add($"{cls.Name}: one of your scores must be a d4.");
        return errors;
    }

    // ---------------------------------------------------------------- Pool

    public static int PowerHeaBonus(IEnumerable<CharacterPower> powers) => powers.Sum(p =>
    {
        var def = Catalog.FindPower(p.PowerId);
        if (def is null) return 0;
        int b = def.HeaBonus;
        if (def.Choice == ChoiceKind.HeaOrGrt && p.Choice.Contains("HEA", StringComparison.OrdinalIgnoreCase)) b += 6;
        return b;
    });

    public static int PowerGrtBonus(IEnumerable<CharacterPower> powers) => powers.Sum(p =>
    {
        var def = Catalog.FindPower(p.PowerId);
        if (def is null) return 0;
        int b = def.GrtBonus;
        if (def.Choice == ChoiceKind.HeaOrGrt && p.Choice.Contains("GRT", StringComparison.OrdinalIgnoreCase)) b += 4;
        return b;
    });

    /// <summary>1st-level HEA: maxCOM + maxMOV plus people/power bonuses (score roll bonuses don't count).</summary>
    public static int StartingHea(IReadOnlyDictionary<Score, int> dice, IEnumerable<CharacterPower> powers) =>
        dice[Score.Com] + dice[Score.Mov] + PowerHeaBonus(powers);

    /// <summary>1st-level GRT: maxREA + maxWIL plus power bonuses.</summary>
    public static int StartingGrt(IReadOnlyDictionary<Score, int> dice, IEnumerable<CharacterPower> powers) =>
        dice[Score.Rea] + dice[Score.Wil] + PowerGrtBonus(powers);

    public static int MaxHea(Character c) => c.BaseHea + c.Advancement.Sum(a => a.HeaGain) + c.HeaMaxAdjust;
    public static int MaxGrt(Character c) => c.BaseGrt + c.Advancement.Sum(a => a.GrtGain) + c.GrtMaxAdjust;

    public static int StartingScrip(IReadOnlyDictionary<Score, int> dice) => 10 * (dice[Score.Wil] + dice[Score.Cha]);

    // ---------------------------------------------------------------- Armor

    public static int NaturalDr(Character c) => c.Powers.Sum(p => Catalog.FindPower(p.PowerId)?.NaturalDr ?? 0);

    public static int TotalDr(Character c) => Math.Min(Catalog.MaxNaturalDr, c.Armor.Dr + NaturalDr(c));

    // ---------------------------------------------------------------- Powers

    public static CharacterPower MakePower(PowerDef def, PowerSource source, string origin, string choice = "", int level = 1, bool gainAll = false)
    {
        var p = new CharacterPower
        {
            PowerId = def.Id,
            Name = def.Name,
            Source = source,
            Origin = origin,
            Type = def.Type,
            Cost = def.Cost,
            Summary = def.Summary,
            Choice = choice,
            GainAll = gainAll,
            Drawback = def.Drawback,
            LevelGained = level,
        };

        // Spill-touched (and any Weird choice) takes on the chosen power's type, cost and text.
        if (def.Choice == ChoiceKind.WeirdPower && Catalog.FindWeird(choice) is { } weird)
        {
            p.Choice = weird.Name;
            p.Type = weird.Type;
            p.Cost = weird.Cost;
            p.Summary = weird.Summary;
        }
        return p;
    }

    /// <summary>Powers a character may take when gaining a level.</summary>
    public static LevelUpOptions LevelUpOptions(Character c, int newLevel)
    {
        var owned = c.Powers.Select(p => p.PowerId).ToHashSet();
        var cls = Catalog.FindClass(c.ClassId);
        var classPowers = cls?.Powers.Where(p => !owned.Contains(p.Id)).ToList() ?? [];
        var advanced = Catalog.AdvancedPowers.Where(p => !owned.Contains(p.Id) || p.Id == "enshroud").ToList();
        bool weirdAllowed = newLevel >= 10 || cls?.CanLearnWeird == true;
        var ownedWeird = c.Powers.Where(p => p.Source == PowerSource.Weird || p.PowerId == "spill-touched")
            .Select(p => p.Source == PowerSource.Weird ? p.PowerId : Catalog.FindWeird(p.Choice)?.Id)
            .ToHashSet();
        var weird = weirdAllowed ? Catalog.WeirdPowers.Where(w => !ownedWeird.Contains(w.Id)).ToList() : [];
        var otherClass = Catalog.Classes.Where(k => k.Id != c.ClassId)
            .SelectMany(k => k.Powers.Where(p => !owned.Contains(p.Id) && p.Choice == ChoiceKind.None).Select(p => (Class: k, Power: p)))
            .ToList();
        return new LevelUpOptions(classPowers, advanced, weird, otherClass, weirdAllowed);
    }

    /// <summary>Applies a level-up: +1 level, +6 pool split as chosen, +1 power (optionally giving one up when multiclassing).</summary>
    public static void ApplyLevelUp(Character c, int heaGain, CharacterPower newPower, string? giveUpPowerId)
    {
        if (heaGain < 0 || heaGain > Catalog.PoolPerLevel)
            throw new ArgumentOutOfRangeException(nameof(heaGain), $"HEA gain must be between 0 and {Catalog.PoolPerLevel}.");
        int grtGain = Catalog.PoolPerLevel - heaGain;
        c.Level++;
        newPower.LevelGained = c.Level;

        string givenUp = "";
        if (!string.IsNullOrEmpty(giveUpPowerId))
        {
            var drop = c.Powers.FirstOrDefault(p => p.PowerId == giveUpPowerId && !p.GainAll)
                ?? throw new InvalidOperationException("That power can't be given up.");
            c.Powers.Remove(drop);
            givenUp = drop.DisplayName;
        }

        c.Powers.Add(newPower);
        c.Advancement.Add(new LevelUpRecord
        {
            Level = c.Level,
            HeaGain = heaGain,
            GrtGain = grtGain,
            PowerGained = newPower.DisplayName,
            PowerGivenUp = givenUp,
        });
        c.HeaCurrent += heaGain;
        c.GrtCurrent += grtGain;
        c.Updated = DateTimeOffset.UtcNow;
    }

    // ---------------------------------------------------------------- Gear

    /// <summary>Gear ids in a class's free starting kit, given the player's kit choices and powers.</summary>
    public static List<string> StartingKit(ClassDef cls, IReadOnlyList<int> choiceIndexes, IEnumerable<string> powerIds)
    {
        var ids = new List<string>(cls.KitGearIds);
        for (int i = 0; i < cls.KitChoices.Length; i++)
        {
            int pick = i < choiceIndexes.Count ? choiceIndexes[i] : 0;
            ids.AddRange(cls.KitChoices[i].Options[Math.Clamp(pick, 0, cls.KitChoices[i].Options.Length - 1)].GearIds);
        }
        return ids;
    }

    /// <summary>Extra kit lines that aren't catalog items (conditional and narrative kit).</summary>
    public static List<string> StartingKitExtras(ClassDef cls, IEnumerable<string> powerIds)
    {
        var powers = powerIds.ToHashSet();
        var extras = new List<string>();
        if (cls.Id == "rigger" && powers.Contains("instruments")) extras.Add("Instruments rig (homemade detector)");
        if (cls.Id == "sawbones" && powers.Contains("chems")) extras.Add("Chems kit (d6 + level doses)");
        if (cls.Id == "sensitive") extras.Add("A personal oddment from the Spill");
        return extras;
    }

    /// <summary>Puts a catalog item on the character: weapons and armor in their tables, the rest on the gear lines.</summary>
    public static void AddGear(Character c, GearDef g)
    {
        switch (g.Kind)
        {
            case GearKind.Weapon when g.Weapon is { } w:
                c.Weapons.Add(new Weapon
                {
                    Name = g.Name,
                    Damage = w.Damage,
                    Range = w.Range,
                    Tags = string.IsNullOrEmpty(w.Legality) || w.Legality == "—" ? w.Tags : $"{w.Tags} · {w.Legality.ToLowerInvariant()}",
                    Shots = w.Shots,
                });
                break;
            case GearKind.Armor when g.ArmorDr > c.Armor.Dr || string.IsNullOrEmpty(c.Armor.Name):
                if (!string.IsNullOrEmpty(c.Armor.Name)) c.Gear.Add($"{c.Armor.Name} (DR {c.Armor.Dr})");
                c.Armor = new Armor { Name = g.Name, Dr = g.ArmorDr, Notes = g.Notes };
                break;
            case GearKind.Armor:
                c.Gear.Add($"{g.Name} (DR {g.ArmorDr})");
                break;
            default:
                c.Gear.Add(g.Name);
                break;
        }
    }

    public static string Signed(int n) => n > 0 ? $"+{n}" : n.ToString();
}

public sealed record LevelUpOptions(
    List<PowerDef> ClassPowers,
    List<PowerDef> AdvancedPowers,
    List<PowerDef> WeirdPowers,
    List<(ClassDef Class, PowerDef Power)> OtherClassPowers,
    bool WeirdAllowed);
