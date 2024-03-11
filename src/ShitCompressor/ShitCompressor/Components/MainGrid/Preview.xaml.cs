namespace ShitCompressor.Components.MainGrid
{
    using ShitCompressor.Classes;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class Preview : UserControl
    {
        public Preview()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CImage image = (CImage)(sender as FrameworkElement).DataContext;
            image.OpenInput();
        }
    }
}
