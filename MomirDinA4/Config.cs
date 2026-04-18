using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4;

[method: JsonConstructor]
public class Config(
    string host,
    ushort port,
    bool skipScryfallUpdate,
    int momirAvatarMtgoId
    )
{
    private const string ConfigFileName = "config.json";

    public static readonly Config Instance = ReadConfig();

    public string Host { get; } = host;
    public ushort Port { get; } = port;
    public bool SkipScryfallUpdate { get; } = skipScryfallUpdate;
    public int MomirAvatarMtgoId { get; } = momirAvatarMtgoId;

    public Config() : this("localhost", 8001, true, 23965)
    { 
    }

    private static Config ReadConfig()
    {
        if (!File.Exists(ConfigFileName))
        {
            var config = new Config();
            File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(config));
            return config;
        }

        return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName)) ?? throw new Exception("Could not read config file at " + ConfigFileName);
    }
}
