namespace Домашка;

using System;
using System.Collections.Generic;

public class Config
{
    public Dictionary<string, decimal> CoffeePrices { get; set; }
    public Dictionary<string, int> IngredientQuantities { get; set; }

    public Config()
    {
        CoffeePrices = new Dictionary<string, decimal>();
        IngredientQuantities = new Dictionary<string, int>();
    }
}