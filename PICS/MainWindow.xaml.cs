using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WebEye.Controls.Wpf;

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
        public List<WebCameraId> CameraIDs;

        public MainWindow()
        {
            DCX = new PicsDataContext();
            ExpMananger = new ExperimentManager(ExperimentDataPath);

            InitializeComponent();

            this.KeyDown += new KeyEventHandler(OnKeyboardDown);
            this.DataContext = DCX;
            UpdateCameraList();
            UpdateExperimentControls();
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCameraControlRatio();
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
            UpdateCameraControlRatio();
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
            CameraIDs = new List<WebCameraId>(CameraControl.GetVideoCaptureDevices());
            foreach (WebCameraId camera in CameraIDs)
            {
                DCX.CameraDevices.Add(camera.Name);
            }
        }

        private void UpdateCameraControlRatio()
        {
            double Ratio;
            if (CameraControl.IsCapturing)
            {
                Ratio = ((double)CameraControl.VideoSize.Height) / CameraControl.VideoSize.Width;
                CameraControl.Height = CameraControl.ActualWidth * Ratio;
            }
            else
            {
                CameraControl.Width = Card_CameraControlHousing.Width;
                CameraControl.Height = Card_CameraControlHousing.Height;
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
            CameraControl.StopCapture();
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
                CameraControl.StartCapture(CameraIDs[ComboBox_Camera.SelectedIndex]);
                CameraControl.Visibility = Visibility.Visible;
                Icon_Camera.Visibility = Visibility.Hidden;
            }
        }

        private void CameraCaptureRoutine()
        {
            if (CameraControl.IsCapturing)
            {
                System.Drawing.Bitmap currentImage = CameraControl.GetCurrentImage();
                string savePath = GetFinalSavePath();
                currentImage.Save(savePath);
                //ShowPopupMessage($"Image is saved at {savePath}");
                currentImage.Dispose();
            }
            else
            {
                ShowPopupMessage("Select and start a camera first!");
            }
        }

        private void SingleTrialRoutine()
        {
            CameraCaptureRoutine();
            if (CameraControl.IsCapturing)
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
                "- F9: Capture\n" +
                "- F11: Previous Experiment\n" +
                "- F12: Next Experiment";
            ShowPopupMessage(helpString);
        }
    }
}
