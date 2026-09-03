# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0
- **Targeting Preference**: Upgrade netstandard2.0 to netstandard2.1, remove .NET Framework targets, and use net8.0/net10.0 for modern .NET targets.

## Source Control
- **Source Branch**: master
- **Working Branch**: dotnet-version-upgrade-net10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: Bottom-Up

### Project Structure
- Project Approach: In-place
- Package Management: Per-Project (defer CPM to post-migration)

### Compatibility
- Unsupported API Handling: Fix Inline

## Strategy
**Selected**: Bottom-Up (Dependency-First)
**Rationale**: The solution contains 13 projects with .NET Framework targets and a 3-level dependency graph, so dependency-first execution minimizes migration risk.

### Execution Constraints
- Execute framework migration in dependency order, with task-level decomposition by dependency tiers.
- Keep package management per-project during migration; defer central package management until post-migration stabilization.
- Resolve API incompatibilities inline during upgrade tasks (no deferred stub tracks).
- Validate build and tests after each completed task before proceeding.
