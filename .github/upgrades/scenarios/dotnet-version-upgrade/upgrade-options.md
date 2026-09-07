# Upgrade Options — Batec.Azure.Data.Extensions

Assessment: 13 projects with mixed net462/netstandard2.0/net6.0/net7.0 targets, dependency depth to level 2, and API compatibility findings for net10.0.

## Strategy

### Upgrade Strategy
.NET Framework targets are present across multiple projects, so tiered dependency migration is required.

| Value | Description |
|-------|-------------|
| **Bottom-Up** (selected) | Upgrade dependency-leaf libraries first and move upward tier by tier with validation at each tier. |

## Project Structure

### Project Approach
Projects are class libraries and test projects, and the requested end state removes .NET Framework targets.

| Value | Description |
|-------|-------------|
| Multi-targeting | Keep legacy and modern targets temporarily to support mixed consumers during transition. |
| **In-place** (selected) | Replace legacy targets directly with the modern target set and migrate consumers in upgrade order. |

### Package Management
The solution has many projects without centralized package management and crosses the .NET Framework to modern .NET boundary.

| Value | Description |
|-------|-------------|
| Central Package Management (CPM) | Create Directory.Packages.props and centralize package versions during the upgrade. |
| **Per-Project (defer CPM to post-migration)** (selected) | Keep per-project package versions during migration and defer CPM until all projects are on modern targets. |

## Compatibility

### Unsupported API Handling
Assessment flagged source-incompatible APIs that require code-level changes for net10.0.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API incompatibilities within the same upgrade task, without stubs. |
| Defer Complex Changes | Apply simple fixes now and defer complex API replacements behind temporary stubs and follow-up subtasks. |
