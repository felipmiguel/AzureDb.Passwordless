# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade all projects to the requested modern target set by replacing `netstandard2.0` with `netstandard2.1`, removing `.NET Framework` targets, and moving modern targets to `net8.0` and `net10.0`.
**Scope**: 13 SDK-style projects with a 3-level dependency graph and mixed current TFMs (`net462`, `netstandard2.0`, `net6.0`, `net7.0`).

### Selected Strategy
**Bottom-Up (Dependency-First)** — Upgrade from dependency-leaf projects upward through dependent tiers.
**Rationale**: The solution includes multiple .NET Framework-targeted projects and cross-project dependencies, so tier-aware execution reduces breakage risk.

Dependency graph (assessment-derived):
- Tier 1: Batec.Azure.Data.Extensions.Common, Batec.Azure.Data.Extensions.MySqlConnector, Batec.Azure.Data.Extensions.Npgsql, Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore, Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore, Batec.Core.TestFramework, Sample.Repository
- Tier 2: Batec.Azure.Data.Extensions.Common.Tests, Batec.Azure.Data.Extensions.MySqlConnector.Tests, Batec.Azure.Data.Extensions.Npgsql.Tests, Sample.Repository.Tests
- Tier 3: Batec.Azure.Data.Extensions.Npgsql.EntityFrameworkCore.Tests, Batec.Azure.Data.Extensions.Pomelo.EntityFrameworkCore.Tests

## Tasks

### 01-validate-upgrade-prerequisites: Validate SDK/tooling prerequisites

Validate that the local SDK/toolchain can build the solution for `net8.0` and `net10.0`, and confirm there are no workflow blockers before applying project-wide TFM updates. This task establishes the baseline needed for stable execution and prevents partial migration failures caused by environment mismatch.

The task also confirms strategy constraints are actionable (in-place migration, per-project package management during migration, inline API fixes) and prepares execution to proceed directly into project updates.

**Done when**: .NET SDK compatibility for target frameworks is confirmed, restore/build prerequisites are validated, and no environment blockers remain for upgrade execution.

---

### 02-upgrade-project-targets-and-packages: Upgrade project TFMs, packages, and API compatibility

Upgrade all project files from the current mixed target frameworks to the requested modern set. For projects currently targeting `net462;netstandard2.0`, remove .NET Framework and move to `netstandard2.1;net8.0;net10.0` where applicable. For projects currently targeting `net6.0;net7.0`, move to `net8.0;net10.0`. Apply package updates recommended by assessment, including vulnerability remediation, and remove packages now covered by framework references.

This task includes source-level compatibility fixes identified by assessment (`Api.0002`) in Npgsql-related projects, and keeps package management per-project (no CPM during active migration). Because this spans many projects and dependency levels, execution can be decomposed into dependency-tier subtasks while preserving overall task ownership.

**Done when**: All project TFMs match the requested target set, .NET Framework targets are removed, required package updates/removals are applied (including vulnerable package upgrade), API compatibility issues are resolved inline, and the full solution builds/tests successfully.

---

### 03-final-validation-and-cleanup: Run full validation and capture post-migration cleanup

Run full-solution validation after upgrade completion, including restore, build, and tests across affected projects. Confirm no dependency conflicts remain and no build warnings persist in modified projects.

Capture any deferred post-migration recommendations in workflow artifacts, including central package management adoption once all projects are stabilized on modern targets.

**Done when**: Full solution restore/build/test passes, warnings are resolved in modified projects, and post-migration recommendations are documented.
