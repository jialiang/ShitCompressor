namespace ShitCompressor {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window {
        public static MainWindow ActiveWindow { get; set; }

        public List<string> PathsToCleanupOnClose { get; set; } = new List<string>();

        private int IsLoadingCounter = 0;

        public MainWindow() {
            InitializeComponent();

            ActiveWindow = this;
        }

        public void ShowLoading() {
            IsLoadingCounter += 1;
            LoadingScreen.Visibility = Visibility.Visible;
        }

        public void HideLoading() {
            IsLoadingCounter -= 1;
            if (IsLoadingCounter == 0) {
                LoadingScreen.Visibility = Visibility.Collapsed;
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            foreach (string path in PathsToCleanupOnClose) {
                try {
                    Directory.Delete(path, true);
                } catch (Exception exception) {
                    Debug.WriteLine(exception.Message);
                }
            }
        }
    }
}
