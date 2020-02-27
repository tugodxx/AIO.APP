using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MVVMC;
using AIO.APP.Models;
using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Common;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Size = OpenCvSharp.Size;

namespace AIO.APP.Views
{
    public class ScannerViewModel :MVVMCViewModel
    {
        static VideoCapture _capture;
        static Mat _frame;
        static Bitmap _image;
        static Thread _cameraThread;
        static Thread _imageThread;
        bool _isCameraRunning;
        bool _isImageRunning;
        const int frameWidth = 480;
        const int frameHeight = 200;
        private bool isSubmit;
        private Mat _cropimgMat;
        private int delaycount1=0;
        private int delaycount2=0;
        private bool waitpic = true;

        #region param
        private BarcodeInfo _tmpBarcodeInfo;

        public BarcodeInfo TmpBarcodeInfo
        {
            get { return _tmpBarcodeInfo; }
            set
            {
                _tmpBarcodeInfo = value;
                OnPropertyChanged();
            }
        }

        private string _barcodeText="######";

        public string BarcodeText
        {
            get { return _barcodeText; }
            set
            {
                _barcodeText = value;
                OnPropertyChanged();
            }
        }

        private int _dectdelay=10;

        public int Dectdelay
        {
            get { return _dectdelay; }
            set
            {
                _dectdelay = value;
                OnPropertyChanged();
            }
        }

        private int _switchdelay=10;

        public int Switchdelay
        {
            get { return _switchdelay; }
            set
            {
                _switchdelay = value;
                OnPropertyChanged();
            }
        }

        private bool _isAutoSubmit=false;

        public bool  IsAutoSubmit
        {
            get { return _isAutoSubmit; }
            set
            {
                _isAutoSubmit = value;
                OnPropertyChanged();
            }
        }














        private string _barcodeFormat="####";

        public string BarcodeFormat
        {
            get { return _barcodeFormat; }
            set
            {
                _barcodeFormat= value;
                OnPropertyChanged();
            }
        }


        private BitmapSource _streamSource;

        public BitmapSource StreamSource
        {
            get { return _streamSource; }
            set
            {
                _streamSource = value; 
                OnPropertyChanged();
            }

        }
        private BitmapSource _barcodeImg;

        public BitmapSource BarcodeImg
        {
            get { return _barcodeImg; }
            set
            {
                _barcodeImg = value; 
                OnPropertyChanged();
            }

        }
        private double _threshold=120;

        public double Threshold
        {
            get { return _threshold; }
            set
            {
                _threshold = value; 
               // Console.WriteLine(value);
                OnPropertyChanged();
            }

        }

        private int _selCameraIndex;

        public int SelCameraIndex
        {
            get { return _selCameraIndex; }
            set
            {
                _selCameraIndex = value; 
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
        #endregion


        #region icommand
        
        private ICommand _openCameraCommand { get; set; }

        public ICommand OpenCameraCommand
        {
            get
            {
                if (_openCameraCommand  == null)
                {
                    _openCameraCommand  = new DelegateCommand(() =>
                    {
                       
                        CameraInitalize();
                       
                    });
                }
                return _openCameraCommand;
            }
        }

        private ICommand _loadCameraCommand { get; set; }

        public ICommand LoadCameraCommand
        {
            get
            {
                return _loadCameraCommand ?? (_loadCameraCommand = new DelegateCommand(() =>
                {
                    CameraList = GetCamera();
                }));
            }
        }
        
        private ICommand _unloadCameraCommand { get; set; }

        public ICommand UnloadCameraCommand
        {
            get
            {
                return _unloadCameraCommand ?? (_unloadCameraCommand = new DelegateCommand(() =>
                {
                    _cameraThread.Abort();
                    
                    _capture.Dispose();
                    _frame.Dispose();
                    _image.Dispose();
                    _isCameraRunning = false;
                }));
            }
        }

        private ICommand _startCameraCommand { get; set; }

        public ICommand StartCameraCommand
        {
            get
            {
                return _startCameraCommand ?? (_startCameraCommand = new DelegateCommand(() =>
                {

                    CameraInitalize();
                }));
            }
        }


        private ICommand _stopCameraCommand { get; set; }

        public ICommand StopCameraCommand
        {
            get
            {
                return _stopCameraCommand ?? (_stopCameraCommand = new DelegateCommand(() =>
                {
                    _cameraThread.Abort();
                    _capture.Dispose();
                    _frame.Dispose();
                    _image.Dispose();
                    _isCameraRunning = false;
                }));
            }
        }

        private ICommand _submitCommand { get; set; }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new DelegateCommand(() =>
                {
                    isSubmit = true;
                    waitpic = true;
                }));
            }
        }

        private ICommand _cancelCommand { get; set; }

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() =>
                {
                    isSubmit = false;
                    waitpic = true;
                    TmpBarcodeInfo =new BarcodeInfo()
                    {
                        Text = string.Empty,Format = "Cancelled"
                    };
                }));
            }
        }
        
        public ICommand _switchAutoSubmitCommand { get; set; }

        public ICommand SwitchAutoSubmitCommand
        {
            get
            {
                if (_switchAutoSubmitCommand  == null)
                {
                    _switchAutoSubmitCommand  = new DelegateCommand(() => {IsAutoSubmit = !IsAutoSubmit; });
                }
                return _switchAutoSubmitCommand;
            }
        }
        #endregion

        private List<string> GetCamera()
        {
            var cameraStorage = new List<string>();
            DsDevice[] directShowCameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            for (int i = 0; i < directShowCameras.Length; i++)
            {
                // Console.WriteLine(directShowCameras[i].Name);
                cameraStorage.Add(directShowCameras[i].Name);
            }

            return cameraStorage;

        }

        public void CameraInitalize()
        {
            if (_isCameraRunning)
            { return;}
            CaptureCamera();
            _isCameraRunning = true;
        }
        
        private void CaptureCamera()
        {
            _cameraThread = new Thread(CaptureCameraCallback);
           //CaptureCameraCallback();
            _cameraThread.Start();
        }

        private void CaptureCameraCallback()
        {
            TmpBarcodeInfo =new BarcodeInfo();
            _frame = new Mat();
            _capture = new VideoCapture
            {
                FrameWidth = 960,
                FrameHeight = 720
            };
          //  _capture.Set(VideoCaptureProperties.FrameHeight, 200.0);
         //   _capture.Set(VideoCaptureProperties.FrameWidth , .0);
            _capture.Open(_selCameraIndex);
          //  var wb = new WriteableBitmap(_capture.FrameWidth, _capture.FrameHeight, 96, 96, PixelFormats.Bgr24, null);
            if (!_capture.IsOpened())
            {
                return;
            }

            while (_isCameraRunning)
            {
                _capture.Read(_frame);
                _image = _frame.ToBitmap();
               // StreamImg?.Dispose();
               StreamSource = _image.ToBitmapSource();

               if (IsAutoSubmit | waitpic)
               {
                   if (delaycount2 >= _switchdelay)
                   {
                       delaycount2 = 0;
                       TmpBarcodeInfo=  DetectBarcode(_capture.RetrieveMat(), _threshold);
                   }
                   delaycount2 = delaycount2 + 1;
               }
               
                if (IsAutoSubmit)
                {
                   // BarcodeImg = _cropimgMat.ToBitmapSource();
                    BarcodeText = TmpBarcodeInfo.Text;
                    BarcodeFormat = TmpBarcodeInfo.Format;
                    if (!string.IsNullOrWhiteSpace(TmpBarcodeInfo.Text))
                    {
                        var saveFile = @"{MasterPara.LocalDb}\Picture\{BarcodeText}.jpg";
                        _cropimgMat.ImWrite(saveFile);
                        TmpBarcodeInfo =new BarcodeInfo()
                        {
                            Text = string.Empty,Format = "submitted"
                        };
                    }
                }
                else
                {
                    if (waitpic)
                    {
                        if (!string.IsNullOrWhiteSpace(TmpBarcodeInfo.Text))
                        {
                            // BarcodeImg = _cropimgMat.ToBitmapSource();
                            BarcodeText = TmpBarcodeInfo.Text;
                            BarcodeFormat = TmpBarcodeInfo.Format;
                            waitpic = false;
                        }
                    }

                    if (isSubmit)
                    {
                        var saveFile = $@"{MasterPara.LocalDataDir}LocalData\Picture\{BarcodeText}.jpg";
                        _cropimgMat.ImWrite(saveFile);
                        isSubmit = false;
                        waitpic = true;
                        TmpBarcodeInfo =new BarcodeInfo()
                        {
                            Text = string.Empty,Format = "submitted"
                        };
                    }

                }
                    
                
                //  Cv2.ImShow("111",_frame);
                Cv2.WaitKey(1);
            }
        }

        /*public void ImageInitalize()
        {
            if (_isImageRunning)
            { return;}
            CaptureCamera();
            _isImageRunning = true;
        }
        
        private void CaptureImage()
        {
            _imageThread = new Thread(BarcodeTask);
            //CaptureCameraCallback();
            _imageThread.Start();
        }


        private void BarcodeTask()
        {
            
            BarcodeFormat = tmp.Format;
            BarcodeText = tmp.Text;

        }*/


      private  BarcodeInfo DetectBarcode(Mat source , double thresh, bool debug = false)
      {

          //  Console.WriteLine("\nProcessing: {0}", fileName);
          // load the image and convert it to grayscale
          // var image = new Mat(fileName);
         // var image = StreamImg.ToMat();
         var barcoderesult =new BarcodeInfo();
          var image =  source;
          if (debug)
          {
                Cv2.ImShow("Source", image);
                Cv2.WaitKey(1); // do events
          }

            var gray = new Mat();
            var channels = image.Channels();
            if (channels > 1)
            {
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                image.CopyTo(gray);
            }


            // compute the Scharr gradient magnitude representation of the images
            // in both the x and y direction
            var gradX = new Mat();
            Cv2.Sobel(gray, gradX, MatType.CV_32F, xorder: 1, yorder: 0, ksize: -1);
            //Cv2.Scharr(gray, gradX, MatType.CV_32F, xorder: 1, yorder: 0);

            var gradY = new Mat();
            Cv2.Sobel(gray, gradY, MatType.CV_32F, xorder: 0, yorder: 1, ksize: -1);
            //Cv2.Scharr(gray, gradY, MatType.CV_32F, xorder: 0, yorder: 1);

            // subtract the y-gradient from the x-gradient
            var gradient = new Mat();
            Cv2.Subtract(gradX, gradY, gradient);
            Cv2.ConvertScaleAbs(gradient, gradient);

            if (debug)
            {
                Cv2.ImShow("Gradient", gradient);
                Cv2.WaitKey(1); // do events
            }


            // blur and threshold the image
            var blurred = new Mat();
            Cv2.Blur(gradient, blurred, new Size(9, 9));

            var threshImage = new Mat();
            Cv2.Threshold(blurred, threshImage, thresh, 255, ThresholdTypes.Binary);

            if (debug)
            {
                Cv2.ImShow("Thresh", threshImage);
                Cv2.WaitKey(1); // do events
            }


            // construct a closing kernel and apply it to the thresholded image
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(21, 7));
            var closed = new Mat();
            Cv2.MorphologyEx(threshImage, closed, MorphTypes.Close, kernel);

            if (debug)
            {
                Cv2.ImShow("Closed", closed);
                Cv2.WaitKey(1); // do events
            }


            // perform a series of erosions and dilations
            Cv2.Erode(closed, closed, null, iterations: 4);
            Cv2.Dilate(closed, closed, null, iterations: 4);

            if (debug)
            {
                Cv2.ImShow("Erode & Dilate", closed);
                Cv2.WaitKey(1); // do events
            }


            //find the contours in the thresholded image, then sort the contours
            //by their area, keeping only the largest one

            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(
                closed,
                out contours,
                out hierarchyIndexes,
                mode: RetrievalModes.CComp,
                method: ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
            {
             //   throw new NotSupportedException("Couldn't find any object in the image.");
            Console.WriteLine("Couldn't find any object in the image.");
            return barcoderesult;
            }

            var contourIndex = 0;
            var previousArea = 0;
            var biggestContourRect = Cv2.BoundingRect(contours[0]);
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                var boundingRect = Cv2.BoundingRect(contour); //Find bounding rect for each contour
                var boundingRectArea = boundingRect.Width * boundingRect.Height;
                if (boundingRectArea > previousArea)
                {
                    biggestContourRect = boundingRect;
                    previousArea = boundingRectArea;
                }

                contourIndex = hierarchyIndexes[contourIndex].Next;
            }

            /*biggestContourRect.Width += 10;
            biggestContourRect.Height += 10;
            biggestContourRect.Left -= 5;
            biggestContourRect.Top -= 5;*/


            var barcode = new Mat(image, biggestContourRect); //Crop the image
            Cv2.CvtColor(barcode, barcode, ColorConversionCodes.BGRA2GRAY);

            _cropimgMat =   barcode;
            
            Cv2.ImShow("Barcode", barcode);

           // Cv2.WaitKey(1); // do events

            var barcodeClone = barcode.Clone();
            if (delaycount1 < _dectdelay)
            {
                delaycount1 = delaycount1 + 1;
                barcoderesult = new BarcodeInfo()
                {
                    Text = String.Empty,
                    Format = "waiting detecting"
                };
                return barcoderesult;
            }

            delaycount1 = 0;

            barcoderesult = GetBarcodeText(barcodeClone);

            if (string.IsNullOrWhiteSpace(barcoderesult.Text))
            {
                Console.WriteLine("Enhancing the barcode...");
                //Cv2.AdaptiveThreshold(barcode, barcode, 255,
                //AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 9, 1);
                //var th = 119;
                var th = 100;
                Cv2.Threshold(barcode, barcode, th, 255, ThresholdTypes.Tozero);
                Cv2.Threshold(barcode, barcode, th, 255, ThresholdTypes.Binary);
                barcoderesult = GetBarcodeText(barcode);
            }

            Cv2.Rectangle(image,
                new Point(biggestContourRect.X, biggestContourRect.Y),
                new Point(biggestContourRect.X + biggestContourRect.Width, biggestContourRect.Y + biggestContourRect.Height),
                new Scalar(0, 255, 0),
                2);

            if (debug)
            {
                Cv2.ImShow("Segmented Source", image);
                Cv2.WaitKey(1); // do events
            }

            //Cv2.WaitKey(0);
            //Cv2.DestroyAllWindows();

            return barcoderesult;
        }

       private  BarcodeInfo GetBarcodeText(Mat barcode)
        {
            // `ZXing.Net` needs a white space around the barcode
            var barcodeWithWhiteSpace = new Mat(new Size(barcode.Width + 30, barcode.Height + 30), MatType.CV_8U, Scalar.White);
            var drawingRect = new Rect(new Point(15, 15), new Size(barcode.Width, barcode.Height));
            var roi = barcodeWithWhiteSpace[drawingRect];
            barcode.CopyTo(roi);
           
              //  Cv2.ImShow("Enhanced Barcode", barcodeWithWhiteSpace);
             //   Cv2.WaitKey(1); // do events
       

            return DecodeBarcodeText(barcodeWithWhiteSpace.ToBitmap());
        }

        private  BarcodeInfo DecodeBarcodeText(System.Drawing.Bitmap barcodeBitmap)
        {
            var tmpbnarcode =new BarcodeInfo();
            try
            {
                 var source = new BitmapLuminanceSource(barcodeBitmap);

            // using http://zxingnet.codeplex.com/
            // PM> Install-Package ZXing.Net
            var reader = new BarcodeReader(null, null, ls => new GlobalHistogramBinarizer(ls))
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    //PureBarcode = true,
                    /*PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.CODE_128
                        //BarcodeFormat.EAN_8,
                        //BarcodeFormat.CODE_39,
                        //BarcodeFormat.UPC_A
                    }*/
                }
            };

            //var newhint = new KeyValuePair<DecodeHintType, object>(DecodeHintType.ALLOWED_EAN_EXTENSIONS, new Object());
            //reader.Options.Hints.Add(newhint);

            var result = reader.Decode(source);
            if (result == null)
            {
                Console.WriteLine("Decode failed.");
                tmpbnarcode.Format = "Decode failed.";
                tmpbnarcode.Text = String.Empty;
                return  tmpbnarcode;

            }

            Console.WriteLine("BarcodeFormat: {0}", result.BarcodeFormat);
            Console.WriteLine("Result: {0}", result.Text);

            tmpbnarcode.Format =  result.BarcodeFormat.ToString();
            tmpbnarcode.Text = result.Text;

         //   var writer = new BarcodeWriter
         //   {
          //      Format = result.BarcodeFormat,
          //      Options = { Width = 200, Height = 50, Margin = 4},
         //       Renderer = new ZXing.Rendering.BitmapRenderer()
          //  };
          //  var barcodeImage = writer.Write(result.Text);
         //   Cv2.ImShow("BarcodeWriter", barcodeImage.ToMat());

            return  tmpbnarcode;
            }
            catch (Exception e)
            {
                tmpbnarcode.Format = "Decode failed.";
                tmpbnarcode.Text = String.Empty;
                return  tmpbnarcode;
              // Console.WriteLine(e);
              //  throw;
            }


          


           
        }




    }
}
