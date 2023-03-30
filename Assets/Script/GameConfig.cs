using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ConsoleApp1
{
    public class GameConfig
    {
        public  string difficulty;
        public  JObject config;

        private readonly Dictionary<string, ValueGenerator> generatorFactory = new Dictionary<string, ValueGenerator>
        {
            { "fixed", new FixedValueGenerator() },
            { "random_range", new RandomRangeValueGenerator() },
            { "poisson", new PoissonValueGenerator() },
            { "normal", new NormalValueGenerator() }
        };

        public GameConfig(string difficulty)
        {
            this.difficulty = difficulty;
            config = LoadConfig();
        }

        private JObject LoadConfig()
        {
            string exeDir = Application.dataPath;
            string filePath = System.IO.Path.Combine(exeDir,  "game_config.json");
            string fileContent = System.IO.File.ReadAllText(filePath);


            JObject configData = JObject.Parse(fileContent);
            return configData[difficulty] as JObject;
        }

        public long GetValue(string category, string key = null)
        {
            if (!config.ContainsKey(category))
            {
                throw new KeyNotFoundException($"Category '{category}' not found in game config.");
            }

            JObject categoryConfig = (JObject)config[category];
            if (!categoryConfig.ContainsKey("generation_method"))
            {
                throw new KeyNotFoundException($"Category '{category}' does not contain a 'generation_method' key.");
            }

            string generationMethod = (string)categoryConfig["generation_method"];
            ValueGenerator generator = generatorFactory[generationMethod];
            return generator.Generate(categoryConfig);
        }

        public Dictionary<string, int> GetFixedCountLhsRhs()
        {
            JObject fixedCountConfig = (JObject)config["fixed_count_lhs_rhs"];
            return new Dictionary<string, int>
            {
                { "fixed_lhs_count", (int)fixedCountConfig["fixed_lhs_count"] },
                { "fixed_lhs_size", (int)fixedCountConfig["fixed_lhs_size"] },
                { "fixed_rhs_count", (int)fixedCountConfig["fixed_rhs_count"] },
                { "fixed_rhs_size", (int)fixedCountConfig["fixed_rhs_size"] }
            };
        }

        public ValueGenerator GetLhsSizeGenerator()
        {
            JObject lhsSizeConfig = (JObject)config["lhs_size"];
            string generationMethod = lhsSizeConfig["generation_method"].ToString();
            return generatorFactory[generationMethod];
        }

        public ValueGenerator GetRhsSizeGenerator()
        {
            JObject rhsSizeConfig = (JObject)config["rhs_size"];
            string generationMethod = rhsSizeConfig["generation_method"].ToString();
            return generatorFactory[generationMethod];
        }

        public JObject GetConfig()
        {
            return config;
        }

        public string GetDifficulty()
        {
            return difficulty;
        }
    }
}