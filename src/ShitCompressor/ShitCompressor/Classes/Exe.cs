using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShitCompressor.utilities {
    public class Exe {
        public string Name {
            get; private set;
        }

        public string Signature {
            get; private set;
        }

        public string Filename {
            get; private set;
        }

        public Exe(string name, string signature, string filename) {
            Name = name;
            Signature = signature;
            Filename = filename;
        }
    }

    public class EncoderExe : Exe, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public int DefaultQuality {
            get; private set;
        }

        public bool DefaultEnabled {
            get; private set;
        }

        public List<string> Input {
            get; private set;
        }

        public string Output {
            get; private set;
        }

        public int Quality {
            get; set;
        }

        public int QualityMin
        {
            get; set;
        }

        public int QualityMax
        {
            get; set;
        }

        public bool IsEnabled {
            get; set;
        }

        public bool IsSpecial {
            get; private set;
        }

        public EncoderExe(
            string name,
            string signature,
            string filename,
            List<string> input,
            string output,
            int defaultQuality = 0,
            bool defaultEnabled = true,
            bool isSpecial = false,
            int qualityMin = 50,
            int qualityMax = 100
        ) : base(name, signature, filename) {
            Input = input;
            Output = output;
            DefaultQuality = defaultQuality;
            DefaultEnabled = defaultEnabled;
            IsSpecial = isSpecial;

            Quality = defaultQuality;
            QualityMin = qualityMin;
            QualityMax = qualityMax;
            IsEnabled = defaultEnabled;
        }

        public EncoderExe(EncoderExe exe) : this(
            exe.Name,
            exe.Signature,
            exe.Filename,
            exe.Input,
            exe.Output,
            exe.Quality,
            exe.IsEnabled,
            exe.IsSpecial,
            exe.QualityMin,
            exe.QualityMax
        ) {
        }

        public void IncreaseQuality()
        {
            Quality = Math.Min(Quality + 1, QualityMax);
            OnPropertyChanged("Quality");
        }

        public void DecreaseQuality() {
            Quality = Math.Max(Quality - 1, QualityMin);
            OnPropertyChanged("Quality");
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
