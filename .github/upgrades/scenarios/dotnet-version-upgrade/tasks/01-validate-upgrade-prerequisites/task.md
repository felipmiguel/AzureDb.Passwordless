# 01-validate-upgrade-prerequisites: Validate SDK/tooling prerequisites

Validate that the local SDK/toolchain can build the solution for `net8.0` and `net10.0`, and confirm there are no workflow blockers before applying project-wide TFM updates. This task establishes the baseline needed for stable execution and prevents partial migration failures caused by environment mismatch.

The task also confirms strategy constraints are actionable (in-place migration, per-project package management during migration, inline API fixes) and prepares execution to proceed directly into project updates.

## Scope Inventory

- Affected projects: full solution validation scope (`Batec.Azure.Data.Extensions.sln`)
- Distinct concerns: SDK availability, global.json compatibility, baseline restore/build behavior
- Change signals: restore/build currently blocked by package vulnerability warnings-as-errors and NU1507 source-mapping warning-as-error in selected projects

## Research Findings

- `validate_dotnet_sdk_installation(net10.0)` succeeded (compatible SDK installed).
- `validate_dotnet_sdk_in_globaljson(net10.0)` reported no `global.json`, so no SDK pinning blocker exists.
- Baseline `dotnet restore`/`dotnet build` fail before migration due to existing package-security warnings treated as errors (`NU1902`/`NU1903`) and package-source warning as error (`NU1507`) in current state.
- These failures are not SDK/tooling blockers; they are migration-work items that will be handled in Task 02 by package upgrades and project-level cleanup.

## Execution Decision

Task is atomic. No decomposition required.

**Done when**: .NET SDK compatibility for target frameworks is confirmed, restore/build prerequisites are validated, and no environment blockers remain for upgrade execution.
