using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICS
{
    public class PicsDataContext : INotifyPropertyChanged
    {
        public Collection<string> GenderList { get; set; }
        public Collection<int> AgeList { get; set; }
        public Collection<string> CameraDevices { get; set; }

        private string folderTag;
        public string FolderTag
        {
            get => folderTag;
            set
            {
                if (value == null)
                {
                    folderTag = "---";
                    FolderTagEnabled = false;
                }
                else
                {
                    folderTag = value;
                    FolderTagEnabled = true;
                }
                NotifyPropertyChanged(nameof(FolderTag));
            }
        }
        private bool folderTagEnabled;
        public bool FolderTagEnabled
        {
            get => folderTagEnabled;
            private set
            {
                folderTagEnabled = value;
                NotifyPropertyChanged(nameof(FolderTagEnabled));
            }
        }

        private bool camControlIsEnabled;
        public bool CamControlsEnabled
        {
            get => camControlIsEnabled;
            set
            {
                camControlIsEnabled = value;
                NotifyPropertyChanged(nameof(CamControlsEnabled));
            }
        }

        public PicsDataContext()
        {
            GenderList = new Collection<string> { "Male", "Female", "Other" };
            
            AgeList = new Collection<int>();
            for (int i = 1; i < 101; i++)
            {
                AgeList.Add(i);
            }

            CameraDevices = new Collection<string>();

            FolderTag = null;
            CamControlsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
