namespace Домашка;

class Program
{
    static CoffeeMachine machine = new CoffeeMachine();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Добро пожаловать в Кофемашину!");

        while (true)
        {
            ShowMainMenu();
            string choice = ReadInputWithExit();

            if (choice == null)
                continue;

            ProcessMainMenuChoice(choice);
        }
    }
    
    static string ReadInputWithExit()
    {
        string input = Console.ReadLine();
        
        if (input?.ToLower() == "exit")
        {
            Console.WriteLine("️Возврат в главное меню...");
            return null;
        }
        
        return input;
    }

    static void ShowMainMenu()
    {
        Console.WriteLine("ГЛАВНОЕ МЕНЮ");
        Console.WriteLine("1. Приготовить кофе");
        Console.WriteLine("2. Пополнить запасы");
        Console.WriteLine("3. Показать остатки");
        Console.WriteLine("4. Сохранить конфигурацию");
        Console.WriteLine("5. Конец смены (отчёт)"); 
        Console.WriteLine("0. Выход из программы");
        Console.Write("Выберите пункт (или 'exit' в любой момент): ");
    }

    static void ProcessMainMenuChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                MakeCoffeeFlow();
                break;
            case "2":
                RefillIngredients();
                break;
            case "3":
                machine.ShowIngredients();
                break;
            case "4":
                machine.SaveConfiguration();
                break;
            case "5":
                FileManager.GenerateEndOfDayReport();
                break;
            case "0":
                Console.WriteLine("Спасибо за покупку! До свидания!");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Неверный выбор! Попробуйте снова.");
                break;
        }
    }
    
    static void MakeCoffeeFlow()
    {
        Console.WriteLine("ПРИГОТОВЛЕНИЕ КОФЕ");
        
        machine.ShowCoffeeMenu();
        Console.Write("Введите тип кофе (номер или название): ");
        string coffeeChoice = ReadInputWithExit();
        if (coffeeChoice == null) return;

        Coffee selectedCoffee = GetCoffeeByChoice(coffeeChoice);
        if (selectedCoffee == null)
        {
            Console.WriteLine("Такой кофе нет в меню!");
            return;
        }
        
        Console.Write("Введите объём (мл) [200/300/400]: ");
        string volumeInput = ReadInputWithExit();
        if (volumeInput == null) return;

        int volume = 200;
        if (int.TryParse(volumeInput, out int v) && (v == 200 || v == 300 || v == 400))
        {
            volume = v;
        }
        else
        {
            Console.WriteLine("Неверный объём, установлен 200мл");
        }
        
        Console.WriteLine("ДОБАВКИ (можно выбрать несколько через пробел): ");
        Console.WriteLine("1. Сахар (бесплатно)");
        Console.WriteLine("2. Корица (бесплатно)");
        Console.WriteLine("3. Сироп Карамель (+20₽ за порцию)");
        Console.WriteLine("4. Сироп Ваниль (+20₽ за порцию)");
        Console.WriteLine("5. Сироп Фундук (+20₽ за порцию)");
        Console.WriteLine("0. Нет добавок");
        Console.Write("Выберите добавки (например: 1 3 или 2 4 5): ");

        string addonsInput = ReadInputWithExit();
        if (addonsInput == null) return;

        List<string> selectedAddons = new List<string>();
        int totalSyrupPortions = 0;
        string addonsDisplay = "Нет";

        if (addonsInput != "0")
        {
            string[] choices = addonsInput.Split(' ');
            List<string> addonNames = new List<string>();
    
            foreach (string choice in choices)
            {
                if (choice == "1") addonNames.Add("Сахар");
                else if (choice == "2") addonNames.Add("Корица");
                else if (choice == "3") 
                { 
                    addonNames.Add("Сироп Карамель"); 
                    totalSyrupPortions++; 
                }
                else if (choice == "4") 
                { 
                    addonNames.Add("Сироп Ваниль"); 
                    totalSyrupPortions++; 
                }
                else if (choice == "5") 
                { 
                    addonNames.Add("Сироп Фундук"); 
                    totalSyrupPortions++; 
                }
            }
    
            if (addonNames.Count > 0)
            {
                addonsDisplay = string.Join(", ", addonNames);
            }
        }
        
        decimal finalPrice = CalculatePrice(selectedCoffee, volume, addonsDisplay, totalSyrupPortions);
        
        Console.WriteLine("ВАШ ЗАКАЗ");
        Console.WriteLine($" {selectedCoffee.Name}");
        Console.WriteLine($" Объём: {volume}мл");
        Console.WriteLine($" Добавки: {addonsDisplay}");
        Console.WriteLine($" ИТОГО: {finalPrice}₽");
        Console.Write("Введите 'купить' для оплаты или 'exit' для отмены: ");
        
        string confirm = Console.ReadLine();
        if (confirm?.ToLower() == "exit")
        {
            Console.WriteLine("Заказ отменен.");
            return;
        }
        
        if (confirm?.ToLower() == "купить" || confirm?.ToLower() == "buy")
        {
            Console.Write("Внесите сумму: ");
            string paymentInput = ReadInputWithExit();
            if (paymentInput == null) return;

            if (decimal.TryParse(paymentInput, out decimal payment))
            {
                if (payment >= finalPrice)
                {
                    if (machine.MakeCoffee(selectedCoffee, volume, addonsDisplay, totalSyrupPortions))
                    {
                        FileManager.LogSale(selectedCoffee.Name, finalPrice);
                        machine.SaveConfiguration();
                        
                        decimal change = payment - finalPrice;
                        if (change > 0)
                        {
                            Console.WriteLine($"Сдача: {change}₽");
                        }
                        Console.WriteLine("Кофе готовится! Приятного аппетита!");
                    }
                    else
                    {
                        Console.WriteLine("Не удалось приготовить кофе!");
                    }
                }
                else
                {
                    Console.WriteLine($"Недостаточно средств! Нужно {finalPrice}₽");
                }
            }
            else
            {
                Console.WriteLine("Неверная сумма!");
            }
        }
        else
        {
            Console.WriteLine("Неверная команда!");
        }
    }
    
    static Coffee GetCoffeeByChoice(string choice)
    {
        if (int.TryParse(choice, out int index) && 
            index > 0 && index <= machine.CoffeeTypes.Count)
        {
            return machine.CoffeeTypes[index - 1];
        }
        
        foreach (var coffee in machine.CoffeeTypes)
        {
            if (coffee.Name.ToLower().Contains(choice.ToLower()))
            {
                return coffee;
            }
        }

        return null;
    }
    
    static decimal CalculatePrice(Coffee coffee, int volume, string addon, int syrupPortions = 0)
    {
        decimal price = coffee.Price;
        
        if (volume == 300) price += 20;
        if (volume == 400) price += 40;
        
        if (syrupPortions > 0)
        {
            price += syrupPortions * 20;
        }

        return price;
    }
    
    static void RefillIngredients()
    {
        machine.RefillIngredient();
    }
}