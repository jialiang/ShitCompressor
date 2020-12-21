namespace ShitCompressor {
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    public partial class MainWindow : Window {
        public static List<string> PathsToCleanupOnClose { get; set; } = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            foreach (string path in PathsToCleanupOnClose) {
                if (Directory.Exists(path)) {
                    Directory.Delete(path, true);
                }
            }
        }
    }
}
