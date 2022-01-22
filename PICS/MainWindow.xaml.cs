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

        public MainWindow()
        {
            DCX = new PicsDataContext();
            ExpMananger = new ExperimentManager(ExperimentDataPath);
            InitializeComponent();
            this.DataContext = DCX;
            UpdateExperimentDetail();
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
    }
}
