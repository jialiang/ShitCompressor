namespace ShitCompressor.Classes
{
    using ShitCompressor.utilities;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;

    public class CImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ConcurrentDictionary<string, Process> PendingProcesses = new();
        private readonly List<CancellationTokenSource> CancellationTokenSourceList = new();

        public FileInfo InputInfo { get; private set; }

        public string OriginalSizeText { get; private set; }

        public bool IsSelected { get; set; } = true;

        public BitmapImage PreviewImage { get; set; }

        public string Status { get; private set; } = "Ready"; //Ready, Working

        public SortableBindingList<EncoderExe> EncoderList { get; set; } = new SortableBindingList<EncoderExe>();

        public SortableBindingList<Result> ResultList { get; private set; } = new SortableBindingList<Result>();

        public CImage(FileInfo inputInfo, BitmapImage preview)
        {
            InputInfo = inputInfo;
            OriginalSizeText = Utilities.ByteToKbString(InputInfo.Length);
            PreviewImage = preview;

            List<EncoderExe> activeEncoderList = Globals.AllEncodersFromSettings;

            foreach (EncoderExe encoder in activeEncoderList)
            {
                string isFileSupported = encoder.Input.FirstOrDefault(input => $".{input}" == InputInfo.Extension.ToLower());

                if (isFileSupported == null)
                {
                    continue;
                }

                EncoderList.Add(new EncoderExe(encoder));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static Dictionary<string, double> GetQualityScores(string inputPathname, string outputPathname, string mapPathname)
        {
            Dictionary<string, double> qualityScores = new();

            foreach (Exe qualityCalculator in Globals.qualityCalculators)
            {
                qualityScores.Add(qualityCalculator.Name, -1.0);

                if (qualityCalculator.Name == "butteraugli")
                {
                    if (!Globals.UseButteraugli) continue;

                    string extension = Path.GetExtension(mapPathname);

                    if (extension != ".jpg" && extension != ".png") continue;
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), "exe", qualityCalculator.Filename);
                string arguments = qualityCalculator.Signature.
                    Replace("{InputP}", $"\"{inputPathname}\"").
                    Replace("{OutputP}", $"\"{outputPathname}\"").
                    Replace("{MapP}", $"\"{mapPathname}\"");
                string errorMessage = "";

                CountdownEvent countdownEvent = new(2);

                using Process process = Utilities.ProcessCreator(path, arguments);

                process.OutputDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                    {
                        countdownEvent.Signal();
                        return;
                    }

                    try
                    {
                        qualityScores[qualityCalculator.Name] = double.Parse(e.Data);
                    }
                    catch (Exception exception)
                    {
                        errorMessage += $"Error parsing score: {exception.Message}\n";
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                    {
                        countdownEvent.Signal();
                        return;
                    }

                    errorMessage += e.Data + '\n';
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                countdownEvent.Wait();

                if (errorMessage != "")
                {
                    Utilities.Alert(errorMessage, $"Error calculating {qualityCalculator.Name} score");
                }
            }

            return qualityScores;
        }

        private void Optimize(DateTime startTime)
        {
            string startTimeText = $"{startTime:h:mm:ss tt}";

            Random random = new();

            List<Result> thisRunResultList = new();
            List<Task> taskList = new();
            List<string> filesToCleanup = new();

            string exeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "exe");
            string tempOutputDirectory = Path.Combine(InputInfo.DirectoryName, Globals.TempStorageFolder);

            Directory.CreateDirectory(tempOutputDirectory);

            bool autoSetOptimized = ResultList.Count == 0;

            string inputPathname = InputInfo.FullName;
            string normalizedInputPath = inputPathname;

            if (InputInfo.Extension == ".png" && Status == "Working")
            {
                using Bitmap inputBitmap = new(inputPathname);
                if (Image.IsAlphaPixelFormat(inputBitmap.PixelFormat))
                {
                    using Bitmap normalizedInputBitmap = Utilities.RemoveAlpha(inputBitmap);
                    normalizedInputPath = Path.Combine(tempOutputDirectory, $"{random.Next()}-in.png");
                    normalizedInputBitmap.Save(normalizedInputPath, ImageFormat.Png);
                    filesToCleanup.Add(normalizedInputPath);
                }
            }

            foreach (EncoderExe encoder in EncoderList)
            {
                if (!encoder.IsEnabled || Status != "Working")
                {
                    continue;
                }

                CancellationTokenSource cancellationTokenSource = new();
                CancellationTokenSourceList.Add(cancellationTokenSource);

                Task task = Task.Run(() =>
                {
                    string outputFilename = Utilities.ChangeExtension(
                        Utilities.AppendToFilename(InputInfo.Name, $"({encoder.Name}) ({encoder.Quality}) ({random.Next()})"),
                        encoder.Output
                    );
                    string outputPathname = Path.Combine(tempOutputDirectory, outputFilename);

                    string quality = encoder.Quality.ToString();
                    string pathToEncoder = Path.Combine(exeDirectory, encoder.Filename);
                    string arguments = encoder.Signature.
                        Replace("{InputP}", $"\"{normalizedInputPath}\"").
                        Replace("{OutputP}", $"\"{outputPathname}\"").
                        Replace("{Quality}", quality);

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    using (Process process = Utilities.ProcessCreator(pathToEncoder, arguments))
                    {
                        PendingProcesses.TryAdd(encoder.Name, process);

                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();

                        PendingProcesses.TryRemove(new KeyValuePair<string, Process>(encoder.Name, process));
                    }

                    FileInfo outputInfo = new(outputPathname);
                    bool hasErrorWithOutput = !outputInfo.Exists || outputInfo.Length == 0;

                    Result result = null;
                    Dictionary<string, double> qualityScores = null;
                    string outputMd5 = null;

                    if (!hasErrorWithOutput)
                    {
                        outputMd5 = Utilities.CheckMD5(outputInfo.FullName);
                        result = ResultList.SingleOrDefault(r => r.Md5 == outputMd5 && (quality == r.QualitySetting || quality == "0"));

                        // MD5 matches existing entry, update existing entry
                        if (result != null)
                        {
                            int timeElapsed = (int)(DateTime.Now - startTime).TotalSeconds;

                            result.UpdateTimings(timeElapsed, startTimeText);
                            File.Delete(outputPathname);
                        }
                    }

                    // New entry
                    if (result == null)
                    {
                        if (!hasErrorWithOutput)
                        {
                            string normalizedOutputPath = outputPathname;

                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            qualityScores = GetQualityScores(normalizedInputPath, normalizedOutputPath, outputPathname);
                        }

                        int timeElapsed = (int)(DateTime.Now - startTime).TotalSeconds;

                        result = new Result(
                            encoder.Name,
                            encoder.Output,
                            encoder.IsSpecial,
                            quality,
                            outputInfo,
                            outputMd5,
                            InputInfo,
                            qualityScores,
                            timeElapsed,
                            startTimeText
                        );

                        Application.Current.Dispatcher.Invoke(
                            new Action(() => { ResultList.Add(result); })
                        );
                    }

                    thisRunResultList.Add(result);
                }, cancellationTokenSource.Token);

                taskList.Add(task);
            }

            try
            {
                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            foreach (CancellationTokenSource cancellationTokenSource in CancellationTokenSourceList.ToList())
            {
                cancellationTokenSource.Dispose();
                CancellationTokenSourceList.Remove(cancellationTokenSource);
            }

            foreach (string path in filesToCleanup)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            if (autoSetOptimized)
            {
                Result resultWithSmallestSize = null;

                foreach (Result result in thisRunResultList)
                {
                    if (result.Status != "OK")
                    {
                        continue;
                    }

                    if (result.IsSpecialEncoder)
                    {
                        result.SetPreferred();
                        continue;
                    }

                    if (resultWithSmallestSize == null || result.OutputInfo.Length < resultWithSmallestSize.OutputInfo.Length)
                    {
                        resultWithSmallestSize = result;
                    }
                }

                if (resultWithSmallestSize != null)
                {
                    resultWithSmallestSize.SetPreferred();
                }
            }
        }

        public async void Optimize()
        {
            if (Status != "Ready")
            {
                Debug.WriteLine("Busy");
                return;
            }

            if (!PendingProcesses.IsEmpty)
            {
                Debug.WriteLine("Optimization already in progress.");
                return;
            }

            EncoderExe hasEnabledEncoder = EncoderList.FirstOrDefault(e => e.IsEnabled);

            if (hasEnabledEncoder == null)
            {
                Debug.WriteLine("All encoders disabled.");
                return;
            }

            Status = "Working";
            OnPropertyChanged("Status");

            DateTime startTime = DateTime.Now;
            await Task.Run(() =>
            {
                Optimize(startTime);
            });

            Status = "Ready";
            OnPropertyChanged("Status");
        }

        public void Cancel(bool willBeRemoved = false)
        {
            foreach (CancellationTokenSource cancellationTokenSource in CancellationTokenSourceList.ToList())
            {
                cancellationTokenSource.Cancel();
                CancellationTokenSourceList.Remove(cancellationTokenSource);
            }

            foreach (Process process in PendingProcesses.Values)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        List<Process> childProcessList = Utilities.GetAllChildProcesses((UInt32)process.Id);

                        foreach (Process childProcess in childProcessList)
                        {
                            childProcess.Kill();
                            childProcess.Dispose();
                        }

                        process.Kill();
                        process.Dispose();
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }

            PendingProcesses.Clear();

            if (!willBeRemoved)
            {
                Status = "Ready";
                OnPropertyChanged("Status");
            }
        }

        public void OpenInputFolder()
        {
            try
            {
                Process.Start("explorer", InputInfo.DirectoryName);
            }
            catch (Exception exception)
            {
                Utilities.Alert(exception.Message);
            }
        }

        public void UpdateIsSelected(bool value)
        {
            IsSelected = value;
            OnPropertyChanged("IsSelected");
        }

        public void OpenInput()
        {
            try
            {
                Process.Start("explorer", InputInfo.FullName);
            }
            catch (Exception exception)
            {
                Utilities.Alert(exception.Message);
            }
        }
    }
}
