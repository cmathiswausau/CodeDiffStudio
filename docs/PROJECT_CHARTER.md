# CodeDiff Studio — Project Charter

## Vision
Build a configurable Windows desktop source-code comparison and merge tool, suitable for developers comparing files, directories, and Git versions.

## Goals
- Compare source files with accurate line and character differences.
- Allow configurable comparison rules and named profiles.
- Provide a customizable WPF editor interface.
- Support safe editing, two-way and three-way merging.
- Compare folder trees and eventually integrate local Git repositories.
- Be fully testable on a single Windows development computer.

## Platform
C#, .NET, WPF, Visual Studio 2026. Architecture separates UI, comparison engine, filesystem, configuration, integrations, and tests.

## Nonfunctional requirements
- Comparisons must never modify source files.
- Merging requires explicit user action and saving.
- Preserve original text, encoding and line endings when possible.
- Long-running comparisons are cancellable and do not freeze the UI.
- Automated tests cover diff correctness, ignored differences, and merge safety.
- Follow StyleCop conventions and document design decisions.

## Scope
### Version 0.1
Solution architecture; two-file loading; side-by-side read-only comparison; diff navigation; line and character highlighting; unit tests.

### Later releases
0.2 comparison profiles and themes; 0.3 editing and two-way merge; 0.4 folder comparison; 0.5 three-way merge; 0.6 Git and language-aware comparisons; 1.0 quality and release readiness.

## Open design decisions
- Final UI layout (Visual Studio-like, streamlined, or hybrid).
- Initial diff algorithm and fallback strategy.
- Editor component evaluation and licensing.
- Settings schema and rule precedence.
- Handling large files and external modifications.

## Repository visibility
Initially public; owner may make private later. Never commit secrets, credentials, or private data. Public clones may persist after visibility changes.
