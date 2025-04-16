using System.Text.Json;

namespace Cleaner;

public class Program
{
    public static void Main (string[] args)
    {
        var configPath = args.Length == 1 ? args[0] : "config.json";
        var config = LoadConfig (configPath);
        Console.WriteLine ("Конфиг загружен.");
        Console.WriteLine ($"Период автоудаления (дней): {config.RunFrequency} (укажите параметр [\"RunFrequency\": X], где X - количество дней хранения файвов)");
        Console.WriteLine ($"Проверять автозапуск: {config.AddToStartup} (укажите параметр [\"DirectoriesToClean\": [\"X\"]], где X - путь к папке, которую необходимо очистить)");
        Console.WriteLine ($"Список папок для очистки: {string.Join ("; ", config.DirectoriesToClean ?? [])} (укажите параметр [\"AddToStartup\": [true/false]], true - добавлять в автозапуск, false - не добавлять в автозапуск)");

        if (config.AddToStartup == true) EnsureAutoStart ();
    }

    private static Config LoadConfig (string path)
    {
        if (!Path.Exists (path)) throw new Exception ($"не удалось найти путь [{path}]");
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

    private static void EnsureAutoStart ()
    {
        string startupFolder = Environment.GetFolderPath (Environment.SpecialFolder.Startup);
        string shortcutPath = Path.Combine (startupFolder, "WinCleaner.lnk");
        string exePath = System.Reflection.Assembly.GetExecutingAssembly ().Location;

        if (!File.Exists (shortcutPath))
        {
            Console.WriteLine ("Добавление программы в автозапуск...");
            CreateShortcut (shortcutPath, exePath);
        }
        else
        {
            Console.WriteLine ("Программа уже добавлена в автозапуск.");
        }
    }

    private static void CreateShortcut (string shortcutPath, string targetPath)
    {
#if NETCOREAPP || NET5_0_OR_GREATER
        if (OperatingSystem.IsWindows ())
#endif
        {
            var shellType = Type.GetTypeFromProgID ("WScript.Shell") ?? throw new Exception ("не удалось получить тип shell для добавления в автозапуск.");
            dynamic shell = Activator.CreateInstance (shellType) ?? throw new Exception ("не удалось создать shell для добавления в автозапуск.");
            dynamic shortcut = shell.CreateShortcut (shortcutPath);

            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName (targetPath);
            shortcut.Save ();
        }
        else
        {
            throw new Exception ("Добавление в автозапуск доступно только для Windows.");
        }
    }
}