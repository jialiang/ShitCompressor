namespace ShitCompressor.Components.MainGrid {
    using ShitCompressor.Classes;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class Results : UserControl {
        public Results() {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e) {
            Result result = (Result) (sender as FrameworkElement).DataContext;
            result.SetPreferred();
        }

        private void Size_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Result result = (Result) (sender as FrameworkElement).DataContext;
            result.OpenOptimizedImage();
        }

        private void SSIM_Map_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Result result = (Result) (sender as FrameworkElement).DataContext;
            result.OpenSsimMap();
        }

        private void Artifact_Edges_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Result result = (Result) (sender as FrameworkElement).DataContext;
            result.OpenEdgeDiffMap();
        }

        private void Remove_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Result result = (Result) (sender as FrameworkElement).DataContext;
            ((SortableBindingList<Result>) dataGrid.ItemsSource).Remove(result);
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            MainGrid.ActiveGrid.InvokeScrollEvent(e);
        }
    }
}
