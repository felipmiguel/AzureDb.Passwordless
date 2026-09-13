# Task 02.02 Progress Details

## Summary
Upgraded vulnerable package versions and removed framework-included package references to clear restore-time security and downgrade failures.

## Changes Made
- Updated `Directory.Packages.props`:
  - `Azure.Identity` -> `1.21.0`
  - `Azure.Core` -> `1.53.0`
  - `Npgsql` -> `8.0.9`
  - `Npgsql.EntityFrameworkCore.PostgreSQL` -> `9.0.4`
  - `Pomelo.EntityFrameworkCore.MySql` -> `9.0.0`
  - `Microsoft.EntityFrameworkCore` / `Microsoft.EntityFrameworkCore.Design` -> `9.0.19`
  - `Microsoft.Extensions.DependencyInjection` -> `10.0.11`
  - `Microsoft.Extensions.Configuration*` -> `10.0.11`
  - `Microsoft.NET.Test.Sdk` -> `18.9.0`
  - Removed central entries for `System.Threading.Tasks.Extensions` and `System.ValueTuple`
- Removed framework-included `PackageReference` entries from project files:
  - `System.Threading.Tasks.Extensions` removed from `Batec.Core.TestFramework.csproj`
  - `System.ValueTuple` removed from test project files in Common, MySqlConnector, Npgsql, Npgsql.EntityFrameworkCore, and Pomelo.EntityFrameworkCore test projects
- Added repository `nuget.config` with package source mapping to satisfy NU1507 and allow `Batec.*` packages from `nuget.org` and `Ontime-Packages`.

## Validation
- `dotnet restore Batec.Azure.Data.Extensions.sln` now succeeds.
- NU1902/NU1903 restore blockers are resolved.
- Remaining build/test work is handled in subsequent API-fix and full-validation subtask.

## Files Modified
- `Directory.Packages.props`
- `nuget.config`
- `Batec.Core.TestFramework/Batec.Core.TestFramework.csproj`
- `Batec.Azure.Data.Extensions.Common/tests/Batec.Azure.Data.Extensions.Common.Tests.csproj`
- `Batec.Azure.Data.Extensions.MySqlConnector/tests/Batec.Azure.Data.Extensions.MySqlConnector.Tests.csproj`
- `Batec.Azure.Data.Extensions.Npgsql/tests/Batec.Azure.Data.Extensions.Npgsql.Tests.csproj`
- `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore/tests/Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests.csproj`
- `Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore/tests/Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests.csproj`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.02-package-updates-and-cleanup/task.md`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.02-package-updates-and-cleanup/progress-details.md`
