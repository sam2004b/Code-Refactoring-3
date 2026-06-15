using System.Text.Json;

namespace AutoServiceApp.Storage;

public class JsonFileStore<T> : IDataProvider<T>
{
    private const string BrokenFileSuffix = ".broken.";

    public string Folder { get; set; }

    public JsonSerializerOptions Options { get; set; }
        = new() { WriteIndented = true };

    public JsonFileStore()
    {
        var root = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        Folder = Path.Combine(root, "AutoServiceApp");

        Directory.CreateDirectory(Folder);
    }

    public List<T> Load(string name)
    {
        var file = Path.Combine(Folder, name);

        if (!File.Exists(file))
            return new List<T>();

        try
        {
            return ReadAndDeserialize(file);
        }
        catch
        {
            CreateBackup(file, name);
            return new List<T>();
        }
    }

    public void Save(string name, List<T> values)
    {
        var file = Path.Combine(Folder, name);

        var json = JsonSerializer.Serialize(
            values,
            Options);

        File.WriteAllText(file, json);
    }

    private List<T> ReadAndDeserialize(string file)
    {
        var json = File.ReadAllText(file);

        return JsonSerializer.Deserialize<List<T>>(
            json,
            Options) ?? new List<T>();
    }

    private void CreateBackup(string file, string name)
    {
        var bad = Path.Combine(
            Folder,
            name + BrokenFileSuffix + DateTime.Now.Ticks);

        try
        {
            File.Copy(file, bad);
        }
        catch
        {
        }
    }
}