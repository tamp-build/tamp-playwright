using System.IO;
using Tamp;
using Xunit;

namespace Tamp.Playwright.V1.Tests;

/// <summary>
/// TAM-161: every wrapper accepts both <c>Action&lt;TSettings&gt;</c> (fluent)
/// and a pre-built <c>TSettings</c> (object-init). The two forms must produce
/// identical CommandPlans.
/// </summary>
public sealed class ObjectInitTests
{
    private static Tool FakeTool(string name = "playwright") =>
        new(AbsolutePath.Create(Path.Combine(Path.GetTempPath(), name)));

    private static int IndexOf(IReadOnlyList<string> args, string value, int start = 0)
    {
        for (var i = start; i < args.Count; i++)
            if (args[i] == value) return i;
        return -1;
    }

    [Fact]
    public void Test_ObjectInit_Emits_Identical_Plan_To_Fluent()
    {
        var t = FakeTool();
        var fluent = Playwright.Test(t, s => s
            .SetConfig("playwright.config.ts")
            .AddBrowser("chromium")
            .AddBrowser("firefox")
            .AddProject("smoke")
            .SetGrep("auth.*")
            .SetHeaded()
            .SetForbidOnly()
            .SetRetries(2)
            .SetWorkers(4)
            .SetShard(2, 4)
            .AddReporter("list")
            .AddReporter("junit")
            .SetTrace("on-first-retry")
            .SetPassWithNoTests());

        var objectInit = Playwright.Test(t, new PlaywrightTestSettings
        {
            Config = "playwright.config.ts",
            Browsers = { "chromium", "firefox" },
            Projects = { "smoke" },
            Grep = "auth.*",
            Headed = true,
            ForbidOnly = true,
            Retries = 2,
            Workers = "4",
            Shard = "2/4",
            Reporters = { "list", "junit" },
            Trace = "on-first-retry",
            PassWithNoTests = true,
        });

        Assert.Equal(fluent.Executable, objectInit.Executable);
        Assert.Equal(fluent.Arguments, objectInit.Arguments);
    }

    [Fact]
    public void Install_ObjectInit_Emits_Identical_Plan_To_Fluent()
    {
        var t = FakeTool();
        var fluent = Playwright.Install(t, s => s
            .AddBrowser("chromium")
            .AddBrowser("webkit")
            .SetWithDeps()
            .SetForce()
            .SetOnlyShell());

        var objectInit = Playwright.Install(t, new PlaywrightInstallSettings
        {
            Browsers = { "chromium", "webkit" },
            WithDeps = true,
            Force = true,
            OnlyShell = true,
        });

        Assert.Equal(fluent.Arguments, objectInit.Arguments);
    }

    [Fact]
    public void Codegen_ObjectInit_Round_Trips()
    {
        var args = Playwright.Codegen(FakeTool(), new PlaywrightCodegenSettings
        {
            Target = "csharp",
            Browser = "chromium",
            Output = "captured.cs",
            Device = "iPhone 13",
            ViewportSize = "1280,720",
            Url = "https://example.com",
        }).Arguments;

        Assert.Contains("--target", args);
        Assert.Equal("csharp", args[IndexOf(args, "--target") + 1]);
        Assert.Contains("--browser", args);
        Assert.Equal("chromium", args[IndexOf(args, "--browser") + 1]);
        Assert.Contains("--device", args);
        Assert.Equal("iPhone 13", args[IndexOf(args, "--device") + 1]);
        Assert.Contains("--viewport-size", args);
        Assert.Equal("1280,720", args[IndexOf(args, "--viewport-size") + 1]);
        // url tails the verb
        Assert.Equal("https://example.com", args[^1]);
    }

    [Fact]
    public void ShowReport_ObjectInit_Round_Trips()
    {
        var args = Playwright.ShowReport(FakeTool(), new PlaywrightShowReportSettings
        {
            Host = "0.0.0.0",
            Port = 9323,
            ReportFolder = "playwright-report",
        }).Arguments;

        Assert.Contains("--host", args);
        Assert.Equal("0.0.0.0", args[IndexOf(args, "--host") + 1]);
        Assert.Contains("--port", args);
        Assert.Equal("9323", args[IndexOf(args, "--port") + 1]);
        Assert.Equal("playwright-report", args[^1]);
    }

    [Fact]
    public void MergeReports_ObjectInit_Round_Trips()
    {
        var args = Playwright.MergeReports(FakeTool(), new PlaywrightMergeReportsSettings
        {
            Config = "playwright.config.ts",
            Reporters = { "html", "junit" },
            ReportFolder = "blob-reports",
        }).Arguments;

        Assert.Equal("merge-reports", args[0]);
        Assert.Contains("--config", args);
        var r1 = IndexOf(args, "--reporter");
        var r2 = IndexOf(args, "--reporter", r1 + 1);
        Assert.Equal("html", args[r1 + 1]);
        Assert.Equal("junit", args[r2 + 1]);
        Assert.Equal("blob-reports", args[^1]);
    }

    [Fact]
    public void Open_ObjectInit_Round_Trips()
    {
        var args = Playwright.Open(FakeTool(), new PlaywrightOpenSettings
        {
            Browser = "firefox",
            Device = "Pixel 5",
            ViewportSize = "1024,768",
            LoadStorage = "auth.json",
            Url = "https://example.com",
        }).Arguments;

        Assert.Equal("open", args[0]);
        Assert.Equal("https://example.com", args[^1]);
        Assert.Contains("firefox", args);
        Assert.Contains("auth.json", args);
    }

    [Fact]
    public void Remaining_Verbs_ObjectInit_Surface_Compiles_And_Returns_CommandPlan()
    {
        // Smoke: every remaining wrapper accepts an object-init settings arg and returns a non-null CommandPlan.
        var t = FakeTool();
        Assert.NotNull(Playwright.InstallDeps(t, new PlaywrightInstallDepsSettings { Browsers = { "chromium" }, DryRun = true }));
        Assert.NotNull(Playwright.Uninstall(t, new PlaywrightUninstallSettings { All = true }));
        Assert.NotNull(Playwright.ClearCache(t, new PlaywrightClearCacheSettings()));
    }

    [Fact]
    public void ObjectInit_Null_Settings_Throws()
    {
        var t = FakeTool();
        Assert.Throws<ArgumentNullException>(() => Playwright.Test(t, (PlaywrightTestSettings)null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Install(t, (PlaywrightInstallSettings)null!));
    }

    [Fact]
    public void ObjectInit_Null_Tool_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Playwright.Test(null!, new PlaywrightTestSettings()));
        Assert.Throws<ArgumentNullException>(() => Playwright.ClearCache(null!, new PlaywrightClearCacheSettings()));
    }
}
