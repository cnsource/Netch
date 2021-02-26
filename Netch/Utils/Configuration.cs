using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Netch.Models;

namespace Netch.Utils
{
    public static class Configuration
    {
        /// <summary>
        ///     数据目录
        /// </summary>
        public const string DATA_DIR = "data";

        /// <summary>
        ///     设置
        /// </summary>
        public static readonly string SETTINGS_JSON = $"{DATA_DIR}\\settings.json";

        /// <summary>
        ///     加载配置
        /// </summary>
        public static void Load()
        {
            if (File.Exists(SETTINGS_JSON))
            {
                Global.Settings = ParseSetting(File.ReadAllText(SETTINGS_JSON));
            }
            else
            {
                // 弹出提示
                i18N.Load("System");

                // 创建 data 文件夹并保存默认设置
                Save();
            }
        }

        public static Setting ParseSetting(string text)
        {
            try
            {
                var settings = JsonSerializer.Deserialize<Setting>(text);
                var settingsElement = JsonSerializer.Deserialize<JsonElement>(text);

                if (settingsElement.TryGetProperty("Server", out var serverElement) && serverElement.EnumerateArray().Any())
                    settings.Server.AddRange(JsonSerializer.Deserialize<IEnumerable<Server>>(serverElement.GetRawText(),
                        new JsonSerializerOptions {Converters = {new ServerConverterWithTypeDiscriminator()}})!);

                if (settings.Profiles.Count > 1 && settings.Profiles[0].Index == settings.Profiles[1].Index)
                    for (var i = 0; i < settings.Profiles.Count; i++)
                        settings.Profiles[i].Index = i;

                return settings;
            }
            catch (Exception e)
            {
                Logging.Error(e.ToString());
                return new Setting();
            }
        }

        /// <summary>
        ///     保存配置
        /// </summary>
        public static void Save()
        {
            if (!Directory.Exists(DATA_DIR))
                Directory.CreateDirectory(DATA_DIR);

            JsonSerializer.SerializeAsync(File.Create(SETTINGS_JSON),
                Global.Settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IgnoreNullValues = true,
                    IncludeFields = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
    }
}