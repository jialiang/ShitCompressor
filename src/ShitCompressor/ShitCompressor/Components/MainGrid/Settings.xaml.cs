namespace ShitCompressor.Components.MainGrid {
    using ShitCompressor.utilities;
    using System.Windows;
    using System.Windows.Controls;

    public partial class Settings : UserControl {
        public Settings() {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            MainGrid.ActiveGrid.InvokeScrollEvent(e);
        }

        private void Quality_Down_Click(object sender, System.Windows.RoutedEventArgs e) {
            EncoderExe encoder = (EncoderExe) (sender as FrameworkElement).DataContext;
            encoder.DecreaseQuality();
        }

        private void Quality_Up_Click(object sender, System.Windows.RoutedEventArgs e) {
            EncoderExe encoder = (EncoderExe) (sender as FrameworkElement).DataContext;
            encoder.IncreaseQuality();
        }
    }
}
