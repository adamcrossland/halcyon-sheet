using System.Reflection;
using Halcyon.Core.Model;
using Halcyon.Core.Rules;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Halcyon.Core.Pdf;

public sealed record SheetOptions
{
    /// <summary>Print current HEA/GRT, exposure, conditions and "used" boxes. Off gives a clean sheet for pencil tracking.</summary>
    public bool IncludeTracking { get; init; } = true;
}

/// <summary>Fills the flat Character Sheet.pdf template by drawing text over its field boxes.</summary>
public sealed class SheetRenderer
{
    static readonly XColor Ink = XColor.FromArgb(24, 36, 92);
    static readonly XBrush InkBrush = new XSolidBrush(Ink);
    static readonly XPen InkPen = new(Ink, 1.1);

    // Widths of the printed labels inside power cards (measured from the template).
    const double PowerLabel = 24.44, TypeLabel = 16.77, CostLabel = 17.18, GainAllLabel = 32.13, DrawbackLabel = 77.93, ChosenLabel = 27.94;
    const double PipSpacing = 10.08, PipStart = 2.88, PipSize = 8.64;
    const double ReloadsLabel = 66;
    // How far above the rule power-card text sits (points). Descenders must clear the underline.
    const double PowerLineLift = 3.6, PowerHeaderLift = 2.4;

    readonly SheetLayout _layout;
    readonly Func<Stream> _openTemplate;

    public SheetRenderer(SheetLayout? layout = null, Func<Stream>? openTemplate = null)
    {
        _layout = layout ?? SheetLayout.Default;
        _openTemplate = openTemplate ?? OpenEmbeddedTemplate;
        SheetFontResolver.EnsureInstalled();
    }

    public static Stream OpenEmbeddedTemplate() =>
        Assembly.GetExecutingAssembly().GetManifestResourceStream("Halcyon.Core.Assets.CharacterSheet.pdf")
        ?? throw new InvalidOperationException("Embedded CharacterSheet.pdf is missing.");

    public byte[] Render(Character c, SheetOptions? options = null)
    {
        options ??= new SheetOptions();
        using var template = _openTemplate();
        using var input = new MemoryStream();
        template.CopyTo(input);
        input.Position = 0;
        using var doc = PdfReader.Open(input, PdfDocumentOpenMode.Modify);
        if (doc.PageCount < 2) throw new InvalidOperationException("The character sheet template must have two pages.");

        // Browser-printed PDFs can leave graphics state (transparency, clipping, transforms) set at the end of
        // their content stream. Wrap the template's content in q/Q so our drawing starts from a clean state.
        foreach (var page in doc.Pages)
        {
            page.Contents.PrependContent().CreateStream("q\n"u8.ToArray());
            page.Contents.AppendContent().CreateStream("Q\n"u8.ToArray());
        }

        var overflow = new List<(string Heading, List<string> Lines)>();
        using (var g1 = XGraphics.FromPdfPage(doc.Pages[0], XGraphicsPdfPageOptions.Append))
            DrawPage1(new Pen(g1, _layout), c, options, overflow);
        using (var g2 = XGraphics.FromPdfPage(doc.Pages[1], XGraphicsPdfPageOptions.Append))
            DrawPage2(new Pen(g2, _layout), c, options, overflow);

        if (overflow.Count > 0) DrawOverflowPages(doc, c, overflow);

        doc.Info.Title = $"Halcyon — {c.Name}";
        doc.Info.Subject = "Runner's File";
        using var output = new MemoryStream();
        doc.Save(output, false);
        return output.ToArray();
    }

    // ---------------------------------------------------------------- Page 1

    static void DrawPage1(Pen p, Character c, SheetOptions o, List<(string, List<string>)> overflow)
    {
        var people = Catalog.FindPeople(c.PeopleId);
        var cls = Catalog.FindClass(c.ClassId);

        p.Field("name", c.Name);
        p.Field("people", people?.Name ?? c.PeopleId);
        p.Field("class", cls?.Name ?? c.ClassId);
        p.Field("level", c.Level.ToString());
        p.Field("look", c.Look);
        p.Field("crew", c.Crew);
        p.Field("player", c.Player);
        p.Field("burden", c.Burden, size: 8, min: 5.5);

        foreach (var (score, _, _) in Catalog.ScoreInfo)
        {
            var key = score.ToString().ToLowerInvariant();
            int die = c.Dice.GetValueOrDefault(score, 0);
            if (die > 0) p.Centered(p.Box($"{key}.die") with { H = 22 }, die.ToString(), 13, bold: true);
            int bonus = CharacterRules.ScoreBonus(c, score);
            if (bonus != 0) p.Right($"{key}.bonus", CharacterRules.Signed(bonus), 9, bold: true);
            if (die > 0) p.Right($"{key}.max", die.ToString(), 9, bold: true);
        }

        int maxHea = CharacterRules.MaxHea(c), maxGrt = CharacterRules.MaxGrt(c);
        p.Centered(p.Box("hea.max") with { Y = p.Box("hea.max").Y + 8, H = 28 }, maxHea.ToString(), 15, bold: true);
        p.Centered(p.Box("grt.max") with { Y = p.Box("grt.max").Y + 8, H = 28 }, maxGrt.ToString(), 15, bold: true);
        if (o.IncludeTracking)
        {
            p.Centered(p.Box("hea.cur") with { Y = p.Box("hea.cur").Y + 8, H = 28 }, c.HeaCurrent.ToString(), 16);
            p.Centered(p.Box("grt.cur") with { Y = p.Box("grt.cur").Y + 8, H = 28 }, c.GrtCurrent.ToString(), 16);
            for (int i = 1; i <= Math.Min(12, c.Exposure); i++) p.Cross($"exp.{i}");
        }
        for (int i = 1; i <= Math.Min(12, c.SpillMarks); i++) p.Dot($"mark.{i}");
        p.Line("marks.line", c.SpillMarkNote, 7.5);

        if (o.IncludeTracking)
        {
            var k = c.Conditions;
            if (k.Prone) p.Cross("cond.prone");
            p.Line("cond.bleed", k.Bleed, 7.5);
            p.Line("cond.burn", k.Burn, 7.5);
            p.Line("cond.stun", k.Stun, 7.5);
            p.Line("cond.fatigue", k.Fatigue, 7.5);
            p.Line("cond.toxin", k.Toxin, 7.5);
            p.Line("cond.groggy", k.Groggy, 7.5);
            p.Line("cond.lostgrt", k.LostMaxGrt, 7.5);
        }

        for (int i = 0; i < Math.Min(3, c.Weapons.Count); i++)
        {
            var w = c.Weapons[i];
            var row = $"weapon{i + 1}";
            p.Cell($"{row}.name", w.Name, 8, bold: true);
            p.Cell($"{row}.dmg", w.Damage, 8);
            p.Cell($"{row}.range", w.Range, 7.5);
            p.Cell($"{row}.tags", w.Tags, 6.5, min: 4.5);
            DrawShots(p, p.Box($"{row}.shots"), w.Shots);
        }
        if (c.Weapons.Count > 3)
            overflow.Add(("More weapons", c.Weapons.Skip(3).Select(w => $"{w.Name} — {w.Damage}, {w.Range}. {w.Tags} {w.Shots}".Trim()).ToList()));
        if (!string.IsNullOrWhiteSpace(c.Reloads))
        {
            var b = p.Box("reloads");
            p.Cell(b with { X = b.X + ReloadsLabel, W = b.W - ReloadsLabel }, c.Reloads, 7);
        }

        if (!string.IsNullOrWhiteSpace(c.Armor.Name) || c.Armor.Dr > 0 || CharacterRules.NaturalDr(c) > 0)
        {
            p.Cell("armor.name", c.Armor.Name, 8, bold: true);
            p.CellCentered("armor.dr", c.Armor.Dr.ToString(), 9);
            p.CellCentered("armor.natdr", CharacterRules.NaturalDr(c).ToString(), 9);
            p.CellCentered("armor.total", CharacterRules.TotalDr(c).ToString(), 9, bold: true);
            p.Cell("armor.notes", c.Armor.Notes, 7, min: 4.5);
        }

        // Class powers: the four the runner took at 1st level (plus any later ones that don't fit go to Advancement).
        var classPowers = FirstLevelClassPowers(c).ToList();
        for (int i = 0; i < Math.Min(4, classPowers.Count); i++)
        {
            var pw = classPowers[i];
            var slot = $"cp{i + 1}";
            p.Label($"{slot}.name", PowerLabel, pw.DisplayName, 7.5, bold: true);
            p.Label($"{slot}.type", TypeLabel, pw.Type, 6.5);
            var summary = p.Label($"{slot}.cost", CostLabel, pw.Cost, 6.5, fitOrFail: true) ? pw.Summary : $"[{pw.Cost}] {pw.Summary}";
            p.Wrapped([$"{slot}.l1", $"{slot}.l2"], summary, 7, 5, PowerLineLift);
        }

        // People powers: gain-all, gain-all drawback, then the two chosen.
        var peoplePowers = c.Powers.Where(x => x.Source == PowerSource.People).ToList();
        var gainAll = peoplePowers.Where(x => x.GainAll && !x.Drawback).ToList();
        var drawbacks = peoplePowers.Where(x => x.GainAll && x.Drawback).ToList();
        var chosen = peoplePowers.Where(x => !x.GainAll).ToList();
        DrawPeoplePower(p, "pp1", GainAllLabel, gainAll.ElementAtOrDefault(0));
        DrawPeoplePower(p, "pp2", DrawbackLabel, drawbacks.ElementAtOrDefault(0) ?? gainAll.ElementAtOrDefault(1));
        DrawPeoplePower(p, "pp3", ChosenLabel, chosen.ElementAtOrDefault(0));
        DrawPeoplePower(p, "pp4", ChosenLabel, chosen.ElementAtOrDefault(1));
        var extraPeople = gainAll.Skip(drawbacks.Count > 0 ? 1 : 2).Concat(drawbacks.Skip(1)).Concat(chosen.Skip(2)).ToList();
        if (extraPeople.Count > 0)
            overflow.Add(("More people powers", extraPeople.Select(PowerLine).ToList()));
    }

    static IEnumerable<CharacterPower> FirstLevelClassPowers(Character c) =>
        c.Powers.Where(x => x.Source == PowerSource.Class && x.LevelGained <= 1);

    static IEnumerable<CharacterPower> AdvancementPowers(Character c) =>
        c.Powers.Where(x => x.Source != PowerSource.People && !(x.Source == PowerSource.Class && x.LevelGained <= 1))
            .OrderBy(x => x.LevelGained);

    static void DrawPeoplePower(Pen p, string slot, double labelWidth, CharacterPower? pw)
    {
        if (pw is null) return;
        p.Label($"{slot}.name", labelWidth, pw.DisplayName, 7.5, bold: true);
        var summary = p.Label($"{slot}.cost", CostLabel, pw.Cost == "—" ? "" : pw.Cost, 6.5, fitOrFail: true) ? pw.Summary : $"[{pw.Cost}] {pw.Summary}";
        p.Wrapped([$"{slot}.l1"], summary, 7, 4.8, PowerLineLift);
    }

    static string PowerLine(CharacterPower pw) =>
        $"{pw.DisplayName} ({pw.Type}{(pw.Cost is "" or "—" ? "" : ", " + pw.Cost)}) — {pw.Summary}" +
        (pw.LevelGained > 1 ? $" [level {pw.LevelGained}]" : "");

    static void DrawShots(Pen p, FieldBox cell, string shots)
    {
        if (string.IsNullOrWhiteSpace(shots)) return;
        p.Cell(cell with { X = cell.X + 124, W = cell.W - 126 }, shots, 6, min: 4);
        // Strike out pips beyond the weapon's capacity.
        var digits = new string(shots.Where(char.IsDigit).ToArray());
        if (!int.TryParse(digits, out int cap) || cap >= 12 || cap <= 0) return;
        double cy = cell.Y + cell.H / 2;
        for (int i = cap; i < 12; i++)
        {
            double cx = cell.X + PipStart + i * PipSpacing + PipSize / 2;
            p.G.DrawLine(InkPen, cx - 3.2, cy + 3.2, cx + 3.2, cy - 3.2);
        }
    }

    // ---------------------------------------------------------------- Page 2

    static void DrawPage2(Pen p, Character c, SheetOptions o, List<(string, List<string>)> overflow)
    {
        p.Field("p2.name", c.Name);

        for (int i = 0; i < Math.Min(5, c.Leavings.Count); i++)
        {
            var l = c.Leavings[i];
            var row = $"leav{i + 1}";
            p.Cell($"{row}.name", l.Name, 7.5, bold: true);
            p.CellCentered($"{row}.str", l.Strangeness, 8);
            if (l.Tuned) p.CrossCenter($"{row}.tuned");
            if (l.Packed) p.CrossCenter($"{row}.packed");
            p.Cell($"{row}.use", l.Use, 6.5, min: 4.5);
            p.Cell($"{row}.quirk", l.Quirk, 6.5, min: 4.5);
        }
        if (c.Leavings.Count > 5)
            overflow.Add(("More leavings", c.Leavings.Skip(5).Select(l =>
                $"{l.Name} (strangeness {l.Strangeness}{(l.Tuned ? ", tuned" : "")}{(l.Packed ? ", packed" : "")}) — {l.Use}. {l.Quirk}").ToList()));

        var gear = c.Gear.Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
        int gearSlots = 10;
        if (gear.Count > gearSlots)
        {
            overflow.Add(("More gear & kit", gear.Skip(gearSlots - 1).ToList()));
            gear = [.. gear.Take(gearSlots - 1), "(continued on the extra page)"];
        }
        for (int i = 0; i < gear.Count; i++) p.Line($"gear{i + 1}", gear[i], 8);
        p.Field("oddment", c.Oddment, size: 8, min: 5.5);

        p.Field("scrip", c.Scrip.ToString(), size: 12, bold: true);
        p.Field("stash", c.Stashed, size: 8, min: 5.5);
        p.Field("debts", c.Debts, size: 8, min: 5.5);
        p.Field("permits", c.Permits, size: 8, min: 5.5);

        for (int i = 0; i < Math.Min(3, c.Contacts.Count); i++)
        {
            var k = c.Contacts[i];
            p.Cell($"contact{i + 1}.who", k.Who, 7.5, bold: true);
            p.Cell($"contact{i + 1}.owes", k.OwesOrWants, 6.5, min: 4.5);
            if (o.IncludeTracking && k.Used) p.CrossCenter($"contact{i + 1}.used");
        }
        if (c.Contacts.Count > 3)
            overflow.Add(("More contacts", c.Contacts.Skip(3).Select(k => $"{k.Who} — {k.OwesOrWants}{(k.Used ? " (used)" : "")}").ToList()));

        for (int i = 1; i <= Math.Min(10, c.Level); i++) p.Cross($"lvl.{i}");
        if (c.Level > 10)
        {
            var last = p.Box("lvl.10");
            p.Text(new FieldBox(2, last.Right + 3, last.Y, 20, last.H), c.Level.ToString(), 8, bold: true, XStringFormats.CenterLeft);
        }

        var adv = AdvancementPowers(c).ToList();
        for (int i = 0; i < Math.Min(4, adv.Count); i++)
        {
            var pw = adv[i];
            var slot = $"ap{i + 1}";
            var name = pw.Source switch
            {
                PowerSource.OtherClass => $"{pw.DisplayName} [{pw.Origin}]",
                _ => pw.DisplayName,
            };
            p.Label($"{slot}.name", PowerLabel, $"L{pw.LevelGained}: {name}", 7, bold: true);
            var summary = p.Label($"{slot}.cost", CostLabel, pw.Cost == "—" ? "" : pw.Cost, 6.5, fitOrFail: true) ? pw.Summary : $"[{pw.Cost}] {pw.Summary}";
            p.Wrapped([$"{slot}.l1"], $"{pw.Type}. {summary}", 6.5, 4.5, PowerLineLift);
        }
        if (adv.Count > 4)
            overflow.Add(("More advancement powers", adv.Skip(4).Select(PowerLine).ToList()));

        var took = c.WhatTheSpillTook.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        if (took.Count == 0 && c.SpillMarks > 0)
            took = Catalog.SpillMarkTable.Take(Math.Min(12, c.SpillMarks)).Select((t, i) => $"{i + 1}. {t}").ToList();
        if (took.Count > 5)
        {
            overflow.Add(("What the Spill took", took));
            took = [.. took.Take(4), "(all marks listed on the extra page)"];
        }
        for (int i = 0; i < took.Count; i++) p.Line($"took{i + 1}", took[i], 7.5, min: 5);

        p.Line("file1", c.WhyGoBackIn, 8, min: 5.5);
        p.Line("file2", c.WhoIsWaiting, 8, min: 5.5);
        p.Line("file3", c.WhatYouSaw, 8, min: 5.5);

        var leftover = p.Wrapped(["notes1", "notes2", "notes3"], c.Notes, 8, 6.5);
        if (leftover.Count > 0) overflow.Add(("Notes (continued)", leftover));
    }

    // ---------------------------------------------------------------- Overflow page

    static void DrawOverflowPages(PdfDocument doc, Character c, List<(string Heading, List<string> Lines)> sections)
    {
        var headFont = new XFont(SheetFontResolver.Family, 11, XFontStyleEx.Bold);
        var titleFont = new XFont(SheetFontResolver.Family, 16, XFontStyleEx.Bold);
        var bodyFont = new XFont(SheetFontResolver.Family, 9, XFontStyleEx.Regular);
        const double left = 36, width = 540, top = 40, bottom = 760, lineH = 12;

        PdfPage NewPage(out XGraphics g, out double y)
        {
            var page = doc.AddPage();
            page.Width = XUnit.FromPoint(612);
            page.Height = XUnit.FromPoint(792);
            g = XGraphics.FromPdfPage(page);
            g.DrawString("HALCYON", titleFont, XBrushes.Black, new XPoint(left, top));
            g.DrawString($"Runner's File, continued · {c.Name}", bodyFont, XBrushes.Black, new XPoint(left + 100, top));
            g.DrawLine(new XPen(XColors.Black, 1.5), left, top + 6, left + width, top + 6);
            y = top + 26;
            return page;
        }

        NewPage(out var gfx, out var cursor);
        try
        {
            foreach (var (heading, lines) in sections)
            {
                if (cursor + 3 * lineH > bottom) { gfx.Dispose(); NewPage(out gfx, out cursor); }
                gfx.DrawString(heading.ToUpperInvariant(), headFont, XBrushes.Black, new XPoint(left, cursor));
                gfx.DrawLine(new XPen(XColors.Black, 0.75), left, cursor + 3, left + width, cursor + 3);
                cursor += lineH + 4;
                foreach (var line in lines)
                    foreach (var wrapped in Wrap(gfx, "• " + line, bodyFont, width))
                    {
                        if (cursor > bottom) { gfx.Dispose(); NewPage(out gfx, out cursor); }
                        gfx.DrawString(wrapped, bodyFont, InkBrush, new XPoint(left, cursor));
                        cursor += lineH;
                    }
                cursor += 8;
            }
        }
        finally { gfx.Dispose(); }
    }

    internal static List<string> Wrap(XGraphics g, string text, XFont font, double width)
    {
        var lines = new List<string>();
        foreach (var para in text.Replace("\r", "").Split('\n'))
        {
            var current = "";
            foreach (var word in para.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var attempt = current.Length == 0 ? word : current + " " + word;
                if (g.MeasureString(attempt, font).Width <= width || current.Length == 0) current = attempt;
                else { lines.Add(current); current = word; }
            }
            lines.Add(current);
        }
        return lines;
    }

    // ---------------------------------------------------------------- Drawing helpers

    sealed class Pen(XGraphics g, SheetLayout layout)
    {
        public XGraphics G { get; } = g;

        public FieldBox Box(string key) => layout[key];

        XFont Font(double size, bool bold) => new(SheetFontResolver.Family, size, bold ? XFontStyleEx.Bold : XFontStyleEx.Regular);

        /// <summary>Largest font size (from <paramref name="size"/> down to <paramref name="min"/>) at which text fits the width.</summary>
        XFont Fit(string text, double width, double size, double min, bool bold, out bool fits)
        {
            for (double s = size; s >= min; s -= 0.25)
            {
                var f = Font(s, bold);
                if (G.MeasureString(text, f).Width <= width) { fits = true; return f; }
            }
            fits = false;
            return Font(min, bold);
        }

        string Truncate(string text, XFont font, double width)
        {
            if (G.MeasureString(text, font).Width <= width) return text;
            while (text.Length > 1 && G.MeasureString(text + "…", font).Width > width) text = text[..^1];
            return text.TrimEnd() + "…";
        }

        public void Text(FieldBox b, string text, double size, bool bold, XStringFormat format) =>
            G.DrawString(text, Font(size, bold), InkBrush, new XRect(b.X, b.Y, b.W, b.H), format);

        /// <summary>A labelled write-in field: text sits on the rule at the bottom of the box.</summary>
        public void Field(string key, string? text, double size = 10, double min = 6, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var b = Box(key);
            double w = b.W - 4;
            var f = Fit(text, w, size, min, bold, out _);
            G.DrawString(Truncate(text, f, w), f, InkBrush, new XPoint(b.X + 2.5, b.Bottom - 2.2));
        }

        /// <summary>A ruled line: text sits on it.</summary>
        public void Line(string key, string? text, double size, double min = 6)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            var b = Box(key);
            double w = b.W - 3;
            var f = Fit(text, w, size, min, false, out _);
            G.DrawString(Truncate(text, f, w), f, InkBrush, new XPoint(b.X + 1.5, b.Bottom - 2));
        }

        /// <summary>A table cell: text vertically centered.</summary>
        public void Cell(string key, string? text, double size, double min = 5, bool bold = false) => Cell(Box(key), text, size, min, bold);

        public void Cell(FieldBox b, string? text, double size, double min = 5, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            double w = b.W - 5;
            var f = Fit(text, w, size, min, bold, out _);
            G.DrawString(Truncate(text, f, w), f, InkBrush, new XRect(b.X + 3, b.Y, w, b.H), XStringFormats.CenterLeft);
        }

        public void CellCentered(string key, string? text, double size, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            Centered(Box(key), text, size, bold);
        }

        public void Centered(FieldBox b, string text, double size, bool bold = false)
        {
            var f = Fit(text, b.W - 2, size, 5, bold, out _);
            G.DrawString(text, f, InkBrush, new XRect(b.X, b.Y, b.W, b.H), XStringFormats.Center);
        }

        /// <summary>Right-aligned on the field's rule (score bonus / max lines).</summary>
        public void Right(string key, string text, double size, bool bold = false)
        {
            var b = Box(key);
            var f = Font(size, bold);
            var w = G.MeasureString(text, f).Width;
            G.DrawString(text, f, InkBrush, new XPoint(b.Right - w - 2, b.Bottom - 2));
        }

        /// <summary>Text written after a printed label in a power card header. Returns false if it didn't fit.</summary>
        public bool Label(string key, double labelWidth, string? text, double size, bool bold = false, bool fitOrFail = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;
            var b = Box(key);
            double x = b.X + labelWidth + 3, w = b.Right - x - 1;
            var f = Fit(text, w, size, fitOrFail ? 5 : 4.5, bold, out bool fits);
            if (fitOrFail && !fits) return false;
            // Sit on the printed label's baseline rather than the card's rule.
            G.DrawString(Truncate(text, f, w), f, InkBrush, new XPoint(x, b.Bottom - PowerHeaderLift));
            return true;
        }

        /// <summary>Wraps text over several ruled lines, shrinking the font to fit. Returns lines that still didn't fit.</summary>
        public List<string> Wrapped(string[] keys, string? text, double size, double min, double lift = 2)
        {
            if (string.IsNullOrWhiteSpace(text)) return [];
            var boxes = keys.Select(Box).ToArray();
            double w = boxes.Min(b => b.W) - 3;
            List<string> lines = [];
            XFont f = Font(size, false);
            for (double s = size; s >= min; s -= 0.25)
            {
                f = Font(s, false);
                lines = Wrap(G, text, f, w);
                if (lines.Count <= boxes.Length) break;
            }
            for (int i = 0; i < Math.Min(lines.Count, boxes.Length); i++)
            {
                var line = lines[i];
                if (i == boxes.Length - 1 && lines.Count > boxes.Length && keys[0].StartsWith("notes")) line += " …";
                else if (i == boxes.Length - 1 && lines.Count > boxes.Length) line = Truncate(string.Join(" ", lines.Skip(i)), f, w);
                G.DrawString(line, f, InkBrush, new XPoint(boxes[i].X + 1.5, boxes[i].Bottom - lift));
            }
            return lines.Count > boxes.Length && keys[0].StartsWith("notes") ? lines.Skip(boxes.Length).ToList() : [];
        }

        public void Cross(string key)
        {
            var b = Box(key);
            const double inset = 2.5;
            G.DrawLine(InkPen, b.X + inset, b.Y + inset, b.Right - inset, b.Bottom - inset);
            G.DrawLine(InkPen, b.X + inset, b.Bottom - inset, b.Right - inset, b.Y + inset);
        }

        public void CrossCenter(string key)
        {
            var b = Box(key);
            double cx = b.X + b.W / 2, cy = b.Y + b.H / 2, r = 3.2;
            G.DrawLine(InkPen, cx - r, cy - r, cx + r, cy + r);
            G.DrawLine(InkPen, cx - r, cy + r, cx + r, cy - r);
        }

        public void Dot(string key)
        {
            var b = Box(key);
            const double inset = 2.6;
            G.DrawEllipse(InkBrush, b.X + inset, b.Y + inset, b.W - 2 * inset, b.H - 2 * inset);
        }
    }
}
