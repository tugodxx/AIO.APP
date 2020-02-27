using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MVVMC;
using AIO.APP.Models;
using DirectShowLib;
using OpenCvSharp;

namespace AIO.APP.Views
{
    public class ScannerViewModel :MVVMCViewModel
    {
        VideoCapture cap;
        WriteableBitmap wb;

        private Bitmap _streamImg;

        public Bitmap StreamImg
        {
            get { return _streamImg; }
            set
            {
                _streamImg = value;
                OnPropertyChanged();
            }
        }

        
        private List<string> _cameraList;

        public List<string> CameraList
        {
            get { return _cameraList; }
            set
            {
                _cameraList = value; 
                OnPropertyChanged();
            }
        }

        public ICommand _openCameraCommand { get; set; }

        public ICommand OpenCameraCommand
        {
            get
            {
                if (_openCameraCommand  == null)
                {
                    _openCameraCommand  = new DelegateCommand(() =>
                    {
                      
                    });
                }
                return _openCameraCommand;
            }
        }

        public ICommand _loadCameraCommand { get; set; }

        public ICommand LoadCameraCommand
        {
            get
            {
                if (_loadCameraCommand  == null)
                {
                    _loadCameraCommand  = new DelegateCommand(() =>
                    {
                        CameraList = GetCamera();
                    });
                }
                return _loadCameraCommand;
            }
        }


        private List<string> GetCamera()
        {
            var camerastorage = new List<string>();
            DsDevice[] directShowCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            for (int i = 0; i < directShowCameras.Length; i++)
            {
                // Console.WriteLine(directShowCameras[i].Name);
                camerastorage.Add(directShowCameras[i].Name);
            }

            return camerastorage;

        }




    }
}
