using System.Text;
using Halcyon.Core.Pdf;
using Halcyon.Core.Storage;
using Halcyon.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Where character files live. Override with Halcyon:DataDirectory in appsettings or HALCYON__DATADIRECTORY.
var dataDir = builder.Configuration["Halcyon:DataDirectory"];
if (string.IsNullOrWhiteSpace(dataDir))
    dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HalcyonSheet", "characters");
builder.Services.AddSingleton(new CharacterStore(dataDir));

// The PDF template is embedded; Halcyon:TemplatePath points at a replacement with the same layout.
var templatePath = builder.Configuration["Halcyon:TemplatePath"];
builder.Services.AddSingleton(_ => string.IsNullOrWhiteSpace(templatePath)
    ? new SheetRenderer()
    : new SheetRenderer(openTemplate: () => File.OpenRead(templatePath)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapGet("/characters/{id:guid}/sheet.pdf", (Guid id, bool? tracking, bool? download, CharacterStore store, SheetRenderer renderer) =>
{
    var c = store.Get(id);
    if (c is null) return Results.NotFound();
    var pdf = renderer.Render(c, new SheetOptions { IncludeTracking = tracking ?? true });
    var name = FileName(c.Name, "pdf");
    return download == true
        ? Results.File(pdf, "application/pdf", name)
        : Results.File(pdf, "application/pdf");
});

app.MapGet("/characters/{id:guid}/export.json", (Guid id, CharacterStore store) =>
{
    var c = store.Get(id);
    return c is null
        ? Results.NotFound()
        : Results.File(Encoding.UTF8.GetBytes(CharacterStore.Export(c)), "application/json", FileName(c.Name, "json"));
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Logger.LogInformation("Character files: {Dir}", dataDir);
app.Run();

static string FileName(string name, string ext)
{
    var safe = new string((string.IsNullOrWhiteSpace(name) ? "runner" : name)
        .Select(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' ? ch : '-').ToArray()).Trim('-');
    return $"{(safe.Length == 0 ? "runner" : safe)}.{ext}";
}
