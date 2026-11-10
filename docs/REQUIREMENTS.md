# Product Requirements — Draft

## File comparison
- Open two files in read-only side-by-side panes.
- Highlight added, deleted, and modified lines and characters.
- Navigate differences; optionally synchronize scrolling.
- Detect moved blocks in a later iteration.
- Support common text encodings and line-ending preservation.

## Configurability
- Ignore whitespace, blank lines, and case.
- Optional language-aware comment filtering and regex-based exclusions.
- Save profiles by language or project; allow session overrides.
- Show which rules are active and avoid altering source text when comparing.

## Merge
- Edit either pane; copy selected lines or change blocks.
- Undo/redo, save-as, backup options, external modification detection.
- Three-way base/local/incoming conflict handling.

## Folder comparison
- Recursive folder tree; filters and exclusions; content comparison.
- Open file pairs; preview selective synchronization.

## Interface
- Dark/light/custom themes, configurable highlights, fonts, shortcuts, layouts and panels.
- Keyboard accessibility and non-color difference indicators.

## Advanced
- C#, JSON, XML, SQL, JavaScript profiles.
- Structured JSON comparison, Git comparisons, diff exports, CLI mode.

## Acceptance criteria for v0.1
1. User can select two local text files without modifying either.
2. Equal files report zero differences.
3. Insertions, deletions, and modifications appear correctly in a side-by-side display.
4. Next/previous difference navigation is available.
5. Tests cover empty files, identical files, changed lines, Unicode, and mixed line endings.
6. The WPF UI remains responsive during comparisons and errors are reported clearly.

All requirements remain subject to design review before implementation.
