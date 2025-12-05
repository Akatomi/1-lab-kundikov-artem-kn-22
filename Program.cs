using System;
using System.Collections.Generic;

class Program
{
    class Product
    {
        public int Code;
        public string Name;
        public string Material;
        public string[] Colors;
        public string[] Sizes;
        public int[] Stock;
        public double Price;
        public string Season;
    }

    class CartItem
    {
        public Product Product;
        public string Size;
        public string Color;
        public int Quantity;
        public int TicketCode;
    }

    static List<Product> products = new List<Product>();
    static List<CartItem> cart = new List<CartItem>();
    static Random rnd = new Random();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        InitializeProducts();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Онлайн магазин одягу Contre Le Destin ===");
            Console.WriteLine("\n1. Каталог одягу");
            Console.WriteLine("2. Пошук за кодом товару");
            Console.WriteLine("3. Переглянути корзину");
            Console.WriteLine("4. Вихід");
            Console.Write("Оберіть дію: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": Catalog(); break;
                case "2": SearchByCode(); break;
                case "3": ViewCart(); break;
                case "4": return;
                default: Console.WriteLine("Невірна дія!"); Pause(); break;
            }
        }
    }

    static void Catalog()
    {
        Console.Clear();
        Console.WriteLine("=== Каталог одягу ===");
        Console.WriteLine("Оберіть сезон: 1-Зима, 2-Літо, 3-Осінь, 4-Весна");
        Console.Write("Введіть номер сезону: ");
        string seasonChoice = Console.ReadLine();
        string season = seasonChoice switch
        {
            "1" => "Зима",
            "2" => "Літо",
            "3" => "Осінь",
            "4" => "Весна",
            _ => null
        };
        if (season == null) { Console.WriteLine("Невірний вибір!"); Pause(); return; }

        
        Console.ForegroundColor = season switch
        {
            "Зима" => ConsoleColor.Blue,
            "Літо" => ConsoleColor.Red,
            "Осінь" => ConsoleColor.Yellow,
            "Весна" => ConsoleColor.DarkYellow,
            _ => ConsoleColor.White
        };

        var seasonProducts = products.FindAll(p => p.Season == season);
        if (seasonProducts.Count == 0) { Console.WriteLine("Немає товарів на цей сезон."); Pause(); return; }

        foreach (var p in seasonProducts)
        {
            Console.WriteLine($"Код: {p.Code} | {p.Name} | Ціна: {p.Price} грн | Наявність: {p.Stock[0]}-{p.Stock[^1]} шт");
        }
        Console.ResetColor();

        BuyProductPrompt(seasonProducts);
    }

    static void BuyProductPrompt(List<Product> availableProducts)
    {
        while (true)
        {
            Console.Write("Введіть код товару для покупки (або Enter для повернення): ");
            string input = Console.ReadLine();
            if (input == "") return;

            if (!int.TryParse(input, out int code))
            {
                Console.WriteLine("Невірний код!");
                continue;
            }

            Product product = availableProducts.Find(p => p.Code == code);
            if (product == null)
            {
                Console.WriteLine("Товар не знайдено!");
                continue;
            }

            Console.WriteLine("Доступні кольори:");
            for (int i = 0; i < product.Colors.Length; i++) Console.WriteLine($"{i + 1}. {product.Colors[i]}");
            Console.Write("Оберіть колір (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int colorIndex) || colorIndex < 1 || colorIndex > product.Colors.Length) { Console.WriteLine("Невірний колір!"); continue; }
            colorIndex -= 1;

            Console.WriteLine("Доступні розміри:");
            for (int i = 0; i < product.Sizes.Length; i++) Console.WriteLine($"{i + 1}. {product.Sizes[i]}");
            Console.Write("Оберіть розмір (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int sizeIndex) || sizeIndex < 1 || sizeIndex > product.Sizes.Length) { Console.WriteLine("Невірний розмір!"); continue; }
            sizeIndex -= 1;

            Console.Write($"Введіть кількість (макс. {Math.Min(5, product.Stock[sizeIndex])}): ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 1 || qty > 5 || qty > product.Stock[sizeIndex]) { Console.WriteLine("Неможливо купити таку кількість!"); continue; }

            cart.Add(new CartItem { Product = product, Color = product.Colors[colorIndex], Size = product.Sizes[sizeIndex], Quantity = qty, TicketCode = rnd.Next(1000, 9999) });
            product.Stock[sizeIndex] -= qty;
            Console.WriteLine("Товар додано до корзини!");
            Pause();
            return;
        }
    }

    static void SearchByCode()
    {
        while (true)
        {
            Console.Write("Введіть код товару для пошуку (Enter для виходу): ");
            string input = Console.ReadLine();
            if (input == "") return;
            if (!int.TryParse(input, out int code)) { Console.WriteLine("Невірний код!"); continue; }
            Product product = products.Find(p => p.Code == code);
            if (product == null) { Console.WriteLine("Товар не знайдено!"); continue; }
            Console.WriteLine($"Знайдено: {product.Name} | Ціна: {product.Price} грн");
            BuyProductPrompt(new List<Product> { product });
            return;
        }
    }

    static void ViewCart()
    {
        Console.Clear();
        if (cart.Count == 0) { Console.WriteLine("Корзина порожня."); Pause(); return; }

        double total = 0;
        Console.WriteLine("=== Вміст корзини ===");
        foreach (var item in cart)
        {
            double itemTotal = item.Quantity * item.Product.Price;
            total += itemTotal;
            Console.WriteLine($"{item.Product.Name} | Розмір: {item.Size} | Колір: {item.Color} | Кількість: {item.Quantity} | Сума: {itemTotal} грн | Код: {item.TicketCode}");
        }
        Console.WriteLine($"Загальна сума: {total} грн");

        Console.WriteLine("\n1. Оформити замовлення");
        Console.WriteLine("2. Очистити корзину");
        Console.WriteLine("Enter - Повернутися в головне меню");
        Console.Write("Оберіть дію: ");
        string choice = Console.ReadLine();

        if (choice == "1") PlaceOrder(total);
        else if (choice == "2") { cart.Clear(); Console.WriteLine("Корзина очищена."); Pause(); }
    }

    static void PlaceOrder(double total)
    {
        Console.Write("Введіть ваше ім'я: ");
        string name = Console.ReadLine();
        Console.Write("Місто: ");
        string city = Console.ReadLine();
        Console.Write("Вулиця: ");
        string street = Console.ReadLine();
        Console.Write("Номер будинку: ");
        string house = Console.ReadLine();

        int orderNumber = rnd.Next(10000, 99999);
        Console.Clear();
        Console.WriteLine("=== Чек покупки ===");
        Console.WriteLine($"Замовлення №{orderNumber}");
        Console.WriteLine($"Покупець: {name}");
        Console.WriteLine($"Адреса: {city}, {street}, {house}\n");

        foreach (var item in cart)
        {
            double itemTotal = item.Quantity * item.Product.Price;
            Console.WriteLine($"{item.Product.Name} | Розмір: {item.Size} | Колір: {item.Color} | Кількість: {item.Quantity} | Сума: {itemTotal} грн");
        }
        Console.WriteLine($"\nЗагальна сума: {total} грн");
        cart.Clear();
        Pause();
    }

    static void Pause()
    {
        Console.WriteLine("Натисніть Enter для продовження...");
        Console.ReadLine();
    }

    static void InitializeProducts()
    {
        string[] sizes = { "S", "M", "L", "XL", "XXL" };
        string[] colorsMale = { "Чорний", "Синій", "Сірий", "Коричневий", "Зелений" };
        string[] colorsFemale = { "Червоний", "Рожевий", "Жовтий", "Білий", "Бежевий" };

       

        
        for (int i = 0; i < 5; i++)
        {
            products.Add(new Product { Code = 100 + i + 1, Name = $"Чоловіча зимова річ Contre Le Destin {i + 1}", Price = 1000 + i * 100, Season = "Зима", Colors = colorsMale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Шерсть" });
            products.Add(new Product { Code = 105 + i + 1, Name = $"Жіноча зимова річ Contre Le Destin {i + 1}", Price = 950 + i * 100, Season = "Зима", Colors = colorsFemale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Шерсть" });
        }

        
        for (int i = 0; i < 5; i++)
        {
            products.Add(new Product { Code = 200 + i + 1, Name = $"Чоловіча літня річ Contre Le Destin {i + 1}", Price = 500 + i * 50, Season = "Літо", Colors = colorsMale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Бавовна" });
            products.Add(new Product { Code = 205 + i + 1, Name = $"Жіноча літня річ Contre Le Destin {i + 1}", Price = 450 + i * 50, Season = "Літо", Colors = colorsFemale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Бавовна" });
        }

        
        for (int i = 0; i < 5; i++)
        {
            products.Add(new Product { Code = 300 + i + 1, Name = $"Чоловіча осіння річ Contre Le Destin {i + 1}", Price = 800 + i * 50, Season = "Осінь", Colors = colorsMale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Шерсть" });
            products.Add(new Product { Code = 305 + i + 1, Name = $"Жіноча осіння річ Contre Le Destin {i + 1}", Price = 750 + i * 50, Season = "Осінь", Colors = colorsFemale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Шифон" });
        }

       
        for (int i = 0; i < 5; i++)
        {
            products.Add(new Product { Code = 400 + i + 1, Name = $"Чоловіча весняна річ Contre Le Destin {i + 1}", Price = 900 + i * 50, Season = "Весна", Colors = colorsMale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Бавовна" });
            products.Add(new Product { Code = 405 + i + 1, Name = $"Жіноча весняна річ Contre Le Destin {i + 1}", Price = 850 + i * 50, Season = "Весна", Colors = colorsFemale, Sizes = sizes, Stock = new int[5] { 5, 5, 5, 5, 5 }, Material = "Бавовна" });
        }
    }
}

