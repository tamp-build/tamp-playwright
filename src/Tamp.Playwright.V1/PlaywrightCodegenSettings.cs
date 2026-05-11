namespace Tamp.Playwright.V1;

/// <summary>
/// Settings for <c>playwright codegen [url]</c> — opens the recorder
/// UI. Mostly interactive, but the flags are useful for scripted
/// captures.
/// </summary>
public sealed class PlaywrightCodegenSettings : PlaywrightSettingsBase
{
    /// <summary>Optional URL to navigate to on start.</summary>
    public string? Url { get; set; }

    /// <summary>Target language. Maps to <c>--target</c>. Values: <c>javascript</c>, <c>python</c>, <c>python-async</c>, <c>java</c>, <c>csharp</c>, …</summary>
    public string? Target { get; set; }

    /// <summary>Browser engine. Maps to <c>--browser</c>.</summary>
    public string? Browser { get; set; }

    /// <summary>Output file. Maps to <c>--output</c> / <c>-o</c>.</summary>
    public string? Output { get; set; }

    /// <summary>Device descriptor (e.g. <c>"iPhone 13"</c>). Maps to <c>--device</c>.</summary>
    public string? Device { get; set; }

    /// <summary>Browser viewport size like <c>"1280,720"</c>. Maps to <c>--viewport-size</c>.</summary>
    public string? ViewportSize { get; set; }

    /// <summary>Save state file. Maps to <c>--save-storage</c>.</summary>
    public string? SaveStorage { get; set; }

    /// <summary>Load state file. Maps to <c>--load-storage</c>.</summary>
    public string? LoadStorage { get; set; }

    public PlaywrightCodegenSettings SetUrl(string? url) { Url = url; return this; }
    public PlaywrightCodegenSettings SetTarget(string lang) { Target = lang; return this; }
    public PlaywrightCodegenSettings SetBrowser(string name) { Browser = name; return this; }
    public PlaywrightCodegenSettings SetOutput(string path) { Output = path; return this; }
    public PlaywrightCodegenSettings SetDevice(string device) { Device = device; return this; }
    public PlaywrightCodegenSettings SetViewportSize(string size) { ViewportSize = size; return this; }
    public PlaywrightCodegenSettings SetSaveStorage(string path) { SaveStorage = path; return this; }
    public PlaywrightCodegenSettings SetLoadStorage(string path) { LoadStorage = path; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "codegen";
        if (!string.IsNullOrEmpty(Target)) { yield return "--target"; yield return Target!; }
        if (!string.IsNullOrEmpty(Browser)) { yield return "--browser"; yield return Browser!; }
        if (!string.IsNullOrEmpty(Output)) { yield return "--output"; yield return Output!; }
        if (!string.IsNullOrEmpty(Device)) { yield return "--device"; yield return Device!; }
        if (!string.IsNullOrEmpty(ViewportSize)) { yield return "--viewport-size"; yield return ViewportSize!; }
        if (!string.IsNullOrEmpty(SaveStorage)) { yield return "--save-storage"; yield return SaveStorage!; }
        if (!string.IsNullOrEmpty(LoadStorage)) { yield return "--load-storage"; yield return LoadStorage!; }
        if (!string.IsNullOrEmpty(Url)) yield return Url!;
    }
}
