using AForge.Video;
using AForge.Video.DirectShow;
using System.Collections.Generic;

namespace PICS
{
    public class CameraFacade
    {
        public List<string> VideoDeviceNameList { get; private set; }
        private FilterInfoCollection videoDevicesList;
        public VideoCaptureDevice videoSource;
        public bool IsCapturing { get; private set; }
        public NewFrameEventHandler OnNewFrame { get; set; }

        public CameraFacade(NewFrameEventHandler onNewFrame)
        {
            IsCapturing = false;
            VideoDeviceNameList = new List<string>();
            OnNewFrame = onNewFrame;
            UpdateVideoDeviceList();
        }

        ~CameraFacade()
        {
            StopCapturing();
        }

        public void UpdateVideoDeviceList()
        {
            VideoDeviceNameList.Clear();
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoDevice in videoDevicesList)
            {
                VideoDeviceNameList.Add(videoDevice.Name);
            }
        }

        public void StartCapturing(int deviceIndex)
        {
            if (!IsCapturing)
            {
                videoSource = new VideoCaptureDevice(videoDevicesList[deviceIndex].MonikerString);
                videoSource.NewFrame += OnNewFrame;
                videoSource.Start();
                IsCapturing = true;
            }
        }

        public void StopCapturing()
        {
            if (IsCapturing)
            {
                videoSource.Stop();
                IsCapturing = false;
            }
        }

        //private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        //{
        //    BitmapImage bi;
        //    using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
        //    {
        //        bi = bitmap.ToBitmapImage();
        //    }
        //    bi.Freeze(); // avoid cross thread operations and prevents leaks
        //    _ = Dispatcher.BeginInvoke(new ThreadStart(delegate { ImageControl.Source = bi; }));
        //}
    }
}
