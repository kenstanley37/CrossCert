using Core.Models;
using System.Text.Json;

namespace Core.Services
{
    public static class ConfigStore
    {
        public static string GetConfigPath()
        {
            var basePath = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                : Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "", ".config");

            return Path.Combine(basePath, "CrossCert", "config.json");
        }

        public static ClientConfig Load()
        {
            var path = GetConfigPath();
            if (!File.Exists(path)) return new ClientConfig();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ClientConfig>(json) ?? new ClientConfig();
        }

        public static void Save(ClientConfig config)
        {
            var path = GetConfigPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }

}
