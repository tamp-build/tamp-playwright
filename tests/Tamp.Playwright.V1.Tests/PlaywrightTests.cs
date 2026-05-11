using System.IO;
using Bogus;
using Tamp;
using Xunit;

namespace Tamp.Playwright.V1.Tests;

public sealed class PlaywrightTests
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
    public void Every_Verb_Uses_Tool_Path()
    {
        var t = FakeTool();
        Assert.Equal(t.Executable.Value, Playwright.Test(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.Install(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.InstallDeps(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.Uninstall(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.Codegen(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.ShowReport(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.MergeReports(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.ClearCache(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.Open(t).Executable);
        Assert.Equal(t.Executable.Value, Playwright.Raw(t, "--version").Executable);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("install")]
    [InlineData("install-deps")]
    [InlineData("uninstall")]
    [InlineData("codegen")]
    [InlineData("show-report")]
    [InlineData("merge-reports")]
    [InlineData("clear-cache")]
    [InlineData("open")]
    public void Verbs_Begin_With_Their_Verb_Token(string verb)
    {
        var plan = verb switch
        {
            "test" => Playwright.Test(FakeTool()),
            "install" => Playwright.Install(FakeTool()),
            "install-deps" => Playwright.InstallDeps(FakeTool()),
            "uninstall" => Playwright.Uninstall(FakeTool()),
            "codegen" => Playwright.Codegen(FakeTool()),
            "show-report" => Playwright.ShowReport(FakeTool()),
            "merge-reports" => Playwright.MergeReports(FakeTool()),
            "clear-cache" => Playwright.ClearCache(FakeTool()),
            "open" => Playwright.Open(FakeTool()),
            _ => throw new InvalidOperationException()
        };
        Assert.Equal(verb, plan.Arguments[0]);
    }

    // ---------- test ----------

    [Fact]
    public void Test_Default_Is_Just_The_Verb()
    {
        Assert.Equal(["test"], Playwright.Test(FakeTool()).Arguments);
    }

    [Fact]
    public void Test_All_Major_Flags_Round_Trip()
    {
        var plan = Playwright.Test(FakeTool(), s => s
            .SetConfig("playwright.config.ts")
            .AddBrowser("chromium")
            .AddBrowser("firefox")
            .AddProject("smoke")
            .AddProject("e2e")
            .SetGrep("auth.*login")
            .SetGrepInvert(".*flaky.*")
            .SetHeaded()
            .SetForbidOnly()
            .SetFullyParallel()
            .SetGlobalTimeout(900_000)
            .SetTimeout(30_000)
            .SetMaxFailures(5)
            .SetRetries(2)
            .SetWorkers(4)
            .SetRepeatEach(3)
            .SetShard(2, 4)
            .AddReporter("list")
            .AddReporter("junit")
            .SetOutput("test-results")
            .SetTrace("on-first-retry")
            .SetQuiet()
            .SetPassWithNoTests()
            .SetIgnoreSnapshots());
        var args = plan.Arguments;

        Assert.Contains("--config", args); Assert.Contains("playwright.config.ts", args);

        // browsers + projects emit as repeated flag pairs in declared order
        var b1 = IndexOf(args, "--browser");
        var b2 = IndexOf(args, "--browser", b1 + 1);
        Assert.Equal("chromium", args[b1 + 1]);
        Assert.Equal("firefox", args[b2 + 1]);

        var p1 = IndexOf(args, "--project");
        var p2 = IndexOf(args, "--project", p1 + 1);
        Assert.Equal("smoke", args[p1 + 1]);
        Assert.Equal("e2e", args[p2 + 1]);

        Assert.Contains("--grep", args); Assert.Contains("auth.*login", args);
        Assert.Contains("--grep-invert", args); Assert.Contains(".*flaky.*", args);
        Assert.Contains("--headed", args);
        Assert.Contains("--forbid-only", args);
        Assert.Contains("--fully-parallel", args);
        Assert.Contains("--global-timeout", args); Assert.Contains("900000", args);
        Assert.Contains("--timeout", args); Assert.Contains("30000", args);
        Assert.Contains("--max-failures", args); Assert.Contains("5", args);
        Assert.Contains("--retries", args); Assert.Contains("2", args);
        Assert.Contains("--workers", args); Assert.Contains("4", args);
        Assert.Contains("--repeat-each", args); Assert.Contains("3", args);
        Assert.Contains("--shard", args); Assert.Contains("2/4", args);

        var r1 = IndexOf(args, "--reporter");
        var r2 = IndexOf(args, "--reporter", r1 + 1);
        Assert.Equal("list", args[r1 + 1]);
        Assert.Equal("junit", args[r2 + 1]);

        Assert.Contains("--output", args); Assert.Contains("test-results", args);
        Assert.Contains("--trace", args); Assert.Contains("on-first-retry", args);
        Assert.Contains("--quiet", args);
        Assert.Contains("--pass-with-no-tests", args);
        Assert.Contains("--ignore-snapshots", args);
    }

    [Fact]
    public void Test_UpdateSnapshots_Without_Mode_Just_Emits_Flag()
    {
        var plan = Playwright.Test(FakeTool(), s => s.SetUpdateSnapshots());
        var args = plan.Arguments;
        var idx = IndexOf(args, "--update-snapshots");
        Assert.True(idx >= 0);
        // Either last or followed by a positional / unrelated flag.
        Assert.True(idx == args.Count - 1 || args[idx + 1].StartsWith("--") || args[idx + 1].Contains('/'));
    }

    [Fact]
    public void Test_UpdateSnapshots_With_Mode_Emits_Mode()
    {
        var plan = Playwright.Test(FakeTool(), s => s.SetUpdateSnapshots("changed"));
        var args = plan.Arguments;
        var idx = IndexOf(args, "--update-snapshots");
        Assert.Equal("changed", args[idx + 1]);
    }

    [Fact]
    public void Test_OnlyChanged_With_And_Without_Ref()
    {
        var withRef = Playwright.Test(FakeTool(), s => s.SetOnlyChanged("main"));
        var idx1 = IndexOf(withRef.Arguments, "--only-changed");
        Assert.Equal("main", withRef.Arguments[idx1 + 1]);

        var noRef = Playwright.Test(FakeTool(), s => s.SetOnlyChanged());
        var idx2 = IndexOf(noRef.Arguments, "--only-changed");
        Assert.True(idx2 == noRef.Arguments.Count - 1 || noRef.Arguments[idx2 + 1].StartsWith("--"));
    }

    [Fact]
    public void Test_File_Filters_Tail_The_Verb()
    {
        var plan = Playwright.Test(FakeTool(), s => s
            .AddFileFilter("tests/auth/**/*.spec.ts")
            .AddFileFilter("tests/billing/**/*.spec.ts"));
        var args = plan.Arguments;
        Assert.Equal("tests/auth/**/*.spec.ts", args[^2]);
        Assert.Equal("tests/billing/**/*.spec.ts", args[^1]);
    }

    [Fact]
    public void Test_Ui_Host_And_Port()
    {
        var plan = Playwright.Test(FakeTool(), s => s.SetUi().SetUiHost("0.0.0.0").SetUiPort(7777));
        Assert.Contains("--ui", plan.Arguments);
        Assert.Contains("--ui-host", plan.Arguments);
        Assert.Contains("0.0.0.0", plan.Arguments);
        Assert.Contains("--ui-port", plan.Arguments);
        Assert.Contains("7777", plan.Arguments);
    }

    [Fact]
    public void Test_List_Flag_Round_Trips()
    {
        var plan = Playwright.Test(FakeTool(), s => s.SetList());
        Assert.Contains("--list", plan.Arguments);
    }

    [Fact]
    public void Test_NoDeps_Round_Trips()
    {
        var plan = Playwright.Test(FakeTool(), s => s.SetNoDeps());
        Assert.Contains("--no-deps", plan.Arguments);
    }

    // ---------- install ----------

    [Fact]
    public void Install_All_Browsers_When_None_Specified()
    {
        var plan = Playwright.Install(FakeTool());
        Assert.Equal(["install"], plan.Arguments);
    }

    [Fact]
    public void Install_With_Browsers_And_Flags()
    {
        var plan = Playwright.Install(FakeTool(), s => s
            .AddBrowser("chromium")
            .AddBrowser("webkit")
            .SetWithDeps()
            .SetForce()
            .SetOnlyShell());
        var args = plan.Arguments;
        Assert.Contains("--with-deps", args);
        Assert.Contains("--force", args);
        Assert.Contains("--only-shell", args);
        Assert.Equal("chromium", args[^2]);
        Assert.Equal("webkit", args[^1]);
    }

    [Fact]
    public void Install_DryRun_Round_Trips()
    {
        var plan = Playwright.Install(FakeTool(), s => s.SetDryRun().AddBrowser("chromium"));
        Assert.Contains("--dry-run", plan.Arguments);
    }

    [Fact]
    public void Install_NoShell_And_OnlyShell_Can_Both_Be_Set()
    {
        // The CLI rejects this combination at runtime, but the wrapper
        // shouldn't pre-judge — emit what the user said. This guards
        // against well-meaning "smart" filtering that hides real bugs.
        var plan = Playwright.Install(FakeTool(), s => s.SetNoShell().SetOnlyShell());
        Assert.Contains("--no-shell", plan.Arguments);
        Assert.Contains("--only-shell", plan.Arguments);
    }

    [Fact]
    public void InstallDeps_With_Browsers()
    {
        var plan = Playwright.InstallDeps(FakeTool(), s => s.AddBrowser("chromium"));
        Assert.Equal(["install-deps", "chromium"], plan.Arguments);
    }

    [Fact]
    public void InstallDeps_DryRun_Round_Trips()
    {
        var plan = Playwright.InstallDeps(FakeTool(), s => s.SetDryRun());
        Assert.Contains("--dry-run", plan.Arguments);
    }

    [Fact]
    public void Uninstall_All_Flag()
    {
        var plan = Playwright.Uninstall(FakeTool(), s => s.SetAll());
        Assert.Equal(["uninstall", "--all"], plan.Arguments);
    }

    // ---------- codegen ----------

    [Fact]
    public void Codegen_Url_Tails_The_Verb()
    {
        var plan = Playwright.Codegen(FakeTool(), s => s
            .SetTarget("csharp")
            .SetBrowser("chromium")
            .SetOutput("captured.cs")
            .SetUrl("https://example.com"));
        var args = plan.Arguments;
        Assert.Equal("https://example.com", args[^1]);
        Assert.Contains("--target", args);
        Assert.Contains("csharp", args);
        Assert.Contains("--browser", args);
        Assert.Contains("--output", args);
        Assert.Contains("captured.cs", args);
    }

    [Fact]
    public void Codegen_Device_And_ViewportSize()
    {
        var plan = Playwright.Codegen(FakeTool(), s => s
            .SetDevice("iPhone 13")
            .SetViewportSize("1280,720"));
        Assert.Contains("--device", plan.Arguments);
        Assert.Contains("iPhone 13", plan.Arguments);
        Assert.Contains("--viewport-size", plan.Arguments);
        Assert.Contains("1280,720", plan.Arguments);
    }

    // ---------- reports ----------

    [Fact]
    public void ShowReport_With_Folder()
    {
        var plan = Playwright.ShowReport(FakeTool(), s => s
            .SetHost("0.0.0.0")
            .SetPort(9323)
            .SetReportFolder("playwright-report"));
        var args = plan.Arguments;
        Assert.Contains("--host", args);
        Assert.Contains("0.0.0.0", args);
        Assert.Contains("--port", args);
        Assert.Contains("9323", args);
        Assert.Equal("playwright-report", args[^1]);
    }

    [Fact]
    public void MergeReports_Reporter_And_Folder()
    {
        var plan = Playwright.MergeReports(FakeTool(), s => s
            .SetConfig("playwright.config.ts")
            .AddReporter("html")
            .AddReporter("junit")
            .SetReportFolder("blob-reports"));
        var args = plan.Arguments;
        Assert.Equal("merge-reports", args[0]);
        Assert.Contains("--config", args);
        var r1 = IndexOf(args, "--reporter");
        var r2 = IndexOf(args, "--reporter", r1 + 1);
        Assert.Equal("html", args[r1 + 1]);
        Assert.Equal("junit", args[r2 + 1]);
        Assert.Equal("blob-reports", args[^1]);
    }

    [Fact]
    public void ClearCache_Is_Just_The_Verb()
    {
        Assert.Equal(["clear-cache"], Playwright.ClearCache(FakeTool()).Arguments);
    }

    [Fact]
    public void Open_With_All_Flags()
    {
        var plan = Playwright.Open(FakeTool(), s => s
            .SetBrowser("firefox")
            .SetDevice("Pixel 5")
            .SetViewportSize("1024,768")
            .SetLoadStorage("auth.json")
            .SetUrl("https://example.com"));
        var args = plan.Arguments;
        Assert.Equal("open", args[0]);
        Assert.Equal("https://example.com", args[^1]);
    }

    // ---------- raw ----------

    [Fact]
    public void Raw_Requires_Args()
    {
        Assert.Throws<ArgumentException>(() => Playwright.Raw(FakeTool()));
    }

    [Fact]
    public void Raw_Forwards_Verbatim()
    {
        var plan = Playwright.Raw(FakeTool(), "--version");
        Assert.Equal(["--version"], plan.Arguments);
    }

    // ---------- null guards ----------

    [Fact]
    public void Null_Tool_Throws_For_Every_Verb()
    {
        Assert.Throws<ArgumentNullException>(() => Playwright.Test(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Install(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.InstallDeps(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Uninstall(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Codegen(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.ShowReport(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.MergeReports(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.ClearCache(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Open(null!));
        Assert.Throws<ArgumentNullException>(() => Playwright.Raw(null!, "--help"));
    }

    [Fact]
    public void Test_Many_Browsers_Preserve_Order_Under_Random_Names()
    {
        // Repeat-flag ordering is observable to test consumers (browser
        // priority for shared-resource contention etc.), so verify.
        var faker = new Faker();
        var names = Enumerable.Range(0, 5).Select(_ => faker.Random.AlphaNumeric(6)).ToArray();
        var plan = Playwright.Test(FakeTool(), s =>
        {
            foreach (var n in names) s.AddBrowser(n);
        });
        var observed = new List<string>();
        for (var i = 0; i < plan.Arguments.Count - 1; i++)
            if (plan.Arguments[i] == "--browser") observed.Add(plan.Arguments[i + 1]);
        Assert.Equal(names, observed);
    }

    [Fact]
    public void WorkingDirectory_And_Env_Flow_To_Plan()
    {
        var cwd = Path.GetTempPath();
        var plan = Playwright.Test(FakeTool(), s => s
            .SetWorkingDirectory(cwd)
            .SetEnv("CI", "true"));
        Assert.Equal(cwd, plan.WorkingDirectory);
        Assert.Equal("true", plan.Environment["CI"]);
    }
}
