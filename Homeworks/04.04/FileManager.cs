namespace Домашка;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;

public static class FileManager
{
    private static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    private static string salesHistoryPath = Path.Combine(Directory.GetCurrentDirectory(), "sales_history.txt");

    public static void SaveConfig(Config config)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(config, options);
        File.WriteAllText(configPath, json, Encoding.UTF8);
        Console.WriteLine("Конфигурация сохранена в config.json!");
    }

    public static Config LoadConfig()
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath, Encoding.UTF8);
            return JsonSerializer.Deserialize<Config>(json);
        }
        return null;
    }
    public static void LogSale(string coffeeName, decimal price)
    {
        string dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string logEntry = $"[{dateTime}] Продано: {coffeeName}, Цена: {price}\n";
        
        File.AppendAllText(salesHistoryPath, logEntry, Encoding.UTF8);
    }
    public static void GenerateEndOfDayReport()
    {
        string[] sales = ReadSalesHistory();
        decimal totalRevenue = 0;
        int salesCount = 0;
    
        foreach (string line in sales)
        {
            if (line.Contains("Цена:"))
            {
                try
                {
                    string pricePart = line.Split("Цена:")[1].Trim();
                    string priceStr = new string(pricePart.TakeWhile(char.IsDigit).ToArray());
                    
                    if (decimal.TryParse(priceStr, out decimal price))
                    {
                        totalRevenue += price;
                        salesCount++;
                    }
                }
                catch { }
            }
        }
        
        var report = new
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            TotalSales = salesCount,
            TotalRevenue = totalRevenue,
            GeneratedAt = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
        };
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    
        string fileName = $"report_{DateTime.Now:yyyy_MM_dd}.json";
        string reportPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        
        string json = JsonSerializer.Serialize(report, options);
        File.WriteAllText(reportPath, json, Encoding.UTF8);
        
        Console.WriteLine("ОТЧЁТ ЗА СМЕНУ");
        Console.WriteLine($"Дата: {DateTime.Now:dd.MM.yyyy}");
        Console.WriteLine($"Всего продаж: {salesCount}");
        Console.WriteLine($"Выручка: {totalRevenue}₽");
        Console.WriteLine($"Файл: {fileName}");
    }
    
    public static string[] ReadSalesHistory()
    {
        if (File.Exists(salesHistoryPath))
        {
            return File.ReadAllLines(salesHistoryPath, Encoding.UTF8);
        }
        return new string[0];
    }
}