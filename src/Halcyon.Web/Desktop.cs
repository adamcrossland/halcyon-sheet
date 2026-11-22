using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Halcyon.Web;

/// <summary>
/// "Desktop mode" for the packaged app: listen only on this computer, pick a free port,
/// and open the player's browser once the server is up.
/// </summary>
internal static class Desktop
{
    public const int PreferredPort = 5180;

    /// <summary>
    /// The published app keeps wwwroot next to the executable. When it's double-clicked the working
    /// directory can be anywhere (on macOS it's the home folder), so use the executable's folder.
    /// </summary>
    public static string? ContentRoot() =>
        Directory.Exists(Path.Combine(AppContext.BaseDirectory, "wwwroot")) ? AppContext.BaseDirectory : null;

    /// <summary>True when no URL was given on the command line or in ASPNETCORE_URLS.</summary>
    public static bool NoUrlsConfigured(IConfiguration config) =>
        string.IsNullOrWhiteSpace(config["urls"]) && string.IsNullOrWhiteSpace(config["ASPNETCORE_URLS"]);

    public static string LocalUrl() => $"http://127.0.0.1:{(PortIsFree(PreferredPort) ? PreferredPort : 0)}";

    static bool PortIsFree(int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    /// <summary>Once the server is listening, print where it is and open it in the default browser.</summary>
    public static void OpenBrowserWhenReady(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var url = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault();
            if (url is null) return;
            Console.WriteLine();
            Console.WriteLine("  HALCYON - Runner's Files");
            Console.WriteLine($"  Running at {url}");
            Console.WriteLine("  Your browser should open by itself. If it doesn't, open that address.");
            Console.WriteLine("  Close this window (or press Ctrl+C) to stop.");
            Console.WriteLine();
            if (Environment.GetEnvironmentVariable("HALCYON_NO_BROWSER") is not { Length: > 0 }) OpenBrowser(url);
        });
    }

    static void OpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            else if (OperatingSystem.IsMacOS())
                Process.Start("open", url);
            else
                Process.Start("xdg-open", url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  (Couldn't open a browser automatically: {ex.Message})");
        }
    }
}
