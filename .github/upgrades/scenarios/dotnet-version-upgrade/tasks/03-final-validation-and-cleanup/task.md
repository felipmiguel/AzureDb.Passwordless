# 03-final-validation-and-cleanup: Run full validation and capture post-migration cleanup

Run full-solution validation after upgrade completion, including restore, build, and tests across affected projects. Confirm no dependency conflicts remain and no build warnings persist in modified projects.

Capture any deferred post-migration recommendations in workflow artifacts, including central package management adoption once all projects are stabilized on modern targets.

## Scope Inventory
- Affected projects: all test projects that depend on DB environment variables through `TestEnvironment` (`Npgsql`, `Npgsql.EntityFrameworkCore`, `MySqlConnector`, `Pomelo.EntityFrameworkCore` test suites).
- Distinct concerns:
  - Integration test gating when DB env vars are missing
  - Preserve normal execution when env vars are present
  - Keep final restore/build validation green

## Research Findings
- DB integration test environment classes resolve connection settings through `Batec.Core.TestFramework.TestEnvironment.GetVariable(...)`.
- Missing variables currently propagate as null values and cause runtime failures instead of deterministic skip behavior.
- Common required variables are `POSTGRESQL_FQDN`, `POSTGRESQL_DATABASE`, `POSTGRESQL_SERVER_ADMIN`, `MYSQL_FQDN`, `MYSQL_DATABASE`, and `MYSQL_SERVER_ADMIN`.
- Centralized skip behavior can be implemented once in `GetVariable` so all integration tests consistently skip when required env vars are not set.

## Execution Plan
1. Update `TestEnvironment.GetVariable` to skip tests via NUnit when required variables are missing/empty.
2. Run full restore/build to ensure no regressions.
3. Run tests and confirm suites are skipped rather than failing when env vars are unavailable.
4. Document final post-migration recommendations.

**Done when**: Full solution restore/build/test passes or explicitly skips environment-dependent integration tests when DB env vars are missing, warnings are resolved in modified projects, and post-migration recommendations are documented.
