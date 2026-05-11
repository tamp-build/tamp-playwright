namespace Tamp.Playwright.V1;

/// <summary>Facade for the Playwright 1.x CLI.</summary>
/// <remarks>
/// <para>Resolve via <c>[NuGetPackage(UseSystemPath = true)]</c>:</para>
/// <code>
/// [NuGetPackage("playwright", UseSystemPath = true)]
/// readonly Tool PlaywrightTool;
/// </code>
/// </remarks>
public static class Playwright
{
    public static CommandPlan Test(Tool tool, Action<PlaywrightTestSettings>? configure = null)
        => Build<PlaywrightTestSettings>(tool, configure);

    public static CommandPlan Install(Tool tool, Action<PlaywrightInstallSettings>? configure = null)
        => Build<PlaywrightInstallSettings>(tool, configure);

    public static CommandPlan InstallDeps(Tool tool, Action<PlaywrightInstallDepsSettings>? configure = null)
        => Build<PlaywrightInstallDepsSettings>(tool, configure);

    public static CommandPlan Uninstall(Tool tool, Action<PlaywrightUninstallSettings>? configure = null)
        => Build<PlaywrightUninstallSettings>(tool, configure);

    public static CommandPlan Codegen(Tool tool, Action<PlaywrightCodegenSettings>? configure = null)
        => Build<PlaywrightCodegenSettings>(tool, configure);

    public static CommandPlan ShowReport(Tool tool, Action<PlaywrightShowReportSettings>? configure = null)
        => Build<PlaywrightShowReportSettings>(tool, configure);

    public static CommandPlan MergeReports(Tool tool, Action<PlaywrightMergeReportsSettings>? configure = null)
        => Build<PlaywrightMergeReportsSettings>(tool, configure);

    public static CommandPlan ClearCache(Tool tool, Action<PlaywrightClearCacheSettings>? configure = null)
        => Build<PlaywrightClearCacheSettings>(tool, configure);

    public static CommandPlan Open(Tool tool, Action<PlaywrightOpenSettings>? configure = null)
        => Build<PlaywrightOpenSettings>(tool, configure);

    public static CommandPlan Raw(Tool tool, params string[] arguments)
    {
        if (tool is null) throw new ArgumentNullException(nameof(tool));
        if (arguments is null || arguments.Length == 0)
            throw new ArgumentException("Raw requires at least one argument.", nameof(arguments));
        var s = new PlaywrightRawSettings();
        s.AddArgs(arguments);
        return s.ToCommandPlan(tool);
    }

    private static CommandPlan Build<T>(Tool tool, Action<T>? configure) where T : PlaywrightSettingsBase, new()
    {
        if (tool is null) throw new ArgumentNullException(nameof(tool));
        var s = new T();
        configure?.Invoke(s);
        return s.ToCommandPlan(tool);
    }

    // ---- Object-init overloads (TAM-161) ----
    // Two equivalent authoring styles; both produce identical CommandPlans. Fluent
    // stays canonical in docs and `tamp init` templates; object-init available for
    // consumers who prefer the C# initializer shape.
    //
    //     Playwright.Test(tool, new() { Config = "playwright.config.ts", Headed = true });
    //
    // is equivalent to:
    //
    //     Playwright.Test(tool, s => s.SetConfig("playwright.config.ts").SetHeaded());

    public static CommandPlan Test(Tool tool, PlaywrightTestSettings settings) => Build(tool, settings);
    public static CommandPlan Install(Tool tool, PlaywrightInstallSettings settings) => Build(tool, settings);
    public static CommandPlan InstallDeps(Tool tool, PlaywrightInstallDepsSettings settings) => Build(tool, settings);
    public static CommandPlan Uninstall(Tool tool, PlaywrightUninstallSettings settings) => Build(tool, settings);
    public static CommandPlan Codegen(Tool tool, PlaywrightCodegenSettings settings) => Build(tool, settings);
    public static CommandPlan ShowReport(Tool tool, PlaywrightShowReportSettings settings) => Build(tool, settings);
    public static CommandPlan MergeReports(Tool tool, PlaywrightMergeReportsSettings settings) => Build(tool, settings);
    public static CommandPlan ClearCache(Tool tool, PlaywrightClearCacheSettings settings) => Build(tool, settings);
    public static CommandPlan Open(Tool tool, PlaywrightOpenSettings settings) => Build(tool, settings);

    private static CommandPlan Build<T>(Tool tool, T settings) where T : PlaywrightSettingsBase
    {
        if (tool is null) throw new ArgumentNullException(nameof(tool));
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        return settings.ToCommandPlan(tool);
    }
}
