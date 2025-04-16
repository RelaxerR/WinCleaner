namespace Cleaner;

public class Config
{
    public List<string>? DirectoriesToClean { get; set; }
    public int? RunFrequency { get; set; }
    public bool? AddToStartup { get; set; }
}