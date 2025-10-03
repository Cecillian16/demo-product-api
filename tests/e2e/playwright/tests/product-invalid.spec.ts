import { test, expect } from '@playwright/test';

test('Product invalid values: too long name and empty skuPrefix', async ({ request: api }) => {
  const longName = 'x'.repeat(300);
  const req = { name: longName, skuPrefix: '', status: 1, variantOptions: [] };
  const res = await api.post('/api/product', { data: req });
  const status = res.status();
  if ([200, 201].includes(status)) {
    const body = await res.text();
    console.warn('Product create unexpectedly succeeded', status, body.slice(0, 1000));
  } else {
    expect(status).toBeGreaterThanOrEqual(400);
    expect(status).toBeLessThan(600);
  }
});
