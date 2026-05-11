namespace Tamp.Playwright.V1;

/// <summary>
/// Common base for <c>playwright &lt;verb&gt;</c> settings. Each verb
/// drives its own arg list; this base only holds the cross-cutting
/// CWD + env knobs (Playwright's top-level options are mostly per-verb).
/// </summary>
public abstract class PlaywrightSettingsBase
{
    public string? WorkingDirectory { get; set; }
    public Dictionary<string, string> EnvironmentVariables { get; } = new();

    public PlaywrightSettingsBase SetWorkingDirectory(string? cwd) { WorkingDirectory = cwd; return this; }
    public PlaywrightSettingsBase SetEnv(string key, string value) { EnvironmentVariables[key] = value; return this; }

    protected abstract IEnumerable<string> BuildVerbArguments();

    protected virtual IReadOnlyList<Secret> CollectSecrets() => Array.Empty<Secret>();

    public CommandPlan ToCommandPlan(Tool tool)
    {
        if (tool is null) throw new ArgumentNullException(nameof(tool));
        var args = BuildVerbArguments().ToList();
        return new CommandPlan
        {
            Executable = tool.Executable.Value,
            Arguments = args,
            Environment = new Dictionary<string, string>(EnvironmentVariables),
            WorkingDirectory = WorkingDirectory,
            Secrets = CollectSecrets(),
        };
    }
}
