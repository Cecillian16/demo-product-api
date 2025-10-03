import { APIRequestContext } from '@playwright/test';

export type CreatedIds = {
  products: string[];
  productItems: string[];
  bundles: string[];
};

export function makeCreatedIds(): CreatedIds {
  return { products: [], productItems: [], bundles: [] };
}

export async function teardown(api: APIRequestContext, ids: CreatedIds) {
  // Delete bundles first (they reference items)
  for (const b of ids.bundles) {
    try { await api.delete(`/api/bundle/${b}`); } catch (e) { /* ignore */ }
  }
  for (const pi of ids.productItems) {
    try { await api.delete(`/api/productitem/${pi}`); } catch (e) { /* ignore */ }
  }
  for (const p of ids.products) {
    try { await api.delete(`/api/product/${p}`); } catch (e) { /* ignore */ }
  }
}
