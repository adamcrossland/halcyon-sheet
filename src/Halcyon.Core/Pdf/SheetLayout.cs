using System.Globalization;
using System.Reflection;

namespace Halcyon.Core.Pdf;

/// <summary>A field box on the template, in PDF points from the page's top-left corner.</summary>
public readonly record struct FieldBox(int Page, double X, double Y, double W, double H)
{
    public double Right => X + W;
    public double Bottom => Y + H;
}

/// <summary>Field positions on Character Sheet.pdf, loaded from the embedded SheetLayout.txt.</summary>
public sealed class SheetLayout
{
    readonly Dictionary<string, FieldBox> _boxes;

    SheetLayout(Dictionary<string, FieldBox> boxes) => _boxes = boxes;

    public FieldBox this[string key] => _boxes.TryGetValue(key, out var b) ? b : throw new KeyNotFoundException($"No sheet field '{key}'.");

    public bool Has(string key) => _boxes.ContainsKey(key);

    public IReadOnlyCollection<string> Keys => _boxes.Keys;

    public static SheetLayout Default { get; } = LoadEmbedded();

    public static SheetLayout Parse(TextReader reader)
    {
        var boxes = new Dictionary<string, FieldBox>(StringComparer.Ordinal);
        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var parts = line.Split('|');
            if (parts.Length != 6) throw new FormatException($"Bad layout line: {line}");
            double D(int i) => double.Parse(parts[i], CultureInfo.InvariantCulture);
            boxes[parts[0]] = new FieldBox(int.Parse(parts[1], CultureInfo.InvariantCulture), D(2), D(3), D(4), D(5));
        }
        return new SheetLayout(boxes);
    }

    static SheetLayout LoadEmbedded()
    {
        using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Halcyon.Core.Assets.SheetLayout.txt")
            ?? throw new InvalidOperationException("Embedded SheetLayout.txt is missing.");
        using var r = new StreamReader(s);
        return Parse(r);
    }
}
