import { test, expect } from '@playwright/test';
import { makeCreatedIds, teardown } from './helpers';

test('ProductItem create/get/update/delete', async ({ request: api }) => {
  const ids = makeCreatedIds();
  try {
    // create product first
    const productReq = { name: `P ${Date.now()}`, skuPrefix: 'PX', status: 1, variantOptions: [] };
    const pc = await api.post('/api/product', { data: productReq });
    expect([200,201]).toContain(pc.status());
    const pbody = await pc.json();
    const productId = pbody.productId;
    ids.products.push(productId);

    const itemReq = { productId, sku: `ITEM-${Date.now()}`, status: 1, weight: 1.1, volume: 0.5, variantValues: [] };
    const ic = await api.post('/api/productitem', { data: itemReq });
    expect([200,201]).toContain(ic.status());
    const ibody = await ic.json();
    const itemId = ibody.productItemId;
    ids.productItems.push(itemId);

    const get = await api.get(`/api/productitem/${itemId}`);
    expect(get.status()).toBe(200);

    const upd = await api.put(`/api/productitem/${itemId}`, { data: itemReq });
    expect([200,204]).toContain(upd.status());

    const del = await api.delete(`/api/productitem/${itemId}`);
    expect([200,204]).toContain(del.status());
    ids.productItems = ids.productItems.filter(x => x !== itemId);
  } finally {
    await teardown(api, ids);
  }
});
