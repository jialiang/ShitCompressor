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

        public static readonly List<EncoderExe> AllEncoders = new List<EncoderExe>()
        {
            new EncoderExe(
                "guetzli",
                "--quality {Quality} {InputP} {OutputP}",
                "guetzli.exe",
                new List<string>(){ "jpg", "png" },
                "jpg",
                95,
                false
            ),
            new EncoderExe(
                "mozjpeg",
                "{InputP} {OutputP} {Quality}",
                "cjpeg.bat",
                new List<string>(){ "jpg", "png" },
                "jpg",
                95
            ),
            new EncoderExe(
                "jpegtran",
                "{InputP} {OutputP}",
                "jpegtran.bat",
                new List<string>(){ "jpg" },
                "jpg",
                0
            ),
            new EncoderExe(
                "jpegoptim",
                "{InputP} {OutputP} --strip-all --force --max={Quality}",
                "imageoptim.bat",
                new List<string>(){ "jpg" },
                "jpg",
                95
            ),
            new EncoderExe(
                "webp",
                "-q {Quality} -noalpha {InputP} -o {OutputP}",
                "cwebp.exe",
                new List<string>(){ "jpg", "png" },
                "webp",
                95,
                true,
                true
            )
        };

        public static List<EncoderExe> AllEncodersFromSettings { get; private set; } = null;

        public static readonly List<Exe> qualityCalculators = new List<Exe>()
        {
            new Exe("butteraugli", "{InputP} {OutputP}", "butteraugli.exe"),
            new Exe("ssimulacra", "{InputP} {OutputP} {MapP}", "ssimulacra.exe")
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

                List<EncoderExe> encoderList = new List<EncoderExe>();

                foreach (var setting in settings.EncoderSettings) {
                    List<string> settingInput = new List<string>();

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
                        (bool) setting.Special
                    ));
                }

                AllEncodersFromSettings = encoderList;
            } catch (Exception exception) {
                AllEncodersFromSettings = null;
                return $"Error parsing settings.json. Application defaults will be used.\n\n{exception.Message}";
            }

            return null;
        }
    }
}
