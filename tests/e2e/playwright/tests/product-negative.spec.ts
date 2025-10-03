import { test, expect } from '@playwright/test';

test('Product negative: missing required fields', async ({ request: api }) => {
  // Missing name and skuPrefix
  const bad = { description: 'no name', status: 1 };
  const res = await api.post('/api/product', { data: bad });
  // Expect 400 Bad Request (validation)
  expect([400, 422]).toContain(res.status());
});
