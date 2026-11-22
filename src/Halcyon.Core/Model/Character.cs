namespace Halcyon.Core.Model;

public enum Score { Com, Mov, Rea, Wil, Cha }

public enum PowerSource { Class, People, Advanced, Weird, OtherClass, Custom }

/// <summary>A runner's file: everything that goes on the character sheet.</summary>
public sealed class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;

    // Identity
    public string Name { get; set; } = "";
    public string Player { get; set; } = "";
    public string PeopleId { get; set; } = "";
    public string ClassId { get; set; } = "";
    public int Level { get; set; } = 1;
    public string Look { get; set; } = "";
    public string Crew { get; set; } = "";
    public string Burden { get; set; } = "";

    // Scores: die size (4,6,8,10,12) per score. Bonuses come from powers.
    public Dictionary<Score, int> Dice { get; set; } = new()
    {
        [Score.Com] = 6, [Score.Mov] = 6, [Score.Rea] = 6, [Score.Wil] = 6, [Score.Cha] = 6,
    };

    /// <summary>Pool maximums before level-ups, fixed at creation.</summary>
    public int BaseHea { get; set; }
    public int BaseGrt { get; set; }
    /// <summary>Manual changes to max HEA/GRT (e.g. −1 HEA for coming back from 0).</summary>
    public int HeaMaxAdjust { get; set; }
    public int GrtMaxAdjust { get; set; }
    public int HeaCurrent { get; set; }
    public int GrtCurrent { get; set; }

    public List<CharacterPower> Powers { get; set; } = [];
    public List<LevelUpRecord> Advancement { get; set; } = [];

    // The Spill
    public int Exposure { get; set; }
    public int SpillMarks { get; set; }
    public string SpillMarkNote { get; set; } = "";
    public List<string> WhatTheSpillTook { get; set; } = [];

    public Conditions Conditions { get; set; } = new();

    // Gear
    public List<Weapon> Weapons { get; set; } = [];
    public string Reloads { get; set; } = "";
    public Armor Armor { get; set; } = new();
    public List<string> Gear { get; set; } = [];
    public string Oddment { get; set; } = "";
    public List<Leaving> Leavings { get; set; } = [];

    // Money & obligations
    public int Scrip { get; set; }
    public string Stashed { get; set; } = "";
    public string Debts { get; set; } = "";
    public string Permits { get; set; } = "";
    public List<Contact> Contacts { get; set; } = [];

    // The file on you
    public string WhyGoBackIn { get; set; } = "";
    public string WhoIsWaiting { get; set; } = "";
    public string WhatYouSaw { get; set; } = "";
    public string Notes { get; set; } = "";
}

public sealed class CharacterPower
{
    public string PowerId { get; set; } = "";
    public string Name { get; set; } = "";
    public PowerSource Source { get; set; }
    /// <summary>Class/people the power came from (for multiclassing and display).</summary>
    public string Origin { get; set; } = "";
    public string Type { get; set; } = "";
    public string Cost { get; set; } = "";
    public string Summary { get; set; } = "";
    /// <summary>Player's choice for powers that need one (a score, a talent, a Weird power…).</summary>
    public string Choice { get; set; } = "";
    public bool GainAll { get; set; }
    public bool Drawback { get; set; }
    public int LevelGained { get; set; } = 1;

    public string DisplayName => string.IsNullOrWhiteSpace(Choice) ? Name : $"{Name} ({Choice})";
}

public sealed class LevelUpRecord
{
    public int Level { get; set; }
    public int HeaGain { get; set; }
    public int GrtGain { get; set; }
    public string PowerGained { get; set; } = "";
    public string PowerGivenUp { get; set; } = "";
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Conditions
{
    public bool Prone { get; set; }
    public string Bleed { get; set; } = "";
    public string Burn { get; set; } = "";
    public string Stun { get; set; } = "";
    public string Fatigue { get; set; } = "";
    public string Toxin { get; set; } = "";
    public string Groggy { get; set; } = "";
    public string LostMaxGrt { get; set; } = "";
}

public sealed class Weapon
{
    public string Name { get; set; } = "";
    public string Damage { get; set; } = "";
    public string Range { get; set; } = "";
    public string Tags { get; set; } = "";
    public string Shots { get; set; } = "";
}

public sealed class Armor
{
    public string Name { get; set; } = "";
    public int Dr { get; set; }
    public string Notes { get; set; } = "";
}

public sealed class Leaving
{
    public string Name { get; set; } = "";
    public string Strangeness { get; set; } = "";
    public bool Tuned { get; set; }
    public bool Packed { get; set; }
    public string Use { get; set; } = "";
    public string Quirk { get; set; } = "";
}

public sealed class Contact
{
    public string Who { get; set; } = "";
    public string OwesOrWants { get; set; } = "";
    public bool Used { get; set; }
}
