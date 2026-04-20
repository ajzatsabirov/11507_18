namespace Домашка;
using System;
using System.Collections.Generic;
using System.Linq;

public class CoffeeMachine
{
    public List<Coffee> CoffeeTypes { get; set; }
    public Dictionary<string, Ingredient> Ingredients { get; set; }

    public CoffeeMachine()
    {
        CoffeeTypes = new List<Coffee>();
        Ingredients = new Dictionary<string, Ingredient>();
        
        Config config = FileManager.LoadConfig();

        if (config != null)
        {
            Console.WriteLine("Загружена конфигурация из config.json");
            
            void LoadIngredient(string key, string name, int defaultQty, int maxQty)
            {
                int qty = config.IngredientQuantities.ContainsKey(key) 
                    ? config.IngredientQuantities[key] 
                    : defaultQty;
                Ingredients[key] = new Ingredient(name, qty, maxQty);
            }

            LoadIngredient("water", "Вода", 1000, 2000);
            LoadIngredient("milk", "Молоко", 500, 1000);
            LoadIngredient("coffee", "Кофейные зерна", 300, 600);
            LoadIngredient("sugar", "Сахар", 100, 200);
            LoadIngredient("cinnamon", "Корица", 50, 100);
            LoadIngredient("syrup_caramel", "Сироп Карамель", 200, 400);
            LoadIngredient("syrup_vanilla", "Сироп Ваниль", 200, 400);
            LoadIngredient("syrup_hazelnut", "Сироп Фундук", 200, 400);
        }
        else
        {
            Console.WriteLine("Создаём новую конфигурацию...");
            
            Ingredients["water"] = new Ingredient("Вода", 1000, 2000);
            Ingredients["milk"] = new Ingredient("Молоко", 500, 1000);
            Ingredients["coffee"] = new Ingredient("Кофейные зерна", 300, 600);
            Ingredients["sugar"] = new Ingredient("Сахар", 100, 200);
            Ingredients["cinnamon"] = new Ingredient("Корица", 50, 100);
            Ingredients["syrup_caramel"] = new Ingredient("Сироп Карамель", 200, 400);
            Ingredients["syrup_vanilla"] = new Ingredient("Сироп Ваниль", 200, 400);
            Ingredients["syrup_hazelnut"] = new Ingredient("Сироп Фундук", 200, 400);
        }

        AddCoffeeTypes();
    }

    private void AddCoffeeTypes()
    {
        var espresso = new Coffee("Эспрессо", 75);
        espresso.AddIngredientRequirement("water", 30);
        espresso.AddIngredientRequirement("coffee", 25);
        CoffeeTypes.Add(espresso);

        var cappuccino = new Coffee("Капучино", 85);
        cappuccino.AddIngredientRequirement("water", 30);
        cappuccino.AddIngredientRequirement("coffee", 20);
        cappuccino.AddIngredientRequirement("milk", 100);
        CoffeeTypes.Add(cappuccino);

        var latte = new Coffee("Латте", 95);
        latte.AddIngredientRequirement("water", 20);
        latte.AddIngredientRequirement("coffee", 15);
        latte.AddIngredientRequirement("milk", 150);
        CoffeeTypes.Add(latte);

        var americano = new Coffee("Американо", 80);
        americano.AddIngredientRequirement("water", 80);
        americano.AddIngredientRequirement("coffee", 20);
        CoffeeTypes.Add(americano);
    }

    public bool CanMakeCoffee(Coffee coffee, int volume = 200)
    {
        double volumeMultiplier = (double)volume / 200;
        
        foreach (var req in coffee.Ingredients)
        {
            int required = (int)(req.Value * volumeMultiplier);
            if (!Ingredients.ContainsKey(req.Key) || 
                !Ingredients[req.Key].HasEnough(required))
            {
                return false;
            }
        }
        return true;
    }

    public bool MakeCoffee(Coffee coffee, int volume = 200, string addon = "Нет", int syrupPortions = 0)
    {
        double volumeMultiplier = (double)volume / 200;

        if (!CanMakeCoffee(coffee, volume))
        {
            Console.WriteLine("Недостаточно ингредиентов!");
            ShowLowIngredients(coffee, volume);
            return false;
        }
        
        foreach (var req in coffee.Ingredients)
        {
            int required = (int)(req.Value * volumeMultiplier);
            Ingredients[req.Key].Use(required);
        }
        
        if (addon.Contains("Сахар") && Ingredients.ContainsKey("sugar"))
            Ingredients["sugar"].Use(5);
        
        if (addon.Contains("Корица") && Ingredients.ContainsKey("cinnamon"))
            Ingredients["cinnamon"].Use(5);
        
        if (syrupPortions > 0)
        {
            string syrupKey = "syrup_caramel";
            if (addon.Contains("Ваниль")) syrupKey = "syrup_vanilla";
            if (addon.Contains("Фундук")) syrupKey = "syrup_hazelnut";
            
            if (Ingredients.ContainsKey(syrupKey))
            {
                Ingredients[syrupKey].Use(30 * syrupPortions);
            }
        }

        Console.WriteLine($"Готовим {coffee.Name}...");
        System.Threading.Thread.Sleep(2000);

        return true;
    }

    private void ShowLowIngredients(Coffee coffee, int volume)
    {
        double volumeMultiplier = (double)volume / 200;
        
        Console.WriteLine("Не хватает:");
        foreach (var req in coffee.Ingredients)
        {
            int required = (int)(req.Value * volumeMultiplier);
            if (!Ingredients.ContainsKey(req.Key) || 
                !Ingredients[req.Key].HasEnough(required))
            {
                var ing = Ingredients[req.Key];
                Console.WriteLine($"  - {ing.Name}: нужно {required}, есть {ing.Quantity}");
            }
        }
        Console.WriteLine();
    }

    public void RefillIngredient()
    {
        Console.WriteLine("Доступные ингредиенты для пополнения:");
        int index = 1;
        var ingList = Ingredients.Values.ToList();
        
        foreach (var ing in ingList)
        {
            Console.WriteLine($"{index}. {ing}");
            index++;
        }
        Console.WriteLine($"{index}. Пополнить всё");
        Console.WriteLine("0. Назад");

        Console.Write("Выберите что пополнить (номер): ");
        string choice = Console.ReadLine();
        
        if (choice?.ToLower() == "exit") return;

        if (int.TryParse(choice, out int choiceNum) && 
            choiceNum >= 1 && choiceNum <= index)
        {
            if (choiceNum == index)
            {
                foreach (var ing in Ingredients.Values)
                {
                    ing.Refill(ing.MaxQuantity - ing.Quantity);
                }
                Console.WriteLine("Все ингредиенты пополнены!");
            }
            else
            {
                var ing = ingList[choiceNum - 1];
                int amount = ing.MaxQuantity - ing.Quantity;
                ing.Refill(amount);
                Console.WriteLine($"{ing.Name} пополнен на {amount} единиц!");
            }
        }
    }

    public void ShowCoffeeMenu()
    {
        Console.WriteLine("МЕНЮ КОФЕ:");
        for (int i = 0; i < CoffeeTypes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {CoffeeTypes[i]}");
        }
        Console.WriteLine();
    }

    public void ShowIngredients()
    {
        Console.WriteLine("Остатки ингредиентов:");
        foreach (var ing in Ingredients.Values)
        {
            Console.WriteLine($"  {ing}");
        }
        Console.WriteLine();
    }
    
    public void SaveConfiguration()
    {
        Config config = new Config();

        foreach (var coffee in CoffeeTypes)
        {
            config.CoffeePrices[coffee.Name] = coffee.Price;
        }

        foreach (var kvp in Ingredients)
        {
            config.IngredientQuantities[kvp.Key] = kvp.Value.Quantity;
        }

        FileManager.SaveConfig(config);
    }
}