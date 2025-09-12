# Features
- Product Variant CRUD
- Product Bundle CRUD
- Product Item CRUD

# Prerequisite
- .NET 8 SDK
- PostgreSQL
- EF Core Tool (migration)

# My Approach
- Clean (Layered) Architecture
- Repository Pattern
- EF Core + Dapper
- FluentValidation
- Dependency Injection
- Swagger Integration

# Database Diagram

```mermaid

erDiagram
  Product ||--o{ VariantOption : "has options"
  VariantOption ||--o{ VariantOptionValue : "has values"
  Product ||--o{ ProductItem : "has SKUs"
  ProductItem ||--o{ ProductItemVariantValue : "has option-value"
  VariantOption ||--o{ ProductItemVariantValue : "option"
  VariantOptionValue ||--o{ ProductItemVariantValue : "value"

  ProductItem ||--o{ Inventory : "stock"
  Location ||--o{ Inventory : "at location"

  Bundle ||--o{ BundleItem : "contains"
  BundleItem }o--|| ProductItem : "child item"
  BundleItem }o--|| Bundle : "child bundle"

  Price }o--|| Product : "price for product"
  Price }o--|| ProductItem : "price for SKU"

  Product {
    GUID product_id PK
    string name
    string sku_prefix
    int status
    GUID version_group_id
    date valid_from
    date valid_to
  }

  VariantOption {
    GUID variant_option_id PK
    GUID product_id FK
    string name
  }

  VariantOptionValue {
    GUID variant_option_value_id PK
    GUID variant_option_id FK
    string value
    string code
  }

  ProductItem {
    GUID product_item_id PK
    GUID product_id FK
    string sku
    string barcode
    int status
    decimal weight
    decimal volume
    date created_at
    date updated_at
  }

  ProductItemVariantValue {
    GUID product_item_id PK,FK
    GUID variant_option_id PK,FK
    GUID variant_option_value_id FK
  }

  Location {
    GUID location_id PK
    string name
    string type
    string address1
    string address2
    string city
    string country
  }

  Inventory {
    GUID inventory_id PK
    GUID product_item_id FK
    GUID location_id FK
    int on_hand
    int reserved
    int in_transit
    int reorder_point
    date updated_at
  }

  Price {
    GUID price_id PK
    string entity_type
    GUID entity_id
    string currency
    decimal list_price
    decimal sale_price
    date valid_from
    date valid_to
  }

  Bundle {
    GUID bundle_id PK
    string name
    string description
    int status
    date valid_from
    date valid_to
  }

  BundleItem {
    GUID id PK
    GUID bundle_id FK
    GUID child_id FK
    decimal quantity
  }

```