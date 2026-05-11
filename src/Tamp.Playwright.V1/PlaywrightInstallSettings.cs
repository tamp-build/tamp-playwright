namespace Tamp.Playwright.V1;

/// <summary>
/// Settings for <c>playwright install [browsers...]</c>. CI typically
/// pairs <c>--with-deps</c> for Linux runners so apt installs the
/// codec / font / GTK system packages.
/// </summary>
public sealed class PlaywrightInstallSettings : PlaywrightSettingsBase
{
    /// <summary>Browser names. Empty = all. Values: <c>chromium</c>, <c>firefox</c>, <c>webkit</c>, <c>msedge</c>, <c>chrome</c>, <c>chromium-headless-shell</c>.</summary>
    public List<string> Browsers { get; } = [];

    /// <summary>Also install system-level deps (apt). Maps to <c>--with-deps</c>.</summary>
    public bool WithDeps { get; set; }

    /// <summary>Show what would install without actually fetching. Maps to <c>--dry-run</c>.</summary>
    public bool DryRun { get; set; }

    /// <summary>Force reinstall even if browser is up to date. Maps to <c>--force</c>.</summary>
    public bool Force { get; set; }

    /// <summary>Skip the headless shell. Maps to <c>--no-shell</c>.</summary>
    public bool NoShell { get; set; }

    /// <summary>Only install the headless shell (skip full browser). Maps to <c>--only-shell</c>.</summary>
    public bool OnlyShell { get; set; }

    public PlaywrightInstallSettings AddBrowser(string name) { Browsers.Add(name); return this; }
    public PlaywrightInstallSettings SetWithDeps(bool v = true) { WithDeps = v; return this; }
    public PlaywrightInstallSettings SetDryRun(bool v = true) { DryRun = v; return this; }
    public PlaywrightInstallSettings SetForce(bool v = true) { Force = v; return this; }
    public PlaywrightInstallSettings SetNoShell(bool v = true) { NoShell = v; return this; }
    public PlaywrightInstallSettings SetOnlyShell(bool v = true) { OnlyShell = v; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "install";
        if (WithDeps) yield return "--with-deps";
        if (DryRun) yield return "--dry-run";
        if (Force) yield return "--force";
        if (NoShell) yield return "--no-shell";
        if (OnlyShell) yield return "--only-shell";
        foreach (var b in Browsers) yield return b;
    }
}
