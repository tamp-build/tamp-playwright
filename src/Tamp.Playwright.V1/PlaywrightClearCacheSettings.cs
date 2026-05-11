namespace Tamp.Playwright.V1;

/// <summary>Settings for <c>playwright clear-cache</c> — wipe Playwright's local browser/asset cache.</summary>
public sealed class PlaywrightClearCacheSettings : PlaywrightSettingsBase
{
    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "clear-cache";
    }
}
