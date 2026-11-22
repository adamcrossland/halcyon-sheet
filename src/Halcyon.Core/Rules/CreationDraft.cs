using Halcyon.Core.Model;

namespace Halcyon.Core.Rules;

/// <summary>State of the character-creation wizard. <see cref="Build"/> turns it into a 1st-level character.</summary>
public sealed class CreationDraft
{
    public string Name { get; set; } = "";
    public string Player { get; set; } = "";
    public string Look { get; set; } = "";
    public string Crew { get; set; } = "";
    public string PeopleId { get; set; } = "";
    public string ClassId { get; set; } = "";

    public Dictionary<Score, int> Dice { get; set; } = new();
    public string Quickbuild { get; set; } = "";

    public List<string> ClassPowerIds { get; set; } = [];
    public List<string> PeopleChoiceIds { get; set; } = [];
    /// <summary>Player choices keyed by power id (score for Free of fate, Weird power for Spill-touched…).</summary>
    public Dictionary<string, string> Choices { get; set; } = new();

    public List<int> KitChoices { get; set; } = [];
    public List<string> PurchasedGearIds { get; set; } = [];
    public string Oddment { get; set; } = "";

    public string WhyGoBackIn { get; set; } = "";
    public string WhoIsWaiting { get; set; } = "";
    public string WhatYouSaw { get; set; } = "";
    public string Debts { get; set; } = "";
    public List<Contact> Contacts { get; set; } = [new(), new(), new()];

    public ClassDef? Class => Catalog.FindClass(ClassId);
    public PeopleDef? People => Catalog.FindPeople(PeopleId);

    public string Choice(string powerId) => Choices.TryGetValue(powerId, out var v) ? v : "";

    // ---------------------------------------------------------------- Validation per step

    public List<string> ValidateConcept()
    {
        var e = new List<string>();
        if (string.IsNullOrWhiteSpace(Name)) e.Add("Give your runner a name.");
        if (People is null) e.Add("Choose a people.");
        if (Class is null) e.Add("Choose a class.");
        return e;
    }

    public List<string> ValidateScores() => CharacterRules.ValidateScores(Dice, Class);

    public List<string> ValidateClassPowers()
    {
        var e = new List<string>();
        var cls = Class;
        if (cls is null) return ["Choose a class first."];
        if (ClassPowerIds.Count != Catalog.PowersAtFirstLevel)
            e.Add($"Choose exactly {Catalog.PowersAtFirstLevel} class powers ({ClassPowerIds.Count} chosen).");
        foreach (var req in cls.Powers.Where(p => p.Required && !ClassPowerIds.Contains(p.Id)))
            e.Add($"{cls.Name}s must take {req.Name}.");
        foreach (var p in cls.Powers.Where(p => ClassPowerIds.Contains(p.Id) && p.Choice != ChoiceKind.None))
            if (string.IsNullOrWhiteSpace(Choice(p.Id))) e.Add($"{p.Name}: choose {p.ChoicePrompt.ToLowerInvariant()}.");
        return e;
    }

    public List<string> ValidatePeoplePowers()
    {
        var e = new List<string>();
        var ppl = People;
        if (ppl is null) return ["Choose a people first."];
        if (PeopleChoiceIds.Count != Catalog.PeopleChoices)
            e.Add($"Choose exactly {Catalog.PeopleChoices} people powers ({PeopleChoiceIds.Count} chosen).");
        foreach (var p in ppl.GainAll.Concat(ppl.Choose.Where(p => PeopleChoiceIds.Contains(p.Id))).Where(p => p.Choice != ChoiceKind.None))
            if (string.IsNullOrWhiteSpace(Choice(p.Id))) e.Add($"{p.Name}: choose {p.ChoicePrompt.ToLowerInvariant()}.");
        return e;
    }

    public List<string> ValidateGear() => Scrip < 0 ? [$"You're {-Scrip} scrip over budget."] : [];

    public List<string> ValidateAll() =>
        [.. ValidateConcept(), .. ValidateScores(), .. ValidateClassPowers(), .. ValidatePeoplePowers(), .. ValidateGear()];

    // ---------------------------------------------------------------- Derived values

    public List<CharacterPower> BuildPowers()
    {
        var list = new List<CharacterPower>();
        if (Class is { } cls)
            foreach (var p in cls.Powers.Where(p => ClassPowerIds.Contains(p.Id)))
                list.Add(CharacterRules.MakePower(p, PowerSource.Class, cls.Name, Choice(p.Id)));
        if (People is { } ppl)
        {
            foreach (var p in ppl.GainAll)
                list.Add(CharacterRules.MakePower(p, PowerSource.People, ppl.Name, Choice(p.Id), gainAll: true));
            foreach (var p in ppl.Choose.Where(p => PeopleChoiceIds.Contains(p.Id)))
                list.Add(CharacterRules.MakePower(p, PowerSource.People, ppl.Name, Choice(p.Id)));
        }
        return list;
    }

    public bool ScoresComplete => Dice.Count == 5;

    public int Hea => ScoresComplete ? CharacterRules.StartingHea(Dice, BuildPowers()) : 0;
    public int Grt => ScoresComplete ? CharacterRules.StartingGrt(Dice, BuildPowers()) : 0;
    public int StartingScrip => ScoresComplete ? CharacterRules.StartingScrip(Dice) : 0;
    public int Spent => PurchasedGearIds.Sum(id => Catalog.FindGear(id)?.Price ?? 0);
    public int Scrip => StartingScrip - Spent;

    public List<string> KitGearIds => Class is { } cls ? CharacterRules.StartingKit(cls, KitChoices, ClassPowerIds) : [];

    public void ApplyQuickbuild(Quickbuild qb)
    {
        Quickbuild = qb.Name;
        Dice = qb.Dice;
        ClassPowerIds = [.. qb.PowerIds];
        if (qb.Note?.StartsWith("Spill-touched: ") == true)
        {
            var weird = Catalog.FindWeird(qb.Note["Spill-touched: ".Length..]);
            if (weird is not null) Choices["spill-touched"] = weird.Name;
        }
    }

    // ---------------------------------------------------------------- Build

    public Character Build()
    {
        var errors = ValidateAll();
        if (errors.Count > 0) throw new InvalidOperationException(string.Join(" ", errors));
        var cls = Class!;
        var powers = BuildPowers();

        var c = new Character
        {
            Name = Name.Trim(),
            Player = Player.Trim(),
            Look = Look.Trim(),
            Crew = Crew.Trim(),
            PeopleId = PeopleId,
            ClassId = ClassId,
            Level = 1,
            Burden = cls.BurdenLine,
            Dice = new Dictionary<Score, int>(Dice),
            Powers = powers,
            Oddment = Oddment.Trim(),
            WhyGoBackIn = WhyGoBackIn.Trim(),
            WhoIsWaiting = WhoIsWaiting.Trim(),
            WhatYouSaw = WhatYouSaw.Trim(),
            Debts = Debts.Trim(),
            Contacts = Contacts.Where(x => !string.IsNullOrWhiteSpace(x.Who)).ToList(),
            Scrip = Scrip,
        };
        c.BaseHea = CharacterRules.StartingHea(c.Dice, powers);
        c.BaseGrt = CharacterRules.StartingGrt(c.Dice, powers);
        c.HeaCurrent = c.BaseHea;
        c.GrtCurrent = c.BaseGrt;

        foreach (var def in powers.Select(p => Catalog.FindPower(p.PowerId)).Where(d => d?.GrantsWeapon is not null))
        {
            var w = def!.GrantsWeapon!;
            c.Weapons.Add(new Weapon { Name = def.Name, Damage = w.Damage, Range = w.Range, Tags = w.Tags, Shots = w.Shots });
        }

        foreach (var id in KitGearIds.Concat(PurchasedGearIds))
            if (Catalog.FindGear(id) is { } g) CharacterRules.AddGear(c, g);
        c.Gear.AddRange(CharacterRules.StartingKitExtras(cls, ClassPowerIds));
        c.Gear = c.Gear.GroupBy(g => g).Select(g => g.Count() > 1 ? $"{g.Key} ×{g.Count()}" : g.Key).ToList();

        if (PeopleId == "cultivar")
        {
            var maker = Choice("maker");
            if (!string.IsNullOrWhiteSpace(maker)) c.Notes = $"Maker: {maker.Trim()}";
        }
        return c;
    }
}
