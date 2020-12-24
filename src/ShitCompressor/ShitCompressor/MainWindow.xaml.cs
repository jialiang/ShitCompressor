namespace ShitCompressor {
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window {
        public static List<string> PathsToCleanupOnClose { get; set; } = new List<string>();

        public MainWindow() {
            InitializeComponent();

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
