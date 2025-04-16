using System.Text.Json;

namespace Cleaner;

public class Program
{
    public static void Main (string[] args)
    {
        var configPath = args.Length == 1 ? args[0] : "config.json";
        var config = LoadConfig (configPath);
        Console.WriteLine ($"Конфиг загружен.");
        Console.WriteLine ($"Период автоудаления (дней): {config.RunFrequency} (укажите параметр [\"RunFrequency\": X], где X - количество дней хранения файвов)");
        Console.WriteLine ($"Проверять автозапуск: {config.AddToStartup} (укажите параметр [\"DirectoriesToClean\": [\"X\"]], где X - путь к папке, которую необходимо очистить)");
        Console.WriteLine ($"Список папок для очистки: {string.Join ("; ", config.DirectoriesToClean ?? [])} (укажите параметр [\"AddToStartup\": [true/false]], true - добавлять в автозапуск, false - не добавлять в автозапуск)");
    }

    public static Config LoadConfig (string path)
    {
        if (!Path.Exists (path)) throw new Exception ($"Не удалось найти путь [{path}]");
        string configJson = File.ReadAllText (path);

        try
        {
            var config = JsonSerializer.Deserialize<Config> (configJson) ?? throw new Exception ($"Некорректный формат конфигурационного файла.");
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception ($"Ошибка при загрузке конфига: {ex.Message}");
        }
    }
}