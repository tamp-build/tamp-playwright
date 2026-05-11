namespace Tamp.Playwright.V1;

/// <summary>
/// Settings for <c>playwright merge-reports [report-folder]</c> —
/// stitches sharded test runs back together for a single HTML report.
/// </summary>
public sealed class PlaywrightMergeReportsSettings : PlaywrightSettingsBase
{
    /// <summary>Folder containing the per-shard blob reports.</summary>
    public string? ReportFolder { get; set; }

    /// <summary>Reporter to use for the merged output. Repeated as <c>--reporter &lt;name&gt;</c>.</summary>
    public List<string> Reporters { get; } = [];

    /// <summary>Config path. Maps to <c>--config</c>.</summary>
    public string? Config { get; set; }

    public PlaywrightMergeReportsSettings SetReportFolder(string? path) { ReportFolder = path; return this; }
    public PlaywrightMergeReportsSettings AddReporter(string name) { Reporters.Add(name); return this; }
    public PlaywrightMergeReportsSettings SetConfig(string? path) { Config = path; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "merge-reports";
        if (!string.IsNullOrEmpty(Config)) { yield return "--config"; yield return Config!; }
        foreach (var r in Reporters) { yield return "--reporter"; yield return r; }
        if (!string.IsNullOrEmpty(ReportFolder)) yield return ReportFolder!;
    }
}
