# Tamp.Playwright

Playwright CLI wrapper for [Tamp](https://github.com/tamp-build/tamp).

| Package | Playwright | Status |
|---|---|---|
| [`Tamp.Playwright.V1`](src/Tamp.Playwright.V1) | 1.x | preview |

Requires `Tamp.Core ≥ 1.0.3`.

## Why a separate repo

Playwright ships fast — new minor every 4–6 weeks. The satellite-repo
pattern lets the wrapper track Playwright independently of `tamp` core.

## Install

```xml
<PackageVersion Include="Tamp.Playwright.V1" Version="0.1.0" />
```

```xml
<PackageReference Include="Tamp.Playwright.V1" />
```

## Verbs

| Verb | Notes |
|---|---|
| `Test` | The canonical CI verb. Browsers, projects, sharding (`SetShard(1, 4)`), reporters, retries, traces, snapshots, UI mode. |
| `Install` | Browser install. `--with-deps`, `--dry-run`, `--force`, `--no-shell` / `--only-shell`. |
| `InstallDeps` | OS-level deps only (Linux). |
| `Uninstall` | Remove browsers. `--all`. |
| `Codegen` | Recorder UI. `--target`, `--browser`, `--output`, `--device`, `--viewport-size`. |
| `ShowReport` | Open HTML report. |
| `MergeReports` | Stitch sharded blob reports. |
| `ClearCache` | Wipe browser/asset cache. |
| `Open` | Inspector against URL. |
| `Raw` | Escape hatch. |

## Quick example — sharded CI run

```csharp
using Tamp;
using Tamp.Playwright.V1;

[NuGetPackage("playwright", UseSystemPath = true)]
readonly Tool PlaywrightTool = null!;

[Parameter("Shard index (1-based)")]
readonly int Shard;

[Parameter("Total shards")]
readonly int TotalShards = 4;

Target InstallBrowsers => _ => _.Executes(() =>
    Playwright.Install(PlaywrightTool, s => s
        .AddBrowser("chromium")
        .SetWithDeps()
        .SetWorkingDirectory(RootDirectory / "frontend")));

Target E2E => _ => _
    .DependsOn(nameof(InstallBrowsers))
    .Executes(() =>
        Playwright.Test(PlaywrightTool, s => s
            .SetForbidOnly()
            .SetShard(Shard, TotalShards)
            .AddReporter("blob")
            .AddReporter("junit")
            .SetTrace("on-first-retry")
            .SetRetries(2)
            .SetWorkers(4)
            .SetWorkingDirectory(RootDirectory / "frontend")));

Target MergeShards => _ => _.Executes(() =>
    Playwright.MergeReports(PlaywrightTool, s => s
        .AddReporter("html")
        .AddReporter("junit")
        .SetReportFolder("blob-reports")
        .SetWorkingDirectory(RootDirectory / "frontend")));
```

## Releasing

See [MAINTAINERS.md](MAINTAINERS.md).
