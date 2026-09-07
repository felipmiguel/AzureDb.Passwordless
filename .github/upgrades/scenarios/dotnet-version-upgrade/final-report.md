# .NET Version Upgrade — Report

**Scenario:** Upgrade .NET projects to newer .NET versions
**Outcome:** ✅ Fully completed
**Projects affected:** 13
**Tasks:** 5/5 completed

---

## Summary

The solution was upgraded from a mixed target baseline (`net462`, `netstandard2.0`, `net6.0`, `net7.0`) to modern targets aligned with the requested policy. .NET Framework targets were removed, shared library targeting was moved to `netstandard2.1;net8.0;net10.0`, and test/sample targets were moved to `net8.0;net10.0`.

Package vulnerabilities and downgrade conflicts were resolved, including updates to Azure, Npgsql, EF Core provider, and Microsoft.Extensions package lines. API compatibility changes required by newer Npgsql/EF provider versions were applied, and integration tests were adjusted to skip cleanly when required DB environment variables are missing.

---

## What Changed

### Packages

| Project | Package | Change | From → To |
|---------|---------|--------|-----------|
| Central (`Directory.Packages.props`) | Azure.Identity | Updated | 1.9.0 → 1.21.0 |
| Central (`Directory.Packages.props`) | Azure.Core | Updated | 1.34.0 → 1.53.0 |
| Central (`Directory.Packages.props`) | Npgsql | Updated | 7.0.4 → 8.0.9 |
| Central (`Directory.Packages.props`) | Npgsql.EntityFrameworkCore.PostgreSQL | Updated | 7.0.4 → 9.0.4 |
| Central (`Directory.Packages.props`) | Pomelo.EntityFrameworkCore.MySql | Updated | 7.0.0 → 9.0.0 |
| Central (`Directory.Packages.props`) | Microsoft.EntityFrameworkCore | Updated | 7.0.9 → 9.0.19 |
| Central (`Directory.Packages.props`) | Microsoft.EntityFrameworkCore.Design | Updated | 7.0.9 → 9.0.19 |
| Central (`Directory.Packages.props`) | Microsoft.Extensions.DependencyInjection | Updated | 7.0.0 → 10.0.11 |
| Central (`Directory.Packages.props`) | Microsoft.Extensions.Configuration* | Updated | 7.0.0 → 10.0.11 |
| Central (`Directory.Packages.props`) | Microsoft.NET.Test.Sdk | Updated | 17.6.3 → 18.9.0 |
| Multiple test/support projects | System.ValueTuple | Removed | 4.5.0 → framework-provided |
| Batec.Core.TestFramework | System.Threading.Tasks.Extensions | Removed | 4.5.4 → framework-provided |

### Code Modifications

- **Project file changes**
  - Updated framework matrices in `Directory.Build.props` and `Sample.Repository/Directory.Build.props`.
  - Updated provider project overrides:
	- `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.csproj`
	- `Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.csproj`

- **Configuration and restore/build tooling**
  - Added `nuget.config` with package source mapping to resolve NU1507 and ensure package-source compatibility for `Batec.*` packages.

- **API migrations**
  - `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/src/DbContextOptionsBuilderExtension.cs`
	- Replaced obsolete `ProvidePasswordCallback(...)` with `ConfigureDataSource(...)` + `UseAzureADAuthentication(...)`.
  - Removed obsolete `TrustServerCertificate` usage in:
	- `Batec.Azure.Data.Extensions.Npgsql/tests/NpgsqlTestEnvironment.cs`
	- `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/tests/NpgsqlTestEnvironment.cs`

- **Test behavior hardening**
  - `Batec.Core.TestFramework/TestEnvironment.cs`
	- Added env-var gating via `Assert.Ignore(...)` to skip integration tests when required DB variables are missing.
  - `Batec.Core.TestFramework/Batec.Core.TestFramework.csproj`
	- Set as support project to prevent direct test-host execution.

### Git Commits

| SHA | Message |
|-----|---------|
| c8ac8d5 | upgrade(03-final-validation-and-cleanup): finalize validation and test gating |
| 0407f0e | upgrade(02.03-api-fixes-and-validation): fix npgsql api compatibility |
| c9eab52 | upgrade(02.02-package-updates-and-cleanup): update vulnerable dependencies |
| 6d64c0e | upgrade(02.01-target-framework-matrix): retarget framework matrix |
| a5afe6a | upgrade(01-validate-upgrade-prerequisites): validate sdk and baseline |

---

## Task Breakdown

| Task | Description | Outcome | Content | Details |
|------|-------------|---------|---------|---------|
| `01-validate-upgrade-prerequisites` | Validate SDK/tooling prerequisites | ✅ SDK compatibility and baseline blockers identified | [task.md](tasks/01-validate-upgrade-prerequisites/task.md) | [progress-details.md](tasks/01-validate-upgrade-prerequisites/progress-details.md) |
| `02.01-target-framework-matrix` | Retarget framework matrices and project overrides | ✅ Removed `net462` and updated framework matrices | [task.md](tasks/02.01-target-framework-matrix/task.md) | [progress-details.md](tasks/02.01-target-framework-matrix/progress-details.md) |
| `02.02-package-updates-and-cleanup` | Upgrade vulnerable packages and cleanup framework-included references | ✅ Vulnerability blockers resolved and restore stabilized | [task.md](tasks/02.02-package-updates-and-cleanup/task.md) | [progress-details.md](tasks/02.02-package-updates-and-cleanup/progress-details.md) |
| `02.03-api-fixes-and-validation` | Fix API incompatibilities and validate solution | ✅ Npgsql/EF compatibility fixes applied and build validated | [task.md](tasks/02.03-api-fixes-and-validation/task.md) | [progress-details.md](tasks/02.03-api-fixes-and-validation/progress-details.md) |
| `03-final-validation-and-cleanup` | Final validation and cleanup | ✅ Restore/build/test pass with expected integration skips | [task.md](tasks/03-final-validation-and-cleanup/task.md) | [progress-details.md](tasks/03-final-validation-and-cleanup/progress-details.md) |

---

## Decisions Made

- **Flow mode:** Automatic.
- **Target framework policy:** `netstandard2.0` → `netstandard2.1`, remove .NET Framework targets, and use `net8.0/net10.0` for modern .NET targets.
- **Upgrade strategy:** Bottom-Up (dependency-first).
- **Project approach:** In-place migration.
- **Package management:** Keep existing per-project execution posture during migration (CPM already present in repository structure).
- **API handling:** Fix incompatible APIs inline (no deferred stubs).
- **Test behavior preference:** Skip integration tests when required DB environment variables are missing.

---

## Build & Test Results

| Scope | Build | Tests | Warnings |
|------|-------|-------|----------|
| Full solution (`Batec.Azure.Data.Extensions.sln`) | ✅ `dotnet build -c Release` succeeded | ✅ `dotnet test -c Release --no-build` succeeded (2 passed, 10 skipped, 0 failed) | 0 |
| Integration-path tests without DB env vars | N/A | ✅ Skipped intentionally via env-var gating | N/A |

---

## Known Gaps & Follow-up Items

- **CI integration coverage** — For full live integration verification, configure required DB infrastructure and environment variables in CI (PostgreSQL/MySQL endpoints and credentials).
- **Optional modernization** — Evaluate Aspire integration as a follow-up modernization step.
