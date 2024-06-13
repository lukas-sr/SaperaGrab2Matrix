using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.Examples.NET.Utils;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DALSA.SaperaLT.Examples.NET.CSharp.GrabConsole
{
    class GrabConsole
   {
        public static SapAcquisition Acq = null;
        public static SapAcqDevice AcqDevice = null;
        public static SapBuffer Buffers = null;
        public static SapTransfer Xfer = null;
        public static SapView View = null;
        public static int numFrames = 1;
        public static int sizeArr = 0;
        public static Int16[,] framesArr = null;
        public MyAcquisitionParams acqParams;
        private static int countFrame = 0;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static void Main(string[] args)
        {
            string path = @"C:\temp\IC-47_TDI.ccf";

            GrabConsole gbConsole = new GrabConsole();
            gbConsole.setConfigFile(path);
            Console.Write("Number of Frames: ");
            numFrames = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            gbConsole.StartGrabFramesTDI(numFrames);

            Console.WriteLine(framesArr[0, 0].ToString());
        }
        public GrabConsole()
        {
            acqParams = new MyAcquisitionParams
            {
                ResourceIndex = 0,
                ServerName = "Xtium-CLHS_PX8_1"
            };
        }
        public bool setConfigFile(string filePath)
        {
            acqParams.ConfigFileName = filePath;

            if ((acqParams.ConfigFileName != null) && (acqParams.ServerName.Equals("Xtium-CLHS_PX8_1")))
            {
                return true;
            }

            return false;
        }

        public void StartGrabFramesTDI(int numFrames)
        {
            SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

            if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0)
            {
                Acq = new SapAcquisition(loc, acqParams.ConfigFileName);
                Buffers = new SapBufferWithTrash(2, Acq, SapBuffer.MemoryType.ScatterGather);
                Xfer = new SapAcqToBuf(Acq, Buffers);

                // Create acquisition object
                if (!Acq.Create())
                {
                    DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                    return;
                }
                Acq.EnableEvent(SapAcquisition.AcqEventType.StartOfFrame);
            }

            else if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.AcqDevice) > 0)
            {
                AcqDevice = new SapAcqDevice(loc, acqParams.ConfigFileName);
                Buffers = new SapBufferWithTrash(2, AcqDevice, SapBuffer.MemoryType.ScatterGather);
                Xfer = new SapAcqDeviceToBuf(AcqDevice, Buffers);

                // Create acquisition object
                if (!AcqDevice.Create())
                {
                    DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                    return;
                }
            }

            View = new SapView(Buffers);

            // End of frame event
            Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            Xfer.XferNotify += new SapXferNotifyHandler(Xfer_XferNotify);
            Xfer.XferNotifyContext = View;

            // Create buffer object
            if (!Buffers.Create())
            {
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // For TDI Case the heigth of the buffer must be equal 1
            sizeArr = Buffers.Width * 1;
            InitializeFrameArray(numFrames, sizeArr);

            // Create buffer object
            if (!Xfer.Create())
            {
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // Create buffer object
            if (!View.Create())
            {
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }

            //Xfer.Grab();
            if (numFrames > 0 && numFrames < 100)
            {
                Xfer.Snap(numFrames);
            }
            else
            {
                Xfer.Snap();
            }
            Xfer.Wait(10000);
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            loc.Dispose();
        }

        public virtual void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            // save Buffer
            Buffers.GetAddress(out IntPtr buffAddress);
            SaveFrameArray(sizeArr, buffAddress);

            // refresh view
            SapView View = args.Context as SapView;
            View.Show();
        }
        public static void InitializeFrameArray(int dim1, int dim2)
        {
            Console.WriteLine("Initializing frame array with dimension " + dim1.ToString() + "x" + dim2.ToString() + "...");
            framesArr = (Int16[,])Array.CreateInstance(typeof(Int16), dim1, dim2);
        }
        public static void SaveFrameArray(int size, IntPtr buffAddress)
        {
            Int16[] imageData = new Int16[size];
            Marshal.Copy(buffAddress, imageData, 0, size);

            for (int i = 0; i < size; i++)
            {
                framesArr[countFrame, i] = imageData[i];
            }
            Console.WriteLine("Saving frame nº " + (countFrame+1).ToString() + "...");
            countFrame++;
        }
        static void DestroysObjects(SapAcquisition acq, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
        {
            Console.WriteLine("Destroying objects...");
            if (xfer != null)
            {
                xfer.Destroy();
                xfer.Dispose();
            }

            if (camera != null)
            {
                camera.Destroy();
                camera.Dispose();
            }

             if (acq != null)
             {
                acq.Destroy();
                acq.Dispose();
             }

             if (buf != null)
             {
                buf.Destroy();
                buf.Dispose();
             }

             if (view != null)
             {
                view.Destroy();
                view.Dispose();
             }
        } 
   }
}
