This folder contains YAML test case specifications used by the QA team and implemented by Playwright tests.

Files:
- tc-api-001-product.yml - Product create/retrieve
- tc-api-002-productitem.yml - ProductItem create/retrieve
- tc-api-003-bundle.yml - Bundle create/retrieve
- tc-api-004-inventory.yml - Inventory create/retrieve
- tc-api-005-price.yml - Price create/retrieve
- tc-api-006-location.yml - Location create/retrieve

Note: The Playwright E2E test `tests/e2e/playwright/tests/api.e2e.spec.ts` implements the happy-path flow covering these cases in sequence.
