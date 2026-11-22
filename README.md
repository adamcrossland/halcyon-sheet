# Halcyon Runner's Files

A small web app for players of **Halcyon** (a Grit System game) to create runners, level them up, track them between sessions, and print a filled-in copy of the official *Runner's File* character sheet.

## Run it

Requires the .NET 9 SDK.

```powershell
dotnet run --project src/Halcyon.Web --urls http://localhost:5180
```

Then open http://localhost:5180.

## Packages for players (Windows and macOS)

```powershell
./publish.ps1
```

This builds three zips in `artifacts/packages`. Each one runs without installing .NET:

| Zip | For |
| --- | --- |
| `Halcyon-windows.zip` | Windows 10/11 (64-bit) |
| `Halcyon-mac-apple-silicon.zip` | Macs with M1 or later |
| `Halcyon-mac-intel.zip` | Older Intel Macs |

Use `./publish.ps1 -Targets windows` to build just one. Each zip includes a `READ ME.txt` for players.

- **Windows:** players double-click `Halcyon.exe`. A console window opens along with their browser, and closing the window stops the app. Because the app isn't code-signed, SmartScreen warns the first time ("More info" → "Run anyway").
- **macOS:** players right-click `Start Halcyon.command` → Open the first time. Apple Silicon Macs only run signed programs, and Windows can't sign Mac apps, so that script clears the download quarantine and gives the app a local ad-hoc signature (`codesign --sign -`) before starting it. To get rid of the warning entirely you'd need an Apple Developer account to sign and notarize the app on a Mac.

The packaged app runs in **desktop mode** (`Halcyon:Desktop` is true in `appsettings.json`). It listens only on `127.0.0.1`, uses port 5180 or any free port if that one is taken, and opens the default browser. `dotnet run` uses development settings, where desktop mode is off. To host it as a website instead, set `Halcyon:Desktop` to false.

## What it does

- **New runner wizard**: concept (people, class, name roller from *The Book of Odds*), scores (either array, or a class quickbuild), class powers (with the Sensitive's required *Spill-touched* and its Weird power), people powers (with choices like *Free of fate* or *Protagonistic*), starting kit, shopping with the starting budget, an oddment roller, and the "file on you" questions.
- **Level-up wizard**: split the 6 pool between HEA and GRT, then take a class power, an advanced power, a Spill-touch power (level 10+, or any level for the Sensitive), or multiclass (take another class's power and give one up).
- **Runner page**: edit everything on the sheet between sessions: current pool, exposure and marks (with a "coming home" d12 roll), conditions, weapons, armor, leavings, gear, contacts, money and notes.
- **PDF**: fills `Character Sheet.pdf`. *Clean PDF* leaves current HEA/GRT, exposure, conditions and "used" boxes blank so you can track them in pencil. Anything that doesn't fit the sheet (more than 3 weapons, more than 4 advancement powers, long notes, and so on) goes on an extra page.
- **Export / import** a runner as JSON to move it between machines or share it with the GM.

## Icon

The artwork is in `packaging/icon`. `halcyon-icon.svg` is the full icon and `halcyon-icon-small.svg` is a simplified version for 32 px and smaller. The PNGs in `packaging/icon/png` were rendered from those two files. After changing the artwork, re-render the PNGs at the same sizes and names, then run `packaging/icon/build-ico.ps1`. That rebuilds `halcyon.ico`, which becomes the `Halcyon.exe` icon, and copies the browser-tab icons into `wwwroot`.

On macOS the icon shows in the browser tab only. A bare program or `.command` file can't carry an icon through a zip; that needs a `.app` bundle.

## Where things live

| Path | What |
| --- | --- |
| `src/Halcyon.Core/Rules/Catalog.cs` | Rules content: classes, peoples, powers, gear, tables. Edit here when the rules change. |
| `src/Halcyon.Core/Rules/CharacterRules.cs` | Derived values (HEA, GRT, DR, scrip) and the level-up logic. |
| `src/Halcyon.Core/Rules/CreationDraft.cs` | Creation wizard state, validation and building the character. |
| `src/Halcyon.Core/Pdf/` | PDF filling. `Assets/SheetLayout.txt` holds every field's position on the template. |
| `src/Halcyon.Core/Assets/CharacterSheet.pdf` | The template (a copy of `Character Sheet.pdf`). |
| `src/Halcyon.Web/` | Blazor Server UI. |
| `tests/Halcyon.Tests/` | Rebuilds the *Slow Water* pregens and checks HEA/GRT/scrip, plus PDF and storage tests. |

Characters are saved as one JSON file each in `%LOCALAPPDATA%\HalcyonSheet\characters`. Change it with `Halcyon:DataDirectory` in `appsettings.json` (or the `Halcyon__DataDirectory` environment variable).

## Changing the sheet

The template PDF has no form fields, so the app draws text at fixed positions. If you change `Character Sheet.html` and re-print the PDF:

1. Copy the new PDF over `src/Halcyon.Core/Assets/CharacterSheet.pdf` (or point `Halcyon:TemplatePath` at it).
2. If the layout moved, re-measure the field boxes into `src/Halcyon.Core/Assets/SheetLayout.txt` (format: `key|page|x|y|width|height` in PDF points from the top-left; the values were taken from the HTML's element positions at print size, plus the 0.35 in page margin).

## Rule interpretations

- Score bonuses (*Free of fate*, *Made for purpose*, *Logic core*, *Quick*) add to rolls, not to maxCOM/maxMOV for HEA. This matches the *Slow Water* pregens (Ash: d12 (+1) COM, 26 HEA).
- Natural DR (*pelt or scale*, *bark hide*, *plated*) stacks with armor to a maximum of 4.
- Starting HEA/GRT are fixed at creation; later changes to max pool come from level-ups and the "adjust max" fields (for example −1 HEA after coming back from 0).

## Fonts and hosting

The PDF uses Segoe UI or Arial on Windows, and DejaVu Sans or Liberation Sans on Linux. Set `HALCYON_FONT_DIR` to use another folder. The app has no logins: everyone who can reach it sees every runner, so run it locally or on a private network.

PDFsharp is pinned to 6.1.1 (MIT). Version 6.5 adds a license check that fails without a license key.
