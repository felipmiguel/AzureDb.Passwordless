# 02.02-package-updates-and-cleanup: Upgrade vulnerable/recommended packages and remove framework-included package references

# 02.02-package-updates-and-cleanup

## Objective
Address package-related upgrade findings by updating central package versions and removing package references now included in framework references.

## Scope
- Directory.Packages.props version updates
- Project files removing System.Threading.Tasks.Extensions and System.ValueTuple references where applicable
- Keep CPM in place (existing repository behavior)

## Done when
- Vulnerable package versions are upgraded
- Framework-included packages are removed from project references
- Restore completes without NU1902/NU1903 vulnerability errors
