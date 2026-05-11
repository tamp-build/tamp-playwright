namespace Tamp.Playwright.V1;

/// <summary>
/// Settings for <c>playwright install-deps [browsers...]</c> — installs
/// OS-level package deps WITHOUT downloading browsers. Only meaningful
/// on Linux runners. Often paired with cache restore where browsers
/// are already on disk.
/// </summary>
public sealed class PlaywrightInstallDepsSettings : PlaywrightSettingsBase
{
    public List<string> Browsers { get; } = [];
    public bool DryRun { get; set; }

    public PlaywrightInstallDepsSettings AddBrowser(string name) { Browsers.Add(name); return this; }
    public PlaywrightInstallDepsSettings SetDryRun(bool v = true) { DryRun = v; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "install-deps";
        if (DryRun) yield return "--dry-run";
        foreach (var b in Browsers) yield return b;
    }
}
