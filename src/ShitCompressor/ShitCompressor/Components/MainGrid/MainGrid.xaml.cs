namespace ShitCompressor.Components.MainGrid {
    using ShitCompressor.Classes;
    using ShitCompressor.utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    public partial class MainGrid : UserControl {
        public static SortableBindingList<CImage> ImageList { get; set; } = new SortableBindingList<CImage>();

        public static MainGrid ActiveGrid {
            get; private set;
        }

        private ScrollViewer scrollViewer;

        public MainGrid() {
            InitializeComponent();

            ActiveGrid = this;
            Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            scrollViewer = Utilities.GetScrollViewer(this);
        }

        public void InvokeScrollEvent(MouseWheelEventArgs e) {
            MouseWheelEventArgs eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = MouseWheelEvent,
                Source = e.Source
            };

            scrollViewer.RaiseEvent(eventArg);

            e.Handled = true;
        }

        private async void File_Drop(object sender, DragEventArgs e) {
            List<string> errorMessages = new List<string>();
            string[] droppedFilePathnames = (string[]) e.Data.GetData(DataFormats.FileDrop);

            string errorParsingSettings = Globals.SetAllEncodersFromSettings();

            if (errorParsingSettings != null) {
                errorMessages.Add(errorParsingSettings);
            }

            MainWindow.ActiveWindow.ShowLoading();

            await Task.Run(() => {
                foreach (string pathname in droppedFilePathnames) {
                    FileInfo inputInfo = new FileInfo(pathname);

                    if (!Globals.ValidExtensions.Contains(inputInfo.Extension.ToLower())) {
                        errorMessages.Add($"{pathname} has an invalid extension.");
                        continue;
                    }

                    if (ImageList.ToList().SingleOrDefault(image => image.InputInfo.FullName == pathname) != null) {
                        errorMessages.Add($"{pathname} is a duplicate");
                        continue;
                    }

                    BitmapImage preview = new BitmapImage();

                    try {
                        using Stream fs = new FileStream(pathname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                        preview.BeginInit();
                        preview.StreamSource = fs;
                        preview.DecodePixelWidth = 240;
                        preview.CacheOption = BitmapCacheOption.OnLoad;
                        preview.EndInit();
                        preview.Freeze();
                    } catch (Exception exception) {
                        errorMessages.Add($"Error with readding {pathname} as image file: {exception.Message}");
                        continue;
                    }

                    Application.Current.Dispatcher.Invoke(
                        new Action(() => {
                            ImageList.Add(new CImage(inputInfo, preview));
                            scrollViewer.ScrollToBottom();
                        })
                    );

                    string tempFolder = Path.Combine(Path.GetDirectoryName(pathname), Globals.TempStorageFolder);

                    if (MainWindow.ActiveWindow.PathsToCleanupOnClose.Find((path) => path.Equals(tempFolder)) == null) {
                        MainWindow.ActiveWindow.PathsToCleanupOnClose.Add(tempFolder);
                    }
                }
            });

            MainWindow.ActiveWindow.HideLoading();

            if (errorMessages.Count != 0) {
                Utilities.Alert(string.Join("\n\n", errorMessages.ToArray()));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            foreach (CImage image in ImageList) {
                image.UpdateIsSelected(true);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
            foreach (CImage image in ImageList) {
                image.UpdateIsSelected(false);
            }
        }
    }
}
