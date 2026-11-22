using Halcyon.Core.Model;
using Halcyon.Core.Pdf;
using Halcyon.Core.Rules;
using Halcyon.Core.Storage;

namespace Halcyon.Tests;

/// <summary>Rebuilds the Slow Water pregens with the wizard model and checks the numbers match the scenario.</summary>
public class PregenTests
{
    internal static CreationDraft Dace()
    {
        var d = new CreationDraft { Name = "Dace Holloway", Player = "Test", PeopleId = "human", ClassId = "guide", Oddment = Catalog.Oddments[0] };
        d.ApplyQuickbuild(d.Class!.Quickbuilds[0]);
        d.PeopleChoiceIds = ["runners-family", "protagonistic"];
        d.Choices["protagonistic"] = "+4 max GRT";
        d.PurchasedGearIds = ["service-revolver", "rope", "chemlights"];
        return d;
    }

    internal static CreationDraft Ash()
    {
        var d = new CreationDraft { Name = "Ash", PeopleId = "cultivar", ClassId = "gun" };
        d.ApplyQuickbuild(d.Class!.Quickbuilds[1]);
        d.ClassPowerIds = ["fan-the-hammer", "quickdraw", "cover-me", "take-the-bullet"];
        d.Choices["made-for-purpose"] = "COM";
        d.Choices["maker"] = "Verrin";
        d.PeopleChoiceIds = ["pain-dampers", "pheromones"];
        d.KitChoices = [0, 0];
        d.PurchasedGearIds = ["knuckledusters", "gas-mask", "shelf-life-dose"];
        return d;
    }

    internal static CreationDraft Sennet()
    {
        var d = new CreationDraft { Name = "Sennet", PeopleId = "quill", ClassId = "rigger" };
        d.ApplyQuickbuild(d.Class!.Quickbuilds[0]);
        d.ClassPowerIds = ["make-it-sing", "field-theory", "instruments", "jury-rig"];
        d.PeopleChoiceIds = ["storm-sense", "wide-eyes"];
        d.PurchasedGearIds = ["derringer", "lead-lined-case", "gas-mask"];
        return d;
    }

    internal static CreationDraft Owl()
    {
        var d = new CreationDraft { Name = "Dr. Odile Marchand (Owl)", PeopleId = "spillborn", ClassId = "sawbones" };
        d.ApplyQuickbuild(d.Class!.Quickbuilds[0]);
        d.PeopleChoiceIds = ["dark-eyes", "snag-sense"];
        d.PurchasedGearIds = ["pocket-revolver", "heavy-long-coat", "lead-lined-case", "body-bag"];
        return d;
    }

    [Theory]
    [InlineData("Dace", 16, 24, 93)]
    [InlineData("Ash", 26, 16, 121)]
    [InlineData("Sennet", 10, 22, 55)]
    [InlineData("Owl", 12, 22, 58)]
    public void Pregens_match_scenario(string who, int hea, int grt, int scrip)
    {
        var draft = who switch { "Dace" => Dace(), "Ash" => Ash(), "Sennet" => Sennet(), _ => Owl() };
        Assert.Empty(draft.ValidateAll());
        var c = draft.Build();
        Assert.Equal(hea, CharacterRules.MaxHea(c));
        Assert.Equal(grt, CharacterRules.MaxGrt(c));
        Assert.Equal(scrip, c.Scrip);
    }

    [Fact]
    public void Ash_has_plus_one_com_and_vest()
    {
        var c = Ash().Build();
        Assert.Equal(1, CharacterRules.ScoreBonus(c, Score.Com));
        Assert.Equal("Ballistic vest", c.Armor.Name);
        Assert.Equal(2, CharacterRules.TotalDr(c));
        Assert.Contains(c.Weapons, w => w.Name == "Service revolver");
    }

    [Fact]
    public void Sensitive_requires_d4_and_spill_touched()
    {
        var d = new CreationDraft { Name = "X", PeopleId = "human", ClassId = "sensitive" };
        d.Dice = new() { [Score.Com] = 12, [Score.Mov] = 10, [Score.Rea] = 8, [Score.Wil] = 6, [Score.Cha] = 6 };
        d.ClassPowerIds = ["listen", "lure", "static", "echo"];
        Assert.NotEmpty(d.ValidateScores());
        Assert.Contains(d.ValidateClassPowers(), e => e.Contains("Spill-touched"));

        d.ApplyQuickbuild(d.Class!.Quickbuilds[0]);
        Assert.Empty(d.ValidateScores());
        Assert.Empty(d.ValidateClassPowers());
        var p = d.BuildPowers().Single(x => x.PowerId == "spill-touched");
        Assert.Equal("Slip", p.Choice);
        Assert.Equal("3/6/9 GRT", p.Cost);
    }

    [Fact]
    public void Level_up_adds_pool_and_power_and_supports_multiclass()
    {
        var c = Dace().Build();
        var opts = CharacterRules.LevelUpOptions(c, 2);
        Assert.DoesNotContain(opts.ClassPowers, p => p.Id == "throw-a-bolt");
        Assert.False(opts.WeirdAllowed);

        CharacterRules.ApplyLevelUp(c, 2, CharacterRules.MakePower(opts.ClassPowers.First(p => p.Id == "sleep-rough"), PowerSource.Class, "Guide"), null);
        Assert.Equal(2, c.Level);
        Assert.Equal(18, CharacterRules.MaxHea(c));
        Assert.Equal(28, CharacterRules.MaxGrt(c));

        var gunPower = Catalog.FindClass("gun")!.Powers.First(p => p.Id == "quickdraw");
        CharacterRules.ApplyLevelUp(c, 6, CharacterRules.MakePower(gunPower, PowerSource.OtherClass, "Gun"), "gut-feeling");
        Assert.Equal(3, c.Level);
        Assert.DoesNotContain(c.Powers, p => p.PowerId == "gut-feeling");
        Assert.Equal("Gut feeling", c.Advancement[^1].PowerGivenUp);
        Assert.Equal(24, CharacterRules.MaxHea(c));
    }

    [Fact]
    public void Weird_allowed_at_level_ten()
    {
        var c = Dace().Build();
        c.Level = 9;
        Assert.True(CharacterRules.LevelUpOptions(c, 10).WeirdAllowed);
    }

    [Fact]
    public void Store_round_trips()
    {
        var dir = Path.Combine(Path.GetTempPath(), "halcyon-test-" + Guid.NewGuid().ToString("N"));
        try
        {
            var store = new CharacterStore(dir);
            var c = Ash().Build();
            store.Save(c);
            var back = store.Get(c.Id)!;
            Assert.Equal(c.Name, back.Name);
            Assert.Equal(c.Dice[Score.Com], back.Dice[Score.Com]);
            Assert.Equal(c.Powers.Count, back.Powers.Count);
            var imported = store.Import(CharacterStore.Export(c));
            Assert.NotEqual(c.Id, imported.Id);
            Assert.Equal(2, store.List().Count);
        }
        finally { Directory.Delete(dir, true); }
    }
}

public class PdfTests
{
    [Fact]
    public void Layout_has_all_fields()
    {
        var keys = SheetLayout.Default.Keys;
        Assert.Contains("name", keys);
        Assert.Contains("notes3", keys);
        Assert.Equal(201, keys.Count);
    }

    [Fact]
    public void Renders_two_page_pdf_and_overflow_page()
    {
        var c = PregenTests.Dace().Build();
        c.Exposure = 4;
        c.SpillMarks = 2;
        c.Notes = "Owes Vera Ostrow a conversation. " + string.Concat(Enumerable.Repeat("Long note text that keeps going. ", 40));
        var bytes = new SheetRenderer().Render(c);
        Assert.True(bytes.Length > 10_000);
        Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));

        using var doc = PdfSharp.Pdf.IO.PdfReader.Open(new MemoryStream(bytes), PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
        Assert.Equal(3, doc.PageCount);

        var outDir = Environment.GetEnvironmentVariable("HALCYON_SAMPLE_DIR");
        if (!string.IsNullOrEmpty(outDir))
        {
            File.WriteAllBytes(Path.Combine(outDir, "dace.pdf"), bytes);
            var ash = PregenTests.Ash().Build();
            ash.Leavings.Add(new Leaving { Name = "Warm stone", Strangeness = "1", Tuned = true, Use = "Always warm", Quirk = "Worth 5 scrip" });
            File.WriteAllBytes(Path.Combine(outDir, "ash.pdf"), new SheetRenderer().Render(ash));
        }
    }
}
