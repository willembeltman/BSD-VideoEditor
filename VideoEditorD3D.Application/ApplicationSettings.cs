using Newtonsoft.Json;

namespace VideoEditorD3D.Application;

public class ApplicationSettings
{
    public string? LastDatabaseFullName { get; set; } = null;

    public static ApplicationSettings Load()
    {
        if (File.Exists("application.json"))
        {
            var json = File.ReadAllText("application.json");
            var config = JsonConvert.DeserializeObject<ApplicationSettings>(json);
            return config!;
        }
        else
        {
            var json = JsonConvert.SerializeObject(new ApplicationSettings());
            File.WriteAllText("application.json", json);
            throw new Exception("made a new application config, please review it");
        }
    }
    public void Save()
    {
        var json = JsonConvert.SerializeObject(this);
        File.WriteAllText("application.json", json);
    }
}