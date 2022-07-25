namespace ShitCompressor.utilities {
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class Globals {
        public static readonly string[] ValidExtensions = { ".jpg", ".png" };

        public static string TempStorageFolder { get; private set; } = "shit-compressor-temp";

        public static string OutputFolder { get; private set; } = "optimized";

        public static string OutputSuffix { get; private set; } = "(optimized)";

        public static List<EncoderExe> AllEncodersFromSettings { get; private set; } = null;

        public static readonly List<Exe> qualityCalculators = new()
        {
            new Exe("butteraugli", "{InputP} {OutputP}", "butteraugli.exe"),
            new Exe("ssimulacra", "{InputP} {OutputP} {MapP}", "ssimx.exe")
        };

        public static bool UseButteraugli { get; private set; } = false;

        public static string SetAllEncodersFromSettings() {
            string pathToSettings = Path.Combine(Directory.GetCurrentDirectory(), "exe", "settings.json");

            try {
                string settingsString = File.ReadAllText(pathToSettings);
                dynamic settings = JsonConvert.DeserializeObject(settingsString);

                OutputFolder = (string) settings.OutputFolder ?? OutputFolder;
                OutputSuffix = (string) settings.OutputSuffix ?? OutputSuffix;
                UseButteraugli = (bool) (settings.UseButteraugli ?? UseButteraugli);

                List<EncoderExe> encoderList = new();

                foreach (var setting in settings.EncoderSettings) {
                    List<string> settingInput = new();

                    foreach (string input in setting.Input) {
                        settingInput.Add(input);
                    }

                    encoderList.Add(new EncoderExe(
                        (string) setting.Name,
                        (string) setting.Signature,
                        (string) setting.Filename,
                        settingInput,
                        (string) setting.Output,
                        (int) setting.DefaultQuality,
                        (bool) setting.DefaultEnabled,
                        (bool) setting.Special,
                        (int) setting.QualityMin,
                        (int) setting.QualityMax
                    ));
                }

                AllEncodersFromSettings = encoderList;
            } catch (Exception exception) {
                AllEncodersFromSettings = null;
                return $"Error parsing settings.json.\n\n{exception.Message}";
            }

            return null;
        }
    }
}
