namespace ShitCompressor.Classes
{
    using ShitCompressor.utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class Result : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsPreferred { get; set; } = false;

        public string EncoderName { get; private set; }

        public string EncoderOutputFormat { get; private set; }

        public bool IsSpecialEncoder { get; private set; }

        public string QualitySetting { get; private set; }

        public FileInfo OutputInfo { get; private set; }

        public FileInfo InputInfo { get; private set; }

        public string Md5 { get; private set; } = "-";

        public string OptimizedSizeText { get; private set; } = "-";

        public string SavingsText { get; private set; } = "-";

        public string Ssimulacra { get; private set; } = "-";

        public string Butteraugli { get; private set; } = "-";

        public string Status { get; private set; } = "Error";

        public int TimeElapsed { get; private set; }

        public string StartTimeText { get; private set; }

        public Result(
            string encoderName,
            string encoderOutputFormat,
            bool isSpecialEncoder,
            string qualitySetting,
            FileInfo outputInfo,
            string md5,
            FileInfo inputInfo,
            Dictionary<string, double> qualityScores,
            int timeElapsed,
            string startTimeText
        )
        {
            EncoderName = encoderName;
            EncoderOutputFormat = encoderOutputFormat;
            IsSpecialEncoder = isSpecialEncoder;
            QualitySetting = qualitySetting == "0" ? "-" : qualitySetting;
            OutputInfo = outputInfo;
            InputInfo = inputInfo;
            TimeElapsed = timeElapsed;
            StartTimeText = startTimeText;

            if (!OutputInfo.Exists || OutputInfo.Length == 0)
            {
                return;
            }

            OptimizedSizeText = Utilities.ByteToKbString(OutputInfo.Length);

            long originalSize = InputInfo.Length;
            long sizeDifference = OutputInfo.Length - originalSize;
            string sizeDifferencePercent = Math.Round((decimal)sizeDifference / (decimal)originalSize * 100, 0).ToString("+#;-#;0");

            SavingsText = $"{Utilities.ByteToKbString(sizeDifference)} ({sizeDifferencePercent}%)";

            if (qualityScores != null && qualityScores["ssimulacra"] != -1.0000)
            {
                Ssimulacra = Math.Round(qualityScores["ssimulacra"], 4).ToString("0.0000", CultureInfo.InvariantCulture);
            }
            else
            {
                Ssimulacra = "-";
            }

            if (qualityScores != null && qualityScores["butteraugli"] != -1.0000)
            {
                Butteraugli = Math.Round(qualityScores["butteraugli"], 4).ToString("0.0000", CultureInfo.InvariantCulture);
            }
            else
            {
                Butteraugli = "-";
            }

            Md5 = md5;
            Status = "OK";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void SetPreferred()
        {
            IsPreferred = true;

            string parentDir = InputInfo.DirectoryName;
            string preferredFileName = Utilities.ChangeExtension(
                Utilities.AppendToFilename(InputInfo.Name, Globals.OutputSuffix),
                EncoderOutputFormat
            );

            Directory.CreateDirectory(Path.Combine(parentDir, Globals.OutputFolder));
            File.Copy(OutputInfo.FullName, Path.Combine(parentDir, Globals.OutputFolder, preferredFileName), true);

            OnPropertyChanged("IsPreferred");
        }

        public void OpenOptimizedImage()
        {
            if (File.Exists(OutputInfo.FullName))
            {
                Process.Start("explorer", $"\"{OutputInfo.FullName}\"");
            }
            else
            {
                Utilities.Alert($"{OutputInfo.FullName} is missing");
            }
        }

        public void OpenSsimMap()
        {
            string pathname = $"{OutputInfo.FullName}.ssim.png";

            if (File.Exists(pathname))
            {
                Process.Start("explorer", pathname);
            }
            else
            {
                Utilities.Alert($"{pathname} is missing.");
            }
        }

        public void OpenEdgeDiffMap()
        {
            string pathname = $"{OutputInfo.FullName}.edgediff.png";

            if (File.Exists(pathname))
            {
                Process.Start("explorer", pathname);
            }
            else
            {
                Utilities.Alert($"{pathname} is missing.");
            }
        }

        public void UpdateTimings(int timeElapsed, string startTimeText)
        {
            TimeElapsed = timeElapsed;
            StartTimeText = startTimeText;

            OnPropertyChanged("TimeElapsed");
            OnPropertyChanged("StartTimeText");
        }
    }
}
