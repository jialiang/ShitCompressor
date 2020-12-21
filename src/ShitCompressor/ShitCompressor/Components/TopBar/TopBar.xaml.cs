﻿namespace ShitCompressor.Components.TopBar {
    using ShitCompressor.Classes;
    using ShitCompressor.utilities;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    public partial class TopBar : UserControl {
        public TopBar() {
            InitializeComponent();
        }

        private void Optimize_Click(object sender, RoutedEventArgs e) {
            foreach (CImage image in MainGrid.MainGrid.ImageList) {
                if (image.IsSelected) {
                    image.Optimize();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            foreach (CImage image in MainGrid.MainGrid.ImageList) {
                if (image.IsSelected) {
                    image.Cancel();
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            foreach (CImage image in new SortableBindingList<CImage>(MainGrid.MainGrid.ImageList)) {
                if (image.IsSelected) {
                    image.Cancel(true);
                    MainGrid.MainGrid.ImageList.Remove(image);
                }
            }
        }

        private void Settings_Edit_Click(object sender, RoutedEventArgs e) {
            string pathToSettings = Path.Combine(Directory.GetCurrentDirectory(), "exe", "settings.json");

            if (File.Exists(pathToSettings)) {
                Process.Start("explorer", $"\"{pathToSettings}\"");
            } else {
                Utilities.Alert("Settings.json is missing");
            }
        }

        private void Settings_Reload_Click(object sender, RoutedEventArgs e) {
            string message = "Settings.json loaded successfully.\n\nNew settings will be applied to subsequent imports.";
            string errorParsingSettings = Globals.SetAllEncodersFromSettings();

            if (errorParsingSettings != null) {
                message = errorParsingSettings;
            }

            Utilities.Alert(message, "Success");
        }
    }
}