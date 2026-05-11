namespace Tamp.Playwright.V1;

/// <summary>Settings for <c>playwright open [url]</c> — opens the Inspector against a URL.</summary>
public sealed class PlaywrightOpenSettings : PlaywrightSettingsBase
{
    public string? Url { get; set; }
    public string? Browser { get; set; }
    public string? Device { get; set; }
    public string? ViewportSize { get; set; }
    public string? LoadStorage { get; set; }

    public PlaywrightOpenSettings SetUrl(string? url) { Url = url; return this; }
    public PlaywrightOpenSettings SetBrowser(string name) { Browser = name; return this; }
    public PlaywrightOpenSettings SetDevice(string device) { Device = device; return this; }
    public PlaywrightOpenSettings SetViewportSize(string size) { ViewportSize = size; return this; }
    public PlaywrightOpenSettings SetLoadStorage(string path) { LoadStorage = path; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "open";
        if (!string.IsNullOrEmpty(Browser)) { yield return "--browser"; yield return Browser!; }
        if (!string.IsNullOrEmpty(Device)) { yield return "--device"; yield return Device!; }
        if (!string.IsNullOrEmpty(ViewportSize)) { yield return "--viewport-size"; yield return ViewportSize!; }
        if (!string.IsNullOrEmpty(LoadStorage)) { yield return "--load-storage"; yield return LoadStorage!; }
        if (!string.IsNullOrEmpty(Url)) yield return Url!;
    }
}
