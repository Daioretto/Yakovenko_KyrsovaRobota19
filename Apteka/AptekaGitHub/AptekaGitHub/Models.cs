namespace Apteka;

public class Pharmacy
{
    public int Key { get; set; }
    public string Name { get; set; } = "";
    public DateTime CurrentDate { get; set; } = DateTime.Today;

    public Pharmacy() { }
    public Pharmacy(int key, string name, DateTime currentDate)
    {
        Key = key;
        Name = name;
        CurrentDate = currentDate;
    }
}

public class Product
{
    public int Key { get; set; }
    public string Name { get; set; } = "";
    public string[] Packages { get; set; } = Array.Empty<string>();
    public decimal Price { get; set; }

    public Product() { }
    public Product(int key, string name, string[] packages, decimal price)
    {
        Key = key;
        Name = name;
        Packages = packages;
        Price = price;
    }
}

public class Supplier
{
    public int Key { get; set; }
    public string Name { get; set; } = "";

    public Supplier() { }
    public Supplier(int key, string name)
    {
        Key = key;
        Name = name;
    }
}

// Запис про товар, який знаходиться в конкретній аптеці.
public class PharmacyStock
{
    public int Key { get; set; }
    public int PharmacyKey { get; set; }
    public int ProductKey { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }

    public PharmacyStock() { }
    public PharmacyStock(int key, int pharmacyKey, int productKey, int quantity, DateTime expiryDate)
    {
        Key = key;
        PharmacyKey = pharmacyKey;
        ProductKey = productKey;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

// Пропозиція постачальника: кількість товару та перевага від 1 до 3.
public class SupplierOffer
{
    public int Key { get; set; }
    public int SupplierKey { get; set; }
    public int ProductKey { get; set; }
    public int Quantity { get; set; }
    public int Priority { get; set; }

    public SupplierOffer() { }
    public SupplierOffer(int key, int supplierKey, int productKey, int quantity, int priority)
    {
        Key = key;
        SupplierKey = supplierKey;
        ProductKey = productKey;
        Quantity = quantity;
        Priority = priority;
    }
}
