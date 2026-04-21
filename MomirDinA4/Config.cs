using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4;

[method: JsonConstructor]
public record Config(
    string Host,
    ushort Port,
    bool SkipScryfallUpdate,
    int MomirAvatarMtgoId,
    string? CubeFilename,
    bool CubeCardsAreScryfallIdsInsteadOfNames,
    bool DefaultFilterIncludesDigitalCards,
    bool DefaultFilterIncludesFunnyCards
    )
{
    private const string ConfigFileName = "config.json";

    public static readonly Config Instance = ReadConfig();


    public Config() : this("localhost", 8001, true, 23965, null, false, false, true)
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
