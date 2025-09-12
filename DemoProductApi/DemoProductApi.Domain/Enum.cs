namespace DemoProductApi.Domain;
public enum Status { Active = 1, Inactive = 2, Discontinued = 3 }
public enum AttributeScope { Product = 1, Item = 2 }
public enum AttributeDataType { Text = 1, Number = 2, Bool = 3, Date = 4, Json = 5 }
public enum PriceEntityType { Product = 1, Item = 2, Bundle = 3 }
public enum BundleChildType { Item = 1, Bundle = 2 }
public enum BundlePricingRuleType { Fixed = 1, PercentOff = 2, SumParts = 3 }
public enum ApplyToScope { All = 1, RequiredOnly = 2, SpecificChildren = 3 }
