# 02.02-package-updates-and-cleanup: Upgrade vulnerable/recommended packages and remove framework-included package references

## Objective
Address package-related upgrade findings by updating central package versions and removing package references now included in framework references.

## Scope
- Directory.Packages.props version updates
- Project files removing System.Threading.Tasks.Extensions and System.ValueTuple references where applicable
- Keep CPM in place (existing repository behavior)

## Research Findings
- The repository is already CPM-enabled via `Directory.Packages.props` (`ManagePackageVersionsCentrally=true`).
- Vulnerability blockers in restore are tied to:
  - `Azure.Identity` 1.9.0 (direct and transitive vulnerabilities including Microsoft.Identity.Client)
  - `Npgsql` 7.0.4
- Framework-included package references identified for removal:
  - `System.Threading.Tasks.Extensions` in `Batec.Core.TestFramework.csproj`
  - `System.ValueTuple` in multiple `*.Tests.csproj` files
- Supported package versions queried for updated TFMs:
  - `Azure.Identity` -> `1.21.0`
  - `Npgsql` -> `8.0.9` for net10/net8/netstandard2.1 scope
  - `Npgsql.EntityFrameworkCore.PostgreSQL` -> `9.0.4` for provider project scope
  - `Pomelo.EntityFrameworkCore.MySql` -> `9.0.0` for provider project scope

## Execution Plan
1. Update vulnerable package versions in `Directory.Packages.props`.
2. Update provider package versions to supported modern targets.
3. Remove framework-included package references from project files.
4. Validate restore no longer fails with NU1902/NU1903 vulnerability errors.

## Done when
- Vulnerable package versions are upgraded
- Framework-included packages are removed from project references
- Restore completes without NU1902/NU1903 vulnerability errors
