namespace Tamp.Playwright.V1;

/// <summary>Settings for <c>playwright uninstall</c> — removes browsers from the local cache.</summary>
public sealed class PlaywrightUninstallSettings : PlaywrightSettingsBase
{
    /// <summary>Remove browsers from all clients (system-wide). Maps to <c>--all</c>.</summary>
    public bool All { get; set; }

    public PlaywrightUninstallSettings SetAll(bool v = true) { All = v; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "uninstall";
        if (All) yield return "--all";
    }
}
