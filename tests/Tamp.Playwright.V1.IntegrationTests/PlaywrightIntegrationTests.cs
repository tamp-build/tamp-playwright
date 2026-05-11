using System.IO;
using Tamp;
using Xunit;
using Xunit.Abstractions;

namespace Tamp.Playwright.V1.IntegrationTests;

/// <summary>
/// Exercises the wrapper against a real Playwright 1.x CLI. We
/// intentionally limit to browser-independent verbs:
///
///   - Playwright's <c>test --list</c> still preflights browser
///     downloads under 1.x (even with --list, project config drives
///     browser checks). That makes <c>test</c> unrunnable in CI
///     without first burning ~300MB on browser installs per OS.
///   - The wrapper itself is just CLI argv synthesis, so we don't
///     need a real run to cover correctness — unit tests do that.
///
/// What we DO cover here:
///   - <c>--version</c> via Raw (proves wrapper-to-process plumbing)
///   - <c>install --dry-run</c> with single + multiple browsers
///     (proves repeated positional emission round-trips)
///   - <c>--help</c> via Raw, comparing the wrapper's argv to a
///     known-good shape of the CLI surface
/// </summary>
public sealed class PlaywrightIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public PlaywrightIntegrationTests(ITestOutputHelper output) => _output = output;

    private static string? ResolveOnPath(string baseName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        var names = OperatingSystem.IsWindows()
            ? new[] { $"{baseName}.cmd", $"{baseName}.exe", $"{baseName}.bat", $"{baseName}.ps1", baseName }
            : new[] { baseName };
        foreach (var dir in pathEnv.Split(Path.PathSeparator))
        {
            if (string.IsNullOrEmpty(dir)) continue;
            foreach (var n in names)
            {
                var c = Path.Combine(dir, n);
                if (File.Exists(c)) return c;
            }
        }
        return null;
    }

    private static Tool ResolveTool() =>
        new(AbsolutePath.Create(ResolveOnPath("playwright")
            ?? throw new InvalidOperationException("playwright not found on PATH. Install: npm i -g @playwright/test@1")));

    private CaptureResult Run(CommandPlan plan)
    {
        _output.WriteLine($"$ {plan.Executable} {string.Join(' ', plan.Arguments)}");
        var result = ProcessRunner.Capture(plan);
        foreach (var line in result.Lines)
            _output.WriteLine($"  [{line.Type}] {line.Text}");
        _output.WriteLine($"  → exit {result.ExitCode}");
        return result;
    }

    [Fact]
    public void Raw_Version_Reports_1_x()
    {
        var tool = ResolveTool();
        var plan = Playwright.Raw(tool, "--version");
        var result = Run(plan);
        Assert.Equal(0, result.ExitCode);
        var combined = result.StdoutText + result.StderrText;
        Assert.Matches(@"[Vv]ersion\s+1\.\d+\.\d+|1\.\d+\.\d+", combined);
    }

    [Fact]
    public void Install_DryRun_Single_Browser()
    {
        var tool = ResolveTool();
        var plan = Playwright.Install(tool, s => s.SetDryRun().AddBrowser("chromium"));
        var result = Run(plan);
        Assert.Equal(0, result.ExitCode);
        var combined = result.StdoutText + result.StderrText;
        Assert.Contains("chromium", combined, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Install_DryRun_Multiple_Browsers_Round_Trips()
    {
        // Repeated positional args at the tail — verifies the wrapper's
        // emission order is what the CLI parser expects.
        var tool = ResolveTool();
        var plan = Playwright.Install(tool, s => s
            .SetDryRun()
            .AddBrowser("chromium")
            .AddBrowser("firefox"));
        var result = Run(plan);
        Assert.Equal(0, result.ExitCode);
        var combined = result.StdoutText + result.StderrText;
        Assert.Contains("chromium", combined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("firefox", combined, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Raw_Test_Help_Surfaces_Expected_Flags()
    {
        // `playwright test --help` prints the test verb's argument table
        // — confirms our wrapper's flag names match the CLI's. If
        // Playwright renames a flag in a future minor, this is the
        // shape that breaks first.
        var tool = ResolveTool();
        var plan = Playwright.Raw(tool, "test", "--help");
        var result = Run(plan);
        Assert.Equal(0, result.ExitCode);
        var combined = result.StdoutText + result.StderrText;
        foreach (var flag in new[] { "--config", "--browser", "--project", "--shard", "--reporter", "--retries", "--workers", "--grep", "--max-failures", "--update-snapshots" })
        {
            Assert.Contains(flag, combined);
        }
    }

    [Fact]
    public void Raw_Install_Help_Surfaces_Expected_Flags()
    {
        var tool = ResolveTool();
        var plan = Playwright.Raw(tool, "install", "--help");
        var result = Run(plan);
        Assert.Equal(0, result.ExitCode);
        var combined = result.StdoutText + result.StderrText;
        foreach (var flag in new[] { "--with-deps", "--dry-run", "--force" })
        {
            Assert.Contains(flag, combined);
        }
    }
}
