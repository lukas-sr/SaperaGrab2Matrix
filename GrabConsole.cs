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
        static SapAcquisition Acq = null;
        static SapAcqDevice AcqDevice = null;
        static SapBuffer Buffers = null;
        static SapTransfer Xfer = null;
        static SapView View = null;
        static int countFrame = 0;
        static List<Int16[,]> frames = new List<Int16[,]>();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static void Xfer_XferNotify(object sender, SapXferNotifyEventArgs args)
        {
            // refresh view
            SapView View = args.Context as SapView;
            View.Show();

            // save Buffer
            Console.WriteLine("Buffers.Count: " + Buffers.Count.ToString() + " " +
                              "Buffers.Index: " + Buffers.Index.ToString()); 
            int width = Buffers.Width;
            int height = 1;//Buffers.Height;
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Buffers.GetAddress(out IntPtr buffAddress);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString() + " s");

            //SaveImageData2File(width, height, buffAddress);
            SaveFrames2LabView(width, height, buffAddress);

        }

        private static void SaveImageData2File(int width, int height, IntPtr buffAddress)
        {
            Int16[] imageData = new Int16[width * height];
            Marshal.Copy(buffAddress, imageData, 0, width * height);

            string filePath = @"C:\temp\matrix" + countFrame + ".txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        writer.Write(imageData[i+j].ToString() + " ");
                    }
                    writer.WriteLine();
                }
            }
        }

        private static void SaveFrames2LabView(int width, int height, IntPtr buffAddress)
        {
            Int16[] imageData = new Int16[height * width];
            Int16[,] matrix = new Int16[height, width];
            Marshal.Copy(buffAddress, imageData, 0, height * width);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    matrix[i, j] = imageData[i + j];
                }
            }

            frames.Insert(countFrame, matrix);
            countFrame++;
        }
        
        private static void Frame2File()
        {
            string filePath = @"C:\temp\frames.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach(var mat in frames)
                {
                    writer.WriteLine("Init > ");
                    for (int i = 0; i < mat.GetLength(0); i++)
                    {
                        for ( int j = 0; j < mat.GetLength(1); j++)
                        {
                            writer.Write(mat[i, j].ToString() + " ");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // This is only meaningful for .NET 5 and up in order to show the console, since the project
            // needs to be configured as a Windows application with Windows Forms when it uses SapView
            AllocConsole();
            Console.WriteLine("Sapera Console Grab Example (C# version)\n");

            MyAcquisitionParams acqParams = new MyAcquisitionParams();
            // Call GetOptions to determine which acquisition device to use and which config
            // file (CCF) should be loaded to configure it.
            if (!GetOptions(args, acqParams))
            {
               Console.WriteLine("\nPress any key to terminate\n");
               Console.ReadKey(true);
               return;
            }

            SapLocation loc = new SapLocation(acqParams.ServerName, acqParams.ResourceIndex);

            if (SapManager.GetResourceCount(acqParams.ServerName, SapManager.ResourceType.Acq) > 0)
            {
               Acq = new SapAcquisition(loc, acqParams.ConfigFileName);
               Buffers = new SapBufferWithTrash(2, Acq, SapBuffer.MemoryType.ScatterGather);
               Xfer = new SapAcqToBuf(Acq, Buffers);

               // Create acquisition object
               if (!Acq.Create())
               {
                  Console.WriteLine("Error during SapAcquisition creation!\n");
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
                  Console.WriteLine("Error during SapAcqDevice creation!\n");
                  DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                  return;
               }
            }
            Console.WriteLine("Select how many frames: ");
            int userInput = int.Parse(Console.ReadLine());

            View = new SapView(Buffers);
            // End of frame event
            Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            Xfer.XferNotify += new SapXferNotifyHandler(Xfer_XferNotify);
            Xfer.XferNotifyContext = View;

            // Create buffer object
            if (!Buffers.Create())
            {
                Console.WriteLine("Error during SapBuffer creation!\n");
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }    

            // Create buffer object
            if (!Xfer.Create())
            {
                Console.WriteLine("Error during SapTransfer creation!\n");
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }

            // Create buffer object
            if (!View.Create())
            {
                Console.WriteLine("Error during SapView creation!\n");
                DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
                return;
            }

            //Xfer.Grab();
            if (userInput > 0 && userInput < 100)
            {
                Xfer.Snap(userInput);
            }
            else
            {
                Xfer.Snap();
            }

            Console.WriteLine("\n\nGrab started, press a key to freeze");
            //Console.ReadKey(true);
            Xfer.Wait(2000);
            DestroysObjects(Acq, AcqDevice, Buffers, Xfer, View);
            loc.Dispose();
        }

        static bool GetOptions(string[] args, MyAcquisitionParams acqParams)
        {
            // Check if arguments were passed
            if (args.Length > 1)
            return ExampleUtils.GetOptionsFromCommandLine(args, acqParams);
            else
            return ExampleUtils.GetOptionsFromQuestions(acqParams);
        }

        static void DestroysObjects(SapAcquisition acq, SapAcqDevice camera, SapBuffer buf, SapTransfer xfer, SapView view)
        {
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
             Frame2File();
             Console.WriteLine("\nPress any key to terminate\n");
             Console.ReadKey(true);
        } 
   }
}
