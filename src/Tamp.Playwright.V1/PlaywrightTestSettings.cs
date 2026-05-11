namespace Tamp.Playwright.V1;

/// <summary>
/// Settings for <c>playwright test</c> — the canonical CI verb.
/// Covers browser/project selection, sharding, reporters, retries,
/// timeouts, snapshot updates, and the headed/UI/trace knobs.
/// </summary>
public sealed class PlaywrightTestSettings : PlaywrightSettingsBase
{
    /// <summary>Path to playwright config. Maps to <c>--config</c> / <c>-c</c>.</summary>
    public string? Config { get; set; }

    /// <summary>Browser engines. Repeated as <c>--browser &lt;name&gt;</c>. Values: <c>chromium</c>, <c>firefox</c>, <c>webkit</c>, <c>all</c>.</summary>
    public List<string> Browsers { get; } = [];

    /// <summary>Project names from playwright.config. Repeated as <c>--project &lt;name&gt;</c>.</summary>
    public List<string> Projects { get; } = [];

    /// <summary>Test grep pattern. Maps to <c>--grep</c> / <c>-g</c>.</summary>
    public string? Grep { get; set; }

    /// <summary>Inverse grep — exclude tests matching. Maps to <c>--grep-invert</c>.</summary>
    public string? GrepInvert { get; set; }

    /// <summary>Run headed. Maps to <c>--headed</c>.</summary>
    public bool Headed { get; set; }

    /// <summary>Debug mode (Playwright Inspector). Maps to <c>--debug</c> / <c>-d</c>.</summary>
    public bool Debug { get; set; }

    /// <summary>Forbid <c>test.only(...)</c> in CI. Maps to <c>--forbid-only</c>.</summary>
    public bool ForbidOnly { get; set; }

    /// <summary>Run all tests in parallel (override per-file <c>describe.parallel</c>). Maps to <c>--fully-parallel</c>.</summary>
    public bool FullyParallel { get; set; }

    /// <summary>Global timeout (ms) across the entire run. Maps to <c>--global-timeout</c>.</summary>
    public int? GlobalTimeout { get; set; }

    /// <summary>Per-test timeout (ms). Maps to <c>--timeout</c>.</summary>
    public int? Timeout { get; set; }

    /// <summary>Max failures before bailing. Maps to <c>--max-failures</c> / <c>-x</c> (which is shorthand for 1).</summary>
    public int? MaxFailures { get; set; }

    /// <summary>Retry count. Maps to <c>--retries</c>.</summary>
    public int? Retries { get; set; }

    /// <summary>Worker count or fraction. Maps to <c>--workers</c> / <c>-j</c>.</summary>
    public string? Workers { get; set; }

    /// <summary>Repeat each test N times. Maps to <c>--repeat-each</c>.</summary>
    public int? RepeatEach { get; set; }

    /// <summary>Shard spec like <c>1/3</c>. Maps to <c>--shard</c>.</summary>
    public string? Shard { get; set; }

    /// <summary>Reporters. Repeated as <c>--reporter &lt;name&gt;</c>. Values: <c>line</c>, <c>list</c>, <c>dot</c>, <c>html</c>, <c>junit</c>, <c>github</c>, <c>json</c>, …</summary>
    public List<string> Reporters { get; } = [];

    /// <summary>Output dir for traces/screenshots/videos. Maps to <c>--output</c>.</summary>
    public string? Output { get; set; }

    /// <summary>Trace mode. Maps to <c>--trace</c>. Values: <c>on</c>, <c>off</c>, <c>on-first-retry</c>, <c>retain-on-failure</c>.</summary>
    public string? Trace { get; set; }

    /// <summary>Quiet mode. Maps to <c>--quiet</c> / <c>-q</c>.</summary>
    public bool Quiet { get; set; }

    /// <summary>Pass when no tests are found. Maps to <c>--pass-with-no-tests</c>.</summary>
    public bool PassWithNoTests { get; set; }

    /// <summary>List tests without running. Maps to <c>--list</c>.</summary>
    public bool List { get; set; }

    /// <summary>Don't validate dependencies. Maps to <c>--no-deps</c>.</summary>
    public bool NoDeps { get; set; }

    /// <summary>Update snapshots. <c>true</c> = flag only; non-null string = <c>--update-snapshots &lt;mode&gt;</c> (all/changed/missing/none).</summary>
    public bool UpdateSnapshots { get; set; }
    public string? UpdateSnapshotsMode { get; set; }

    /// <summary>Run only tests reachable from changed files. <c>--only-changed [ref]</c>.</summary>
    public bool OnlyChanged { get; set; }
    public string? OnlyChangedRef { get; set; }

    /// <summary>UI mode. Maps to <c>--ui</c>.</summary>
    public bool Ui { get; set; }

    /// <summary>UI host. Maps to <c>--ui-host</c>.</summary>
    public string? UiHost { get; set; }

    /// <summary>UI port. Maps to <c>--ui-port</c>.</summary>
    public int? UiPort { get; set; }

    /// <summary>Ignore snapshot comparisons. Maps to <c>--ignore-snapshots</c>.</summary>
    public bool IgnoreSnapshots { get; set; }

    /// <summary>Positional test file filters (glob patterns or paths).</summary>
    public List<string> FileFilters { get; } = [];

    public new PlaywrightTestSettings SetWorkingDirectory(string? cwd) { WorkingDirectory = cwd; return this; }
    public PlaywrightTestSettings SetConfig(string? path) { Config = path; return this; }
    public PlaywrightTestSettings AddBrowser(string name) { Browsers.Add(name); return this; }
    public PlaywrightTestSettings AddProject(string name) { Projects.Add(name); return this; }
    public PlaywrightTestSettings SetGrep(string pattern) { Grep = pattern; return this; }
    public PlaywrightTestSettings SetGrepInvert(string pattern) { GrepInvert = pattern; return this; }
    public PlaywrightTestSettings SetHeaded(bool v = true) { Headed = v; return this; }
    public PlaywrightTestSettings SetDebug(bool v = true) { Debug = v; return this; }
    public PlaywrightTestSettings SetForbidOnly(bool v = true) { ForbidOnly = v; return this; }
    public PlaywrightTestSettings SetFullyParallel(bool v = true) { FullyParallel = v; return this; }
    public PlaywrightTestSettings SetGlobalTimeout(int ms) { GlobalTimeout = ms; return this; }
    public PlaywrightTestSettings SetTimeout(int ms) { Timeout = ms; return this; }
    public PlaywrightTestSettings SetMaxFailures(int n) { MaxFailures = n; return this; }
    public PlaywrightTestSettings SetRetries(int n) { Retries = n; return this; }
    public PlaywrightTestSettings SetWorkers(string spec) { Workers = spec; return this; }
    public PlaywrightTestSettings SetWorkers(int n) { Workers = n.ToString(); return this; }
    public PlaywrightTestSettings SetRepeatEach(int n) { RepeatEach = n; return this; }
    public PlaywrightTestSettings SetShard(int current, int total) { Shard = $"{current}/{total}"; return this; }
    public PlaywrightTestSettings AddReporter(string name) { Reporters.Add(name); return this; }
    public PlaywrightTestSettings SetOutput(string path) { Output = path; return this; }
    public PlaywrightTestSettings SetTrace(string mode) { Trace = mode; return this; }
    public PlaywrightTestSettings SetQuiet(bool v = true) { Quiet = v; return this; }
    public PlaywrightTestSettings SetPassWithNoTests(bool v = true) { PassWithNoTests = v; return this; }
    public PlaywrightTestSettings SetList(bool v = true) { List = v; return this; }
    public PlaywrightTestSettings SetNoDeps(bool v = true) { NoDeps = v; return this; }
    public PlaywrightTestSettings SetUpdateSnapshots(string? mode = null) { UpdateSnapshots = true; UpdateSnapshotsMode = mode; return this; }
    public PlaywrightTestSettings SetOnlyChanged(string? @ref = null) { OnlyChanged = true; OnlyChangedRef = @ref; return this; }
    public PlaywrightTestSettings SetUi(bool v = true) { Ui = v; return this; }
    public PlaywrightTestSettings SetUiHost(string host) { UiHost = host; return this; }
    public PlaywrightTestSettings SetUiPort(int port) { UiPort = port; return this; }
    public PlaywrightTestSettings SetIgnoreSnapshots(bool v = true) { IgnoreSnapshots = v; return this; }
    public PlaywrightTestSettings AddFileFilter(string pattern) { FileFilters.Add(pattern); return this; }

    protected override IEnumerable<string> BuildVerbArguments()
    {
        yield return "test";
        if (!string.IsNullOrEmpty(Config)) { yield return "--config"; yield return Config!; }
        foreach (var b in Browsers) { yield return "--browser"; yield return b; }
        foreach (var p in Projects) { yield return "--project"; yield return p; }
        if (!string.IsNullOrEmpty(Grep)) { yield return "--grep"; yield return Grep!; }
        if (!string.IsNullOrEmpty(GrepInvert)) { yield return "--grep-invert"; yield return GrepInvert!; }
        if (Headed) yield return "--headed";
        if (Debug) yield return "--debug";
        if (ForbidOnly) yield return "--forbid-only";
        if (FullyParallel) yield return "--fully-parallel";
        if (GlobalTimeout is { } gt) { yield return "--global-timeout"; yield return gt.ToString(); }
        if (Timeout is { } t) { yield return "--timeout"; yield return t.ToString(); }
        if (MaxFailures is { } mf) { yield return "--max-failures"; yield return mf.ToString(); }
        if (Retries is { } r) { yield return "--retries"; yield return r.ToString(); }
        if (!string.IsNullOrEmpty(Workers)) { yield return "--workers"; yield return Workers!; }
        if (RepeatEach is { } re) { yield return "--repeat-each"; yield return re.ToString(); }
        if (!string.IsNullOrEmpty(Shard)) { yield return "--shard"; yield return Shard!; }
        foreach (var rep in Reporters) { yield return "--reporter"; yield return rep; }
        if (!string.IsNullOrEmpty(Output)) { yield return "--output"; yield return Output!; }
        if (!string.IsNullOrEmpty(Trace)) { yield return "--trace"; yield return Trace!; }
        if (Quiet) yield return "--quiet";
        if (PassWithNoTests) yield return "--pass-with-no-tests";
        if (List) yield return "--list";
        if (NoDeps) yield return "--no-deps";
        if (UpdateSnapshots)
        {
            yield return "--update-snapshots";
            if (!string.IsNullOrEmpty(UpdateSnapshotsMode)) yield return UpdateSnapshotsMode!;
        }
        if (OnlyChanged)
        {
            yield return "--only-changed";
            if (!string.IsNullOrEmpty(OnlyChangedRef)) yield return OnlyChangedRef!;
        }
        if (Ui) yield return "--ui";
        if (!string.IsNullOrEmpty(UiHost)) { yield return "--ui-host"; yield return UiHost!; }
        if (UiPort is { } up) { yield return "--ui-port"; yield return up.ToString(); }
        if (IgnoreSnapshots) yield return "--ignore-snapshots";
        foreach (var f in FileFilters) yield return f;
    }
}
