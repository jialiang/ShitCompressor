namespace ShitCompressor.Components.MainGrid {
    using ShitCompressor.Classes;
    using System.Windows;
    using System.Windows.Controls;

    public partial class Actions : UserControl {
        public Actions() {
            InitializeComponent();
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e) {
            CImage image = (CImage) (sender as FrameworkElement).DataContext;
            image.Cancel(true);

            MainGrid.ImageList.Remove(image);
        }

        private void Optimize_Button_Click(object sender, RoutedEventArgs e) {
            CImage image = (CImage) (sender as FrameworkElement).DataContext;
            image.Optimize();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e) {
            CImage image = (CImage) (sender as FrameworkElement).DataContext;
            image.Cancel();
        }

        private void Open_Folder_Button_Click(object sender, RoutedEventArgs e) {
            CImage image = (CImage) (sender as FrameworkElement).DataContext;
            image.OpenInputFolder();
        }
    }
}
