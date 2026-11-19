## Change and scope

Describe the problem, resulting behavior, and related issues.

## Validation

List automated and manual checks with their actual results. Distinguish source review, debugger inspection, and hands-on UI testing. Note any important scenario that remains untested.

## Known follow-up work

Link deferred issues and describe what is outside this increment. Explain any remaining risk that affects this promotion.

## Review checklist

- [ ] Base branch follows feature/fix/docs -> dev -> test -> main.
- [ ] User perspective: file identity, cancellation, errors, recovery, and visible comparison state are coherent for the changed flows.
- [ ] Developer perspective: correctness, operation ordering, resource bounds, and relevant regression coverage have been reviewed.
- [ ] Required CI checks pass; incomplete manual acceptance checks are identified.
- [ ] No new paid services, higher spending limits, or automatic paid overage are introduced.
