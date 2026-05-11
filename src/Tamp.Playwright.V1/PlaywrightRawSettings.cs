namespace Tamp.Playwright.V1;

/// <summary>Escape hatch for verb/flag combinations we haven't typed.</summary>
public sealed class PlaywrightRawSettings : PlaywrightSettingsBase
{
    public List<string> RawArguments { get; } = [];

    public PlaywrightRawSettings AddArgs(params string[] args) { RawArguments.AddRange(args); return this; }

    protected override IEnumerable<string> BuildVerbArguments() => RawArguments;
}
