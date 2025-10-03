import { test, expect } from '@playwright/test';
import { makeCreatedIds, teardown } from './helpers';

test('Bundle bulk create and cleanup', async ({ request: api }) => {
  const ids = makeCreatedIds();
  try {
    // prepare a product + item
    const pc = await api.post('/api/product', { data: { name: `BulkP ${Date.now()}`, skuPrefix: 'BP', status: 1, variantOptions: [] } });
    expect([200,201]).toContain(pc.status());
    const productId = (await pc.json()).productId;
    ids.products.push(productId);

    const ic = await api.post('/api/productitem', { data: { productId, sku: `BI-${Date.now()}`, status: 1, variantValues: [] } });
    expect([200,201]).toContain(ic.status());
    const productItemId = (await ic.json()).productItemId;
    ids.productItems.push(productItemId);

    const bulkReq = [
      { name: `Bulk1 ${Date.now()}`, description: 'b1', status: 1, items: [{ childProductItemId: productItemId, quantity: 1 }] },
      { name: `Bulk2 ${Date.now()}`, description: 'b2', status: 1, items: [{ childProductItemId: productItemId, quantity: 2 }] }
    ];

    const res = await api.post('/api/bundle/bulk', { data: bulkReq });
    expect(res.status()).toBe(200);
    const arr = await res.json();
    expect(Array.isArray(arr)).toBeTruthy();
    for (const b of arr) ids.bundles.push(b.bundleId || b.id);
  } finally {
    await teardown(api, ids);
  }
});
