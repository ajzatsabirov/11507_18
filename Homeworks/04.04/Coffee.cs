namespace Домашка;

public class Coffee
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Dictionary<string, int> Ingredients { get; set; }

    public Coffee(string name, decimal price)
    {
        Name = name;
        Price = price;
        Ingredients = new Dictionary<string, int>();
    }
    
    public void AddIngredientRequirement(string ingredientName, int amount)
    {
        Ingredients[ingredientName] = amount;
    }
    
    public int GetRequiredAmount(string ingredientName)
    {
        if (Ingredients.ContainsKey(ingredientName))
        {
            return Ingredients[ingredientName];
        }
        return 0;
    }

    public override string ToString()
    {
        return $"{Name} - {Price}₽";
    }
}

public class Ingredient
{
    public string Name;
    public int Quantity;
    public int MaxQuantity;

    public Ingredient(string name, int quantity, int maxQuantity)
    {
        Name = name;
        Quantity = quantity;
        MaxQuantity = maxQuantity;
    }
    
    public bool HasEnough(int required)
    {
        return Quantity >= required;
    }
    
    public void Use(int amount)
    {
        if (amount <= Quantity)
        {
            Quantity -= amount;
        }
        else
        {
            Quantity = 0;
        }
    }
    
    public void Refill(int amount)
    {
        if (amount < 0) return;
        
        Quantity += amount;
        
        if (Quantity > MaxQuantity)
        {
            Quantity = MaxQuantity;
        }
    }
    
    public int GetFillPercentage()
    {
        if (MaxQuantity == 0) return 0;
        return (int)((double)Quantity / MaxQuantity * 100);
    }

    public override string ToString()
    {
        string status = Quantity > MaxQuantity * 0.3 ? "+" : "️-";
        return $"{status} {Name}: {Quantity}/{MaxQuantity} ({GetFillPercentage()}%)";
    }
}