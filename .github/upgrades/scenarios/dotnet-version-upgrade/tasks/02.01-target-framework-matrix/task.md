# 02.01-target-framework-matrix: Retarget framework matrices and project overrides

## Objective
Update all target framework definitions to the requested set: class-library matrix to include netstandard2.1 and modern TFMs, remove net462 everywhere, and update test/sample matrices to net8.0/net10.0.

## Scope
- Directory.Build.props
- Sample.Repository/Directory.Build.props
- Project-level RequiredTargetFrameworks overrides in provider projects

## Research Findings
- Root `Directory.Build.props` currently drives most project TFMs through `RequiredTargetFrameworks`, with non-test projects on `net462;netstandard2.0` and test/sample projects on `net7.0;net6.0`.
- `Sample.Repository/Directory.Build.props` overrides sample/test matrix to `net7.0;net6.0` and must be updated for consistency.
- Provider projects `Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore` and `Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore` each define local `RequiredTargetFrameworks` currently set to `net6.0;net7.0`.
- All affected project files already use `TargetFrameworks>$(RequiredTargetFrameworks)</TargetFrameworks>`, so changing these properties updates all TFMs without per-project edits.

## Execution Plan
1. Update root framework matrix to remove .NET Framework and adopt `net10.0;net8.0;netstandard2.1` for non-test targets.
2. Update test/sample matrix to `net10.0;net8.0` in both root and sample-local props.
3. Update provider project local overrides to `net10.0;net8.0`.
4. Validate by restoring solution and confirming no remaining `net462` in project/props definitions.

## Done when
- All RequiredTargetFrameworks values reflect the requested modern targets
- No project resolves to net462
- Solution restore reaches package validation stage without TFM mismatches
