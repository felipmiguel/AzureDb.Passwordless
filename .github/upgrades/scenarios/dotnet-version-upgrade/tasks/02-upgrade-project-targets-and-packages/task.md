# 02-upgrade-project-targets-and-packages: Upgrade project TFMs, packages, and API compatibility

Upgrade all project files from the current mixed target frameworks to the requested modern set. For projects currently targeting `net462;netstandard2.0`, remove .NET Framework and move to `netstandard2.1;net8.0;net10.0` where applicable. For projects currently targeting `net6.0;net7.0`, move to `net8.0;net10.0`. Apply package updates recommended by assessment, including vulnerability remediation, and remove packages now covered by framework references.

This task includes source-level compatibility fixes identified by assessment (`Api.0002`) in Npgsql-related projects, and keeps package management per-project (no CPM during active migration). Because this spans many projects and dependency levels, execution can be decomposed into dependency-tier subtasks while preserving overall task ownership.

**Done when**: All project TFMs match the requested target set, .NET Framework targets are removed, required package updates/removals are applied (including vulnerable package upgrade), API compatibility issues are resolved inline, and the full solution builds/tests successfully.
