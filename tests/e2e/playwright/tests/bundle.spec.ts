import { test, expect } from '@playwright/test';
import { makeCreatedIds, teardown } from './helpers';

test('Bundle create/get/delete', async ({ request: api }) => {
  const ids = makeCreatedIds();
  try {
    // create product + item
    const productReq = { name: `BProd ${Date.now()}`, skuPrefix: 'BP', status: 1, variantOptions: [] };
    const pc = await api.post('/api/product', { data: productReq });
    expect([200,201]).toContain(pc.status());
    const productId = (await pc.json()).productId;
    ids.products.push(productId);

    const itemReq = { productId, sku: `BITEM-${Date.now()}`, status: 1, variantValues: [] };
    const ic = await api.post('/api/productitem', { data: itemReq });
    expect([200,201]).toContain(ic.status());
    const productItemId = (await ic.json()).productItemId;
    ids.productItems.push(productItemId);

    const bundleReq = { name: `Bundle ${Date.now()}`, description: 'E2E', status: 1, items: [{ childProductItemId: productItemId, quantity: 1 }] };
    const bc = await api.post('/api/bundle', { data: bundleReq });
    expect([200,201]).toContain(bc.status());
    const bundleId = (await bc.json()).bundleId;
    ids.bundles.push(bundleId);

    const get = await api.get(`/api/bundle/${bundleId}`);
    expect(get.status()).toBe(200);

    const del = await api.delete(`/api/bundle/${bundleId}`);
    expect([200,204]).toContain(del.status());
    ids.bundles = ids.bundles.filter(x => x !== bundleId);
  } finally {
    await teardown(api, ids);
  }
});
