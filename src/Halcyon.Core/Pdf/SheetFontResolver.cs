using PdfSharp.Fonts;

namespace Halcyon.Core.Pdf;

/// <summary>
/// Supplies the TrueType files PDFsharp embeds in the filled sheet. Looks for Segoe UI / Arial on Windows
/// and DejaVu / Liberation on Linux and macOS, or a folder given in HALCYON_FONT_DIR.
/// </summary>
internal sealed class SheetFontResolver : IFontResolver
{
    public const string Family = "SheetInk";
    const string Regular = "SheetInk-Regular";
    const string Bold = "SheetInk-Bold";

    static readonly Lock Gate = new();
    static bool _installed;

    readonly string _regularPath;
    readonly string _boldPath;

    SheetFontResolver(string regular, string bold) => (_regularPath, _boldPath) = (regular, bold);

    public static void EnsureInstalled()
    {
        lock (Gate)
        {
            if (_installed) return;
            if (GlobalFontSettings.FontResolver is null)
            {
                var (regular, bold) = FindFonts();
                GlobalFontSettings.FontResolver = new SheetFontResolver(regular, bold);
            }
            _installed = true;
        }
    }

    static (string Regular, string Bold) FindFonts()
    {
        var dirs = new List<string>();
        if (Environment.GetEnvironmentVariable("HALCYON_FONT_DIR") is { Length: > 0 } custom) dirs.Add(custom);
        dirs.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts"));
        dirs.AddRange([
            "/usr/share/fonts/truetype/dejavu", "/usr/share/fonts/dejavu", "/usr/share/fonts/TTF",
            "/usr/share/fonts/truetype/liberation", "/usr/share/fonts/liberation",
            "/Library/Fonts", "/System/Library/Fonts/Supplemental",
        ]);

        (string, string)[] pairs =
        [
            ("segoeui.ttf", "segoeuib.ttf"),
            ("arial.ttf", "arialbd.ttf"),
            ("Arial.ttf", "Arial Bold.ttf"),
            ("DejaVuSans.ttf", "DejaVuSans-Bold.ttf"),
            ("LiberationSans-Regular.ttf", "LiberationSans-Bold.ttf"),
        ];

        foreach (var dir in dirs.Where(d => !string.IsNullOrEmpty(d) && Directory.Exists(d)))
            foreach (var (r, b) in pairs)
            {
                var rp = Path.Combine(dir, r);
                if (!File.Exists(rp)) continue;
                var bp = Path.Combine(dir, b);
                return (rp, File.Exists(bp) ? bp : rp);
            }

        throw new InvalidOperationException(
            "No usable TrueType font found for the PDF. Install DejaVu Sans or set HALCYON_FONT_DIR to a folder containing segoeui.ttf/arial.ttf/DejaVuSans.ttf.");
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic) =>
        new(isBold ? Bold : Regular);

    public byte[]? GetFont(string faceName) => File.ReadAllBytes(faceName == Bold ? _boldPath : _regularPath);
}
