using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            this.DataContext = DCX;
            UpdateExperimentDetail();
            UpdateCameraList();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCameraControlRatio();
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            string tag = GetSanitizedFolderTag();
            if(tag != null)
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
            ExpMananger.PreviousExperiment();
            UpdateExperimentDetail(); 
        }

        private void Button_ExpRight_Click(object sender, RoutedEventArgs e)
        {
            ExpMananger.NextExperiment();
            UpdateExperimentDetail();
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
            CameraCaptureRoutine();
        }

        private void ClearUserInputs()
        {
            TextBox_Name.Text = null;
            ComboBox_Gender.SelectedIndex = -1;
            ComboBox_Age.SelectedIndex = -1;
        }

        private string GetSanitizedFolderTag()
        {
            if(TextBox_Name.Text.Length == 0)
            {
                ShowPopupMessage("[Name] cannot be empty!");
            }
            else if(ComboBox_Gender.SelectedIndex == -1)
            {
                ShowPopupMessage("[Gender] must be selected!");
            }
            else if(ComboBox_Age.SelectedIndex == -1)
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

        private void CameraStopRoutine()
        {
            UpdateCameraList();
            CameraControl.StopCapture();
        }

        private void CameraStartRoutine()
        {
            if(ComboBox_Camera.SelectedIndex == -1)
            {
                ShowPopupMessage("Select a valid camera first");
                UpdateCameraList();
            }
            else
            {
                CameraControl.StartCapture(CameraIDs[ComboBox_Camera.SelectedIndex]);
            }
        }

        private void CameraCaptureRoutine()
        {
            if(CameraControl.IsCapturing)
            {
                System.Drawing.Bitmap currentImage = CameraControl.GetCurrentImage();
                string savePath = GetFinalSavePath();
                currentImage.Save(savePath);
                ShowPopupMessage($"Image is saved at {savePath}");
                currentImage.Dispose();
            }
            else
            {
                ShowPopupMessage("Select and start a camera first!");
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
    }
}
