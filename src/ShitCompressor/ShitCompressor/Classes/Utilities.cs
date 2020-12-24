namespace ShitCompressor.Classes {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Color = System.Drawing.Color;
    using PixelFormat = System.Drawing.Imaging.PixelFormat;

    internal static class Utilities {
        public static string ByteToKbString(long b, int decimals = 0) {
            return Math.Round((decimal) (b / 1024), decimals) + " kB";
        }

        public static string AppendToFilename(string filename, string append) {
            return string.Join(".", filename.Split('.').Select((part, index) => index == 0 ? $"{part} {append}" : part));
        }

        public static string ChangeExtension(string filename, string newExtension) {
            return string.Join(".", filename.Split('.').Reverse().Skip(1).Reverse()) + "." + newExtension;
        }

        public static Process ProcessCreator(string filename, string arguments) {
            Process process = new Process();

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;

            process.EnableRaisingEvents = true;

            return process;
        }

        public static List<Process> GetAllChildProcesses(UInt32 parentProcessId) {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();

            List<Process> childProcessList = new List<Process>();

            if (collection.Count > 0) {
                foreach (var item in collection) {
                    uint childProcessId = (uint) item["ProcessId"];
                    if ((int) childProcessId != Environment.ProcessId) {
                        childProcessList.AddRange(GetAllChildProcesses(childProcessId));

                        try {
                            Process childProcess = Process.GetProcessById((int) childProcessId);
                            childProcessList.Add(childProcess);
                        } catch (Exception exception) {
                            Debug.WriteLine(exception.Message);
                        }
                    }
                }
            }

            return childProcessList;
        }

        public static Bitmap RemoveAlpha(Bitmap bitmap) {
            Bitmap result = new Bitmap(bitmap.Size.Width, bitmap.Size.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(result);

            graphics.Clear(Color.Black);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.DrawImage(bitmap, 0, 0, bitmap.Size.Width, bitmap.Size.Height);

            return result;
        }

        public static string CheckMD5(string filename) {
            using MD5 md5 = MD5.Create();
            using FileStream stream = File.OpenRead(filename);
            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }

        public static ScrollViewer GetScrollViewer(UIElement element) {
            ScrollViewer retour = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++) {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer viewer) {
                    retour = viewer;
                } else {
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }
            return retour;
        }

        public static void Alert(string message, string title = "Error") {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly
            );
        }
    }
}
