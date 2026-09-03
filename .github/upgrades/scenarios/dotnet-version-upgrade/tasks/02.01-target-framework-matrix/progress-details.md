# Task 02.01 Progress Details

## Summary
Updated the framework target matrices to remove .NET Framework and move to the requested modern target sets.

## Changes Made
- Updated root target framework defaults in `Directory.Build.props`:
  - non-test projects: `net10.0;net8.0;netstandard2.1`
  - test/sample/support projects: `net10.0;net8.0`
- Updated `Sample.Repository/Directory.Build.props` to `net10.0;net8.0`.
- Updated project-level overrides:
  - `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.csproj` → `net10.0;net8.0`
  - `Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.csproj` → `net10.0;net8.0`
- Enriched subtask `task.md` with scope inventory and research findings.

## Validation
- Verified no `net462` remains in `.csproj`/`.props` files outside `obj/bin` and workflow artifacts.
- `dotnet restore` progressed without target-framework mismatch errors; remaining failures are package vulnerability/source-mapping issues to resolve in package subtask.

## Files Modified
- `Directory.Build.props`
- `Sample.Repository/Directory.Build.props`
- `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.csproj`
- `Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore/src/Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.csproj`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.01-target-framework-matrix/task.md`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.01-target-framework-matrix/progress-details.md`
