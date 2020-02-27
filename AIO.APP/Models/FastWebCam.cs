using System;
using OpenCvSharp;
using System.Drawing;
using System.Threading;
using OpenCvSharp.Extensions;

namespace AIO.APP.Models
{
    public class FastWebCam
    {
        /// <summary>
        /// Initalizes camera feed
        /// </summary>
        /// <param name="AutoActivate">If set to false this disables the camera from starting, you must call the Initalize method.</param>
        public FastWebCam(bool AutoActivate = true)
        {
            if (AutoActivate)
            {
                Initalize();
            }
        }

        // Create class-level accesible variables
        static VideoCapture capture;
        static Mat frame;
        static Bitmap image;
        private static Thread camera;
        private static bool isCameraRunning = false;
        private bool imagetakinginprogress = false;

        /// <summary>
        /// Starts the camera feed
        /// </summary>
        public void Initalize()
        {
            isCameraRunning = true;
            CaptureCamera();
           
        }

        private static void CaptureCamera()
        {
            camera = new Thread(CaptureCameraCallback);
          // CaptureCameraCallback();
            camera.Start();
            Console.WriteLine(camera.ThreadState);
        }

        private static void CaptureCameraCallback()
        {
            Console.WriteLine("zaza");
            if (!isCameraRunning)
            {
                return;
            }
            Console.WriteLine("zaza");
            frame = new Mat();
            capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                while (isCameraRunning)
                {

                    capture.Read(frame);
                    image = BitmapConverter.ToBitmap(frame);
                  //  Cv2.WaitKey(1);
                }
            }
        }

        /// <summary>
        /// Gets a bitmap from the camera
        /// </summary>
        /// <returns>Bitmap image from camera</returns>
        public Bitmap GetBitmap()
        {
            if (isCameraRunning)
            {
                while (imagetakinginprogress) { }
                try
                {
                    return new Bitmap(image);
                }
                catch
                {
                    return new Bitmap(image);
                }
            }
            else
                throw new Exception("Cannot take picutre if the camera is not initalized!");
        }

        /// <summary>
        /// Deinitalizes the camera. Can be reinitalized.
        /// </summary>
        public void Deinitalize()
        {
            camera.Abort();
            capture.Release();
            isCameraRunning = false;
        }

        /// <summary>
        /// Destroys the camera
        /// </summary>
        ~FastWebCam()
        {
            Deinitalize();
            capture.Dispose();
            frame.Dispose();
            image.Dispose();
        }

    }
}
