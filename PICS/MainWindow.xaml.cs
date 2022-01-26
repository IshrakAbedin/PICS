using AForge.Video;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PICS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string ExperimentDataPath = "./ExperimentData.json";
        public PicsDataContext DCX;
        public ExperimentManager ExpMananger;

        private readonly CameraFacade Camera;
        private bool captureFlag = false;
        private string savePath = "";

        public MainWindow()
        {
            DCX = new PicsDataContext();
            ExpMananger = new ExperimentManager(ExperimentDataPath);

            InitializeComponent();

            Camera = new CameraFacade(new NewFrameEventHandler(Video_NewFrame));

            this.KeyDown += new KeyEventHandler(OnKeyboardDown);
            this.DataContext = DCX;
            UpdateCameraList();
            UpdateExperimentControls();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Camera.StopCapturing();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                bi = bitmap.ToBitmapImage();

                if (captureFlag)
                {
                    captureFlag = false;
                    _ = Task.Run(() =>
                      {
                          try
                          {
                              bitmap.Save(savePath);
                          }
                          finally
                          {
                              bitmap.Dispose();
                          }
                      });
                }
                else
                {
                    bitmap.Dispose();
                }

                bi.Freeze(); // avoid cross thread operations and prevents leaks
                _ = Dispatcher.BeginInvoke(new ThreadStart(delegate { CameraControl.Source = bi; }));
            }
            catch (Exception exc)
            {
                //_ = MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error");
                ShowPopupMessage("Error on _videoSource_NewFrame:\n" + exc.Message);
                CameraStopRoutine();
                Environment.Exit(1);
            }
        }

        private void OnKeyboardDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    DisplayHelp();
                    break;
                case Key.F3:
                    ResetAll();
                    break;
                case Key.F5:
                    FlipCameraView();
                    break;
                case Key.F9:
                    SingleTrialRoutine();
                    break;
                case Key.F11:
                    PreviousExperimentRoutine();
                    break;
                case Key.F12:
                    NextExperimentRoutine();
                    break;
                default:
                    break;
            }
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            string tag = GetSanitizedFolderTag();
            if (tag != null)
            {
                DCX.FolderTag = tag;
                DCX.CamControlsEnabled = true;
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            DCX.FolderTag = null;
            DCX.CamControlsEnabled = false;
            ClearUserInputs();
        }

        private void Button_PopupAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogueHost_PopupLog.IsOpen = false;
        }

        private void Button_ExpLeft_Click(object sender, RoutedEventArgs e)
        {
            PreviousExperimentRoutine();
        }

        private void Button_ExpRight_Click(object sender, RoutedEventArgs e)
        {
            NextExperimentRoutine();
        }

        private void Button_CameraStop_Click(object sender, RoutedEventArgs e)
        {
            CameraStopRoutine();
        }

        private void Button_CameraStart_Click(object sender, RoutedEventArgs e)
        {
            CameraStartRoutine();
        }

        private void Button_CameraCapture_Click(object sender, RoutedEventArgs e)
        {
            SingleTrialRoutine();
        }

        private void ClearUserInputs()
        {
            TextBox_Name.Text = null;
            ComboBox_Gender.SelectedIndex = -1;
            ComboBox_Age.SelectedIndex = -1;
        }

        private void ResetAll()
        {
            DCX.FolderTag = null;
            DCX.CamControlsEnabled = false;
            ClearUserInputs();
            ExpMananger.ResetExperiment();
            UpdateExperimentControls();
            CameraStopRoutine();
        }

        private string GetSanitizedFolderTag()
        {
            if (TextBox_Name.Text.Length == 0)
            {
                ShowPopupMessage("[Name] cannot be empty!");
            }
            else if (ComboBox_Gender.SelectedIndex == -1)
            {
                ShowPopupMessage("[Gender] must be selected!");
            }
            else if (ComboBox_Age.SelectedIndex == -1)
            {
                ShowPopupMessage("[Age] must be selected!");
            }
            else
            {
                return $"{TextBox_Name.Text.Replace(' ', '_')}_{ComboBox_Gender.SelectedItem}_{ComboBox_Age.SelectedItem}";
            }
            return null;
        }

        private void ShowPopupMessage(string message)
        {
            TextBlock_Popup.Text = message;
            DialogueHost_PopupLog.IsOpen = true;
        }

        private void UpdateExperimentDetail()
        {
            TextBlock_ExperimentContent.Text = ExpMananger.CurrentExperimentInfo;
            Label_ExpProgress.Content = ExpMananger.ProgressString;
            ProgressBar_ExpProgress.Value = ExpMananger.ProgressValue;
            Button_ExpLeft.IsEnabled = !ExpMananger.InFirstExperiment;
            Button_ExpRight.IsEnabled = !ExpMananger.InLastExperiment;
        }

        private void UpdateBadge()
        {
            Badge_Iteration.Badge = ExpMananger.CurrentIterationCount;
        }

        private void UpdateCheckIcon()
        {
            Icon_DoneCheck.Visibility = ExpMananger.IsCurrentOneDone ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateExperimentControls()
        {
            UpdateExperimentDetail();
            UpdateBadge();
            UpdateCheckIcon();
        }

        private void UpdateCameraList()
        {
            DCX.CameraDevices.Clear();
            foreach (string cameraName in Camera.VideoDeviceNameList)
            {
                DCX.CameraDevices.Add(cameraName);
            }
        }

        private void PreviousExperimentRoutine()
        {
            _ = ExpMananger.PreviousExperiment();
            UpdateExperimentControls();
        }

        private void NextExperimentRoutine()
        {
            _ = ExpMananger.NextExperiment();
            UpdateExperimentControls();
        }

        private void CameraStopRoutine()
        {
            UpdateCameraList();
            Camera.StopCapturing();
            CameraControl.Visibility = Visibility.Hidden;
            Icon_Camera.Visibility = Visibility.Visible;
        }

        private void CameraStartRoutine()
        {
            if (ComboBox_Camera.SelectedIndex == -1)
            {
                ShowPopupMessage("Select a valid camera first");
                UpdateCameraList();
            }
            else
            {
                Camera.StartCapturing(ComboBox_Camera.SelectedIndex);
                CameraControl.Visibility = Visibility.Visible;
                Icon_Camera.Visibility = Visibility.Hidden;
            }
        }

        private void CameraCaptureRoutine()
        {
            if (Camera.IsCapturing)
            {
                savePath = GetFinalSavePath();
                //ShowPopupMessage($"Image is saved at {savePath}");
                captureFlag = true;
            }
            else
            {
                ShowPopupMessage("Select and start a camera first!");
            }
        }

        private void SingleTrialRoutine()
        {
            CameraCaptureRoutine();
            if (Camera.IsCapturing)
            {
                bool completed = ExpMananger.CompleteSingleTrial();
                if (completed)
                {
                    ShowPopupMessage("Thank you, the experiment is completed.");
                    ResetAll();
                }
                UpdateExperimentControls();
            }
        }

        private void FlipCameraView()
        {
            ScaleTransform_CameraControl.ScaleX *= -1;
        }

        private string GetFinalSavePath()
        {
            string baseDir = ExpMananger.SaveDir;
            string userDir = DCX.FolderTag;
            string cameraDir = ComboBox_Camera.SelectedItem.ToString();
            string saveTag = $"{ExpMananger.CurrentExperimentSaveTag}_{ExpMananger.CurrentIterationCount}.png";
            string finalDir = System.IO.Path.Combine(baseDir, userDir, cameraDir);
            PathUtility.CreateDirectoryIfDoesNotExist(finalDir);
            string finalPath = System.IO.Path.Combine(baseDir, userDir, cameraDir, saveTag);
            return finalPath;
        }

        private void DisplayHelp()
        {
            string helpString =
                "Written by Mohammad Ishrak Abedin, 2022.\n\n" +
                "Keyboard Shortcuts:\n\n" +
                "- F1: Help\n" +
                "- F3: Reset\n" +
                "- F5: Flip Camera View\n" +
                "- F9: Capture\n" +
                "- F11: Previous Experiment\n" +
                "- F12: Next Experiment";
            ShowPopupMessage(helpString);
        }
    }
}
