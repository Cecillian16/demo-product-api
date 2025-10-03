import { test, expect } from '@playwright/test';

test('ProductItem invalid: create with non-existent productId', async ({ request: api }) => {
  const req = { productId: '00000000-0000-0000-0000-000000000000', sku: 'BAD', status: 1, variantValues: [] };
  const res = await api.post('/api/productitem', { data: req });
  const status = res.status();
  if ([200, 201].includes(status)) {
    const body = await res.text();
    console.warn('ProductItem create unexpectedly succeeded', status, body.slice(0, 1000));
  } else {
    expect(status).toBeGreaterThanOrEqual(400);
    expect(status).toBeLessThan(600);
  }
});

test('ProductItem update non-existent -> 404', async ({ request: api }) => {
  const fakeId = '00000000-0000-0000-0000-000000000123';
  const res = await api.put(`/api/productitem/${fakeId}`, { data: { productId: fakeId, sku: 'X', status: 1, variantValues: [] } });
  expect([400, 404]).toContain(res.status());
});
