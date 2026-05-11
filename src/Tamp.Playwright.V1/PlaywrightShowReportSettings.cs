namespace Tamp.Playwright.V1;

/// <summary>Settings for <c>playwright show-report [report-folder]</c> — open the HTML report locally.</summary>
public sealed class PlaywrightShowReportSettings : PlaywrightSettingsBase
{
    public string? ReportFolder { get; set; }
    public string? Host { get; set; }
    public int? Port { get; set; }

    public PlaywrightShowReportSettings SetReportFolder(string? path) { ReportFolder = path; return this; }
    public PlaywrightShowReportSettings SetHost(string host) { Host = host; return this; }
    public PlaywrightShowReportSettings SetPort(int port) { Port = port; return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "show-report";
        if (!string.IsNullOrEmpty(Host)) { yield return "--host"; yield return Host!; }
        if (Port is { } p) { yield return "--port"; yield return p.ToString(); }
        if (!string.IsNullOrEmpty(ReportFolder)) yield return ReportFolder!;
    }
}
