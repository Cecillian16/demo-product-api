import { test, expect } from '@playwright/test';
import { makeCreatedIds, teardown } from './helpers';

test('Product create/get/update/delete', async ({ request: api }) => {
  const ids = makeCreatedIds();
  try {
    const productReq = {
      name: `Product E2E ${Date.now()}`,
      skuPrefix: `P${Date.now() % 10000}`,
      description: 'E2E product',
      status: 1,
      variantOptions: []
    };
    const created = await api.post('/api/product', { data: productReq });
    expect([200,201]).toContain(created.status());
    const body = await created.json();
    const id = body.productId;
    expect(id).toBeTruthy();
    ids.products.push(id);

    // GET
    const get = await api.get(`/api/product/${id}`);
    expect(get.status()).toBe(200);

    // UPDATE
    const updateReq = { ...productReq, name: productReq.name + ' updated' };
    const upd = await api.put(`/api/product/${id}`, { data: updateReq });
    expect([200,204]).toContain(upd.status());

    // DELETE
    const del = await api.delete(`/api/product/${id}`);
    expect([200,204]).toContain(del.status());
    // remove from tracked since deleted
    ids.products = ids.products.filter(x => x !== id);
  } finally {
    await teardown(api, ids);
  }
});
