# Development Workflow

## Issue hierarchy
Epics describe major systems. Features and user stories are tracked as issues; implementation tasks may use issue sub-issues or task lists. Bugs are separate issues.

## Status
Backlog -> Ready -> In Progress -> In Review -> Done. Blocked is a separate label or field.

## Branches
- main: stable, validated release branch
- test: validation branch; receives changes from dev
- dev: integration branch for ongoing development
- feature/issue-N-short-description
- fix/issue-N-short-description
- docs/issue-N-short-description

Create feature, fix, and docs branches from dev. Merge completed work through pull requests in this order: feature/fix/docs -> dev -> test -> main. Validate changes on test before promoting them to main.

At the start and end of each workday, check the active branch, uncommitted changes, and unpushed commits. Commit and push a reviewable checkpoint or remind the developer before ending the session.

## Definition of ready
Clear objective, acceptance criteria, dependencies, and test approach.

## Definition of done
Acceptance criteria met; relevant tests added and passing; StyleCop checks addressed; docs updated; pull request reviewed and merged.

## Pull requests
Reference the issue, explain changes and test evidence, and avoid unrelated modifications.

## CI plan
On pull requests targeting dev, test, and main: restore, build, run tests and static analysis. Also run after pushes to test and main to validate promotions. Superseded runs of the same workflow/ref are cancelled. Skip post-merge pushes to dev to reduce duplicate compute use. Add Windows packaging in later releases.

## Project fields to configure
Status, Priority (P0-P3), Type (Epic/Feature/Story/Task/Bug), Target Release, Estimate, and Area.

## Cost controls and review

Use only free features or existing paid-plan allowances. Do not enable new subscriptions, paid trials that renew, additional metered spending, larger paid runners, or paid AI reviewers. Keep account hard-stop budgets at $0 for additional spending; stop work if included limits block execution.

Use standard hosted Windows runners for WPF checks, read-only workflow permissions, and the 15-minute job timeout. The GitHub account billing page remains the source of truth for allowance and usage; GitHub Actions budgets do not govern external AI-provider charges or existing subscriptions.

Use the PR template's user and developer checklists for reviews. Record actual verification methods and known deferred issues. Reusable checklists do not require an AI agent or additional service. Do not assume an AI review is free merely because it appears on GitHub; existing automatic Codex reviews are controlled separately and should be checked before requesting extra AI reviews.
