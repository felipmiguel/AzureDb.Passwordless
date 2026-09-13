# 02-upgrade-project-targets-and-packages: Upgrade project TFMs, packages, and API compatibility

Upgrade all project files from the current mixed target frameworks to the requested modern set. For projects currently targeting `net462;netstandard2.0`, remove .NET Framework and move to `netstandard2.1;net8.0;net10.0` where applicable. For projects currently targeting `net6.0;net7.0`, move to `net8.0;net10.0`. Apply package updates recommended by assessment, including vulnerability remediation, and remove packages now covered by framework references.

This task includes source-level compatibility fixes identified by assessment (`Api.0002`) in Npgsql-related projects. Because this spans many projects and dependency levels, execution will be decomposed into tiered subtasks.

## Scope Inventory

- Affected projects: all 13 projects in the solution (7 dependency-leaf libraries/samples and 6 dependent test projects).
- Distinct concerns:
  - TFM migration matrix centralized via `RequiredTargetFrameworks`
  - Package version/security upgrades in central package file
  - Removal of framework-included compatibility packages
  - API compatibility fix in Npgsql extension code
- Dependency shape: 3-level graph from assessment (`Level 0 -> Level 2`), requiring ordered validation.

## Research Findings

- TFM values are primarily controlled by `Directory.Build.props` (`RequiredTargetFrameworks`) and `Sample.Repository/Directory.Build.props`, with project-level overrides in EFCore provider projects.
- This repository already uses central package management (`Directory.Packages.props`), including vulnerable versions:
  - `Azure.Identity` 1.9.0 (NU1902/NU1903)
  - `Npgsql` 7.0.4 (NU1903)
- Packages flagged as framework-included and removable from project references include `System.Threading.Tasks.Extensions` and `System.ValueTuple`.
- Assessment API incompatibility is concentrated in `Batec.Azure.Data.Extensions.Npgsql/src/NpgsqlDataSourceBuilderExtensions.cs`.
- Baseline restore/build currently fails due warnings treated as errors (`NU1507`, `NU1902`, `NU1903`), so this task must resolve package/tooling compatibility as part of completion.

## Execution Plan

- Subtask 02.01: Update target framework matrices and project-level overrides.
- Subtask 02.02: Update central package versions and remove framework-included package references.
- Subtask 02.03: Apply API compatibility fixes and run full restore/build/test validation.

**Done when**: All project TFMs match the requested target set, .NET Framework targets are removed, required package updates/removals are applied (including vulnerable package upgrade), API compatibility issues are resolved inline, and the full solution builds/tests successfully.
