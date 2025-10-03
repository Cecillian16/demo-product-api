Playwright E2E tests

How to run locally:

Prerequisites:
- API running at http://localhost:5000 (or set E2E_BASE_URL env)
- PostgreSQL available and migrated/seeded (CI uses a container)
- Node dependencies installed at repo root: `npm ci`
- Playwright browsers installed: `npx playwright install --with-deps`

Run tests:

From repo root:

```bash
npm run e2e
```

Or run directly in the e2e folder:

```bash
cd tests/e2e/playwright
npx playwright test
```

Notes:
- Tests create resources (products, items, bundles, etc.) and do not currently clean up after themselves.
- To run against a different URL, set E2E_BASE_URL, e.g.: `E2E_BASE_URL=http://localhost:5001 npm run e2e`
