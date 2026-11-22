using Halcyon.Core.Model;

namespace Halcyon.Core.Rules;

public enum ChoiceKind
{
    None,
    /// <summary>Pick a score (Free of fate, Made for purpose).</summary>
    Score,
    /// <summary>Free text (Political, Imprinted skill, Built-in tool…).</summary>
    Text,
    /// <summary>+6 max HEA or +4 max GRT (Protagonistic).</summary>
    HeaOrGrt,
    /// <summary>Pick a Weird (Spill-touch) power (Spill-touched).</summary>
    WeirdPower,
}

public sealed record PowerDef(
    string Id,
    string Name,
    string Type,
    string Cost,
    string Summary)
{
    public bool Marquee { get; init; }
    public bool Required { get; init; }
    public bool Drawback { get; init; }
    public ChoiceKind Choice { get; init; }
    public string ChoicePrompt { get; init; } = "";
    /// <summary>Fixed score bonus (Logic core: +1 REA).</summary>
    public Score? ScoreBonus { get; init; }
    public int HeaBonus { get; init; }
    public int GrtBonus { get; init; }
    public int NaturalDr { get; init; }
    /// <summary>Weapon the power grants, shown in the weapons table.</summary>
    public WeaponDef? GrantsWeapon { get; init; }
    /// <summary>The power text is in the Grit System SRD rather than the Halcyon docs.</summary>
    public string? SrdSource { get; init; }
}

public sealed record Quickbuild(string Name, int Com, int Mov, int Rea, int Wil, int Cha, string[] PowerIds, string? Note = null)
{
    public Dictionary<Score, int> Dice => new()
    {
        [Score.Com] = Com, [Score.Mov] = Mov, [Score.Rea] = Rea, [Score.Wil] = Wil, [Score.Cha] = Cha,
    };
}

/// <summary>One choice inside a class starting kit ("a service revolver or an autopistol").</summary>
public sealed record KitChoice(string Prompt, KitOption[] Options);

public sealed record KitOption(string Label, string[] GearIds);

public sealed record ClassDef(
    string Id,
    string Name,
    string Role,
    string Blurb,
    string BurdenName,
    string BurdenText,
    PowerDef[] Powers,
    Quickbuild[] Quickbuilds,
    string[] KitGearIds,
    KitChoice[] KitChoices)
{
    public string[] Restrictions { get; init; } = [];
    /// <summary>One score must be a d4 (Sensitive).</summary>
    public bool RequiresD4 { get; init; }
    /// <summary>May take a Weird power instead of a class power at any level-up (Sensitive).</summary>
    public bool CanLearnWeird { get; init; }
    public string BurdenLine => $"{BurdenName}. {BurdenText}";
}

public sealed record PeopleDef(
    string Id,
    string Name,
    string Kind,
    string Blurb,
    PowerDef[] GainAll,
    PowerDef[] Choose)
{
    public string BestAt { get; init; } = "";
    public string WorstAt { get; init; } = "";
}

public enum GearKind { Weapon, Armor, Kit, Vehicle, Service }

public sealed record WeaponDef(string Damage, string Range, string Tags, string Legality, string Shots);

public sealed record GearDef(string Id, string Name, GearKind Kind, int? Price, string Notes)
{
    public WeaponDef? Weapon { get; init; }
    public int ArmorDr { get; init; }
    public string Legality { get; init; } = "";
    public string Category { get; init; } = "";
}
