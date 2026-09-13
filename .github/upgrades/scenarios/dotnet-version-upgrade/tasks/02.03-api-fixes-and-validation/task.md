# 02.03-api-fixes-and-validation: Fix API incompatibilities and validate full solution

## Objective
Apply required source compatibility fixes for net10.0 and validate the complete upgraded solution build and tests.

## Scope
- Npgsql extension API compatibility changes
- Full solution restore/build and targeted test runs

## Research Findings
- Assessment API findings were concentrated in Npgsql extension projects.
- After package upgrades, new compile blockers appeared as obsolete APIs treated as errors (`CS0618`):
  - `NpgsqlDbContextOptionsBuilder.ProvidePasswordCallback(...)`
  - `NpgsqlConnectionStringBuilder.TrustServerCertificate`
- Required migration pattern was to configure password provisioning through data source configuration for modern Npgsql/EF provider versions.

## Applied Fixes
- Updated EF Core extension to replace obsolete `ProvidePasswordCallback(...)` usage with `ConfigureDataSource(...)` and `UseAzureADAuthentication(...)`.
- Removed obsolete `TrustServerCertificate` assignment from Npgsql test environment builders in both Npgsql test projects.

## Validation Plan
1. Restore solution.
2. Build full solution in Release mode and ensure warning-free success for touched projects.
3. Run solution tests and capture any environment-dependent failures.

## Done when
- API compatibility issues flagged by assessment are resolved inline
- Full solution builds without errors
- Affected tests pass and warnings in touched projects are resolved
