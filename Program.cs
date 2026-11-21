using System.Globalization;

namespace Apteka;

internal class Program
{
    private static readonly string DataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
    private static readonly string[] Paths =
    {
        Path.Combine(DataFolder, "pharmacies.txt"),
        Path.Combine(DataFolder, "products.txt"),
        Path.Combine(DataFolder, "suppliers.txt"),
        Path.Combine(DataFolder, "pharmacy_stock.txt"),
        Path.Combine(DataFolder, "supplier_offers.txt")
    };

    private static List<Pharmacy> pharmacies = new();
    private static List<Product> products = new();
    private static List<Supplier> suppliers = new();
    private static List<PharmacyStock> pharmacyStock = new();
    private static List<SupplierOffer> supplierOffers = new();

    private static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
        Directory.CreateDirectory(DataFolder);
        CreateSampleFiles();
        LoadData();

        while (true)
        {
            ShowMenu();
            Console.Write("Оберіть пункт меню: ");
            string choice = Console.ReadLine() ?? "";
            Console.Clear();

            switch (choice)
            {
                case "1": ShowPharmacyProducts(); break;
                case "2": ShowExpiredProducts(); break;
                case "3": ShowAvailableFromSuppliers(); break;
                case "4": ShowPharmacyTotal(); break;
                case "5": FindProductAtSuppliers(); break;
                case "6": FindSupplierByPackage(); break;
                case "7": AddProductToPharmacy(); break;
                case "8": RemoveProductFromPharmacy(); break;
                case "9": ShowAllData(); break;
                case "0": SaveData(); Console.WriteLine("Дані збережено. Роботу завершено."); return;
                default: Console.WriteLine("Такого пункту меню немає."); break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення до меню...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("============================================");
        Console.WriteLine("        ІНФОРМАЦІЙНА СИСТЕМА «АПТЕКА»");
        Console.WriteLine("============================================");
        Console.WriteLine("1 – Товари та їх кількість у кожній аптеці");
        Console.WriteLine("2 – Товари, термін придатності яких закінчився");
        Console.WriteLine("3 – Товари, які можна замовити у постачальників");
        Console.WriteLine("4 – Сумарна вартість товарів в аптеці");
        Console.WriteLine("5 – Постачальники та кількість товару за назвою");
        Console.WriteLine("6 – Постачальники товару із заданим фасуванням");
        Console.WriteLine("7 – Додати товар у задану аптеку");
        Console.WriteLine("8 – Видалити товар з аптеки");
        Console.WriteLine("9 – Переглянути всі довідники");
        Console.WriteLine("0 – Зберегти дані та вийти");
        Console.WriteLine("--------------------------------------------");
    }

    private static void CreateSampleFiles()
    {
        if (Paths.Any(File.Exists)) return;

        File.WriteAllLines(Paths[0], new[]
        {
            "1;Аптека Здорова родина;2026-11-21",
            "2;Аптека Біля дому;2026-11-21"
        });
        File.WriteAllLines(Paths[1], new[]
        {
            "1;Парацетамол;500 мг|1000 мг;45,50",
            "2;Ібупрофен;200 мг|400 мг;62,00",
            "3;Вітамін C;500 мг|1000 мг;89,90",
            "4;Сольовий розчин;0,9%;38,00"
        });
        File.WriteAllLines(Paths[2], new[]
        {
            "1;ФармаПостач",
            "2;МедСервіс",
            "3;Здоров'я-Трейд"
        });
        File.WriteAllLines(Paths[3], new[]
        {
            "1;1;1;25;2027-06-30",
            "2;1;2;12;2026-10-01",
            "3;1;3;18;2027-03-15",
            "4;2;1;10;2027-06-30",
            "5;2;4;30;2028-01-31"
        });
        File.WriteAllLines(Paths[4], new[]
        {
            "1;1;1;100;1",
            "2;2;1;80;2",
            "3;3;2;60;1",
            "4;1;3;40;3",
            "5;2;3;75;1",
            "6;3;4;120;2",
            "7;1;2;50;2"
        });
    }

    private static void LoadData()
    {
        pharmacies = File.ReadAllLines(Paths[0]).Where(x => !string.IsNullOrWhiteSpace(x)).Select(line =>
        {
            string[] p = line.Split(';');
            return new Pharmacy(int.Parse(p[0]), p[1], DateTime.Parse(p[2]));
        }).ToList();

        products = File.ReadAllLines(Paths[1]).Where(x => !string.IsNullOrWhiteSpace(x)).Select(line =>
        {
            string[] p = line.Split(';');
            return new Product(int.Parse(p[0]), p[1], p[2].Split('|'), decimal.Parse(p[3].Replace(',', '.'), CultureInfo.InvariantCulture));
        }).ToList();

        suppliers = File.ReadAllLines(Paths[2]).Where(x => !string.IsNullOrWhiteSpace(x)).Select(line =>
        {
            string[] p = line.Split(';');
            return new Supplier(int.Parse(p[0]), p[1]);
        }).ToList();

        pharmacyStock = File.ReadAllLines(Paths[3]).Where(x => !string.IsNullOrWhiteSpace(x)).Select(line =>
        {
            string[] p = line.Split(';');
            return new PharmacyStock(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]), DateTime.Parse(p[4]));
        }).ToList();

        supplierOffers = File.ReadAllLines(Paths[4]).Where(x => !string.IsNullOrWhiteSpace(x)).Select(line =>
        {
            string[] p = line.Split(';');
            return new SupplierOffer(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]), int.Parse(p[4]));
        }).ToList();
    }

    private static void SaveData()
    {
        File.WriteAllLines(Paths[0], pharmacies.Select(x => $"{x.Key};{x.Name};{x.CurrentDate:yyyy-MM-dd}"));
        File.WriteAllLines(Paths[1], products.Select(x => $"{x.Key};{x.Name};{string.Join('|', x.Packages)};{x.Price.ToString("0.00", CultureInfo.GetCultureInfo("uk-UA"))}"));
        File.WriteAllLines(Paths[2], suppliers.Select(x => $"{x.Key};{x.Name}"));
        File.WriteAllLines(Paths[3], pharmacyStock.Select(x => $"{x.Key};{x.PharmacyKey};{x.ProductKey};{x.Quantity};{x.ExpiryDate:yyyy-MM-dd}"));
        File.WriteAllLines(Paths[4], supplierOffers.Select(x => $"{x.Key};{x.SupplierKey};{x.ProductKey};{x.Quantity};{x.Priority}"));
    }

    private static void ShowPharmacyProducts()
    {
        var query = from stock in pharmacyStock
                    join pharmacy in pharmacies on stock.PharmacyKey equals pharmacy.Key
                    join product in products on stock.ProductKey equals product.Key
                    orderby pharmacy.Name, product.Name
                    select new { pharmacy.Name, Product = product.Name, stock.Quantity, stock.ExpiryDate };

        Console.WriteLine("Аптека\t\tТовар\t\tКількість\tТермін придатності");
        foreach (var item in query)
            Console.WriteLine($"{item.Name}\t{item.Product}\t\t{item.Quantity}\t\t{item.ExpiryDate:dd.MM.yyyy}");
        if (!query.Any()) Console.WriteLine("Записів не знайдено.");
    }

    private static void ShowExpiredProducts()
    {
        var query = from stock in pharmacyStock
                    join pharmacy in pharmacies on stock.PharmacyKey equals pharmacy.Key
                    join product in products on stock.ProductKey equals product.Key
                    where stock.ExpiryDate.Date < pharmacy.CurrentDate.Date
                    orderby stock.ExpiryDate
                    select new { Pharmacy = pharmacy.Name, Product = product.Name, stock.Quantity, stock.ExpiryDate };

        Console.WriteLine("Аптека\t\tТовар\t\tКількість\tТермін придатності");
        foreach (var item in query)
            Console.WriteLine($"{item.Pharmacy}\t{item.Product}\t\t{item.Quantity}\t\t{item.ExpiryDate:dd.MM.yyyy}");
        if (!query.Any()) Console.WriteLine("Прострочених товарів не знайдено.");
    }

    private static void ShowAvailableFromSuppliers()
    {
        var query = from offer in supplierOffers
                    join supplier in suppliers on offer.SupplierKey equals supplier.Key
                    join product in products on offer.ProductKey equals product.Key
                    where offer.Quantity > 0
                    orderby offer.Priority, product.Name
                    select new { supplier.Name, Product = product.Name, offer.Quantity, offer.Priority };

        Console.WriteLine("Постачальник\tТовар\t\tДоступно\tПеревага");
        foreach (var item in query)
            Console.WriteLine($"{item.Name}\t{item.Product}\t\t{item.Quantity}\t\t{item.Priority}");
        if (!query.Any()) Console.WriteLine("Немає товарів для замовлення.");
    }

    private static void ShowPharmacyTotal()
    {
        ListPharmacies();
        int pharmacyKey = ReadInt("Введіть ключ аптеки: ");
        var query = from stock in pharmacyStock
                    join product in products on stock.ProductKey equals product.Key
                    where stock.PharmacyKey == pharmacyKey
                    select new { product.Name, product.Price, stock.Quantity, Sum = product.Price * stock.Quantity };

        if (!pharmacies.Any(x => x.Key == pharmacyKey))
        {
            Console.WriteLine("Аптеку не знайдено.");
            return;
        }
        foreach (var item in query)
            Console.WriteLine($"{item.Name}: {item.Quantity} шт. × {item.Price:C} = {item.Sum:C}");
        Console.WriteLine($"Загальна вартість товарів: {query.Sum(x => x.Sum):C}");
    }

    private static void FindProductAtSuppliers()
    {
        Console.Write("Введіть назву товару: ");
        string name = Console.ReadLine() ?? "";
        var query = from offer in supplierOffers
                    join supplier in suppliers on offer.SupplierKey equals supplier.Key
                    join product in products on offer.ProductKey equals product.Key
                    where product.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                    orderby offer.Quantity descending
                    select new { supplier.Name, Product = product.Name, offer.Quantity, offer.Priority };

        foreach (var item in query)
            Console.WriteLine($"{item.Name}: {item.Product}, кількість {item.Quantity}, перевага {item.Priority}");
        if (!query.Any()) Console.WriteLine("Товар у постачальників не знайдено.");
    }

    private static void FindSupplierByPackage()
    {
        Console.Write("Введіть фасування (наприклад, 500 мг): ");
        string package = Console.ReadLine() ?? "";
        var query = from offer in supplierOffers
                    join supplier in suppliers on offer.SupplierKey equals supplier.Key
                    join product in products on offer.ProductKey equals product.Key
                    where product.Packages.Any(p => p.Equals(package, StringComparison.OrdinalIgnoreCase))
                          && offer.Quantity > 0
                    orderby offer.Priority
                    select new { supplier.Name, Product = product.Name, offer.Quantity, offer.Priority };

        foreach (var item in query)
            Console.WriteLine($"{item.Name}: {item.Product}, {item.Quantity} шт., перевага {item.Priority}");
        if (!query.Any()) Console.WriteLine("Постачальників із таким фасуванням не знайдено.");
    }

    private static void AddProductToPharmacy()
    {
        ListPharmacies();
        int pharmacyKey = ReadInt("Ключ аптеки: ");
        if (!pharmacies.Any(x => x.Key == pharmacyKey))
        {
            Console.WriteLine("Аптеку не знайдено.");
            return;
        }
        ListProducts();
        int productKey = ReadInt("Ключ товару: ");
        if (!products.Any(x => x.Key == productKey))
        {
            Console.WriteLine("Товар не знайдено.");
            return;
        }
        int quantity = ReadInt("Кількість: ");
        if (quantity <= 0)
        {
            Console.WriteLine("Кількість повинна бути більшою за нуль.");
            return;
        }
        DateTime expiry = ReadDate("Термін придатності (РРРР-ММ-ДД): ");
        var existing = pharmacyStock.FirstOrDefault(x => x.PharmacyKey == pharmacyKey && x.ProductKey == productKey && x.ExpiryDate == expiry);
        if (existing != null) existing.Quantity += quantity;
        else pharmacyStock.Add(new PharmacyStock(NextKey(pharmacyStock.Select(x => x.Key)), pharmacyKey, productKey, quantity, expiry));
        SaveData();
        Console.WriteLine("Товар додано до аптеки.");
    }

    private static void RemoveProductFromPharmacy()
    {
        ListPharmacies();
        int pharmacyKey = ReadInt("Ключ аптеки: ");
        var rows = from stock in pharmacyStock
                   join product in products on stock.ProductKey equals product.Key
                   where stock.PharmacyKey == pharmacyKey
                   select new { stock, ProductName = product.Name };
        foreach (var row in rows)
            Console.WriteLine($"Запис №{row.stock.Key}: {row.ProductName}, кількість {row.stock.Quantity}");
        int stockKey = ReadInt("Введіть номер запису, який потрібно видалити: ");
        var selected = pharmacyStock.FirstOrDefault(x => x.Key == stockKey && x.PharmacyKey == pharmacyKey);
        if (selected == null)
        {
            Console.WriteLine("Запис не знайдено.");
            return;
        }
        pharmacyStock.Remove(selected);
        SaveData();
        Console.WriteLine("Товар видалено з аптеки.");
    }

    private static void ShowAllData()
    {
        Console.WriteLine("АПТЕКИ");
        foreach (var x in pharmacies) Console.WriteLine($"{x.Key}. {x.Name} (дата: {x.CurrentDate:dd.MM.yyyy})");
        Console.WriteLine("\nТОВАРИ");
        foreach (var x in products) Console.WriteLine($"{x.Key}. {x.Name}; фасування: {string.Join(", ", x.Packages)}; ціна: {x.Price:C}");
        Console.WriteLine("\nПОСТАЧАЛЬНИКИ");
        foreach (var x in suppliers) Console.WriteLine($"{x.Key}. {x.Name}");
        Console.WriteLine("\nПРОПОЗИЦІЇ ПОСТАЧАЛЬНИКІВ");
        foreach (var x in supplierOffers)
        {
            string supplier = suppliers.FirstOrDefault(s => s.Key == x.SupplierKey)?.Name ?? "Невідомий постачальник";
            string product = products.FirstOrDefault(p => p.Key == x.ProductKey)?.Name ?? "Невідомий товар";
            Console.WriteLine($"{supplier} – {product}: {x.Quantity} шт., перевага {x.Priority}");
        }
    }

    private static void ListPharmacies()
    {
        foreach (var x in pharmacies) Console.WriteLine($"{x.Key}. {x.Name}");
    }

    private static void ListProducts()
    {
        foreach (var x in products) Console.WriteLine($"{x.Key}. {x.Name} ({x.Price:C})");
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value)) return value;
            Console.WriteLine("Потрібно ввести ціле число.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime value)) return value;
            Console.WriteLine("Неправильний формат дати.");
        }
    }

    private static int NextKey(IEnumerable<int> keys) => keys.DefaultIfEmpty(0).Max() + 1;
}
