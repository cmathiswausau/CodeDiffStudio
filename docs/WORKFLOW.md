# Development Workflow

## Issue hierarchy
Epics describe major systems. Features and user stories are tracked as issues; implementation tasks may use issue sub-issues or task lists. Bugs are separate issues.

## Status
Backlog -> Ready -> In Progress -> In Review -> Done. Blocked is a separate label or field.

## Branches
- main: stable integration branch
- feature/issue-N-short-description
- fix/issue-N-short-description
- docs/issue-N-short-description

## Definition of ready
Clear objective, acceptance criteria, dependencies, and test approach.

## Definition of done
Acceptance criteria met; relevant tests added and passing; StyleCop checks addressed; docs updated; pull request reviewed and merged.

## Pull requests
Reference the issue, explain changes and test evidence, and avoid unrelated modifications.

## CI plan
On push and pull request: restore, build, run tests and static analysis. Add Windows packaging in later releases.

## Project fields to configure
Status, Priority (P0-P3), Type (Epic/Feature/Story/Task/Bug), Target Release, Estimate, and Area.
